namespace ZedGraph
{
  using System.Drawing;

  public interface IValuesToolTip
  {
    /// <summary>
    /// Sets the caption for the tool tip at the specified point.
    /// </summary>
    /// <param name="caption">The caption.</param>
    /// <param name="point">The point.</param>
    void Set(string caption, Point point);

    /// <summary>
    /// Get tooltip's value for given control
    /// </summary>
    string Get();

    /// <summary>
    /// True if tooltip is active
    /// </summary>
    bool Active { get; set; }
  }
}
