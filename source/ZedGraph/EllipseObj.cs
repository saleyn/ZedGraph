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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
  /// <summary>
  /// A class that represents a bordered and/or filled ellipse object on
  /// the graph.  A list of EllipseObj objects is maintained by the
  /// <see cref="GraphObjList"/> collection class.  The ellipse is defined
  /// as the ellipse that would be contained by the rectangular box as
  /// defined by the <see cref="Location"/> property.
  /// </summary>
  /// 
  /// <author> John Champion </author>
  /// <version> $Revision: 3.3 $ $Date: 2007-01-25 07:56:08 $ </version>
  [Serializable]
  public class EllipseObj : BoxObj, ICloneable, ISerializable
  {
  #region Fields
    private float _angle = 0f;
    #endregion

  #region Properties
    public float Angle
    {
      get { return _angle; }
      set { _angle = value; }
    }
  #endregion

  #region Constructors
    /// <overloads>Constructors for the <see cref="EllipseObj"/> object</overloads>
    /// <summary>
    /// A constructor that allows the position and size
    /// of the <see cref="EllipseObj"/> to be pre-specified.  Other properties are defaulted.
    /// </summary>
    /// <param name="x">The x location for this <see cref="BoxObj" />.  This will be in units determined by
    /// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    /// <param name="y">The y location for this <see cref="BoxObj" />.  This will be in units determined by
    /// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    /// <param name="width">The width of this <see cref="BoxObj" />.  This will be in units determined by
    /// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    /// <param name="height">The height of this <see cref="BoxObj" />.  This will be in units determined by
    /// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    public EllipseObj(double x, double y, double width, double height)
      : base(x, y, width, height)
    {
    }

    /// <summary>
    /// A default constructor that places the <see cref="EllipseObj"/> at location (0,0),
    /// with width/height of (1,1).  Other properties are defaulted.
    /// </summary>
    public EllipseObj() : base()
    {
    }

    /// <summary>
    /// A constructor that allows the position, border color, and solid fill color
    /// of the <see cref="EllipseObj"/> to be pre-specified.
    /// </summary>
    /// <param name="borderColor">An arbitrary <see cref="System.Drawing.Color"/> specification
    /// for the ellipse border</param>
    /// <param name="fillColor">An arbitrary <see cref="System.Drawing.Color"/> specification
    /// for the ellipse fill (will be a solid color fill)</param>
    /// <param name="x">The x location for this <see cref="BoxObj" />.  This will be in units determined by
    /// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    /// <param name="y">The y location for this <see cref="BoxObj" />.  This will be in units determined by
    /// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    /// <param name="width">The width of this <see cref="BoxObj" />.  This will be in units determined by
    /// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    /// <param name="height">The height of this <see cref="BoxObj" />.  This will be in units determined by
    /// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    public EllipseObj(double x, double y, double width, double height, Color borderColor, Color fillColor)
      : base(x, y, width, height, borderColor, fillColor)
    {
    }

    /// <summary>
    /// A constructor that allows the position, border color, and two-color
    /// gradient fill colors
    /// of the <see cref="EllipseObj"/> to be pre-specified.
    /// </summary>
    /// <param name="borderColor">An arbitrary <see cref="System.Drawing.Color"/> specification
    /// for the ellipse border</param>
    /// <param name="fillColor1">An arbitrary <see cref="System.Drawing.Color"/> specification
    /// for the start of the ellipse gradient fill</param>
    /// <param name="fillColor2">An arbitrary <see cref="System.Drawing.Color"/> specification
    /// for the end of the ellipse gradient fill</param>
    /// <param name="x">The x location for this <see cref="BoxObj" />.  This will be in units determined by
    /// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    /// <param name="y">The y location for this <see cref="BoxObj" />.  This will be in units determined by
    /// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    /// <param name="width">The width of this <see cref="BoxObj" />.  This will be in units determined by
    /// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    /// <param name="height">The height of this <see cref="BoxObj" />.  This will be in units determined by
    /// <see cref="ZedGraph.Location.CoordinateFrame" />.</param>
    public EllipseObj(double x, double y, double width, double height, Color borderColor,
              Color fillColor1, Color fillColor2) :
        base(x, y, width, height, borderColor, fillColor1, fillColor2)
    {
    }

    /// <summary>
    /// The Copy Constructor
    /// </summary>
    /// <param name="rhs">The <see cref="EllipseObj"/> object from
    /// which to copy</param>
    public EllipseObj(EllipseObj rhs) : base(rhs)
    {
      rhs._angle = this._angle;
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
    public new EllipseObj Clone()
    {
      return new EllipseObj(this);
    }

  #endregion

  #region Serialization
    /// <summary>
    /// Current schema value that defines the version of the serialized file
    /// </summary>
    public const int schema3 = 11;

    /// <summary>
    /// Constructor for deserializing objects
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
    /// </param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
    /// </param>
    protected EllipseObj(SerializationInfo info, StreamingContext context) : base(info, context)
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      int sch = info.GetInt32("schema3");
      if (sch >= 11)
        _angle = (float)info.GetDouble("angle");
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
      info.AddValue("schema3", schema3);
      info.AddValue("angle", _angle);
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
    override public void Draw(Graphics g, PaneBase pane, float scaleFactor)
    {
      // Convert the arrow coordinates from the user coordinate system
      // to the screen coordinate system
      RectangleF pixRect = this.Location.TransformRect(pane);

            //GraphPane gPane = pane as GraphPane;

            //System.Diagnostics.Trace.WriteLine("XScale " + gPane.XAxis.Scale);
            //System.Diagnostics.Trace.WriteLine("YScale " + gPane.YAxis.Scale);

            //System.Diagnostics.Trace.WriteLine(pixRect.X);

            if (Math.Abs(pixRect.Left) < 100000 &&
          Math.Abs(pixRect.Top) < 100000 &&
          Math.Abs(pixRect.Right) < 100000 &&
          Math.Abs(pixRect.Bottom) < 100000)
      {
        GraphicsState state = g.Save();

        g.SmoothingMode = SmoothingMode.AntiAlias;

        Matrix matrix = g.Transform;

        matrix.RotateAt(Angle, Center(pixRect));
        //matrix.Rotate(Angle);

        g.Transform = matrix;
        if (_fill.IsVisible)
          using (Brush brush = _fill.MakeBrush(pixRect))
            g.FillEllipse(brush, pixRect);

        if (_border.IsVisible)
          using (Pen pen = _border.GetPen(pane, scaleFactor))
          {
            if (IsMoving)
            {
              // Set the DashCap to round.
              pen.DashCap = DashCap.Round;

              // Create a custom dash pattern.
              pen.DashPattern = new float[] { 4.0F, 4.0F };
            }

            g.DrawEllipse(pen, pixRect);

            if (IsSelected)
            {
              Brush brush = new SolidBrush(Color.White);

              g.FillRectangles(brush, EdgeRects(pane));

              pen.DashStyle = DashStyle.Solid;

              g.DrawRectangles(pen, EdgeRects(pane));
            }
          }

        g.Restore(state);
      }
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
      //if (!base.PointInBox(pt, pane, g, scaleFactor))
      //  return false;

            using (GraphicsPath path = MakePath(pane))
                return path.IsVisible(pt);
        }
  #endregion

  #region Overwrite Methods
        override public GraphicsPath MakePath(PaneBase pane)
        {
            GraphicsPath path = new GraphicsPath();

            // transform the x,y location from the user-defined
            // coordinate frame to the screen pixel location
            RectangleF pixRect = _location.TransformRect(pane);

            path.AddEllipse(pixRect);

            Matrix matrix = new Matrix();
            matrix.RotateAt(Angle, Center(pixRect));
            path.Transform(matrix);

            return path;
        }

        internal GraphicsPath MakeInnerPath(PaneBase pane)
        {
            GraphicsPath path = new GraphicsPath();

            // transform the x,y location from the user-defined
            // coordinate frame to the screen pixel location
            RectangleF pixRect = _location.TransformRect(pane);

            float left = pixRect.Left;
            float top = pixRect.Top;
            float right = pixRect.Right;
            float bottom = pixRect.Bottom;

            float h = (bottom - top) / 2;
            float w = (right - left) / 2;

            float centerX = (left + right) / 2;
            float centerY = (top + bottom) / 2;

            path.AddPolygon(new PointF[] {
                new PointF(left + w, top),
                new PointF(right, top + h),
                new PointF(left + w, bottom),
                new PointF(left, top + h) });

            Matrix matrix = new Matrix();
            matrix.RotateAt(Angle, Center(pixRect));
            path.Transform(matrix);

            return path;
        }

        internal GraphicsPath MakeOuterPath(PaneBase pane)
        {
            GraphicsPath path = new GraphicsPath();

            // transform the x,y location from the user-defined
            // coordinate frame to the screen pixel location
            RectangleF pixRect = _location.TransformRect(pane);

            path.AddRectangle(pixRect);

            Matrix matrix = new Matrix();
            matrix.RotateAt(Angle, Center(pixRect));
            path.Transform(matrix);

            return path;
        }

        override public List<PointPairList> FilterPoints(PaneBase pane, IPointList target)
        {
            List<PointPairList> lst = new List<PointPairList>(2);

            lst.Add(new PointPairList());
            lst.Add(new PointPairList());

            GraphicsPath path = MakePath(pane);
            GraphicsPath outerPath = MakeOuterPath(pane);
            GraphicsPath innerPath = MakeInnerPath(pane);

            Axis yAxis = (pane as GraphPane).YAxis;
            Axis xAxis = (pane as GraphPane).XAxis;

            for (int i = 0; i < target.Count; i++)
            {
                var pp = target[i];

                float x = xAxis.Scale.Transform(pp.X);
                float y = yAxis.Scale.Transform(pp.Y);

                PointF pt = new PointF(x, y);

                if (outerPath != null && !outerPath.IsVisible(pt))
                {
                    lst[1].Add(pp.X, pp.Y, pp.Z);
                    continue;
                }

                if (innerPath != null && innerPath.IsVisible(pt))
                {
                    lst[0].Add(pp.X, pp.Y, pp.Z);
                    continue;
                }

                if (path != null && path.IsVisible(pt))
                {
                    lst[0].Add(pp.X, pp.Y, pp.Z);
                }
                else
                {
                    lst[1].Add(pp.X, pp.Y, pp.Z);
                }
            }

            return lst;
        }

        override public RectangleF[] EdgeRects(PaneBase pane)
    {
      RectangleF pixRect = _location.TransformRect(pane);

      RectangleF[] rects = new RectangleF[4];

      float left = pixRect.Left;
      float top = pixRect.Top;
      float right = pixRect.Right;
      float bottom = pixRect.Bottom;

      float h = (bottom - top) / 2;
      float w = (right - left) / 2;

      float centerX = (left + right) / 2;
      float centerY = (top + bottom) / 2;

      // left: 0
      rects[0] = new RectangleF(left - 2, centerY - 2, 4, 4);

      // right: 1
      rects[1] = new RectangleF(right - 2, centerY - 2, 4, 4);

      // top: 2
      rects[2] = new RectangleF(centerX - 2, top - 2, 4, 4);

      // bottom: 3
      rects[3] = new RectangleF(centerX - 2, bottom - 2, 4, 4);

      return rects;
    }

    private static PointF Center(RectangleF pixRect)
    {
      return new PointF((pixRect.Left + pixRect.Right) / 2,
        (pixRect.Top + pixRect.Bottom) / 2);
    }
    private PointF Center(PaneBase pane)
    {
      RectangleF pixRect = _location.TransformRect(pane);

      return Center(pixRect);
    }

    override public bool FindNearestEdge(PointF pt, PaneBase pane, out int index)
    {
      RectangleF pixRect = _location.TransformRect(pane);

      float left = pixRect.Left;
      float top = pixRect.Top;
      float right = pixRect.Right;
      float bottom = pixRect.Bottom;

      float h = (bottom - top) / 2;
      float w = (right - left) / 2;

      float centerX = (left + right) / 2;
      float centerY = (top + bottom) / 2;

      float angle = (float)(-180 * Math.Atan2(pt.Y - centerY, pt.X - centerX) / Math.PI);
      float distance = (float)(Math.Sqrt(Math.Pow(pt.Y - centerY, 2) + Math.Pow(pt.X - centerX, 2)));
      float diff = angle - -Angle;

      index = -1;

      // check angle first
      if (Math.Abs(diff) < 1 && Math.Abs(distance - w) < 2)
      {
        index = 1;
      }
      else if ((Math.Abs(diff - 180) < 1 || Math.Abs(diff + 180) < 1)
        && Math.Abs(distance - w) < 2)
      {
        index = 0;
      }
      else if (Math.Abs(diff - 90) < 1 && Math.Abs(distance - h) < 2)
      {
        index = 2;
      }
      else if (Math.Abs(diff - -90) < 1 && Math.Abs(distance - h) < 2)
      {
        index = 3;
      }

      //System.Diagnostics.Trace.WriteLine(String.Format("angle {0} {1}  diff {2} {3}",
      //    angle, Angle, diff, index));

      return index != -1;
    }

    override public void ResizeEdge(int edge, PointF pt, PaneBase pane)
    {
            // set edget to right-bottom edge when edget is -1
            if (edge == int.MaxValue)
                edge = 1;

            /** sample code
            // convert location to screen coordinate
            PointF ptPix1 = pane.GeneralTransform(obj.Location.X1, obj.Location.Y1,
                    obj.Location.CoordinateFrame);

            PointF ptPix2 = pane.GeneralTransform(obj.Location.X2, obj.Location.Y2,
                    obj.Location.CoordinateFrame);

            // calc new position
            ptPix1.X += (mousePt.X - _dragStartPt.X);
            ptPix1.Y += (mousePt.Y - _dragStartPt.Y);

            ptPix2.X += (mousePt.X - _dragStartPt.X);
            ptPix2.Y += (mousePt.Y - _dragStartPt.Y);

            // convert to user coordinate
            PointD pt1 = pane.GeneralReverseTransform(ptPix1, obj.Location.CoordinateFrame);
            PointD pt2 = pane.GeneralReverseTransform(ptPix2, obj.Location.CoordinateFrame);

            obj.Location.X = pt1.X;
            obj.Location.Y = pt1.Y;
            obj.Location.Width = pt2.X - pt1.X;
            obj.Location.Height = pt2.Y - pt1.Y;
            */
            GraphPane gPane = pane as GraphPane;

            switch (edge)
      {
        case 0: // resize left
        case 1: // resize right
          {
            PointF o = Center(gPane);

            double ds = Utils.Distance(pt.Y - o.Y, pt.X - o.X);
            double angle = Utils.AngleInDegree(pt.Y - o.Y, pt.X - o.X);

            if (ds > 0.01)
            {
                            //_location.X = (o.X - ds) / pane.Rect.Width;
                            //_location.Width = ds * 2 / pane.Rect.Width;

                            PointD pt1 = gPane.GeneralReverseTransform((float)(o.X - ds), 0,
                                _location.CoordinateFrame);
                            PointD pt2 = gPane.GeneralReverseTransform((float)(o.X + ds), 0,
                                _location.CoordinateFrame);

                            _location.X = pt1.X;
                            _location.Width = pt2.X - pt1.X;
                        }

            Angle = (float)angle;
          }
          break;

        case 2: // resize top
        case 3: // resize bottom
          {
            PointF o = Center(pane);
            double ds = Utils.Distance(pt.Y - o.Y, pt.X - o.X);

            if (ds > 0.01)
            {
                            //_location.Y = (o.Y - ds) / pane.Rect.Height;
                            //_location.Height = ds * 2 / pane.Rect.Height;

                            PointD pt1 = gPane.GeneralReverseTransform(0, (float)(o.Y - ds),
                                _location.CoordinateFrame);
                            PointD pt2 = gPane.GeneralReverseTransform(0, (float)(o.Y + ds),
                                _location.CoordinateFrame);

                            _location.Y = pt1.Y;
                            _location.Height = pt2.Y - pt1.Y;
                        }
          }
          break;
      }
    }
  #endregion
  }
}
