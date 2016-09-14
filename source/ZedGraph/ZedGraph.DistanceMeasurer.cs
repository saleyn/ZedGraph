using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace ZedGraph
{
  public class DistanceMeasurer
  {
    private ZedGraphControl m_Control;
    private Boolean         m_MeasureStateActive = false;
    private Boolean         m_MeasurerActivated = false;
    private Color           m_LineColor, m_FontColor;
    private float           m_FontSize;
    private LineItem        m_MeasurerBeam = null;
    private List<LineItem>  m_MeasurerBeamList = null;
    private List<TextObj>   m_MeasurerBeamTextList = null;

    public CoordType        Coord { get; set; }

    public delegate void ContextMenuBuilderEventHandler(
        ZedGraphControl sender,
        ContextMenuStrip menuStrip,
        Point mousePt,
        ZedGraphControl.ContextMenuObjectState objState);
    public delegate bool ZedMouseEventHandler(ZedGraphControl control, MouseEventArgs e);

    #region Constructors

    // constructor - add a subscription to the event
    public DistanceMeasurer(ZedGraphControl control, Color lineColor, Color fontColor,
                            float fontSize, CoordType coord)
    {
      m_Control = control;
      m_LineColor = lineColor;
      m_FontColor = fontColor;
      m_FontSize = fontSize;
      Coord = coord;
      m_MeasurerBeamList = new List<LineItem>();
      m_MeasurerBeamTextList = new List<TextObj>();

      m_Control.ContextMenuBuilder += MyContextMenuBuilder;
    }

    ~DistanceMeasurer()
    {
      m_Control.ContextMenuBuilder -= MyContextMenuBuilder;
    }

    private void MyContextMenuBuilder(ZedGraphControl control, ContextMenuStrip menuStrip,
                                      Point mousePt,
                                      ZedGraphControl.ContextMenuObjectState objState)
    {
      // remove any inactive measurer beam (which was remove by the graphpane)
      // this implementation ASSUMES that this function is called PRIOR to activating 
      // any of the following menuStrip functions
      RemoveInactiveMeasurerBeams();

      // create a seperator
      var seperator1 = new ToolStripSeparator();
      seperator1.Name = "measurer_menu_seperator_tag";
      seperator1.Tag = "measurer_menu_seperator_tag";
      menuStrip.Items.Add(seperator1);

      // create the "Measure Distace" menu item
      var item1 = new ToolStripMenuItem(); // Create a new item
      item1.Name = "measurer_distance_tag";
      item1.Tag = "measurer_distance_tag";
      item1.Text = "Measure Distance";
      item1.Checked = m_MeasureStateActive;
      item1.Click += new EventHandler(MenuStrip_Measurer);
        // Add a handler that will respond when that menu item is selected
      menuStrip.Items.Add(item1); // Add the menu item to the menu

      // create the "Clear Last Measure" menu item
      var item2 = new ToolStripMenuItem(); // Create a new item
      item2.Name = "measurer_clear_last_tag";
      item2.Tag = "measurer_clear_last_tag";
      item2.Text = "Clear Last Measure";
      item2.Enabled = IsActiveMeasurerBeams();
      item2.Click += new EventHandler(MenuStrip_ClearLast);
        // Add a handler that will respond when that menu item is selected
      menuStrip.Items.Add(item2); // Add the menu item to the menu

      // create the "Clear All Measurements" menu item
      var item3 = new ToolStripMenuItem(); // Create a new item
      item3.Name = "measurer_clear_all_tag";
      item3.Tag = "measurer_clear_all_tag";
      item3.Text = "Clear All Measurements";
      item3.Enabled = IsActiveMeasurerBeams();
      item3.Click += new EventHandler(MenuStrip_ClearAll);
        // Add a handler that will respond when that menu item is selected
      menuStrip.Items.Add(item3); // Add the menu item to the menu
    }

    #endregion

    #region Mouse Events

    protected void MenuStrip_Measurer(object sender, EventArgs e)
    {
      m_MeasureStateActive = !m_MeasureStateActive;
      if (m_MeasureStateActive)
      {
        this.m_Control.MouseDownEvent += ZG_MouseDownEvent;
        this.m_Control.MouseUpEvent += ZG_MouseUpEvent;
        this.m_Control.MouseMoveEvent += ZG_MouseMoveEvent;
      }
      else
      {
        this.m_Control.MouseDownEvent -= ZG_MouseDownEvent;
        this.m_Control.MouseUpEvent -= ZG_MouseUpEvent;
        this.m_Control.MouseMoveEvent -= ZG_MouseMoveEvent;
      }
    }

    protected void MenuStrip_ClearLast(object sender, EventArgs e)
    {
      while (m_MeasurerBeamList.Count > 0)
      {
        var temp = m_MeasurerBeamList[m_MeasurerBeamList.Count - 1];
        m_MeasurerBeamList.Remove(temp);
        var tempText = m_MeasurerBeamTextList[m_MeasurerBeamTextList.Count - 1];
        m_MeasurerBeamTextList.Remove(tempText);
        if (m_Control.GraphPane.GraphObjList.IndexOf(tempText) > -1)
          m_Control.GraphPane.GraphObjList.Remove(tempText);

        if (m_Control.GraphPane.CurveList.IndexOf(temp) <= -1) continue;

        m_Control.GraphPane.CurveList.Remove(temp);
        break;
      }
      m_Control.Refresh();
    }

    protected void MenuStrip_ClearAll(object sender, EventArgs e)
    {
      while (m_MeasurerBeamList.Count > 0)
      {
        LineItem temp = m_MeasurerBeamList[m_MeasurerBeamList.Count - 1];
        m_MeasurerBeamList.Remove(temp);
        TextObj tempText = m_MeasurerBeamTextList[m_MeasurerBeamTextList.Count - 1];
        m_MeasurerBeamTextList.Remove(tempText);
        if (m_Control.GraphPane.GraphObjList.IndexOf(tempText) > -1)
        {
          m_Control.GraphPane.GraphObjList.Remove(tempText);
        }
        if (m_Control.GraphPane.CurveList.IndexOf(temp) > -1)
        {
          m_Control.GraphPane.CurveList.Remove(temp);
        }
      }
      m_Control.Refresh();
    }

    private bool IsActiveMeasurerBeams()
    {
      // will return true if there is at least one valid measurer beam
      return (m_MeasurerBeamList.Count > 0);
    }

    private void RemoveInactiveMeasurerBeams()
    {
      if (m_MeasurerBeamList.Count == 0)
        return;

      // will remove any "beam" from our list if it is not found inside the GraphPane
      for (int i = m_MeasurerBeamList.Count - 1; i >= 0; i--)
        if (m_Control.GraphPane.CurveList.IndexOf(m_MeasurerBeamList[i]) == -1)
          m_MeasurerBeamList.RemoveAt(i);
    }

    private bool ZG_MouseDownEvent(ZedGraphControl control, MouseEventArgs e)
    {
      if (!e.Button.Equals(MouseButtons.Left)) return false;

      // place anchor point and create line curve object
      var point = control.GraphPane.ReverseTransformCoord(e.X, e.Y, Coord);

      var scale = control.GraphPane.XAxis.Scale;
      var value = scale.Value(control.GraphPane, (int)Math.Round(point.X), point.X);
      double[] xVec = {value, value};
      double[] yVec = {point.Y, point.Y};
      m_MeasurerBeam = m_Control.GraphPane.AddCurve("", xVec, yVec, m_LineColor);
      m_MeasurerBeam.Symbol.Size = 0;
      m_MeasurerBeam.Symbol.Type = SymbolType.None;
      m_MeasurerBeam.Tag = "ZedGraphMeasurerLineItem";
      m_MeasurerBeam.IsY2Axis = Coord == CoordType.AxisXY2Scale;

      // indicate to the mouse movement event that we are currently placing the measurer-beam
      m_MeasurerActivated = true;

      // tell the ZedGraphControl not to do anything else with this event
      return true;
    }

    private bool ZG_MouseMoveEvent(ZedGraphControl control, MouseEventArgs e)
    {
      if (!m_MeasurerActivated) return false; // tell the ZedGraphControl to handle it

      // update the target point
      var point = control.GraphPane.ReverseTransformCoord(e.X, e.Y, Coord);

      var scale = control.GraphPane.XAxis.Scale;
      var value = scale.Value(control.GraphPane, (int)Math.Round(point.X), point.X);

      m_MeasurerBeam.Points[1].X = value;
      m_MeasurerBeam.Points[1].Y = point.Y;
      //      control.GraphPane.ReverseTransform(new Point(e.X, e.Y),
      //                                         out m_MeasurerBeam.Points[1].X, out m_MeasurerBeam.Points[1].Y);
      // force a redraw
      control.Refresh();

      // tell the ZedGraphControl not to do anything else with this event
      return true;
    }

    private bool ZG_MouseUpEvent(ZedGraphControl control, MouseEventArgs e)
    {
      if (!e.Button.Equals(MouseButtons.Left)) return false;

      // indicate to the mouse movement event that we finished with the measurer-beam
      m_MeasurerActivated = false;

      // calculate the distance
      var ds = 1.0;
      if ((m_Control.GraphPane.XAxis.Type == AxisType.Date) ||
          (m_Control.GraphPane.XAxis.Type == AxisType.DateAsOrdinal))
        switch (m_Control.GraphPane.XAxis.Scale.MajorUnit)
        {
          case DateUnit.Year:
            ds = 1.0/365.0;
            break;
          case DateUnit.Day:
            ds = 1.0;
            break;
          case DateUnit.Hour:
            ds = 24.0;
            break;
          case DateUnit.Minute:
            ds = 24.0*60.0;
            break;
          case DateUnit.Second:
            ds = 24.0*60.0*60.0;
            break;
          case DateUnit.Millisecond:
            ds = 24.0*60.0*60.0*1000.0;
            break;
        }
      double dx = (m_MeasurerBeam.Points[1].X - m_MeasurerBeam.Points[0].X)*ds;
      double dy = m_MeasurerBeam.Points[1].Y - m_MeasurerBeam.Points[0].Y;
      double distance = Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

      // if this is only a point and not an actual measurer beam - remove it
      // otherwise, add it to our "watch-list"
      if (distance == 0.0)
        m_Control.GraphPane.CurveList.Remove(m_MeasurerBeam);
      else
      {
        // calculate the position for the text
        double cx = (m_MeasurerBeam.Points[1].X + m_MeasurerBeam.Points[0].X)/2;
        double cy = (m_MeasurerBeam.Points[1].Y + m_MeasurerBeam.Points[0].Y)/2;

        // calculate the angle for the text (must be done in screen coordinates)
        PointF p1 = control.GraphPane.GeneralTransform(m_MeasurerBeam.Points[0].X,
                                                       m_MeasurerBeam.Points[0].Y, Coord);
        PointF p2 = control.GraphPane.GeneralTransform(m_MeasurerBeam.Points[1].X,
                                                       m_MeasurerBeam.Points[1].Y, Coord);

        // add text to describe the distance (if one dimension is much bigger than the other 
        // AND axes type are different - create a text only for the bigger dimension)
        var text = new TextObj($"{distance:N3}", cx, cy,
                               Coord, AlignH.Center, AlignV.Bottom);
        if (!m_Control.GraphPane.YAxis.Type.Equals(m_Control.GraphPane.XAxis.Type))
        {
          double dpx = Math.Abs(p2.X - p1.X);
          double dpy = Math.Abs(p2.Y - p1.Y);
          if (dpx > 20.0*dpy)
            text.Text =
              $"{Math.Abs(dx):N3} [ {m_Control.GraphPane.XAxis.Scale.MajorUnit}s ]";
          else if (dpy > 20.0*dpx)
            text.Text = $"{Math.Abs(dy):N3} [ {m_Control.GraphPane.YAxis.Type} ]";
        }
        text.FontSpec.Angle = 0.0f;
          // Convert.ToSingle(-Math.Atan((p2.Y - p1.Y) / (p2.X - p1.X)) * 180.0 / Math.PI);
        text.FontSpec.FontColor = m_FontColor;
        text.FontSpec.Size = m_FontSize;
        text.FontSpec.Border.IsVisible = false;
        m_Control.GraphPane.GraphObjList.Add(text);

        // add tooltip to describe the distance (and add a point in the middle)
        var dXdYstr =
          $"Distance: {distance:N3}\ndy: {Math.Abs(dy):N3} [{m_Control.GraphPane.YAxis.Type}]\n" +
          $"dx: {Math.Abs(dx):N3} [{m_Control.GraphPane.XAxis.Scale.MajorUnit}]";

        m_MeasurerBeam.AddPoint(cx, cy);
        for (int i = 0; i < 3; i++)
          m_MeasurerBeam.Points[i].Tag = dXdYstr;

        // add curve and its text to the "watch-list" 
        m_MeasurerBeamTextList.Add(text);
        m_MeasurerBeamList.Add(m_MeasurerBeam);
      }

      // force a redraw
      control.Refresh();

      // tell the ZedGraphControl not to do anything else with this event
      return true;
    }

    #endregion
  }
}
