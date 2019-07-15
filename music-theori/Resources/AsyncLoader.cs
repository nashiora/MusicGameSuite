using System.Collections.Generic;
using System.Threading.Tasks;

namespace theori.Resources
{
    public class AsyncLoader
    {
        class LoadingTask
        {
            public readonly IAsyncLoadable Loadable;

            public Task<bool> AsyncLoadTask { get; private set; }

            public bool IsLoadCompleted => AsyncLoadTask.IsCompleted;
            public bool IsLoadSuccessful => AsyncLoadTask.Result;
            public bool IsFinalizeSuccessful { get; private set; } = false;

            public LoadingTask(IAsyncLoadable loadable)
            {
                Loadable = loadable;
            }

            public void DoLoad() => AsyncLoadTask = Task.Run(() => Loadable.AsyncLoad());
            public void DoFinalize()
            {
                if (!IsLoadCompleted || !IsLoadSuccessful || IsFinalizeSuccessful) return;
                IsFinalizeSuccessful = Loadable.AsyncFinalize();
            }
        }

        private readonly Dictionary<IAsyncLoadable, LoadingTask> m_loadables = new Dictionary<IAsyncLoadable, LoadingTask>();
        private bool m_started = false, m_completed = false;

        private Dictionary<IAsyncLoadable, LoadingTask>.ValueCollection.Enumerator? m_loadableEnumerator = null;

        public bool IsLoadCompleted
        {
            get
            {
                foreach (var task in m_loadables.Values)
                    if (!task.IsLoadCompleted) return false;
                return true;
            }
        }
        public bool IsLoadSuccessful
        {
            get
            {
                foreach (var task in m_loadables.Values)
                    if (!task.IsLoadSuccessful) return false;
                return true;
            }
        }

        public bool IsFinalizeSuccessful
        {
            get
            {
                foreach (var task in m_loadables.Values)
                    if (!task.IsFinalizeSuccessful) return false;
                return true;
            }
        }

        public bool IsCompleted => m_completed;

        public bool Failed
        {
            get
            {
                if (IsLoadCompleted && !IsLoadSuccessful)
                    return true;
                if (IsCompleted && !IsFinalizeSuccessful)
                    return true;
                return false;
            }
        }

        public AsyncLoader()
        {
        }

        public void Add(IAsyncLoadable loadable)
        {
            if (m_started || m_completed) return;

            if (m_loadables.ContainsKey(loadable)) return;
            m_loadables[loadable] = new LoadingTask(loadable);
        }

        public void LoadAll()
        {
            if (m_started || m_completed) return;
            m_started = true;

            foreach (var task in m_loadables.Values)
                task.DoLoad(); // creates a task
        }

        /// <summary>
        /// Called from the main thread when we want to check for the completetion of loadables.
        /// When a loadable is completed, this will finalize them; this blocks the main thread.
        /// </summary>
        public void Update()
        {
            if (m_completed) return;

            if (!m_started || !IsLoadCompleted) return;
            if (IsFinalizeSuccessful)
            {
                m_completed = true;
                return;
            }

            // finalize once per frame
            if (m_loadableEnumerator == null)
                m_loadableEnumerator = m_loadables.Values.GetEnumerator();

            var enumerator = m_loadableEnumerator.Value;
            if (enumerator.MoveNext())
            {
                LoadingTask current = enumerator.Current;
                current.DoFinalize();
            }
        }
    }
}
