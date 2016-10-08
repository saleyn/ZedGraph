//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2006  John Champion
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

#region Using directives

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

#endregion

namespace ZedGraph
{
  /// <summary>
  /// This class handles the drawing of the curve <see cref="OHLCBar"/> objects.
  /// </summary>
  /// 
  /// <author> John Champion </author>
  /// <version> $Revision: 3.5 $ $Date: 2007-04-16 00:03:02 $ </version>
  [Serializable]
  public class OHLCBarCluster : OHLCBar, ICloneable
  {
    #region Fields

    /// <summary>
    /// Current schema value that defines the version of the serialized file
    /// </summary>
    private const int schema2 = 11;

    #endregion

    #region Defaults

    /// <summary>
    /// A simple struct that defines the
    /// default property values for the <see cref="OHLCBarCluster"/> class.
    /// </summary>
    public new struct Default
    {
      /// <summary>
      /// The default fillcolor for drawing the rising case CandleSticks
      /// (<see cref="JapaneseCandleStick.RisingFill"/> property).
      /// </summary>
      public static Color RisingColor      = Color.White;
      /// <summary>
      /// The default fillcolor for drawing the falling case CandleSticks
      /// (<see cref ="JapaneseCandleStick.FallingFill"/> property).
      /// </summary>
      public static Color FallingColor     = Color.Black;

      public static Color ClusterBaseColor = Color.DimGray;

      public static float ClusterStep      = 0.0005f;
    }

    #endregion

    #region Properties

    public float                  ClusterStep   = Default.ClusterStep;
    public SortedList<int, Color> VolumeHeatMap = new SortedList<int, Color>();

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor that sets all <see cref="OHLCBar"/> properties to
    /// default values as defined in the <see cref="Default"/> class.
    /// </summary>
    public OHLCBarCluster() : this(LineBase.Default.Color) { }

    /// <summary>
    /// Default constructor that sets the
    /// <see cref="Color"/> as specified, and the remaining
    /// <see cref="OHLCBar"/> properties to default
    /// values as defined in the <see cref="Default"/> class.
    /// </summary>
    /// <param name="color">A <see cref="Color"/> value indicating
    /// the color of the symbol
    /// </param>
    public OHLCBarCluster(Color color) : base(color)
    {}

    /// <summary>
    /// The Copy Constructor
    /// </summary>
    /// <param name="rhs">The <see cref="OHLCBar"/> object from which to copy</param>
    public OHLCBarCluster(OHLCBarCluster rhs) : base(rhs)
    {}

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
    public new OHLCBarCluster Clone()
    {
      return new OHLCBarCluster(this);
    }

    #endregion

    #region Serialization

    /// <summary>
    /// Constructor for deserializing objects
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
    /// </param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
    /// </param>
    protected OHLCBarCluster(SerializationInfo info, StreamingContext context) :
      base(info, context)
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      int sch = info.GetInt32("schema");
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
      info.AddValue("schema", schema2);
    }

    #endregion

    #region Rendering Methods

    protected override void BeforeDraw(Graphics g, GraphPane pane, 
                                       Axis valueAxis, CurveItem curve, IPointPair pt,
                                       float pixBase, float pixHigh, float pixLow, float halfSize)
    {
      var p = pt as CandleClusterPt;

      if (p?.Volumes != null)
      {
        //using (var backBrush = new SolidBrush(Default.ClusterBaseColor))
        //  g.FillRectangle(backBrush, pixBase - halfSize, pixHigh, halfSize*2, pixLow - pixHigh);

        var prevPricePix = pixLow;

        foreach (var v in p.Volumes)
        {
          var nextPricePix = valueAxis.Scale.Transform(curve.IsOverrideOrdinal, 0, v.Item1);

          var color = getVolumeColor(v.Item2);

          if (color != Default.ClusterBaseColor)
            using (var brush = new SolidBrush(color))
              g.FillRectangle(brush, pixBase - halfSize/2, nextPricePix, halfSize, prevPricePix-nextPricePix);

          prevPricePix = nextPricePix;
        }
      }

      base.BeforeDraw(g, pane, valueAxis, curve, pt, pixBase, pixHigh, pixLow, halfSize);
    }

    #endregion

    #region Private Methods

    private Color getVolumeColor(int volume)
    {
      var color = VolumeHeatMap.LastOrDefault(x => x.Key <= volume);
      return color.Value.IsEmpty ? Default.ClusterBaseColor : color.Value;
    }

    #endregion
  }
}