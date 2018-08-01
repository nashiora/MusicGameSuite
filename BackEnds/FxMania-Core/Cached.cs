namespace FxMania
{
    public struct Cached<T>
    {
        public delegate T PropertyUpdater();

        private bool isValid;
        public bool IsValid => isValid;

        private PropertyUpdater updateDelegate;

        public static implicit operator T(Cached<T> value)
        {
            return value.Value;
        }

        /// <summary>
        /// Refresh this cached object with a custom delegate.
        /// </summary>
        /// <param name="providedDelegate"></param>
        public T Refresh(PropertyUpdater providedDelegate)
        {
            updateDelegate = updateDelegate ?? providedDelegate;
            return MakeValidOrDefault();
        }

        /// <summary>
        /// Refresh this property.
        /// </summary>
        public T MakeValidOrDefault()
        {
            if (IsValid) return value;

            if (!EnsureValid())
                return default(T);

            return value;
        }

        /// <summary>
        /// Refresh using a cached delegate.
        /// </summary>
        /// <returns>Whether refreshing was possible.</returns>
        public bool EnsureValid()
        {
            if (IsValid) return true;

            if (updateDelegate == null)
                return false;

            value = updateDelegate();
            isValid = true;

            return true;
        }

        /// <summary>
        /// Invalidate the cache of this object.
        /// </summary>
        /// <returns>True if we invalidated from a valid state.</returns>
        public bool Invalidate()
        {
            if (isValid)
            {
                isValid = false;
                return true;
            }

            return false;
        }

        private T value;

        public T Value
        {
            get
            {
                if (!isValid)
                    MakeValidOrDefault();

                return value;
            }

            set { throw new System.NotSupportedException("Can't manually update value!"); }
        }
    }
}
