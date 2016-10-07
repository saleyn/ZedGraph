﻿//============================================================================
//DynFilteredPointList class
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

namespace ZedGraph
{
  /// <summary>
  /// An example of an <see cref="IPointList" /> implementation that stores large datasets, and
  /// selectively filters the output data depending on the displayed range.
  /// </summary>
  /// <remarks>
  /// This class will refilter the data points each time <see cref="FilterData" /> is called. 
  /// The data is filtered down to a maximum of <see cref="MaxPts" /> points, 
  /// within the data bounds of a minimum and maximum data range. 
  /// If the property <see cref="IsApplyHighLowLogic"/> is set to true, the data is 
  /// filtered as follows:
  /// The data is divided into segments of equal width in the X axis with each
  /// segment containing 4 points, the start, {minimum and maximum} and end.
  /// Each segment could be a pixel for example.
  /// If <see cref="IsApplyHighLowLogic"/> is set to false, the algorithm simply skips
  /// points to achieve the desired total number of points.
  /// Input arrays are assumed to be monotonically increasing in X, 
  /// but not necessarily equally spaced in X.
  /// </remarks>
  /// <seealso cref="PointPairList" />
  /// <seealso cref="BasicArrayPointList" />
  /// <seealso cref="IPointList" />
  /// <seealso cref="IPointListEdit" />
  ///
  /// <author> ingineer based on John Champion's FilteredPointList class</author>
  /// <version> $Revision: $ $Date: $ </version>
  [Serializable]
  public class DynFilteredPointList : IPointListEdit
  {

    #region Fields

    /// <summary>
    /// Instance of a List of x values
    /// </summary>
    private List<double> _x;
    /// <summary>
    /// Instance of a List of y values
    /// </summary>
    private List<double> _y;

    /// <summary>
    /// Instance of an array of filtered x value indices
    /// </summary>
    private List<int> _filtdInds;

    /// <summary>
    /// The number of points per segment to filter.  In each segment
    /// the first, {min then max or max then min}, and last points are used.
    /// This constant must not be changed without rewriting the 
    /// filtering algorithm (i.e. Filterdata()).
    /// </summary>
    private const int POINTS_PER_SEGMENT = 4;
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
    public PointPair this[int index]
    {
      get
      {
        var xVal = PointPair.Missing;
        var yVal = PointPair.Missing;
        if (index < 0 || index >= this.Count)
          return new PointPair(xVal, yVal, PointPair.Missing, null);

        var fIndex = _filtdInds[index];
        if (fIndex < _x.Count)
        {
          xVal = _x[fIndex];
          yVal = _y[fIndex];
        }
        //xVal = _x[_filtdInds[index]];
        //yVal = _y[_filtdInds[index]];

        return new PointPair(xVal, yVal, PointPair.Missing, null);
      }
      set
      {
        var ind = _x.BinarySearch(value.X);

        if (ind < 0)
          ind = ~ind;

        if (ind > _x.Count) return;

        _x.Insert(ind, value.X);
        _y.Insert(ind, value.Y);
      }
    }

    public int Ordinal(double value)
    {
      int ind = _x.BinarySearch(value);
      if (ind < 0)
        ind = ~ind;

      return ind;
    }

    /// <summary>
    /// Returns the number of points according to the current state of the filter.
    /// </summary>
    public int Count => _filtdInds.Count;

    /// <summary>
    /// Returns the number of points in the underlying data set.
    /// </summary>
    public int UnFilteredCount => _x.Count;

    /// <summary>
    /// Gets the desired number of filtered points to output.  You can set this value by
    /// calling <see cref="FilterData" />.
    /// </summary>
    public int MaxPts { get; private set; } = -1;

    /// <summary>
    /// Gets or sets a value that determines if the High-Low filtering logic 
    /// will be applied.
    /// </summary>
    /// <remarks>
    /// The high-low filtering logic takes 4 points of each segment.
    /// The first point, the highest then lowest Y value or vice versa 
    /// (depending which came first) and then last value of this segment.
    /// Set this value to true to apply this logic, or false to just use whatever 
    /// value lies at the start of each quarter of each segment.
    /// </remarks>
    public bool IsApplyHighLowLogic { get; set; }

    /// <summary>
    /// Gets the index of minimum value for the range of X data that are included in the filtered result.
    /// </summary>
    public int MinBoundIndex { get; private set; } = -1;

    /// <summary>
    /// Gets the index of maximum value for the range of X data that are included in the filtered result.
    /// </summary>
    public int MaxBoundIndex { get; private set; } = -1;

    #endregion

    #region Constructors
    /// <summary>
    /// Constructor to initialize the PointPairList with no PointPairs.
    /// </summary>
    public DynFilteredPointList()
    {
      _x = new List<double>();
      _y = new List<double>();

      _filtdInds = new List<int>();
    }

    /// <summary>
    /// Constructor to initialize the PointPairList from two arrays of
    /// type double.
    /// <remarks>Assumes the _x and _y arrays are monotonically increasing</remarks>
    /// </summary>
    public DynFilteredPointList(double[] x, double[] y)
        : this(new List<double>(x), new List<double>(y))
    { }

    /// <summary>
    /// Constructor to initialize the PointPairList from two Lists of
    /// type double.
    /// <remarks>Assumes the _x and _y arrays are monotonically increasing</remarks>
    /// </summary>
    public DynFilteredPointList(List<double> x, List<double> y)
    {
      _x = x;
      _y = y;
      _filtdInds = new List<int>();
    }

    /// <summary>
    /// The Copy Constructor
    /// </summary>
    /// <remarks>Assumes the _x and _y arrays are monotonically increasing</remarks>
    /// <param name="rhs">The FilteredPointList from which to copy</param>
    public DynFilteredPointList(DynFilteredPointList rhs)
    {
      // the only way we can deep Clone a list is to use GetRange
      _x = new List<double>(rhs._x.GetRange(0, rhs._x.Count).ToArray()); // Clone rhs._x
      _y = new List<double>(rhs._y.GetRange(0, rhs._y.Count).ToArray()); // Clone rhs._y

      _filtdInds = new List<int>(rhs._filtdInds.GetRange(0, rhs._filtdInds.Count)); // Clone rhs._filtdPts

      MinBoundIndex = rhs.MinBoundIndex;
      MaxBoundIndex = rhs.MaxBoundIndex;
      MaxPts = rhs.MaxPts;

      IsApplyHighLowLogic = rhs.IsApplyHighLowLogic;
    }

    /// <summary>
    /// Deep-copy clone routine
    /// </summary>
    /// <returns>A new, independent copy of the FilteredPointList</returns>
    public virtual object Clone()
    {
      return new DynFilteredPointList(this);
    }


    #endregion

    #region Public Methods

    /// <summary>
    /// Set the data bounds to the specified minimum, maximum, and point count.
    /// Use values of min=double.MinValue and max=double.MaxValue to get the full range of data.
    /// Use maxPts=-1 to not limit the number of points.  Call this method anytime the zoom
    /// range is changed or data is added to the list.
    /// </summary>
    /// <remarks>
    /// This function works correctly with non-equally spaced data.
    /// The filtering algorithm divides the whole range into segments containing
    /// 4 points each.  These 4 points are the first point in the segment, the min
    /// then max or max then min (depending which appears first in the underlying dataset),
    /// and the last point.
    /// Hence, in the filtered dataset, the number of points is always a multiple of 4.
    /// </remarks>
    /// <param name="min">The lower bound for the X data of interest</param>
    /// <param name="max">The upper bound for the X data of interest</param>
    /// <param name="maxPts">The maximum number of points allowed to be output by the filter. 
    /// Setting this to 4 times the number of pixels on the X axis produces the
    /// best results.</param>
    /// <param name="filterOnlyNewData">True to filter only new data since the 
    /// last time FilterData was called, plus last filter subrange.</param>
    public void FilterData(double min, double max, int maxPts = -1, bool filterOnlyNewData = false)
    {
      MaxPts = maxPts;

      setMinMaxBoundIndex(min, max);

      if (MaxBoundIndex <= MinBoundIndex)
        return;

      if (!filterOnlyNewData)
        _filtdInds.Clear();

      var elemsToFilter = (MaxBoundIndex - MinBoundIndex) + 1; // +1 for last pt to touch Y2Axis

      // if too few points (or we've been asked not to filter), don't filter
      if (_x.Count > 0 && (elemsToFilter < maxPts || maxPts == -1))
      {
        _filtdInds.Clear();
        for (int i = MinBoundIndex; i < (MinBoundIndex + elemsToFilter); i++)
          _filtdInds.Add(i);

        return;
      }

      // each segment will contain 4 points,
      // the first, {min then max or max then min}, and last. 
      var segmentWidth = ((max - min) * POINTS_PER_SEGMENT) / maxPts;

      var ind = MinBoundIndex;

      // if filterOnlyNewData, delete the last segment and
      // move ind to the start of the (empty) last segment
      if (filterOnlyNewData && _filtdInds.Count >= POINTS_PER_SEGMENT)
      {
        // remove last segment (i.e. 4 points)
        for (var i = 0; i < POINTS_PER_SEGMENT; i++)
          _filtdInds.RemoveAt(_filtdInds.Count - 1);

        ind = _filtdInds[_filtdInds.Count - 1] + 1;
      }

      while (ind <= MaxBoundIndex)
      {
        var nextSegmentStart = _x[ind] + segmentWidth;

        if (IsApplyHighLowLogic)
        {
          _filtdInds.Add(ind); // add the first element of the segment

          int minInd, maxInd;

          // we will check the value of every element of this segment
          // and then pick the min and max and add them to _xFiltd and _yFiltd
          // in the order that they appeared in the segment.
          // NB: the index (ind) is moved to the start of the next segment
          getMinMaxIndices(ref ind, nextSegmentStart, out minInd, out maxInd);

          // add the min and max of this segment in the order they appeared
          addMinMaxIndices(minInd, maxInd);

          // add last element of this segment (not the start of the next)
          _filtdInds.Add(ind - 1);
        }
        else
        {
          // we need to add 4 elements in this segment and we are not applying
          // highLowLogic so we add the first, the 1/4th, the middle and 3/4th
          for (var i = 1; i <= POINTS_PER_SEGMENT; i++)
          {
            // add the next point of this segment
            _filtdInds.Add(ind);

            if (ind >= MaxBoundIndex)
            {
              ind++;
              break;
            }

            // move to the next quarter of this segment
            moveIndexToXVal(ref ind, nextSegmentStart + ((segmentWidth * i) / POINTS_PER_SEGMENT));
          }
        }
      }
    }

    /// <summary>
    /// Appends a point to the end of the list.  The data are passed in as a <see cref="PointPair" />
    /// object.
    /// </summary>
    /// <remarks>A data point that is not monotonically increasing will not be added.</remarks>
    /// <param name="point">The <see cref="PointPair" /> object containing the data to be added.</param>
    public void Add(PointPair point)
    {
      if (_x.Count > 0 && point.X < _x[_x.Count - 1])
        return;

      _x.Add(point.X);
      _y.Add(point.Y);

      MaxPts++;
    }

    /// <summary>
    /// Appends a point to the end of the list.  The data are passed in as two <see cref="Double" />
    /// types.
    /// </summary>
    /// <remarks>A data point that is not monotonically increasing will not be added.</remarks>
    /// <param name="x">The <see cref="Double" /> value containing the X data to be added.</param>
    /// <param name="y">The <see cref="Double" /> value containing the Y data to be added.</param>
    public void Add(double x, double y)
    {
      if (_x.Count > 0 && x < _x[_x.Count - 1])
        return;

      _x.Add(x);
      _y.Add(y);

      MaxPts++;
    }

    /// <summary>
    /// Remove an old item from the list containing the whole underlying data.
    /// </summary>
    /// <remarks><see cref="FilterData"/> should be called after removing a point 
    /// to ensure the filtered subset contains valid indices into the underlying list.
    /// </remarks>
    /// <returns>The removed item or if the list was empty, null.</returns>
    public PointPair Remove()
    {
      if (_x.Count == 0)
        return null;

      int prevLastIndex = _x.Count - 1;

      PointPair pp = new PointPair(_x[prevLastIndex], _y[prevLastIndex]);

      _x.RemoveAt(prevLastIndex);
      _y.RemoveAt(prevLastIndex);

      return pp;
    }

    /// <summary>
    /// Remove the <see cref="PointPair" /> at the specified index in the list 
    /// containing the whole underlying data.
    /// <remarks><see cref="FilterData"/> should be called after removing a point 
    /// to ensure the filtered subset contains valid indices into the underlying list.
    /// </remarks>
    /// </summary>
    /// <param name="index">The ordinal position of the item to be removed.
    /// Throws an <see cref="ArgumentOutOfRangeException" /> if index is less than
    /// zero or greater than or equal to <see cref="Count" />
    /// </param>
    public void RemoveAt(int index)
    {
      if (index >= _x.Count || index >= _x.Count || index < 0)
        throw new ArgumentOutOfRangeException();

      _x.RemoveAt(index);
      _y.RemoveAt(index);
    }

    /// <summary>
    /// Clears all data points from the list.  After calling this method,
    /// <see cref="IPointList.Count" /> will be zero.
    /// </summary>
    public void Clear()
    {
      _x.Clear();
      _y.Clear();

      _filtdInds.Clear();

      MaxPts = -1;
      MinBoundIndex = -1;
      MaxBoundIndex = -1;
    }

    #endregion

    #region Private Methods
    /// <summary>
    /// Set <see cref="MinBoundIndex"/> and <see cref="MaxBoundIndex"/> 
    /// based on the X values passed by the user.
    /// </summary>
    private void setMinMaxBoundIndex(double min, double max)
    {
      // find the index of the start and end of the bounded range
      int first = _x.BinarySearch(min);
      int last  = _x.BinarySearch(max);

      // Make sure the bounded indices are legitimate
      // if BinarySearch() doesn't find the value, it returns the bitwise
      // complement of the index of the 1st element larger than the sought value

      if (first < 0)
        first = first == -1 ? 0 : ~(first + 1);

      if (last < 0)
        last = ~last;

      if (last >= _x.Count && _x.Count > 0)
        last = _x.Count - 1;

      MinBoundIndex = first;
      MaxBoundIndex = last;
    }

    /// <summary>
    /// Gets the indices of the minimum and maximum within the subrange 
    /// between index and nextSegment which is the value of X.  Index is 
    /// updated to the index of nextSegment.
    /// </summary>
    private void getMinMaxIndices(ref int index, double nextSegmentStart,
        out int minIndex, out int maxIndex)
    {
      double y;
      double miny = double.MaxValue, maxy = double.MinValue;

      minIndex = maxIndex = index;

      while (_x[index] < nextSegmentStart)
      {
        y = _y[index];

        if (y > maxy)
        {
          maxy = y;
          maxIndex = index;
        }

        if (y < miny)
        {
          miny = y;
          minIndex = index;
        }

        index++;
        if (index >= _x.Count)
          break;
      }
    }

    /// <summary>
    /// Add min and max indices to the end of <see cref="_filtdInds"/>
    /// </summary>
    private void addMinMaxIndices(int minIndex, int maxIndex)
    {
      if (minIndex < maxIndex)
      {
        _filtdInds.Add(minIndex);
        _filtdInds.Add(maxIndex);
      }
      else
      {
        _filtdInds.Add(maxIndex);
        _filtdInds.Add(minIndex);
      }
    }

    /// <summary>
    /// Search <see cref="_x"/> for xVal and set the index to it.
    /// </summary>
    private void moveIndexToXVal(ref int index, double xVal)
    {
      int ind = _x.BinarySearch(xVal);

      if (ind < 0)
        ind = ~ind;

      if (ind >= _x.Count && _x.Count > 0)
        ind = _x.Count - 1;

      index = ind;
    }
    #endregion
  }
}