using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Control;
using Spectre.Alpha;
using Spectre.Tables;
using Spectre.Cells;

namespace Spectre
{
    class Program
    {
        static void Main(string[] args)
        {


            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();


            Host Enviro = new Host();
            Enviro.AddLibrary(new Libraries.LibraryRandom(Enviro));
            Enviro.AddLibrary(new Libraries.TableLibrary(Enviro));
            Enviro.AddLibrary(new Libraries.ArrayLibrary(Enviro));
            Enviro.AddLibrary(new Libraries.MathLibrary(Enviro));

            string script = System.IO.File.ReadAllText(@"C:\Users\pwdlu_000\Documents\Spectre\Spectre\Scripting\TestScript.txt");
            Enviro.Engine.Execute(script);
            Enviro.ShutDown();
            
            Console.WriteLine("::::::::::::::::::::::::::::::::: Complete :::::::::::::::::::::::::::::::::");
            Console.WriteLine("Run Time: {0}", sw.Elapsed);
            string t = Console.ReadLine();


        }

        public static void DumpPageMap(string Path, Table Store)
        {

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(Path))
            {

                sw.WriteLine("PageID\tLastID\tNextID\tDownID\tUpID\tParent");
                for (int i = 0; i < Store.PageCount; i++)
                {
                    Page p = Store.GetPage(i);
                    sw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", p.PageID, p.LastPageID, p.NextPageID, p.X0, p.X1, p.ParentPageID));
                }

            }

        }



    }
}
