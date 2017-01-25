// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OHLCBarItemTests.cs" company="ZedGraph Project">
//   This library is free software; you can redistribute it and/or
//   modify it under the terms of the GNU Lesser General Public
//   License as published by the Free Software Foundation; either
//   version 2.1 of the License, or (at your option) any later version.
//   
//   This library is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//   Lesser General Public License for more details.
//   
//   You should have received a copy of the GNU Lesser General Public
//   License along with this library; if not, write to the Free Software
//   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright>
// <summary>
//   Defines the OHLCBarItemTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ZedGraph
{
  using System;
  using System.Drawing;

  using NUnit.Framework;

  /// <summary>
  /// The ohlc bar item tests.
  /// </summary>
  [TestFixture]
  public class OHLCBarItemTests
  {
    /// <summary>
    /// Tests that the <see cref="CurveItem.GetBarWidth"/> of the <see cref="OHLCBarItem"/>
    /// with an ordinal axis type and intraday values in the curve returns a width of 3 pixels.
    /// </summary>
    [Test]
    public void GetBarWidth_OrdinalAxisIntradayValues_3Pixels()
    {
      GraphPane myPane = new GraphPane();
      myPane.Rect = new RectangleF(0, 0, 640f, 480f);
      myPane.XAxis.Type = AxisType.DateAsOrdinal;

      var spl = CreateStockPointList(5);
      OHLCBarItem myCurve = myPane.AddOHLCBar("trades", spl, Color.Black);

      AxisChangeAndDraw(myPane);

      Assert.That(myCurve.Bar.GetBarWidth(myPane, myPane.XAxis, 1.0f), Is.EqualTo(3f));
    }

    /// <summary>
    /// Tests that the <see cref="CurveItem.GetBarWidth"/> of the <see cref="OHLCBarItem"/>
    /// with an ordinal axis type and daily values in the curve returns a width of 3 pixels.
    /// </summary>
    [Test]
    public void GetBarWidth_OrdinalAxisDailyValues_3Pixels()
    {
      GraphPane myPane = new GraphPane();
      myPane.Rect = new RectangleF(0, 0, 640f, 480f);

      var spl = CreateStockPointList(60 * 24);
      OHLCBarItem myCurve = myPane.AddOHLCBar("trades", spl, Color.Black);

      myPane.XAxis.Type = AxisType.DateAsOrdinal;

      AxisChangeAndDraw(myPane);

      Assert.That(myCurve.Bar.GetBarWidth(myPane, myPane.XAxis, 1.0f), Is.EqualTo(3f));
    }

    /// <summary>
    /// Tests that the <see cref="CurveItem.GetBarWidth"/> of the <see cref="OHLCBarItem"/>
    /// with an ordinal axis type and weekly values in the curve returns a width of 3 pixels.
    /// </summary>
    [Test]
    public void GetBarWidth_OrdinalAxisWeeklyValues_3Pixels()
    {
      GraphPane myPane = new GraphPane();
      myPane.Rect = new RectangleF(0, 0, 640f, 480f);
      myPane.XAxis.Type = AxisType.DateAsOrdinal;

      var spl = CreateStockPointList(60 * 24 * 7);
      OHLCBarItem myCurve = myPane.AddOHLCBar("trades", spl, Color.Black);

      AxisChangeAndDraw(myPane);

      Assert.That(myCurve.Bar.GetBarWidth(myPane, myPane.XAxis, 1.0f), Is.EqualTo(3f));
    }

    /// <summary>
    /// Tests that the <see cref="CurveItem.GetBarWidth"/> of the <see cref="OHLCBarItem"/>
    /// with a non-ordinal axis type and intraday values in the curve returns a width of 2 pixels.
    /// </summary>
    [Test]
    public void GetBarWidth_NonOrdinalAxisIntradayValues_2Pixels()
    {
      GraphPane myPane = new GraphPane();
      myPane.Rect = new RectangleF(0, 0, 640f, 480f);
      myPane.XAxis.Type = AxisType.Date;

      var spl = CreateStockPointList(5);

      OHLCBarItem myCurve = myPane.AddOHLCBar("trades", spl, Color.Black);

      AxisChangeAndDraw(myPane);

      Assert.That(myCurve.Bar.GetBarWidth(myPane, myPane.XAxis, 1.0f), Is.EqualTo(2f));
    }

    /// <summary>
    /// Tests that the <see cref="CurveItem.GetBarWidth"/> of the <see cref="OHLCBarItem"/>
    /// with a non-ordinal axis type and daily values in the curve returns a width of 2 pixels.
    /// </summary>
    [Test]
    public void GetBarWidth_NonOrdinalAxisDailyValues_2Pixels()
    {
      GraphPane myPane = new GraphPane();
      myPane.Rect = new RectangleF(0, 0, 640f, 480f);
      myPane.XAxis.Type = AxisType.Date;

      var spl = CreateStockPointList(60 * 24);

      OHLCBarItem myCurve = myPane.AddOHLCBar("trades", spl, Color.Black);

      AxisChangeAndDraw(myPane);

      Assert.That(myCurve.Bar.GetBarWidth(myPane, myPane.XAxis, 1.0f), Is.EqualTo(2f));
    }

    /// <summary>
    /// Tests that the <see cref="CurveItem.GetBarWidth"/> of the <see cref="OHLCBarItem"/>
    /// with a non-ordinal axis type and weekly values in the curve returns a width of 2 pixels.
    /// </summary>
    [Test]
    public void GetBarWidth_NonOrdinalAxisWeeklyValues_2Pixels()
    {
      GraphPane myPane = new GraphPane();
      myPane.Rect = new RectangleF(0, 0, 640f, 480f);
      myPane.XAxis.Type = AxisType.Date;

      var spl = CreateStockPointList(60 * 24 * 7);

      OHLCBarItem myCurve = myPane.AddOHLCBar("trades", spl, Color.Black);

      AxisChangeAndDraw(myPane);

      Assert.That(myCurve.Bar.GetBarWidth(myPane, myPane.XAxis, 1.0f), Is.EqualTo(2f));
    }

    private static void AxisChangeAndDraw(GraphPane myPane)
    {
      using (Bitmap b = new Bitmap(640, 480))
      {
        using (Graphics g = Graphics.FromImage(b))
        {
          myPane.AxisChange(g);

          myPane.Draw(g);
        }
      }
    }

    private static StockPointList<StockPt> CreateStockPointList(long valueStepSizeMinutes)
    {
      var spl = new StockPointList<StockPt>();
      Random rand = new Random();

      XDate xDate = new XDate(2013, 1, 1);
      var open = 50.0f;

      for (int i = 0; i < 50; i++)
      {
        double x = xDate.XLDate;
				var close = (float)(open + rand.NextDouble() * 10.0 - 5.0);
				var hi    = (float)(Math.Max( open, close ) + rand.NextDouble() * 5.0);
				var low   = (float)(Math.Min( open, close ) - rand.NextDouble() * 5.0);

        StockPt pt = new StockPt(x, open, hi, low, close, 100000);
        spl.Add(pt);

        open = close;
        xDate.AddMinutes(valueStepSizeMinutes);

        if (XDate.XLDateToDayOfWeek(xDate.XLDate) == 6)
        {
          xDate.AddDays(2.0);
        }
      }

      return spl;
    }
  }
}
