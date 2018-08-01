using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Collections
{
    public interface IOrderedList : ICollection, IEnumerable
    {
        bool AllowDuplicates { get; }

        //
        // Summary:
        //     Gets the element at the specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the element to get or set.
        //
        // Returns:
        //     The element at the specified index.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     index is not a valid index in the System.Collections.IList.
        //
        //   T:System.NotSupportedException:
        //     The property is set and the System.Collections.IList is read-only.
        object this[int index] { get; set; }

        //
        // Summary:
        //     Adds an item to the System.Collections.IList.
        //
        // Parameters:
        //   value:
        //     The object to add to the System.Collections.IList.
        //
        // Returns:
        //     The position into which the new element was inserted, or -1 to indicate that
        //     the item was not inserted into the collection.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The System.Collections.IList is read-only.-or- The System.Collections.IList has
        //     a fixed size.
        int Add(object value);
        //
        // Summary:
        //     Removes all items from the System.Collections.IList.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The System.Collections.IList is read-only.
        void Clear();
        //
        // Summary:
        //     Determines whether the System.Collections.IList contains a specific value.
        //
        // Parameters:
        //   value:
        //     The object to locate in the System.Collections.IList.
        //
        // Returns:
        //     true if the System.Object is found in the System.Collections.IList; otherwise,
        //     false.
        bool Contains(object value);
        //
        // Summary:
        //     Determines the index of a specific item in the System.Collections.IList.
        //
        // Parameters:
        //   value:
        //     The object to locate in the System.Collections.IList.
        //
        // Returns:
        //     The index of value if found in the list; otherwise, -1.
        int IndexOf(object value);
        //
        // Summary:
        //     Removes the first occurrence of a specific object from the System.Collections.IList.
        //
        // Parameters:
        //   value:
        //     The object to remove from the System.Collections.IList.
        //
        // Exceptions:
        //   T:System.NotSupportedException:
        //     The System.Collections.IList is read-only.-or- The System.Collections.IList has
        //     a fixed size.
        void Remove(object value);
        //
        // Summary:
        //     Removes the System.Collections.IList item at the specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the item to remove.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     index is not a valid index in the System.Collections.IList.
        //
        //   T:System.NotSupportedException:
        //     The System.Collections.IList is read-only.-or- The System.Collections.IList has
        //     a fixed size.
        void RemoveAt(int index);
    }
}
 
namespace System.Collections.Generic
{
    public interface IOrderedList<T> : ICollection<T>, IEnumerable<T>//, IReadOnlyCollection<T>
    {
        bool AllowDuplicates { get; }
        
        //
        // Summary:
        //     Gets the element at the specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the element to get or set.
        //
        // Returns:
        //     The element at the specified index.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     index is not a valid index in the System.Collections.Generic.IList`1.
        //
        //   T:System.NotSupportedException:
        //     The property is set and the System.Collections.Generic.IList`1 is read-only.
        T this[int index] { get; }

        //
        // Summary:
        //     Determines the index of a specific item in the System.Collections.Generic.IList`1.
        //
        // Parameters:
        //   item:
        //     The object to locate in the System.Collections.Generic.IList`1.
        //
        // Returns:
        //     The index of item if found in the list; otherwise, -1.
        int IndexOf(T item);
        //
        // Summary:
        //     Removes the System.Collections.Generic.IList`1 item at the specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the item to remove.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     index is not a valid index in the System.Collections.Generic.IList`1.
        //
        //   T:System.NotSupportedException:
        //     The System.Collections.Generic.IList`1 is read-only.
        void RemoveAt(int index);
    }

    // TODO(local): Might make sense to split the AllowDuplicates code into a StrictOrderedList<T> instead.

    public class OrderedList<T> : IOrderedList<T>, IOrderedList//, IReadOnlyList<T>
    {
        private static readonly string SET_INDEX_MESSAGE = "OrderedList<T> does not support directly setting based on index. Use { nameof(OrderedList<T>) }.{ nameof(Add) }";
        private static readonly string DUPLICATE_MESSAGE = "This OrderedList<T> doesn't support duplicate entries.";

        private readonly List<T> data;
        private readonly IComparer<T> comparer;

        #region Properties

        private bool allowDups;

        bool IOrderedList.AllowDuplicates { get { return AllowDuplicates; } }
        public virtual bool AllowDuplicates { get { return allowDups; } set { allowDups = value; } }

        public int Capacity
        {
            get { return data.Capacity; }
            set { data.Capacity = value; }
        }

        public int Count { get { return data.Count; } }

        object IOrderedList.this[int index]
        {
            get { return data[index]; }
            set { throw new NotImplementedException(SET_INDEX_MESSAGE); }
        }

        public T this[int index]
        {
            get { return data[index]; }
            set { throw new NotImplementedException(SET_INDEX_MESSAGE); }
        }

        bool ICollection.IsSynchronized { get { return false; } }
        object ICollection.SyncRoot { get { return data; } }
        bool ICollection<T>.IsReadOnly { get { return false; } }

        #endregion

        #region Constructors

        protected OrderedList(List<T> data, IComparer<T> comparer, bool allowDups)
        {
            this.data = data;
            this.comparer = comparer;
            AllowDuplicates = allowDups;
        }

        public OrderedList(bool allowDups = true)
            : this(new List<T>(), Comparer<T>.Default, allowDups)
        {
        }

        public OrderedList(IComparer<T> comparer, bool allowDups = true)
            : this(new List<T>(), comparer, allowDups)
        {
        }

        public OrderedList(int capacity, IComparer<T> comparer, bool allowDups = true)
            : this(new List<T>(capacity), comparer, allowDups)
        {
        }

        public OrderedList(int capacity, bool allowDups = true)
            : this(new List<T>(capacity), Comparer<T>.Default, allowDups)
        {
        }

        public OrderedList(IEnumerable<T> collection, bool allowDups = true)
            : this(collection, Comparer<T>.Default, allowDups)
        {
        }

        public OrderedList(IEnumerable<T> collection, IComparer<T> comparer, bool allowDups = true)
            : this(new List<T>(collection), comparer, allowDups)
        {
            data.Sort(comparer);
        }

        #endregion

        #region Methods

        void ICollection<T>.Add(T item) { Add(item); }
        int IOrderedList.Add(object item) { return Add((T)item); }

        public virtual int Add(T item)
        {
            int index = BinarySearch(item);
            if (index < 0)
                index = ~index;
            else // we found the item
            {
                if (!AllowDuplicates)
                    throw new InvalidOperationException(string.Format("{0}: {1} -> {2}", DUPLICATE_MESSAGE, data[index], item));
            }
            data.Insert(index, item);
            return index;
        }

        public virtual void AddRange(IEnumerable<T> collection)
        {
            var list = new List<T>(collection);
            if (list.Count == 0)
                return;

            if (data.Count == 0)
            {
                data.AddRange(collection);
                data.Sort(comparer);
                return;
            }

            list.Sort(comparer);
            int searchLength = data.Count;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                T item = list[i];

                int insertIndex = BinarySearch(0, searchLength, item);
                if (insertIndex < 0)
                    insertIndex = ~insertIndex;
                else
                {
                    if (AllowDuplicates)
                    {
                        while (--insertIndex >= 0 && data[insertIndex].Equals(item))
                            ; // do nothing
                        insertIndex++;
                    }
                    else throw new InvalidOperationException(DUPLICATE_MESSAGE);
                }

                if (insertIndex <= 0)
                {
                    data.InsertRange(0, list.GetRange(0, i + 1));
                    break;
                }

                searchLength = insertIndex - 1;
                item = data[searchLength];

                int endInsert = i;
                while (--i >= 0 && comparer.Compare(list[i], item) > 0)
                    ; // do nothing

                i++;
                data.InsertRange(insertIndex, list.GetRange(i, endInsert - i + 1));
            }
        }

        public ReadOnlyCollection<T> AsReadOnly() { return data.AsReadOnly(); }

        public virtual void Clear() { data.Clear(); }

        bool IOrderedList.Contains(object item) { return Contains((T)item); }
        public bool Contains(T item) { return BinarySearch(item) >= 0; }

        void ICollection.CopyTo(Array array, int arrayIndex) { data.CopyTo((T[])array, arrayIndex); }
        public void CopyTo(T[] array) { data.CopyTo(array); }
        public void CopyTo(T[] array, int arrayIndex) { data.CopyTo(array, arrayIndex); }
        public void CopyTo(int index, T[] array, int arrayIndex, int count) { data.CopyTo(index, array, arrayIndex, count); }

        public void ForEach(Action<T> action)
        {
            if (action == null)
                return;
            foreach (T item in data)
                action(item);
        }

        IEnumerator IEnumerable.GetEnumerator() { return data.GetEnumerator(); }
        public IEnumerator<T> GetEnumerator() { return data.GetEnumerator(); }

        public List<T> GetRange(int index, int count) { return data.GetRange(index, count); }

        void IOrderedList.Remove(object item) { Remove((T)item); }
        public virtual bool Remove(T item)
        {
            int index = BinarySearch(item);
            if (index < 0)
                return false;
            data.RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index) { data.RemoveAt(index); }
        public void RemoveRange(int index, int count) { data.RemoveRange(index, count); }

        public T[] ToArray() { return data.ToArray(); }

        public void TrimExcess() { data.TrimExcess(); }

        int IOrderedList.IndexOf(object item) { return IndexOf((T)item); }
        public int IndexOf(T item)
        {
            int index = BinarySearch(item);
            if (index < 0)
                return -1;

            if (AllowDuplicates)
            {
                while(--index >= 0 && data[index].Equals(item))
                    ; // do nothing
                return index + 1;
            }
            else return -1;
        }

        public int LastIndexOf(T item)
        {
            if (!AllowDuplicates)
                return IndexOf(item);

            int index = BinarySearch(item);
            if (index < 0)
                return -1;

            while (++index < data.Count && data[index].Equals(item))
                ; // do nothing
            return index - 1;
        }

        public T[] WithinRange(T min, T max)
        {
            if (comparer.Compare(min,max) > 0)
                throw new ArgumentException("min must be <= max");

            int minSearchLength;
            int maxIndex = BinarySearch(max);
            if (maxIndex >= 0)
            {
                minSearchLength = maxIndex + 1;
                if (AllowDuplicates)
                {
                    while (++maxIndex < data.Count && comparer.Compare(max, data[maxIndex]) == 0)
                        ; // do nothing;
                    --maxIndex;
                }
            }
            else
            {
                minSearchLength = ~maxIndex;
                if (minSearchLength <= 0)
                    return new T[0];
                maxIndex = minSearchLength - 1;
            }

            int minIndex = BinarySearch(0, minSearchLength, min);
            if (minIndex >= 0)
            {
                if (AllowDuplicates)
                {
                    while (--minIndex >= 0 && comparer.Compare(max, data[minIndex]) == 0)
                        ; // do nothing
                    ++minIndex;
                }
            }
            else
            {
                minIndex = ~minIndex;
                if (minIndex > maxIndex)
                    return new T[0];
            }

            int count = maxIndex - minIndex + 1;
            var result = new T[count];

            CopyTo(minIndex, result, 0, count);
            return result;
        }

        public int BinarySearch(T item) { return data.BinarySearch(item, comparer); }
        public int BinarySearch(int index, int count, T item) { return data.BinarySearch(index, count, item, comparer); }

        #endregion

        public virtual void Sort()
        {
            data.Sort();
        }
    }
}
