//============================================================================
//ZedGraph Class Library - A Flexible Charting Library for .Net
//Copyright (C) 2006 John Champion and Jerry Vos
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Timers;
using ZedGraph;

namespace ZedGraph.Demo
{
	/// <summary>
	/// Summary description for SimpleDemo.
	/// </summary>
	public class OHLCBarRealTimeDemo : DemoBase
	{

		public OHLCBarRealTimeDemo()
      : base("Demonstration of the OHLCBar Chart Type",
             "OHLCBar Real-Time Demo", DemoType.Bar)
    {
      m_Timer = new Timer { Interval = 500, Enabled = false };
      m_Data = new StockPointList();
      m_EMAData = new PointPairList();
      m_Rand = new Random();

      var now = fillSampleData();

      m_Pane = base.GraphPane;

      m_Pane.Title.Text = "OHLC Real-Time Bar Chart Demo";
      //m_Pane.XAxis.Title.Text = "Trading Date";
      //m_Pane.Y2Axis.Title.Text = "Share Price, $US";
      m_Pane.Title.IsVisible = false;
      m_Pane.Legend.IsVisible = false;
      m_Pane.Margin.Top = 5;
      m_Pane.Margin.Left = 0;
      m_Pane.Margin.Right = 50;
      m_Pane.Margin.Bottom = 0;
      m_Pane.Legend.Gap = 0;
      m_Pane.XAxis.Title.IsVisible = false;
      m_Pane.YAxis.Title.IsVisible = false;
      m_Pane.YAxis.MinSpace = 0;
      m_Pane.Y2Axis.Title.IsVisible = false;
      //m_Pane.Y2Axis.MinSpace = 50;
      //m_Pane.Y2Axis.AxisGap = 10;

      // Add OHLC bars
      OHLCBarItem myCurve = m_Pane.AddOHLCBar("trades", m_Data, Color.Black);
      myCurve.Bar.IsAutoSize = true;
      myCurve.Bar.Color = Color.DodgerBlue;

      // Cardinal spline smoothing function
      LineItem curve = m_Pane.AddCurve("EMA(0.9)", m_EMAData, Color.LightCoral, SymbolType.None);
      curve.Line.IsSmooth = true;
      curve.Line.SmoothTension = 0.5F;

      // Enable the Y2 axis display
      m_Pane.YAxis.IsVisible                   = false;
      m_Pane.Y2Axis.IsVisible                  = true;

      m_Pane.XAxis.MajorGrid.IsVisible         = true;
      m_Pane.XAxis.MajorGrid.DashOff           = 7;
      m_Pane.XAxis.MajorGrid.DashOn            = 1;
      m_Pane.Y2Axis.MajorGrid.IsVisible        = true;
      m_Pane.Y2Axis.MajorGrid.DashOff          = 7;
      m_Pane.Y2Axis.MajorGrid.DashOn           = 1;
      m_Pane.XAxis.MajorGrid.Color             = Color.SlateGray;
      m_Pane.Y2Axis.MajorGrid.Color            = Color.SlateGray;

      // Use DateAsOrdinal to skip weekend gaps
      m_Pane.XAxis.Type                        = AxisType.Date;
      m_Pane.XAxis.Scale.MajorUnit             = DateUnit.Minute;
      m_Pane.XAxis.Scale.MinorUnit             = DateUnit.Second;
      m_Pane.XAxis.Scale.Format                = "yyyy-MM-dd\nHH:mm:ss";
      m_Pane.XAxis.Scale.FontSpec.Size         = 9;
      m_Pane.XAxis.Scale.FontSpec.ScaleFactor  = 1.0f;
      m_Pane.XAxis.Scale.Max                   = new XDate(now);
      m_Pane.XAxis.Scale.Min                   = new XDate(now - TimeSpan.FromSeconds(60 * 15));
      m_Pane.XAxis.MajorTic.IsBetweenLabels    = true;
      m_Pane.XAxis.MinorTic.Size               = 0;
      m_Pane.XAxis.MinorTic.IsInside           = false;
      m_Pane.XAxis.MajorTic.IsInside           = false;
      m_Pane.XAxis.MinorTic.IsOutside          = true;

      m_Pane.Y2Axis.MinorTic.IsInside          = false;
      m_Pane.Y2Axis.MajorTic.IsInside          = false;
      m_Pane.Y2Axis.MinorTic.IsOutside         = true;
      m_Pane.Y2Axis.MajorTic.IsOutside         = true;
      m_Pane.Y2Axis.Scale.FontSpec.Size        = 11;
      m_Pane.Y2Axis.Scale.FontSpec.ScaleFactor = 1.0f;

      // pretty it up a little
      m_Pane.Chart.Fill = new Fill(Color.Black);
      m_Pane.Fill = new Fill(Color.SlateGray, Color.FromArgb(220, 220, 255), 45.0f);

      // Add a BoxObj to show a colored band behind the graph data
      m_Line = new LineObj(Color.Silver, 0, LastPoint.Y, m_Pane.Rect.Width, LastPoint.Y);
      m_Line.Location.CoordinateFrame = CoordType.AxisXYScale;
      // Align the left-top of the box to (0, 110)
      m_Line.Location.AlignH = AlignH.Left;
      //m_Line.Location.AlignV = AlignV.Top;
      // place the box behind the axis items, so the grid is drawn on top of it
      m_Line.ZOrder = ZOrder.A_InFront;
      m_Line.IsClippedToChartRect = true;
      m_Pane.GraphObjList.Add(m_Line);

      base.ZedGraphControl.AxisChange();

      m_Timer.Elapsed += (o, args) =>
      {
        calc(DateTime.Now, true);
        //base.ZedGraphControl.AxisChange();
        base.ZedGraphControl.Invalidate();
      };

      m_XHair = new LineObj(Color.SlateGray, 0, 0, m_Pane.Rect.Width, 0)
      {
        IsClippedToChartRect = true,
        ZOrder = ZOrder.A_InFront,
        IsVisible = false
      };
      m_YHair = new LineObj(Color.SlateGray, 0, 0, 0, m_Pane.Rect.Height)
      {
        IsClippedToChartRect = true,
        ZOrder = ZOrder.A_InFront,
        IsVisible = false
      };

      m_XHair.Line.Style = DashStyle.Dash;
      m_XHair.Location.CoordinateFrame = CoordType.AxisXYScale;
      m_XHair.Location.AlignH = AlignH.Left;
      m_XHair.Location.AlignV = AlignV.Top;
      m_YHair.Line.Style = DashStyle.Dash;
      m_YHair.Location.CoordinateFrame = CoordType.AxisXYScale;
      m_YHair.Location.AlignH = AlignH.Left;
      m_YHair.Location.AlignV = AlignV.Top;

      m_Pane.GraphObjList.Add(m_XHair);
      m_Pane.GraphObjList.Add(m_YHair);

      //ZedGraphControl.GraphPane.XAxis.Scale.Max = m_Pane.XAxis.Scale.Max;
      //ZedGraphControl.GraphPane.XAxis.Scale.Min = m_Pane.XAxis.Scale.Min;

      ZedGraphControl.MouseMove += ZedGraphControl_MouseMove;
      ZedGraphControl.MouseLeave += ZedGraphControl_MouseLeave;
      m_Timer.Enabled = true;
    }

	  private DateTime fillSampleData()
	  {
      // First day is jan 1st
      var now = DateTime.Now;
      m_Now = new XDate(now - TimeSpan.FromSeconds(60 * 15));
      m_Open = 50.0;

      for (var i = 0; i < 60 * 15; i += 5)
      {
        m_Now.AddSeconds(5);
        calc(m_Now, false);
      }

      return now;
    }

    public override void Activate()
	  {
      ZedGraphControl.CrossHair = false;
      //ZedGraphControl.IsShowHScrollBar = true;
      ZedGraphControl.IsEnableHZoom = false;
      ZedGraphControl.IsEnableVZoom = true;
      ZedGraphControl.IsAutoScrollRange = true;
    }

    public override void Deactivate()
	  {
      ZedGraphControl.CrossHair = true;
      //ZedGraphControl.IsShowHScrollBar = false;
      ZedGraphControl.IsEnableHZoom = true;
      ZedGraphControl.IsEnableVZoom = true;
      ZedGraphControl.IsAutoScrollRange = false;
    }

    private readonly Timer m_Timer;
    private readonly StockPointList m_Data;
    private readonly PointPairList  m_EMAData;

    private XDate  m_Now;
    private double m_Open;
    private readonly Random m_Rand;
    private readonly GraphPane m_Pane;
    private readonly LineObj m_Line;

    private readonly LineObj m_XHair;
    private readonly LineObj m_YHair;

    private const double EMA_ALPHA = 0.9;
    private double       m_EMA     = 0;

    private StockPt   LastPoint    => m_Data.Count    > 0 ? ((StockPt)m_Data[m_Data.Count - 1]) : new StockPt();
    private PointPair LastEMAPoint => m_EMAData.Count > 0 ? m_EMAData[m_EMAData.Count - 1] : new PointPair();

	  //private static readonly double s_Interval = ;

    private void calc(XDate now, bool timer)
	  {
      m_Timer.Enabled = false;
      var tm = new XDate(now);
      tm.AddSeconds(-5);
      var diff = (now - tm);
      var add = !timer || (m_Data.Count > 0 && ((now.XLDate - LastPoint.Date) > diff));
      StockPt pt = null;

      if (add)
      {
        double close = m_Open + m_Rand.NextDouble()*10.0 - 5.0;
        double hi    = Math.Max(m_Open, close) + m_Rand.NextDouble()*5.0;
        double low   = Math.Min(m_Open, close) - m_Rand.NextDouble()*5.0;

        var x  = now.XLDate;
        pt = new StockPt(x, hi, low, m_Open, close, 100000);

        m_Data.Add(pt);
        m_Open = close;

        m_EMA = EMA_ALPHA * close + (1.0 - EMA_ALPHA) * m_EMA;
        m_EMAData.Add(x, m_EMA);
      }
      else if (m_Data.Count > 0)
      {
        pt       = LastPoint;
        pt.Close = m_Open + m_Rand.NextDouble() * 10.0 - 5.0;
        pt.High  = Math.Max(pt.High, Math.Max(m_Open, pt.Close) + m_Rand.NextDouble() * 5.0);
        pt.Low   = Math.Min(pt.Low,  Math.Min(m_Open, pt.Close) - m_Rand.NextDouble() * 5.0);
      }

      if (m_Line != null)
      {
        m_Line.Location.Y = pt.Close;
        m_Line.Location.Width = m_Pane.Rect.Width;
      }

      m_Now = now;
      m_Timer.Enabled = true;
    }

    private void ZedGraphControl_MouseLeave(object sender, EventArgs e)
    {
      if (!m_XHair.IsVisible) return;

      m_XHair.IsVisible = false;
      m_YHair.IsVisible = false;
      ZedGraphControl.Refresh();
    }

    private void ZedGraphControl_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
    {
      double x, y;
      m_Pane.ReverseTransform(e.Location, out x, out y);

      #region crosshair

      //out of the bounds
      if (x < m_Pane.XAxis.Scale.Min ||
          x > m_Pane.XAxis.Scale.Max ||
          y < m_Pane.Y2Axis.Scale.Min ||
          y > m_Pane.Y2Axis.Scale.Max)
      {
        ZedGraphControl_MouseLeave(new object(), new EventArgs());
      }
      else//ok draw
      {
        m_XHair.Location.X = m_Pane.XAxis.Scale.Min;
        m_XHair.Location.Y = y;
        m_XHair.Location.Width = m_Pane.XAxis.Scale.Max - m_Pane.XAxis.Scale.Min;
        m_XHair.Location.Height = 0;

        m_YHair.Location.X = x;
        m_YHair.Location.Y = m_Pane.Y2Axis.Scale.Min;
        m_YHair.Location.Width = 0;
        m_YHair.Location.Height = m_Pane.Y2Axis.Scale.Max - m_Pane.Y2Axis.Scale.Min;

        m_XHair.IsVisible = true;
        m_YHair.IsVisible = true;

        ZedGraphControl.Refresh();
      }

      #endregion
    }
  }
}
