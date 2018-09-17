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


    public abstract class TableExpression : Expression
    {

        public TableExpression(Host Host, Expression Parent)
            : base(Host, Parent)
        {

        }

        public override CellAffinity TypeOf()
        {
            return CellAffinity.TREF;
        }

        public override int SizeOf()
        {
            return -1;
        }

        public abstract Schema Columns
        {
            get;
        }

        public abstract Table Select(SpoolSpace Memory); 

        public abstract void WriteTo(RecordWriter Writer);
        
        
    }

    //public sealed class txSelect : TableExpression
    //{

    //    private TableExpression _Source;
    //    private ExpressionCollection _Fields;
    //    private Expression _Where;
    //    private string _NameSpace;
    //    private string _Alias;

    //    public txSelect(Host Host, TableExpression Parent, TableExpression Source, ExpressionCollection Fields, Expression Where, string NameSpace, string Alias)
    //        : base(Host, Parent)
    //    {
    //        this._Source = Source;
    //        this._Fields = Fields;
    //        this._Where = Where;
    //        this._NameSpace = NameSpace;
    //        this._Alias = Alias;
    //    }

    //    public override List<TableExpression> Children
    //    {
    //        get
    //        {
    //            return new List<TableExpression>() { this._Source };
    //        }
    //    }

    //    public override Schema Columns
    //    {
    //        get { return this._Fields.Columns; }
    //    }

    //    public override void Evaluate(SpoolSpace Memory, RecordWriter Writer)
    //    {

    //        Table t = this._Source.Select(Memory);

    //        Loop through all records in the table //
    //        using (RecordReader rr = t.OpenReader())
    //        {

    //            Set the tensor in memory //
    //           Record x = rr.ReadNext();
    //            Memory[this._NameSpace].SetTensor(this._Alias, new Matrix(x));

    //            Check that the filter is valid //
    //            Cell y = this._Where.Evaluate(Memory);
    //            if (!y.IsNull && y.valueBOOL == true)
    //            {
    //                Record z = this._Fields.ToRecord(Memory);
    //                Writer.Insert(z);
    //            }

    //        }

    //    }

    //}



    //public abstract class TableExpression
    //{


    //    protected Host _Host;
    //    protected TableExpression _Parent;

    //    public TableExpression(Host Host, TableExpression Parent)
    //    {
    //        this._Host = Host;
    //        this._Parent = Parent;
    //    }

    //    public TableExpression Parent
    //    {
    //        get { return this._Parent; }
    //    }

    //    public virtual List<TableExpression> Children
    //    {
    //        get { return new List<TableExpression>(); }
    //    }

    //    public virtual string Unparse()
    //    {
    //        return this.GetType().ToString();
    //    }

    //    public virtual string Name()
    //    {
    //        return "X0";
    //    }

    //    public abstract Schema Columns
    //    {
    //        get;
    //    }

    //    public abstract void Evaluate(SpoolSpace Memory, RecordWriter Writer);

    //    public virtual Table Select(SpoolSpace Memory, string Path)
    //    {

    //        Table t = this._Host.CreateTable(Path, this.Columns, Page.DEFAULT_SIZE);
    //        using (RecordWriter rw = t.OpenWriter())
    //        {
    //            this.Evaluate(Memory, rw);
    //        }
    //        return t;

    //    }

    //    public virtual Table Select(SpoolSpace Memory)
    //    {
    //        return this.Select(Memory, Host.RandomPath());
    //    }

    //    // Derived Classes //
    //    public sealed class txLookup : TableExpression
    //    {

    //        private string _Path;

    //        public txLookup(Host Host, TableExpression Parent, string Path)
    //            : base(Host, Parent)
    //        {
    //            this._Path = Path;
    //        }

    //        public override Schema Columns
    //        {
    //            get { return this._Host.OpenTable(this._Path).Columns; }
    //        }

    //        public override void Evaluate(SpoolSpace Memory, RecordWriter Writer)
    //        {
    //            Writer.Consume(this._Host.OpenTable(this._Path).OpenReader());
    //        }

    //        public override Table Select(SpoolSpace Memory)
    //        {
    //            return this._Host.OpenTable(this._Path);
    //        }

    //    }

    //    //public sealed class txSelect : TableExpression
    //    //{

    //    //    private TableExpression _Source;
    //    //    private ExpressionCollection _Fields;
    //    //    private Expression _Where;
    //    //    private string _NameSpace;
    //    //    private string _Alias;

    //    //    public txSelect(Host Host, TableExpression Parent, TableExpression Source, ExpressionCollection Fields, Expression Where, string NameSpace, string Alias)
    //    //        : base(Host, Parent)
    //    //    {
    //    //        this._Source = Source;
    //    //        this._Fields = Fields;
    //    //        this._Where = Where;
    //    //        this._NameSpace = NameSpace;
    //    //        this._Alias = Alias;
    //    //    }

    //    //    public override List<TableExpression> Children
    //    //    {
    //    //        get
    //    //        {
    //    //            return new List<TableExpression>() { this._Source };
    //    //        }
    //    //    }

    //    //    public override Schema Columns
    //    //    {
    //    //        get { return this._Fields.Columns; }
    //    //    }

    //    //    public override void Evaluate(SpoolSpace Memory, RecordWriter Writer)
    //    //    {

    //    //        Table t = this._Source.Select(Memory);

    //    //        // Loop through all records in the table //
    //    //        using (RecordReader rr = t.OpenReader())
    //    //        {

    //    //            // Set the tensor in memory //
    //    //            Record x = rr.ReadNext();
    //    //            Memory[this._NameSpace].SetTensor(this._Alias, new Matrix(x));

    //    //            // Check that the filter is valid //
    //    //            Cell y = this._Where.Evaluate(Memory);
    //    //            if (!y.IsNull && y.valueBOOL == true)
    //    //            {
    //    //                Record z = this._Fields.ToRecord(Memory);
    //    //                Writer.Insert(z);
    //    //            }

    //    //        }

    //    //    }

    //    //}

    //    //public abstract class txJoin : TableExpression
    //    //{

    //    //    /// <summary>
    //    //    /// Represents a join algorithm
    //    //    /// </summary>
    //    //    public enum JoinAlgorithm
    //    //    {
    //    //        NestedLoop,
    //    //        QuasiNestedLoop,
    //    //        SortMerge
    //    //    }

    //    //    /// <summary>
    //    //    /// Represents a join type
    //    //    /// </summary>
    //    //    public enum JoinType
    //    //    {
    //    //        INNER,
    //    //        LEFT,
    //    //        ANTI_LEFT
    //    //    }

    //    //    protected TableExpression _LTable;
    //    //    protected TableExpression _RTable;
    //    //    protected ExpressionCollection _Fields;
    //    //    protected RecordMatcher _Predicate;
    //    //    protected Expression _Where;
    //    //    protected JoinType _Type;
    //    //    protected string _LAlias = "L";
    //    //    protected string _RAlias = "R";
    //    //    protected string _NameSpace;

    //    //    public txJoin(Host Host, TableExpression Parent, TableExpression LTable, string LAlias, TableExpression RTable, string RAlias, 
    //    //        ExpressionCollection Fields, RecordMatcher Predicate, Expression Where, JoinType Type, string NameSpace)
    //    //        : base(Host, Parent)
    //    //    {
    //    //        this._LTable = LTable;
    //    //        this._RTable = RTable;
    //    //        this._Fields = Fields;
    //    //        this._Predicate = Predicate;
    //    //        this._Where = Where;
    //    //        this._Type = Type;
    //    //        this._LAlias = LAlias;
    //    //        this._RAlias = RAlias;
    //    //        this._NameSpace = NameSpace;
    //    //    }

    //    //    /// <summary>
    //    //    /// Gets the underlying columns
    //    //    /// </summary>
    //    //    public override Schema Columns
    //    //    {
    //    //        get { return this._Fields.Columns; }
    //    //    }

    //    //    /// <summary>
    //    //    /// Checks if the expression can render
    //    //    /// </summary>
    //    //    protected void CheckRender()
    //    //    {
    //    //        if (this._LAlias == this._RAlias)
    //    //            throw new Exception("The left and right record pointers cannot be identical");
    //    //    }

    //    //    /// <summary>
    //    //    /// 
    //    //    /// </summary>
    //    //    /// <param name="Variants"></param>
    //    //    public void InitializeResolver(SpoolSpace Variants)
    //    //    {
    //    //        if (!Variants.Exists(this._NameSpace))
    //    //            Variants.Add(this._NameSpace, new Spool(this._Host, this._NameSpace));
    //    //        if (!Variants[this._NameSpace].TensorExists(this._LAlias))
    //    //            Variants[this._NameSpace].DeclareTensor(this._LAlias, new Matrix(this._LTable.Columns.NullRecord));
    //    //        if (!Variants[this._NameSpace].TensorExists(this._RAlias))
    //    //            Variants[this._NameSpace].DeclareTensor(this._RAlias, new Matrix(this._RTable.Columns.NullRecord));
    //    //    }

    //    //    /// <summary>
    //    //    /// 
    //    //    /// </summary>
    //    //    /// <param name="Variants"></param>
    //    //    public void CleanUpResolver(SpoolSpace Variants)
    //    //    {
    //    //        if (Variants[this._NameSpace].TensorExists(this._LAlias))
    //    //            Variants[this._NameSpace].DropTensor(this._LAlias);
    //    //        if (Variants[this._NameSpace].TensorExists(this._RAlias))
    //    //            Variants[this._NameSpace].DropTensor(this._RAlias);
    //    //    }

    //    //    // Optimizer //
    //    //    /// <summary>
    //    //    /// Calculates the optiminal join
    //    //    /// </summary>
    //    //    /// <param name="LCount"></param>
    //    //    /// <param name="RCount"></param>
    //    //    /// <returns></returns>
    //    //    public static JoinAlgorithm Optimize(long LCount, long RCount, bool LIndex, bool RIndex)
    //    //    {

    //    //        double nl = Util.CostCalculator.NestedLoopJoinCost(LCount, RCount);
    //    //        double qnl = Util.CostCalculator.QuasiNestedLoopJoinCost(LCount, RCount, LIndex);
    //    //        double sm = Util.CostCalculator.SortMergeNestedLoopJoinCost(LCount, RCount, LIndex, RIndex);

    //    //        if (sm < nl && sm < qnl)
    //    //            return JoinAlgorithm.SortMerge;
    //    //        else if (qnl < nl)
    //    //            return JoinAlgorithm.QuasiNestedLoop;

    //    //        return JoinAlgorithm.NestedLoop;

    //    //    }

    //    //    /// <summary>
    //    //    /// Creates a table expression implenting the sort merge algorithm
    //    //    /// </summary>
    //    //    public sealed class TableExpressionJoinSortMerge : txJoin
    //    //    {

    //    //        public override void Evaluate(SpoolSpace Variants, RecordWriter Writer)
    //    //        {

    //    //            // Check each table //
    //    //            this.CheckRender();

    //    //            // Render each table //
    //    //            Table Left = this._LTable.Select(Variants);
    //    //            Table Right = this._RTable.Select(Variants);

    //    //            // Fix the resolver //
    //    //            this.InitializeResolver(Variants);

    //    //            // Get the left and right join index //
    //    //            Index lidx = Left.GetIndex(this._Predicate.LeftKey);
    //    //            if (lidx == null)
    //    //                lidx = Index.BuildTemporaryIndex(Left, this._Predicate.LeftKey);
    //    //            Index ridx = Right.GetIndex(this._Predicate.RightKey);
    //    //            if (ridx == null)
    //    //                ridx = Index.BuildTemporaryIndex(Right, this._Predicate.RightKey);

    //    //            // Get the join tags //
    //    //            bool Intersection = (this._Type == JoinType.INNER || this._Type == JoinType.LEFT);
    //    //            bool Antisection = (this._Type == JoinType.LEFT || this._Type == JoinType.ANTI_LEFT);

    //    //            // Open a read stream //
    //    //            RecordReader lstream = lidx.OpenReader();
    //    //            RecordReader rstream = ridx.OpenReader();

    //    //            // Main loop through both left and right
    //    //            while (lstream.CanAdvance && rstream.CanAdvance)
    //    //            {

    //    //                int Compare = this._Predicate.Compare(lstream.Read(), rstream.Read());

    //    //                // Left is less than right, control left
    //    //                if (Compare < 0)
    //    //                {
    //    //                    lstream.Advance();
    //    //                }
    //    //                // AWValue is less than left, control right, but also output an anti join record
    //    //                else if (Compare > 0)
    //    //                {

    //    //                    if (Antisection)
    //    //                    {
    //    //                        Variants[this._NameSpace].SetTensor(this._LAlias, new Matrix(lstream.Read()));
    //    //                        Variants[this._NameSpace].SetTensor(this._RAlias, new Matrix(rstream.Columns.NullRecord));
    //    //                        if (this._Where.Evaluate(Variants))
    //    //                        {
    //    //                            Writer.Insert(this._Fields.ToRecord(Variants));
    //    //                        }
    //    //                    }
    //    //                    rstream.Advance();

    //    //                }
    //    //                else if (Intersection) // Compare == 0
    //    //                {

    //    //                    // Save the loop-result //
    //    //                    int NestedLoopCount = 0;

    //    //                    // Loop through all possible tuples //
    //    //                    while (Compare == 0)
    //    //                    {

    //    //                        // Render the record and potentially output //
    //    //                        Variants[this._NameSpace].SetTensor(this._LAlias, new Matrix(lstream.Read()));
    //    //                        Variants[this._NameSpace].SetTensor(this._RAlias, new Matrix(rstream.Read()));

    //    //                        if (this._Where.Evaluate(Variants))
    //    //                        {
    //    //                            Writer.Insert(this._Fields.ToRecord(Variants));
    //    //                        }

    //    //                        // Advance the right table //
    //    //                        rstream.Advance();
    //    //                        NestedLoopCount++;

    //    //                        // Check if this advancing pushed us to the end of the table //
    //    //                        if (!rstream.CanAdvance)
    //    //                            break;

    //    //                        // Reset the compare token //
    //    //                        Compare = this._Predicate.Compare(lstream.Read(), rstream.Read());

    //    //                    }

    //    //                    // Revert the nested loops //
    //    //                    rstream.Revert(NestedLoopCount);

    //    //                    // Step the left stream //
    //    //                    lstream.Advance();

    //    //                }
    //    //                else
    //    //                {
    //    //                    lstream.Advance();
    //    //                }

    //    //            }

    //    //            // Do Anti-Join //
    //    //            if (Antisection)
    //    //            {

    //    //                // Assign the right table to null //
    //    //                Variants[this._NameSpace].SetTensor(this._RAlias, new Matrix(rstream.Columns.NullRecord));

    //    //                // Walk the rest of the left table //
    //    //                while (lstream.CanAdvance)
    //    //                {

    //    //                    Variants[this._NameSpace].SetTensor(this._LAlias, new Matrix(lstream.Read()));
    //    //                    if (this._Where.Evaluate(Variants))
    //    //                    {
    //    //                        Writer.Insert(this._Fields.ToRecord(Variants));
    //    //                    }

    //    //                }

    //    //            }

    //    //        }

    //    //    }

    //    //    /// <summary>
    //    //    /// Implents the quasi-nested loop algorithm, where the right table uses an index
    //    //    /// </summary>
    //    //    public sealed class TableExpressionJoinQuasiNestedLoop : txJoin
    //    //    {

    //    //        public override void Evaluate(SpoolSpace Variants, RecordWriter Writer)
    //    //        {

    //    //            // Check each table //
    //    //            this.CheckRender();

    //    //            // Render each table //
    //    //            Table Left = this._LTable.Select(Variants);
    //    //            Table Right = this._RTable.Select(Variants);

    //    //            // Create a resolver //
    //    //            this.InitializeResolver(Variants);

    //    //            // Get the right join index //
    //    //            Index ridx = Right.GetIndex(this._Predicate.RightKey);
    //    //            if (ridx == null)
    //    //                ridx = Index.BuildTemporaryIndex(Right, this._Predicate.RightKey);

    //    //            // Get the join tags //
    //    //            bool Intersection = (this._Type == JoinType.INNER || this._Type == JoinType.LEFT), Antisection = (this._Type == JoinType.LEFT || this._Type == JoinType.ANTI_LEFT);

    //    //            // Open a read stream //
    //    //            RecordReader lstream = Left.OpenReader();

    //    //            // Loop through //
    //    //            while (lstream.CanAdvance)
    //    //            {

    //    //                // Open the right stream //
    //    //                Record lrec = lstream.ReadNext();
    //    //                RecordReader rstream = ridx.OpenStrictReader(Record.SplitUpper(lrec, this._Predicate.RightKey));

    //    //                // Only Loop through if there's actually a match //
    //    //                if (rstream != null)
    //    //                {

    //    //                    // Loop through the right stream //
    //    //                    while (rstream.CanAdvance)
    //    //                    {

    //    //                        // Read the records for the predicate //
    //    //                        Record rrec = rstream.ReadNext();

    //    //                        // Check the predicate //
    //    //                        if (this._Predicate.Compare(lrec, rrec) == 0 && Intersection)
    //    //                        {

    //    //                            // Load the variant //
    //    //                            Variants[this._NameSpace].SetTensor(this._LAlias, new Matrix(lstream.Read()));
    //    //                            Variants[this._NameSpace].SetTensor(this._RAlias, new Matrix(rstream.Read()));

    //    //                            // Evaluate the where //
    //    //                            if (this._Where.Evaluate(Variants))
    //    //                            {
    //    //                                Record x = this._Fields.ToRecord(Variants);
    //    //                                Writer.Insert(x);
    //    //                            }

    //    //                        } // Inner Predicate check

    //    //                    } // AWValue While

    //    //                } // Inner

    //    //                else if (Antisection)
    //    //                {

    //    //                    // Load the variant //
    //    //                    Variants[this._NameSpace].SetTensor(this._LAlias, new Matrix(lstream.Read()));
    //    //                    Variants[this._NameSpace].SetTensor(this._RAlias, new Matrix(rstream.Columns.NullRecord));

    //    //                    // Evaluate the where //
    //    //                    if (this._Where.Evaluate(Variants))
    //    //                    {
    //    //                        Record x = this._Fields.ToRecord(Variants);
    //    //                        Writer.Insert(x);
    //    //                    }

    //    //                }// Anti

    //    //            } // Left main loop

    //    //            // Clean up //
    //    //            this.CleanUpResolver(Variants);

    //    //        }

    //    //    }

    //    //    /// <summary>
    //    //    /// Implements the nested loop join algorithm
    //    //    /// </summary>
    //    //    public sealed class TableExpressionJoinNestedLoop : txJoin
    //    //    {

    //    //        public override void Evaluate(SpoolSpace Variants, RecordWriter Writer)
    //    //        {

    //    //            // Check each table //
    //    //            this.CheckRender();

    //    //            // Render each table //
    //    //            Table Left = this._LTable.Select(Variants);
    //    //            Table Right = this._RTable.Select(Variants);

    //    //            // Create a resolver //
    //    //            this.InitializeResolver(Variants);

    //    //            // Get the join tags //
    //    //            bool Intersection = (this._Type == JoinType.INNER || this._Type == JoinType.LEFT), Antisection = (this._Type == JoinType.LEFT || this._Type == JoinType.ANTI_LEFT);

    //    //            // Open a read stream //
    //    //            RecordReader lstream = Left.OpenReader();

    //    //            // Loop through //
    //    //            while (lstream.CanAdvance)
    //    //            {

    //    //                // Open the right stream //
    //    //                RecordReader rstream = Right.OpenReader();
    //    //                Record lrec = lstream.ReadNext();

    //    //                // Create the matcher tag //
    //    //                bool MatchFound = false;

    //    //                // Loop through the right stream //
    //    //                while (rstream.CanAdvance)
    //    //                {

    //    //                    // Read the records for the predicate //
    //    //                    Record rrec = rstream.ReadNext();

    //    //                    // Check the predicate //
    //    //                    if (this._Predicate.Compare(lrec, rrec) == 0 && Intersection)
    //    //                    {

    //    //                        // Load the variant //
    //    //                        Variants[this._NameSpace].SetTensor(this._LAlias, new Matrix(lstream.Read()));
    //    //                        Variants[this._NameSpace].SetTensor(this._RAlias, new Matrix(rstream.Read()));

    //    //                        // Tag taht we found a match //
    //    //                        MatchFound = true;

    //    //                        // Evaluate the where //
    //    //                        if (this._Where.Evaluate(Variants))
    //    //                        {
    //    //                            Record x = this._Fields.ToRecord(Variants);
    //    //                            Writer.Insert(x);
    //    //                        }

    //    //                    }

    //    //                }

    //    //                // Handle the fail match //
    //    //                if (!MatchFound && Antisection)
    //    //                {

    //    //                    // Load the variant //
    //    //                    Variants[this._NameSpace].SetTensor(this._LAlias, new Matrix(lstream.Read()));
    //    //                    Variants[this._NameSpace].SetTensor(this._RAlias, new Matrix(rstream.Columns.NullRecord));

    //    //                    // Tag taht we found a match //
    //    //                    MatchFound = true;

    //    //                    // Evaluate the where //
    //    //                    if (this._Where.Evaluate(Variants))
    //    //                    {
    //    //                        Record x = this._Fields.ToRecord(Variants);
    //    //                        Writer.Insert(x);
    //    //                    }

    //    //                }


    //    //            }

    //    //        }

    //    //    }


    //    //}

    //    //public abstract class txGroupBy : TableExpression
    //    //{
    //    //}

    //}

}
