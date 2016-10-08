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
using System.Runtime.Serialization;
using System.Security.Permissions;

#if ( !DOTNET1 )  // Is this a .Net 2 compilation?
using System.Collections.Generic;
#endif

namespace ZedGraph
{
  
  /// <summary>
  /// This class contains the data and methods for an individual curve within
  /// a graph pane.  It carries the settings for the curve including the
  /// key and item names, colors, symbols and sizes, linetypes, etc.
  /// </summary>
  /// 
  /// <author> John Champion
  /// modified by Jerry Vos </author>
  /// <version> $Revision: 3.43 $ $Date: 2007-11-03 04:41:28 $ </version>
  [Serializable]
  public abstract class CurveItem : ISerializable, ICloneable
  {
    /// <summary>
    /// Event called before drawing a point's object
    /// </summary>
    public event Action<CurveItem, LineBase, int> BeforeDrawEvent;

  #region Fields

    [CLSCompliant(false)]
    protected Dictionary<double, PointPair> _ordinalIndex;
     
    /// <summary>
    /// A tag object for use by the user.  This can be used to store additional
    /// information associated with the <see cref="CurveItem"/>.  ZedGraph does
    /// not use this value for any purpose.
    /// </summary>
    /// <remarks>
    /// Note that, if you are going to Serialize ZedGraph data, then any type
    /// that you store in <see cref="Tag"/> must be a serializable type (or
    /// it will cause an exception).
    /// </remarks>
    public object Tag;

    /// <summary>
    /// Private field that saves index of Y Axis that this <see cref="CurveItem"/> belongs.
    /// </summary>
    private int _yAxisIndex;

    /// <summary>
    /// Current schema value that defines the version of the serialized file
    /// </summary>
    private const int _schema = 12;

    /// <summary>
    /// Creation time using for zOrder sorting
    /// </summary>
    private long _created;

    #endregion

    #region Constructors
    /// <summary>
    /// <see cref="CurveItem"/> constructor the pre-specifies the curve label, the
    /// x and y data values as a <see cref="IPointList"/>, the curve
    /// type (Bar or Line/Symbol), the <see cref="Color"/>, and the
    /// <see cref="SymbolType"/>. Other properties of the curve are
    /// defaulted to the values in the <see cref="GraphPane.Default"/> class.
    /// </summary>
    /// <param name="label">A string label (legend entry) for this curve</param>
    /// <param name="x">An array of double precision values that define
    /// the independent (X axis) values for this curve</param>
    /// <param name="y">An array of double precision values that define
    /// the dependent (Y axis) values for this curve</param>
    protected CurveItem( string label, double[] x, double[] y, int zOrder=-1 ) :
        this( label, new PointPairList( x, y ), zOrder )
    {}

    /// <summary>
    /// <see cref="CurveItem"/> constructor the pre-specifies the curve label, the
    /// x and y data values as a <see cref="IPointList"/>, the curve
    /// type (Bar or Line/Symbol), the <see cref="Color"/>, and the
    /// <see cref="SymbolType"/>. Other properties of the curve are
    /// defaulted to the values in the <see cref="GraphPane.Default"/> class.
    /// </summary>
    /// <param name="label">A string label (legend entry) for this curve</param>
    /// <param name="points">A <see cref="IPointList"/> of double precision value pairs that define
    /// the X and Y values for this curve</param>
    protected CurveItem( string label, IPointList points, int zOrder=-1 )
    {
      Label             = new Label(label, null);
      IsVisible         = true;
      IsOverrideOrdinal = false;
      Tag               = null;
      IsX2Axis          = false;
      IsY2Axis          = false;
      _yAxisIndex       = 0;
      Link              = new Link();
      ZOrder            = zOrder;
      Points            = points ?? new PointPairList();
      _created          = DateTime.Now.Ticks;
    }
      
    /// <summary>
    /// <see cref="CurveItem"/> constructor that specifies the label of the CurveItem.
    /// This is the same as <c>CurveItem(label, null, null)</c>.
    /// <seealso cref="CurveItem( string, double[], double[] )"/>
    /// </summary>
    /// <param name="label">A string label (legend entry) for this curve</param>
    protected CurveItem( string label, int zOrder=-1 ): this( label, null, zOrder )
    {}

    /// <summary>
    /// Default ctor
    /// </summary>
    protected CurveItem() : this(null) {}

    /// <summary>
    /// The Copy Constructor
    /// </summary>
    /// <param name="rhs">The CurveItem object from which to copy</param>
    protected CurveItem( CurveItem rhs )
    {
      Label             = rhs.Label.Clone();
      IsVisible         = rhs.IsVisible;
      IsOverrideOrdinal = rhs.IsOverrideOrdinal;
      IsX2Axis          = rhs.IsX2Axis;
      IsY2Axis          = rhs.IsY2Axis;
      _yAxisIndex       = rhs._yAxisIndex;
      Tag               = (rhs.Tag is ICloneable) ? ((ICloneable)rhs.Tag).Clone() : rhs.Tag;
      Points            = (IPointList)rhs.Points.Clone();
      Link              = rhs.Link.Clone();
      ZOrder            = rhs.ZOrder;
      _created          = rhs._created;
    }

    /// <summary>
    /// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
    /// calling the typed version of Clone.
    /// </summary>
    /// <remarks>
    /// Note that this method must be called with an explicit cast to ICloneable, and
    /// that it is inherently virtual.  For example:
    /// <code>
    /// ParentClass foo = new ChildClass();
    /// ChildClass bar = (ChildClass) ((ICloneable)foo).Clone();
    /// </code>
    /// Assume that ChildClass is inherited from ParentClass.  Even though foo is declared with
    /// ParentClass, it is actually an instance of ChildClass.  Calling the ICloneable implementation
    /// of Clone() on foo actually calls ChildClass.Clone() as if it were a virtual function.
    /// </remarks>
    /// <returns>A deep copy of this object</returns>
    object ICloneable.Clone()
    {
      throw new NotImplementedException( "Can't clone an abstract base type -- child types must implement ICloneable" );
      //return new PaneBase( this );
    }

  #endregion

  #region Serialization
    /// <summary>
    /// Constructor for deserializing objects
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
    /// </param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
    /// </param>
    protected CurveItem( SerializationInfo info, StreamingContext context )
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      var sch           = info.GetInt32( "schema" );

      Label             = (Label) info.GetValue( "label", typeof(Label) );

      if ( sch >= 11 )
      { 
        IsX2Axis        = info.GetBoolean( "isX2Axis" );
        IsY2Axis        = info.GetBoolean( "isY2Axis" );
      }
      IsVisible         = info.GetBoolean( "isVisible" );
      IsOverrideOrdinal = info.GetBoolean( "isOverrideOrdinal" );

      // Data Points are always stored as a PointPairList, regardless of the
      // actual original type (which could be anything that supports IPointList).
      Points            = (PointPairList) info.GetValue( "points", typeof(PointPairList) );
      Tag               = info.GetValue( "Tag", typeof(object) );
      _yAxisIndex       = Math.Max(0, info.GetInt32( "yAxisIndex" ));
      Link              = (Link) info.GetValue( "link", typeof(Link) );
      ZOrder            = info.GetInt32("zOrder");
      _created          = info.GetInt64("created");
    }
    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
    [SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
    public virtual void GetObjectData( SerializationInfo info, StreamingContext context )
    {
      info.AddValue("schema",            _schema);
      info.AddValue("label",             Label);
      info.AddValue("isX2Axis",          IsX2Axis);
      info.AddValue("isY2Axis",          IsY2Axis);
      info.AddValue("isVisible",         IsVisible);
      info.AddValue("isOverrideOrdinal", IsOverrideOrdinal);

      // if points is already a PointPairList, use it
      // otherwise, create a new PointPairList so it can be serialized
      var list = Points is PointPairList ? (PointPairList)Points : new PointPairList(Points);

      info.AddValue("points",           list);
      info.AddValue("Tag",              Tag);
      info.AddValue("yAxisIndex",       YAxisIndex);
      info.AddValue("link",             Link);
      info.AddValue("zOrder",           ZOrder);
      info.AddValue("created",          _created);
    }
    #endregion

    #region Properties
    /// <summary>
    /// A <see cref="ZedGraph.Label" /> instance that represents the <see cref="ZedGraph.Legend"/>
    /// entry for the this <see cref="CurveItem"/> object
    /// </summary>
    public Label Label { get; }

    /// <summary>
    /// The <see cref="Line"/>/<see cref="Symbol"/>/<see cref="Bar"/> 
    /// color (FillColor for the Bar).  This is a common access to
    /// <see cref="LineBase.Color">Line.Color</see>,
    /// <see cref="LineBase.Color">Border.Color</see>, and
    /// <see cref="Fill.Color">Fill.Color</see> properties for this curve.
    /// </summary>
    public Color Color
    {
      get
      {
        if (this is BarItem)
          return ((BarItem)this).Bar.Fill.Color;
        if (this is LineItem && ((LineItem)this).Line.IsVisible)
          return ((LineItem)this).Line.Color;
        if (this is LineItem)
          return ((LineItem)this).Symbol.Border.Color;
        if (this is ErrorBarItem)
          return ((ErrorBarItem)this).Bar.Color;
        if (this is HiLowBarItem)
          return ((HiLowBarItem)this).Bar.Fill.Color;
        return Color.Empty;
      }
      set 
      {
        if ( this is BarItem )
        {
          ((BarItem) this).Bar.Fill.Color        = value;
        }
        else if ( this is LineItem )
        {
          ((LineItem) this).Line.Color           = value;
          ((LineItem) this).Symbol.Border.Color  = value;
          ((LineItem) this).Symbol.Fill.Color    = value;
        }
        else if ( this is ErrorBarItem )
          ((ErrorBarItem) this).Bar.Color        = value;
        else if ( this is HiLowBarItem )
          ((HiLowBarItem) this).Bar.Fill.Color   = value;
      }
    }

    /// <summary>
    /// Determines whether this <see cref="CurveItem"/> is visible on the graph.
    /// Note that this value turns the curve display on or off, but it does not
    /// affect the display of the legend entry.  To hide the legend entry, you
    /// have to set <see cref="ZedGraph.Label.IsVisible"/> to false.
    /// </summary>
    public bool IsVisible { get; set; }

    // Revision: JCarpenter 10/06
    /// <summary>
    /// Determines whether this <see cref="CurveItem"/> is selected on the graph.
    /// Note that this value changes the curve displayed color, but it does not
    /// affect the display of the legend entry. To hide the legend entry, you
    /// have to set <see cref="ZedGraph.Label.IsVisible"/> to false.
    /// </summary>
    public bool IsSelected { get; set; }

    // Revision: JCarpenter 10/06
    /// <summary>
    /// Determines whether this <see cref="CurveItem"/> can be selected in the graph.
    /// </summary>
    public bool IsSelectable { get; set; }

    /// <summary>
    /// Gets or sets a value which allows you to override the normal
    /// ordinal axis behavior.
    /// </summary>
    /// <remarks>
    /// Normally for an ordinal axis type, the actual data values corresponding to the ordinal
    /// axis will be ignored (essentially they are replaced by ordinal values, e.g., 1, 2, 3, etc).
    /// If IsOverrideOrdinal is true, then the user data values will be used (even if they don't
    /// make sense).  Fractional values are allowed, such that a value of 1.5 is between the first and
    /// second ordinal position, etc.
    /// </remarks>
    /// <seealso cref="AxisType.Ordinal"/>
    /// <seealso cref="AxisType.Text"/>
    public bool IsOverrideOrdinal { get; set; }

    /// <summary>
    /// Gets or sets a value that determines which X axis this <see cref="CurveItem"/>
    /// is assigned to.
    /// </summary>
    /// <remarks>
    /// The
    /// <see cref="ZedGraph.XAxis"/> is on the bottom side of the graph and the
    /// <see cref="ZedGraph.X2Axis"/> is on the top side.  Assignment to an axis
    /// determines the scale that is used to draw the curve on the graph.
    /// </remarks>
    /// <value>true to assign the curve to the <see cref="ZedGraph.X2Axis"/>,
    /// false to assign the curve to the <see cref="ZedGraph.XAxis"/></value>
    public bool IsX2Axis { get; set; }

    /// <summary>
    /// Gets or sets a value that determines which Y axis this <see cref="CurveItem"/>
    /// is assigned to.
    /// </summary>
    /// <remarks>
    /// The
    /// <see cref="ZedGraph.YAxis"/> is on the left side of the graph and the
    /// <see cref="ZedGraph.Y2Axis"/> is on the right side.  Assignment to an axis
    /// determines the scale that is used to draw the curve on the graph.  Note that
    /// this value is used in combination with the <see cref="YAxisIndex" /> to determine
    /// which of the Y Axes (if there are multiples) this curve belongs to.
    /// </remarks>
    /// <value>true to assign the curve to the <see cref="ZedGraph.Y2Axis"/>,
    /// false to assign the curve to the <see cref="ZedGraph.YAxis"/></value>
    public bool IsY2Axis { get; set; }

    /// <summary>
    /// Gets or sets the index number of the Y Axis to which this
    /// <see cref="CurveItem" /> belongs.
    /// </summary>
    /// <remarks>
    /// This value is essentially an index number into the <see cref="GraphPane.YAxisList" />
    /// or <see cref="GraphPane.Y2AxisList" />, depending on the setting of
    /// <see cref="IsY2Axis" />.
    /// </remarks>
    public int YAxisIndex
    {
      get { return _yAxisIndex; }
      set
      {
        if (value < 0) throw new ArgumentException($"Invalid YAxisIndex: {value}");
        _yAxisIndex = value;
      }
    }

    /// <summary>
    /// Determines whether this <see cref="CurveItem"/>
    /// is a <see cref="BarItem"/>.
    /// </summary>
    /// <value>true for a bar chart, or false for a line or pie graph</value>
    public bool IsBar => this is BarItem || this is HiLowBarItem || this is ErrorBarItem;

    /// <summary>
    /// Determines whether this <see cref="CurveItem"/>
    /// is a <see cref="PieItem"/>.
    /// </summary>
    /// <value>true for a pie chart, or false for a line or bar graph</value>
    public bool IsPie => this is PieItem;

    /// <summary>
    /// Determines whether this <see cref="CurveItem"/>
    /// is a <see cref="LineItem"/>.
    /// </summary>
    /// <value>true for a line chart, or false for a bar type</value>
    public bool IsLine => this is LineItem;

    /// <summary>
    /// Gets a flag indicating if the Z data range should be included in the axis scaling calculations.
    /// </summary>
    /// <param name="pane">The parent <see cref="GraphPane" /> of this <see cref="CurveItem" />.
    /// </param>
    /// <value>true if the Z data are included, false otherwise</value>
    internal abstract bool IsZIncluded( GraphPane pane );
    
    /// <summary>
    /// Gets a flag indicating if the X axis is the independent axis for this <see cref="CurveItem" />
    /// </summary>
    /// <param name="pane">The parent <see cref="GraphPane" /> of this <see cref="CurveItem" />.
    /// </param>
    /// <value>true if the X axis is independent, false otherwise</value>
    internal abstract bool IsXIndependent( GraphPane pane );
    
    /// <summary>
    /// Readonly property that gives the number of points that define this
    /// <see cref="CurveItem"/> object, which is the number of points in the
    /// <see cref="Points"/> data collection.
    /// </summary>
    public int NPts => Points?.Count ?? 0;

    /// <summary>
    /// The <see cref="IPointList"/> of X,Y point sets that represent this
    /// <see cref="CurveItem"/>.
    /// </summary>
    public IPointList Points { get; private set; }

    /// <summary>
    /// An accessor for the <see cref="PointPair"/> datum for this <see cref="CurveItem"/>.
    /// Index is the ordinal reference (zero based) of the point.
    /// </summary>
    public IPointPair this[int index] =>
      Points == null ? new PointPair( PointPair.Missing, PointPair.Missing ) : Points[index];

    /// <summary>
    /// Gets or sets the hyperlink information for this <see cref="CurveItem" />.
    /// </summary>
    // /// <seealso cref="ZedGraph.Web.IsImageMap" />
    public Link Link { get; set; }

    /// <summary>
    /// Drawing order of this curve (the higher the more upfront precedence is given in drawing)
    /// </summary>
    public int ZOrder { get; set; }

  #endregion
  
  #region Rendering Methods

    public override string ToString()
    {
      return Label.Text;
    }

    /// <summary>
    /// Do all rendering associated with this <see cref="CurveItem"/> to the specified
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
    /// <param name="pos">The ordinal position of the current <see cref="Bar"/>
    /// curve.</param>
    /// <param name="scaleFactor">
    /// The scaling factor to be used for rendering objects.  This is calculated and
    /// passed down by the parent <see cref="ZedGraph.GraphPane"/> object using the
    /// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
    /// font sizes, etc. according to the actual size of the graph.
    /// </param>
    public abstract void Draw( Graphics g, GraphPane pane, int pos, float scaleFactor );
    
    /// <summary>
    /// Draw a legend key entry for this <see cref="CurveItem"/> at the specified location.
    /// This abstract base method passes through to <see cref="BarItem.DrawLegendKey"/> or
    /// <see cref="LineItem.DrawLegendKey"/> to do the rendering.
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
    public abstract void DrawLegendKey( Graphics g, GraphPane pane, RectangleF rect, float scaleFactor );
    
  #endregion

  #region Utility Methods

    /// <summary>
    /// Add a single x,y coordinate point to the end of the points collection for this curve.
    /// </summary>
    /// <param name="x">The X coordinate value</param>
    /// <param name="y">The Y coordinate value</param>
    public void AddPoint( double x, double y )
    {
      this.AddPoint( new PointPair( x, y ) );
    }

    /// <summary>
    /// Add a <see cref="PointPair"/> object to the end of the points collection for this curve.
    /// </summary>
    /// <remarks>
    /// This method will only work if the <see cref="IPointList" /> instance reference
    /// at <see cref="Points" /> supports the <see cref="IPointListEdit" /> interface.
    /// Otherwise, it does nothing.
    /// </remarks>
    /// <param name="point">A reference to the <see cref="PointPair"/> object to
    /// be added</param>
    public void AddPoint( PointPair point )
    {
      if ( Points == null )
        Points = new PointPairList();

      if (!(Points is IPointListEdit))
        throw new NotImplementedException();
      ((IPointListEdit)Points).Add(point);
    }

    /// <summary>
    /// Clears the points from this <see cref="CurveItem"/>.  This is the same
    /// as <c>CurveItem.Points.Clear()</c>.
    /// </summary>
    /// <remarks>
    /// This method will only work if the <see cref="IPointList" /> instance reference
    /// at <see cref="Points" /> supports the <see cref="IPointListEdit" /> interface.
    /// Otherwise, it does nothing.
    /// </remarks>
    public void Clear()
    {
      if (!(Points is IPointListEdit))
        throw new NotImplementedException();
      ((IPointListEdit)Points).Clear();
    }

    /// <summary>
    /// Removes a single point from this <see cref="CurveItem" />.
    /// </summary>
    /// <remarks>
    /// This method will only work if the <see cref="IPointList" /> instance reference
    /// at <see cref="Points" /> supports the <see cref="IPointListEdit" /> interface.
    /// Otherwise, it does nothing.
    /// </remarks>
    /// <param name="index">The ordinal position of the point to be removed.</param>
    public void RemovePoint( int index )
    {
      if (!(Points is IPointListEdit))
        throw new NotImplementedException();
      ((IPointListEdit)Points).RemoveAt(index);
    }

    /// <summary>
    /// Get the X Axis instance (either <see cref="XAxis" /> or <see cref="X2Axis" />) to
    /// which this <see cref="CurveItem" /> belongs.
    /// </summary>
    /// <param name="pane">The <see cref="GraphPane" /> object to which this curve belongs.</param>
    /// <returns>Either a <see cref="XAxis" /> or <see cref="X2Axis" /> to which this
    /// <see cref="CurveItem" /> belongs.
    /// </returns>
    public Axis GetXAxis( GraphPane pane )
    {
      return IsX2Axis ? (Axis)pane.X2Axis : pane.XAxis;
    }

    /// <summary>
    /// Get the Y Axis instance (either <see cref="YAxis" /> or <see cref="Y2Axis" />) to
    /// which this <see cref="CurveItem" /> belongs.
    /// </summary>
    /// <remarks>
    /// This method safely retrieves a Y Axis instance from either the <see cref="GraphPane.YAxisList" />
    /// or the <see cref="GraphPane.Y2AxisList" /> using the values of <see cref="YAxisIndex" /> and
    /// <see cref="IsY2Axis" />.  If the value of <see cref="YAxisIndex" /> is out of bounds, the
    /// default <see cref="YAxis" /> or <see cref="Y2Axis" /> is used.
    /// </remarks>
    /// <param name="pane">The <see cref="GraphPane" /> object to which this curve belongs.</param>
    /// <returns>Either a <see cref="YAxis" /> or <see cref="Y2Axis" /> to which this
    /// <see cref="CurveItem" /> belongs.
    /// </returns>
    public Axis GetYAxis( GraphPane pane )
    {
      return IsY2Axis ? (Axis)pane.Y2AxisList[_yAxisIndex < pane.Y2AxisList.Count ? _yAxisIndex : 0]
                      :       pane.YAxisList [_yAxisIndex < pane.YAxisList.Count  ? _yAxisIndex : 0];
    }

    /// <summary>
    /// Get the index of the Y Axis in the <see cref="YAxis" /> or <see cref="Y2Axis" /> list to
    /// which this <see cref="CurveItem" /> belongs.
    /// </summary>
    /// <remarks>
    /// This method safely retrieves a Y Axis index into either the <see cref="GraphPane.YAxisList" />
    /// or the <see cref="GraphPane.Y2AxisList" /> using the values of <see cref="YAxisIndex" /> and
    /// <see cref="IsY2Axis" />.  If the value of <see cref="YAxisIndex" /> is out of bounds, the
    /// default <see cref="YAxis" /> or <see cref="Y2Axis" /> is used, which is index zero.
    /// </remarks>
    /// <param name="pane">The <see cref="GraphPane" /> object to which this curve belongs.</param>
    /// <returns>An integer value indicating which index position in the list applies to this
    /// <see cref="CurveItem" />
    /// </returns>
    public int GetYAxisIndex( GraphPane pane )
    {
      var count = IsY2Axis ? pane.Y2AxisList.Count : pane.YAxisList.Count;
      return _yAxisIndex < count ? _yAxisIndex : 0;
    }

    /// <summary>
    /// Loads some pseudo unique colors/symbols into this CurveItem.  This
    /// is the same as <c>MakeUnique(ColorSymbolRotator.StaticInstance)</c>.
    /// <seealso cref="ColorSymbolRotator.StaticInstance"/>
    /// <seealso cref="ColorSymbolRotator"/>
    /// <seealso cref="MakeUnique(ColorSymbolRotator)"/>
    /// </summary>
    public void MakeUnique()
    {
      MakeUnique( ColorSymbolRotator.StaticInstance );
    }

    /// <summary>
    /// Loads some pseudo unique colors/symbols into this CurveItem.  This
    /// is mainly useful for differentiating a set of new CurveItems without
    /// having to pick your own colors/symbols.
    /// <seealso cref="MakeUnique(ColorSymbolRotator)"/>
    /// </summary>
    /// <param name="rotator">
    /// The <see cref="ColorSymbolRotator"/> that is used to pick the color
    ///  and symbol for this method call.
    /// </param>
    public virtual void MakeUnique( ColorSymbolRotator rotator )
    {
      Color = rotator.NextColor;
    }
  
    /// <summary>
    /// Go through the list of <see cref="PointPair"/> data values for this <see cref="CurveItem"/>
    /// and determine the minimum and maximum values in the data.
    /// </summary>
    /// <param name="xMin">The minimum X value in the range of data</param>
    /// <param name="xMax">The maximum X value in the range of data</param>
    /// <param name="yMin">The minimum Y value in the range of data</param>
    /// <param name="yMax">The maximum Y value in the range of data</param>
    /// <param name="ignoreInitial">ignoreInitial is a boolean value that
    /// affects the data range that is considered for the automatic scale
    /// ranging (see <see cref="GraphPane.IsIgnoreInitial"/>).  If true, then initial
    /// data points where the Y value is zero are not included when
    /// automatically determining the scale <see cref="Scale.Min"/>,
    /// <see cref="Scale.Max"/>, and <see cref="Scale.MajorStep"/> size.  All data after
    /// the first non-zero Y value are included.
    /// </param>
    /// <param name="isBoundedRanges">
    /// Determines if the auto-scaled axis ranges will subset the
    /// data points based on any manually set scale range values.
    /// </param>
    /// <param name="pane">
    /// A reference to the <see cref="GraphPane"/> object that is the parent or
    /// owner of this object.
    /// </param>
    /// <seealso cref="GraphPane.IsBoundedRanges"/>
    public virtual void GetRange(out double xMin, out double xMax,
                    out double yMin, out double yMax,
                    bool ignoreInitial,
                    bool isBoundedRanges,
                    GraphPane pane )
    {
      // The lower and upper bounds of allowable data for the X values.  These
      // values allow you to subset the data values.  If the X range is bounded, then
      // the resulting range for Y will reflect the Y values for the points within the X
      // bounds.
      var xLBound = double.MinValue;
      var xUBound = double.MaxValue;
      var yLBound = double.MinValue;
      var yUBound = double.MaxValue;

      // initialize the values to outrageous ones to start
      var xMinOrd = int.MaxValue;
      var xMaxOrd = int.MinValue;

      xMin = yMin = double.MaxValue;
      xMax = yMax = double.MinValue;

      var yAxis  =  GetYAxis( pane );
      var xAxis  =  GetXAxis( pane );
      if ( yAxis == null || xAxis == null )
        return;

      if ( isBoundedRanges )
      {
        xLBound = xAxis.Scale._lBound;
        xUBound = xAxis.Scale._uBound;
        yLBound = yAxis.Scale._lBound;
        yUBound = yAxis.Scale._uBound;
      }


      var isZIncluded    = IsZIncluded( pane );
      var isXIndependent = IsXIndependent( pane );
      var isXLog         = xAxis.Scale.IsLog;
      var isYLog         = yAxis.Scale.IsLog;
      var isXOrdinal     = xAxis.Scale.IsAnyOrdinal;
      var isYOrdinal     = yAxis.Scale.IsAnyOrdinal;
      var isZOrdinal     = (isXIndependent ? yAxis : xAxis).Scale.IsAnyOrdinal;

      int from, to;

      /*
      if (false) //isXOrdinal)
      {
        //from = xAxis.Scale.MinAuto ? 0            : Scale.MinMax(0, (int)xAxis.Scale.Min, Points.Count-1);
        //to   = xAxis.Scale.MaxAuto ? Points.Count : Scale.MinMax(0, (int)xAxis.Scale.Max+1, Points.Count);
        from = Scale.MinMax((int)Math.Round(xMin), (int)Math.Round(xAxis.Scale.Min), Points.Count-1);
        to   = Scale.MinMax((int)Math.Round(xMax), (int)Math.Round(xAxis.Scale.Max)+1, Points.Count);
      }
      else
      */
      {
        from = 0;
        to   = Points.Count;
      }

      // Loop over each point in the arrays
      //foreach ( PointPair point in this.Points )
      for ( var i=from; i < to; i++ )
      {
        var point = Points[i];

        //double curX = isXOrdinal ? i + 1 : point.X;
        // FIXME:
        var curOrd = i + 1;
        var curX   = isXOrdinal && !IsOverrideOrdinal ? curOrd : point.X;
        var curY   = isYOrdinal ? i + 1 : point.Y;
        var loZ    = isZOrdinal ? i + 1 : Math.Min(point.LowValue, point.HighValue); //point.Z;
        var hiZ    = isZOrdinal ? i + 1 : Math.Max(point.LowValue, point.HighValue);//point.Z;

        var outOfBounds = curX < xLBound || curX > xUBound ||
                          curY < yLBound || curY > yUBound ||
                          (isZIncluded && ((isXIndependent  && (loZ < yLBound || hiZ > yUBound)) ||
                                           (!isXIndependent && (loZ < xLBound || hiZ > xUBound)))) ||
                          (curX <= 0 && isXLog) || (curY <= 0 && isYLog);

        // ignoreInitial becomes false at the first non-zero
        // Y value
        if (  ignoreInitial && curY != 0 &&
            curY != PointPair.Missing )
          ignoreInitial = false;

        if (ignoreInitial || outOfBounds || curX == PointPair.Missing || curY == PointPair.Missing)
          continue;

        //if (curX < xMin) xMin = curX;
        //if (curX > xMax) xMax = curX;
        //FIXME:
        if (curX < xMin) { xMin = curX; xMinOrd = curOrd; }
        if (curX > xMax) { xMax = curX; xMaxOrd = curOrd; }
        if (curY < yMin) yMin = curY;
        if (curY > yMax) yMax = curY;

        if (!isZIncluded) continue;

        if (isXIndependent)
        {
          if (loZ == PointPair.Missing && hiZ == PointPair.Missing) continue;

          if (loZ < yMin) yMin = loZ;
          if (hiZ > yMax) yMax = hiZ;
        }
        else
        {
          if (loZ == PointPair.Missing && hiZ == PointPair.Missing) continue;

          if (loZ < xMin) xMin = loZ;
          if (hiZ > xMax) xMax = hiZ;
        }
      }

      if (isXOrdinal && !IsOverrideOrdinal)
      {
        xMin = xMinOrd;
        xMax = xMaxOrd;
      }
    }

    /// <summary>Returns a reference to the <see cref="Axis"/> object that is the "base"
    /// (independent axis) from which the values are drawn. </summary>
    /// <remarks>
    /// This property is determined by the value of <see cref="BarSettings.Base"/> for
    /// <see cref="BarItem"/>, <see cref="ErrorBarItem"/>, and <see cref="HiLowBarItem"/>
    /// types.  It is always the X axis for regular <see cref="LineItem"/> types.
    /// Note that the <see cref="BarSettings.Base" /> setting can override the
    /// <see cref="IsY2Axis" /> and <see cref="YAxisIndex" /> settings for bar types
    /// (this is because all the bars that are clustered together must share the
    /// same base axis).
    /// </remarks>
    /// <seealso cref="BarBase"/>
    /// <seealso cref="ValueAxis"/>
    public virtual Axis BaseAxis( GraphPane pane )
    {
      BarBase barBase;

      if ( this is BarItem || this is ErrorBarItem || this is HiLowBarItem )
        barBase = pane._barSettings.Base;
      else
        barBase = IsX2Axis ? BarBase.X2 : BarBase.X;

      switch (barBase)
      {
        case BarBase.X:
          return pane.XAxis;
        case BarBase.X2:
          return pane.X2Axis;
        case BarBase.Y:
          return pane.YAxis;
        default:
          return pane.Y2Axis;
      }
    }

    /// <summary>Returns a reference to the <see cref="Axis"/> object that is the "value"
    /// (dependent axis) from which the points are drawn. </summary>
    /// <remarks>
    /// This property is determined by the value of <see cref="BarSettings.Base"/> for
    /// <see cref="BarItem"/>, <see cref="ErrorBarItem"/>, and <see cref="HiLowBarItem"/>
    /// types.  It is always the Y axis for regular <see cref="LineItem"/> types.
    /// </remarks>
    /// <seealso cref="BarBase"/>
    /// <seealso cref="BaseAxis"/>
    public virtual Axis ValueAxis( GraphPane pane )
    {
      var barBase = this is BarItem || this is ErrorBarItem || this is HiLowBarItem
        ? pane._barSettings.Base
        : BarBase.X;

      return barBase == BarBase.X || barBase == BarBase.X2
        ? GetYAxis(pane)
        : GetXAxis(pane);
    }

    /// <summary>
    /// Calculate the width of each bar, depending on the actual bar type
    /// </summary>
    /// <returns>The width for an individual bar, in pixel units</returns>
    public float GetBarWidth( GraphPane pane )
    {
      // Total axis width = 
      // npts * ( nbars * ( bar + bargap ) - bargap + clustgap )
      // cg * bar = cluster gap
      // npts = max number of points in any curve
      // nbars = total number of curves that are of type IsBar
      // bar = bar width
      // bg * bar = bar gap
      // therefore:
      // totwidth = npts * ( nbars * (bar + bg*bar) - bg*bar + cg*bar )
      // totwidth = bar * ( npts * ( nbars * ( 1 + bg ) - bg + cg ) )
      // solve for bar

      float barWidth;

      if ( this is ErrorBarItem )
        barWidth = ((ErrorBarItem)this).Bar.Symbol.Size * pane.CalcScaleFactor();
//      else if ( this is HiLowBarItem && pane._barSettings.Type != BarType.ClusterHiLow )
//        barWidth = (float) ( ((HiLowBarItem)this).Bar.GetBarWidth( pane,
//            ((HiLowBarItem)this).BaseAxis(pane), pane.CalcScaleFactor() ) );
//        barWidth = (float) ( ((HiLowBarItem)this).Bar.Size *
//            pane.CalcScaleFactor() );
      else // BarItem or LineItem
      {
        // For stacked bar types, the bar width will be based on a single bar
        var numBars = 1.0F;
        if ( pane._barSettings.Type == BarType.Cluster )
          numBars = pane.CurveList.NumClusterableBars;

        var denom = numBars * ( 1.0F + pane._barSettings.MinBarGap ) -
              pane._barSettings.MinBarGap + pane._barSettings.MinClusterGap;
        if ( denom <= 0 )
          denom = 1;
        barWidth = pane.BarSettings.GetClusterWidth() / denom;
      }

      return barWidth <= 0 ? 1 : barWidth;
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
    public abstract bool GetCoords( GraphPane pane, int i, out string coords );

    /// <summary>
    /// Comparer used for sorting curves by their ZOrder for drawing.
    /// </summary>
    /// <remarks>
    /// Curves that have ZOrder set have a precedence over those that don't.
    /// The rest of the curves are ordered in the order of time of their addition to a CurveList.
    /// </remarks>
    internal struct ZOrderComparer : IComparer<CurveItem>
    {
      public int Compare(CurveItem lhs, CurveItem rhs)
      {
        var l = lhs.ZOrder >= 0 ? lhs.ZOrder : lhs._created;
        var r = rhs.ZOrder >= 0 ? rhs.ZOrder : rhs._created;
        return l.CompareTo(r);
      }
    }

    #endregion

    #region Inner classes

    /// <summary>
    /// Compares <see cref="CurveItem"/>'s based on the point value at the specified
    /// index and for the specified axis.
    /// <seealso cref="System.Collections.ArrayList.Sort()"/>
    /// </summary>
    public class Comparer : IComparer<CurveItem>
    {
      private readonly int index;
      private readonly SortType sortType;

      /// <summary>
      ///   Constructor for Comparer.
      /// </summary>
      /// <param name="type">The axis type on which to sort.</param>
      /// <param name="index">The index number of the point on which to sort</param>
      public Comparer(SortType type, int index)
      {
        sortType = type;
        this.index = index;
      }

      /// <summary>
      ///   Compares two <see cref="CurveItem" />s using the previously specified index value
      ///   and axis.  Sorts in descending order.
      /// </summary>
      /// <param name="l">Curve to the left.</param>
      /// <param name="r">Curve to the right.</param>
      /// <returns>-1, 0, or 1 depending on l.X's relation to r.X</returns>
      public int Compare(CurveItem l, CurveItem r)
      {
        if (l == null && r == null)
          return 0;
        if (l == null)
          return -1;
        if (r == null)
          return 1;

        if (r.NPts <= index)
          r = null;
        if (l.NPts <= index)
          l = null;

        double lVal, rVal;

        if (sortType == SortType.XValues)
        {
          lVal = l != null ? Math.Abs(l[index].X) : PointPairBase.Missing;
          rVal = r != null ? Math.Abs(r[index].X) : PointPairBase.Missing;
        }
        else
        {
          lVal = l != null ? Math.Abs(l[index].Y) : PointPairBase.Missing;
          rVal = r != null ? Math.Abs(r[index].Y) : PointPairBase.Missing;
        }

        if ((lVal == PointPairBase.Missing) || double.IsInfinity(lVal) ||
            double.IsNaN(lVal))
          l = null;
        if ((rVal == PointPairBase.Missing) || double.IsInfinity(rVal) ||
            double.IsNaN(rVal))
          r = null;

        if ((l == null && r == null) || (Math.Abs(lVal - rVal) < 1e-10))
          return 0;
        if (l == null)
          return -1;
        if (r == null)
          return 1;
        return rVal < lVal ? -1 : 1;
      }
    }

    internal virtual void OnBeforeDrawEvent(LineBase obj, int i)
    {
      BeforeDrawEvent?.Invoke(this, obj, i);
    }

    #endregion
  }
}



