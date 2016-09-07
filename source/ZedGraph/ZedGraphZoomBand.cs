//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright ?2004  John Champion
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
using System.Windows.Forms;

namespace ZedGraph
{
  /// <summary>
  /// Provides a replacement zoom band. This zoom band is translucent and
  /// allows different colors to be defined for the border and for the background.
  /// Despite the translucence, this zoom band provides good performance due
  /// to the fact that it is a separate window and, as such, will be rendered by
  /// the compositer, which most new operating systems offload to the GPU.
  /// </summary>
  public class ZedGraphZoomBand : Form
  {
    private Color _borderColor;
    private Brush _borderBrush;
    private Pen   _borderPen;

    /// <summary>
    /// Defines the color of the border of this zoom band.
    /// </summary>
    public Color BorderColor
    {
      get { return _borderColor; }
      set
      {
        if (_borderBrush != null)
        {
          _borderBrush.Dispose();
          _borderBrush = null;
        }
        _borderBrush = new SolidBrush(value);
        if (_borderPen != null)
        {
          _borderPen.Dispose();
          _borderPen = null;
        }
        _borderPen = new Pen(_borderBrush);
        _borderColor = value;
      }
    }

    /// <summary>
    /// Creates a new frmZoomBand to be used as a zoom band (similar in function
    /// to the standard Windows 'rubberband', except that this one is transparent
    /// and rendered by the compositer (because it is its own form,) rather than
    /// being painted by the user or by XORing pixel color values. This allows high
    /// performance on most systems, especially Windows XP and higher, where the
    /// compositer runs on the GPU.
    /// </summary>
    public ZedGraphZoomBand(Control control, Point loc)
    {
      SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      loc.Offset(control.PointToScreen(control.ClientRectangle.Location));
      _borderBrush = null;
      _borderPen   = null;
      Location     = loc;
      Size         = new Size(0, 0);
      BorderColor  = SystemColors.ActiveBorder;
      InitializeComponent();
    }

    /// <summary>
    /// Repaints this frmZoomBand as soon as it is resized (to prevent corruption
    /// of the background.)
    /// </summary>
    /// <param name="e">EventArgs for the Resize event. Will be passed to any handlers
    /// registered for the Resize event.</param>
    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      this.Invalidate();
      this.Update();
    }

    /// <summary>
    /// Disposes unmanaged resources when this form is closed (namely, the Brush
    /// and Pen used to render the background.)
    /// </summary>
    /// <param name="e">EventArgs for the Closed event. Will be passed to any handlers
    /// registered for the Close event.</param>
    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
      _borderBrush?.Dispose();
      _borderPen?.Dispose();
    }

    /// <summary>
    /// Paints the background of this frmZoomBand (which is all of the painting,
    /// as frmZoomBand has no foreground elements.) Fills the area of this form
    /// with the background color and then draws the border around the edges.
    /// </summary>
    /// <param name="e">PaintEventArgs for the PaintBackground event. Used to
    /// determine the size of the form's drawable area and the graphics context
    /// to which it should be drawn.</param>
    protected override void OnPaintBackground(PaintEventArgs e)
    {
      //base.OnPaintBackground(e);
      var fillArea = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
      e.Graphics.Clear(this.BackColor);
      //e.Graphics.Clear(Color.White);
      //e.Graphics.FillRectangle(SystemBrushes.MenuHighlight, fillArea);
      e.Graphics.DrawRectangle(_borderPen, new Rectangle(0, 0, fillArea.Width, fillArea.Height));
    }

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer _components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
        _components?.Dispose();
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.SuspendLayout();
      // 
      // ZedGraphZoomBand
      // 
      this.AutoScaleDimensions = new SizeF(6F, 13F);
      this.AutoScaleMode       = AutoScaleMode.Font;
      this.BackColor           = SystemColors.MenuHighlight;
      this.ControlBox          = false;
      this.Enabled             = false;
      this.FormBorderStyle     = FormBorderStyle.None;
      this.Name                = "ZedGraphZoomBand";
      this.Opacity             = 0.4;
      this.ShowIcon            = false;
      this.ShowInTaskbar       = false;
      this.StartPosition       = FormStartPosition.Manual;
      this.Text                = "ZedGraphZoomBand";
      this.TopMost             = true;
      this.ResumeLayout(false);
    }

    #endregion
  }
}