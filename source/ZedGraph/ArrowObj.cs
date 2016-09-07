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
  /// A class that represents a graphic arrow or line object on the graph.  A list of
  /// ArrowObj objects is maintained by the <see cref="GraphObjList"/> collection class.
  /// </summary>
  /// 
  /// <author> John Champion </author>
  /// <version> $Revision: 3.4 $ $Date: 2007-01-25 07:56:08 $ </version>
  [Serializable]
  public class ArrowObj : LineObj, ICloneable, ISerializable
  {
    #region Fields
    /// <summary>
    /// Private field that stores the arrowhead size, measured in points.
    /// Use the public property <see cref="Size"/> to access this value.
    /// </summary>
    private float _size;
    /// <summary>
    /// Private boolean field that stores the arrowhead state.
    /// Use the public property <see cref="IsArrowHead"/> to access this value.
    /// </summary>
    /// <value> true if an arrowhead is to be drawn, false otherwise </value>
    private bool _isArrowHead;

    /// <summary>
    /// Private boolean field that stores the arrowhead width factor.
    /// Use the public property <see cref="ArrowHeadFactor"/> to access this value.
    /// </summary>
    /// <value>The larger the value the more elongated the arrow head is. Default: 2.0</value>
    private float _arrowHeadFactor;

    #endregion

    #region Defaults
    /// <summary>
    /// A simple struct that defines the
    /// default property values for the <see cref="ArrowObj"/> class.
    /// </summary>
    new public struct Default
    {
      /// <summary>
      /// The default size for the <see cref="ArrowObj"/> item arrowhead
      /// (<see cref="ArrowObj.Size"/> property).  Units are in points (1/72 inch).
      /// </summary>
      public static float Size = 12.0F;
      /// <summary>
      /// The default display mode for the <see cref="ArrowObj"/> item arrowhead
      /// (<see cref="ArrowObj.IsArrowHead"/> property).  true to show the
      /// arrowhead, false to hide it.
      /// </summary>
      public static bool IsArrowHead = true;

      /// <summary>
      /// The default factor of arrow head's width. The larger the value, the more
      /// elongated the arrow head is.
      /// </summary>
      public static float ArrowHeadFactor = 2.0f;
    }
    #endregion

    #region Properties
    /// <summary>
    /// The size of the arrowhead.
    /// </summary>
    /// <remarks>The display of the arrowhead can be
    /// enabled or disabled with the <see cref="IsArrowHead"/> property.
    /// </remarks>
    /// <value> The size is defined in points (1/72 inch) </value>
    /// <seealso cref="Default.Size"/>
    public float Size
    {
      get { return _size; }
      set { _size = value; }
    }
    /// <summary>
    /// Determines whether or not to draw an arrowhead
    /// </summary>
    /// <value> true to show the arrowhead, false to show the line segment
    /// only</value>
    /// <seealso cref="Default.IsArrowHead"/>
    public bool IsArrowHead
    {
      get { return _isArrowHead; }
      set { _isArrowHead = value; }
    }

    /// <summary>
    /// Parameter that controls the width of arrow head
    /// </summary>
    public float ArrowHeadFactor
    {
      get { return _arrowHeadFactor;                  }
      set { _arrowHeadFactor = Math.Max(0.1f, value); }
    }

    /// <summary>
    /// Fill of the arrow object that controlls how it is painted
    /// </summary>
    public Fill Fill => _line.GradientFill;

    #endregion

    #region Constructors
    /// <overloads>Constructors for the <see cref="ArrowObj"/> object</overloads>
    /// <summary>
    /// A constructor that allows the position, color, and size of the
    /// <see cref="ArrowObj"/> to be pre-specified.
    /// </summary>
    /// <param name="color">An arbitrary <see cref="System.Drawing.Color"/> specification
    /// for the arrow</param>
    /// <param name="size">The size of the arrowhead, measured in points.</param>
    /// <param name="x1">The x position of the starting point that defines the
    /// arrow.  The units of this position are specified by the
    /// <see cref="Location.CoordinateFrame"/> property.</param>
    /// <param name="y1">The y position of the starting point that defines the
    /// arrow.  The units of this position are specified by the
    /// <see cref="Location.CoordinateFrame"/> property.</param>
    /// <param name="x2">The x position of the ending point that defines the
    /// arrow.  The units of this position are specified by the
    /// <see cref="Location.CoordinateFrame"/> property.</param>
    /// <param name="y2">The y position of the ending point that defines the
    /// arrow.  The units of this position are specified by the
    /// <see cref="Location.CoordinateFrame"/> property.</param>
    public ArrowObj( Color color, float size,
                     double x1, double y1, double x2, double y2 )
      : base( color, x1, y1, x2, y2 )
    {
      _isArrowHead     = Default.IsArrowHead;
      _size            = size;
      _arrowHeadFactor = Default.ArrowHeadFactor;
      Fill.Type        = FillType.Solid;
      Fill.Brush       = new SolidBrush(color);
      Fill.Color       = color;
      Fill.SecondaryValueGradientColor = color;
    }

    /// <summary>
    /// A constructor that allows only the position of the
    /// arrow to be pre-specified.  All other properties are set to
    /// default values
    /// </summary>
    /// <param name="x1">The x position of the starting point that defines the
    /// <see cref="ArrowObj"/>.  The units of this position are specified by the
    /// <see cref="Location.CoordinateFrame"/> property.</param>
    /// <param name="y1">The y position of the starting point that defines the
    /// <see cref="ArrowObj"/>.  The units of this position are specified by the
    /// <see cref="Location.CoordinateFrame"/> property.</param>
    /// <param name="x2">The x position of the ending point that defines the
    /// <see cref="ArrowObj"/>.  The units of this position are specified by the
    /// <see cref="Location.CoordinateFrame"/> property.</param>
    /// <param name="y2">The y position of the ending point that defines the
    /// <see cref="ArrowObj"/>.  The units of this position are specified by the
    /// <see cref="Location.CoordinateFrame"/> property.</param>
    public ArrowObj( double x1, double y1, double x2, double y2 )
      : this( LineBase.Default.Color, Default.Size, x1, y1, x2, y2 )
    {}

    /// <summary>
    /// Default constructor -- places the <see cref="ArrowObj"/> at location
    /// (0,0) to (1,1).  All other values are defaulted.
    /// </summary>
    public ArrowObj() : this( LineBase.Default.Color, Default.Size, 0, 0, 1, 1 )
    {}

    /// <summary>
    /// The Copy Constructor
    /// </summary>
    /// <param name="rhs">The <see cref="ArrowObj"/> object from which to copy</param>
    public ArrowObj( ArrowObj rhs ) : base( rhs )
    {
      _size = rhs.Size;
      _isArrowHead = rhs.IsArrowHead;
      _arrowHeadFactor = rhs._arrowHeadFactor;
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
    public new ArrowObj Clone()
    {
      return new ArrowObj( this );
    }

    #endregion

    #region Serialization
    /// <summary>
    /// Current schema value that defines the version of the serialized file
    /// </summary>
    public const int schema3 = 10;

    /// <summary>
    /// Constructor for deserializing objects
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
    /// </param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
    /// </param>
    protected ArrowObj( SerializationInfo info, StreamingContext context )
      : base( info, context )
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      int sch = info.GetInt32( "schema3" );

      _size = info.GetSingle( "size" );
      _isArrowHead = info.GetBoolean( "isArrowHead" );
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
      info.AddValue( "schema3", schema3 );
      info.AddValue( "size", _size );
      info.AddValue( "isArrowHead", _isArrowHead );
    }
    #endregion

    #region Rendering Methods
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
    override public void Draw( Graphics g, PaneBase pane, float scaleFactor )
    {
      var pn = pane as GraphPane;
      if (pn == null) return;

      var yAxis = GetYAxis(pn);
      var xAxis = GetXAxis(pn);

      // Convert the arrow coordinates from the user coordinate system
      // to the screen coordinate system
      var pix1 = new PointF(xAxis.Scale.Transform(Location.X),
                            yAxis.Scale.Transform(Location.Y));
      var pix2 = new PointF(xAxis.Scale.Transform(Location.X2),
                            yAxis.Scale.Transform(Location.Y2));

      //PointF pix1 = this.Location.TransformTopLeft( pane );
      //PointF pix2 = this.Location.TransformBottomRight( pane );

      if (Math.Abs(pix1.X) > -100000 || Math.Abs(pix1.Y) > 100000 ||
          Math.Abs(pix2.X) > -100000 || Math.Abs(pix2.Y) > 100000)
        return;

      // get a scaled size for the arrowhead
      var sz         = Math.Abs(_size);
      var scaledSize = sz * scaleFactor;
      var halfSize   = scaledSize / 2f;

      // calculate the length and the angle of the arrow "vector"
      var dy = pix2.Y - pix1.Y;
      var dx = pix2.X - pix1.X;
      var length = (float)(Math.Abs(dx) < float.Epsilon ? Math.Sqrt( dx * dx + dy * dy ) : dx);

      // Save the old transform matrix
      Matrix transform = g.Transform;
      // Move the coordinate system so it is located at the ending point
      // of this arrow
      g.TranslateTransform( pix2.X, pix2.Y );

      if (Math.Abs(dy) > float.Epsilon)
      {
        var angle = (float)Math.Atan2( dy, dx ) * 180.0F / (float)Math.PI;
        // Rotate the coordinate system according to the angle of this arrow
        // about the starting point
        g.RotateTransform(angle);
      }

      // Only show the arrowhead if required
      if ( _isArrowHead )
      {
        // Create a polygon representing the arrowhead based on the scaled
        // size
        var hsize     = scaledSize / _arrowHeadFactor;
        var headLen   = scaledSize;
        PointF[] polyPt =
        {
          new PointF(0,        0), 
          new PointF(headLen,  halfSize+hsize), 
          new PointF(headLen,  halfSize), 
          new PointF(length,   halfSize), 
          new PointF(length,  -halfSize),
          new PointF(headLen, -halfSize),
          new PointF(headLen, -halfSize-hsize),
          new PointF(0,        0)
        };

        // get a pen according to this arrow properties, and render the arrow
        using (var pen = _line.GetPen(pane, scaleFactor))
          if (Fill.Type == FillType.None)
            g.DrawPolygon(pen, polyPt);
          else
            g.FillPolygon(Fill.Brush, polyPt);
      }
      else if (Fill.Type != FillType.None)
        // get a pen according to this arrow properties, and render the body
        using (var pen = _line.GetPen(pane, scaleFactor))
          g.DrawRectangle(pen, 0, -halfSize, length, scaledSize);
      else
        g.FillRectangle(Fill.Brush, 0, -halfSize, length, scaledSize);

      // Restore the transform matrix back to its original state
      g.Transform = transform;
    }

    #endregion

  }
}
