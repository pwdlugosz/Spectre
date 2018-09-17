using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Cells;
using Spectre.Structures;

namespace Spectre.Tables
{

    // Collections //
    public class IndexCollection
    {

        public const int MAX_INDEX_COUNT = 8;

        private Heap<TreeIndex> _Indexes;

        public IndexCollection()
        {
            this._Indexes = new Heap<TreeIndex>();
        }

        public IndexCollection(List<IndexHeader> Headers, Table Parent)
            : this()
        {
            foreach (IndexHeader h in Headers)
            {
                TreeIndex i = new TreeIndex(Parent, Parent, h);
                this.AddIndex(i);
            }
        }

        public TreeIndex this[string Name]
        {
            get { return this._Indexes[Name]; }
        }

        public TreeIndex this[int IndexOf]
        {
            get { return this._Indexes[IndexOf]; }
        }

        public int Count
        {
            get { return 0; }
        }

        public void AddIndex(TreeIndex Idx)
        {
            if (this.Count >= IndexCollection.MAX_INDEX_COUNT)
                throw new Exception("Cannot have more than eight indexes");
            this._Indexes.Allocate(Idx.Header.Name, Idx);
        }

        public void CreateIndex(Table Parent, string Name, Key Key)
        {
            if (this.Count >= IndexCollection.MAX_INDEX_COUNT)
                throw new Exception("Cannot have more than eight indexes");
            TreeIndex idx = new TreeIndex(Parent, Parent, Name, Key);
            this._Indexes.Allocate(Name, idx);
            Parent.Header.IndexHeaders.Add(idx.Header);
        }

        public void Insert(Record Element, RecordKey Key)
        {
            for (int i = 0; i < this._Indexes.Count; i++)
            {
                this._Indexes[i].Insert(Element, Key);
            }
        }

        public TreeIndex Optimize(Key Key)
        {

            for (int i = 0; i < this.Count; i++)
            {
                if (Key.EqualsWeak(this._Indexes[i].IndexColumns, Key))
                    return this._Indexes[i];
            }

            return null;

        }

    }

}
