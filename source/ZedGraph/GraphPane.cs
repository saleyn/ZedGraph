//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright ?2004  John Champion
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
  // <summary>
  // <c>ZedGraph</c> is a class library and UserControl (<see cref="ZedGraphControl"/>) that display
  // 2D line graphs of user specified data.  The <c>ZedGraph</c> namespace includes all functionality
  // required to draw, modify, and update the graph.
  // </summary>

  /// <summary>
  ///   Class <see cref="GraphPane" /> encapsulates the graph pane, which is all display elements
  ///   associated with an individual graph.
  /// </summary>
  /// <remarks>
  ///   This class is the outside "wrapper"
  ///   for the ZedGraph classes, and provides the interface to access the attributes
  ///   of the graph.  You can have multiple graphs in the same document or form,
  ///   just instantiate multiple GraphPane's.
  /// </remarks>
  /// <author> John Champion modified by Jerry Vos </author>
  /// <version> $Revision: 3.81 $ $Date: 2007-09-30 07:44:11 $ </version>
  [Serializable]
  public class GraphPane : PaneBase, ICloneable, ISerializable
  {
    #region Events

    /// <summary>
    ///   A delegate to provide notification through the <see cref="AxisChangeEvent" />
    ///   when <see cref="AxisChange()" /> is called.
    /// </summary>
    /// <param name="pane">
    ///   The <see cref="GraphPane" /> for which AxisChange() has
    ///   been called.
    /// </param>
    /// <seealso cref="AxisChangeEvent" />
    public delegate void AxisChangeEventHandler(GraphPane pane);

    /// <summary>
    ///   A delegate that allows notification after a 'Draw' operation has been completed by a pane.
    /// </summary>
    /// <param name="pane">The <see cref="GraphPane" /> object that performed the 'Draw'.</param>
    public delegate void DrawEventHandler(GraphPane pane);

    /// <summary>
    ///   Subscribe to this event to be notified when <see cref="AxisChange()" /> is called.
    /// </summary>
    public event AxisChangeEventHandler AxisChangeEvent;

    /// <summary>
    ///   Subscribe to this event to be notified when 'Draw' operation has been completed by a pane.
    /// </summary>
    [Bindable(true)]
    [Category("Events")]
    [Description(
       "Subscribe to this event to be notified when 'Draw' operation has been completed by a pane."
     )]
    public event DrawEventHandler DrawEvent;

    #endregion

    #region Private Fields

    // Item subclasses ////////////////////////////////////////////////////////////////////

    internal BarSettings _barSettings;

    internal Chart _chart;

    #endregion

    #region Defaults

    /// <summary>
    ///   A simple struct that defines the
    ///   default property values for the <see cref="GraphPane" /> class.
    /// </summary>
    public new struct Default
    {
      /// <summary>
      ///   The default width of a bar cluster
      ///   on a <see cref="Bar" /> graph.  This value only applies to
      ///   <see cref="Bar" /> graphs, and only when the
      ///   <see cref="Axis.Type" /> is <see cref="AxisType.Linear" />,
      ///   <see cref="AxisType.Log" /> or <see cref="AxisType.Date" />.
      ///   This dimension is expressed in terms of X scale user units.
      /// </summary>
      /// <seealso cref="ZedGraph.BarSettings.Default.MinClusterGap" />
      /// <seealso cref="ZedGraph.BarSettings.MinBarGap" />
      public static double ClusterScaleWidth = 1.0;

      /// <summary>
      ///   The default settings for the <see cref="Axis" /> scale bounded ranges option
      ///   (<see cref="GraphPane.IsBoundedRanges" /> property).
      ///   true to have the auto-scale-range code subset the data according to any
      ///   manually set scale values, false otherwise.
      /// </summary>
      public static bool IsBoundedRanges = false;

      /// <summary>
      ///   The default settings for the <see cref="Axis" /> scale ignore initial
      ///   zero values option (<see cref="GraphPane.IsIgnoreInitial" /> property).
      ///   true to have the auto-scale-range code ignore the initial data points
      ///   until the first non-zero Y value, false otherwise.
      /// </summary>
      public static bool IsIgnoreInitial = false;

      /// <summary>
      ///   The default value for the <see cref="GraphPane.LineType" /> property, which
      ///   determines if the lines are drawn in normal or "stacked" mode.  See the
      ///   <see cref="ZedGraph.LineType" /> for more information.
      /// </summary>
      /// <seealso cref="GraphPane.LineType" />
      public static LineType LineType = LineType.Normal;

      /// <summary>
      ///   The tolerance that is applied to the
      ///   <see cref="GraphPane.FindNearestPoint(PointF,out CurveItem,out int)" /> routine.
      ///   If a given curve point is within this many pixels of the mousePt, the curve
      ///   point is considered to be close enough for selection as a nearest point
      ///   candidate.
      /// </summary>
      public static double NearestTol = 7.0;
    }

    #endregion

    #region Class Instance Properties

    /// <summary>
    ///   Gets the <see cref="BarSettings" /> instance for this <see cref="GraphPane" />,
    ///   which stores the global properties for bar type charts.
    /// </summary>
    public BarSettings BarSettings => _barSettings;

    /// <summary>
    ///   Gets the <see cref="Chart" /> instance for this <see cref="GraphPane" />.
    /// </summary>
    public Chart Chart => _chart;

    /// <summary>
    ///   Gets or sets the clipping region for curves for this <see cref="GraphPane" />
    /// </summary>
    /// <value>A reference to a RectangleF object</value>
    public RectangleF CurveClipRect { get; set; }

    /// <summary>
    ///   Gets or sets the list of <see cref="CurveItem" /> items for this <see cref="GraphPane" />
    /// </summary>
    /// <value>A reference to a <see cref="CurveList" /> collection object</value>
    public CurveList CurveList { get; set; }

    /// <summary>
    ///   Accesses the <see cref="X2Axis" /> for this graph
    /// </summary>
    /// <value>A reference to a <see cref="X2Axis" /> object</value>
    public X2Axis X2Axis { get; }

    /// <summary>
    ///   Accesses the <see cref="XAxis" /> for this graph
    /// </summary>
    /// <value>A reference to a <see cref="XAxis" /> object</value>
    public XAxis XAxis { get; }

    /// <summary>
    ///   Accesses the primary <see cref="Y2Axis" /> for this graph
    /// </summary>
    /// <value>A reference to a <see cref="Y2Axis" /> object</value>
    /// <seealso cref="YAxisList" />
    /// <seealso cref="Y2AxisList" />
    public Y2Axis Y2Axis => Y2AxisList[0];

    /// <summary>
    ///   Gets the collection of Y2 axes that belong to this <see cref="GraphPane" />.
    /// </summary>
    public Y2AxisList Y2AxisList { get; }

    /// <summary>
    ///   Accesses the primary <see cref="YAxis" /> for this graph
    /// </summary>
    /// <value>A reference to a <see cref="YAxis" /> object</value>
    /// <seealso cref="YAxisList" />
    /// <seealso cref="Y2AxisList" />
    public YAxis YAxis => YAxisList[0];

    /// <summary>
    ///   Gets the collection of Y axes that belong to this <see cref="GraphPane" />.
    /// </summary>
    public YAxisList YAxisList { get; }

    #endregion

    #region General Properties

    /// <summary>
    ///   Gets or sets a value that determines if ZedGraph should modify the scale ranges
    ///   for the Y and Y2 axes such that the number of major steps, and therefore the
    ///   major grid lines, line up.
    /// </summary>
    /// <remarks>
    ///   This property affects the way that <see cref="AxisChange()" /> selects the scale
    ///   ranges for the Y and Y2 axes.  It applies to the scale ranges of all Y and Y2 axes,
    ///   but only if the <see cref="Scale.MaxAuto" /> is set to true.<br />
    /// </remarks>
    public bool IsAlignGrids { get; set; }

    /// <summary>
    ///   Gets or sets a boolean value that determines if the auto-scaled axis ranges will
    ///   subset the data points based on any manually set scale range values.
    /// </summary>
    /// <remarks>
    ///   The bounds provide a means to subset the data.  For example, if all the axes are set to
    ///   autoscale, then the full range of data are used.  But, if the XAxis.Min and XAxis.Max values
    ///   are manually set, then the Y data range will reflect the Y values within the bounds of
    ///   XAxis.Min and XAxis.Max.  Set to true to subset the data, or false to always include
    ///   all data points when calculating scale ranges.
    /// </remarks>
    public bool IsBoundedRanges { get; set; }

    /// <summary>
    ///   Gets or sets a boolean value that affects the data range that is considered
    ///   for the automatic scale ranging.
    /// </summary>
    /// <remarks>
    ///   If true, then initial data points where the Y value
    ///   is zero are not included when automatically determining the scale <see cref="Scale.Min" />,
    ///   <see cref="Scale.Max" />, and <see cref="Scale.MajorStep" /> size.
    ///   All data after the first non-zero Y value are included.
    /// </remarks>
    /// <seealso cref="Default.IsIgnoreInitial" />
    [Bindable(true)]
    [Browsable(true)]
    [Category("Display")]
    [NotifyParentProperty(true)]
    [Description("Determines whether the auto-ranged scale will include all data points" +
                 " or just the visible data points")]
    public bool IsIgnoreInitial { get; set; }

    /// <summary>
    ///   Gets or sets a value that determines whether or not initial
    ///   <see cref="PointPairBase.Missing" /> values will cause the line segments of
    ///   a curve to be discontinuous.
    /// </summary>
    /// <remarks>
    ///   If this field is true, then the curves
    ///   will be plotted as continuous lines as if the Missing values did not exist.
    ///   Use the public property <see cref="IsIgnoreMissing" /> to access
    ///   this value.
    /// </remarks>
    public bool IsIgnoreMissing { get; set; }

    /// <summary>
    ///   Gets a value that indicates whether or not the <see cref="ZoomStateStack" /> for
    ///   this <see cref="GraphPane" /> is empty.  Note that this value is only used for
    ///   the <see cref="ZedGraphControl" />.
    /// </summary>
    public bool IsZoomed => !ZoomStack.IsEmpty;

    /// <summary>
    ///   Determines how the <see cref="LineItem" />
    ///   graphs will be displayed. See the <see cref="ZedGraph.LineType" /> enum
    ///   for the individual types available.
    /// </summary>
    /// <seealso cref="Default.LineType" />
    public LineType LineType { get; set; }

    /// <summary>
    ///   Gets a reference to the <see cref="ZoomStateStack" /> for this <see cref="GraphPane" />.
    ///   The <see cref="ZoomStateStack" /> stores prior <see cref="ZoomState" /> objects
    ///   containing scale range information.  This enables zooming and panning functionality
    ///   for the <see cref="ZedGraphControl" />.
    /// </summary>
    public ZoomStateStack ZoomStack { get; }

    #endregion

    #region Constructors

    /// <summary>
    ///   Default Constructor.  Sets the <see cref="PaneBase.Rect" /> to (0, 0, 500, 375), and
    ///   sets the <see cref="PaneBase.Title" /> and <see cref="Axis.Title" /> values to empty
    ///   strings.
    /// </summary>
    public GraphPane()
      : this(new RectangleF(0, 0, 500, 375), "", "", "") {}

    /// <summary>
    ///   Constructor for the <see cref="GraphPane" /> object.  This routine will
    ///   initialize all member variables and classes, setting appropriate default
    ///   values as defined in the <see cref="Default" /> class.
    /// </summary>
    /// <param name="rect">
    ///   A rectangular screen area where the graph is to be displayed.
    ///   This area can be any size, and can be resize at any time using the
    ///   <see cref="PaneBase.Rect" /> property.
    /// </param>
    /// <param name="title">The <see cref="PaneBase.Title" /> for this <see cref="GraphPane" /></param>
    /// <param name="xTitle">The <see cref="Axis.Title" /> for the <see cref="XAxis" /></param>
    /// <param name="yTitle">The <see cref="Axis.Title" /> for the <see cref="YAxis" /></param>
    public GraphPane(RectangleF rect, string title, string xTitle, string yTitle)
      : base(title, rect)
    {
      XAxis           = new XAxis(xTitle);
      X2Axis          = new X2Axis("");

      YAxisList       = new YAxisList();
      Y2AxisList      = new Y2AxisList();

      YAxisList.Add(new YAxis(yTitle));
      Y2AxisList.Add(new Y2Axis(string.Empty));

      CurveList       = new CurveList();
      ZoomStack       = new ZoomStateStack();

      IsIgnoreInitial = Default.IsIgnoreInitial;
      IsBoundedRanges = Default.IsBoundedRanges;
      IsAlignGrids    = false;

      _chart          = new Chart();

      _barSettings    = new BarSettings(this);

      LineType        = Default.LineType;
    }

    /// <summary>
    ///   The Copy Constructor
    /// </summary>
    /// <param name="rhs">The GraphPane object from which to copy</param>
    public GraphPane(GraphPane rhs)
      : base(rhs)
    {
      // copy values for all the value types
      IsIgnoreInitial = rhs.IsIgnoreInitial;
      IsBoundedRanges = rhs.IsBoundedRanges;
      IsAlignGrids    = rhs.IsAlignGrids;

      _chart          = rhs._chart.Clone();

      _barSettings    = new BarSettings(rhs._barSettings, this);

      LineType        = rhs.LineType;

      // copy all the reference types with deep copies
      XAxis           = new XAxis(rhs.XAxis);
      X2Axis          = new X2Axis(rhs.X2Axis);

      YAxisList       = new YAxisList(rhs.YAxisList);
      Y2AxisList      = new Y2AxisList(rhs.Y2AxisList);

      CurveList       = new CurveList(rhs.CurveList);
      ZoomStack       = new ZoomStateStack(rhs.ZoomStack);
    }

    /// <summary>
    ///   Typesafe, deep-copy clone method.
    /// </summary>
    /// <returns>A new, independent copy of this class</returns>
    public GraphPane Clone()
    {
      return new GraphPane(this);
    }

    /// <summary>
    ///   Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
    ///   calling the typed version of <see cref="Clone" />
    /// </summary>
    /// <returns>A deep copy of this object</returns>
    object ICloneable.Clone()
    {
      return Clone();
    }

    #endregion

    #region Serialization

    /// <summary>
    ///   Current schema value that defines the version of the serialized file
    /// </summary>
    //changed to 2 when yAxisList and y2AxisList were added
    //changed to 3 when chart object was added
    //changed to 10 when refactored to version 5
    //changed to 11 when added x2axis
    public const int schema2 = 11;

    /// <summary>
    ///   Constructor for deserializing objects
    /// </summary>
    /// <param name="info">
    ///   A <see cref="SerializationInfo" /> instance that defines the serialized data
    /// </param>
    /// <param name="context">
    ///   A <see cref="StreamingContext" /> instance that contains the serialized data
    /// </param>
    protected GraphPane(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      var sch         = info.GetInt32("schema2");

      XAxis           = (XAxis)info.GetValue("xAxis", typeof(XAxis));
      X2Axis          = (X2Axis)info.GetValue("x2Axis", typeof(X2Axis));
      YAxisList       = (YAxisList)info.GetValue("yAxisList", typeof(YAxisList));
      Y2AxisList      = (Y2AxisList)info.GetValue("y2AxisList", typeof(Y2AxisList));
      CurveList       = (CurveList)info.GetValue("curveList", typeof(CurveList));
      _chart          = (Chart)info.GetValue("chart", typeof(Chart));
      _barSettings    = (BarSettings)info.GetValue("barSettings", typeof(BarSettings));
      _barSettings._ownerPane = this;

      IsIgnoreInitial = info.GetBoolean("isIgnoreInitial");
      IsBoundedRanges = info.GetBoolean("isBoundedRanges");
      IsIgnoreMissing = info.GetBoolean("isIgnoreMissing");
      IsAlignGrids    = info.GetBoolean("isAlignGrids");

      LineType        = (LineType)info.GetValue("lineType", typeof(LineType));

      ZoomStack       = new ZoomStateStack();
    }

    /// <summary>
    ///   Populates a <see cref="SerializationInfo" /> instance with the data needed to serialize the target object
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo" /> instance that defines the serialized data</param>
    /// <param name="context">A <see cref="StreamingContext" /> instance that contains the serialized data</param>
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);

      info.AddValue("schema2",         schema2);
      info.AddValue("xAxis",           XAxis);
      info.AddValue("x2Axis",          X2Axis);
      info.AddValue("yAxisList",       YAxisList);
      info.AddValue("y2AxisList",      Y2AxisList);
      info.AddValue("curveList",       CurveList);
      info.AddValue("chart",           _chart);
      info.AddValue("barSettings",     _barSettings);
      info.AddValue("isIgnoreInitial", IsIgnoreInitial);
      info.AddValue("isBoundedRanges", IsBoundedRanges);
      info.AddValue("isIgnoreMissing", IsIgnoreMissing);
      info.AddValue("isAlignGrids",    IsAlignGrids);
      info.AddValue("lineType",        LineType);
    }

    #endregion

    #region Rendering Methods

    /// <summary>
    ///   AxisChange causes the axes scale ranges to be recalculated based on the current data range.
    /// </summary>
    /// <remarks>
    ///   There is no obligation to call AxisChange() for manually scaled axes.  AxisChange() is only
    ///   intended to handle auto scaling operations.  Call this function anytime you change, add, or
    ///   remove curve data to insure that the scale range of the axes are appropriate for the data range.
    ///   This method calculates
    ///   a scale minimum, maximum, and step size for each axis based on the current curve data.
    ///   Only the axis attributes (min, max, step) that are set to auto-range
    ///   (<see cref="Scale.MinAuto" />, <see cref="Scale.MaxAuto" />, <see cref="Scale.MajorStepAuto" />)
    ///   will be modified.  You must call <see cref="Control.Invalidate()" /> after calling
    ///   AxisChange to make sure the display gets updated.<br />
    ///   This overload of AxisChange just uses a throw-away bitmap as Graphics.
    ///   If you have a Graphics instance available from your Windows Form, you should use
    ///   the <see cref="AxisChange(Graphics)" /> overload instead.
    /// </remarks>
    public void AxisChange()
    {
      using (var img = new Bitmap((int)Rect.Width, (int)Rect.Height))
      using (var g   = Graphics.FromImage(img))
        AxisChange(g);
    }

    /// <summary>
    ///   AxisChange causes the axes scale ranges to be recalculated based on the current data range.
    /// </summary>
    /// <remarks>
    ///   There is no obligation to call AxisChange() for manually scaled axes.  AxisChange() is only
    ///   intended to handle auto scaling operations.  Call this function anytime you change, add, or
    ///   remove curve data to insure that the scale range of the axes are appropriate for the data range.
    ///   This method calculates
    ///   a scale minimum, maximum, and step size for each axis based on the current curve data.
    ///   Only the axis attributes (min, max, step) that are set to auto-range
    ///   (<see cref="Scale.MinAuto" />, <see cref="Scale.MaxAuto" />, <see cref="Scale.MajorStepAuto" />)
    ///   will be modified.  You must call
    ///   <see cref="Control.Invalidate()" /> after calling AxisChange to make sure the display gets updated.
    /// </remarks>
    /// <param name="g">
    ///   A graphic device object to be drawn into.  This is normally e.Graphics from the
    ///   PaintEventArgs argument to the Paint() method.
    /// </param>
    public void AxisChange(Graphics g)
    {
      // Get the scale range of the data (all curves)
      CurveList.GetRange(IsIgnoreInitial, IsBoundedRanges, this);

      // Determine the scale factor
      CalcScaleFactor();

      // For pie charts, go ahead and turn off the axis displays if it's only pies
      if (CurveList.IsPieOnly)
      {
        //don't want to display axis or border if there's only pies
        XAxis.IsVisible = false;
        X2Axis.IsVisible = false;
        YAxis.IsVisible = false;
        Y2Axis.IsVisible = false;
        _chart.Border.IsVisible = false;
        //this.Legend.Position = LegendPos.TopCenter;
      }

      // Set the ClusterScaleWidth, if needed
      //_barSettings.CalcClusterScaleWidth();
      if (_barSettings.ClusterScaleWidthAuto)
        _barSettings._clusterScaleWidth = 1.0;

      // if the ChartRect is not yet determined, then pick a scale based on a default ChartRect
      // size (using 75% of Rect -- code is in Axis.CalcMaxLabels() )
      // With the scale picked, call CalcChartRect() so calculate a real ChartRect
      // then let the scales re-calculate to make sure that the assumption was ok

      // Pick new scales based on the range
      PickScale(g, CalcScaleFactor());
      var rect = CalcChartRect(g);

      if (_chart._isRectAuto)
        _chart._rect = rect;

      // Pick new scales based on the range (need to call it again so that tick sizes
      // are calculated based on scale's new min/max settings
      //PickScale( g, CalcScaleFactor() );

      // Set the ClusterScaleWidth, if needed
      _barSettings.CalcClusterScaleWidth();

      // Trigger the AxisChangeEvent
      AxisChangeEvent?.Invoke(this);
    }

    /// <summary>
    ///   Calculate the <see cref="ZedGraph.Chart.Rect" /> based on the <see cref="PaneBase.Rect" />.
    /// </summary>
    /// <remarks>
    ///   The ChartRect
    ///   is the plot area bounded by the axes, and the rect is the total area as
    ///   specified by the client application.
    /// </remarks>
    /// <param name="g">
    ///   A graphic device object to be drawn into.  This is normally e.Graphics from the
    ///   PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <returns>The calculated chart rect, in pixel coordinates.</returns>
    public RectangleF CalcChartRect(Graphics g)
    {
      // Calculate the chart rect, deducting the area for the scales, titles, legend, etc.
      //int    hStack;
      //float  legendWidth, legendHeight;

      return CalcChartRect(g, CalcScaleFactor());
    }

    public RectangleF CalcChartRect(Graphics g, float scaleFactor)
    {
      // chart rect starts out at the full pane rect less the margins
      //   and less space for the Pane title
      var clientRect = CalcClientRect(g, scaleFactor);

      //float minSpaceX = 0;
      //float minSpaceY = 0;
      //float minSpaceY2 = 0;
      float totSpaceY = 0;
      //float spaceY2 = 0;

      // actual minimum axis space for the left side of the chart rect
      float minSpaceL = 0;
      // actual minimum axis space for the right side of the chart rect
      float minSpaceR = 0;
      // actual minimum axis space for the bottom side of the chart rect
      float minSpaceB = 0;
      // actual minimum axis space for the top side of the chart rect
      float minSpaceT = 0;

      XAxis.CalcSpace(g, this, scaleFactor, out minSpaceB);
      X2Axis.CalcSpace(g, this, scaleFactor, out minSpaceT);

      //minSpaceB = _xAxis.tmpMinSpace;

      foreach (var axis in YAxisList)
      {
        float fixedSpace;
        var tmp = axis.CalcSpace(g, this, scaleFactor, out fixedSpace);
        //if ( !axis.CrossAuto || axis.Cross < _xAxis.Min )
        if (axis.IsCrossShifted(this))
          totSpaceY += tmp;

        minSpaceL += fixedSpace;
      }
      foreach (var axis in Y2AxisList)
      {
        float fixedSpace;
        var tmp = axis.CalcSpace(g, this, scaleFactor, out fixedSpace);
        //if ( !axis.CrossAuto || axis.Cross < _xAxis.Min )
        if (axis.IsCrossShifted(this))
          totSpaceY += tmp;

        minSpaceR += fixedSpace;
      }

      float spaceB = 0, spaceT = 0, spaceL = 0, spaceR = 0;

      SetSpace(XAxis, clientRect.Height - XAxis._tmpSpace, ref spaceB, ref spaceT);
      //      minSpaceT = Math.Max( minSpaceT, spaceT );
      SetSpace(X2Axis, clientRect.Height - X2Axis._tmpSpace, ref spaceT, ref spaceB);
      XAxis._tmpSpace = spaceB;
      X2Axis._tmpSpace = spaceT;

      // TODO: Set axis.Rect for each X and Y axises

      float totSpaceL = 0;
      float totSpaceR = 0;

      foreach (var axis in YAxisList)
      {
        SetSpace(axis, clientRect.Width - totSpaceY, ref spaceL, ref spaceR);
        minSpaceR = Math.Max(minSpaceR, spaceR);
        totSpaceL += spaceL;
        axis._tmpSpace = spaceL;
      }
      foreach (var axis in Y2AxisList)
      {
        SetSpace(axis, clientRect.Width - totSpaceY, ref spaceR, ref spaceL);
        minSpaceL = Math.Max(minSpaceL, spaceL);
        totSpaceR += spaceR;
        axis._tmpSpace = spaceR;
      }

      var tmpRect = clientRect;

      totSpaceL = Math.Max(totSpaceL, minSpaceL);
      totSpaceR = Math.Max(totSpaceR, minSpaceR);
      spaceB = Math.Max(spaceB, minSpaceB);
      spaceT = Math.Max(spaceT, minSpaceT);

      tmpRect.X += totSpaceL;
      tmpRect.Width -= totSpaceL + totSpaceR;
      tmpRect.Height -= spaceT + spaceB;
      tmpRect.Y += spaceT;

      _legend.CalcRect(g, this, scaleFactor, ref tmpRect);

      return tmpRect;
    }

    /// <summary>
    ///   Draw all elements in the <see cref="GraphPane" /> to the specified graphics device.
    /// </summary>
    /// <remarks>
    ///   This method
    ///   should be part of the Paint() update process.  Calling this routine will redraw all
    ///   features of the graph.  No preparation is required other than an instantiated
    ///   <see cref="GraphPane" /> object.
    /// </remarks>
    /// <param name="g">
    ///   A graphic device object to be drawn into.  This is normally e.Graphics from the
    ///   PaintEventArgs argument to the Paint() method.
    /// </param>
    public override void Draw(Graphics g)
    {
      // Calculate the chart rect, deducting the area for the scales, titles, legend, etc.
      //int    hStack;
      //float  legendWidth, legendHeight;

      // Draw the pane border & background fill, the title, and the GraphObj objects that lie at
      // ZOrder.G_BehindAll
      base.Draw(g);

      if ((_rect.Width <= 1) || (_rect.Height <= 1))
        return;

      // Clip everything to the rect
      g.SetClip(_rect);

      var scaleFactor = CalcScaleFactor();

      // if the size of the ChartRect is determined automatically, then do so
      // otherwise, calculate the legendrect, scalefactor, hstack, and legendwidth parameters
      // but leave the ChartRect alone
      if (_chart._isRectAuto)
        CurveClipRect = _chart._rect = CalcChartRect(g, scaleFactor);
      else
        CurveClipRect = CalcChartRect(g, scaleFactor);

      // do a sanity check on the ChartRect
      if ((_chart._rect.Width < 1) || (_chart._rect.Height < 1))
        return;

      // Draw the graph features only if there is at least one curve with data
      // if ( _curveList.HasData() &&
      // Go ahead and draw the graph, even without data.  This makes the control
      // version still look like a graph before it is fully set up
      var showGraf = AxisRangesValid();

      // Setup the axes for graphing - This setup must be done before
      // the GraphObj's are drawn so that the Transform functions are
      // ready.  Also, this should be done before CalcChartRect so that the
      // Axis.Cross - shift parameter can be calculated.
      XAxis.Scale.SetupScaleData(this);
      X2Axis.Scale.SetupScaleData(this);
      foreach (var axis in YAxisList)
        axis.Scale.SetupScaleData(this);
      foreach (var axis in Y2AxisList)
        axis.Scale.SetupScaleData(this);

      // Draw the GraphItems that are behind the Axis objects
      if (showGraf)
        GraphObjList.Draw(g, this, scaleFactor, ZOrder.G_BehindChartFill);

      // Fill the axis background
      _chart.Fill.Draw(g, _chart._rect);

      if (showGraf)
      {
        // Draw the GraphItems that are behind the CurveItems
        GraphObjList.Draw(g, this, scaleFactor, ZOrder.F_BehindGrid);

        DrawGrid(g, scaleFactor);

        // Draw the GraphItems that are behind the CurveItems
        GraphObjList.Draw(g, this, scaleFactor, ZOrder.E_BehindCurves);

        // Clip the points to the actual plot area
        g.SetClip(CurveClipRect, CombineMode.Intersect);

        if (!g.IsClipEmpty) // update region may not be in chart at all
          CurveList.Draw(g, this, scaleFactor);
        g.SetClip(_rect);

        // Draw the GraphItems that are behind the Axis objects
        GraphObjList.Draw(g, this, scaleFactor, ZOrder.D_BehindAxis);

        // Draw the Axes
        XAxis.Draw(g, this, scaleFactor, 0.0f);
        X2Axis.Draw(g, this, scaleFactor, 0.0f);

        float shift = 0;
        foreach (var axis in YAxisList)
        {
          axis.Draw(g, this, scaleFactor, shift);
          shift += axis._tmpSpace;
        }

        shift = 0;
        foreach (var axis in Y2AxisList)
        {
          axis.Draw(g, this, scaleFactor, shift);
          shift += axis._tmpSpace;
        }

        // Draw the GraphItems that are behind the Axis border
        GraphObjList.Draw(g, this, scaleFactor, ZOrder.C_BehindChartBorder);
      }

      // Border the axis itself
      _chart.Border.Draw(g, this, scaleFactor, _chart._rect);

      if (showGraf)
      {
        // Draw the GraphItems that are behind the Legend object
        GraphObjList.Draw(g, this, scaleFactor, ZOrder.B_BehindLegend);

        _legend.Draw(g, this, scaleFactor);

        // Draw the GraphItems that are in front of all other items
        GraphObjList.Draw(g, this, scaleFactor, ZOrder.A_InFront);
      }

      // Reset the clipping
      g.ResetClip();

      // Reset scale data
      // this sets the temp values to NaN to cause an exception if these values are
      // being used improperly
      // Don't do this, since the web control needs access
      /*
      _xAxis.Scale.ResetScaleData();
      foreach ( Axis axis in _yAxisList )
        axis.Scale.ResetScaleData();
      foreach ( Axis axis in _y2AxisList )
        axis.Scale.ResetScaleData();
      */

      DrawEvent?.Invoke(this);
    }

    /// <summary>
    ///   This method will set the <see cref="Axis.MinSpace" /> property for all three axes;
    ///   <see cref="XAxis" />, <see cref="YAxis" />, and <see cref="Y2Axis" />.
    /// </summary>
    /// <remarks>
    ///   The <see cref="Axis.MinSpace" />
    ///   is calculated using the currently required space multiplied by a fraction
    ///   (<paramref>bufferFraction</paramref>).
    ///   The currently required space is calculated using <see cref="Axis.CalcSpace" />, and is
    ///   based on current data ranges, font sizes, etc.  The "space" is actually the amount of space
    ///   required to fit the tic marks, scale labels, and axis title.
    ///   The calculation is done by calling the <see cref="Axis.SetMinSpaceBuffer" /> method for
    ///   each <see cref="Axis" />.
    /// </remarks>
    /// <param name="g">
    ///   A graphic device object to be drawn into.  This is normally e.Graphics from the
    ///   PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="bufferFraction">
    ///   The amount of space to allocate for the axis, expressed
    ///   as a fraction of the currently required space.  For example, a value of 1.2 would
    ///   allow for 20% extra above the currently required space.
    /// </param>
    /// <param name="isGrowOnly">
    ///   If true, then this method will only modify the <see cref="Axis.MinSpace" />
    ///   property if the calculated result is more than the current value.
    /// </param>
    public void SetMinSpaceBuffer(Graphics g, float bufferFraction, bool isGrowOnly)
    {
      XAxis.SetMinSpaceBuffer(g, this, bufferFraction, isGrowOnly);
      X2Axis.SetMinSpaceBuffer(g, this, bufferFraction, isGrowOnly);
      foreach (var axis in YAxisList)
        axis.SetMinSpaceBuffer(g, this, bufferFraction, isGrowOnly);
      foreach (var axis in Y2AxisList)
        axis.SetMinSpaceBuffer(g, this, bufferFraction, isGrowOnly);
    }

    internal void DrawGrid(Graphics g, float scaleFactor)
    {
      XAxis.DrawGrid(g, this, scaleFactor, 0.0f);
      X2Axis.DrawGrid(g, this, scaleFactor, 0.0f);

      var shiftPos = 0.0f;
      foreach (var yAxis in YAxisList)
      {
        yAxis.DrawGrid(g, this, scaleFactor, shiftPos);
        shiftPos += yAxis._tmpSpace;
      }

      shiftPos = 0.0f;
      foreach (var y2Axis in Y2AxisList)
      {
        y2Axis.DrawGrid(g, this, scaleFactor, shiftPos);
        shiftPos += y2Axis._tmpSpace;
      }
    }

    private bool AxisRangesValid()
    {
      var showGraf =
        ((double.IsNaN(XAxis.Scale._min) && double.IsNaN(XAxis.Scale._max)) ||
         (XAxis.Scale._min < XAxis.Scale._max)) &&
        ((double.IsNaN(X2Axis.Scale._min) && double.IsNaN(X2Axis.Scale._max)) ||
         (X2Axis.Scale._min < X2Axis.Scale._max));
      foreach (var axis in YAxisList)
        if (axis.Scale._min >= axis.Scale._max)
          showGraf = false;
      foreach (var axis in Y2AxisList)
        if (axis.Scale._min >= axis.Scale._max)
          showGraf = false;

      return showGraf;
    }

    private void ForceNumTics(Axis axis, int numTics)
    {
      if (!axis.Scale.MaxAuto) return;
      var nTics = axis.Scale.CalcNumTics();
      if (nTics < numTics)
        axis.Scale.MaxLinearized += axis.Scale._majorStep*(numTics - nTics);
    }

    private void PickScale(Graphics g, float scaleFactor)
    {
      var maxTics = 0;

      XAxis.Scale.PickScale(this, g, scaleFactor);
      X2Axis.Scale.PickScale(this, g, scaleFactor);

      foreach (var axis in YAxisList)
      {
        axis.Scale.PickScale(this, g, scaleFactor);
        if (!axis.Scale.MaxAuto) continue;

        var nTics = axis.Scale.CalcNumTics();
        maxTics = nTics > maxTics ? nTics : maxTics;
      }
      foreach (var axis in Y2AxisList)
      {
        axis.Scale.PickScale(this, g, scaleFactor);
        if (!axis.Scale.MaxAuto) continue;

        var nTics = axis.Scale.CalcNumTics();
        maxTics = nTics > maxTics ? nTics : maxTics;
      }

      if (!IsAlignGrids) return;

      foreach (var axis in YAxisList)
        ForceNumTics(axis, maxTics);

      foreach (var axis in Y2AxisList)
        ForceNumTics(axis, maxTics);
    }

    /// <summary>
    ///   Calculate the <see cref="ZedGraph.Chart.Rect" /> based on the <see cref="PaneBase.Rect" />.
    /// </summary>
    /// <remarks>
    ///   The ChartRect
    ///   is the plot area bounded by the axes, and the rect is the total area as
    ///   specified by the client application.
    /// </remarks>
    /// <param name="g">
    ///   A graphic device object to be drawn into.  This is normally e.Graphics from the
    ///   PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="scaleFactor">
    ///   The scaling factor for the features of the graph based on the <see cref="PaneBase.BaseDimension" />.  This
    ///   scaling factor is calculated by the <see cref="PaneBase.CalcScaleFactor()" /> method.  The scale factor
    ///   represents a linear multiple to be applied to font sizes, symbol sizes, etc.
    /// </param>
    /// <returns>The calculated chart rect, in pixel coordinates.</returns>
    private void SetSpace(Axis axis, float clientSize, ref float spaceNorm,
                          ref float spaceAlt)
    {
      //spaceNorm = 0;
      //spaceAlt = 0;

      var crossFrac = axis.CalcCrossFraction(this);
      var crossPix = crossFrac*(1 + crossFrac)*(1 + crossFrac*crossFrac)*clientSize;

      if (!axis.IsPrimary(this) && axis.IsCrossShifted(this))
        axis._tmpSpace = 0;

      if (axis._tmpSpace < crossPix)
        axis._tmpSpace = 0;
      else if (crossPix > 0)
        axis._tmpSpace -= crossPix;

      if (axis.Scale.IsLabelsInside &&
          (axis.IsPrimary(this) || ((crossFrac != 0.0) && (crossFrac != 1.0))))
        spaceAlt = axis._tmpSpace;
      else
        spaceNorm = axis._tmpSpace;
    }

    #endregion

    #region AddCurve Methods

    /// <summary>
    ///   Add a bar type curve (<see cref="CurveItem" /> object) to the plot with
    ///   the given data points (<see cref="IPointList" />) and properties.
    ///   This is simplified way to add curves without knowledge of the
    ///   <see cref="CurveList" /> class.  An alternative is to use
    ///   the <see cref="ZedGraph.CurveList" /> Add() method.
    /// </summary>
    /// <param name="label">
    ///   The text label (string) for the curve that will be
    ///   used as a <see cref="Legend" /> entry.
    /// </param>
    /// <param name="points">
    ///   A <see cref="IPointList" /> of double precision value pairs that define
    ///   the X and Y values for this curve
    /// </param>
    /// <param name="color">The color to used to fill the bars</param>
    /// <returns>
    ///   A <see cref="CurveItem" /> class for the newly created bar curve.
    ///   This can then be used to access all of the curve properties that
    ///   are not defined as arguments to the
    ///   <see cref="AddBar(string,IPointList,Color)" /> method.
    /// </returns>
    public BarItem AddBar(string label, IPointList points, Color color, int zOrder = -1)
    {
      var curve = new BarItem(label, points, color, zOrder);
      CurveList.Add(curve);
      return curve;
    }

    /// <summary>
    ///   Add a bar type curve (<see cref="CurveItem" /> object) to the plot with
    ///   the given data points (double arrays) and properties.
    ///   This is simplified way to add curves without knowledge of the
    ///   <see cref="CurveList" /> class.  An alternative is to use
    ///   the <see cref="ZedGraph.CurveList" /> Add() method.
    /// </summary>
    /// <param name="label">
    ///   The text label (string) for the curve that will be
    ///   used as a <see cref="Legend" /> entry.
    /// </param>
    /// <param name="x">
    ///   An array of double precision X values (the
    ///   independent values) that define the curve.
    /// </param>
    /// <param name="y">
    ///   An array of double precision Y values (the
    ///   dependent values) that define the curve.
    /// </param>
    /// <param name="color">The color to used for the bars</param>
    /// <returns>
    ///   A <see cref="CurveItem" /> class for the newly created bar curve.
    ///   This can then be used to access all of the curve properties that
    ///   are not defined as arguments to the
    ///   <see cref="AddBar(string,double[],double[],Color)" /> method.
    /// </returns>
    public BarItem AddBar(string label, double[] x, double[] y, Color color,
                          int zOrder = -1)
    {
      var curve = new BarItem(label, x, y, color, zOrder);
      CurveList.Add(curve);
      return curve;
    }

    /// <summary>
    ///   Add a curve (<see cref="CurveItem" /> object) to the plot with
    ///   the given data points (double arrays) and properties.
    ///   This is simplified way to add curves without knowledge of the
    ///   <see cref="CurveList" /> class.  An alternative is to use
    ///   the <see cref="ZedGraph.CurveList" /> Add() method.
    /// </summary>
    /// <param name="label">
    ///   The text label (string) for the curve that will be
    ///   used as a <see cref="Legend" /> entry.
    /// </param>
    /// <param name="x">
    ///   An array of double precision X values (the
    ///   independent values) that define the curve.
    /// </param>
    /// <param name="y">
    ///   An array of double precision Y values (the
    ///   dependent values) that define the curve.
    /// </param>
    /// <param name="color">
    ///   The color to used for the curve line,
    ///   symbols, etc.
    /// </param>
    /// <returns>
    ///   A <see cref="CurveItem" /> class for the newly created curve.
    ///   This can then be used to access all of the curve properties that
    ///   are not defined as arguments to the
    ///   <see cref="AddCurve(string,double[],double[],Color)" /> method.
    /// </returns>
    public LineItem AddCurve(string label, double[] x, double[] y, Color color,
                             int zOrder = -1)
    {
      var curve = new LineItem(label, x, y, color, SymbolType.Default, zOrder: zOrder);
      CurveList.Add(curve);
      return curve;
    }

    /// <summary>
    ///   Add a curve (<see cref="CurveItem" /> object) to the plot with
    ///   the given data points (<see cref="IPointList" />) and properties.
    ///   This is simplified way to add curves without knowledge of the
    ///   <see cref="CurveList" /> class.  An alternative is to use
    ///   the <see cref="ZedGraph.CurveList" /> Add() method.
    /// </summary>
    /// <param name="label">
    ///   The text label (string) for the curve that will be
    ///   used as a <see cref="Legend" /> entry.
    /// </param>
    /// <param name="points">
    ///   A <see cref="IPointList" /> of double precision value pairs that define
    ///   the X and Y values for this curve
    /// </param>
    /// <param name="color">
    ///   The color to used for the curve line,
    ///   symbols, etc.
    /// </param>
    /// <returns>
    ///   A <see cref="CurveItem" /> class for the newly created curve.
    ///   This can then be used to access all of the curve properties that
    ///   are not defined as arguments to the
    ///   <see cref="AddCurve(string,IPointList,Color)" /> method.
    /// </returns>
    public LineItem AddCurve(string label, IPointList points, Color color, int zOrder = -1)
    {
      var curve = new LineItem(label, points, color, SymbolType.Default, zOrder: zOrder);
      CurveList.Add(curve);
      return curve;
    }

    /// <summary>
    ///   Add a curve (<see cref="CurveItem" /> object) to the plot with
    ///   the given data points (double arrays) and properties.
    ///   This is simplified way to add curves without knowledge of the
    ///   <see cref="CurveList" /> class.  An alternative is to use
    ///   the <see cref="ZedGraph.CurveList" /> Add() method.
    /// </summary>
    /// <param name="label">
    ///   The text label (string) for the curve that will be
    ///   used as a <see cref="Legend" /> entry.
    /// </param>
    /// <param name="x">
    ///   An array of double precision X values (the
    ///   independent values) that define the curve.
    /// </param>
    /// <param name="y">
    ///   An array of double precision Y values (the
    ///   dependent values) that define the curve.
    /// </param>
    /// <param name="color">
    ///   The color to used for the curve line,
    ///   symbols, etc.
    /// </param>
    /// <param name="symbolType">
    ///   A symbol type (<see cref="SymbolType" />)
    ///   that will be used for this curve.
    /// </param>
    /// <returns>
    ///   A <see cref="CurveItem" /> class for the newly created curve.
    ///   This can then be used to access all of the curve properties that
    ///   are not defined as arguments to the
    ///   <see cref="AddCurve(string,double[],double[],Color,SymbolType)" /> method.
    /// </returns>
    public LineItem AddCurve(string label, double[] x, double[] y,
                             Color color, SymbolType symbolType, int zOrder = -1)
    {
      var curve = new LineItem(label, x, y, color, symbolType, zOrder: zOrder);
      CurveList.Add(curve);
      return curve;
    }

    /// <summary>
    ///   Add a curve (<see cref="CurveItem" /> object) to the plot with
    ///   the given data points (<see cref="IPointList" />) and properties.
    ///   This is simplified way to add curves without knowledge of the
    ///   <see cref="CurveList" /> class.  An alternative is to use
    ///   the <see cref="ZedGraph.CurveList" /> Add() method.
    /// </summary>
    /// <param name="label">
    ///   The text label (string) for the curve that will be
    ///   used as a <see cref="Legend" /> entry.
    /// </param>
    /// <param name="points">
    ///   A <see cref="IPointList" /> of double precision value pairs that define
    ///   the X and Y values for this curve
    /// </param>
    /// <param name="color">
    ///   The color to used for the curve line,
    ///   symbols, etc.
    /// </param>
    /// <param name="symbolType">
    ///   A symbol type (<see cref="SymbolType" />)
    ///   that will be used for this curve.
    /// </param>
    /// <returns>
    ///   A <see cref="CurveItem" /> class for the newly created curve.
    ///   This can then be used to access all of the curve properties that
    ///   are not defined as arguments to the
    ///   <see cref="AddCurve(string,IPointList,Color,SymbolType)" /> method.
    /// </returns>
    public LineItem AddCurve(string label, IPointList points,
                             Color color, SymbolType symbolType, int zOrder = -1)
    {
      var curve = new LineItem(label, points, color, symbolType, zOrder: zOrder);
      CurveList.Add(curve);
      return curve;
    }

    /// <summary>
    ///   Add an error bar set (<see cref="ErrorBarItem" /> object) to the plot with
    ///   the given data points (<see cref="IPointList" />) and properties.
    ///   This is simplified way to add curves without knowledge of the
    ///   <see cref="CurveList" /> class.  An alternative is to use
    ///   the <see cref="ZedGraph.CurveList" /> Add() method.
    /// </summary>
    /// <param name="label">
    ///   The text label (string) for the curve that will be
    ///   used as a <see cref="Legend" /> entry.
    /// </param>
    /// <param name="x">
    ///   An array of double precision X values (the
    ///   independent values) that define the curve.
    /// </param>
    /// <param name="y">
    ///   An array of double precision Y values (the
    ///   dependent values) that define the curve.
    /// </param>
    /// <param name="baseValue">
    ///   An array of double precision values that define the
    ///   base value (the bottom) of the bars for this curve.
    /// </param>
    /// <param name="color">
    ///   The color to used for the curve line,
    ///   symbols, etc.
    /// </param>
    /// <returns>
    ///   An <see cref="ErrorBarItem" /> class for the newly created curve.
    ///   This can then be used to access all of the curve properties that
    ///   are not defined as arguments to the
    ///   <see cref="AddErrorBar(string,IPointList,Color)" /> method.
    /// </returns>
    public ErrorBarItem AddErrorBar(string label, double[] x, double[] y,
                                    double[] baseValue, Color color, int zOrder = -1)
    {
      var curve = new ErrorBarItem(label, new PointPairList(x, y, baseValue),
                                   color, zOrder);
      CurveList.Add(curve);
      return curve;
    }

    /// <summary>
    ///   Add an error bar set (<see cref="ErrorBarItem" /> object) to the plot with
    ///   the given data points (<see cref="IPointList" />) and properties.
    ///   This is simplified way to add curves without knowledge of the
    ///   <see cref="CurveList" /> class.  An alternative is to use
    ///   the <see cref="ZedGraph.CurveList" /> Add() method.
    /// </summary>
    /// <param name="label">
    ///   The text label (string) for the curve that will be
    ///   used as a <see cref="Legend" /> entry.
    /// </param>
    /// <param name="points">
    ///   A <see cref="IPointList" /> of double precision value pairs that define
    ///   the X and Y values for this curve
    /// </param>
    /// <param name="color">
    ///   The color to used for the curve line,
    ///   symbols, etc.
    /// </param>
    /// <returns>
    ///   An <see cref="ErrorBarItem" /> class for the newly created curve.
    ///   This can then be used to access all of the curve properties that
    ///   are not defined as arguments to the
    ///   <see cref="AddErrorBar(string,IPointList,Color)" /> method.
    /// </returns>
    public ErrorBarItem AddErrorBar(string label, IPointList points, Color color,
                                    int zOrder = -1)
    {
      var curve = new ErrorBarItem(label, points, color, zOrder);
      CurveList.Add(curve);
      return curve;
    }

    /// <summary>
    ///   Add a "High-Low" bar type curve (<see cref="HiLowBarItem" /> object) to the plot with
    ///   the given data points (double arrays) and properties.
    ///   This is simplified way to add curves without knowledge of the
    ///   <see cref="CurveList" /> class.  An alternative is to use
    ///   the <see cref="ZedGraph.CurveList" /> Add() method.
    /// </summary>
    /// <param name="label">
    ///   The text label (string) for the curve that will be
    ///   used as a <see cref="Legend" /> entry.
    /// </param>
    /// <param name="x">
    ///   An array of double precision X values (the
    ///   independent values) that define the curve.
    /// </param>
    /// <param name="y">
    ///   An array of double precision Y values (the
    ///   dependent values) that define the curve.
    /// </param>
    /// <param name="baseVal">
    ///   An array of double precision values that define the
    ///   base value (the bottom) of the bars for this curve.
    /// </param>
    /// <param name="color">The color to used for the bars</param>
    /// <returns>
    ///   A <see cref="HiLowBarItem" /> class for the newly created bar curve.
    ///   This can then be used to access all of the curve properties that
    ///   are not defined as arguments to the
    ///   <see cref="AddHiLowBar(string,double[],double[],double[],Color)" /> method.
    /// </returns>
    public HiLowBarItem AddHiLowBar(string label, double[] x, double[] y,
                                    double[] baseVal, Color color, int zOrder = -1)
    {
      var curve = new HiLowBarItem(label, x, y, baseVal, color, zOrder);
      CurveList.Add(curve);
      return curve;
    }

    /// <summary>
    ///   Add a hi-low bar type curve (<see cref="CurveItem" /> object) to the plot with
    ///   the given data points (<see cref="IPointList" />) and properties.
    ///   This is simplified way to add curves without knowledge of the
    ///   <see cref="CurveList" /> class.  An alternative is to use
    ///   the <see cref="ZedGraph.CurveList" /> Add() method.
    /// </summary>
    /// <param name="label">
    ///   The text label (string) for the curve that will be
    ///   used as a <see cref="Legend" /> entry.
    /// </param>
    /// <param name="points">
    ///   A <see cref="IPointList" /> of double precision value Trio's that define
    ///   the X, Y, and lower dependent values for this curve
    /// </param>
    /// <param name="color">The color to used to fill the bars</param>
    /// <returns>
    ///   A <see cref="HiLowBarItem" /> class for the newly created bar curve.
    ///   This can then be used to access all of the curve properties that
    ///   are not defined as arguments to the
    ///   <see cref="AddHiLowBar(string,IPointList,Color)" /> method.
    /// </returns>
    public HiLowBarItem AddHiLowBar(string label, IPointList points, Color color,
                                    int zOrder = -1)
    {
      var curve = new HiLowBarItem(label, points, color, zOrder);
      CurveList.Add(curve);
      return curve;
    }

    /// <summary>
    ///   Add a japanesecandlestick graph (<see cref="JapaneseCandleStickItem" /> object) to the plot with
    ///   the given data points (<see cref="IPointList" />) and properties.
    /// </summary>
    /// <remarks>
    ///   This is simplified way to add curves without knowledge of the
    ///   <see cref="CurveList" /> class.  An alternative is to use
    ///   the <see cref="ZedGraph.CurveList" /> Add() method.
    ///   Note that the <see cref="IPointList" />
    ///   should contain <see cref="StockPt" /> objects instead of <see cref="PointPair" />
    ///   objects in order to contain all the data values required for this curve type.
    /// </remarks>
    /// <param name="label">
    ///   The text label (string) for the curve that will be
    ///   used as a <see cref="Legend" /> entry.
    /// </param>
    /// <param name="points">
    ///   A <see cref="IPointList" /> of double precision value pairs that define
    ///   the X and Y values for this curve
    /// </param>
    /// <returns>
    ///   A <see cref="CurveItem" /> class for the newly created curve.
    ///   This can then be used to access all of the curve properties that
    ///   are not defined as arguments to the
    ///   <see cref="AddJapaneseCandleStick(string,IPointList)" /> method.
    /// </returns>
    public JapaneseCandleStickItem AddJapaneseCandleStick(string label, IPointList points,
                                                          int zOrder = -1)
    {
      var curve = new JapaneseCandleStickItem(label, points, zOrder);
      CurveList.Add(curve);
      return curve;
    }

    /// <summary>
    ///   Add a candlestick graph (<see cref="OHLCBarItem" /> object) to the plot with
    ///   the given data points (<see cref="IPointList" />) and properties.
    /// </summary>
    /// <remarks>
    ///   This is simplified way to add curves without knowledge of the
    ///   <see cref="CurveList" /> class.  An alternative is to use
    ///   the <see cref="ZedGraph.CurveList" /> Add() method.
    ///   Note that the <see cref="IPointList" />
    ///   should contain <see cref="StockPt" /> objects instead of <see cref="PointPair" />
    ///   objects in order to contain all the data values required for this curve type.
    /// </remarks>
    /// <param name="label">
    ///   The text label (string) for the curve that will be
    ///   used as a <see cref="Legend" /> entry.
    /// </param>
    /// <param name="points">
    ///   A <see cref="IPointList" /> of double precision value pairs that define
    ///   the X and Y values for this curve
    /// </param>
    /// <param name="color">
    ///   The color to used for the curve line,
    ///   symbols, etc.
    /// </param>
    /// <returns>
    ///   A <see cref="CurveItem" /> class for the newly created curve.
    ///   This can then be used to access all of the curve properties that
    ///   are not defined as arguments to the
    ///   <see cref="AddOHLCBar(string,IPointList,Color)" /> method.
    /// </returns>
    public OHLCBarItem AddOHLCBar(string label, IPointList points, Color color,
                                  int zOrder = -1)
    {
      var curve = new OHLCBarItem(label, points, color, zOrder);
      CurveList.Add(curve);
      return curve;
    }

    /// <summary>
    ///   Add a candlestick cluster graph (<see cref="OHLCBarClusterItem" /> object)
    ///   to the plot with the given data points (<see cref="IPointList" />) and properties.
    /// </summary>
    /// <remarks>
    ///   This is simplified way to add curves without knowledge of the
    ///   <see cref="CurveList" /> class.  An alternative is to use
    ///   the <see cref="ZedGraph.CurveList" /> Add() method.
    ///   Note that the <see cref="IPointList" />
    ///   should contain <see cref="StockPt" /> objects instead of <see cref="PointPair" />
    ///   objects in order to contain all the data values required for this curve type.
    /// </remarks>
    /// <param name="label">
    ///   The text label (string) for the curve that will be
    ///   used as a <see cref="Legend" /> entry.
    /// </param>
    /// <param name="points">
    ///   A <see cref="IPointList" /> of double precision value pairs that define
    ///   the X and Y values for this curve
    /// </param>
    /// <param name="color">
    ///   The color to used for the curve line,
    ///   symbols, etc.
    /// </param>
    /// <returns>
    ///   A <see cref="CurveItem" /> class for the newly created curve.
    ///   This can then be used to access all of the curve properties that
    ///   are not defined as arguments to the
    ///   <see cref="AddOHLCBar(string,IPointList,Color)" /> method.
    /// </returns>
    public OHLCBarClusterItem AddOHLCBarCluster(string label, IPointList points, int zOrder = -1)
    {
      var curve = new OHLCBarClusterItem(label, points, zOrder);
      CurveList.Add(curve);
      return curve;
    }

    /// <summary>
    ///   Add a <see cref="PieItem" /> to the display.
    /// </summary>
    /// <param name="value">The value associated with this <see cref="PieItem" />item.</param>
    /// <param name="color">The display color for this <see cref="PieItem" />item.</param>
    /// <param name="displacement">
    ///   The amount this <see cref="PieItem" />item will be
    ///   displaced from the center of the <see cref="PieItem" />.
    /// </param>
    /// <param name="label">Text label for this <see cref="PieItem" /></param>
    /// <returns>a reference to the <see cref="PieItem" /> constructed</returns>
    public PieItem AddPieSlice(double value, Color color, double displacement, string label)
    {
      var slice = new PieItem(value, color, displacement, label);
      CurveList.Add(slice);
      return slice;
    }

    /// <summary>
    ///   Add a <see cref="PieItem" /> to the display, providing a gradient fill for the pie color.
    /// </summary>
    /// <param name="value">The value associated with this <see cref="PieItem" /> instance.</param>
    /// <param name="color1">
    ///   The starting display color for the gradient <see cref="Fill" /> for this
    ///   <see cref="PieItem" /> instance.
    /// </param>
    /// <param name="color2">
    ///   The ending display color for the gradient <see cref="Fill" /> for this
    ///   <see cref="PieItem" /> instance.
    /// </param>
    /// <param name="fillAngle">The angle for the gradient <see cref="Fill" />.</param>
    /// <param name="displacement">
    ///   The amount this <see cref="PieItem" />  instance will be
    ///   displaced from the center point.
    /// </param>
    /// <param name="label">Text label for this <see cref="PieItem" /> instance.</param>
    public PieItem AddPieSlice(double value, Color color1, Color color2, float fillAngle,
                               double displacement, string label)
    {
      var slice = new PieItem(value, color1, color2, fillAngle, displacement, label);
      CurveList.Add(slice);
      return slice;
    }

    /// <summary>
    ///   Creates all the <see cref="PieItem" />s for a single Pie Chart.
    /// </summary>
    /// <param name="values">
    ///   double array containing all <see cref="PieItem.Value" />s
    ///   for a single PieChart.
    /// </param>
    /// <param name="labels">
    ///   string array containing all <see cref="CurveItem.Label" />s
    ///   for a single PieChart.
    /// </param>
    /// <returns>
    ///   an array containing references to all <see cref="PieItem" />s comprising
    ///   the Pie Chart.
    /// </returns>
    public PieItem[] AddPieSlices(double[] values, string[] labels)
    {
      var slices = new PieItem[values.Length];
      for (var x = 0; x < values.Length; x++)
      {
        slices[x] = new PieItem(values[x], labels[x]);
        CurveList.Add(slices[x]);
      }
      return slices;
    }

    /// <summary>
    ///   Add a stick graph (<see cref="StickItem" /> object) to the plot with
    ///   the given data points (double arrays) and properties.
    ///   This is simplified way to add curves without knowledge of the
    ///   <see cref="CurveList" /> class.  An alternative is to use
    ///   the <see cref="ZedGraph.CurveList" /> Add() method.
    /// </summary>
    /// <param name="label">
    ///   The text label (string) for the curve that will be
    ///   used as a <see cref="Legend" /> entry.
    /// </param>
    /// <param name="x">
    ///   An array of double precision X values (the
    ///   independent values) that define the curve.
    /// </param>
    /// <param name="y">
    ///   An array of double precision Y values (the
    ///   dependent values) that define the curve.
    /// </param>
    /// <param name="color">
    ///   The color to used for the curve line,
    ///   symbols, etc.
    /// </param>
    /// <returns>
    ///   A <see cref="StickItem" /> class for the newly created curve.
    ///   This can then be used to access all of the curve properties that
    ///   are not defined as arguments to the
    ///   <see cref="AddStick(string,double[],double[],Color)" /> method.
    /// </returns>
    public StickItem AddStick(string label, double[] x, double[] y, Color color,
                              int zOrder = -1)
    {
      var curve = new StickItem(label, x, y, color, zOrder);
      CurveList.Add(curve);

      return curve;
    }

    /// <summary>
    ///   Add a stick graph (<see cref="StickItem" /> object) to the plot with
    ///   the given data points (<see cref="IPointList" />) and properties.
    ///   This is simplified way to add curves without knowledge of the
    ///   <see cref="CurveList" /> class.  An alternative is to use
    ///   the <see cref="ZedGraph.CurveList" /> Add() method.
    /// </summary>
    /// <param name="label">
    ///   The text label (string) for the curve that will be
    ///   used as a <see cref="Legend" /> entry.
    /// </param>
    /// <param name="points">
    ///   A <see cref="IPointList" /> of double precision value pairs that define
    ///   the X and Y values for this curve
    /// </param>
    /// <param name="color">
    ///   The color to used for the curve line,
    ///   symbols, etc.
    /// </param>
    /// <returns>
    ///   A <see cref="CurveItem" /> class for the newly created curve.
    ///   This can then be used to access all of the curve properties that
    ///   are not defined as arguments to the
    ///   <see cref="AddStick(string,IPointList,Color)" /> method.
    /// </returns>
    public StickItem AddStick(string label, IPointList points, Color color, int zOrder = -1)
    {
      var curve = new StickItem(label, points, color, zOrder);
      CurveList.Add(curve);

      return curve;
    }

    #endregion

    #region General Utility Methods

    /// <summary>
    ///   Add a secondary <see cref="Y2Axis" /> (right side) to the list of axes
    ///   in the Graph.
    /// </summary>
    /// <remarks>
    ///   Note that the primary <see cref="Y2Axis" /> is always included by default.
    ///   This method turns off the <see cref="MajorTic" /> and <see cref="MinorTic" />
    ///   <see cref="MinorTic.IsOpposite" /> and <see cref="MinorTic.IsInside" />
    ///   properties by default.
    /// </remarks>
    /// <param name="title">The title for the <see cref="Y2Axis" />.</param>
    /// <returns>the ordinal position (index) in the <see cref="Y2AxisList" />.</returns>
    public int AddY2Axis(string title)
    {
      var axis = new Y2Axis(title);
      axis.MajorTic.IsOpposite = false;
      axis.MinorTic.IsOpposite = false;
      axis.MajorTic.IsInside = false;
      axis.MinorTic.IsInside = false;
      Y2AxisList.Add(axis);
      return Y2AxisList.Count - 1;
    }

    /// <summary>
    ///   Add a secondary <see cref="YAxis" /> (left side) to the list of axes
    ///   in the Graph.
    /// </summary>
    /// <remarks>
    ///   Note that the primary <see cref="YAxis" /> is always included by default.
    ///   This method turns off the <see cref="MajorTic" /> and <see cref="MinorTic" />
    ///   <see cref="MinorTic.IsOpposite" /> and <see cref="MinorTic.IsInside" />
    ///   properties by default.
    /// </remarks>
    /// <param name="title">The title for the <see cref="YAxis" />.</param>
    /// <returns>the ordinal position (index) in the <see cref="YAxisList" />.</returns>
    public int AddYAxis(string title)
    {
      var axis = new YAxis(title);
      axis.MajorTic.IsOpposite = false;
      axis.MinorTic.IsOpposite = false;
      axis.MajorTic.IsInside = false;
      axis.MinorTic.IsInside = false;
      YAxisList.Add(axis);
      return YAxisList.Count - 1;
    }

    /// <summary>
    ///   Find the axis that lies at the specified mouse (screen) point.
    /// </summary>
    /// <seealso cref="FindNearestObject" />
    public bool FindAxis(PointF mousePt, Graphics g, out Axis nearestObj, out int index,
                         out RectangleF rect)
    {
      nearestObj = null;
      index = -1;

      // Make sure that the axes & data are being drawn
      if (!AxisRangesValid())
      {
        rect = new RectangleF();
        return false;
      }

      object obj;
      var res = findAxis(mousePt, g, out obj, out index, out rect, CalcScaleFactor(),
                         ZOrder.H_BehindAll);

      if (res)
        nearestObj = (Axis)obj;

      return res;
    }

    // Revision: JCarpenter 10/06
    /// <summary>
    ///   Find any objects that exist within the specified (screen) rectangle.
    ///   This method will search through all of the graph objects, such as
    ///   <see cref="Axis" />, <see cref="Legend" />, <see cref="PaneBase.Title" />,
    ///   <see cref="GraphObj" />, and <see cref="CurveItem" />.
    ///   and see if the objects' bounding boxes are within the specified (screen) rectangle
    ///   This method returns true if any are found.
    /// </summary>
    public bool FindContainedObjects(RectangleF rectF, Graphics g,
                                     out CurveList containedObjs)
    {
      containedObjs = new CurveList();

      foreach (var ci in CurveList)
        for (var i = 0; i < ci.Points.Count; i++)
          if ((ci.Points[i].X > rectF.Left) &&
              (ci.Points[i].X < rectF.Right) &&
              (ci.Points[i].Y > rectF.Bottom) &&
              (ci.Points[i].Y < rectF.Top))
            containedObjs.Add(ci);
      return containedObjs.Count > 0;
    }

    /// <summary>
    ///   Search through the <see cref="GraphObjList" /> and <see cref="CurveList" /> for
    ///   items that contain active <see cref="Link" /> objects.
    /// </summary>
    /// <param name="mousePt">The mouse location where the click occurred</param>
    /// <param name="g">An appropriate <see cref="Graphics" /> instance</param>
    /// <param name="scaleFactor">The current scaling factor for drawing operations.</param>
    /// <param name="source">
    ///   The clickable object that was found.  Typically a type of
    ///   <see cref="GraphObj" /> or a type of <see cref="CurveItem" />.
    /// </param>
    /// <param name="link">
    ///   The <see cref="Link" /> instance that is contained within
    ///   the <see paramref="source" /> object.
    /// </param>
    /// <param name="index">
    ///   An index value, indicating which point was clicked for
    ///   <see cref="CurveItem" /> type objects.
    /// </param>
    /// <returns>
    ///   returns true if a clickable link was found under the
    ///   <see paramref="mousePt" />, or false otherwise.
    /// </returns>
    public bool FindLinkableObject(PointF mousePt, Graphics g, float scaleFactor,
                                   out object source, out Link link, out int index)
    {
      index = -1;

      // First look for graph objects that lie in front of the data points
      foreach (var graphObj in GraphObjList)
      {
        link = graphObj._link;

        if (!link.IsActive) continue;
        if (!graphObj.PointInBox(mousePt, this, g, scaleFactor)) continue;

        source = graphObj;
        return true;
      }

      // Second, look at the curve data points
      foreach (var curve in CurveList)
      {
        link = curve.Link;

        if (!link.IsActive) continue;
        CurveItem nearestCurve;

        if (!FindNearestPoint(mousePt, curve, out nearestCurve, out index)) continue;

        source = curve;
        return true;
      }

      // Third, look for graph objects that lie behind the data points
      foreach (var graphObj in GraphObjList)
      {
        link = graphObj._link;

        if (!link.IsActive) continue;
        if (!graphObj.PointInBox(mousePt, this, g, scaleFactor)) continue;

        source = graphObj;
        return true;
      }

      source = null;
      link = null;
      index = -1;
      return false;
    }

    /// <summary>
    ///   Find the object that lies closest to the specified mouse (screen) point.
    /// </summary>
    /// <remarks>
    ///   This method will search through all of the graph objects, such as
    ///   <see cref="Axis" />, <see cref="Legend" />, <see cref="PaneBase.Title" />,
    ///   <see cref="GraphObj" />, and <see cref="CurveItem" />.
    ///   If the mouse point is within the bounding box of the items (or in the case
    ///   of <see cref="ArrowObj" /> and <see cref="CurveItem" />, within
    ///   <see cref="Default.NearestTol" /> pixels), then the object will be returned.
    ///   You must check the type of the object to determine what object was
    ///   selected (for example, "if ( object is Legend ) ...").  The
    ///   <see paramref="index" /> parameter returns the index number of the item
    ///   within the selected object (such as the point number within a
    ///   <see cref="CurveItem" /> object.
    /// </remarks>
    /// <param name="mousePt">The screen point, in pixel coordinates.</param>
    /// <param name="g">
    ///   A graphic device object to be drawn into.  This is normally e.Graphics from the
    ///   PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="nearestObj">
    ///   A reference to the nearest object to the
    ///   specified screen point.  This can be any of <see cref="Axis" />,
    ///   <see cref="Legend" />, <see cref="PaneBase.Title" />,
    ///   <see cref="TextObj" />, <see cref="ArrowObj" />, or <see cref="CurveItem" />.
    ///   Note: If the pane title is selected, then the <see cref="GraphPane" /> object
    ///   will be returned.
    /// </param>
    /// <param name="index">
    ///   The index number of the item within the selected object
    ///   (where applicable).  For example, for a <see cref="CurveItem" /> object,
    ///   <see paramref="index" /> will be the index number of the nearest data point,
    ///   accessible via <see cref="CurveItem.Points">CurveItem.Points[index]</see>.
    ///   index will be -1 if no data points are available.
    /// </param>
    /// <returns>true if an object was found, false otherwise.</returns>
    /// <seealso cref="FindNearestObject" />
    public override bool FindNearestObject(PointF mousePt, Graphics g,
                                           out object nearestObj, out int index)
    {
      nearestObj = null;
      index = -1;

      // Make sure that the axes & data are being drawn
      if (AxisRangesValid())
      {
        var scaleFactor = CalcScaleFactor();
        //int      hStack;
        //float    legendWidth, legendHeight;
        RectangleF tmpRect;
        GraphObj saveGraphItem = null;
        var saveIndex = -1;
        var saveZOrder = ZOrder.H_BehindAll;

        // Calculate the chart rect, deducting the area for the scales, titles, legend, etc.
        var tmpChartRect = CalcChartRect(g, scaleFactor);

        // See if the point is in a GraphObj
        // If so, just save the object and index so we can see if other overlying objects were
        // intersected as well.
        if (GraphObjList.FindPoint(mousePt, this, g, scaleFactor, out index))
        {
          saveGraphItem = GraphObjList[index];
          saveIndex = index;
          saveZOrder = saveGraphItem.ZOrder;
        }

        // See if the point is in the legend
        if ((saveZOrder <= ZOrder.B_BehindLegend) &&
            Legend.FindPoint(mousePt, this, scaleFactor, out index))
        {
          nearestObj = Legend;
          return true;
        }

        // See if the point is in the Pane Title
        var paneTitleBox = _title.FontSpec.BoundingBox(g, _title.Text, scaleFactor);
        if ((saveZOrder <= ZOrder.H_BehindAll) && _title.IsVisible)
        {
          tmpRect = new RectangleF((_rect.Left + _rect.Right - paneTitleBox.Width)/2,
                                   _rect.Top + Margin.Top*scaleFactor,
                                   paneTitleBox.Width, paneTitleBox.Height);
          if (tmpRect.Contains(mousePt))
          {
            nearestObj = this;
            return true;
          }
        }

        RectangleF dummy;

        // See if mouse point lies on one of the axises
        if (findAxis(mousePt, g, out nearestObj, out index, out dummy, scaleFactor,
                     saveZOrder))
          return true;

        CurveItem curve;
        // See if it's a data point
        if ((saveZOrder <= ZOrder.E_BehindCurves) &&
            FindNearestPoint(mousePt, out curve, out index))
        {
          nearestObj = curve;
          return true;
        }

        if (saveGraphItem == null) return false;

        index = saveIndex;
        nearestObj = saveGraphItem;
        return true;
      }

      return false;
    }

    /// <summary>
    ///   Find the data point that lies closest to the specified mouse (screen)
    ///   point for the specified curve.
    /// </summary>
    /// <remarks>
    ///   This method will search only through the points for the specified
    ///   curve to determine which point is
    ///   nearest the mouse point.  It will only consider points that are within
    ///   <see cref="Default.NearestTol" /> pixels of the screen point.
    /// </remarks>
    /// <param name="mousePt">The screen point, in pixel coordinates.</param>
    /// <param name="nearestCurve">
    ///   A reference to the <see cref="CurveItem" />
    ///   instance that contains the closest point.  nearestCurve will be null if
    ///   no data points are available.
    /// </param>
    /// <param name="targetCurve">
    ///   A <see cref="CurveItem" /> object containing
    ///   the data points to be searched.
    /// </param>
    /// <param name="iNearest">
    ///   The index number of the closest point.  The
    ///   actual data vpoint will then be <see cref="CurveItem.Points">CurveItem.Points[iNearest]</see>
    ///   .  iNearest will
    ///   be -1 if no data points are available.
    /// </param>
    /// <returns>
    ///   true if a point was found and that point lies within
    ///   <see cref="Default.NearestTol" /> pixels
    ///   of the screen point, false otherwise.
    /// </returns>
    public bool FindNearestPoint(PointF mousePt, CurveItem targetCurve,
                                 out CurveItem nearestCurve, out int iNearest)
    {
      var targetCurveList = new CurveList();
      targetCurveList.Add(targetCurve);
      return FindNearestPoint(mousePt, targetCurveList,
                              out nearestCurve, out iNearest);
    }

    /// <summary>
    ///   Find the data point that lies closest to the specified mouse (screen)
    ///   point.
    /// </summary>
    /// <remarks>
    ///   This method will search through all curves in
    ///   <see cref="GraphPane.CurveList" /> to find which point is
    ///   nearest.  It will only consider points that are within
    ///   <see cref="Default.NearestTol" /> pixels of the screen point.
    /// </remarks>
    /// <param name="mousePt">The screen point, in pixel coordinates.</param>
    /// <param name="nearestCurve">
    ///   A reference to the <see cref="CurveItem" />
    ///   instance that contains the closest point.  nearestCurve will be null if
    ///   no data points are available.
    /// </param>
    /// <param name="iNearest">
    ///   The index number of the closest point.  The
    ///   actual data vpoint will then be <see cref="CurveItem.Points">CurveItem.Points[iNearest]</see>
    ///   .  iNearest will
    ///   be -1 if no data points are available.
    /// </param>
    /// <returns>
    ///   true if a point was found and that point lies within
    ///   <see cref="Default.NearestTol" /> pixels
    ///   of the screen point, false otherwise.
    /// </returns>
    public bool FindNearestPoint(PointF mousePt,
                                 out CurveItem nearestCurve, out int iNearest)
    {
      return FindNearestPoint(mousePt, CurveList,
                              out nearestCurve, out iNearest);
    }

    /// <summary>
    ///   Find the data point that lies closest to the specified mouse (screen)
    ///   point.
    /// </summary>
    /// <remarks>
    ///   This method will search through the specified list of curves to find which point is
    ///   nearest.  It will only consider points that are within
    ///   <see cref="Default.NearestTol" /> pixels of the screen point, and it will
    ///   only consider <see cref="CurveItem" />'s that are in
    ///   <paramref name="targetCurveList" />.
    /// </remarks>
    /// <param name="mousePt">The screen point, in pixel coordinates.</param>
    /// <param name="targetCurveList">
    ///   A <see cref="CurveList" /> object containing
    ///   a subset of <see cref="CurveItem" />'s to be searched.
    /// </param>
    /// <param name="nearestCurve">
    ///   A reference to the <see cref="CurveItem" />
    ///   instance that contains the closest point.  nearestCurve will be null if
    ///   no data points are available.
    /// </param>
    /// <param name="iNearest">
    ///   The index number of the closest point.  The
    ///   actual data vpoint will then be <see cref="CurveItem.Points">CurveItem.Points[iNearest]</see>
    ///   .  iNearest will
    ///   be -1 if no data points are available.
    /// </param>
    /// <returns>
    ///   true if a point was found and that point lies within
    ///   <see cref="Default.NearestTol" /> pixels
    ///   of the screen point, false otherwise.
    /// </returns>
    public bool FindNearestPoint(PointF mousePt, CurveList targetCurveList,
                                 out CurveItem nearestCurve, out int iNearest)
    {
      CurveItem nearestBar = null;
      var iNearestBar = -1;
      nearestCurve = null;
      iNearest = -1;

      // If the point is outside the ChartRect, always return false
      if (!_chart._rect.Contains(mousePt))
        return false;

      double x, x2;
      double[] y;
      double[] y2;

      //ReverseTransform( mousePt, out x, out y, out y2 );
      ReverseTransform(mousePt, out x, out x2, out y, out y2);

      if (!AxisRangesValid())
        return false;

      var valueHandler = new ValueHandler(this, false);

      //double  yPixPerUnit = chartRect.Height / ( yAxis.Max - yAxis.Min );
      //double  y2PixPerUnit; // = chartRect.Height / ( y2Axis.Max - y2Axis.Min );

      double yPixPerUnitAct, yAct, yMinAct, yMaxAct, xAct;
      var minDist = 1e20;
      double xVal, yVal, dist = 99999, distX, distY;
      var tolSquared = Default.NearestTol*Default.NearestTol;

      var iBar = 0;

      foreach (var curve in targetCurveList)
      {
        //test for pie first...if it's a pie rest of method superfluous
        if (curve is PieItem && curve.IsVisible)
        {
          if ((((PieItem)curve).SlicePath != null) &&
              ((PieItem)curve).SlicePath.IsVisible(mousePt))
          {
            nearestBar = curve;
            iNearestBar = 0;
          }

          continue;
        }
        if (!curve.IsVisible) continue;

        var yIndex = curve.GetYAxisIndex(this);
        var yAxis = curve.GetYAxis(this);
        var xAxis = curve.GetXAxis(this);

        if (curve.IsY2Axis)
        {
          yAct = y2[yIndex];
          yMinAct = Y2AxisList[yIndex].Scale._min;
          yMaxAct = Y2AxisList[yIndex].Scale._max;
        }
        else
        {
          yAct = y[yIndex];
          yMinAct = YAxisList[yIndex].Scale._min;
          yMaxAct = YAxisList[yIndex].Scale._max;
        }

        yPixPerUnitAct = _chart._rect.Height/(yMaxAct - yMinAct);
        var xPixPerUnit = _chart._rect.Width/(xAxis.Scale._max - xAxis.Scale._min);
        xAct = xAxis is XAxis ? x : x2;

        var points = curve.Points;
        var barWidth = curve.GetBarWidth(this);
        var baseAxis = curve.BaseAxis(this);
        var isXBaseAxis = baseAxis is XAxis || baseAxis is X2Axis;
        var barWidthUserHalf = isXBaseAxis
                                 ? barWidth/xPixPerUnit/2.0
                                 : barWidth/yPixPerUnitAct/2.0;

        if (points == null) continue;
        for (var iPt = 0; iPt < curve.NPts; iPt++)
        {
          // xVal is the user scale X value of the current point
          xVal = xAxis.Scale.IsAnyOrdinal && !curve.IsOverrideOrdinal
                   ? iPt + 1.0
                   : points[iPt].X;

          // yVal is the user scale Y value of the current point
          yVal = yAxis.Scale.IsAnyOrdinal && !curve.IsOverrideOrdinal
                   ? iPt + 1.0
                   : points[iPt].Y;

          if ((xVal == PointPairBase.Missing) || (yVal == PointPairBase.Missing)) continue;

          if (curve.IsBar || curve is IBarItem)
          {
            double baseVal, lowVal, hiVal;
            valueHandler.GetValues(curve, iPt, out baseVal,
                                   out lowVal, out hiVal);

            if (lowVal > hiVal)
            {
              var tmpVal = lowVal;
              lowVal = hiVal;
              hiVal = tmpVal;
            }

            if (isXBaseAxis)
            {
              var centerVal = valueHandler.BarCenterValue(curve, barWidth, iPt, xVal, iBar);

              if ((xAct < centerVal - barWidthUserHalf) ||
                  (xAct > centerVal + barWidthUserHalf) ||
                  (yAct < lowVal) || (yAct > hiVal))
                continue;
            }
            else
            {
              var centerVal = valueHandler.BarCenterValue(curve, barWidth, iPt, yVal, iBar);

              if ((yAct < centerVal - barWidthUserHalf) ||
                  (yAct > centerVal + barWidthUserHalf) ||
                  (xAct < lowVal) || (xAct > hiVal))
                continue;
            }

            if (nearestBar == null)
            {
              iNearestBar = iPt;
              nearestBar = curve;
            }
          }
          else if ((xVal >= xAxis.Scale._min) && (xVal <= xAxis.Scale._max) &&
                   (yVal >= yMinAct) && (yVal <= yMaxAct))
          {
            if (curve is LineItem && (LineType == LineType.Stack))
            {
              double zVal;
              valueHandler.GetValues(curve, iPt, out xVal, out zVal, out yVal);
            }

            distX = (xVal - xAct)*xPixPerUnit;
            distY = (yVal - yAct)*yPixPerUnitAct;
            dist = distX*distX + distY*distY;

            if (dist >= minDist)
              continue;

            minDist = dist;
            iNearest = iPt;
            nearestCurve = curve;
          }
        }

        if (curve.IsBar)
          iBar++;
      }

      if (nearestCurve is LineItem)
      {
        var halfSymbol = ((LineItem)nearestCurve).Symbol.Size*CalcScaleFactor()/2;
        minDist -= halfSymbol*halfSymbol;
        if (minDist < 0)
          minDist = 0;
      }

      if ((minDist >= tolSquared) && (nearestBar != null))
      {
        // if no point met the tolerance, but a bar was found, use it
        nearestCurve = nearestBar;
        iNearest = iNearestBar;
        return true;
      }
      if (minDist < tolSquared)
        return true;
      return false;
    }

    /// <summary>
    ///   Transform a data point from screen coordinates (pixels) to
    ///   the specified coordinate type  (<see cref="CoordType" />).
    /// </summary>
    /// <remarks>
    ///   This method implicitly assumes that <see cref="ZedGraph.Chart.Rect" />
    ///   has already been calculated via <see cref="AxisChange()" /> or
    ///   <see cref="Draw" /> methods, or the <see cref="ZedGraph.Chart.Rect" /> is
    ///   set manually (see <see cref="ZedGraph.Chart.IsRectAuto" />).
    /// </remarks>
    /// <param name="ptF">
    ///   The X,Y pair that defines the point in
    ///   screen.
    /// </param>
    /// <param name="coord">
    ///   A <see cref="CoordType" /> type that defines the
    ///   coordinate system in which the output pair is defined.
    /// </param>
    /// <returns>
    ///   A point in user coordinates that corresponds to the
    ///   specified screen point.
    /// </returns>
    public PointD GeneralReverseTransform(PointF ptF, CoordType coord, int yAxisIndex = 0)
    {
      // Setup the scaling data based on the chart rect
      XAxis.Scale.SetupScaleData(this);
      if (yAxisIndex < YAxisList.Count)
        YAxisList[yAxisIndex].Scale.SetupScaleData(this);
      if (yAxisIndex < Y2AxisList.Count)
        Y2AxisList[yAxisIndex].Scale.SetupScaleData(this);
      /*
      foreach ( Axis axis in _yAxisList )
        axis.Scale.SetupScaleData( this, axis );
      foreach ( Axis axis in _y2AxisList )
        axis.Scale.SetupScaleData( this, axis );
      */

      return ReverseTransformCoord(ptF.X, ptF.Y, coord, yAxisIndex);
    }

    /// <summary>
    ///   Transform a data point from screen coordinates (pixels) to
    ///   the specified coordinate type  (<see cref="CoordType" />).
    /// </summary>
    /// <remarks>
    ///   This method implicitly assumes that <see cref="ZedGraph.Chart.Rect" />
    ///   has already been calculated via <see cref="AxisChange()" /> or
    ///   <see cref="Draw" /> methods, or the <see cref="ZedGraph.Chart.Rect" /> is
    ///   set manually (see <see cref="ZedGraph.Chart.IsRectAuto" />).
    ///   Note that this method is more accurate than the <see cref="GeneralReverseTransform(PointF, CoordType)" />
    ///   overload, since it uses double types.  This would typically only be significant for
    ///   <see cref="AxisType.Date" /> coordinates.
    /// </remarks>
    /// <param name="x">The x coordinate that defines the location in screen</param>
    /// <param name="y">The y coordinate that defines the location in screen</param>
    /// <param name="coord">
    ///   A <see cref="CoordType" /> type that defines the
    ///   coordinate system in which the output pair is defined.
    /// </param>
    /// <returns>
    ///   A point in user coordinates that corresponds to the
    ///   specified screen point.
    /// </returns>
    public PointD GeneralReverseTransform(float x, float y, CoordType coord,
                                          int yAxisIndex = 0)
    {
      // Setup the scaling data based on the chart rect
      XAxis.Scale.SetupScaleData(this);
      if (yAxisIndex < YAxisList.Count)
        YAxisList[yAxisIndex].Scale.SetupScaleData(this);
      if (yAxisIndex < Y2AxisList.Count)
        Y2AxisList[yAxisIndex].Scale.SetupScaleData(this);
      /*
      foreach ( Axis axis in _yAxisList )
        axis.Scale.SetupScaleData( this, axis );
      foreach ( Axis axis in _y2AxisList )
        axis.Scale.SetupScaleData( this, axis );
      */

      return ReverseTransformCoord(x, y, coord);
    }

    /// <summary>
    ///   Transform a data point from the specified coordinate type
    ///   (<see cref="CoordType" />) to screen coordinates (pixels).
    /// </summary>
    /// <remarks>
    ///   This method implicitly assumes that <see cref="ZedGraph.Chart.Rect" />
    ///   has already been calculated via <see cref="AxisChange()" /> or
    ///   <see cref="Draw" /> methods, or the <see cref="ZedGraph.Chart.Rect" /> is
    ///   set manually (see <see cref="ZedGraph.Chart.IsRectAuto" />).
    /// </remarks>
    /// <param name="ptF">
    ///   The X,Y pair that defines the point in user
    ///   coordinates.
    /// </param>
    /// <param name="coord">
    ///   A <see cref="CoordType" /> type that defines the
    ///   coordinate system in which the X,Y pair is defined.
    /// </param>
    /// <returns>
    ///   A point in screen coordinates that corresponds to the
    ///   specified user point.
    /// </returns>
    public PointF GeneralTransform(PointF ptF, CoordType coord, int yAxisIndex = 0)
    {
      // Setup the scaling data based on the chart rect
      XAxis.Scale.SetupScaleData(this);
      if (yAxisIndex < YAxisList.Count)
        YAxisList[yAxisIndex].Scale.SetupScaleData(this);
      if (yAxisIndex < Y2AxisList.Count)
        Y2AxisList[yAxisIndex].Scale.SetupScaleData(this);
      /*
      foreach ( Axis axis in _yAxisList )
        axis.Scale.SetupScaleData( this, axis );
      foreach ( Axis axis in _y2AxisList )
        axis.Scale.SetupScaleData( this, axis );
      */
      return TransformCoord(ptF.X, ptF.Y, coord, yAxisIndex);
    }

    /// <summary>
    ///   Transform a data point from the specified coordinate type
    ///   (<see cref="CoordType" />) to screen coordinates (pixels).
    /// </summary>
    /// <remarks>
    ///   This method implicitly assumes that <see cref="ZedGraph.Chart.Rect" />
    ///   has already been calculated via <see cref="AxisChange()" /> or
    ///   <see cref="Draw" /> methods, or the <see cref="ZedGraph.Chart.Rect" /> is
    ///   set manually (see <see cref="ZedGraph.Chart.IsRectAuto" />).
    /// </remarks>
    /// <param name="pt">
    ///   The X,Y pair that defines the point in user
    ///   coordinates.
    /// </param>
    /// <param name="coord">
    ///   A <see cref="CoordType" /> type that defines the
    ///   coordinate system in which the X,Y pair is defined.
    /// </param>
    /// <returns>
    ///   A point in screen coordinates that corresponds to the
    ///   specified user point.
    /// </returns>
    public PointF GeneralTransform(PointD pt, CoordType coord, int yAxisIndex = 0)
    {
      // Setup the scaling data based on the chart rect
      XAxis.Scale.SetupScaleData(this);
      if (yAxisIndex < YAxisList.Count)
        YAxisList[yAxisIndex].Scale.SetupScaleData(this);
      if (yAxisIndex < Y2AxisList.Count)
        Y2AxisList[yAxisIndex].Scale.SetupScaleData(this);
      /*
      foreach (Axis axis in _yAxisList)
        axis.Scale.SetupScaleData(this, axis);
      foreach (Axis axis in _y2AxisList)
        axis.Scale.SetupScaleData(this, axis);
      */
      return TransformCoord(pt.X, pt.Y, coord, yAxisIndex);
    }

    /// <summary>
    ///   Transform a data point from the specified coordinate type
    ///   (<see cref="CoordType" />) to screen coordinates (pixels).
    /// </summary>
    /// <remarks>
    ///   This method implicitly assumes that <see cref="ZedGraph.Chart.Rect" />
    ///   has already been calculated via <see cref="AxisChange()" /> or
    ///   <see cref="Draw" /> methods, or the <see cref="ZedGraph.Chart.Rect" /> is
    ///   set manually (see <see cref="ZedGraph.Chart.IsRectAuto" />).
    ///   Note that this method is more accurate than the <see cref="GeneralTransform(PointF,CoordType)" />
    ///   overload, since it uses double types.  This would typically only be significant for
    ///   <see cref="AxisType.Date" /> coordinates.
    /// </remarks>
    /// <param name="x">The x coordinate that defines the location in user space</param>
    /// <param name="y">The y coordinate that defines the location in user space</param>
    /// <param name="coord">
    ///   A <see cref="CoordType" /> type that defines the
    ///   coordinate system in which the X,Y pair is defined.
    /// </param>
    /// <returns>
    ///   A point in screen coordinates that corresponds to the
    ///   specified user point.
    /// </returns>
    public PointF GeneralTransform(double x, double y, CoordType coord, int yAxisIndex = 0)
    {
      // Setup the scaling data based on the chart rect
      XAxis.Scale.SetupScaleData(this);
      if (yAxisIndex < YAxisList.Count)
        YAxisList[yAxisIndex].Scale.SetupScaleData(this);
      if (yAxisIndex < Y2AxisList.Count)
        Y2AxisList[yAxisIndex].Scale.SetupScaleData(this);
      /*
      foreach ( Axis axis in _yAxisList )
        axis.Scale.SetupScaleData( this, axis );
      foreach ( Axis axis in _y2AxisList )
        axis.Scale.SetupScaleData( this, axis );
      */

      return TransformCoord(x, y, coord, yAxisIndex);
    }

    /// <summary>
    ///   Return the user scale values that correspond to the specified screen
    ///   coordinate position (pixels).  This overload assumes the default
    ///   <see cref="XAxis" /> and <see cref="YAxis" />.
    /// </summary>
    /// <remarks>
    ///   This method implicitly assumes that <see cref="ZedGraph.Chart.Rect" />
    ///   has already been calculated via <see cref="AxisChange()" /> or
    ///   <see cref="Draw" /> methods, or the <see cref="ZedGraph.Chart.Rect" /> is
    ///   set manually (see <see cref="ZedGraph.Chart.IsRectAuto" />).
    /// </remarks>
    /// <param name="ptF">
    ///   The X,Y pair that defines the screen coordinate
    ///   point of interest
    /// </param>
    /// <param name="x">
    ///   The resultant value in user coordinates from the
    ///   <see cref="XAxis" />
    /// </param>
    /// <param name="y">
    ///   The resultant value in user coordinates from the
    ///   primary <see cref="YAxis" />
    /// </param>
    public void ReverseTransform(PointF ptF, out double x, out double y)
    {
      // Setup the scaling data based on the chart rect
      XAxis.Scale.SetupScaleData(this);
      YAxis.Scale.SetupScaleData(this);

      x = XAxis.Scale.ReverseTransform(ptF.X);
      y = YAxis.Scale.ReverseTransform(ptF.Y);
    }

    /// <summary>
    ///   Return the user scale values that correspond to the specified screen
    ///   coordinate position (pixels).
    /// </summary>
    /// <remarks>
    ///   This method implicitly assumes that <see cref="ZedGraph.Chart.Rect" />
    ///   has already been calculated via <see cref="AxisChange()" /> or
    ///   <see cref="Draw" /> methods, or the <see cref="ZedGraph.Chart.Rect" /> is
    ///   set manually (see <see cref="ZedGraph.Chart.IsRectAuto" />).
    /// </remarks>
    /// <param name="ptF">
    ///   The X,Y pair that defines the screen coordinate
    ///   point of interest
    /// </param>
    /// <param name="x">
    ///   The resultant value in user coordinates from the
    ///   <see cref="XAxis" />
    /// </param>
    /// <param name="x2">
    ///   The resultant value in user coordinates from the
    ///   <see cref="X2Axis" />
    /// </param>
    /// <param name="y">
    ///   The resultant value in user coordinates from the
    ///   primary <see cref="YAxis" />
    /// </param>
    /// <param name="y2">
    ///   The resultant value in user coordinates from the
    ///   primary <see cref="Y2Axis" />
    /// </param>
    public void ReverseTransform(PointF ptF, out double x, out double x2, out double y,
                                 out double y2)
    {
      // Setup the scaling data based on the chart rect
      XAxis.Scale.SetupScaleData(this);
      X2Axis.Scale.SetupScaleData(this);
      YAxis.Scale.SetupScaleData(this);
      Y2Axis.Scale.SetupScaleData(this);

      x = XAxis.Scale.ReverseTransform(ptF.X);
      x2 = X2Axis.Scale.ReverseTransform(ptF.X);
      y = YAxis.Scale.ReverseTransform(ptF.Y);
      y2 = Y2Axis.Scale.ReverseTransform(ptF.Y);
    }

    /// <summary>
    ///   Return the user scale values that correspond to the specified screen
    ///   coordinate position (pixels).
    /// </summary>
    /// <remarks>
    ///   This method implicitly assumes that <see cref="ZedGraph.Chart.Rect" />
    ///   has already been calculated via <see cref="AxisChange()" /> or
    ///   <see cref="Draw" /> methods, or the <see cref="ZedGraph.Chart.Rect" /> is
    ///   set manually (see <see cref="ZedGraph.Chart.IsRectAuto" />).
    /// </remarks>
    /// <param name="ptF">
    ///   The X,Y pair that defines the screen coordinate
    ///   point of interest
    /// </param>
    /// <param name="isX2Axis">
    ///   true to return data that corresponds to an
    ///   <see cref="X2Axis" />, false for an <see cref="XAxis" />.
    /// </param>
    /// <param name="isY2Axis">
    ///   true to return data that corresponds to a
    ///   <see cref="Y2Axis" />, false for a <see cref="YAxis" />.
    /// </param>
    /// <param name="yAxisIndex">
    ///   The ordinal index of the Y or Y2 axis from which
    ///   to return data (see <seealso cref="YAxisList" />, <seealso cref="Y2AxisList" />)
    /// </param>
    /// <param name="x">
    ///   The resultant value in user coordinates from the
    ///   <see cref="XAxis" />
    /// </param>
    /// <param name="y">
    ///   The resultant value in user coordinates from the
    ///   primary <see cref="YAxis" />
    /// </param>
    public void ReverseTransform(PointF ptF, bool isX2Axis, bool isY2Axis, int yAxisIndex,
                                 out double x, out double y)
    {
      // Setup the scaling data based on the chart rect
      var xAxis = isX2Axis ? (Axis)X2Axis : XAxis;

      xAxis.Scale.SetupScaleData(this);
      x = xAxis.Scale.ReverseTransform(ptF.X);

      Axis yAxis = null;
      if (isY2Axis && (Y2AxisList.Count > yAxisIndex))
        yAxis = Y2AxisList[yAxisIndex];
      else if (!isY2Axis && (YAxisList.Count > yAxisIndex))
        yAxis = YAxisList[yAxisIndex];

      if (yAxis != null)
      {
        yAxis.Scale.SetupScaleData(this);
        y = yAxis.Scale.ReverseTransform(ptF.Y);
      }
      else
        y = PointPairBase.Missing;
    }

    /// <summary>
    ///   Return the user scale values that correspond to the specified screen
    ///   coordinate position (pixels) for all y axes.
    /// </summary>
    /// <remarks>
    ///   This method implicitly assumes that <see cref="ZedGraph.Chart.Rect" />
    ///   has already been calculated via <see cref="AxisChange()" /> or
    ///   <see cref="Draw" /> methods, or the <see cref="ZedGraph.Chart.Rect" /> is
    ///   set manually (see <see cref="ZedGraph.Chart.IsRectAuto" />).
    /// </remarks>
    /// <param name="ptF">
    ///   The X,Y pair that defines the screen coordinate
    ///   point of interest
    /// </param>
    /// <param name="x">
    ///   The resultant value in user coordinates from the
    ///   <see cref="XAxis" />
    /// </param>
    /// <param name="x2">
    ///   The resultant value in user coordinates from the
    ///   <see cref="X2Axis" />
    /// </param>
    /// <param name="y">
    ///   An array of resultant values in user coordinates from the
    ///   list of <see cref="YAxis" /> instances.  This method allocates the
    ///   array for you, according to the number of <see cref="YAxis" /> objects
    ///   in the list.
    /// </param>
    /// <param name="y2">
    ///   An array of resultant values in user coordinates from the
    ///   list of <see cref="Y2Axis" /> instances.  This method allocates the
    ///   array for you, according to the number of <see cref="Y2Axis" /> objects
    ///   in the list.
    /// </param>
    public void ReverseTransform(PointF ptF, out double x, out double x2, out double[] y,
                                 out double[] y2)
    {
      // Setup the scaling data based on the chart rect
      XAxis.Scale.SetupScaleData(this);
      x = XAxis.Scale.ReverseTransform(ptF.X);
      X2Axis.Scale.SetupScaleData(this);
      x2 = X2Axis.Scale.ReverseTransform(ptF.X);

      y = new double[YAxisList.Count];
      y2 = new double[Y2AxisList.Count];

      for (var i = 0; i < YAxisList.Count; i++)
      {
        Axis axis = YAxisList[i];
        axis.Scale.SetupScaleData(this);
        y[i] = axis.Scale.ReverseTransform(ptF.Y);
      }

      for (var i = 0; i < Y2AxisList.Count; i++)
      {
        Axis axis = Y2AxisList[i];
        axis.Scale.SetupScaleData(this);
        y2[i] = axis.Scale.ReverseTransform(ptF.Y);
      }
    }

    private bool findAxis(PointF mousePt, Graphics g, out object nearestObj, out int index,
                          out RectangleF rect,
                          float scaleFactor, ZOrder saveZOrder)
    {
      // Calculate the chart rect, deducting the area for the scales, titles, legend, etc.
      var tmpChartRect = CalcChartRect(g, scaleFactor);

      var left = tmpChartRect.Left;

      // See if the point is in one of the Y Axes
      for (var yIndex = 0; yIndex < YAxisList.Count; yIndex++)
      {
        Axis yAxis = YAxisList[yIndex];
        //var width = yAxis._tmpSpace;
        //if (width <= 0) continue;

        //rect = new RectangleF(left - width, tmpChartRect.Top,
        //                      width, tmpChartRect.Height);
        if ((saveZOrder <= ZOrder.D_BehindAxis) && yAxis.Scale.Rect.Contains(mousePt))
        {
          //yAxis.Rect = rect
          nearestObj = yAxis;
          index = yIndex;
          rect = yAxis.Scale.Rect;
          return true;
        }

        //left -= width;
      }

      left = tmpChartRect.Right;

      // See if the point is in one of the Y2 Axes
      for (var yIndex = 0; yIndex < Y2AxisList.Count; yIndex++)
      {
        var y2Axis = Y2AxisList[yIndex];
        //var width = y2Axis._tmpSpace;
        //if (!(width > 0)) continue;

        //rect = new RectangleF(left, tmpChartRect.Top,
        //                      width, tmpChartRect.Height);
        if ((saveZOrder <= ZOrder.D_BehindAxis) && y2Axis.Scale.Rect.Contains(mousePt))
        {
          //y2Axis.Rect = rect
          nearestObj = y2Axis;
          index = yIndex;
          rect = y2Axis.Scale.Rect;
          return true;
        }

        //left += width;
      }

      // See if the point is in the X Axis
      //var height = XAxis._tmpSpace;

      //rect = new RectangleF(tmpChartRect.Left, tmpChartRect.Bottom,
      //                      tmpChartRect.Width, height);
        //_rect.Bottom - tmpChartRect.Bottom );

      if ((saveZOrder <= ZOrder.D_BehindAxis) && XAxis.Scale.Rect.Contains(mousePt))
      {
        nearestObj = XAxis;
        index = 0;
        rect = XAxis.Scale.Rect;
        return true;
      }

      // See if the point is in the X2 Axis
      //height = X2Axis._tmpSpace;

      //rect = new RectangleF(tmpChartRect.Left,
      //                      tmpChartRect.Top - height,
      //                      tmpChartRect.Width,
      //                      height);
      if ((saveZOrder <= ZOrder.D_BehindAxis) && X2Axis.Scale.Rect.Contains(mousePt))
      {
        nearestObj = X2Axis;
        index = 0;
        rect = X2Axis.Scale.Rect;
        return true;
      }

      nearestObj = null;
      index = -1;
      rect = new RectangleF();
      return false;
    }

    #endregion
  }
}