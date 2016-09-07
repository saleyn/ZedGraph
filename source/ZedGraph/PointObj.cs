//============================================================================
// Author: Serge Aleynikov (2016-09-01)
// Author: James Dunkerley (2010-01-08)
// see:    https://sourceforge.net/p/zedgraph/feature-requests/79/
//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright (c) 2004 John Champion
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

namespace ZedGraph
{
  /// <summary>
  /// A class that represents a marker at a point on the graph.  
  /// A list of PointObj objects is maintained by the <see cref="GraphObjList"/> collection class.
  /// Based on BoxObj
  /// </summary>
  [Serializable]
  public class PointObj : GraphObj, ICloneable
  {
    #region Defaults
    /// <summary>
    /// A simple struct that defines the
    /// default property values for the <see cref="PointObj"/> class.
    /// </summary>
    new public struct Default
    {
      /// <summary>
      /// The default size of the <see cref="PointObj"/>.  Units are points.
      /// </summary>
      public static float Size = 5.0f;
      /// <summary>
      /// The default pen width to be used for drawing curve symbols
      /// (<see cref="ZedGraph.LineBase.Width"/> property).  Units are points.
      /// </summary>
      public static float PenWidth = Symbol.Default.PenWidth;
      /// <summary>
      /// The default color for filling in this <see cref="Symbol"/>
      /// (<see cref="ZedGraph.Fill.Color"/> property).
      /// </summary>
      public static Color FillColor = Symbol.Default.FillColor;
      /// <summary>
      /// The default custom brush for filling in this <see cref="Symbol"/>
      /// (<see cref="ZedGraph.Fill.Brush"/> property).
      /// </summary>
      public static Brush FillBrush = Symbol.Default.FillBrush;
      /// <summary>
      /// The default fill mode for the curve (<see cref="ZedGraph.Fill.Type"/> property).
      /// </summary>
      public static FillType FillType = Symbol.Default.FillType;
      /// <summary>
      /// The default for drawing frames around symbols (<see cref="ZedGraph.LineBase.IsVisible"/> property).
      /// true to display symbol frames, false to hide them.
      /// </summary>
      public static bool IsBorderVisible = Symbol.Default.IsBorderVisible;
      /// <summary>
      /// The default color for drawing symbols (<see cref="ZedGraph.LineBase.Color"/> property).
      /// </summary>
      public static Color BorderColor = Symbol.Default.BorderColor;
      /// <summary>
      /// The default SymbolType used for the <see cref="PointObj"/> symboltype property.
      /// </summary>
      public static SymbolType Type = SymbolType.XCross;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the size of the <see cref="Symbol"/>
    /// </summary>
    /// <value>Size in points (1/72 inch)</value>
    /// <seealso cref="Default.Size"/>
    public float Size
    {
      get { return (float)Location.Height; }
      set { Location.Height = Location.Width = value; }
    }

    /// <summary>
    /// Gets or sets the <see cref="ZedGraph.Fill"/> data for this
    /// <see cref="Symbol"/>.
    /// </summary>
    public Fill Fill { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="ZedGraph.Border"/> data for this
    /// <see cref="Symbol"/>, which controls the border outline of the symbol.
    /// </summary>
    public Border Border { get; set; }

    /// <summary>
    /// Gets or sets the type (shape) of the <see cref="Symbol"/>
    /// </summary>
    /// <value>A <see cref="SymbolType"/> enum value indicating the shape</value>
    /// <seealso cref="Default.Type"/>
    public SymbolType Type { get; set; }

    #endregion

    #region Constructors

    /// <overloads>Constructors for the <see cref="PointObj"/> object</overloads>
    /// <summary>
    /// A constructor that allows the position, size, and color of the <see cref="PointObj"/> to be specified.
    /// </summary>
    /// <param name="x">The x location for this <see cref="PointObj" />.  This will be in units determined by <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    /// <param name="y">The y location for this <see cref="PointObj" />.  This will be in units determined by <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    /// <param name="type">A <see cref="SymbolType"/> enum value indicating the shape of the symbol</param>
    /// <param name="size">The size of the <see cref="PointObj" /> in Pixels</param>
    /// <param name="color">An arbitrary <see cref="System.Drawing.Color"/> specification for the poit</param>
    public PointObj(double x, double y, double size, SymbolType type, Color color)
      : base(x, y, size, size)
    {
      Type   = type;
      Border = new Border(Default.IsBorderVisible, color, Default.PenWidth);
      Fill   = new Fill(color, Default.FillBrush, Default.FillType);
    }

    /// <summary>
    /// A constructor that allows the position of the <see cref="PointObj"/> to be pre-specified.  Other properties are defaulted.
    /// </summary>
    /// <param name="x">The x location for this <see cref="PointObj" />.  This will be in units determined by <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    /// <param name="y">The y location for this <see cref="PointObj" />.  This will be in units determined by <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    /// <param name="size">The size of the <see cref="PointObj" /> in Pixels</param>
    public PointObj(double x, double y, double size)
        : this(x, y, size, Default.Type, Default.FillColor)
    {}

    /// <summary>
    /// A constructor that allows the position of the <see cref="PointObj"/> to be pre-specified.  Other properties are defaulted.
    /// </summary>
    /// <param name="x">The x location for this <see cref="PointObj" />.  This will be in units determined by <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    /// <param name="y">The y location for this <see cref="PointObj" />.  This will be in units determined by <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    /// <param name="type">A <see cref="SymbolType"/> enum value indicating the shape of the symbol</param>
    /// <param name="color">An arbitrary <see cref="System.Drawing.Color"/> specification for the poit</param>
    public PointObj(double x, double y, SymbolType type, Color color)
      : this(x, y, Default.Size, type, color)
    {}

    /// <summary>
    /// A default constructor that creates a <see cref="PointObj"/> using a location of (0,0),
    /// and a width,height of (1,1).  Other properties are defaulted.
    /// </summary>
    public PointObj() : this(0, 0, Default.Size)
    {}

    /// <summary>
    /// The Copy Constructor
    /// </summary>
    /// <param name="rhs">The <see cref="PointObj"/> object from which to copy</param>
    public PointObj(PointObj rhs)
      : base(rhs)
    {
      Border = rhs.Border.Clone();
      Fill   = rhs.Fill.Clone();
      Type   = rhs.Type;
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
    public PointObj Clone()
    {
      return new PointObj(this);
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
    protected PointObj(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      int sch = info.GetInt32("schema2");

      Type = (SymbolType)info.GetValue("type", typeof(SymbolType));
      Fill = (Fill)info.GetValue("fill", typeof(Fill));
      Border = (Border)info.GetValue("border", typeof(Border));
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
      info.AddValue("schema2", schema2);
      info.AddValue("type", Type);
      info.AddValue("fill", Fill);
      info.AddValue("border", Border);
    }
    #endregion

    #region Rendering Methods
    private RectangleF GetPointRect(PaneBase pane, float scaleFactor)
    {
      var pixRect = this.Location.TransformRect(pane);
      var w = ((float)this.Location.Width)  * scaleFactor / 2.0f;
      var h = ((float)this.Location.Height) * scaleFactor / 2.0f;
      return new RectangleF(pixRect.Left - w, pixRect.Top - h, 2f * w, 2f * h);
    }

    private Point GetLocation(GraphPane pane, float scaleFactor)
    {
      var yAxis = GetYAxis(pane);
      var xAxis = GetXAxis(pane);

      // Convert the coordinates from the user coordinate system
      // to the screen coordinate system
      return new Point((int)Math.Round(xAxis.Scale.Transform(Location.X) * scaleFactor),
                       (int)Math.Round(yAxis.Scale.Transform(Location.Y) * scaleFactor));
    }

    /// <summary>
    /// Render this object to the specified <see cref="Graphics"/> device.
    /// </summary>
    /// <remarks>
    /// This method is normally only called by the Draw method
    /// of the parent <see cref="GraphObjList"/> collection object.
    /// </remarks>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally e.Graphics from the
    /// PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="pane">
    /// A reference to the <see cref="PaneBase"/> object that is the parent or
    /// owner of this object.
    /// </param>
    /// <param name="scaleFactor">
    /// The scaling factor to be used for rendering objects.  This is calculated and
    /// passed down by the parent <see cref="GraphPane"/> object using the
    /// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
    /// font sizes, etc. according to the actual size of the graph.
    /// </param>
    override public void Draw(Graphics g, PaneBase pane, float scaleFactor)
    {
      var gPane = pane as GraphPane;
      if (pane == null) return;

      // Convert the arrow coordinates from the user coordinate system
      // to the screen coordinate system
      var pix = GetLocation(gPane, scaleFactor);

      Symbol.DrawSymbol(g, gPane, Type, pix.X, pix.Y, Size, IsVisible, Fill, Border, scaleFactor, false, new PointPair());
    }

    /// <summary>
    /// Determine if the specified screen point lies inside the bounding box of this
    /// <see cref="BoxObj"/>.
    /// </summary>
    /// <param name="pt">The screen point, in pixels</param>
    /// <param name="pane">
    /// A reference to the <see cref="PaneBase"/> object that is the parent or
    /// owner of this object.
    /// </param>
    /// <param name="g">
    /// A graphic device object to be drawn into.  This is normally e.Graphics from the
    /// PaintEventArgs argument to the Paint() method.
    /// </param>
    /// <param name="scaleFactor">
    /// The scaling factor to be used for rendering objects.  This is calculated and
    /// passed down by the parent <see cref="GraphPane"/> object using the
    /// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
    /// font sizes, etc. according to the actual size of the graph.
    /// </param>
    /// <returns>true if the point lies in the bounding box, false otherwise</returns>
    override public bool PointInBox(PointF pt, PaneBase pane, Graphics g, float scaleFactor)
    {
      if (!base.PointInBox(pt, pane, g, scaleFactor))
        return false;

      // transform the x,y location from the user-defined
      // coordinate frame to the screen pixel location
      RectangleF pixRect = this.GetPointRect(pane, scaleFactor);

      return pixRect.Contains(pt);
    }

    /// <summary>
    /// Determines the shape type and Coords values for this GraphObj
    /// </summary>
    override public void GetCoords(PaneBase pane, Graphics g, float scaleFactor,
            out string shape, out string coords)
    {
      // transform the x,y location from the user-defined
      // coordinate frame to the screen pixel location
      RectangleF pixRect = this.GetPointRect(pane, scaleFactor);

      shape = "rect";
      coords = String.Format("{0:f0},{1:f0},{2:f0},{3:f0}",
                  pixRect.Left, pixRect.Top, pixRect.Right, pixRect.Bottom);
    }
    #endregion
  }
}