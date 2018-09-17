using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Statements;
using Spectre.Expressions;
using Spectre.Cells;
using Spectre.Tables;
using Spectre.Control;
using System.IO;

namespace Spectre.Libraries
{

    public sealed class TableLibrary : Library
    {

        /*
        RowCount
        ColumnCount
        ColumnNames
        ColumnTypes
        ColumnSizes

        Drop
        Copy
        Rename
        Move
        Export
        Import
        ImportInto
        */

        public TableLibrary(Host Host)
            : base(Host, "TABLE")
        {

        }

        public override ExpressionFunction ExpressionLookup(string Name)
        {

            switch (Name.ToUpper())
            {
                case "ROWCOUNT":
                case "ROW_COUNT":
                    return new RowCount(this._Host);
                case "COLUMNCOUNT":
                case "COLUMN_COUNT":
                    return new ColumnCount(this._Host);
                case "COLUMNNAMES":
                case "COLUMN_NAMES":
                    return new ColumnNames(this._Host);
                case "COLUMNTYPES":
                case "COLUMN_TYPES":
                    return new ColumnTypes(this._Host);
                case "COLUMNSIZES":
                case "COLUMN_SIZES":
                    return new ColumnSizes(this._Host);
            }

            return null;

        }

        public override Statement StatementLookup(string Name)
        {

            switch(Name.ToUpper())
            {
                case "EXPORT":
                    return new Export(this._Host, null);
                case "IMPORT":
                    return new Import(this._Host, null);
            }

            return null;

        }

        // Expressions //

        public sealed class RowCount : ExpressionFunction
        {

            public RowCount(Host Host)
                : base(Host, null, "ROWCOUNT",1,1)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.LONG;
            }

            public override int SizeOf()
            {
                return CellSerializer.LONG_SIZE;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                if (x.Affinity != CellAffinity.TREF)
                    return CellValues.NullLONG;
                return this._Host.TableStore.RequestTable(x.valueTREF).RecordCount;
            }

        }

        public sealed class ColumnCount : ExpressionFunction
        {

            public ColumnCount(Host Host)
                : base(Host, null, "COLUMNCOUNT", 1, 1)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.LONG;
            }

            public override int SizeOf()
            {
                return CellSerializer.LONG_SIZE;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                if (x.Affinity != CellAffinity.TREF)
                    return CellValues.NullLONG;
                return this._Host.TableStore.RequestTable(x.valueTREF).Columns.Count;
            }

        }

        public sealed class ColumnNames : ExpressionFunction
        {

            public ColumnNames(Host Host)
                : base(Host, null, "COLUMNNAMES", 1, 1)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.ARRAY;
            }

            public override int SizeOf()
            {
                return CellSerializer.DEFAULT_VARIABLE_LEN;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                if (x.Affinity != CellAffinity.TREF)
                    return CellValues.NullARRAY;
                CellArray y = new CellArray();
                foreach (string n in this._Host.TableStore.RequestTable(x.valueTREF).Columns.Names)
                    y.Append(n);
                return y;
            }

        }

        public sealed class ColumnTypes : ExpressionFunction
        {

            public ColumnTypes(Host Host)
                : base(Host, null, "COLUMNTYPES", 1, 1)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.ARRAY;
            }

            public override int SizeOf()
            {
                return CellSerializer.DEFAULT_VARIABLE_LEN;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                if (x.Affinity != CellAffinity.TREF)
                    return CellValues.NullARRAY;
                CellArray y = new CellArray();
                foreach (CellAffinity n in this._Host.TableStore.RequestTable(x.valueTREF).Columns.Types)
                    y.Append((byte)n);
                return y;
            }

        }

        public sealed class ColumnSizes : ExpressionFunction
        {

            public ColumnSizes(Host Host)
                : base(Host, null, "COLUMNSIZES", 1, 1)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.ARRAY;
            }

            public override int SizeOf()
            {
                return CellSerializer.DEFAULT_VARIABLE_LEN;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                if (x.Affinity != CellAffinity.TREF)
                    return CellValues.NullARRAY;
                CellArray y = new CellArray();
                foreach (int n in this._Host.TableStore.RequestTable(x.valueTREF).Columns.Sizes)
                    y.Append(n);
                return y;
            }

        }

        // Import / Export //
        public sealed class Export : Statement
        {

            /*
             * Table: table to export
             * Path: file to export to
             * Delim: deliminator
             * Qualifier: text qualifier
             * 
             */

            public Export(Host Host, Statement Parent)
                : base(Host, Parent)
            {

            }
            
            public override void Invoke(SpoolSpace Memory)
            {

                this.CheckParameters(2, 4);

                Table Source = this._Parameters[0].Select(Memory);
                string Path = this._Parameters[1].Evaluate(Memory);
                string Delim = this._Parameters.Count >= 3 ? this._Parameters[2].Evaluate(Memory).valueCSTRING : "\t";
                string TextQ = this._Parameters.Count == 4 ? this._Parameters[3].Evaluate(Memory).valueCSTRING : "";

                using (RecordReader reader = Source.OpenReader())
                {

                    using (StreamWriter writer = new StreamWriter(Path))
                    {

                        string x = Source.Columns.ToNameString(Delim);
                        writer.WriteLine(x);

                        while(reader.CanAdvance)
                        {
                            Record r = reader.ReadNext();
                            string rec = Util.StringUtil.ToString(r, Delim, TextQ);
                            writer.WriteLine(rec);
                        }

                    }

                }

            }

        }

        public sealed class Import : Statement
        {

            public Import(Host Host, Statement Parent)
                : base(Host, Parent)
            {

            }

            public override void Invoke(SpoolSpace Memory)
            {

                Table Destination = this._Parameters[0].Select(Memory);
                string Source = this._Parameters[1].Evaluate(Memory).valueCSTRING;
                char[] Delims = this._Parameters[2].Evaluate(Memory).valueCSTRING.ToCharArray();
                char Escape = (this._Parameters.Count >= 4 ? this._Parameters[3].Evaluate(Memory).valueCSTRING.First() : char.MaxValue);
                int Skips = (this._Parameters.Count == 5 ? this._Parameters[4].Evaluate(Memory).valueINT : 0);

                using (StreamReader Reader = new StreamReader(Source))
                {

                    int i = 0;
                    while (i < Skips && !Reader.EndOfStream)
                    {
                        i++;
                        Reader.ReadLine();
                    }

                    using (RecordWriter Writer = Destination.OpenWriter())
                    {

                        while(!Reader.EndOfStream)
                        {
                            Record r = Util.StringUtil.ToRecord(Reader.ReadLine(), Destination.Columns, Delims, Escape);
                            Writer.Insert(r);
                        }

                    }


                }


            }


        }

    }

}
