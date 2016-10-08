//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2004  John Champion
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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
  /// <summary>
  /// This class handles the drawing of the curve <see cref="Symbol"/> objects.
  /// The symbols are the small shapes that appear over each defined point
  /// along the curve.
  /// </summary>
  /// 
  /// <author> John Champion </author>
  /// <version> $Revision: 3.37 $ $Date: 2007-09-19 06:41:56 $ </version>
  [Serializable]
  public class Symbol : ICloneable, ISerializable
  {
  #region Defaults
    /// <summary>
    /// A simple struct that defines the
    /// default property values for the <see cref="ZedGraph.Symbol"/> class.
    /// </summary>
    public struct Default
    {
      // Default Symbol properties
      /// <summary>
      /// The default size for curve symbols (<see cref="Symbol.Size"/> property),
      /// in units of points.
      /// </summary>
      public static float Size = 7;
      /// <summary>
      /// The default pen width to be used for drawing curve symbols
      /// (<see cref="ZedGraph.LineBase.Width"/> property).  Units are points.
      /// </summary>
      public static float PenWidth = 1.0F;
      /// <summary>
      /// The default color for filling in this <see cref="Symbol"/>
      /// (<see cref="ZedGraph.Fill.Color"/> property).
      /// </summary>
      public static Color FillColor = Color.Red;
      /// <summary>
      /// The default custom brush for filling in this <see cref="Symbol"/>
      /// (<see cref="ZedGraph.Fill.Brush"/> property).
      /// </summary>
      public static Brush FillBrush = null;
      /// <summary>
      /// The default fill mode for the curve (<see cref="ZedGraph.Fill.Type"/> property).
      /// </summary>
      public static FillType FillType = FillType.None;
      /// <summary>
      /// The default symbol type for curves (<see cref="Symbol.Type"/> property).
      /// This is defined as a <see cref="ZedGraph.SymbolType"/> enumeration.
      /// </summary>
      public static SymbolType Type = SymbolType.Square;
      /// <summary>
      /// The default value for the <see cref="Symbol.IsAntiAlias"/>
      /// property.
      /// </summary>
      public static bool IsAntiAlias = false;
      /// <summary>
      /// The default display mode for symbols (<see cref="Symbol.IsVisible"/> property).
      /// true to display symbols, false to hide them.
      /// </summary>
      public static bool IsVisible = true;
      /// <summary>
      /// The default for drawing frames around symbols (<see cref="ZedGraph.LineBase.IsVisible"/> property).
      /// true to display symbol frames, false to hide them.
      /// </summary>
      public static bool IsBorderVisible = true;
      /// <summary>
      /// The default color for drawing symbols (<see cref="ZedGraph.LineBase.Color"/> property).
      /// </summary>
      public static Color BorderColor = Color.Red;
    }
  #endregion

  #region Properties
    /// <summary>
    /// Gets or sets the size of the <see cref="Symbol"/>
    /// </summary>
    /// <value>Size in points (1/72 inch)</value>
    /// <seealso cref="Default.Size"/>
    public float Size { get; set; }

    /// <summary>
    /// Gets or sets the type (shape) of the <see cref="Symbol"/>
    /// </summary>
    /// <value>A <see cref="SymbolType"/> enum value indicating the shape</value>
    /// <seealso cref="Default.Type"/>
    public SymbolType Type { get; set; }

    /// <summary>
    /// Gets or sets a value that determines if the symbols are drawn using
    /// Anti-Aliasing capabilities from the <see cref="Graphics" /> class.
    /// </summary>
    /// <remarks>
    /// If this value is set to true, then the <see cref="Graphics.SmoothingMode" />
    /// property will be set to <see cref="SmoothingMode.HighQuality" /> only while
    /// this <see cref="Symbol" /> is drawn.  A value of false will leave the value of
    /// <see cref="Graphics.SmoothingMode" /> unchanged.
    /// </remarks>
    public bool IsAntiAlias { get; set; }

    /// <summary>
    /// Gets or sets a property that shows or hides the <see cref="Symbol"/>.
    /// </summary>
    /// <value>true to show the symbol, false to hide it</value>
    /// <seealso cref="Default.IsVisible"/>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="ZedGraph.Fill"/> data for this
    /// <see cref="Symbol"/>.
    /// </summary>
    public Fill  Fill { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="ZedGraph.Border"/> data for this
    /// <see cref="Symbol"/>, which controls the border outline of the symbol.
    /// </summary>
    public Border Border { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="GraphicsPath"/> data for this
    /// <see cref="Symbol"/>, describing the user-defined symbol type.
    /// </summary>
    /// <remarks>
    /// This value only applies if <see cref="Symbol.Type">Symbol.Type</see>
    /// is <see cref="SymbolType.UserDefined">SymbolType.UserDefined</see>
    /// </remarks>
    public GraphicsPath UserSymbol { get; set; }

  #endregion
  
  #region Constructors
    /// <summary>
    /// Default constructor that sets all <see cref="Symbol"/> properties to default
    /// values as defined in the <see cref="Default"/> class.
    /// </summary>
    public Symbol() : this( SymbolType.Default, Color.Empty )
    {
    }

    /// <summary>
    /// Default constructor that sets the <see cref="SymbolType"/> and
    /// <see cref="Color"/> as specified, and the remaining
    /// <see cref="Symbol"/> properties to default
    /// values as defined in the <see cref="Default"/> class.
    /// </summary>
    /// <param name="type">A <see cref="SymbolType"/> enum value
    /// indicating the shape of the symbol</param>
    /// <param name="color">A <see cref="Color"/> value indicating
    /// the color of the symbol
    /// </param>
    public Symbol( SymbolType type, Color color)
      : this(type, color, Default.Size)
    {}

    /// <summary>
    /// Default constructor that sets the <see cref="SymbolType"/> and
    /// <see cref="Color"/> as specified, and the remaining
    /// <see cref="Symbol"/> properties to default
    /// values as defined in the <see cref="Default"/> class.
    /// </summary>
    /// <param name="type">A <see cref="SymbolType"/> enum value
    /// indicating the shape of the symbol</param>
    /// <param name="color">A <see cref="Color"/> value indicating
    /// the color of the symbol
    /// </param>
    /// <param name="size">A <see cref="Size"/> value
    /// indicating the size the symbol</param>
    public Symbol( SymbolType type, Color color, float size )
    {
      Size        = size;
      Type        = type;
      IsAntiAlias = Default.IsAntiAlias;
      IsVisible   = Default.IsVisible;
      Border      = new Border( Default.IsBorderVisible, color, Default.PenWidth );
      Fill        = new Fill( color, Default.FillBrush, Default.FillType );
      UserSymbol  = null;
    }

    /// <summary>
    /// The Copy Constructor
    /// </summary>
    /// <param name="rhs">The Symbol object from which to copy</param>
    public Symbol( Symbol rhs )
    {
      Size        = rhs.Size;
      Type        = rhs.Type;
      IsAntiAlias = rhs.IsAntiAlias;
      IsVisible   = rhs.IsVisible;
      Fill        = rhs.Fill.Clone();
      Border      = rhs.Border.Clone();
      UserSymbol  = rhs.UserSymbol?.Clone() as GraphicsPath;
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
    public Symbol Clone()
    {
      return new Symbol( this );
    }

  #endregion

  #region Serialization
    /// <summary>
    /// Current schema value that defines the version of the serialized file
    /// </summary>
    public const int schema = 11;

    /// <summary>
    /// Constructor for deserializing objects
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
    /// </param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
    /// </param>
    protected Symbol( SerializationInfo info, StreamingContext context )
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      int sch     = info.GetInt32( "schema" );

      Size        = info.GetSingle( "size" );
      Type        = (SymbolType) info.GetValue( "type", typeof(SymbolType) );
      IsAntiAlias = info.GetBoolean( "isAntiAlias" );
      IsVisible   = info.GetBoolean( "isVisible" );
      Fill        = (Fill) info.GetValue( "fill", typeof(Fill) );
      Border      = (Border) info.GetValue( "border", typeof(Border) );

      UserSymbol = sch >= 11
        ? (GraphicsPath)info.GetValue("userSymbol", typeof(GraphicsPath))
        : null;
    }
    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
    [SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
    public virtual void GetObjectData( SerializationInfo info, StreamingContext context )
    {
      info.AddValue( "schema", schema );
      info.AddValue( "size", Size );
      info.AddValue( "type", Type );
      info.AddValue( "isAntiAlias", IsAntiAlias );
      info.AddValue( "isVisible", IsVisible );
      info.AddValue( "fill", Fill );
      info.AddValue( "border", Border );
      info.AddValue( "userSymbol", UserSymbol );
    }
    #endregion

    #region Rendering Methods

    /// <summary>
    /// Draw the <see cref="Symbol"/> to the specified <see cref="Graphics"/> device
    /// at the specified location.  This routine draws a single symbol.
    /// </summary>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally e.Graphics from the
    /// PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="x">The x position of the center of the symbol in
    /// pixel units</param>
    /// <param name="y">The y position of the center of the symbol in
    /// pixel units</param>
    /// <param name="path">A <see cref="GraphicsPath"/> previously constructed by
    /// <see cref="MakePath"/> for this symbol</param>
    /// <param name="pen">A <see cref="Pen"/> class representing the standard pen for this symbol</param>
    /// <param name="brush">A <see cref="Brush"/> class representing a default solid brush for this symbol
    /// If this symbol uses a <see cref="LinearGradientBrush"/>, it will be created on the fly for
    /// each point, since it has to be scaled to the individual point coordinates.</param>
    /// <param name="type">Symbol type to draw</param>
    /// <param name="fill">Fill object to use for drawing</param>
    /// <param name="border">Border object to use for drawing</param>
    private static bool drawSymbol( Graphics g, int x, int y, GraphicsPath path, Pen pen, Brush brush,
      SymbolType type, Fill fill, Border border)
    {
      if (type == SymbolType.None || Math.Abs(x) >= 100000 || Math.Abs(y) >= 100000)
        return false;

      var saveMatrix = g.Transform;
      g.TranslateTransform( x, y );

      // Fill or draw the symbol as required
      if ( fill.IsVisible )
        g.FillPath( brush, path );
      //FillPoint( g, x, y, scaleFactor, pen, brush );

      if ( border.IsVisible )
        g.DrawPath( pen, path );
      //DrawPoint( g, x, y, scaleFactor, pen );

      g.Transform = saveMatrix;
      return true;
    }

    /// <summary>
    /// Draw the <see cref="Symbol"/> to the specified <see cref="Graphics"/> device
    /// at the specified location.  This routine draws a single symbol.
    /// </summary>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally e.Graphics from the
    /// PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="pane">
    /// A reference to the <see cref="ZedGraph.GraphPane"/> object that is the parent or
    /// owner of this object.
    /// </param>
    /// <param name="x">The x position of the center of the symbol in
    /// pixel units</param>
    /// <param name="y">The y position of the center of the symbol in
    /// pixel units</param>
    /// <param name="scaleFactor">
    /// The scaling factor for the features of the graph based on the <see cref="PaneBase.BaseDimension"/>.  This
    /// scaling factor is calculated by the <see cref="PaneBase.CalcScaleFactor"/> method.  The scale factor
    /// represents a linear multiple to be applied to font sizes, symbol sizes, etc.
    /// </param>
    /// <param name="dataValue">The data value to be used for a value-based
    /// color gradient.  This is only applicable for <see cref="FillType.GradientByX"/>,
    /// <see cref="FillType.GradientByY"/> or <see cref="FillType.GradientByZ"/>.</param>
    /// <param name="isSelected">Indicates that the <see cref="Symbol" /> should be drawn
    /// with attributes from the <see cref="Selection" /> class.
    /// </param>
    public void DrawSymbol( Graphics g, GraphPane pane, int x, int y,
                            float scaleFactor, bool isSelected, IPointPair dataValue )
    {
      var source = isSelected ? Selection.Symbol : this;
      DrawSymbol(g, pane, Type, x, y, Size, IsVisible, source.Fill, source.Border,
        scaleFactor, IsAntiAlias, dataValue);
    }

    /// <summary>
    /// Used internally to draw symbol.
    /// </summary>
    internal static void DrawSymbol(Graphics g, GraphPane pane, SymbolType type,
      int x, int y, float size,
      bool isVisible, Fill fill, Border border, float scaleFactor,
      bool isAntiAlias, IPointPair dataValue)
    {
      // Only draw if the symbol is visible
      if (!isVisible || type == SymbolType.None || Math.Abs(x) >= 100000 || Math.Abs(y) >= 100000)
        return;

      var sModeSave = g.SmoothingMode;
      if (isAntiAlias)
        g.SmoothingMode = SmoothingMode.HighQuality;

      using (var pen    = border.GetPen(pane, scaleFactor, dataValue))
      using (var path   = MakePath(g, type, scaleFactor, size))
      using (var brush  = fill.MakeBrush(path.GetBounds(), dataValue))
        drawSymbol(g, x, y, path, pen, brush, type, fill, border);

      g.SmoothingMode = sModeSave;
    }

    /// <summary>
    /// Create a <see cref="GraphicsPath"/> struct for the current symbol based on the
    /// specified scaleFactor and assuming the symbol will be centered at position 0,0.
    /// </summary>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally e.Graphics from the
    /// PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="scaleFactor">
    /// The scaling factor for the features of the graph based on the <see cref="PaneBase.BaseDimension"/>.  This
    /// scaling factor is calculated by the <see cref="PaneBase.CalcScaleFactor"/> method.  The scale factor
    /// represents a linear multiple to be applied to font sizes, symbol sizes, etc.</param>
    /// <returns>Returns the <see cref="GraphicsPath"/> for the current symbol</returns>
    public GraphicsPath MakePath(Graphics g, float scaleFactor)
    {
      return MakePath(g, Type, scaleFactor, Size, UserSymbol);
    }

    static internal GraphicsPath MakePath( Graphics g, SymbolType type, float scaleFactor, float size,
      GraphicsPath userSymbol = null )
    {
      var   scaledSize = size * scaleFactor;
      float hsize      = scaledSize / 2,
            hsize1     = hsize + 1;
      
      var path = new GraphicsPath();
      var tp   = type == SymbolType.Default || (type == SymbolType.UserDefined && userSymbol == null) ? Default.Type : type;

      switch ( tp )
      {
      case SymbolType.Square:
        path.AddLine( -hsize, -hsize, hsize, -hsize );
        path.AddLine( hsize, -hsize, hsize, hsize );
        path.AddLine( hsize, hsize, -hsize, hsize );
        path.AddLine( -hsize, hsize, -hsize, -hsize );
        break;
      case SymbolType.Diamond:
        path.AddLine( 0, -hsize, hsize, 0 );
        path.AddLine( hsize, 0, 0, hsize );
        path.AddLine( 0, hsize, -hsize, 0 );
        path.AddLine( -hsize, 0, 0, -hsize );
        break;
      case SymbolType.Triangle:
        path.AddLine( 0, -hsize, hsize, hsize );
        path.AddLine( hsize, hsize, -hsize, hsize );
        path.AddLine( -hsize, hsize, 0, -hsize );
        break;
      case SymbolType.Circle:
        path.AddEllipse( -hsize, -hsize, scaledSize, scaledSize );
        break;
      case SymbolType.XCross:
        path.AddLine( -hsize, -hsize, hsize1, hsize1 );
        path.StartFigure();
        path.AddLine( hsize, -hsize, -hsize1, hsize1 );
        break;
      case SymbolType.Plus:
        path.AddLine( 0, -hsize, 0, hsize1 );
        path.StartFigure();
        path.AddLine( -hsize, 0, hsize1, 0 );
        break;
      case SymbolType.Star:
        path.AddLine( 0, -hsize, 0, hsize1 );
        path.StartFigure();
        path.AddLine( -hsize, 0, hsize1, 0 );
        path.StartFigure();
        path.AddLine( -hsize, -hsize, hsize1, hsize1 );
        path.StartFigure();
        path.AddLine( hsize, -hsize, -hsize1, hsize1 );
        break;
      case SymbolType.TriangleDown:
        path.AddLine( 0, hsize, hsize, -hsize );
        path.AddLine( hsize, -hsize, -hsize, -hsize );
        path.AddLine( -hsize, -hsize, 0, hsize );
        break;
      case SymbolType.HDash:
        path.AddLine( -hsize, 0, hsize1, 0 );
        break;
      case SymbolType.VDash:
        path.AddLine( 0, -hsize, 0, hsize1 );
        break;
      case SymbolType.ArrowUp:
      {
        // Coordinate 0,0 corresponds to the pointing tip of the arrow
        var len = 2*scaledSize;
        path.AddLine(0, 0, -scaledSize, scaledSize);
        path.AddLine(-scaledSize, scaledSize, -hsize, scaledSize);
        path.AddLine(-hsize, scaledSize, -hsize, len);
        path.AddLine(-hsize, len, hsize, len);
        path.AddLine( hsize, len, hsize, scaledSize);
        path.AddLine( hsize, scaledSize,  scaledSize, scaledSize);
        path.AddLine(scaledSize, scaledSize, 0, 0);
        break;
      }
      case SymbolType.ArrowDown:
      {
        // Coordinate 0,0 corresponds to the pointing tip of the arrow
        var len = 2*scaledSize;
        path.AddLine(0, 0, -scaledSize, -scaledSize);
        path.AddLine(-scaledSize, -scaledSize, -hsize, -scaledSize);
        path.AddLine(-hsize, -scaledSize, -hsize, -len);
        path.AddLine(-hsize, -len, hsize, -len);
        path.AddLine( hsize, -len, hsize, -scaledSize);
        path.AddLine( hsize, -scaledSize,  scaledSize, -scaledSize);
        path.AddLine(scaledSize, -scaledSize, 0, 0);
        break;
      }
      case SymbolType.ArrowLeft:
      {
        // Coordinate 0,0 corresponds to the pointing tip of the arrow
        var len = 2*scaledSize;
        path.AddLine(0, 0, scaledSize, scaledSize);
        path.AddLine(scaledSize, scaledSize, scaledSize, hsize);
        path.AddLine(scaledSize, hsize, len, hsize);
        path.AddLine(len, hsize, len, -hsize);
        path.AddLine(len,-hsize, scaledSize, -hsize);
        path.AddLine(scaledSize, -hsize, scaledSize, -scaledSize);
        path.AddLine(scaledSize, -scaledSize, 0, 0);
        break;
      }
      case SymbolType.ArrowRight:
      {
        // Coordinate 0,0 corresponds to the pointing tip of the arrow
        var len = 2*scaledSize;
        path.AddLine(0, 0, -scaledSize, -scaledSize);
        path.AddLine(-scaledSize, -scaledSize, -scaledSize, -hsize);
        path.AddLine(-scaledSize, -hsize, -len, -hsize);
        path.AddLine(-len, -hsize, -len, hsize);
        path.AddLine(-len,  hsize, -scaledSize, hsize);
        path.AddLine(-scaledSize, hsize, -scaledSize, scaledSize);
        path.AddLine(-scaledSize, scaledSize, 0, 0);
        break;
      }

      case SymbolType.UserDefined:
        if (userSymbol == null)
          throw new ArgumentNullException(nameof(UserSymbol));
        using (var gpath = (GraphicsPath)userSymbol.Clone())
        {
          var scaleTransform = new Matrix(scaledSize, 0.0f, 0.0f, scaledSize, 0.0f, 0.0f);
          gpath.Transform(scaleTransform);
        }
        break;

      case SymbolType.None:
        break;
      case SymbolType.Default:
        break;

      default:
        throw new ArgumentOutOfRangeException();
      }

      return path;
    }

    /// <summary>
    /// Draw this <see cref="CurveItem"/> to the specified <see cref="Graphics"/>
    /// device as a symbol at each defined point.  The routine
    /// only draws the symbols; the lines are draw by the
    /// <see cref="Line.DrawCurve"/> method.  This method
    /// is normally only called by the Draw method of the
    /// <see cref="CurveItem"/> object
    /// </summary>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally e.Graphics from the
    /// PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="pane">
    /// A reference to the <see cref="GraphPane"/> object that is the parent or
    /// owner of this object.
    /// </param>
    /// <param name="curve">A <see cref="LineItem"/> representing this
    /// curve.</param>
    /// <param name="scaleFactor">
    /// The scaling factor to be used for rendering objects.  This is calculated and
    /// passed down by the parent <see cref="GraphPane"/> object using the
    /// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
    /// font sizes, etc. according to the actual size of the graph.
    /// </param>
    /// <param name="isSelected">Indicates that the <see cref="Symbol" /> should be drawn
    /// with attributes from the <see cref="Selection" /> class.
    /// </param>
    public void Draw(Graphics g, GraphPane pane, LineItem curve, float scaleFactor, bool isSelected)
    {
      var source = isSelected ? Selection.Symbol : this;

      var minX = (int)pane.Chart.Rect.Left;
      var maxX = (int)pane.Chart.Rect.Right;
      var minY = (int)pane.Chart.Rect.Top;
      var maxY = (int)pane.Chart.Rect.Bottom;

      var points = curve.Points;

      if (points == null || !IsVisible || (!Border.IsVisible && !Fill.IsVisible)) return;

      var sModeSave = g.SmoothingMode;
      if (IsAntiAlias)
        g.SmoothingMode = SmoothingMode.HighQuality;

      // For the sake of speed, go ahead and create a solid brush and a pen
      // If it's a gradient fill, it will be created on the fly for each symbol
      //SolidBrush  brush = new SolidBrush( this.fill.Color );

      using (var pen = source.Border.GetPen(pane, scaleFactor))
      using (var path = MakePath(g, scaleFactor))
      {
        var rect = path.GetBounds();

        using (var brush = source.Fill.MakeBrush(rect))
        {
          var valueHandler = new ValueHandler(pane, false);
          var xScale = curve.GetXAxis(pane).Scale;
          var yScale = curve.GetYAxis(pane).Scale;

          var xIsLog = xScale.IsLog;
          var yIsLog = yScale.IsLog;
          var xIsOrdinal = xScale.IsAnyOrdinal;

          var xMin = xScale.Min;
          var xMax = xScale.Max;

          // (Dale-a-b) we'll set an element to true when it has been drawn  
          var isPixelDrawn = new bool[maxX + 1, maxY + 1];

          // Loop over each defined point              
          for (var i = 0; i < points.Count; i++)
          {
            // Get the user scale values for the current point
            // use the valueHandler only for stacked types
            double curX;
            double curY;
            if (pane.LineType == LineType.Stack)
            {
              double lowVal;
              valueHandler.GetValues(curve, i, out curX, out lowVal, out curY);
            }

            // otherwise, just access the values directly.  Avoiding the valueHandler for
            // non-stacked types is an optimization to minimize overhead in case there are
            // a large number of points.
            else
            {
              curX = points[i].X;
              curY = curve is StickItem ? points[i].Z : points[i].Y;
            }

            // Any value set to double max is invalid and should be skipped
            // This is used for calculated values that are out of range, divide
            //   by zero, etc.
            // Also, any value <= zero on a log scale is invalid

            if (curX == PointPair.Missing || curY == PointPair.Missing ||
                double.IsNaN(curX) || double.IsNaN(curY) || double.IsInfinity(curX) ||
                double.IsInfinity(curY) || (!(curX > 0) && xIsLog) ||
                (yIsLog && !(curY > 0.0)) ||
                (!xIsOrdinal && (!(curX >= xMin) || !(curX <= xMax))))
              continue;

            // Transform the user scale values to pixel locations
            var tmpX = (int)xScale.Transform(curve.IsOverrideOrdinal, i, curX);
            var tmpY = (int)yScale.Transform(curve.IsOverrideOrdinal, i, curY);

            // Maintain an array of "used" pixel locations to avoid duplicate drawing operations
            if (tmpX >= minX && tmpX <= maxX && tmpY >= minY && tmpY <= maxY) // guard against the zoom-in case
            {
              if (isPixelDrawn[tmpX, tmpY])
                continue;
              isPixelDrawn[tmpX, tmpY] = true;
            }

            // If the fill type for this symbol is a Gradient by value type,
            // the make a brush corresponding to the appropriate current value
            if (Fill.IsGradientValueType || Border.GradientFill.IsGradientValueType)
            {
              using (var tBrush = Fill.MakeBrush(rect, points[i]))
              using (var tPen   = Border.GetPen(pane, scaleFactor, points[i]))
                drawSymbol(g, tmpX, tmpY, path, tPen, tBrush, Type, Fill, Border);
            }
            else
            {
              // Otherwise, the brush is already defined
              // Draw the symbol at the specified pixel location
              drawSymbol(g, tmpX, tmpY, path, pen, brush, Type, Fill, Border);
            }
          }
        }
      }

      g.SmoothingMode = sModeSave;
    }

    #endregion
  }
}