using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre;
using Spectre.Cells;
using Spectre.Control;
using Spectre.Structures;
using Spectre.Tables;
using Spectre.Expressions;

namespace Spectre.Statements
{

    public abstract class Statement
    {

        public enum RaiseAlert : byte
        {
            /// <summary>
            /// No element get's raised
            /// </summary>
            NoRaise,

            /// <summary>
            /// Breaks a loop. In the case of nested loops, doesn't break the parent loops.
            /// </summary>
            Break,

            /// <summary>
            /// Breaks all loops in a given statement
            /// </summary>
            Return,

            /// <summary>
            /// Terminates the entire program
            /// </summary>
            Exit

        }

        public enum AssignmentType
        {
            Equals,
            PlusEquals,
            MinusEquals,
            MultEquals,
            DivEquals,
            Div2Equals,
            ModEquals,
            Mod2Equals,
            AndEquals,
            OrEquals,
            XorEquals,
            PlusPlus,
            MinusMinus
        }

        protected Host _Host;
        protected Statement _Parent;
        protected List<Statement> _Children;
        protected ExpressionCollection _Parameters;
        protected RaiseAlert _Raise = RaiseAlert.NoRaise;
        protected List<string> _BoundTables = new List<string>();

        public Statement(Host Host, Statement Parent)
        {
            this._Host = Host;
            this._Parent = Parent;
            this._Children = new List<Statement>();
            this._Parameters = new ExpressionCollection();
        }

        public Host Host
        {
            get { return this._Host; }
        }

        public Statement Parent
        {
            get { return this._Parent; }
        }

        public List<Statement> Children
        {
            get { return this._Children; }
        }

        public ExpressionCollection Parameters
        {
            get { return this._Parameters; }
        }

        public RaiseAlert RaiseElement
        {
            get { return this._Raise; }
        }

        public virtual bool IsBreakable
        {
            get { return false; }
        }

        public virtual bool IsReturnable
        {
            get { return false; }
        }

        public void Raise(RaiseAlert Raise)
        {
            if (this._Parent != null)
                this._Raise = Raise;
        }

        public virtual void BeginInvoke(SpoolSpace Memory)
        {

        }

        public virtual void BeginInvokeChildren(SpoolSpace Memory)
        {
            foreach (Statement s in this._Children)
                s.BeginInvoke(Memory);
        }

        public virtual void EndInvoke(SpoolSpace Memory)
        {

        }

        public virtual void EndInvokeChildren(SpoolSpace Memory)
        {
            foreach (Statement s in this._Children)
                s.EndInvoke(Memory);
        }

        public void BindTables(SpoolSpace Memory)
        {
            foreach(Expression x in this._Parameters.AllTables)
            {
                Memory.Add(x.NameOf(), new Spool.RecordSpindle(x.NameOf(), x.Columns));
                this._BoundTables.Add(x.NameOf());
            }
        }

        public void BurnBoundTables(SpoolSpace Memory)
        {
            foreach(string t in this._BoundTables)
            {
                Memory.Drop(t);
            }
        }

        public abstract void Invoke(SpoolSpace Memory);

        public virtual void InvokeChildren(SpoolSpace Memory)
        {
            foreach (Statement s in this._Children)
            {
                s.Invoke(Memory);
                if (this.RaiseElement == RaiseAlert.Break || this.RaiseElement == RaiseAlert.Return || this.RaiseElement == RaiseAlert.Exit)
                    break;
            }
        }

        public void AddChild(Statement Value)
        {
            Value._Parent = Value;
            this._Children.Add(Value);
        }

        public void AddChildren(params Statement[] Values)
        {
            foreach (Statement s in Values)
                this.AddChild(s);
        }

        public void CheckParameters(int MinParametCount, int MaxParamCount)
        {

            int Len = this._Parameters.Count;

            // No parameter constraints
            if (MinParametCount == -1 && MaxParamCount == -1)
                return;

            // No lower bound, but we max out at a certain count
            if (MinParametCount == -1 && Len <= MaxParamCount)
                return;

            // No upper bound, but we have a minium count
            if (MinParametCount <= Len && MaxParamCount == -1)
                return;

            // Lower and upper bound
            if (MinParametCount <= Len && MaxParamCount >= Len)
                return;

            throw new Exception(string.Format("Invalid paramter length {0}; expecting something between {1},{2}", Len, (MinParametCount == -1 ? 0 : MinParametCount), (MaxParamCount == -1 ? "Unlimited" : MaxParamCount.ToString())));

        }

        public sealed class Assignment : Statement
        {

            private string _LibName;
            private string _VarName;
            private AssignmentType _Affinity;

            public Assignment(Host Host, Statement Parent, string LibName, string VarName, AssignmentType Affinity)
                : base(Host, Parent)
            {
                this._LibName = LibName;
                this._VarName = VarName;
                this._Affinity = Affinity;
            }

            public override void Invoke(SpoolSpace Memory)
            {

                if (this._Parameters.Count == 0 && (this._Affinity == AssignmentType.MinusMinus || this._Affinity == AssignmentType.PlusPlus))
                {
                    this.AssignVar(Memory);
                }
                else if (this._Parameters.Count == 1 && this._Affinity != AssignmentType.MinusMinus && this._Affinity != AssignmentType.PlusPlus)
                {
                    this.AssignVar(Memory);
                }
                else if (this._Parameters.Count == 1 && (this._Affinity == AssignmentType.MinusMinus || this._Affinity == AssignmentType.PlusPlus))
                {
                    this.AssignArray(Memory);
                }
                else if (this._Parameters.Count >= 2 && this._Affinity != AssignmentType.MinusMinus && this._Affinity != AssignmentType.PlusPlus)
                {
                    this.AssignArray(Memory);
                }
                else
                {
                    throw new Exception("Assignment is invalid");
                }

            }

            private void AssignVar(SpoolSpace Memory)
            {

                Cell x = (this._Parameters.Count == 0 ? CellValues.NullINT : this._Parameters[0].Evaluate(Memory));
                switch (this._Affinity)
                {

                    case AssignmentType.Equals:
                        Memory[this._LibName][this._VarName] = x;
                        break;
                    case AssignmentType.PlusEquals:
                        Memory[this._LibName][this._VarName] += x;
                        break;
                    case AssignmentType.MinusEquals:
                        Memory[this._LibName][this._VarName] -= x;
                        break;
                    case AssignmentType.MultEquals:
                        Memory[this._LibName][this._VarName] *= x;
                        break;
                    case AssignmentType.DivEquals:
                        Memory[this._LibName][this._VarName] /= x;
                        break;
                    case AssignmentType.Div2Equals:
                        Memory[this._LibName][this._VarName] = CellOperations.CheckDivide(Memory[this._LibName][this._VarName], x);
                        break;
                    case AssignmentType.ModEquals:
                        Memory[this._LibName][this._VarName] %= x;
                        break;
                    case AssignmentType.Mod2Equals:
                        Memory[this._LibName][this._VarName] = CellOperations.CheckMod(Memory[this._LibName][this._VarName], x);
                        break;
                    case AssignmentType.AndEquals:
                        Memory[this._LibName][this._VarName] &= x;
                        break;
                    case AssignmentType.OrEquals:
                        Memory[this._LibName][this._VarName] |= x;
                        break;
                    case AssignmentType.XorEquals:
                        Memory[this._LibName][this._VarName] ^= x;
                        break;
                    case AssignmentType.PlusPlus:
                        Memory[this._LibName][this._VarName]++;
                        break;
                    case AssignmentType.MinusMinus:
                        Memory[this._LibName][this._VarName]--;
                        break;
                    default:
                        throw new Exception("Operation is invalid");

                }


            }

            private void AssignArray(SpoolSpace Memory)
            {

                if (this._Parameters.Count < 2)
                    throw new Exception("Cannot assign an array without an index");

                Cell x = Memory[this._LibName][this._VarName];
                if (!x.IsArray)
                    throw new Exception("Requires an array");
                //Cell y = this._Parameters[this._Parameters.Count - 1].Evaluate(Memory);

                CellArray z = x.valueARRAY;
                int Offset = (this._Affinity == AssignmentType.PlusPlus || this._Affinity == AssignmentType.MinusMinus ? 0 : 1);
                int idx = 0;
                for (int i = 0; i < this._Parameters.Count - Offset - 1; i++)
                {
                    idx = this._Parameters[i].Evaluate(Memory).valueINT;
                    if (!x[idx].IsArray)
                        throw new Exception("Can only access an index from an array");
                    x = x[idx];
                }

                Cell q = (Offset == 0 ? CellValues.NullINT : this._Parameters[this._Parameters.Count -1].Evaluate(Memory));
                switch (this._Affinity)
                {

                    case AssignmentType.Equals:
                        z[idx] = q;
                        return;
                    case AssignmentType.PlusEquals:
                        z[idx] += q;
                        return;
                    case AssignmentType.MinusEquals:
                        z[idx] -= q;
                        return;
                    case AssignmentType.MultEquals:
                        z[idx] *= q;
                        return;
                    case AssignmentType.DivEquals:
                        z[idx] /= q;
                        return;
                    case AssignmentType.Div2Equals:
                        z[idx] = CellOperations.CheckDivide(z[idx], q);
                        return;
                    case AssignmentType.ModEquals:
                        z[idx] %= q;
                        return;
                    case AssignmentType.Mod2Equals:
                        z[idx] = CellOperations.CheckMod(z[idx], q);
                        return;
                    case AssignmentType.AndEquals:
                        z[idx] &= q;
                        return;
                    case AssignmentType.OrEquals:
                        z[idx] |= q;
                        return;
                    case AssignmentType.XorEquals:
                        z[idx] ^= q;
                        return;
                    case AssignmentType.PlusPlus:
                        z[idx]++;
                        return;
                    case AssignmentType.MinusMinus:
                        z[idx]--;
                        return;

                }

                throw new Exception("Operation is invalid");


            }

        }

        public sealed class Print : Statement
        {

            public Print(Host Host, Statement Parent)
                : base(Host, Parent)
            {

            }

            public override void Invoke(SpoolSpace Memory)
            {

                Cell x = this._Parameters[0].Evaluate(Memory);
                if (x.Affinity == CellAffinity.TREF)
                {
                    Table t = this._Host.OpenTable(x.valueTREF);
                    using (RecordReader rr = t.OpenReader())
                    {
                        this._Host.IO.WriteLine(t.Columns.ToNameString());
                        while (rr.CanAdvance)
                        {
                            this._Host.IO.WriteLine(rr.ReadNext().ToString());
                        }
                    }
                }
                else
                {
                    this._Host.IO.WriteLine(x.valueCSTRING);
                }

            }

        }

        public sealed class DoSet : Statement
        {

            public DoSet(Host Host, Statement Parent)
                : base(Host, Parent)
            {

            }

            public override void Invoke(SpoolSpace Memory)
            {
                this.InvokeChildren(Memory);
            }

        }

        public sealed class ForeachLoop : Statement
        {

            private string _LibName;
            private string _VarName;

            public ForeachLoop(Host Host, Statement Master, string LibName, string VarName)
                : base(Host, Master)
            {
                this._LibName = LibName;
                this._VarName = VarName;
            }

            public override void BeginInvoke(SpoolSpace Memory)
            {
                base.BeginInvoke(Memory);
                this.BeginInvokeChildren(Memory);
            }

            public override void Invoke(SpoolSpace Memory)
            {

                Cell c = this._Parameters[0].Evaluate(Memory);

                if (c.Affinity == CellAffinity.BYTE)
                {
                    this.ForEachByte(Memory, c.valueBYTE);
                }
                else if (c.Affinity == CellAffinity.SHORT)
                {
                    this.ForEachShort(Memory, c.valueSHORT);
                }
                else if (c.Affinity == CellAffinity.INT)
                {
                    this.ForEachInt(Memory, c.valueINT);
                }
                else if (c.Affinity == CellAffinity.LONG)
                {
                    this.ForEachLong(Memory, c.valueLONG);
                }
                else if (c.Affinity == CellAffinity.BINARY)
                {
                    this.ForEachBinary(Memory, c.valueBINARY, false);
                }
                else if (c.Affinity == CellAffinity.BSTRING)
                {
                    this.ForEachBString(Memory, c.valueBSTRING);
                }
                else if (c.Affinity == CellAffinity.CSTRING)
                {
                    this.ForEachCString(Memory, c.valueCSTRING);
                }
                else if (c.Affinity == CellAffinity.ARRAY)
                {
                    this.ForEachArray(Memory, c.valueARRAY, false);
                }
                else if (c.Affinity == CellAffinity.TREF)
                {
                    this.ForEachTable(Memory, this._Host.OpenTable(c.valueTREF));
                }
                else
                {
                    throw new Exception();
                }

            }

            public override void EndInvoke(SpoolSpace Memory)
            {
                this.EndInvokeChildren(Memory);
                base.EndInvoke(Memory);
            }

            public override bool IsBreakable
            {
                get
                {
                    return true;
                }
            }

            private void ForEachByte(SpoolSpace Memory, byte Value)
            {

                Memory[this._LibName].Declare(this._VarName, CellValues.EmptyBYTE);
                for (byte b = 0; b < Value; b++)
                {
                    Memory[this._LibName][this._VarName] = new Cell(b);
                    this.InvokeChildren(Memory);
                    if (this.RaiseElement == RaiseAlert.Break || this.RaiseElement == RaiseAlert.Return || this.RaiseElement == RaiseAlert.Exit)
                        break;
                }

            }

            private void ForEachShort(SpoolSpace Memory, short Value)
            {

                Memory[this._LibName].Declare(this._VarName, CellValues.EmptySHORT);
                for (short b = 0; b < Value; b++)
                {
                    Memory[this._LibName][this._VarName] = new Cell(b);
                    this.InvokeChildren(Memory);
                    if (this.RaiseElement == RaiseAlert.Break || this.RaiseElement == RaiseAlert.Return || this.RaiseElement == RaiseAlert.Exit)
                        break;
                }

            }

            private void ForEachInt(SpoolSpace Memory, int Value)
            {

                Memory[this._LibName].Declare(this._VarName, CellValues.EmptyINT);
                for (int b = 0; b < Value; b++)
                {
                    Memory[this._LibName][this._VarName] = new Cell(b);
                    this.InvokeChildren(Memory);
                    if (this.RaiseElement == RaiseAlert.Break || this.RaiseElement == RaiseAlert.Return || this.RaiseElement == RaiseAlert.Exit)
                        break;
                }

            }

            private void ForEachLong(SpoolSpace Memory, long Value)
            {

                Memory[this._LibName].Declare(this._VarName, CellValues.EmptyLONG);
                for (long b = 0; b < Value; b++)
                {
                    Memory[this._LibName][this._VarName] = new Cell(b);
                    this.InvokeChildren(Memory);
                    if (this.RaiseElement == RaiseAlert.Break || this.RaiseElement == RaiseAlert.Return || this.RaiseElement == RaiseAlert.Exit)
                        break;
                }

            }

            private void ForEachBinary(SpoolSpace Memory, byte[] Binary, bool Update)
            {

                Memory[this._LibName].Declare(this._VarName, CellValues.NullBYTE);
                int i = 0;
                foreach (byte b in Binary)
                {
                    Memory[this._LibName][this._VarName] = new Cell(b);
                    this.InvokeChildren(Memory);
                    if (Update)
                    {
                        Binary[i] = Memory[this._LibName][this._VarName];
                        i++;
                    }
                    if (this.RaiseElement == RaiseAlert.Break || this.RaiseElement == RaiseAlert.Return || this.RaiseElement == RaiseAlert.Exit)
                        break;
                }

            }

            private void ForEachBString(SpoolSpace Memory, BString Text)
            {

                Memory[this._LibName].Declare(this._VarName, CellValues.NullBSTRING);
                foreach (byte b in Text.ToByteArray)
                {
                    Memory[this._LibName][this._VarName] = new BString(b);
                    this.InvokeChildren(Memory);
                    if (this.RaiseElement == RaiseAlert.Break || this.RaiseElement == RaiseAlert.Return || this.RaiseElement == RaiseAlert.Exit)
                        break;
                }

            }

            private void ForEachCString(SpoolSpace Memory, string Text)
            {

                Memory[this._LibName].Declare(this._VarName, CellValues.EmptyCSTRING);
                foreach (char c in Text)
                {
                    Memory[this._LibName][this._VarName] = c.ToString();
                    this.InvokeChildren(Memory);
                    if (this.RaiseElement == RaiseAlert.Break || this.RaiseElement == RaiseAlert.Return || this.RaiseElement == RaiseAlert.Exit)
                        break;
                }

            }

            private void ForEachArray(SpoolSpace Memory, CellArray Values, bool Update)
            {

                Memory[this._LibName].Declare(this._VarName, CellValues.NullBOOL);
                int i = 0;
                foreach (Cell b in Values)
                {
                    Memory[this._LibName][this._VarName] = b;
                    this.InvokeChildren(Memory);
                    if (Update)
                    {
                        Values[i] = Memory[this._LibName][this._VarName];
                        i++;
                    }
                    if (this.RaiseElement == RaiseAlert.Break || this.RaiseElement == RaiseAlert.Return || this.RaiseElement == RaiseAlert.Exit)
                        break;
                }

            }

            private void ForEachTable(SpoolSpace Memory, Table Value)
            {

                //Memory[this._LibName].Declare(this._VarName, CellValues.NullARRAY);
                Memory.Add(this._VarName, new Spool.RecordSpindle(this._VarName, Value.Columns));
                using (RecordReader rr = Value.OpenReader())
                {
                    while (rr.CanAdvance)
                    {
                        Memory[this._VarName].Set(rr.ReadNext());
                        this.InvokeChildren(Memory);
                        if (this.RaiseElement == RaiseAlert.Break || this.RaiseElement == RaiseAlert.Return || this.RaiseElement == RaiseAlert.Exit)
                            break;
                    }
                }
                Memory.Drop(this._VarName);

            }
            
        }

        public sealed class ForLoop : Statement
        {

            private Statement _L0;
            private Expression _L1;
            private Statement _L2;

            public ForLoop(Host Host, Statement Parent, Statement Start, Expression Predicate, Statement Loop)
                : base(Host, Parent)
            {
                this._L0 = Start;
                this._L1 = Predicate;
                this._L2 = Loop;
            }

            public override bool IsBreakable
            {
                get
                {
                    return true;
                }
            }

            public override void Invoke(SpoolSpace Memory)
            {

                this._L0.Invoke(Memory);

                while (this._L1.Evaluate(Memory).valueBOOL)
                {
                    this.InvokeChildren(Memory);
                    if (this.RaiseElement == RaiseAlert.Break || this.RaiseElement == RaiseAlert.Return || this.RaiseElement == RaiseAlert.Exit)
                        break;
                    this._L2.Invoke(Memory);
                }

            }

        }

        public sealed class WhileLoop : Statement
        {

            public WhileLoop(Host Host, Statement Parent)
                : base(Host, Parent)
            {

            }

            public override bool IsBreakable
            {
                get
                {
                    return true;
                }
            }

            public override void Invoke(SpoolSpace Memory)
            {

                Expression x = this._Parameters[0];

                while (x.Evaluate(Memory).valueBOOL)
                {
                    this.InvokeChildren(Memory);
                    if (this.RaiseElement == RaiseAlert.Break || this.RaiseElement == RaiseAlert.Return || this.RaiseElement == RaiseAlert.Exit)
                        break;
                }

            }

        }

        public sealed class If : Statement
        {

            private bool _HasElse = false;

            public If(Host Host, Statement Parent, bool HasElse)
                : base(Host, Parent)
            {
                this._HasElse = HasElse;
            }

            public override void Invoke(SpoolSpace Memory)
            {

                int i = 0;
                foreach (Expression x in this._Parameters)
                {

                    if (x.Evaluate(Memory).valueBOOL == true)
                    {
                        this._Children[i].Invoke(Memory);
                        return;
                    }
                    i++;

                }

                if (this._HasElse)
                    this._Children.Last().Invoke(Memory);

            }

        }

        public sealed class Break : Statement
        {

            public Break(Host Host, Statement Statement)
                : base(Host, Statement)
            {

            }

            public override void Invoke(SpoolSpace Memory)
            {

                Statement x = this._Parent;
                while (x != null)
                {
                    if (x.IsBreakable)
                    {
                        x.Raise(RaiseAlert.Break);
                        return;
                    }
                    else
                    {
                        x = x._Parent;
                    }
                }

            }

        }

        public sealed class Return : Statement
        {

            public Return(Host Host, Statement Statement)
                : base(Host, Statement)
            {

            }

            public override void Invoke(SpoolSpace Memory)
            {

                Statement x = this._Parent;
                while (x != null)
                {
                    if (x.IsReturnable)
                    {
                        x.Raise(RaiseAlert.Return);
                        return;
                    }
                    else
                    {
                        x = x._Parent;
                    }
                }

            }

        }

        public sealed class Exit : Statement
        {

            public Exit(Host Host, Statement Statement)
                : base(Host, Statement)
            {

            }

            public override void Invoke(SpoolSpace Memory)
            {

                Statement x = this._Parent;
                while (x != null)
                {
                    x.Raise(RaiseAlert.Exit);
                    x = x._Parent;
                }

            }

        }

        public sealed class InsertIntoTable : Statement
        {

            private string _LibName;
            private string _VarName;
            private string _Path;
            private RecordWriter _Writer;
            private ExpressionCollection _Select;

            public InsertIntoTable(Host Host, Statement Parent, string LibName, string VarName)
                : base(Host, Parent)
            {
                this._LibName = LibName;
                this._VarName = VarName;
            }

            public override void BeginInvoke(SpoolSpace Memory)
            {
                this._Path = Memory[this._LibName][this._VarName];
                this._Writer = this._Host.OpenTable(this._Path).OpenWriter();
                this._Select = new ExpressionCollection(this._Parameters[0] as Expression.ArrayLiteral);
            }

            public override void Invoke(SpoolSpace Memory)
            {
                Record r = this._Select.ToRecord(Memory);
                this._Writer.Insert(r);
            }

            public override void EndInvoke(SpoolSpace Memory)
            {
                this._Writer.Close();
            }


        }

        public sealed class InsertIntoArray : Statement
        {

            private string _LibName;
            private string _VarName;

            public InsertIntoArray(Host Host, Statement Parent, string LibName, string VarName)
                : base(Host, Parent)
            {
                this._LibName = LibName;
                this._VarName = VarName;
            }

            public override void BeginInvoke(SpoolSpace Memory)
            {
            }

            public override void Invoke(SpoolSpace Memory)
            {
                Memory[this._LibName][this._VarName].ARRAY.Append(this._Parameters[0].Evaluate(Memory));
            }

            public override void EndInvoke(SpoolSpace Memory)
            {
            }


        }

        public sealed class InLine : Statement
        {
            
            public InLine(Host Host, Statement Parent)
                : base(Host, Parent)
            {

            }

            public override void Invoke(SpoolSpace Memory)
            {

                this.CheckParameters(1, 3);

                if (this._Parameters.Count == 2)
                    throw new Exception("Inlining requires either one or three parameters");

                Cell RawScripts = this._Parameters[0].Evaluate(Memory);
                string[] Scripts = RawScripts.Affinity == CellAffinity.ARRAY ? new string[] { RawScripts.valueCSTRING } : CellConverter.ToCStringArray(RawScripts.valueARRAY);
                string[] SearchFor = (this._Parameters.Count != 3 ? new string[] { } : CellConverter.ToCStringArray(this._Parameters[1].Evaluate(Memory)));
                string[] ReplaceWith = (this._Parameters.Count != 3 ? new string[] { } : CellConverter.ToCStringArray(this._Parameters[2].Evaluate(Memory)));

                for (int i = 0; i < Scripts.Length; i++)
                {
                    string Script = this.FixParameters(Scripts[i], SearchFor, ReplaceWith);
                    this._Host.Engine.Execute(Script);
                }


            }

            private string FixParameters(string Script, string[] SearchFor, string[] ReplaceWith)
            {

                if (SearchFor.Length != ReplaceWith.Length)
                    throw new Exception("The paramter and value arrays must be the same length");

                StringBuilder sb = new StringBuilder(Script);
                for(int i = 0; i < SearchFor.Length; i++)
                {
                    sb.Replace(SearchFor[i], ReplaceWith[i]);
                }

                return sb.ToString();

            }


        }

    }

}
