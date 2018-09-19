using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Statements;
using Spectre.Expressions;
using Spectre.Cells;
using Spectre.Tables;
using Spectre.Control;
using Antlr4.Runtime.Misc;

namespace Spectre.Scripting
{

    public sealed class StatementVisitor : S_ScriptBaseVisitor<Statement>
    {

        private Host _Host;
        private ExpressionVisitor _expr;
        private Statement _Master;
        private SpoolSpaceContext _Context;

        public StatementVisitor(Host Host, ExpressionVisitor ExpressionFactory)
            :base()
        {
            this._Host = Host;
            this._expr = ExpressionFactory;
            this._Context = ExpressionFactory.Context;
        }

        public StatementVisitor(Host Host)
            : this(Host, new ExpressionVisitor(Host))
        {

        }

        public override Statement VisitSTMR_Assign([NotNull] S_ScriptParser.STMR_AssignContext context)
        {

            Statement.AssignmentType v = Statement.AssignmentType.Equals;
            bool assign = context.assignment().ASSIGN() != null;
            string LibName = context.name().unit_name().Length == 1 ? SystemNameSpaces.GLOBAL : context.name().unit_name()[0].GetText();
            string VarName = context.name().unit_name().Length == 1 ? context.name().unit_name()[0].GetText() : context.name().unit_name()[1].GetText();
            
            if (this._Context.SpoolExists(LibName) && !this._Context[LibName].Exists(VarName))
                this._Context[LibName].Add(VarName);

            if (assign && context.assignment().PLUS() != null)
                v = Statement.AssignmentType.PlusEquals;
            else if (assign && context.assignment().MINUS() != null)
                v = Statement.AssignmentType.MinusEquals;
            else if (assign && context.assignment().MUL() != null)
                v = Statement.AssignmentType.MultEquals;
            else if (assign && context.assignment().DIV() != null)
                v = Statement.AssignmentType.DivEquals;
            else if (assign && context.assignment().DIV2() != null)
                v = Statement.AssignmentType.Div2Equals;
            else if (assign && context.assignment().MOD() != null)
                v = Statement.AssignmentType.ModEquals;
            else if (assign && context.assignment().MOD2() != null)
                v = Statement.AssignmentType.Mod2Equals;

            // Check if we have a table //
            //if (v == Statement.AssignmentType.PlusEquals && this._Host.Spools.Exists(LibName) && this._Host.Spools[LibName].Exists(VarName) && this._Host.Spools[LibName][VarName].Affinity == CellAffinity.TREF)
            //{
            //    string tref = this._Host.Spools[LibName][VarName].valueTREF;
            //    Table t = this._Host.OpenTable(tref);
            //    Expression a = this._expr.Visit(context.expression()[0]);
            //    bool ContainsAggregate = (a is Expression.ArrayLiteral && a.ContainsAggregate);

            //    Statement s = null;
            //    if (!ContainsAggregate)
            //    {
            //        s = new TableInsert.GenericTableInsert(this._Host, this._Master, t);
            //        s.Parameters.Add(a);
            //    }
            //    else
            //    {
            //        s = new TableInsert.DictionGroupBy(this._Host, this._Master, t);
            //        foreach(Expression exp in a.Children)
            //        {
            //            s.Parameters.Add(exp);
            //        }
            //    }

            //    this._Master = s;
            //    return s;
            //}

            Statement x = new Statement.Assignment(this._Host, this._Master, LibName, VarName, v);

            foreach(S_ScriptParser.ExpressionContext e in context.expression())
            {
                Expression n = this._expr.Visit(e);
                x.Parameters.Add("F" + x.Parameters.Count.ToString(), n);
            }

            return x;
            
        }

        public override Statement VisitSTMR_Increment([NotNull] S_ScriptParser.STMR_IncrementContext context)
        {

            Statement.AssignmentType v = Statement.AssignmentType.PlusPlus;
            string LibName = context.name().unit_name().Length == 1 ? SystemNameSpaces.GLOBAL : context.name().unit_name()[0].GetText();
            string VarName = context.name().unit_name().Length == 1 ? context.name().unit_name()[0].GetText() : context.name().unit_name()[1].GetText();

            if (context.increment().PLUS() != null)
                v = Statement.AssignmentType.PlusPlus;
            else if (context.increment().MINUS() != null)
                v = Statement.AssignmentType.MinusMinus;

            Statement x = new Statement.Assignment(this._Host, this._Master, LibName, VarName, v);

            foreach (S_ScriptParser.ExpressionContext e in context.expression())
            {
                Expression n = this._expr.Visit(e);
                x.Parameters.Add("F" + x.Parameters.Count.ToString(), n);
            }

            return x;


        }

        public override Statement VisitSTMR_Append([NotNull] S_ScriptParser.STMR_AppendContext context)
        {
            
            string LibName = context.name().unit_name().Length == 1 ? SystemNameSpaces.GLOBAL : context.name().unit_name()[0].GetText();
            string VarName = context.name().unit_name().Length == 1 ? context.name().unit_name()[0].GetText() : context.name().unit_name()[1].GetText();
            
            // Check if we have a table //
            if (this._Host.Spools.Exists(LibName) && this._Host.Spools[LibName].Exists(VarName) && this._Host.Spools[LibName][VarName].Affinity == CellAffinity.TREF)
            {
                string tref = this._Host.Spools[LibName][VarName].valueTREF;
                Table t = this._Host.OpenTable(tref);
                Expression a = this._expr.Visit(context.expression()[0]);
                bool ContainsAggregate = (a is Expression.ArrayLiteral && a.ContainsAggregate);

                Statement s = null;
                if (!ContainsAggregate)
                {
                    s = new TableInsert.GenericTableInsert(this._Host, this._Master, t);
                    s.Parameters.Add(a);
                }
                else
                {
                    s = new TableInsert.DictionGroupBy(this._Host, this._Master, t);
                    foreach (Expression exp in a.Children)
                    {
                        s.Parameters.Add(exp);
                    }
                }

                this._Master = s;
                return s;
            }

            // Check if we are an array //
            if (this._Host.Spools.Exists(LibName) && this._Host.Spools[LibName].Exists(VarName) && this._Host.Spools[LibName][VarName].Affinity == CellAffinity.ARRAY)
            {
                Statement.InsertIntoArray x = new Statement.InsertIntoArray(this._Host, this._Master, LibName, VarName);
                foreach (S_ScriptParser.ExpressionContext e in context.expression())
                {
                    Expression n = this._expr.Visit(e);
                    x.Parameters.Add(n.NameOf(), n);
                }
                this._Master = x;
                return x;
            }

            throw new Exception("Append is only valid for table and array types");

        }

        public override Statement VisitSTMR_Print([NotNull] S_ScriptParser.STMR_PrintContext context)
        {

            Statement s = new Statement.Print(this._Host, this._Master);
            s.Parameters.Add("PrintVariable", this._expr.Render(context.expression()));
            return s;
        }

        public override Statement VisitSTMR_Set([NotNull] S_ScriptParser.STMR_SetContext context)
        {

            Statement x = new Statement.DoSet(this._Host, this._Master);
            foreach(S_ScriptParser.StatementContext ctx in context.statement())
            {
                x.AddChild(this.Visit(ctx));
            }
            return x;

        }

        public override Statement VisitSTMR_Do([NotNull] S_ScriptParser.STMR_DoContext context)
        {

            Statement x = new Statement.DoSet(this._Host, this._Master);
            foreach (S_ScriptParser.StatementContext ctx in context.statement())
            {
                x.AddChild(this.Visit(ctx));
            }
            return x;

        }

        public override Statement VisitSTMR_Exec([NotNull] S_ScriptParser.STMR_ExecContext context)
        {

            string LibName = GetLibrary(context.name(), SystemNameSpaces.BASE);
            string StatementName = GetName(context.name());

            if (!(this._Host.Libraries.Exists(LibName) && this._Host.Libraries[LibName].StatementExists(StatementName)))
            {
                throw new Exception(string.Format("Method or subroutine '{0}.{1}' does not exist", LibName, StatementName));
            }

            Statement x = this._Host.Libraries[LibName].StatementLookup(StatementName);
            bool IsScripted = (x is ScriptedStatement);
            int i = 0;
           
            foreach (S_ScriptParser.ExpressionContext p in context.expression())
            {
                Expression exp = this._expr.Render(p);
                if (!IsScripted)
                {
                    x.Parameters.Add(exp);
                }
                else
                {
                    x.Parameters[i] = exp;
                    i++;
                }
            }

            this._Master = x;
            return x;

        }

        public override Statement VisitSTMR_Inline([NotNull] S_ScriptParser.STMR_InlineContext context)
        {
            throw new NotImplementedException("Statement.Inline");
            Statement.InLine x = new Statement.InLine(this._Host, this._Master);
            Expression Script = this._expr.Render(context.expression()[0]);
            Expression Parameters = this._expr.Render(context.expression()[1]);
            Expression Values = this._expr.Render(context.expression()[2]);
            x.Parameters.Add(Script);
            x.Parameters.Add(Parameters);
            x.Parameters.Add(Values);
            this._Master = x;
            return x;
        }

        public override Statement VisitSTMR_ForEach([NotNull] S_ScriptParser.STMR_ForEachContext context)
        {

            string LibName = SystemNameSpaces.GLOBAL;
            string VarName = context.unit_name().GetText();

            if (this._Context.SpoolExists(LibName) && !this._Context[LibName].Exists(VarName))
                this._Context[LibName].Add(VarName);
            
            Statement x = new Statement.ForeachLoop(this._Host, this._Master, LibName, VarName);
            foreach (S_ScriptParser.StatementContext ctx in context.statement())
            {
                x.AddChild(this.Visit(ctx));
            }
            x.Parameters.Add("Enumeration", this._expr.Render(context.expression()));

            this._Master = x;
            return x;

        }
        
        public override Statement VisitSTMR_For([NotNull] S_ScriptParser.STMR_ForContext context)
        {
            
            Statement L0 = this.Visit(context.statement()[0]);
            Expression L1 = this._expr.Render(context.expression());
            Statement L2 = this.Visit(context.statement()[1]);

            Statement x = new Statement.ForLoop(this._Host, this._Master, L0, L1, L0);

            for (int i = 2; i < context.statement().Length; i++)
            {
                Statement y = this.Visit(context.statement()[i]);
            }

            this._Master = x;
            return x;

        }

        public override Statement VisitSTMR_ForEachJoin([NotNull] S_ScriptParser.STMR_ForEachJoinContext context)
        {

            string LAlias = context.unit_name()[0].GetText();
            Expression LeftX = this._expr.Render(context.expression()[0]);
            if (LeftX == null) throw new Exception("The left expression must be a table");
            LeftX.Name = LAlias;

            string RAlias = context.unit_name()[1].GetText();
            Expression RightX = this._expr.Render(context.expression()[1]);
            if (RightX == null) throw new Exception("The right expression must be a table");
            RightX.Name = RAlias;

            this._Context.AddSpool(LAlias, LeftX.Columns);
            this._Context.AddSpool(RAlias, RightX.Columns);
            //this._expr.DefaultContext.Push(LAlias);
            //this._expr.DefaultContext.Push(RAlias);

            var keys = this.RenderJoinKeys(context.join_predicate(), LeftX.Columns, LAlias, RightX.Columns, RAlias);

            TableLoop.Join.JoinAffinity x = TableLoop.Join.JoinAffinity.Inner;
            if (context.OR() != null)
                x = TableLoop.Join.JoinAffinity.Left;
            else if (context.XOR() != null)
                x = TableLoop.Join.JoinAffinity.AntiLeft;
            else if (context.AND() != null)
                x = TableLoop.Join.JoinAffinity.Inner;

            Statement y = new TableLoop.Join.SortMergeJoin(this._Host, this._Master, SystemNameSpaces.GLOBAL, Expression.TrueForAll, keys.Item1, keys.Item2, x);
            y.Parameters.Add(LeftX);
            y.Parameters.Add(RightX);

            foreach (S_ScriptParser.StatementContext ctx in context.statement())
            {
                y.AddChild(this.Visit(ctx));
            }

            this._Context.DropSpool(LAlias);
            this._Context.DropSpool(RAlias);
            //this._expr.DefaultContext.Pop();
            //this._expr.DefaultContext.Pop();

            return y;

        }

        public override Statement VisitSTMR_While([NotNull] S_ScriptParser.STMR_WhileContext context)
        {

            Statement x = new Statement.WhileLoop(this._Host, this._Master);
            foreach (S_ScriptParser.StatementContext ctx in context.statement())
            {
                x.AddChild(this.Visit(ctx));
            }
            x.Parameters.Add("Enumeration", this._expr.Render(context.expression()));

            this._Master = x;
            return x;

        }

        public override Statement VisitSTMR_If([NotNull] S_ScriptParser.STMR_IfContext context)
        {

            Statement.If x = new Statement.If(this._Host, this._Master, context.K_ELSE() != null);
            foreach(S_ScriptParser.StatementContext s in context.statement())
            {
                x.AddChild(this.Visit(s));
            }
            foreach(S_ScriptParser.ExpressionContext e in context.expression())
            {
                x.Parameters.Add("F" + x.Parameters.Count.ToString(), this._expr.Render(e));
            }
            
            this._Master = x;
            return x;

        }

        public override Statement VisitSTMR_Break([NotNull] S_ScriptParser.STMR_BreakContext context)
        {

            Statement x = null;

            if (context.K_BREAK() != null)
            {
                x = new Statement.Break(this._Host, this._Master);
            }
            else if (context.K_RETURN() != null)
            {
                x = new Statement.Return(this._Host, this._Master);
            }
            else if (context.K_EXIT() != null)
            {
                x = new Statement.Exit(this._Host, this._Master);
            }
            else
            {
                throw new Exception();
            }

            return x;

        }

        public override Statement VisitSTMR_CreateStatement([NotNull] S_ScriptParser.STMR_CreateStatementContext context)
        {

            string Library = this.GetLibrary(context.name()[0], SystemNameSpaces.BASE);
            string Name = this.GetName(context.name()[0]);

            this._Context.AddSpool(Name);
            //this._expr.DefaultContext.Push(Name);

            ScriptedStatement y = new ScriptedStatement(this._Host, null, Name);
            for (int i = 1; i < context.name().Length; i++)
            {
                string PName = this.GetName(context.name()[i]);
                //this._Context[Name].Add(PName);
                y.ParameterNames.Add(PName);
                y.Parameters.Add(PName, new Expression.Literal(null, null, CellValues.NullINT));
            }

            foreach(S_ScriptParser.StatementContext ctx in context.statement())
            {
                y.AddChild(this.Visit(ctx));
            }

            CompileStatement x = new CompileStatement(this._Host, this._Master, Library, Name);
            x.Statement = y;

            this._Context.DropSpool(Name);
            //this._expr.DefaultContext.Pop();

            this._Master = x;
            return x;

        }

        public Statement Render(S_ScriptParser.StatementContext context)
        {
            this._Master = null;
            return this.Visit(context);
        }

        private Tuple<Key, Key> RenderJoinKeys(S_ScriptParser.Join_predicateContext context, Schema LColumns, string LAlias, Schema RColumns, string RAlias)
        {

            Key LKey = new Key();
            Key RKey = new Key();
            foreach(S_ScriptParser.Join_predicate_unitContext x in context.join_predicate_unit())
            {
                this.AppendJoinKey(x, LColumns, LAlias, LKey, RColumns, RAlias, RKey);
            }

            return new Tuple<Key, Key>(LKey, RKey);

        }

        private void AppendJoinKey(S_ScriptParser.Join_predicate_unitContext context, Schema LColumns, string LAlias, Key LKey, Schema RColumns, string RAlias, Key RKey)
        {

            string LNameSpace = (context.name()[0].unit_name().Length == 1 ? null : context.name()[0].unit_name()[0].GetText());
            string LName = context.name()[0].unit_name().Last().GetText();
            string RNameSpace = (context.name()[1].unit_name().Length == 1 ? null : context.name()[1].unit_name()[0].GetText());
            string RName = context.name()[1].unit_name().Last().GetText();

            // Look for a missing left alias
            if (LNameSpace == null && LColumns.Contains(LName))
                LNameSpace = LAlias;
            else if (LNameSpace == null && RColumns.Contains(LName))
                LNameSpace = RAlias;
            else if (LNameSpace == null)
                throw new Exception(string.Format("Column name '{0}' is invalid", LName));

            // Look for a missing right alias
            if (RNameSpace == null && LColumns.Contains(RName))
                RNameSpace = LAlias;
            else if (LNameSpace == null && RColumns.Contains(RName))
                RNameSpace = RAlias;
            else if (LNameSpace == null)
                throw new Exception(string.Format("Column name '{0}' is invalid", LName));

            // Check which namespace this belongs to
            if (LNameSpace == LAlias && RNameSpace == RAlias)
            {
                int lidx = LColumns.ColumnIndex(LName);
                int ridx = RColumns.ColumnIndex(RName);
                if (lidx == Schema.OFFSET_NULL) throw new Exception(string.Format("Column '{0}' does not exist in '{1}'", LName, LAlias));
                if (ridx == Schema.OFFSET_NULL) throw new Exception(string.Format("Column '{0}' does not exist in '{1}'", RName, RAlias));
                LKey.Add(lidx);
                RKey.Add(ridx);
            }
            else if (RNameSpace == LAlias && LNameSpace == RAlias)
            {
                string x = LName;
                LName = RName;
                RName = x;
                int lidx = LColumns.ColumnIndex(LName);
                int ridx = RColumns.ColumnIndex(RName);
                if (lidx == Schema.OFFSET_NULL) throw new Exception(string.Format("Column '{0}' does not exist in '{1}'", LName, LAlias));
                if (ridx == Schema.OFFSET_NULL) throw new Exception(string.Format("Column '{0}' does not exist in '{1}'", RName, RAlias));
                LKey.Add(lidx);
                RKey.Add(ridx);
            }
            else
            {
                throw new Exception(string.Format("Join predicate is invalid '{0}.{1}' and '{2}.{3}'", LNameSpace, LName, RNameSpace, RName));
            }

        }

        private string GetLibrary(S_ScriptParser.NameContext context, string DefaultName)
        {
            return (context.unit_name().Length == 2 ? context.unit_name()[0].GetText() : DefaultName);
        }

        private string GetName(S_ScriptParser.NameContext context)
        {
            return (context.unit_name().Length == 2 ? context.unit_name()[1].GetText() : context.unit_name()[0].GetText());
        }

    }

}
