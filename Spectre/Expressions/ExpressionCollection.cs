using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Tables;
using Spectre.Cells;
using Spectre.Structures;

namespace Spectre.Expressions
{

    public sealed class ExpressionCollection : IEnumerable<Expression>, System.Collections.IEnumerable
    {

        private Heap<Expression> _Values;

        public ExpressionCollection()
        {
            this._Values = new Heap<Expression>();
        }
        
        public ExpressionCollection(Expression.ArrayLiteral Seed)
            : this()
        {
            foreach(Expression x in Seed.Children)
            {
                this.Add(x.NameOf(), x);
            }
        }

        public Schema Columns
        {
            get
            {
                Schema s = new Schema();
                foreach (var x in this._Values.Entries)
                {
                    s.Add(x.Key, x.Value.TypeOf(), x.Value.SizeOf());
                }
                return s;
            }
        }
        
        public int Count
        {
            get { return this._Values.Count; }
        }

        public bool ContainsAggregate
        {
            get
            {
                foreach(Expression x in this)
                {
                    if (x.IsAggregate) return true;
                }
                return false;
            }
        }

        public bool ContainsNonAggregate
        {
            get
            {
                foreach (Expression x in this)
                {
                    if (!x.IsAggregate) return true;
                }
                return false;
            }
        }
    
        public List<Expression> AllTables
        {
            get
            {
                List<Expression> x = new List<Expression>();
                foreach(Expression e in this._Values.Values)
                {
                    if(e.TypeOf() == CellAffinity.TREF) x.Add(e);
                }
                return x;
            }
        }

        public Record Initialize(SpoolSpace Memory)
        {
            RecordBuilder rb = new RecordBuilder();
            foreach(Expression x in this._Values)
            {
                rb.Add(x.Initialize(Memory));
            }
            return rb.ToRecord();
        }

        public Record Accumulate(SpoolSpace Memory, Record Work)
        {
            RecordBuilder rb = new RecordBuilder();
            for(int i = 0; i < this._Values.Count; i++)
            {
                rb.Add(this._Values[i].Accumulate(Memory, Work[i]));
            }
            return rb.ToRecord();
        }
        
        public Record AggRender(Record Work)
        {
            RecordBuilder rb = new RecordBuilder();
            for(int i = 0; i < Work.Count; i++)
            {
                rb.Add(this._Values[i].AggRender(Work[i]));
            }
            return rb.ToRecord();
        }

        public Record ToRecord(SpoolSpace Memory)
        {
            RecordBuilder rb = new RecordBuilder();
            foreach (Expression sx in this._Values.Values)
            {
                rb.Add(sx.Evaluate(Memory));
            }
            return rb.ToRecord();
        }

        internal Record ToAggShellRecord(SpoolSpace Memory)
        {
            RecordBuilder rb = new RecordBuilder();
            foreach (Expression x in this._Values.Values)
            {
                if (x.IsAggregate)
                    rb.Add(x.Initialize(Memory));
                else
                    rb.Add(x.Evaluate(Memory));
            }
            return rb.ToRecord();
        }

        public CellArray ToArray(SpoolSpace Memory)
        {
            CellArray x = new CellArray();
            foreach (Expression sx in this._Values.Values)
            {
                x.Append(sx.Evaluate(Memory));
            }
            return x;
        }

        public void Add(string Alias, Expression Value)
        {
            if (this._Values.Exists(Alias))
                throw new Exception(string.Format("Field '{0}' already exists", Alias));
            this._Values.Allocate(Alias, Value);
        }

        public void Add(Expression Value)
        {
            this.Add("F0" + this._Values.Count.ToString(), Value);
        }
        
        public Expression this[int Index]
        {
            get { return this._Values[Index]; }
            set { this._Values[Index] = value; }
        }

        public Expression this[string Alias]
        {
            get { return this._Values[Alias]; }
            set { this._Values[Alias] = value; }
        }

        public string Alias(int Index)
        {
            return this._Values.Name(Index);
        }

        IEnumerator<Expression> IEnumerable<Expression>.GetEnumerator()
        {
            return this._Values.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._Values.Values.GetEnumerator();
        }

        public static ExpressionCollection Unpack(Expression Value)
        {
            ExpressionCollection x = new ExpressionCollection();
            foreach(Expression y in Value.Children)
            {
                x.Add(y.NameOf("F0" + x.Count.ToString()), y);
            }
            return x;
        }

        public static Expression Pack(ExpressionCollection Value)
        {
            Expression x = new Expression.ArrayLiteral(null, null);
            foreach(Expression y in Value._Values)
            {
                x.AddChild(y);
            }
            return x;
        }

    }

}
