using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Structures;

namespace Spectre.Cells
{


    /// <summary>
    /// Methods for comparing cells
    /// </summary>
    public static class CellComparer
    {

        public static bool Equals(Cell A, Cell B)
        {

            if (A.NULL == 1 && B.NULL == 1)
                return true;
            else if (A.NULL == 1 || B.NULL == 1)
                return false;

            if (A.AFFINITY == B.AFFINITY)
            {

                if (A.AFFINITY == CellAffinity.BOOL)
                    return A.BOOL == B.BOOL;
                else if (A.AFFINITY == CellAffinity.DATE_TIME)
                    return A.DATE == B.DATE;
                else if (A.AFFINITY == CellAffinity.BYTE)
                    return A.BYTE == B.BYTE;
                else if (A.AFFINITY == CellAffinity.SHORT)
                    return A.SHORT == B.SHORT;
                else if (A.AFFINITY == CellAffinity.INT)
                    return A.INT == B.INT;
                else if (A.AFFINITY == CellAffinity.LONG)
                    return A.LONG == B.LONG;
                else if (A.AFFINITY == CellAffinity.SINGLE)
                    return A.SINGLE == B.SINGLE;
                else if (A.AFFINITY == CellAffinity.DOUBLE)
                    return A.DOUBLE == B.DOUBLE;
                else if (A.AFFINITY == CellAffinity.BINARY)
                    return ByteArrayCompare(A.BINARY, B.BINARY) == 0;
                else if (A.AFFINITY == CellAffinity.BSTRING)
                    return A.BSTRING == B.BSTRING;
                else if (A.Affinity == CellAffinity.CSTRING || A.AFFINITY == CellAffinity.TREF)
                    return StringComparer.OrdinalIgnoreCase.Compare(A.CSTRING, B.CSTRING) == 0;
                else if (A.AFFINITY == CellAffinity.ARRAY)
                {
                    if (A.ARRAY.Count != B.ARRAY.Count) return false;
                    if (A.ARRAY.IsMin && A.ARRAY.IsMin) return true;
                    if (A.ARRAY.IsMax && A.ARRAY.IsMax) return true;
                    for (int i = 0; i < A.ARRAY.Count; i++)
                    {
                        if (!Equals(A.ARRAY[i], B.ARRAY[i])) return false;
                    }
                    return true;
                }

            }
            else
            {
                
                // Note that ARRAY and TREF can only be considered equal if both cells are the same affinity
                
                if (A.AFFINITY == CellAffinity.CSTRING || B.AFFINITY == CellAffinity.CSTRING)
                    return StringComparer.OrdinalIgnoreCase.Compare(A.valueCSTRING, B.valueCSTRING) == 0;
                else if (A.AFFINITY == CellAffinity.BSTRING || B.AFFINITY == CellAffinity.BSTRING)
                    return A.valueBSTRING == B.valueBSTRING;
                else if (A.AFFINITY == CellAffinity.BINARY || B.AFFINITY == CellAffinity.BINARY)
                    return ByteArrayCompare(A.BINARY, B.BINARY) == 0;
                else if (A.AFFINITY == CellAffinity.DOUBLE || B.AFFINITY == CellAffinity.DOUBLE)
                    return A.valueDOUBLE == B.valueDOUBLE;
                else if (A.AFFINITY == CellAffinity.SINGLE || B.AFFINITY == CellAffinity.SINGLE)
                    return A.valueSINGLE == B.valueSINGLE;
                else if (A.AFFINITY == CellAffinity.LONG || B.AFFINITY == CellAffinity.LONG)
                    return A.valueLONG == B.valueLONG;
                else if (A.AFFINITY == CellAffinity.INT || B.AFFINITY == CellAffinity.INT)
                    return A.valueINT == B.valueINT;
                else if (A.AFFINITY == CellAffinity.SHORT || B.AFFINITY == CellAffinity.SHORT)
                    return A.valueSHORT == B.valueSHORT;
                else if (A.AFFINITY == CellAffinity.BYTE || B.AFFINITY == CellAffinity.BYTE)
                    return A.valueBYTE == B.valueBYTE;
                else if (A.AFFINITY == CellAffinity.DATE_TIME || B.AFFINITY == CellAffinity.DATE_TIME)
                    return A.valueDATE_TIME == B.valueDATE_TIME;
                else if (A.valueBOOL == B.valueBOOL)
                    return A.valueBOOL == B.valueBOOL;

            }

            return false;

        }

        public static bool EqualsStrict(Cell A, Cell B)
        {

            if (A.AFFINITY != B.AFFINITY)
                return false;

            if (A.NULL == 1 && B.NULL == 1)
                return true;
            else if (A.NULL == 1 || B.NULL == 1)
                return false;

            if (A.AFFINITY == CellAffinity.BOOL)
                return A.BOOL == B.BOOL;
            else if (A.AFFINITY == CellAffinity.DATE_TIME)
                return A.DATE == B.DATE;
            else if (A.AFFINITY == CellAffinity.BYTE)
                return A.BYTE == B.BYTE;
            else if (A.AFFINITY == CellAffinity.SHORT)
                return A.SHORT == B.SHORT;
            else if (A.AFFINITY == CellAffinity.INT)
                return A.INT == B.INT;
            else if (A.AFFINITY == CellAffinity.LONG)
                return A.LONG == B.LONG;
            else if (A.AFFINITY == CellAffinity.SINGLE)
                return A.SINGLE == B.SINGLE;
            else if (A.AFFINITY == CellAffinity.DOUBLE)
                return A.DOUBLE == B.DOUBLE;
            else if (A.AFFINITY == CellAffinity.BINARY)
                return ByteArrayCompare(A.BINARY, B.BINARY) == 0;
            else if (A.AFFINITY == CellAffinity.BSTRING)
                return A.BSTRING == B.BSTRING;
            else if (A.Affinity == CellAffinity.CSTRING)
                return StringComparer.OrdinalIgnoreCase.Compare(A.CSTRING, B.CSTRING) == 0;
            else if (A.Affinity == CellAffinity.TREF)
                return StringComparer.OrdinalIgnoreCase.Compare(A.CSTRING, B.CSTRING) == 0;
            else if (A.AFFINITY == CellAffinity.ARRAY)
            {
                if (A.ARRAY.Count != B.ARRAY.Count) return false;
                if (A.ARRAY.IsMin && A.ARRAY.IsMin) return true;
                if (A.ARRAY.IsMax && A.ARRAY.IsMax) return true;
                for (int i = 0; i < A.ARRAY.Count; i++)
                {
                    if (!Equals(A.ARRAY[i], B.ARRAY[i])) return false;
                }
                return true;
            }
            return false;

        }

        public static int Compare(Cell A, Cell B)
        {

            if (A.NULL == 1 && B.NULL == 1)
                return 0;
            else if (A.NULL == 1)
                return 1;
            else if (B.NULL == 1)
                return -1;

            CellAffinity max = CellAffinityHelper.Highest(A.AFFINITY, B.AFFINITY);

            if (A.AFFINITY == B.AFFINITY)
            {

                if (A.AFFINITY == CellAffinity.BOOL)
                    return (A.BOOL == B.BOOL ? 0 : (A.BOOL ? 1 : -1));

                else if (A.AFFINITY == CellAffinity.DATE_TIME)
                    return DateTime.Compare(A.DATE, B.DATE);

                else if (A.AFFINITY == CellAffinity.BYTE)
                    return (A.BYTE == B.BYTE ? 0 : (A.BYTE < B.BYTE ? -1 : 1));

                else if (A.AFFINITY == CellAffinity.SHORT)
                    return (A.SHORT == B.SHORT ? 0 : (A.SHORT < B.SHORT ? -1 : 1));

                else if (A.AFFINITY == CellAffinity.INT)
                    return (A.INT == B.INT ? 0 : (A.INT < B.INT ? -1 : 1));

                else if (A.AFFINITY == CellAffinity.LONG)
                    return (A.LONG == B.LONG ? 0 : (A.LONG < B.LONG ? -1 : 1));

                else if (A.AFFINITY == CellAffinity.SINGLE)
                    return (A.SINGLE == B.SINGLE ? 0 : (A.SINGLE < B.SINGLE ? -1 : 1));

                else if (A.AFFINITY == CellAffinity.DOUBLE)
                    return (A.DOUBLE == B.DOUBLE ? 0 : (A.DOUBLE < B.DOUBLE ? -1 : 1));

                else if (A.AFFINITY == CellAffinity.BINARY)
                    return ByteArrayCompare(A.BINARY, B.BINARY);

                else if (A.AFFINITY == CellAffinity.BSTRING)
                    return BString.CompareStrict(A.BSTRING, B.BSTRING);

                else if (A.Affinity == CellAffinity.CSTRING || A.Affinity == CellAffinity.TREF)
                    return StringComparer.OrdinalIgnoreCase.Compare(A.CSTRING, B.CSTRING);

                else if (A.AFFINITY == CellAffinity.ARRAY)
                {

                    // Minimums //
                    if ((A.ARRAY.IsMin && !B.ARRAY.IsMin) || (!A.ARRAY.IsMax && B.ARRAY.IsMax)) 
                        return -1;
                    if ((!A.ARRAY.IsMin && B.ARRAY.IsMin) || (A.ARRAY.IsMax && !B.ARRAY.IsMax)) 
                        return 1;
                    if ((A.ARRAY.IsMin && B.ARRAY.IsMin) || (A.ARRAY.IsMax && B.ARRAY.IsMax)) 
                        return 01;


                    if (A.ARRAY.Count != B.ARRAY.Count) return (A.ARRAY.Count > B.ARRAY.Count ? 1 : -1);
                    for (int i = 0; i < A.ARRAY.Count; i++)
                    {
                        int v = Compare(A.ARRAY[i], B.ARRAY[i]);
                        if (v != 0) return v;
                    }
                    return 0;
                }

            }
            else
            {
                if (max == CellAffinity.ARRAY)
                    return 1;

                if (max == CellAffinity.TREF)
                    return StringComparer.OrdinalIgnoreCase.Compare(A.valueTREF, B.valueTREF);

                if (max == CellAffinity.CSTRING)
                    return StringComparer.OrdinalIgnoreCase.Compare(A.valueCSTRING, B.valueCSTRING);

                else if (max == CellAffinity.BSTRING)
                    return BString.CompareStrict(A.valueBSTRING, B.valueBSTRING);

                else if (max == CellAffinity.BINARY)
                    return ByteArrayCompare(A.valueBINARY, B.valueBINARY);

                else if (max == CellAffinity.DOUBLE)
                    return (A.valueDOUBLE == B.valueDOUBLE ? 0 : (A.valueDOUBLE < B.valueDOUBLE ? -1 : 1));

                else if (max == CellAffinity.SINGLE)
                    return (A.valueSINGLE == B.valueSINGLE ? 0 : (A.valueSINGLE < B.valueSINGLE ? -1 : 1));

                else if (max == CellAffinity.LONG)
                    return (A.valueLONG == B.valueLONG ? 0 : (A.valueLONG < B.valueLONG ? -1 : 1));

                else if (max == CellAffinity.INT)
                    return (A.valueINT == B.valueINT ? 0 : (A.valueINT < B.valueINT ? -1 : 1));

                else if (max == CellAffinity.SHORT)
                    return (A.valueSHORT == B.valueSHORT ? 0 : (A.valueSHORT < B.valueSHORT ? -1 : 1));

                else if (max == CellAffinity.BYTE)
                    return (A.valueBYTE == B.valueBYTE ? 0 : (A.valueBYTE < B.valueBYTE ? -1 : 1));

                else if (max == CellAffinity.DATE_TIME)
                    return DateTime.Compare(A.valueDATE_TIME, B.valueDATE_TIME);

                else if (max == CellAffinity.BOOL)
                    return (A.valueBOOL == B.valueBOOL ? 0 : (A.valueBOOL ? 1 : -1));

            }

            return 0;

        }

        internal static int ByteArrayCompare(byte[] A, byte[] B)
        {

            if (A.Length != B.Length)
                return A.Length - B.Length;

            int c = 0;
            for (int i = 0; i < A.Length; i++)
            {
                c = A[i] - B[i];
                if (c != 0)
                    return c;
            }
            return 0;

        }

    }



}
