using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre;
using Spectre.Cells;
using Spectre.Expressions;
using Spectre.Control;
using Spectre.Statements;
using Spectre.Structures;
using Spectre.Tables;

namespace Spectre.Libraries
{

    /// <summary>
    /// Seed class for all libraries
    /// </summary>
    public abstract class Library
    {

        protected Host _Host;
        protected string _Name;
        protected Heap<ScriptedStatement> _ScriptedStatements;
        //protected Heap<ScriptedExpression> _ScriptedExpressions;

        public Library(Host Host, string Name)
        {
            this._Host = Host;
            this._Name = Name;
            this._ScriptedStatements = new Heap<ScriptedStatement>();
        }

        public Host Host
        {
            get { return this._Host; }
        }

        public string Name
        {
            get { return this._Name; }
        }

        public abstract ExpressionFunction ExpressionLookup(string Name);

        public virtual bool ExpressionExists(string Name)
        {
            return this.ExpressionLookup(Name) != null;
        }

        public abstract Statement StatementLookup(string Name);

        public virtual bool StatementExists(string Name)
        {
            return this.StatementLookup(Name) != null;
        }

        public void AddScriptedStatement(string Name, ScriptedStatement Value)
        {
            this._ScriptedStatements.Reallocate(Name, Value);
        }

    }

}

