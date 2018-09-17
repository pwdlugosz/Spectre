using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Structures;

namespace Spectre.Cells
{

    /// <summary>
    /// Support for converting cells to differnt cell types
    /// </summary>
    public static class CellConverter
    {

        /// <summary>
        /// Casts a cell to a given affinity
        /// </summary>
        /// <param name="AWValue"></param>
        /// <param name="Affinity"></param>
        /// <returns></returns>
        public static Cell Cast(Cell Value, CellAffinity Affinity)
        {

            if (Affinity == CellAffinity.VARIANT)
                throw new Exception("Cannot cast to a variant");

            switch (Affinity)
            {
                case CellAffinity.BOOL:
                    return ToBOOL(Value);
                case CellAffinity.DATE_TIME:
                    return ToDATE_TIME(Value);
                case CellAffinity.BYTE:
                    return ToBYTE(Value);
                case CellAffinity.SHORT:
                    return ToSHORT(Value);
                case CellAffinity.INT:
                    return ToINT(Value);
                case CellAffinity.LONG:
                    return ToLONG(Value);
                case CellAffinity.SINGLE:
                    return ToFLOAT(Value);
                case CellAffinity.DOUBLE:
                    return ToDOUBLE(Value);
                case CellAffinity.BINARY:
                    return ToBINARY(Value);
                case CellAffinity.BSTRING:
                    return ToBSTRING(Value);
                case CellAffinity.CSTRING:
                    return ToCSTRING(Value);
                case CellAffinity.EQUATION:
                    return ToEQUATION(Value);
            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinity));

        }

        /// <summary>
        /// Converts a cell value to a bool cell
        /// </summary>
        /// <param name="AWValue"></param>
        /// <returns></returns>
        public static Cell ToBOOL(Cell Value)
        {

            if (Value.Affinity == CellAffinity.BOOL)
                return Value;

            if (Value.IsNull)
                return CellValues.NullBOOL;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullBOOL;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullBOOL;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullBOOL;

            if (Value.AFFINITY == CellAffinity.EQUATION) return CellValues.NullBOOL;
    
            switch (Value.AFFINITY)
            {

                case CellAffinity.BYTE:
                    if (Value.BYTE != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.SHORT:
                    if (Value.SHORT != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.INT:
                    if (Value.INT != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.LONG:
                    if (Value.LONG != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.SINGLE:
                    if (Value.SINGLE != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.DOUBLE:
                    if (Value.DOUBLE != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length == 0)
                        return CellValues.NullBOOL;
                    if (Value.BINARY[0] != 0)
                        return CellValues.True;
                    else
                        return CellValues.False;
                case CellAffinity.BSTRING:
                    if (BString.CompareStrictIgnoreCase(Value.BSTRING, Cell.TRUE_BSTRING) == 0)
                        Value.BOOL = true;
                    else if (BString.CompareStrictIgnoreCase(Value.BSTRING, Cell.FALSE_BSTRING) == 0)
                        Value.BOOL = false;
                    else
                        return CellValues.NullBOOL;
                    break;
                case CellAffinity.CSTRING:
                    if (StringComparer.OrdinalIgnoreCase.Compare(Value.CSTRING, Cell.TRUE_STRING) == 0)
                        Value.BOOL = true;
                    else if (StringComparer.OrdinalIgnoreCase.Compare(Value.CSTRING, Cell.FALSE_STRING) == 0)
                        Value.BOOL = false;
                    else
                        return CellValues.NullBOOL;
                    break;
                case CellAffinity.ARRAY:
                    return CellValues.NullBOOL;

            }

            return CellValues.NullBOOL;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AWValue"></param>
        /// <returns></returns>
        public static Cell ToDATE_TIME(Cell Value)
        {

            if (Value.Affinity == CellAffinity.DATE_TIME)
                return Value;

            if (Value.IsNull)
                return CellValues.NullDATE;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullDATE;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullDATE;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullDATE;

            if (Value.AFFINITY == CellAffinity.EQUATION) return CellValues.NullDATE;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return CellValues.NullDATE;
                case CellAffinity.BYTE:
                    return new Cell(DateTime.FromBinary((long)Value.BYTE));
                case CellAffinity.SHORT:
                    return new Cell(DateTime.FromBinary((long)Value.SHORT));
                case CellAffinity.INT:
                    return new Cell(DateTime.FromBinary((long)Value.INT));
                case CellAffinity.LONG:
                    return new Cell(DateTime.FromBinary((long)Value.LONG));
                case CellAffinity.SINGLE:
                    return new Cell(DateTime.FromBinary((long)Value.SINGLE));
                case CellAffinity.DOUBLE:
                    return new Cell(DateTime.FromBinary((long)Value.DOUBLE));
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length < 8)
                        return CellValues.NullDATE;
                    return new Cell(DateTime.FromBinary(BitConverter.ToInt64(Value.BINARY, 0)));
                case CellAffinity.BSTRING:
                case CellAffinity.CSTRING:
                    return CellParser.DateParse(Value.valueCSTRING);
                case CellAffinity.ARRAY:
                    return CellValues.NullDATE;
            }

            return CellValues.NullDATE;

        }

        /// <summary>
        /// Converts a cell to a byte value
        /// </summary>
        /// <param name="AWValue"></param>
        /// <returns></returns>
        public static Cell ToBYTE(Cell Value)
        {

            if (Value.Affinity == CellAffinity.BYTE)
                return Value;

            if (Value.IsNull)
                return CellValues.NullBYTE;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullBYTE;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullBYTE;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullBYTE;

            if (Value.AFFINITY == CellAffinity.EQUATION) return CellValues.NullBYTE;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((byte)(Value.BOOL ? 1 : 0));
                case CellAffinity.SHORT:
                    return new Cell((byte)(Value.SHORT & byte.MaxValue));
                case CellAffinity.INT:
                    return new Cell((byte)(Value.INT & byte.MaxValue));
                case CellAffinity.DATE_TIME:
                case CellAffinity.LONG:
                    return new Cell((byte)(Value.LONG & byte.MaxValue));
                case CellAffinity.SINGLE:
                    return new Cell((byte)(Value.SINGLE));
                case CellAffinity.DOUBLE:
                    return new Cell((byte)(Value.DOUBLE));
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length == 0)
                        return CellValues.NullBYTE;
                    return new Cell(Value.BINARY[1]);
                case CellAffinity.BSTRING:
                case CellAffinity.CSTRING:
                    byte b = 0;
                    if (byte.TryParse(Value.valueCSTRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullBYTE;
                case CellAffinity.ARRAY:
                    return CellValues.NullBYTE;
            }

            return CellValues.NullBYTE;

        }

        /// <summary>
        /// Converts a value to a short
        /// </summary>
        /// <param name="AWValue"></param>
        /// <returns></returns>
        public static Cell ToSHORT(Cell Value)
        {

            if (Value.Affinity == CellAffinity.SHORT)
                return Value;

            if (Value.IsNull)
                return CellValues.NullSHORT;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullSHORT;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullSHORT;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullSHORT;

            if (Value.AFFINITY == CellAffinity.EQUATION) return CellValues.NullSHORT;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((short)(Value.BOOL ? 1 : 0));
                case CellAffinity.BYTE:
                    return new Cell((short)(Value.BYTE));
                case CellAffinity.INT:
                    return new Cell((short)(Value.INT & short.MaxValue));
                case CellAffinity.DATE_TIME:
                case CellAffinity.LONG:
                    return new Cell((short)(Value.LONG & short.MaxValue));
                case CellAffinity.SINGLE:
                    return new Cell((short)(Value.SINGLE));
                case CellAffinity.DOUBLE:
                    return new Cell((short)(Value.DOUBLE));
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length < 2)
                        return CellValues.NullSHORT;
                    return new Cell(BitConverter.ToInt16(Value.BINARY, 0));
                case CellAffinity.BSTRING:
                case CellAffinity.CSTRING:
                    short b = 0;
                    if (short.TryParse(Value.valueCSTRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullSHORT;
                case CellAffinity.ARRAY:
                    return CellValues.NullSHORT;
            }

            return CellValues.NullSHORT;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AWValue"></param>
        /// <returns></returns>
        public static Cell ToINT(Cell Value)
        {

            if (Value.Affinity == CellAffinity.INT)
                return Value;

            if (Value.IsNull)
                return CellValues.NullINT;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullINT;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullINT;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullINT;

            if (Value.AFFINITY == CellAffinity.EQUATION) return CellValues.NullINT;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((int)(Value.BOOL ? 1 : 0));
                case CellAffinity.BYTE:
                    return new Cell((int)(Value.BYTE));
                case CellAffinity.SHORT:
                    return new Cell((int)(Value.SHORT));
                case CellAffinity.DATE_TIME:
                case CellAffinity.LONG:
                    return new Cell((int)(Value.LONG & int.MaxValue));
                case CellAffinity.SINGLE:
                    return new Cell((int)(Value.SINGLE));
                case CellAffinity.DOUBLE:
                    return new Cell((int)(Value.DOUBLE));
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length < 4)
                        return CellValues.NullINT;
                    return new Cell(BitConverter.ToInt32(Value.BINARY, 0));
                case CellAffinity.BSTRING:
                case CellAffinity.CSTRING:
                    int b = 0;
                    if (int.TryParse(Value.valueCSTRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullINT;
                case CellAffinity.ARRAY:
                    return CellValues.NullINT;
            }

            return CellValues.NullINT;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AWValue"></param>
        /// <returns></returns>
        public static Cell ToLONG(Cell Value)
        {

            if (Value.Affinity == CellAffinity.LONG)
                return Value;

            if (Value.IsNull)
                return CellValues.NullLONG;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullLONG;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullLONG;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullLONG;

            if (Value.AFFINITY == CellAffinity.EQUATION) return CellValues.NullLONG;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((long)(Value.BOOL ? 1 : 0));
                case CellAffinity.BYTE:
                    return new Cell((long)(Value.BYTE));
                case CellAffinity.SHORT:
                    return new Cell((long)(Value.SHORT));
                case CellAffinity.INT:
                    return new Cell((long)(Value.INT));
                case CellAffinity.DATE_TIME:
                    return new Cell((long)(Value.LONG));
                case CellAffinity.SINGLE:
                    return new Cell((long)(Value.SINGLE));
                case CellAffinity.DOUBLE:
                    return new Cell((long)(Value.DOUBLE));
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length < 8)
                        return CellValues.NullLONG;
                    return new Cell(BitConverter.ToInt64(Value.BINARY, 0));
                case CellAffinity.BSTRING:
                case CellAffinity.CSTRING:
                    long b = 0;
                    if (long.TryParse(Value.valueCSTRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullLONG;
                case CellAffinity.ARRAY:
                    return CellValues.NullLONG;
            }

            return CellValues.NullLONG;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AWValue"></param>
        /// <returns></returns>
        public static Cell ToFLOAT(Cell Value)
        {

            if (Value.Affinity == CellAffinity.SINGLE)
                return Value;

            if (Value.IsNull)
                return CellValues.NullSINGLE;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullSINGLE;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullSINGLE;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullSINGLE;

            if (Value.AFFINITY == CellAffinity.EQUATION) return CellValues.NullSINGLE;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((float)(Value.BOOL ? 1 : 0));
                case CellAffinity.BYTE:
                    return new Cell((float)(Value.BYTE));
                case CellAffinity.SHORT:
                    return new Cell((float)(Value.SHORT));
                case CellAffinity.INT:
                    return new Cell((float)(Value.INT));
                case CellAffinity.DATE_TIME:
                case CellAffinity.LONG:
                    return new Cell((float)(Value.LONG));
                case CellAffinity.SINGLE:
                    return new Cell((float)(Value.SINGLE));
                case CellAffinity.DOUBLE:
                    return new Cell((float)(Value.DOUBLE));
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length < 4)
                        return CellValues.NullSINGLE;
                    return new Cell(BitConverter.ToSingle(Value.BINARY, 0));
                case CellAffinity.BSTRING:
                case CellAffinity.CSTRING:
                    float b = 0;
                    if (float.TryParse(Value.valueCSTRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullSINGLE;
                case CellAffinity.ARRAY:
                    return CellValues.NullSINGLE;
            }

            return CellValues.NullSINGLE;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AWValue"></param>
        /// <returns></returns>
        public static Cell ToDOUBLE(Cell Value)
        {

            if (Value.Affinity == CellAffinity.DOUBLE)
                return Value;

            if (Value.IsNull)
                return CellValues.NullDOUBLE;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullDOUBLE;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullDOUBLE;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullDOUBLE;

            if (Value.AFFINITY == CellAffinity.EQUATION) return CellValues.NullDOUBLE;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell((double)(Value.BOOL ? 1 : 0));
                case CellAffinity.BYTE:
                    return new Cell((double)(Value.BYTE));
                case CellAffinity.SHORT:
                    return new Cell((double)(Value.SHORT));
                case CellAffinity.INT:
                    return new Cell((double)(Value.INT));
                case CellAffinity.DATE_TIME:
                case CellAffinity.LONG:
                    return new Cell((double)(Value.LONG));
                case CellAffinity.SINGLE:
                    return new Cell((double)(Value.SINGLE));
                case CellAffinity.BINARY:
                    if (Value.BINARY.Length < 8)
                        return CellValues.NullDOUBLE;
                    return new Cell(BitConverter.ToDouble(Value.BINARY, 0));
                case CellAffinity.BSTRING:
                case CellAffinity.CSTRING:
                    double b = 0;
                    if (double.TryParse(Value.valueCSTRING, out b))
                        return new Cell(b);
                    else
                        return CellValues.NullDOUBLE;
                case CellAffinity.ARRAY:
                    return CellValues.NullDOUBLE;
            }

            return CellValues.NullDOUBLE;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AWValue"></param>
        /// <returns></returns>
        public static Cell ToBINARY(Cell Value)
        {

            if (Value.Affinity == CellAffinity.BINARY)
                return Value;

            if (Value.IsNull)
                return CellValues.NullBLOB;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullBLOB;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullBLOB;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullBLOB;

            if (Value.AFFINITY == CellAffinity.EQUATION) return CellValues.NullBLOB;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell(Value.BOOL ? new byte[1] { 1 } : new byte[1] { 0 });
                case CellAffinity.BYTE:
                    return new Cell(new byte[1] { Value.BYTE });
                case CellAffinity.SHORT:
                    return new Cell(BitConverter.GetBytes(Value.SHORT));
                case CellAffinity.INT:
                    return new Cell(BitConverter.GetBytes(Value.INT));
                case CellAffinity.DATE_TIME:
                case CellAffinity.LONG:
                    return new Cell(BitConverter.GetBytes(Value.LONG));
                case CellAffinity.SINGLE:
                    return new Cell(BitConverter.GetBytes(Value.SINGLE));
                case CellAffinity.DOUBLE:
                    return new Cell(BitConverter.GetBytes(Value.DOUBLE));
                case CellAffinity.BSTRING:
                    return new Cell(Value.BSTRING.ToByteArray);
                case CellAffinity.CSTRING:
                    return new Cell(System.Text.ASCIIEncoding.Unicode.GetBytes(Value.CSTRING));
                case CellAffinity.ARRAY:
                    return CellValues.NullBLOB;

            }

            return CellValues.NullBLOB;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AWValue"></param>
        /// <returns></returns>
        public static Cell ToBSTRING(Cell Value)
        {

            if (Value.Affinity == CellAffinity.BSTRING)
                return Value;

            if (Value.IsNull)
                return CellValues.NullBSTRING;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullBSTRING;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullBSTRING;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullBSTRING;

            if (Value.AFFINITY == CellAffinity.EQUATION) return CellValues.NullBSTRING;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell(new BString(Value.BOOL ? Cell.TRUE_STRING : Cell.FALSE_STRING));
                case CellAffinity.BYTE:
                    return new Cell(new BString(Value.BYTE.ToString()));
                case CellAffinity.SHORT:
                    return new Cell(new BString(Value.SHORT.ToString()));
                case CellAffinity.INT:
                    return new Cell(new BString(Value.INT.ToString()));
                case CellAffinity.LONG:
                    return new Cell(new BString(Value.LONG.ToString()));
                case CellAffinity.DATE_TIME:
                    return new Cell(new BString(Value.DATE.ToString()));
                case CellAffinity.SINGLE:
                    return new Cell(new BString(Value.SINGLE.ToString()));
                case CellAffinity.DOUBLE:
                    return new Cell(new BString(Value.DOUBLE.ToString()));
                case CellAffinity.BINARY:
                    return new Cell(new BString(Value.BINARY));
                case CellAffinity.CSTRING:
                    return new Cell(new BString(Value.CSTRING));
                case CellAffinity.ARRAY:
                    return CellValues.NullBSTRING;
            }

            return CellValues.NullBSTRING;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AWValue"></param>
        /// <returns></returns>
        public static Cell ToCSTRING(Cell Value)
        {

            if (Value.Affinity == CellAffinity.CSTRING)
                return Value;

            if (Value.IsNull)
                return CellValues.NullCSTRING;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullCSTRING;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullCSTRING;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullCSTRING;

            if (Value.AFFINITY == CellAffinity.EQUATION) return CellValues.NullCSTRING;

            switch (Value.AFFINITY)
            {

                case CellAffinity.BOOL:
                    return new Cell(Value.BOOL ? Cell.TRUE_STRING : Cell.FALSE_STRING);
                case CellAffinity.BYTE:
                    return new Cell(Value.BYTE.ToString());
                case CellAffinity.SHORT:
                    return new Cell(Value.SHORT.ToString());
                case CellAffinity.INT:
                    return new Cell(Value.INT.ToString());
                case CellAffinity.LONG:
                    return new Cell(Value.LONG.ToString());
                case CellAffinity.DATE_TIME:
                    return new Cell(Value.DATE.ToString());
                case CellAffinity.SINGLE:
                    return new Cell(Value.SINGLE.ToString());
                case CellAffinity.DOUBLE:
                    return new Cell(Value.DOUBLE.ToString());
                case CellAffinity.BINARY:
                    return new Cell(System.Text.ASCIIEncoding.Unicode.GetString(Value.BINARY, 0, Value.BINARY.Length / 2));
                case CellAffinity.BSTRING:
                    return new Cell(Value.CSTRING);
                case CellAffinity.ARRAY:
                    return CellValues.NullCSTRING;
            }

            return CellValues.NullCSTRING;


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToTREF(Cell Value)
        {

            if (Value.Affinity == CellAffinity.TREF)
                return Value;

            return CellValues.NullTREF;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToARRAY(Cell Value)
        {

            if (Value.Affinity == CellAffinity.ARRAY)
                return Value;

            if (Value.IsNull)
                return CellValues.NullARRAY;

            if (Value.BINARY == null && Value.AFFINITY == CellAffinity.BINARY)
                return CellValues.NullARRAY;

            if (Value.BSTRING == null && Value.AFFINITY == CellAffinity.BSTRING)
                return CellValues.NullARRAY;

            if (Value.CSTRING == null && Value.AFFINITY == CellAffinity.CSTRING)
                return CellValues.NullARRAY;

            if (Value.AFFINITY == CellAffinity.EQUATION) return CellValues.NullARRAY;

            CellArray x = new CellArray();
            x.Append(Value);
            return new Cell(x);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Cell ToEQUATION(Cell Value)
        {

            if (Value.AFFINITY == CellAffinity.EQUATION)
                return Value;

            return new Cell(new Expressions.Expression.Literal(null, null, Value));

        }
        
        // 'Convert' variables //
        /// <summary>
        /// Returns the bool AWValue if the affinity is 'BOOL', true if the LONG property is 0, false otherwise
        /// </summary>
        public static bool ConvertBool(Cell Value)
        {
            if (Value.AFFINITY == CellAffinity.BOOL) return Value.BOOL;
            return Value.LONG == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public static byte ConvertByte(Cell Value)
        {

            if (Value.AFFINITY == CellAffinity.BYTE)
                return Value.BYTE;

            if (Value.AFFINITY == CellAffinity.SHORT)
                return (byte)(Value.SHORT & 255);
            if (Value.AFFINITY == CellAffinity.INT)
                return (byte)(Value.INT & 255);
            if (Value.AFFINITY == CellAffinity.LONG || Value.AFFINITY == CellAffinity.DATE_TIME)
                return (byte)(Value.LONG & 255);
            if (Value.AFFINITY == CellAffinity.SINGLE)
                return (byte)(Value.SINGLE);
            if (Value.AFFINITY == CellAffinity.DOUBLE)
                return (byte)Value.DOUBLE;
            if (Value.AFFINITY == CellAffinity.BOOL)
                return Value.BOOL ? (byte)1 : (byte)0;

            return 0;
            
        }

        /// <summary>
        /// 
        /// </summary>
        public static short ConvertShort(Cell Value)
        {

            if (Value.AFFINITY == CellAffinity.SHORT)
                return Value.SHORT;

            if (Value.AFFINITY == CellAffinity.BYTE)
                return (short)Value.BYTE;
            if (Value.AFFINITY == CellAffinity.INT)
                return (short)(Value.INT & 255);
            if (Value.AFFINITY == CellAffinity.LONG || Value.AFFINITY == CellAffinity.DATE_TIME)
                return (short)(Value.LONG & 255);
            if (Value.AFFINITY == CellAffinity.SINGLE)
                return (short)(Value.SINGLE);
            if (Value.AFFINITY == CellAffinity.DOUBLE)
                return (short)Value.DOUBLE;
            if (Value.AFFINITY == CellAffinity.BOOL)
                return Value.BOOL ? (short)1 : (short)0;

            return 0;
            
        }

        /// <summary>
        /// 
        /// </summary>
        public static int ConvertInt(Cell Value)
        {

            if (Value.AFFINITY == CellAffinity.INT)
                return Value.INT;

            if (Value.AFFINITY == CellAffinity.BYTE)
                return (int)Value.BYTE;
            if (Value.AFFINITY == CellAffinity.SHORT)
                return (int)Value.SHORT;
            if (Value.AFFINITY == CellAffinity.LONG || Value.AFFINITY == CellAffinity.DATE_TIME)
                return (int)(Value.LONG & 255);
            if (Value.AFFINITY == CellAffinity.SINGLE)
                return (int)(Value.SINGLE);
            if (Value.AFFINITY == CellAffinity.DOUBLE)
                return (int)Value.DOUBLE;
            if (Value.AFFINITY == CellAffinity.BOOL)
                return Value.BOOL ? (int)1 : (int)0;

            return 0;
            
        }

        /// <summary>
        /// 
        /// </summary>
        public static long ConvertLong(Cell Value)
        {

            if (Value.AFFINITY == CellAffinity.LONG || Value.AFFINITY == CellAffinity.DATE_TIME)
                return Value.LONG;

            if (Value.AFFINITY == CellAffinity.BYTE)
                return (long)Value.BYTE;
            if (Value.AFFINITY == CellAffinity.SHORT)
                return (long)Value.SHORT;
            if (Value.AFFINITY == CellAffinity.INT)
                return (long)Value.INT;
            if (Value.AFFINITY == CellAffinity.SINGLE)
                return (long)(Value.SINGLE);
            if (Value.AFFINITY == CellAffinity.DOUBLE)
                return (long)Value.DOUBLE;
            if (Value.AFFINITY == CellAffinity.BOOL)
                return Value.BOOL ? (long)1 : (long)0;

            return 0;
            
        }

        /// <summary>
        /// 
        /// </summary>
        public static float ConvertSingle(Cell Value)
        {

            if (Value.AFFINITY == CellAffinity.SINGLE)
                return Value.SINGLE;

            if (Value.AFFINITY == CellAffinity.BYTE)
                return (float)Value.BYTE;
            if (Value.AFFINITY == CellAffinity.SHORT)
                return (float)Value.SHORT;
            if (Value.AFFINITY == CellAffinity.INT)
                return (float)Value.INT;
            if (Value.AFFINITY == CellAffinity.LONG || Value.AFFINITY == CellAffinity.DATE_TIME)
                return (float)Value.LONG;
            if (Value.AFFINITY == CellAffinity.DOUBLE)
                return (float)Value.DOUBLE;
            if (Value.AFFINITY == CellAffinity.BOOL)
                return Value.BOOL ? (float)1 : (float)0;

            return 0;

        }

        /// <summary>
        /// Return the DOUBLE AWValue if the affinity is DOUBLE, casts the LONG as an DOUBLE if the affinity is a LONG, 0 otherwise
        /// </summary>
        public static double ConvertDouble(Cell Value)
        {

            if (Value.AFFINITY == CellAffinity.DOUBLE)
                return Value.DOUBLE;

            if (Value.AFFINITY == CellAffinity.BYTE)
                return (double)Value.BYTE;
            if (Value.AFFINITY == CellAffinity.SHORT)
                return (double)Value.SHORT;
            if (Value.AFFINITY == CellAffinity.INT)
                return (double)Value.INT;
            if (Value.AFFINITY == CellAffinity.LONG || Value.AFFINITY == CellAffinity.DATE_TIME)
                return (double)Value.LONG;
            if (Value.AFFINITY == CellAffinity.SINGLE)
                return (double)Value.SINGLE;
            if (Value.AFFINITY == CellAffinity.BOOL)
                return Value.BOOL ? (double)1 : (double)0;

            return 0;

        }

        /// <summary>
        /// Returns the Spike DATE_TIME if the affinity is DATE_TIME, otherwise return the minimum date time .Net AWValue
        /// </summary>
        public static DateTime ConvertDateTime(Cell Value)
        {
            if (Value.Affinity == CellAffinity.DATE_TIME) return Value.DATE;
            return DateTime.MinValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public static BString ConvertBString(Cell Value)
        {

            if (Value.IsNull)
                return new BString(Cell.NULL_STRING_TEXT);

            switch (Value.Affinity)
            {

                case CellAffinity.BYTE:
                    return new BString(Value.BYTE.ToString());
                case CellAffinity.SHORT:
                    return new BString(Value.SHORT.ToString());
                case CellAffinity.INT:
                    return new BString(Value.INT.ToString());
                case CellAffinity.LONG:
                    return new BString(Value.LONG.ToString());

                case CellAffinity.SINGLE:
                    return new BString(Math.Round(Value.SINGLE, Cell.NUMERIC_ROUNDER).ToString());
                case CellAffinity.DOUBLE:
                    return new BString(Math.Round(Value.DOUBLE, Cell.NUMERIC_ROUNDER).ToString());

                case CellAffinity.BOOL:
                    return new BString(Value.BOOL ? Cell.TRUE_STRING : Cell.FALSE_STRING);

                case CellAffinity.DATE_TIME:
                    return new BString(CellFormater.ToLongDate(Value.DATE));

                case CellAffinity.BSTRING:
                    return Value.BSTRING;
                case CellAffinity.CSTRING:
                    return new BString(Value.CSTRING);

                case CellAffinity.BINARY:
                    return new BString(Cell.HEX_LITERARL + BitConverter.ToString(Value.BINARY).Replace("-", ""));

                default:
                    return BString.Empty;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        public static string ConvertCString(Cell Value)
        {

            if (Value.IsNull)
                return Cell.NULL_STRING_TEXT;

            switch (Value.Affinity)
            {

                case CellAffinity.BYTE:
                    return Value.BYTE.ToString();
                case CellAffinity.SHORT:
                    return Value.SHORT.ToString();
                case CellAffinity.INT:
                    return Value.INT.ToString();
                case CellAffinity.LONG:
                    return Value.LONG.ToString();

                case CellAffinity.SINGLE:
                    return Math.Round(Value.SINGLE, Cell.NUMERIC_ROUNDER).ToString();
                case CellAffinity.DOUBLE:
                    return Math.Round(Value.DOUBLE, Cell.NUMERIC_ROUNDER).ToString();

                case CellAffinity.BOOL:
                    return Value.BOOL ? Cell.TRUE_STRING : Cell.FALSE_STRING;

                case CellAffinity.DATE_TIME:
                    return CellFormater.ToShortDate(Value.DATE);

                case CellAffinity.BSTRING:
                    return Value.BSTRING.ToString();

                case CellAffinity.CSTRING:
                    return Value.CSTRING;

                case CellAffinity.BINARY:
                    return Cell.HEX_LITERARL + BitConverter.ToString(Value.BINARY).Replace("-", "");

                case CellAffinity.ARRAY:
                    return ArrayStringBuilder(Value.ARRAY);

                case CellAffinity.TREF:
                    return Value.valueTREF;

                default:
                    return "";

            }

        }

        /// <summary>
        /// 
        /// </summary>
        public static byte[] ConvertBinary(Cell Value)
        {

            if (Value.AFFINITY == CellAffinity.BINARY)
                return Value.NULL == 1 ? new byte[0] : Value.BINARY;

            if (Value.AFFINITY == CellAffinity.BOOL)
                return Value.BOOL == true ? new byte[1] { 1 } : new byte[1] { 0 };
            else if (Value.AFFINITY == CellAffinity.BYTE)
                return new byte[1] { Value.BYTE };
            else if (Value.AFFINITY == CellAffinity.SHORT)
                return BitConverter.GetBytes(Value.SHORT);
            else if (Value.AFFINITY == CellAffinity.SINGLE || Value.AFFINITY == CellAffinity.INT)
                return BitConverter.GetBytes(Value.INT);
            else if (Value.AFFINITY == CellAffinity.DATE_TIME || Value.AFFINITY == CellAffinity.LONG || Value.AFFINITY == CellAffinity.DOUBLE)
                return BitConverter.GetBytes(Value.LONG);
            else if (Value.AFFINITY == CellAffinity.BSTRING)
                return Value.BSTRING.ToByteArray;
            else // CSTRING
                return ASCIIEncoding.BigEndianUnicode.GetBytes(Value.CSTRING);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static CellArray ConvertArray(Cell Value)
        {

            if (Value.Affinity != CellAffinity.ARRAY)
                return new CellArray(new List<Cell>() { Value });

            if (Value.IsNull)
                return new CellArray();

            return Value.ARRAY;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Expressions.Expression ConvertExpression(Cell Value)
        {

            if (Value.Affinity != CellAffinity.EQUATION)
                return new Expressions.Expression.Literal(null, null, Value);

            if (Value.IsNull)
                return null;

            return Value.EQUATION;

        }
        
        /// <summary>
        /// Gets the AWValue of the cell as an object
        /// </summary>
        public static object ConvertObject(Cell Value)
        {

            switch (Value.AFFINITY)
            {
                case CellAffinity.BOOL: return Value.BOOL;
                case CellAffinity.DATE_TIME: return Value.DATE;
                case CellAffinity.BYTE: return Value.BYTE;
                case CellAffinity.SHORT: return Value.SHORT;
                case CellAffinity.INT: return Value.INT;
                case CellAffinity.LONG: return Value.LONG;
                case CellAffinity.SINGLE: return Value.SINGLE;
                case CellAffinity.DOUBLE: return Value.DOUBLE;
                case CellAffinity.BINARY: return Value.BINARY;
                case CellAffinity.BSTRING: return Value.BSTRING;
                case CellAffinity.CSTRING: return Value.CSTRING;
                case CellAffinity.ARRAY: return Value.ARRAY;
            }
            return CellValues.NullINT;
            
        }
   
        // Arrays //

        public static bool[] ToBoolArray(CellArray Value)
        {
            return Value.Select((x) => { return x.valueBOOL; }).ToArray();
        }

        public static DateTime[] ToDateTimeArray(CellArray Value)
        {
            return Value.Select((x) => { return x.valueDATE_TIME; }).ToArray();
        }

        public static byte[] ToByteArray(CellArray Value)
        {
            return Value.Select((x) => { return x.valueBYTE; }).ToArray();
        }

        public static short[] ToSHORTArray(CellArray Value)
        {
            return Value.Select((x) => { return x.valueSHORT; }).ToArray();
        }

        public static int[] ToINTArray(CellArray Value)
        {
            return Value.Select((x) => { return x.valueINT; }).ToArray();
        }

        public static long[] ToLONGArray(CellArray Value)
        {
            return Value.Select((x) => { return x.valueLONG; }).ToArray();
        }

        public static float[] ToSINGLEArray(CellArray Value)
        {
            return Value.Select((x) => { return x.valueSINGLE; }).ToArray();
        }

        public static double[] ToDOUBLEArray(CellArray Value)
        {
            return Value.Select((x) => { return x.valueDOUBLE; }).ToArray();
        }

        public static byte[][] ToBinaryArray(CellArray Value)
        {
            return Value.Select((x) => { return x.valueBINARY; }).ToArray();
        }

        public static BString[] ToBStringArray(CellArray Value)
        {
            return Value.Select((x) => { return x.valueBSTRING; }).ToArray();
        }

        public static string[] ToCStringArray(CellArray Value)
        {
            return Value.Select((x) => { return x.valueCSTRING; }).ToArray();
        }

        public static CellArray[] ToArrayArray(CellArray Value)
        {
            return Value.Select((x) => { return x.valueARRAY; }).ToArray();
        }

        public static string[] ToTRefArray(CellArray Value)
        {
            return Value.Select((x) => { return x.valueTREF; }).ToArray();
        }

        private static string ArrayStringBuilder(CellArray Array)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (Cell c in Array)
            {
                if (c.IsNull)
                {
                    sb.Append(Cell.NULL_STRING_TEXT);
                }
                else if (c.IsArray)
                {
                    sb.Append(ArrayStringBuilder(c.ARRAY));
                }
                else
                {
                    sb.Append(c.valueCSTRING);
                }
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();

        }

    }

}
