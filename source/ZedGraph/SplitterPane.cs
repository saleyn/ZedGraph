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
      base.Title._isVisible = false;
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
    private Point     m_Start;
    internal void OnMouseDown(ZedGraphControl sender, MouseEventArgs e)
    {
      if ((e.Button == MouseButtons.Left) && (e.Clicks == 1))
      {
        m_HighLight?.Hide();
        var client = sender.MasterPane.ClientRect;
        var loc = new Point((int)(client.Location.X + Rect.X), (int)(client.Location.Y + Rect.Y));
        var sz  = Size;
        m_Start = loc;
        m_HighLight = new HighLight() { Location = loc, Size = sz };
        m_HighLight.ShowInactiveTopmost();
        m_HighLight.Height = sz.Height;
        m_HighLight.Width  = sz.Width;
        return; //SplitBegin(e.X, e.Y);
      }
    }

    internal void OnMouseUp(ZedGraphControl sender, MouseEventArgs e)
    {
      m_HighLight?.Close();
      m_HighLight = null;
      /*
      if (splitTarget == null) return;
      CalcSplitLine(GetSplitSize(e.X, e.Y), 0);
      SplitEnd(true);
      */
    }

    internal void OnMouseMove(ZedGraphControl sender, MouseEventArgs e)
    {
      if (m_HighLight == null)
        return;

      var client = sender.MasterPane.ClientRect;
      var x = Vertical ? (int)(e.X + client.Location.X) : m_Start.X;
      var y = Vertical ? m_Start.Y : (int)(e.Y + client.Location.Y);

      m_HighLight.Location = new Point(x, y);

      /*
      if (splitTarget != null)
      {
        int x = e.X + base.Left;
        int y = e.Y + base.Top;
        var rectangle = CalcSplitLine(GetSplitSize(e.X, e.Y), 0);
        int splitX = rectangle.X;
        int splitY = rectangle.Y;
        OnSplitterMoving(new SplitterEventArgs(x, y, splitX, splitY));
      }
      */
    }

    #endregion

    #region Private Methods
    /*
    internal void SplitBegin(int x, int y)
    {
      var data = CalcSplitBounds();
      if (minSize >= maxSize) return;
      anchor = new Point(x,y);
      splitSize = GetSplitSize(anchor);
      DrawSplitBar(1);
    }

    private void SplitEnd(bool accept)
    {
      DrawSplitBar(3);
      Capture = false;
      if (splitterMessageFilter != null)
      {
        Application.RemoveMessageFilter(splitterMessageFilter);
        splitterMessageFilter = null;
      }
      if (accept)
        ApplySplitPosition();
      else if (splitSize != initTargetSize)
        SplitPosition = initTargetSize;
      anchor = Point.Empty;
    }

    private void SplitMove(Point point)
    {
      var x = point.X;
      var y = point.Y;
      int size =  GetSplitSize((x - base.Left) + anchor.X, (y - base.Top) + anchor.Y);
      if (size == splitSize) return;
      splitSize = size;
      DrawSplitBar(2);
    }
    */

    #region Public Local Classes

    internal class HighLight : Form
    {
      private const int HWND_TOPMOST = -1;
      private const int SW_SHOWNOACTIVATE = 4;
      private const uint SWP_NOACTIVATE = 0x0010;

      public HighLight()
      {
        FormBorderStyle = FormBorderStyle.None;
        BackColor = Color.Black;
        Opacity = 0;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
      }

      public void ShowInactiveTopmost()
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


    #endregion

  }
}