using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ZedGraph.Demo
{
  /// <summary>
  /// This chart control displays cross-hair
  /// </summary>
  public class ZedGraphControlExt : ZedGraphControl
  {
    private Point     m_MousePosition;
    private Rectangle m_CrosshairXLine;
    private Rectangle m_CrosshairYLine;
    private bool      m_MouseInBounds;


    // The axis that is currently hovered by the mouse
    private object    m_HoveredYAxis;
    // The graphpane that contains the axis
    private GraphPane m_FoundPane;
    // The scale of the axis before it is panned
    private double    m_MovedYAxisMin;
    private double    m_MovedYAxisMax;
    // The Y on the axis when the panning operation is starting
    private float     m_MovedYAxisStartY;

    public ZedGraphControlExt()
    {
      ZoomModifierKeys = Keys.Shift;
      PanModifierKeys  = Keys.Control;
    }

    /// <summary>
    /// When true - the chart shows crosshair
    /// </summary>
    public bool CrossHair { get; set; } = true;

    public Pen  CrossHairPen { get; set; } = new Pen(Color.Silver) {DashStyle = DashStyle.Dash};

    protected override void OnMouseEnter(EventArgs e)
    {
      m_MouseInBounds = true;
      base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      m_MouseInBounds = false;

      if (CrossHair)
      {
        // Clear the previous lines since we just left the control
        this.Invalidate(m_CrosshairXLine);
        this.Invalidate(m_CrosshairYLine);
      }

      base.OnMouseLeave(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);

      if (CrossHair)
      {
        // Invalidate both lines
        this.Invalidate(m_CrosshairXLine);
        this.Invalidate(m_CrosshairYLine);

        // Save our new mouse position
        m_MousePosition = e.Location;

        // Calculate the portion of the window in which we painted our last crosshair
        // (with a little extra space to avoid artifacts during zooming).
        var sz = this.ClientSize;
        m_CrosshairXLine = new Rectangle(0, m_MousePosition.Y-1, sz.Width, m_MousePosition.Y);
        m_CrosshairYLine = new Rectangle(m_MousePosition.X-1, 0, m_MousePosition.X, sz.Height);

        // Invalidate both lines
        this.Invalidate(m_CrosshairXLine);
        this.Invalidate(m_CrosshairYLine);
        //this.Update();
      }

      var pt = e.Location;

      if (e.Button == MouseButtons.Left &&
         ((Control.ModifierKeys == PanModifierKeys ||
          (PanModifierKeys2 != Keys.None && Control.ModifierKeys == PanModifierKeys2))))
      {
        if (m_HoveredYAxis == null) return;
        var scale = (m_HoveredYAxis is YAxis) ? ((YAxis)m_HoveredYAxis).Scale : ((Y2Axis)m_HoveredYAxis).Scale;
        var yOffset = scale.ReverseTransform(pt.Y) - scale.ReverseTransform(m_MovedYAxisStartY);
        scale.Min = m_MovedYAxisMin - yOffset;
        scale.Max = m_MovedYAxisMax - yOffset;

        Invalidate();
      }
      else if (Control.ModifierKeys == ZoomModifierKeys  ||
               (ZoomModifierKeys2 != Keys.None && Control.ModifierKeys == ZoomModifierKeys2) ||
               Control.ModifierKeys == PanModifierKeys   ||
               (PanModifierKeys2  != Keys.None && Control.ModifierKeys == PanModifierKeys2))
      {
        var foundObject = findZedGraphObject(new Point(e.X, e.Y));
        m_HoveredYAxis = foundObject as YAxis ?? (object)(foundObject as Y2Axis);

        if (m_HoveredYAxis != null)
          Cursor = Cursors.SizeNS;
        else if (IsShowPointValues)
          Cursor = Cursors.Cross;
        else
          Cursor = Cursors.Default;
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);  // allow the control to paint itself first...

      if (!CrossHair || !m_MouseInBounds) return;

      var g = e.Graphics;
      var sz = this.ClientSize;
      g.DrawLine(CrossHairPen, 0, m_MousePosition.Y, sz.Width, m_MousePosition.Y);
      g.DrawLine(CrossHairPen, m_MousePosition.X, 0, m_MousePosition.X, sz.Height);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
      base.OnMouseWheel(e);

      if (m_HoveredYAxis == null) return;

      if (Control.ModifierKeys != ZoomModifierKeys &&
         (ZoomModifierKeys2 == Keys.None || Control.ModifierKeys != ZoomModifierKeys2))
        return;

      var direction = e.Delta < 1 ? -.05f : .05f;
      var scale = (m_HoveredYAxis is YAxis) ? ((YAxis)m_HoveredYAxis).Scale : ((Y2Axis)m_HoveredYAxis).Scale;
      var increment = direction * (scale.Max - scale.Min);
      var newMin = scale.Min + increment;
      var newMax = scale.Max - increment;

      scale.Min = newMin;
      scale.Max = newMax;

      m_FoundPane.AxisChange();

      this.Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      base.OnMouseUp(e);
      m_HoveredYAxis = null;
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);

      if (e.Button != MouseButtons.Left) return;
      if (m_HoveredYAxis == null) return;

      m_MovedYAxisStartY = e.Location.Y;
      var scale = (m_HoveredYAxis is YAxis) ? ((YAxis)m_HoveredYAxis).Scale : ((Y2Axis)m_HoveredYAxis).Scale;
      m_MovedYAxisMin = scale.Min;
      m_MovedYAxisMax = scale.Max;
    }

    private object findZedGraphObject(Point pt)
    {
      m_FoundPane = MasterPane.FindPane(pt);
      if (m_FoundPane == null) return null;
      object foundObject;
      int forget;

      using (var g = CreateGraphics())
        if (m_FoundPane.FindNearestObject(pt, g, out foundObject, out forget))
          return foundObject;

      return null;
    }
  }
}