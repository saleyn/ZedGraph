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
  /// Encapsulates a CandleStick curve type that displays a vertical (or horizontal)
  /// line displaying the range of data values at each sample point, plus an starting
  /// mark and an ending mark signifying the opening and closing value for the sample.
  /// </summary>
  /// <remarks>For this type to work properly, your <see cref="IPointList" /> must contain
  /// <see cref="StockPt" /> objects, rather than ordinary <see cref="PointPair" /> types.
  /// This is because the <see cref="OHLCBarItem"/> type actually displays 5 data values
  /// but the <see cref="PointPair" /> only stores 3 data values.  The <see cref="StockPt" />
  /// stores <see cref="StockPt.Date" />, <see cref="StockPt.Close" />,
  /// <see cref="StockPt.Open" />, <see cref="StockPt.High" />, and
  /// <see cref="StockPt.Low" /> members.
  /// For a vertical CandleStick chart, the opening value is drawn as a horizontal line
  /// segment to the left of the vertical range bar, and the closing value is a horizontal
  /// line segment to the right.  The total length of these two line segments is controlled
  /// by the <see cref="ZedGraph.OHLCBar.Size" /> property, which is specified in
  /// points (1/72nd inch), and scaled according to <see cref="PaneBase.CalcScaleFactor" />.
  /// The candlesticks are drawn horizontally or vertically depending on the
  /// value of <see cref="BarSettings.Base"/>, which is a
  /// <see cref="ZedGraph.BarBase"/> enum type.</remarks>
  /// <author> John Champion </author>
  /// <version> $Revision: 3.4 $ $Date: 2007-12-31 00:23:05 $ </version>
  [Serializable]
  public class OHLCBarItem : CurveItem, ICloneable, IBarItem
  {
    #region Fields

    [CLSCompliant(false)] protected float _dotSize = 1f;
    [CLSCompliant(false)] protected float _dotHalfSize = 0.5f;

    #endregion

    #region Properties
    /// <summary>
    /// Gets a reference to the <see cref="OHLCBar"/> class defined
    /// for this <see cref="OHLCBarItem"/>.
    /// </summary>
    public OHLCBar Bar { get; }

    /// <summary>
    /// Gets a flag indicating if the X axis is the independent axis for this <see cref="CurveItem" />
    /// </summary>
    /// <param name="pane">The parent <see cref="GraphPane" /> of this <see cref="CurveItem" />.
    /// </param>
    /// <value>true if the X axis is independent, false otherwise</value>
    internal override bool IsXIndependent( GraphPane pane )
    {
      return pane._barSettings.Base == BarBase.X;
    }

    /// <summary>
    /// Gets a flag indicating if the Z data range should be included in the axis scaling calculations.
    /// </summary>
    /// <remarks>
    /// IsZIncluded is true for <see cref="OHLCBarItem" /> objects, since the Y and Z
    /// values are defined as the High and Low values for the day.</remarks>
    /// <param name="pane">The parent <see cref="GraphPane" /> of this <see cref="CurveItem" />.
    /// </param>
    /// <value>true if the Z data are included, false otherwise</value>
    internal override bool IsZIncluded( GraphPane pane )
    {
      return true;
    }

    /// <summary>
    ///   Size of dots drawn at the High/Low end of a candlestick
    /// </summary>
    public float DotSize
    {
      get { return _dotSize; }
      set { _dotSize = Math.Max(value, 1f); _dotHalfSize = _dotSize / 2f; }
    }

    /// <summary>
    /// Half size of a dot drawn at the High/Low end of a candlestick
    /// </summary>
    internal float DotHalfSize => _dotHalfSize;

    /// <summary>
    /// Color of a dot drawn at the High end of the bar (None means no dot)
    /// </summary>
    public Color HighDotColor { get; set; } = Default.HighDotColor;

    /// <summary>
    /// Color of a dot drawn at the High end of the bar (None means no dot)
    /// </summary>
    public Color LowDotColor { get; set; } = Default.LowDotColor;

    #endregion

    #region Defaults

    /// <summary>
    /// A simple struct that defines the
    /// default property values for the <see cref="ZedGraph.JapaneseCandleStick"/> class.
    /// </summary>
    public struct Default
    {
      /// <summary>
      /// The default color of the dot drawn at High price of a CandleStick
      /// </summary>
      public static Color HighDotColor = Color.LightPink;

      /// <summary>
      /// The default color of the dot drawn at Low price of a CandleStick
      /// </summary>
      public static Color LowDotColor = Color.LightSkyBlue;
    }

    #endregion

    #region Constructors
    /// <summary>
    /// Create a new <see cref="OHLCBarItem"/>, specifying only the legend label.
    /// </summary>
    /// <param name="label">The label that will appear in the legend.</param>
    public OHLCBarItem( string label, int zOrder=-1)
      : base( label, zOrder )
    {
      Bar = MakeBar();
      DotSize = 3;
    }

    /// <summary>
    /// Create a new <see cref="OHLCBarItem"/> using the specified properties.
    /// </summary>
    /// <param name="label">The _label that will appear in the legend.</param>
    /// <param name="points">An <see cref="IPointList"/> of double precision values that define
    /// the Date, Close, Open, High, and Low values for the curve.  Note that this
    /// <see cref="IPointList" /> should contain <see cref="StockPt" /> items rather
    /// than <see cref="PointPair" /> items.
    /// </param>
    /// <param name="color">
    /// The <see cref="System.Drawing.Color" /> to use for drawing the candlesticks.</param>
    public OHLCBarItem( string label, IPointList points, Color color, int zOrder=-1 )
      : base( label, points, zOrder )
    {
      Bar = MakeBar(color);
      DotSize = 3;
    }

    /// <summary>
    /// The Copy Constructor
    /// </summary>
    /// <param name="rhs">The <see cref="OHLCBarItem"/> object from which to copy</param>
    public OHLCBarItem( OHLCBarItem rhs )
      : base( rhs )
    {
      Bar = rhs.Bar.Clone();
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
    public OHLCBarItem Clone()
    {
      return new OHLCBarItem( this );
    }

    protected virtual OHLCBar MakeBar(Color color = default(Color))
    {
      return new OHLCBar(color.IsEmpty ? LineBase.Default.Color : color);
    }

  #endregion

  #region Serialization

    /// <summary>
    /// Current schema value that defines the version of the serialized file
    /// </summary>
    public const int schema2 = 10;

    /// <summary>
    /// Constructor for deserializing objects
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
    /// </param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
    /// </param>
    protected OHLCBarItem( SerializationInfo info, StreamingContext context )
      : base( info, context )
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      int sch = info.GetInt32( "schema2" );

      Bar = (OHLCBar)info.GetValue( "stick", typeof( OHLCBar ) );
      DotSize      = info.GetSingle("dotSize");
      HighDotColor = (Color)info.GetValue("highDotColor", typeof(Color));
      LowDotColor  = (Color)info.GetValue("lowDotColor", typeof(Color));
    }
    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
    [SecurityPermissionAttribute( SecurityAction.Demand, SerializationFormatter = true )]
    public override void GetObjectData( SerializationInfo info, StreamingContext context )
    {
      base.GetObjectData( info, context );

      info.AddValue("schema2",      schema2);
      info.AddValue("stick",        Bar);
      info.AddValue("dotSize",      DotSize);
      info.AddValue("highDotColor", HighDotColor);
      info.AddValue("lowDotColor",  LowDotColor);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Do all rendering associated with this <see cref="OHLCBarItem"/> to the specified
    /// <see cref="Graphics"/> device.  This method is normally only
    /// called by the Draw method of the parent <see cref="ZedGraph.CurveList"/>
    /// collection object.
    /// </summary>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally e.Graphics from the
    /// PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="pane">
    /// A reference to the <see cref="ZedGraph.GraphPane"/> object that is the parent or
    /// owner of this object.
    /// </param>
    /// <param name="pos">The ordinal position of the current <see cref="OHLCBarItem"/>
    /// curve.</param>
    /// <param name="scaleFactor">
    /// The scaling factor to be used for rendering objects.  This is calculated and
    /// passed down by the parent <see cref="ZedGraph.GraphPane"/> object using the
    /// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
    /// font sizes, etc. according to the actual size of the graph.
    /// </param>
    public override void Draw(Graphics g, GraphPane pane, int pos, float scaleFactor)
    {
      if (IsVisible)
        Bar.Draw(g, pane, this, BaseAxis(pane), ValueAxis(pane), scaleFactor);
    }

    /// <summary>
    /// Draw a legend key entry for this <see cref="OHLCBarItem"/> at the specified location
    /// </summary>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally e.Graphics from the
    /// PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="pane">
    /// A reference to the <see cref="ZedGraph.GraphPane"/> object that is the parent or
    /// owner of this object.
    /// </param>
    /// <param name="rect">The <see cref="RectangleF"/> struct that specifies the
    /// location for the legend key</param>
    /// <param name="scaleFactor">
    /// The scaling factor to be used for rendering objects.  This is calculated and
    /// passed down by the parent <see cref="ZedGraph.GraphPane"/> object using the
    /// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
    /// font sizes, etc. according to the actual size of the graph.
    /// </param>
    public override void DrawLegendKey( Graphics g, GraphPane pane, RectangleF rect,
                  float scaleFactor )
    {
      float pixBase, pixHigh, pixLow, pixOpen, pixClose;

      if ( pane._barSettings.Base == BarBase.X )
      {
        pixBase  = rect.Left + rect.Width / 2.0F;
        pixHigh  = rect.Top;
        pixLow   = rect.Bottom;
        pixOpen  = pixHigh + rect.Height / 4;
        pixClose = pixLow - rect.Height / 4;
      }
      else
      {
        pixBase  = rect.Top + rect.Height / 2.0F;
        pixHigh  = rect.Right;
        pixLow   = rect.Left;
        pixOpen  = pixHigh - rect.Width / 4;
        pixClose = pixLow + rect.Width / 4;
      }

      var halfSize = 2.0f * scaleFactor;

      using ( var pen = new Pen( Bar.Color, Bar.Width ) )
      {
        Bar.Draw(g, pane, pane._barSettings.Base == BarBase.X, pixBase, pixHigh,
                 pixLow, pixOpen, pixClose, halfSize, pen, float.NaN);
      }
    }

    /// <summary>
    /// Determine the coords for the rectangle associated with a specified point for 
    /// this <see cref="CurveItem" />
    /// </summary>
    /// <param name="pane">The <see cref="GraphPane" /> to which this curve belongs</param>
    /// <param name="i">The index of the point of interest</param>
    /// <param name="coords">A list of coordinates that represents the "rect" for
    /// this point (used in an html AREA tag)</param>
    /// <returns>true if it's a valid point, false otherwise</returns>
    public override bool GetCoords( GraphPane pane, int i, out string coords )
    {
      coords = string.Empty;

      if ( i < 0 || i >= Points.Count )
        return false;

      var valueAxis = ValueAxis( pane );
      var baseAxis = BaseAxis( pane );

      var halfSize = Bar.Size * pane.CalcScaleFactor();

      var pt   = Points[i];
      var date = pt.X;
      double high;
      double low;
      if (pt is StockPt)
      {
        var p = (StockPt)pt;
        high  = p.High;
        low   = p.Low;
      }
      else
      {
        high  = pt.Y;
        low   = pt.Z;
      }

      if (pt.IsInvalid || (date <= 0  && baseAxis.Scale.IsLog) ||
         ((high <= 0   ||  low  <= 0) && valueAxis.Scale.IsLog))
        return false;

      var pixBase = baseAxis.Scale.Transform( IsOverrideOrdinal, i, date );
      var pixHigh = valueAxis.Scale.Transform( IsOverrideOrdinal, i, high );
      var pixLow  = valueAxis.Scale.Transform( IsOverrideOrdinal, i, low );

      // Calculate the pixel location for the side of the bar (on the base axis)
      var pixSide = pixBase - halfSize;

      // Draw the bar
      coords = baseAxis is IXAxis
        ? $"{pixSide:f0},{pixLow:f0},{pixSide + halfSize*2:f0},{pixHigh:f0}"
        : $"{pixLow:f0},{pixSide:f0},{pixHigh:f0},{pixSide + halfSize*2:f0}";

      return true;
    }

  #endregion

  }
}
