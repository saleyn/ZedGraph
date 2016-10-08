//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2006  John Champion
//
//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.
//
//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

#region Using directives

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

#endregion

namespace ZedGraph
{
  /// <summary>
  /// This class handles the drawing of the curve <see cref="OHLCBar"/> objects.
  /// </summary>
  /// 
  /// <author> John Champion </author>
  /// <version> $Revision: 3.5 $ $Date: 2007-04-16 00:03:02 $ </version>
  [Serializable]
  public class OHLCBar : LineBase, ICloneable
  {
    #region Fields

    /// <summary>
    /// Private field that stores the total width for the Opening/Closing line
    /// segments.  Use the public property <see cref="Size"/> to access this value.
    /// </summary>
    [CLSCompliant(false)] protected float _size;

    /// <summary>
    /// The result of the autosize calculation, which is the size of the bars in
    /// user scale units.  This is converted to pixels at draw time.
    /// </summary>
    internal double _userScaleSize = 1.0;

    #endregion

    #region Defaults

    /// <summary>
    /// A simple struct that defines the
    /// default property values for the <see cref="ZedGraph.OHLCBar"/> class.
    /// </summary>
    public new struct Default
    {
      // Default Symbol properties
      /// <summary>
      /// The default width for the candlesticks (see <see cref="OHLCBar.Size" />),
      /// in units of points.
      /// </summary>
      public static float Size = 12;

      /// <summary>
      /// The default display mode for symbols (<see cref="OHLCBar.IsOpenCloseVisible"/> property).
      /// true to display symbols, false to hide them.
      /// </summary>
      public static bool IsOpenCloseVisible = true;

      /// <summary>
      /// The default value for the <see cref="ZedGraph.OHLCBar.IsAutoSize" /> property.
      /// </summary>
      public static bool IsAutoSize = true;

      /// <summary>
      /// The default color of the dot drawn at High price of a CandleStick
      /// </summary>
      public static Color HighDotColor = Color.Red;

      /// <summary>
      /// The default color of the dot drawn at Low price of a CandleStick
      /// </summary>
      public static Color LowDotColor = Color.Green;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a property that shows or hides the <see cref="OHLCBar"/> open/close "wings".
    /// </summary>
    /// <value>true to show the CandleStick wings, false to hide them</value>
    /// <seealso cref="Default.IsOpenCloseVisible"/>
    public bool IsOpenCloseVisible { get; set; }

    /// <summary>
    /// Gets or sets the total width to be used for drawing the opening/closing line
    /// segments ("wings") of the <see cref="OHLCBar" /> items. Units are points.
    /// </summary>
    /// <remarks>The size of the candlesticks can be set by this value, which
    /// is then scaled according to the scaleFactor (see
    /// <see cref="PaneBase.CalcScaleFactor"/>).  Alternatively,
    /// if <see cref="IsAutoSize"/> is true, the bar width will
    /// be set according to the maximum available cluster width less
    /// the cluster gap (see <see cref="BarSettings.GetClusterWidth"/>
    /// and <see cref="BarSettings.MinClusterGap"/>).  That is, if
    /// <see cref="IsAutoSize"/> is true, then the value of
    /// <see cref="Size"/> will be ignored.  If you modify the value of Size,
    /// then <see cref="IsAutoSize" /> will be automatically set to false.
    /// </remarks>
    /// <value>Size in points (1/72 inch)</value>
    /// <seealso cref="Default.Size"/>
    public float Size
    {
      get { return _size; }
      set { _size = value; IsAutoSize = false; }
    }

    /// <summary>
    /// Gets or sets a value that determines if the <see cref="Size" /> property will be
    /// calculated automatically based on the minimum axis scale step size between
    /// bars.
    /// </summary>
    public bool IsAutoSize    { get; set; }

    /// <summary>
    /// Color of a dot drawn at the High end of the bar (None means no dot)
    /// </summary>
    public Color HighDotColor { get; set; }

    /// <summary>
    /// Color of a dot drawn at the High end of the bar (None means no dot)
    /// </summary>
    public Color LowDotColor { get; set; }

    /// <summary>
    /// Factor used to divide bar width
    /// </summary>
    public virtual double WidthDivisor => 1.5f;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor that sets all <see cref="OHLCBar"/> properties to
    /// default values as defined in the <see cref="Default"/> class.
    /// </summary>
    public OHLCBar() : this(LineBase.Default.Color) {}

    /// <summary>
    /// Default constructor that sets the
    /// <see cref="Color"/> as specified, and the remaining
    /// <see cref="OHLCBar"/> properties to default
    /// values as defined in the <see cref="Default"/> class.
    /// </summary>
    /// <param name="color">A <see cref="Color"/> value indicating
    /// the color of the symbol
    /// </param>
    public OHLCBar(Color color) : base(color)
    {
      _size = Default.Size;
      IsAutoSize = Default.IsAutoSize;
      IsOpenCloseVisible = Default.IsOpenCloseVisible;
    }

    /// <summary>
    /// The Copy Constructor
    /// </summary>
    /// <param name="rhs">The <see cref="OHLCBar"/> object from which to copy</param>
    public OHLCBar(OHLCBar rhs) : base(rhs)
    {
      IsOpenCloseVisible = rhs.IsOpenCloseVisible;
      _size              = rhs._size;
      IsAutoSize         = rhs.IsAutoSize;
      HighDotColor       = rhs.HighDotColor;
      LowDotColor        = rhs.LowDotColor;
    }

    /// <summary>
    /// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
    /// calling the typed version of <see cref="Clone" />
    /// </summary>
    /// <returns>A deep copy of this object</returns>
    object ICloneable.Clone()
    {
      return this.Clone();
    }

    /// <summary>
    /// Typesafe, deep-copy clone method.
    /// </summary>
    /// <returns>A new, independent copy of this class</returns>
    public OHLCBar Clone()
    {
      return new OHLCBar(this);
    }

    #endregion

    #region Serialization

    /// <summary>
    /// Current schema value that defines the version of the serialized file
    /// </summary>
    public const int schema = 10;

    /// <summary>
    /// Constructor for deserializing objects
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
    /// </param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
    /// </param>
    protected OHLCBar(SerializationInfo info, StreamingContext context) :
      base(info, context)
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      int sch = info.GetInt32("schema");

      IsOpenCloseVisible = info.GetBoolean("isOpenCloseVisible");
      _size              = info.GetSingle("size");
      IsAutoSize         = info.GetBoolean("isAutoSize");
      HighDotColor       = (Color)info.GetValue("highDotColor", typeof(Color));
      LowDotColor        = (Color)info.GetValue("lowDotColor", typeof(Color));
    }

    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info,            context);
      info.AddValue("schema",             schema);
      info.AddValue("isOpenCloseVisible", IsOpenCloseVisible);
      info.AddValue("size",               _size);
      info.AddValue("isAutoSize",         IsAutoSize);
      info.AddValue("highDotColor",       HighDotColor);
      info.AddValue("lowDotColor",        LowDotColor);
    }

    #endregion

    #region Rendering Methods

    /// <summary>
    /// Draw the <see cref="OHLCBar"/> to the specified <see cref="Graphics"/>
    /// device at the specified location.
    /// </summary>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally e.Graphics from the
    /// PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="pane">
    /// A reference to the <see cref="GraphPane"/> object that is the parent or
    /// owner of this object.
    /// </param>
    /// <param name="isXBase">boolean value that indicates if the "base" axis for this
    /// <see cref="OHLCBar"/> is the X axis.  True for an <see cref="XAxis"/> base,
    /// false for a <see cref="YAxis"/> or <see cref="Y2Axis"/> base.</param>
    /// <param name="pixBase">The independent axis position of the center of the candlestick in
    /// pixel units</param>
    /// <param name="pixHigh">The dependent axis position of the top of the candlestick in
    /// pixel units</param>
    /// <param name="pixLow">The dependent axis position of the bottom of the candlestick in
    /// pixel units</param>
    /// <param name="pixOpen">The dependent axis position of the opening value of the candlestick in
    /// pixel units</param>
    /// <param name="pixClose">The dependent axis position of the closing value of the candlestick in
    /// pixel units</param>
    /// <param name="halfSize">
    /// The scaled width of the candlesticks, pixels</param>
    /// <param name="pen">A pen with attributes of <see cref="Color"/> and
    /// <see cref="LineBase.Width"/> for this <see cref="OHLCBar"/></param>
    /// <param name="dotHalfSize">The scaled half size of the dot drawn on top/bottom end of a bar</param>
    internal virtual void Draw(Graphics g, GraphPane pane, bool isXBase, float pixBase,
                               float pixHigh, float pixLow, float pixOpen, float pixClose,
                               float halfSize, Pen pen, float dotHalfSize)
    {
      if (isXBase)
      {
        if (Math.Abs(pixLow) < 1000000 && Math.Abs(pixHigh) < 1000000)
          g.DrawLine(pen, pixBase, pixHigh, pixBase, pixLow);
        if (IsOpenCloseVisible)
        {
          if (Math.Abs(pixOpen)  < 1000000)
            g.DrawLine(pen, pixBase - halfSize, pixOpen, pixBase, pixOpen);
          if (Math.Abs(pixClose) < 1000000)
            g.DrawLine(pen, pixBase, pixClose, pixBase + halfSize, pixClose);
        }
      }
      else
      {
        if (Math.Abs(pixLow) < 1000000 && Math.Abs(pixHigh) < 1000000)
          g.DrawLine(pen, pixHigh, pixBase, pixLow, pixBase);
        if (IsOpenCloseVisible)
        {
          if (Math.Abs(pixOpen)  < 1000000)
            g.DrawLine(pen, pixOpen, pixBase - halfSize, pixOpen, pixBase);
          if (Math.Abs(pixClose) < 1000000)
            g.DrawLine(pen, pixClose, pixBase, pixClose, pixBase + halfSize);
        }
      }

      if (!float.IsNaN(dotHalfSize))
        DrawHighLowDots(g, isXBase, pixBase, pixHigh, pixLow, dotHalfSize);
    }

    /// <summary>
    /// Method called just before drawing a bar
    /// </summary>
    protected virtual void BeforeDraw(Graphics g, GraphPane pane, Axis valueAxis,
                                      CurveItem curve, IPointPair pt,
                                      float pixBase, float pixHigh, float pixLow, float halfDotSz)
    {}

    /// <summary>
    /// Draw dots at the high/low ends of a bar
    /// </summary>
    protected void DrawHighLowDots(Graphics g, bool isXBase, float pixBase, float pixHigh, float pixLow, float halfDotSz)
    {
      var dotSize = halfDotSz * 2;

      if (HighDotColor != Color.Empty)
        using (var brush = new SolidBrush(HighDotColor))
        {
          if (isXBase)
            g.FillEllipse(brush, pixBase - halfDotSz, pixHigh - halfDotSz, dotSize, dotSize);
          else
            g.FillEllipse(brush, pixHigh - halfDotSz, pixBase - halfDotSz, dotSize, dotSize);
        }

      if (LowDotColor != Color.Empty)
        using (var brush = new SolidBrush(LowDotColor))
        {
          if (isXBase)
            g.FillEllipse(brush, pixBase - halfDotSz, pixLow  - halfDotSz, dotSize, dotSize);
          else
            g.FillEllipse(brush, pixLow  - halfDotSz, pixBase - halfDotSz, dotSize, dotSize);
        }
    }

    /// <summary>
    /// Draw all the <see cref="OHLCBar"/>'s to the specified <see cref="Graphics"/>
    /// device as a candlestick at each defined point.
    /// </summary>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally e.Graphics from the
    /// PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="pane">
    /// A reference to the <see cref="GraphPane"/> object that is the parent or
    /// owner of this object.
    /// </param>
    /// <param name="curve">A <see cref="OHLCBarItem"/> object representing the
    /// <see cref="OHLCBar"/>'s to be drawn.</param>
    /// <param name="baseAxis">The <see cref="Axis"/> class instance that defines the base (independent)
    /// axis for the <see cref="OHLCBar"/></param>
    /// <param name="valueAxis">The <see cref="Axis"/> class instance that defines the value (dependent)
    /// axis for the <see cref="OHLCBar"/></param>
    /// <param name="scaleFactor">
    /// The scaling factor to be used for rendering objects.  This is calculated and
    /// passed down by the parent <see cref="GraphPane"/> object using the
    /// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
    /// font sizes, etc. according to the actual size of the graph.
    /// </param>
    public virtual void Draw(Graphics g, GraphPane pane, OHLCBarItem curve,
                             Axis baseAxis, Axis valueAxis, float scaleFactor)
    {
      //ValueHandler valueHandler = new ValueHandler( pane, false );

      if (curve.Points == null) return;

      //float halfSize = _size * scaleFactor;
      var halfSize = GetBarWidth(pane, baseAxis, scaleFactor);
      var dotHalfSize = Math.Max(curve.DotHalfSize, IsAutoSize ? Math.Max(2, halfSize / 4) : curve.DotHalfSize)
                      * scaleFactor;

      using (var pen = !curve.IsSelected
                         ? new Pen(Color, Width)
                         : new Pen(Selection.Border.Color, Selection.Border.Width))
        //        using ( Pen pen = new Pen( _color, _penWidth ) )
      {
        // Loop over each defined point              
        for (int i = 0; i < curve.Points.Count; i++)
        {
          var pt = curve.Points[i];
          double date;
          double open;
          double high;
          double low;
          double close;
          GetOHLC(pt, out date, out open, out high, out low, out close);

          // Any value set to double max is invalid and should be skipped
          // This is used for calculated values that are out of range, divide
          //   by zero, etc.
          // Also, any value <= zero on a log scale is invalid

          if (curve.Points[i].IsInvalid || (!(date > 0) && baseAxis.Scale.IsLog) ||
              ((!(high > 0) || !(low > 0)) && valueAxis.Scale.IsLog))
            continue;

          var pixBase  =
            (int)(baseAxis.Scale.Transform(curve.IsOverrideOrdinal, i, date) + 0.5);
          //pixBase = baseAxis.Scale.Transform( curve.IsOverrideOrdinal, i, date );
          var pixHigh  = valueAxis.Scale.Transform(curve.IsOverrideOrdinal, i, high);
          var pixLow   = valueAxis.Scale.Transform(curve.IsOverrideOrdinal, i, low);
          var pixOpen  = PointPairBase.IsValueInvalid(open)
                          ? float.MaxValue
                          : valueAxis.Scale.Transform(curve.IsOverrideOrdinal, i, open);

          var pixClose = PointPair.IsValueInvalid(close)
                          ? float.MaxValue
                          : valueAxis.Scale.Transform(curve.IsOverrideOrdinal, i, close);

          if (pixBase == PointPair.Missing) continue;

          BeforeDraw(g, pane, valueAxis, curve, pt, pixBase, pixHigh, pixLow, halfSize);

          var gradient = !curve.IsSelected && this.GradientFill.IsGradientValueType;

          if (gradient)
          {
            using (var tPen = GetPen(pane, scaleFactor, pt))
              Draw(g, pane, baseAxis is IXAxis,
                   pixBase, pixHigh, pixLow, pixOpen,
                   pixClose, halfSize, tPen, dotHalfSize);
          }
          else
            Draw(g, pane, baseAxis is IXAxis,
                 pixBase, pixHigh, pixLow, pixOpen,
                 pixClose, halfSize, pen, dotHalfSize);
        }
      }
    }

    /// <summary>
    /// Returns the width of the candleStick, in pixels, based on the settings for
    /// <see cref="Size"/> and <see cref="IsAutoSize"/>.
    /// </summary>
    /// <param name="pane">The parent <see cref="GraphPane"/> object.</param>
    /// <param name="baseAxis">The <see cref="Axis"/> object that
    /// represents the bar base (independent axis).</param>
    /// <param name="scaleFactor">
    /// The scaling factor to be used for rendering objects.  This is calculated and
    /// passed down by the parent <see cref="GraphPane"/> object using the
    /// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
    /// font sizes, etc. according to the actual size of the graph.
    /// </param>
    /// <returns>The width of each bar, in pixel units</returns>
    public virtual float GetBarWidth(GraphPane pane, Axis baseAxis, float scaleFactor)
    {
      var width = IsAutoSize
                    ? baseAxis.Scale.GetClusterWidth(_userScaleSize)/
                      (1.0F + pane._barSettings.MinClusterGap) / WidthDivisor
                    : Size*scaleFactor / WidthDivisor;

      // use integral size
      return (int)(width + 0.5f);
    }

    #endregion

    protected static void GetOHLC(IPointPair pt, out double date, out double o,
                                  out double h, out double l, out double c)
    {
      date = pt.X;
      if (pt is StockPt)
      {
        var p = (StockPt)pt;
        o = p.Open;
        h = p.High;
        l = p.Low;
        c = p.Close;
      }
      else
      {
        o = PointPair.Missing;
        h = pt.Y;
        l = pt.Z;
        c = PointPair.Missing;
      }
    }
  }
}
