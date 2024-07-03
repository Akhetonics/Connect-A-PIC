using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_Core.Helpers
{
    public class UniqueObservableCollection<T> : ObservableCollection<T> where T : IEquatable<T>
    {
        protected override void InsertItem(int index, T item)
        {
            if (IsItemInCollection(item)) return;
            base.InsertItem(index, item);
        }

        private bool IsItemInCollection(T item)
        {
            foreach (T existingItem in Items)
            {
                if (existingItem.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }
        protected override void ClearItems()
        {
            while (Items.Count > 0)
            {
                RemoveItem(0);
            }
        }

        protected override void SetItem(int index, T item)
        {
            if (IsItemInCollection(item)) return;
            base.SetItem(index, item);
        }
    }
}
