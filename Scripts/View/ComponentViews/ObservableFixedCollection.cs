using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ConnectAPIC.Scripts.View.ComponentViews
{
    public class ObservableFixedCollection<T> : ObservableCollection<T>
    {
        public ObservableFixedCollection(IEnumerable<T> collection) : base(collection)
        {
        }

        public ObservableFixedCollection() : base()
        {
        }

        protected override void ClearItems()
        {
            throw new NotSupportedException("Removing items is not supported.");
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            throw new NotSupportedException("Removing items is not supported.");
        }

        protected override void SetItem(int index, T item)
        {
            throw new NotSupportedException("Replace items is not supported.");
        }
    }
}
