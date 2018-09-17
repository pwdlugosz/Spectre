using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Tables;
using Spectre.Structures;

namespace Spectre.Cells
{


    /// <summary>
    /// Serializes cellular data
    /// </summary>
    public static class CellSerializer
    {

        public const int DEFAULT_VARIABLE_LEN = 64;
        public const int MAX_ARRAY_LEN = (int)ushort.MaxValue;
        public const int MAX_CSTRING_LEN = (int)short.MaxValue;
        public const int MAX_BSTRING_LEN = (int)ushort.MaxValue;
        public const int MAX_BINARY_LEN = (int)1024;

        public const int META_SIZE = 1;
        public const int LEN_SIZE = 2;
        public const int CHAR_SIZE = 2;
        public const int MEM_SIZE = 16;

        public const int BOOL_SIZE = 1;
        public const int BYTE_SIZE = 1;
        public const int SHORT_SIZE = 2;
        public const int INT_SIZE = 4;
        public const int FLOAT_SIZE = 4;
        public const int LONG_SIZE = 8;
        public const int DOUBLE_SIZE = 8;
        public const int DATE_SIZE = 8;

        /// <summary>
        /// Reads a cell form a buffer
        /// </summary>
        /// <param name="Buffer">The memory buffer</param>
        /// <param name="Location">The location to read from</param>
        /// <param name="NewLocation">Outputs the new location</param>
        /// <returns></returns>
        public static int Read(byte[] Buffer, int Location, out Cell Value)
        {

            // Get the meta data //
            byte m = Buffer[Location];
            Location++;
            CellAffinity a = m < 100 ? (CellAffinity)m : (CellAffinity)(m - 100);
            if (m >= 100)
            {
                Value = CellValues.Null(a);
                return Location;
            }

            Cell C = new Cell(a);
            C.NULL = 0;

            // Booleans //
            if (a == CellAffinity.BOOL)
            {
                C.BOOL = (Buffer[Location] == 1);
                Location++;
            }
            else if (a == CellAffinity.BYTE)
            {
                C.BYTE = Buffer[Location];
                Location++;
            }
            else if (a == CellAffinity.SHORT)
            {
                C.B0 = Buffer[Location];
                C.B1 = Buffer[Location + 1];
                Location += 2;
            }
            else if (a == CellAffinity.INT || a == CellAffinity.SINGLE)
            {
                C.B0 = Buffer[Location];
                C.B1 = Buffer[Location + 1];
                C.B2 = Buffer[Location + 2];
                C.B3 = Buffer[Location + 3];
                Location += 4;
            }
            else if (a == CellAffinity.LONG || a == CellAffinity.DOUBLE || a == CellAffinity.DATE_TIME)
            {
                C.B0 = Buffer[Location];
                C.B1 = Buffer[Location + 1];
                C.B2 = Buffer[Location + 2];
                C.B3 = Buffer[Location + 3];
                C.B4 = Buffer[Location + 4];
                C.B5 = Buffer[Location + 5];
                C.B6 = Buffer[Location + 6];
                C.B7 = Buffer[Location + 7];
                Location += 8;
            }
            else if (a == CellAffinity.BSTRING || a == CellAffinity.BINARY)
            {
                C.B0 = Buffer[Location];
                C.B1 = Buffer[Location + 1];
                Location += 2;

                byte[] b = new byte[C.SHORT];
                for (int i = 0; i < C.SHORT; i++)
                {
                    b[i] = Buffer[Location + i];
                }
                Location += C.SHORT;

                if (a == CellAffinity.BINARY)
                    C.BINARY = b;
                else
                    C.BSTRING = new BString(b);
            }
            else if (a == CellAffinity.CSTRING || a == CellAffinity.TREF)
            {

                C.B0 = Buffer[Location];
                C.B1 = Buffer[Location + 1];
                Location += 2;

                byte[] b = new byte[(int)C.SHORT * 2];
                for (int i = 0; i < (int)C.SHORT * 2; i++)
                {
                    b[i] = Buffer[Location + i];
                }
                Location += (int)C.SHORT * 2;
                C.CSTRING = System.Text.ASCIIEncoding.Unicode.GetString(b);

            }
            else if (a == CellAffinity.ARRAY)
            {
                
                C.B0 = Buffer[Location];
                C.B1 = Buffer[Location + 1];
                Location += 2;

                CellArray x = new CellArray();
                if (C.SHORT == -1)
                {
                    x = CellArray.MinimumArray;
                }
                else if (C.SHORT == -2)
                {
                    x = CellArray.MximumArray;
                }
                else
                {
                    for (int i = 0; i < (int)C.SHORT; i++)
                    {
                        Cell c;
                        Location = Read(Buffer, Location, out c);
                        x.Append(c);
                    }
                }
                C.ARRAY = x;

            }

            Value = C;

            return Location;

        }

        /// <summary>
        /// Reads a record from a buffer
        /// </summary>
        /// <param name="Buffer"></param>
        /// <param name="Location"></param>
        /// <param name="Length"></param>
        /// <param name="NewLocation"></param>
        /// <returns></returns>
        public static int Read(byte[] Buffer, int Location, int Length, out Record Value)
        {

            Cell[] c = new Cell[Length];
            for (int i = 0; i < Length; i++)
            {
                Location = Read(Buffer, Location, out c[i]);
            }

            Value = new Record(c);
            return Location;

        }

        /// <summary>
        /// Writes data to a buffer
        /// </summary>
        /// <param name="Buffer"></param>
        /// <param name="AWValue"></param>
        /// <param name="Location"></param>
        /// <param name="NewLocation"></param>
        public static int Write(byte[] Buffer, Cell Value, int Location)
        {

            if (Value.AFFINITY == CellAffinity.VARIANT || Value.AFFINITY == CellAffinity.EQUATION)
            {
                throw new Exception("Cannot serialize variants or equations");
            }

            // Write the meta data //
            byte meta = (byte)(Value.NULL == 1 ? 100 + (byte)Value.AFFINITY : (byte)Value.AFFINITY);
            Buffer[Location] = meta;
            Location++;

            // Don't write anything else if the data is null
            if (Value.NULL == 1)
            {
                return Location;
            }

            // Write the data //
            switch (Value.AFFINITY)
            {
                case CellAffinity.BOOL:
                    Buffer[Location] = (byte)(Value.BOOL ? 1 : 0);
                    Location++;
                    break;
                case CellAffinity.BYTE:
                    Buffer[Location] = Value.B0;
                    Location++;
                    break;
                case CellAffinity.SHORT:
                    Buffer[Location] = Value.B0;
                    Buffer[Location + 1] = Value.B1;
                    Location += 2;
                    break;
                case CellAffinity.INT:
                case CellAffinity.SINGLE:
                    Buffer[Location] = Value.B0;
                    Buffer[Location + 1] = Value.B1;
                    Buffer[Location + 2] = Value.B2;
                    Buffer[Location + 3] = Value.B3;
                    Location += 4;
                    break;
                case CellAffinity.DATE_TIME:
                case CellAffinity.DOUBLE:
                case CellAffinity.LONG:
                    Buffer[Location] = Value.B0;
                    Buffer[Location + 1] = Value.B1;
                    Buffer[Location + 2] = Value.B2;
                    Buffer[Location + 3] = Value.B3;
                    Buffer[Location + 4] = Value.B4;
                    Buffer[Location + 5] = Value.B5;
                    Buffer[Location + 6] = Value.B6;
                    Buffer[Location + 7] = Value.B7;
                    Location += 8;
                    break;
                case CellAffinity.BINARY:
                    Value.SHORT = (short)Value.BINARY.Length;
                    Buffer[Location] = Value.B0;
                    Buffer[Location + 1] = Value.B1;
                    Location += 2;
                    Array.Copy(Value.BINARY, 0, Buffer, Location, Value.BINARY.Length);
                    Location += (int)Value.SHORT;
                    break;
                case CellAffinity.BSTRING:
                    Value.SHORT = (short)Value.BSTRING.Length;
                    Buffer[Location] = Value.B0;
                    Buffer[Location + 1] = Value.B1;
                    Location += 2;
                    Array.Copy(Value.BSTRING.ToByteArray, 0, Buffer, Location, Value.BSTRING.Length);
                    Location += (int)Value.SHORT;
                    break;
                case CellAffinity.CSTRING:
                case CellAffinity.TREF:
                    Value.SHORT = (short)Value.CSTRING.Length;
                    Buffer[Location] = Value.B0;
                    Buffer[Location + 1] = Value.B1;
                    Location += 2;
                    Array.Copy(System.Text.ASCIIEncoding.Unicode.GetBytes(Value.CSTRING), 0, Buffer, Location, Value.CSTRING.Length * 2);
                    Location += (int)Value.SHORT * 2;
                    break;
                case CellAffinity.ARRAY:
                    if (Value.ARRAY.IsMin)
                    {
                        Value.SHORT = (short)(-1);
                        Buffer[Location] = Value.B0;
                        Buffer[Location + 1] = Value.B1;
                        Location += 2;
                    }
                    else if (Value.ARRAY.IsMax)
                    {
                        Value.SHORT = (short)(-2);
                        Buffer[Location] = Value.B0;
                        Buffer[Location + 1] = Value.B1;
                        Location += 2;
                    }
                    else
                    {
                        Value.SHORT = (short)Value.ARRAY.Count;
                        Buffer[Location] = Value.B0;
                        Buffer[Location + 1] = Value.B1;
                        Location += 2;
                        foreach (Cell c in Value.ARRAY)
                        {
                            Location = Write(Buffer, c, Location);
                        }
                    }
                    break;
            }

            return Location;

        }

        /// <summary>
        /// Writes a record to a buffer
        /// </summary>
        /// <param name="Buffer"></param>
        /// <param name="AWValue"></param>
        /// <param name="Location"></param>
        /// <param name="NewLocation"></param>
        public static int Write(byte[] Buffer, Record Value, int Location)
        {

            for (int i = 0; i < Value.Count; i++)
            {
                Location = Write(Buffer, Value[i], Location);
            }

            return Location;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Affinity"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static int MemorySize(Cell C)
        {

            switch (C.AFFINITY)
            {
                case CellAffinity.BOOL:
                case CellAffinity.BYTE:
                case CellAffinity.SHORT:
                case CellAffinity.INT:
                case CellAffinity.SINGLE:
                case CellAffinity.LONG:
                case CellAffinity.DOUBLE:
                case CellAffinity.DATE_TIME:
                    return MEM_SIZE;
                case CellAffinity.BINARY:
                    return C.BINARY.Length + MEM_SIZE;
                case CellAffinity.BSTRING:
                    return C.BSTRING.Length + MEM_SIZE;
                case CellAffinity.CSTRING:
                    return C.CSTRING.Length * CHAR_SIZE + MEM_SIZE;
                case CellAffinity.ARRAY:
                    int size = 0;
                    foreach (Cell v in C.ARRAY)
                        size += MemorySize(v);
                    return size;
                default:
                    throw new Exception();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Affinity"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static int DiskSize(Cell C)
        {

            switch (C.AFFINITY)
            {
                case CellAffinity.BOOL:
                    return BOOL_SIZE + META_SIZE;
                case CellAffinity.BYTE:
                    return BYTE_SIZE + META_SIZE;
                case CellAffinity.SHORT:
                    return SHORT_SIZE + META_SIZE;
                case CellAffinity.INT:
                    return INT_SIZE + META_SIZE;
                case CellAffinity.SINGLE:
                    return FLOAT_SIZE + META_SIZE;
                case CellAffinity.LONG:
                    return LONG_SIZE + META_SIZE;
                case CellAffinity.DOUBLE:
                    return DOUBLE_SIZE + META_SIZE;
                case CellAffinity.DATE_TIME:
                    return DATE_SIZE + META_SIZE;
                case CellAffinity.BINARY:
                    return C.Length + META_SIZE + LEN_SIZE;
                case CellAffinity.BSTRING:
                    return C.Length + META_SIZE + LEN_SIZE;
                case CellAffinity.CSTRING:
                    return C.Length * CHAR_SIZE + META_SIZE + LEN_SIZE;
                case CellAffinity.ARRAY:
                    int size = 0;
                    foreach (Cell v in C.ARRAY)
                        size += DiskSize(v);
                    return size + META_SIZE + LEN_SIZE;
                default:
                    throw new Exception();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Affinity"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static int Length(CellAffinity Affinity, int Length)
        {

            switch (Affinity)
            {
                case CellAffinity.BOOL:
                    return BOOL_SIZE;
                case CellAffinity.BYTE:
                    return BYTE_SIZE;
                case CellAffinity.SHORT:
                    return SHORT_SIZE;
                case CellAffinity.INT:
                    return INT_SIZE;
                case CellAffinity.SINGLE:
                    return FLOAT_SIZE;
                case CellAffinity.LONG:
                    return LONG_SIZE;
                case CellAffinity.DOUBLE:
                    return DOUBLE_SIZE;
                case CellAffinity.DATE_TIME:
                    return DATE_SIZE;
                case CellAffinity.BINARY:
                case CellAffinity.BSTRING:
                case CellAffinity.CSTRING:
                case CellAffinity.ARRAY:
                case CellAffinity.TREF:
                case CellAffinity.EQUATION:
                    return Length;
                default:
                    throw new Exception();
            }

        }

        /// <summary>
        /// Gets the default memory size; for variable length data types, assumes 64 elements
        /// </summary>
        /// <param name="Affinity"></param>
        /// <returns></returns>
        //public static int DefaultMemorySize(CellAffinity Affinity)
        //{
        //    return MemorySize(Affinity, DEFAULT_VARIABLE_LEN);
        //}

        /// <summary>
        /// Gets the default disk size; for variable length data types, assumes 64 elements
        /// </summary>
        /// <param name="Affinity"></param>
        /// <returns></returns>
        //public static int DefaultDiskSize(CellAffinity Affinity)
        //{
        //    return DiskSize(Affinity, DEFAULT_VARIABLE_LEN);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Affinity"></param>
        /// <returns></returns>
        public static int DefaultLength(CellAffinity Affinity)
        {
            return Length(Affinity, DEFAULT_VARIABLE_LEN);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Affinity"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static int FixLength(CellAffinity Affinity, int Length)
        {

            switch (Affinity)
            {
                case CellAffinity.BOOL:
                    return BOOL_SIZE;
                case CellAffinity.BYTE:
                    return BYTE_SIZE;
                case CellAffinity.SHORT:
                    return SHORT_SIZE;
                case CellAffinity.INT:
                    return INT_SIZE;
                case CellAffinity.SINGLE:
                    return FLOAT_SIZE;
                case CellAffinity.LONG:
                    return LONG_SIZE;
                case CellAffinity.DOUBLE:
                    return DOUBLE_SIZE;
                case CellAffinity.DATE_TIME:
                    return DATE_SIZE;
                case CellAffinity.BINARY:
                    return Math.Min(Length, MAX_BINARY_LEN);
                case CellAffinity.BSTRING:
                    return Math.Min(Length, MAX_BSTRING_LEN);
                case CellAffinity.CSTRING:
                case CellAffinity.TREF:
                case CellAffinity.VARIANT:
                    return Math.Min(Length, MAX_CSTRING_LEN);
                case CellAffinity.ARRAY:
                    return Math.Min(Length, MAX_ARRAY_LEN);
                default:
                    throw new Exception();

            }

        }

    }


}
