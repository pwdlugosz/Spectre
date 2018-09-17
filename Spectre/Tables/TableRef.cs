using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Control;

namespace Spectre.Tables
{
    
    public class TableRef
    {

        private Host _Host;
        private Table _Table;
        private Schema _Columns;
        private string _Key;

        public TableRef(Table Value)
        {
            this._Host = Value.Host;
            this._Table = Value;
            this._Columns = Value.Columns;
            this._Key = Value.Header.Path;
        }

        public TableRef(Host Host, string Path)
        {

            this._Host = Host;
            this._Key = Path;
            this._Columns = TableStore.Buffer(Path).Columns;

        }

        public Host Host
        {
            get { return this._Host; }
        }

        public Table Value
        {

            get
            {
                if (this._Table == null)
                {
                    return this._Host.TableStore.RequestTable(this._Key);
                }
                else
                {
                    return this._Table;
                }
            }

        }

        public Schema Columns
        {
            get
            {
                return this._Columns;
            }
        }

        public string Path
        {
            get
            {
                return this._Key;
            }
        }

    }

}
