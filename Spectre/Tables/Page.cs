﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Spectre.Cells;

namespace Spectre.Tables
{

    // PageManager //
    /// <summary>
    /// Base class for record storage
    /// </summary>
    public class Page
    {

        public const int BASE_PAGE_TYPE = 0;
        public const int SORTED_PAGE_TYPE = 1;
        public const int BTREE_PAGE_TYPE = 9;

        /* Page Header:
         * 0-4: HashKey (always 1)
         * 4-4: PageID
         * 8-4: LastPageID
         * 12-4: NextPageID
         * 16-4: PageSize
         * 20-4: FieldCount
         * 24-4: RecordCount
         * 28-4: CheckSum
         * 32-4: PageType
         * 36-4: UsedSpace
         * 40-4: X0
         * 44-4: X1
         * 48-4: X2
         * 52-4: X3
         * 56-1: B0
         * 57-1: B1
         * 58-1: B2
         * 59-1: B3
         * 60-4: Dead space
         * 
         */
        public const int HASH_KEY = 1;
        public const int OFFSET_HASH_KEY = 0;
        public const int OFFSET_PAGE_ID = 4;
        public const int OFFSET_LAST_ID = 8;
        public const int OFFSET_NEXT_ID = 12;
        public const int OFFSET_SIZE = 16;
        public const int OFFSET_FCOUNT = 20;
        public const int OFFSET_RCOUNT = 24;
        public const int OFFSET_CHECKSUM = 28;
        public const int OFFSET_TYPE = 32;
        public const int OFFSET_USED_SPACE = 36;
        public const int OFFSET_X0 = 40;
        public const int OFFSET_X1 = 44;
        public const int OFFSET_X2 = 48;
        public const int OFFSET_X3 = 52;
        public const int OFFSET_B0 = 56;
        public const int OFFSET_B1 = 57;
        public const int OFFSET_B2 = 58;
        public const int OFFSET_B3 = 59;
        public const int OFFSET_PARENT_PAGE_ID = 60;
        public const int OFFSET_RECORD_TABLE = 64;
        public const int SIZE_ELEMENT = 4;

        /// <summary>
        /// The null index is always -1
        /// </summary>
        public const int NULL_INDEX = -1;

        /// <summary>
        /// Default size is 64 KB
        /// </summary>
        public const int DEFAULT_SIZE = 64 * 1024; // 64k

        /// <summary>
        /// The minimum page size is 1 KB
        /// </summary>
        public const int MIN_PAGE_SIZE = 1024; // 1k

        /// <summary>
        /// The maximum page size is 1 MB
        /// </summary>
        public const int MAX_PAGE_SIZE = 1024 * 1024; // 1 MB

        /// <summary>
        /// The header size is 64 bytes
        /// </summary>
        public const int HEADER_SIZE = 64; // Currently using 40 bytes, 16 are taken by the 'X' variable and 8 are free for future use

        protected int _PageSize = DEFAULT_SIZE;
        protected int _PageID = 0;
        protected int _LastPageID = 0;
        protected int _NextPageID = 0;
        protected int _FieldCount = 0;
        protected int _Capacity = 0;
        protected int _UsedSpace = 0;
        protected int _CheckSum = 0;
        protected int _Type = BASE_PAGE_TYPE;
        protected int _X0 = 0;
        protected int _X1 = 0;
        protected int _X2 = 0;
        protected int _X3 = 0;
        protected byte _B0 = 0;
        protected byte _B1 = 0;
        protected byte _B2 = 0;
        protected byte _B3 = 0;
        protected int _ParentPageID = -1;

        protected long _Version = 0; // This never touches the disk; it incements each time the page is altered; if 0, the page was never altered

        protected List<Record> _Elements;

        public Page(int PageSize, int PageID, int LastPageID, int NextPageID, int FieldCount, int UsedSpace)
        {

            // Check some stuff //
            if (PageSize % 4096 != 0 || PageSize < 4096)
                throw new ArgumentException("PageSize must be a multiple of 4096 bytes and be greater than or equal to 4096 bytes");
            if (PageID < 0)
                throw new ArgumentException("PageID must be greater than 0");
            if (PageSize < MIN_PAGE_SIZE)
                throw new Exception(string.Format("PageSize must be at least {0}", MIN_PAGE_SIZE));
            if (PageSize > MAX_PAGE_SIZE)
                throw new Exception(string.Format("PageSize must be less than {0}", MAX_PAGE_SIZE));


            this._PageSize = PageSize;
            this._PageID = PageID;
            this._LastPageID = LastPageID;
            this._NextPageID = NextPageID;
            this._FieldCount = FieldCount;
            this._UsedSpace = UsedSpace;
            this._Capacity = 0;
            this._Elements = new List<Record>(this._Capacity);
            this._Version = 1;

        }

        // Non-Virtuals //
        /// <summary>
        /// Represents the number of times the page has been altered; if this is 0, the page doesnt need to be saved to disk
        /// </summary>
        public long Version
        {
            get { return this._Version; }
            set { this._Version = value; }
        }

        /// <summary>
        /// True if the page is the first link in a linked list; false otherwise
        /// </summary>
        public bool IsOrigin
        {
            get { return this.LastPageID == NULL_INDEX; }
        }

        /// <summary>
        /// True if the page is the last link in a linked list; false otherwise
        /// </summary>
        public bool IsTerminal
        {
            get { return this.NextPageID == NULL_INDEX; }
        }

        /// <summary>
        /// Gets the first records on the page
        /// </summary>
        public Record OriginRecord
        {

            get
            {
                if (this.IsEmpty)
                    throw new IndexOutOfRangeException("Page is empty");
                return this.Select(0);
            }

        }

        /// <summary>
        /// Gets the last record on the page
        /// </summary>
        public Record TerminalRecord
        {
            get
            {
                if (this.IsEmpty)
                    throw new IndexOutOfRangeException("Page is empty");
                return this.Select(this.Count - 1);
            }
        }

        /// <summary>
        /// Gets a record key for a given row id
        /// </summary>
        /// <param name="RowID"></param>
        /// <returns></returns>
        public RecordKey GetKey(int RowID)
        {
            return new RecordKey(this.PageID, RowID);
        }

        /// <summary>
        /// Gets the key for the first record
        /// </summary>
        /// <returns></returns>
        public RecordKey GetFirstKey()
        {
            return new RecordKey(this.PageID, 0);
        }

        /// <summary>
        /// Gets the key for the last record
        /// </summary>
        /// <returns></returns>
        public RecordKey GetLastKey()
        {
            return new RecordKey(this.PageID, this.Count == 0 ? 0 : this.Count - 1);
        }

        /// <summary>
        /// Finds the first record satisfying a condition
        /// </summary>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public Record SelectFirst(IRecordSeeker Filter)
        {
            return this.Select(this.SeekFirst(Filter));
        }

        /// <summary>
        /// Finds the last record satisfying a condition
        /// </summary>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public Record SelectLast(IRecordSeeker Filter)
        {
            return this.Select(this.SeekLast(Filter));
        }

        /// <summary>
        /// Finds all records satisfying a condition
        /// </summary>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public IEnumerable<Record> SelectAll(IRecordSeeker Filter)
        {

            int[] idx = this.Seek(Filter);
            List<Record> elements = new List<Record>();
            foreach (int i in idx)
            {
                elements.Add(this.Select(i));
            }
            return elements;

        }

        /// <summary>
        /// Deletes the first record satisfying a condition
        /// </summary>
        /// <param name="Filter"></param>
        public void DeleteFirst(IRecordSeeker Filter)
        {
            this.Delete(this.SeekFirst(Filter));
        }

        /// <summary>
        /// Deletes the last record satisfying a condition
        /// </summary>
        /// <param name="Filter"></param>
        public void DeleteLast(IRecordSeeker Filter)
        {
            this.Delete(this.SeekLast(Filter));
        }

        /// <summary>
        /// Deletes all the records satisfying a condition
        /// </summary>
        /// <param name="Filter"></param>
        public void DeleteAll(IRecordSeeker Filter)
        {
            int[] idx = this.Seek(Filter);
            foreach (int i in idx)
            {
                this.Delete(i);
            }
        }

        // Virtuals //
        /// <summary>
        /// True if the page has no records; false otherwise
        /// </summary>
        public virtual bool IsEmpty
        {
            get { return this.Count == 0; }
        }

        /// <summary>
        /// A count of the columns this page's records contain
        /// </summary>
        public virtual int FieldCount
        {
            get { return this._FieldCount; }
        }

        /// <summary>
        /// True if the page can accept the record size passed
        /// </summary>
        /// <param name="Size"></param>
        /// <returns></returns>
        public virtual bool CanAccept(int Size)
        {
            return Size <= this.FreeSpace;
        }

        /// <summary>
        /// True if the page can accept the record passed
        /// </summary>
        /// <param name="AWValue"></param>
        /// <returns></returns>
        public virtual bool CanAccept(Record Value)
        {
            return Value.DiskCost <= this.FreeSpace;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Size"></param>
        /// <param name="RowID"></param>
        /// <returns></returns>
        public virtual bool CanUpdate(int Size, int RowID)
        {
            return (Size - this._Elements[RowID].DiskCost) <= this.FreeSpace;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="RowID"></param>
        /// <returns></returns>
        public virtual bool CanUpdate(Record Value, int RowID)
        {
            return (Value.DiskCost - this._Elements[RowID].DiskCost) <= this.FreeSpace;
        }

        /// <summary>
        /// The amount of space used up (in bytes)
        /// </summary>
        public virtual int UsedSpace
        {
            get { return this._UsedSpace; }
        }

        /// <summary>
        /// The amount of space free (in bytes) excluding the header
        /// </summary>
        public virtual int FreeSpace
        {
            get { return this._PageSize - Page.HEADER_SIZE - this._UsedSpace; }
        }

        /// <summary>
        /// Represents a unique ID for the given page type
        /// </summary>
        public virtual int PageType
        {
            get { return this._Type; }
        }

        /// <summary>
        /// Represents the number of records a page contains
        /// </summary>
        public virtual int Count
        {
            get { return this._Elements.Count; }
        }

        /// <summary>
        /// Represents all records the page contains
        /// </summary>
        public virtual IEnumerable<Record> Elements
        {
            get { return this._Elements; }
        }

        /// <summary>
        /// The size in bytes of the page header
        /// </summary>
        public virtual int HeaderSize
        {
            get { return Page.HEADER_SIZE; }
        }

        /// <summary>
        /// The page ID preceding this page; -1 if the page is the first in a linked list
        /// </summary>
        public virtual int LastPageID
        {
            get { return this._LastPageID; }
            set
            {
                if ((value == this._PageID || value == this._NextPageID) && value != -1)
                    throw new ArgumentException(string.Format("LastPageID passed ({0}) cannot be the same as the PageID ({1}) or the NextPageID ({2})", value, this._PageID, this._NextPageID));
                this._LastPageID = value;
            }
        }

        /// <summary>
        /// The page ID of the next page; -1 if the page is the last in a linked list
        /// </summary>
        public virtual int NextPageID
        {
            get { return this._NextPageID; }
            set
            {
                if (this.PageID == 11 && value == 44) throw new Exception();
                if ((value == this._PageID || value == this._LastPageID) && value != -1)
                    throw new ArgumentException(string.Format("NextPageID passed ({0}) cannot be the same as the PageID ({1}) or the LastPageID ({2})", value, this._PageID, this._LastPageID));
                this._NextPageID = value;
            }
        }

        /// <summary>
        /// The ID of the Spike page
        /// </summary>
        public virtual int PageID
        {
            get { return this._PageID; }
        }

        /// <summary>
        /// The size in bytes of this page
        /// </summary>
        public virtual int PageSize
        {
            get { return this._PageSize; }
        }

        /// <summary>
        /// The page checksum; NOT IMPLEMENTED
        /// </summary>
        public virtual int CheckSum
        {
            get 
            {
                // Meta data cs //
                int cs = (this._PageSize + 1) ^ (this._PageID + 2) ^ (this._LastPageID + 3) ^ (this._NextPageID + 4);
                cs = (cs + 127) * 19470708 % (1 + this._FieldCount + this._Capacity + this._UsedSpace + this._Type);
                cs = cs ^ ((this._X0 + 1) * (this._X2 + 1) * (this._X2 + 1) * (this._X3 + 1) + (int)(this._B0 + this._B1 + this._B2 + this._B3));

                // Elements check sum //
                for (int i = 0; i < this._Elements.Count; i++)
                {
                    cs ^= (i * this._Elements[i].GetHashCode());
                }

                return cs; 
            }
        }

        /// <summary>
        /// The parent page's ID; for non tree tables, this is -1
        /// </summary>
        public virtual int ParentPageID
        {
            get { return this._ParentPageID; }
            set { this._ParentPageID = value; }
        }

        /// <summary>
        /// Removes a row from the page
        /// </summary>
        /// <param name="RowID"></param>
        public virtual void Delete(int RowID)
        {
            if (!CheckRowID(RowID))
                throw new IndexOutOfRangeException(string.Format("RowID is invalid: {0}", RowID));
            this._UsedSpace -= (this._Elements[RowID].DiskCost);
            this._Elements.RemoveAt(RowID);
            this._Version++;
        }

        /// <summary>
        /// Deletes the last record in the page
        /// </summary>
        public virtual void DeleteLast()
        {
            if (this.Count == 0)
                return;
            this._Elements.RemoveAt(this._Elements.Count - 1);
            this._Version++;
        }

        /// <summary>
        /// Inserts a row into the page at the end
        /// </summary>
        /// <param name="Key"></param>
        public virtual void Insert(Record Element)
        {

            if (!this.CanAccept(Element))
                throw new Exception("Page is full");
            this._Elements.Add(Element);
            this._Version++;
            this._UsedSpace += Element.DiskCost;

        }

        /// <summary>
        /// Inserts a record into the page at the begining
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="RowID"></param>
        public virtual void Insert(Record Element, int RowID)
        {

            if (!this.CanAccept(Element))
                throw new Exception("Page is full");
            if (RowID < 0 || RowID > this.Count)
                throw new IndexOutOfRangeException(string.Format("RowID is invalid: {0}", RowID));
            this._Elements.Insert(RowID, Element);
            this._Version++;
            this._UsedSpace += Element.DiskCost;

        }

        /// <summary>
        /// Inserts a record into the page sequentially
        /// </summary>
        /// <param name="Element"></param>
        /// <param name="Compare"></param>
        public virtual void InsertSequential(Record Element, Key Compare)
        {

            if (!this.CanAccept(Element))
                throw new Exception("Page is full");
            int idx = this._Elements.BinarySearch(Element, Compare);
            if (idx < 0) idx = ~idx;
            this._Elements.Insert(idx, Element);
            this._Version++;
            this._UsedSpace += Element.DiskCost;

        }

        /// <summary>
        /// Inserts a record into the page sequentially
        /// </summary>
        /// <param name="Element"></param>
        public virtual void InsertSequential(Record Element)
        {

            if (!this.CanAccept(Element))
                throw new Exception("Page is full");
            int idx = this._Elements.BinarySearch(Element);
            if (idx < 0) idx = ~idx;
            this._Elements.Insert(idx, Element);
            this._Version++;
            this._UsedSpace += Element.DiskCost;

        }

        /// <summary>
        /// Updates a record in the page
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="RowID"></param>
        public virtual void Update(Record Element, int RowID)
        {

            if (!CheckRowID(RowID))
                throw new IndexOutOfRangeException(string.Format("RowID is invalid: {0}", RowID));
            
            // See if we have the right size //
            int a = this._Elements[RowID].DiskCost;
            int b = Element.DiskCost;
            if (!this.CanAccept(b - a))
                throw new Exception("Cannot update this record");
            this._Elements[RowID] = Element;
            this._Version++;
            this._UsedSpace += (b - a);

        }

        /// <summary>
        /// Finds the first record in the page satisfying a condition
        /// </summary>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public virtual int SeekFirst(IRecordSeeker Filter)
        {

            int idx = 0;
            while (idx < this.Count)
            {
                if (Filter.Equals(this._Elements[idx]))
                    return idx;
                idx++;
            }
            return Page.NULL_INDEX;

        }

        /// <summary>
        /// Finds the last record in the page satisfying a condition
        /// </summary>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public virtual int SeekLast(IRecordSeeker Filter)
        {

            int idx = this.Count - 1;
            while (idx >= 0)
            {
                if (Filter.Equals(this._Elements[idx]))
                    return idx;
                idx--;
            }
            return Page.NULL_INDEX;

        }

        /// <summary>
        /// Finds all records in the page satisfying a condition
        /// </summary>
        /// <param name="Filter"></param>
        /// <returns></returns>
        public virtual int[] Seek(IRecordSeeker Filter)
        {

            List<int> idx = new List<int>();
            for (int i = 0; i < this.Count; i++)
            {
                if (Filter.Equals(this._Elements[i]))
                    idx.Add(i);
            }
            return idx.ToArray();

        }

        /// <summary>
        /// Gets a page from a given position
        /// </summary>
        /// <param name="RowID"></param>
        /// <returns></returns>
        public virtual Record Select(int RowID)
        {

            if (!CheckRowID(RowID))
                throw new IndexOutOfRangeException(string.Format("RowID is invalid: {0}", RowID));
            return this._Elements[RowID];

        }

        /// <summary>
        /// Sorts all records on the page
        /// </summary>
        /// <param name="ClusterKey"></param>
        public virtual void Sort(IRecordMatcher SortKey)
        {
            this._Elements.Sort(SortKey);
            this._Version++;
        }

        /// <summary>
        /// Finds the location of a given record on the page
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public virtual int Search(Record Element)
        {

            for (int i = 0; i < this._Elements.Count; i++)
            {
                if (Record.Compare(Element, this._Elements[i]) == 0)
                    return i;
            }
            return -1;

        }

        /// <summary>
        /// Creates a new page with a given ID and last/next ID
        /// </summary>
        /// <param name="PageID"></param>
        /// <param name="LastPageID"></param>
        /// <param name="NextPageID"></param>
        /// <returns></returns>
        public virtual Page Generate(int PageID, int LastPageID, int NextPageID)
        {
            return new Page(this.PageSize, PageID, LastPageID, NextPageID, this.FieldCount, 0);
        }

        /// <summary>
        /// Splits a page at a given point, returning a new page with the upper half of the data
        /// </summary>
        /// <param name="Pivot"></param>
        /// <param name="PageID"></param>
        /// <param name="LastPageID"></param>
        /// <param name="NewPageID"></param>
        /// <returns></returns>
        public virtual Page SplitUpper(int PageID, int LastPageID, int NextPageID, int Pivot)
        {

            if (this.Count < 2)
                throw new IndexOutOfRangeException("Cannot split a page with fewer than 2 records");
            if (Pivot == 0 || Pivot == this.Count - 1)
                throw new IndexOutOfRangeException("Cannot split on the first or last record");
            if (Pivot < 0)
                throw new IndexOutOfRangeException(string.Format("Pivot ({0}) must be greater than 0", Pivot));
            if (Pivot >= this.Count)
                throw new IndexOutOfRangeException(string.Format("The pivot ({0}) cannot be greater than the element count ({1})", Pivot, this.Count));

            Page p = this.Generate(PageID, LastPageID, NextPageID);
            for (int i = Pivot; i < this.Count; i++)
            {
                p._Elements.Add(this._Elements[i]);
                int Cost = this._Elements[i].DiskCost;
                p._UsedSpace += Cost;
                this._UsedSpace -= Cost;
            }
            this._Elements.RemoveRange(Pivot, this.Count - Pivot);
            this._Version++;
            p._Version++;
            return p;

        }

        /// <summary>
        /// Splits a page in half, returning the upper half of the page
        /// </summary>
        /// <param name="PageID"></param>
        /// <param name="LastPageID"></param>
        /// <param name="NextPageID"></param>
        /// <returns></returns>
        public virtual Page SplitUpper(int PageID, int LastPageID, int NextPageID)
        {
            return this.SplitUpper(PageID, LastPageID, NextPageID, this.Count / 2);
        }

        /// <summary>
        /// Splits a page at a given point, returning a new page with the lower half of the data
        /// </summary>
        /// <param name="PageID"></param>
        /// <param name="LastPageID"></param>
        /// <param name="NextPageID"></param>
        /// <param name="Pivot"></param>
        /// <returns></returns>
        public virtual Page SplitLower(int PageID, int LastPageID, int NextPageID, int Pivot)
        {

            if (this.Count < 2)
                throw new IndexOutOfRangeException("Cannot split a page with fewer than 2 records");
            if (Pivot == 0 || Pivot == this.Count - 1)
                throw new IndexOutOfRangeException("Cannot split on the first or last record");
            if (Pivot < 0)
                throw new IndexOutOfRangeException(string.Format("Pivot ({0}) must be greater than 0", Pivot));
            if (Pivot >= this.Count)
                throw new IndexOutOfRangeException(string.Format("The pivot ({0}) cannot be greater than the element count ({1})", Pivot, this.Count));

            Page p = this.Generate(PageID, LastPageID, NextPageID);
            for (int i = 0; i < Pivot; i++)
            {
                p._Elements.Add(this._Elements[i]);
                int Cost = this._Elements[i].DiskCost;
                p._UsedSpace += Cost;
                this._UsedSpace -= Cost;
            }
            this._Elements.RemoveRange(0, Pivot);
            this._Version++;
            p._Version++;
            return p;

        }

        /// <summary>
        /// Splits a page in half, returning the lower half of the page
        /// </summary>
        /// <param name="PageID"></param>
        /// <param name="LastPageID"></param>
        /// <param name="NextPageID"></param>
        /// <returns></returns>
        public virtual Page SplitLower(int PageID, int LastPageID, int NextPageID)
        {
            return this.SplitLower(PageID, LastPageID, NextPageID, this.Count / 2);
        }

        // Overrides //
        /// <summary>
        /// Returns the page id as the hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this._PageID;
        }

        /// <summary>
        /// Prints all data records to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {

            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (Record r in this._Elements)
            {
                sb.AppendLine(string.Format("{0} : {1}", i, r.ToString()));
                i++;
            }

            return sb.ToString();

        }

        // Debug methods //
        /// <summary>
        /// Creates a string with all values from the page listed in order
        /// </summary>
        /// <returns></returns>
        internal virtual string DebugDump()
        {

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < this._Elements.Count; i++)
            {
                sb.AppendLine(string.Format("{0} : {1}", i, this._Elements[i].ToString(',')));
            }
            return sb.ToString();

        }

        /// <summary>
        /// Gets a string element describing where the page is in the page change, and how many records are stored on it
        /// </summary>
        /// <returns></returns>
        internal string MapElement()
        {
            return string.Format("<{0},{1},{2}> : {3} of {4} : {5}", this.PageID, this.LastPageID, this.NextPageID, this.Count, this._Capacity, this.PageSize);
        }

        // Private Methods //
        /// <summary>
        /// True means the RowID is valid for 
        /// </summary>
        /// <param name="RowID"></param>
        /// <returns></returns>
        protected bool CheckRowID(int RowID)
        {
            return !(RowID < 0 || RowID >= this.Count);
        }

        /// <summary>
        /// Gets the X0 integer
        /// </summary>
        public int X0
        {
            get { return this._X0; }
            set { this._X0 = value; }
        }

        /// <summary>
        /// Gets the X1 integer
        /// </summary>
        public int X1
        {
            get { return this._X1; }
            set { this._X1 = value; }
        }

        /// <summary>
        /// Gets the X2 integer
        /// </summary>
        public int X2
        {
            get { return this._X2; }
            set { this._X2 = value; }
        }

        /// <summary>
        /// Gets the X3 integer
        /// </summary>
        public int X3
        {
            get { return this._X3; }
            set { this._X3 = value; }
        }

        /// <summary>
        /// B0 byte
        /// </summary>
        public byte B0
        {
            get { return this._B0; }
            set { this._B0 = value; }
        }

        /// <summary>
        /// B1 byte
        /// </summary>
        public byte B1
        {
            get { return this._B1; }
            set { this._B1 = value; }
        }

        /// <summary>
        /// B2 byte
        /// </summary>
        public byte B2
        {
            get { return this._B2; }
            set { this._B2 = value; }
        }

        /// <summary>
        /// B3 byte
        /// </summary>
        public byte B3
        {
            get { return this._B3; }
            set { this._B3 = value; }
        }

        /// <summary>
        /// If true, this page is stored in the page cache, false otherwise; this is used by internal processes to make sure any changes made to the page get saved back to disk
        /// </summary>
        public bool Cached
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the base record list
        /// </summary>
        public List<Record> Cache
        {
            get { return this._Elements; }
            set { this._Elements = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string PageDump()
        {


            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("PageSize: {0}", this.PageSize));
            sb.AppendLine(string.Format("PageID: {0}", this.PageID));
            sb.AppendLine(string.Format("LastPageID: {0}", this.LastPageID));
            sb.AppendLine(string.Format("NextPageID: {0}", this.NextPageID));
            sb.AppendLine(string.Format("UsedSpace: {0}", this.UsedSpace));
            sb.AppendLine(string.Format("FreeSpace: {0}", this.FreeSpace));
            //sb.AppendLine(string.Format("UsedSpace: {0}", this.));
            sb.AppendLine(string.Format("CheckSum: {0}", this.CheckSum));
            sb.AppendLine(string.Format("PageType: {0}", this.PageType));
            sb.AppendLine(string.Format("X0: {0}", this.X0));
            sb.AppendLine(string.Format("X1: {0}", this.X1));
            sb.AppendLine(string.Format("X2: {0}", this.X2));
            sb.AppendLine(string.Format("X3: {0}", this.X3));
            sb.AppendLine(string.Format("B0: {0}", this.B0));
            sb.AppendLine(string.Format("B1: {0}", this.B1));
            sb.AppendLine(string.Format("B2: {0}", this.B2));
            sb.AppendLine(string.Format("B3: {0}", this.B3));
            sb.AppendLine(string.Format("ParentPageID: {0}", this.ParentPageID));
            //sb.AppendLine("<Elements>");
            //foreach (Record r in this._Elements)
            //{
            //    sb.AppendLine(r.ToString());
            //}
            return sb.ToString();

        }

        // Hashing methods //
        /// <summary>
        /// Reads a page form a buffer
        /// </summary>
        /// <param name="Buffer"></param>
        /// <param name="Location"></param>
        /// <returns></returns>
        public static Page Read(byte[] Buffer, long Location)
        {

            // Check the hash key //
            int HashKey = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_HASH_KEY);
            if (HashKey != Page.HASH_KEY)
                throw new Exception("Hash key is invalid, cannot de-serialize");

            // Read Header Elements //
            int PageID = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_PAGE_ID);
            int LastPageID = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_LAST_ID);
            int NextPageID = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_NEXT_ID);
            int Size = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_SIZE);
            int FieldCount = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_FCOUNT);
            int Count = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_RCOUNT);
            int CheckSum = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_CHECKSUM);
            int PageType = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_TYPE);
            int UsedSpace = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_USED_SPACE);
            Location += HEADER_SIZE;

            Page element = new Page(Size, PageID, LastPageID, NextPageID, FieldCount, UsedSpace);
            element._CheckSum = CheckSum;
            element._Type = PageType;
            element._X0 = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_X0);
            element._X1 = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_X1);
            element._X2 = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_X2);
            element._X3 = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_X3);
            element._B0 = Buffer[(int)Location + OFFSET_B0];
            element._B1 = Buffer[(int)Location + OFFSET_B1];
            element._B2 = Buffer[(int)Location + OFFSET_B2];
            element._B3 = Buffer[(int)Location + OFFSET_B3];
            element._ParentPageID = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_PARENT_PAGE_ID);

            // Read in records //
            for (int k = 0; k < Count; k++)
            {

                // Array //
                Cell[] q = new Cell[FieldCount];

                // Get cells //
                for (int j = 0; j < FieldCount; j++)
                {

                    Cell C;

                    // Read the affinity //
                    CellAffinity a = (CellAffinity)Buffer[Location];
                    Location++;

                    // Read nullness //
                    bool b = (Buffer[Location] == 1);
                    Location++;

                    // If we are null, then exit
                    // for security reasons, we do not want to write any ghost data if the cell is null //
                    if (b == true)
                    {
                        C = new Cell(a);
                    }
                    else
                    {

                        // Cell b //
                        C = new Cell(a);
                        C.NULL = 0;

                        // BOOL //
                        if (a == CellAffinity.BOOL)
                        {
                            C.B0 = Buffer[Location];
                            Location++;
                        }

                        // BINARY //
                        else if (a == CellAffinity.BINARY)
                        {

                            C.B4 = Buffer[Location];
                            C.B5 = Buffer[Location + 1];
                            C.B6 = Buffer[Location + 2];
                            C.B7 = Buffer[Location + 3];
                            Location += 4;
                            byte[] blob = new byte[C.INT_B];
                            for (int i = 0; i < blob.Length; i++)
                            {
                                blob[i] = Buffer[Location];
                                Location++;
                            }
                            C = new Cell(blob);

                        }

                        // CSTRING //
                        else if (a == CellAffinity.CSTRING)
                        {

                            C.B4 = Buffer[Location];
                            C.B5 = Buffer[Location + 1];
                            C.B6 = Buffer[Location + 2];
                            C.B7 = Buffer[Location + 3];
                            Location += 4;
                            char[] chars = new char[C.INT_B];
                            for (int i = 0; i < C.INT_B; i++)
                            {
                                byte c1 = Buffer[Location];
                                Location++;
                                byte c2 = Buffer[Location];
                                Location++;
                                chars[i] = (char)(((int)c2) | (int)(c1 << 8));
                            }
                            C = new Cell(new string(chars));

                        }

                        // Double, Ints, Dates //
                        else
                        {
                            C.B0 = Buffer[Location];
                            C.B1 = Buffer[Location + 1];
                            C.B2 = Buffer[Location + 2];
                            C.B3 = Buffer[Location + 3];
                            C.B4 = Buffer[Location + 4];
                            C.B5 = Buffer[Location + 5];
                            C.B6 = Buffer[Location + 6];
                            C.B7 = Buffer[Location + 7];
                            Location += 8;
                        }

                    }

                    q[j] = C;

                }

                element._Elements.Add(new Record(q));

            }

            return element;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Buffer"></param>
        /// <param name="Location"></param>
        /// <returns></returns>
        public static Page Read2(byte[] Buffer, long Location)
        {

            // Check the hash key //
            int HashKey = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_HASH_KEY);
            if (HashKey != Page.HASH_KEY)
                throw new Exception("Hash key is invalid, cannot de-serialize");

            // Read Header Elements //
            int PageID = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_PAGE_ID);
            int LastPageID = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_LAST_ID);
            int NextPageID = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_NEXT_ID);
            int Size = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_SIZE);
            int FieldCount = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_FCOUNT);
            int Count = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_RCOUNT);
            int CheckSum = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_CHECKSUM);
            int PageType = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_TYPE);
            int UsedSpace = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_USED_SPACE);

            Page element = new Page(Size, PageID, LastPageID, NextPageID, FieldCount, UsedSpace);
            element._CheckSum = CheckSum;
            element._Type = PageType;
            element._X0 = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_X0);
            element._X1 = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_X1);
            element._X2 = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_X2);
            element._X3 = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_X3);
            element._B0 = Buffer[(int)Location + OFFSET_B0];
            element._B1 = Buffer[(int)Location + OFFSET_B1];
            element._B2 = Buffer[(int)Location + OFFSET_B2];
            element._B3 = Buffer[(int)Location + OFFSET_B3];
            element._ParentPageID = BitConverter.ToInt32(Buffer, (int)Location + OFFSET_PARENT_PAGE_ID);
            Location += HEADER_SIZE;

            // Read in records //
            for (int k = 0; k < Count; k++)
            {
                Record r = null;
                Location = CellSerializer.Read(Buffer, (int)Location, FieldCount, out r);
                element._Elements.Add(r);
            }

            return element;

        }

        /// <summary>
        /// Writes a page to a buffer
        /// </summary>
        /// <param name="Buffer"></param>
        /// <param name="Location"></param>
        /// <param name="Key"></param>
        public static void Write(byte[] Buffer, long Location, Page Element)
        {

            // Write the header data //
            Array.Copy(BitConverter.GetBytes(HASH_KEY), 0, Buffer, Location + OFFSET_HASH_KEY, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.PageID), 0, Buffer, Location + OFFSET_PAGE_ID, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.LastPageID), 0, Buffer, Location + OFFSET_LAST_ID, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.NextPageID), 0, Buffer, Location + OFFSET_NEXT_ID, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.PageSize), 0, Buffer, Location + OFFSET_SIZE, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.FieldCount), 0, Buffer, Location + OFFSET_FCOUNT, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.Count), 0, Buffer, Location + OFFSET_RCOUNT, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.CheckSum), 0, Buffer, Location + OFFSET_CHECKSUM, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.PageType), 0, Buffer, Location + OFFSET_TYPE, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.UsedSpace), 0, Buffer, Location + OFFSET_USED_SPACE, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element._X0), 0, Buffer, Location + OFFSET_X0, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element._X1), 0, Buffer, Location + OFFSET_X1, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element._X2), 0, Buffer, Location + OFFSET_X2, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element._X3), 0, Buffer, Location + OFFSET_X3, SIZE_ELEMENT);
            Buffer[Location + OFFSET_B0] = Element._B0;
            Buffer[Location + OFFSET_B1] = Element._B1;
            Buffer[Location + OFFSET_B2] = Element._B2;
            Buffer[Location + OFFSET_B3] = Element._B3;
            Array.Copy(BitConverter.GetBytes(Element._ParentPageID), 0, Buffer, Location + OFFSET_PARENT_PAGE_ID, SIZE_ELEMENT);
            Location += HEADER_SIZE;

            //int v = 0, w = 0;
            // Start writting the record data //
            foreach (Record R in Element._Elements)
            {

                //Console.WriteLine("{0} : {1} : {2}", v, R.DiskCost, (int)Location - w);
                //v++;
                //w = (int)Location;

                // Write each cell //
                for (int j = 0; j < R.Count; j++)
                {

                    Cell C = R[j];

                    // Write the affinity //
                    Buffer[Location] = ((byte)C.AFFINITY);
                    Location++;

                    // Write nullness //
                    Buffer[Location] = C.NULL;
                    Location++;

                    // If we are null, then exit
                    // for security reasons, we do not want to write any ghost data if the cell is null //
                    if (C.NULL == 0)
                    {

                        // Bool //
                        if (C.AFFINITY == CellAffinity.BOOL)
                        {
                            Buffer[Location] = (C.BOOL == true ? (byte)1 : (byte)0);
                            Location++;
                        }

                        // BINARY //
                        else if (C.AFFINITY == CellAffinity.BINARY)
                        {

                            C.INT_B = C.BINARY.Length;
                            Buffer[Location] = (C.B4);
                            Buffer[Location + 1] = (C.B5);
                            Buffer[Location + 2] = (C.B6);
                            Buffer[Location + 3] = (C.B7);
                            Location += 4;

                            for (int i = 0; i < C.BINARY.Length; i++)
                            {
                                Buffer[Location + i] = C.BINARY[i];
                            }

                            Location += C.BINARY.Length;

                        }

                        // CSTRING //
                        else if (C.AFFINITY == CellAffinity.CSTRING)
                        {

                            C.INT_B = C.CSTRING.Length;
                            Buffer[Location] = (C.B4);
                            Buffer[Location + 1] = (C.B5);
                            Buffer[Location + 2] = (C.B6);
                            Buffer[Location + 3] = (C.B7);
                            Location += 4;

                            for (int i = 0; i < C.CSTRING.Length; i++)
                            {
                                byte c1 = (byte)(C.CSTRING[i] >> 8);
                                byte c2 = (byte)(C.CSTRING[i] & 255);
                                Buffer[Location] = c1;
                                Location++;
                                Buffer[Location] = c2;
                                Location++;
                            }

                        }

                        // Double, int, date //
                        else
                        {

                            Buffer[Location] = C.B0;
                            Buffer[Location + 1] = C.B1;
                            Buffer[Location + 2] = C.B2;
                            Buffer[Location + 3] = C.B3;
                            Buffer[Location + 4] = C.B4;
                            Buffer[Location + 5] = C.B5;
                            Buffer[Location + 6] = C.B6;
                            Buffer[Location + 7] = C.B7;
                            Location += 8;
                        }

                    }

                }

            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Buffer"></param>
        /// <param name="Location"></param>
        /// <param name="Element"></param>
        public static void Write2(byte[] Buffer, long Location, Page Element)
        {

            // Write the header data //
            Array.Copy(BitConverter.GetBytes(HASH_KEY), 0, Buffer, Location + OFFSET_HASH_KEY, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.PageID), 0, Buffer, Location + OFFSET_PAGE_ID, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.LastPageID), 0, Buffer, Location + OFFSET_LAST_ID, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.NextPageID), 0, Buffer, Location + OFFSET_NEXT_ID, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.PageSize), 0, Buffer, Location + OFFSET_SIZE, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.FieldCount), 0, Buffer, Location + OFFSET_FCOUNT, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.Count), 0, Buffer, Location + OFFSET_RCOUNT, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.CheckSum), 0, Buffer, Location + OFFSET_CHECKSUM, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.PageType), 0, Buffer, Location + OFFSET_TYPE, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element.UsedSpace), 0, Buffer, Location + OFFSET_USED_SPACE, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element._X0), 0, Buffer, Location + OFFSET_X0, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element._X1), 0, Buffer, Location + OFFSET_X1, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element._X2), 0, Buffer, Location + OFFSET_X2, SIZE_ELEMENT);
            Array.Copy(BitConverter.GetBytes(Element._X3), 0, Buffer, Location + OFFSET_X3, SIZE_ELEMENT);
            Buffer[Location + OFFSET_B0] = Element._B0;
            Buffer[Location + OFFSET_B1] = Element._B1;
            Buffer[Location + OFFSET_B2] = Element._B2;
            Buffer[Location + OFFSET_B3] = Element._B3;
            Array.Copy(BitConverter.GetBytes(Element._ParentPageID), 0, Buffer, Location + OFFSET_PARENT_PAGE_ID, SIZE_ELEMENT);
            Location += HEADER_SIZE;

            // Start writting the record data //
            foreach (Record R in Element._Elements)
            {
                Location = CellSerializer.Write(Buffer, R, (int)Location);
            }


        }


    }

}
