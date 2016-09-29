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

#region Using directives

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

#endregion Using directives

namespace ZedGraph
{
  /// <summary>
  /// A collection class containing a list of <see cref="GraphPane"/> objects
  /// organized together in some form.
  /// </summary>
  ///
  /// <author>John Champion</author>
  /// <version> $Revision: 3.26 $ $Date: 2007-11-05 18:28:56 $ </version>
  [Serializable]
  public class MasterPane : PaneBase, ICloneable, ISerializable, IDeserializationCallback
  {
    #region Fields

    /// <summary>
    /// Private field that stores the boolean value that determines whether
    /// <see cref="_countList"/> is specifying rows or columns.
    /// </summary>
    private bool _isColumnSpecified;

    /// <summary>
    /// private field that saves the paneLayout format specified when
    /// <see cref="SetLayout(Graphics,PaneLayout)"/> was called. This value will
    /// default to <see cref="MasterPane.Default.PaneLayout"/> if
    /// <see cref="SetLayout(Graphics,PaneLayout)"/> (or an overload) was never called.
    /// </summary>
    private PaneLayout _paneLayout;
    /// <summary>
    /// private field that stores the row/column size proportional values as specified
    /// to the <see cref="SetLayout(Graphics,bool,int[],float[])"/> method.  This
    /// value will be null if <see cref="SetLayout(Graphics,bool,int[],float[])"/>
    /// was never called. The first element of each tuple corresponds to the proportion
    /// of the corresponding row/column in the <see cref="_countList"/>, and the second
    /// element contains proportions of columns/rows in the I'th row/column.
    /// </summary>
    private Tuple<float, float[]>[] _prop;

    /// <summary>
    /// Current schema value that defines the version of the serialized file
    /// </summary>
    // schema changed to 2 with addition of 'prop'
    // schema changed to 11 with addition of 'isAntiAlias'
    private const int _schema = 12;

    /// <summary>
    /// The rectangle that is set to the the clipping area in the call to OnPaint.
    /// </summary>
    [CLSCompliant(false)]
    protected RectangleF _clipRectF;

    private bool _layoutChanged;

    #endregion Fields

    #region Defaults

    /// <summary>
    /// A simple struct that defines the
    /// default property values for the <see cref="MasterPane"/> class.
    /// </summary>
    public new struct Default
    {
      /// <summary>
      /// The default value for the <see cref="MasterPane.InnerPaneGap"/> property.
      /// This is the size of the margin between adjacent <see cref="GraphPane"/>
      /// objects, in units of points (1/72 inch).
      /// </summary>
      /// <seealso cref="MasterPane.InnerPaneGap"/>
      public static float InnerPaneGap = 10;

      /// <summary>
      /// The default value for the <see cref="IsCommonScaleFactor"/> property.
      /// </summary>
      public static bool IsCommonScaleFactor = false;

      /// <summary>
      /// The default value for the <see cref="Legend.IsVisible"/> property for
      /// the <see cref="MasterPane"/> class.
      /// </summary>
      public static bool IsShowLegend = false;

      /// <summary>
      /// The default value for the <see cref="IsUniformLegendEntries"/> property.
      /// </summary>
      public static bool IsUniformLegendEntries = false;

      /// <summary>
      /// The default pane layout for
      /// <see cref="DoLayout(Graphics)"/>
      /// method calls.
      /// </summary>
      /// <seealso cref="SetLayout(Graphics,PaneLayout)" />
      /// <seealso cref="SetLayout(Graphics,int,int)" />
      /// <seealso cref="SetLayout(Graphics,bool,int[])" />
      /// <seealso cref="SetLayout(Graphics,bool,int[],float[])" />
      /// <seealso cref="ReSize(Graphics,RectangleF)" />
      public static PaneLayout PaneLayout = PaneLayout.SquareColPreferred;
      /// <summary>
      /// The default value for the <see cref="MasterPane.PaneSplitterSize"/> property.
      /// </summary>
      public static float PaneSplitterSize = 3;
    }

    #endregion Defaults

    #region Properties

    /// <summary>
    /// Gets or sets the size of the margin between adjacent <see cref="GraphPane"/>
    /// objects.
    /// </summary>
    /// <remarks>This property is scaled according to <see cref="PaneBase.CalcScaleFactor"/>,
    /// based on <see cref="PaneBase.BaseDimension"/>.  The default value comes from
    /// <see cref="Default.InnerPaneGap"/>.
    /// </remarks>
    /// <value>The value is in points (1/72nd inch).</value>
    public float InnerPaneGap { get; set; } = Default.InnerPaneGap;

    /// <summary>
    /// Gets or sets a value that determines if all drawing operations for this
    /// <see cref="MasterPane" /> will be forced to operate in Anti-alias mode.
    /// Note that if this value is set to "true", it overrides the setting for sub-objects.
    /// Otherwise, the sub-object settings (such as <see cref="FontSpec.IsAntiAlias"/>)
    /// will be honored.
    /// </summary>
    public bool IsAntiAlias { get; set; }

    /// <summary>
    /// Gets or sets a value that determines if the
    /// <see cref="DoLayout(Graphics)" /> method will automatically set the
    /// <see cref="PaneBase.BaseDimension" />
    /// of each <see cref="GraphPane" /> in the <see cref="ZedGraph.PaneList" /> such that the
    /// scale factors have the same value.
    /// </summary>
    /// <remarks>
    /// The scale factors, calculated by <see cref="PaneBase.CalcScaleFactor" />, determine
    /// scaled font sizes, tic lengths, etc.  This function will insure that for
    /// multiple graphpanes, a certain specified font size will be the same for
    /// all the panes.
    /// </remarks>
    /// <seealso cref="SetLayout(Graphics,PaneLayout)" />
    /// <seealso cref="SetLayout(Graphics,int,int)" />
    /// <seealso cref="SetLayout(Graphics,bool,int[])" />
    /// <seealso cref="SetLayout(Graphics,bool,int[],float[])" />
    /// <seealso cref="ReSize(Graphics,RectangleF)" />
    public bool IsCommonScaleFactor { get; set; } = Default.IsCommonScaleFactor;

    /// <summary>
    /// Gets or set the value of the   <see cref="IsUniformLegendEntries"/>
    /// </summary>
    public bool IsUniformLegendEntries { get; set; } = Default.IsUniformLegendEntries;

    /// <summary>
    /// Gets or sets the <see cref="ZedGraph.PaneList"/> collection instance that holds the list of
    /// <see cref="GraphPane"/> objects that are included in this <see cref="MasterPane"/>.
    /// </summary>
    /// <seealso cref="Add"/>
    /// <seealso cref="MasterPane.this[int]"/>
    public PaneList PaneList { get; set; }

    /*
        /// <summary>
        /// Gets the <see cref="PaneLayoutMgr" /> instance, which manages the pane layout
        /// settings, and handles the layout functions.
        /// </summary>
        /// <seealso cref="ZedGraph.PaneLayoutMgr.SetLayout(PaneLayout)" />
        /// <seealso cref="ZedGraph.PaneLayoutMgr.SetLayout(int,int)" />
        /// <seealso cref="ZedGraph.PaneLayoutMgr.SetLayout(bool,int[])" />
        /// <seealso cref="ZedGraph.PaneLayoutMgr.SetLayout(bool,int[],float[])" />
        /// <seealso cref="ReSize" />
        public PaneLayoutMgr PaneLayoutMgr
        {
          get { return _paneLayoutMgr; }
        }
    */
    /// <summary>
    /// Gets or sets the size of the pane splitter
    /// </summary>
    public float PaneSplitterSize { get; set; } = Default.PaneSplitterSize;
    #endregion Properties

    #region Constructors

    /// <summary>
    /// Default constructor for the class.  Sets the <see cref="PaneBase.Rect"/> to (0, 0, 500, 375).
    /// </summary>
    public MasterPane() : this("", new RectangleF(0, 0, 500, 375)) {}

    /// <summary>
    /// Default constructor for the class.  Specifies the <see cref="PaneBase.Title"/> of
    /// the <see cref="MasterPane"/>, and the size of the <see cref="PaneBase.Rect"/>.
    /// </summary>
    public MasterPane(string title, RectangleF paneRect) : base(title, paneRect)
    {
      InnerPaneGap = Default.InnerPaneGap;

      //_paneLayoutMgr = new PaneLayoutMgr();

      PaneList = new PaneList();

      _legend.IsVisible = Default.IsShowLegend;

      InitLayout();
    }

    /// <summary>
    /// The Copy Constructor - Make a deep-copy clone of this class instance.
    /// </summary>
    /// <param name="rhs">The <see cref="MasterPane"/> object from which to copy</param>
    public MasterPane(MasterPane rhs) : base(rhs)
    {
      // copy all the value types
      //_paneLayoutMgr = rhs._paneLayoutMgr.Clone();
      InnerPaneGap = rhs.InnerPaneGap;
      IsUniformLegendEntries = rhs.IsUniformLegendEntries;
      IsCommonScaleFactor = rhs.IsCommonScaleFactor;

      // Then, fill in all the reference types with deep copies
      PaneList = rhs.PaneList.Clone();

      _paneLayout = rhs._paneLayout;
      _isColumnSpecified = rhs._isColumnSpecified;
      _prop = rhs._prop;
      IsAntiAlias = rhs.IsAntiAlias;
    }
    /// <summary>
    /// Typesafe, deep-copy clone method.
    /// </summary>
    /// <returns>A new, independent copy of this class</returns>
    public MasterPane Clone()
    {
      return new MasterPane(this);
    }

    private void InitLayout()
    {
      _paneLayout = Default.PaneLayout;
      _isColumnSpecified = false;
      _prop = null;
    }
    /// <summary>
    /// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
    /// calling the typed version of <see cref="Clone" /> to make a deep copy.
    /// </summary>
    /// <returns>A deep copy of this object</returns>
    object ICloneable.Clone()
    {
      return this.Clone();
    }
    #endregion Constructors

    #region Serialization

    /// <summary>
    /// Constructor for deserializing objects
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
    /// </param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
    /// </param>
    protected MasterPane(SerializationInfo info, StreamingContext context) : base(info, context)
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      int sch = info.GetInt32("schema2");

      PaneList = (PaneList)info.GetValue("paneList", typeof(PaneList));
      //_paneLayoutMgr = (PaneLayoutMgr) info.GetValue( "paneLayoutMgr", typeof(PaneLayoutMgr) );
      InnerPaneGap = info.GetSingle("innerPaneGap");

      IsUniformLegendEntries = info.GetBoolean("isUniformLegendEntries");
      IsCommonScaleFactor = info.GetBoolean("isCommonScaleFactor");

      _paneLayout = (PaneLayout)info.GetValue("paneLayout", typeof(PaneLayout));

      _isColumnSpecified = info.GetBoolean("isColumnSpecified");
      _prop = (Tuple<float, float[]>[])info.GetValue("prop", typeof(Tuple<float, float[]>[]));

      if (sch >= 11)
        IsAntiAlias = info.GetBoolean("isAntiAlias");
    }

    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("schema2", _schema);

      info.AddValue("paneList", PaneList);
      //info.AddValue( "paneLayoutMgr", _paneLayoutMgr );
      info.AddValue("innerPaneGap", InnerPaneGap);

      info.AddValue("isUniformLegendEntries", IsUniformLegendEntries);
      info.AddValue("isCommonScaleFactor", IsCommonScaleFactor);

      info.AddValue("paneLayout", _paneLayout);
      info.AddValue("isColumnSpecified", _isColumnSpecified);
      info.AddValue("prop", _prop);

      info.AddValue("isAntiAlias", IsAntiAlias);
    }

    /// <summary>
    /// Respond to the callback when the MasterPane objects are fully initialized.
    /// </summary>
    /// <param name="sender"></param>
    public void OnDeserialization(object sender)
    {
      Bitmap bitmap = new Bitmap(10, 10);
      Graphics g = Graphics.FromImage(bitmap);
      ReSize(g, _rect);
    }

    #endregion Serialization

    #region Properties

    /// <summary>
    /// Indexer to access the specified <see cref="GraphPane"/> object from <see cref="ZedGraph.PaneList"/>
    /// by its ordinal position in the list.
    /// </summary>
    /// <param name="index">The ordinal position (zero-based) of the
    /// <see cref="GraphPane"/> object to be accessed.</param>
    /// <value>A <see cref="GraphPane"/> object reference.</value>
    public PaneBase this[int index]
    {
      get { return PaneList[index]; }
      set { PaneList[index] = value; }
    }

    /// <summary>
    /// Indexer to access the specified <see cref="GraphPane"/> object from <see cref="ZedGraph.PaneList"/>
    /// by its <see cref="PaneBase.Title"/> string.
    /// </summary>
    /// <param name="title">The string title of the
    /// <see cref="GraphPane"/> object to be accessed.</param>
    /// <value>A <see cref="GraphPane"/> object reference.</value>
    public PaneBase this[string title] => PaneList[title];

    #endregion

    #region Public Methods

    /// <summary>
    /// Add a <see cref="GraphPane"/> object to the <see cref="ZedGraph.PaneList"/> collection at the end of the list.
    /// </summary>
    /// <param name="pane">A reference to the <see cref="GraphPane"/> object to
    /// be added</param>
    /// <seealso cref="IList.Add"/>
    public void Add(PaneBase pane)
    {
      PaneList.Add(pane);
    }

    /// <summary>
    /// Call <see cref="GraphPane.AxisChange()"/> for all <see cref="GraphPane"/> objects in the
    /// <see cref="ZedGraph.PaneList"/> list.
    /// </summary>
    /// <remarks>
    /// This overload of AxisChange just uses a throw-away bitmap as Graphics.
    /// If you have a Graphics instance available from your Windows Form, you should use
    /// the <see cref="AxisChange(Graphics)" /> overload instead.
    /// </remarks>
    public void AxisChange()
    {
      using (var img = new Bitmap((int)this.Rect.Width, (int)this.Rect.Height))
      using (Graphics g = Graphics.FromImage(img))
        AxisChange(g);
    }

    /// <summary>
    /// Call <see cref="GraphPane.AxisChange()"/> for all <see cref="GraphPane"/> objects in the
    /// <see cref="ZedGraph.PaneList"/> list.
    /// </summary>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally e.Graphics from the
    /// PaintEventArgs argument to the Paint() method.
    /// </param>
    public void AxisChange(Graphics g)
    {
      foreach (var pane in PaneList.Where(p => p is GraphPane).Cast<GraphPane>())
        pane.AxisChange(g);
    }

    /// <summary>
    /// Method that forces the scale factor calculations
    /// (via <see cref="PaneBase.CalcScaleFactor" />),
    /// to give a common scale factor for all <see cref="GraphPane" /> objects in the
    /// <see cref="ZedGraph.PaneList" />.
    /// </summary>
    /// <remarks>
    /// This will make it such that a given font size will result in the same output font
    /// size for all <see cref="GraphPane" />'s.  Note that this does not make the scale
    /// factor for the <see cref="GraphPane" />'s the same as that of the
    /// <see cref="MasterPane" />.
    /// </remarks>
    /// <seealso cref="IsCommonScaleFactor" />
    public void CommonScaleFactor()
    {
      if (!IsCommonScaleFactor) return;

      // Find the maximum scaleFactor of all the GraphPanes
      var maxFactor = PaneList.Select(pane => pane.CalcScaleFactor()).Max();

      // Now, calculate the base dimension
      PaneList.ForEach
        (pane => { pane.BaseDimension *= pane.ScaleFactor / maxFactor; pane.OnResizePaneEvent(); });
    }

    /// <summary>
    /// Render all the <see cref="GraphPane"/> objects in the <see cref="ZedGraph.PaneList"/> to the
    /// specified graphics device.
    /// </summary>
    /// <remarks>This method should be part of the Paint() update process.  Calling this routine
    /// will redraw all
    /// features of all the <see cref="GraphPane"/> items.  No preparation is required other than
    /// instantiated <see cref="GraphPane"/> objects that have been added to the list with the
    /// <see cref="Add"/> method.
    /// </remarks>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally e.Graphics from the
    /// PaintEventArgs argument to the Paint() method.
    /// </param>
    public override void Draw(Graphics g)
    {
      if (_layoutChanged)
        doLayout(g);

      // Save the clipping region
      _clipRectF = g.ClipBounds;

      // Save current AntiAlias mode
      var sModeSave = g.SmoothingMode;
      var sHintSave = g.TextRenderingHint;
      var sCompQual = g.CompositingQuality;
      var sInterpMode = g.InterpolationMode;

      SetAntiAliasMode(g, IsAntiAlias);

      // Draw the pane border & background fill, the title, and the GraphObj objects that lie at
      // ZOrder.GBehindAll
      base.Draw(g);

      if (_rect.Width <= 1 || _rect.Height <= 1)
        return;

      var scaleFactor = CalcScaleFactor();

      // Clip everything to the rect
      g.SetClip(_rect);

      // For the MasterPane, All GraphItems go behind the GraphPanes, except those that
      // are explicity declared as ZOrder.AInFront
      GraphObjList.Draw(g, this, scaleFactor, ZOrder.G_BehindChartFill);
      GraphObjList.Draw(g, this, scaleFactor, ZOrder.E_BehindCurves);
      GraphObjList.Draw(g, this, scaleFactor, ZOrder.D_BehindAxis);
      GraphObjList.Draw(g, this, scaleFactor, ZOrder.C_BehindChartBorder);

      // Reset the clipping
      g.ResetClip();

      foreach (var pane in PaneList)
      {
        if (pane is GraphPane)
          ((GraphPane)pane).CurveClipRect = _clipRectF;
        pane.Draw(g);
      }

      // Clip everything to the rect
      g.SetClip(_rect);

      GraphObjList.Draw(g, this, scaleFactor, ZOrder.B_BehindLegend);

      // Recalculate the legend rect, just in case it has not yet been done
      // innerRect is the area for the GraphPane's
      var innerRect = CalcClientRect(g, scaleFactor);
      _legend.CalcRect(g, this, scaleFactor, ref innerRect);
      //this.legend.SetLocation( this,

      _legend.Draw(g, this, scaleFactor);

      GraphObjList.Draw(g, this, scaleFactor, ZOrder.A_InFront);

      // Reset the clipping
      g.ResetClip();

      // Restore original anti-alias mode
      g.SmoothingMode = sModeSave;
      g.TextRenderingHint = sHintSave;
      g.CompositingQuality = sCompQual;
      g.InterpolationMode = sInterpMode;
    }

    /// <summary>
    /// Find the <see cref="GraphPane"/> within the <see cref="ZedGraph.PaneList"/> that contains the
    /// <see paramref="mousePt"/> within its <see cref="Chart.Rect"/>.
    /// </summary>
    /// <param name="mousePt">The mouse point location where you want to search</param>
    /// <returns>A <see cref="GraphPane"/> object that contains the mouse point, or
    /// null if no <see cref="GraphPane"/> was found.</returns>
    public GraphPane FindChartRect(PointF mousePt)
    {
      return (GraphPane)PaneList.FirstOrDefault
        (pane => pane is GraphPane && ((GraphPane)pane).Chart._rect.Contains(mousePt));
    }

    /// <summary>
    /// Find the pane and the object within that pane that lies closest to the specified
    /// mouse (screen) point.
    /// </summary>
    /// <remarks>
    /// This method first finds the <see cref="GraphPane"/> within the list that contains
    /// the specified mouse point.  It then calls the <see cref="GraphPane.FindNearestObject"/>
    /// method to determine which object, if any, was clicked.  With the exception of the
    /// <see paramref="pane"/>, all the parameters in this method are identical to those
    /// in the <see cref="GraphPane.FindNearestObject"/> method.
    /// If the mouse point lies within the <see cref="PaneBase.Rect"/> of any
    /// <see cref="GraphPane"/> item, then that pane will be returned (otherwise it will be
    /// null).  Further, within the selected pane, if the mouse point is within the
    /// bounding box of any of the items (or in the case
    /// of <see cref="ArrowObj"/> and <see cref="CurveItem"/>, within
    /// <see cref="GraphPane.Default.NearestTol"/> pixels), then the object will be returned.
    /// You must check the type of the object to determine what object was
    /// selected (for example, "if ( object is Legend ) ...").  The
    /// <see paramref="index"/> parameter returns the index number of the item
    /// within the selected object (such as the point number within a
    /// <see cref="CurveItem"/> object.
    /// </remarks>
    /// <param name="mousePt">The screen point, in pixel coordinates.</param>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally e.Graphics from the
    /// PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="pane">A reference to the <see cref="GraphPane"/> object that was clicked.</param>
    /// <param name="nearestObj">A reference to the nearest object to the
    /// specified screen point.  This can be any of <see cref="Axis"/>,
    /// <see cref="Legend"/>, <see cref="PaneBase.Title"/>,
    /// <see cref="TextObj"/>, <see cref="ArrowObj"/>, or <see cref="CurveItem"/>.
    /// Note: If the pane title is selected, then the <see cref="GraphPane"/> object
    /// will be returned.
    /// </param>
    /// <param name="index">The index number of the item within the selected object
    /// (where applicable).  For example, for a <see cref="CurveItem"/> object,
    /// <see paramref="index"/> will be the index number of the nearest data point,
    /// accessible via <see cref="CurveItem.Points">CurveItem.Points[index]</see>.
    /// index will be -1 if no data points are available.</param>
    /// <returns>true if a <see cref="GraphPane"/> was found, false otherwise.</returns>
    /// <seealso cref="GraphPane.FindNearestObject"/>
    public bool FindNearestPaneObject(PointF mousePt, Graphics g, out PaneBase pane,
      out object nearestObj, out int index)
    {
      pane = null;
      nearestObj = null;
      index = -1;

      GraphObj saveGraphItem = null;
      int saveIndex = -1;
      float scaleFactor = CalcScaleFactor();

      // See if the point is in a GraphObj
      // If so, just save the object and index so we can see if other overlying objects were
      // intersected as well.
      if (this.GraphObjList.FindPoint(mousePt, this, g, scaleFactor, out index))
      {
        saveGraphItem = this.GraphObjList[index];
        saveIndex = index;

        // If it's an "In-Front" item, then just return it
        if (saveGraphItem.ZOrder == ZOrder.A_InFront)
        {
          nearestObj = saveGraphItem;
          index = saveIndex;
          return true;
        }
      }

      foreach (var tPane in PaneList.Where(tPane => tPane.Rect.Contains(mousePt))) {
        pane = tPane;
        return tPane.FindNearestObject(mousePt, g, out nearestObj, out index);
      }

      // If no items were found in the GraphPanes, then return the item found on the MasterPane (if any)
      if (saveGraphItem == null) return false;

      nearestObj = saveGraphItem;
      index = saveIndex;
      return true;
    }

    /// <summary>
    /// Find the <see cref="GraphPane"/> within the <see cref="ZedGraph.PaneList"/> that contains the
    /// <see paramref="mousePt"/> within its <see cref="PaneBase.Rect"/>.
    /// </summary>
    /// <param name="mousePt">The mouse point location where you want to search</param>
    /// <returns>A <see cref="GraphPane"/> object that contains the mouse point, or
    /// null if no <see cref="GraphPane"/> was found.</returns>
    public PaneBase FindPane(PointF mousePt)
    {
      return PaneList.FirstOrDefault(pane => pane.Rect.Contains(mousePt));
    }

    /// <summary>
    /// Redo the layout using the current size of the <see cref="PaneBase.Rect"/>,
    /// and also handle resizing the
    /// contents by calling <see cref="DoLayout(Graphics)"/>.
    /// </summary>
    /// <remarks>This method will use the pane layout that was specified by a call to
    /// <see cref="SetLayout(Graphics,PaneLayout)"/>.  If
    /// <see cref="SetLayout(Graphics,PaneLayout)"/> has not previously been called,
    /// it will default to <see cref="Default.PaneLayout"/>.
    /// </remarks>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally e.Graphics from the
    /// PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <seealso cref="SetLayout(Graphics,PaneLayout)" />
    /// <seealso cref="SetLayout(Graphics,int,int)" />
    /// <seealso cref="SetLayout(Graphics,bool,int[])" />
    /// <seealso cref="SetLayout(Graphics,bool,int[],float[])" />
    public void ReSize(Graphics g)
    {
      ReSize(g, _rect);
    }

    /// <summary>
    /// Change the size of the <see cref="PaneBase.Rect"/>, and also handle resizing the
    /// contents by calling <see cref="DoLayout(Graphics)"/>.
    /// </summary>
    /// <remarks>This method will use the pane layout that was specified by a call to
    /// <see cref="SetLayout(Graphics,PaneLayout)"/>.  If
    /// <see cref="SetLayout(Graphics,PaneLayout)"/> has not previously been called,
    /// it will default to <see cref="Default.PaneLayout"/>.
    /// </remarks>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally e.Graphics from the
    /// PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="rect"></param>
    /// <seealso cref="SetLayout(Graphics,PaneLayout)" />
    /// <seealso cref="SetLayout(Graphics,int,int)" />
    /// <seealso cref="SetLayout(Graphics,bool,int[])" />
    /// <seealso cref="SetLayout(Graphics,bool,int[],float[])" />
    public override void ReSize(Graphics g, RectangleF rect)
    {
      _rect = rect;
      doLayout(g);
      CommonScaleFactor();
    }

    /// <overloads>The SetLayout() methods setup the desired layout of the
    /// <see cref="GraphPane" /> objects within a <see cref="MasterPane" />.  These functions
    /// do not make any changes, they merely set the parameters so that future calls
    /// to <see cref="PaneBase.ReSize" /> or <see cref="DoLayout(Graphics)" />
    /// will use the desired layout.<br /><br />
    /// The layout options include a set of "canned" layouts provided by the
    /// <see cref="ZedGraph.PaneLayout" /> enumeration, options to just set a specific
    /// number of rows and columns of panes (and all pane sizes are the same), and more
    /// customized options of specifying the number or rows in each column or the number of
    /// columns in each row, along with proportional values that determine the size of each
    /// individual column or row.
    /// </overloads>
    /// <summary>
    /// Automatically set all of the <see cref="GraphPane"/> <see cref="PaneBase.Rect"/>'s in
    /// the list to a pre-defined layout configuration from a <see cref="PaneLayout" />
    /// enumeration.
    /// </summary>
    /// <remarks>This method uses a <see cref="PaneLayout"/> enumeration to describe the type of layout
    /// to be used.  Overloads are available that provide other layout options</remarks>
    /// <param name="paneLayout">A <see cref="PaneLayout"/> enumeration that describes how
    /// the panes should be laid out within the <see cref="PaneBase.Rect"/>.</param>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally created with a call to
    /// the CreateGraphics() method of the Control or Form.
    /// </param>
    /// <seealso cref="SetLayout(Graphics,int,int)" />
    /// <seealso cref="SetLayout(Graphics,bool,int[])" />
    /// <seealso cref="SetLayout(Graphics,bool,int[],float[])" />
    public void SetLayout(Graphics g, PaneLayout paneLayout)
    {
      InitLayout();

      _paneLayout = paneLayout;

      doLayout(g);
    }

    /// <summary>
    /// Automatically set all of the <see cref="GraphPane"/> <see cref="PaneBase.Rect"/>'s in
    /// the list to a reasonable configuration.
    /// </summary>
    /// <remarks>This method explicitly specifies the number of rows and columns to use
    /// in the layout, and all <see cref="GraphPane" /> objects will have the same size.
    /// Overloads are available that provide other layout options</remarks>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally created with a call to
    /// the CreateGraphics() method of the Control or Form.
    /// </param>
    /// <param name="rows">The number of rows of <see cref="GraphPane"/> objects
    /// to include in the layout</param>
    /// <param name="columns">The number of columns of <see cref="GraphPane"/> objects
    /// to include in the layout</param>
    /// <seealso cref="SetLayout(Graphics,PaneLayout)" />
    /// <seealso cref="SetLayout(Graphics,bool,int[])" />
    /// <seealso cref="SetLayout(Graphics,bool,int[],float[])" />
    public void SetLayout(Graphics g, int rows, int columns)
    {
      InitLayout();

      if (rows < 1)
        rows = 1;
      if (columns < 1)
        columns = 1;

      setLayout(g, true, doProportions(rows, columns));
    }

    /// <summary>
    /// Automatically set all of the <see cref="GraphPane"/> <see cref="PaneBase.Rect"/>'s in
    /// the list to the specified configuration.
    /// </summary>
    /// <remarks>This method specifies the number of panes in each row or column, allowing for
    /// irregular layouts.</remarks>
    /// <remarks>This method specifies the number of rows in each column, or the number of
    /// columns in each row, allowing for irregular layouts.  Additionally, a
    /// <see paramref="proportion" /> parameter is provided that allows varying column or
    /// row sizes.  Overloads for SetLayout() are available that provide other layout options.
    /// </remarks>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally created with a call to
    /// the CreateGraphics() method of the Control or Form.
    /// </param>
    /// <param name="isColumnSpecified">Specifies whether the number of columns in each row, or
    /// the number of rows in each column will be specified.  A value of true indicates the
    /// number of columns in each row are specified in <see paramref="_countList"/>.</param>
    /// <param name="countList">An integer array specifying either the number of columns in
    /// each row or the number of rows in each column, depending on the value of
    /// <see paramref="isColumnSpecified"/>.</param>
    /// <param name="proportion">An array of float values specifying proportional sizes for each
    /// row or column.  Note that these proportions apply to the non-specified dimension -- that is,
    /// if <see paramref="isColumnSpecified"/> is true, then these proportions apply to the row
    /// heights, and if <see paramref="isColumnSpecified"/> is false, then these proportions apply
    /// to the column widths.  The values in this array are arbitrary floats -- the dimension of
    /// any given row or column is that particular proportional value divided by the sum of all
    /// the values.  For example, let <see paramref="isColumnSpecified"/> be true, and
    /// <see paramref="proportion"/> is an array with values of { 1.0, 2.0, 3.0 }.  The sum of
    /// those values is 6.0.  Therefore, the first row is 1/6th of the available height, the
    /// second row is 2/6th's of the available height, and the third row is 3/6th's of the
    /// available height.
    /// </param>
    /// <seealso cref="SetLayout(Graphics,PaneLayout)" />
    /// <seealso cref="SetLayout(Graphics,int,int)" />
    /// <seealso cref="SetLayout(Graphics,bool,int[])" />
    public void SetLayout(Graphics g, bool isColumnSpecified, int[] countList)
    {
      SetLayout(g, isColumnSpecified, doProportions(countList));
    }

    /// <summary>
    /// Set the proportions of each <see cref="GraphPane"/>'s <see cref="PaneBase.Rect"/> in
    /// the list to the specified configuration.  This method is suitable only for single
    /// column or single row configurations.
    /// </summary>
    /// <remarks>E.g. for a three rows configurations with proportions of rows 2/6, 1/6, and 3/6,
    /// set <see paramref="proportion"/> to <code>{2, 1, 3}</code></remarks>
    public void SetLayout(Graphics g, bool isColumnSpecified, float[] proportions)
    {
      setLayout(g, isColumnSpecified,
        proportions.Aggregate(
          new List<Tuple<float, float[]>>(),
          (list, f) => { list.Add(new Tuple<float, float[]>(f, null)); return list; }).ToArray()
      );
    }

    /// <summary>
    /// Set the proportions of each <see cref="GraphPane"/>'s <see cref="PaneBase.Rect"/> in
    /// the list to the specified configuration.  If a <see cref="SplitterPane"/> is needed
    /// to be inserted, use the value 0.0 for its proportion.
    /// </summary>
    /// Example:
    /// <code>
    /// /*
    ///  Configuration with 3 columns containing: 1 pane, 1 splitter, 2 panes with splitters, 3 panes:
    ///  +--------+ | +----+ +-----------+
    ///  |        | | |    | |           |
    ///  |        | | |    | |           |
    ///  |        | | |    | +-----------+
    ///  |        | | +----+ +-----------+
    ///  |        | | ------ |           |
    ///  |        | | +----+ |           |
    ///  |        | | |    | +-----------+
    ///  |        | | |    | +-----------+
    ///  |        | | |    | |           |
    ///  |        | | |    | |           |
    ///  +--------+ | +----+ +-----------+
    /// */
    /// var proportions = new[]
    /// {
    ///   new Tuple<float, float[]>(2f, new[] {1f}),
    ///   new Tuple<float, float[]>(0f, null),                // Vertical splitter
    ///   new Tuple<float, float[]>(1f, new[] {1f, 0f, 1f}),  // Column with 2 row being a horizontal splitter
    ///   new Tuple<float, float[]>(3f, new[] {1f, 1f, 1f})
    /// };
    /// </code>
    /// <param name="g">Graphics reference</param>
    /// <param name="isColumnSpecified">When true, then given proportions apply to the row
    /// heights, and when false, then these proportions apply to the column widths.</param>
    /// <param name="proportions">Array of tuples, where the first element contains a proportion
    /// of the corresponding column (when <see paramref="isColumnSpecified"/> is true) or row
    /// (when <see paramref="isColumnSpecified"/> is false), and the second element of a tuple
    /// contains proportions of corresponding rows (or columns) within the column/row.</param>
    public void SetLayout(Graphics g, bool isColumnSpecified, Tuple<float, float[]>[] proportions)
    {
      setLayout(g, isColumnSpecified, proportions);
    }

    /// <summary>
    /// Adjust proportions of panels ajacent to given splitter.
    /// </summary>
    /// <param name="splitter">The splitter pane being moved</param>
    /// <param name="proportion">The new proportional split in range (0.0 ... 1.0) of adjacent
    /// panels</param>
    public void SetProportion(SplitterPane splitter, float proportion)
    {
      if (proportion <= 0.0 || proportion >= 1.0)
        return;

      if (splitter.PaneIndex < 1 || splitter.PaneIndex >= PaneList.Count-1)
        return;

      var idx  = splitter.PaneIndex - 1;
      var pane = 0;

      for (int ii = 0; ii < _prop.Length; ++ii)
      {
        var pi = _prop[ii];

        if (pi.Item2 == null || pane == idx && ii + 1 < _prop.Length && Math.Abs(_prop[ii + 1].Item1) < float.Epsilon)
        {
          if (pane < idx)
          {
            pane++;
            continue;
          }

          if (ii + 2 >= _prop.Length)
            return;
          var next = _prop[ii + 2];
          var sum = pi.Item1 + next.Item1;
          _prop[ii]     = new Tuple<float, float[]>(sum * proportion, pi.Item2);
          _prop[ii + 2] = new Tuple<float, float[]>(sum * (1f - proportion), next.Item2);
          break;
        }
        else
        {
          var inc = pi.Item2.Length;
          if (pane + inc < idx)
          {
            pane += inc;
            continue;
          }

          var jj = idx - pane;

          if (jj + 2 >= pi.Item2.Length)
            return;

          var prev = pi.Item2[jj];
          var next = pi.Item2[jj + 2];
          var sum = prev + next;
          pi.Item2[jj] = sum * proportion;
          pi.Item2[jj + 2] = sum * (1f - proportion);
          _prop[ii] = new Tuple<float, float[]>(pi.Item1, pi.Item2);
          break;
        }
      }

      _layoutChanged = true;
    }

    #endregion

    #region Private Methods

    private void setLayout(Graphics g, bool isColumnSpecified,
                           Tuple<float, float[]>[] proportions)
    {
      InitLayout();

      // Validation check
      var cells         = proportions.Aggregate(0, (a,tuple) => a + (tuple.Item2?.Length ?? 1));
      var splitterCells = proportions.Aggregate(0,
        (a, tuple) => a + (Math.Abs(tuple.Item1) < float.Epsilon
                            ? 1 : (tuple.Item2?.Count(x => Math.Abs(x) < float.Epsilon) ?? 0)));
      var rows          = proportions.Length;
      var splitterPanes = PaneList.Count(p => p is SplitterPane);
      var expectPanes   = PaneList.Count - splitterPanes;
      var panes         = cells - splitterCells;

      if (splitterCells != splitterPanes)
        throw new ArgumentException
          ($"Mismatch in the number of splitter panes ({splitterPanes}) and splitter cells ({splitterCells}) "+
           $"in the proportions list: {proportions}");

      if (panes != expectPanes)
        throw new ArgumentException
          ($"Invalid number of pane rows/cells: {rows}/{cells} of graphic panes: {expectPanes}, "+
           $"and pane count in proportions: {panes}");

      var pane = 0;
      foreach (var p in proportions)
      {
        if (p.Item1 != 0.0 && PaneList[pane] is SplitterPane)
          throw new ArgumentException($"Pane#{pane} is not expected to be a splitter!");
        if (p.Item1 == 0.0 && !(PaneList[pane] is SplitterPane))
          throw new ArgumentException($"Pane#{pane} is expected to be a splitter!");

        if (p.Item2 == null)
          pane++;
        else
          foreach (var pr in p.Item2)
          {
            if (pr != 0.0 && PaneList[pane] is SplitterPane)
              throw new ArgumentException($"Pane#{pane} is not expected to be a splitter!");
            if (pr == 0.0 && !(PaneList[pane] is SplitterPane))
              throw new ArgumentException($"Pane#{pane} is expected to be a splitter!");
            pane++;
          }
      }

      // Sum up the total proportional factors
      var sumDim1 = proportions.Aggregate(0f, (a, tuple) => a + tuple.Item1);
      // Make prop sum to 1.0
      if (sumDim1 > 0f)
        for (var j = 0; j < proportions.Length; ++j)
          if (proportions[j].Item2 != null) // It can be null if the row j is a splitter
          {
            var sumDim2 = proportions[j].Item2.Aggregate(0f, (a, f) => a + f);
            if (sumDim2 > 0f)
              proportions[j] = new Tuple<float, float[]>(
                proportions[j].Item1/sumDim1,
                proportions[j].Item2.Select(f => f/sumDim2).ToArray()
                );
          }

      _isColumnSpecified = isColumnSpecified;
      _prop = proportions;

      doLayout(g);
    }

    /// <summary>
    /// Modify the <see cref="GraphPane" /> <see cref="PaneBase.Rect" /> sizes of each
    /// <see cref="GraphPane" /> such that they fit within the <see cref="MasterPane" />
    /// in a pre-configured layout.
    /// </summary>
    /// <remarks>The <see cref="SetLayout(Graphics,PaneLayout)" /> method (and overloads) is
    /// used for setting the layout configuration.</remarks>
    /// <seealso cref="SetLayout(Graphics,PaneLayout)" />
    /// <seealso cref="SetLayout(Graphics,int,int)" />
    /// <seealso cref="SetLayout(Graphics,bool,int[])" />
    /// <seealso cref="SetLayout(Graphics,bool,int[],float[])" />
    private Tuple<float, float[]>[] createProportions()
    {
      var count = PaneList.Count(p => !(p is SplitterPane));
      if (count == 0)
        return new Tuple<float, float[]>[] {};

      int rows, cols, root = (int)(Math.Sqrt(count) + 0.9999999);

      switch (_paneLayout)
      {
        case PaneLayout.ForceSquare:
          return doProportions(root, root);

        case PaneLayout.SingleColumn:
          _isColumnSpecified = true;
          return doProportions(count, 1);

        case PaneLayout.SingleRow:
          _isColumnSpecified = false;
          return doProportions(1, count);

        case PaneLayout.SquareRowPreferred:
          rows = root;
          cols = count <= root*(root - 1) ? root - 1 : root;
          return doProportions(rows, cols);

        case PaneLayout.ExplicitCol12:
          _isColumnSpecified = true;
          return doProportions(new[] {1, 2});

        case PaneLayout.ExplicitCol21:
          _isColumnSpecified = true;
          return doProportions(new[] {2, 1});

        case PaneLayout.ExplicitCol23:
          _isColumnSpecified = true;
          return doProportions(new[] {2, 3});

        case PaneLayout.ExplicitCol32:
          _isColumnSpecified = true;
          return doProportions(new[] {3, 2});

        case PaneLayout.ExplicitRow12:
          _isColumnSpecified = false;
          return doProportions(new[] {1, 2});

        case PaneLayout.ExplicitRow21:
          _isColumnSpecified = false;
          return doProportions(new[] {2, 1});

        case PaneLayout.ExplicitRow23:
          _isColumnSpecified = false;
          return doProportions(new[] {2, 3});

        case PaneLayout.ExplicitRow32:
          _isColumnSpecified = false;
          return doProportions(new[] {3, 2});

        case PaneLayout.SquareColPreferred:
        default:
          rows = count <= root*(root - 1) ? root - 1 : root;
          cols = root;
          return doProportions(rows, cols);
      }
    }

    /// <summary>
    /// Internal method that applies a previously set layout with a rows per column or
    /// columns per row configuration.  This method is only called by
    /// <see cref="DoLayout(Graphics)" />.
    /// </summary>
    private void doLayout(Graphics g)
    {
      if (_prop == null)
        _prop = createProportions();

      Func<PaneList, int, bool> isSplitter = (list, i) =>
                                             i < list.Count && list[i] is SplitterPane;

      // calculate scaleFactor on "normal" pane size (BaseDimension)
      var scaleFactor = CalcScaleFactor();

      // innerRect is the area for the GraphPane's
      var innerRect = CalcClientRect(g, scaleFactor);
      _legend.CalcRect(g, this, scaleFactor, ref innerRect);

      // scaled InnerGap is the area between the GraphPane.Rect's
      var scaledInnerGap = InnerPaneGap*scaleFactor;

      var iPane = 0;

      if (_isColumnSpecified)
      {
        var rows           = _prop.Length;
        var y              = innerRect.Y;
        var rowSplitters   = _prop.Count(tuple => Math.Abs(tuple.Item1) < float.Epsilon);
        var graphPaneRows  = rows - rowSplitters;
        var graphPaneVGaps = graphPaneRows - rowSplitters - 1;

        foreach (var prow in _prop)
        {
          int cols, colSplitters, graphPaneCols, graphPaneHGaps;
          if (prow.Item2 == null)
          {
            cols           = 1;
            colSplitters   = Math.Abs(prow.Item1) < float.Epsilon ? 1 : 0;
            graphPaneCols  = cols - colSplitters;
            graphPaneHGaps = 0;
          }
          else
          {
            cols           = prow.Item2.Length;
            colSplitters   = prow.Item2.Count(n => Math.Abs(n) < float.Epsilon);
            graphPaneCols  = cols - colSplitters;
            graphPaneHGaps = graphPaneCols - colSplitters - 1;
          }

          var thisHPaneSplitter = this[iPane] is SplitterPane;
          if (thisHPaneSplitter)
            ((SplitterPane)this[iPane]).PaneIndex = iPane;

          var width = graphPaneCols == 0
                        ? innerRect.Width
                        : prow.Item1 < float.Epsilon
                          ? PaneSplitterSize
                          : (innerRect.Width - graphPaneHGaps*scaledInnerGap -
                             colSplitters*PaneSplitterSize);

          var defWidth = graphPaneCols == 0 ? width : width / graphPaneCols;

          var height = graphPaneRows == 0
                        ? innerRect.Height
                        : prow.Item1 < float.Epsilon
                          ? PaneSplitterSize
                          : (innerRect.Height - graphPaneVGaps * scaledInnerGap -
                             rowSplitters * PaneSplitterSize) * prow.Item1;

          var x = innerRect.X;

          for (var col = 0; col < cols; col++)
          {
            if (iPane >= PaneList.Count)
              return;

            var thisVPaneSplitter = this[iPane] is SplitterPane;
            if (thisVPaneSplitter)
              ((SplitterPane)this[iPane]).PaneIndex = iPane;
            var w = prow.Item2 == null
                      ? defWidth
                      : prow.Item2[col] < float.Epsilon
                        ? PaneSplitterSize
                        : width * prow.Item2[col];

            this[iPane++].Rect = new RectangleF(x, y, w, height);

            x += w;

            if (!thisVPaneSplitter && !isSplitter(PaneList, iPane))
              x += scaledInnerGap;
          }

          y += height;

          if (!thisHPaneSplitter && !isSplitter(PaneList, iPane))
            y += scaledInnerGap;
        }
      }
      else
      {
        var cols           = _prop.Length;
        var x              = innerRect.X;
        var colSplitters   = _prop.Count(tuple => Math.Abs(tuple.Item1) < float.Epsilon);
        var graphPaneCols  = cols - colSplitters;
        var graphPaneHGaps = graphPaneCols - colSplitters - 1;

        foreach (var pcol in _prop)
        {
          int rows, rowSplitters, graphPaneRows, graphPaneVGaps;
          if (pcol.Item2 == null)
          {
            rows           = 1;
            rowSplitters   = Math.Abs(pcol.Item1) < float.Epsilon ? 1 : 0;
            graphPaneRows  = rows - rowSplitters;
            graphPaneVGaps = 0;
          }
          else
          {
            rows           = pcol.Item2.Length;
            rowSplitters   = pcol.Item2.Count(n => Math.Abs(n) < float.Epsilon);
            graphPaneRows  = rows - rowSplitters;
            graphPaneVGaps = graphPaneRows - rowSplitters - 1;
          }

          var thisVPaneSplitter = this[iPane] is SplitterPane;
          if (thisVPaneSplitter)
            ((SplitterPane)this[iPane]).PaneIndex = iPane;

          var height = graphPaneRows == 0
                         ? innerRect.Height
                         : thisVPaneSplitter
                           ? PaneSplitterSize
                           : (innerRect.Height -
                              graphPaneVGaps*scaledInnerGap -
                              rowSplitters*PaneSplitterSize);

          var defHeight = graphPaneRows == 0 ? height : height / graphPaneRows;

          var width = graphPaneCols == 0
                        ? innerRect.Width
                        : pcol.Item1 < float.Epsilon
                          ? PaneSplitterSize
                          : (innerRect.Width - graphPaneHGaps*scaledInnerGap -
                             colSplitters*PaneSplitterSize) * pcol.Item1;

          var y = innerRect.Y;

          for (var row = 0; row < rows; row++)
          {
            if (iPane >= PaneList.Count)
              return;

            var thisHPaneSplitter = this[iPane] is SplitterPane;
            if (thisHPaneSplitter)
              ((SplitterPane)this[iPane]).PaneIndex = iPane;
            var h = pcol.Item2 == null
                      ? defHeight
                      : pcol.Item2[row] < float.Epsilon
                        ? PaneSplitterSize
                        : height * pcol.Item2[row];

            this[iPane++].Rect = new RectangleF(x, y, width, h);

            y += h;

            if (!thisHPaneSplitter && !isSplitter(PaneList, iPane))
              y += scaledInnerGap;
          }

          x += width;

          if (!thisVPaneSplitter && !isSplitter(PaneList, iPane))
            x += scaledInnerGap;
        }
      }

      _layoutChanged = false;
    }

    private Tuple<float, float[]>[] doProportions(int rows, int cols)
    {
      return doProportions(Enumerable.Repeat(cols, rows).ToArray());
    }

    private Tuple<float, float[]>[] doProportions(IReadOnlyList<int> countList)
    {
      var cells = countList.Aggregate(0, (a, n) => a + n);
      var rows = countList.Count;
      var splitters = PaneList.Count(p => p is SplitterPane);
      var panes = PaneList.Count - splitters;

      if (cells != panes)
        throw new ArgumentException
          ($"Invalid number of pane rows/cells: {rows}/{cells} (graphic panes: {panes})");

      if (panes == 1)
        return new[] {new Tuple<float, float[]>(1f, null)};

      var prop = new List<Tuple<float, float[]>>();
      int pane = 0, i = 0;
      while (i < rows && pane < PaneList.Count)
      {
        if (PaneList[pane] is SplitterPane)
        {
          pane++;
          prop.Add(new Tuple<float, float[]>(0f, null));
        }
        else
        {
          var j = 0;
          var list = new List<float>();
          while (j < countList[i] && pane < PaneList.Count)
          {
            if (PaneList[pane++] is SplitterPane)
              list.Add(0f);
            else
            {
              list.Add(1f);
              j++;
            }
          }
          // Enumerable.Repeat(1.0f, cols).ToArray()
          prop.Add(new Tuple<float, float[]>(1f, list.ToArray()));
          i++;
        }
      }

      if (pane != PaneList.Count)
        throw new ArgumentException
          ($"Invalid number of splitters: countList={countList}, panes={panes}, splitters={splitters}");

      // Sum up the total proportional factors
      var sumDim1 = prop.Aggregate(0f, (a, tuple) => a + tuple.Item1);
      // Make prop sum to 1.0
      if (sumDim1 > 0f)
        for (var j = 0; j < prop.Count; ++j)
          if (prop[j].Item2 != null) // It can be null if the row j is a splitter
          {
            var sumDim2 = prop[j].Item2.Aggregate(0f, (a, f) => a + f);
            if (sumDim2 > 0f)
              prop[j] = new Tuple<float, float[]>(prop[j].Item1/sumDim1,
                                                  prop[j].Item2.Select(f => f/sumDim2)
                                                         .ToArray());
          }

      return prop.ToArray();
    }

    #endregion

  }
}