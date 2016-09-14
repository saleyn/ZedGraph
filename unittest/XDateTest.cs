using System;
using NUnit.Framework;

namespace ZedGraph
{
  /// <summary>
  /// The ohlc bar item tests.
  /// </summary>
  [TestFixture]
  public class XDateTest
  {
    [Test]
    public void TestJulian()
    {
      var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
      var j = new XDate(unixEpoch);
      var u = j.UnixEpochMilliseconds;
      Assert.That((long)j.XLDate, Is.EqualTo(25569), "XLDate epoch mismatch");
      Assert.That(u,     Is.EqualTo(0),              "XLDate: invalid epoch");
      j.UnixEpochMilliseconds = 0;
      Assert.That(j.DateTime, Is.EqualTo(unixEpoch), "XLDate: invalid unix date time");

      var now = DateTime.Now; //new DateTime(636094402538180240L);
      var d   = new XDate(now);
      var now0 = d.DateTime;
      Assert.That(now0.Ticks, Is.EqualTo(now.Ticks));
      var ms1 = now.Millisecond;
      var ms2 = d.DateTime.Millisecond;
      var ns1 = now.ToString("yyyy-MM-dd HH:MM:ss.ffffff");
      var ns2 = d.DateTime.ToString("yyyy-MM-dd HH:MM:ss.ffffff");
      var ns3 = d.ToString("yyyy-MM-dd HH:MM:ss.ffffff");
      Assert.That(ms2, Is.EqualTo(ms1));
      Assert.That(ns2, Is.EqualTo(ns1));
      Assert.That(ns3, Is.EqualTo(ns1));

      var now1 = XDate.XLDateToDateTime(d);
      Assert.That(now1, Is.EqualTo(now));

    }
  }
}
