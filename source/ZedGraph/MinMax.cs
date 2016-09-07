// Copyright (c) 2010 Serge Aleynikov
// LGPL license
// see https://github.com/saleyn/utxx/blob/master/include/utxx/detail/running_stat_impl.hpp

using System;
using System.Collections.Generic;

namespace ZedGraph
{
  /// <summary>
  /// Given a container that supports IList inteface, this class calculates
  /// a running min/max in a window with O(1) ammortized efficiency.
  /// </summary>
  public class MinMax<T> where T : IComparable
  {
    #region Constructors

    public MinMax(IList<T> list, int capacity = 256)
    {
      if (capacity == 0)
        throw new ArgumentException(nameof(capacity));
      if (list == null)
        throw new ArgumentNullException(nameof(list));

      m_List    = list;
      m_MinIdx  = 0;
      m_MaxIdx  = 0;
      m_MinFifo = new Deque<int>(capacity);
      m_MaxFifo = new Deque<int>(capacity);
      m_Mask    = capacity-1;
      m_End     = 0;
    }

    #endregion

    #region Fields

    private readonly IList<T>   m_List;
    private readonly Deque<int> m_MinFifo;
    private readonly Deque<int> m_MaxFifo;
    private int                 m_MinIdx;
    private int                 m_MaxIdx;
    private readonly int        m_Mask;
    private int                 m_End;

    #endregion

    #region Properties

    public T    Min   => m_List[m_MinIdx];
    public T    Max   => m_List[m_MaxIdx];
    public bool Empty => m_End == 0;

    #endregion

    #region Public Methods

    public void Clear()
    {
      m_MinFifo.Clear();
      m_MaxFifo.Clear();
      m_MinIdx = m_MaxIdx = 0;
      m_End = 0;
    }

    public void Add(T sample)
    {
      updateMinMax(sample);
      m_List.Add(sample);
      m_End++;
    }

    public void Update(int startIdx = 0)
    {
      Clear();
      if (m_List.Count == 0)
        return;

      if (startIdx >= m_List.Count)
        throw new ArgumentOutOfRangeException(nameof(startIdx));

      m_End = startIdx;

      while (m_End < m_List.Count)
        updateMinMax(m_List[++m_End]);
    }

    #endregion

    #region Private Methods

    private bool isNotInWindow(int idx)
    {
      var    diff = m_End > m_Mask ? m_End - m_Mask : 0;
      return idx  < diff;
    }

    private void updateMinMax(T sample)
    {
      if (Empty)
      {
        m_MinIdx = m_MaxIdx = m_End;
        return;
      }

      var prev = m_End - 1;

      if (sample.CompareTo(m_List[prev]) > 0)
      {
        //overshoot
        m_MinFifo.Add(prev);
        if (isNotInWindow(m_MinFifo[0]))
          m_MinFifo.RemoveAt(0);

        while (!m_MaxFifo.Empty)
        {
          var i = m_MaxFifo[m_MaxFifo.Count - 1];
          if (sample.CompareTo(m_List[i]) <= 0)
          {
            var front = m_MaxFifo[0];
            if (isNotInWindow(front))
            {
              if (m_MaxIdx == front)
                m_MaxIdx = m_End;
              m_MaxFifo.RemoveAt(0);
            }
            break;
          }
          m_MaxFifo.RemoveAt(m_MaxFifo.Count - 1);
        }
      }
      else
      {
        m_MaxFifo.Add(prev);
        if (isNotInWindow(m_MaxFifo[0]))
          m_MaxFifo.RemoveAt(0);
        while (!m_MinFifo.Empty)
        {
          var i = m_MinFifo[m_MinFifo.Count - 1];
          if (sample.CompareTo(m_List[i]) >= 0)
          {
            var front = m_MinFifo[0];
            if (isNotInWindow(front))
            {
              if (m_MinIdx == front)
                m_MinIdx = m_End;
              m_MinFifo.RemoveAt(0);
            }
            break;
          }
          m_MinFifo.RemoveAt(m_MinFifo.Count - 1);
        }
      }

      var  idx = m_MaxFifo.Empty ? m_MaxIdx : m_MaxFifo[0];
      m_MaxIdx = sample.CompareTo(m_List[idx]) > 0 ? m_End : idx;
      idx      = m_MinFifo.Empty ? m_MinIdx : m_MinFifo[0];
      m_MinIdx = sample.CompareTo(m_List[idx]) < 0 ? m_End : idx;
    }

    #endregion
  }
}