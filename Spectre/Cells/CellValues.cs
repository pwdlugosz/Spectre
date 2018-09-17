using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Structures;

namespace Spectre.Cells
{

    /// <summary>
    /// Support for special cell values
    /// </summary>
    public static class CellValues
    {

        public static Cell Null(CellAffinity Affinty)
        {

            if (Affinty == CellAffinity.VARIANT)
                throw new Exception("The variant affinity is invalid");

            switch (Affinty)
            {
                case CellAffinity.BOOL:
                    return NullBOOL;
                case CellAffinity.BYTE:
                    return NullBYTE;
                case CellAffinity.SHORT:
                    return NullSHORT;
                case CellAffinity.INT:
                    return NullINT;
                case CellAffinity.LONG:
                    return NullLONG;
                case CellAffinity.SINGLE:
                    return NullSINGLE;
                case CellAffinity.DOUBLE:
                    return NullDOUBLE;
                case CellAffinity.DATE_TIME:
                    return NullDATE;
                case CellAffinity.BINARY:
                    return NullBLOB;
                case CellAffinity.BSTRING:
                    return NullBSTRING;
                case CellAffinity.CSTRING:
                    return NullCSTRING;
                case CellAffinity.TREF:
                    return NullTREF;
                case CellAffinity.ARRAY:
                    return NullARRAY;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        public static Cell Zero(CellAffinity Affinty)
        {

            if (Affinty == CellAffinity.VARIANT)
                throw new Exception("The variant affinity is invalid");

            switch (Affinty)
            {

                case CellAffinity.BYTE:
                    return ZeroBYTE;
                case CellAffinity.SHORT:
                    return ZeroSHORT;
                case CellAffinity.INT:
                    return ZeroINT;
                case CellAffinity.LONG:
                    return ZeroLONG;
                case CellAffinity.SINGLE:
                    return ZeroSINGLE;
                case CellAffinity.DOUBLE:
                    return ZeroDOUBLE;
                case CellAffinity.DATE_TIME:
                    return ZeroDATE;
                case CellAffinity.BINARY:
                    return ZeroBLOB;
                case CellAffinity.BSTRING:
                    return ZeroBSTRING;
                case CellAffinity.CSTRING:
                    return ZeroCSTRING;
                case CellAffinity.TREF:
                    return ZeroTREF;
                case CellAffinity.ARRAY:
                    return ZeroARRAY;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        public static Cell One(CellAffinity Affinty)
        {

            if (Affinty == CellAffinity.VARIANT)
                throw new Exception("The variant affinity is invalid");

            switch (Affinty)
            {

                case CellAffinity.BYTE:
                    return OneBYTE;
                case CellAffinity.SHORT:
                    return OneSHORT;
                case CellAffinity.INT:
                    return OneINT;
                case CellAffinity.LONG:
                    return OneLONG;
                case CellAffinity.SINGLE:
                    return OneSINGLE;
                case CellAffinity.DOUBLE:
                    return OneDOUBLE;
                case CellAffinity.DATE_TIME:
                    return OneDATE;
                case CellAffinity.BINARY:
                    return OneBLOB;
                case CellAffinity.BSTRING:
                    return OneBSTRING;
                case CellAffinity.CSTRING:
                    return OneCSTRING;
                case CellAffinity.TREF:
                    return OneTREF;
                case CellAffinity.ARRAY:
                    return OneARRAY;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        public static Cell Min(CellAffinity Affinty)
        {

            if (Affinty == CellAffinity.VARIANT)
                throw new Exception("The variant affinity is invalid");

            switch (Affinty)
            {
                case CellAffinity.BOOL:
                    return MinBOOL;
                case CellAffinity.BYTE:
                    return MinBYTE;
                case CellAffinity.SHORT:
                    return MinSHORT;
                case CellAffinity.INT:
                    return MinINT;
                case CellAffinity.LONG:
                    return MinLONG;
                case CellAffinity.SINGLE:
                    return MinSINGLE;
                case CellAffinity.DOUBLE:
                    return MinDOUBLE;
                case CellAffinity.DATE_TIME:
                    return MinDATE;
                case CellAffinity.BINARY:
                    return MinBLOB;
                case CellAffinity.BSTRING:
                    return MinBSTRING;
                case CellAffinity.CSTRING:
                    return MinCSTRING;
                case CellAffinity.TREF:
                    return MinTREF;
                case CellAffinity.ARRAY:
                    return MinARRAY;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        public static Cell Max(CellAffinity Affinty)
        {

            if (Affinty == CellAffinity.VARIANT)
                throw new Exception("The variant affinity is invalid");

            switch (Affinty)
            {

                case CellAffinity.BOOL:
                    return MaxBOOL;
                case CellAffinity.DATE_TIME:
                    return MaxDATE;
                case CellAffinity.BYTE:
                    return MaxBYTE;
                case CellAffinity.SHORT:
                    return MaxSHORT;
                case CellAffinity.INT:
                    return MaxINT;
                case CellAffinity.LONG:
                    return MaxLONG;
                case CellAffinity.SINGLE:
                    return MaxSINGLE;
                case CellAffinity.DOUBLE:
                    return MaxDOUBLE;
                case CellAffinity.BINARY:
                    return MaxBLOB;
                case CellAffinity.BSTRING:
                    return MaxBSTRING;
                case CellAffinity.CSTRING:
                    return MaxCSTRING;
                case CellAffinity.TREF:
                    return MaxTREF;
                case CellAffinity.ARRAY:
                    return MaxARRAY;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        public static Cell Empty(CellAffinity Affinty)
        {

            if (Affinty == CellAffinity.VARIANT)
                throw new Exception("The variant affinity is invalid");

            switch (Affinty)
            {

                case CellAffinity.BYTE:
                    return EmptyBYTE;
                case CellAffinity.SHORT:
                    return EmptySHORT;
                case CellAffinity.INT:
                    return EmptyINT;
                case CellAffinity.LONG:
                    return EmptyLONG;
                case CellAffinity.SINGLE:
                    return EmptySINGLE;
                case CellAffinity.DOUBLE:
                    return EmptyDOUBLE;
                case CellAffinity.DATE_TIME:
                    return EmptyDATE;
                case CellAffinity.BINARY:
                    return EmptyBLOB;
                case CellAffinity.BSTRING:
                    return EmptyBSTRING;
                case CellAffinity.CSTRING:
                    return EmptyCSTRING;
                case CellAffinity.TREF:
                    return EmptyTREF;
                case CellAffinity.ARRAY:
                    return EmptyARRAY;

            }

            throw new Exception(string.Format("Affinity '{0}' is invalid", Affinty));

        }

        // BOOL
        public static readonly Cell NullBOOL = new Cell(CellAffinity.BOOL);
        public static readonly Cell ZeroBOOL = new Cell(false);
        public static readonly Cell OneBOOL = new Cell(true);
        public static readonly Cell MinBOOL = new Cell(false);
        public static readonly Cell MaxBOOL = new Cell(true);
        public static readonly Cell EmptyBOOL = new Cell(false);

        // DATE_TIME
        public static readonly Cell NullDATE = new Cell(CellAffinity.DATE_TIME);
        public static readonly Cell ZeroDATE = new Cell(DateTime.MinValue);
        public static readonly Cell OneDATE = new Cell(DateTime.MaxValue);
        public static readonly Cell MinDATE = new Cell(DateTime.MinValue);
        public static readonly Cell MaxDATE = new Cell(DateTime.MaxValue);
        public static readonly Cell EmptyDATE = new Cell(DateTime.MinValue);

        // BYTE
        public static readonly Cell NullBYTE = new Cell(CellAffinity.BYTE);
        public static readonly Cell ZeroBYTE = new Cell((byte)0);
        public static readonly Cell OneBYTE = new Cell((byte)1);
        public static readonly Cell MinBYTE = new Cell(byte.MinValue);
        public static readonly Cell MaxBYTE = new Cell(byte.MaxValue);
        public static readonly Cell EmptyBYTE = new Cell((byte)0);

        // SHORT
        public static readonly Cell NullSHORT = new Cell(CellAffinity.SHORT);
        public static readonly Cell ZeroSHORT = new Cell((short)0);
        public static readonly Cell OneSHORT = new Cell((short)1);
        public static readonly Cell MinSHORT = new Cell(short.MinValue);
        public static readonly Cell MaxSHORT = new Cell(short.MaxValue);
        public static readonly Cell EmptySHORT = new Cell((short)0);

        // INT
        public static readonly Cell NullINT = new Cell(CellAffinity.INT);
        public static readonly Cell ZeroINT = new Cell((int)0);
        public static readonly Cell OneINT = new Cell((int)1);
        public static readonly Cell MinINT = new Cell(int.MinValue);
        public static readonly Cell MaxINT = new Cell(int.MaxValue);
        public static readonly Cell EmptyINT = new Cell((int)0);

        // LONG
        public static readonly Cell NullLONG = new Cell(CellAffinity.LONG);
        public static readonly Cell ZeroLONG = new Cell((long)0);
        public static readonly Cell OneLONG = new Cell((long)1);
        public static readonly Cell MinLONG = new Cell(long.MinValue);
        public static readonly Cell MaxLONG = new Cell(long.MaxValue);
        public static readonly Cell EmptyLONG = new Cell((long)0);

        // SINGLE
        public static readonly Cell NullSINGLE = new Cell(CellAffinity.SINGLE);
        public static readonly Cell ZeroSINGLE = new Cell((float)0);
        public static readonly Cell OneSINGLE = new Cell((float)1);
        public static readonly Cell MinSINGLE = new Cell(float.MinValue);
        public static readonly Cell MaxSINGLE = new Cell(float.MaxValue);
        public static readonly Cell EmptySINGLE = new Cell((float)0);

        // DOUBLE
        public static readonly Cell NullDOUBLE = new Cell(CellAffinity.DOUBLE);
        public static readonly Cell ZeroDOUBLE = new Cell((double)0);
        public static readonly Cell OneDOUBLE = new Cell((double)1);
        public static readonly Cell MinDOUBLE = new Cell(double.MinValue);
        public static readonly Cell MaxDOUBLE = new Cell(double.MaxValue);
        public static readonly Cell EmptyDOUBLE = new Cell((double)0);

        // BINARY
        public static readonly Cell NullBLOB = new Cell(CellAffinity.BINARY);
        public static readonly Cell ZeroBLOB = new Cell(new byte[1] { 0 });
        public static readonly Cell OneBLOB = new Cell(new byte[1] { 1 });
        public static readonly Cell MinBLOB = new Cell(new byte[0] { });
        public static readonly Cell MaxBLOB = new Cell(System.Text.ASCIIEncoding.UTF8.GetBytes(new string(char.MaxValue, 2048)));
        public static readonly Cell EmptyBLOB = new Cell(new byte[0] { });

        // BSTRING
        public static readonly Cell NullBSTRING = new Cell(CellAffinity.BSTRING);
        public static readonly Cell ZeroBSTRING = new Cell(new BString("ZERO"));
        public static readonly Cell OneBSTRING = new Cell(new BString("ONE"));
        public static readonly Cell MinBSTRING = new Cell(BString.Empty);
        public static readonly Cell MaxBSTRING = new Cell(new BString(255, 4096));
        public static readonly Cell EmptyBSTRING = new Cell(BString.Empty);

        // CSTRING
        public static readonly Cell NullCSTRING = new Cell(CellAffinity.CSTRING);
        public static readonly Cell ZeroCSTRING = new Cell("ZERO");
        public static readonly Cell OneCSTRING = new Cell("ONE");
        public static readonly Cell MinCSTRING = new Cell("");
        public static readonly Cell MaxCSTRING = new Cell(new string(char.MaxValue, 4096));
        public static readonly Cell EmptyCSTRING = new Cell("");

        // TREF
        public static readonly Cell NullTREF = new Cell(CellAffinity.TREF);
        public static readonly Cell ZeroTREF = new Cell(CellAffinity.TREF);
        public static readonly Cell OneTREF = new Cell(CellAffinity.TREF);
        public static readonly Cell MinTREF = new Cell("");
        public static readonly Cell MaxTREF = new Cell(new string(char.MaxValue, 4096));
        public static readonly Cell EmptyTREF = new Cell("");

        // ARRAY
        public static readonly Cell NullARRAY = new Cell(CellAffinity.ARRAY);
        public static readonly Cell ZeroARRAY = new Cell(CellAffinity.ARRAY);
        public static readonly Cell OneARRAY = new Cell(CellAffinity.ARRAY);
        public static readonly Cell MinARRAY = new Cell(CellArray.MinimumArray);
        public static readonly Cell MaxARRAY = new Cell(CellArray.MximumArray);
        public static readonly Cell EmptyARRAY = new Cell(new CellArray());

        // Special Values //
        public static readonly Cell Pi = new Cell(Math.PI);
        public static readonly Cell E = new Cell(Math.E);
        public static readonly Cell Epsilon = new Cell(double.Epsilon);
        public static readonly Cell True = new Cell(true);
        public static readonly Cell False = new Cell(false);


    }


}
