//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright ?2005  John Champion
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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
  /// <summary>
  /// A class that represents a bordered and/or filled polygon object on
  /// the graph.  A list of <see cref="PolyObj"/> objects is maintained by
  /// the <see cref="GraphObjList"/> collection class.
  /// </summary>
  /// 
  /// <author> John Champion </author>
  /// <version> $Revision: 3.4 $ $Date: 2007-01-25 07:56:09 $ </version>
  [Serializable]
  public class PolyObj : BoxObj, ICloneable, ISerializable
  {

  #region Fields
    private List<PointD> _points = new List<PointD>();

    /// <summary>
    /// private value that determines if the polygon will be automatically closed.
    /// true to close the figure, false to leave it "open."  Use the public property
    /// <see cref="IsClosedFigure" /> to access this value.
    /// </summary>
    private bool _isClosedFigure = true;
  #endregion

  #region Properties

    /// <summary>
    /// Gets or sets the <see cref="PointD"/> array that defines
    /// the polygon.  This will be in units determined by
    /// <see cref="ZedGraph.Location.CoordinateFrame"/>.
    /// </summary>
    public List<PointD> Points
    {
      get { return _points; }
      set { _points = value; }
    }

    /// <summary>
    /// Gets or sets a value that determines if the polygon will be automatically closed.
    /// true to close the figure, false to leave it "open."
    /// </summary>
    /// <remarks>
    /// This boolean determines whether or not the CloseFigure() method will be called
    /// to fully close the path of the polygon.  This value defaults to true, and for any
    /// closed figure it should fine.  If you want to draw a line that does not close into
    /// a shape, then you should set this value to false.  For a figure that is naturally
    /// closed (e.g., the first point of the polygon is the same as the last point),
    /// leaving this value set to false may result in minor pixel artifacts due to
    /// rounding.
    /// </remarks>
    public bool IsClosedFigure
    {
      get { return _isClosedFigure; }
      set { _isClosedFigure = value; }
    }

    #endregion

    #region Constructors
        /// <overloads>Constructors for the <see cref="PolyObj"/> object</overloads>
        /// <summary>
        /// A constructor that allows the position, border color, and solid fill color
        /// of the <see cref="PolyObj"/> to be pre-specified.
        /// </summary>
        /// <param name="borderColor">An arbitrary <see cref="System.Drawing.Color"/> specification
        /// for the box border</param>
        /// <param name="fillColor">An arbitrary <see cref="System.Drawing.Color"/> specification
        /// for the box fill (will be a solid color fill)</param>
        /// <param name="points">The <see cref="PointD"/> array that defines
        /// the polygon.  This will be in units determined by
        /// <see cref="ZedGraph.Location.CoordinateFrame"/>.
        /// </param>
        public PolyObj( PointD[] points, Color borderColor, Color fillColor ) :
        base( 0, 0, 1, 1, borderColor, fillColor )
    {
            if (points.Length > 0)
            {
                AddPoints(points);
            }
    }

    public PolyObj(PointD point, Color borderColor, Color fillColor) :
        base(0, 0, 1, 1, borderColor, fillColor)
    {
            //_points.Add(point);
            AddPoint(point);
    }

    /// <summary>
    /// A constructor that allows the position
    /// of the <see cref="PolyObj"/> to be pre-specified.  Other properties are defaulted.
    /// </summary>
    /// <param name="points">The <see cref="PointD"/> array that defines
    /// the polygon.  This will be in units determined by
    /// <see cref="ZedGraph.Location.CoordinateFrame"/>.
    /// </param>
    public PolyObj( PointD[] points ) : base( 0, 0, 1, 1 )
    {
            if (points.Length > 0)
            {
                AddPoints(points);
            }
        }

    /// <summary>
    /// A default constructor that creates a <see cref="PolyObj"/> from an empty
    /// <see cref="PointD"/> array.  Other properties are defaulted.
    /// </summary>
    public PolyObj() : this( new PointD[0] )
    {
    }

    /// <summary>
    /// A constructor that allows the position, border color, and two-color
    /// gradient fill colors
    /// of the <see cref="PolyObj"/> to be pre-specified.
    /// </summary>
    /// <param name="borderColor">An arbitrary <see cref="System.Drawing.Color"/> specification
    /// for the box border</param>
    /// <param name="fillColor1">An arbitrary <see cref="System.Drawing.Color"/> specification
    /// for the start of the box gradient fill</param>
    /// <param name="fillColor2">An arbitrary <see cref="System.Drawing.Color"/> specification
    /// for the end of the box gradient fill</param>
    /// <param name="points">The <see cref="PointD"/> array that defines
    /// the polygon.  This will be in units determined by
    /// <see cref="ZedGraph.Location.CoordinateFrame"/>.
    /// </param>
    public PolyObj( PointD[] points, Color borderColor,
              Color fillColor1, Color fillColor2 ) :
        base( 0, 0, 1, 1, borderColor, fillColor1, fillColor2 )
    {
            if (points.Length > 0)
            {
                AddPoints(points);
            }
        }

    /// <summary>
    /// The Copy Constructor
    /// </summary>
    /// <param name="rhs">The <see cref="PolyObj"/> object from which to copy</param>
    public PolyObj( PolyObj rhs ) : base( rhs )
    {
      rhs._points.AddRange(_points);
      rhs._isClosedFigure = _isClosedFigure;
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
    public new PolyObj Clone()
    {
      return new PolyObj( this );
    }

  #endregion

  #region Serialization
    /// <summary>
    /// Current schema value that defines the version of the serialized file
    /// </summary>
    public const int schema3 = 12;

    /// <summary>
    /// Constructor for deserializing objects
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
    /// </param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
    /// </param>
    protected PolyObj( SerializationInfo info, StreamingContext context ) : base( info, context )
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      int sch = info.GetInt32( "schema3" );
      
      if (sch >= 12) 
        _points = (List<PointD>) info.GetValue( "points", typeof(List<PointD>) );
      
      if ( schema3 >= 11 )
        _isClosedFigure = info.GetBoolean( "isClosedFigure" );

    }
    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
    [SecurityPermissionAttribute(SecurityAction.Demand,SerializationFormatter=true)]
    public override void GetObjectData( SerializationInfo info, StreamingContext context )
    {
      base.GetObjectData( info, context );
      info.AddValue( "schema3", schema3 );

      info.AddValue( "points", _points );
      info.AddValue( "isClosedFigure", _isClosedFigure );
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
      if ( _points != null && _points.Count > 1 )
      {
        using ( GraphicsPath path = MakePath( pane ) )
        {
          // Fill or draw the symbol as required
          if ( _fill.IsVisible )
          {
            using ( Brush brush = this.Fill.MakeBrush( path.GetBounds() ) )
              g.FillPath( brush, path );
          }

          if ( _border.IsVisible )
          {
            var sm = g.SmoothingMode;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen pen = _border.GetPen(pane, scaleFactor))
            {
              if (IsMoving)
              {
                // Set the DashCap to round.
                pen.DashCap = DashCap.Round;

                // Create a custom dash pattern.
                pen.DashPattern = new float[] { 4.0F, 4.0F };
              }

              g.DrawPath(pen, path);


              if (!_isClosedFigure)
              {
                PointF lastPt = path.GetLastPoint();
                PointF firstPt = path.PathPoints[0];

                // Set the DashCap to round.
                pen.DashCap = DashCap.Round;

                // Create a custom dash pattern.
                pen.DashPattern = new float[] { 4.0F, 4.0F };

                g.DrawLine(pen, firstPt.X, firstPt.Y, lastPt.X, lastPt.Y);
              }

              if (IsSelected)
              {
                Brush brush = new SolidBrush(Color.White);

                g.FillRectangles(brush, EdgeRects(pane));

                pen.DashStyle = DashStyle.Solid;

                g.DrawRectangles(pen, EdgeRects(pane));
              }
            }

            g.SmoothingMode = sm;
          }
        }
      }
    }

        internal PointF SafeTransform(PointD pt, GraphPane pane, CoordType coord)
        {
            PointF pixPt = new PointF();

            if (pt.X != 0) // for first point
                pixPt = pane.GeneralTransform(pt.X, pt.Y, coord);

            return pixPt;
        }

        public override void UpdateLocation(PaneBase _pane, float dx, float dy)
        {
            GraphPane pane = _pane as GraphPane;

            // update each points
            for (int i = 0; i < _points.Count; i++)
            {
                // convert location to screen coordinate
                PointF ptPix1 = pane.GeneralTransform(_points[i].X, _points[i].Y,
                        _location.CoordinateFrame);

                // calc new position
                ptPix1.X += (float)dx;
                ptPix1.Y += (float)dy;

                // convert to user coordinate
                PointD pt1 = pane.GeneralReverseTransform(ptPix1, _location.CoordinateFrame);

                _points[i] = pt1;
            }

            OnLocationChanged(pane, dx, dy);
        }

        override public GraphicsPath MakePath( PaneBase pane )
    {
      GraphicsPath path = new GraphicsPath();
      bool first = true;
      PointF lastPt = new PointF();

            GraphPane gPane = pane as GraphPane;

            foreach ( PointD pt in _points )
      {
                // Convert the coordinates from the user coordinate system
                // to the screen coordinate system
                // Offset the points by the location value
                //PointF pixPt = SafeTransform(pt, gPane, _location.CoordinateFrame);

                PointF pixPt = gPane.GeneralTransform(pt.X, pt.Y, _location.CoordinateFrame);

                if (  Math.Abs( pixPt.X ) < 100000 &&
            Math.Abs( pixPt.Y ) < 100000 )
        {
          if ( first )
            first = false;
          else
            path.AddLine( lastPt, pixPt );

          lastPt = pixPt;
        }
      }

      if (_isClosedFigure)
        path.CloseFigure();


      return path;
    }

    override public RectangleF[] EdgeRects(PaneBase pane)
    {
      RectangleF[] rects = new RectangleF[_points.Count];

            GraphPane gPane = pane as GraphPane;

            for (int i = 0; i < rects.Length; i++)
      {
                //PointF pixPt = SafeTransform(_points[i], gPane, _location.CoordinateFrame);
                PointF pixPt = gPane.GeneralTransform(_points[i].X, _points[i].Y, _location.CoordinateFrame);
                rects[i] = new RectangleF(pixPt.X - 4, pixPt.Y - 4, 8, 8);
            }

      return rects;
    }

    override public bool FindNearestEdge(PointF pt, PaneBase pane, out int index)
    {
      RectangleF[] edges = EdgeRects(pane);

      index = -1;

      for (int i = 0; i < edges.Length; i++)
      {
        if (edges[i].Contains(pt))
        {
          index = i;
          break;
        }
      }

      return index != -1;
    }

    override public void ResizeEdge(int edge, PointF pt, PaneBase pane)
    {
      // when edge is int.MaxValue, we assume it is last point
      if (edge == int.MaxValue)
        edge = _points.Count - 1;

      // do nothing if edge is invalid
      if (edge < 0 || edge >= _points.Count)
        return;

            GraphPane gPane = pane as GraphPane;

            PointD ptPix = gPane.GeneralReverseTransform(
                                pt.X,
                                pt.Y, _location.CoordinateFrame);

            _points[edge] = ptPix;
        }

    /// <summary>
    /// Determine if the specified screen point lies inside the bounding box of this
    /// <see cref="PolyObj"/>.
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
    override public bool PointInBox( PointF pt, PaneBase pane, Graphics g, float scaleFactor )
    {
      if ( _points != null && _points.Count > 1 )
      {
                //if (!base.PointInBox(pt, pane, g, scaleFactor))
                //    return false;

                using (GraphicsPath path = MakePath(pane))
                {
                    return path.IsVisible(pt) || path.IsOutlineVisible(pt, new Pen(Color.AliceBlue, 2)) ;
                }
                    
      }
      else
        return false;
    }

        public override RectangleF BoundingRect(PaneBase pane)
        {
            GraphPane  gPane = pane as GraphPane;

            float x1 = 100000;
            float y1 = 100000;
            float x2 = -100000;
            float y2 = -100000;

            foreach (PointD pt in _points)
            {
                // Convert the coordinates from the user coordinate system
                // to the screen coordinate system
                // Offset the points by the location value
                //PointF pixPt = SafeTransform(pt, gPane, _location.CoordinateFrame);

                PointF pixPt = gPane.GeneralTransform(pt.X, pt.Y, _location.CoordinateFrame);

                x1 = Math.Min(x1, pixPt.X);
                y1 = Math.Min(y1, pixPt.Y);

                x2 = Math.Max(x2, pixPt.X);
                y2 = Math.Max(y2, pixPt.Y);
                
            }

            return new RectangleF(x1, y1, x2 - x1, y2 - y1);
        }
        #endregion

    #region Point Related Method
        public void AddPoint(PointD pt)
    {
            AddPoint(pt.X, pt.Y);
    }

    public void AddPoint(double x, double y)
    {
            //_points.Add(new PointD(x - _location.X, y - _location.Y));

            _points.Add(new PointD(x, y));

            //System.Diagnostics.Trace.WriteLine(String.Format("points {0}  {1} {2}",
            //    _points.Count, x, y));
        }

        public void AddPoints(PointD[] points)
        {
            foreach (var pt in points) 
            {
                AddPoint(pt);
            }
        }

    public PointD LastPoint
    {
      get
      {
        if (_points.Count > 0)
          return _points[_points.Count - 1];
        else
          return new PointD(0, 0);
      }

      set
      {
        if (_points.Count > 0)
          _points[_points.Count - 1] = value;
      }
    }

        override public PointF[] ScreenPoints(PaneBase pane)
        {
            PointF[] points = new PointF[_points.Count];

            GraphPane gPane = pane as GraphPane;

            for (int i = 0; i < points.Length; i++)
            {
                PointF pixPt = gPane.GeneralTransform(_points[i].X, _points[i].Y, _location.CoordinateFrame);

                points[i] = pixPt;
            }


            return points;
        }

        override public PointD[] EdgePoints()
        {
            PointD[] points = new PointD[_points.Count];

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = _points[i];
            }

            return points;
        }
        #endregion

    }
}
