//============================================================================
//ZedGraph Class Library - A Flexible Charting Library for .Net
//Copyright (C) 2005 John Champion and Jerry Vos
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ZedGraph.Demo
{
  public class FilteredPointDemo : DemoBase
  {
    public FilteredPointDemo()
       : base("A demo demonstrating filtering of PointPairList.",
              "Filtered Point Demo", DemoType.Line)
    {
      ZedGraphControl.IsShowPointValues = true;
      var points = new PointPairList();
      for (double i = 0; i <= 5.0; i += 1)
        points.Add(i, i);

      var curve = GraphPane.AddCurve("A Label", points, Color.ForestGreen);

      ZedGraphControl.RestoreScale(ZedGraphControl.GraphPane);
      ZedGraphControl.ZoomEvent += (sender, oldstate, newstate) => LogVisibility(sender, curve, points);
      ZedGraphControl.AxisChange();
    }

    private static PointPairList VisiblePoints(GraphPane pane, LineItem lineItem, PointPairList points)
    {
      var pointPairList = new PointPairList();
      pointPairList.AddRange(points.Where(pp => IsVisible(pane, lineItem, pp)).ToList());
      return pointPairList;
    }

    private static bool IsVisible(GraphPane pane, LineItem lineItem, PointPair point)
    {
      var xScale = lineItem.GetXAxis(pane).Scale;
      var yScale = lineItem.GetYAxis(pane).Scale;
      var visible = point.X > xScale.Min && point.X < xScale.Max && point.Y > yScale.Min && point.Y < yScale.Max;
      return visible;
    }

    private static void LogVisibility(ZedGraphControl obj, LineItem lineItem, PointPairList points)
    {
      List<PointPair> visiblePoints = VisiblePoints(obj.GraphPane, lineItem, points);
      obj.GraphPane.XAxis.Title.Text = "Visible: " + string.Join(",", visiblePoints.Select(pp => string.Format("({0:N1},{1:N1})", pp.X, pp.Y)));
    }
  }
}