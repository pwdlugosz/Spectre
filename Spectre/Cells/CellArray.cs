using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectre.Cells
{
    
    /// <summary>
    /// An array of cells
    /// </summary>
    public sealed class CellArray : IEnumerable<Cell>, IEnumerable
    {

        private enum CellArrayType : byte
        {
            Normal = 0,
            Minimum = 1,
            Maximum = 2,
            Distinct = 3,
            Sorted = 4
        }

        private List<Cell> _Cells;
        private CellArrayType _Type = CellArrayType.Normal;

        public CellArray(List<Cell> Cells)
        {
            this._Cells = Cells;
        }

        public CellArray()
            :this(new List<Cell>())
        {
        }

        public CellArray(CellArray Value)
            : this()
        {
            foreach (Cell x in Value)
            {
                if (x.AFFINITY != CellAffinity.ARRAY)
                    this._Cells.Add(x);
                else
                    this._Cells.Add(new Cell(new CellArray(x.ARRAY)));
            }
        }

        public CellArray(Cell[] Cells)
            : this(new List<Cell>(Cells))
        {
        }

        public CellArray(Tables.Record Value)
            : this(Value._data)
        {
        }

        public int Count
        {
            get { return this._Cells.Count; }
        }

        public Cell this[int Index]
        {
            get 
            { 
                return this._Cells[Index]; 
            }
            set 
            {
                if (!this.IsNormal) return;
                this._Cells[Index] = value; 
            }
        }

        public CellArray this[int Index, int Length]
        {
            get
            {
                if (Index + Length >= this.Count)
                    throw new IndexOutOfRangeException();
                if (Length == 1)
                    return this._Cells[Index];
                CellArray x = new CellArray();
                for (int i = Index; i < Index + Length; i++)
                {
                    x.Append(this[i]);
                }
                return x;
            }
        }

        public bool IsMin
        {
            get { return this._Type == CellArrayType.Minimum; }
        }

        public bool IsMax
        {
            get { return this._Type == CellArrayType.Maximum; }
        }

        public bool IsNormal
        {
            get { return this._Type == CellArrayType.Normal; }
        }

        public void Append(Cell Value)
        {
            switch(this._Type)
            {
                case CellArrayType.Minimum:
                case CellArrayType.Maximum:
                    break;
                case CellArrayType.Sorted:
                    int idx = this._Cells.BinarySearch(Value);
                    idx = (idx < 0 ? ~idx : idx);
                    this._Cells.Insert(idx, Value);
                    break;
                case CellArrayType.Distinct:
                    if (!this._Cells.Contains(Value))
                        this._Cells.Add(Value);
                    break;
                case CellArrayType.Normal:
                    this._Cells.Add(Value);
                    break;
            }
        }

        public void Enqueue(Cell Value)
        {
            if (this._Type != CellArrayType.Normal)
                return;
            this._Cells.Add(Value);
        }

        public void Push(Cell Value)
        {
            if (this._Cells.Count == 0)
                this._Cells.Add(Value);
            else
                this._Cells.Insert(0, Value);
        }

        public Cell Dequeue()
        {
            if (this._Cells.Count == 0)
                throw new Exception("Array is empty");
            Cell x = this._Cells[0];
            this._Cells.RemoveAt(0);
            return x;
        }

        public Cell Pop()
        {
            if (this._Cells.Count == 0)
                throw new Exception("Array is empty");
            Cell x = this._Cells[this._Cells.Count - 1];
            this._Cells.RemoveAt(this._Cells.Count - 1);
            return x;
        }

        public Cell[] ToArray()
        {
            return this._Cells.ToArray();
        }
        
        public void Sort()
        {
            this._Cells.Sort(CellComparer.Compare);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._Cells.GetEnumerator();
        }

        IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator()
        {
            return this._Cells.GetEnumerator();
        }

        // Vectors //
        public static CellArray Vector(int Length, Cell Value)
        {
            CellArray ca = new CellArray();
            for (int i = 0; i < Length; i++)
                ca.Append(Value);
            return ca;
        }

        public static CellArray Vector(int Length, CellAffinity Affinity)
        {
            return CellArray.Vector(Length, new Cell(Affinity));
        }

        public static CellArray Vector(int Length)
        {
            return CellArray.Vector(Length, CellAffinity.DOUBLE);
        }

        // Matrixes //
        public static CellArray Matrix(int RowLength, int ColumnLength, Cell Value)
        {

            CellArray v = new CellArray();
            for (int i = 0; i < RowLength; i++)
            {
                v.Append(new Cell(CellArray.Vector(ColumnLength, Value)));
            }
            return v;

        }

        public static CellArray Matrix(int RowLength, int ColumnLength, CellAffinity Affinity)
        {
            return CellArray.Matrix(RowLength, ColumnLength, new Cell(Affinity));
        }

        public static CellArray Matrix(int RowLength, int ColumnLength)
        {
            return CellArray.Matrix(RowLength, ColumnLength, CellAffinity.DOUBLE);
        }

        public static CellArray Identity(int Dimension, CellAffinity Type)
        {
            if (!CellAffinityHelper.IsNumeric(Type))
                return new CellArray();

            Cell zero = CellValues.Zero(Type);
            Cell one = CellValues.One(Type);

            CellArray x = CellArray.Matrix(Dimension, Dimension);

            for (int i = 0; i < Dimension; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    x[i].ARRAY[j] = (i == j ? one : zero);
                }
            }

            return x;

        }

        public static CellArray Identity(int Dimension)
        {
            return CellArray.Identity(Dimension, CellAffinity.DOUBLE);
        }
        
        public static int[] ColumnLengths(CellArray A)
        {
            int[] indexes = new int[A.Count];
            int i = 0;
            foreach(Cell c in A)
            {
                indexes[i++] = (c.AFFINITY == CellAffinity.ARRAY ? c.ARRAY.Count : 1);
            }
            return indexes;
        }

        public static bool IsMatrix(CellArray A, out int ColumnCount)
        {
            int[] idx = ColumnLengths(A);
            int Min = int.MaxValue, Max = int.MinValue;
            foreach (int i in idx)
            {
                Min = Math.Min(Min, i);
                Max = Math.Max(Max, i);
            }
            ColumnCount = Max;
            return Min == Max;
        }

        public static bool IsMatrix(CellArray A)
        {
            int i = 0;
            return IsMatrix(A, out i);
        }

        public static int ColumnCount(CellArray A)
        {
            int ColumnCount = 0;
            bool b = IsMatrix(A, out ColumnCount);
            return ColumnCount;
        }

        public static CellArray Transpose(CellArray A)
        {
            int Rows = A.Count;
            int Columns = 0;

            if (!CellArray.IsMatrix(A, out Columns))
                throw new Exception("Cannot transpose a non-matrix");

            CellArray B = CellArray.Matrix(Columns, Rows);
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    B[j].ARRAY[i] = A[i].ARRAY[j];
                }
            }
            return B;

        }

        public static CellArray Multiply(CellArray A, CellArray B)
        {

            if (!CellArray.IsMatrix(A) || !CellArray.IsMatrix(B))
                throw new Exception("Both A and B must be matrixes");

            int A_Rows = A.Count, B_Rows = B.Count;
            int A_Cols = CellArray.ColumnCount(A), B_Cols = CellArray.ColumnCount(B);
            
            if (A_Cols != B_Rows)
            {
                throw new Exception(string.Format("Dimension mismatch A {0}:{1} B {2}:{3}", A_Rows, A_Cols, B_Rows, B_Cols));
            }
            
            CellArray C = CellArray.Matrix(A_Rows, B_Cols);

            // Main Loop //
            for (int i = 0; i < A_Rows; i++)
            {

                // Sub Loop One //
                for (int j = 0; j < B_Cols; j++)
                {

                    // Sub Loop Two //
                    for (int k = 0; k < A_Cols; k++)
                    {

                        C[i].valueARRAY[j] = (C[i].valueARRAY[j].IsNull ? A[i].valueARRAY[k] * B[k].valueARRAY[j] : A[i].valueARRAY[k] * B[k].valueARRAY[j] + C[i].valueARRAY[j]);

                    }

                }

            }

            // Return C //
            return C;

        }

        public static CellArray Inverse(CellArray A)
        {

            if (!CellArray.IsMatrix(A))
                throw new Exception("A must be a matrix");

            int A_Rows = A.Count;
            int A_Cols = CellArray.ColumnCount(A);

            if (A_Rows != A_Cols)
            {
                throw new Exception(string.Format("Matrix must be square {0}:{1}", A_Rows, A_Cols));
            }

            LUDecomposition engine = new LUDecomposition(A);

            CellArray B = CellArray.Identity(A_Rows);

            return engine.solve(B);

        }

        public static Cell Determinant(CellArray A)
        {
            if (!CellArray.IsMatrix(A))
                throw new Exception("A must be a matrix");

            int A_Rows = A.Count;
            int A_Cols = CellArray.ColumnCount(A);

            if (A_Rows != A_Cols)
            {
                throw new Exception(string.Format("Matrix must be square {0}:{1}", A_Rows, A_Cols));
            }

            LUDecomposition engine = new LUDecomposition(A);

            return engine.det();

        }

        public static Cell DotProduct(List<CellArray> Values)
        {
            int max = Values.Max((x) => { return x.Count; });
            int min = Values.Min((x) => { return x.Count; });
            if (max != min) throw new Exception(string.Format("Cell arrays must all have the same length; min length {0}; max length {1}", min, max));

            Cell dp = CellValues.ZeroBYTE;
            for (int i = 0; i < max; i++)
            {
                Cell ip = CellValues.OneBYTE;
                for (int j = 0; j < Values.Count; j++)
                {
                    ip *= Values[j][i];
                }
                dp += ip;
            }

            return dp;

        }

        public static Cell DotProduct(CellArray A, CellArray B)
        {

            if (A.Count != B.Count)
                throw new Exception("Cell arrays must have the same length");

            Cell x = CellValues.ZeroBYTE;
            for (int i = 0; i < A.Count; i++)
            {
                x += A[i] * B[i];
            }

            return x;

        }

        // Values //
        public static CellArray MinimumArray
        {
            get
            {
                CellArray x = new CellArray();
                x._Type = CellArrayType.Minimum;
                return x;
            }
        }

        public static CellArray MximumArray
        {
            get
            {
                CellArray x = new CellArray();
                x._Type = CellArrayType.Maximum;
                return x;
            }
        }

        // Statics //
        public static CellArray Clone(CellArray A)
        {

            CellArray x = new CellArray();
            foreach (Cell c in A)
            {
                if (c.IsArray)
                    x.Append(CellArray.Clone(c));
                else
                    x.Append(c);
            }
            return x;

        }

        /// <summary>
        /// This class was 'borrowed' from NIST's matrix numerics library, which was re-coded from Java
        /// </summary>
        private class LUDecomposition
        {

            private CellArray LU;
            private int m, n, pivsign;
            private int[] piv;
            private Cell _zero = CellValues.ZeroDOUBLE;
            private Cell _one = CellValues.OneDOUBLE;

            public LUDecomposition(CellArray A)
            {

                // Use a "left-looking", dot-product, Crout/Doolittle algorithm.

                LU = CellArray.Clone(A);
                m = A.Count;
                n = ColumnCount(A);
                piv = new int[m];
                for (int i = 0; i < m; i++)
                {
                    piv[i] = i;
                }
                pivsign = 1;
                Cell[] LUcolj = new Cell[m];

                // Outer loop.

                for (int j = 0; j < n; j++)
                {

                    // Make a copy of the j-th column to localize references.

                    for (int i = 0; i < m; i++)
                    {
                        LUcolj[i] = LU[i].ARRAY[j];
                    }

                    // Apply previous transformations.

                    for (int i = 0; i < m; i++)
                    {

                        // Most of the time is spent in the following dot product.

                        int kmax = Math.Min(i, j);
                        Cell s = this._zero;
                        for (int k = 0; k < kmax; k++)
                        {
                            s += LU[i].ARRAY[k] * LUcolj[k];
                        }

                        LU[i].ARRAY[j] = LUcolj[i] -= s;

                    }

                    // Find pivot and exchange if necessary.

                    int p = j;
                    for (int i = j + 1; i < m; i++)
                    {
                        if (CellFunctions.Abs(LUcolj[i]) > CellFunctions.Abs(LUcolj[p]))
                        {
                            p = i;
                        }
                    }
                    if (p != j)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            Cell t = LU[p, k];
                            LU[p].ARRAY[k] = LU[j].ARRAY[k];
                            LU[j].ARRAY[k] = t;
                        }
                        int l = piv[p];
                        piv[p] = piv[j];
                        piv[j] = l;
                        pivsign = -pivsign;
                    }

                    // Compute multipliers.

                    if (j < m && LU[j].ARRAY[j] != this._zero)
                    {
                        for (int i = j + 1; i < m; i++)
                        {
                            LU[i].ARRAY[j] /= LU[j].ARRAY[j];
                        }
                    }
                }
            }

            public bool isNonsingular()
            {
                for (int j = 0; j < n; j++)
                {
                    if (LU[j].ARRAY[j] == this._zero)
                        return false;
                }
                return true;
            }

            public CellArray getL()
            {

                CellArray L = CellArray.Matrix(m, n);
                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i > j)
                        {
                            L[i].ARRAY[j] = LU[i].ARRAY[j];
                        }
                        else if (i == j)
                        {
                            L[i].ARRAY[j] = CellValues.One(CellAffinity.DOUBLE);
                        }
                        else
                        {
                            L[i].ARRAY[j] = CellValues.Zero(CellAffinity.DOUBLE);
                        }
                    }
                }
                return L;

            }

            public CellArray getU()
            {

                CellArray X = CellArray.Matrix(n, n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (i <= j)
                        {
                            X[i].ARRAY[j] = LU[i].ARRAY[j];
                        }
                        else
                        {
                            X[i].ARRAY[j] = CellValues.Zero(CellAffinity.DOUBLE);
                        }
                    }
                }
                return X;

            }

            public int[] getPivot()
            {

                int[] p = new int[m];
                for (int i = 0; i < m; i++)
                {
                    p[i] = piv[i];
                }
                return p;

            }

            public Cell det()
            {

                if (m != n)
                {
                    throw new Exception("Matrix must be square.");
                }
                Cell d = CellConverter.Cast(new Cell(pivsign), CellAffinity.DOUBLE);
                for (int j = 0; j < n; j++)
                {
                    d *= LU[j, j];
                }
                return d;

            }

            public CellArray solve(CellArray B)
            {

                if (B.Count != m)
                {
                    throw new Exception("Matrix row dimensions must agree.");
                }
                if (!this.isNonsingular())
                {
                    throw new Exception("Matrix is singular.");
                }

                // Copy right hand side with pivoting
                int nx = CellArray.ColumnCount(B);
                CellArray X = this.getMatrix(B, piv, 0, nx - 1);

                // Solve L*Y = B(piv,:)
                for (int k = 0; k < n; k++)
                {

                    for (int i = k + 1; i < n; i++)
                    {

                        for (int j = 0; j < nx; j++)
                        {
                            X[i].ARRAY[j] -= X[k].ARRAY[j] * LU[i].ARRAY[k];
                        }

                    }

                }
                // Solve U*X = Y;
                for (int k = n - 1; k >= 0; k--)
                {

                    for (int j = 0; j < nx; j++)
                    {
                        X[k].ARRAY[j] /= LU[k].ARRAY[k];
                    }

                    for (int i = 0; i < k; i++)
                    {

                        for (int j = 0; j < nx; j++)
                        {
                            X[i].ARRAY[j] -= X[k].ARRAY[j] * LU[i].ARRAY[k];
                        }

                    }

                }

                return X;

            }

            public CellArray getMatrix(CellArray A, int[] r, int j0, int j1)
            {

                CellArray X = CellArray.Matrix(r.Length, j1 - j0 + 1);

                for (int i = 0; i < r.Length; i++)
                {
                    for (int j = j0; j <= j1; j++)
                    {
                        X[i].ARRAY[j - j0] = A[r[i]].ARRAY[j];
                    }
                }

                return X;

            }

        }

        //private struct MatrixEnumerator : IEnumerator<Cell>, IEnumerator, IDisposable
        //{

        //    private CellArray _M;
        //    private int _i;
        //    private int _j;
        //    private Cell _v;

        //    public MatrixEnumerator(CellArray M)
        //    {
        //        this._M = M;
        //        this._i = 0;
        //        this._j = 0;
        //        this._v = CellValues.NullINT;
        //    }

        //    Cell IEnumerator<Cell>.Current
        //    {
        //        get { return this._M[this._i, this._j]; }
        //    }

        //    object IEnumerator.Current
        //    {
        //        get { return this._M[this._i, this._j]; }
        //    }

        //    public bool MoveNext()
        //    {
        //        if (this._i < this._M.RowCount)
        //            this._i++;
        //        if (this._j < this._M.ColumnCount)
        //            this._j++;
        //        if (this._i < this._M.RowCount && this._j < this._M.ColumnCount)
        //        {
        //            this._v = this._M[this._i, this._j];
        //            return true;
        //        }
        //        else
        //        {
        //            this._v = CellValues.NullINT;
        //            return false;
        //        }
        //    }

        //    public void Reset()
        //    {
        //        this._i = 0;
        //        this._j = 0;
        //    }

        //    public void Dispose()
        //    {

        //    }

        //}

    }

}
