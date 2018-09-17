using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre;
using Spectre.Cells;
using Spectre.Control;
using Spectre.Statements;
using Spectre.Expressions;
using Antlr4.Runtime;
using Antlr4;

namespace Spectre.Scripting
{

    public sealed class ScriptProcessor
    {

        private Host _Host;
        private int _InceptionDepth = 0;

        public ScriptProcessor(Host Host)
        {
            this.Host = Host;
        }

        public int InceptionDepth
        {
            get { return this._InceptionDepth; }
            set { this._InceptionDepth = value; }
        }
        
        public Host Host
        {
            get { return this._Host; }
            private set { this._Host = value; }
        }
        
        public void Execute(string Script)
        {

            // Create a token stream and do lexal analysis //
            AntlrInputStream TextStream = new AntlrInputStream(Script);
            S_ScriptLexer lex = new S_ScriptLexer(TextStream);

            // Parse the script //
            CommonTokenStream RyeTokenStream = new CommonTokenStream(lex);
            S_ScriptParser par = new S_ScriptParser(RyeTokenStream);
            
            // Create an executer object //
            StatementVisitor processor = new StatementVisitor(this.Host);

            // Create a spool space //
            SpoolSpace mem = this._Host.Spools;

            // Load the call stack and/or parse the errors
            foreach (S_ScriptParser.StatementContext ctx in par.compileUnit().statement())
            {
                Statement stmt = processor.Render(ctx);
                stmt.BeginInvoke(mem);
                stmt.Invoke(mem);
                stmt.EndInvoke(mem);
            }
            
        }
        
    }

}
