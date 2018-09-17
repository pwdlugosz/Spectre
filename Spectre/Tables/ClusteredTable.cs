﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Cells;
using Spectre.Control;

namespace Spectre.Tables
{

    /// <summary>
    /// Creates a table sorted usina a b+tree that can be saved to disk
    /// </summary>
    //public class ClusteredTable : Table, IReadable
    //{

    //    protected Cluster _Cluster;

    //    /// <summary>
    //    /// This method should be used for creating a table object from an existing table on disk
    //    /// </summary>
    //    /// <param name="Host"></param>
    //    /// <param name="Header"></param>
    //    /// <param name="ClusterKey"></param>
    //    public ClusteredTable(Host Host, TableHeader Header)
    //        : base(Host, Header)
    //    {

    //        if (Header.RootPageID == -1)
    //            throw new ArgumentException("The root page ID cannot be null");

    //        // Get the sort key //
    //        Key k = Header.ClusterKey;

    //        // Get the root page ID //
    //        ClusterPage root = ClusterPage.Mutate(this.GetPage(Header.RootPageID), k);

    //        // Cluster //
    //        this._Cluster = new Cluster(this, this.Columns, k, root, this.Header, Header.ClusterKeyState);

    //        this._TableType = "CLUSTER_SCRIBE";

    //    }

    //    /// <summary>
    //    /// This method should be used for creating a brand new table object
    //    /// </summary>
    //    /// <param name="Host"></param>
    //    /// <param name="Alias"></param>
    //    /// <param name="Dir"></param>
    //    /// <param name="Columns"></param>
    //    /// <param name="PageSize"></param>
    //    /// <param name="ClusterKey"></param>
    //    public ClusteredTable(Host Host, string Path, Schema Columns, Key ClusterColumns, ClusterState State, int PageSize)
    //        : base(Host, Path, Columns, PageSize)
    //    {

    //        this._Cluster = Cluster.CreateClusteredIndex(this, ClusterColumns, State);
    //        this._TableType = "CLUSTER_SCRIBE";
    //        this._Header.ClusterKey = ClusterColumns;

    //    }

    //    /// <summary>
    //    /// This method should be used for creating a brand new table object
    //    /// </summary>
    //    /// <param name="Host"></param>
    //    /// <param name="Alias"></param>
    //    /// <param name="Dir"></param>
    //    /// <param name="Columns"></param>
    //    /// <param name="ClusterColumns"></param>
    //    public ClusteredTable(Host Host, string Path, Schema Columns, Key ClusterColumns, ClusterState State)
    //        : this(Host, Path, Columns, ClusterColumns, State, Page.DEFAULT_SIZE)
    //    {
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="Host"></param>
    //    /// <param name="Alias"></param>
    //    /// <param name="Dir"></param>
    //    /// <param name="Columns"></param>
    //    /// <param name="ClusterColumns"></param>
    //    public ClusteredTable(Host Host, string Path, Schema Columns, Key ClusterColumns)
    //        : this(Host, Path, Columns, ClusterColumns, ClusterState.Universal, Page.DEFAULT_SIZE)
    //    {
    //    }

    //    /// <summary>
    //    /// Inner B+Tree
    //    /// </summary>
    //    public Cluster BaseTree
    //    {
    //        get { return this._Cluster; }
    //    }

    //    /// <summary>
    //    /// Appends a record to a table
    //    /// </summary>
    //    /// <param name="AWValue"></param>
    //    public override void Insert(Record Value)
    //    {

    //        // Step the version //
    //        this._Version++;

    //        this._Cluster.Insert(Value);

    //    }

    //    /// <summary>
    //    /// Opens a record reader that focuses on a single key
    //    /// </summary>
    //    /// <param name="Key"></param>
    //    /// <returns></returns>
    //    public override RecordReader OpenReader(Record Key)
    //    {

    //        if (Key.Count != this._Cluster.IndexColumns.Count)
    //            return this.OpenReader();
    //        RecordKey l = this._Cluster.SeekFirst(Key, false);
    //        RecordKey u = this._Cluster.SeekLast(Key, false);
    //        return new RecordReaderBase(this, l, u);

    //    }

    //    /// <summary>
    //    /// Opens a record reader to focus on records between A and B (inclusive)
    //    /// </summary>
    //    /// <param name="LKey"></param>
    //    /// <param name="UKey"></param>
    //    /// <returns></returns>
    //    public override RecordReader OpenReader(Record LKey, Record UKey)
    //    {

    //        if (LKey.Count != UKey.Count || LKey.Count != this._Cluster.IndexColumns.Count)
    //            return this.OpenReader();
    //        RecordKey lk = this._Cluster.SeekFirst(LKey, false);
    //        RecordKey uk = this._Cluster.SeekLast(UKey, false);
    //        return new RecordReaderBase(this, lk, uk);

    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="Alias"></param>
    //    /// <param name="IndexColumns"></param>
    //    public override void CreateIndex(string Name, Key IndexColumns)
    //    {
    //        throw new Exception("Cannot create indexes on clustered tables");
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="Alias"></param>
    //    /// <returns></returns>
    //    public override Index GetIndex(string Name)
    //    {
    //        throw new Exception("Cannot request indexes on clustered tables");
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="IndexColumns"></param>
    //    /// <returns></returns>
    //    public override Index GetIndex(Key IndexColumns)
    //    {
    //        if (Tables.Key.EqualsStrong(IndexColumns, this._Header.ClusterKey))
    //            return new DerivedIndex(this);
    //        return null;
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public override void PreSerialize()
    //    {
    //        this.SetPage(this._Cluster.Root);
    //    }

    //    // Methods not implemented //
    //    /// <summary>
    //    /// Splits a table into N sub tables
    //    /// </summary>
    //    /// <param name="PartitionIndex"></param>
    //    /// <param name="PartitionCount"></param>
    //    /// <returns></returns>
    //    public override Table Partition(int PartitionIndex, int PartitionCount)
    //    {
    //        throw new NotImplementedException();
    //    }

    //}

    public class TreeTable : Table, IReadable
    {

        protected BinaryRecordTree _Cluster;

        /// <summary>
        /// This method should be used for creating a table object from an existing table on disk
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="Header"></param>
        /// <param name="ClusterKey"></param>
        public TreeTable(Host Host, TableHeader Header)
            : base(Host, Header)
        {

            if (Header.RootPageID == -1)
                throw new ArgumentException("The root page ID cannot be null");

            // Get the sort key //
            Key k = Header.ClusterKey;

            // Get the root page ID //
            Page root = this.GetPage(Header.RootPageID);

            // Cluster //
            BinaryRecordTree.TreeAffinity x = this._Header.ClusterKeyState;
            this._Cluster = new BinaryRecordTree(this, k, this.Columns, root, this.Header, x);

            this._TableType = "CLUSTER_SCRIBE";

        }

        /// <summary>
        /// This method should be used for creating a brand new table object
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="Alias"></param>
        /// <param name="Dir"></param>
        /// <param name="Columns"></param>
        /// <param name="PageSize"></param>
        /// <param name="ClusterKey"></param>
        public TreeTable(Host Host, string Path, Schema Columns, Key ClusterColumns, BinaryRecordTree.TreeAffinity State, int PageSize)
            : base(Host, Path, Columns, PageSize)
        {

            this._Cluster = BinaryRecordTree.CreateClusteredIndex(this, ClusterColumns, State);
            this._TableType = "CLUSTER_SCRIBE";
            this._Header.ClusterKey = ClusterColumns;

        }

        /// <summary>
        /// This method should be used for creating a brand new table object
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="Alias"></param>
        /// <param name="Dir"></param>
        /// <param name="Columns"></param>
        /// <param name="ClusterColumns"></param>
        public TreeTable(Host Host, string Path, Schema Columns, Key ClusterColumns, BinaryRecordTree.TreeAffinity State)
            : this(Host, Path, Columns, ClusterColumns, State, Page.DEFAULT_SIZE)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="Alias"></param>
        /// <param name="Dir"></param>
        /// <param name="Columns"></param>
        /// <param name="ClusterColumns"></param>
        public TreeTable(Host Host, string Path, Schema Columns, Key ClusterColumns)
            : this(Host, Path, Columns, ClusterColumns, BinaryRecordTree.TreeAffinity.Unconstrained, Page.DEFAULT_SIZE)
        {
        }

        /// <summary>
        /// Inner B+Tree
        /// </summary>
        public BinaryRecordTree BaseTree
        {
            get { return this._Cluster; }
        }

        /// <summary>
        /// Appends a record to a table
        /// </summary>
        /// <param name="AWValue"></param>
        public override void Insert(Record Value)
        {

            // Step the version //
            this._Version++;

            this._Cluster.Insert(Value);

        }

        /// <summary>
        /// Opens a record reader that focuses on a single key
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public override RecordReader OpenReader(Record Key)
        {

            if (Key.Count != this._Cluster.Comparer.Count)
                return this.OpenReader();
            RecordKey l = this._Cluster.FindFirstKey(Key, false);
            RecordKey u = this._Cluster.FindLastKey(Key, false);
            return new RecordReaderBase(this, l, u);

        }

        /// <summary>
        /// Opens a record reader to focus on records between A and B (inclusive)
        /// </summary>
        /// <param name="LKey"></param>
        /// <param name="UKey"></param>
        /// <returns></returns>
        public override RecordReader OpenReader(Record LKey, Record UKey)
        {

            if (LKey.Count != UKey.Count || LKey.Count != this._Cluster.Comparer.Count)
                return this.OpenReader();
            RecordKey lk = this._Cluster.FindFirstKey(LKey, false);
            RecordKey uk = this._Cluster.FindLastKey(UKey, false);
            return new RecordReaderBase(this, lk, uk);

        }

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
        /// 
        /// </summary>
        /// <param name="Alias"></param>
        /// <returns></returns>
        public override TreeIndex GetIndex(string Name)
        {
            throw new Exception("Cannot request indexes on clustered tables");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IndexColumns"></param>
        /// <returns></returns>
        public override TreeIndex GetIndex(Key IndexColumns)
        {
            if (Tables.Key.EqualsStrong(IndexColumns, this._Header.ClusterKey))
                return new ClusteredIndex(this);
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void PreSerialize()
        {
            this.SetPage(this._Cluster.Root);
        }

        // Methods not implemented //
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

    }


}
