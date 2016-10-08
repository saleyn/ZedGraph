//============================================================================
//PointPairList Class
//Copyright © 2006  John Champion
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZedGraph
{
  internal struct DoubleComparer : IComparer<double>
  {
    public int Compare(double x, double y) { return Math.Abs(x - y) < 1e-9 ? 0 : x > y ? 1 : -1; }

    public static bool LT(double x, double y) { return x < y || (y - x) < 1e-9; }
  }

  /// <summary>
  /// Interface that elements of StockPointList must support
  /// </summary>
  public interface IStockPt : IPointPair
  {
    DateTime TimeStamp { get; }
    double   Date      { get; }
    double   Open      { get; }
    double   High      { get; }
    double   Low       { get; }
    double   Close     { get; }

    new IStockPt Clone();
  }

  /// <summary>
  /// A collection class containing a list of <see cref="StockPt"/> objects
  /// that define the set of points to be displayed on the curve.
  /// </summary>
  /// 
  /// <author> John Champion based on code by Jerry Vos</author>
  /// <version> $Revision: 3.4 $ $Date: 2007-02-18 05:51:54 $ </version>
  [Serializable]
  public class StockPointList<T> : List<IStockPt>, IPointListEdit, IOrdinalPointList
    where T : IStockPt, new()
  {
    private readonly List<Tuple<double,int>> _dateIndex;
    private int                              _offset;
     
  #region Properties

    /// <summary>
    /// Indexer to access the specified <see cref="StockPt"/> object by
    /// its ordinal position in the list.
    /// </summary>
    /// <param name="index">The ordinal position (zero-based) of the
    /// <see cref="StockPt"/> object to be accessed.</param>
    /// <value>A <see cref="StockPt"/> object reference.</value>
    public new IPointPair this[int index]
    {
      get { return base[index]; }
      set
      {
        if (!(value is IStockPt))
          throw new InvalidOperationException($"Invalid value type of {value}");
        if (_dateIndex != null)
        {
          if (Math.Abs(base[index].X - value.X) > 1e-9)
            throw new ArgumentException
              ($"Cannot change date on item#{index}: {new XDate(base[index].X)} to {new XDate(value.X)}");
        }
        base[index] = ((IStockPt)value).Clone();
      }
    }

    /// <summary>
    /// Indexer for getting the index that corresponds to given date
    /// if the list contains ordinal dates.
    /// </summary>
    int IOrdinalPointList.Ordinal(double date)
    {
      var idx = indexOf(date);
      if (idx < 0 || idx == _dateIndex.Count) return -1;
      return _dateIndex[idx].Item2 - _offset;
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor for the collection class
    /// </summary>
    public StockPointList() : this(false) {}

    /// <summary>
    /// Constructor for the collection class that can enable ordinal index
    /// </summary>
    public StockPointList(bool ordinal)
    {
      _offset = 0;
      if (ordinal)
        _dateIndex = new List<Tuple<double, int>>();
    }

    /// <summary>
    /// The Copy Constructor
    /// </summary>
    /// <param name="rhs">The StockPointList from which to copy</param>
    public StockPointList( StockPointList<T> rhs )
      : this(rhs._dateIndex != null)
    {
      _offset = 0;

      foreach (var pp in rhs.Cast<IStockPt>())
      {
        Add(pp.Clone());

        if (rhs._dateIndex == null) continue;

        _dateIndex.Add(new Tuple<double, int>(pp.Date, Count-1));
      }
    }

    /// <summary>
    /// Implement the <see cref="ICloneable" /> interface in a typesafe manner by just
    /// calling the typed version of <see cref="Clone" />
    /// </summary>
    /// <returns>A deep copy of this object</returns>
    object ICloneable.Clone()
    {
      return Clone();
    }

    /// <summary>
    /// Typesafe, deep-copy clone method.
    /// </summary>
    /// <returns>A new, independent copy of this class</returns>
    public StockPointList<T> Clone()
    {
      return new StockPointList<T>( this );
    }

  #endregion

  #region Methods

    /// <summary>
    /// Add a <see cref="StockPt"/> object to the collection at the end of the list.
    /// </summary>
    /// <param name="point">The <see cref="StockPt"/> object to
    /// be added</param>
    public void Add( T point )
    {
      base.Add( point.Clone() );
      if (_dateIndex == null) return;

      var date = point.X;
      if (_dateIndex.Count == 0 || date > _dateIndex[_dateIndex.Count-1].Item1)
        _dateIndex.Add(new Tuple<double, int>(date, _offset + Count-1));
      else
        throw new ArgumentException($"Dates in time-series must be chronologically increasing ({date})");
    }

    /// <summary>
    /// Add a <see cref="PointPair"/> object to the collection at the end of the list.
    /// </summary>
    /// <param name="point">The <see cref="PointPair"/> object to be added</param>
    public void Add( IPointPair point )
    {
      if (!(point is StockPt))
        throw new ArgumentException("Only points of StockPt type can be added to StockPointList!");
      base.Add( ((T)point).Clone() );
    }

    /// <summary>
    /// Add a <see cref="StockPt"/> object to the collection at the end of the list using
    /// the specified values.  The unspecified values (low, open, close) are all set to
    /// <see cref="PointPairBase.Missing" />.
    /// </summary>
    /// <param name="date">An <see cref="XDate" /> value</param>
    /// <param name="high">The high value for the day</param>
    /// <returns>The zero-based ordinal index where the point was added in the list.</returns>
    public void Add( double date, double high )
    {
      add(date, PointPair.Missing, high, PointPair.Missing, PointPair.Missing, 0);
    }

    /// <summary>
    /// Add a single point to the <see cref="PointPairList"/> from values of type double.
    /// </summary>
    /// <param name="date">An <see cref="XDate" /> value</param>
    /// <param name="open">The opening value for the day</param>
    /// <param name="high">The high value for the day</param>
    /// <param name="low">The low value for the day</param>
    /// <param name="close">The closing value for the day</param>
    /// <param name="vol">The trading volume for the day</param>
    /// <returns>The zero-based ordinal index where the point was added in the list.</returns>
    public void Add(double date, double open, double high, double low, double close, int vol)
    {
      add(date, open, high, low, close, vol);
    }

    /// <summary>
    /// Access the <see cref="StockPt" /> at the specified ordinal index.
    /// </summary>
    /// <remarks>
    /// To be compatible with the <see cref="IPointList" /> interface, the
    /// <see cref="StockPointList" /> must implement an index that returns a
    /// <see cref="PointPair" /> rather than a <see cref="StockPt" />.  This method
    /// will return the actual <see cref="StockPt" /> at the specified position.
    /// </remarks>
    /// <param name="index">The ordinal position (zero-based) in the list</param>
    /// <returns>The specified <see cref="StockPt" />.
    /// </returns>
    public IPointPair GetAt( int index )
    {
      return base[index];
    }

    public new void RemoveAt(int index)
    {
      if (_dateIndex != null)
      {
        if (index > 0 || index < Count - 1)
          throw new ArgumentException
            ($"Cannot remove intermediate data points (index={index}, offset={_offset}, count={Count})");

        var point = this[index];
        var date  = point.X;

        var idx   = indexOf(date);
        if (idx > -1 && idx < _dateIndex.Count)
          _dateIndex.RemoveAt(idx);

        if (index == 0)
          _offset++;
      }

      base.RemoveAt(index);
    }

    public new void Clear()
    {
      base.Clear();
      _dateIndex?.Clear();
    }

    #endregion

    /// <summary>
    /// Returns an index of the first element which does not compare less than date value.
    /// </summary>
    public IPointPair IndexOf(double date)
    {
      var idx = indexOf(date);
      if (idx < 0 || idx == _dateIndex.Count) return null;
      var i = _dateIndex[idx].Item2 - _offset;
      return base[i];
    }

    private int indexOf(double date)
    {
      if (_dateIndex == null)    return -1;
      if (_dateIndex.Count == 0) return  0;

      //var comp = Comparer<T>.Default;
      int    lo = 0, hi = Count-1;
      while (lo < hi)
      {
        var m = lo + (hi - lo) / 2;
        if (DoubleComparer.LT(_dateIndex[m].Item1, date))
          lo = m + 1;
        else
          hi = m - 1;
      }
      return DoubleComparer.LT(_dateIndex[lo].Item1, date) ? lo+1 : lo;
    }

    private void add(double date, double open, double high, double low, double close, int vol)
    {
      if (typeof(T) != typeof(StockPt))
        throw new InvalidOperationException($"Invalid data type {typeof(T)}: expected {typeof(StockPt)}");
      var p  = new T() as StockPt;
      p.Date  = date;
      p.Open  = open;
      p.High  = high;
      p.Low   = low;
      p.Close = close;
      p.Vol   = vol;
      Add(p);
    }

    public new IEnumerator<IPointPair> GetEnumerator()
    {
      foreach (var p in this)
        yield return p;
    }
  }
}