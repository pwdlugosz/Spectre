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

    public abstract class TableLoop : Statement
    {

        protected string _SpoolName;
        protected Expression _Filter;

        public TableLoop(Host Host, Statement Parent, string SpoolName, Expression Filter)
            :base(Host, Parent)
        {
            this._SpoolName = SpoolName;
            this._Filter = Filter;
        }

        public override bool IsBreakable
        {
            get
            {
                return true;
            }
        }

        public override void BeginInvoke(SpoolSpace Memory)
        {
            this.BeginInvokeChildren(Memory);
        }

        public override void EndInvoke(SpoolSpace Memory)
        {
            this.EndInvokeChildren(Memory);
        }

        public sealed class Select : TableLoop
        {
            
            public Select(Host Host, Statement Parent, string SpoolName, Expression Filter)
                : base(Host, Parent, SpoolName, Filter)
            {
            }

            private void AddSpools(SpoolSpace Memory)
            {
                string name = this._Parameters[0].NameOf();
                Memory.Add(name, new Spool.RecordSpindle(name, this._Parameters[0].Columns));
            }

            private void RemoveSpools(SpoolSpace Memory)
            {
                Memory.Drop(this._Parameters[0].NameOf());
            }

            public override void Invoke(SpoolSpace Memory)
            {

                // Get the table source //
                if (this._Parameters[0].TypeOf() != CellAffinity.TREF)
                {
                    throw new Exception("Table loops require a tabular expression");
                }

                this.AddSpools(Memory);

                // Get the table 
                Table t = this._Parameters[0].Select(Memory);
                
                // Loop
                using (RecordReader rr = t.OpenReader())
                {
                    while (rr.CanAdvance)
                    {

                        Memory[this._SpoolName].Set(rr.ReadNext());

                        if (this._Filter.Evaluate(Memory).valueBOOL)
                        {
                            this.InvokeChildren(Memory);
                        }

                    }

                }

                this.RemoveSpools(Memory);

            }

        }

        public sealed class Select2 : TableLoop
        {
            
            private string _SpoolNameLive;
            private ExpressionCollection _Fields;

            public Select2(Host Host, Statement Parent, string SpoolNameTemp, string SpoolNameLive)
                : base(Host, Parent, SpoolNameTemp, Expression.TrueForAll)
            {
                this._SpoolNameLive = SpoolNameLive;
            }

            private void AddSpools(SpoolSpace Memory)
            {
                Memory.Add(this._SpoolName, new Spool.RecordSpindle(this._SpoolName, this._Parameters[0].Columns));
                Memory.Add(this._SpoolNameLive, new Spool.RecordSpindle(this._SpoolNameLive, this._Fields.Columns));
            }

            private void RemoveSpools(SpoolSpace Memory)
            {
                Memory.Drop(this._SpoolName);
                Memory.Drop(this._SpoolNameLive);
            }

            public override void Invoke(SpoolSpace Memory)
            {

                // Get the table source //
                if (this._Parameters[0].TypeOf() != CellAffinity.TREF)
                {
                    throw new Exception("Table loops require a tabular expression");
                }

                // Get the fields //
                this._Fields = ExpressionCollection.Unpack(this._Parameters[1]);

                // Add memory 
                this.AddSpools(Memory);

                // Get the table 
                Table t = this._Parameters[0].Select(Memory);
                
                // Loop
                using (RecordReader rr = t.OpenReader())
                {

                    while (rr.CanAdvance)
                    {

                        Memory[this._SpoolName].Set(rr.ReadNext());
                        Memory[this._SpoolNameLive].Set(this._Fields.ToRecord(Memory));

                        if (this._Filter.Evaluate(Memory).valueBOOL)
                        {
                            this.InvokeChildren(Memory);
                        }

                    }

                }

                this.RemoveSpools(Memory);

            }

        }

        public abstract class Join : TableLoop
        {
            
            protected Key _LKey;
            protected Key _RKey;
            protected JoinAffinity _Affinity;

            public enum JoinAffinity
            {
                Inner,
                Left,
                AntiLeft
            }

            public Join(Host Host, Statement Parent, string SpoolName, Expression Filter, Key LKey, Key RKey, JoinAffinity Affinity)
                : base(Host, Parent, SpoolName, Filter)
            {
                this._LKey = LKey;
                this._RKey = RKey;
                this._Affinity = Affinity;
            }
            
            public Key LeftKey
            {
                get { return this._LKey; }
            }

            public Key RightKey
            {
                get { return this._RKey; }
            }

            public Expression Filter
            {
                get { return this._Filter; }
            }

            public JoinAffinity Affinity
            {
                get { return this._Affinity; }
                set { this._Affinity = value; }
            }

            protected void AddSpools(SpoolSpace Memory)
            {
                string lname = this._Parameters[0].NameOf();
                Memory.Add(lname, new Spool.RecordSpindle(lname, this._Parameters[0].Columns));
                string rname = this._Parameters[1].NameOf();
                Memory.Add(rname, new Spool.RecordSpindle(rname, this._Parameters[1].Columns));
            }

            protected void RemoveSpools(SpoolSpace Memory)
            {
                Memory.Drop(this._Parameters[0].NameOf());
                Memory.Drop(this._Parameters[1].NameOf());
            }

            public sealed class NestedLoopJoin : Join
            {

                public NestedLoopJoin(Host Host, Statement Parent, string SpoolName, Expression Filter, Key LKey, Key RKey, JoinAffinity Affinity)
                    : base(Host, Parent, SpoolName, Filter, LKey, RKey, Affinity)
                {

                }

                public override void Invoke(SpoolSpace Memory)
                {

                    this.AddSpools(Memory);

                    Table ltable = (this._Parameters[0]).Select(Memory);
                    Table rtable = (this._Parameters[1]).Select(Memory);
                    string lalias = this._Parameters[0].NameOf();
                    string ralias = this._Parameters[1].NameOf();
                    
                    using (RecordReader lstream = ltable.OpenReader())
                    {

                        while(lstream.CanAdvance)
                        {

                            Record l = lstream.ReadNext();
                            Memory[lalias].Set(l);
                            bool found = false;

                            using (RecordReader rstream = rtable.OpenReader())
                            {

                                while(rstream.CanAdvance)
                                {

                                    Record r = rstream.ReadNext();
                                    int compare = Record.Compare(l, this._LKey, r, this._RKey);
                                    if (compare == 0)
                                    {
                                        Memory[ralias].Set(r);
                                        found = true;
                                        if ((this._Affinity == JoinAffinity.Inner || this._Affinity == JoinAffinity.Left) && this._Filter.Evaluate(Memory).valueBOOL)
                                        {
                                            this.InvokeChildren(Memory);
                                        }
                                        
                                    }

                                } // End inner loop

                            }

                            if (!found && (this._Affinity == JoinAffinity.Left || this._Affinity == JoinAffinity.AntiLeft))
                            {
                                Memory[ralias].Set(rtable.Columns.NullRecord);
                                if (this._Filter.Evaluate(Memory).valueBOOL)
                                    this.InvokeChildren(Memory);
                            }

                        } // End outer loop


                    }

                    this.RemoveSpools(Memory);

                }

            }

            public sealed class QuasiNestedLoopJoin : Join
            {

                public QuasiNestedLoopJoin(Host Host, Statement Parent, string SpoolName, Expression Filter, Key LKey, Key RKey, JoinAffinity Affinity)
                    : base(Host, Parent, SpoolName, Filter, LKey, RKey, Affinity)
                {

                }

                public override void Invoke(SpoolSpace Memory)
                {

                    this.AddSpools(Memory);

                    Table ltable = (this._Parameters[0]).Select(Memory);
                    Table rtable = (this._Parameters[1]).Select(Memory);
                    string lalias = this._Parameters[0].NameOf();
                    string ralias = this._Parameters[1].NameOf();
                    TreeIndex rindex = rtable.GetIndex(this._RKey) ?? rtable.CreateTemporyIndex(this._RKey);
                    
                    using (RecordReader lstream = ltable.OpenReader())
                    {

                        while (lstream.CanAdvance)
                        {

                            Record l = lstream.ReadNext();
                            Memory[lalias].Set(l);
                            bool found = false;

                            using (RecordReader rstream = rindex.OpenStrictReader(Record.Split(l, this._LKey)))
                            {

                                while (rstream != null && rstream.CanAdvance)
                                {

                                    Record r = rstream.ReadNext();
                                    int compare = Record.Compare(l, this._LKey, r, this._RKey);
                                    if (compare == 0)
                                    {
                                        Memory[ralias].Set(r);
                                        found = true;
                                        if ((this._Affinity == JoinAffinity.Inner || this._Affinity == JoinAffinity.Left) && this._Filter.Evaluate(Memory).valueBOOL)
                                        {
                                            this.InvokeChildren(Memory);
                                        }

                                    }

                                } // End inner loop

                            }

                            if (!found && (this._Affinity == JoinAffinity.Left || this._Affinity == JoinAffinity.AntiLeft))
                            {
                                Memory[ralias].Set(rtable.Columns.NullRecord);
                                if (this._Filter.Evaluate(Memory).valueBOOL)
                                    this.InvokeChildren(Memory);
                            }

                        } // End outer loop

                    }

                    this.RemoveSpools(Memory);

                }

            }

            public sealed class SortMergeJoin : Join
            {

                public SortMergeJoin(Host Host, Statement Parent, string SpoolName, Expression Filter, Key LKey, Key RKey, JoinAffinity Affinity)
                    : base(Host, Parent, SpoolName, Filter, LKey, RKey, Affinity)
                {
                }

                public override void Invoke(SpoolSpace Memory)
                {
                    
                    // Add spools //
                    this.AddSpools(Memory);

                    // Render each table //
                    Table ltable = (this._Parameters[0]).Select(Memory);
                    Table rtable = (this._Parameters[1]).Select(Memory);
                    string lalias = this._Parameters[0].NameOf();
                    string ralias = this._Parameters[1].NameOf();

                    // Get the left and right join index //
                    TreeIndex lidx = ltable.GetIndex(this._LKey) ?? TreeIndex.BuildTemporaryIndex(ltable, this._LKey);
                    //lidx.Tree.DumpMeta(@"C:\Users\pwdlu_000\Documents\Spectre_Projects\Test\treeJoinT1.txt");
                    TreeIndex ridx = rtable.GetIndex(this._RKey) ?? TreeIndex.BuildTemporaryIndex(rtable, this._RKey);

                    // Get the join tags //
                    bool Intersection = (this._Affinity == JoinAffinity.Inner || this._Affinity == JoinAffinity.Left);
                    bool Antisection = (this._Affinity == JoinAffinity.Left || this._Affinity == JoinAffinity.AntiLeft);

                    // Open a read stream //
                    RecordReader lstream = lidx.OpenReader();
                    RecordReader rstream = ridx.OpenReader();

                    // Render some null records //
                    Record RNull = rstream.Columns.NullRecord;

                    // Main loop through both left and right
                    while (lstream.CanAdvance && rstream.CanAdvance)
                    {

                        Record lrec = lstream.Read();
                        Record rrec = rstream.Read();
                        int Compare = Record.Compare(lrec, this._LKey, rrec, this._RKey);

                        // Left is less than right, control left
                        if (Compare < 0)
                        {
                            lstream.Advance();
                        }
                        // AWValue is less than left, control right, but also output an anti join record
                        else if (Compare > 0)
                        {

                            if (Antisection)
                            {
                                Memory[lalias].Set(lrec);
                                Memory[ralias].Set(RNull);
                                if (this._Filter.Evaluate(Memory))
                                {
                                    this.InvokeChildren(Memory);
                                }
                            }
                            rstream.Advance();

                        }
                        else if (Intersection) // Compare == 0
                        {

                            // Save the loop-result //
                            int NestedLoopCount = 0;

                            // Loop through all possible tuples //
                            while (Compare == 0)
                            {

                                // Render the record and potentially output //
                                Memory[lalias].Set(lstream.Read());
                                Memory[ralias].Set(rstream.Read());
                                if (this._Filter.Evaluate(Memory))
                                {
                                    this.InvokeChildren(Memory);
                                }

                                // Advance the right table //
                                rstream.Advance();
                                NestedLoopCount++;

                                // Check if this advancing pushed us to the end of the table //
                                if (!rstream.CanAdvance)
                                    break;

                                // Reset the compare token //
                                Compare = Record.Compare(lstream.Read(), this._LKey, rstream.Read(), this._RKey);

                            }

                            // Revert the nested loops //
                            rstream.Revert(NestedLoopCount);

                            // Step the left stream //
                            lstream.Advance();

                        }
                        else
                        {
                            lstream.Advance();
                        }

                    }

                    // Do Anti-Join //
                    if (Antisection)
                    {

                        // Assign the right table to null //
                        Memory[ralias].Set(RNull);

                        // Walk the rest of the left table //
                        while (lstream.CanAdvance)
                        {

                            Memory[lalias].Set(lstream.ReadNext());
                            if (this._Filter.Evaluate(Memory))
                            {
                                this.InvokeChildren(Memory);
                            }

                        }

                    }

                    // Add spools //
                    this.RemoveSpools(Memory);
                }

            }
            
        }

    }
    
}
