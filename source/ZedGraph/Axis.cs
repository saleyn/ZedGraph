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
  ///   The Axis class is an abstract base class that encompasses all properties
  ///   and methods required to define a graph Axis.
  /// </summary>
  /// <remarks>
  ///   This class is inherited by the
  ///   <see cref="XAxis" />, <see cref="YAxis" />, and <see cref="Y2Axis" /> classes
  ///   to define specific characteristics for those types.
  /// </remarks>
  /// <author> John Champion modified by Jerry Vos </author>
  /// <version> $Revision: 3.76 $ $Date: 2008-02-16 23:21:48 $ </version>
  [Serializable]
  public abstract class Axis : ISerializable, ICloneable
  {
    #region Defaults

    /// <summary>
    ///   A simple struct that defines the
    ///   default property values for the <see cref="Axis" /> class.
    /// </summary>
    public struct Default
    {
      /// <summary>
      ///   The default size for the gap between multiple axes
      ///   (<see cref="Axis.AxisGap" /> property). Units are in points (1/72 inch).
      /// </summary>
      public static float AxisGap = 5;

      /// <summary>
      ///   The default setting for the gap between the scale labels and the axis title.
      /// </summary>
      public static float TitleGap = 0.0f;

      /// <summary>
      ///   The default font family for the <see cref="Axis" /> <see cref="Title" /> text
      ///   font specification <see cref="FontSpec" />
      ///   (<see cref="FontSpec.Family" /> property).
      /// </summary>
      public static string TitleFontFamily = "Arial";

      /// <summary>
      ///   The default font size for the <see cref="Axis" /> <see cref="Title" /> text
      ///   font specification <see cref="FontSpec" />
      ///   (<see cref="FontSpec.Size" /> property).  Units are
      ///   in points (1/72 inch).
      /// </summary>
      public static float TitleFontSize = 14;

      /// <summary>
      ///   The default font color for the <see cref="Axis" /> <see cref="Title" /> text
      ///   font specification <see cref="FontSpec" />
      ///   (<see cref="FontSpec.FontColor" /> property).
      /// </summary>
      public static Color TitleFontColor = Color.Black;

      /// <summary>
      ///   The default font bold mode for the <see cref="Axis" /> <see cref="Title" /> text
      ///   font specification <see cref="FontSpec" />
      ///   (<see cref="FontSpec.IsBold" /> property). true
      ///   for a bold typeface, false otherwise.
      /// </summary>
      public static bool TitleFontBold = true;

      /// <summary>
      ///   The default font italic mode for the <see cref="Axis" /> <see cref="Title" /> text
      ///   font specification <see cref="FontSpec" />
      ///   (<see cref="FontSpec.IsItalic" /> property). true
      ///   for an italic typeface, false otherwise.
      /// </summary>
      public static bool TitleFontItalic = false;

      /// <summary>
      ///   The default font underline mode for the <see cref="Axis" /> <see cref="Title" /> text
      ///   font specification <see cref="FontSpec" />
      ///   (<see cref="FontSpec.IsUnderline" /> property). true
      ///   for an underlined typeface, false otherwise.
      /// </summary>
      public static bool TitleFontUnderline = false;

      /// <summary>
      ///   The default color for filling in the <see cref="Title" /> text background
      ///   (see <see cref="ZedGraph.Fill.Color" /> property).
      /// </summary>
      public static Color TitleFillColor = Color.White;

      /// <summary>
      ///   The default custom brush for filling in the <see cref="Title" /> text background
      ///   (see <see cref="ZedGraph.Fill.Brush" /> property).
      /// </summary>
      public static Brush TitleFillBrush = null;

      /// <summary>
      ///   The default fill mode for filling in the <see cref="Title" /> text background
      ///   (see <see cref="ZedGraph.Fill.Type" /> property).
      /// </summary>
      public static FillType TitleFillType = FillType.None;

      /// <summary>
      ///   The default color for the <see cref="Axis" /> itself
      ///   (<see cref="Axis.Color" /> property).  This color only affects the
      ///   the axis border.
      /// </summary>
      public static Color BorderColor = Color.Black;

      /// <summary>
      ///   The default value for <see cref="Axis.IsAxisSegmentVisible" />, which determines
      ///   whether or not the scale segment itself is visible
      /// </summary>
      public static bool IsAxisSegmentVisible = true;

      /// <summary>
      ///   The default setting for the <see cref="Axis" /> scale axis type
      ///   (<see cref="Axis.Type" /> property).  This value is set as per
      ///   the <see cref="AxisType" /> enumeration
      /// </summary>
      public static AxisType Type = AxisType.Linear;

      /// <summary>
      ///   The default color for the axis segment.
      /// </summary>
      public static Color Color = Color.Black;

      /// <summary>
      ///   The default setting for the axis space allocation.  This term, expressed in
      ///   points (1/72 inch) and scaled according to <see cref="PaneBase.ScaleFactor" /> for the
      ///   <see cref="GraphPane" />, determines the minimum amount of space an axis must
      ///   have between the <see cref="Chart.Rect" /> and the
      ///   <see cref="PaneBase.Rect" />.  This minimum space
      ///   applies whether <see cref="Axis.IsVisible" /> is true or false.
      /// </summary>
      public static float MinSpace = 0f;
    }

    #endregion

    #region Class Fields

    /// <summary>
    ///   Private fields for the <see cref="Axis" /> scale rendering properties.
    ///   Use the public properties <see cref="Cross" /> and <see cref="Scale.BaseTic" />
    ///   for access to these values.
    /// </summary>
    private double _cross;

    /// <summary>
    ///   A tag object for use by the user.  This can be used to store additional
    ///   information associated with the <see cref="Axis" />.  ZedGraph does
    ///   not use this value for any purpose.
    /// </summary>
    /// <remarks>
    ///   Note that, if you are going to Serialize ZedGraph data, then any type
    ///   that you store in <see cref="Tag" /> must be a serializable type (or
    ///   it will cause an exception).
    /// </remarks>
    public object Tag { get; set; }

    /// <summary>
    ///   Temporary values for axis space calculations (see <see cref="CalcSpace" />).
    /// </summary>
    internal float _tmpSpace;

    #endregion

    #region Events

    /// <summary>
    ///   A delegate that allows full custom formatting of the Axis labels
    /// </summary>
    /// <param name="pane">
    ///   The <see cref="GraphPane" /> for which the label is to be
    ///   formatted
    /// </param>
    /// <param name="axis">The <see cref="Scale" /> of interest.</param>
    /// <param name="val">The value to be formatted</param>
    /// <param name="index">The zero-based index of the label to be formatted</param>
    /// <returns>
    ///   A string value representing the label, or null if the ZedGraph should go ahead
    ///   and generate the label according to the current settings
    /// </returns>
    /// <seealso cref="ScaleFormatEvent" />
    public delegate string ScaleFormatHandler(
      GraphPane pane, Axis axis, double val, int index);

    /// <summary>
    ///   Subscribe to this event to handle custom formatting of the scale labels.
    /// </summary>
    public event ScaleFormatHandler ScaleFormatEvent;

    // Revision: JCarpenter 10/06
    /// <summary>
    ///   Allow customization of title based on user preferences.
    /// </summary>
    /// <param name="axis">The <see cref="Axis" /> of interest.</param>
    /// <returns>
    ///   A string value representing the label, or null if the ZedGraph should go ahead
    ///   and generate the label according to the current settings.  To make the title
    ///   blank, return "".
    /// </returns>
    /// <seealso cref="ScaleFormatEvent" />
    public delegate string ScaleTitleEventHandler(Axis axis);

    //Revision: JCarpenter 10/06
    /// <summary>
    ///   Allow customization of the title when the scale is very large
    ///   Subscribe to this event to handle custom formatting of the scale axis label.
    /// </summary>
    public event ScaleTitleEventHandler ScaleTitleEvent;

    #endregion

    #region Constructors

    /// <summary>
    ///   Default constructor for <see cref="Axis" /> that sets all axis properties
    ///   to default values as defined in the <see cref="Default" /> class.
    /// </summary>
    protected Axis()
    {
      Scale = new LinearScale(this);
      Cross = 0.0;
      CrossAuto = true;

      MajorTic = new MajorTic();
      MinorTic = new MinorTic();

      MajorGrid = new MajorGrid();
      MinorGrid = new MinorGrid();

      AxisGap = Default.AxisGap;
      MinSpace = Default.MinSpace;
      IsVisible = true;

      IsAxisSegmentVisible = Default.IsAxisSegmentVisible;

      Title = new AxisLabel("", Default.TitleFontFamily, Default.TitleFontSize,
                            Default.TitleFontColor, Default.TitleFontBold,
                            Default.TitleFontUnderline, Default.TitleFontItalic);
      Title.FontSpec.Fill = new Fill(Default.TitleFillColor, Default.TitleFillBrush,
                                     Default.TitleFillType);

      Title.FontSpec.Border.IsVisible = false;

      Color = Default.Color;
    }

    /// <summary>
    ///   Constructor for <see cref="Axis" /> that sets all axis properties
    ///   to default values as defined in the <see cref="Default" /> class,
    ///   except for the <see cref="Title" />.
    /// </summary>
    /// <param name="title">A string containing the axis title</param>
    protected Axis(string title) : this()
    {
      Title.Text = title;
    }

    /// <summary>
    ///   The Copy Constructor.
    /// </summary>
    /// <param name="rhs">The Axis object from which to copy</param>
    protected Axis(Axis rhs)
    {
      Scale = rhs.Scale.Clone(this);
      Cross = rhs.Cross;
      CrossAuto = rhs.CrossAuto;
      MajorTic = rhs.MajorTic.Clone();
      MinorTic = rhs.MinorTic.Clone();
      MajorGrid = rhs.MajorGrid.Clone();
      MinorGrid = rhs.MinorGrid.Clone();
      IsVisible = rhs.IsVisible;
      IsAxisSegmentVisible = rhs.IsAxisSegmentVisible;
      Title = rhs.Title.Clone();
      AxisGap = rhs.AxisGap;
      MinSpace = rhs.MinSpace;
      Color = rhs.Color;

      if (rhs.LineHObjs != null)
        LineHObjs = rhs.LineHObjs.Clone();
    }

    /// <summary>
    ///   Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
    ///   calling the typed version of Clone.
    /// </summary>
    /// <remarks>
    ///   Note that this method must be called with an explicit cast to ICloneable, and
    ///   that it is inherently virtual.  For example:
    ///   <code>
    /// ParentClass foo = new ChildClass();
    /// ChildClass bar = (ChildClass) ((ICloneable)foo).Clone();
    /// </code>
    ///   Assume that ChildClass is inherited from ParentClass.  Even though foo is declared with
    ///   ParentClass, it is actually an instance of ChildClass.  Calling the ICloneable implementation
    ///   of Clone() on foo actually calls ChildClass.Clone() as if it were a virtual function.
    /// </remarks>
    /// <returns>A deep copy of this object</returns>
    object ICloneable.Clone()
    {
      throw new NotImplementedException(
                                        "Can't clone an abstract base type -- child types must implement ICloneable");
      //return new PaneBase( this );
    }

    #endregion

    #region Serialization

    /// <summary>
    ///   Current schema value that defines the version of the serialized file
    /// </summary>
    public const int schema = 11;

    /// <summary>
    ///   Constructor for deserializing objects
    /// </summary>
    /// <param name="info">
    ///   A <see cref="SerializationInfo" /> instance that defines the serialized data
    /// </param>
    /// <param name="context">
    ///   A <see cref="StreamingContext" /> instance that contains the serialized data
    /// </param>
    protected Axis(SerializationInfo info, StreamingContext context)
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      var sch = info.GetInt32("schema");
      Cross = info.GetDouble("cross");
      CrossAuto = info.GetBoolean("crossAuto");
      MajorTic = (MajorTic)info.GetValue("MajorTic", typeof(MajorTic));
      MinorTic = (MinorTic)info.GetValue("MinorTic", typeof(MinorTic));
      MajorGrid = (MajorGrid)info.GetValue("majorGrid", typeof(MajorGrid));
      MinorGrid = (MinorGrid)info.GetValue("minorGrid", typeof(MinorGrid));
      IsVisible = info.GetBoolean("isVisible");
      Title = (AxisLabel)info.GetValue("title", typeof(AxisLabel));
      MinSpace = info.GetSingle("minSpace");
      Color = (Color)info.GetValue("color", typeof(Color));
      IsAxisSegmentVisible = info.GetBoolean("isAxisSegmentVisible");
      AxisGap = info.GetSingle("axisGap");
      Scale = (Scale)info.GetValue("scale", typeof(Scale));
      Scale._ownerAxis = this;
      LineHObjs = (LineHObjList)info.GetValue("LineHObjs", typeof(LineHObjList));
    }

    /// <summary>
    ///   Populates a <see cref="SerializationInfo" /> instance with the data needed to serialize the target object
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo" /> instance that defines the serialized data</param>
    /// <param name="context">A <see cref="StreamingContext" /> instance that contains the serialized data</param>
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("schema", schema);

      info.AddValue("cross", Cross);
      info.AddValue("crossAuto", CrossAuto);

      info.AddValue("MajorTic", MajorTic);
      info.AddValue("MinorTic", MinorTic);
      info.AddValue("majorGrid", MajorGrid);
      info.AddValue("minorGrid", MinorGrid);

      info.AddValue("isVisible", IsVisible);
      info.AddValue("title", Title);
      info.AddValue("minSpace", MinSpace);
      info.AddValue("color", Color);
      info.AddValue("isAxisSegmentVisible", IsAxisSegmentVisible);
      info.AddValue("axisGap", AxisGap);
      info.AddValue("scale", Scale);
      info.AddValue("LineHObjs", LineHObjs);
    }

    #endregion

    #region Scale Properties

    /// <summary>
    ///   Gets the <see cref="Scale" /> instance associated with this <see cref="Axis" />.
    /// </summary>
    public Scale Scale { get; set; }

    /// <summary>
    ///   Gets or sets the scale value at which this axis should cross the "other" axis.
    /// </summary>
    /// <remarks>
    ///   This property allows the axis to be shifted away from its default location.
    ///   For example, for a graph with an X range from -100 to +100, the Y Axis can be located
    ///   at the X=0 value rather than the left edge of the ChartRect.  This value can be set
    ///   automatically based on the state of <see cref="CrossAuto" />.  If
    ///   this value is set manually, then <see cref="CrossAuto" /> will
    ///   also be set to false.  The "other" axis is the axis the handles the second dimension
    ///   for the graph.  For the XAxis, the "other" axis is the YAxis.  For the YAxis or
    ///   Y2Axis, the "other" axis is the XAxis.
    /// </remarks>
    /// <value> The value is defined in user scale units </value>
    /// <seealso cref="Scale.Min" />
    /// <seealso cref="Scale.Max" />
    /// <seealso cref="Scale.MajorStep" />
    /// <seealso cref="CrossAuto" />
    public double Cross
    {
      get { return _cross; }
      set
      {
        _cross = value;
        CrossAuto = false;
      }
    }

    /// <summary>
    ///   Gets or sets a value that determines whether or not the <see cref="Cross" /> value
    ///   is set automatically.
    /// </summary>
    /// <value>
    ///   Set to true to have ZedGraph put the axis in the default location, or false
    ///   to specify the axis location manually with a <see cref="Cross" /> value.
    /// </value>
    /// <seealso cref="Scale.Min" />
    /// <seealso cref="Scale.Max" />
    /// <seealso cref="Scale.MajorStep" />
    /// <seealso cref="Cross" />
    public bool CrossAuto { get; set; }

    /// <summary>
    ///   Gets or sets the minimum axis space allocation.
    /// </summary>
    /// <remarks>
    ///   This term, expressed in
    ///   points (1/72 inch) and scaled according to <see cref="PaneBase.ScaleFactor" />
    ///   for the <see cref="GraphPane" />, determines the minimum amount of space
    ///   an axis must have between the <see cref="Chart.Rect">Chart.Rect</see> and the
    ///   <see cref="PaneBase.Rect">GraphPane.Rect</see>.  This minimum space
    ///   applies whether <see cref="IsVisible" /> is true or false.
    /// </remarks>
    public float MinSpace { get; set; }

    /// <summary>
    ///   Gets or sets the list of <see cref="LineHObj" /> items for this <see cref="GraphPane" />
    /// </summary>
    /// <value>A reference to a <see cref="LineHObjList" /> collection object</value>
    protected LineHObjList LineHObjs { get; set; }

    #endregion

    #region Tic Properties

    /// <summary>
    ///   The color to use for drawing this <see cref="Axis" />.
    /// </summary>
    /// <remarks>
    ///   This affects only the axis segment (see <see cref="IsAxisSegmentVisible" />),
    ///   since the <see cref="Title" />,
    ///   <see cref="Scale" />, <see cref="MajorTic" />, <see cref="MinorTic" />,
    ///   <see cref="MajorGrid" />, and <see cref="MinorGrid" />
    ///   all have their own color specification.
    /// </remarks>
    /// <value>
    ///   The color is defined using the
    ///   <see cref="System.Drawing.Color" /> class
    /// </value>
    /// <seealso cref="Default.Color" />
    /// .
    /// <seealso cref="IsVisible" />
    public Color Color { get; set; }

    /// <summary>
    ///   Gets a reference to the <see cref="ZedGraph.MajorTic" /> class instance
    ///   for this <see cref="Axis" />.  This class stores all the major tic settings.
    /// </summary>
    public MajorTic MajorTic { get; }

    /// <summary>
    ///   Gets a reference to the <see cref="ZedGraph.MinorTic" /> class instance
    ///   for this <see cref="Axis" />.  This class stores all the minor tic settings.
    /// </summary>
    public MinorTic MinorTic { get; }

    #endregion

    #region Grid Properties

    /// <summary>
    ///   Gets a reference to the <see cref="MajorGrid" /> class that contains the properties
    ///   of the major grid.
    /// </summary>
    public MajorGrid MajorGrid { get; }

    /// <summary>
    ///   Gets a reference to the <see cref="MinorGrid" /> class that contains the properties
    ///   of the minor grid.
    /// </summary>
    public MinorGrid MinorGrid { get; }

    #endregion

    #region Type Properties

    /// <summary>
    ///   This property determines whether or not the <see cref="Axis" /> is shown.
    /// </summary>
    /// <remarks>
    ///   Note that even if
    ///   the axis is not visible, it can still be actively used to draw curves on a
    ///   graph, it will just be invisible to the user
    /// </remarks>
    /// <value>true to show the axis, false to disable all drawing of this axis</value>
    /// <seealso cref="Scale.IsVisible" />
    /// <seealso cref="XAxis.Default.IsVisible" />
    /// <seealso cref="YAxis.Default.IsVisible" />
    /// <seealso cref="Y2Axis.Default.IsVisible" />
    public bool IsVisible { get; set; }

    /// <summary>
    ///   Gets or sets a property that determines whether or not the axis segment (the line that
    ///   represents the axis itself) is drawn.
    /// </summary>
    /// <remarks>
    ///   Under normal circumstances, this value won't affect the appearance of the display because
    ///   the Axis segment is overlain by the Axis border (see <see cref="Chart.Border" />).
    ///   However, when the border is not visible, or when <see cref="Axis.CrossAuto" /> is set to
    ///   false, this value will make a difference.
    /// </remarks>
    public bool IsAxisSegmentVisible { get; set; }

    /// <summary>
    ///   Gets or sets the <see cref="AxisType" /> for this <see cref="Axis" />.
    /// </summary>
    /// <remarks>
    ///   The type can be either <see cref="AxisType.Linear" />,
    ///   <see cref="AxisType.Log" />, <see cref="AxisType.Date" />,
    ///   or <see cref="AxisType.Text" />.
    /// </remarks>
    /// <seealso cref="Scale.IsLog" />
    /// <seealso cref="Scale.IsText" />
    /// <seealso cref="Scale.IsOrdinal" />
    /// <seealso cref="Scale.IsDate" />
    /// <seealso cref="Scale.IsReverse" />
    public AxisType Type
    {
      get { return Scale.Type; }
      set { Scale = Scale.MakeNewScale(Scale, value); }
    }

    #endregion

    #region Label Properties

    /// <summary>
    ///   Gets or sets the <see cref="Label" /> class that contains the title of this
    ///   <see cref="Axis" />.
    /// </summary>
    /// <remarks>
    ///   The title normally shows the basis and dimensions of
    ///   the scale range, such as "Time (Years)".  The title is only shown if the
    ///   <see cref="Label.IsVisible" /> property is set to true.  If the Title text is empty,
    ///   then no title is shown, and no space is "reserved" for the title on the graph.
    /// </remarks>
    /// <value>the title is a string value</value>
    /// <seealso cref="AxisLabel.IsOmitMag" />
    public AxisLabel Title { get; set; }

    /// <summary>
    ///   The size of the gap between multiple axes (see <see cref="GraphPane.YAxisList" /> and
    ///   <see cref="GraphPane.Y2AxisList" />).
    /// </summary>
    /// <remarks>
    ///   This size will be scaled
    ///   according to the <see cref="PaneBase.ScaleFactor" /> for the
    ///   <see cref="GraphPane" />
    /// </remarks>
    /// <value>The axis gap is measured in points (1/72 inch)</value>
    /// <seealso cref="Default.AxisGap" />
    /// .
    public float AxisGap { get; set; }

    #endregion

    #region Rendering Methods

    /// <summary>
    ///   Restore the scale ranging to automatic mode, and recalculate the
    ///   <see cref="Axis" /> scale ranges
    /// </summary>
    /// <param name="pane">
    ///   A reference to the <see cref="GraphPane" /> object that is the parent or
    ///   owner of this object.
    /// </param>
    /// <param name="g">
    ///   A graphic device object to be drawn into.  This is normally e.Graphics from the
    ///   PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <seealso cref="Scale.MinAuto" />
    /// <seealso cref="Scale.MaxAuto" />
    /// <seealso cref="Scale.MajorStepAuto" />
    /// <seealso cref="Scale.MagAuto" />
    /// <seealso cref="Scale.FormatAuto" />
    public void RestoreAutoScale(GraphPane pane, Graphics g, bool autoFormat = false)
    {
      Scale.MinAuto = true;
      Scale.MaxAuto = true;
      Scale.MajorStepAuto = true;
      Scale.MinorStepAuto = true;
      CrossAuto = true;
      Scale.MagAuto = true;
      //this.numDecAuto = true;
      if (autoFormat)
        Scale.FormatAuto = true;
      pane.AxisChange(g);
    }

    /// <summary>
    ///   Return the user scale value that corresponds to the specified screen
    ///   coordinate position (pixels).
    /// </summary>
    public double ReverseTransform(GraphPane pane, int pixCoord)
    {
      // Setup the scaling data based on the chart rect
      Scale.SetupScaleData(pane);
      return Scale.ReverseTransform(pixCoord);
    }

    /// <summary>
    ///   Do all rendering associated with this <see cref="Axis" /> to the specified
    ///   <see cref="Graphics" /> device.
    /// </summary>
    /// <remarks>
    ///   This method is normally only
    ///   called by the Draw method of the parent <see cref="GraphPane" /> object.
    /// </remarks>
    /// <param name="g">
    ///   A graphic device object to be drawn into.  This is normally e.Graphics from the
    ///   PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="pane">
    ///   A reference to the <see cref="GraphPane" /> object that is the parent or
    ///   owner of this object.
    /// </param>
    /// <param name="scaleFactor">
    ///   The scaling factor to be used for rendering objects.  This is calculated and
    ///   passed down by the parent <see cref="GraphPane" /> object using the
    ///   <see cref="PaneBase.ScaleFactor" /> method, and is used to proportionally adjust
    ///   font sizes, etc. according to the actual size of the graph.
    /// </param>
    /// <param name="shiftPos">
    ///   The number of pixels to shift to account for non-primary axis position (e.g.,
    ///   the second, third, fourth, etc. <see cref="YAxis" /> or <see cref="Y2Axis" />.
    /// </param>
    public void Draw(Graphics g, GraphPane pane, float scaleFactor, float shiftPos)
    {
      var saveMatrix = g.Transform;

      Scale.SetupScaleData(pane);

      if (!IsVisible) return;

      var smode = g.SmoothingMode;
      g.SmoothingMode = SmoothingMode.None;

      SetTransformMatrix(g, pane, scaleFactor);

      shiftPos = CalcTotalShift(pane, scaleFactor, shiftPos);

      Scale.Draw(g, pane, scaleFactor, shiftPos);

      //DrawTitle( g, pane, scaleFactor );

      // Draw horizontal lines and label
      LineHObjs?.Draw(g, pane, this, saveMatrix, scaleFactor, shiftPos);

      g.Transform = saveMatrix;

      g.SmoothingMode = smode;
    }

    internal void DrawGrid(Graphics g, GraphPane pane, float scaleFactor, float shiftPos)
    {
      if (!IsVisible) return;

      var saveMatrix = g.Transform;
      SetTransformMatrix(g, pane, scaleFactor);

      var baseVal = Scale.CalcBaseTic();
      float topPix, rightPix;
      Scale.GetTopRightPix(pane, out topPix, out rightPix);

      shiftPos = CalcTotalShift(pane, scaleFactor, shiftPos);

      Scale.DrawGrid(g, pane, baseVal, topPix, scaleFactor);

      DrawMinorTics(g, pane, baseVal, shiftPos, scaleFactor, topPix);

      g.Transform = saveMatrix;
    }

    /// <summary>
    ///   This method will set the <see cref="MinSpace" /> property for this <see cref="Axis" />
    ///   using the currently required space multiplied by a fraction (<paramref>bufferFraction</paramref>).
    /// </summary>
    /// <remarks>
    ///   The currently required space is calculated using <see cref="CalcSpace" />, and is
    ///   based on current data ranges, font sizes, etc.  The "space" is actually the amount of space
    ///   required to fit the tic marks, scale labels, and axis title.
    /// </remarks>
    /// <param name="g">
    ///   A graphic device object to be drawn into.  This is normally e.Graphics from the
    ///   PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="pane">
    ///   A reference to the <see cref="GraphPane" /> object that is the parent or
    ///   owner of this object.
    /// </param>
    /// <param name="bufferFraction">
    ///   The amount of space to allocate for the axis, expressed
    ///   as a fraction of the currently required space.  For example, a value of 1.2 would
    ///   allow for 20% extra above the currently required space.
    /// </param>
    /// <param name="isGrowOnly">
    ///   If true, then this method will only modify the <see cref="MinSpace" />
    ///   property if the calculated result is more than the current value.
    /// </param>
    public void SetMinSpaceBuffer(Graphics g, GraphPane pane, float bufferFraction,
                                  bool isGrowOnly)
    {
      // save the original value of minSpace
      var oldSpace = MinSpace;
      // set minspace to zero, since we don't want it to affect the CalcSpace() result
      MinSpace = 0;
      // Calculate the space required for the current graph assuming scalefactor = 1.0
      // and apply the bufferFraction
      float fixedSpace;
      var space = CalcSpace(g, pane, 1.0F, out fixedSpace)*bufferFraction;
      // isGrowOnly indicates the minSpace can grow but not shrink
      if (isGrowOnly)
        space = Math.Max(oldSpace, space);
      // Set the minSpace
      MinSpace = space;
    }

    /// <summary>
    ///   Setup the Transform Matrix to handle drawing of this <see cref="Axis" />
    /// </summary>
    /// <param name="g">
    ///   A graphic device object to be drawn into.  This is normally e.Graphics from the
    ///   PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="pane">
    ///   A reference to the <see cref="GraphPane" /> object that is the parent or
    ///   owner of this object.
    /// </param>
    /// <param name="scaleFactor">
    ///   The scaling factor to be used for rendering objects.  This is calculated and
    ///   passed down by the parent <see cref="GraphPane" /> object using the
    ///   <see cref="PaneBase.ScaleFactor" /> method, and is used to proportionally adjust
    ///   font sizes, etc. according to the actual size of the graph.
    /// </param>
    public abstract void SetTransformMatrix(Graphics g, GraphPane pane, float scaleFactor);

    /// <summary>
    ///   Calculate the "shift" size, in pixels, in order to shift the axis from its default
    ///   location to the value specified by <see cref="Cross" />.
    /// </summary>
    /// <param name="pane">
    ///   A reference to the <see cref="GraphPane" /> object that is the parent or
    ///   owner of this object.
    /// </param>
    /// <returns>The shift amount measured in pixels</returns>
    internal abstract float CalcCrossShift(GraphPane pane);

    /// <summary>
    ///   Gets the "Cross" axis that corresponds to this axis.
    /// </summary>
    /// <remarks>
    ///   The cross axis is the axis which determines the of this Axis when the
    ///   <see cref="Axis.Cross">Axis.Cross</see> property is used.  The
    ///   cross axis for any <see cref="XAxis" /> or <see cref="X2Axis" />
    ///   is always the primary <see cref="YAxis" />, and
    ///   the cross axis for any <see cref="YAxis" /> or <see cref="Y2Axis" /> is
    ///   always the primary <see cref="XAxis" />.
    /// </remarks>
    /// <param name="pane">
    ///   A reference to the <see cref="GraphPane" /> object that is the parent or
    ///   owner of this object.
    /// </param>
    public abstract Axis GetCrossAxis(GraphPane pane);

    //    abstract internal float GetMinPix( GraphPane pane );

    //abstract internal float CalcCrossFraction( GraphPane pane );

    /// <summary>
    ///   Returns the linearized actual cross position for this axis, reflecting the settings of
    ///   <see cref="Cross" />, <see cref="CrossAuto" />, and <see cref="Scale.IsReverse" />.
    /// </summary>
    /// <remarks>
    ///   If the value of <see cref="Cross" /> lies outside the axis range, it is
    ///   limited to the axis range.
    /// </remarks>
    internal double EffectiveCrossValue(GraphPane pane)
    {
      var crossAxis = GetCrossAxis(pane);

      // Use Linearize here instead of _minLinTemp because this method is called
      // as part of CalcRect() before scale is fully setup
      var min = crossAxis.Scale.Linearize(crossAxis.Scale._min);
      var max = crossAxis.Scale.Linearize(crossAxis.Scale._max);

      if (CrossAuto)
        return crossAxis.Scale.IsReverse == (this is Y2Axis || this is X2Axis) ? max : min;

      if (Cross < min)
        return min;
      if (Cross > max)
        return max;
      return Scale.Linearize(Cross);
    }

    /// <summary>
    ///   Return true if difference between two values is below the scale's resolution
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    internal void MinScaleLimit(double v1, ref double v2)
    {
      var limit = Scale.MajorStep*Scale.MajorUnitMultiplier;
      if ((v1 > float.MaxValue) || (v2 > float.MaxValue)) return;
      var diff = Math.Abs(v1 - v2);
      if ((Scale.MajorStep < float.MaxValue) && (diff < limit))
        v2 = v1 < v2 ? v1 + limit : v1 - limit;
    }

    /// <summary>
    ///   Returns true if the axis is shifted at all due to the setting of
    ///   <see cref="Cross" />.  This function will always return false if
    ///   <see cref="CrossAuto" /> is true.
    /// </summary>
    internal bool IsCrossShifted(GraphPane pane)
    {
      if (CrossAuto)
        return false;
      var crossAxis = GetCrossAxis(pane);
      if (((this is XAxis || this is YAxis) && !crossAxis.Scale.IsReverse) ||
          ((this is X2Axis || this is Y2Axis) && crossAxis.Scale.IsReverse))
      {
        if (Cross <= crossAxis.Scale._min)
          return false;
      }
      else
      {
        if (Cross >= crossAxis.Scale._max)
          return false;
      }

      return true;
    }

    /// <summary>
    ///   Calculates the proportional fraction of the total cross axis width at which
    ///   this axis is located.
    /// </summary>
    /// <param name="pane"></param>
    /// <returns></returns>
    internal float CalcCrossFraction(GraphPane pane)
    {
      // if this axis is not shifted due to the Cross value
      if (!IsCrossShifted(pane))
      {
        // if it's the primary axis and the scale labels are on the inside, then we
        // don't need to save any room for the axis labels (they will be inside the chart rect)
        if (IsPrimary(pane) && Scale.IsLabelsInside)
          return 1.0f;
        // otherwise, it's a secondary (outboard) axis and we always save room for the axis and labels.
        return 0.0f;
      }

      var effCross = EffectiveCrossValue(pane);
      var crossAxis = GetCrossAxis(pane);

      // Use Linearize here instead of _minLinTemp because this method is called
      // as part of CalcRect() before scale is fully setup
      //      double max = crossAxis._scale._maxLinTemp;
      //      double min = crossAxis._scale._minLinTemp;
      var max = crossAxis.Scale.Linearize(crossAxis.Scale._min);
      var min = crossAxis.Scale.Linearize(crossAxis.Scale._max);
      float frac;

      if (((this is XAxis || this is YAxis) &&
           (Scale.IsLabelsInside == crossAxis.Scale.IsReverse)) ||
          ((this is X2Axis || this is Y2Axis) &&
           (Scale.IsLabelsInside != crossAxis.Scale.IsReverse)))
        frac = (float)((effCross - min)/(max - min));
      else
        frac = (float)((max - effCross)/(max - min));

      if (frac < 0.0f)
        frac = 0.0f;
      if (frac > 1.0f)
        frac = 1.0f;

      return frac;
    }

    private float CalcTotalShift(GraphPane pane, float scaleFactor, float shiftPos)
    {
      if (!IsPrimary(pane))
        if (IsCrossShifted(pane))
        {
          shiftPos = 0;
        }
        else
        {
          // Scaled size (pixels) of a tic
          var ticSize = MajorTic.ScaledTic(scaleFactor);

          // if the scalelabels are on the inside, shift everything so the axis is drawn,
          // for example, to the left side of the available space for a YAxis type
          if (Scale.IsLabelsInside)
          {
            shiftPos += _tmpSpace;

            // shift the axis to leave room for the outside tics
            if (MajorTic.IsOutside || MajorTic.IsCrossOutside ||
                MinorTic.IsOutside || MinorTic.IsCrossOutside)
              shiftPos -= ticSize;
          }
          else
          {
            // if it's not the primary axis, add a tic space for the spacing between axes
            shiftPos += AxisGap*scaleFactor;

            // if it has inside tics, leave another tic space
            if (MajorTic.IsInside || MajorTic.IsCrossInside ||
                MinorTic.IsInside || MinorTic.IsCrossInside)
              shiftPos += ticSize;
          }
        }

      // shift is the position of the actual axis line itself
      // everything else is based on that position.
      var crossShift = CalcCrossShift(pane);
      shiftPos += crossShift;
      /*
      if (this is IXAxis)
      {
        Rect.X     = 0f;
        Rect.Width = (startPos - shiftPos);
      }
      else
      {
        Rect.Y      = 0f;
        Rect.Height = (startPos - shiftPos);
      }
      */
      return shiftPos;
    }

    /// <summary>
    ///   Calculate the space required (pixels) for this <see cref="Axis" /> object.
    /// </summary>
    /// <remarks>
    ///   This is the total space (vertical space for the X axis, horizontal space for
    ///   the Y axes) required to contain the axis.  If <see cref="Cross" /> is zero, then
    ///   this space will be the space required between the <see cref="Chart.Rect" /> and
    ///   the <see cref="PaneBase.Rect" />.  This method sets the internal values of
    ///   <see cref="_tmpSpace" /> for use by the <see cref="GraphPane.CalcChartRect(Graphics)" />
    ///   method.
    /// </remarks>
    /// <param name="g">
    ///   A graphic device object to be drawn into.  This is normally e.Graphics from the
    ///   PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="pane">
    ///   A reference to the <see cref="GraphPane" /> object that is the parent or
    ///   owner of this object.
    /// </param>
    /// <param name="scaleFactor">
    ///   The scaling factor to be used for rendering objects.  This is calculated and
    ///   passed down by the parent <see cref="GraphPane" /> object using the
    ///   <see cref="PaneBase.ScaleFactor" /> method, and is used to proportionally adjust
    ///   font sizes, etc. according to the actual size of the graph.
    /// </param>
    /// <param name="fixedSpace">
    ///   The amount of space (pixels) at the edge of the ChartRect
    ///   that is always required for this axis, even if the axis is shifted by the
    ///   <see cref="Cross" /> value.
    /// </param>
    /// <returns>
    ///   Returns the space, in pixels, required for this axis (between the
    ///   rect and ChartRect)
    /// </returns>
    public float CalcSpace(Graphics g, GraphPane pane, float scaleFactor,
                           out float fixedSpace)
    {
      //fixedSpace = 0;

      //Typical character height for the scale font
      var charHeight = Scale._fontSpec.GetHeight(scaleFactor);
      // Scaled size (pixels) of a tic
      var ticSize = MajorTic.ScaledTic(scaleFactor);
      // Scaled size (pixels) of the axis gap
      var axisGap = AxisGap*scaleFactor;
      var scaledLabelGap = Scale.LabelGap*charHeight;
      var scaledTitleGap = Title.GetScaledGap(scaleFactor);

      // The minimum amount of space to reserve for the NORMAL position of the axis.  This would
      // be the left side of the chart rect for the Y axis, the right side for the Y2 axis, etc.
      // This amount of space is based on the need to reserve space for tics, etc., even if the
      // Axis.Cross property causes the axis to be in a different location.
      fixedSpace  = 0;
      _tmpSpace   = 0; // The actual space needed for this axis (ignoring the setting of Axis.Cross)
      float labelSpace = 0; // The actual space need for drawing label and label gap.
      float fromOffset = 0; // The offset of the axis' scale at the end of the tick lines + tick gap.

      // Account for the Axis
      if (IsVisible)
      {
        var hasTic = MajorTic.IsOutside || MajorTic.IsCrossOutside ||
                     MinorTic.IsOutside || MinorTic.IsCrossOutside;

        // account for the tic space.  Leave the tic space for any type of outside tic (Outside Tic Space)
        if (hasTic)
        {
          fromOffset = ticSize;
          _tmpSpace  = ticSize;
          fixedSpace = ticSize;
        }

        // if this is not the primary axis
        if (!IsPrimary(pane))
        {
          // always leave an extra tic space for the space between the multi-axes (Axis Gap)
          fromOffset += axisGap;
          _tmpSpace  += axisGap;

          // if it has inside tics, leave another tic space (Inside Tic Space)
          if (MajorTic.IsInside || MajorTic.IsCrossInside ||
              MinorTic.IsInside || MinorTic.IsCrossInside)
            _tmpSpace += ticSize;
        }

        // tic takes up 1x tic
        // space between tic and scale label is 0.5 tic
        // scale label is GetScaleMaxSpace()
        // space between scale label and axis label is 0.5 tic

        // account for the tic labels + 'LabelGap' tic gap between the tic and the label
        labelSpace = Scale.GetScaleMaxSpace(g, pane, scaleFactor, true).Height;

        _tmpSpace  += labelSpace + scaledLabelGap;
        fromOffset += scaledLabelGap;

        var str  = MakeTitle();

        // Only add space for the title if there is one
        // Axis Title gets actual height
        // if ( str.Length > 0 && _title._isVisible )
        if (!string.IsNullOrEmpty(str) && Title.IsVisible)
        {
          //tmpSpace += this.TitleFontSpec.BoundingBox( g, str, scaleFactor ).Height;
          fixedSpace  = Title.FontSpec.BoundingBox(g, str, scaleFactor).Height +
                        scaledTitleGap;
          _tmpSpace  += fixedSpace;

          fixedSpace += scaledTitleGap;
        }
      }

      // for the Y axes, make sure that enough space is left to fit the first
      // and last X axis scale label
      if (IsPrimary(pane) && ((this is YAxis && ((!pane.XAxis.Scale.IsSkipFirstLabel &&
                                                  !pane.XAxis.Scale.IsReverse) ||
                                                 (!pane.XAxis.Scale.IsSkipLastLabel &&
                                                  pane.XAxis.Scale.IsReverse))) ||
                              (this is Y2Axis && ((!pane.XAxis.Scale.IsSkipFirstLabel &&
                                                   pane.XAxis.Scale.IsReverse) ||
                                                  (!pane.XAxis.Scale.IsSkipLastLabel &&
                                                   !pane.XAxis.Scale.IsReverse)))) &&
          pane.XAxis.IsVisible && pane.XAxis.Scale.IsVisible)
      {
        // half the width of the widest item, plus a gap of 1/2 the charheight
        var tmp = pane.XAxis.Scale.GetScaleMaxSpace(g, pane, scaleFactor, true).Width/2.0F;
        //+ charHeight / 2.0F;
        //if ( tmp > tmpSpace )
        //  tmpSpace = tmp;

        fixedSpace = Math.Max(tmp, fixedSpace);
      }

      // Verify that the minSpace property was satisfied
      _tmpSpace  = Math.Max(_tmpSpace,  MinSpace*scaleFactor);
      fixedSpace = Math.Max(fixedSpace, MinSpace*scaleFactor);

      // Update rectangle occupied by the axis' scale excluding the axis title
      if (this is IXAxis)
      {
        var top = this is XAxis
                ? pane.Chart.Rect.Bottom + fromOffset
                : pane.Chart.Rect.Top    - fromOffset - labelSpace;
        Scale.Rect = new RectangleF(pane.Chart.Rect.Left, top, pane.Chart.Rect.Width, labelSpace);
      }
      else
      {
        var left = this is YAxis
                 ? pane.Chart.Rect.Left  - fromOffset - labelSpace
                 : pane.Chart.Rect.Right + fromOffset;
        Scale.Rect = new RectangleF(left, pane.Chart.Rect.Top, labelSpace, pane.Chart.Rect.Height);
      }

      return _tmpSpace;
    }

    /// <summary>
    ///   Determines if this <see cref="Axis" /> object is a "primary" one.
    /// </summary>
    /// <remarks>
    ///   The primary axes are the <see cref="XAxis" /> (always), the first
    ///   <see cref="YAxis" /> in the <see cref="GraphPane.YAxisList" />
    ///   (<see cref="CurveItem.YAxisIndex" /> = 0),  and the first
    ///   <see cref="Y2Axis" /> in the <see cref="GraphPane.Y2AxisList" />
    ///   (<see cref="CurveItem.YAxisIndex" /> = 0).  Note that
    ///   <see cref="GraphPane.YAxis" /> and <see cref="GraphPane.Y2Axis" />
    ///   always reference the primary axes.
    /// </remarks>
    /// <param name="pane">
    ///   A reference to the <see cref="GraphPane" /> object that is the parent or
    ///   owner of this object.
    /// </param>
    /// <returns>
    ///   true for a primary <see cref="Axis" /> (for the <see cref="XAxis" />,
    ///   this is always true), false otherwise
    /// </returns>
    internal abstract bool IsPrimary(GraphPane pane);

    internal void FixZeroLine(Graphics g, GraphPane pane, float scaleFactor,
                              float left, float right)
    {
      // restore the zero line if needed (since the fill tends to cover it up)
      if (!IsVisible || !MajorGrid._isZeroLine || !(Scale._min < 0.0) ||
          !(Scale._max > 0.0))
        return;

      var zeroPix = Scale.Transform(0.0);

      using (
        var zeroPen = new Pen(Color, pane.ScaledPenWidth(MajorGrid._penWidth, scaleFactor))
      )
      {
        g.DrawLine(zeroPen, left, zeroPix, right, zeroPix);
      }
    }

    /// <summary>
    ///   Draw the minor tic marks as required for this <see cref="Axis" />.
    /// </summary>
    /// <param name="g">
    ///   A graphic device object to be drawn into.  This is normally e.Graphics from the
    ///   PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="pane">
    ///   A reference to the <see cref="ZedGraph.GraphPane" /> object that is the parent or
    ///   owner of this object.
    /// </param>
    /// <param name="baseVal">
    ///   The scale value for the first major tic position.  This is the reference point
    ///   for all other tic marks.
    /// </param>
    /// <param name="shift">
    ///   The number of pixels to shift this axis, based on the
    ///   value of <see cref="Cross" />.  A positive value is into the ChartRect relative to
    ///   the default axis position.
    /// </param>
    /// <param name="scaleFactor">
    ///   The scaling factor to be used for rendering objects.  This is calculated and
    ///   passed down by the parent <see cref="GraphPane" /> object using the
    ///   <see cref="PaneBase.ScaleFactor" /> method, and is used to proportionally adjust
    ///   font sizes, etc. according to the actual size of the graph.
    /// </param>
    /// <param name="topPix">
    ///   The pixel location of the far side of the ChartRect from this axis.
    ///   This value is the ChartRect.Height for the XAxis, or the ChartRect.Width
    ///   for the YAxis and Y2Axis.
    /// </param>
    public void DrawMinorTics(Graphics g, GraphPane pane, double baseVal, float shift,
                              float scaleFactor, float topPix)
    {
      if ((!MinorTic.IsOutside && !MinorTic.IsOpposite && !MinorTic.IsInside &&
           !MinorTic.IsCrossOutside && !MinorTic.IsCrossInside && !MinorGrid._isVisible) ||
          !IsVisible)
        return;

      double tMajor = Scale._majorStep*Scale.MajorUnitMultiplier,
             tMinor = Scale._minorStep*Scale.MinorUnitMultiplier;

      if (!Scale.IsLog && (tMinor >= tMajor)) return;
      var minorScaledTic = MinorTic.ScaledTic(scaleFactor);

      // Minor tics start at the minimum value and step all the way thru
      // the full scale.  This means that if the minor step size is not
      // an even division of the major step size, the minor tics won't
      // line up with all of the scale labels and major tics.
      var first    = Scale._minLinTemp;
      var last     = Scale._maxLinTemp;
      var dVal     = first;

      var iTic     = Scale.CalcMinorStart(baseVal);
      var MajorTic = 0;
      var majorVal = Scale.CalcMajorTicValue(baseVal, MajorTic);

      using (var pen          = new Pen(MinorTic.Color, pane.ScaledPenWidth(MinorTic.PenWidth, scaleFactor)))
      using (var minorGridPen = MinorGrid.GetPen(pane, scaleFactor))
      {
        // Draw the minor tic marks
        while ((dVal < last) && (iTic < 5000))
        {
          // Calculate the scale value for the current tic
          dVal = Scale.CalcMinorTicValue(baseVal, iTic);
          // Maintain a value for the current major tic
          if (dVal > majorVal)
            majorVal = Scale.CalcMajorTicValue(baseVal, ++MajorTic);

          // Make sure that the current value does not match up with a major tic
          if ((((Math.Abs(dVal) < 1e-20) && (Math.Abs(dVal - majorVal) > 1e-20)) ||
               ((Math.Abs(dVal) > 1e-20) && (Math.Abs((dVal - majorVal)/dVal) > 1e-10))) &&
              (dVal >= first) && (dVal <= last))
          {
            var pixVal = Scale.LocalTransform(dVal);
            MinorGrid.Draw(g, minorGridPen, pixVal, topPix);
            MinorTic.Draw(g, pane, pen, pixVal, topPix, shift, minorScaledTic);
          }

          iTic++;
        }
      }
    }

    /// <summary>
    ///   Draw the title for this <see cref="Axis" />.
    /// </summary>
    /// <remarks>
    ///   On entry, it is assumed that the
    ///   graphics transform has been configured so that the origin is at the left side
    ///   of this axis, and the axis is aligned along the X coordinate direction.
    /// </remarks>
    /// <param name="g">
    ///   A graphic device object to be drawn into.  This is normally e.Graphics from the
    ///   PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="pane">
    ///   A reference to the <see cref="GraphPane" /> object that is the parent or
    ///   owner of this object.
    /// </param>
    /// <param name="shiftPos">
    ///   The number of pixels to shift this axis, based on the
    ///   value of <see cref="Cross" />.  A positive value is into the ChartRect relative to
    ///   the default axis position.
    /// </param>
    /// <param name="scaleFactor">
    ///   The scaling factor to be used for rendering objects.  This is calculated and
    ///   passed down by the parent <see cref="GraphPane" /> object using the
    ///   <see cref="PaneBase.ScaleFactor" /> method, and is used to proportionally adjust
    ///   font sizes, etc. according to the actual size of the graph.
    /// </param>
    public void DrawTitle(Graphics g, GraphPane pane, float shiftPos, float scaleFactor)
    {
      var str = MakeTitle();

      // If the Axis is visible, draw the title
      //if ( _isVisible && _title._isVisible && str.Length > 0 )
      if (!IsVisible || !Title.IsVisible || string.IsNullOrEmpty(str)) return;

      var hasTic = Scale.IsLabelsInside
                     ? MajorTic.IsInside || MajorTic.IsCrossInside ||
                       MinorTic.IsInside || MinorTic.IsCrossInside
                     : MajorTic.IsOutside || MajorTic.IsCrossOutside ||
                       MinorTic.IsOutside || MinorTic.IsCrossOutside;

      // Calculate the title position in screen coordinates
      var x = (Scale._maxPix - Scale._minPix)/2;

      var scaledTic = MajorTic.ScaledTic(scaleFactor);
      var scaledLabelGap = Scale._fontSpec.GetHeight(scaleFactor)*Scale.LabelGap;
      var scaledTitleGap = Title.GetScaledGap(scaleFactor);

      // The space for the scale labels is only reserved if the axis is not shifted due to the
      // cross value.  Note that this could be a problem if the axis is only shifted slightly,
      // since the scale value labels may overlap the axis title.  However, it's not possible to
      // calculate that actual shift amount at this point, because the ChartRect rect has not yet been
      // calculated, and the cross value is determined using a transform of scale values (which
      // rely on ChartRect).

      var gap = scaledTic*(hasTic ? 1.0f : 0.0f) +
                Title.FontSpec.BoundingBox(g, str, scaleFactor).Height/2.0F;
      var y = Scale.IsVisible
                ? Scale.GetScaleMaxSpace(g, pane, scaleFactor, true).Height +
                  scaledLabelGap
                : 0;

      y = Scale.IsLabelsInside ? shiftPos - y - gap : shiftPos + y + gap;

      if (!CrossAuto && !Title._isTitleAtCross)
        y = Math.Max(y, gap);

      // Add in the TitleGap space
      y += scaledTitleGap;

      // Draw the title
      Title.FontSpec.Draw(g, pane, str, x, y, AlignH.Center, AlignV.Center, scaleFactor);
    }

    private string MakeTitle()
    {
      if (Title.Text == null)
        Title.Text = "";

      // Revision: JCarpenter 10/06
      // Allow customization of the modified title when the scale is very large
      // The event handler can edit the full label.  If the handler returns
      // null, then the title will be the default.
      if (ScaleTitleEvent == null)
        return (Scale._mag != 0) && !Title._isOmitMag && !Scale.IsLog
                 ? Title.Text + $" (10^{Scale._mag})"
                 : Title.Text;

      var label = ScaleTitleEvent(this);
      if (label != null)
        return label;

      // If the Mag is non-zero and IsOmitMag == false, and IsLog == false,
      // then add the mag indicator to the title.
      return (Scale._mag != 0) && !Title._isOmitMag && !Scale.IsLog
               ? Title.Text + $" (10^{Scale._mag})"
               : Title.Text;
    }

    /// <summary>
    ///   Make a value label for the axis at the specified ordinal position.
    /// </summary>
    /// <remarks>
    ///   This method properly accounts for <see cref="Scale.IsLog" />,
    ///   <see cref="Scale.IsText" />,
    ///   and other axis format settings.  It also implements the ScaleFormatEvent such that
    ///   custom labels can be created.
    /// </remarks>
    /// <param name="pane">
    ///   A reference to the <see cref="GraphPane" /> object that is the parent or
    ///   owner of this object.
    /// </param>
    /// <param name="index">
    ///   The zero-based, ordinal index of the label to be generated.  For example, a value of 2 would
    ///   cause the third value label on the axis to be generated.
    /// </param>
    /// <param name="dVal">
    ///   The numeric value associated with the label.  This value is ignored for log
    ///   (<see cref="Scale.IsLog" />)
    ///   and text (<see cref="Scale.IsText" />) type axes.
    /// </param>
    /// <returns>The resulting value label as a <see cref="string" /></returns>
    internal string MakeLabelEventWorks(GraphPane pane, int index, double dVal)
    {
      // if there is a valid ScaleFormatEvent, then try to use it to create the label
      // the label will be non-null if it's to be used
      if (ScaleFormatEvent == null)
        return Scale != null ? Scale.MakeLabel(pane, index, dVal) : "?";

      var label = ScaleFormatEvent(pane, this, dVal, index);
      if (label != null)
        return label;

      // second try.  If there's no custom ScaleFormatEvent, then just call
      // _scale.MakeLabel according to the type of scale
      return Scale != null ? Scale.MakeLabel(pane, index, dVal) : "?";
    }

    /// <summary>
    /// Draw label corresponding to <see cref="pixX"/> X coordinate.
    /// </summary>
    /// <returns>Label's rectangle to be used for invalidation</returns>
    internal Rectangle DrawXValueLabel(Graphics g, GraphPane pane, int pixX, FontSpec fontSpec)
    {
      var x     = ReverseTransform(pane, pixX);
      var text  = Scale.MakeLabel(pane, 0, x);
      var sizeF = g.MeasureString(text, fontSpec.Font) + new SizeF(2,2);
      return drawValueLabel(g, pane, text,
        new RectangleF(pixX - sizeF.Width / 2, Scale.Rect.Top - 1, sizeF.Width, Scale.Rect.Height+1),
                       fontSpec, CenterHTextFormat);
    }

    /// <summary>
    /// Draw label corresponding to <see cref="pixY"/> Y coordinate.
    /// </summary>
    /// <returns>Label's rectangle to be used for invalidation</returns>
    internal Rectangle DrawYValueLabel(Graphics g, GraphPane pane, int pixY, FontSpec fontSpec)
    {
      var y     = ReverseTransform(pane, pixY);
      var text  = Scale.MakeLabel(pane, 0, y);
      var sizeF = g.MeasureString(text, fontSpec.Font) + new SizeF(2, 2);

      return drawValueLabel(g, pane, text,
        new RectangleF(Scale.Rect.Left, pixY - 2 - sizeF.Height / 2, Scale.Rect.Width, sizeF.Height+2),
                       fontSpec, CenterVTextFormat);
    }

    static readonly StringFormat CenterVTextFormat = new StringFormat
    {
      Alignment     = StringAlignment.Far,
      LineAlignment = StringAlignment.Center
    };

    static readonly StringFormat CenterHTextFormat = new StringFormat
    {
      Alignment     = StringAlignment.Center,
      LineAlignment = StringAlignment.Center
    };

    private Rectangle drawValueLabel(
      Graphics g, GraphPane pane, string text, RectangleF rectF, FontSpec fontSpec, StringFormat fmt)
    {
      var rect = Rectangle.Round(rectF);

      g.FillRectangle(fontSpec.Fill.Brush, rectF);

      if (fontSpec.Border.IsVisible)
        using (var pen = new Pen(fontSpec.Border.Color, fontSpec.Border.Width))
          g.DrawRectangle(pen, rect);

      if (this is IXAxis)
        rectF.Offset(0, 2);
      else
        rectF.Offset(-1, 1);

      g.DrawString(text, fontSpec.Font, fontSpec.TextBrush, rectF, fmt);

      rect.Inflate(7, 7);

      return rect;
    }

    #endregion
  }
}