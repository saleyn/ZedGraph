//============================================================================
// Author: Serge Aleynikov
// Date:   2016-09-01
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
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
  /// <summary>
  /// This class represents a horizontal line that stretches horizontally through a pane
  /// </summary>
  public class LineHObj : LineBase, ICloneable, ISerializable
  {
  #region Constructors
    public LineHObj(Axis axis, Color color, object tag = null) : base(color)
    {
      Value  = 0.0;
      Tag    = tag;
      _brush = new SolidBrush(color);
    }

    public LineHObj(LineHObj rhs) : base(rhs)
    {
      Value  = rhs.Value;
      Tag    = rhs.Tag;
      _brush = new SolidBrush(Color);
    }
    #endregion

    #region Serialization
    /// <summary>
    /// Current schema value that defines the version of the serialized file
    /// </summary>
    internal const int Schema = 1;

    /// <summary>
    /// Constructor for deserializing objects
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
    /// </param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
    /// </param>
    protected LineHObj(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      int ss = info.GetInt32("Schema");
      Value  = info.GetDouble("Value");
      Tag    = info.GetValue("Tag", typeof(object));
      _brush = new SolidBrush(Color);
    }
    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("Schema",  Schema);
      info.AddValue("Value",   Value);
      info.AddValue("Tag",     Tag);
    }
  #endregion

  #region Fields
    private readonly SolidBrush _brush;
  #endregion

  #region Properties
    /// <summary>
    /// Value associated with Y coordinate
    /// </summary>
    public double Value { get; set; }

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
    public object Tag { get; set; }

  #endregion

  #region Methods
    /// <summary>
    /// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
    /// calling the typed version of <see cref="Clone" />
    /// </summary>
    /// <returns>A deep copy of this object</returns>
    object ICloneable.Clone() { return this.Clone(); }

    /// <summary>
    /// Typesafe, deep-copy clone method.
    /// </summary>
    /// <returns>A new, independent copy of this class</returns>
    public LineHObj Clone() { return new LineHObj(this); }
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
    public void Draw(Graphics g, PaneBase pn, Axis axis, Matrix transform, float scaleFactor, float shift)
    {
      var pane = pn as GraphPane;
      if (pane == null) return;

      var ylab = axis.Scale.LocalTransform(Value);
      var y    = axis.Scale.Transform(Value);

      if (y < pane.Chart._rect.Top || y >= pane.Chart._rect.Bottom) return;

      var charHeight   = axis.Scale.FontSpec.Font.Height; // GetHeight(scaleFactor);
      var maxLabelSize = axis.Scale._maxSpace; // GetScaleMaxSpace(g, pane, scaleFactor, true);
      var maxSpace     = maxLabelSize.Height;
      var scaledTic    = axis.MajorTic.ScaledTic(scaleFactor);

      PointF[] tri =
      {
        new PointF(ylab,  1.0f),
        new PointF(ylab + charHeight/2.0f, scaledTic+3.0f),
        new PointF(ylab + charHeight/2.0f, scaledTic+maxSpace-1.0f),
        new PointF(ylab - charHeight/2.0f, scaledTic+maxSpace-1.0f),
        new PointF(ylab - charHeight/2.0f, scaledTic+3.0f)
      };

      g.FillPolygon(_brush, tri, FillMode.Winding);

      axis.Scale.DrawLabel(g, pane, 0, Value, ylab-1.0f, shift, maxSpace, scaledTic,
                           charHeight, scaleFactor);
      var old = g.Transform;
      g.Transform = transform;

      // get a pen according to this arrow properties
      using (var pen = GetPen(pane, scaleFactor))
        g.DrawLine(pen, pane.Chart._rect.Left, y, pane.Chart._rect.Right, y);

      g.Transform = old;
    }
    #endregion
  }

  [Serializable]
  public class LineHObjList : List<LineHObj>, ICloneable
  {
  #region Constructors
    /// <summary>
    /// Default constructor for the <see cref="GraphObjList"/> collection class
    /// </summary>
    public LineHObjList() {}

    /// <summary>
    /// The Copy Constructor
    /// </summary>
    /// <param name="rhs">The <see cref="GraphObjList"/> object from which to copy</param>
    public LineHObjList(LineHObjList rhs)
    {
      foreach (LineHObj item in rhs)
        this.Add((LineHObj)((ICloneable)item).Clone());
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
    public LineHObjList Clone()
    {
      return new LineHObjList(this);
    }

    #endregion

  #region Methods
    /// <summary>
    /// Indexer to access the specified <see cref="GraphObj"/> object by its <see cref="GraphObj.Tag"/>.
    /// Note that the <see cref="GraphObj.Tag"/> must be a <see cref="String"/> type for this method
    /// to work.
    /// </summary>
    /// <param name="tag">The <see cref="String"/> type tag to search for.</param>
    /// <value>A <see cref="GraphObj"/> object reference.</value>
    /// <seealso cref="IndexOfTag"/>
    public LineHObj this[string tag]
    {
      get
      {
        var    index = IndexOfTag(tag);
        return index >= 0 ? (this[index]) : null;
      }
    }

    /// <summary>
    /// Return the zero-based position index of the
    /// <see cref="GraphObj"/> with the specified <see cref="GraphObj.Tag"/>.
    /// </summary>
    /// <remarks>In order for this method to work, the <see cref="GraphObj.Tag"/>
    /// property must be of type <see cref="String"/>.</remarks>
    /// <param name="tag">The <see cref="String"/> tag that is in the
    /// <see cref="GraphObj.Tag"/> attribute of the item to be found.
    /// </param>
    /// <returns>The zero-based index of the specified <see cref="GraphObj"/>,
    /// or -1 if the <see cref="GraphObj"/> is not in the list</returns>
    public int IndexOfTag(object tag)
    {
      var index = 0;
      foreach (LineHObj p in this)
      {
        if (p.Tag.Equals(tag))
          return index;
        index++;
      }

      return -1;
    }

    #endregion

    #region Render Methods
    /// <summary>
    /// Render text to the specified <see cref="Graphics"/> device
    /// by calling the Draw method of each <see cref="GraphObj"/> object in
    /// the collection.
    /// </summary>
    /// <remarks>This method is normally only called by the Draw method
    /// of the parent <see cref="GraphPane"/> object.
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
    /// <param name="zOrder">A <see cref="ZOrder"/> enumeration that controls
    /// the placement of this <see cref="GraphObj"/> relative to other
    /// graphic objects.  The order of <see cref="GraphObj"/>'s with the
    /// same <see cref="ZOrder"/> value is control by their order in
    /// this <see cref="GraphObjList"/>.</param>
    internal void Draw(Graphics g, PaneBase pane, Axis axis, Matrix transform, float scaleFactor, float shift)
    {
      // Draw the items in reverse order, so the last items in the
      // list appear behind the first items (consistent with CurveList)
      for (int i = this.Count - 1; i >= 0; i--)
      {
        var item = this[i];
        if (!item.IsVisible) continue;

        item.Draw(g, (GraphPane)pane, axis, transform, scaleFactor, shift);
      }
    }

    /// <summary>
    /// Determine if a mouse point is within any <see cref="GraphObj"/>, and if so, 
    /// return the index number of the the <see cref="GraphObj"/>.
    /// </summary>
    /// <param name="mousePt">The screen point, in pixel coordinates.</param>
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
    /// <param name="index">The index number of the <see cref="TextObj"/>
    ///  that is under the mouse point.  The <see cref="TextObj"/> object is
    /// accessible via the <see cref="GraphObjList" /> indexer property.
    /// </param>
    /// <returns>true if the mouse point is within a <see cref="GraphObj"/> bounding
    /// box, false otherwise.</returns>
    /// <seealso cref="GraphPane.FindNearestObject"/>
    public bool FindPoint(PointF mousePt, Axis axis, Graphics g, float scaleFactor, out int index)
    {
      index = -1;

      // Search in reverse direction to honor the Z-order
      for (int i = Count - 1; i >= 0; i--)
      {
        var y = axis.Scale.Transform(this[i].Value);
        var w = this[i].Width / 2;

        if (mousePt.Y >= y - w && mousePt.Y <= y + w)
        {
          index = i;
          break;
        }
      }

      return index >= 0;
    }


    #endregion
  }
}
