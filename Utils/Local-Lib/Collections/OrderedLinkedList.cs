namespace System.Collections.Generic
{
    public interface ILinkable<T>
        where T : class
    {
        T Previous { get; set; }
        T Next { get; set; }
    }

    public class OrderedLinkedList<T> : OrderedList<T>
        where T : class, ILinkable<T>
    {
        #region Constructors

        public OrderedLinkedList(bool allowDups = true)
            : base(new List<T>(), Comparer<T>.Default, allowDups)
        {
        }

        public OrderedLinkedList(IComparer<T> comparer, bool allowDups = true)
            : base(new List<T>(), comparer, allowDups)
        {
        }

        public OrderedLinkedList(int capacity, IComparer<T> comparer, bool allowDups = true)
            : base(new List<T>(capacity), comparer, allowDups)
        {
        }

        public OrderedLinkedList(int capacity, bool allowDups = true)
            : base(new List<T>(capacity), Comparer<T>.Default, allowDups)
        {
        }

        public OrderedLinkedList(IEnumerable<T> collection, bool allowDups = true)
            : this(collection, Comparer<T>.Default, allowDups)
        {
        }

        public OrderedLinkedList(IEnumerable<T> collection, IComparer<T> comparer, bool allowDups = true)
            : base(new List<T>(collection), comparer, allowDups)
        {
            Relink();
        }
        
        #endregion

        private void Link(int index)
        {
            var item = this[index];

            item.Previous = index == 0 ? null : this[index - 1];
            if (item.Previous != null)
                item.Previous.Next = item;

            item.Next = index == Count - 1 ? null : this[index + 1];
            if (item.Next != null)
                item.Next.Previous = item;
        }

        private void LinkRange(int startIndex, int count)
        {
            for (int i = startIndex; i < startIndex + count; i++)
            {
                var item = this[i];
                item.Previous = i == 0 ? null : this[i - 1];
                item.Next = i == Count - 1 ? null : this[i + 1];
            }
        }

        public override int Add(T item)
        {
            int index = base.Add(item);
            Link(index);
            return index;
        }

        // TODO(local): more efficient linking plz
        public override void AddRange(IEnumerable<T> collection)
        {
            base.AddRange(collection);
            LinkRange(0, Count);
        }

        public override void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                var item = this[i];
                item.Previous = item.Next = null;
            }

            base.Clear();
        }

        public override bool Remove(T item)
        {
            int index = BinarySearch(item);
            if (index < 0)
                return false;
            RemoveAt(index);

            var prev = item.Previous;
            var next = item.Next;

            if (prev != null) prev.Next = next;
            if (next != null) next.Previous = prev;

            item.Next = null;
            item.Previous = null;

            return true;
        }

        public override void Sort()
        {
            base.Sort();
            Relink();
        }

        public void Relink()
        {
            for (int i = 0; i < Count; i++)
            {
                var item = this[i];
                item.Previous = i == 0 ? null : this[i - 1];
                item.Next = i == Count - 1 ? null : this[i + 1];
            }
        }
    }
}
