using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Spectre.Libraries;
using Spectre.Tables;
using Spectre.Expressions;
using Spectre.Cells;
using Spectre.Structures;
using Spectre.Scripting;

namespace Spectre.Control
{

    /// <summary>
    /// Represents an envrioment to that core pulse features run off of
    /// </summary>
    public sealed class Host
    {

        public const int DEBUG_STATE = 1;
        private StreamWriter _DebugWriter;

        public const string HOST_NAME = "SPECTRE";
        public const string HOST_VERSION = "0.6.0";

        /// <summary>
        /// The name of the disk based temp schema
        /// </summary>
        public const string TEMP = "TEMP";
        public const string GLOBAL = "GLOBAL";
        public const string USER = "USER";
        public const string RANDOM_NAME_PREFIX = "TEMP";

        public readonly long StartTicks = DateTime.Now.Ticks;

        private const string DIR_MAIN = "Spectre_Projects";
        private const string DIR_TEMP = "Temp";
        private const string DIR_TEST = "Test";
        private const string DIR_LOG = "Log";

        private Communicator _IO;
        private CellRandom _RNG;
        private Stopwatch _Timer;
        private SpoolSpace _Store;
        private TableStore _Cache;
        private Heap<Library> _Libraries;
        private static long _Tocks = 0;
        private ScriptProcessor _Engine;
        
        /// <summary>
        /// Creates a host
        /// </summary>
        public Host()
        {

            // Check the directory
            Host.CheckDir();

            // Timer
            this._Timer = Stopwatch.StartNew();

            // Communicator
            this._IO = new CommandLineCommunicator();

            // Random number generator
            this._RNG = new CellRandom();

            // Tables
            this._Cache = new TableStore(this);

            // Spools
            this._Store = new SpoolSpace();
            this._Store.Add(GLOBAL, new Spool.HeapSpindle(GLOBAL));

            // Libraries 
            this._Libraries = new Heap<Library>();
            this._Libraries.Allocate("BASE", new BaseLibrary(this));

            // Engine 
            this._Engine = new ScriptProcessor(this);

            // Possibly create a log writer //
            if (DEBUG_STATE == 1)
            {
                this._DebugWriter = new StreamWriter(DebugLogPath);
            }

        }

        /// <summary>
        /// Shuts down the host
        /// </summary>
        public void ShutDown()
        {

            this.TableStore.ShutDown();

            if (DEBUG_STATE == 1)
            {
                this._DebugWriter.Flush();
                this._DebugWriter.Close();
                this._DebugWriter = null;
            }

        }

        /// <summary>
        /// Internal table store
        /// </summary>
        public SpoolSpace Spools
        {
            get { return this._Store; }
        }

        /// <summary>
        /// Internal non-debugger log
        /// </summary>
        public Communicator IO
        {
            get { return this._IO; }
        }

        /// <summary>
        /// Gets the elapsed time since the host was launched
        /// </summary>
        public long Elapsed
        {
            get { return this._Timer.ElapsedMilliseconds; }
        }

        /// <summary>
        /// Gets the connection to TempDB
        /// </summary>
        public string TempDB
        {
            get { return Host.TempDir; }
        }

        /// <summary>
        /// 
        /// </summary>
        public TableStore TableStore
        {
            get { return this._Cache; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public ScriptProcessor Engine
        {
            get { return this._Engine; }
        }

        /// <summary>
        /// Contains all the libraries
        /// </summary>
        public Heap<Library> Libraries
        {
            get { return this._Libraries; }
        }

        /// <summary>
        /// Adds a library
        /// </summary>
        /// <param name="Element"></param>
        public void AddLibrary(Library Element)
        {
            this._Libraries.Allocate(Element.Name, Element);
        }

        /// <summary>
        /// Base random number generator
        /// </summary>
        public CellRandom BaseRNG
        {
            get { return this._RNG; }
        }
        
        /// <summary>
        /// Sets the RNG seed
        /// </summary>
        /// <param name="Seed"></param>
        public void SetSeed(int Seed)
        {
            this._RNG = new CellRandom(Seed);
        }

        // Table Support //
        /// <summary>
        /// Opens a table given a path
        /// </summary>
        /// <param name="Key">The path to the file</param>
        /// <returns></returns>
        public Table OpenTable(string Key)
        {
            return this._Cache.RequestTable(Key);
        }

        /// <summary>
        /// Creates a table with a cluster index
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Alias"></param>
        /// <param name="Columns"></param>
        /// <param name="ClusterColumns"></param>
        /// <param name="State"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public TreeTable CreateTable(string Path, Schema Columns, Key ClusterColumns, BinaryRecordTree.TreeAffinity State, int PageSize)
        {
            this._Cache.DropTable(Path);
            TreeTable t = new TreeTable(this, Path, Columns, ClusterColumns, State, PageSize);
            return t;
        }

        /// <summary>
        /// Creates a table with a given index; the page size is defaulted
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Alias"></param>
        /// <param name="Columns"></param>
        /// <param name="ClusterColumns"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public TreeTable CreateTable(string Path, Schema Columns, Key ClusterColumns, BinaryRecordTree.TreeAffinity State)
        {
            return this.CreateTable(Path, Columns, ClusterColumns, State, Page.DEFAULT_SIZE);
        }

        /// <summary>
        /// Creates a table with a clustered index; this assumes a default page size and a universal index
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Alias"></param>
        /// <param name="Columns"></param>
        /// <param name="ClusterColumns"></param>
        /// <returns></returns>
        public TreeTable CreateTable(string Path, Schema Columns, Key ClusterColumns)
        {
            return this.CreateTable(Path, Columns, ClusterColumns, BinaryRecordTree.TreeAffinity.Unconstrained, Page.DEFAULT_SIZE);
        }

        /// <summary>
        /// Creates a heap table
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Alias"></param>
        /// <param name="Columns"></param>
        /// <returns></returns>
        public HeapTable CreateTable(string Path, Schema Columns, int PageSize)
        {
            this._Cache.DropTable(Path);
            return new HeapTable(this, Path, Columns, PageSize);
        }

        /// <summary>
        /// Creates a dictionary table
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Alias"></param>
        /// <param name="KeyColumns"></param>
        /// <param name="ValueColumns"></param>
        /// <returns></returns>
        public DictionaryTable CreateDictionary(string Path, Schema Columns, Key DictionaryColumns)
        {
            this._Cache.DropTable(Path);
            return new DictionaryTable(this, Path, Columns, DictionaryColumns, Page.DEFAULT_SIZE);
        }

        /// <summary>
        /// Creates a temporary table and adds to the recycle bin
        /// </summary>
        /// <param name="Columns"></param>
        /// <returns></returns>
        public HeapTable CreateTempTable(Schema Columns)
        {
            HeapTable t = this.CreateTable(Host.RandomPath(), Columns, Page.DEFAULT_SIZE);
            this._Cache.PlaceInRecycleBin(t.Key);
            return t;
        }

        /// <summary>
        /// Checks if a table exists
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="Alias"></param>
        /// <returns></returns>
        public bool TableExists(string Path)
        {
            return this._Cache.TableExists(Path);
        }

        /// <summary>
        /// Checks if the table is system generated temporary table
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        public bool IsSystemTemp(Table T)
        {
            return T.Name.StartsWith(RANDOM_NAME_PREFIX) && T.Header.Directory == this.TempDB;
        }

        // Debugging //
        public void DebugPrint(string Text, params object[] Obj)
        {

            if (DEBUG_STATE == 0)
                return;

            string x = new string('\t', Math.Max(0, this.DebugDepth));
            this._DebugWriter.WriteLine(string.Format(x + Text, Obj));

        }

        public int DebugDepth
        {
            get;
            set;
        }

        public static long GetXID()
        {
            Cell x = new Cell(0);
            byte[] y = Guid.NewGuid().ToByteArray();
            x.B0 = (byte)(y[0] ^ y[8]);
            x.B1 = (byte)(y[1] ^ y[9]);
            x.B2 = (byte)(y[2] ^ y[10]);
            x.B3 = (byte)(y[3] ^ y[11]);
            x.B4 = (byte)(y[4] ^ y[12]);
            x.B5 = (byte)(y[5] ^ y[13]);
            x.B6 = (byte)(y[6] ^ y[14]);
            x.B7 = (byte)(y[7] ^ y[15]);
            return x.LONG < 0 ? ~x.LONG : x.LONG;
        }

        public static long Tocks()
        {
            return _Tocks++;
        }

        // Directories //
        public static string MainDir
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + DIR_MAIN + "\\"; }
        }

        public static string TempDir
        {
            get { return MainDir + DIR_TEMP + "\\"; }
        }

        public static string LogDir
        {
            get { return MainDir + DIR_LOG + "\\"; }
        }

        public static string TestDir
        {
            get { return MainDir + DIR_TEST + "\\"; }
        }

        public static void CheckDir()
        {

            if (!Directory.Exists(MainDir)) 
                Directory.CreateDirectory(MainDir);
            if (!Directory.Exists(TempDir)) 
                Directory.CreateDirectory(TempDir);
            if (!Directory.Exists(LogDir)) 
                Directory.CreateDirectory(LogDir);
            if (!Directory.Exists(TestDir)) 
                Directory.CreateDirectory(TestDir);

        }

        public static string RandomName
        {
            get
            {
                Guid x = Guid.NewGuid();
                return RANDOM_NAME_PREFIX + "_" + x.ToString().Replace("-", "");
            }
        }

        public static string RandomPath()
        {
            return TableHeader.DeriveV1Path(TempDir, RandomName);
        }

        public static string LongDateString
        {
            
            get
            {
                DateTime dt = DateTime.Now;
                return string.Format("{0}{1}{2}_{3}{4}{5}{6}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
            }

        }

        internal static string DebugLogPath
        {

            get
            {
                return LogDir + "_DEBUG_LOG_" + LongDateString + ".txt";
            }

        }

    }

}
