using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Cells;
using Spectre.Control;

namespace Spectre.Tables
{

    /// <summary>
    /// Represents a disk based key-value table
    /// </summary>
    public class DictionaryTable : TreeTable, IReadable
    {

        //protected int _KeyCount = 0;
        //protected int _ValueCount = 0;
        protected Key _KeyFields;
        protected Key _ValueFields;

        /*
         * The _LastRef and _LastKey store the last referenced record key and row pointer. This is used to speed up opperations when the dictionary is used as the 
         * temporary storage for aggregate data. This only optimizes 
         * -- Often time the program will first request the value, and if it does exist, it will try to update the value; this causes two index seeks that could get
         *      reduced to one if we check the last key.
         * -- This also offers a large speed up if we're dealing with ordered or partially ordered data
         */
        protected RecordKey _LastRef = RecordKey.RecordNotFound;
        protected Record _LastKey = null;
        
        public DictionaryTable(Host Host, TableHeader Header)
            : base(Host, Header)
        {
            this._KeyFields = Header.ClusterKey;
            this._ValueFields = this._ValueFields.Mirror(this.Columns.Count);
            this._TableType = "DICTIONARY_SCRIBE";
        }

        public DictionaryTable(Host Host, string Path, Schema Columns, Key KeyColumns, int PageSize)
            : base(Host, Path, Columns, KeyColumns, BinaryRecordTree.TreeAffinity.Unique, PageSize)
        {
            //this._KeyCount = KeyColumns.Count;
            //this._ValueCount = ValueColumns.Count;
            this._KeyFields = KeyColumns;

            this._ValueFields = KeyColumns.Mirror(Columns.Count);
            this._TableType = "DICTIONARY_SCRIBE";
        }

        // Methods not implemented //   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Alias"></param>
        /// <param name="IndexColumns"></param>
        public override void CreateIndex(string Name, Key IndexColumns)
        {
            throw new Exception("Cannot create indexes on clustered tables");
        }

        /// <summary>
        /// Splits a table into N sub tables
        /// </summary>
        /// <param name="PartitionIndex"></param>
        /// <param name="PartitionCount"></param>
        /// <returns></returns>
        public override Table Partition(int PartitionIndex, int PartitionCount)
        {
            throw new NotImplementedException();
        }

        // Dictionary Methods //
        /// <summary>
        /// Returns a key containing all the key fields
        /// </summary>
        public Key KeyFields
        {
            get { return this._KeyFields; }
        }

        /// <summary>
        /// Returns a key containing all the value fields
        /// </summary>
        public Key ValueFields
        {
            get { return this._ValueFields; }
        }

        /// <summary>
        /// Adds a key-value pair
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="AWValue"></param>
        public void Add(Record Key, Record Value)
        {

            // Step the version //
            this._Version++;

            // Check that everything's ok //
            if (Key.Count != this._KeyFields.Count || Value.Count != this._ValueFields.Count)
                throw new ArgumentException("Key or value passed is/are invalid");

            Record r = Record.Join(Key, Value);

            this.Insert(r);

        }

        /// <summary>
        /// Sets a value
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="AWValue"></param>
        public void SetValue(Record Key, Record Value)
        {

            // Step the version //
            this._Version++;

            // Get the final record value //
            Record r = Record.Join(Key, Value);

            // If exists, then update, otherwise add
            RecordKey ptr = this._Cluster.FindFirstKey(Key, true);
            if (ptr.IsNotFound)
            {
                this.Insert(r);
            }
            else
            {
                this._Cluster.Update(ptr, r);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Composite"></param>
        public void SetValue(Record Composite)
        {
            // Step the version //
            this._Version++;
            
            // If exists, then update, otherwise add
            RecordKey ptr = this._Cluster.FindFirst(Composite, true);
            if (ptr.IsNotFound)
            {
                this.Insert(Composite);
            }
            else
            {
                this._Cluster.Update(ptr, Composite);
            }
        }
        
        /// <summary>
        /// Gets a value from the tree
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public Record GetValue(Record Key)
        {

            Record x = this.GetKeyValue(Key);

            if (x == null)
                return null;

            return Record.Split(x, this._ValueFields);

        }

        /// <summary>
        /// Gets a record, invluding the key and value
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public Record GetKeyValue(Record Key)
        {

            // Check the last key first //
            if (this._LastKey != null)
            {
                if (Record.Compare(Key, this._LastKey) == 0)
                {
                    return this._Cluster.Storage.GetPage(this._LastRef.PAGE_ID).Select(this._LastRef.ROW_ID);
                }
            }

            // Get the record key //
            RecordKey x = this._Cluster.FindFirstKey(Key, true);

            // This should really only trigger if the table is empty; actually, this should never trigger //
            if (x.IsNotFound)
                return null;

            // Select the record, and if it's null, return //
            Record y = this._Cluster.Storage.TrySelect(x);
            if (y == null)
                return null;

            // Need to check if we didn't find the actual record //
            if (Record.Compare(y, Key, this._KeyFields) == 0)
            {

                this._LastRef = x;
                this._LastKey = Key;
                return y;
            }

            // Not found //
            return null;

        }

        /// <summary>
        /// Checks if the table contains a key
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool ContainsKey(Record Key)
        {
            return this.GetKeyValue(Key) == null;
        }
        
        /// <summary>
        /// Gets a value from the tree
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public Record GetValue2(Record Composite)
        {

            Record x = this.GetKeyValue(Composite);

            if (x == null)
                return null;

            return Record.Split(x, this._ValueFields);

        }

        /// <summary>
        /// Gets a record, invluding the key and value
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public Record GetKeyValue2(Record Composite)
        {

            // Check the last key first //
            if (this._LastKey != null)
            {
                if (Record.Compare(Record.Split(Composite, this._KeyFields), this._LastKey) == 0)
                {
                    return this._Cluster.Storage.GetPage(this._LastRef.PAGE_ID).Select(this._LastRef.ROW_ID);
                }
            }

            // Get the record key //
            RecordKey x = this._Cluster.FindFirst(Composite, true);

            // This should really only trigger if the table is empty; actually, this should never trigger //
            if (x.IsNotFound)
                return null;

            // Select the record, and if it's null, return //
            Record y = this._Cluster.Storage.TrySelect(x);
            if (y == null)
                return null;

            // Need to check if we didn't find the actual record //
            if (Record.Compare(y, Composite, this._KeyFields) == 0)
            {

                this._LastRef = x;
                this._LastKey = Record.Split(Composite, this._KeyFields);
                return y;
            }

            // Not found //
            return null;

        }

        /// <summary>
        /// Checks if the table contains a key
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public bool ContainsKey2(Record Composite)
        {
            return this.GetKeyValue2(Composite) != null;
        }


    }

}
