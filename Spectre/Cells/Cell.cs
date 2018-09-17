using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Spectre.Structures;
using Spectre.Tables;
using Spectre.Expressions;

namespace Spectre.Cells
{

    /// <summary>
    /// The basic unit of in memory data
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
    public struct Cell 
    {

        // Cell constants //
        public const string NULL_STRING_TEXT = "@@NULL"; // the null AWValue text
        public const string HEX_LITERARL = "0x"; // the expected qualifier for a hex string 
        public const int MAX_STRING_LENGTH = 64 * 1024 * 1024; // maximum length of a string, 64k

        // Cell internal statics //
        internal static int NUMERIC_ROUNDER = 5; // used for rounding double values 
        internal static int DATE_FORMAT = 1; // 0 = full date time, 1 = date only, 2 = time only
        internal static string TRUE_STRING = "TRUE";
        internal static string FALSE_STRING = "FALSE";
        internal static BString TRUE_BSTRING = "TRUE";
        internal static BString FALSE_BSTRING = "FALSE";

        #region Runtime_Variables

        // Metadata elements //
        /// <summary>
        /// The cell affinity, offset 9
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(9)]
        internal CellAffinity AFFINITY;

        /// <summary>
        /// The null byte indicator, offset 10
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(10)]
        internal byte NULL;

        // Elements variables //
        /// <summary>
        /// The .Net bool AWValue, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal bool BOOL;

        /// <summary>
        /// The .Net DateTime variable, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal DateTime DATE;

        /// <summary>
        /// The .Net short AWValue, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal byte BYTE;

        /// <summary>
        /// The .Net short AWValue, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal short SHORT;

        /// <summary>
        /// The .Net int AWValue, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal int INT;

        /// <summary>
        /// The .Net long AWValue, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal long LONG;

        /// <summary>
        /// The .Net float AWValue, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal float SINGLE;

        /// <summary>
        /// The .Net double AWValue, offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal double DOUBLE;

        /// <summary>
        /// The .Net byte[] variable, offset 12
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(12)]
        internal byte[] BINARY;

        /// <summary>
        /// Represents an immutable 8-bit string
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(12)]
        internal BString BSTRING;

        /// <summary>
        /// The .Net string variable, offset 12
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(12)]
        internal string CSTRING;

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(12)]
        internal CellArray ARRAY;

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(12)]
        internal Expression EQUATION;

        // Extended elements //
        /// <summary>
        /// The .Net integer AWValue at offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal int INT_A;

        /// <summary>
        /// The .Net integer AWValue at offset 4
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(4)]
        internal int INT_B;

        /// <summary>
        /// The .Net float AWValue at offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal float FLOAT_A;

        /// <summary>
        /// The .Net float AWValue at offset 4
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(4)]
        internal float FLOAT_B;

        /// <summary>
        /// The .Net ulong AWValue at offset 0
        /// </summary>
        [System.Runtime.InteropServices.FieldOffset(0)]
        internal ulong ULONG;

        [System.Runtime.InteropServices.FieldOffset(0)]
        internal byte B0;

        [System.Runtime.InteropServices.FieldOffset(1)]
        internal byte B1;

        [System.Runtime.InteropServices.FieldOffset(2)]
        internal byte B2;

        [System.Runtime.InteropServices.FieldOffset(3)]
        internal byte B3;

        [System.Runtime.InteropServices.FieldOffset(4)]
        internal byte B4;

        [System.Runtime.InteropServices.FieldOffset(5)]
        internal byte B5;

        [System.Runtime.InteropServices.FieldOffset(6)]
        internal byte B6;

        [System.Runtime.InteropServices.FieldOffset(7)]
        internal byte B7;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a boolean cell
        /// </summary>
        /// <param name="AWValue">A .Net bool</param>
        public Cell(bool Value)
            : this()
        {
            this.BOOL = Value;
            this.AFFINITY = CellAffinity.BOOL;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 64 bit date-time cell
        /// </summary>
        /// <param name="AWValue">A .Net DateTime</param>
        public Cell(DateTime Value)
            : this()
        {
            this.DATE = Value;
            this.AFFINITY = CellAffinity.DATE_TIME;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 8 bit integer cell
        /// </summary>
        /// <param name="AWValue">A .Net long or int8</param>
        public Cell(byte Value)
            : this()
        {

            this.BYTE = Value;
            this.AFFINITY = CellAffinity.BYTE;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 16 bit integer cell
        /// </summary>
        /// <param name="AWValue">A .Net long or int16</param>
        public Cell(short Value)
            : this()
        {

            this.SHORT = Value;
            this.AFFINITY = CellAffinity.SHORT;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 32 bit integer cell
        /// </summary>
        /// <param name="AWValue">A .Net long or int32</param>
        public Cell(int Value)
            : this()
        {

            this.INT = Value;
            this.AFFINITY = CellAffinity.INT;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 64 bit integer cell
        /// </summary>
        /// <param name="AWValue">A .Net long or int64</param>
        public Cell(long Value)
            : this()
        {

            this.LONG = Value;
            this.AFFINITY = CellAffinity.LONG;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 32 bit numeric cell
        /// </summary>
        /// <param name="AWValue">A .Net double or Double</param>
        public Cell(float Value)
            : this()
        {
            this.SINGLE = Value;
            this.AFFINITY = CellAffinity.SINGLE;
            this.NULL = 0;
        }

        /// <summary>
        /// Creates a 64 bit numeric cell
        /// </summary>
        /// <param name="AWValue">A .Net double or Double</param>
        public Cell(double Value)
            : this()
        {
            this.DOUBLE = Value;
            this.AFFINITY = CellAffinity.DOUBLE;
            this.NULL = 0;
        }

        /// <summary>
        /// Creats a BINARY cell
        /// </summary>
        /// <param name="AWValue">A .Net array of bytes</param>
        public Cell(byte[] Value)
            : this()
        {
            this.BINARY = Value;
            this.NULL = 0;
            this.AFFINITY = CellAffinity.BINARY;
            for (int i = 0; i < Value.Length; i++)
                this.INT_A += Value[i] * i;
            this.INT_A = this.INT_A ^ Value.Length;
            this.INT_B = Value.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AWValue"></param>
        public Cell(BString Value)
            : this()
        {

            this.AFFINITY = CellAffinity.BSTRING;

            // Handle null strings //
            if (Value == null)
            {
                this.BSTRING = BString.Empty;
                this.NULL = 1;
                return;
            }

            // Fix the values
            if (Value.Length == 0) // fix instances that are zero length
                Value = BString.Empty;
            else if (Value.Length >= MAX_STRING_LENGTH) // Fix strings that are too long
                Value = Value.Substring(0, MAX_STRING_LENGTH);

            this.BSTRING = Value;
            this.NULL = 0;

            this.INT_A = Value.GetHashCode();
            this.INT_B = Value.Length;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AWValue"></param>
        public Cell(string Value)
            : this()
        {

            // Set the affinity //
            this.AFFINITY = CellAffinity.CSTRING;

            // Handle null strings //
            if (Value == null)
            {
                this.CSTRING = "\0";
                this.NULL = 1;
                return;
            }

            // Fix the values
            if (Value.Length == 0) // fix instances that are zero length
                Value = "\0";
            else if (Value.Length >= MAX_STRING_LENGTH) // Fix strings that are too long
                Value = Value.Substring(0, MAX_STRING_LENGTH);

            this.CSTRING = Value;
            this.NULL = 0;

            this.INT_A = Value.GetHashCode();
            this.INT_B = Value.Length;

        }

        /// <summary>
        /// Creates a table
        /// </summary>
        /// <param name="Value"></param>
        public Cell(Table Value)
            : this()
        {
            this.AFFINITY = CellAffinity.TREF;
            this.CSTRING = Value.Key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AWValue"></param>
        public Cell(CellArray Value)
            : this()
        {

            // Set the affinity //
            this.AFFINITY = CellAffinity.ARRAY;

            // Handle null strings //
            if (Value == null)
            {
                this.NULL = 1;
                this.ARRAY = new CellArray();
                return;
            }
            this.ARRAY = Value;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public Cell(Expression Value)
            :this()
        {

            this.AFFINITY = CellAffinity.EQUATION;

            // Handle nulls //
            if (Value == null)
            {
                this.NULL = 1;
                this.EQUATION = null;
                return;
            }
            this.EQUATION = Value;

        }

        /// <summary>
        /// Creates a cell of a given affinity that is null
        /// </summary>
        /// <param name="Type">An affinity of the new cell</param>
        public Cell(CellAffinity Type)
            : this()
        {

            if (Type == CellAffinity.VARIANT)
                throw new Exception("Cannot create a null variant cell");

            this.AFFINITY = Type;
            this.NULL = 1;
            if (Type == CellAffinity.CSTRING)
                this.CSTRING = "";
            else if (Type == CellAffinity.BSTRING)
                this.BSTRING = BString.Empty;
            else if (Type == CellAffinity.BINARY)
                this.BINARY = new byte[0];
        }

        // -- Auto Casts -- //
        /// <summary>
        /// Creates a 64 bit integer cell
        /// </summary>
        /// <param name="ValueA">A .Net 32 bit integer that will make up the first 4 bytes of integer</param>
        /// <param name="ValueB"></param>
        internal Cell(int ValueA, int ValueB)
            : this()
        {

            // Set these values //
            this.INT_A = ValueA;
            this.INT_B = ValueB;
            this.AFFINITY = CellAffinity.LONG;
            this.NULL = 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AWValue"></param>
        /// <param name="Affinity"></param>
        internal Cell(ulong Value, CellAffinity Affinity)
            : this()
        {
            this.AFFINITY = Affinity;
            this.ULONG = Value;
            this.NULL = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The cell's Spike affinity
        /// </summary>
        public CellAffinity Affinity
        {
            get { return this.AFFINITY; }
        }

        /// <summary>
        /// True if the cell is numeric, false otherwise
        /// </summary>
        public bool IsNumeric
        {
            get { return CellAffinityHelper.IsNumeric(this.AFFINITY); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsArray
        {
            get { return this.AFFINITY == CellAffinity.ARRAY; }
        }

        /// <summary>
        /// True == null, False == not null
        /// </summary>
        public bool IsNull
        {
            get { return NULL == 1; }
        }

        /// <summary>
        /// True if the numeric AWValue is 0 or if the variable length AWValue has a zero length
        /// </summary>
        public bool IsZero
        {
            get
            {
                if (this.IsNull) return false;
                switch (this.Affinity)
                {
                    case CellAffinity.BYTE:
                    case CellAffinity.SHORT:
                    case CellAffinity.INT:
                    case CellAffinity.LONG: return this.LONG == 0;
                    case CellAffinity.SINGLE:
                    case CellAffinity.DOUBLE: return this.DOUBLE == 0;
                    case CellAffinity.BOOL: return !this.BOOL;
                    case CellAffinity.BSTRING: return this.BSTRING.Length == 0;
                    case CellAffinity.CSTRING: return this.CSTRING.Length == 0;
                    case CellAffinity.BINARY: return this.BINARY.Length == 0;
                    default: return false;
                }
            }

        }

        /// <summary>
        /// Returns true if the integer AWValue or double AWValue is 1, or if the boolean is true, false otherwise
        /// </summary>
        public bool IsOne
        {

            get
            {
                if (this.IsNull) return false;
                switch (this.Affinity)
                {
                    case CellAffinity.BYTE:
                    case CellAffinity.SHORT:
                    case CellAffinity.INT:
                    case CellAffinity.LONG: return this.LONG == 1;
                    case CellAffinity.SINGLE:
                    case CellAffinity.DOUBLE: return this.DOUBLE == 1;
                    case CellAffinity.BOOL: return this.BOOL;
                    default: return false;
                }
            }

        }

        /// <summary>
        /// Returns the length of the cell
        /// </summary>
        public int Length
        {
            get
            {

                int len = 0;
                if (this.AFFINITY == CellAffinity.CSTRING && this.CSTRING != null)
                    len = this.CSTRING.Length;
                if (this.AFFINITY == CellAffinity.BSTRING && this.BSTRING != null)
                    len = this.BSTRING.Length;
                else if (this.AFFINITY == CellAffinity.BINARY && this.BINARY != null)
                    len = this.BINARY.Length;
                else if (this.AFFINITY == CellAffinity.ARRAY && this.ARRAY != null)
                    len = this.ARRAY.Count;
                else if (this.AFFINITY == CellAffinity.EQUATION)
                    len = this.EQUATION == null ? -1 : this.EQUATION.Children.Count;
                return CellSerializer.Length(this.AFFINITY, len);
                    
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public Cell this[int Index, int Length]
        {

            get
            {

                if (this.NULL == 1)
                    return new Cell(this.AFFINITY);
                
                switch (this.AFFINITY)
                {
                    case CellAffinity.ARRAY:
                        if (Length == 1) return this.ARRAY[Index];
                        return this.ARRAY[Index, Length];
                    case CellAffinity.CSTRING:
                        return new Cell(this.CSTRING.Substring(Index, Length));
                    case CellAffinity.BSTRING:
                        return new Cell(this.BSTRING.Substring(Index, Length));
                    case CellAffinity.BINARY:
                        byte[] b = new byte[Length];
                        Array.Copy(this.BINARY, Index, b, 0, Length);
                        return new Cell(b);
                }

                return new Cell(this.AFFINITY);

            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public Cell this[int Index]
        {
            get { return this[Index, 1]; }

            set
            {
                if (this.Affinity != CellAffinity.ARRAY)
                    throw new Exception("Can only assign arrays");
            }
        }

        #endregion

        #region SafeValues

        /// <summary>
        ///
        /// </summary>
        public bool valueBOOL
        {
            get
            {
                return CellConverter.ConvertBool(this);
            }
        }

        /// <summary>
        /// Returns the Spike DATE_TIME if the affinity is DATE_TIME, otherwise return the minimum date time .Net AWValue
        /// </summary>
        public DateTime valueDATE_TIME
        {
            get
            {
                return CellConverter.ConvertDateTime(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte valueBYTE
        {
            get
            {
                return CellConverter.ConvertByte(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public short valueSHORT
        {
            get
            {
                return CellConverter.ConvertShort(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int valueINT
        {
            get
            {
                return CellConverter.ConvertInt(this);
            }
        }

        /// <summary>
        /// Return the LONG AWValue if the affinity is LONG, casts the DOUBLE as an LONG if the affinity is a DOUBLE, 0 otherwise
        /// </summary>
        public long valueLONG
        {
            get
            {
                return CellConverter.ConvertLong(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float valueSINGLE
        {
            get
            {
                return CellConverter.ConvertSingle(this);
            }
        }

        /// <summary>
        /// Return the DOUBLE AWValue if the affinity is DOUBLE, casts the LONG as an DOUBLE if the affinity is a LONG, 0 otherwise
        /// </summary>
        public double valueDOUBLE
        {
            get
            {
                return CellConverter.ConvertDouble(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BString valueBSTRING
        {
            get
            {
                return CellConverter.ConvertBString(this);
            }
        }

        /// <summary>
        /// If the cell is null, returns '@@NULL'; otherwise, casts the AWValue as a string
        /// </summary>
        public string valueCSTRING
        {
            get
            {
                return CellConverter.ConvertCString(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string valueTREF
        {
            get
            {
                if (this.CSTRING == null)
                    return null;
                return this.CSTRING;
            }
        }

        /// <summary>
        /// If the affinity is null, returns an empty byte array; if the AWValue is a BINARY, returns the BINARY; if the AWValue is a stirng, returns the string as a byte array, unless the string has a hex prefix, then it converts the hex string to a byte array; otherwise it converts an LONG, DOUBLE, BOOL to a byte array.
        /// </summary>
        public byte[] valueBINARY
        {
            get
            {
                return CellConverter.ConvertBinary(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CellArray valueARRAY
        {
            get
            {
                return CellConverter.ConvertArray(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Expression valueEQUATION
        {
            get
            {
                return CellConverter.ConvertExpression(this);
            }
        }

        /// <summary>
        /// Gets the cell value as an object
        /// </summary>
        public object valueObject
        {

            get
            {
                return CellConverter.ConvertObject(this);
            }

        }

        #endregion

        #region Overrides

        /// <summary>
        /// Returns the valueString property
        /// </summary>
        /// <returns>A string reprsentation of a cell</returns>
        public override string ToString()
        {

            // Check if null //
            if (this.IsNull == true) return Cell.NULL_STRING_TEXT;

            return this.valueCSTRING;

        }

        /// <summary>
        /// Casts an object as a cell then compares it to the Spike instance
        /// </summary>
        /// <param name="obj">The object being tested for equality</param>
        /// <returns>A boolean indicating both objects are equal</returns>
        public override bool Equals(object obj)
        {
            return CellComparer.Compare(this, (Cell)obj) == 0;
        }

        /// <summary>
        /// If null, return int.MinValue, for LONG, DOUBLE, BOOL, and DATE_TIME, return INT_A; for blobs, returns the sum of all bytes; for strings, returns the sum of the (i + 1) OriginalPage char[i]
        /// </summary>
        /// <returns>An integer hash code</returns>
        public override int GetHashCode()
        {

            if (this.NULL == 1)
                return int.MinValue;

            switch(this.Affinity)
            {
                case CellAffinity.BOOL:
                    return this.BOOL.GetHashCode();
                case CellAffinity.BYTE:
                    return (int)this.BYTE;
                case CellAffinity.SHORT:
                    return (short)this.SHORT;
                case CellAffinity.INT:
                case CellAffinity.SINGLE:
                    return this.INT;
                case CellAffinity.LONG:
                case CellAffinity.DOUBLE:
                case CellAffinity.DATE_TIME:
                    return (int)this.LONG ^ (int)(this.LONG >> 32);
                case CellAffinity.BINARY:
                    int a = 0;
                    for (int i = 0; i < this.BINARY.Length; i++)
                    {
                        a += (1 + i) * this.BINARY[i];
                        a ^= a % 17;
                    }
                    return a;
                case CellAffinity.BSTRING:
                    int b = 0, v = 0;
                    foreach(byte c in this.BSTRING.ToByteArray)
                    {
                        b += (1 + v) * c;
                        b ^= b % 37;
                        v++;
                    }
                    return b;
                case CellAffinity.TREF:
                case CellAffinity.CSTRING:
                    int j = 0, k = 0;
                    foreach(char x in this.CSTRING)
                    {
                        j += (1 + k) * (int)x;
                        j ^= j % 53;
                    }
                    return j;
                case CellAffinity.ARRAY:
                    int n = 0;
                    foreach(Cell c in this.ARRAY)
                    {
                        n += c.GetHashCode();
                    }
                    n += this.Length * this.Length ^ int.MaxValue;
                    return n;
            }

            throw new Exception();

        }

        #endregion

        #region Operators

        /// <summary>
        /// Performs the 'NOT' opperation, will return for null for DATE_TIME, CSTRING, and BLOBs
        /// </summary>
        /// <param name="C">A cell</param>
        /// <returns>A cell</returns>
        public static Cell operator !(Cell C)
        {
            return CellOperations.Not(C);
        }

        /// <summary>
        /// Adds two cells together for LONG and DOUBLE, concatentates strings, returns null otherwise
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">AWValue cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator +(Cell C1, Cell C2)
        {
            return CellOperations.Add(C1, C2);
        }

        /// <summary>
        /// Converts either an LONG or DOUBLE to a positve AWValue, returns the cell passed otherwise
        /// </summary>
        /// <param name="C">A cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator +(Cell C)
        {
            return C;
        }

        /// <summary>
        /// Adds one to the given cell for an LONG or DOUBLE, returns the cell passed otherwise
        /// </summary>
        /// <param name="C">The cell argument</param>
        /// <returns>Cell result</returns>
        public static Cell operator ++(Cell C)
        {
            return CellOperations.AutoIncrement(C);
        }

        /// <summary>
        /// Subtracts two cells together for LONG and DOUBLE, repalces instances of C2 in C1, for date times, return the tick count difference as an LONG
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">AWValue cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator -(Cell C1, Cell C2)
        {
            return CellOperations.Substract(C1, C2);
        }

        /// <summary>
        /// Converts either an LONG or DOUBLE to a negative AWValue, returns the cell passed otherwise
        /// </summary>
        /// <param name="C">A cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator -(Cell C)
        {
            return CellOperations.Minus(C);
        }

        /// <summary>
        /// Subtracts one to the given cell for an LONG or DOUBLE, returns the cell passed otherwise
        /// </summary>
        /// <param name="C">The cell argument</param>
        /// <returns>Cell result</returns>
        public static Cell operator --(Cell C)
        {
            return CellOperations.AutoDecrement(C);
        }

        /// <summary>
        /// Multiplies two cells together for LONG and DOUBLE; if C1 is a string and C2 is either int/double, repeats the string C2 times; 
        /// otherwise, returns the cell passed otherwise
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">AWValue cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator *(Cell C1, Cell C2)
        {
            return CellOperations.Multiply(C1, C2);
        }

        /// <summary>
        /// Divides two cells together for LONG and DOUBLE, returns the cell passed otherwise as null
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">AWValue cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator /(Cell C1, Cell C2)
        {
            return CellOperations.Divide(C1, C2);
        }

        /// <summary>
        /// Perform modulo between two cells together for LONG and DOUBLE, returns the cell passed otherwise
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">AWValue cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator %(Cell C1, Cell C2)
        {
            return CellOperations.Mod(C1, C2);
        }

        /// <summary>
        /// Return the bitwise AND for all types
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">AWValue cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator &(Cell C1, Cell C2)
        {
            return CellOperations.And(C1, C2);
        }

        /// <summary>
        /// Returns the bitwise OR for all types
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">AWValue cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator |(Cell C1, Cell C2)
        {
            return CellOperations.Or(C1, C2);
        }

        /// <summary>
        /// Returns the bitwise XOR for all types
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">AWValue cell</param>
        /// <returns>Cell result</returns>
        public static Cell operator ^(Cell C1, Cell C2)
        {
            return CellOperations.Xor(C1, C2);
        }

        /// <summary>
        /// Checks if two cells are equal
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">AWValue cell</param>
        /// <returns>A boolean</returns>
        public static bool operator ==(Cell C1, Cell C2)
        {
            if (C1.NULL == 1 && C2.NULL == 1) return true;
            return CellComparer.Compare(C1, C2) == 0;
        }

        /// <summary>
        /// Checks if two cells are not equal
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">AWValue cell</param>
        /// <returns>A boolean</returns>
        public static bool operator !=(Cell C1, Cell C2)
        {
            if (C1.NULL == 1 && C2.NULL == 1) return true;
            return CellComparer.Compare(C1, C2) != 0;
        }

        /// <summary>
        /// Checks if C1 is less than C2
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">AWValue cell</param>
        /// <returns>A boolean</returns>
        public static bool operator <(Cell C1, Cell C2)
        {
            return CellComparer.Compare(C1, C2) < 0;
        }

        /// <summary>
        /// Checks if C1 is less than or equal to C2
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">AWValue cell</param>
        /// <returns>A boolean</returns>
        public static bool operator <=(Cell C1, Cell C2)
        {
            return CellComparer.Compare(C1, C2) <= 0;
        }

        /// <summary>
        /// Checks if C1 is greater than C2
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">AWValue cell</param>
        /// <returns>A boolean</returns>
        public static bool operator >(Cell C1, Cell C2)
        {
            return CellComparer.Compare(C1, C2) > 0;
        }

        /// <summary>
        /// Checks if C1 is greater than or equal to C2
        /// </summary>
        /// <param name="C1">Left cell</param>
        /// <param name="C2">AWValue cell</param>
        /// <returns>A boolean</returns>
        public static bool operator >=(Cell C1, Cell C2)
        {
            return CellComparer.Compare(C1, C2) >= 0;
        }

        /// <summary>
        /// Determines whether or not a cell is 'TRUE'; if the cell is not null it returns the boolean AWValue
        /// </summary>
        /// <param name="C">The cell AWValue</param>
        /// <returns></returns>
        public static bool operator true(Cell C)
        {
            return C.NULL == 0 && C.BOOL;
        }

        /// <summary>
        /// Determines whether or not a cell is 'FALSE'; if the cell is null or the BOOL AWValue is false, returns false
        /// </summary>
        /// <param name="C">The cell AWValue</param>
        /// <returns></returns>
        public static bool operator false(Cell C)
        {
            return !(C.NULL == 0 && C.BOOL);
        }

        #endregion

        #region ImplicitConversion

        public static implicit operator bool(Cell C)
        {
            return C.valueBOOL;
        }

        public static implicit operator Cell(bool Value)
        {
            return new Cell(Value);
        }

        public static implicit operator DateTime(Cell C)
        {
            return C.valueDATE_TIME;
        }

        public static implicit operator Cell(DateTime Value)
        {
            return new Cell(Value);
        }

        public static implicit operator byte(Cell C)
        {
            return C.valueBYTE;
        }

        public static implicit operator Cell(byte Value)
        {
            return new Cell(Value);
        }

        public static implicit operator short(Cell C)
        {
            return C.valueSHORT;
        }

        public static implicit operator Cell(short Value)
        {
            return new Cell(Value);
        }

        public static implicit operator int(Cell C)
        {
            return C.valueINT;
        }

        public static implicit operator Cell(int Value)
        {
            return new Cell(Value);
        }

        public static implicit operator long(Cell C)
        {
            return C.valueLONG;
        }

        public static implicit operator Cell(long Value)
        {
            return new Cell(Value);
        }

        public static implicit operator float(Cell C)
        {
            return C.valueSINGLE;
        }

        public static implicit operator Cell(float Value)
        {
            return new Cell(Value);
        }

        public static implicit operator double(Cell C)
        {
            return C.valueDOUBLE;
        }

        public static implicit operator Cell(double Value)
        {
            return new Cell(Value);
        }

        public static implicit operator byte[](Cell C)
        {
            return C.valueBINARY;
        }

        public static implicit operator Cell(byte[] Value)
        {
            return new Cell(Value);
        }

        public static implicit operator BString(Cell C)
        {
            return C.valueBSTRING;
        }

        public static implicit operator Cell(BString Value)
        {
            return new Cell(Value);
        }

        public static implicit operator string(Cell C)
        {
            return C.valueCSTRING;
        }

        public static implicit operator Cell(string Value)
        {
            return new Cell(Value);
        }

        public static implicit operator CellArray(Cell C)
        {
            return C.valueARRAY;
        }

        public static implicit operator Cell(CellArray Value)
        {
            return new Cell(Value);
        }

        #endregion

    }

}
