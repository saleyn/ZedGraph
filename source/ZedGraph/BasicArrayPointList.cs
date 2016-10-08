//============================================================================
//BasicArrayPointList Class
//Copyright © 2005  John Champion
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZedGraph
{
  /// <summary>
  /// Compact structure with only two storage members X and Y.
  /// </summary>
  /// <remarks>intended for very large datasets</remarks>
  public struct CompactPt : IPointPair
  {
    public CompactPt(double x, double y) { X = x; Y = y; }
    public CompactPt(IPointPair rhs) { X = rhs.X; Y = rhs.Y; }

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get { return Y; } set { Y = value; } }

    public object Tag { get { return null; } set { } }
    public double LowValue => Y;
    public double HighValue => Y;
    public double ColorValue => Y;
    public bool IsValid => Y != PointPairBase.Missing && Y != PointPairBase.Missing;
    public bool IsInvalid => !IsValid;
    public bool IsFiltered => false;

    public IPointPair Clone()
    {
      return new CompactPt(X, Y);
    }

    object ICloneable.Clone()
    {
      return Clone();
    }
  }

  /// <summary>
  /// A data collection class for ZedGraph, provided as an alternative to <see cref="PointPairList" />.
  /// </summary>
  /// <remarks>
  /// The data storage class for ZedGraph can be any type, so long as it uses the <see cref="IPointList" />
  /// interface.  This class, albeit simple, is a demonstration of implementing the <see cref="IPointList" />
  /// interface to provide a simple data collection using only two arrays.  The <see cref="IPointList" />
  /// interface can also be used as a layer between ZedGraph and a database, for example.
  /// </remarks>
  /// <seealso cref="PointPairList" />
  /// <seealso cref="IPointList" />
  /// 
  /// <author> John Champion</author>
  /// <version> $Revision: 3.4 $ $Date: 2007-02-18 05:51:53 $ </version>
  [Serializable]
  public class BasicArrayPointList : IPointList
  {
    #region Fields

    /// <summary>
    /// Instance of an array of values
    /// </summary>
    private readonly CompactPt[] _data;

    #endregion

    #region Properties
    /// <summary>
    /// Indexer to access the specified <see cref="PointPair"/> object by
    /// its ordinal position in the list.
    /// </summary>
    /// <remarks>
    /// Returns <see cref="PointPairBase.Missing" /> for any value of <see paramref="index" />
    /// that is outside of its corresponding array bounds.
    /// </remarks>
    /// <param name="index">The ordinal position (zero-based) of the
    /// <see cref="PointPair"/> object to be accessed.</param>
    /// <value>A <see cref="PointPair"/> object reference.</value>
    public IPointPair this[ int index ]
    {
      get
      {
        return index >= 0 && index < _data.Length 
          ? _data[index]
          : new CompactPt(PointPairBase.Missing, PointPairBase.Missing);
      }
      set
      {
        if (index < 0 || index >= _data.Length)
          throw new ArgumentOutOfRangeException($"Invalid index: {index} (count={Count})");
        _data[index] = new CompactPt(value);
      }
    }

    /// <summary>
    /// Returns the number of points available in the arrays.  Count will be the greater
    /// of the lengths of the X and Y arrays.
    /// </summary>
    public int Count => _data.Length;

    #endregion

  #region Constructors

    /// <summary>
    /// Constructor to initialize the PointPairList from two arrays of
    /// type double.
    /// </summary>
    public BasicArrayPointList( double[] x, double[] y )
    {
      var len = Math.Max(x.Length, y.Length);
      var dt  = new CompactPt[len];
      if (x.Length <= y.Length)
      {
        for(var i=0; i < x.Length; ++i) dt[i] = new CompactPt(x[i], y[i]);
        for(var i=x.Length; i < y.Length; ++i) dt[i] = new CompactPt(PointPairBase.Missing, y[i]);
      }
      else
      {
        for (var i = 0; i < y.Length; ++i) dt[i] = new CompactPt(x[i], y[i]);
        for (var i = y.Length; i < x.Length; ++i) dt[i] = new CompactPt(x[i], PointPairBase.Missing);
      }

      _data = dt;
    }

    /// <summary>
    /// The Copy Constructor
    /// </summary>
    /// <param name="rhs">The PointPairList from which to copy</param>
    public BasicArrayPointList( BasicArrayPointList rhs )
    {
      _data = new CompactPt[rhs.Count];
      Array.Copy(rhs._data, _data, rhs.Count);
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
    public BasicArrayPointList Clone()
    {
      return new BasicArrayPointList( this );
    }


    #endregion

    public IEnumerator<IPointPair> GetEnumerator()
    {
      return _data.Cast<IPointPair>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
