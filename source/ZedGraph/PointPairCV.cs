//============================================================================
//PointPairCV Class
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
using System.Collections;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

#if ( !DOTNET1 )  // Is this a .Net 2 compilation?
using System.Collections.Generic;
#endif

namespace ZedGraph
{
  /// <summary>
  /// A simple instance that stores a data point (X, Y, Z).  This differs from a regular
  /// <see cref="PointPair" /> in that it maps the <see cref="ColorValue" /> property
  /// to an independent value.  That is, <see cref="ColorValue" /> and
  /// <see cref="PointPair.Z" /> are not related (as they are in the
  /// <see cref="PointPair" />).
  /// </summary>
  public class PointPairCV : PointPair
  {
  #region Constructors

    /// <summary>
    /// Creates a point pair with the specified X, Y, and base value.
    /// </summary>
    /// <param name="x">This pair's x coordinate.</param>
    /// <param name="y">This pair's y coordinate.</param>
    /// <param name="z">This pair's z or lower dependent coordinate.</param>
    public PointPairCV( double x, double y, double z )
      : base( x, y, z, null )
    {}

  #endregion

  }
}
