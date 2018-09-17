using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Cells;
using Spectre.Tables;
using Spectre.Structures;
using Spectre.Control;
using Spectre.Statements;
using Spectre.Expressions;

namespace Spectre.Statements
{

    public abstract class TableInsert : Statement
    {

        protected Table _Output;
        protected RecordWriter _Writer;
        protected int _SourceColumnCount = 0;

        public TableInsert(Host Host, Statement Parent, Table Output)
            : base(Host, Parent)
        {
            this._Output = Output;
            this._SourceColumnCount = Output.Columns.Count;
        }

        public override void BeginInvoke(SpoolSpace Memory)
        {
            this._Writer = this._Output.OpenWriter();
        }
        
        public override void EndInvoke(SpoolSpace Memory)
        {
            this._Writer.Close();
        }
        
        protected Record CheckValue(Record Value)
        {

            // Checks if we were given an array expression
            if (Value.Count == 1 && Value.Count != this._SourceColumnCount && Value[0].IsArray)
            {
                Value = new Record(Value[0].ARRAY.ToArray());
            }

            // Check the record and source column count
            if (Value.Count != this._SourceColumnCount)
            {
                throw new Exception(string.Format("Invalid record count; expecting '{0}' but expression has '{1}' fields", Value.Count, this._SourceColumnCount));
            }

            return Value;

        }

        // Generic insert
        public sealed class GenericTableInsert : TableInsert
        {

            public GenericTableInsert(Host Host, Statement Parent, Table Output)
                : base(Host, Parent, Output)
            {
                this._Writer = Output.OpenWriter();
            }

            public override void BeginInvoke(SpoolSpace Memory)
            {
                if (this._Parameters.ContainsAggregate || this._Parameters.Count == 0)
                    throw new Exception("Generic table inserts cannot contain aggregate functions and must contain at least one expression");
                base.BeginInvoke(Memory);
            }
            
            public override void Invoke(SpoolSpace Memory)
            {
                Record value = this._Parameters.ToRecord(Memory);
                value = this.CheckValue(value);
                this._Writer.Insert(value);
            }
            
        }

        // Dictionary group by
        public sealed class DictionGroupBy : TableInsert
        {

            private DictionaryTable _StorageTree;

            public DictionGroupBy(Host Host, Statement Parent, Table Output)
                : base(Host, Parent, Output)
            {

            }

            public override void BeginInvoke(SpoolSpace Memory)
            {

                // Do the checks, need an aggregate and we must be part of a loop
                if (!this._Parameters.ContainsAggregate || !this._Parameters.ContainsNonAggregate)
                    throw new Exception("Dictionary aggregates must contain at least one aggregate and one non-aggregate");

                //if (this._Parent == null || !this._Parent.IsBreakable)
                //    throw new Exception("Aggregates can only appear in loop statements");

                // We don't need to invoke the base method

                // Build the keys
                Key k = new Key(), v = new Key();
                int idx = 0;
                foreach (Expression x in this._Parameters)
                {
                    if (x.IsAggregate)
                        v.Add(idx);
                    else
                        k.Add(idx);
                    idx++;
                }

                // Set up the staging table //
                this._StorageTree = new DictionaryTable(this._Host, Host.RandomPath(), this._Parameters.Columns, k, Page.DEFAULT_SIZE);
                
            }

            public override void Invoke(SpoolSpace Memory)
            {

                // At this point, the required record(s) are loaded into the spool space
                Record work = this._Parameters.ToAggShellRecord(Memory);
                work = this.CheckValue(work);

                // Check if it's in our dictionary table
                if (this._StorageTree.ContainsKey2(work))
                {
                    work = this._StorageTree.GetKeyValue2(work);
                }

                // Accumulate
                work = this._Parameters.Accumulate(Memory, work);

                // Push back
                this._StorageTree.SetValue(work);

            }

            public override void EndInvoke(SpoolSpace Memory)
            {

                // Open the base stream 
                using (RecordWriter writer = this._Output.OpenWriter())
                {

                    // Open a reader //
                    using (RecordReader reader = this._StorageTree.OpenReader())
                    {

                        while (reader.CanAdvance)
                        {

                            Record work = reader.ReadNext();
                            Record finale = this._Parameters.AggRender(work);
                            writer.Insert(finale);

                        } // end while

                    } // end reader using

                } // end using writer

                // Drop the tree
                this._Host.TableStore.DropTable(this._StorageTree.Key);

            }

        }
        

    }

}
