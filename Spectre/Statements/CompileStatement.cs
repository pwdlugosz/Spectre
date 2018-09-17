using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre;
using Spectre.Cells;
using Spectre.Control;
using Spectre.Structures;
using Spectre.Tables;
using Spectre.Expressions;


namespace Spectre.Statements
{

    public class CompileStatement : Statement
    {

        private string _Library;
        private string _Name;
        private ScriptedStatement _Statement;

        public CompileStatement(Host Host, Statement Parent, string Library, string Name)
            : base(Host, Parent)
        {
            this._Library = Library;
            this._Name = Name;
            this._Statement = new ScriptedStatement(Host, null, Name);
        }
        
        public ScriptedStatement Statement
        {
            get { return this._Statement; }
            set { this._Statement = value; }
        }

        public override void Invoke(SpoolSpace Memory)
        {
            this._Host.Libraries[this._Library].AddScriptedStatement(this._Name, this._Statement);
        }

    }

}
