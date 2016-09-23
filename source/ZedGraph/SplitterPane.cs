//============================================================================
// Author: Serge Aleynikov
// Date:   2016-09-16
// Copyright © 2016 Serge Aleynikov
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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ZedGraph
{
  public class SplitterPane : PaneBase
  {
    #region Constructors

    private void ctor(bool vertical)
    {
      base.IsFontsScaled    = false;
      base.IsPenWidthScaled = false;
      base.MouseWheelAction = MouseWheelActions.None;
      base.Title.IsVisible  = false;
      base.Legend.IsVisible = false;
      base.TitleGap         = 0f;
      Border.IsVisible      = false;
      Fill.Color            = Color.Transparent;
      Vertical              = vertical;
    }

    public SplitterPane(SplitterPane rhs) : base(rhs) { ctor(rhs.Vertical); }

    public SplitterPane(bool vertical)
      : base("", new RectangleF(MasterPane.Default.PaneSplitterSize,
                                MasterPane.Default.PaneSplitterSize,
                                MasterPane.Default.PaneSplitterSize,
                                MasterPane.Default.PaneSplitterSize))
    {
      ctor(vertical);
    }

    #endregion

    #region Serialization

    private const int Schema = 1;

    /// <summary>
    /// Constructor for deserializing objects
    /// </summary>
    protected SplitterPane(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      var sch  = info.GetInt32("schema");
      var vert = info.GetBoolean("vertical");

      ctor(vert);
    }

    /// <summary>
    /// Populates a <see cref="SerializationInfo"/> instance with the data needed to serialize the target object
    /// </summary>
    /// <param name="info">A <see cref="SerializationInfo"/> instance that defines the serialized data</param>
    /// <param name="context">A <see cref="StreamingContext"/> instance that contains the serialized data</param>
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);

      info.AddValue("schema",   Schema);
      info.AddValue("vertical", m_Vertical);
      ctor(m_Vertical);
    }

    #endregion

    #region Fields

    private bool m_Vertical;

    #endregion

    #region Properties

    /// <summary>
    /// Splitter direction
    /// </summary>
    public bool Vertical
    {
      get { return m_Vertical; }
      set { m_Vertical = value; Cursor = value ? Cursors.VSplit : Cursors.HSplit; }
    }

    /// <summary>
    /// Index of the splitter pane updated by master pane during layout
    /// </summary>
    public int PaneIndex { get; internal set; }

    /// <summary>
    /// Mouse cursor to display when mouse hovers over this pane
    /// </summary>
    public Cursor Cursor { get; private set; }

    #endregion

    #region Hidden Base Properties

    internal new Legend            Legend           => base.Legend;
    internal new Label             Title            => base.Title;
    internal new GraphObjList      GraphObjList     => base.GraphObjList;
    internal new float             BaseDimension    => base.BaseDimension;
    internal new float             TitleGap         => base.TitleGap;
    internal new bool              IsFontsScaled    => base.IsFontsScaled;
    internal new bool              IsPenWidthScaled => base.IsPenWidthScaled;
    internal new MouseWheelActions MouseWheelAction => base.MouseWheelAction;

    #endregion

    #region Internal Methods

    private HighLight m_HighLight;
    private Point     m_Start, m_Last;
    private int       m_Min,   m_Max;

    internal void OnMouseDown(ZedGraphControl sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left || e.Clicks != 1) return;

      m_HighLight?.Hide();

      var loc = Location;
      loc.Offset(sender.PointToScreen(sender.ClientRectangle.Location));

      m_HighLight = new HighLight(loc, Size);
      m_Start     = m_Last = e.Location;

      if (PaneIndex > 0 && sender.MasterPane != null && PaneIndex < sender.MasterPane.PaneList.Count-1)
      {
        var before = sender.MasterPane.PaneList[PaneIndex-1];
        var after  = sender.MasterPane.PaneList[PaneIndex+1];

        var min = before.Location; 
        var max = new Point((int)after.Rect.Right, (int)after.Rect.Bottom);

        m_Min = Vertical ? min.X : min.Y;
        m_Max = Vertical ? max.X : max.Y;
      }
    }

    internal void OnMouseUp(ZedGraphControl sender, MouseEventArgs e)
    {
      m_HighLight?.Close();
      m_HighLight = null;

      var range = m_Max - m_Min;

      if (e.Location == m_Start || Vertical ? (e.Location.X <= m_Min || e.Location.X >= m_Max)
                                            : (e.Location.Y <= m_Min || e.Location.Y >= m_Max)
                                || range == 0)
        return;

      var ratio = (float)((Vertical ? e.Location.X : e.Location.Y) - m_Min) / range;

      sender.MasterPane.SetProportion(this, ratio);
      sender.Invalidate();
      sender.Refresh();
    }

    internal void OnMouseMove(ZedGraphControl sender, MouseEventArgs e)
    {
      if (m_HighLight == null)
        return;
      if (e.Location == m_Last || Vertical ? (e.Location.X <= m_Min || e.Location.X >= m_Max)
                                            : (e.Location.Y <= m_Min || e.Location.Y >= m_Max))
        return;

      var offset = Vertical
                 ? new Point(e.X - m_Last.X, 0)
                 : new Point(0, e.Y - m_Last.Y);
      var loc = m_HighLight.Location;
      loc.Offset(offset);
      m_HighLight.Location = loc;

      m_Last = e.Location;
    }

    #endregion

    #region Public Local Classes

    internal class HighLight : Form
    {
      private const int HWND_TOPMOST = -1;
      private const int SW_SHOWNOACTIVATE = 4;
      private const uint SWP_NOACTIVATE = 0x0010;

      public HighLight(Point location, Size sz)
      {
        FormBorderStyle = FormBorderStyle.None;
        BackColor       = Color.Black;
        Opacity         = 0;
        ShowIcon        = false;
        ShowInTaskbar   = false;
        StartPosition   = FormStartPosition.Manual;

        ShowInactiveTopmost();
        Location = location;
        Size     = sz;
      }

      private void ShowInactiveTopmost()
      {
        ShowWindow(Handle, SW_SHOWNOACTIVATE);
        SetWindowPos(Handle.ToInt32(), HWND_TOPMOST,
                     Left, Top, Width, Height,
                     SWP_NOACTIVATE);
        Opacity = 0.3;
      }

      protected override void OnDeactivate(EventArgs e)
      {
        base.OnDeactivate(e);
        Hide();
      }

      [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
      private static extern bool SetWindowPos(int hWnd, int hWndInsertAfter,
                                              int X, int Y, int cx, int cy, uint uFlags);
      [DllImport("user32.dll")]
      private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }

    #endregion
  }
}