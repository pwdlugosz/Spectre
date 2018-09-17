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

    public sealed class MathLibrary : Library
    {

        /*
         * SIN
         * COS
         * TAN
         * ASIN
         * ACOS
         * ATAN
         * SINH
         * COSH
         * TANH
         * ASINH
         * ACOSH
         * ATANH
         * EXP
         * LOG
         * ROUND
         * ABS
         * SIGN
         * MODPOW
         * DLOG
         * 
         * 
         */
         
        public MathLibrary(Host Host)
            : base(Host, "MATH")
        {

        }

        public override ExpressionFunction ExpressionLookup(string Name)
        {

            switch(Name.ToUpper())
            {
                case "SIN":
                    return new FuncSin(Host, null);
                case "COS":
                    return new FuncCos(Host, null);
                case "TAN":
                    return new FuncTan(Host, null);
                case "ASIN":
                    return new FuncASin(Host, null);
                case "ACOS":
                    return new FuncACos(Host, null);
                case "ATAN":
                    return new FuncATan(Host, null);
                case "SINH":
                    return new FuncSinh(Host, null);
                case "COSH":
                    return new FuncCosh(Host, null);
                case "TANH":
                    return new FuncTanh(Host, null);
                case "ASINH":
                    return new FuncASinh(Host, null);
                case "ACOSH":
                    return new FuncACosh(Host, null);
                case "ATANH":
                    return new FuncATanh(Host, null);
                case "ATAN2":
                    return new FuncATan2(Host, null);
                case "EXP":
                    return new FuncExp(Host, null);
                case "LOG":
                    return new FuncLog(Host, null);
                case "ROUND":
                    return new FuncRound(Host, null);
                case "SIGN":
                    return new FuncSign(Host, null);
                case "ABS":
                    return new FuncAbs(Host, null);
                case "MODPOW":
                    return new FuncModPow(Host, null);
                case "DLOG":
                    return new FuncDLog(Host, null);

            }

            return null;

        }

        public override Statement StatementLookup(string Name)
        {
            throw new Exception(string.Format("Method '{0}' does not exist", Name));
        }

        public sealed class FuncSin : ExpressionFunction
        {

            public FuncSin(Host Host, Expression Parent)
                : base(Host, Parent, "SIN", 1,1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.Sin(x);
            }

        }

        public sealed class FuncCos : ExpressionFunction
        {

            public FuncCos(Host Host, Expression Parent)
                : base(Host, Parent, "COS", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.Cos(x);
            }

        }

        public sealed class FuncTan : ExpressionFunction
        {

            public FuncTan(Host Host, Expression Parent)
                : base(Host, Parent, "TAN", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.Tan(x);
            }

        }

        public sealed class FuncASin : ExpressionFunction
        {

            public FuncASin(Host Host, Expression Parent)
                : base(Host, Parent, "ASIN", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.ArcSin(x);
            }

        }

        public sealed class FuncACos : ExpressionFunction
        {

            public FuncACos(Host Host, Expression Parent)
                : base(Host, Parent, "ACOS", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.ArcCos(x);
            }

        }

        public sealed class FuncATan : ExpressionFunction
        {

            public FuncATan(Host Host, Expression Parent)
                : base(Host, Parent, "ATAN", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.ArcTan(x);
            }

        }

        public sealed class FuncSinh : ExpressionFunction
        {

            public FuncSinh(Host Host, Expression Parent)
                : base(Host, Parent, "SINH", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.Sinh(x);
            }

        }

        public sealed class FuncCosh : ExpressionFunction
        {

            public FuncCosh(Host Host, Expression Parent)
                : base(Host, Parent, "COSH", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.Cosh(x);
            }

        }

        public sealed class FuncTanh : ExpressionFunction
        {

            public FuncTanh(Host Host, Expression Parent)
                : base(Host, Parent, "TANH", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.Tanh(x);
            }

        }

        public sealed class FuncASinh : ExpressionFunction
        {

            public FuncASinh(Host Host, Expression Parent)
                : base(Host, Parent, "ASINH", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.ArcSinh(x);
            }

        }

        public sealed class FuncACosh : ExpressionFunction
        {

            public FuncACosh(Host Host, Expression Parent)
                : base(Host, Parent, "ACOSH", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.ArcCosh(x);
            }

        }

        public sealed class FuncATanh : ExpressionFunction
        {

            public FuncATanh(Host Host, Expression Parent)
                : base(Host, Parent, "ATANH", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.ArcTanh(x);
            }

        }

        public sealed class FuncATan2 : ExpressionFunction
        {

            public FuncATan2(Host Host, Expression Parent)
                : base(Host, Parent, "ATAN2", 2, 2)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                Cell y = this._Children[0].Evaluate(Memory);
                return CellFunctions.ATan2(x, y);
            }

        }
        
        public sealed class FuncExp : ExpressionFunction
        {

            public FuncExp(Host Host, Expression Parent)
                : base(Host, Parent, "EXP", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.Exp(x);
            }

        }

        public sealed class FuncLog : ExpressionFunction
        {

            public FuncLog(Host Host, Expression Parent)
                : base(Host, Parent, "LOG", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.Log(x);
            }

        }

        public sealed class FuncRound : ExpressionFunction
        {

            public FuncRound(Host Host, Expression Parent)
                : base(Host, Parent, "ROUND", 2, 2)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                Cell y = this._Children[1].Evaluate(Memory);
                return CellFunctions.Round(x, y.valueINT);
            }

        }

        public sealed class FuncAbs : ExpressionFunction
        {

            public FuncAbs(Host Host, Expression Parent)
                : base(Host, Parent, "ABS", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.Abs(x);
            }

        }

        public sealed class FuncSign : ExpressionFunction
        {

            public FuncSign(Host Host, Expression Parent)
                : base(Host, Parent, "SIGN", 1, 1)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                return CellFunctions.Sign(x);
            }

        }

        public sealed class FuncModPow : ExpressionFunction
        {

            public FuncModPow(Host Host, Expression Parent)
                : base(Host, Parent, "MODPOW", 3, 3)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                Cell y = this._Children[1].Evaluate(Memory);
                Cell z = this._Children[2].Evaluate(Memory);
                return CellFunctions.ModPow(x, y, z);
            }

        }

        public sealed class FuncDLog : ExpressionFunction
        {

            public FuncDLog(Host Host, Expression Parent)
                : base(Host, Parent, "DLOG", 2, 2)
            {

            }

            public override Cell Evaluate(SpoolSpace Memory)
            {
                this.CheckParameters();
                Cell x = this._Children[0].Evaluate(Memory);
                Cell y = this._Children[1].Evaluate(Memory);
                return CellFunctions.DLog(x, y);
            }

        }


    }



}
