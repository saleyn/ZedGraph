using System.Collections.Generic;
using NUnit.Framework;

namespace ZedGraph
{
  /// <summary>
  /// Tests for the Scale Class.
  /// </summary>
  [TestFixture]
  public class MinMaxTests
  {
    /// <summary>
    /// Test that values all the way to epsilon get a mag value set.
    /// </summary>
    [Test]
    public void MinMaxTest()
    {
      var list = new List<int>();
      var mm = new ZedGraph.MinMax<int>(list, 4);

      mm.Add(10);
      mm.Add(14);
      mm.Add(11);
      mm.Add(12);
      mm.Add(15);
      mm.Add(13);
      mm.Add(19);

      Assert.That(mm.Min, Is.EqualTo(12));
      Assert.That(mm.Max, Is.EqualTo(19));
    }
  }
}
