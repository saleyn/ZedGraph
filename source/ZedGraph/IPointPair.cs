//============================================================================
// Author: Serge Aleynikov
// Date:   2016-09-06
//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright © 2004  John Champion
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

namespace ZedGraph
{
  /// <summary>
  /// Generic interface of a data point
  /// </summary>
  public interface IPointPair : ICloneable
  {
    double X   { get; set; }
    double Y   { get; set; }
    double Z   { get; set; }
    object Tag { get; set; }

    double LowValue   { get; }
    double HighValue  { get; }
    double ColorValue { get; }
    bool   IsValid    { get; }
    bool   IsInvalid  { get; }
    bool   IsFiltered { get; }

    new IPointPair Clone();
  }
}
