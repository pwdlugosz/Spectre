using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre;
using Spectre.Cells;
using Spectre.Expressions;
using Spectre.Statements;
using Spectre.Control;

namespace Spectre.Libraries
{

    /*
     * VECTOR(N)
     * MATRIX(N,M)
     * MMULT
     * TRANSPOSE
     * INVERSE
     * DOT
     * PUSH
     * POP
     * ENQUEUE
     * DEQUEUE
     * FIRST
     * LAST
     *  
     */

    public sealed class ArrayLibrary : Library
    {

        public ArrayLibrary(Host Host)
            : base(Host, "ARRAY")
        {

        }

        public override ExpressionFunction ExpressionLookup(string Name)
        {

            switch(Name.ToUpper())
            {
                case "VECTOR":
                    return new FuncVector(this._Host, null);
                case "MATRIX":
                    return new FuncMatrix(this._Host, null);
                case "MMULT":
                    return new FuncMMult(this._Host, null);
                case "MDETERMINANT":
                case "MDETERM":
                case "MDET":
                    return new FuncMDeterminant(this._Host, null);
                case "DOT":
                case "DOTPROD":
                case "DOTPRODUCT":
                    return new FuncDot(this._Host, null);
                case "TRANSPOSE":
                    return new FuncTranspose(this._Host, null);
                case "INVERSE":
                case "MINVERSE":
                    return new FuncInverse(this._Host, null);
                case "FIRST":
                    return new FuncFirst(this._Host, null);
                case "LAST":
                    return new FuncLast(this._Host, null);
                case "POP":
                    return new FuncPop(this._Host, null);
                case "DEQUEUE":
                    return new FuncDequeue(this._Host, null);

            }

            return null;

        }

        public override Statement StatementLookup(string Name)
        {

            switch (Name.ToUpper())
            {

                case "PUSH":
                    return new VoidPush(this._Host, null);
                case "ENQUEUE":
                    return new VoidEnqueue(this._Host, null);

            }

            throw new Exception(string.Format("Method '{0}' does not exist", Name));

        }

        public sealed class FuncVector : ExpressionFunction
        {

            public FuncVector(Host Host, Expression Parent)
                : base(Host, Parent, "VECTOR", 1, 2)
            {
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.ARRAY;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();
                Cell Length = this._Children[0].Evaluate(Memory);
                Cell Default = (this._Children.Count >= 2 ? this._Children[1].Evaluate(Memory) : CellValues.NullBOOL);

                CellArray x = CellArray.Vector(Length.valueINT, Default);

                return new Cell(x);

            }

        }

        public sealed class FuncMatrix : ExpressionFunction
        {

            public FuncMatrix(Host Host, Expression Parent)
                : base(Host, Parent, "MATRIX", 2, 3)
            {
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.ARRAY;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();
                Cell RowCount = this._Children[0].Evaluate(Memory);
                Cell ColumnCount = this._Children[1].Evaluate(Memory);
                Cell Default = (this._Children.Count >= 3 ? this._Children[2].Evaluate(Memory) : CellValues.NullBOOL);

                CellArray x = CellArray.Matrix(RowCount.valueINT, ColumnCount.valueINT, Default);
                return new Cell(x);

            }

        }

        public sealed class FuncMMult : ExpressionFunction
        {

            public FuncMMult(Host Host, Expression Parent)
                : base(Host, Parent, "MMULT", 2, 2)
            {
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.ARRAY;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();

                CellArray x = this._Children[0].Evaluate(Memory);
                CellArray y = this._Children[1].Evaluate(Memory);
                CellArray z = CellArray.Multiply(x, y);

                return new Cell(z);

            }
            
        }

        public sealed class FuncTranspose : ExpressionFunction
        {

            public FuncTranspose(Host Host, Expression Parent)
                : base(Host, Parent, "TRANSPOSE", 1, 1)
            {

            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.ARRAY;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                if (!x.IsArray)
                    throw new Exception("Transpose requires a cell array");
                CellArray y = x.valueARRAY;
                if (!CellArray.IsMatrix(y))
                    throw new Exception("Transpose requires a matrix");
                return new Cell(CellArray.Transpose(y));

            }

        }

        public sealed class FuncInverse : ExpressionFunction
        {

            public FuncInverse(Host Host, Expression Parent)
                : base(Host, Parent, "INVERSE", 1, 1)
            {
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.ARRAY;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                if (!x.IsArray)
                    throw new Exception("Inverse requires a cell array");
                CellArray y = x.valueARRAY;
                if (!CellArray.IsMatrix(y))
                    throw new Exception("Inverse requires a matrix");
                if (y.Count != CellArray.ColumnCount(y))
                    throw new Exception("Inverse requires a square matrix");
                return new Cell(CellArray.Inverse(y));

            }

        }

        public sealed class FuncFirst : ExpressionFunction
        {

            public FuncFirst(Host Host, Expression Parent)
                : base(Host, Parent, "FIRST", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                if (!x.IsArray)
                    throw new Exception("First requires a cell array");
                if (x.ARRAY.Count == 0)
                    return CellValues.NullINT;
                return x.ARRAY.First();

            }

        }

        public sealed class FuncLast : ExpressionFunction
        {

            public FuncLast(Host Host, Expression Parent)
                : base(Host, Parent, "LAST", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                if (!x.IsArray)
                    throw new Exception("Last requires a cell array");
                if (x.ARRAY.Count == 0)
                    return CellValues.NullINT;
                return x.ARRAY.Last();

            }

        }

        public sealed class FuncPop : ExpressionFunction
        {

            public FuncPop(Host Host, Expression Parent)
                : base(Host, Parent, "POP", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                if (!x.IsArray)
                    throw new Exception("Pop requires a cell array");
                if (x.ARRAY.Count == 0)
                    return CellValues.NullINT;
                return x.ARRAY.Pop();

            }

        }
        
        public sealed class FuncDequeue : ExpressionFunction
        {

            public FuncDequeue(Host Host, Expression Parent)
                : base(Host, Parent, "DEQUEUE", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                if (!x.IsArray)
                    throw new Exception("Pop requires a cell array");
                if (x.ARRAY.Count == 0)
                    return CellValues.NullINT;
                return x.ARRAY.Dequeue();

            }

        }

        public sealed class FuncMDeterminant : ExpressionFunction
        {

            public FuncMDeterminant(Host Host, Expression Parent)
                : base(Host, Parent, "MDETERMINANT", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                if (!x.IsArray)
                    throw new Exception("Determinant requires a cell array");
                CellArray y = x.valueARRAY;
                if (!CellArray.IsMatrix(y))
                    throw new Exception("Determinant requires a matrix");
                return CellArray.Determinant(y);

            }

        }

        public sealed class FuncDot : ExpressionFunction
        {

            public FuncDot(Host Host, Expression Parent)
                : base(Host, Parent, "DOT", 2, -1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();
                List<CellArray> values = new List<CellArray>();
                foreach(Expression y in this._Children)
                {
                    Cell q = y.Evaluate(Memory);
                    if (q.Affinity != CellAffinity.ARRAY)
                        throw new Exception(string.Format("Only array arguments can be passed to the Dot function; affinity passed '{0}'", q.Affinity));
                    values.Add(q.valueARRAY);
                }
                return CellArray.DotProduct(values);

            }

        }

        public sealed class VoidPush : Statement
        {

            public VoidPush(Host Host, Statement Parent)
                : base(Host, Parent)
            {

            }

            public override void Invoke(SpoolSpace Memory)
            {

                this.CheckParameters(2, 2);

                Cell x = this._Parameters[0].Evaluate(Memory);
                Cell y = this._Parameters[1].Evaluate(Memory);

                if (!x.IsArray)
                    return;

                x.valueARRAY.Push(y);
            }

        }

        public sealed class VoidEnqueue : Statement
        {

            public VoidEnqueue(Host Host, Statement Parent)
                : base(Host, Parent)
            {

            }

            public override void Invoke(SpoolSpace Memory)
            {

                this.CheckParameters(2, 2);

                Cell x = this._Parameters[0].Evaluate(Memory);
                Cell y = this._Parameters[1].Evaluate(Memory);

                if (!x.IsArray)
                    return;

                x.valueARRAY.Enqueue(y);
            }

        }


    }

}
