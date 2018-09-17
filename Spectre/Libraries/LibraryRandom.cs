using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Cells;
using Spectre.Control;
using Spectre.Expressions;
using Spectre.Statements;
using Spectre.Structures;

namespace Spectre.Libraries
{

    public sealed class LibraryRandom : Library
    {

        private CellRandom _Source;

        public LibraryRandom(Host Host)
            : base(Host, "RANDOM")
        {
            this._Source = Host.BaseRNG;
        }

        public override ExpressionFunction ExpressionLookup(string Name)
        {
            
            switch(Name.ToUpper())
            {

                case "RANDBOOL":
                    return new RandBool(this._Host, this._Source);
                case "RANDDATE":
                    return new RandDate(this._Host, this._Source);
                case "RANDBYTE":
                    return new RandByte(this._Host, this._Source);
                case "RANDSHORT":
                    return new RandShort(this._Host, this._Source);
                case "RANDINT":
                    return new RandInt(this._Host, this._Source);
                case "RANDLONG":
                    return new RandLong(this._Host, this._Source);
                case "RANDSINGLE":
                    return new RandSingle(this._Host, this._Source);
                case "RANDDOUBLE":
                    return new RandDouble(this._Host, this._Source);
                case "RANDBINARY":
                    return new RandBinary(this._Host, this._Source);
                case "RANDBSTRING":
                    return new RandBString(this._Host, this._Source);
                case "RANDCSTRING":
                    return new RandCString(this._Host, this._Source);
                default:
                    return null;
            }

        }

        public override Statement StatementLookup(string Name)
        {
            return null;
        }

        public sealed class RandBool : ExpressionFunction
        {

            private CellRandom _Source;

            public RandBool(Host Host, CellRandom Source)
                :base(Host, null, "RANDBOOL", 0,1)
            {
                this._Source = Source;
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.BOOL;
            }

            public override int SizeOf()
            {
                return CellSerializer.BOOL_SIZE;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();

                double pr = this._Children.Count == 1 ? this._Children[0].Evaluate(Memory).valueDOUBLE : 0.5;

                return this._Source.NextBool(pr);

            }

        }

        public sealed class RandDate : ExpressionFunction
        {

            private CellRandom _Source;

            public RandDate(Host Host, CellRandom Source)
                : base(Host, null, "RANDDATE", 0, 2)
            {
                this._Source = Source;
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.DATE_TIME;
            }

            public override int SizeOf()
            {
                return CellSerializer.DATE_SIZE;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();

                DateTime min = DateTime.MinValue;
                if (this._Children.Count == 2) min = this._Children[0].Evaluate(Memory).valueDATE_TIME;

                DateTime max = DateTime.MaxValue;
                if (this._Children.Count == 2) max = this._Children[1].Evaluate(Memory).valueDATE_TIME;
                if (this._Children.Count == 1) max = this._Children[0].Evaluate(Memory).valueDATE_TIME;

                return this._Source.NextDate(min, max);

            }

        }

        public sealed class RandByte : ExpressionFunction
        {

            private CellRandom _Source;

            public RandByte(Host Host, CellRandom Source)
                : base(Host, null, "RANDBYTE", 0, 2)
            {
                this._Source = Source;
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.BYTE;
            }

            public override int SizeOf()
            {
                return CellSerializer.BYTE_SIZE;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();

                byte min = 0;
                if (this._Children.Count == 2) min = this._Children[0].Evaluate(Memory).valueBYTE;

                byte max = byte.MaxValue;
                if (this._Children.Count == 2) max = this._Children[1].Evaluate(Memory).valueBYTE;
                if (this._Children.Count == 1) max = this._Children[0].Evaluate(Memory).valueBYTE;

                return this._Source.NextByte(min, max);

            }

        }

        public sealed class RandShort : ExpressionFunction
        {

            private CellRandom _Source;

            public RandShort(Host Host, CellRandom Source)
                : base(Host, null, "RANDSHORT", 0, 2)
            {
                this._Source = Source;
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.SHORT;
            }

            public override int SizeOf()
            {
                return CellSerializer.SHORT_SIZE;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();

                short min = 0;
                if (this._Children.Count == 2) min = this._Children[0].Evaluate(Memory).valueSHORT;

                short max = short.MaxValue;
                if (this._Children.Count == 2) max = this._Children[1].Evaluate(Memory).valueSHORT;
                if (this._Children.Count == 1) max = this._Children[0].Evaluate(Memory).valueSHORT;

                return this._Source.NextShort(min, max);

            }

        }

        public sealed class RandInt : ExpressionFunction
        {

            private CellRandom _Source;

            public RandInt(Host Host, CellRandom Source)
                : base(Host, null, "RANDINT", 0, 2)
            {
                this._Source = Source;
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.INT;
            }

            public override int SizeOf()
            {
                return CellSerializer.INT_SIZE;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();

                int min = 0;
                if (this._Children.Count == 2) min = this._Children[0].Evaluate(Memory).valueINT;

                int max = int.MaxValue;
                if (this._Children.Count == 2) max = this._Children[1].Evaluate(Memory).valueINT;
                if (this._Children.Count == 1) max = this._Children[0].Evaluate(Memory).valueINT;

                return this._Source.NextInt(min, max);

            }

        }

        public sealed class RandLong : ExpressionFunction
        {

            private CellRandom _Source;

            public RandLong(Host Host, CellRandom Source)
                : base(Host, null, "RANDLONG", 0, 2)
            {
                this._Source = Source;
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.LONG;
            }

            public override int SizeOf()
            {
                return CellSerializer.LONG_SIZE;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();

                long min = 0;
                if (this._Children.Count == 2) min = this._Children[0].Evaluate(Memory).valueLONG;

                long max = long.MaxValue;
                if (this._Children.Count == 2) max = this._Children[1].Evaluate(Memory).valueLONG;
                if (this._Children.Count == 1) max = this._Children[0].Evaluate(Memory).valueLONG;

                return this._Source.NextLong(min, max);

            }

        }

        public sealed class RandSingle : ExpressionFunction
        {

            private CellRandom _Source;

            public RandSingle(Host Host, CellRandom Source)
                : base(Host, null, "RANDSINGLE", 0, 2)
            {
                this._Source = Source;
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.SINGLE;
            }

            public override int SizeOf()
            {
                return CellSerializer.FLOAT_SIZE;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();

                Single min = 0;
                if (this._Children.Count == 2) min = this._Children[0].Evaluate(Memory).valueSINGLE;

                Single max = Single.MaxValue;
                if (this._Children.Count == 2) max = this._Children[1].Evaluate(Memory).valueSINGLE;
                if (this._Children.Count == 1) max = this._Children[0].Evaluate(Memory).valueSINGLE;

                return this._Source.NextSingle(min, max);

            }

        }

        public sealed class RandDouble : ExpressionFunction
        {

            private CellRandom _Source;

            public RandDouble(Host Host, CellRandom Source)
                : base(Host, null, "RANDDOUBLE", 0, 2)
            {
                this._Source = Source;
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.DOUBLE;
            }

            public override int SizeOf()
            {
                return CellSerializer.DOUBLE_SIZE;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();

                double min = 0;
                if (this._Children.Count == 2) min = this._Children[0].Evaluate(Memory).valueDOUBLE;

                double max = Single.MaxValue;
                if (this._Children.Count == 2) max = this._Children[1].Evaluate(Memory).valueDOUBLE;
                if (this._Children.Count == 1) max = this._Children[0].Evaluate(Memory).valueDOUBLE;

                return this._Source.NextDouble(min, max);

            }

        }

        public sealed class RandBinary : ExpressionFunction
        {

            private CellRandom _Source;

            public RandBinary(Host Host, CellRandom Source)
                : base(Host, null, "RANDBINARY", 1, 2)
            {
                this._Source = Source;
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.BINARY;
            }

            public override int SizeOf()
            {
                return CellSerializer.DEFAULT_VARIABLE_LEN;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();

                int len = this._Children[0].Evaluate(Memory).valueINT;
                if (this._Children.Count == 1)
                    return this._Source.NextBinary(len);

                byte[] corpus = this._Children[1].Evaluate(Memory).valueBINARY;

                return this._Source.NextBinary(len, corpus);

            }

        }

        public sealed class RandBString : ExpressionFunction
        {

            private CellRandom _Source;

            public RandBString(Host Host, CellRandom Source)
                : base(Host, null, "RANDBSTRING", 1, 2)
            {
                this._Source = Source;
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.BSTRING;
            }

            public override int SizeOf()
            {
                return CellSerializer.DEFAULT_VARIABLE_LEN;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();

                int len = this._Children[0].Evaluate(Memory).valueINT;
                if (this._Children.Count == 1)
                    return this._Source.NextBString(len);

                BString corpus = this._Children[1].Evaluate(Memory).valueBSTRING;

                return this._Source.NextBString(len, corpus);

            }

        }

        public sealed class RandCString : ExpressionFunction
        {

            private CellRandom _Source;

            public RandCString(Host Host, CellRandom Source)
                : base(Host, null, "RANDCSTRING", 1, 2)
            {
                this._Source = Source;
            }

            public override CellAffinity TypeOf()
            {
                return CellAffinity.CSTRING;
            }

            public override int SizeOf()
            {
                return CellSerializer.DEFAULT_VARIABLE_LEN;
            }

            public override Cell Evaluate(SpoolSpace Memory)
            {

                this.CheckParameters();

                int len = this._Children[0].Evaluate(Memory).valueINT;
                if (this._Children.Count == 1)
                    return this._Source.NextBString(len);

                string corpus = this._Children[1].Evaluate(Memory).valueCSTRING;

                return this._Source.NextCString(len, corpus);

            }

        }


        // Rand()
        // RandBool()
        // RandDate()
        // RandByte()
        // RandShort()
        // RandInt()
        // RandLong()
        // RandSingle()
        // RandDouble()
        // RandBinary()
        // RandBString()
        // RandCString()
        // RandArray()

    }

}
