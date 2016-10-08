//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2007  John Champion
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
  /// A class that handles the basic attributes of a line segment.
  /// </summary>
  /// <remarks>
  /// This is the base class for <see cref="Line" /> and <see cref="Border" /> classes.
  /// </remarks>
  /// <author> John Champion </author>
  /// <version> $Revision: 3.2 $ $Date: 2007-03-17 18:43:44 $ </version>
  [Serializable]
  public class LineBase : ICloneable, ISerializable
  {

  #region Defaults

    /// <summary>
    /// A simple struct that defines the
    /// default property values for the <see cref="LineBase"/> class.
    /// </summary>
    public struct Default
    {
      /// <summary>
      /// The default mode for displaying line segments (<see cref="LineBase.IsVisible"/>
      /// property).  True to show the line segments, false to hide them.
      /// </summary>
      public static bool IsVisible = true;
      /// <summary>
      /// The default width for line segments (<see cref="LineBase.Width"/> property).
      /// Units are points (1/72 inch).
      /// </summary>
      public static float Width = 1;
      /// <summary>
      /// The default value for the <see cref="LineBase.IsAntiAlias"/>
      /// property.
      /// </summary>
      public static bool IsAntiAlias = false;

      /// <summary>
      /// The default drawing style for line segments (<see cref="LineBase.Style"/> property).
      /// This is defined with the <see cref="DashStyle"/> enumeration.
      /// </summary>
      public static DashStyle Style = DashStyle.Solid;
      /// <summary>
      /// The default "dash on" size for drawing the line
      /// (<see cref="LineBase.DashOn"/> property). Units are in points (1/72 inch).
      /// </summary>
      public static float DashOn = 1.0F;
      /// <summary>
      /// The default "dash off" size for drawing the the line
      /// (<see cref="LineBase.DashOff"/> property). Units are in points (1/72 inch).
      /// </summary>
      public static float DashOff = 1.0F;

      /// <summary>
      /// The default color for the line.
      /// This is the default value for the <see cref="LineBase.Color"/> property.
      /// </summary>
      public static Color Color = Color.Black;
    }

  #endregion

  #region Properties

    /// <summary>
    /// The color of the <see cref="Line"/>.  Note that this color value can be
    /// overridden if the <see cref="Fill.Type">GradientFill.Type</see> is one of the
    /// <see cref="FillType.GradientByX" />,
    /// <see cref="FillType.GradientByY" />, <see cref="FillType.GradientByZ" />,
    /// and <see cref="FillType.GradientByColorValue" /> types.
    /// </summary>
    /// <seealso cref="GradientFill"/>
    public Color Color { get; set; }

    /// <summary>
    /// The style of the <see cref="Line"/>, defined as a <see cref="DashStyle"/> enum.
    /// This allows the line to be solid, dashed, or dotted.
    /// </summary>
    /// <seealso cref="Default.Style"/>
    /// <seealso cref="DashOn" />
    /// <seealso cref="DashOff" />
    public DashStyle Style { get; set; }

    /// <summary>
    /// The "Dash On" mode for drawing the line.
    /// </summary>
    /// <remarks>
    /// This is the distance, in points (1/72 inch), of the dash segments that make up
    /// the dashed grid lines.  This setting is only valid if 
    /// <see cref="Style" /> is set to <see cref="DashStyle.Custom" />.
    /// </remarks>
    /// <value>The dash on length is defined in points (1/72 inch)</value>
    /// <seealso cref="DashOff"/>
    /// <seealso cref="IsVisible"/>
    /// <seealso cref="Default.DashOn"/>.
    public float DashOn { get; set; }

    /// <summary>
    /// The "Dash Off" mode for drawing the line.
    /// </summary>
    /// <remarks>
    /// This is the distance, in points (1/72 inch), of the spaces between the dash
    /// segments that make up the dashed grid lines.  This setting is only valid if 
    /// <see cref="Style" /> is set to <see cref="DashStyle.Custom" />.
    /// </remarks>
    /// <value>The dash off length is defined in points (1/72 inch)</value>
    /// <seealso cref="DashOn"/>
    /// <seealso cref="IsVisible"/>
    /// <seealso cref="Default.DashOff"/>.
    public float DashOff { get; set; }

    /// <summary>
    /// The pen width used to draw the <see cref="Line"/>, in points (1/72 inch)
    /// </summary>
    /// <seealso cref="Default.Width"/>
    public float Width { get; set; }

    /// <summary>
    /// Gets or sets a property that shows or hides the <see cref="Line"/>.
    /// </summary>
    /// <value>true to show the line, false to hide it</value>
    /// <seealso cref="Default.IsVisible"/>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets a value that determines if the lines are drawn using
    /// Anti-Aliasing capabilities from the <see cref="Graphics" /> class.
    /// </summary>
    /// <remarks>
    /// If this value is set to true, then the <see cref="Graphics.SmoothingMode" />
    /// property will be set to <see cref="SmoothingMode.HighQuality" /> only while
    /// this <see cref="Line" /> is drawn.  A value of false will leave the value of
    /// <see cref="Graphics.SmoothingMode" /> unchanged.
    /// </remarks>
    public bool IsAntiAlias { get; set; }

    /// <summary>
    /// Gets or sets a custom <see cref="Fill" /> class.
    /// </summary>
    /// <remarks>This fill is used strictly for <see cref="FillType.GradientByX" />,
    /// <see cref="FillType.GradientByY" />, <see cref="FillType.GradientByZ" />,
    /// and <see cref="FillType.GradientByColorValue" /> calculations to determine
    /// the color of the line.  It overrides the <see cref="Color" /> property if
    /// one of the above <see cref="FillType" /> values are selected.
    /// </remarks>
    /// <seealso cref="Color"/>
    public Fill GradientFill { get; set; }

  #endregion

  #region Constructors

    /// <summary>
    /// Default constructor that sets all <see cref="LineBase"/> properties to default
    /// values as defined in the <see cref="Default"/> class.
    /// </summary>
    public LineBase()
      : this( Color.Empty )
    {
    }

    /// <summary>
    /// Constructor that sets the color property to the specified value, and sets
    /// the remaining <see cref="LineBase"/> properties to default
    /// values as defined in the <see cref="Default"/> class.
    /// </summary>
    /// <param name="color">The color to assign to this new Line object</param>
    public LineBase( Color color )
      : this( color, Color.Empty )
    {}

    /// <summary>
    /// Constructor that sets the color property to the specified value, and sets
    /// the remaining <see cref="LineBase"/> properties to default
    /// values as defined in the <see cref="Default"/> class.
    /// </summary>
    /// <param name="color">The color to assign to this new Line object</param>
    public LineBase( Color color, Color color2 )
    {
      Width        = Default.Width;
      Style        = Default.Style;
      DashOn       = Default.DashOn;
      DashOff      = Default.DashOff;
      IsVisible    = Default.IsVisible;
      Color        = color.IsEmpty ? Default.Color : color;
      IsAntiAlias  = Default.IsAntiAlias;
      GradientFill = new Fill(Color, color2) {Type = FillType.None};
    }

    /// <summary>
    /// The Copy Constructor
    /// </summary>
    /// <param name="rhs">The LineBase object from which to copy</param>
    public LineBase( LineBase rhs )
    {
      Width        = rhs.Width;
      Style        = rhs.Style;
      DashOn       = rhs.DashOn;
      DashOff      = rhs.DashOff;
      IsVisible    = rhs.IsVisible;
      Color        = rhs.Color;
      IsAntiAlias  = rhs.IsAntiAlias;
      GradientFill = new Fill( rhs.GradientFill );
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

    // /// <summary>
    // /// Typesafe, deep-copy clone method.
    // /// </summary>
    // /// <returns>A new, independent copy of this class</returns>
    //public LineBase Clone()
    //{
    //  return new LineBase( this );
    //}

  #endregion

  #region Serialization

    /// <summary>
    /// Current schema value that defines the version of the serialized file
    /// </summary>
    public const int schema0 = 12;

    /// <summary>
    /// Constructor for deserializing objects
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the
    /// serialized data
    /// </param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains
    /// the serialized data
    /// </param>
    protected LineBase( SerializationInfo info, StreamingContext context )
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      int sch = info.GetInt32( "schema0" );

      Width = info.GetSingle( "width" );
      Style = (DashStyle)info.GetValue( "style", typeof( DashStyle ) );
      DashOn = info.GetSingle( "dashOn" );
      DashOff = info.GetSingle( "dashOff" );
      IsVisible = info.GetBoolean( "isVisible" );
      IsAntiAlias = info.GetBoolean( "isAntiAlias" );
      Color = (Color)info.GetValue( "color", typeof( Color ) );
      GradientFill = (Fill)info.GetValue( "gradientFill", typeof( Fill ) );
    }
    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize
    /// the target object
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the
    /// serialized data</param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the
    /// serialized data</param>
    [SecurityPermissionAttribute( SecurityAction.Demand, SerializationFormatter = true )]
    public virtual void GetObjectData( SerializationInfo info, StreamingContext context )
    {
      info.AddValue( "schema0", schema0 );

      info.AddValue( "width", Width );
      info.AddValue( "style", Style );
      info.AddValue( "dashOn", DashOn );
      info.AddValue( "dashOff", DashOff );
      info.AddValue( "isVisible", IsVisible );
      info.AddValue( "isAntiAlias", IsAntiAlias );
      info.AddValue( "color", Color );
      info.AddValue( "gradientFill", GradientFill );
    }

  #endregion

  #region Methods

    /// <summary>
    /// Create a <see cref="Pen" /> object based on the properties of this
    /// <see cref="LineBase" />.
    /// </summary>
    /// <param name="pane">The owner <see cref="GraphPane" /> of this
    /// <see cref="LineBase" />.
    /// </param>
    /// <param name="scaleFactor">
    /// The scaling factor to be used for rendering objects.  This is calculated and
    /// passed down by the parent <see cref="GraphPane"/> object using the
    /// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
    /// font sizes, etc. according to the actual size of the graph.
    /// </param>
    /// <returns>A <see cref="Pen" /> object with the properties of this <see cref="LineBase" />
    /// </returns>
    public Pen GetPen( PaneBase pane, float scaleFactor )
    {
      return GetPen( pane, scaleFactor, null );
    }

    /// <summary>
    /// Create a <see cref="Pen" /> object based on the properties of this
    /// <see cref="LineBase" />.
    /// </summary>
    /// <param name="pane">The owner <see cref="GraphPane" /> of this
    /// <see cref="LineBase" />.
    /// </param>
    /// <param name="scaleFactor">
    /// The scaling factor to be used for rendering objects.  This is calculated and
    /// passed down by the parent <see cref="GraphPane"/> object using the
    /// <see cref="PaneBase.CalcScaleFactor"/> method, and is used to proportionally adjust
    /// font sizes, etc. according to the actual size of the graph.
    /// </param>
    /// <param name="dataValue">The data value to be used for a value-based
    /// color gradient.  This is only applicable if <see cref="Fill.Type">GradientFill.Type</see>
    /// is one of <see cref="FillType.GradientByX"/>,
    /// <see cref="FillType.GradientByY"/>, <see cref="FillType.GradientByZ"/>,
    /// or <see cref="FillType.GradientByColorValue" />.
    /// </param>
    /// <returns>A <see cref="Pen" /> object with the properties of this <see cref="LineBase" />
    /// </returns>
    public Pen GetPen( PaneBase pane, float scaleFactor, IPointPair dataValue )
    {
      var color = Color;
      if ( GradientFill.IsGradientValueType )
        color = GradientFill.GetGradientColor( dataValue );

      var pen = new Pen(color, pane.ScaledPenWidth(Width, scaleFactor))
      {
        DashStyle = Style
      };

      if (Style != DashStyle.Custom) return pen;

      if (DashOff <= 1e-10 || DashOn <= 1e-10)
        pen.DashStyle = DashStyle.Solid;
      else
      {
        pen.DashStyle = DashStyle.Custom;
        pen.DashPattern = new[] {DashOn, DashOff};
      }

      return pen;
    }

  #endregion
  }
}
