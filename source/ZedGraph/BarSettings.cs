//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright � 2006  John Champion
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
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
  /// <summary>
  ///   Class that handles the global settings for bar charts
  /// </summary>
  /// <author> John Champion </author>
  /// <version> $Revision: 3.6 $ $Date: 2007-12-30 23:27:39 $ </version>
  [Serializable]
  public class BarSettings : ISerializable
  {
    #region Defaults

    /// <summary>
    ///   A simple struct that defines the
    ///   default property values for the <see cref="BarSettings" /> class.
    /// </summary>
    public struct Default
    {
      /// <summary>
      ///   The default value for the <see cref="BarSettings.Base" />, which determines the base
      ///   <see cref="Axis" /> from which the <see cref="Bar" /> graphs will be displayed.
      /// </summary>
      /// <seealso cref="BarSettings.Base" />
      public static BarBase Base = BarBase.X;

      /// <summary>
      ///   The default width of a bar cluster
      ///   on a <see cref="Bar" /> graph.  This value only applies to
      ///   <see cref="Bar" /> graphs, and only when the
      ///   <see cref="Axis.Type" /> is <see cref="AxisType.Linear" />,
      ///   <see cref="AxisType.Log" /> or <see cref="AxisType.Date" />.
      ///   This dimension is expressed in terms of X scale user units.
      /// </summary>
      /// <seealso cref="Default.MinClusterGap" />
      /// <seealso cref="BarSettings.MinBarGap" />
      public static double ClusterScaleWidth = 1.0;

      /// <summary>
      ///   The default value for <see cref="BarSettings.ClusterScaleWidthAuto" />.
      /// </summary>
      public static bool ClusterScaleWidthAuto = true;

      /// <summary>
      ///   The default dimension gap between each individual bar within a bar cluster
      ///   on a <see cref="Bar" /> graph.
      ///   This dimension is expressed in terms of the normal bar width.
      /// </summary>
      /// <seealso cref="Default.MinClusterGap" />
      /// <seealso cref="BarSettings.MinBarGap" />
      public static float MinBarGap = 0.2F;

      /// <summary>
      ///   The default dimension gap between clusters of bars on a
      ///   <see cref="Bar" /> graph.
      ///   This dimension is expressed in terms of the normal bar width.
      /// </summary>
      /// <seealso cref="Default.MinBarGap" />
      /// <seealso cref="BarSettings.MinClusterGap" />
      public static float MinClusterGap = 1.0F;

      /// <summary>
      ///   The default value for the <see cref="BarSettings.Type" /> property, which
      ///   determines if the bars are drawn overlapping eachother in a "stacked" format,
      ///   or side-by-side in a "cluster" format.  See the <see cref="ZedGraph.BarType" />
      ///   for more information.
      /// </summary>
      /// <seealso cref="BarSettings.Type" />
      public static BarType Type = BarType.Cluster;
    }

    #endregion

    #region Fields

    /// <summary>
    ///   Private field that determines the width of a bar cluster (for bar charts)
    ///   in user scale units.  Normally, this value is 1.0 because bar charts are typically
    ///   <see cref="AxisType.Ordinal" /> or <see cref="AxisType.Text" />, and the bars are
    ///   defined at ordinal values (1.0 scale units apart).  For <see cref="AxisType.Linear" />
    ///   or other scale types, you can use this value to scale the bars to an arbitrary
    ///   user scale. Use the public property <see cref="ClusterScaleWidth" /> to access this
    ///   value.
    /// </summary>
    internal double _clusterScaleWidth;

    /// <summary>
    ///   private field that stores the owner GraphPane that contains this BarSettings instance.
    /// </summary>
    internal GraphPane _ownerPane;

    #endregion

    #region Constructors

    /// <summary>
    ///   Constructor to build a <see cref="BarSettings" /> instance from the defaults.
    /// </summary>
    public BarSettings(GraphPane parentPane)
    {
      MinClusterGap = Default.MinClusterGap;
      MinBarGap = Default.MinBarGap;
      _clusterScaleWidth = Default.ClusterScaleWidth;
      ClusterScaleWidthAuto = Default.ClusterScaleWidthAuto;
      Base = Default.Base;
      Type = Default.Type;

      _ownerPane = parentPane;
    }

    /// <summary>
    ///   Copy constructor
    /// </summary>
    /// <param name="rhs">the <see cref="BarSettings" /> instance to be copied.</param>
    /// <param name="parentPane">
    ///   The <see cref="GraphPane" /> that will be the
    ///   parent of this new BarSettings object.
    /// </param>
    public BarSettings(BarSettings rhs, GraphPane parentPane)
    {
      MinClusterGap = rhs.MinClusterGap;
      MinBarGap = rhs.MinBarGap;
      _clusterScaleWidth = rhs._clusterScaleWidth;
      ClusterScaleWidthAuto = rhs.ClusterScaleWidthAuto;
      Base = rhs.Base;
      Type = rhs.Type;

      _ownerPane = parentPane;
    }

    #endregion

    #region Bar Properties

    /// <summary>
    ///   Determines the base axis from which <see cref="Bar" />
    ///   graphs will be displayed.
    /// </summary>
    /// <remarks>
    ///   The base axis is the axis from which the bars grow with
    ///   increasing value. The value is of the enumeration type <see cref="ZedGraph.BarBase" />.
    /// </remarks>
    /// <seealso cref="Default.Base" />
    public BarBase Base { get; set; }

    /// <summary>
    ///   The width of an individual bar cluster on a <see cref="Bar" /> graph.
    ///   This value only applies to bar graphs plotted on non-ordinal X axis
    ///   types (<see cref="AxisType.Linear" />, <see cref="AxisType.Log" />, and
    ///   <see cref="AxisType.Date" />.
    /// </summary>
    /// <remarks>
    ///   This value can be calculated automatically if <see cref="ClusterScaleWidthAuto" />
    ///   is set to true.  In this case, ClusterScaleWidth will be calculated if
    ///   <see cref="Base" /> refers to an <see cref="Axis" /> of a non-ordinal type
    ///   (<see cref="Scale.IsAnyOrdinal" /> is false).  The ClusterScaleWidth is calculated
    ///   from the minimum difference found between any two points on the <see cref="Base" />
    ///   <see cref="Axis" /> for any <see cref="BarItem" /> in the
    ///   <see cref="GraphPane.CurveList" />.  The ClusterScaleWidth is set automatically
    ///   each time <see cref="GraphPane.AxisChange()" /> is called.  Calculations are
    ///   done by the <see cref="BarSettings.CalcClusterScaleWidth" /> method.
    /// </remarks>
    /// <seealso cref="Default.ClusterScaleWidth" />
    /// <seealso cref="ClusterScaleWidthAuto" />
    /// <seealso cref="MinBarGap" />
    /// <seealso cref="MinClusterGap" />
    public double ClusterScaleWidth
    {
      get { return _clusterScaleWidth; }
      set
      {
        _clusterScaleWidth = value;
        ClusterScaleWidthAuto = false;
      }
    }

    /// <summary>
    ///   Gets or sets a property that determines if the <see cref="ClusterScaleWidth" /> will be
    ///   calculated automatically.
    /// </summary>
    /// <remarks>
    ///   true for the <see cref="ClusterScaleWidth" /> to be calculated
    ///   automatically based on the available data, false otherwise.  This value will
    ///   be set to false automatically if the <see cref="ClusterScaleWidth" /> value
    ///   is changed by the user.
    /// </remarks>
    /// <seealso cref="Default.ClusterScaleWidthAuto" />
    /// <seealso cref="ClusterScaleWidth" />
    public bool ClusterScaleWidthAuto { get; set; }

    /// <summary>
    ///   The minimum space between individual <see cref="Bar">Bars</see>
    ///   within a cluster, expressed as a
    ///   fraction of the bar size.
    /// </summary>
    /// <seealso cref="Default.MinBarGap" />
    /// <seealso cref="MinClusterGap" />
    /// <seealso cref="ClusterScaleWidth" />
    public float MinBarGap { get; set; }

    /// <summary>
    ///   The minimum space between <see cref="Bar" /> clusters, expressed as a
    ///   fraction of the bar size.
    /// </summary>
    /// <seealso cref="Default.MinClusterGap" />
    /// <seealso cref="MinBarGap" />
    /// <seealso cref="ClusterScaleWidth" />
    public float MinClusterGap { get; set; }

    /// <summary>
    ///   Determines how the <see cref="BarItem" />
    ///   graphs will be displayed. See the <see cref="ZedGraph.BarType" /> enum
    ///   for the individual types available.
    /// </summary>
    /// <seealso cref="Default.Type" />
    public BarType Type { get; set; }

    #endregion

    #region Serialization

    /// <summary>
    ///   Current schema value that defines the version of the serialized file
    /// </summary>
    private const int Schema = 10;

    /// <summary>
    ///   Constructor for deserializing objects
    /// </summary>
    /// <remarks>
    ///   You MUST set the _ownerPane property after deserializing a BarSettings object.
    /// </remarks>
    /// <param name="info">
    ///   A <see cref="SerializationInfo" /> instance that defines the
    ///   serialized data
    /// </param>
    /// <param name="context">
    ///   A <see cref="StreamingContext" /> instance that contains
    ///   the serialized data
    /// </param>
    internal BarSettings(SerializationInfo info, StreamingContext context)
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      var sch = info.GetInt32("schema");

      MinClusterGap         = info.GetSingle("minClusterGap");
      MinBarGap             = info.GetSingle("minBarGap");
      _clusterScaleWidth    = info.GetDouble("clusterScaleWidth");
      ClusterScaleWidthAuto = info.GetBoolean("clusterScaleWidthAuto");
      Base                  = (BarBase)info.GetValue("base", typeof(BarBase));
      Type                  = (BarType)info.GetValue("type", typeof(BarType));
    }

    /// <summary>
    ///   Populates a <see cref="SerializationInfo" /> instance with the data needed to serialize the target object
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo" /> instance that defines the serialized data</param>
    /// <param name="context">A <see cref="StreamingContext" /> instance that contains the serialized data</param>
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("schema", Schema);
      info.AddValue("minClusterGap",         MinClusterGap);
      info.AddValue("minBarGap",             MinBarGap);
      info.AddValue("clusterScaleWidth",     _clusterScaleWidth);
      info.AddValue("clusterScaleWidthAuto", ClusterScaleWidthAuto);
      info.AddValue("base",                  Base);
      info.AddValue("type",                  Type);
    }

    #endregion

    #region Methods

    /// <summary>
    ///   Determine the <see cref="Axis" /> from which the <see cref="Bar" /> charts are based.
    /// </summary>
    /// <seealso cref="ZedGraph.BarBase" />
    /// <seealso cref="BarSettings" />
    /// <seealso cref="ZedGraph.BarSettings.Base" />
    /// <seealso cref="Scale.GetClusterWidth(GraphPane)" />
    /// <returns>The <see cref="Axis" /> class for the axis from which the bars are based</returns>
    public Axis BarBaseAxis()
    {
      Axis barAxis;
      if (Base == BarBase.Y)
        barAxis = _ownerPane.YAxis;
      else if (Base == BarBase.Y2)
        barAxis = _ownerPane.Y2Axis;
      else if (Base == BarBase.X2)
        barAxis = _ownerPane.X2Axis;
      else
        barAxis = _ownerPane.XAxis;

      return barAxis;
    }

    /// <summary>
    ///   Calculate the width of an individual bar cluster on a <see cref="BarItem" /> graph.
    ///   This value only applies to bar graphs plotted on non-ordinal X axis
    ///   types (<see cref="Scale.IsAnyOrdinal" /> is false).
    /// </summary>
    /// <remarks>
    ///   This value can be calculated automatically if <see cref="ClusterScaleWidthAuto" />
    ///   is set to true.  In this case, ClusterScaleWidth will be calculated if
    ///   <see cref="Base" /> refers to an <see cref="Axis" /> of a non-ordinal type
    ///   (<see cref="Scale.IsAnyOrdinal" /> is false).  The ClusterScaleWidth is calculated
    ///   from the minimum difference found between any two points on the <see cref="Base" />
    ///   <see cref="Axis" /> for any <see cref="BarItem" /> in the
    ///   <see cref="GraphPane.CurveList" />.  The ClusterScaleWidth is set automatically
    ///   each time <see cref="GraphPane.AxisChange()" /> is called.
    /// </remarks>
    /// <seealso cref="Default.ClusterScaleWidth" />
    /// <seealso cref="ClusterScaleWidthAuto" />
    /// <seealso cref="MinBarGap" />
    /// <seealso cref="MinClusterGap" />
    public void CalcClusterScaleWidth()
    {
      var baseAxis = BarBaseAxis();

      // First, calculate the clusterScaleWidth for BarItem objects
      if (ClusterScaleWidthAuto && !baseAxis.Scale.IsAnyOrdinal)
      {
        var minStep = _ownerPane.CurveList
                                .Where(curve => curve is BarItem)
                                .Select(curve => GetMinStepSize(curve.Points, baseAxis))
                                .Concat(new[] { double.MaxValue })
                                .Min();

        if (minStep == double.MaxValue)
          minStep = 1.0;

        _clusterScaleWidth = minStep;
      }

      // Second, calculate the sizes of any HiLowBarItem and JapaneseCandleStickItem objects
      foreach (var curve in _ownerPane.CurveList)
      {
        var list = curve.Points;

        //        if ( curve is HiLowBarItem &&
        //            (curve as HiLowBarItem).Bar.IsAutoSize )
        //        {
        //          ( curve as HiLowBarItem ).Bar._userScaleSize =
        //                GetMinStepSize( list, baseAxis );
        //        }
        //        else if ( curve is JapaneseCandleStickItem &&
        if (curve is OHLCBarItem && (curve as OHLCBarItem).Bar.IsAutoSize)
          (curve as OHLCBarItem).Bar._userScaleSize = GetMinStepSize(list, baseAxis);
        else if (curve is JapaneseCandleStickItem && (curve as JapaneseCandleStickItem).Bar.IsAutoSize)
          (curve as JapaneseCandleStickItem).Bar._userScaleSize = GetMinStepSize(list, baseAxis);
      }
    }

    /// <summary>
    ///   Determine the width, in screen pixel units, of each bar cluster including
    ///   the cluster gaps and bar gaps.
    /// </summary>
    /// <remarks>
    ///   This method calls the <see cref="Scale.GetClusterWidth(GraphPane)" />
    ///   method for the base <see cref="Axis" /> for <see cref="Bar" /> graphs
    ///   (the base <see cref="Axis" /> is assigned by the <see cref="ZedGraph.BarSettings.Base" />
    ///   property).
    /// </remarks>
    /// <seealso cref="ZedGraph.BarBase" />
    /// <seealso cref="ZedGraph.BarSettings" />
    /// <seealso cref="Scale.GetClusterWidth(GraphPane)" />
    /// <seealso cref="ZedGraph.BarSettings.Type" />
    /// <returns>The width of each bar cluster, in pixel units</returns>
    public float GetClusterWidth()
    {
      return BarBaseAxis().Scale.GetClusterWidth(_ownerPane);
    }

    /// <summary>
    ///   Determine the minimum increment between individual points to be used for
    ///   calculating a bar size that fits without overlapping
    /// </summary>
    /// <param name="list">
    ///   The <see cref="IPointList" /> list of points for the bar
    ///   of interest
    /// </param>
    /// <param name="baseAxis">The base axis for the bar</param>
    /// <returns>The minimum increment between bars along the base axis</returns>
    internal static double GetMinStepSize(IPointList list, Axis baseAxis)
    {
      var minStep = double.MaxValue;

      if ((list.Count == 0) || baseAxis.Scale.IsAnyOrdinal)
        return 1.0;

      var lastPt = list[0];
      for (var i = 1; i < list.Count; i++)
      {
        var pt = list[i];
        if (!pt.IsInvalid || !lastPt.IsInvalid)
        {
          var step = baseAxis is IXAxis ? pt.X - lastPt.X : pt.Y - lastPt.Y;

          if ((step > 0) && (step < minStep))
            minStep = step;
        }

        lastPt = pt;
      }

      var range = baseAxis.Scale.MaxLinearized - baseAxis.Scale.MinLinearized;
      if (range <= 0)
        minStep = 1.0;
      //      else if ( minStep <= 0 || minStep < 0.001 * range || minStep > range )
      else if ((minStep <= 0) || (minStep > range))
        minStep = 0.1*range;

      return minStep;
    }

    #endregion
  }
}