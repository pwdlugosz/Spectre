
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

    public sealed class ScriptedStatement : Statement
    {

        private string _Name;
        private List<string> _ParameterNames;

        public ScriptedStatement(Host Host, Statement Parent, string Name)
            : base(Host, Parent)
        {
            this._Name = Name;
            this._ParameterNames = new List<string>();
        }

        public string Name
        {
            get { return this._Name; }
        }

        public List<string> ParameterNames
        {
            get { return this._ParameterNames; }
        }
        
        public void Bind(SpoolSpace Context)
        {

            if (this._Parameters.Count != this._ParameterNames.Count)
                throw new Exception(string.Format("Arguments for '{0}' are invalid; expecting {1} parameter(s) but recieved {2}", this._Name, this._ParameterNames.Count, this._Parameters.Count));

            this.BindTables(Context);

            Context.Drop(this._Name);
            Context.Add(this._Name, new Spool.HeapSpindle(this._Name));

            for (int i = 0; i < this._ParameterNames.Count; i++)
            {
                Context[this._Name].Declare(this._ParameterNames[i], this._Parameters[i].Evaluate(Context));
            }


        }

        public override void Invoke(SpoolSpace Memory)
        {

            // Bind First
            this.Bind(Memory);

            // Invoke all
            this.InvokeChildren(Memory);

            Memory.Drop(this._Name);

            this.BurnBoundTables(Memory);
            
        }

    }

}
