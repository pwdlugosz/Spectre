using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Cells;
using Spectre.Expressions;
using Spectre.Control;
using Spectre.Structures;
using Spectre.Tables;
using Spectre.Libraries;
using Antlr4.Runtime.Misc;
//using Antlr4.Runtime;
//using Antlr4.Runtime.Tree;

namespace Spectre.Scripting
{

    public sealed class ExpressionVisitor : S_ScriptBaseVisitor<Expression>
    {

        private Host _Host;
        private Expression _Master;
        private SpoolSpaceContext _Context;

        public ExpressionVisitor(Host Host)
            : base()
        {
            this._Host = Host;
            this._Context = new SpoolSpaceContext();
            this._Context.AddSpool(Host.GLOBAL);
        }

        public Expression Master
        {
            get { return this._Master; }
            set { this._Master = value; }
        }

        public SpoolSpaceContext Context
        {
            get { return this._Context; }
            set { this._Context = value; }
        }
        
        public override Expression VisitEXPR_Uniary([NotNull] S_ScriptParser.EXPR_UniaryContext context)
        {

            Expression x = this.Visit(context.expression());
            Expression y = x;
            if (context.MINUS() != null)
            {
                y = new Expression.Unary.Minus(this._Host, this._Master, x);
            }
            else if (context.NOT() != null)
            {
                y = new Expression.Unary.Not(this._Host, this._Master, x);
            }
            else if (context.QUESTION() != null)
            {
                y = new Expression.Unary.Question(this._Host, this._Master, x);
            }

            this._Master = y;

            return y;

        }

        public override Expression VisitEXPR_Power([NotNull] S_ScriptParser.EXPR_PowerContext context)
        {
            Expression x = this.Visit(context.expression()[0]);
            Expression y = this.Visit(context.expression()[1]);
            Expression z = new Expression.Binary.Power(this._Host, this._Master, x, y);
            this._Master = z;
            return this._Master;
        }

        public override Expression VisitEXPR_MultDivMod([NotNull] S_ScriptParser.EXPR_MultDivModContext context)
        {
            Expression x = this.Visit(context.expression()[0]);
            Expression y = this.Visit(context.expression()[1]);
            Expression z = null;
            if (context.MUL() != null)
            {
                z = new Expression.Binary.Multiply(this._Host, this._Master, x, y);
            }
            else if (context.DIV() != null)
            {
                z = new Expression.Binary.Divide(this._Host, this._Master, x, y);
            }
            else if (context.DIV2() != null)
            {
                z = new Expression.Binary.CheckDivide(this._Host, this._Master, x, y);
            }
            else if (context.MOD() != null)
            {
                z = new Expression.Binary.Mod(this._Host, this._Master, x, y);
            }
            else if (context.MOD2() != null)
            {
                z = new Expression.Binary.CheckDivide(this._Host, this._Master, x, y);
            }
            this._Master = z;
            return this._Master;
        }

        public override Expression VisitEXPR_AddSub([NotNull] S_ScriptParser.EXPR_AddSubContext context)
        {
            Expression x = this.Visit(context.expression()[0]);
            Expression y = this.Visit(context.expression()[1]);
            Expression z = null;
            if (context.PLUS() != null)
            {
                z = new Expression.Binary.Add(this._Host, this._Master, x, y);
            }
            else if (context.MINUS() != null)
            {
                z = new Expression.Binary.Subtract(this._Host, this._Master, x, y);
            }
            this._Master = z;
            return this._Master;
        }

        public override Expression VisitEXPR_Equality([NotNull] S_ScriptParser.EXPR_EqualityContext context)
        {
            Expression x = this.Visit(context.expression()[0]);
            Expression y = this.Visit(context.expression()[1]);
            Expression z = null;
            if (context.EQ() != null)
            {
                z = new Expression.Binary.Equals(this._Host, this._Master, x, y);
            }
            else if (context.SEQ() != null)
            {
                z = new Expression.Binary.EqualsStrict(this._Host, this._Master, x, y);
            }
            else if (context.NEQ() != null)
            {
                z = new Expression.Binary.NotEquals(this._Host, this._Master, x, y);
            }
            else if (context.SNEQ() != null)
            {
                z = new Expression.Binary.NotEqualsStrict(this._Host, this._Master, x, y);
            }
            this._Master = z;
            return this._Master;
        }

        public override Expression VisitEXPR_GreaterLesser([NotNull] S_ScriptParser.EXPR_GreaterLesserContext context)
        {
            Expression x = this.Visit(context.expression()[0]);
            Expression y = this.Visit(context.expression()[1]);
            Expression z = null;
            if (context.GT() != null)
            {
                z = new Expression.Binary.GreaterThan(this._Host, this._Master, x, y);
            }
            else if (context.GTE() != null)
            {
                z = new Expression.Binary.GreaterThanEquals(this._Host, this._Master, x, y);
            }
            else if (context.LT() != null)
            {
                z = new Expression.Binary.LessThan(this._Host, this._Master, x, y);
            }
            else if (context.LTE() != null)
            {
                z = new Expression.Binary.LessThanEquals(this._Host, this._Master, x, y);
            }
            this._Master = z;
            return this._Master;
        }

        public override Expression VisitEXPR_LogicalAnd([NotNull] S_ScriptParser.EXPR_LogicalAndContext context)
        {
            Expression x = this.Visit(context.expression()[0]);
            Expression y = this.Visit(context.expression()[1]);
            Expression z = new Expression.Binary.And(this._Host, this._Master, x, y);
            this._Master = z;
            return this._Master;
        }

        public override Expression VisitEXPR_LogicalOr([NotNull] S_ScriptParser.EXPR_LogicalOrContext context)
        {
            Expression x = this.Visit(context.expression()[0]);
            Expression y = this.Visit(context.expression()[1]);
            Expression z = null;
            if (context.OR() != null)
            {
                z = new Expression.Binary.Or(this._Host, this._Master, x, y);
            }
            else if (context.XOR() != null)
            {
                z = new Expression.Binary.Xor(this._Host, this._Master, x, y);
            }
            this._Master = z;
            return this._Master;
        }

        public override Expression VisitEXPR_BitShiftRotate([NotNull] S_ScriptParser.EXPR_BitShiftRotateContext context)
        {
            Expression x = this.Visit(context.expression()[0]);
            Expression y = this.Visit(context.expression()[1]);
            Expression z = null;
            if (context.R_SHIFT() != null)
            {
                z = new Expression.Binary.RightShift(this._Host, this._Master, x, y);
            }
            else if (context.R_ROTATE() != null)
            {
                z = new Expression.Binary.RightRotate(this._Host, this._Master, x, y);
            }
            else if (context.L_SHIFT() != null)
            {
                z = new Expression.Binary.LeftShift(this._Host, this._Master, x, y);
            }
            else if (context.L_ROTATE() != null)
            {
                z = new Expression.Binary.LeftRotate(this._Host, this._Master, x, y);
            }
            this._Master = z;
            return this._Master;
        }

        public override Expression VisitEXPR_Indexer([NotNull] S_ScriptParser.EXPR_IndexerContext context)
        {
            Expression x = this.Visit(context.expression()[0]);
            Expression idx = this.Visit(context.expression()[1]);
            Expression y = new Expression.Indexer(this._Host, this._Master, x, idx);
            this._Master = y;
            return y;
        }

        public override Expression VisitEXPR_ExpressionType([NotNull] S_ScriptParser.EXPR_ExpressionTypeContext context)
        {

            CellAffinity a = CellAffinity.VARIANT;

            if (context.type().T_BOOL() != null)
                a = CellAffinity.BOOL;
            else if (context.type().T_DATE() != null)
                a = CellAffinity.DATE_TIME;
            else if (context.type().T_BYTE() != null)
                a = CellAffinity.BYTE;
            else if (context.type().T_SHORT() != null)
                a = CellAffinity.SHORT;
            else if (context.type().T_INT() != null)
                a = CellAffinity.INT;
            else if (context.type().T_LONG() != null)
                a = CellAffinity.LONG;
            else if (context.type().T_SINGLE() != null)
                a = CellAffinity.SINGLE;
            else if (context.type().T_DOUBLE() != null)
                a = CellAffinity.DOUBLE;
            else if (context.type().T_BINARY() != null)
                a = CellAffinity.BINARY;
            else if (context.type().T_BSTRING() != null)
                a = CellAffinity.BSTRING;
            else if (context.type().T_CSTRING() != null)
                a = CellAffinity.CSTRING;
            else if (context.type().T_TABLE() != null)
                a = CellAffinity.TREF;
            else if (context.type().T_ARRAY() != null)
                a = CellAffinity.ARRAY;

            return new Expression.Literal(this._Host, this._Master, new Cell((int)a));

        }

        public override Expression VisitEXPR_Literal([NotNull] S_ScriptParser.EXPR_LiteralContext context)
        {

            string s = context.sliteral().GetText();

            Cell x = CellValues.NullINT;

            if (context.sliteral().LITERAL_BOOL() != null)
                x = CellParser.ParseBOOL(s);
            else if (context.sliteral().LITERAL_DATE_TIME() != null)
                x = CellParser.ParseDATE(s);
            else if (context.sliteral().LITERAL_BYTE() != null)
                x = CellParser.ParseBYTE(s);
            else if (context.sliteral().LITERAL_SHORT() != null)
                x = CellParser.ParseSHORT(s);
            else if (context.sliteral().LITERAL_INT() != null)
                x = CellParser.ParseINT(s);
            else if (context.sliteral().LITERAL_LONG() != null)
                x = CellParser.ParseLONG(s);
            else if (context.sliteral().LITERAL_SINGLE() != null)
                x = CellParser.ParseSINGLE(s);
            else if (context.sliteral().LITERAL_DOUBLE() != null)
                x = CellParser.ParseDOUBLE(s);
            else if (context.sliteral().LITERAL_BINARY() != null)
                x = CellParser.ParseBINARY(s);
            else if (context.sliteral().LITERAL_BSTRING() != null)
                x = CellParser.ParseBSTRING(s);
            else if (context.sliteral().LITERAL_CSTRING() != null)
                x = CellParser.ParseCSTRING(s);

            return new Expression.Literal(this._Host, this._Master, x);



        }

        public override Expression VisitEXPR_ArrayLiteral([NotNull] S_ScriptParser.EXPR_ArrayLiteralContext context)
        {
            Expression x = new Expression.ArrayLiteral(this._Host, this._Master);
            foreach (S_ScriptParser.ExpressionContext y in context.expression())
            {
                Expression z = this.Visit(y);
                x.AddChild(z);
            }
            this._Master = x;
            return x;
        }

        public override Expression VisitEXPR_WildCard([NotNull] S_ScriptParser.EXPR_WildCardContext context)
        {

            string LibName = context.unit_name().GetText();
            Expression x = new Expression.ArrayWildCard(this._Host, this._Master, LibName);
            return x;

        }

        public override Expression VisitEXPR_IfNull([NotNull] S_ScriptParser.EXPR_IfNullContext context)
        {
            Expression x = this.Visit(context.expression()[0]);
            Expression y = this.Visit(context.expression()[1]);
            Expression z = null;
            if (context.NULLIF() != null)
            {
                z = new Expression.Binary.NullIf(this._Host, this._Master, x, y);
            }
            else if (context.IFNULL() != null)
            {
                z = new Expression.Binary.IfNull(this._Host, this._Master, x, y);
            }
            else
            {
                throw new Exception();
            }
            this._Master = z;
            return z;
        }

        public override Expression VisitEXPR_If([NotNull] S_ScriptParser.EXPR_IfContext context)
        {
            Expression x = this.Visit(context.expression()[0]);
            Expression y = this.Visit(context.expression()[1]);
            Expression z = (context.expression().Length == 2 ? null : this.Visit(context.expression()[2]));
            Expression if_func = new Expression.If(this._Host, this._Master);
            if_func.AddChild(x);
            if_func.AddChild(y);
            if_func.AddChild(z);
            this._Master = if_func;
            return if_func;
        }

        public override Expression VisitEXPR_Function([NotNull] S_ScriptParser.EXPR_FunctionContext context)
        {

            // General expression
            Expression x = null;

            // Exact name?
            if (context.name().unit_name().Length == 2)
            {

                string lib = context.name().unit_name()[0].GetText();
                string name = context.name().unit_name()[1].GetText();

                if (this._Host.Libraries.Exists(lib) && this._Host.Libraries[lib].ExpressionExists(name))
                {
                    x = this._Host.Libraries[lib].ExpressionLookup(name);
                }
                else
                {
                    throw new Exception(string.Format("Function '{0}.{1}' does not exist", lib, name));
                }
                
            }
            // Ambiguous name 
            else if (context.name().unit_name().Length == 1)
            {

                string name = context.name().unit_name()[0].GetText();

                foreach(Library lib in this._Host.Libraries)
                {
                    x = lib.ExpressionLookup(name);
                    if (x != null) break;
                }

                if (x == null)
                    throw new Exception(string.Format("Function '{0}' does not exist", name));
            }
            
            // Load all the parameters //
            foreach(S_ScriptParser.ExpressionContext ctx in context.expression())
            {
                x.AddChild(this.Visit(ctx));
            }

            this._Master = x;

            return x;

        }

        public override Expression VisitEXPR_AggFunction([NotNull] S_ScriptParser.EXPR_AggFunctionContext context)
        {

            //if (this._Master != null && !(this._Master is Expression.ArrayLiteral))
            //    throw new Exception("Aggregate functions cannot be nested, unless it's part of an array expression");

            string name = context.name().unit_name().Last().GetText();
            Expression.Aggregate a = Expression.Aggregate.Render(this._Host, name);
            foreach(S_ScriptParser.ExpressionContext ctx in context.expression())
            {
                a.AddChild(this.Visit(ctx));
            }

            this._Master = a;
            return a;
        }

        public override Expression VisitEXPR_VarName([NotNull] S_ScriptParser.EXPR_VarNameContext context)
        {

            //bool IsNaked = (context.name().unit_name().Length != 2);
            //string SpoolName = (IsNaked ? this._DefaultContext.Peek() : context.name().unit_name()[0].GetText());
            //string VarName = (IsNaked ? context.name().unit_name()[0].GetText() : context.name().unit_name()[1].GetText());
            //return new Expression.Lookup(this._Host, this._Master, SpoolName, VarName);
            
            // Proper name, X.Y //
            if (context.name().unit_name().Length == 2)
            {

                string x = context.name().unit_name()[0].GetText();
                string y = context.name().unit_name()[1].GetText();

                // Can be either a variable from a different name space or a table's identifier
                if (this._Context.SpoolExists(x))
                {
                    return new Expression.Lookup(this._Host, this._Master, x, y);
                }
                throw new Exception(string.Format("Namespace '{0}' is not in context", x));

            }
            // Otherwise, it's a naked name, such as X //
            else
            {

                string x = context.name().unit_name()[0].GetText();

                foreach (string name in this._Context.Aliases)
                {
                    if (this._Context[name].Exists(x))
                    {
                        return new Expression.Lookup(this._Host, this._Master, name, x);
                    }
                }

            }

            throw new Exception(string.Format("Variable '{0}' is invalid or not found", context.GetText()));

        }

        public override Expression VisitEXPR_Alias([NotNull] S_ScriptParser.EXPR_AliasContext context)
        {

            Expression x = this.Visit(context.expression());
            x.Name = context.unit_name().GetText();
            this._Master = x;
            return x;

        }

        public override Expression VisitEXPR_Equation([NotNull] S_ScriptParser.EXPR_EquationContext context)
        {

            Expression x = this.Visit(context.expression());
            this._Master = new Expression.Equation(this._Host, this._Master);
            this._Master.AddChild(x);
            return this._Master;

        }

        public override Expression VisitEXPR_BoundEquation([NotNull] S_ScriptParser.EXPR_BoundEquationContext context)
        {

            List<string> TableNames = new List<string>();
            List<string> Aliases = new List<string>();

            for(int i = 0; i < context.unit_name().Length; i += 2)
            {
                string t1 = context.unit_name()[i].GetText();
                string a1 = context.unit_name()[i + 1].GetText();
                TableNames.Add(t1);
                Aliases.Add(a1);
                Cell tref = this._Host.Spools[SystemNameSpaces.GLOBAL][t1];
                if (tref.Affinity != CellAffinity.TREF) throw new Exception("Equations can only be bound to tables");
                this._Context.AddSpool(a1, this._Host.OpenTable(tref).Columns);
            }
            
            Expression x = this.Visit(context.expression());
            this._Master = new Expression.Equation(this._Host, this._Master);
            this._Master.AddChild(x);

            foreach(string a2 in Aliases)
            {
                this._Context.DropSpool(a2);
            }

            return this._Master;

        }

        public override Expression VisitEXPR_Collapse([NotNull] S_ScriptParser.EXPR_CollapseContext context)
        {
            Expression x = this.Visit(context.expression());
            this._Master = new Expression.Collapse(this._Host, this._Master);
            this._Master.AddChild(x);
            return this._Master;
        }

        public override Expression VisitEXPR_Parens([NotNull] S_ScriptParser.EXPR_ParensContext context)
        {
            return this.Visit(context.expression());
        }

        public Expression Render(S_ScriptParser.ExpressionContext context)
        {
            this._Master = null;
            return this.Visit(context);
        }





    }



}
