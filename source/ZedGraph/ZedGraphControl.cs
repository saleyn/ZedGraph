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
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace ZedGraph
{
  /// <summary>
  /// The ZedGraphControl class provides a UserControl interface to the
  /// <see cref="ZedGraph"/> class library.  This allows ZedGraph to be installed
  /// as a control in the Visual Studio toolbox.  You can use the control by simply
  /// dragging it onto a form in the Visual Studio form editor.  All graph
  /// attributes are accessible via the <see cref="ZedGraphControl.GraphPane"/>
  /// property.
  /// </summary>
  /// <author> John Champion revised by Jerry Vos </author>
  /// <version> $Revision: 3.86 $ $Date: 2007-11-03 04:41:29 $ </version>
  public partial class ZedGraphControl : UserControl
  {

  #region Private Fields

    /// <summary>
    /// This private field contains the instance for the MasterPane object of this control.
    /// You can access the MasterPane object through the public property
    /// <see cref="ZedGraphControl.MasterPane"/>. This is nulled when this Control is
    /// disposed.
    /// </summary>
    private MasterPane _masterPane;

    private SaveFileDialog _saveFileDialog;

    // Revision: JCarpenter 10/06

    private ScrollRange _xScrollRange;

    private bool _isShowHScrollBar = false;
    private bool _isShowVScrollBar = false;
    //private bool    isScrollY2 = false;

    private bool _isSynchronizeXAxes = false;
    private bool _isSynchronizeYAxes = false;

    //private System.Windows.Forms.HScrollBar hScrollBar1;
    //private System.Windows.Forms.VScrollBar vScrollBar1;

    // The range of values to use the scroll control bars
    private const int _ScrollControlSpan = int.MaxValue;
    // The ratio of the largeChange to the smallChange for the scroll bars
    private const int _ScrollSmallRatio = 10;

    private bool _isZoomOnMouseCenter = false;

    /// <summary>
    /// private field that stores a <see cref="PrintDocument" /> instance, which maintains
    /// a persistent selection of printer options.
    /// </summary>
    /// <remarks>
    /// This is needed so that a "Print" action utilizes the settings from a prior
    /// "Page Setup" action.</remarks>
    private PrintDocument _pdSave = null;
    //private PrinterSettings printSave = null;
    //private PageSettings pageSave = null;

    #endregion

  #region Fields: Buttons & Keys Properties

    // Setting this field to Keys.Shift here
    // causes an apparent bug to crop up in VS 2003, by which it will have the value:
    // "System.Windows.Forms.Keys.Shift+None", which won't compile

    // Setting this field to Keys.Shift here
    // causes an apparent bug to crop up in VS 2003, by which it will have the value:
    // "System.Windows.Forms.Keys.Shift+None", which won't compile

    #endregion

  #region Fields: Temporary state variables

    /// <summary>
    /// Internal variable that indicates the control is currently being zoomed. 
    /// </summary>
    private bool _isZooming = false;
    /// <summary>
    /// Internal variable that indicates the control is currently being panned.
    /// </summary>
    private bool _isPanning = false;
    /// <summary>
    /// Internal variable that indicates a point value is currently being edited.
    /// </summary>
    private bool _isEditing = false;

    // Revision: JCarpenter 10/06
    /// <summary>
    /// Internal variable that indicates the control is currently using selection. 
    /// </summary>
    private bool _isSelecting = false;

    /// <summary>
    /// Internal variable that stores the <see cref="GraphPane"/> reference for the Pane that is
    /// currently being zoomed or panned.
    /// </summary>
    private PaneBase _currentPane = null;
    /// <summary>
    /// Internal variable that stores a rectangle which is either the zoom rectangle, or the incremental
    /// pan amount since the last mousemove event.
    /// </summary>
    private Point _dragStartPt;
    private Point _dragEndPt;

    private int _dragIndex;
    private CurveItem  _dragCurve;
    private IPointPair _dragStartPair;
    /// <summary>
    /// private field that stores the state of the scale ranges prior to starting a panning action.
    /// </summary>
    private ZoomState _zoomState;
    private ZoomStateStack _zoomStateStack;

    /// <summary>
    /// private field that stores the state of drag graph
    /// </summary>
    private GraphDragState _graphDragState;
    private bool _isGraphDragging = false;

    //temporarily save the location of a context menu click so we can use it for reference
    // Note that Control.MousePosition ends up returning the position after the mouse has
    // moved to the menu item within the context menu.  Therefore, this point is saved so
    // that we have the point at which the context menu was first right-clicked
    internal Point _menuClickPt;

  #endregion

  #region Constructors

    /// <summary>
    /// Default Constructor
    /// </summary>
    public ZedGraphControl()
    {
      InitializeComponent();
      InitializeComponentPartial();
      this._tooltip = ValuesToolTip.Create(this, this.pointToolTip);

      // These commands do nothing, but they get rid of the compiler warnings for
      // unused events
      bool b = MouseDown == null || MouseUp == null || MouseMove == null;

      // Link in these events from the base class, since we disable them from this class.
      base.MouseDown += ZedGraphControl_MouseDown;
      base.MouseUp   += ZedGraphControl_MouseUp;
      base.MouseMove += ZedGraphControl_MouseMove;

      //this.MouseWheel += new System.Windows.Forms.MouseEventHandler( this.ZedGraphControl_MouseWheel );

      // Use double-buffering for flicker-free updating:
      SetStyle( ControlStyles.UserPaint    | ControlStyles.AllPaintingInWmPaint
              | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true );
      //isTransparentBackground = false;
      //SetStyle( ControlStyles.Opaque, false );
      SetStyle( ControlStyles.SupportsTransparentBackColor, true );
      //this.BackColor = Color.Transparent;

      Rectangle rect = new Rectangle( 0, 0, this.Size.Width, this.Size.Height );
      _masterPane = new MasterPane( "", rect );
      _masterPane.Margin.All = 0;
      _masterPane.Title.IsVisible = false;

      string titleStr = ZedGraphLocale.title_def;
      string xStr = ZedGraphLocale.x_title_def;
      string yStr = ZedGraphLocale.y_title_def;

      //GraphPane graphPane = new GraphPane( rect, "Title", "X Axis", "Y Axis" );
      GraphPane graphPane = new GraphPane( rect, titleStr, xStr, yStr );
      using ( Graphics g = this.CreateGraphics() )
      {
        graphPane.AxisChange( g );
        //g.Dispose();
      }
      _masterPane.Add( graphPane );

      this.hScrollBar1.Minimum = 0;
      this.hScrollBar1.Maximum = 100;
      this.hScrollBar1.Value = 0;

      this.vScrollBar1.Minimum = 0;
      this.vScrollBar1.Maximum = 100;
      this.vScrollBar1.Value = 0;

      _xScrollRange = new ScrollRange( true );

      YScrollRangeList.Add( new ScrollRange( true ) );
      Y2ScrollRangeList.Add( new ScrollRange( false ) );

      _zoomState = null;
      _zoomStateStack = new ZoomStateStack();

      _graphDragState = new GraphDragState();

      CrossHairFontSpec = new FontSpec
      {
        FontColor = Color.Black,
        Size      = 9,
        Border    = { IsVisible = true    },
        Fill      = { Color = Color.Beige, Brush = new SolidBrush(Color.Beige) },
        TextBrush = new SolidBrush(Color.Black)
      };
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if the components should be
    /// disposed, false otherwise</param>
    protected override void Dispose( bool disposing )
    {
      lock ( this )
      {
        if ( disposing )
          components?.Dispose();
        base.Dispose( disposing );

        _masterPane = null;
      }
    }
  
  #endregion

  #region Methods

    /// <summary>
    /// Called by the system to update the control on-screen
    /// </summary>
    /// <param name="e">
    /// A PaintEventArgs object containing the Graphics specifications
    /// for this Paint event.
    /// </param>
    protected override void OnPaint( PaintEventArgs e )
    {
      lock ( this )
      {
        if ( BeenDisposed || _masterPane == null || GraphPane == null )
          return;

        if ( hScrollBar1 != null && this.GraphPane != null &&
          vScrollBar1 != null && YScrollRangeList != null )
        {
          SetScroll( hScrollBar1, this.GraphPane.XAxis, _xScrollRange.Min, _xScrollRange.Max );
          SetScroll( vScrollBar1, this.GraphPane.YAxis, YScrollRangeList[0].Min,
            YScrollRangeList[0].Max );
        }

        base.OnPaint( e );

        // Add a try/catch pair since the users of the control can't catch this one
        try { _masterPane.Draw(e.Graphics); }
        catch (Exception er)
        {
          var s = er.ToString();
        }

        if (IsShowCrossHair && _mouseInBounds && !_lastCrosshairPoint.IsEmpty)
        {
          var g = e.Graphics;
          if (CrossHairType == CrossHairType.MasterPane)
          {
            var sz = this.ClientSize;
            g.DrawLine(CrossHairPen, _lastCrosshairPoint.X, 0, _lastCrosshairPoint.X, sz.Height);
            g.DrawLine(CrossHairPen, 0, _lastCrosshairPoint.Y, sz.Width, _lastCrosshairPoint.Y);
            return;
          }

          var pane = _currentPane as GraphPane;

          if (pane != null && !pane.CurveList.Any(c => c.IsPie)
                           && pane.Chart.Rect.Contains(_lastCrosshairPoint))
          {
            // Draw cross-hair lines
            var rect = pane.Chart.Rect;
            g.SetClip(rect);
            g.DrawLine(CrossHairPen, (int)rect.Left, _lastCrosshairPoint.Y, (int)rect.Right, _lastCrosshairPoint.Y);
            g.DrawLine(CrossHairPen, _lastCrosshairPoint.X, (int)rect.Top, _lastCrosshairPoint.X, (int)rect.Bottom);
            g.ResetClip();
            
            var xaxis = pane.XAxis.Scale.Valid ? (Axis)pane.XAxis : pane.X2Axis;
            var yaxis = pane.YAxis.Scale.Valid ? (Axis)pane.YAxis : pane.Y2Axis;

            CrossHairFontSpec.ScaleFactor = _masterPane.ScaleFactor;

            // Draw crosshair values at each axis
            _lastCrosshairXlabelRect = xaxis.DrawXValueLabel(g, pane, _lastCrosshairPoint.X, CrossHairFontSpec);
            _lastCrosshairYlabelRect = yaxis.DrawYValueLabel(g, pane, _lastCrosshairPoint.Y, CrossHairFontSpec);
          }
        }
      }
    }

    /// <summary>
    /// Called when the control has been resized.
    /// </summary>
    /// <param name="sender">
    /// A reference to the control that has been resized.
    /// </param>
    /// <param name="e">
    /// An EventArgs object.
    /// </param>
    protected void ZedGraphControl_ReSize( object sender, System.EventArgs e )
    {
      lock ( this )
      {
        if ( BeenDisposed || _masterPane == null )
          return;

        Size newSize = this.Size;

        if ( _isShowHScrollBar )
        {
          hScrollBar1.Visible = true;
          newSize.Height -= this.hScrollBar1.Size.Height;
          hScrollBar1.Location = new Point( 0, newSize.Height );
          hScrollBar1.Size = new Size( newSize.Width, hScrollBar1.Height );
        }
        else
          hScrollBar1.Visible = false;

        if ( _isShowVScrollBar )
        {
          vScrollBar1.Visible = true;
          newSize.Width -= this.vScrollBar1.Size.Width;
          vScrollBar1.Location = new Point( newSize.Width, 0 );
          vScrollBar1.Size = new Size( vScrollBar1.Width, newSize.Height );
        }
        else
          vScrollBar1.Visible = false;

        using ( Graphics g = CreateGraphics() )
          _masterPane.ReSize( g, new RectangleF( 0, 0, newSize.Width, newSize.Height ) );

        this.Invalidate();
      }
    }
    /// <summary>This performs an axis change command on the graphPane.
    /// </summary>
    /// <remarks>
    /// This is the same as
    /// <c>ZedGraphControl.GraphPane.AxisChange( ZedGraphControl.CreateGraphics() )</c>, however,
    /// this method also calls <see cref="SetScrollRangeFromData" /> if <see cref="IsAutoScrollRange" />
    /// is true.
    /// </remarks>
    public virtual void AxisChange()
    {
      lock ( this )
      {
        if ( BeenDisposed || _masterPane == null )
          return;

        using ( Graphics g = this.CreateGraphics() )
        {
          _masterPane.AxisChange( g );
          //g.Dispose();
        }

        if ( IsAutoScrollRange )
          SetScrollRangeFromData();
      }
    }
  #endregion

  #region Zoom States

    /// <summary>
    /// Save the current states of the GraphPanes to a separate collection.  Save a single
    /// (<see paramref="primaryPane" />) GraphPane if the panes are not synchronized
    /// (see <see cref="IsSynchronizeXAxes" /> and <see cref="IsSynchronizeYAxes" />),
    /// or save a list of states for all GraphPanes if the panes are synchronized.
    /// </summary>
    /// <param name="primaryPane">The primary GraphPane on which zoom/pan/scroll operations
    /// are taking place</param>
    /// <param name="type">The <see cref="ZoomState.StateType" /> that describes the
    /// current operation</param>
    /// <returns>The <see cref="ZoomState" /> that corresponds to the
    /// <see paramref="primaryPane" />.
    /// </returns>
    private ZoomState ZoomStateSave( GraphPane primaryPane, ZoomState.StateType type )
    {
      ZoomStateClear();

      if ( _isSynchronizeXAxes || _isSynchronizeYAxes )
      {
        foreach ( var pane in _masterPane.PaneList.Where(p => p is GraphPane).Cast<GraphPane>())
        {
          var state = new ZoomState( pane, type );
          if ( pane == primaryPane )
            _zoomState = state;
          _zoomStateStack.Add( state );
        }
      }
      else
        _zoomState = new ZoomState( primaryPane, type );

      return _zoomState;
    }

    /// <summary>
    /// Restore the states of the GraphPanes to a previously saved condition (via
    /// <see cref="ZoomStateSave" />.  This is essentially an "undo" for live
    /// pan and scroll actions.  Restores a single
    /// (<see paramref="primaryPane" />) GraphPane if the panes are not synchronized
    /// (see <see cref="IsSynchronizeXAxes" /> and <see cref="IsSynchronizeYAxes" />),
    /// or save a list of states for all GraphPanes if the panes are synchronized.
    /// </summary>
    /// <param name="primaryPane">The primary GraphPane on which zoom/pan/scroll operations
    /// are taking place</param>
    private void ZoomStateRestore( GraphPane primaryPane )
    {
      if (_isSynchronizeXAxes || _isSynchronizeYAxes)
      {
        for (var i = 0; i < _masterPane.PaneList.Count; i++)
          if (i < _zoomStateStack.Count)
            _zoomStateStack[i].ApplyState(_masterPane.PaneList[i] as GraphPane);
      }
      else
        _zoomState?.ApplyState(primaryPane);

      ZoomStateClear();
    }

    /// <summary>
    /// Place the previously saved states of the GraphPanes on the individual GraphPane
    /// <see cref="ZedGraph.GraphPane.ZoomStack" /> collections.  This provides for an
    /// option to undo the state change at a later time.  Save a single
    /// (<see paramref="primaryPane" />) GraphPane if the panes are not synchronized
    /// (see <see cref="IsSynchronizeXAxes" /> and <see cref="IsSynchronizeYAxes" />),
    /// or save a list of states for all GraphPanes if the panes are synchronized.
    /// </summary>
    /// <param name="primaryPane">The primary GraphPane on which zoom/pan/scroll operations
    /// are taking place</param>
    /// <returns>The <see cref="ZoomState" /> that corresponds to the
    /// <see paramref="primaryPane" />.
    /// </returns>
    private void ZoomStatePush( PaneBase primaryPane )
    {
      var pane = primaryPane as GraphPane;
      if (pane == null) return;

      if ( _isSynchronizeXAxes || _isSynchronizeYAxes )
      {
        for ( int i = 0; i < _masterPane.PaneList.Count; i++ )
        {
          if ( i < _zoomStateStack.Count && _masterPane.PaneList[i] is GraphPane)
            ((GraphPane)_masterPane.PaneList[i]).ZoomStack.Add( _zoomStateStack[i] );
        }
      }
      else if ( _zoomState != null )
        pane.ZoomStack.Add( _zoomState );

      ZoomStateClear();
    }

    /// <summary>
    /// Clear the collection of saved states.
    /// </summary>
    private void ZoomStateClear()
    {
      _zoomStateStack.Clear();
      _zoomState = null;
    }

    /// <summary>
    /// Clear all states from the undo stack for each GraphPane.
    /// </summary>
    private void ZoomStatePurge()
    {
      foreach ( var pane in _masterPane.PaneList.Where(p => p is GraphPane).Cast<GraphPane>())
        pane.ZoomStack.Clear();
    }

  #endregion

  }
}
