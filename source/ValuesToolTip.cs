namespace ZedGraph
{
  using System;
  using System.Drawing;
  using System.Windows.Forms;

  public class ValuesToolTip : IValuesToolTip
  {
    #region Fields

    /// <summary>
    /// The last caption that was set.
    /// </summary>
    private string lastCaption;

    /// <summary>
    /// The last point a tool tip caption was set at.
    /// </summary>
    private Point lastPoint;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ValuesToolTip"/> class.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <param name="activeCallback">The active callback.</param>
    /// <param name="setToolTipCallback">The set tool tip callback.</param>
    /// <exception cref="System.ArgumentNullException">
    /// control
    /// or
    /// activeCallback
    /// or
    /// setToolTipCallback
    /// </exception>
    public ValuesToolTip(Control control, ToolTip toolTip)
    {
      if (control == null)
        throw new ArgumentNullException(nameof(control));

      Control = control;
      ToolTip = toolTip;
    }

    #endregion

    #region Properties and Indexers

    /// <summary>
    /// Gets the control that this tool tip instance handles.
    /// </summary>
    /// <value>
    /// The control that this tool tip instance handles.
    /// </value>
    internal Control Control { get; }

    /// <summary>
    /// Internal toolTip
    /// </summary>
    internal ToolTip ToolTip { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Enables/Disables the tool tip.
    /// </summary>
    public bool Active
    {
      get { return ToolTip.Active;  }
      set { ToolTip.Active = value; }
    }

    /// <summary>
    /// Sets the specified caption.
    /// </summary>
    /// <param name="caption">The caption.</param>
    public void Set(string caption)
    {
      Set(caption, lastPoint);
    }

    /// <summary>
    /// Sets the caption for the tool tip at the specified point.
    /// </summary>
    /// <param name="caption">The caption.</param>
    /// <param name="point">The point.</param>
    public void Set(string caption, Point point)
    {
      if (point == this.lastPoint && caption == this.lastCaption)
        return;

      ToolTip.SetToolTip(Control, caption);
      this.lastPoint   = point;
      this.lastCaption = caption;
    }

    /// <summary>
    /// Get current ToolTip's caption
    /// </summary>
    public string Get()
    {
      return ToolTip.GetToolTip(Control);
    }

    /// <summary>
    /// Creates a <see cref="ValuesToolTip"/> for the specified control,
    /// using the supplied tooltip to display values.
    /// </summary>
    /// <param name="control">The control.</param>
    /// <param name="toolTip">The tool tip.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentNullException">toolTip</exception>
    public static ValuesToolTip Create(Control control, ToolTip toolTip)
    {
      if (toolTip == null)
        throw new ArgumentNullException(nameof(toolTip));

      return new ValuesToolTip(control, toolTip);
    }

    #endregion
  }
}
