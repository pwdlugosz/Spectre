using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Structures;
using Spectre.Expressions;

namespace Spectre.Cells
{
    
    public static class CellOperations
    {

        public static Cell Not(Cell C)
        {

            if (C.NULL == 1) return C;

            switch (C.AFFINITY)
            {

                case CellAffinity.BOOL:
                    C.BOOL = !C.BOOL;
                    return C;
                case CellAffinity.DATE_TIME:
                case CellAffinity.BYTE:
                case CellAffinity.SHORT:
                case CellAffinity.INT:
                case CellAffinity.LONG:
                case CellAffinity.SINGLE:
                case CellAffinity.DOUBLE:
                    C.LONG = ~C.LONG;
                    return C;

                case CellAffinity.ARRAY:
                    CellArray a = new CellArray();
                    foreach (Cell b in C.ARRAY)
                        a.Append(CellOperations.Not(b));
                    return new Cell(a);
                case CellAffinity.CSTRING:
                    StringBuilder c = new StringBuilder();
                    foreach (char d in C.CSTRING)
                        c.Append(~d);
                    return new Cell(c.ToString());
                case CellAffinity.BSTRING:
                    BString.BStringBuilder e = new BString.BStringBuilder();
                    foreach (byte f in C.BSTRING.ToByteArray)
                        e.Append((byte)(~f));
                    return new Cell(e.ToBString());
                case CellAffinity.BINARY:
                    List<byte> g = new List<byte>();
                    foreach (byte h in C.BINARY)
                        g.Add((byte)(~h));
                    return new Cell(g.ToArray());
                case CellAffinity.EQUATION:
                    Expression x = C.EQUATION;
                    Expression y = new Expression.Unary.Not(x.Host, x.Parent, x);
                    return new Cell(y);

            }


            return C;

        }

        public static Cell Minus(Cell C)
        {

            if (C.NULL == 1) return C;

            switch (C.AFFINITY)
            {

                case CellAffinity.SHORT:
                    C.SHORT = (short)(-C.SHORT);
                    return C;
                case CellAffinity.INT:
                    C.INT = -C.INT;
                    return C;
                case CellAffinity.LONG:
                    C.LONG = -C.LONG;
                    return C;
                case CellAffinity.SINGLE:
                    C.SINGLE = -C.SINGLE;
                    return C;
                case CellAffinity.DOUBLE:
                    C.DOUBLE = -C.DOUBLE;
                    return C;

                case CellAffinity.ARRAY:
                    CellArray a = new CellArray();
                    foreach (Cell b in C.ARRAY)
                        a.Append(CellOperations.Minus(b));
                    return new Cell(a);
                case CellAffinity.EQUATION:
                    Expression x = C.EQUATION;
                    Expression y = new Expression.Unary.Minus(x.Host, x.Parent, x);
                    return new Cell(y);

            }

            return new Cell(C.AFFINITY);

        }

        public static Cell Add(Cell A, Cell B)
        {

            CellAffinity x = CellAffinityHelper.Highest(A.AFFINITY, B.AFFINITY);

            if (A.IsNull || B.IsNull) return new Cell(x);

            if (A.IsArray && B.IsArray && A.ARRAY.Count == B.ARRAY.Count)
            {
                CellArray y = new CellArray();
                for (int i = 0; i < A.ARRAY.Count; i++)
                    y.Append(CellOperations.Add(A.ARRAY[i], B.ARRAY[i]));
                return new Cell(y);
            }
            
            switch(x)
            {
                case CellAffinity.DOUBLE: return new Cell(A.valueDOUBLE + B.valueDOUBLE);
                case CellAffinity.SINGLE: return new Cell(A.valueSINGLE + B.valueSINGLE);
                case CellAffinity.LONG: return new Cell(A.valueLONG + B.valueLONG);
                case CellAffinity.INT: return new Cell(A.valueINT + B.valueINT);
                case CellAffinity.SHORT: return new Cell(A.valueSHORT + B.valueSHORT);
                case CellAffinity.BYTE: return new Cell(A.valueBYTE + B.valueBYTE);
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.Add(null, null, A.valueEQUATION, B.valueEQUATION));
            }
                
            return new Cell(x);

        }

        public static Cell Substract(Cell A, Cell B)
        {

            CellAffinity x = CellAffinityHelper.Highest(A.AFFINITY, B.AFFINITY);

            if (A.IsNull || B.IsNull) return new Cell(x);

            if (A.IsArray && B.IsArray && A.ARRAY.Count == B.ARRAY.Count)
            {
                CellArray y = new CellArray();
                for (int i = 0; i < A.ARRAY.Count; i++)
                    y.Append(CellOperations.Substract(A.ARRAY[i], B.ARRAY[i]));
                return new Cell(y);
            }

            switch (x)
            {
                case CellAffinity.DOUBLE: return new Cell(A.valueDOUBLE - B.valueDOUBLE);
                case CellAffinity.SINGLE: return new Cell(A.valueSINGLE - B.valueSINGLE);
                case CellAffinity.LONG: return new Cell(A.valueLONG - B.valueLONG);
                case CellAffinity.INT: return new Cell(A.valueINT - B.valueINT);
                case CellAffinity.SHORT: return new Cell(A.valueSHORT - B.valueSHORT);
                case CellAffinity.BYTE: return new Cell(A.valueBYTE - B.valueBYTE);
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.Subtract(null, null, A.valueEQUATION, B.valueEQUATION));
            }

            return new Cell(x);

        }

        public static Cell Multiply(Cell A, Cell B)
        {

            CellAffinity x = CellAffinityHelper.Highest(A.AFFINITY, B.AFFINITY);
            
            if (A.IsNull || B.IsNull) return new Cell(x);

            if (A.IsArray && B.IsArray && A.ARRAY.Count == B.ARRAY.Count)
            {
                CellArray y = new CellArray();
                for (int i = 0; i < A.ARRAY.Count; i++)
                    y.Append(CellOperations.Multiply(A.ARRAY[i], B.ARRAY[i]));
                return new Cell(y);
            }
            
            switch (x)
            {
                case CellAffinity.DOUBLE: return new Cell(A.valueDOUBLE * B.valueDOUBLE);
                case CellAffinity.SINGLE: return new Cell(A.valueSINGLE * B.valueSINGLE);
                case CellAffinity.LONG: return new Cell(A.valueLONG * B.valueLONG);
                case CellAffinity.INT: return new Cell(A.valueINT * B.valueINT);
                case CellAffinity.SHORT: return new Cell(A.valueSHORT * B.valueSHORT);
                case CellAffinity.BYTE: return new Cell(A.valueBYTE * B.valueBYTE);
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.Multiply(null, null, A.valueEQUATION, B.valueEQUATION));
            }

            return new Cell(x);

        }

        public static Cell Divide(Cell A, Cell B)
        {

            CellAffinity x = CellAffinityHelper.Highest(A.AFFINITY, B.AFFINITY);

            if (A.IsNull || B.IsNull) return new Cell(x);

            if (A.IsArray && B.IsArray && A.ARRAY.Count == B.ARRAY.Count)
            {
                CellArray y = new CellArray();
                for (int i = 0; i < A.ARRAY.Count; i++)
                    y.Append(CellOperations.Divide(A.ARRAY[i], B.ARRAY[i]));
                return new Cell(y);
            }

            switch (x)
            {
                case CellAffinity.DOUBLE: return B.valueDOUBLE == 0d ? CellValues.NullDOUBLE : new Cell(A.valueDOUBLE / B.valueDOUBLE);
                case CellAffinity.SINGLE: return B.valueSINGLE == 0f ? CellValues.NullSINGLE : new Cell(A.valueSINGLE / B.valueSINGLE);
                case CellAffinity.LONG: return B.valueLONG == 0L ? CellValues.NullLONG : new Cell(A.valueLONG / B.valueLONG);
                case CellAffinity.INT: return B.valueINT == 0 ? CellValues.NullINT : new Cell(A.valueINT / B.valueINT);
                case CellAffinity.SHORT: return B.valueSHORT == 0 ? CellValues.NullSHORT : new Cell(A.valueSHORT / B.valueSHORT);
                case CellAffinity.BYTE: return B.valueBYTE == 0 ? CellValues.NullBYTE : new Cell(A.valueBYTE / B.valueBYTE);
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.Divide(null, null, A.valueEQUATION, B.valueEQUATION));
            }

            return new Cell(x);

        }

        public static Cell CheckDivide(Cell A, Cell B)
        {

            CellAffinity x = CellAffinityHelper.Highest(A.AFFINITY, B.AFFINITY);

            if (A.IsNull || B.IsNull) return new Cell(x);

            if (A.IsArray && B.IsArray && A.ARRAY.Count == B.ARRAY.Count)
            {
                CellArray y = new CellArray();
                for (int i = 0; i < A.ARRAY.Count; i++)
                    y.Append(CellOperations.CheckDivide(A.ARRAY[i], B.ARRAY[i]));
                return new Cell(y);
            }

            switch (x)
            {
                case CellAffinity.DOUBLE: return B.valueDOUBLE == 0d ? CellValues.ZeroDOUBLE : new Cell(A.valueDOUBLE / B.valueDOUBLE);
                case CellAffinity.SINGLE: return B.valueSINGLE == 0f ? CellValues.ZeroSINGLE : new Cell(A.valueSINGLE / B.valueSINGLE);
                case CellAffinity.LONG: return B.valueLONG == 0L ? CellValues.ZeroLONG : new Cell(A.valueLONG / B.valueLONG);
                case CellAffinity.INT: return B.valueINT == 0 ? CellValues.ZeroINT : new Cell(A.valueINT / B.valueINT);
                case CellAffinity.SHORT: return B.valueSHORT == 0 ? CellValues.ZeroSHORT : new Cell(A.valueSHORT / B.valueSHORT);
                case CellAffinity.BYTE: return B.valueBYTE == 0 ? CellValues.ZeroBYTE : new Cell(A.valueBYTE / B.valueBYTE);
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.CheckDivide(null, null, A.valueEQUATION, B.valueEQUATION));
            }

            return new Cell(x);

        }

        public static Cell Mod(Cell A, Cell B)
        {

            CellAffinity x = CellAffinityHelper.Highest(A.AFFINITY, B.AFFINITY);

            if (A.IsNull || B.IsNull) return new Cell(x);

            if (A.IsArray && B.IsArray && A.ARRAY.Count == B.ARRAY.Count)
            {
                CellArray y = new CellArray();
                for (int i = 0; i < A.ARRAY.Count; i++)
                    y.Append(CellOperations.Mod(A.ARRAY[i], B.ARRAY[i]));
                return new Cell(y);
            }

            switch (x)
            {
                case CellAffinity.DOUBLE: return B.valueDOUBLE == 0d ? CellValues.NullDOUBLE : new Cell(A.valueDOUBLE % B.valueDOUBLE);
                case CellAffinity.SINGLE: return B.valueSINGLE == 0f ? CellValues.NullSINGLE : new Cell(A.valueSINGLE % B.valueSINGLE);
                case CellAffinity.LONG: return B.valueLONG == 0L ? CellValues.NullLONG : new Cell(A.valueLONG % B.valueLONG);
                case CellAffinity.INT: return B.valueINT == 0 ? CellValues.NullINT : new Cell(A.valueINT % B.valueINT);
                case CellAffinity.SHORT: return B.valueSHORT == 0 ? CellValues.NullSHORT : new Cell(A.valueSHORT % B.valueSHORT);
                case CellAffinity.BYTE: return B.valueBYTE == 0 ? CellValues.NullBYTE : new Cell(A.valueBYTE % B.valueBYTE);
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.Mod(null, null, A.valueEQUATION, B.valueEQUATION));
            }

            return new Cell(x);

        }

        public static Cell CheckMod(Cell A, Cell B)
        {

            CellAffinity x = CellAffinityHelper.Highest(A.AFFINITY, B.AFFINITY);

            if (A.IsNull || B.IsNull) return new Cell(x);

            if (A.IsArray && B.IsArray && A.ARRAY.Count == B.ARRAY.Count)
            {
                CellArray y = new CellArray();
                for (int i = 0; i < A.ARRAY.Count; i++)
                    y.Append(CellOperations.CheckDivide(A.ARRAY[i], B.ARRAY[i]));
                return new Cell(y);
            }

            switch (x)
            {
                case CellAffinity.DOUBLE: return B.valueDOUBLE == 0d ? CellValues.ZeroDOUBLE : new Cell(A.valueDOUBLE % B.valueDOUBLE);
                case CellAffinity.SINGLE: return B.valueSINGLE == 0f ? CellValues.ZeroSINGLE : new Cell(A.valueSINGLE % B.valueSINGLE);
                case CellAffinity.LONG: return B.valueLONG == 0L ? CellValues.ZeroLONG : new Cell(A.valueLONG % B.valueLONG);
                case CellAffinity.INT: return B.valueINT == 0 ? CellValues.ZeroINT : new Cell(A.valueINT % B.valueINT);
                case CellAffinity.SHORT: return B.valueSHORT == 0 ? CellValues.ZeroSHORT : new Cell(A.valueSHORT % B.valueSHORT);
                case CellAffinity.BYTE: return B.valueBYTE == 0 ? CellValues.ZeroBYTE : new Cell(A.valueBYTE % B.valueBYTE);
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.CheckMod(null, null, A.valueEQUATION, B.valueEQUATION));
            }

            return new Cell(x);

        }

        public static Cell And(Cell A, Cell B)
        {

            CellAffinity x = CellAffinityHelper.Highest(A.AFFINITY, B.AFFINITY);

            if (A.IsNull || B.IsNull)
                return new Cell(x);

            switch (x)
            {
                case CellAffinity.BOOL: return new Cell(A.valueBOOL && B.valueBOOL);
                case CellAffinity.DATE_TIME:
                case CellAffinity.BYTE:
                case CellAffinity.SHORT:
                case CellAffinity.INT:
                case CellAffinity.LONG:
                case CellAffinity.SINGLE:
                case CellAffinity.DOUBLE:
                    A.LONG = A.LONG & B.LONG;
                    return A;
                case CellAffinity.BINARY:
                    return new Cell(BitHelper.BufferAnd(A.valueBINARY, B.valueBINARY));
                case CellAffinity.BSTRING:
                    byte[] C = BitHelper.BufferAnd(A.valueBSTRING._elements, B.valueBSTRING._elements);
                    return new Cell(new BString(C));
                case CellAffinity.CSTRING:
                    byte[] v = BitHelper.BufferAnd(A.valueBINARY, B.valueBINARY);
                    return new Cell(ASCIIEncoding.Unicode.GetString(v));
                case CellAffinity.TREF:
                    return CellValues.NullTREF;
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.And(null, null, A.valueEQUATION, B.valueEQUATION));
            }

            throw new Exception();

        }

        public static Cell Or(Cell A, Cell B)
        {

            CellAffinity x = CellAffinityHelper.Highest(A.AFFINITY, B.AFFINITY);

            if (A.IsNull || B.IsNull)
                return new Cell(x);

            switch (x)
            {
                case CellAffinity.BOOL: return new Cell(A.valueBOOL || B.valueBOOL);
                case CellAffinity.DATE_TIME:
                case CellAffinity.BYTE:
                case CellAffinity.SHORT:
                case CellAffinity.INT:
                case CellAffinity.LONG:
                case CellAffinity.SINGLE:
                case CellAffinity.DOUBLE:
                    A.LONG = A.LONG | B.LONG;
                    return A;
                case CellAffinity.BINARY:
                    return new Cell(BitHelper.BufferOr(A.valueBINARY, B.valueBINARY));
                case CellAffinity.BSTRING:
                    byte[] C = BitHelper.BufferOr(A.valueBSTRING._elements, B.valueBSTRING._elements);
                    return new Cell(new BString(C));
                case CellAffinity.CSTRING:
                    byte[] v = BitHelper.BufferOr(A.valueBINARY, B.valueBINARY);
                    return new Cell(ASCIIEncoding.Unicode.GetString(v));
                case CellAffinity.TREF:
                    return CellValues.NullTREF;
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.Or(null, null, A.valueEQUATION, B.valueEQUATION));
            }

            throw new Exception();

        }

        public static Cell Xor(Cell A, Cell B)
        {

            CellAffinity x = CellAffinityHelper.Highest(A.AFFINITY, B.AFFINITY);

            if (A.IsNull || B.IsNull)
                return new Cell(x);

            switch (x)
            {
                case CellAffinity.BOOL: return new Cell(A.valueBOOL ^ B.valueBOOL);
                case CellAffinity.DATE_TIME:
                case CellAffinity.BYTE:
                case CellAffinity.SHORT:
                case CellAffinity.INT:
                case CellAffinity.LONG:
                case CellAffinity.SINGLE:
                case CellAffinity.DOUBLE:
                    A.LONG = A.LONG ^ B.LONG;
                    return A;
                case CellAffinity.BINARY:
                    return new Cell(BitHelper.BufferXor(A.valueBINARY, B.valueBINARY));
                case CellAffinity.BSTRING:
                    byte[] C = BitHelper.BufferXor(A.valueBSTRING._elements, B.valueBSTRING._elements);
                    return new Cell(new BString(C));
                case CellAffinity.CSTRING:
                    byte[] v = BitHelper.BufferXor(A.valueBINARY, B.valueBINARY);
                    return new Cell(ASCIIEncoding.Unicode.GetString(v));
                case CellAffinity.TREF:
                    return CellValues.NullTREF;
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.Xor(null, null, A.valueEQUATION, B.valueEQUATION));

            }

            throw new Exception();

        }

        public static Cell AutoIncrement(Cell A)
        {

            if (A.IsNull) return A;

            if (A.IsArray)
            {
                CellArray y = new CellArray();
                for (int i = 0; i < A.ARRAY.Count; i++)
                    y.Append(CellOperations.AutoIncrement(A.ARRAY[i]));
            }

            switch (A.AFFINITY)
            {
                case CellAffinity.DOUBLE: return new Cell(A.valueDOUBLE + 1D);
                case CellAffinity.SINGLE: return new Cell(A.valueSINGLE + 1F);
                case CellAffinity.LONG: return new Cell(A.valueLONG + 1L);
                case CellAffinity.INT: return new Cell(A.valueINT + 1);
                case CellAffinity.SHORT: return new Cell(A.valueSHORT + 1);
                case CellAffinity.BYTE: return new Cell(A.valueBYTE + 1);
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.Add(null, null, A.valueEQUATION, new Expression.Literal(null,null, 1)));
            }

            return new Cell(A.AFFINITY);

        }

        public static Cell AutoDecrement(Cell A)
        {

            if (A.IsNull) return A;

            if (A.IsArray)
            {
                CellArray y = new CellArray();
                for (int i = 0; i < A.ARRAY.Count; i++)
                    y.Append(CellOperations.AutoDecrement(A.ARRAY[i]));
            }

            switch (A.AFFINITY)
            {
                case CellAffinity.DOUBLE: return new Cell(A.valueDOUBLE - 1D);
                case CellAffinity.SINGLE: return new Cell(A.valueSINGLE - 1F);
                case CellAffinity.LONG: return new Cell(A.valueLONG - 1L);
                case CellAffinity.INT: return new Cell(A.valueINT - 1);
                case CellAffinity.SHORT: return new Cell(A.valueSHORT - 1);
                case CellAffinity.BYTE: return new Cell(A.valueBYTE - 1);
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.Add(null, null, A.valueEQUATION, new Expression.Literal(null, null, -1)));
            }

            return new Cell(A.AFFINITY);

        }

        public static Cell LeftShift(Cell C, int X)
        {

            if (C.IsNull)
                return C;

            switch (C.Affinity)
            {

                case CellAffinity.BOOL:
                    return CellValues.NullBOOL;
                case CellAffinity.DATE_TIME:
                    return CellValues.NullDATE;
                case CellAffinity.BYTE:
                    C.B0 = (byte)(C.B0 << X);
                    return C;
                case CellAffinity.SHORT:
                    C.SHORT = (short)(C.SHORT << X);
                    return C;
                case CellAffinity.SINGLE:
                case CellAffinity.INT:
                    C.INT = (int)(C.INT << X);
                    return C;
                case CellAffinity.DOUBLE:
                case CellAffinity.LONG:
                    C.LONG = (long)(C.LONG << X);
                    return C;
                case CellAffinity.BINARY:
                    C.BINARY = BitHelper.ShiftLeft(C.BINARY, X);
                    return C;
                case CellAffinity.BSTRING:
                    C.BSTRING = new BString(BitHelper.ShiftLeft(C.BSTRING._elements, X));
                    return C;
                case CellAffinity.CSTRING:
                    C.CSTRING = UTF8Encoding.Unicode.GetString(BitHelper.ShiftLeft(UTF8Encoding.Unicode.GetBytes(C.CSTRING), X));
                    return C;
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.LeftShift(null, null, C.valueEQUATION, new Expression.Literal(null, null, X)));

            }

            throw new Exception();

        }

        public static Cell RightShift(Cell C, int X)
        {

            if (C.IsNull)
                return C;

            switch (C.Affinity)
            {

                case CellAffinity.BOOL:
                    return CellValues.NullBOOL;
                case CellAffinity.DATE_TIME:
                    return CellValues.NullDATE;
                case CellAffinity.BYTE:
                    C.B0 = (byte)(C.B0 >> X);
                    return C;
                case CellAffinity.SHORT:
                    C.SHORT = (short)(C.SHORT >> X);
                    return C;
                case CellAffinity.SINGLE:
                case CellAffinity.INT:
                    C.INT = (int)(C.INT >> X);
                    return C;
                case CellAffinity.DOUBLE:
                case CellAffinity.LONG:
                    C.LONG = (long)(C.LONG >> X);
                    return C;
                case CellAffinity.BINARY:
                    C.BINARY = BitHelper.ShiftRight(C.BINARY, X);
                    return C;
                case CellAffinity.BSTRING:
                    C.BSTRING = new BString(BitHelper.ShiftRight(C.BSTRING._elements, X));
                    return C;
                case CellAffinity.CSTRING:
                    C.CSTRING = UTF8Encoding.Unicode.GetString(BitHelper.ShiftRight(UTF8Encoding.Unicode.GetBytes(C.CSTRING), X));
                    return C;
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.RightShift(null, null, C.valueEQUATION, new Expression.Literal(null, null, X)));

            }

            throw new Exception();

        }

        public static Cell LeftRotate(Cell C, int X)
        {

            if (C.IsNull)
                return C;

            switch (C.Affinity)
            {

                case CellAffinity.BOOL:
                    return CellValues.NullBOOL;
                case CellAffinity.DATE_TIME:
                    return CellValues.NullDATE;
                case CellAffinity.BYTE:
                    C.B0 = BitHelper.RotateLeft(C.B0, X);
                    return C;
                case CellAffinity.SHORT:
                    C.SHORT = BitHelper.RotateLeft(C.SHORT, X);
                    return C;
                case CellAffinity.SINGLE:
                case CellAffinity.INT:
                    C.INT = BitHelper.RotateLeft(C.INT, X);
                    return C;
                case CellAffinity.DOUBLE:
                case CellAffinity.LONG:
                    C.LONG = BitHelper.RotateLeft(C.LONG, X);
                    return C;
                case CellAffinity.BINARY:
                    C.BINARY = BitHelper.RotateLeft(C.BINARY, X);
                    return C;
                case CellAffinity.BSTRING:
                    C.BSTRING = new BString(BitHelper.RotateLeft(C.BSTRING._elements, X));
                    return C;
                case CellAffinity.CSTRING:
                    C.CSTRING = UTF8Encoding.Unicode.GetString(BitHelper.RotateLeft(UTF8Encoding.Unicode.GetBytes(C.CSTRING), X));
                    return C;
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.LeftRotate(null, null, C.valueEQUATION, new Expression.Literal(null, null, X)));
                    
            }

            throw new Exception();

        }

        public static Cell RightRotate(Cell C, int X)
        {

            if (C.IsNull)
                return C;

            switch (C.Affinity)
            {

                case CellAffinity.BOOL:
                    return CellValues.NullBOOL;
                case CellAffinity.DATE_TIME:
                    return CellValues.NullDATE;
                case CellAffinity.BYTE:
                    C.B0 = BitHelper.RotateRight(C.B0, X);
                    return C;
                case CellAffinity.SHORT:
                    C.SHORT = BitHelper.RotateRight(C.SHORT, X);
                    return C;
                case CellAffinity.SINGLE:
                case CellAffinity.INT:
                    C.INT = BitHelper.RotateRight(C.INT, X);
                    return C;
                case CellAffinity.DOUBLE:
                case CellAffinity.LONG:
                    C.LONG = BitHelper.RotateRight(C.LONG, X);
                    return C;
                case CellAffinity.BINARY:
                    C.BINARY = BitHelper.RotateRight(C.BINARY, X);
                    return C;
                case CellAffinity.BSTRING:
                    C.BSTRING = new BString(BitHelper.RotateRight(C.BSTRING._elements, X));
                    return C;
                case CellAffinity.CSTRING:
                    C.CSTRING = UTF8Encoding.Unicode.GetString(BitHelper.RotateRight(UTF8Encoding.Unicode.GetBytes(C.CSTRING), X));
                    return C;
                case CellAffinity.EQUATION:
                    return new Cell(new Expression.Binary.RightRotate(null, null, C.valueEQUATION, new Expression.Literal(null, null, X)));

            }

            throw new Exception();

        }

        ///// <summary>
        ///// Adds two cells together for LONG and DOUBLE, concatentates strings, returns null otherwise
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>Cell result</returns>
        //public static Cell Add(Cell A, Cell B)
        //{

        //    if (A.IsNull || B.IsNull)
        //        return new Cell(CellAffinityHelper.Highest(A.AFFINITY, B.AFFINITY));

        //    if (A.AFFINITY == CellAffinity.ARRAY && B.AFFINITY == CellAffinity.ARRAY && A.ARRAY.Count == B.ARRAY.Count)
        //    {
        //        Cell x = new Cell(new CellArray());
        //        for (int i = 0; i < A.ARRAY.Count; i++)
        //        {
        //            x.ARRAY.Append(CellOperations.Add(A.ARRAY[i],B.ARRAY[i]));
        //        }
        //        return x;
        //    }
        //    if (A.AFFINITY == CellAffinity.DOUBLE || B.AFFINITY == CellAffinity.DOUBLE)
        //    {
        //        return new Cell(A.valueDOUBLE + B.valueDOUBLE);
        //    }
        //    else if (A.AFFINITY == CellAffinity.SINGLE || B.AFFINITY == CellAffinity.SINGLE)
        //    {
        //        return new Cell(A.valueSINGLE + B.valueSINGLE);
        //    }
        //    else if (A.AFFINITY == CellAffinity.LONG || B.AFFINITY == CellAffinity.LONG)
        //    {
        //        return new Cell(A.valueLONG + B.valueLONG);
        //    }
        //    else if (A.AFFINITY == CellAffinity.INT || B.AFFINITY == CellAffinity.INT)
        //    {
        //        return new Cell(A.valueINT + B.valueINT);
        //    }
        //    else if (A.AFFINITY == CellAffinity.SHORT || B.AFFINITY == CellAffinity.SHORT)
        //    {
        //        return new Cell(A.valueSHORT + B.valueSHORT);
        //    }
        //    else if (A.AFFINITY == CellAffinity.BYTE || B.AFFINITY == CellAffinity.BYTE)
        //    {
        //        return new Cell(A.valueBYTE + B.valueBYTE);
        //    }

        //    return new Cell(CellAffinityHelper.Highest(A.AFFINITY, B.AFFINITY));

        //}

        ///// <summary>
        ///// Converts either an LONG or DOUBLE to a positve AWValue, returns the cell passed otherwise
        ///// </summary>
        ///// <param name="C">A cell</param>
        ///// <returns>Cell result</returns>
        //public static Cell Plus(Cell C)
        //{

        //    // Check nulls //
        //    if (C.NULL == 1)
        //        return C;

        //    if (C.AFFINITY == CellAffinity.SINGLE)
        //        C.SINGLE = +C.SINGLE;
        //    else if (C.AFFINITY == CellAffinity.DOUBLE)
        //        C.DOUBLE = +C.DOUBLE;
        //    else if (C.AFFINITY == CellAffinity.SHORT)
        //        C.SHORT = (short)(+C.SHORT);
        //    else if (C.AFFINITY == CellAffinity.INT)
        //        C.INT = +C.INT;
        //    else if (C.AFFINITY == CellAffinity.LONG)
        //        C.LONG = +C.LONG;
        //    else
        //        C.NULL = 1;

        //    return C;

        //}

        ///// <summary>
        ///// Adds one to the given cell for an LONG or DOUBLE, returns the cell passed otherwise
        ///// </summary>
        ///// <param name="C">The cell argument</param>
        ///// <returns>Cell result</returns>
        //public static Cell PlusPlus(Cell C)
        //{
        //    if (C.NULL == 1)
        //        return C;
        //    if (C.AFFINITY == CellAffinity.SINGLE)
        //        C.SINGLE++;
        //    else if (C.AFFINITY == CellAffinity.DOUBLE)
        //        C.DOUBLE++;
        //    else if (C.AFFINITY == CellAffinity.BYTE)
        //        C.BYTE++;
        //    else if (C.AFFINITY == CellAffinity.SHORT)
        //        C.SHORT++;
        //    else if (C.AFFINITY == CellAffinity.INT)
        //        C.INT++;
        //    else if (C.AFFINITY == CellAffinity.LONG)
        //        C.LONG++;
        //    return C;
        //}

        ///// <summary>
        ///// Subtracts two cells together for LONG and DOUBLE, repalces instances of C2 in C1, for date times, return the tick count difference as an LONG
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>Cell result</returns>
        //public static Cell Minus(Cell C1, Cell C2)
        //{

        //    // Check nulls //
        //    if (C1.NULL == 1) return C1;
        //    else if (C2.NULL == 1) return C2;

        //    // If affinities match //
        //    if (C1.AFFINITY == C2.AFFINITY)
        //    {
        //        if (C1.AFFINITY == CellAffinity.SINGLE)
        //        {
        //            C1.SINGLE -= C2.SINGLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.DOUBLE)
        //        {
        //            C1.DOUBLE -= C2.DOUBLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BYTE)
        //        {
        //            C1.BYTE -= C2.BYTE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.SHORT)
        //        {
        //            C1.SHORT -= C2.SHORT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.INT)
        //        {
        //            C1.INT -= C2.INT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.LONG)
        //        {
        //            C1.LONG -= C2.LONG;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.DATE_TIME)
        //        {
        //            C1.LONG = C1.LONG - C2.LONG;
        //            C1.AFFINITY = CellAffinity.LONG;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BSTRING)
        //        {
        //            C1.BSTRING = C1.BSTRING.Remove(C2.BSTRING);
        //        }
        //        else if (C1.AFFINITY == CellAffinity.CSTRING)
        //        {
        //            C1.CSTRING = C1.CSTRING.Replace(C2.CSTRING, "");
        //        }
        //        else
        //        {
        //            C1.NULL = 1;
        //        }

        //    }
        //    else
        //    {

        //        if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
        //        {
        //            C1.CSTRING = C1.valueCSTRING.Replace(C2.valueCSTRING, "");
        //            C1.AFFINITY = CellAffinity.CSTRING;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
        //        {
        //            C1.BSTRING = C1.valueBSTRING.Replace(C2.valueBSTRING, "");
        //            C1.AFFINITY = CellAffinity.BSTRING;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BINARY || C2.AFFINITY == CellAffinity.BINARY)
        //        {
        //            C1.AFFINITY = CellAffinity.BINARY;
        //            C1.NULL = 1;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
        //        {
        //            C1.DOUBLE = C1.valueDOUBLE - C2.valueDOUBLE;
        //            C1.AFFINITY = CellAffinity.DOUBLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
        //        {
        //            C1.SINGLE = C1.valueSINGLE - C2.valueSINGLE;
        //            C1.AFFINITY = CellAffinity.SINGLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
        //        {
        //            C1.LONG = C1.valueLONG - C2.valueLONG;
        //            C1.AFFINITY = CellAffinity.LONG;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
        //        {
        //            C1.INT = C1.valueINT - C2.valueINT;
        //            C1.AFFINITY = CellAffinity.INT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
        //        {
        //            C1.SHORT = (short)(C1.valueSHORT - C2.valueSHORT);
        //            C1.AFFINITY = CellAffinity.SHORT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
        //        {
        //            C1.BYTE = (byte)(C1.valueBYTE - C2.valueBYTE);
        //            C1.AFFINITY = CellAffinity.BYTE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
        //        {
        //            C1.AFFINITY = CellAffinity.DATE_TIME;
        //            C1.NULL = 1;
        //        }
        //        else
        //        {
        //            C1.AFFINITY = CellAffinity.BOOL;
        //            C1.NULL = 1;
        //        }

        //    }

        //    // Fix nulls //
        //    if (C1.NULL == 1)
        //    {
        //        C1.ULONG = 0;
        //        C1.CSTRING = "";
        //    }

        //    return C1;

        //}

        ///// <summary>
        ///// Converts either an LONG or DOUBLE to a negative AWValue, returns the cell passed otherwise
        ///// </summary>
        ///// <param name="C">A cell</param>
        ///// <returns>Cell result</returns>
        //public static Cell Minus(Cell C)
        //{

        //    // Check nulls //
        //    if (C.NULL == 1)
        //        return C;

        //    if (C.AFFINITY == CellAffinity.DOUBLE)
        //        C.DOUBLE = -C.DOUBLE;
        //    else if (C.AFFINITY == CellAffinity.SINGLE)
        //        C.SINGLE = -C.SINGLE;
        //    else if (C.AFFINITY == CellAffinity.SHORT)
        //        C.SHORT = (short)(-C.SHORT);
        //    else if (C.AFFINITY == CellAffinity.INT)
        //        C.INT = -C.INT;
        //    else if (C.AFFINITY == CellAffinity.LONG)
        //        C.LONG = -C.LONG;
        //    else if (C.AFFINITY == CellAffinity.CSTRING || C.AFFINITY == CellAffinity.BSTRING)
        //        C.CSTRING = new string(C.CSTRING.Reverse().ToArray());
        //    else
        //        C.NULL = 1;

        //    return C;

        //}

        ///// <summary>
        ///// Subtracts one to the given cell for an LONG or DOUBLE, returns the cell passed otherwise
        ///// </summary>
        ///// <param name="C">The cell argument</param>
        ///// <returns>Cell result</returns>
        //public static Cell MinusMinus(Cell C)
        //{
        //    if (C.NULL == 1)
        //        return C;
        //    if (C.AFFINITY == CellAffinity.DOUBLE)
        //        C.DOUBLE--;
        //    else if (C.AFFINITY == CellAffinity.SINGLE)
        //        C.SINGLE--;
        //    else if (C.AFFINITY == CellAffinity.SHORT)
        //        C.SHORT--;
        //    else if (C.AFFINITY == CellAffinity.INT)
        //        C.INT--;
        //    else if (C.AFFINITY == CellAffinity.LONG)
        //        C.LONG--;
        //    return C;
        //}

        ///// <summary>
        ///// Multiplies two cells together for LONG and DOUBLE; if C1 is a string and C2 is either int/double, repeats the string C2 times; 
        ///// otherwise, returns the cell passed otherwise
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>Cell result</returns>
        //public static Cell Multiply(Cell C1, Cell C2)
        //{

        //    // Check nulls //
        //    if (C1.NULL == 1) return C1;
        //    else if (C2.NULL == 1) return C2;

        //    // If affinities match //
        //    if (C1.AFFINITY == C2.AFFINITY)
        //    {

        //        if (C1.AFFINITY == CellAffinity.DOUBLE)
        //        {
        //            C1.DOUBLE *= C2.DOUBLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.SINGLE)
        //        {
        //            C1.SINGLE *= C2.SINGLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BYTE)
        //        {
        //            C1.BYTE *= C2.BYTE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.SHORT)
        //        {
        //            C1.SHORT *= C2.SHORT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.INT)
        //        {
        //            C1.INT *= C2.INT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.LONG)
        //        {
        //            C1.LONG *= C2.LONG;
        //        }
        //        else
        //        {
        //            C1.NULL = 1;
        //        }

        //    }
        //    else
        //    {

        //        if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
        //        {
        //            C1.AFFINITY = CellAffinity.CSTRING;
        //            C1.NULL = 1;
        //        }
        //        if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
        //        {
        //            C1.AFFINITY = CellAffinity.BSTRING;
        //            C1.NULL = 1;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BINARY || C2.AFFINITY == CellAffinity.BINARY)
        //        {
        //            C1.AFFINITY = CellAffinity.BINARY;
        //            C1.NULL = 1;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
        //        {
        //            C1.DOUBLE = C1.valueDOUBLE * C2.valueDOUBLE;
        //            C1.AFFINITY = CellAffinity.DOUBLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
        //        {
        //            C1.SINGLE = C1.valueSINGLE * C2.valueSINGLE;
        //            C1.AFFINITY = CellAffinity.SINGLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
        //        {
        //            C1.LONG = C1.valueLONG * C2.valueLONG;
        //            C1.AFFINITY = CellAffinity.LONG;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
        //        {
        //            C1.INT = C1.valueINT * C2.valueINT;
        //            C1.AFFINITY = CellAffinity.INT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
        //        {
        //            C1.SHORT = (short)(C1.valueSHORT * C2.valueSHORT);
        //            C1.AFFINITY = CellAffinity.SHORT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
        //        {
        //            C1.BYTE = (byte)(C1.valueBYTE * C2.valueBYTE);
        //            C1.AFFINITY = CellAffinity.BYTE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
        //        {
        //            C1.AFFINITY = CellAffinity.DATE_TIME;
        //            C1.NULL = 1;
        //        }
        //        else
        //        {
        //            C1.AFFINITY = CellAffinity.BOOL;
        //            C1.NULL = 1;
        //        }

        //    }

        //    // Fix nulls //
        //    if (C1.NULL == 1)
        //    {
        //        C1.ULONG = 0;
        //        C1.CSTRING = "";
        //    }

        //    return C1;

        //}

        ///// <summary>
        ///// Divides two cells together for LONG and DOUBLE, returns the cell passed otherwise as null
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>Cell result</returns>
        //public static Cell Divide(Cell C1, Cell C2)
        //{

        //    // Check nulls //
        //    if (C1.NULL == 1) return C1;
        //    else if (C2.NULL == 1) return C2;

        //    // If affinities match //
        //    if (C1.AFFINITY == C2.AFFINITY)
        //    {

        //        if (C1.AFFINITY == CellAffinity.DOUBLE && C2.DOUBLE != 0)
        //        {
        //            C1.DOUBLE /= C2.DOUBLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.SINGLE && C2.SINGLE != 0)
        //        {
        //            C1.SINGLE /= C2.SINGLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.LONG && C2.LONG != 0)
        //        {
        //            C1.LONG /= C2.LONG;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.INT && C2.INT != 0)
        //        {
        //            C1.INT /= C2.INT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.SHORT && C2.SHORT != 0)
        //        {
        //            C1.SHORT /= C2.SHORT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BYTE && C2.BYTE != 0)
        //        {
        //            C1.BYTE /= C2.BYTE;
        //        }
        //        else
        //        {
        //            C1.NULL = 1;
        //        }

        //    }
        //    else
        //    {

        //        if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
        //        {
        //            C1.AFFINITY = CellAffinity.CSTRING;
        //            C1.NULL = 1;
        //        }
        //        if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
        //        {
        //            C1.AFFINITY = CellAffinity.BSTRING;
        //            C1.NULL = 1;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BINARY || C2.AFFINITY == CellAffinity.BINARY)
        //        {
        //            C1.AFFINITY = CellAffinity.BINARY;
        //            C1.NULL = 1;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
        //        {

        //            if (C2.valueDOUBLE != 0)
        //            {
        //                C1.DOUBLE = C1.valueDOUBLE / C2.valueDOUBLE;
        //            }
        //            else
        //            {
        //                C1.NULL = 1;
        //            }
        //            C1.AFFINITY = CellAffinity.DOUBLE;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
        //        {

        //            if (C2.valueSINGLE != 0)
        //            {
        //                C1.SINGLE = C1.valueSINGLE / C2.valueSINGLE;
        //            }
        //            else
        //            {
        //                C1.NULL = 1;
        //            }
        //            C1.AFFINITY = CellAffinity.SINGLE;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
        //        {

        //            if (C2.valueLONG != 0)
        //            {
        //                C1.LONG = C1.valueLONG / C2.valueLONG;
        //            }
        //            else
        //            {
        //                C1.NULL = 1;
        //            }
        //            C1.AFFINITY = CellAffinity.LONG;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
        //        {

        //            if (C2.valueINT != 0)
        //            {
        //                C1.INT = C1.valueINT / C2.valueINT;
        //            }
        //            else
        //            {
        //                C1.NULL = 1;
        //            }
        //            C1.AFFINITY = CellAffinity.INT;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
        //        {

        //            if (C2.valueSHORT != 0)
        //            {
        //                C1.SHORT = (short)(C1.valueSHORT / C2.valueSHORT);
        //            }
        //            else
        //            {
        //                C1.NULL = 1;
        //            }
        //            C1.AFFINITY = CellAffinity.SHORT;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
        //        {

        //            if (C2.valueBYTE != 0)
        //            {
        //                C1.BYTE = (byte)(C1.valueBYTE / C2.valueBYTE);
        //            }
        //            else
        //            {
        //                C1.NULL = 1;
        //            }
        //            C1.AFFINITY = CellAffinity.BYTE;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
        //        {
        //            C1.AFFINITY = CellAffinity.DATE_TIME;
        //            C1.NULL = 1;
        //        }
        //        else
        //        {
        //            C1.AFFINITY = CellAffinity.BOOL;
        //            C1.NULL = 1;
        //        }

        //    }

        //    // Fix nulls //
        //    if (C1.NULL == 1)
        //    {
        //        C1.ULONG = 0;
        //        C1.CSTRING = "";
        //    }

        //    return C1;

        //}

        ///// <summary>
        ///// Divides two cells together for LONG and DOUBLE, returns the cell passed otherwise as null; if C2 is 0, then it returns 0
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>Cell result</returns>
        //public static Cell CheckDivide(Cell C1, Cell C2)
        //{

        //    // Check nulls //
        //    if (C1.NULL == 1)
        //        return C1;
        //    else if (C2.NULL == 1)
        //        return C2;

        //    // If affinities match //
        //    if (C1.AFFINITY == C2.AFFINITY)
        //    {

        //        if (C1.AFFINITY == CellAffinity.DOUBLE && C2.DOUBLE != 0)
        //        {
        //            C1.DOUBLE /= C2.DOUBLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.SINGLE && C2.SINGLE != 0)
        //        {
        //            C1.SINGLE /= C2.SINGLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.LONG && C2.LONG != 0)
        //        {
        //            C1.LONG /= C2.LONG;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.INT && C2.INT != 0)
        //        {
        //            C1.INT /= C2.INT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.SHORT && C2.SHORT != 0)
        //        {
        //            C1.SHORT /= C2.SHORT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BYTE && C2.BYTE != 0)
        //        {
        //            C1.BYTE /= C2.BYTE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BYTE || C1.AFFINITY == CellAffinity.SHORT || C1.AFFINITY == CellAffinity.INT
        //            || C1.AFFINITY == CellAffinity.LONG || C1.AFFINITY == CellAffinity.SINGLE || C1.AFFINITY == CellAffinity.DOUBLE)
        //        {
        //            C1.ULONG = 0;
        //        }
        //        else
        //        {
        //            C1.NULL = 1;
        //        }

        //    }
        //    else
        //    {

        //        if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
        //        {
        //            C1.AFFINITY = CellAffinity.CSTRING;
        //            C1.NULL = 1;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
        //        {
        //            C1.AFFINITY = CellAffinity.BSTRING;
        //            C1.NULL = 1;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BINARY || C2.AFFINITY == CellAffinity.BINARY)
        //        {
        //            C1.AFFINITY = CellAffinity.BINARY;
        //            C1.NULL = 1;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
        //        {

        //            if (C2.valueDOUBLE != 0)
        //            {
        //                C1.DOUBLE = C1.valueDOUBLE / C2.valueDOUBLE;
        //            }
        //            else
        //            {
        //                C1.DOUBLE = 0D;
        //            }
        //            C1.AFFINITY = CellAffinity.DOUBLE;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
        //        {

        //            if (C2.valueSINGLE != 0)
        //            {
        //                C1.SINGLE = C1.valueSINGLE / C2.valueSINGLE;
        //            }
        //            else
        //            {
        //                C1.SINGLE = 0F;
        //            }
        //            C1.AFFINITY = CellAffinity.SINGLE;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
        //        {

        //            if (C2.valueLONG != 0)
        //            {
        //                C1.LONG = C1.valueLONG / C2.valueLONG;
        //            }
        //            else
        //            {
        //                C1.LONG = 0;
        //            }
        //            C1.AFFINITY = CellAffinity.LONG;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
        //        {

        //            if (C2.valueINT != 0)
        //            {
        //                C1.INT = C1.valueINT / C2.valueINT;
        //            }
        //            else
        //            {
        //                C1.INT = 0;
        //            }
        //            C1.AFFINITY = CellAffinity.INT;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
        //        {

        //            if (C2.valueSHORT != 0)
        //            {
        //                C1.SHORT = (short)(C1.valueSHORT / C2.valueSHORT);
        //            }
        //            else
        //            {
        //                C1.SHORT = 0;
        //            }
        //            C1.AFFINITY = CellAffinity.SHORT;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
        //        {

        //            if (C2.valueBYTE != 0)
        //            {
        //                C1.BYTE = (byte)(C1.valueBYTE / C2.valueBYTE);
        //            }
        //            else
        //            {
        //                C1.BYTE = 0;
        //            }
        //            C1.AFFINITY = CellAffinity.BYTE;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
        //        {
        //            C1.AFFINITY = CellAffinity.DATE_TIME;
        //            C1.NULL = 1;
        //        }
        //        else
        //        {
        //            C1.AFFINITY = CellAffinity.BOOL;
        //            C1.NULL = 1;
        //        }

        //    }

        //    // Fix nulls //
        //    if (C1.NULL == 1)
        //    {
        //        C1.ULONG = 0;
        //        C1.CSTRING = "";
        //    }

        //    return C1;

        //}

        ///// <summary>
        ///// Perform modulo between two cells together for LONG and DOUBLE, returns the cell passed otherwise
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>Cell result</returns>
        //public static Cell Mod(Cell C1, Cell C2)
        //{

        //    // Check nulls //
        //    if (C1.NULL == 1) return C1;
        //    else if (C2.NULL == 1) return C2;

        //    // If affinities match //
        //    if (C1.AFFINITY == C2.AFFINITY)
        //    {

        //        if (C1.AFFINITY == CellAffinity.DOUBLE && C2.DOUBLE != 0)
        //        {
        //            C1.DOUBLE %= C2.DOUBLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.SINGLE && C2.SINGLE != 0)
        //        {
        //            C1.SINGLE %= C2.SINGLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.LONG && C2.LONG != 0)
        //        {
        //            C1.LONG %= C2.LONG;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.INT && C2.INT != 0)
        //        {
        //            C1.INT %= C2.INT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.SHORT && C2.SHORT != 0)
        //        {
        //            C1.SHORT %= C2.SHORT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BYTE && C2.BYTE != 0)
        //        {
        //            C1.BYTE %= C2.BYTE;
        //        }
        //        else
        //        {
        //            C1.NULL = 1;
        //        }

        //    }
        //    else
        //    {

        //        if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
        //        {
        //            C1.AFFINITY = CellAffinity.CSTRING;
        //            C1.NULL = 1;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
        //        {
        //            C1.AFFINITY = CellAffinity.BSTRING;
        //            C1.NULL = 1;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BINARY || C2.AFFINITY == CellAffinity.BINARY)
        //        {
        //            C1.AFFINITY = CellAffinity.BINARY;
        //            C1.NULL = 1;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
        //        {

        //            if (C2.valueDOUBLE != 0)
        //            {
        //                C1.DOUBLE = C1.valueDOUBLE % C2.valueDOUBLE;
        //            }
        //            else
        //            {
        //                C1.NULL = 1;
        //            }
        //            C1.AFFINITY = CellAffinity.DOUBLE;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
        //        {

        //            if (C2.valueSINGLE != 0)
        //            {
        //                C1.SINGLE = C1.valueSINGLE % C2.valueSINGLE;
        //            }
        //            else
        //            {
        //                C1.NULL = 1;
        //            }
        //            C1.AFFINITY = CellAffinity.SINGLE;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
        //        {

        //            if (C2.valueLONG != 0)
        //            {
        //                C1.LONG = C1.valueLONG % C2.valueLONG;
        //            }
        //            else
        //            {
        //                C1.NULL = 1;
        //            }
        //            C1.AFFINITY = CellAffinity.LONG;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
        //        {

        //            if (C2.valueINT != 0)
        //            {
        //                C1.INT = C1.valueINT % C2.valueINT;
        //            }
        //            else
        //            {
        //                C1.NULL = 1;
        //            }
        //            C1.AFFINITY = CellAffinity.INT;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
        //        {

        //            if (C2.valueSHORT != 0)
        //            {
        //                C1.SHORT = (short)(C1.valueSHORT % C2.valueSHORT);
        //            }
        //            else
        //            {
        //                C1.NULL = 1;
        //            }
        //            C1.AFFINITY = CellAffinity.SHORT;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
        //        {

        //            if (C2.valueBYTE != 0)
        //            {
        //                C1.BYTE = (byte)(C1.valueBYTE % C2.valueBYTE);
        //            }
        //            else
        //            {
        //                C1.NULL = 1;
        //            }
        //            C1.AFFINITY = CellAffinity.BYTE;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
        //        {
        //            C1.AFFINITY = CellAffinity.DATE_TIME;
        //            C1.NULL = 1;
        //        }
        //        else
        //        {
        //            C1.AFFINITY = CellAffinity.BOOL;
        //            C1.NULL = 1;
        //        }

        //    }

        //    // Fix nulls //
        //    if (C1.NULL == 1)
        //    {
        //        C1.ULONG = 0;
        //        C1.CSTRING = "";
        //    }

        //    return C1;

        //}

        ///// <summary>
        ///// Mods two cells together for LONG and DOUBLE, returns the cell passed otherwise as null; if C2 is 0, then it returns 0
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>Cell result</returns>
        //public static Cell CheckMod(Cell C1, Cell C2)
        //{

        //    // Check nulls //
        //    if (C1.NULL == 1)
        //        return C1;
        //    else if (C2.NULL == 1)
        //        return C2;

        //    // If affinities match //
        //    if (C1.AFFINITY == C2.AFFINITY)
        //    {

        //        if (C1.AFFINITY == CellAffinity.DOUBLE && C2.DOUBLE != 0)
        //        {
        //            C1.DOUBLE %= C2.DOUBLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.SINGLE && C2.SINGLE != 0)
        //        {
        //            C1.SINGLE %= C2.SINGLE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.LONG && C2.LONG != 0)
        //        {
        //            C1.LONG %= C2.LONG;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.INT && C2.INT != 0)
        //        {
        //            C1.INT %= C2.INT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.SHORT && C2.SHORT != 0)
        //        {
        //            C1.SHORT %= C2.SHORT;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BYTE && C2.BYTE != 0)
        //        {
        //            C1.BYTE %= C2.BYTE;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BYTE || C1.AFFINITY == CellAffinity.SHORT || C1.AFFINITY == CellAffinity.INT
        //            || C1.AFFINITY == CellAffinity.LONG || C1.AFFINITY == CellAffinity.SINGLE || C1.AFFINITY == CellAffinity.DOUBLE)
        //        {
        //            C1.ULONG = 0;
        //        }
        //        else
        //        {
        //            C1.NULL = 1;
        //        }

        //    }
        //    else
        //    {

        //        if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
        //        {
        //            C1.AFFINITY = CellAffinity.CSTRING;
        //            C1.NULL = 1;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
        //        {
        //            C1.AFFINITY = CellAffinity.BSTRING;
        //            C1.NULL = 1;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.BINARY || C2.AFFINITY == CellAffinity.BINARY)
        //        {
        //            C1.AFFINITY = CellAffinity.BINARY;
        //            C1.NULL = 1;
        //        }
        //        else if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
        //        {

        //            if (C2.valueDOUBLE != 0)
        //            {
        //                C1.DOUBLE = C1.valueDOUBLE % C2.valueDOUBLE;
        //            }
        //            else
        //            {
        //                C1.DOUBLE = 0D;
        //            }
        //            C1.AFFINITY = CellAffinity.DOUBLE;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
        //        {

        //            if (C2.valueSINGLE != 0)
        //            {
        //                C1.SINGLE = C1.valueSINGLE % C2.valueSINGLE;
        //            }
        //            else
        //            {
        //                C1.SINGLE = 0F;
        //            }
        //            C1.AFFINITY = CellAffinity.SINGLE;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
        //        {

        //            if (C2.valueLONG != 0)
        //            {
        //                C1.LONG = C1.valueLONG % C2.valueLONG;
        //            }
        //            else
        //            {
        //                C1.LONG = 0;
        //            }
        //            C1.AFFINITY = CellAffinity.LONG;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
        //        {

        //            if (C2.valueINT != 0)
        //            {
        //                C1.INT = C1.valueINT % C2.valueINT;
        //            }
        //            else
        //            {
        //                C1.INT = 0;
        //            }
        //            C1.AFFINITY = CellAffinity.INT;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
        //        {

        //            if (C2.valueSHORT != 0)
        //            {
        //                C1.SHORT = (short)(C1.valueSHORT % C2.valueSHORT);
        //            }
        //            else
        //            {
        //                C1.SHORT = 0;
        //            }
        //            C1.AFFINITY = CellAffinity.SHORT;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
        //        {

        //            if (C2.valueBYTE != 0)
        //            {
        //                C1.BYTE = (byte)(C1.valueBYTE % C2.valueBYTE);
        //            }
        //            else
        //            {
        //                C1.BYTE = 0;
        //            }
        //            C1.AFFINITY = CellAffinity.BYTE;

        //        }
        //        else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
        //        {
        //            C1.AFFINITY = CellAffinity.DATE_TIME;
        //            C1.NULL = 1;
        //        }
        //        else
        //        {
        //            C1.AFFINITY = CellAffinity.BOOL;
        //            C1.NULL = 1;
        //        }

        //    }

        //    // Fix nulls //
        //    if (C1.NULL == 1)
        //    {
        //        C1.ULONG = 0;
        //        C1.CSTRING = "";
        //    }

        //    return C1;

        //}

        ///// <summary>
        ///// Return the bitwise AND for all types
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>Cell result</returns>
        //public static Cell And(Cell C1, Cell C2)
        //{

        //    // Check nulls //
        //    if (C1.NULL == 1)
        //        return C1;
        //    else if (C2.NULL == 1)
        //        return C2;

        //    // Handle AND two bools //
        //    if (C1.AFFINITY == CellAffinity.BOOL && C2.AFFINITY == CellAffinity.BOOL)
        //        return (C1.BOOL && C2.BOOL ? CellValues.True : CellValues.False);

        //    // If neither a string or blob //
        //    if (C1.AFFINITY != CellAffinity.CSTRING && C2.AFFINITY != CellAffinity.CSTRING
        //        && C1.AFFINITY != CellAffinity.BSTRING && C2.AFFINITY != CellAffinity.BSTRING
        //        && C1.AFFINITY != CellAffinity.BINARY && C2.AFFINITY != CellAffinity.BINARY)
        //    {

        //        C1.LONG = C1.LONG & C2.LONG;
        //        if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
        //            C1.AFFINITY = CellAffinity.DOUBLE;
        //        else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
        //            C1.AFFINITY = CellAffinity.SINGLE;
        //        else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
        //            C1.AFFINITY = CellAffinity.LONG;
        //        else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
        //            C1.AFFINITY = CellAffinity.INT;
        //        else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
        //            C1.AFFINITY = CellAffinity.SHORT;
        //        else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
        //            C1.AFFINITY = CellAffinity.BYTE;
        //        else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
        //            C1.AFFINITY = CellAffinity.DATE_TIME;
        //        else if (C1.AFFINITY == CellAffinity.BOOL || C2.AFFINITY == CellAffinity.BOOL)
        //            C1.AFFINITY = CellAffinity.BOOL;

        //    }
        //    else if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
        //    {

        //        StringBuilder sb = new StringBuilder();
        //        int t = 0;
        //        for (int i = 0; i < C1.CSTRING.Length; i++)
        //        {
        //            if (t >= C2.valueCSTRING.Length)
        //                t = 0;
        //            sb.Append((char)(C1.valueCSTRING[i] & C2.valueCSTRING[t]));
        //            t++;
        //        }
        //        C1.CSTRING = sb.ToString();

        //    }
        //    else if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
        //    {

        //        StringBuilder sb = new StringBuilder();
        //        int t = 0;
        //        for (int i = 0; i < C1.CSTRING.Length; i++)
        //        {
        //            if (t >= C2.valueCSTRING.Length)
        //                t = 0;
        //            sb.Append((char)(C1.valueCSTRING[i] & C2.valueCSTRING[t]));
        //            t++;
        //        }
        //        C1.CSTRING = sb.ToString();

        //    }
        //    else
        //    {

        //        int t = 0;
        //        byte[] b = C2.valueBINARY;
        //        for (int i = 0; i < C1.BINARY.Length; i++)
        //        {
        //            if (t >= b.Length)
        //                t = 0;
        //            C1.BINARY[i] = (byte)(C1.BINARY[i] & b[t]);
        //            t++;
        //        }

        //    }

        //    return C1;

        //}

        ///// <summary>
        ///// Returns the bitwise OR for all types
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>Cell result</returns>
        //public static Cell Or(Cell C1, Cell C2)
        //{

        //    // Check nulls //
        //    if (C1.NULL == 1) return C1;
        //    else if (C2.NULL == 1) return C2;

        //    // Handle AND two bools //
        //    if (C1.AFFINITY == CellAffinity.BOOL && C2.AFFINITY == CellAffinity.BOOL)
        //        return (C1.BOOL || C2.BOOL ? CellValues.True : CellValues.False);

        //    // If neither a string or blob //
        //    if (C1.AFFINITY != CellAffinity.CSTRING && C2.AFFINITY != CellAffinity.CSTRING
        //        && C1.AFFINITY != CellAffinity.BSTRING && C2.AFFINITY != CellAffinity.BSTRING
        //        && C1.AFFINITY != CellAffinity.BINARY && C2.AFFINITY != CellAffinity.BINARY)
        //    {

        //        C1.LONG = C1.LONG | C2.LONG;
        //        if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
        //            C1.AFFINITY = CellAffinity.DOUBLE;
        //        else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
        //            C1.AFFINITY = CellAffinity.SINGLE;
        //        else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
        //            C1.AFFINITY = CellAffinity.LONG;
        //        else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
        //            C1.AFFINITY = CellAffinity.INT;
        //        else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
        //            C1.AFFINITY = CellAffinity.SHORT;
        //        else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
        //            C1.AFFINITY = CellAffinity.BYTE;
        //        else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
        //            C1.AFFINITY = CellAffinity.DATE_TIME;
        //        else if (C1.AFFINITY == CellAffinity.BOOL || C2.AFFINITY == CellAffinity.BOOL)
        //            C1.AFFINITY = CellAffinity.BOOL;

        //    }
        //    else if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        int t = 0;
        //        for (int i = 0; i < C1.CSTRING.Length; i++)
        //        {
        //            if (t >= C2.valueCSTRING.Length)
        //                t = 0;
        //            sb.Append((char)(C1.CSTRING[i] | C2.valueCSTRING[t]));
        //            t++;
        //        }
        //        C1.CSTRING = sb.ToString();

        //    }
        //    else if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        int t = 0;
        //        for (int i = 0; i < C1.CSTRING.Length; i++)
        //        {
        //            if (t >= C2.valueCSTRING.Length)
        //                t = 0;
        //            sb.Append((char)(C1.CSTRING[i] | C2.valueCSTRING[t]));
        //            t++;
        //        }
        //        C1.CSTRING = sb.ToString();

        //    }
        //    else
        //    {

        //        int t = 0;
        //        byte[] b = C2.valueBINARY;
        //        for (int i = 0; i < C1.BINARY.Length; i++)
        //        {
        //            if (t >= b.Length) t = 0;
        //            C1.BINARY[i] = (byte)(C1.BINARY[i] | b[t]);
        //            t++;
        //        }

        //    }

        //    return C1;

        //}

        ///// <summary>
        ///// Returns the bitwise XOR for all types
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>Cell result</returns>
        //public static Cell Xor(Cell C1, Cell C2)
        //{

        //    // Check nulls //
        //    if (C1.NULL == 1) return C1;
        //    else if (C2.NULL == 1) return C2;

        //    // Handle AND two bools //
        //    if (C1.AFFINITY == CellAffinity.BOOL && C2.AFFINITY == CellAffinity.BOOL)
        //        return (C1.BOOL ^ C2.BOOL ? CellValues.True : CellValues.False);

        //    // If neither a string or blob //
        //    if (C1.AFFINITY != CellAffinity.CSTRING && C2.AFFINITY != CellAffinity.CSTRING
        //        && C1.AFFINITY != CellAffinity.BSTRING && C2.AFFINITY != CellAffinity.BSTRING
        //        && C1.AFFINITY != CellAffinity.BINARY && C2.AFFINITY != CellAffinity.BINARY)
        //    {

        //        C1.LONG = C1.LONG ^ C2.LONG;
        //        if (C1.AFFINITY == CellAffinity.DOUBLE || C2.AFFINITY == CellAffinity.DOUBLE)
        //            C1.AFFINITY = CellAffinity.DOUBLE;
        //        else if (C1.AFFINITY == CellAffinity.SINGLE || C2.AFFINITY == CellAffinity.SINGLE)
        //            C1.AFFINITY = CellAffinity.SINGLE;
        //        else if (C1.AFFINITY == CellAffinity.LONG || C2.AFFINITY == CellAffinity.LONG)
        //            C1.AFFINITY = CellAffinity.LONG;
        //        else if (C1.AFFINITY == CellAffinity.INT || C2.AFFINITY == CellAffinity.INT)
        //            C1.AFFINITY = CellAffinity.INT;
        //        else if (C1.AFFINITY == CellAffinity.SHORT || C2.AFFINITY == CellAffinity.SHORT)
        //            C1.AFFINITY = CellAffinity.SHORT;
        //        else if (C1.AFFINITY == CellAffinity.BYTE || C2.AFFINITY == CellAffinity.BYTE)
        //            C1.AFFINITY = CellAffinity.BYTE;
        //        else if (C1.AFFINITY == CellAffinity.DATE_TIME || C2.AFFINITY == CellAffinity.DATE_TIME)
        //            C1.AFFINITY = CellAffinity.DATE_TIME;
        //        else if (C1.AFFINITY == CellAffinity.BOOL || C2.AFFINITY == CellAffinity.BOOL)
        //            C1.AFFINITY = CellAffinity.BOOL;

        //    }
        //    else if (C1.AFFINITY == CellAffinity.CSTRING || C2.AFFINITY == CellAffinity.CSTRING)
        //    {

        //        StringBuilder sb = new StringBuilder();
        //        int t = 0;
        //        for (int i = 0; i < C1.CSTRING.Length; i++)
        //        {
        //            if (t >= C2.valueCSTRING.Length) t = 0;
        //            sb.Append((char)(C1.CSTRING[i] ^ C2.valueCSTRING[t]));
        //            t++;
        //        }
        //        C1.CSTRING = sb.ToString();

        //    }
        //    else if (C1.AFFINITY == CellAffinity.BSTRING || C2.AFFINITY == CellAffinity.BSTRING)
        //    {

        //        StringBuilder sb = new StringBuilder();
        //        int t = 0;
        //        for (int i = 0; i < C1.CSTRING.Length; i++)
        //        {
        //            if (t >= C2.valueCSTRING.Length) t = 0;
        //            sb.Append((char)(C1.CSTRING[i] ^ C2.valueCSTRING[t]));
        //            t++;
        //        }
        //        C1.CSTRING = sb.ToString();

        //    }
        //    else
        //    {

        //        int t = 0;
        //        byte[] a = C2.valueBINARY;
        //        byte[] b = C2.valueBINARY;
        //        for (int i = 0; i < a.Length; i++)
        //        {
        //            if (t >= b.Length) t = 0;
        //            a[i] = (byte)(a[i] ^ b[t]);
        //            t++;
        //        }
        //        C1.AFFINITY = CellAffinity.BINARY;
        //        C1.BINARY = a;

        //    }
        //    return C1;

        //}

        ///// <summary>
        ///// Checks if two cells are equal
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>A boolean</returns>
        //public static bool Equals(Cell C1, Cell C2)
        //{

        //    if (C1.NULL == 1 && C2.NULL == 1)
        //        return true;

        //    return CellComparer.Compare(C1, C2) == 0;

        //}

        ///// <summary>
        ///// Checks if two cells are not equal
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>A boolean</returns>
        //public static bool NotEquals(Cell C1, Cell C2)
        //{

        //    if (C1.NULL != C2.NULL)
        //        return true;

        //    return CellComparer.Compare(C1, C2) != 0;

        //}

        ///// <summary>
        ///// Checks if C1 is less than C2
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>A boolean</returns>
        //public static bool LessThan(Cell C1, Cell C2)
        //{
        //    return CellComparer.Compare(C1, C2) < 0;
        //}

        ///// <summary>
        ///// Checks if C1 is less than or equal to C2
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>A boolean</returns>
        //public static bool LessThanOrEquals(Cell C1, Cell C2)
        //{
        //    return CellComparer.Compare(C1, C2) <= 0;
        //}

        ///// <summary>
        ///// Checks if C1 is greater than C2
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>A boolean</returns>
        //public static bool GreaterThan(Cell C1, Cell C2)
        //{
        //    return CellComparer.Compare(C1, C2) > 0;
        //}

        ///// <summary>
        ///// Checks if C1 is greater than or equal to C2
        ///// </summary>
        ///// <param name="C1">Left cell</param>
        ///// <param name="C2">AWValue cell</param>
        ///// <returns>A boolean</returns>
        //public static bool GreaterThanOrEquals(Cell C1, Cell C2)
        //{
        //    return CellComparer.Compare(C1, C2) >= 0;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="C"></param>
        ///// <param name="X"></param>
        ///// <returns></returns>
        //public static Cell LeftShift(Cell C, int X)
        //{

        //    if (C.IsNull)
        //        return C;

        //    switch (C.Affinity)
        //    {

        //        case CellAffinity.BOOL:
        //            return CellValues.NullBOOL;
        //        case CellAffinity.DATE_TIME:
        //            return CellValues.NullDATE;
        //        case CellAffinity.BYTE:
        //            C.B0 = (byte)(C.B0 << X);
        //            return C;
        //        case CellAffinity.SHORT:
        //            C.SHORT = (short)(C.SHORT << X);
        //            return C;
        //        case CellAffinity.SINGLE:
        //        case CellAffinity.INT:
        //            C.INT = (int)(C.INT << X);
        //            return C;
        //        case CellAffinity.DOUBLE:
        //        case CellAffinity.LONG:
        //            C.LONG = (long)(C.LONG << X);
        //            return C;
        //        case CellAffinity.BINARY:
        //            C.BINARY = BitHelper.ShiftLeft(C.BINARY, X);
        //            return C;
        //        case CellAffinity.BSTRING:
        //            C.BSTRING = new BString(BitHelper.ShiftLeft(C.BSTRING._elements, X));
        //            return C;
        //        case CellAffinity.CSTRING:
        //            C.CSTRING = UTF8Encoding.Unicode.GetString(BitHelper.ShiftLeft(UTF8Encoding.Unicode.GetBytes(C.CSTRING), X));
        //            return C;

        //    }

        //    throw new Exception();

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="C"></param>
        ///// <param name="X"></param>
        ///// <returns></returns>
        //public static Cell RightShift(Cell C, int X)
        //{

        //    if (C.IsNull)
        //        return C;

        //    switch (C.Affinity)
        //    {

        //        case CellAffinity.BOOL:
        //            return CellValues.NullBOOL;
        //        case CellAffinity.DATE_TIME:
        //            return CellValues.NullDATE;
        //        case CellAffinity.BYTE:
        //            C.B0 = (byte)(C.B0 >> X);
        //            return C;
        //        case CellAffinity.SHORT:
        //            C.SHORT = (short)(C.SHORT >> X);
        //            return C;
        //        case CellAffinity.SINGLE:
        //        case CellAffinity.INT:
        //            C.INT = (int)(C.INT >> X);
        //            return C;
        //        case CellAffinity.DOUBLE:
        //        case CellAffinity.LONG:
        //            C.LONG = (long)(C.LONG >> X);
        //            return C;
        //        case CellAffinity.BINARY:
        //            C.BINARY = BitHelper.ShiftRight(C.BINARY, X);
        //            return C;
        //        case CellAffinity.BSTRING:
        //            C.BSTRING = new BString(BitHelper.ShiftRight(C.BSTRING._elements, X));
        //            return C;
        //        case CellAffinity.CSTRING:
        //            C.CSTRING = UTF8Encoding.Unicode.GetString(BitHelper.ShiftRight(UTF8Encoding.Unicode.GetBytes(C.CSTRING), X));
        //            return C;

        //    }

        //    throw new Exception();

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="C"></param>
        ///// <param name="X"></param>
        ///// <returns></returns>
        //public static Cell LeftRotate(Cell C, int X)
        //{

        //    if (C.IsNull)
        //        return C;

        //    switch (C.Affinity)
        //    {

        //        case CellAffinity.BOOL:
        //            return CellValues.NullBOOL;
        //        case CellAffinity.DATE_TIME:
        //            return CellValues.NullDATE;
        //        case CellAffinity.BYTE:
        //            C.B0 = BitHelper.RotateLeft(C.B0, X);
        //            return C;
        //        case CellAffinity.SHORT:
        //            C.SHORT = BitHelper.RotateLeft(C.SHORT, X);
        //            return C;
        //        case CellAffinity.SINGLE:
        //        case CellAffinity.INT:
        //            C.INT = BitHelper.RotateLeft(C.INT, X);
        //            return C;
        //        case CellAffinity.DOUBLE:
        //        case CellAffinity.LONG:
        //            C.LONG = BitHelper.RotateLeft(C.LONG, X);
        //            return C;
        //        case CellAffinity.BINARY:
        //            C.BINARY = BitHelper.RotateLeft(C.BINARY, X);
        //            return C;
        //        case CellAffinity.BSTRING:
        //            C.BSTRING = new BString(BitHelper.RotateLeft(C.BSTRING._elements, X));
        //            return C;
        //        case CellAffinity.CSTRING:
        //            C.CSTRING = UTF8Encoding.Unicode.GetString(BitHelper.RotateLeft(UTF8Encoding.Unicode.GetBytes(C.CSTRING), X));
        //            return C;

        //    }

        //    throw new Exception();

        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="C"></param>
        ///// <param name="X"></param>
        ///// <returns></returns>
        //public static Cell RightRotate(Cell C, int X)
        //{

        //    if (C.IsNull)
        //        return C;

        //    switch (C.Affinity)
        //    {

        //        case CellAffinity.BOOL:
        //            return CellValues.NullBOOL;
        //        case CellAffinity.DATE_TIME:
        //            return CellValues.NullDATE;
        //        case CellAffinity.BYTE:
        //            C.B0 = BitHelper.RotateRight(C.B0, X);
        //            return C;
        //        case CellAffinity.SHORT:
        //            C.SHORT = BitHelper.RotateRight(C.SHORT, X);
        //            return C;
        //        case CellAffinity.SINGLE:
        //        case CellAffinity.INT:
        //            C.INT = BitHelper.RotateRight(C.INT, X);
        //            return C;
        //        case CellAffinity.DOUBLE:
        //        case CellAffinity.LONG:
        //            C.LONG = BitHelper.RotateRight(C.LONG, X);
        //            return C;
        //        case CellAffinity.BINARY:
        //            C.BINARY = BitHelper.RotateRight(C.BINARY, X);
        //            return C;
        //        case CellAffinity.BSTRING:
        //            C.BSTRING = new BString(BitHelper.RotateRight(C.BSTRING._elements, X));
        //            return C;
        //        case CellAffinity.CSTRING:
        //            C.CSTRING = UTF8Encoding.Unicode.GetString(BitHelper.RotateRight(UTF8Encoding.Unicode.GetBytes(C.CSTRING), X));
        //            return C;

        //    }

        //    throw new Exception();

        //}

        public static Cell Join(Cell A, Cell B)
        {

            if (A.IsNull || B.IsNull)
                return new Cell(CellAffinityHelper.Highest(A.AFFINITY, B.AFFINITY));

            if (A.AFFINITY == CellAffinity.ARRAY && B.AFFINITY == CellAffinity.ARRAY)
            {
                CellArray x = new CellArray();
                foreach (Cell c in A.valueARRAY)
                    x.Append(c);
                foreach (Cell c in B.valueARRAY)
                    x.Append(c);
                return new Cell(x);
            }
            else if (A.AFFINITY == CellAffinity.ARRAY && B.AFFINITY != CellAffinity.ARRAY)
            {
                A.ARRAY.Append(B);
                return A;
            }
            else if (A.AFFINITY != CellAffinity.ARRAY && B.AFFINITY == CellAffinity.ARRAY)
            {
                B.ARRAY.Append(A);
                return B;
            }
            else if (A.AFFINITY == CellAffinity.CSTRING || B.AFFINITY == CellAffinity.CSTRING)
            {
                return new Cell(A.valueCSTRING + B.valueCSTRING);
            }
            else if (A.AFFINITY == CellAffinity.BSTRING || B.AFFINITY == CellAffinity.BSTRING)
            {
                return new Cell(A.valueBSTRING + B.valueBSTRING);
            }
            else if (A.AFFINITY == CellAffinity.BINARY || B.AFFINITY == CellAffinity.BINARY)
            {
                byte[] a = A.valueBINARY;
                byte[] b = B.valueBINARY;
                byte[] c = new byte[a.Length + b.Length];
                Array.Copy(a, 0, c, 0, a.Length);
                Array.Copy(b, 0, c, a.Length, b.Length);
                return new Cell(c);
            }
            else if (A.AFFINITY == CellAffinity.DOUBLE || B.AFFINITY == CellAffinity.DOUBLE)
            {
                return new Cell(A.valueDOUBLE + B.valueDOUBLE);
            }
            else if (A.AFFINITY == CellAffinity.SINGLE || B.AFFINITY == CellAffinity.SINGLE)
            {
                return new Cell(A.valueSINGLE + B.valueSINGLE);
            }
            else if (A.AFFINITY == CellAffinity.LONG || B.AFFINITY == CellAffinity.LONG)
            {
                return new Cell(A.valueLONG + B.valueLONG);
            }
            else if (A.AFFINITY == CellAffinity.INT || B.AFFINITY == CellAffinity.INT)
            {
                return new Cell(A.valueINT + B.valueINT);
            }
            else if (A.AFFINITY == CellAffinity.SHORT || B.AFFINITY == CellAffinity.SHORT)
            {
                return new Cell(A.valueSHORT + B.valueSHORT);
            }
            else if (A.AFFINITY == CellAffinity.BYTE || B.AFFINITY == CellAffinity.BYTE)
            {
                return new Cell(A.valueBYTE + B.valueBYTE);
            }

            return new Cell(CellAffinityHelper.Highest(A.AFFINITY, B.AFFINITY));

        }
    
    }

}
