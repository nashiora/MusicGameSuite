using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenRM;
using OpenRM.Voltex;

namespace theori.Game
{
    sealed class LaserRollSampler
    {
        sealed class Segment
        {
            public time_t StartTime, EndTime;
            public float StartValue, EndValue;

            public void Sample(time_t time) => MathL.Lerp(StartValue, EndValue, (float)((double)(time - StartTime) / (double)(EndTime - StartTime)));
        }

        private Chart m_chart;
        private List<Segment> m_segments = new List<Segment>();

        public LaserRollSampler(Chart chart)
        {
            m_chart = chart;

            Generate();
        }

        private void Generate()
        {
            AnalogObject left, right;
            LaserApplicationEvent appl;
            LaserParamsEvent pars;
            
            left = m_chart[(int)StreamIndex.VolL].FirstObject as AnalogObject;
            right = m_chart[(int)StreamIndex.VolR].FirstObject as AnalogObject;
            appl = m_chart[(int)StreamIndex.LaserApplicationKind].FirstObject as LaserApplicationEvent;
            pars = m_chart[(int)StreamIndex.LaserParams].FirstObject as LaserParamsEvent;

            while (left != null || right != null)
            {
                void HandleLoneSegment(ref AnalogObject lone, float dir)
                {
                    m_segments.Add(new Segment()
                    {
                        StartTime = lone.AbsolutePosition,
                        EndTime = lone.AbsoluteEndPosition,
                        StartValue = lone.InitialValue * dir,
                        EndValue = lone.FinalValue * dir,
                    });
                    lone = lone.Next as AnalogObject;
                }

                void HandleOverlap(ref AnalogObject pri, ref AnalogObject sec, float dir)
                {
                    m_segments.Add(new Segment()
                    {
                        StartTime = pri.AbsolutePosition,
                        EndTime = sec.AbsolutePosition,
                        StartValue = pri.InitialValue * dir,
                        EndValue = -dir * sec.InitialValue + MathL.Lerp(pri.InitialValue, pri.FinalValue, (float)((double)(sec.AbsolutePosition - pri.AbsolutePosition) / (double)pri.AbsoluteDuration)),
                    });
                    sec = sec.Next as AnalogObject;
                }
                
                if (left == null || right == null)
                {
                    if (left != null)
                        HandleLoneSegment(ref left, 1);
                    else HandleLoneSegment(ref right, -1);
                }
                else // both non-null
                {
                    if (left.Position >= right.EndPosition)
                        HandleLoneSegment(ref right, -1);
                    else if (right.Position >= left.EndPosition)
                        HandleLoneSegment(ref left, 1);
                    else
                    {
                        // overlap
                        if (left.EndPosition > right.EndPosition)
                            HandleOverlap(ref left, ref right, 1);
                        else HandleOverlap(ref right, ref left, -1);
                    }
                }
            }
        }

        public float Sample(time_t time)
        {
            return 0;
        }
    }
}
