using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Cells;
using System.IO;

namespace Spectre.Tables
{


    /// <summary>
    /// Represents a disk based binary tree of pages
    /// </summary>
    public sealed class BinaryRecordTree
    {

        /// <summary>
        /// Represents the affinity of a tree
        /// </summary>
        public enum TreeAffinity : byte
        {

            /// <summary>
            /// All elements are required to be unique
            /// </summary>
            Unique,

            /// <summary>
            /// All elements are required to be unique, but does not throw an exception if a duplicate is added
            /// </summary>
            Distinct,

            /// <summary>
            /// Allows duplicates
            /// </summary>
            Unconstrained

        }

        /// <summary>
        /// Represents a search pattern
        /// </summary>
        public enum FindAffinity : byte
        {

            /// <summary>
            /// Finds the first occurance
            /// </summary>
            First,

            /// <summary>
            /// Finds any occurance
            /// </summary>
            Any,

            /// <summary>
            /// Find the last occurange
            /// </summary>
            Last
        }

        /// <summary>
        /// Represents a domain
        /// </summary>
        public enum DomainAffinity : int
        {

            /// <summary>
            /// X strictly less than the lowest element
            /// </summary>
            LowerExclusive = -2,

            /// <summary>
            /// X equal to the lowest element
            /// </summary>
            LowerInclusive = -1,

            /// <summary>
            /// X greater than the first element, and less than the last unit
            /// </summary>
            Inclusive = 0,

            /// <summary>
            /// X equal to the highest unit
            /// </summary>
            UpperInclusive = 1,

            /// <summary>
            /// X strictly greater than the highest unit
            /// </summary>
            UpperExclusive = 2,

            /// <summary>
            /// X is equal to both the greatest and lowest units
            /// </summary>
            Everything = 31,

            /// <summary>
            /// The page is empty
            /// </summary>
            Nothing = 32

        }

        /// <summary>
        /// Represents a page-node's afffinity
        /// </summary>
        public enum PageAffinity : byte
        {

            /// <summary>
            /// On the lower side of it's parent node (left side of the tree)
            /// </summary>
            Down,

            /// <summary>
            /// On the upper side of it's parent node (right side of the tree)
            /// </summary>
            Up,

            /// <summary>
            /// The page is the root node
            /// </summary>
            Root

        }

        private static readonly int MAX_PAGE_RECORD_SIZE = -1;

        private Table _Storeage;
        private IElementHeader _Header;
        private Key _Comparer;
        private Key _Comparer2;
        private Page _Root;
        private TreeAffinity _Affinity;
        private Schema _Columns;
        private int _MutateCounter = 0;

        /// <summary>
        /// Creates a tree from an existing object set
        /// </summary>
        /// <param name="Storage"></param>
        /// <param name="Comparer"></param>
        /// <param name="Root"></param>
        /// <param name="Header"></param>
        /// <param name="Affinity"></param>
        public BinaryRecordTree(Table Storage, Key Comparer, Schema Columns, Page Root, IElementHeader Header, TreeAffinity Affinity)
        {

            this._Storeage = Storage;
            this._Header = Header;
            this._Comparer = Comparer;
            this._Affinity = Affinity;
            this._Columns = Columns;
            this._Comparer2 = Key.Build(Comparer.Count);
            if (this._Root == null)
            {
                this._Root = new Page(this._Storeage.PageSize, this._Storeage.GenerateNewPageID, Page.NULL_INDEX, Page.NULL_INDEX, Columns.Count, 0);
                this._Root.X0 = Page.NULL_INDEX;
                this._Root.X1 = Page.NULL_INDEX;
                this._Storeage.SetPage(this._Root);
                this._Storeage.Header.PageCount++;
            }
            else
            {
                this._Root = Root;
                if (this._Root.Count == 0)
                {
                    this._Root.X0 = Page.NULL_INDEX;
                    this._Root.X1 = Page.NULL_INDEX;
                }
            }
            this._Header.OriginPageID = this.SeekOrigin().PageID;
            this._Header.TerminalPageID = this.SeekTerminal().PageID;
            this._Header.RootPageID = this._Root.PageID;

        }

        public BinaryRecordTree(Table Storage, Key Comparer, Schema Columns, TreeAffinity Affinity)
            : this(Storage, Comparer, Columns, null, Storage.Header, Affinity)
        {
        }

        public BinaryRecordTree(Table Storage, Key Comparer, Schema Columns)
            : this(Storage, Comparer, Columns, null, Storage.Header, TreeAffinity.Unconstrained)
        {
        }

        /// <summary>
        /// Gets the root page
        /// </summary>
        public Page Root
        {
            get { return this._Root; }
        }

        /// <summary>
        /// Returns the object hearder, which may be the storage header
        /// </summary>
        public IElementHeader Header
        {
            get { return this._Header; }
        }

        /// <summary>
        /// Returns the base comparer key (ie, the index columns)
        /// </summary>
        public Key Comparer
        {
            get { return this._Comparer; }
        }

        /// <summary>
        /// Returns the storage object for this tree
        /// </summary>
        public Table Storage
        {
            get { return this._Storeage; }
        }

        /// <summary>
        /// Returns the tree's affinity
        /// </summary>
        public TreeAffinity Affinity
        {
            get { return this._Affinity; }
        }

        /// <summary>
        /// Returns the tree's column structure
        /// </summary>
        public Schema Columns
        {
            get { return this._Columns; }
        }

        /// <summary>
        /// Gets the first page in a tree
        /// </summary>
        /// <returns></returns>
        public Page SeekOrigin()
        {

            Page p = this._Root;
            while (true)
            {
                if (p.X0 == Page.NULL_INDEX)
                    return p;
                p = this._Storeage.GetPage(p.X0);
            }

            throw new Exception();

        }

        /// <summary>
        /// Gets the last page in the tree
        /// </summary>
        /// <returns></returns>
        public Page SeekTerminal()
        {

            Page p = this._Root;
            while (true)
            {
                if (p.X1 == Page.NULL_INDEX)
                    return p;
                p = this._Storeage.GetPage(p.X1);
            }

            throw new Exception();

        }

        /// <summary>
        /// Gets a count of all pages diaginally downstream from this node
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public int DownRunCount(Page Value)
        {
            int i = 0;
            while (Value.X0 != Page.NULL_INDEX)
            {
                i++;
                Value = this._Storeage.GetPage(Value.X0);
            }
            return i;
        }

        /// <summary>
        /// Gets a count of all pages diaginally upstream from this node
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public int UpRunCount(Page Value)
        {
            int i = 0;
            while (Value.X1 != Page.NULL_INDEX)
            {
                i++;
                Value = this._Storeage.GetPage(Value.X1);
            }
            return i;
        }

        // Column Stuff Methods //
        /// <summary>
        /// Given a key only, creates a record with the same structure as the schema to do comparisons
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public Record KeyToValue(Record Key)
        {
            Cell[] x = new Cell[this._Columns.Count]; // should be an array of null bools
            for (int i = 0; i < this._Comparer.Count; i++)
            {
                x[this._Comparer[i]] = Key[i];
            }
            return new Record(x);
        }

        // Appending Methods //
        /// <summary>
        /// Adds a record to the tree
        /// </summary>
        /// <param name="Value"></param>
        public void Insert(Record Value)
        {

            // Find the page to insert the value into //
            Page p = this.Find(Value, FindAffinity.Any);

            // Check if the page is full or not //
            if (p.CanAccept(Value) && (p.Count < MAX_PAGE_RECORD_SIZE || MAX_PAGE_RECORD_SIZE < 0))
            {
                this.Insert(p, Value);
                return;
            }

            // Otherwise, split the page //
            Page A = this.Split(p);
            
            // Insert the page //
            DomainAffinity x = this.CalculateDomainAffinity(A, Value, this._Comparer);
            DomainAffinity y = this.CalculateDomainAffinity(p, Value, this._Comparer);
            if (x == DomainAffinity.Everything || x == DomainAffinity.UpperInclusive || x == DomainAffinity.LowerInclusive || x == DomainAffinity.Inclusive || x == DomainAffinity.Nothing)
            {
                this.Insert(A, Value);
            }
            else //if (y == DomainAffinity.Everything || y == DomainAffinity.UpperInclusive || y == DomainAffinity.LowerInclusive || y == DomainAffinity.Inclusive || y == DomainAffinity.Nothing)
            {
                this.Insert(p, Value);
            }


        }

        /// <summary>
        /// Deletes a record from the tree
        /// </summary>
        /// <param name="Value"></param>
        public void Delete(Record Value)
        {

            // Find the first value
            Page p = this.Find(Value, this._Root, FindAffinity.First);

            // Loop through each link in the page chain
            while (true)
            {
                int DeleteCount = this.DeleteFromSinglePage(p, Value);
                if (DeleteCount == 0)
                    break;
                if (p.NextPageID != Page.NULL_INDEX)
                    p = this._Storeage.GetPage(p.NextPageID);
                else
                    break;
            }

        }

        /// <summary>
        /// Updates a record in the tree
        /// </summary>
        /// <param name="OldValue"></param>
        /// <param name="NewValue"></param>
        public void Update(Record OldValue, Record NewValue)
        {

            // There's optimization we can do here:
            // If this table is constrained to be unique, then we only have to do one seek
            if (this.Affinity == TreeAffinity.Distinct || this.Affinity == TreeAffinity.Unique)
            {
                RecordKey pointer = this.FindFirst(OldValue);
                this.Update(pointer, NewValue);
                return;
            }

            // Get the start/stop //
            RecordKey first = this.FindFirst(OldValue, true);
            RecordKey last = this.FindLast(OldValue, true);

            // Check two things: (1) first == last and (2) first is empty, regardless of (1), last will be empty
            if (first.IsNotFound)
                return;
            if (first == last)
            {
                this.Update(first, NewValue);
                return;
            }

            // Tricky part, we have to open a record reader, but we may run into issues if the tree splits during the update, so everyting the tree splits while
            // the update is running, we have to re-calculate the end pointer
            RecordReader rr = new RecordReaderBase(this._Storeage, first, last);
            int LocalMutateCounter = this._MutateCounter;
            while (rr.CanAdvance)
            {

                if (LocalMutateCounter != this._MutateCounter)
                {
                    LocalMutateCounter = this._MutateCounter;
                    last = this.FindLast(OldValue, true);
                    rr = new RecordReaderBase(this._Storeage, rr.PositionKey, last);
                }

                RecordKey pointer = rr.PositionKey;
                this.Update(pointer, NewValue);
                rr.Advance();

            }

        }

        /// <summary>
        /// Updates any record to another record's value
        /// </summary>
        /// <param name="Index"></param>
        /// <param name="NewValue"></param>
        public void Update(RecordKey Index, Record NewValue)
        {

            // Don't even do anything if the key if empty
            if (Index.IsNotFound)
                return;

            // Get the page we need to update 
            Page p = this._Storeage.GetPage(Index.PageID);
            
            // Check if we are updating the key or just the values
            Record OldValue = p.Select(Index.RowID);
            bool SameKey = Record.Compare(OldValue, this._Comparer, NewValue, this._Comparer) == 0;

            // Check that the page has sizing capacity to update AND that the key fields didnt change
            if (p.CanUpdate(NewValue, Index.RowID) && SameKey)
            {
                p.Update(NewValue, Index.RowID);
                return;
            }
            else // If the page doesnt have capacity or we changed the key fields, then we have to delete the old record and insert the new record
            {

                // Delete the record //
                p.Delete(Index.RowID);

                // Insert the new record //
                this.Insert(NewValue); // Note: this will throw an exception if we try to insert a duplicate key

            }

        }

        // Finds //
        /// <summary>
        /// Checks is a page exists in the tree
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="SearchRoot"></param>
        /// <returns></returns>
        public bool Exists(Record Value, Page SearchRoot)
        {
            return this.Find(Value, SearchRoot, FindAffinity.Any).Cache.BinarySearch(Value, this._Comparer) > 0;
        }

        /// <summary>
        /// Checks is a record exists in the tree
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public bool Exists(Record Value)
        {
            return this.Exists(Value, this._Root);
        }

        /// <summary>
        /// Finds a page given a record value
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="SearchRoot"></param>
        /// <param name="SearchType"></param>
        /// <returns></returns>
        public Page Find(Record Value, Page SearchRoot, FindAffinity SearchType)
        {

            if (this.Affinity == TreeAffinity.Distinct || this.Affinity == TreeAffinity.Unique)
                return this.FindDistinct(Value, SearchRoot, SearchType);
            else if (SearchType == FindAffinity.First)
                return this.FindLooseFirst(Value, SearchRoot);
            else if (SearchType == FindAffinity.Last)
                return this.FindLooseLast(Value, SearchRoot);
            else
                return this.FindLooseAny(Value, SearchRoot);

        }

        /// <summary>
        /// Finds a page assuming all records are unique
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="SearchRoot"></param>
        /// <param name="SearchType"></param>
        /// <returns></returns>
        private Page FindDistinct(Record Value, Page SearchRoot, FindAffinity SearchType)
        {

            // Start at the root
            Page p = SearchRoot;

            // Check if we have the root
            if (p.X0 == Page.NULL_INDEX && p.X1 == Page.NULL_INDEX)
                return p;

            while (true)
            {

                DomainAffinity x = CalculateDomainAffinity(p, Value, this._Comparer);

                if (x == DomainAffinity.Everything || x == DomainAffinity.Inclusive || x == DomainAffinity.LowerInclusive || x == DomainAffinity.UpperInclusive)
                {
                    return p;
                }
                else if (x == DomainAffinity.UpperExclusive)
                {
                    if (p.X1 == Page.NULL_INDEX) return p;
                    p = this._Storeage.GetPage(p.X1);
                }
                else if (x == DomainAffinity.LowerExclusive)
                {
                    if (p.X0 == Page.NULL_INDEX) return p;
                    p = this._Storeage.GetPage(p.X0);
                }
                else if (x == DomainAffinity.Nothing)
                {
                    if (p.X0 != Page.NULL_INDEX) return this._Storeage.GetPage(p.X0);
                    if (p.X1 != Page.NULL_INDEX) return this._Storeage.GetPage(p.X1);
                    return p;
                }
                // Shouldnt get here //
                else
                {
                    throw new Exception();
                }

            }

        }

        /// <summary>
        /// Finds a record assuming pages are not unique
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="SearchRoot"></param>
        /// <param name="SearchType"></param>
        /// <returns></returns>
        private Page FindLooseFirst(Record Value, Page SearchRoot)
        {

            // Start at the root
            Page p = SearchRoot;

            // Check if we have the root
            if (p.X0 == Page.NULL_INDEX && p.X1 == Page.NULL_INDEX)
                return p;

            while (true)
            {

                DomainAffinity x = CalculateDomainAffinity(p, Value, this._Comparer);

                if (x == DomainAffinity.LowerExclusive)
                {
                    if (p.X0 != Page.NULL_INDEX)
                        p = this._Storeage.GetPage(p.X0);
                    else
                        return p;
                }
                else if (x == DomainAffinity.LowerInclusive)
                {
                    if (p.X0 != Page.NULL_INDEX)
                    {
                        Page q = this._Storeage.GetPage(p.X0);
                        x = CalculateDomainAffinity(q, Value, this._Comparer);
                        if (x == DomainAffinity.UpperInclusive)
                            return q;
                        p = q;
                    }
                    else
                    {
                        return p;
                    }
                }
                else if (x == DomainAffinity.Everything)
                {
                    if (p.X0 != Page.NULL_INDEX)
                    {
                        Page q = this._Storeage.GetPage(p.X0);
                        x = CalculateDomainAffinity(q, Value, this._Comparer);
                        if (x == DomainAffinity.UpperInclusive)
                            return q;
                        p = q;
                    }
                    else
                    {
                        return p;
                    }
                }
                else if (x == DomainAffinity.Inclusive)
                {
                    return p;
                }
                else if (x == DomainAffinity.UpperInclusive)
                {
                    return p;
                }
                else if (x == DomainAffinity.UpperExclusive)
                {
                    if (p.X1 != Page.NULL_INDEX)
                        p = this._Storeage.GetPage(p.X1);
                    else
                        return p;
                }
                else if (x == DomainAffinity.Nothing)
                {
                    if (p.X0 != Page.NULL_INDEX) return this._Storeage.GetPage(p.X0);
                    if (p.X1 != Page.NULL_INDEX) return this._Storeage.GetPage(p.X1);
                    return p;
                }
                else
                {
                    throw new Exception();
                }

                //return null;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="SearchRoot"></param>
        /// <returns></returns>
        private Page FindLooseAny(Record Value, Page SearchRoot)
        {

            // Start at the root
            Page p = SearchRoot;

            // Check if we have the root
            if (p.X0 == Page.NULL_INDEX && p.X1 == Page.NULL_INDEX)
                return p;
            
            while (true)
            {

                DomainAffinity x = CalculateDomainAffinity(p, Value, this._Comparer);
                
                if (x == DomainAffinity.LowerExclusive)
                {
                    if (p.X0 != Page.NULL_INDEX)
                        p = this._Storeage.GetPage(p.X0);
                    else
                        return p;
                }
                else if (x == DomainAffinity.LowerInclusive)
                {
                    return p;
                }
                else if (x == DomainAffinity.Everything)
                {
                    return p;
                }
                else if (x == DomainAffinity.Inclusive)
                {
                    return p;
                }
                else if (x == DomainAffinity.UpperInclusive)
                {
                    return p;
                }
                else if (x == DomainAffinity.UpperExclusive)
                {
                    if (p.X1 != Page.NULL_INDEX)
                        p = this._Storeage.GetPage(p.X1);
                    else
                        return p;
                }
                else if (x == DomainAffinity.Nothing)
                {
                    if (p.X0 != Page.NULL_INDEX) return this._Storeage.GetPage(p.X0);
                    if (p.X1 != Page.NULL_INDEX) return this._Storeage.GetPage(p.X1);
                    return p;
                }
                else
                {
                    throw new Exception();
                }
                
                //return null;

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="SearchRoot"></param>
        /// <returns></returns>
        private Page FindLooseLast(Record Value, Page SearchRoot)
        {

            // Start at the root
            Page p = SearchRoot;

            // Check if we have the root
            if (p.X0 == Page.NULL_INDEX && p.X1 == Page.NULL_INDEX)
                return p;

            while (true)
            {

                DomainAffinity x = CalculateDomainAffinity(p, Value, this._Comparer);

                if (x == DomainAffinity.LowerExclusive)
                {
                    if (p.X0 != Page.NULL_INDEX)
                    {
                        p = this._Storeage.GetPage(p.X0);
                    }
                    else
                    {
                        return p;
                    }
                }
                else if (x == DomainAffinity.LowerInclusive)
                {
                    return p;
                }
                else if (x == DomainAffinity.Everything)
                {
                    if (p.X1 != Page.NULL_INDEX)
                    {
                        Page q = this._Storeage.GetPage(p.X1);
                        x = this.CalculateDomainAffinity(q, Value, this._Comparer);
                        if (x == DomainAffinity.LowerInclusive)
                            return q;
                        p = q;
                    }
                    else
                    {
                        return p;
                    }
                }
                else if (x == DomainAffinity.Inclusive)
                {
                    return p;
                }
                else if (x == DomainAffinity.UpperInclusive)
                {
                    if (p.X1 != Page.NULL_INDEX)
                    {
                        Page q = this._Storeage.GetPage(p.X1);
                        x = this.CalculateDomainAffinity(q, Value, this._Comparer);
                        if (x == DomainAffinity.LowerInclusive)
                            return q;
                        p = q;
                    }
                    else
                    {
                        return p;
                    }
                }
                else if (x == DomainAffinity.UpperExclusive)
                {
                    if (p.X1 != Page.NULL_INDEX)
                    {
                        p = this._Storeage.GetPage(p.X1);
                    }
                    else
                    {
                        return p;
                    }
                }
                else if (x == DomainAffinity.Nothing)
                {
                    if (p.X0 != Page.NULL_INDEX) return this._Storeage.GetPage(p.X0);
                    if (p.X1 != Page.NULL_INDEX) return this._Storeage.GetPage(p.X1);
                    return p;
                }
                else
                {
                    throw new Exception();
                }

                //return null;

            }

        }

        /// <summary>
        /// Finds a page given a record value
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="SearchRoot"></param>
        /// <returns></returns>
        public Page Find(Record Value, Page SearchRoot)
        {
            return this.Find(Value, SearchRoot, FindAffinity.Any);
        }

        /// <summary>
        /// Finds a page given a record value
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="SearchType"></param>
        /// <returns></returns>
        public Page Find(Record Value, FindAffinity SearchType)
        {
            return this.Find(Value, this._Root, SearchType);
        }

        /// <summary>
        /// Finds a page given a record value
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public Page Find(Record Value)
        {
            return this.Find(Value, this._Root, FindAffinity.Any);
        }

        /// <summary>
        /// Finds a record's position
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Exact"></param>
        /// <returns></returns>
        public RecordKey FindFirst(Record Value, bool Exact)
        {
            Page p = this.Find(Value, this._Root, FindAffinity.First);
            int Index = p.Cache.BinarySearch(Value, this._Comparer);
            if (Index < 0 && Exact) 
            {
                return RecordKey.RecordNotFound;
            }
            else if (Index < 0)
            {
                Index = ~Index;
            }
            return new RecordKey(p.PageID, Index);
        }

        /// <summary>
        /// Finds a record's position
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public RecordKey FindFirst(Record Value)
        {
            return this.FindFirst(Value, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Exact"></param>
        /// <returns></returns>
        public RecordKey FindFirstKey(Record Key, bool Exact)
        {
            Record val = this.KeyToValue(Key);
            return this.FindFirst(val, Exact);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public RecordKey FindFirstKey(Record Key)
        {
            Record val = this.KeyToValue(Key);
            return this.FindFirst(val, false);
        }

        /// <summary>
        /// Finds a record's position
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Exact"></param>
        /// <returns></returns>
        public RecordKey FindLast(Record Value, bool Exact)
        {
            Page p = this.Find(Value, this._Root, FindAffinity.Last);
            int Index = p.Cache.BinarySearch(Value, this._Comparer);
            if (Index < 0 && Exact) 
            {
                return RecordKey.RecordNotFound;
            }
            else if (Index < 0)
            {
                Index = ~Index;
            }
            return new RecordKey(p.PageID, Index);
        }

        /// <summary>
        /// Finds a record's position
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public RecordKey FindLast(Record Value)
        {
            return this.FindLast(Value, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Exact"></param>
        /// <returns></returns>
        public RecordKey FindLastKey(Record Key, bool Exact)
        {
            Record val = this.KeyToValue(Key);
            return this.FindLast(val, Exact);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public RecordKey FindLastKey(Record Key)
        {
            Record val = this.KeyToValue(Key);
            return this.FindLast(val, false);
        }

        /// <summary>
        /// Handle's finding records given an empty page
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="EmptyRoot"></param>
        /// <param name="SearchType"></param>
        /// <returns></returns>
        private Page FindHandleEmptyPages(Record Value, Page EmptyRoot, FindAffinity SearchType)
        {

            return null;

        }

        // Rebalancing //
        /*
         *              A
         *             / \
         *         Parent  C
         *           /
         *        Radix
         *         /
         *     DownChild
         *        
         *              A
         *             / \
         *          Radix C
         *           / \
         *  DownChild  Parent
         *         
         *        
         * 
         */
        private void RebalanceDownRunOf3(Page Radix, Page Parent, Page DownChild)
        {

            // Note: this doesnt change the ordering of the nodes, it only changes the Down/Up links and the parent links
            this._MutateCounter++;

            // If radix has an up child, return
            if (Radix.X1 == Page.NULL_INDEX)
                return;

            // If the parent or down children are null, then return
            if (Parent == null || DownChild == null)
                return;

            // Check that the parent has no up child
            if (Parent.X1 == Page.NULL_INDEX)
                return;

            // Relink the nodes
            Radix.ParentPageID = Parent.ParentPageID;
            Radix.X1 = Parent.PageID;

            Parent.ParentPageID = Radix.PageID;
            Parent.X0 = Page.NULL_INDEX;

            if (Parent.ParentPageID != Page.NULL_INDEX)
            {
                Page GrandParent = this._Storeage.GetPage(Parent.ParentPageID);
                GrandParent.X0 = Radix.PageID;
            }


        }

        // Inserts //
        private void Insert(Page P, Record Value)
        {

            bool Contains = P.Cache.BinarySearch(Value, this._Comparer) > 0;
            if (Contains && this._Affinity == TreeAffinity.Unique)
            {
                throw new Exception(string.Format("Record already exists in '{0}'", (Value * this._Comparer)));
            }
            else if (Contains && this._Affinity == TreeAffinity.Distinct)
            {
                return;
            }

            P.InsertSequential(Value, this._Comparer);

        }

        // Deletes //
        private int DeleteFromSinglePage(Page P, Record Value)
        {

            int idx = P.Cache.BinarySearch(Value, this._Comparer);
            int DeleteCount = 0;
            while (idx > 0)
            {
                P.Delete(idx);
                idx = P.Cache.BinarySearch(Value, this._Comparer);
                DeleteCount++;
            }
            return DeleteCount;

        }

        // Splits //
        private Page Split(Page Value)
        {

            // Need three things:
            // (1). OrdinalPosition: Up if the page is up stream from the parent, down if the page is downstream from the root; down if the page is the root
            // (2). DownIsEmpty: true if there is no down page; false otherwise
            // (3). UpIsEmpty: true if the there is no up page; false otherwise

            this._MutateCounter++;
            
            PageAffinity OrdinalPosition = this.CalculatePageAffinity(Value);
            if (OrdinalPosition == PageAffinity.Root) OrdinalPosition = PageAffinity.Down;
            bool DownIsEmpty = (Value.X0 == Page.NULL_INDEX);
            bool UpIsEmpty = (Value.X1 == Page.NULL_INDEX);
            Page p;
            //string message = "";

            bool ValueIsOrigin = (this._Header.OriginPageID == Value.PageID);
            bool ValueIsTerminal = (this._Header.TerminalPageID == Value.PageID);

            if ((OrdinalPosition == PageAffinity.Down) && !DownIsEmpty && !UpIsEmpty)
            {
                //message = "Split_D_D_XX";
                p = this.Split_D_D_XX(Value);
            }
            else if ((OrdinalPosition == PageAffinity.Down) && DownIsEmpty && !UpIsEmpty)
            {
                //message = "Split_D_D_0X";
                p = this.Split_D_D_0X(Value);
            }
            else if ((OrdinalPosition == PageAffinity.Down) && !DownIsEmpty && UpIsEmpty)
            {
                //message = "Split_D_U_X0";
                p = this.Split_D_U_X0(Value);
            }
            else if ((OrdinalPosition == PageAffinity.Down) && DownIsEmpty && UpIsEmpty)
            {
                //message = "Split_D_D_00";
                p = this.Split_D_D_00(Value);
            }
            else if ((OrdinalPosition == PageAffinity.Up) && !DownIsEmpty && !UpIsEmpty)
            {
                //message = "Split_U_D_XX";
                p = this.Split_U_D_XX(Value);
            }
            else if ((OrdinalPosition == PageAffinity.Up) && DownIsEmpty && !UpIsEmpty)
            {
                //message = "Split_U_D_0X";
                p = this.Split_U_D_0X(Value);
            }
            else if ((OrdinalPosition == PageAffinity.Up) && !DownIsEmpty && UpIsEmpty)
            {
                //message = "Split_U_U_X0";
                p = this.Split_U_U_X0(Value);
            }
            else if ((OrdinalPosition == PageAffinity.Up) && DownIsEmpty && UpIsEmpty)
            {
                //message = "Split_U_D_00";
                p = this.Split_U_D_00(Value);
            }
            else
            {
                throw new Exception("Page is corrupt");
            }
            //Console.WriteLine("PageID={0};LastID={1};NextID={2};DownID={3};UpID={4};Parent={5};Method={6}", p.PageID, p.LastPageID, p.NextPageID, p.X0, p.X1, p.ParentPageID, message);

            Value.Version++;
            p.Version++;

            // Handle the origin and terminal pages
            if (ValueIsOrigin && p.LastPageID == Page.NULL_INDEX) this._Header.OriginPageID = p.PageID;
            if (ValueIsTerminal && p.NextPageID == Page.NULL_INDEX) this._Header.TerminalPageID = p.PageID;
            
            return p;

        }

        /*
        *                  A                
        *                /   \             
        *              [B]    C          
        *              / \   / \              
        *             D   E F   G              
        *
        *
        *                  A                        
        *                /   \               
        *              [B]    C               
        *              / \   / \                
        *            (H)  E F   G              
        *            /                               
        *           D                               
        * 
        * 
        * 
        */
        private Page Split_D_D_XX(Page Value)
        {

            // Get our pages
            Page B = Value;
            Page D = this._Storeage.GetPage(B.X0);
            //Page E = this._Storeage.GetPage(B.X1); 
            int LastPageID = this.SeekMaximum(D).PageID;
            Page H = B.SplitLower(this._Storeage.GenerateNewPageID, LastPageID, B.PageID);

            // Reset all the linking pointers 
            B.X0 = H.PageID;
            B.LastPageID = H.PageID;

            H.ParentPageID = B.PageID;
            H.X0 = D.PageID;
            H.X1 = Page.NULL_INDEX;

            D.ParentPageID = H.PageID;
            if (D.NextPageID == Page.NULL_INDEX)
                D.NextPageID = H.PageID;

            if (LastPageID != Page.NULL_INDEX)
            {
                Page p = this._Storeage.GetPage(LastPageID);
                p.NextPageID = H.PageID;
            }

            // Save
            this._Storeage.SetPage(H);
            this._Storeage.PageCount++;

            return H;

        }

        /*
        *                  A                           
        *                /   \                
        *              [B]    C                 
        *                \   / \                
        *                 E F   G                
        *             
        *                  A                       
        *                /   \                    
        *              [B]    C                
        *              / \    / \               
        *            (D)  E  F   G                 
        *             
        * 
        */
        private Page Split_D_D_0X(Page Value)
        {

            // Get our pages
            Page B = Value;
            int LastPageID = B.LastPageID;
            Page D = B.SplitLower(this._Storeage.GenerateNewPageID, LastPageID, B.PageID);

            // Reset all the linking pointers 
            B.X0 = D.PageID;
            B.LastPageID = D.PageID;

            D.ParentPageID = B.PageID;
            D.X0 = Page.NULL_INDEX;
            D.X1 = Page.NULL_INDEX;

            if (LastPageID != Page.NULL_INDEX)
            {
                Page p = this._Storeage.GetPage(LastPageID);
                p.NextPageID = D.PageID;
            }

            if (Value.PageID == 15) throw new Exception();

            // Save
            this._Storeage.SetPage(D);
            this._Storeage.PageCount++;

            return D;

        }

        /*
        *                 A                           
        *                / \                 
        *              [B]  C                 
        *                               
        *                 A                           
        *                / \                
        *              [B]  C                 
        *             /
        *           (D)                
        *             
        * 
        */
        private Page Split_D_D_00(Page Value)
        {

            // Get our pages
            Page B = Value;
            int LastPageID = B.LastPageID;
            Page D = B.SplitLower(this._Storeage.GenerateNewPageID, LastPageID, B.PageID);

            // Reset all the linking pointers 
            B.X0 = D.PageID;
            B.LastPageID = D.PageID;

            D.ParentPageID = B.PageID;
            D.X0 = Page.NULL_INDEX;
            D.X1 = Page.NULL_INDEX;

            if (LastPageID != Page.NULL_INDEX)
            {
                Page p = this._Storeage.GetPage(LastPageID);
                p.NextPageID = D.PageID;
            }

            // Try to flatten B
            if (B.ParentPageID != Page.NULL_INDEX)
            {
                Page A = this._Storeage.GetPage(B.ParentPageID);
                this.RebalanceDownRunOf3(B, A, D);
            }

            // Save
            this._Storeage.SetPage(D);
            this._Storeage.PageCount++;


            return D;

        }

        /*
        *                 A                           
        *                / \                
        *               B  [C]                
        *                               
        *                 A                           
        *                / \                
        *               B  [C]                
        *                  / 
        *                (D)         
        *             
        * 
        */
        private Page Split_U_D_00(Page Value)
        {

            // Get our pages
            Page C = Value;
            int LastPageID = C.LastPageID;
            Page D = C.SplitLower(this._Storeage.GenerateNewPageID, LastPageID, C.PageID);

            // Reset all the linking pointers 
            C.X0 = D.PageID;
            C.LastPageID = D.PageID;

            D.ParentPageID = C.PageID;
            D.X0 = Page.NULL_INDEX;
            D.X1 = Page.NULL_INDEX;

            // Check root //
            if (LastPageID != Page.NULL_INDEX)
            {
                Page A = this._Storeage.GetPage(LastPageID);
                A.NextPageID = D.PageID;
            }

            // Save
            this._Storeage.SetPage(D);
            this._Storeage.PageCount++;

            return D;

        }

        /*
        *                      A
        *                    /   \
        *                   B     [C]
        *                  / \    / \
        *                 D   E  F   G
        *
        *
        *                  A
        *                /   \
        *               B     [C]
        *              / \    / \
        *             D   E (H)  G 
        *                   /
        *                  F
        * 
        * 
        * 
        */
        private Page Split_U_D_XX(Page Value)
        {

            // Get our pages
            Page C = Value;
            Page F = this._Storeage.GetPage(C.X0);
            int LastPageID = this.SeekMaximum(F).PageID;
            Page H = C.SplitLower(this._Storeage.GenerateNewPageID, LastPageID, C.PageID);

            // Reset all the linking pointers 
            C.X0 = H.PageID;
            C.LastPageID = H.PageID;

            H.ParentPageID = C.PageID;
            H.X0 = F.PageID;
            H.X1 = Page.NULL_INDEX;

            F.ParentPageID = H.PageID;
            if (F.X1 == Page.NULL_INDEX)
                F.NextPageID = H.PageID;

            if (LastPageID != Page.NULL_INDEX)
            {
                Page p = this._Storeage.GetPage(LastPageID);
                p.NextPageID = H.PageID;
            }

            // Save
            this._Storeage.SetPage(H);
            this._Storeage.PageCount++;

            return H;

        }

        /*
        *                  A                           
        *                /   \                
        *               B     C                 
        *              / \     \                
        *             D   E     G                
        *             
        *                  A                       
        *                /   \                    
        *               B    [C]               
        *              / \   / \               
        *             D   E(F)  G                 
        *             
        * 
        */
        private Page Split_U_D_0X(Page Value)
        {

            // Get our pages
            Page C = Value;
            int LastPageID = C.LastPageID;
            Page F = C.SplitLower(this._Storeage.GenerateNewPageID, LastPageID, C.PageID);

            // Reset all the linking pointers 
            C.X0 = F.PageID;
            C.LastPageID = F.PageID;
            F.ParentPageID = C.PageID;
            F.X0 = Page.NULL_INDEX;
            F.X1 = Page.NULL_INDEX;

            // Check if not root
            if (LastPageID != Page.NULL_INDEX)
            {
                Page p = this._Storeage.GetPage(LastPageID);
                p.NextPageID = F.PageID;
            }

            // Save
            this._Storeage.SetPage(F);
            this._Storeage.PageCount++;

            return F;

        }

        /*
        *                  A                           
        *                /   \                
        *              [B]    C                 
        *              /     / \                
        *             D     F   G                
        *             
        *                  A                       
        *                /   \                    
        *              [B]    C                
        *              / \    / \               
        *             D  (E) F   G                 
        *             
        * 
        */
        private Page Split_D_U_X0(Page Value)
        {

            // Get our pages
            Page B = Value;
            int NextPageID = B.NextPageID;
            Page E = B.SplitUpper(this._Storeage.GenerateNewPageID, B.PageID, NextPageID);

            // Reset all the linking pointers 
            B.X1 = E.PageID;
            B.NextPageID = E.PageID;
            E.ParentPageID = B.PageID;
            E.X0 = Page.NULL_INDEX;
            E.X1 = Page.NULL_INDEX;

            // Check if not root
            if (NextPageID != Page.NULL_INDEX)
            {
                Page A = this._Storeage.GetPage(NextPageID);
                A.LastPageID = E.PageID;
            }

            // Save
            this._Storeage.SetPage(E);
            this._Storeage.PageCount++;

            return E;

        }

        /*
        *                  A                           
        *                /   \                
        *               B    [C]                
        *              / \   /                  
        *             D   E F                    
        *             
        *                  A                       
        *                /   \                    
        *              [B]    C                
        *              / \   / \               
        *             D   E F  (G)                
        *             
        * 
        */
        private Page Split_U_U_X0(Page Value)
        {

            // Get our pages
            Page C = Value;
            int NextPageID = C.NextPageID;
            Page G = C.SplitUpper(this._Storeage.GenerateNewPageID, C.PageID, NextPageID);

            // Reset all the linking pointers 
            C.X1 = G.PageID;
            C.NextPageID = G.PageID;

            G.ParentPageID = C.PageID;
            G.X0 = Page.NULL_INDEX;
            G.X1 = Page.NULL_INDEX;

            // Check if not root
            if (NextPageID != Page.NULL_INDEX)
            {
                Page p = this._Storeage.GetPage(NextPageID);
                p.LastPageID = G.PageID;
            }

            // Save
            this._Storeage.SetPage(G);
            this._Storeage.PageCount++;

            return G;

        }

        private DomainAffinity CalculateDomainAffinity(Page Values, Record SearchValue, Key Pattern)
        {

            if (Values.Count == 0)
                return DomainAffinity.Nothing;

            int lower = Record.Compare(SearchValue, Pattern, Values.OriginRecord, Pattern);
            int higher = Record.Compare(SearchValue, Pattern, Values.TerminalRecord, Pattern);

            if (lower == 0 && higher == 0)
                return DomainAffinity.Everything;
            else if (lower == 0)
                return DomainAffinity.LowerInclusive;
            else if (higher == 0)
                return DomainAffinity.UpperInclusive;
            else if (lower < 0)
                return DomainAffinity.LowerExclusive;
            else if (higher > 0)
                return DomainAffinity.UpperExclusive;
            else
                return DomainAffinity.Inclusive;

        }

        private PageAffinity CalculatePageAffinity(Page Value)
        {

            if (Value.ParentPageID == Page.NULL_INDEX)
                return PageAffinity.Root;

            Page Parent = this._Storeage.GetPage(Value.ParentPageID);

            if (Parent.X0 == Value.PageID)
                return PageAffinity.Down;

            if (Parent.X1 == Value.PageID)
                return PageAffinity.Up;

            throw new Exception("Page is corrupt");

        }

        private Page SeekMinimum(Page Value)
        {
            Page p = Value;
            while (true)
            {
                if (p.X0 == Page.NULL_INDEX)
                    return p;
                p = this._Storeage.GetPage(p.X0);
            }
            throw new Exception("Page Is Corrupt");
        }

        private Page SeekMaximum(Page Value)
        {
            Page p = Value;
            while (true)
            {
                if (p.X1 == Page.NULL_INDEX)
                    return p;
                p = this._Storeage.GetPage(p.X1);
            }
            throw new Exception("Page Is Corrupt");
        }

        private Page SeekBackDownParent(Page Value)
        {

            // Handle scenario where we're given the root
            if (Value.ParentPageID == Page.NULL_INDEX)
                return Value;

            // Get the child and parent pages
            Page Child = Value;
            Page Parent = this._Storeage.GetPage(Child.ParentPageID);

            while (true)
            {
                if (Parent.X0 == Child.PageID)
                    return Parent;
                if (Parent.ParentPageID == Page.NULL_INDEX)
                    return Parent;
                Child = Parent;
                Parent = this._Storeage.GetPage(Child.ParentPageID);
            }
            throw new Exception("Page Is Corrupt");

        }

        private Page SeekBackUpParent(Page Value)
        {

            // Handle scenario where we're given the root
            if (Value.ParentPageID == Page.NULL_INDEX)
                return Value;

            // Get the child and parent pages
            Page Child = Value;
            Page Parent = this._Storeage.GetPage(Child.ParentPageID);

            while (true)
            {
                if (Parent.X1 == Child.PageID)
                    return Parent;
                if (Parent.ParentPageID == Page.NULL_INDEX)
                    return Parent;
                Child = Parent;
                Parent = this._Storeage.GetPage(Child.ParentPageID);
            }
            throw new Exception("Page Is Corrupt");

        }

        // Debugs //
        public void DumpData(string Path)
        {

            using (StreamWriter sw = new StreamWriter(Path))
            {

                Page p = this.SeekOrigin();

                while (true)
                {

                    foreach (Record r in p.Cache)
                    {
                        sw.WriteLine(p.PageID.ToString() + ":" + r.ToString(','));
                    }
                    if (p.NextPageID == Page.NULL_INDEX)
                        break;
                    p = this._Storeage.GetPage(p.NextPageID);

                }

            }

        }

        public void DumpMeta(string Path)
        {

            using (StreamWriter sw = new StreamWriter(Path))
            {

                Page p = this.SeekOrigin();
                sw.WriteLine("PageID\tLastPageID\tNextPageID\tParentPageID\tDownPageID\tUpPageID\tRecordCount");
                while (true)
                {

                    string rec = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", p.PageID, p.LastPageID, p.NextPageID, p.ParentPageID, p.X0, p.X1, p.Count);
                    sw.WriteLine(rec);
                    if (p.NextPageID == Page.NULL_INDEX)
                        break;
                    p = this._Storeage.GetPage(p.NextPageID);

                }

            }

        }

        public string TreeString()
        {
            return this.TreeString(this._Root, 0);
        }

        public string TreeString(Page SubRoot, int Level)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{3}{0} : Down {1}, Up {2}", SubRoot.PageID, SubRoot.X0, SubRoot.X1, new string(' ', Level)));
            if (SubRoot.X0 != Page.NULL_INDEX)
                sb.AppendLine(this.TreeString(this._Storeage.GetPage(SubRoot.X0), Level + 1));
            if (SubRoot.X1 != Page.NULL_INDEX)
                sb.AppendLine(this.TreeString(this._Storeage.GetPage(SubRoot.X1), Level + 1));
            return sb.ToString();

        }

        // Statics //
        public static BinaryRecordTree CreateClusteredIndex(Table Parent, Key IndexColumns, TreeAffinity State)
        {
            return new BinaryRecordTree(Parent, IndexColumns, Parent.Columns, null, Parent.Header, State);
        }

        public static BinaryRecordTree OpenClusteredIndex(Table Parent)
        {

            if (Parent.Header.RootPageID == -1)
                throw new ArgumentException("Cannot open a clustered index; no such index exists");
            Key k = Parent.Header.ClusterKey;
            Page root = Parent.GetPage(Parent.Header.RootPageID);
            return new BinaryRecordTree(Parent, k, Parent.Columns, root, Parent.Header, TreeAffinity.Unconstrained);

        }

        public static BinaryRecordTree OpenNonClusteredIndex(Table StorageAndParent, IndexHeader Header)
        {

            Key k = Key.Build(Header.IndexColumns.Count);
            Schema s = Schema.Split(StorageAndParent.Columns, Header.IndexColumns);
            s.Add("@PTR", CellAffinity.LONG, 8, true);
            Page root = StorageAndParent.GetPage(Header.RootPageID);
            return new BinaryRecordTree(StorageAndParent, k, s, root, Header, TreeAffinity.Unconstrained);

        }

        public static Schema NonClusteredIndexColumns(Schema Columns, Key IndexColumns)
        {
            Key k = Key.Build(IndexColumns.Count);
            Schema s = Schema.Split(Columns, IndexColumns);
            s.Add("@PTR", CellAffinity.LONG, 8, true);
            return s;
        }

    }



}
