using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre;
using Spectre.Cells;
using Spectre.Expressions;
using Spectre.Control;
using Spectre.Statements;
using Spectre.Structures;
using Spectre.Tables;

namespace Spectre.Libraries
{

    /// <summary>
    /// 
    /// </summary>
    public sealed class BaseLibrary : Library
    {

        // Functions
        private readonly string[] EXPRESSION_NAMES = { };

        public BaseLibrary(Host Host)
            : base(Host, "GLOBAL")
        {

        }

        public override ExpressionFunction ExpressionLookup(string Name)
        {

            switch (Name.ToUpper())
            {

                case "CAST":
                    return new Cast(this._Host, null);
                case "SIZEOF":
                    return new SizeOfFunc(this._Host, null);
                case "LENGTHOF":
                    return new LengthOf(this._Host, null);
                case "TYPEOF":
                    return new TypeOfFunc(this._Host, null);
                case "NAMEOF":
                    return new NameOf(this._Host, null);
                case "HASHOF":
                    return new HashOf(this._Host, null);
                case "SEARCH":
                    return new Search(this._Host, null);
                case "SPLIT":
                    return new Split(this._Host, null);
                case "GUID":
                    return new GuidFunc(this._Host, null);
                case "CREATE_TABLE":
                    return new CreateTable(this._Host, null);
                case "TPATH":
                    return new TPath(this._Host, null);

                default:
                    return null;
            }

        }

        public override Statement StatementLookup(string Name)
        {
            
            switch(Name.ToUpper())
            {
                case "SORT":
                    return new Sort(this._Host, null);
            }

            if (this._ScriptedStatements.Exists(Name))
                return this._ScriptedStatements[Name];

            return null;

        }
        
        public sealed class Cast : ExpressionFunction
        {

            public Cast(Host Host, Expression Parent)
                : base(Host, Parent, "CAST", 2, 2)
            {
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell c = this._Children[0].Evaluate(Memory);
                Cell d = this._Children[1].Evaluate(Memory);
                CellAffinity a = (CellAffinity)d.BYTE;
                return CellConverter.Cast(c, a);
            }

        }

        public sealed class SizeOfFunc : ExpressionFunction
        {

            public SizeOfFunc(Host Host, Expression Parent)
                : base(Host, Parent, "SIZEOF", 1, 1)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.INT;
            }

            public override int SizeOf()
            {
                return CellSerializer.INT_SIZE;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return new Cell(CellSerializer.DiskSize(x));
            }

        }

        public sealed class LengthOf : ExpressionFunction
        {

            public LengthOf(Host Host, Expression Parent)
                : base(Host, Parent, "LENGTHOF", 1, 1)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.INT;
            }

            public override int SizeOf()
            {
                return CellSerializer.INT_SIZE;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                if (this._Children[0] is Expression.ArrayLiteral)
                    return new Cell((this._Children[0] as Expression.ArrayLiteral).Children.Count);
                Cell x = this._Children[0].Evaluate(Memory);
                return new Cell(x.Length);
            }

        }

        public sealed class TypeOfFunc : ExpressionFunction
        {

            public TypeOfFunc(Host Host, Expression Parent)
                : base(Host, Parent, "TYPEOF", 1, 1)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.BYTE;
            }

            public override int SizeOf()
            {
                return CellSerializer.BYTE_SIZE;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                return new Cell((byte)this._Children[0].Evaluate(Memory).Affinity);
            }

        }
        
        public sealed class NameOf : ExpressionFunction
        {

            public NameOf(Host Host, Expression Parent)
                : base(Host, Parent, "NAMEOF", 1, 1)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.CSTRING;
            }
            
            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                return new Cell(this._Children[0].NameOf());
            }

        }

        public sealed class HashOf : ExpressionFunction
        {

            public HashOf(Host Host, Expression Parent)
                : base(Host, Parent, "HASHOF", 1, 1)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.INT;
            }

            public override int SizeOf()
            {
                return CellSerializer.INT_SIZE;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                return new Cell(this._Children[0].Evaluate(Memory).GetHashCode());
            }

        }
        
        public sealed class Search : ExpressionFunction
        {

            public Search(Host Host, Expression Parent)
                : base(Host, Parent, "SEARCH", 2, 3)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.INT;
            }

            public override int SizeOf()
            {
                return CellSerializer.INT_SIZE;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell a = this._Children[0].Evaluate(Memory);
                Cell b = this._Children[1].Evaluate(Memory);
                int Start = (this._Children.Count == 2 ? 0 : this._Children[2].Evaluate(Memory).valueINT);
                return CellFunctions.Position(a, b, Start);
            }


        }

        public sealed class Split : ExpressionFunction
        {

            public Split(Host Host, Expression Parent)
                : base(Host, Parent, "SPLIT", 2, 3)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();
                Cell Value = this._Children[0].Evaluate(Memory);
                Cell SplitValues = this._Children[1].Evaluate(Memory);
                CellArray z = new CellArray();

                // Two parameters //
                if (this._Children.Count == 2)
                {

                    if (Value.Affinity == CellAffinity.BINARY)
                    {
                        BString x = new BString(Value.valueBINARY);
                        foreach(BString y in x.Split(SplitValues.valueBINARY))
                        {
                            z.Append(new Cell(y.ToByteArray));
                        }
                    }
                    else if (Value.Affinity == CellAffinity.BSTRING)
                    {
                        foreach (BString y in Value.valueBSTRING.Split(SplitValues.valueBINARY))
                        {
                            z.Append(new Cell(y));
                        }
                    }
                    else if (Value.Affinity == CellAffinity.CSTRING)
                    {
                        foreach(string y in Util.StringUtil.Split(Value.valueCSTRING, SplitValues.valueCSTRING.ToCharArray()))
                        {
                            z.Append(new Cell(y));
                        }
                    }

                    return z;

                }

                // Three paramters
                Cell Escape = this._Children[2].Evaluate(Memory);
                if (Value.Affinity == CellAffinity.BINARY)
                {
                    BString x = new BString(Value.valueBINARY);
                    foreach (BString y in x.Split(SplitValues.valueBINARY, Escape.valueBYTE))
                    {
                        z.Append(new Cell(y.ToByteArray));
                    }
                }
                else if (Value.Affinity == CellAffinity.BSTRING)
                {
                    foreach (BString y in Value.valueBSTRING.Split(SplitValues.valueBINARY, Escape.valueBYTE))
                    {
                        z.Append(new Cell(y));
                    }
                }
                else if (Value.Affinity == CellAffinity.CSTRING)
                {
                    foreach (string y in Util.StringUtil.Split(Value.valueCSTRING, SplitValues.valueCSTRING.ToCharArray(), Escape.valueCSTRING.ToCharArray()[0]))
                    {
                        z.Append(new Cell(y));
                    }
                }

                return z;


            }

        }

        public sealed class GuidFunc : ExpressionFunction
        {

            public GuidFunc(Host Host, Expression Parent)
                : base(Host, Parent, "GUID", 0, 0)
            {

            }

            public override int SizeOf()
            {
                return 16;
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.BINARY;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                return new Cell(Guid.NewGuid().ToByteArray());
            }

        }

        public sealed class TPath : ExpressionFunction
        {

            public TPath(Host Host, Expression Parent)
                : base(Host, Parent, "TPATH", 2,2)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.BSTRING;
            }

            public override int SizeOf()
            {
                return -1;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                string Path = this._Children[0].Evaluate(Memory).valueCSTRING;
                string Name = this._Children[1].Evaluate(Memory).valueCSTRING;
                return TableHeader.DeriveV1Path(Path, Name);
            }

        }

        public sealed class CreateTable : ExpressionFunction
        {

            public CreateTable(Host Host, Expression Parent)
                : base(Host, Parent, "CREAT_TABLE", 2, 5)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.TREF;
            }

            public override int SizeOf()
            {
                return CellSerializer.DefaultLength(CellAffinity.TREF);
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();

                Cell Name = this._Children[0].Evaluate(Memory); // Name
                Cell ColNames = this._Children[1].Evaluate(Memory); // Column names
                if (!ColNames.IsArray) throw new Exception("Paramter two must be an array");
         
                Cell FieldTypes = (this._Children.Count >= 3 ? this._Children[2].Evaluate(Memory) : this.DefaultFieldTypes(ColNames.ARRAY.Count));
                if (FieldTypes.IsNull) FieldTypes = this.DefaultFieldTypes(ColNames.ARRAY.Count);
                if (!FieldTypes.IsArray) throw new Exception("Paramter three must be an array");
                if (FieldTypes.ARRAY.Count != ColNames.ARRAY.Count) throw new Exception("The field type array is not the same length as the column name array");

                Cell FieldSizes = (this._Children.Count >= 4 ? this._Children[3].Evaluate(Memory) : this.DefaultFieldSizes(ColNames.ARRAY.Count));
                if (FieldSizes.IsNull) FieldSizes = this.DefaultFieldSizes(ColNames.ARRAY.Count);
                if (!FieldSizes.IsArray) throw new Exception("Paramter four must be an array");
                if (FieldSizes.ARRAY.Count != ColNames.ARRAY.Count) throw new Exception("The field size array is not the same length as the column name array");

                Cell FieldNulls = (this._Children.Count >= 5 ? this._Children[4].Evaluate(Memory) : this.DefaultFieldNulls(ColNames.ARRAY.Count));
                if (FieldNulls.IsNull) FieldNulls = this.DefaultFieldNulls(ColNames.ARRAY.Count);
                if (!FieldNulls.IsArray) throw new Exception("Paramter four must be an array");
                if (FieldNulls.ARRAY.Count != ColNames.ARRAY.Count) throw new Exception("The null field indicator array is not the same length as the column name array");
                
                Schema s = new Schema();
                for(int i = 0; i < ColNames.ARRAY.Count; i++)
                {
                    s.Add(ColNames[i].valueCSTRING, (CellAffinity)FieldTypes[i].valueBYTE, FieldSizes.ARRAY[i].valueINT, FieldNulls.ARRAY[i].valueBOOL);
                }

                Table t = this._Host.CreateTable(Name.valueCSTRING, s, Page.DEFAULT_SIZE);

                return new Cell(t);

            }

            private Cell DefaultFieldTypes(int ColumnCount)
            {
                CellArray x = new CellArray();
                for(int i = 0; i < ColumnCount; i++)
                {
                    x.Append((byte)(CellAffinity.VARIANT));
                }
                return new Cell(x);
            }

            private Cell DefaultFieldSizes(int ColumnCount)
            {
                CellArray x = new CellArray();
                for (int i = 0; i < ColumnCount; i++)
                {
                    x.Append(-1);
                }
                return new Cell(x);
            }

            private Cell DefaultFieldNulls(int ColumnCount)
            {
                CellArray x = new CellArray();
                for (int i = 0; i < ColumnCount; i++)
                {
                    x.Append(true);
                }
                return new Cell(x);
            }

        }

        // Statements //
        public sealed class Sort : Statement
        {

            public Sort(Host Host, Statement Parent)
                : base(Host, Parent)
            {

            }

            public override void Invoke(SpoolSpace Memory)
            {

                Cell x = this._Parameters[0].Evaluate(Memory);

                if (x.Affinity == CellAffinity.ARRAY)
                {
                    x.ARRAY.Sort();
                }
                else if (x.Affinity == CellAffinity.TREF)
                {
                    Table t = this._Host.OpenTable(x.valueTREF);
                    Key k = new Key();
                    if (this._Parameters.Count >= 2)
                    {
                        Cell y = this._Parameters[1].Evaluate(Memory);
                        if (y.IsNumeric)
                        {
                            k.Add(y.valueINT);
                        }
                        else if (y.IsArray)
                        {
                            foreach (Cell z in y.valueARRAY)
                            {
                                k.Add(z.valueINT);
                            }
                        }
                        else
                        {
                            k = Key.Build(t.Columns.Count);
                        }
                    }
                    Spectre.Tables.TableUtil.Sort(t, k);
                }

            }

        }
        

    }
    
}
