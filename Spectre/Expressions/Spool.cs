using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre;
using Spectre.Cells;
using Spectre.Tables;
using Spectre.Control;
using Spectre.Structures;


namespace Spectre.Expressions
{

    public abstract class Spool
    {
        
        protected string _Alias;

        public Spool(string Alias)
        {
            this._Alias = Alias;
        }
        
        public string Alias
        {
            get { return this._Alias; }
        }

        public abstract Schema Columns
        {
            get;
        }

        public abstract void Declare(string Alias, Cell Value);
        
        public abstract void Set(string Alias, Cell Value);

        public abstract void Set(Record Value);

        public abstract bool Exists(string Value);
        
        public abstract Cell Get(string Alias);

        public abstract CellArray GetAll();

        public Cell this[string Alias]
        {
            get { return this.Get(Alias); }
            set { this.Set(Alias, value); }
        }

        public sealed class HeapSpindle : Spool
        {

            private Heap<Cell> _Cells;

            public HeapSpindle(string Alias)
                : base(Alias)
            {
                this._Cells = new Heap<Cell>();
            }

            public override Schema Columns
            {
                get
                {
                    Schema s = new Schema();
                    for(int i = 0; i < this._Cells.Count; i++)
                    {
                        s.Add(this._Cells.Name(i), this._Cells[i].AFFINITY);
                    }
                    return s;
                }
            }

            public override void Declare(string Alias, Cell Value)
            {
                this._Cells.Reallocate(Alias, Value);
            }

            public override void Set(string Alias, Cell Value)
            {
                this._Cells.Reallocate(Alias, Value);
            }

            public override void Set(Record Value)
            {
                throw new Exception("Operation is invalid");
            }

            public override bool Exists(string Value)
            {
                return this._Cells.Exists(Value);
            }

            public override Cell Get(string Alias)
            {
                return this._Cells[Alias];
            }

            public override CellArray GetAll()
            {
                return new CellArray(this._Cells.Values);
            }

        }
        
        public sealed class RecordSpindle : Spool
        {

            private Schema _Columns;
            private Record _Value;

            public RecordSpindle(string Alias, Schema Columns)
                : base(Alias)
            {
                this._Columns = Columns;
                this._Value = new Record(Columns.Count);
            }

            public override Schema Columns
            {
                get
                {
                    return this._Columns;
                }
            }

            public override void Declare(string Alias, Cell Value)
            {
                throw new Exception("Operation is invalid");
            }

            public override void Set(string Alias, Cell Value)
            {
                throw new Exception("Operation is invalid");
            }

            public override void Set(Record Value)
            {
                this._Value = Value;
            }

            public override bool Exists(string Value)
            {
                return this._Columns.ColumnIndex(Value) != -1;
            }

            public override Cell Get(string Alias)
            {
                return this._Value[this._Columns.ColumnIndex(Alias)];
            }

            public override CellArray GetAll()
            {
                return new CellArray(this._Value);
            }

        }

    }

    /// <summary>
    /// Represents a collection of memory
    /// </summary>
    //public sealed class Spool
    //{

    //    private Host _Host;
    //    private string _Alias;
    //    private Heap<Cell> _Values;
        
    //    public Spool(Host Host, string Name)
    //    {
    //        this._Host = Host;
    //        this._Alias = Name;
    //        this._Values = new Heap<Cell>();
    //    }
        
    //    // Names //
    //    public string Alias
    //    {
    //        get { return this._Alias; }
    //    }

    //    // Cells //
    //    public bool Exists(string Name)
    //    {
    //        return this._Values.Exists(Name);
    //    }

    //    public void Declare(string Name, Cell Value)
    //    {
    //        this._Values.Reallocate(Name, Value);
    //    }

    //    public void Declare(string Name, Record Value)
    //    {
    //        this.Declare(Name, new CellArray(Value.BaseArray));
    //    }

    //    public void Set(string Name, Cell Value)
    //    {
    //        this._Values.Reallocate(Name, Value);
    //    }

    //    public void Set(string Name, Record Value)
    //    {
    //        this.Set(Name, new CellArray(Value.BaseArray));
    //    }

    //    public void Drop(string Name)
    //    {
    //        if (this.Exists(Name)) this._Values.Deallocate(Name);
    //    }

    //    public Cell Get(string Name)
    //    {
    //        if (this.Exists(Name))
    //            return this._Values[Name];
    //        throw new Exception(string.Format("Cell '{0}' does not exist", Name));
    //    }
        
    //    public Cell this[string Name]
    //    {
    //        get { return this.Get(Name); }
    //        set { this.Set(Name, value); }
    //    }

    //    // Tables //
    //    public bool TableExists(string Name)
    //    {
    //        return this._Values.Exists(Name) && this._Values[Name].Affinity == CellAffinity.TREF;
    //    }

    //    public Table GetTable(string Name)
    //    {
    //        if (this.TableExists(Name))
    //            return this._Host.OpenTable(this._Values[Name].valueTREF);
    //        throw new Exception(string.Format("Table '{0}' does not exist", Name));
    //    }
        
    //}

    /// <summary>
    /// Represents a collection of spools
    /// </summary>
    public sealed class SpoolSpace
    {

        private Heap<Spool> _Spools;

        public SpoolSpace()
        {
            this._Spools = new Heap<Spool>();
        }

        public SpoolSpace(SpoolSpace Seed)
            : this()
        {
            foreach (KeyValuePair<string, Spool> s in Seed._Spools.Entries)
            {
                this._Spools.Allocate(s.Key, s.Value);
            }
        }

        public Spool this[string Name]
        {
            get { return this._Spools[Name]; }
            private set { this._Spools[Name] = value; }
        }

        public List<Spool> Spools
        {
            get { return this._Spools.Values; }
        }

        public bool Exists(string Name)
        {
            return this._Spools.Exists(Name);
        }

        public void Add(string Name, Spool Value)
        {
            this._Spools.Allocate(Name, Value);
        }

        public void Drop(string Name)
        {
            this._Spools.Deallocate(Name);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class SpoolSpaceContext
    {

        public sealed class SpoolContext
        {

            private SortedSet<string> _Names;

            public SpoolContext()
            {
                this._Names = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            public void Add(string Alias)
            {
                if (this._Names.Contains(Alias))
                    this._Names.Remove(Alias);
                this._Names.Add(Alias);
            }

            public void Add(Schema Columns)
            {
                for(int i = 0; i < Columns.Count; i++)
                {
                    this.Add(Columns.ColumnName(i));
                }
            }

            public bool Exists(string Alias)
            {
                return this._Names.Contains(Alias);
            }
            
            public void Drop(string Alias)
            {
                if (this._Names.Contains(Alias))
                    this._Names.Remove(Alias);
            }
            
        }

        private Heap<SpoolContext> _Conext;
        private LinkedList<string> _NameStack;

        public SpoolSpaceContext()
        {
            this._Conext = new Heap<SpoolContext>();
            this._NameStack = new LinkedList<string>();
        }

        public string PrimaryContextName
        {
            get;
            set;
        }

        public IEnumerable<SpoolContext> Contexes
        {
            get { return this._Conext.Values; }
        }

        public SpoolContext this[string Alias]
        {
            get { return this._Conext[Alias]; }
        }

        public SpoolContext PrimaryContext
        {
            get { return this._Conext[this.PrimaryContextName]; }
        }

        public void AddSpool(string Alias)
        {
            this._Conext.Allocate(Alias, new SpoolContext());
            this.Push(Alias);
        }

        public void AddSpool(string Alias, Schema Columns)
        {
            SpoolContext x = new SpoolContext();
            x.Add(Columns);
            this._Conext.Allocate(Alias, x);
            this.Push(Alias);
        }

        public void DropSpool(string Alias)
        {
            this._Conext.Deallocate(Alias);
            this.Remove(Alias);
        }
        
        public bool SpoolExists(string Alias)
        {
            return this._Conext.Exists(Alias);
        }

        public void Import(Spool Value)
        {
            this.AddSpool(Value.Alias);
        }

        // Name stack methods
        public void Push(string Alias)
        {
            this._NameStack.AddFirst(Alias);
        }

        public string Pop()
        {
            string x = this._NameStack.First.Value;
            this._NameStack.RemoveFirst();
            return x;
        }

        public string Peek()
        {
            return this._NameStack.First.Value;
        }

        public void Remove(string Alias)
        {
            LinkedListNode<string> x = this._NameStack.First;
            while(x != null)
            {
                if (x.Value == Alias)
                {
                    this._NameStack.Remove(Alias);
                }
                x = x.Next;
            }

        }

        public IEnumerable<string> Aliases
        {
            get
            {
                return this._NameStack;
            }
        }

    }

}
