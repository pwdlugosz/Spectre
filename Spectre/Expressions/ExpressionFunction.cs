using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Cells;
using Spectre.Control;
using Spectre.Structures;
using Spectre.Tables;


namespace Spectre.Expressions
{

    public abstract class ExpressionFunction : Expression
    {

        private int _MinParameterCount = -1;
        private int _MaxParameterCount = -1;
        protected string _Name;

        public ExpressionFunction(Host Host, Expression Parent, string Name, int MinParameterCount, int MaxParameterCount)
            : base(Host, Parent)
        {
            this._MinParameterCount = MinParameterCount;
            this._MaxParameterCount = MaxParameterCount;
            this._Name = Name;
        }

        public string Name
        {
            get { return this._Name; }
        }

        public void CheckParameters()
        {

            int Len = this._Children.Count;

            // No parameter constraints
            if (this._MinParameterCount == -1 && this._MaxParameterCount == -1)
                return;

            // No lower bound, but we max out at a certain count
            if (this._MinParameterCount == -1 && Len <= this._MaxParameterCount)
                return;

            // No upper bound, but we have a minium count
            if (this._MinParameterCount <= Len && this._MaxParameterCount == -1)
                return;

            // Lower and upper bound
            if (this._MinParameterCount <= Len && this._MaxParameterCount >= Len)
                return;

            throw new Exception(string.Format("Invalid paramter length {0}; expecting something between {1},{2}", Len, (this._MinParameterCount == -1 ? 0 : this._MinParameterCount), (this._MaxParameterCount == -1 ? "Unlimited" : this._MaxParameterCount.ToString())));

        }

    }

}
