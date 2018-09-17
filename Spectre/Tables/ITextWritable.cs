using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Spectre.Cells;

namespace Spectre.Tables
{
    
    public interface ITextWritable
    {

        void Write(TextWriter Writer);

    }

}
