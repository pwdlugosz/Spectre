using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Control;
using Spectre.Cells;
using Spectre.Tables;

namespace Spectre.Expressions
{

    /// <summary>
    /// Base class for all scalar expressions
    /// </summary>
    public abstract class Expression
    {

        protected Host _Host;
        protected Expression _Parent;
        protected List<Expression> _Children;
        protected string _Alias;

        public Expression(Host Host, Expression Parent)
        {
            this._Host = Host;
            this._Parent = Parent;
            this._Children = new List<Expression>();
        }

        public Host Host
        {
            get { return this._Host; }
        }

        public Expression Parent
        {
            get { return this._Parent; }
        }

        public virtual List<Expression> Children
        {
            get { return this._Children; }
        }
        
        public virtual Schema Columns
        {
            get { return new Schema(); }
        }

        public void AddChild(Expression Child)
        {
            this._Children.Add(Child);
        }

        public void AddChild(params Expression[] Children)
        {
            foreach (Expression x in Children)
                this.AddChild(x);
        }

        public bool ContainsAggregate
        {
            get
            {
                foreach(Expression x in this._Children)
                {
                    if (x.IsAggregate) return true;
                }
                return false;
            }
        }

        public virtual bool IsAggregate
        {
            get { return false; }
        }
        
        public virtual string Unparse()
        {
            return this.GetType().ToString();
        }

        public virtual CellAffinity TypeOf()
        {
            return CellAffinity.VARIANT;
        }

        public virtual int SizeOf()
        {
            return -1;
        }

        public virtual string NameOf()
        {
            return this._Alias ?? "F00";
        }

        public virtual string NameOf(string Default)
        {
            return this._Alias ?? Default;
        }

        internal string Name
        {
            get { return this._Alias; }
            set { this._Alias = value; }
        }

        public virtual Cell Initialize(SpoolSpace Memory)
        {
            return CellValues.NullINT;
        }

        public virtual Cell Accumulate(SpoolSpace Memory, Cell Work)
        {
            return Work;
        }
        
        public virtual Cell AggRender(Cell Work)
        {
            return Work;
        }

        public abstract Cell Evaluate(SpoolSpace Memory);

        public virtual Table Select(SpoolSpace Memory)
        {
            Cell val = this.Evaluate(Memory);
            if (val.Affinity == CellAffinity.TREF)
                return this._Host.OpenTable(val.valueTREF);
            return null;
        }

        public virtual void WriteTo(SpoolSpace Memory, RecordWriter Writer)
        {
            Cell val = this.Evaluate(Memory);
            if (val.Affinity == CellAffinity.TREF)
            {
                Table t = this._Host.OpenTable(val.valueTREF);
                using (RecordReader rr = t.OpenReader())
                {
                    while (rr.CanAdvance)
                    {
                        Writer.Insert(rr.ReadNext());
                    }
                }
            }
        }

        public static Expression TrueForAll
        {
            get
            {
                return new Expression.Literal(null, null, CellValues.True);
            }
        }

        public static Expression FalseForAll
        {
            get
            {
                return new Expression.Literal(null, null, CellValues.False);
            }
        }

        // Protected //
        protected Tuple<CellAffinity, int> SizeTypeOf(List<Expression> Xs)
        {

            int MaxSize = -1;
            CellAffinity MaxAffinity = CellAffinity.BOOL;

            foreach (Expression x in Xs)
            {

                CellAffinity ca = x.TypeOf();
                MaxAffinity = CellAffinityHelper.Highest(MaxAffinity, ca);
                if (MaxAffinity == ca) MaxSize = Math.Max(MaxSize, x.SizeOf());

            }

            return new Tuple<CellAffinity, int>(MaxAffinity, MaxSize);

        }

        // Derived Classes //
        public abstract class Aggregate : Expression
        {
            
            public Aggregate(Host Host)
                : base(Host, null)
            {
            }

            public override bool IsAggregate
            {
                get
                {
                    return true;
                }
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                throw new Exception();
            }

            public sealed class Min : Aggregate
            {

                public Min(Host Host)
                    : base(Host)
                {
                }

                public override Cell Initialize(SpoolSpace Memory)
                {
                    return CellValues.MaxCSTRING;
                }

                public override Cell Accumulate(SpoolSpace Memory, Cell Work)
                {
                    Cell x = this._Children[0].Evaluate(Memory);
                    if (!x.IsNull)
                        return CellFunctions.Min(Work, x);
                    return Work;
                }

                public override Cell AggRender(Cell Work)
                {
                    return Work;
                }

            }

            public sealed class Max : Aggregate
            {

                public Max(Host Host)
                    : base(Host)
                {
                }

                public override Cell Initialize(SpoolSpace Memory)
                {
                    return CellValues.MinBOOL;
                }

                public override Cell Accumulate(SpoolSpace Memory, Cell Work)
                {
                    Cell x = this._Children[0].Evaluate(Memory);
                    if (!x.IsNull)
                        return CellFunctions.Max(Work, x);
                    return Work;
                }

                public override Cell AggRender(Cell Work)
                {
                    return Work;
                }

            }

            public sealed class First : Aggregate
            {

                private bool _Trigger = false;

                public First(Host Host)
                    : base(Host)
                {
                }

                public override Cell Initialize(SpoolSpace Memory)
                {
                    return CellValues.NullBOOL;
                }

                public override Cell Accumulate(SpoolSpace Memory, Cell Work)
                {
                    Cell x = this._Children[0].Evaluate(Memory);
                    if (this._Trigger == false)
                        return x;
                    return Work;
                }

                public override Cell AggRender(Cell Work)
                {
                    return Work;
                }

            }

            public sealed class Last : Aggregate
            {
                
                public Last(Host Host)
                    : base(Host)
                {
                }

                public override Cell Initialize(SpoolSpace Memory)
                {
                    return CellValues.MinBOOL;
                }

                public override Cell Accumulate(SpoolSpace Memory, Cell Work)
                {
                    return this._Children[0].Evaluate(Memory);
                }

                public override Cell AggRender(Cell Work)
                {
                    return Work;
                }

            }

            public sealed class Sum : Aggregate
            {

                public Sum(Host Host)
                    : base(Host)
                {
                }

                public override Cell Initialize(SpoolSpace Memory)
                {
                    return CellValues.MinBOOL;
                }

                public override Cell Accumulate(SpoolSpace Memory, Cell Work)
                {
                    Cell x = this._Children[0].Evaluate(Memory);
                    if (!x.IsNull && x.IsNumeric)
                        return Work + x;
                    return Work;
                }

                public override Cell AggRender(Cell Work)
                {
                    return Work;
                }

            }

            public sealed class Count : Aggregate
            {

                public Count(Host Host)
                    : base(Host)
                {
                }

                public override Cell Initialize(SpoolSpace Memory)
                {
                    return CellValues.MinBOOL;
                }

                public override Cell Accumulate(SpoolSpace Memory, Cell Work)
                {
                    Cell x = this._Children[0].Evaluate(Memory);
                    if (!x.IsNull && x.IsNumeric)
                        Work++;
                    return Work;
                }

                public override Cell AggRender(Cell Work)
                {
                    return Work;
                }

            }

            public sealed class Average : Aggregate
            {

                public Average(Host Host)
                    : base(Host)
                {
                }

                public override Cell Initialize(SpoolSpace Memory)
                {
                    Cell x = CellValues.MinBYTE;
                    return new Cell(new CellArray(new Cell[] { x, x }));
                }

                public override Cell Accumulate(SpoolSpace Memory, Cell Work)
                {

                    if (!Work.IsArray)
                        throw new Exception("Expecting an array");

                    Cell x = this._Children[0].Evaluate(Memory);
                    Cell w = (this._Children.Count == 2 ? this._Children[1].Evaluate(Memory) : CellValues.OneDOUBLE);
                    if (!x.IsNull && !w.IsNull && x.IsNumeric && w.IsNumeric)
                    {
                        // Spool.Record.Array.Value
                        Work[0] += w;
                        Work[1] += x * w;
                    }
                    return Work;
                }

                public override Cell AggRender(Cell Work)
                {
                    if (Work[0].IsZero)
                        return CellValues.Null(Work[1].Affinity);
                    return Work[1] / Work[0];
                }

            }

            public sealed class VariancePopulation : Aggregate
            {

                public VariancePopulation(Host Host)
                    : base(Host)
                {
                }

                public override Cell Initialize(SpoolSpace Memory)
                {
                    Cell x = CellValues.MinBYTE;
                    return new Cell(new CellArray(new Cell[] { x, x, x }));
                }

                public override Cell Accumulate(SpoolSpace Memory, Cell Work)
                {
                    if (!Work.IsArray) throw new Exception("Expecting an array");
                    Cell x = this._Children[0].Evaluate(Memory);
                    Cell w = (this._Children.Count == 2 ? this._Children[1].Evaluate(Memory) : CellValues.OneDOUBLE);
                    if (!x.IsNull && !w.IsNull && x.IsNumeric && w.IsNumeric)
                    {
                        Work[0] += w;
                        Work[1] += x * w;
                        Work[2] += x * x * w;
                    }
                    return Work;
                }

                public override Cell AggRender(Cell Work)
                {
                    if (Work[0].IsZero)
                        return CellValues.Null(Work[1].Affinity);
                    Work[1] = Work[1] / Work[0];
                    Work[2] = Work[2] / Work[0];
                    return Work[2] - Work[1] * Work[1];
                }

            }

            public sealed class VarianceSample : Aggregate
            {

                public VarianceSample(Host Host)
                    : base(Host)
                {
                }

                public override Cell Initialize(SpoolSpace Memory)
                {
                    Cell x = CellValues.MinBYTE;
                    return new Cell(new CellArray(new Cell[] { x, x, x }));
                }

                public override Cell Accumulate(SpoolSpace Memory, Cell Work)
                {
                    if (!Work.IsArray) throw new Exception("Expecting an array");
                    Cell x = this._Children[0].Evaluate(Memory);
                    Cell w = (this._Children.Count == 2 ? this._Children[1].Evaluate(Memory) : CellValues.OneDOUBLE);
                    if (!x.IsNull && !w.IsNull && x.IsNumeric && w.IsNumeric)
                    {
                        Work[0] += w;
                        Work[1] += x * w;
                        Work[2] += x * x * w;
                    }
                    return Work;
                }

                public override Cell AggRender(Cell Work)
                {
                    if (Work[0].IsZero)
                        return CellValues.Null(Work[1].Affinity);
                    Work[1] = Work[1] / Work[0];
                    Work[2] = Work[2] / Work[0];
                    return (Work[2] - Work[1] * Work[1]) * Work[0] / (Work[0] - CellValues.One(Work[1].Affinity));
                }

            }

            public sealed class StandardDeviationPopulation : Aggregate
            {

                public StandardDeviationPopulation(Host Host)
                    : base(Host)
                {
                }

                public override Cell Initialize(SpoolSpace Memory)
                {
                    Cell x = CellValues.MinBYTE;
                    return new Cell(new CellArray(new Cell[] { x, x, x }));
                }

                public override Cell Accumulate(SpoolSpace Memory, Cell Work)
                {
                    if (!Work.IsArray) throw new Exception("Expecting an array");
                    Cell x = this._Children[0].Evaluate(Memory);
                    Cell w = (this._Children.Count == 2 ? this._Children[1].Evaluate(Memory) : CellValues.OneDOUBLE);
                    if (!x.IsNull && !w.IsNull && x.IsNumeric && w.IsNumeric)
                    {
                        Work[0] += w;
                        Work[1] += x * w;
                        Work[2] += x * x * w;
                    }
                    return Work;
                }

                public override Cell AggRender(Cell Work)
                {
                    if (Work[0].IsZero)
                        return CellValues.Null(Work[1].Affinity);
                    Work[1] = Work[1] / Work[0];
                    Work[2] = Work[2] / Work[0];
                    return CellFunctions.Power(Work[2] - Work[1] * Work[1], new Cell(0.5));
                }

            }

            public sealed class StandardDeviationSample : Aggregate
            {

                public StandardDeviationSample(Host Host)
                    : base(Host)
                {
                }

                public override Cell Initialize(SpoolSpace Memory)
                {
                    Cell x = CellValues.MinBYTE;
                    return new Cell(new CellArray(new Cell[] { x, x, x }));
                }

                public override Cell Accumulate(SpoolSpace Memory, Cell Work)
                {
                    if (!Work.IsArray) throw new Exception("Expecting an array");
                    Cell x = this._Children[0].Evaluate(Memory);
                    Cell w = (this._Children.Count == 2 ? this._Children[1].Evaluate(Memory) : CellValues.OneDOUBLE);
                    if (!x.IsNull && !w.IsNull && x.IsNumeric && w.IsNumeric)
                    {
                        Work[0] += w;
                        Work[1] += x * w;
                        Work[2] += x * x * w;
                    }
                    return Work;
                }

                public override Cell AggRender(Cell Work)
                {
                    if (Work[0].IsZero)
                        return CellValues.Null(Work[1].Affinity);
                    Work[1] = Work[1] / Work[0];
                    Work[2] = Work[2] / Work[0];
                    return CellFunctions.Power((Work[2] - Work[1] * Work[1]) * Work[0] / (Work[0] - CellValues.One(Work[1].Affinity)), new Cell(0.5));
                }

            }

            public sealed class Covariance : Aggregate
            {

                public Covariance(Host Host)
                    : base(Host)
                {
                }

                public override Cell Initialize(SpoolSpace Memory)
                {
                    Cell x = CellValues.MinBYTE;
                    return new Cell(new CellArray(new Cell[] { x, x, x, x, x, x }));
                }

                public override Cell Accumulate(SpoolSpace Memory, Cell Work)
                {
                    if (!Work.IsArray) throw new Exception("Expecting an array");
                    Cell x = this._Children[0].Evaluate(Memory);
                    Cell y = this._Children[1].Evaluate(Memory);
                    Cell w = (this._Children.Count == 3 ? this._Children[2].Evaluate(Memory) : CellValues.OneDOUBLE);
                    if (!x.IsNull && !w.IsNull && x.IsNumeric && w.IsNumeric)
                    {
                        Work[0] += w;
                        Work[1] += x * w;
                        Work[2] += x * x * w;
                        Work[3] += y * w;
                        Work[4] += y * y * w;
                        Work[5] += x * y * w;
                    }
                    return Work;
                }

                public override Cell AggRender(Cell Work)
                {
                    if (Work[0].IsZero)
                        return CellValues.Null(Work[1].Affinity);
                    Work[1] = Work[1] / Work[0]; // avgx
                    //a[2] = a[2] / a[0] - a[1] * a[1]; // varx
                    Work[3] = Work[3] / Work[0]; // avgy
                    //a[4] = a[4] / a[0] - a[3] * a[3]; // vary
                    Work[5] = Work[5] / Work[0];
                    return Work[5] - Work[1] * Work[3];
                }

            }

            public sealed class Correlation : Aggregate
            {

                public Correlation(Host Host)
                    : base(Host)
                {
                }

                public override Cell Initialize(SpoolSpace Memory)
                {
                    Cell x = CellValues.MinBYTE;
                    return new Cell(new CellArray(new Cell[] { x, x, x, x, x, x }));
                }

                public override Cell Accumulate(SpoolSpace Memory, Cell Work)
                {
                    if (!Work.IsArray) throw new Exception("Expecting an array");
                    Cell x = this._Children[0].Evaluate(Memory);
                    Cell y = this._Children[1].Evaluate(Memory);
                    Cell w = (this._Children.Count == 3 ? this._Children[2].Evaluate(Memory) : CellValues.OneDOUBLE);
                    if (!x.IsNull && !w.IsNull && x.IsNumeric && w.IsNumeric)
                    {
                        Work[0] += w;
                        Work[1] += x * w;
                        Work[2] += x * x * w;
                        Work[3] += y * w;
                        Work[4] += y * y * w;
                        Work[5] += x * y * w;
                    }
                    return Work;
                }

                public override Cell AggRender(Cell Work)
                {
                    if (Work[0].IsZero)
                        return CellValues.Null(Work[1].Affinity);
                    Work[1] = Work[1] / Work[0]; // avgx
                    Work[2] = Work[2] / Work[0] - Work[1] * Work[1]; // varx
                    Work[3] = Work[3] / Work[0]; // avgy
                    Work[4] = Work[4] / Work[0] - Work[3] * Work[3]; // vary
                    Work[5] = Work[5] / Work[0];
                    return (Work[5] - Work[1] * Work[3]) / CellFunctions.Sqrt(Work[2] * Work[4]);
                }

            }

            public sealed class Intercept : Aggregate
            {

                public Intercept(Host Host)
                    : base(Host)
                {
                }

                public override Cell Initialize(SpoolSpace Memory)
                {
                    Cell x = CellValues.MinBYTE;
                    return new Cell(new CellArray(new Cell[] { x, x, x, x, x, x }));
                }

                public override Cell Accumulate(SpoolSpace Memory, Cell Work)
                {
                    if (!Work.IsArray) throw new Exception("Expecting an array");
                    Cell x = this._Children[0].Evaluate(Memory);
                    Cell y = this._Children[1].Evaluate(Memory);
                    Cell w = (this._Children.Count == 3 ? this._Children[2].Evaluate(Memory) : CellValues.OneDOUBLE);
                    if (!x.IsNull && !w.IsNull && x.IsNumeric && w.IsNumeric)
                    {
                        Work[0] += w;
                        Work[1] += x * w;
                        Work[2] += x * x * w;
                        Work[3] += y * w;
                        Work[4] += y * y * w;
                        Work[5] += x * y * w;
                    }
                    return Work;
                }

                public override Cell AggRender(Cell Work)
                {
                    if (Work[0].IsZero)
                        return CellValues.Null(Work[1].Affinity);
                    Work[1] = Work[1] / Work[0]; // avgx
                    Work[2] = Work[2] / Work[0] - Work[1] * Work[1]; // varx
                    Work[3] = Work[3] / Work[0]; // avgy
                    Work[4] = Work[4] / Work[0] - Work[3] * Work[3]; // vary
                    Work[5] = Work[5] / Work[0];
                    return Work[3] - (Work[5] - Work[1] * Work[3]) / Work[2] * Work[1];
                }

            }

            public sealed class Slope : Aggregate
            {

                public Slope(Host Host)
                    : base(Host)
                {
                }

                public override Cell Initialize(SpoolSpace Memory)
                {
                    Cell x = CellValues.MinBYTE;
                    return new Cell(new CellArray(new Cell[] { x, x, x, x, x, x }));
                }

                public override Cell Accumulate(SpoolSpace Memory, Cell Work)
                {
                    if (!Work.IsArray) throw new Exception("Expecting an array");
                    Cell x = this._Children[0].Evaluate(Memory);
                    Cell y = this._Children[1].Evaluate(Memory);
                    Cell w = (this._Children.Count == 3 ? this._Children[2].Evaluate(Memory) : CellValues.OneDOUBLE);
                    if (!x.IsNull && !w.IsNull && x.IsNumeric && w.IsNumeric)
                    {
                        Work[0] += w;
                        Work[1] += x * w;
                        Work[2] += x * x * w;
                        Work[3] += y * w;
                        Work[4] += y * y * w;
                        Work[5] += x * y * w;
                    }
                    return Work;
                }

                public override Cell AggRender(Cell Work)
                {
                    if (Work[0].IsZero)
                        return CellValues.Null(Work[1].Affinity);
                    Work[1] = Work[1] / Work[0]; // avgx
                    Work[2] = Work[2] / Work[0] - Work[1] * Work[1]; // varx
                    Work[3] = Work[3] / Work[0]; // avgy
                    Work[4] = Work[4] / Work[0] - Work[3] * Work[3]; // vary
                    Work[5] = Work[5] / Work[0];
                    return (Work[5] - Work[1] * Work[3]) / Work[2];
                }

            }

            public static Aggregate Render(Host Host, string Name)
            {

                switch(Name.ToUpper().Trim())
                {
                    case "MIN":
                        return new Aggregate.Min(Host);
                    case "MAX":
                        return new Aggregate.Max(Host);
                    case "SUM":
                        return new Aggregate.Sum(Host);
                    case "COUNT":
                        return new Aggregate.Count(Host);
                    case "FIRST":
                        return new Aggregate.First(Host);
                    case "LAST":
                        return new Aggregate.Last(Host);
                    case "AVERAGE":
                    case "AVG":
                        return new Aggregate.Average(Host);
                    case "VAR":
                    case "VARP":
                        return new Aggregate.VariancePopulation(Host);
                    case "VARS":
                        return new Aggregate.VarianceSample(Host);
                    case "STDEV":
                    case "STDEVP":
                        return new Aggregate.StandardDeviationPopulation(Host);
                    case "STDEVS":
                        return new Aggregate.StandardDeviationSample(Host);
                    case "COVAR":
                    case "COVARIANCE":
                        return new Aggregate.Covariance(Host);
                    case "CORR":
                    case "CORREL":
                    case "CORRELATION":
                        return new Aggregate.Correlation(Host);
                    case "INTERCPT":
                        return new Aggregate.Intercept(Host);
                    case "SLOPE":
                        return new Aggregate.Slope(Host);
                }

                throw new Exception(string.Format("Function '{0}' is not an aggregate", Name));

            }

        }

        public abstract class Unary : Expression
        {

            public Unary(Host Host, Expression Parent, Expression Value)
                : base(Host, Parent)
            {
                this.AddChild(Value);
            }

            public sealed class Minus : Unary
            {

                public Minus(Host Host, Expression Parent, Expression Value)
                    : base(Host, Parent, Value)
                {

                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return -this._Children[0].Evaluate(Memory);
                }

            }

            public sealed class Not : Unary
            {

                public Not(Host Host, Expression Parent, Expression Value)
                    : base(Host, Parent, Value)
                {

                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return !this._Children[0].Evaluate(Memory);
                }

            }

            public sealed class Question : Unary
            {

                public Question(Host Host, Expression Parent, Expression Value)
                    : base(Host, Parent, Value)
                {

                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return this._Children[0].Evaluate(Memory).IsNull ? CellValues.True : CellValues.False;
                }

            }

        }

        public abstract class Binary : Expression
        {

            public Binary(Host Host, Expression Parent, Expression A, Expression B)
                : base(Host, Parent)
            {
                this.AddChild(A, B);
            }

            public sealed class Add : Binary
            {

                public Add(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return this._Children[0].Evaluate(Memory) + this._Children[1].Evaluate(Memory);
                }

            }

            public sealed class Subtract : Binary
            {

                public Subtract(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return this._Children[0].Evaluate(Memory) - this._Children[1].Evaluate(Memory);
                }

            }

            public sealed class Multiply : Binary
            {

                public Multiply(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    //Cell x = this._Children[0].Evaluate(Memory);
                    //Cell y = this._Children[1].Evaluate(Memory);
                    return this._Children[0].Evaluate(Memory) * this._Children[1].Evaluate(Memory);
                }

            }

            public sealed class Divide : Binary
            {

                public Divide(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return this._Children[0].Evaluate(Memory) / this._Children[1].Evaluate(Memory);
                }

            }

            public sealed class CheckDivide : Binary
            {

                public CheckDivide(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellOperations.CheckDivide(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory));
                }

            }

            public sealed class Mod : Binary
            {

                public Mod(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return this._Children[0].Evaluate(Memory) % this._Children[1].Evaluate(Memory);
                }

            }

            public sealed class CheckMod : Binary
            {

                public CheckMod(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellOperations.CheckMod(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory));
                }

            }

            public sealed class Power : Binary
            {

                public Power(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellFunctions.Power(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory));
                }

            }

            public sealed class LeftShift : Binary
            {

                public LeftShift(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellOperations.LeftShift(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory).valueINT);
                }

            }

            public sealed class RightShift : Binary
            {

                public RightShift(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellOperations.RightShift(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory).valueINT);
                }

            }

            public sealed class LeftRotate : Binary
            {

                public LeftRotate(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellOperations.LeftRotate(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory).valueINT);
                }

            }

            public sealed class RightRotate : Binary
            {

                public RightRotate(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellOperations.RightRotate(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory).valueINT);
                }

            }

            public sealed new class Equals : Binary
            {

                public Equals(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellComparer.Equals(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory).valueINT) ? CellValues.True : CellValues.False;
                }

            }

            public sealed class EqualsStrict : Binary
            {

                public EqualsStrict(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellComparer.EqualsStrict(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory).valueINT) ? CellValues.True : CellValues.False;
                }

            }

            public sealed class NotEquals : Binary
            {

                public NotEquals(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return !CellComparer.Equals(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory).valueINT) ? CellValues.True : CellValues.False;
                }

            }

            public sealed class NotEqualsStrict : Binary
            {

                public NotEqualsStrict(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return !CellComparer.EqualsStrict(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory).valueINT) ? CellValues.True : CellValues.False;
                }

            }

            public sealed class LessThan : Binary
            {

                public LessThan(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellComparer.Compare(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory).valueINT) < 0 ? CellValues.True : CellValues.False;
                }

            }

            public sealed class LessThanEquals : Binary
            {

                public LessThanEquals(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellComparer.Compare(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory).valueINT) <= 0 ? CellValues.True : CellValues.False;
                }

            }

            public sealed class GreaterThan : Binary
            {

                public GreaterThan(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellComparer.Compare(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory).valueINT) > 0 ? CellValues.True : CellValues.False;
                }

            }

            public sealed class GreaterThanEquals : Binary
            {

                public GreaterThanEquals(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellComparer.Compare(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory).valueINT) >= 0 ? CellValues.True : CellValues.False;
                }

            }

            public sealed class And : Binary
            {

                public And(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellOperations.And(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory).valueINT);
                }

            }

            public sealed class Or : Binary
            {

                public Or(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellOperations.Or(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory).valueINT);
                }

            }

            public sealed class Xor : Binary
            {

                public Xor(Host Host, Expression Parent, Expression A, Expression B)
                    : base(Host, Parent, A, B)
                {
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    return CellOperations.Xor(this._Children[0].Evaluate(Memory), this._Children[1].Evaluate(Memory).valueINT);
                }

            }

            public sealed class IfNull : Binary
            {

                public IfNull(Host Host, Expression Parent, Expression Predicate, Expression Value)
                    : base(Host, Parent, Predicate, Value)
                {
                    this.AddChild(Predicate);
                    this.AddChild(Value);
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    Cell x = this.Evaluate(Memory);
                    Cell y = this.Evaluate(Memory);
                    return (x.IsNull ? y : x);
                }

            }

            public sealed class NullIf : Binary
            {

                public NullIf(Host Host, Expression Parent, Expression Predicate, Expression Value)
                    : base(Host, Parent, Predicate, Value)
                {
                    this.AddChild(Predicate);
                    this.AddChild(Value);
                }

                public override Cell Evaluate(SpoolSpace Memory)
                {
                    Cell x = this.Evaluate(Memory);
                    Cell y = this.Evaluate(Memory);
                    return (x == y ? CellValues.Null(x.AFFINITY) : x);
                }

            }

        }

        public sealed class If : Expression
        {

            public If(Host Host, Expression Parent)
                : base(Host, Parent)
            {
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                if (this._Children.Count < 2 || this._Children.Count > 3)
                    throw new Exception("If requires either two or three arguments");

                if (this._Children.Count == 2)
                {
                    if (this._Children[0].Evaluate(Memory).valueBOOL)
                        return this._Children[1].Evaluate(Memory);
                    return CellValues.Null(this._Children[1].TypeOf() != CellAffinity.VARIANT ? this._Children[1].TypeOf() : CellAffinity.INT);
                }

                if (this._Children[0].Evaluate(Memory).valueBOOL)
                    return this._Children[1].Evaluate(Memory);
                return this._Children[2].Evaluate(Memory);

            }

        }
        
        public sealed class Literal : Expression
        {

            private Cell _value;

            public Literal(Host Host, Expression Parent, Cell Value)
                : base(Host, Parent)
            {
                this._value = Value;
            }

            public override int SizeOf()
            {
                return CellSerializer.DiskSize(this._value);
            }

            public override CellAffinity TypeOf()
            {
                return this._value.Affinity;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                return this._value;
            }

        }

        public sealed class ArrayLiteral : Expression
        {

            public ArrayLiteral(Host Host, Expression Parent)
                : base(Host, Parent)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.ARRAY;
            }

            public override int SizeOf()
            {
                return CellSerializer.LEN_SIZE + CellSerializer.META_SIZE + this._Children.Sum(x => { return x.SizeOf(); });
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                CellArray x = new CellArray();
                foreach (Expression z in this._Children)
                    x.Append(z.Evaluate(Memory));
                return x;
            }

        }

        public sealed class ArrayWildCard : Expression
        {

            private string _LibName;

            public ArrayWildCard(Host Host, Expression Expression, string LibName)
                :base(Host, Expression)
            {
                this._LibName = LibName;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                return Memory[this._LibName].GetAll();
            }

        }

        public sealed class Indexer : Expression
        {

            public Indexer(Host Host, Expression Parent, Expression Source, Expression Indexer)
                : base(Host, Parent)
            {
                this.AddChild(Source);
                this.AddChild(Indexer);
            }

            public override CellAffinity TypeOf()
            {
                CellAffinity x = this.Children[0].TypeOf();
                if (x == CellAffinity.ARRAY)
                    return CellAffinity.VARIANT;
                return x;
            }

            public override int SizeOf()
            {
                CellAffinity x = this.Children[0].TypeOf();
                if (x == CellAffinity.ARRAY)
                    return base.SizeOf();
                return this.Children[0].SizeOf();
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                Cell x = this._Children[0].Evaluate(Memory);
                Cell Idx = this._Children[1].Evaluate(Memory);
                if (x.IsNull || Idx.IsNull)
                    return CellValues.Null(x.AFFINITY);
                return x[Idx.valueINT];
            }

        }
        
        public sealed class Lookup : Expression
        {

            private string _sName;
            private string _vName;

            public Lookup(Host Host, Expression Parent, string SpoolName, string VarName)
                : base(Host, Parent)
            {
                this._sName = SpoolName;
                this._vName = VarName;
            }

            public override int SizeOf()
            {
                if (this._Host.Spools.Exists(this._sName) && this._Host.Spools[this._sName].Exists(this._vName))
                    return CellSerializer.DiskSize(this._Host.Spools[this._sName].Get(this._vName));
                return base.SizeOf();
            }

            public override CellAffinity TypeOf()
            {
                if (this._Host.Spools.Exists(this._sName) && this._Host.Spools[this._sName].Exists(this._vName))
                    return this._Host.Spools[this._sName].Get(this._vName).Affinity;
                return base.TypeOf();
            }

            public override string NameOf()
            {
                return this._Alias ?? this._vName;
            }

            public override Schema Columns
            {
                get
                {
                    if (this._Host.Spools.Exists(this._sName) && this._Host.Spools[this._sName].Exists(this._vName) && this._Host.Spools[this._sName][this._vName].Affinity == CellAffinity.TREF)
                        return this._Host.OpenTable(this._Host.Spools[this._sName][this._vName].valueTREF).Columns;
                    return base.Columns;
                }
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                return Memory[this._sName].Get(this._vName);
            }

        }
        
        public sealed class Equation : Expression
        {

            public Equation(Host Host, Expression Parent)
                : base(Host, Parent)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.EQUATION;
            }

            public override int SizeOf()
            {
                return -1;
            }

            public override string NameOf()
            {
                return this._Children.Count == 0 ? "F00" : this._Children[0].NameOf();
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                return new Cell(this._Children[0]);
            }

        }

        public sealed class Collapse : Expression
        {

            public Collapse(Host Host, Expression Parent)
                : base(Host, Parent)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                Cell c = this._Children[0].Evaluate(Memory);
                if (c.Affinity != CellAffinity.EQUATION)
                    return c;
                return c.valueEQUATION.Evaluate(Memory);
            }

        }

    }

}
