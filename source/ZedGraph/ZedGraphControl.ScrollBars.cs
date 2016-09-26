//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2007  John Champion
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

using System;
using System.Linq;
using System.Windows.Forms;

namespace ZedGraph
{
  partial class ZedGraphControl
  {
    #region ScrollBars

    private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
    {
      if (GraphPane == null) return;

      if (((e.Type != ScrollEventType.ThumbPosition) &&
           (e.Type != ScrollEventType.ThumbTrack)) ||
          ((e.Type == ScrollEventType.ThumbTrack) &&
           (_zoomState == null)))
        ZoomStateSave(GraphPane, ZoomState.StateType.Scroll);

      for (var i = 0; i < GraphPane.YAxisList.Count; i++)
      {
        var scroll = YScrollRangeList[i];
        if (!scroll.IsScrollable) continue;

        Axis axis = GraphPane.YAxisList[i];
        HandleScroll(axis, e.NewValue, scroll.Min, scroll.Max, vScrollBar1.LargeChange,
                     !axis.Scale.IsReverse);
      }

      for (var i = 0; i < GraphPane.Y2AxisList.Count; i++)
      {
        var scroll = Y2ScrollRangeList[i];
        if (!scroll.IsScrollable) continue;

        Axis axis = GraphPane.Y2AxisList[i];
        HandleScroll(axis, e.NewValue, scroll.Min, scroll.Max, vScrollBar1.LargeChange,
                     !axis.Scale.IsReverse);
      }

      ApplyToAllPanes(GraphPane);

      ProcessEventStuff(vScrollBar1, e);
    }

    private void ApplyToAllPanes(GraphPane primaryPane)
    {
      if (!_isSynchronizeXAxes && !_isSynchronizeYAxes) return;

      foreach (var pane in _masterPane.PaneList
                                      .Where(
                                             pane =>
                                                 (pane != primaryPane) && pane is GraphPane)
                                      .Cast<GraphPane>())
      {
        if (_isSynchronizeXAxes)
        {
          Synchronize(primaryPane.XAxis, pane.XAxis);
          Synchronize(primaryPane.X2Axis, pane.X2Axis);
        }
        if (_isSynchronizeYAxes)
        {
          Synchronize(primaryPane.YAxis, pane.YAxis);
          Synchronize(primaryPane.Y2Axis, pane.Y2Axis);
        }
      }
    }

    private static void Synchronize(Axis source, Axis dest)
    {
      dest.Scale._min = source.Scale._min;
      dest.Scale._max = source.Scale._max;
      dest.Scale._majorStep = source.Scale._majorStep;
      dest.Scale._minorStep = source.Scale._minorStep;
      dest.Scale.MinAuto = source.Scale.MinAuto;
      dest.Scale.MaxAuto = source.Scale.MaxAuto;
      dest.Scale.MajorStepAuto = source.Scale.MajorStepAuto;
      dest.Scale.MinorStepAuto = source.Scale.MinorStepAuto;
    }

    private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
    {
      if (GraphPane == null) return;

      if (((e.Type != ScrollEventType.ThumbPosition) &&
           (e.Type != ScrollEventType.ThumbTrack)) ||
          ((e.Type == ScrollEventType.ThumbTrack) &&
           (_zoomState == null)))
        ZoomStateSave(GraphPane, ZoomState.StateType.Scroll);

      HandleScroll(GraphPane.XAxis, e.NewValue, _xScrollRange.Min, _xScrollRange.Max,
                   hScrollBar1.LargeChange, GraphPane.XAxis.Scale.IsReverse);

      ApplyToAllPanes(GraphPane);

      ProcessEventStuff(hScrollBar1, e);
    }

    private void ProcessEventStuff(ScrollBar scrollBar, ScrollEventArgs e)
    {
      if (e.Type == ScrollEventType.ThumbTrack)
      {
        ScrollProgressEvent?.Invoke(this, hScrollBar1, _zoomState,
                                    new ZoomState(GraphPane, ZoomState.StateType.Scroll));
      }
      else // if ( e.Type == ScrollEventType.ThumbPosition )
      {
        if ((_zoomState != null) && _zoomState.IsChanged(GraphPane))
        {
          //this.GraphPane.ZoomStack.Push( _zoomState );
          ZoomStatePush(GraphPane);

          // Provide Callback to notify the user of pan events
          ScrollDoneEvent?.Invoke(this, hScrollBar1, _zoomState,
                                  new ZoomState(GraphPane, ZoomState.StateType.Scroll));

          _zoomState = null;
        }
      }

      ScrollEvent?.Invoke(scrollBar, e);
    }

    /*
        /// <summary>
        /// Use the MouseCaptureChanged as an indicator for the start and end of a scrolling operation
        /// </summary>
        private void ScrollBarMouseCaptureChanged( object sender, EventArgs e )
        {
          return;
    
          ScrollBar scrollBar = sender as ScrollBar;
          if ( scrollBar != null )
          {
            // If this is the start of a new scroll, then Capture will be true
            if ( scrollBar.Capture )
            {
              // save the original zoomstate
              //_zoomState = new ZoomState( this.GraphPane, ZoomState.StateType.Scroll );
              ZoomStateSave( this.GraphPane, ZoomState.StateType.Scroll );
            }
            else
            {
              // push the prior saved zoomstate, since the scale ranges have already been changed on
              // the fly during the scrolling operation
              if ( _zoomState != null && _zoomState.IsChanged( this.GraphPane ) )
              {
                //this.GraphPane.ZoomStack.Push( _zoomState );
                ZoomStatePush( this.GraphPane );
    
                // Provide Callback to notify the user of pan events
                if ( this.ScrollDoneEvent != null )
                  this.ScrollDoneEvent( this, scrollBar, _zoomState,
                        new ZoomState( this.GraphPane, ZoomState.StateType.Scroll ) );
    
                _zoomState = null;
              }
            }
          }
        }
    */

    private void HandleScroll(Axis axis, int newValue, double scrollMin, double scrollMax,
                              int largeChange, bool reverse)
    {
      if (axis == null) return;
      if (scrollMin > axis.Scale._min)
        scrollMin = axis.Scale._min;
      if (scrollMax < axis.Scale._max)
        scrollMax = axis.Scale._max;

      var span = _ScrollControlSpan - largeChange;
      if (span <= 0)
        return;

      if (reverse)
        newValue = span - newValue;

      var scale = axis.Scale;

      var delta = scale.MaxLinearized - scale.MinLinearized;
      var scrollMin2 = scale.Linearize(scrollMax) - delta;
      scrollMin = scale.Linearize(scrollMin);
      //scrollMax = scale.Linearize( scrollMax );
      var val = scrollMin + newValue/(double)span*
                (scrollMin2 - scrollMin);
      scale.MinLinearized = val;
      scale.MaxLinearized = val + delta;
      /*
                if ( axis.Scale.IsLog )
                {
                  double ratio = axis._scale._max / axis._scale._min;
                  double scrollMin2 = scrollMax / ratio;

                  double val = scrollMin * Math.Exp( (double)newValue / (double)span *
                        ( Math.Log( scrollMin2 ) - Math.Log( scrollMin ) ) );
                  axis._scale._min = val;
                  axis._scale._max = val * ratio;
                }
                else
                {
                  double delta = axis._scale._max - axis._scale._min;
                  double scrollMin2 = scrollMax - delta;

                  double val = scrollMin + (double)newValue / (double)span *
                        ( scrollMin2 - scrollMin );
                  axis._scale._min = val;
                  axis._scale._max = val + delta;
                }
        */
      Invalidate();
    }

    /// <summary>
    ///   Sets the value of the scroll range properties (see <see cref="ScrollMinX" />,
    ///   <see cref="ScrollMaxX" />, <see cref="YScrollRangeList" />, and
    ///   <see cref="Y2ScrollRangeList" /> based on the actual range of the data for
    ///   each corresponding <see cref="Axis" />.
    /// </summary>
    /// <remarks>
    ///   This method is called automatically by <see cref="AxisChange" /> if
    ///   <see cref="IsAutoScrollRange" />
    ///   is true.  Note that this will not be called if you call AxisChange directly from the
    ///   <see cref="GraphPane" />.  For example, zedGraphControl1.AxisChange() works properly, but
    ///   zedGraphControl1.GraphPane.AxisChange() does not.
    /// </remarks>
    public void SetScrollRangeFromData()
    {
      if (GraphPane == null) return;

      var grace = CalcScrollGrace(GraphPane.XAxis.Scale._rangeMin,
                                  GraphPane.XAxis.Scale._rangeMax);

      _xScrollRange.Min = GraphPane.XAxis.Scale._rangeMin - grace;
      _xScrollRange.Max = GraphPane.XAxis.Scale._rangeMax + grace;
      _xScrollRange.IsScrollable = true;

      for (var i = 0; i < GraphPane.YAxisList.Count; i++)
      {
        Axis axis = GraphPane.YAxisList[i];
        grace = CalcScrollGrace(axis.Scale._rangeMin, axis.Scale._rangeMax);
        var range = new ScrollRange(axis.Scale._rangeMin - grace,
                                    axis.Scale._rangeMax + grace,
                                    YScrollRangeList[i].IsScrollable);

        if (i >= YScrollRangeList.Count)
          YScrollRangeList.Add(range);
        else
          YScrollRangeList[i] = range;
      }

      for (var i = 0; i < GraphPane.Y2AxisList.Count; i++)
      {
        Axis axis = GraphPane.Y2AxisList[i];
        grace = CalcScrollGrace(axis.Scale._rangeMin, axis.Scale._rangeMax);
        var range = new ScrollRange(axis.Scale._rangeMin - grace,
                                    axis.Scale._rangeMax + grace,
                                    Y2ScrollRangeList[i].IsScrollable);

        if (i >= Y2ScrollRangeList.Count)
          Y2ScrollRangeList.Add(range);
        else
          Y2ScrollRangeList[i] = range;
      }

      //this.GraphPane.CurveList.GetRange( out scrollMinX, out scrollMaxX,
      //    out scrollMinY, out scrollMaxY, out scrollMinY2, out scrollMaxY2, false, false,
      //    this.GraphPane );
    }

    private double CalcScrollGrace(double min, double max)
    {
      if (Math.Abs(max - min) < 1e-30)
        if (Math.Abs(max) < 1e-30)
          return ScrollGrace;
        else
          return max*ScrollGrace;
      return (max - min)*ScrollGrace;
    }

    private void SetScroll(ScrollBar scrollBar, Axis axis, double scrollMin,
                           double scrollMax)
    {
      if ((scrollBar == null) || (axis == null)) return;

      scrollBar.Minimum = 0;
      scrollBar.Maximum = _ScrollControlSpan - 1;

      if (scrollMin < axis.Scale._min)
        scrollMin = axis.Scale._min;
      if (scrollMax > axis.Scale._max)
        scrollMax = axis.Scale._max;

      var scale = axis.Scale;
      var minLinearized = scale.MinLinearized;
      var maxLinearized = scale.MaxLinearized;
      scrollMin = scale.Linearize(scrollMin);
      scrollMax = scale.Linearize(scrollMax);

      var scrollMin2 = scrollMax - (maxLinearized - minLinearized);
      /*
        if ( axis.Scale.IsLog )
          scrollMin2 = scrollMax / ( axis._scale._max / axis._scale._min );
        else
          scrollMin2 = scrollMax - ( axis._scale._max - axis._scale._min );
        */
      if (scrollMin >= scrollMin2)
      {
        //scrollBar.Visible = false;
        scrollBar.Enabled = false;
        scrollBar.Value = 0;
      }
      else
      {
        var ratio = (maxLinearized - minLinearized)/(scrollMax - scrollMin);

        /*
          if ( axis.Scale.IsLog )
            ratio = ( Math.Log( axis._scale._max ) - Math.Log( axis._scale._min ) ) /
                  ( Math.Log( scrollMax ) - Math.Log( scrollMin ) );
          else
            ratio = ( axis._scale._max - axis._scale._min ) / ( scrollMax - scrollMin );
          */

        var largeChange = (int)(ratio*_ScrollControlSpan + 0.5);
        if (largeChange < 1)
          largeChange = 1;
        scrollBar.LargeChange = largeChange;

        var smallChange = largeChange/_ScrollSmallRatio;
        if (smallChange < 1)
          smallChange = 1;
        scrollBar.SmallChange = smallChange;

        var span = _ScrollControlSpan - largeChange;

        var val = (int)((minLinearized - scrollMin)/(scrollMin2 - scrollMin)*
                        span + 0.5);
        /*
          if ( axis.Scale.IsLog )
            val = (int)( ( Math.Log( axis._scale._min ) - Math.Log( scrollMin ) ) /
                ( Math.Log( scrollMin2 ) - Math.Log( scrollMin ) ) * span + 0.5 );
          else
            val = (int)( ( axis._scale._min - scrollMin ) / ( scrollMin2 - scrollMin ) *
                span + 0.5 );
          */
        if (val < 0)
          val = 0;
        else if (val > span)
          val = span;

        //if ( ( axis is XAxis && axis.IsReverse ) || ( ( ! axis is XAxis ) && ! axis.IsReverse ) )
        if (axis is XAxis == axis.Scale.IsReverse)
          val = span - val;

        if (val < scrollBar.Minimum)
          val = scrollBar.Minimum;
        if (val > scrollBar.Maximum)
          val = scrollBar.Maximum;

        scrollBar.Value = val;
        scrollBar.Enabled = true;
        //scrollBar.Visible = true;
      }
    }

    #endregion
  }
}
