using System.Collections.Generic;

namespace OpenRM
{
    public abstract class EditAction
    {
        internal bool m_applied = false;

        public Chart Chart { get; set; }

        public EditAction(Chart chart)
        {
            Chart = chart;
        }

        protected abstract void Apply_Impl();
        public void Apply()
        {
            if (m_applied) throw new System.InvalidOperationException("Action has already been applied.");
            Apply_Impl();
            m_applied = true;
        }

        protected abstract void Unapply_Impl();
        public void Unapply()
        {
            if (!m_applied) throw new System.InvalidOperationException("Action has not been applied.");
            Unapply_Impl();
            m_applied = false;
        }
    }

    public class EditStack
    {
        private readonly List<EditAction> m_actions = new List<EditAction>();
        private int m_nextIndex = 0;

        public void DoAction(EditAction action)
        {
            // kill undone actions
            while (m_nextIndex < m_actions.Count)
                m_actions.RemoveAt(m_nextIndex);

            action.Apply();

            m_actions.Add(action);
            m_nextIndex++;
        }

        public void UndoAction()
        {
            if (m_nextIndex > 0)
            {
                m_actions[m_nextIndex - 1].Unapply();
                m_nextIndex--;
            }
        }

        public void RedoAction()
        {
            if (m_nextIndex < m_actions.Count)
            {
                m_actions[m_nextIndex].Apply();
                m_nextIndex++;
            }
        }
    }
}
