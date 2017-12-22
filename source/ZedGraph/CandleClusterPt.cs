//============================================================================
//PointPair4 Class
//Copyright © 2006  Jerry Vos & John Champion
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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ZedGraph
{
  public interface IClusterVolume
  {
    float Price    { get; }
    float VolSell  { get; }
    float VolBuy   { get; }
    float Volume   { get; } // Total volume
    float VolDelta { get; } // Buy-Sell volume
  }

  public interface ICandleClusteredVolume : IOHLCV
  {
    IClusterVolume[] Volumes { get; set; }
  }

  /// <summary>
  /// The <see cref="CandleClusterPt"/> class holds OHLC data plus a vector of clustered volumes.
  /// This class extends the <see cref="StockPt"/> to contain Volumes property.
  /// </summary>
  /// <remarks>
  [Serializable]
  public class CandleClusterPt : StockPt, ICandleClusteredVolume
  {
    #region Constructors

    /// <summary>
    /// Default Constructor
    /// </summary>
    public CandleClusterPt() : this(0, 0, 0, 0, 0, 0, 0)
    {}

    /// <summary>
    /// Construct a new StockPt from the specified data values including a Tag property
    /// </summary>
    /// <param name="date">The trading date (<see cref="XDate" />)</param>
    /// <param name="open">The opening stock price</param>
    /// <param name="close">The closing stock price</param>
    /// <param name="high">The daily high stock price</param>
    /// <param name="low">The daily low stock price</param>
    /// <param name="vol">The daily trading volume</param>
    /// <param name="tag">The user-defined <see cref="PointPair.Tag" /> property.</param>
    public CandleClusterPt(double date, float open, float high, float low, float close,
                           float volBuy, float volSell, string tag = null,
                           IClusterVolume[] volumes = null)
      : base(date, open, high, low, close, volBuy, volSell, tag)
    {
      if (volumes != null)
        Volumes = volumes;
    }

    /// <summary>
    /// The StockPt copy constructor.
    /// </summary>
    /// <param name="rhs">The basis for the copy.</param>
    public CandleClusterPt(ICandleClusteredVolume rhs, bool cloneVolumes = true)
      : base(rhs.Date, rhs.Open, rhs.High, rhs.Low, rhs.Close, rhs.VolBuy, rhs.VolSell)
    {
      ctor(rhs, cloneVolumes);
    }

    /// <summary>
    /// The StockPt copy constructor.
    /// </summary>
    /// <param name="rhs">The basis for the copy.</param>
    public CandleClusterPt(CandleClusterPt rhs, bool cloneVolumes = true)
      : this((IPointPair)rhs, cloneVolumes)
    {}

    /// <summary>
    /// The StockPt copy constructor.
    /// </summary>
    /// <param name="rhs">The basis for the copy.</param>
    public CandleClusterPt(IPointPair rhs, bool cloneVolumes = true) : base(rhs)
    {
      ctor(rhs as ICandleClusteredVolume, cloneVolumes);
    }

    private void ctor(ICandleClusteredVolume rhs, bool cloneVolumes)
    {
      if (rhs != null)
      {
        if (cloneVolumes)
        {
          Volumes = new IClusterVolume[rhs.Volumes.Length];
          Array.Copy(rhs.Volumes, Volumes, rhs.Volumes.Length);
        }
        else
          Volumes = rhs.Volumes;
      }
      else
        Volumes = null;
    }

    #endregion

    #region Serialization

    /// <summary>
    /// Current schema value that defines the version of the serialized file
    /// </summary>
    private const int schema4 = 12;

    /// <summary>
    /// Constructor for deserializing objects
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
    /// </param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
    /// </param>
    protected CandleClusterPt(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      var sch = info.GetInt32("schema3");

      Volumes = (IClusterVolume[])info.GetValue("volumes", typeof(IClusterVolume[]));
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
      info.AddValue("schema4", schema4);
      info.AddValue("volumes", Volumes);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Array of volume clusters aggregated by price steps: {Price, Volume}
    /// </summary>
    public IClusterVolume[] Volumes { get; set; }

    #endregion

  }
}