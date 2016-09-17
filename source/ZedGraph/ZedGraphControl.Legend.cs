//============================================================================
// ZedGraphControl methods for handling legend clicks
//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright (c) 2004 John Champion
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ZedGraph
{
  ///--------------------------------------------------------------------------
  /// <summary>
  /// ZedGraphControl's methods for controling visibility of curves
  /// when clicking on a legend.
  /// </summary>
  ///--------------------------------------------------------------------------
  partial class ZedGraphControl
  {
    #region Events

    /// <summary>
    /// A delegate that allows notification of mouse events on Graph objects.
    /// </summary>
    /// <param name="sender">The source <see cref="GraphPane"/> object</param>
    /// <param name="e">A <see cref="MouseEventArgs"/> corresponding to this
    /// event</param>
    /// <seealso cref="MouseDownEvent"/>
    /// <param name="curveLabel">The curve label.</param>
    /// <returns>
    /// Return true if you have handled the mouse event entirely, and you do
    /// not want the <see cref="ZedGraphControl"/> to do any further action
    /// (e.g., starting a zoom operation). Return false if ZedGraph should go
    /// ahead and process the mouse event.
    /// </returns>
    public delegate bool ExtendedZedMouseEventHandler(
      GraphPane sender, MouseEventArgs e, string curveLabel);

    ///------------------------------------------------------------------------
		/// <summary>Delegate for clients who wish to perform manual resetting of
		/// the pane scale.</summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
    ///------------------------------------------------------------------------
		public delegate void ExtendedZedResetScaleEventHandler(
      GraphPane sender, ExtendedZedResetScaleEventArgs args);

    ///------------------------------------------------------------------------
		/// <summary>
		/// Occurs when [mouse down on legend item].
		/// </summary>
    ///------------------------------------------------------------------------
		[Bindable(true), Category("Events"),
     Description("Subscribe to be notified when the mouse button is clicked on a legend item.")]
    public event ExtendedZedMouseEventHandler MouseDownOnLegendItem;

    ///------------------------------------------------------------------------
		/// <summary>
		/// Occurs when [mouse down on master pane legend item].
		/// </summary>
    ///------------------------------------------------------------------------
		[Bindable(true), Category("Events"),
     Description("Subscribe to be notified when the mouse button is clicked in master pane legend area.")]
    public event ExtendedZedMouseEventHandler MouseDownOnMasterPaneLegendItem;

    ///------------------------------------------------------------------------
		/// <summary>
		/// Occurs when [mouse double click on legend item].
		/// </summary>
    ///------------------------------------------------------------------------
		[Bindable(true), Category("Events"),
     Description("Subscribe to be notified when the mouse button is double clicked on a legend item.")]
    public event ExtendedZedMouseEventHandler MouseDoubleClickOnLegendItem;

    ///------------------------------------------------------------------------
		/// <summary>
		/// Occurs when [mouse double click on master pane legend item].
		/// </summary>
    ///------------------------------------------------------------------------
		[Bindable(true), Category("Events"),
     Description("Subscribe to be notified when the mouse button is double clicked on a master pane legend item.")]
    public event ExtendedZedMouseEventHandler MouseDoubleClickOnMasterPaneLegendItem;

    ///------------------------------------------------------------------------
		/// <summary>
		/// Occurs when [mouse double click master pane legend area].
		/// </summary>
    ///------------------------------------------------------------------------
		[Bindable(true), Category("Events"),
     Description("Subscribe to be notified when the mouse button is double clicked in master pane legend area.")]
    public event ExtendedZedMouseEventHandler MouseDoubleClickMasterPaneLegendArea;

    ///------------------------------------------------------------------------
		/// <summary>
		/// Occurs when [mouse double click legend area].
		/// </summary>
    ///------------------------------------------------------------------------
		[Bindable(true), Category("Events"),
     Description("Subscribe to be notified when the mouse button is double clicked in legend area.")]
    public event ExtendedZedMouseEventHandler MouseDoubleClickLegendArea;

    ///------------------------------------------------------------------------
		/// <summary>
		/// Occurs when the graph pane is double clicked.
		/// </summary>
    ///------------------------------------------------------------------------
		[Bindable(true), Category("Events"),
     Description("Subscribe to be notified when the mouse button is double clicked in legend area.")]
    public event ExtendedZedMouseEventHandler MouseDoubleClickGraphPane;

    ///------------------------------------------------------------------------
		/// <summary>
		/// Occurs when the scale of the pane needs to be reset.
		/// </summary>
    ///------------------------------------------------------------------------
		[Bindable(true), Category("Events"),
     Description("Subscribe to manually control the scale resetting.")]
    public event ExtendedZedResetScaleEventHandler ResetScale;

    #endregion Events

    //-------------------------------------------------------------------------

    #region Public Properties

    ///------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets a value indicating whether individual x- and y- zooming is enabled or not.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if the individual x- and y- zooming is enabled; otherwise, <c>false</c>.
    /// </value>
    ///------------------------------------------------------------------------
    //public bool EnableIndividualXYZoom { get; set; }

    ///------------------------------------------------------------------------
		/// <summary>
		/// The list is used for enabling/disabling curves in the display, by right clicking on the legend.
		/// </summary>
    ///------------------------------------------------------------------------
		public List<string> HiddenCurves { get; } = new List<string>();

    ///------------------------------------------------------------------------
    /// <summary>
    /// Gets or sets the font for disabled curves.
    /// </summary>
    /// <value>The font for disabled curves.</value>
    ///------------------------------------------------------------------------
    public FontSpec FontDisabled { get; private set; }

    ///------------------------------------------------------------------------
		/// <summary>
		/// Gets or sets a value indicating whether [zoom to zero Y axis].
		/// </summary>
		/// <value><c>true</c> if [zoom to zero Y axis]; otherwise, <c>false</c>.</value>
    ///------------------------------------------------------------------------
		public bool ZoomToZeroYAxis { get; set; }

    ///------------------------------------------------------------------------
		/// <summary>
		/// If enabled, double-clicking on legend will zoom selected curve.
		/// </summary>
    ///------------------------------------------------------------------------
    public bool ZoomCurveOnLegendClick { get; set; }

    #endregion Public Properties

    //------------------------------------------------------------------------

    #region Public Methods

    ///------------------------------------------------------------------------
    /// <summary>
    /// Resets the scales of the specified <see cref="GraphPane"/> to their
    /// default values, if manually set; otherwise, sets them to autoscale.
    /// </summary>
    /// <remarks>
    /// The zoom stack will be cleared when the scales have been set.</remarks>
    /// <param name="graphPane"></param>
    ///------------------------------------------------------------------------
    public void ResetScaleToDefault(GraphPane graphPane)
    {
      OnResetScale(graphPane);
    }

    ///------------------------------------------------------------------------
		/// <summary>
		/// Adds/removes the index from the curve index list. The list is used for
		/// enabling/disabling curves in the spectrum display.
		/// </summary>
		/// <param name="index">The index of the curve to enable/disable.</param>
    ///------------------------------------------------------------------------
		public void AddRemoveHiddenCurveIndex(string index)
    {
      if (HiddenCurves.Contains(index))
        HiddenCurves.Remove(index);
      else
        HiddenCurves.Add(index);
    }

    ///------------------------------------------------------------------------
		/// <summary>
		/// Adds the curve to hidden list.
		/// </summary>
		/// <param name="index">The index.</param>
    ///------------------------------------------------------------------------
		public void AddCurveToHiddenList(string index)
    {
      if (!HiddenCurves.Contains(index))
        HiddenCurves.Add(index);
    }

    ///------------------------------------------------------------------------
		/// <summary>
		/// Removes the curve from hidden list.
		/// </summary>
		/// <param name="index">The index.</param>
    ///------------------------------------------------------------------------
		public void RemoveCurveFromHiddenList(string index)
    {
      if (HiddenCurves.Contains(index))
        HiddenCurves.Remove(index);
    }

    ///------------------------------------------------------------------------
    /// <summary>
    /// Zooms the curve.
    /// </summary>
    /// <param name="graphPane">The graph pane.</param>
    /// <param name="lineItem">The line item.</param>
    /// <returns></returns>
    ///------------------------------------------------------------------------
    public bool ZoomCurve(GraphPane graphPane, CurveItem lineItem,
      double filterMinX = double.MinValue, double filterMaxX = double.MaxValue)
    {
      if (lineItem.Points.Count == 0)
        return false;
      if (!ZoomCurveOnLegendClick)
      {
        Invalidate();
        return false;
      }

      // Add the zoom to the zoom stack
      var oldState = new ZoomState(graphPane, ZoomState.StateType.Zoom);
      graphPane.ZoomStack.Push(graphPane, ZoomState.StateType.Zoom);
      ZoomEvent?.Invoke(this, oldState, new ZoomState(graphPane, ZoomState.StateType.Zoom));

      if (!SetZoomScale(graphPane, lineItem, filterMinX, filterMaxX))
        return false;

      // Update the pane
      graphPane.AxisChange();
      Invalidate();
      return true;
    }

    /// <summary>
    /// Set zoom scale for a given <see cref="CurveItem"/> to fit the selected
    /// range between filterMinX to filterMaxX.  These filter values can be either
    /// ordinal values (if the X scale is ordinal) or X values.
    /// </summary>
    /// <returns>true if any scales were adjusted.</returns>
    public bool SetZoomScale(GraphPane graphPane, CurveItem lineItem,
      double filterMinX = double.MinValue, double filterMaxX = double.MaxValue)
    {
      // Set the axes scales
      var gap   = 0.0;
      var maxY  = double.MinValue;
      var maxX  = double.MinValue;
      var minY  = double.MaxValue;
      var minX  = double.MaxValue;

      int from = 0, to = lineItem.Points.Count - 1;

      if (filterMinX != double.MinValue || filterMaxX != double.MaxValue)
      {
        var xaxis = graphPane.XAxis.IsPrimary(graphPane) ? (Axis)graphPane.XAxis : graphPane.X2Axis;
        if (xaxis.Scale.IsAnyOrdinal)
        {
          from = filterMinX == double.MinValue
               ? 0 : ZedGraph.Scale.MinMax(0, (int)(filterMinX + 0.5), lineItem.Points.Count - 1);
          to   = filterMaxX == double.MaxValue
               ? lineItem.Points.Count - 1
               : ZedGraph.Scale.MinMax(0, (int)(filterMaxX + 0.5), lineItem.Points.Count - 1);
          filterMinX = double.MinValue;
          filterMaxX = double.MaxValue;
        }
      }

      for (var i = from; i <= to; i++)
      {
        var item = lineItem[i];
        if (item.X == PointPairBase.Missing || item.Y == PointPairBase.Missing)
          continue;
        if (item.X < filterMinX)
          continue;
        if (item.X > filterMaxX)
          break;

        maxY = Math.Max(item.Y, maxY);
        minY = Math.Min(item.Y, minY);
        maxX = Math.Max(item.X, maxX);
        minX = Math.Min(item.X, minX);
      }

      if (minY == double.MaxValue ||
          maxY == double.MinValue ||
          minX == double.MaxValue ||
          maxX == double.MinValue)
        return false;

      // Calculate the border gap and set the graph scale values.

      //gap = Math.Abs(maxY - minY);
      //gap = gap <= float.Epsilon ? 0.5 : gap * 0.05;

      var scale = lineItem.IsY2Axis ? graphPane.Y2AxisList[lineItem.YAxisIndex].Scale
                                    : graphPane.YAxisList[lineItem.YAxisIndex].Scale;
      var grace = (scale.Max - scale.Min) * scale.MaxGrace;
      gap = scale.ReverseTransform(10) + grace;
      scale.Max = maxY + gap;
      scale.Min = ZoomToZeroYAxis ? 0.0 : minY + (gap * -1.0);

      scale = lineItem.IsY2Axis ? graphPane.X2Axis.Scale : graphPane.XAxis.Scale;
      grace = (scale.Max - scale.Min) * scale.MaxGrace;
      gap = scale.ReverseTransform(10) + grace;
      scale.Max = maxX + gap;
      scale.Min = minX - gap;

      return true;
    }

    ///------------------------------------------------------------------------
		/// <summary>
		/// Toggles the curve visible.
		/// </summary>
		/// <param name="graphPane">The graph pane.</param>
		/// <param name="curveLabel">The curve label.</param>
    ///------------------------------------------------------------------------
		public void ToggleCurveVisible(GraphPane graphPane, string curveLabel)
    {
      CurveItem ci = graphPane.CurveList[graphPane.CurveList.IndexOf(curveLabel)];
      if (ci.IsVisible && graphPane.CurveList.Count(c => c.IsVisible) == 1)
        return;

      ci.IsVisible = !ci.IsVisible;
      AddRemoveHiddenCurveIndex(curveLabel);
      if (!ci.IsVisible)
      {
        ci.Tag = ci.Label.FontSpec;
        ci.Label.FontSpec = FontDisabled;
        ci.Label.FontSpec.Border.IsVisible = false;
      }
      else
      {
        ci.Label.FontSpec = null;
      }
      this.Invalidate();
    }

    ///-------------------------------------------------------------------------
		/// <summary>
		/// Makes the curves visible.
		/// </summary>
		/// <param name="graphPane">The graph pane.</param>
    ///-------------------------------------------------------------------------
		public void MakeCurvesVisible(GraphPane graphPane)
    {
      foreach (var curve in graphPane.CurveList.Where(curve => !curve.IsVisible))
      {
        RemoveCurveFromHiddenList(curve.Label.Text);
        curve.IsVisible = true;
        curve.Label.FontSpec = null;
      }
      this.Invalidate();
    }

    ///-------------------------------------------------------------------------
    /// <summary>
    /// Shows the curve exclusive. Hides all orher curves.
    /// </summary>
    /// <param name="graphPane">The graph pane.</param>
    /// <param name="curveLabel">The curve label.</param>
    /// <param name="showAllIfSingleCurve">If single curve is visible, make all
    /// curves visible</param>
    ///-------------------------------------------------------------------------
    public void ShowCurveExclusive(GraphPane graphPane, string curveLabel)
    {
      ShowCurveExclusive(graphPane, curveLabel, false);
    }

    ///-------------------------------------------------------------------------
    /// <summary>
    /// Shows the curve exclusive. Hides all orher curves.
    /// </summary>
    /// <param name="graphPane">The graph pane.</param>
    /// <param name="curveLabel">The curve label.</param>
    /// <param name="showAllIfSingleCurve">If single curve is visible, make all
    /// curves visible</param>
    ///-------------------------------------------------------------------------
    private void ShowCurveExclusive(GraphPane graphPane, string curveLabel,
      bool showAllIfSingleCurve)
    {
      if (!showAllIfSingleCurve || graphPane.CurveList.Count(c => c.IsVisible) == 1)
      {
        MakeCurvesVisible(graphPane);
        return;
      }

      foreach (var curve in graphPane.CurveList.Where(c => c.Label.Text != curveLabel))
      {
        curve.IsVisible = false;
        AddCurveToHiddenList(curve.Label.Text);
        curve.Tag = curve.Label.FontSpec;
        curve.Label.FontSpec = FontDisabled;
        curve.Label.FontSpec.Border.IsVisible = false;
      }

      var ci = graphPane.CurveList[curveLabel];
      ci.IsVisible = true;
      ci.Label.FontSpec = null;
      RemoveCurveFromHiddenList(curveLabel);
      ZoomCurve(graphPane, graphPane.CurveList[graphPane.CurveList.IndexOf(curveLabel)]);
    }

    #endregion Public Methods

    //---------------------------------------------------------------------------

    //---------------------------------------------------------------------------

    #region Non-Public

    //---------------------------------------------------------------------------

    #region Non-Public Methods

    ///--------------------------------------------------------------------------
    /// <summary>
    /// Individual x- and y- zooming. If, during a zoom action, the extention in
    /// one of the directions is less than 10 pixels, the zoom action is only be
    /// done for the direction with the big extension. A line is drawn instead
    /// of the rectangular rubberband.
    /// </summary>
    /// <returns>True, if a line has been drawn, false otherwise.</returns>
    ///--------------------------------------------------------------------------
    /*
    private bool DrawLineInsteadOfRectangle()
    {
      if (!EnableIndividualXYZoom)
        return false;

      Color color = Color.Cyan;
      int xPad = Math.Abs(_dragStartPt.X - _dragEndPt.X);
      int yPad = Math.Abs(_dragStartPt.Y - _dragEndPt.Y);

      if (!_drawCrossline && (xPad > 1 || yPad > 1))
        _drawCrossline = true;

      if (xPad < 10)
      {
        // Draw vertical line.
        Point p1 = new Point(_dragStartPt.X, _dragStartPt.Y);
        Point p2 = new Point(_dragStartPt.X, _dragEndPt.Y);
        ControlPaint.DrawReversibleLine(PointToScreen(p1), PointToScreen(p2), color);
        if (_drawCrossline)
        {
          p1 = new Point(_dragStartPt.X - 10, _dragStartPt.Y);
          p2 = new Point(_dragStartPt.X + 10, _dragStartPt.Y);
          ControlPaint.DrawReversibleLine(PointToScreen(p1), PointToScreen(p2), color);

          p1 = new Point(_dragStartPt.X - 10, _dragEndPt.Y);
          p2 = new Point(_dragStartPt.X + 10, _dragEndPt.Y);
          ControlPaint.DrawReversibleLine(PointToScreen(p1), PointToScreen(p2), color);
        }

        _isEnableHZoom = false;
        _isEnableVZoom = true;
        return true;
      }

      if (yPad < 10)
      {
        // Draw horizontal line.
        Point p1 = new Point(_dragStartPt.X, _dragStartPt.Y);
        Point p2 = new Point(_dragEndPt.X, _dragStartPt.Y);
        ControlPaint.DrawReversibleLine(PointToScreen(p1), PointToScreen(p2), color);
        if (_drawCrossline)
        {
          p1 = new Point(_dragStartPt.X, _dragStartPt.Y - 10);
          p2 = new Point(_dragStartPt.X, _dragStartPt.Y + 10);
          ControlPaint.DrawReversibleLine(PointToScreen(p1), PointToScreen(p2), color);

          p1 = new Point(_dragEndPt.X, _dragStartPt.Y - 10);
          p2 = new Point(_dragEndPt.X, _dragStartPt.Y + 10);
          ControlPaint.DrawReversibleLine(PointToScreen(p1), PointToScreen(p2), color);
        }

        _isEnableVZoom = false;
        _isEnableHZoom = true;
        return true;
      }

      // Draw rectangle as usual.
      _drawCrossline = false;
      _isEnableHZoom = true;
      _isEnableVZoom = true;
      return false;
    }
    */

    ///------------------------------------------------------------------------
		/// <summary>
		/// Handles the Opening event of the ContextMenuStrip control.
		/// It should not pop down when a legend item was clicked.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/>
		/// instance containing the event data.</param>
    ///------------------------------------------------------------------------
		private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
    {
      var graphPane = this.MasterPane.FindPane(this._menuClickPt);
      using (var g = this.CreateGraphics())
      {
        object nearestObject;
        int index;
        if (MasterPane.FindNearestPaneObject(_menuClickPt, g, out graphPane, out nearestObject, out index))
        {
          if (nearestObject is Legend)
            e.Cancel = true;
        }
        else if (MasterPane.Legend.FindPoint(_menuClickPt, MasterPane, MasterPane.CalcScaleFactor(), out index))
          e.Cancel = true;
      }
    }

    private void ZedGraphControl_ContextMenuBuilder(
      ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt,
      ContextMenuObjectState objState)
    {
      // Remove the 'Show Values' menu entry, since this control has its own value display.
      for (int i = 0; i < menuStrip.Items.Count; ++i)
      {
        var oldItem = menuStrip.Items[i];

        if (oldItem.Name == "set_default")
        {
          var item     = new ToolStripMenuItem();
          item.Name    = oldItem.Name;
          item.Tag     = oldItem.Tag;
          item.Text    = oldItem.Text;
          item.Click  += setDefaultItemClick;
          item.Enabled = oldItem.Enabled;

          menuStrip.Items.RemoveAt(i);
          menuStrip.Items.Insert(i, item);

          break;
        }
      }
    }

    ///-------------------------------------------------------------------------
    /// <summary>
    /// Handles the click on the 'Set default range' menu item.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing
    /// the event data.</param>
    ///-------------------------------------------------------------------------
    protected void setDefaultItemClick(object sender, EventArgs e)
    {
      if (_masterPane != null)
      {
        var pane = _masterPane.FindPane(_menuClickPt) as GraphPane;
        if (pane == null) return;

        var handler = ResetScale;
        if (handler != null)
        {
          var args = new ExtendedZedResetScaleEventArgs();
          handler(pane, args);
          if (args.Handled)
          {
            pane.ZoomStack.Clear();
            return;
          }
        }
      }

      MenuClick_RestoreScale(sender, e);
    }

    ///------------------------------------------------------------------------
		/// <summary>
		/// Initializes the Extended ZedGraph partial functionality.
		/// </summary>
    ///------------------------------------------------------------------------
		private void InitializeComponentPartial()
    {
      this.SuspendLayout();
      this.FontDisabled = new FontSpec("Arial", 12, Color.Gray, false, true, false);
      this.FontDisabled.Border.IsVisible = false;
      this.ZoomToZeroYAxis = false;

      // ZedGraphControl
      this.MouseDownEvent += ZedGraphControl_MouseDownEvent;
      this.DoubleClickEvent += ZedGraphControl_DoubleClickEvent;
      this.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
      this.ContextMenuBuilder += ZedGraphControl_ContextMenuBuilder;
      this.ResumeLayout(false);
    }

    ///------------------------------------------------------------------------
		/// <summary>
		/// Zeds the graph control_ mouse down event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/>
		/// instance containing the event data.</param>
		/// <returns>
		/// Return true if you have handled the MouseDown event entirely, and you
		/// do not want the <see cref="ZedGraphControl"/> to do any further action
		/// (e.g., starting a zoom operation).  Return false if ZedGraph should go
		/// ahead and process the MouseDown event.
		/// </returns>
    ///------------------------------------------------------------------------
		private bool ZedGraphControl_MouseDownEvent(ZedGraphControl sender, MouseEventArgs e)
    {
      PointF mousePoint = new PointF(e.X, e.Y);
      int curveIndex = -1;

      if (this.MasterPane.Legend.FindPoint(mousePoint, this.MasterPane, this.MasterPane.CalcScaleFactor(), out curveIndex))
      {
        if (MouseDownOnMasterPaneLegendItem != null)
        {
          return MouseDownOnMasterPaneLegendItem(sender.GraphPane, e, sender.GraphPane.CurveList[curveIndex].Label.Text);
        }
      }

      var graphPane = sender.MasterPane.FindPane(mousePoint) as GraphPane;

      // No graph pane was hit.
      if (graphPane == null)
        return false;

      if (!graphPane.Legend.FindPoint(mousePoint, graphPane, graphPane.CalcScaleFactor(), out curveIndex))
        return false;

      if (curveIndex >= graphPane.CurveList.Count) return false;

      if (this.MouseDownOnLegendItem != null)
        return this.MouseDownOnLegendItem(graphPane, e, graphPane.CurveList[curveIndex].Label.Text);

      switch (e.Button)
      {
        case MouseButtons.Left:
          //this.ZoomCurve(graphPane, graphPane.CurveList[curveIndex]);
          //return true;
          break;

        case MouseButtons.Right:
          ToggleCurveVisible(graphPane, graphPane.CurveList[curveIndex].Label.Text);
          return true;
      }

      return false;
    }

    ///------------------------------------------------------------------------
		/// <summary>
		/// Zeds the graph control_ double click event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/>
		/// instance containing the event data.</param>
		/// <returns>
		/// Return true if you have handled the MouseDown event entirely, and you
		/// do not want the <see cref="ZedGraphControl"/> to do any further action
		/// (e.g., starting a zoom operation).  Return false if ZedGraph should go
		/// ahead and process the MouseDown event.
		/// </returns>
    ///------------------------------------------------------------------------
		private bool ZedGraphControl_DoubleClickEvent(ZedGraphControl sender, MouseEventArgs e)
    {
      PointF mousePoint = e.Location;
      var curveIndex = -1;

      if (MasterPane.Legend.FindPoint(mousePoint, MasterPane, MasterPane.CalcScaleFactor(), out curveIndex))
        MouseDoubleClickOnMasterPaneLegendItem?.Invoke(sender.GraphPane, e, sender.GraphPane.CurveList[curveIndex].Label.Text);

      else if (mousePoint.Y <= this.MasterPane.Legend.Rect.Bottom)
      {
        if (MouseDoubleClickMasterPaneLegendArea?.Invoke(sender.GraphPane, e, "") ?? false)
          return true;

        if (e.Button == MouseButtons.Left)
        {
          this.MakeCurvesVisible(sender.GraphPane);
          return true;
        }
      }

      var graphPane = sender.MasterPane.FindPane(mousePoint) as GraphPane;

      // No graph pane was hit.
      if (graphPane == null)
        return false;

      if (MouseDoubleClickGraphPane?.Invoke(graphPane, e, null) ?? false)
        return true;

      if (!graphPane.Legend.FindPoint(mousePoint, graphPane, graphPane.CalcScaleFactor(), out curveIndex) &&
        mousePoint.Y > graphPane.Legend.Rect.Bottom)
      {
        OnResetScale(graphPane);
        return true;
      }

      if (graphPane.Legend.FindPoint(mousePoint, graphPane, graphPane.CalcScaleFactor(), out curveIndex))
      {
        if (curveIndex < graphPane.CurveList.Count)
        {
          if (MouseDoubleClickOnLegendItem?.Invoke(graphPane, e, graphPane.CurveList[curveIndex].Label.Text) ?? false)
            return true;

          if (e.Button == MouseButtons.Left)
          {
            this.ShowCurveExclusive(graphPane, graphPane.CurveList[curveIndex].Label.Text, true);
            return true;
          }
        }
        else
        {
          if (MouseDoubleClickLegendArea != null)
          {
            MouseDoubleClickLegendArea(graphPane, e, "");
            OnResetScale(graphPane);
            return false;
          }

          if (e.Button == MouseButtons.Left)
          {
            MakeCurvesVisible(graphPane);
            OnResetScale(graphPane);
            return true;
          }
        }
      }
      else if (mousePoint.Y <= graphPane.Legend.Rect.Bottom)
      {
        if (MouseDoubleClickLegendArea != null)
        {
          this.MouseDoubleClickLegendArea(graphPane, e, "");
          OnResetScale(graphPane);
          return false;
        }
        if (e.Button == MouseButtons.Left)
        {
          this.MakeCurvesVisible(graphPane);
          OnResetScale(graphPane);
          return true;
        }
      }
      return false;
    }

    ///------------------------------------------------------------------------
    /// <summary>
    /// Raises the ResetScale event.
    /// </summary>
    /// <param name="graphPane"></param>
    ///------------------------------------------------------------------------
    private void OnResetScale(GraphPane graphPane)
    {
      bool autoScale = true;
      var handler = ResetScale;
      if (handler != null)
      {
        ExtendedZedResetScaleEventArgs args = new ExtendedZedResetScaleEventArgs();
        handler(graphPane, args);
        autoScale = !args.Handled;
      }
      if (autoScale)
      {
        RestoreScale(graphPane);
        IsEnableHZoom = true;
        IsEnableVZoom = true;
      }
      graphPane.ZoomStack.Clear();
    }

    #endregion Non-Public Methods

    //---------------------------------------------------------------------------

    #region Non-Public Members

    /*
    ///--------------------------------------------------------------------------
    /// <summary>
    /// Internal variable, used by the individual x- and y- zooming.
    /// </summary>
    ///--------------------------------------------------------------------------
    private bool _drawCrossline;
    */

    #endregion Non-Public Members

    #endregion Non-Public

    //---------------------------------------------------------------------------
  }

  ///----------------------------------------------------------------------------
  /// <summary>
  /// Arguments for the ResetScale event.
  /// </summary>
  ///----------------------------------------------------------------------------
  public class ExtendedZedResetScaleEventArgs : EventArgs
  {
    /// <summary>
    /// <code>true</code> if the resetting of the scales has been handled manually;
    /// otherwise, <code>false</code>.
    /// </summary>
    public bool Handled { get; set; }
  }
}