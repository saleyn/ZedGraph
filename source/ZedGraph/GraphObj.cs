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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
	/// <summary>
	/// An abstract base class that represents a text object on the graph.  A list of
	/// <see cref="GraphObj"/> objects is maintained by the
	/// <see cref="GraphObjList"/> collection class.
	/// </summary>
	/// 
	/// <author> John Champion </author>
	/// <version> $Revision: 3.1 $ $Date: 2006-06-24 20:26:44 $ </version>
	[Serializable]
	abstract public class GraphObj : ISerializable, ICloneable
	{
	#region Fields
		/// <summary>
		/// Protected field that stores the location of this <see cref="GraphObj"/>.
		/// Use the public property <see cref="Location"/> to access this value.
		/// </summary>
		[CLSCompliant(false)]
		protected Location _location;

		/// <summary>
		/// Protected field that determines whether or not this <see cref="GraphObj"/>
		/// is visible in the graph.  Use the public property <see cref="IsVisible"/> to
		/// access this value.
		/// </summary>
		[CLSCompliant(false)]
		protected bool _isVisible;
		
		/// <summary>
		/// Protected field that determines whether or not the rendering of this <see cref="GraphObj"/>
		/// will be clipped to the ChartRect.  Use the public property <see cref="IsClippedToChartRect"/> to
		/// access this value.
		/// </summary>
		[CLSCompliant(false)]
		protected bool _isClippedToChartRect;
		
		/// <summary>
		/// A tag object for use by the user.  This can be used to store additional
		/// information associated with the <see cref="GraphObj"/>.  ZedGraph does
		/// not use this value for any purpose.
		/// </summary>
		/// <remarks>
		/// Note that, if you are going to Serialize ZedGraph data, then any type
		/// that you store in <see cref="Tag"/> must be a serializable type (or
		/// it will cause an exception).
		/// </remarks>
		public object Tag;

		/// <summary>
		/// Internal field that determines the z-order "depth" of this
		/// item relative to other graphic objects.  Use the public property
		/// <see cref="ZOrder"/> to access this value.
		/// </summary>
		internal ZOrder _zOrder;

		/// <summary>
		/// Internal field that stores the hyperlink information for this object.
		/// </summary>
		internal Link _link;

        [CLSCompliant(false)]
        protected bool _isMoving;

        [CLSCompliant(false)]
        protected bool _isSelected;

    #endregion

    #region Defaults
        /// <summary>
        /// A simple struct that defines the
        /// default property values for the <see cref="GraphObj"/> class.
        /// </summary>
        public struct Default
		{
			// Default text item properties
			/// <summary>
			/// Default value for the vertical <see cref="GraphObj"/>
			/// text alignment (<see cref="GraphObj.Location"/> property).
			/// This is specified
			/// using the <see cref="AlignV"/> enum type.
			/// </summary>
			public static AlignV AlignV = AlignV.Center;
			/// <summary>
			/// Default value for the horizontal <see cref="GraphObj"/>
			/// text alignment (<see cref="GraphObj.Location"/> property).
			/// This is specified
			/// using the <see cref="AlignH"/> enum type.
			/// </summary>
			public static AlignH AlignH = AlignH.Center;
			/// <summary>
			/// The default coordinate system to be used for defining the
			/// <see cref="GraphObj"/> location coordinates
			/// (<see cref="GraphObj.Location"/> property).
			/// </summary>
			/// <value> The coordinate system is defined with the <see cref="CoordType"/>
			/// enum</value>
			public static CoordType CoordFrame = CoordType.AxisXYScale;
			/// <summary>
			/// The default value for <see cref="GraphObj.IsClippedToChartRect"/>.
			/// </summary>
			public static bool IsClippedToChartRect = false;
		}
	#endregion

	#region Properties
		/// <summary>
		/// The <see cref="ZedGraph.Location"/> struct that describes the location
		/// for this <see cref="GraphObj"/>.
		/// </summary>
		public Location Location
		{
			get { return _location; }
			set { _location = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines the z-order "depth" of this
		/// item relative to other graphic objects.
		/// </summary>
		/// <remarks>Note that this controls the z-order with respect to
		/// other elements such as <see cref="CurveItem"/>'s, <see cref="Axis"/>
		/// objects, etc.  The order of <see cref="GraphObj"/> objects having
		/// the same <see cref="ZedGraph.ZOrder"/> value is controlled by their order in
		/// the <see cref="GraphObjList"/>.  The first <see cref="GraphObj"/>
		/// in the list is drawn in front of other <see cref="GraphObj"/>
		/// objects having the same <see cref="ZedGraph.ZOrder"/> value.</remarks>
		public ZOrder ZOrder
		{
			get { return _zOrder; }
			set { _zOrder = value; }
		}
		
		/// <summary>
		/// Gets or sets a value that determines if this <see cref="GraphObj"/> will be
		/// visible in the graph.  true displays the item, false hides it.
		/// </summary>
		public bool IsVisible
		{
			get { return _isVisible; }
			set { _isVisible = value; }
		}

		/// <summary>
		/// Gets or sets a value that determines whether or not the rendering of this <see cref="GraphObj"/>
		/// will be clipped to the <see cref="Chart.Rect"/>.
		/// </summary>
		/// <value>true to clip the <see cref="GraphObj"/> to the <see cref="Chart.Rect"/> bounds,
		/// false to leave it unclipped.</value>
		public bool IsClippedToChartRect
		{
			get { return _isClippedToChartRect; }
			set { _isClippedToChartRect = value; }
		}

		/// <summary>
		/// Gets or sets the hyperlink information for this <see cref="GraphObj" />.
		/// </summary>
		// /// <seealso cref="ZedGraph.Web.IsImageMap" />
		public Link Link
		{
			get { return _link; }
			set { _link = value; }
		}

		/// <summary>
		/// true if the <see cref="ZOrder" /> of this object is set to put it in front
		/// of the <see cref="CurveItem" /> data points.
		/// </summary>
		public bool IsInFrontOfData
		{
			get
			{
				return	_zOrder == ZOrder.A_InFront ||
							_zOrder == ZOrder.B_BehindLegend ||
							_zOrder == ZOrder.C_BehindChartBorder;
			}
		}

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public bool IsMoving
        {
            get { return _isMoving; }
            set { _isMoving = value; }
        }
        #endregion

    #region Constructors
        /// <overloads>
        /// Constructors for the <see cref="GraphObj"/> class.
        /// </overloads>
        /// <summary>
        /// Default constructor that sets all <see cref="GraphObj"/> properties to default
        /// values as defined in the <see cref="Default"/> class.
        /// </summary>
        public GraphObj() :
			this( 0, 0, Default.CoordFrame, Default.AlignH, Default.AlignV )
		{
		}

		/// <summary>
		/// Constructor that sets all <see cref="GraphObj"/> properties to default
		/// values as defined in the <see cref="Default"/> class.
		/// </summary>
		/// <param name="x">The x position of the text.  The units
		/// of this position are specified by the
		/// <see cref="ZedGraph.Location.CoordinateFrame"/> property.  The text will be
		/// aligned to this position based on the <see cref="AlignH"/>
		/// property.</param>
		/// <param name="y">The y position of the text.  The units
		/// of this position are specified by the
		/// <see cref="ZedGraph.Location.CoordinateFrame"/> property.  The text will be
		/// aligned to this position based on the
		/// <see cref="AlignV"/> property.</param>
		public GraphObj( double x, double y ) :
			this( x, y, Default.CoordFrame, Default.AlignH, Default.AlignV )
		{
		}

		/// <summary>
		/// Constructor that creates a <see cref="GraphObj"/> with the specified
		/// coordinates and all other properties to defaults as specified
		/// in the <see cref="Default"/> class..
		/// </summary>
		/// <remarks>
		/// The four coordinates define the starting point and ending point for
		/// <see cref="ArrowObj"/>'s, or the topleft and bottomright points for
		/// <see cref="ImageObj"/>'s.  For <see cref="GraphObj"/>'s that only require
		/// one point, the <see paramref="x2"/> and <see paramref="y2"/> values
		/// will be ignored.  The units of the coordinates are specified by the
		/// <see cref="ZedGraph.Location.CoordinateFrame"/> property.
		/// </remarks>
		/// <param name="x">The x position of the item.</param>
		/// <param name="y">The y position of the item.</param>
		/// <param name="x2">The x2 position of the item.</param>
		/// <param name="y2">The x2 position of the item.</param>
		public GraphObj( double x, double y, double x2, double y2 ) :
			this( x, y, x2, y2, Default.CoordFrame, Default.AlignH, Default.AlignV )
		{
		}

		/// <summary>
		/// Constructor that creates a <see cref="GraphObj"/> with the specified
		/// position and <see cref="CoordType"/>.  Other properties are set to default
		/// values as defined in the <see cref="Default"/> class.
		/// </summary>
		/// <remarks>
		/// The two coordinates define the location point for the object.
		/// The units of the coordinates are specified by the
		/// <see cref="ZedGraph.Location.CoordinateFrame"/> property.
		/// </remarks>
		/// <param name="x">The x position of the item.  The item will be
		/// aligned to this position based on the <see cref="AlignH"/>
		/// property.</param>
		/// <param name="y">The y position of the item.  The item will be
		/// aligned to this position based on the
		/// <see cref="AlignV"/> property.</param>
		/// <param name="coordType">The <see cref="CoordType"/> enum value that
		/// indicates what type of coordinate system the x and y parameters are
		/// referenced to.</param>
		public GraphObj( double x, double y, CoordType coordType ) :
			this( x, y, coordType, Default.AlignH, Default.AlignV )
		{
		}
		
		/// <summary>
		/// Constructor that creates a <see cref="GraphObj"/> with the specified
		/// position, <see cref="CoordType"/>, <see cref="AlignH"/>, and <see cref="AlignV"/>.
		/// Other properties are set to default values as defined in the <see cref="Default"/> class.
		/// </summary>
		/// <remarks>
		/// The two coordinates define the location point for the object.
		/// The units of the coordinates are specified by the
		/// <see cref="ZedGraph.Location.CoordinateFrame"/> property.
		/// </remarks>
		/// <param name="x">The x position of the item.  The item will be
		/// aligned to this position based on the <see cref="AlignH"/>
		/// property.</param>
		/// <param name="y">The y position of the text.  The units
		/// of this position are specified by the
		/// <see cref="ZedGraph.Location.CoordinateFrame"/> property.  The text will be
		/// aligned to this position based on the
		/// <see cref="AlignV"/> property.</param>
		/// <param name="coordType">The <see cref="CoordType"/> enum value that
		/// indicates what type of coordinate system the x and y parameters are
		/// referenced to.</param>
		/// <param name="alignH">The <see cref="ZedGraph.AlignH"/> enum that specifies
		/// the horizontal alignment of the object with respect to the (x,y) location</param>
		/// <param name="alignV">The <see cref="ZedGraph.AlignV"/> enum that specifies
		/// the vertical alignment of the object with respect to the (x,y) location</param>
		public GraphObj( double x, double y, CoordType coordType, AlignH alignH, AlignV alignV )
		{
			_isVisible = true;
			_isClippedToChartRect = Default.IsClippedToChartRect;
			this.Tag = null;
			_zOrder = ZOrder.A_InFront;
			_location = new Location( x, y, coordType, alignH, alignV );
			_link = new Link();
            _isSelected = false;
            _isMoving = false;
		}

		/// <summary>
		/// Constructor that creates a <see cref="GraphObj"/> with the specified
		/// position, <see cref="CoordType"/>, <see cref="AlignH"/>, and <see cref="AlignV"/>.
		/// Other properties are set to default values as defined in the <see cref="Default"/> class.
		/// </summary>
		/// <remarks>
		/// The four coordinates define the starting point and ending point for
		/// <see cref="ArrowObj"/>'s, or the topleft and bottomright points for
		/// <see cref="ImageObj"/>'s.  For <see cref="GraphObj"/>'s that only require
		/// one point, the <see paramref="x2"/> and <see paramref="y2"/> values
		/// will be ignored.  The units of the coordinates are specified by the
		/// <see cref="ZedGraph.Location.CoordinateFrame"/> property.
		/// </remarks>
		/// <param name="x">The x position of the item.</param>
		/// <param name="y">The y position of the item.</param>
		/// <param name="x2">The x2 position of the item.</param>
		/// <param name="y2">The x2 position of the item.</param>
		/// <param name="coordType">The <see cref="CoordType"/> enum value that
		/// indicates what type of coordinate system the x and y parameters are
		/// referenced to.</param>
		/// <param name="alignH">The <see cref="ZedGraph.AlignH"/> enum that specifies
		/// the horizontal alignment of the object with respect to the (x,y) location</param>
		/// <param name="alignV">The <see cref="ZedGraph.AlignV"/> enum that specifies
		/// the vertical alignment of the object with respect to the (x,y) location</param>
		public GraphObj( double x, double y, double x2, double y2, CoordType coordType,
					AlignH alignH, AlignV alignV )
		{
			_isVisible = true;
			_isClippedToChartRect = Default.IsClippedToChartRect;
			this.Tag = null;
			_zOrder = ZOrder.A_InFront;
			_location = new Location( x, y, x2, y2, coordType, alignH, alignV );
			_link = new Link();
            _isSelected = false;
            _isMoving = false;
        }

		/// <summary>
		/// The Copy Constructor
		/// </summary>
		/// <param name="rhs">The <see cref="GraphObj"/> object from which to copy</param>
		public GraphObj( GraphObj rhs )
		{
			// Copy value types
			_isVisible = rhs.IsVisible;
			_isClippedToChartRect = rhs._isClippedToChartRect;
			_zOrder = rhs.ZOrder;

            // copy moving, selected property ?
            _isMoving = rhs._isMoving;
            _isSelected = rhs._isSelected;

			// copy reference types by cloning
			if ( rhs.Tag is ICloneable )
				this.Tag = ((ICloneable) rhs.Tag).Clone();
			else
				this.Tag = rhs.Tag;

			_location = rhs.Location.Clone();
			_link = rhs._link.Clone();
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
		/// Current schema value that defines the version of the serialized file
		/// </summary>
		/// <remarks>
		/// schema changed to 2 when isClippedToChartRect was added.
		/// </remarks>
		public const int schema = 10;

		/// <summary>
		/// Constructor for deserializing objects
		/// </summary>
		/// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
		/// </param>
		/// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
		/// </param>
		protected GraphObj( SerializationInfo info, StreamingContext context )
		{
			// The schema value is just a file version parameter.  You can use it to make future versions
			// backwards compatible as new member variables are added to classes
			int sch = info.GetInt32( "schema" );

			_location = (Location) info.GetValue( "location", typeof(Location) );
			_isVisible = info.GetBoolean( "isVisible" );
			Tag = info.GetValue( "Tag", typeof(object) );
			_zOrder = (ZOrder) info.GetValue( "zOrder", typeof(ZOrder) );

			_isClippedToChartRect = info.GetBoolean( "isClippedToChartRect" );
			_link = (Link) info.GetValue( "link", typeof( Link ) );

            _isSelected = info.GetBoolean("isSelected");
            _isMoving = info.GetBoolean("isMoving");
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
			info.AddValue( "location", _location );
			info.AddValue( "isVisible", _isVisible );
			info.AddValue( "Tag", Tag );
			info.AddValue( "zOrder", _zOrder );

			info.AddValue( "isClippedToChartRect", _isClippedToChartRect );
			info.AddValue( "link", _link );

            info.AddValue( "isSelected", _isSelected );
            info.AddValue( "isMoving", _isMoving );
		}
	#endregion

	#region Rendering Methods
		/// <summary>
		/// Render this <see cref="GraphObj"/> object to the specified <see cref="Graphics"/> device.
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
		/// passed down by the parent <see cref="PaneBase"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		abstract public void Draw( Graphics g, PaneBase pane, float scaleFactor );
		
		/// <summary>
		/// Determine if the specified screen point lies inside the bounding box of this
		/// <see cref="GraphObj"/>.
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
		/// passed down by the parent <see cref="PaneBase"/> object using the
		/// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
		/// font sizes, etc. according to the actual size of the graph.
		/// </param>
		/// <returns>true if the point lies in the bounding box, false otherwise</returns>
		virtual public bool PointInBox( PointF pt, PaneBase pane, Graphics g, float scaleFactor )
		{
			GraphPane gPane = pane as GraphPane;

			if ( gPane != null && _isClippedToChartRect && !gPane.Chart.Rect.Contains( pt ) )
				return false;

			return true;
		}

        /// <summary>
        /// Get path of graph object which is used for PointInBox 
        /// </summary>
        /// <param name="pane"></param>
        /// <returns></returns>
        virtual public GraphicsPath MakePath(PaneBase pane)
        {
            GraphicsPath path = new GraphicsPath();

            // transform the x,y location from the user-defined
            // coordinate frame to the screen pixel location
            RectangleF pixRect = _location.TransformRect(pane);

            path.AddRectangle(pixRect);

            return path;
        }

        /// <summary>
        /// Filter points in curve list and build new point list
        /// </summary>
        /// <param name="pane"></param>
        /// <param name="target"></param>
        /// <returns></returns>
#if false
        virtual public List<PointPairList> FilterPoints(PaneBase pane, IPointList target)
        {
            List<PointPairList> lst = new List<PointPairList>(2);

            lst.Add(new PointPairList());
            lst.Add(new PointPairList());

            GraphicsPath path = MakePath(pane);

            Axis yAxis = (pane as GraphPane).YAxis;
            Axis xAxis = (pane as GraphPane).XAxis;

            for (int i = 0; i < target.Count; i++)
            {
                var pp = target[i];

                float x = xAxis.Scale.Transform(pp.X);
                float y = yAxis.Scale.Transform(pp.Y);

                PointF pt = new PointF(x, y);

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
#else
        virtual public List<PointPairList> FilterPoints(PaneBase pane, IPointList target)
        {
            List<PointPairList> lst = new List<PointPairList>(2);

            lst.Add(new PointPairList());
            lst.Add(new PointPairList());

            GraphicsPath path = MakePath(pane);

            PointF[] points = path.PathPoints;

            Axis yAxis = (pane as GraphPane).YAxis;
            Axis xAxis = (pane as GraphPane).XAxis;
            GraphPane gPane = pane as GraphPane;

            //for (int i = 0; i < points.Length; i++)
            //{
            //    PointD pt = gPane.GeneralReverseTransform(points[i], _location.CoordinateFrame);

            //    points[i].X = (float)pt.X;
            //    points[i].Y = (float)pt.Y;
            //}

            for (int i = 0; i < target.Count; i++)
            {
                var pp = target[i];

                float x = xAxis.Scale.Transform(pp.X);
                float y = yAxis.Scale.Transform(pp.Y);

                PointF pt = new PointF(x, y);

                //if (path != null && path.IsVisible(pt))
                if (path != null && Utils.PtInPolygon(points, pt))
                //if (path != null && Utils.PtInPolygon(points, pp))
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
#endif
        /// <summary>
        /// Find the nearest edge for point which is used to resize graph
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="pane"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        virtual public bool FindNearestEdge(PointF pt, PaneBase pane, out int index)
        {
            index = -1;
            return false;
        }

        /// <summary>
        /// Resize the graph 
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="pt"></param>
        /// <param name="pane"></param>
        virtual public void ResizeEdge(int edge, PointF pt, PaneBase pane)
        {
            // do nothing
        }

        /// <summary>
        /// Determines the shape type and Coords values for this GraphObj
        /// </summary>
        abstract public void GetCoords( PaneBase pane, Graphics g, float scaleFactor,
				out string shape, out string coords );

        /// <summary>
        /// The rect list of each edge
        /// </summary>
        /// <param name="pane"></param>
        /// <returns></returns>
        virtual public RectangleF[] EdgeRects(PaneBase pane)
        {
            return new RectangleF[0];
        }

        /// <summary>
        /// Update location of object according given x/y offset
        /// </summary>
        /// <param name="_pane"></param>
        /// <param name="dx">x offset in screen</param>
        /// <param name="dy">y offset in screen</param>
        virtual public void UpdateLocation(PaneBase _pane, double dx, double dy)
        {
            GraphPane pane = _pane as GraphPane;

            // convert location to screen coordinate
            PointF ptPix1 = pane.GeneralTransform(_location.X1, _location.Y1,
                    _location.CoordinateFrame);

            PointF ptPix2 = pane.GeneralTransform(_location.X2, _location.Y2,
                    _location.CoordinateFrame);

            // calc new position
            ptPix1.X += (float)dx;
            ptPix1.Y += (float)dy;

            ptPix2.X += (float)dx;
            ptPix2.Y += (float)dy;

            // convert to user coordinate
            PointD pt1 = pane.GeneralReverseTransform(ptPix1, _location.CoordinateFrame);
            PointD pt2 = pane.GeneralReverseTransform(ptPix2, _location.CoordinateFrame);

            _location.X = pt1.X;
            _location.Y = pt1.Y;
            _location.Width = pt2.X - pt1.X;
            _location.Height = pt2.Y - pt1.Y;
        }

        virtual public RectangleF BoundingRect(PaneBase pane)
        {
            return _location.TransformRect(pane);
        }
#endregion

    }

    /// <summary>
    /// Helper utils
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Distance between two points
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static double Distance(double dx, double dy)
        {
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Angle between two points with O
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <returns></returns>
        public static double Distance(double x0, double y0, double x1, double y1)
        {
            return Distance(x0 - x1, y0 - y1);
        }

        /// <summary>
        /// Angle for two lines
        /// </summary>
        /// <param name="dy"></param>
        /// <param name="dx"></param>
        /// <returns></returns>
        public static double AngleInDegree(double dy, double dx)
        {
            return 180 * Math.Atan2(dy, dx) / Math.PI;
        }

        /// <summary>
        /// Angle between two points with O
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <returns></returns>
        public static double AngleInDegree(double x1, double y1, double x0, double y0)
        {
            return 180 * Math.Atan2(y1 - y0, x1 - x0) / Math.PI;
        }

        public static bool PtInPolygon(PointF[] points, PointF pt)
        {
            int i, j;
            bool rc = false;

            for (i = 0, j = points.Length - 1; i < points.Length; j = i++)
            {
                if (((points[i].Y > pt.Y) != (points[j].Y > pt.Y))
                    && (pt.X < (points[j].X - points[i].X) * (pt.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                {
                    rc = !rc;
                }
            }

            return rc;
        }

        public static bool PtInPolygon2(PointF[] points, PointF p)
        {
            bool result = false;
            for (int i = 0; i < points.Length - 1; i++)
            {
                if ( (  ( (points[i + 1].Y <= p.Y) && (p.Y < points[i].Y) ) 
                      ||( (points[i].Y <= p.Y) && (p.Y < points[i + 1].Y) )  
                     ) 
                    && (p.X < (points[i].X - points[i + 1].X) * (p.Y - points[i + 1].Y) / (points[i].Y - points[i + 1].Y) + points[i + 1].X))
                {
                    result = !result;
                }
            }
            return result;
        }
    }
}
