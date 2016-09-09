namespace ZedGraph
{
  using System.Drawing;

  partial class ZedGraphControl
  {
    #region Fields

    /// <summary>
    /// The tool tip for displaying the cursor and point values.
    /// </summary>
    private readonly IValuesToolTip _tooltip;

    #endregion

    #region Methods

    /// <summary>
    /// Enables the tool tip.
    /// </summary>
    private void EnableToolTip()
    {
      this._tooltip.Active = true;
    }

    /// <summary>
    /// Sets the tool tip.
    /// </summary>
    /// <param name="caption">The caption.</param>
    /// <param name="point">The point.</param>
    private void SetToolTip(string caption, Point point)
    {
      if (string.IsNullOrEmpty(caption))
      {
        DisableToolTip();
        return;
      }

      _tooltip.Set(caption, point);

      if (!_tooltip.Active)
        EnableToolTip();
    }

    /// <summary>
    /// Disables the tool tip.
    /// </summary>
    private void DisableToolTip()
    {
      this._tooltip.Active = false;
    }

    #endregion
  }
}
