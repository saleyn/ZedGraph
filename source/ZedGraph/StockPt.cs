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
  /// <summary>
  /// The basic <see cref="PointPair" /> class holds three data values (X, Y, Z).  This
  /// class extends the basic PointPair to contain five data values (X, Y, Z, Open, Close).
  /// </summary>
  /// <remarks>
  /// The values are remapped to <see cref="Date" />, <see cref="High" />,
  /// <see cref="Low" />, <see cref="Open" />, and <see cref="Close" />.
  /// </remarks>
  /// 
  /// <author> John Champion </author>
  /// <version> $Revision: 3.4 $ $Date: 2007-02-07 07:46:46 $ </version>
  [Serializable]
  public class StockPt : PointPair, IStockPt
  {

    #region Member variables

    // member variable mapping:
    //   Date  = X
    //   Close = Y
    //   Low   = Z
    //   Open  = Open
    //   High  = High
    //   Vol   = Vol

    /// <summary>
    /// This opening value
    /// </summary>
    public float Open { get; set; }

    /// <summary>
    /// This closing value
    /// </summary>
    public float High { get; set; }

    /// <summary>
    /// Volume value
    /// </summary>
    public int    VolBuy  { get; set; }
    public int    VolSell { get; set; }
    public int    Volume  => VolBuy + VolSell;

    /// <summary>
    /// This is a user value that can be anything.  It is used to provide special 
    /// property-based coloration to the graph elements.
    /// </summary>
    private double _colorValue;

    #endregion

    #region Constructors

    /// <summary>
    /// Default Constructor
    /// </summary>
    public StockPt() : this(0, 0, 0, 0, 0, 0, 0, null)
    {
    }

    /// <summary>
    /// Construct a new StockPt from the specified data values
    /// </summary>
    /// <param name="date">The trading date (<see cref="XDate" />)</param>
    /// <param name="open">The opening stock price</param>
    /// <param name="close">The closing stock price</param>
    /// <param name="high">The daily high stock price</param>
    /// <param name="low">The daily low stock price</param>
    /// <param name="vol">The daily trading volume</param>
    public StockPt(double date, float open, float high, float low, float close, int volBuy, int volSell)
      : this(date, open, high, low, close, volBuy, volSell, null)
    {
    }

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
    public StockPt(double date, float open, float high, float low, float close, int volBuy, int volSell, string tag)
      : base(date, close, low)
    {
      Open       = open;
      High       = high;
      VolBuy     = volBuy;
      VolSell    = volSell;
      ColorValue = PointPair.Missing;
      Tag        = tag;
    }

    /// <summary>
    /// The StockPt copy constructor.
    /// </summary>
    /// <param name="rhs">The basis for the copy.</param>
    public StockPt(StockPt rhs) : base((IPointPair)rhs)
    {
      Open       = rhs.Open;
      High       = rhs.High;
      VolBuy     = rhs.VolBuy;
      VolSell    = rhs.VolSell;
      ColorValue = rhs.ColorValue;
      Tag        = rhs.Tag is ICloneable ? ((ICloneable)rhs.Tag).Clone() : rhs.Tag;
    }

    /// <summary>
    /// The StockPt copy constructor.
    /// </summary>
    /// <param name="rhs">The basis for the copy.</param>
    public StockPt(IPointPair rhs) : base(rhs)
    {
      if (rhs is IOHLCV)
      {
        var pt     = rhs as IOHLCV;
        Open       = pt.Open;
        VolBuy     = pt.VolBuy;
        VolSell    = pt.VolSell;
        High       = pt.High;
        ColorValue = ((StockPt)rhs).ColorValue;
      }
      else
      {
        Open       = PointPair.Missing;
        High       = PointPair.Missing;
        VolBuy     = 0;
        VolSell    = 0;
        ColorValue = PointPair.Missing;
      }
    }

    #endregion

    #region Serialization

    /// <summary>
    /// Current schema value that defines the version of the serialized file
    /// </summary>
    private const int schema3 = 12;

    /// <summary>
    /// Constructor for deserializing objects
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data
    /// </param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data
    /// </param>
    protected StockPt(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      // The schema value is just a file version parameter.  You can use it to make future versions
      // backwards compatible as new member variables are added to classes
      int sch = info.GetInt32("schema3");

      Open       = (float)info.GetDouble("Open");
      High       = (float)info.GetDouble("High");
      VolBuy     = info.GetInt32("VolBuy");
      VolSell    = info.GetInt32("VolSell");
      ColorValue = info.GetDouble("ColorValue");
    }

    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
    [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info,    context);
      info.AddValue("schema3",    schema3);
      info.AddValue("Open",       Open);
      info.AddValue("High",       High);
      info.AddValue("VolBuy",     VolBuy);
      info.AddValue("VolSell",    VolSell);
      info.AddValue("ColorValue", ColorValue);
    }

    #endregion

    #region Properties

    public DateTime TimeStamp => new XDate(Date).DateTime;

    /// <summary>
    /// Map the Date property to the X value
    /// </summary>
    public double Date { get { return X; } set { X = value; } }

    /// <summary>
    /// Map the high property to the Y value
    /// </summary>
    public float  Close { get { return (float)Y; } set { Y = value; } }

    /// <summary>
    /// Trading volume. Map the low property to the Z value
    /// </summary>
    public float  Low { get { return (float)Z; } set { Z = value; } } 

    public override double HighValue => High;

    /// <summary>
    /// The ColorValue property.  This is used with the
    /// <see cref="FillType.GradientByColorValue" /> option.
    /// </summary>
    public override double ColorValue
    {
      get { return _colorValue; }
      set { _colorValue = value; }
    }

    public new IStockPt Clone()
    {
      return new StockPt(this);
    }

    /// <summary>
    /// Readonly value that determines if either the Date, Close, Open, High, or Low
    /// coordinate in this StockPt is an invalid (not plotable) value.
    /// It is considered invalid if it is missing (equal to System.double.Max),
    /// Infinity, or NaN.
    /// </summary>
    /// <returns>true if any value is invalid</returns>
    public bool IsInvalid5D => Date == PointPair.Missing ||
                               Close == PointPair.Missing ||
                               Open == PointPair.Missing ||
                               High == PointPair.Missing ||
                               Low == PointPair.Missing ||
                               double.IsInfinity(Date) ||
                               double.IsInfinity(Close) ||
                               double.IsInfinity(Open) ||
                               double.IsInfinity(High) ||
                               double.IsInfinity(Low) ||
                               double.IsNaN(Date) ||
                               double.IsNaN(Close) ||
                               double.IsNaN(Open) ||
                               double.IsNaN(High) ||
                               double.IsNaN(Low);

    #endregion

    #region Methods

    /// <summary>
    /// Format this StockPt value using the default format.  Example:  "( 12.345, -16.876 )".
    /// The two double values are formatted with the "g" format type.
    /// </summary>
    /// <param name="isShowAll">true to show all the value coordinates</param>
    /// <returns>A string representation of the <see cref="StockPt" />.</returns>
    public override string ToString(bool isShowAll)
    {
      return ToString(PointPair.DefaultFormat, isShowAll);
    }

    /// <summary>
    /// Format this PointPair value using a general format string.
    /// Example:  a format string of "e2" would give "( 1.23e+001, -1.69e+001 )".
    /// If <see paramref="isShowAll"/>
    /// is true, then the third all coordinates are shown.
    /// </summary>
    /// <param name="format">A format string that will be used to format each of
    /// the two double type values (see <see cref="System.double.ToString()"/>).</param>
    /// <returns>A string representation of the PointPair</returns>
    /// <param name="isShowAll">true to show all the value coordinates</param>
    public override string ToString(string format, bool isShowAll)
    {
      var date = new XDate(Date).DateTime;
      return isShowAll
        ? $"({date:yyyy-MM-dd hh:mm:ss}, O:{Open:format}, H:{High:format}, L:{Low:format}, C:{Close:format})"
        : $"({date:yyyy-MM-dd hh:mm:ss}, {Close:format})";
    }

    #endregion
  }
}