using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenRM
{
    public delegate void TimeEventHandler(time_t time);
    public delegate void ObjectEventHandler(Object obj);
    public delegate void EventEventHandler(Event evt);

    public class ChartPlayback
    {
        public event ObjectEventHandler ObjectAppear;
        public event ObjectEventHandler ObjectDisappear;

        public event EventEventHandler EventTrigger;

        public event ObjectEventHandler ObjectBegin;
        public event ObjectEventHandler ObjectEnd;

        public event TimeEventHandler PositionChange;
        public event TimeEventHandler ViewDurationChange;
        public event TimeEventHandler LookBackDurationChange;

        public readonly Chart Chart;

        public ControlPoint CurrentTimingSection { get; protected set; }

        private List<Object>[] m_objectsInView;
        private List<Object>[] m_objectsProcessed;

        private readonly List<Object>[] m_currentObjects;
        
        private time_t m_lastObjectPosition;

        private time_t m_position = 0;
        public time_t Position
        {
            get => m_position;
            set
            {
                m_position = value;
                OnPositionChanged(value);
                UpdateView();
            }
        }

        public time_t EndPosition => Position + ViewDuration;

        private time_t m_viewDuration = 1;
        public time_t ViewDuration
        {
            get => m_viewDuration;
            set
            {
                m_viewDuration = value;
                UpdateView();
                OnViewDurationChanged(value);
            }
        }

        private time_t m_lookBackDuration = 0.25;
        public time_t LookBackDuration
        {
            get => m_lookBackDuration;
            set
            {
                m_lookBackDuration = value;
                UpdateView();
                OnLookBackDurationChanged(value);
            }
        }

        public bool HasEnded { get; private set; }

        public ChartPlayback(Chart chart)
        {
            Chart = chart;

            m_objectsInView = new List<Object>[chart.StreamCount];
            m_objectsInView.Fill(() => new List<Object>());

            m_objectsProcessed = new List<Object>[chart.StreamCount];
            m_objectsProcessed.Fill(() => new List<Object>());

            m_currentObjects = new List<Object>[chart.StreamCount];
            m_currentObjects.Fill(() => new List<Object>());

            m_lastObjectPosition = chart.TimeEnd;

            UpdateView();
        }

        public void ForEachObjectInView(int stream, Action<Object> a)
        {
            if (stream < 0 || stream > Chart.StreamCount) throw new ArgumentOutOfRangeException(nameof(stream), $"stream must be in range [0, { Chart.StreamCount }), got { stream }.");
            m_objectsInView[stream].ForEach(obj => a(obj));
        }

        public void ForEachObjectInViewAll(Action<Object> a)
        {
            for (int i = 0, len = Chart.StreamCount; i < len; i++)
                ForEachObjectInView(i, a);
        }

        protected void UpdateView()
        {
            CurrentTimingSection = Chart.ControlPoints.MostRecent(Position);

            Chart.ForEachObjectInRange(Position, EndPosition, true, obj =>
            {
                if (!m_objectsInView[obj.Stream].Contains(obj))
                {
                    m_objectsInView[obj.Stream].Add(obj);   
                    OnObjectAppeared(obj);
                }
            });

            MarkPassedObjects();

            if (m_objectsInView.Aggregate(0, (a, objs) => a + objs.Count) == 0)
                HasEnded = m_lastObjectPosition < 0 || m_position > m_lastObjectPosition;
            else HasEnded = false;
        }

        private void MarkPassedObjects()
        {
            m_objectsInView.ForEach((s, v) =>
            {
                for (int i = 0; i < v.Count; )
                {
                    var obj = v[i];
                
                    time_t start = obj.AbsolutePosition;
                    time_t end = obj.AbsoluteEndPosition;

                    if (start < m_position)
                    {
                        if (!m_currentObjects[s].Contains(obj) && !m_objectsProcessed[s].Contains(obj))
                        {
                            m_currentObjects[s].Add(obj);
                            if (obj is Event evt)
                                OnEventTrigger(evt);
                            else OnObjectBegan(obj);
                        }
                    }

                    if (end < m_position)
                    {
                        if (m_currentObjects[s].Contains(obj))
                        {
                            m_currentObjects[s].Remove(obj);
                            m_objectsProcessed[s].Add(obj);
                            if (obj is Event evt)
                            {
                            }
                            else OnObjectEnded(obj);
                        }
                    }

                    if (end < m_position - m_lookBackDuration)
                    {
                        OnObjectDisappeared(obj);
                        m_objectsInView[s].RemoveAt(i);
                        m_objectsProcessed[s].Remove(obj);
                    }
                    else
                    {
                        start = obj.AbsolutePosition;
                        i++;
                    }
                }
            });
        }

        protected void OnObjectAppeared(Object obj) => ObjectAppear?.Invoke(obj);
        protected void OnObjectDisappeared(Object obj) => ObjectDisappear?.Invoke(obj);
        
        protected void OnEventTrigger(Event evt) => EventTrigger?.Invoke(evt);

        protected void OnObjectBegan(Object obj) => ObjectBegin?.Invoke(obj);
        protected void OnObjectEnded(Object obj) => ObjectEnd?.Invoke(obj);

        protected void OnPositionChanged(time_t value) => PositionChange?.Invoke(value);
        protected void OnViewDurationChanged(time_t value) => ViewDurationChange?.Invoke(value);
        protected void OnLookBackDurationChanged(time_t value) => LookBackDurationChange?.Invoke(value);
    }
}
