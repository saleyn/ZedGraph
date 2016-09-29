//============================================================================
//ZedGraph Class Library - A Flexible Line Graph/Bar Graph Library in C#
//Copyright ?2007  John Champion
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ZedGraph
{
  partial class ZedGraphControl
  {

  #region Private Fields
    private ZedGraphZoomBand _zoomBand;
    private Point            _lastMousePt;
    private bool             _mouseInBounds;
    private Axis             _mouseHoveredAxis;
    private int              _mouseHoveredYAxisIndex;
    private RectangleF       _mouseHoveredAxisRect;
    internal Point           _lastCrosshairPoint;
    private Rectangle        _lastCrosshairXlabelRect;
    private Rectangle        _lastCrosshairYlabelRect;
  #endregion

  #region Events

    /// <summary>
    /// A delegate that allows subscribing methods to append or modify the context menu.
    /// </summary>
    /// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
    /// <param name="menuStrip">A reference to the <see cref="ContextMenuStrip"/> object
    /// that contains the context menu.
    /// </param>
    /// <param name="mousePt">The point at which the mouse was clicked</param>
    /// <param name="objState">The current context menu state</param>
    /// <seealso cref="ContextMenuBuilder" />
    public delegate void ContextMenuBuilderEventHandler( ZedGraphControl sender,
      ContextMenuStrip menuStrip, Point mousePt, ContextMenuObjectState objState );
    /// <summary>
    /// Subscribe to this event to be able to modify the ZedGraph context menu.
    /// </summary>
    /// <remarks>
    /// The context menu is built on the fly after a right mouse click.  You can add menu items
    /// to this menu by simply modifying the <see paramref="menu"/> parameter.
    /// </remarks>
    [Bindable( true ), Category( "Events" ),
     Description( "Subscribe to this event to be able to modify the ZedGraph context menu" )]
    public event ContextMenuBuilderEventHandler ContextMenuBuilder;

    /// <summary>
    /// A delegate that allows notification of zoom and pan events.
    /// </summary>
    /// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
    /// <param name="oldState">A <see cref="ZoomState"/> object that corresponds to the state of the
    /// <see cref="GraphPane"/> before the zoom or pan event.</param>
    /// <param name="newState">A <see cref="ZoomState"/> object that corresponds to the state of the
    /// <see cref="GraphPane"/> after the zoom or pan event</param>
    /// <seealso cref="ZoomEvent" />
    public delegate void ZoomEventHandler( ZedGraphControl sender, ZoomState oldState,
      ZoomState newState );

    /// <summary>
    /// Subscribe to this event to be notified when the <see cref="GraphPane"/> is zoomed or panned by the user,
    /// either via a mouse drag operation or by the context menu commands.
    /// </summary>
    [Bindable( true ), Category( "Events" ),
     Description( "Subscribe to this event to be notified when the graph is zoomed or panned" )]
    public event ZoomEventHandler ZoomEvent;

    /// <summary>
    /// A delegate that allows notification of scroll events.
    /// </summary>
    /// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
    /// <param name="scrollBar">The source <see cref="ScrollBar"/> object</param>
    /// <param name="oldState">A <see cref="ZoomState"/> object that corresponds to the state of the
    /// <see cref="GraphPane"/> before the scroll event.</param>
    /// <param name="newState">A <see cref="ZoomState"/> object that corresponds to the state of the
    /// <see cref="GraphPane"/> after the scroll event</param>
    /// <seealso cref="ZoomEvent" />
    public delegate void ScrollDoneHandler( ZedGraphControl sender, ScrollBar scrollBar,
      ZoomState oldState, ZoomState newState );

    /// <summary>
    /// Subscribe to this event to be notified when the <see cref="GraphPane"/> is scrolled by the user
    /// using the scrollbars.
    /// </summary>
    [Bindable( true ), Category( "Events" ),
     Description( "Subscribe this event to be notified when a scroll operation using the scrollbars is completed" )]
    public event ScrollDoneHandler ScrollDoneEvent;

    /// <summary>
    /// A delegate that allows notification of scroll events.
    /// </summary>
    /// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
    /// <param name="scrollBar">The source <see cref="ScrollBar"/> object</param>
    /// <param name="oldState">A <see cref="ZoomState"/> object that corresponds to the state of the
    /// <see cref="GraphPane"/> before the scroll event.</param>
    /// <param name="newState">A <see cref="ZoomState"/> object that corresponds to the state of the
    /// <see cref="GraphPane"/> after the scroll event</param>
    /// <seealso cref="ZoomEvent" />
    public delegate void ScrollProgressHandler( ZedGraphControl sender, ScrollBar scrollBar,
      ZoomState oldState, ZoomState newState );

    /// <summary>
    /// Subscribe to this event to be notified when the <see cref="GraphPane"/> is scrolled by the user
    /// using the scrollbars.
    /// </summary>
    [Bindable( true ), Category( "Events" ),
     Description( "Subscribe this event to be notified continuously as a scroll operation is taking place" )]
    public event ScrollProgressHandler ScrollProgressEvent;

    /// <summary>
    /// Subscribe to this event to be notified when the <see cref="GraphPane"/> is scrolled by the user
    /// using the scrollbars.
    /// </summary>
    [Bindable( true ), Category( "Events" ),
     Description( "Subscribe this event to be notified of general scroll events" )]
    public event ScrollEventHandler ScrollEvent;

    /// <summary>
    /// A delegate that receives notification after a point-edit operation is completed.
    /// </summary>
    /// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
    /// <param name="pane">The <see cref="GraphPane"/> object that contains the
    /// point that has been edited</param>
    /// <param name="curve">The <see cref="CurveItem"/> object that contains the point
    /// that has been edited</param>
    /// <param name="iPt">The integer index of the edited <see cref="PointPair"/> within the
    /// <see cref="IPointList"/> of the selected <see cref="CurveItem"/>
    /// </param>
    /// <seealso cref="PointValueEvent" />
    public delegate string PointEditHandler( ZedGraphControl sender, GraphPane pane,
      CurveItem curve, int iPt );

    /// <summary>
    /// Subscribe to this event to receive notifcation and/or respond after a data
    /// point has been edited via <see cref="IsEnableHEdit" /> and <see cref="IsEnableVEdit" />.
    /// </summary>
    /// <example>
    /// <para>To subscribe to this event, use the following in your Form_Load method:</para>
    /// <code>zedGraphControl1.PointEditEvent +=
    /// new ZedGraphControl.PointEditHandler( MyPointEditHandler );</code>
    /// <para>Add this method to your Form1.cs:</para>
    /// <code>
    ///    private string MyPointEditHandler( object sender, GraphPane pane, CurveItem curve, int iPt )
    ///    {
    ///        PointPair pt = curve[iPt];
    ///        return "This value is " + pt.Y.ToString("f2") + " gallons";
    ///    }</code>
    /// </example>
    [Bindable( true ), Category( "Events" ),
     Description( "Subscribe to this event to respond to data point edit actions" )]
    public event PointEditHandler PointEditEvent;

    /// <summary>
    /// A delegate that allows custom formatting of the point value tooltips
    /// </summary>
    /// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
    /// <param name="pane">The <see cref="GraphPane"/> object that contains the point value of interest</param>
    /// <param name="curve">The <see cref="CurveItem"/> object that contains the point value of interest</param>
    /// <param name="iPt">The integer index of the selected <see cref="PointPair"/> within the
    /// <see cref="IPointList"/> of the selected <see cref="CurveItem"/></param>
    /// <seealso cref="PointValueEvent" />
    public delegate string PointValueHandler( ZedGraphControl sender, GraphPane pane,
      CurveItem curve, int iPt );

    /// <summary>
    /// Subscribe to this event to provide custom formatting for the tooltips
    /// </summary>
    /// <example>
    /// <para>To subscribe to this event, use the following in your FormLoad method:</para>
    /// <code>zedGraphControl1.PointValueEvent +=
    /// new ZedGraphControl.PointValueHandler( MyPointValueHandler );</code>
    /// <para>Add this method to your Form1.cs:</para>
    /// <code>
    ///    private string MyPointValueHandler( object sender, GraphPane pane, CurveItem curve, int iPt )
    ///    {
    ///    #region
    ///        PointPair pt = curve[iPt];
    ///        return "This value is " + pt.Y.ToString("f2") + " gallons";
    ///    #endregion
    ///    }</code>
    /// </example>
    [Bindable( true ), Category( "Events" ),
     Description( "Subscribe to this event to provide custom-formatting for data point tooltips" )]
    public event PointValueHandler PointValueEvent;

    /// <summary>
    /// A delegate that allows custom formatting of the cursor value tooltips
    /// </summary>
    /// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
    /// <param name="pane">The <see cref="GraphPane"/> object that contains the cursor of interest</param>
    /// <param name="mousePt">The <see cref="Point"/> object that represents the cursor value location</param>
    /// <seealso cref="CursorValueEvent" />
    public delegate string CursorValueHandler( ZedGraphControl sender, GraphPane pane,
      Point mousePt );

    /// <summary>
    /// Subscribe to this event to provide custom formatting for the cursor value tooltips
    /// </summary>
    /// <example>
    /// <para>To subscribe to this event, use the following in your FormLoad method:</para>
    /// <code>zedGraphControl1.CursorValueEvent +=
    /// new ZedGraphControl.CursorValueHandler( MyCursorValueHandler );</code>
    /// <para>Add this method to your Form1.cs:</para>
    /// <code>
    ///    private string MyCursorValueHandler( object sender, GraphPane pane, Point mousePt )
    ///    {
    ///    #region
    ///    double x, y;
    ///    pane.ReverseTransform( mousePt, out x, out y );
    ///    return "( " + x.ToString( "f2" ) + ", " + y.ToString( "f2" ) + " )";
    ///    #endregion
    ///    }</code>
    /// </example>
    [Bindable( true ), Category( "Events" ),
     Description( "Subscribe to this event to provide custom-formatting for cursor value tooltips" )]
    public event CursorValueHandler CursorValueEvent;

    /// <summary>
    /// A delegate that allows notification of mouse events on Graph objects.
    /// </summary>
    /// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
    /// <param name="e">A <see cref="MouseEventArgs" /> corresponding to this event</param>
    /// <seealso cref="MouseDownEvent" />
    /// <returns>
    /// Return true if you have handled the mouse event entirely, and you do not
    /// want the <see cref="ZedGraphControl"/> to do any further action (e.g., starting
    /// a zoom operation).  Return false if ZedGraph should go ahead and process the
    /// mouse event.
    /// </returns>
    public delegate bool ZedMouseEventHandler( ZedGraphControl sender, MouseEventArgs e );

    /// <summary>
    /// Subscribe to this event to provide notification of MouseDown clicks on graph
    /// objects
    /// </summary>
    /// <remarks>
    /// This event provides for a notification when the mouse is clicked on an object
    /// within any <see cref="GraphPane"/> of the <see cref="MasterPane"/> associated
    /// with this <see cref="ZedGraphControl" />.  This event will use the
    /// <see cref="ZedGraph.MasterPane.FindNearestPaneObject"/> method to determine which object
    /// was clicked.  The boolean value that you return from this handler determines whether
    /// or not the <see cref="ZedGraphControl"/> will do any further handling of the
    /// MouseDown event (see <see cref="ZedMouseEventHandler" />).  Return true if you have
    /// handled the MouseDown event entirely, and you do not
    /// want the <see cref="ZedGraphControl"/> to do any further action (e.g., starting
    /// a zoom operation).  Return false if ZedGraph should go ahead and process the
    /// MouseDown event.
    /// </remarks>
    [Bindable( true ), Category( "Events" ),
     Description( "Subscribe to be notified when the left mouse button is clicked down" )]
    public event ZedMouseEventHandler MouseDownEvent;

    /// <summary>
    /// Hide the standard control MouseDown event so that the ZedGraphControl.MouseDownEvent
    /// can be used.  This is so that the user must return true/false in order to indicate
    /// whether or not we should respond to the event.
    /// </summary>
    [Bindable( false ), Browsable( false )]
    public new event MouseEventHandler MouseDown;
    /// <summary>
    /// Hide the standard control MouseUp event so that the ZedGraphControl.MouseUpEvent
    /// can be used.  This is so that the user must return true/false in order to indicate
    /// whether or not we should respond to the event.
    /// </summary>
    [Bindable( false ), Browsable( false )]
    public new event MouseEventHandler MouseUp;
    /// <summary>
    /// Hide the standard control MouseMove event so that the ZedGraphControl.MouseMoveEvent
    /// can be used.  This is so that the user must return true/false in order to indicate
    /// whether or not we should respond to the event.
    /// </summary>
    [Bindable( false ), Browsable( false )]
    private new event MouseEventHandler MouseMove;
    /// <summary>
    /// Subscribe to this event to provide notification of MouseUp clicks on graph
    /// objects
    /// </summary>
    /// <remarks>
    /// This event provides for a notification when the mouse is clicked on an object
    /// within any <see cref="GraphPane"/> of the <see cref="MasterPane"/> associated
    /// with this <see cref="ZedGraphControl" />.  This event will use the
    /// <see cref="ZedGraph.MasterPane.FindNearestPaneObject"/> method to determine which object
    /// was clicked.  The boolean value that you return from this handler determines whether
    /// or not the <see cref="ZedGraphControl"/> will do any further handling of the
    /// MouseUp event (see <see cref="ZedMouseEventHandler" />).  Return true if you have
    /// handled the MouseUp event entirely, and you do not
    /// want the <see cref="ZedGraphControl"/> to do any further action (e.g., starting
    /// a zoom operation).  Return false if ZedGraph should go ahead and process the
    /// MouseUp event.
    /// </remarks>
    [Bindable( true ), Category( "Events" ),
     Description( "Subscribe to be notified when the left mouse button is released" )]
    public event ZedMouseEventHandler MouseUpEvent;
    /// <summary>
    /// Subscribe to this event to provide notification of MouseMove events over graph
    /// objects
    /// </summary>
    /// <remarks>
    /// This event provides for a notification when the mouse is moving over on the control.
    /// The boolean value that you return from this handler determines whether
    /// or not the <see cref="ZedGraphControl"/> will do any further handling of the
    /// MouseMove event (see <see cref="ZedMouseEventHandler" />).  Return true if you
    /// have handled the MouseMove event entirely, and you do not
    /// want the <see cref="ZedGraphControl"/> to do any further action.
    /// Return false if ZedGraph should go ahead and process the MouseMove event.
    /// </remarks>
    [Bindable( true ), Category( "Events" ),
     Description( "Subscribe to be notified when the mouse is moved inside the control" )]
    public event ZedMouseEventHandler MouseMoveEvent;

    /// <summary>
    /// Subscribe to this event to provide notification of Double Clicks on graph
    /// objects
    /// </summary>
    /// <remarks>
    /// This event provides for a notification when the mouse is double-clicked on an object
    /// within any <see cref="GraphPane"/> of the <see cref="MasterPane"/> associated
    /// with this <see cref="ZedGraphControl" />.  This event will use the
    /// <see cref="ZedGraph.MasterPane.FindNearestPaneObject"/> method to determine which object
    /// was clicked.  The boolean value that you return from this handler determines whether
    /// or not the <see cref="ZedGraphControl"/> will do any further handling of the
    /// DoubleClick event (see <see cref="ZedMouseEventHandler" />).  Return true if you have
    /// handled the DoubleClick event entirely, and you do not
    /// want the <see cref="ZedGraphControl"/> to do any further action. 
    /// Return false if ZedGraph should go ahead and process the
    /// DoubleClick event.
    /// </remarks>
    [Bindable( true ), Category( "Events" ),
     Description( "Subscribe to be notified when the left mouse button is double-clicked" )]
    public event ZedMouseEventHandler DoubleClickEvent;

    /// <summary>
    /// A delegate that allows notification of clicks on ZedGraph objects that have
    /// active links enabled
    /// </summary>
    /// <param name="sender">The source <see cref="ZedGraphControl"/> object</param>
    /// <param name="pane">The source <see cref="GraphPane" /> in which the click
    /// occurred.
    /// </param>
    /// <param name="source">The source object which was clicked.  This is typically
    /// a type of <see cref="CurveItem" /> if a curve point was clicked, or
    /// a type of <see cref="GraphObj" /> if a graph object was clicked.
    /// </param>
    /// <param name="link">The <see cref="Link" /> object, belonging to
    /// <paramref name="source" />, that contains the link information
    /// </param>
    /// <param name="index">An index value, typically used if a <see cref="CurveItem" />
    /// was clicked, indicating the ordinal value of the actual point that was clicked.
    /// </param>
    /// <returns>
    /// Return true if you have handled the LinkEvent entirely, and you do not
    /// want the <see cref="ZedGraphControl"/> to do any further action.
    /// Return false if ZedGraph should go ahead and process the LinkEvent.
    /// </returns>
    public delegate bool LinkEventHandler( ZedGraphControl sender, GraphPane pane,
      object source, Link link, int index );

    /// <summary>
    /// Subscribe to this event to be able to respond to mouse clicks within linked
    /// objects.
    /// </summary>
    /// <remarks>
    /// Linked objects are typically either <see cref="GraphObj" /> type objects or
    /// <see cref="CurveItem" /> type objects.  These object types can include
    /// hyperlink information allowing for "drill-down" type operation.  
    /// </remarks>
    /// <seealso cref="LinkEventHandler"/>
    /// <seealso cref="Link" />
    /// <seealso cref="CurveItem.Link">CurveItem.Link</seealso>
    /// <seealso cref="GraphObj.Link">GraphObj.Link</seealso>
    // /// <seealso cref="ZedGraph.Web.IsImageMap" />
    [Bindable( true ), Category( "Events" ),
     Description( "Subscribe to be notified when a link-enabled item is clicked" )]
    public event LinkEventHandler LinkEvent;

    /// <summary>
    /// Argument for graph event
    /// </summary>
    public enum GraphEventArg
    {
      Start,
      End,
      Moving,
      Resizing
    }
    /// <summary>
    /// A delegate that allows notification of move/modify graph object
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="pane"></param>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void GraphEventHandler(ZedGraphControl sender, GraphPane pane,
        GraphObj source, GraphEventArg e);

    [Bindable(true), Category("Events"),
        Description("Subscribe to be notifiied when graph object is changed")]
    public event GraphEventHandler GraphEvent;
  #endregion

  #region Mouse Events

    protected override void OnMouseEnter(EventArgs e)
    {
      base.OnMouseEnter(e);
      _mouseInBounds = true;
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      base.OnMouseLeave(e);
      _mouseInBounds = false;

      InvalidateCrossHair(new Point());
    }

    /// <summary>
    /// Handle a MouseDown event in the <see cref="ZedGraphControl" />
    /// </summary>
    /// <param name="sender">A reference to the <see cref="ZedGraphControl" /></param>
    /// <param name="e">A <see cref="MouseEventArgs" /> instance</param>
    protected void ZedGraphControl_MouseDown( object sender, MouseEventArgs e )
    {
      _isPanning = false;
      _isZooming = false;
      _isEditing = false;
      _isSelecting = false;
      _currentPane = null;

      var mousePt = new Point( e.X, e.Y );
      PaneBase foundPane = null;

      // Callback for doubleclick events
      if (_masterPane != null)
      {
        if (e.Clicks > 1 && (DoubleClickEvent?.Invoke(this, e) ?? false))
          return;

        // Provide Callback for MouseDown events
        if (MouseDownEvent?.Invoke(this, e) ?? false)
          return;

        foundPane = _masterPane.FindPane(mousePt);

        // See if this is a splitter pane
        if (foundPane is SplitterPane)
        {
          ((SplitterPane)foundPane).OnMouseDown(this, e);
          _currentPane = foundPane;
          return;
        }
      }
      else if (e.Clicks > 1)
        return;

      var pane = foundPane as GraphPane;

      if (pane == null)
        return;

      // (1) see if the click is within a Linkable object within any GraphPane
      if (e.Button == LinkButtons &&
         (LinkModifierKeys == Keys.None || ModifierKeys == LinkModifierKeys))
      {
        using ( var g = this.CreateGraphics() )
        {
          var scaleFactor = pane.CalcScaleFactor();
          object source;
          Link link;
          int index;
          if ( pane.FindLinkableObject( mousePt, g, scaleFactor, out source, out link, out index ) )
          {
            if ( LinkEvent != null && LinkEvent( this, pane, source, link, index ) )
              return;

            var curve = source as CurveItem;
            var url   = curve != null ? link.MakeCurveItemUrl( pane, curve, index ) : link._url;

            if (url != string.Empty)
            {
              System.Diagnostics.Process.Start( url );
              // linkable objects override any other actions with mouse
              return;
            }
          }
        }
      }

      // (2) Check to see if it's within a Chart Rect
      if (!pane.Chart._rect.Contains( mousePt ))
        return;

      //Rectangle rect = new Rectangle( mousePt, new Size( 1, 1 ) );

      // check if dragging graph
      if (IsEnableGraphEdit)
      {
        int index;

        // check if dragAction is actived
        if (_graphDragState.Obj != null && _graphDragState.Obj.FindNearestEdge(mousePt, pane, out index))
        {
          _graphDragState.State = GraphDragState.DragState.Resize;
          _dragStartPt          = mousePt;
          _dragEndPt            = mousePt;
          _dragIndex            = index;
          _graphDragState.Pane  = pane;
        }
        else
        {
          // select object
          object obj;

          var found = pane.FindNearestObject(mousePt, CreateGraphics(), out obj, out index);

          if (_graphDragState.Obj != obj)
          {
            _graphDragState.Reset();
            Refresh();
          }

          if (found)
          {
            if (obj is GraphObj && ((GraphObj)obj).IsSelectable)
            {
              _graphDragState.Obj = obj as GraphObj;
              _graphDragState.Obj.IsSelected = true;
              _graphDragState.State = GraphDragState.DragState.Select;
              _dragStartPt          = mousePt;
              _dragEndPt            = mousePt;
              _dragIndex            = 0;
              _graphDragState.Pane  = pane;
            }
            else if (obj is CurveItem && ((CurveItem)obj).IsSelectable &&
                     SelectButtons == e.Button &&
                     SelectModifierKeys == Keys.None || SelectModifierKeys == ModifierKeys)
            {
              var o = obj as LineItem;
              if (o != null) // It could be some other curve item type (e.g. OHLCBarItem)
              {
                o.IsSelected = !o.IsSelected;

                if (o.IsSelected)
                {
                  o.Symbol.Type = SymbolType.Circle;
                  o.Symbol.Size = 10;
                }
                else
                  o.Symbol.Type = SymbolType.None;
              }
            }
          }
        }
        _isGraphDragging = _graphDragState.Obj != null;

        // skip below if graph dragging is going
        if (_isGraphDragging)
        {
          Refresh();
          return;
        }
      }

      if ((IsEnableHPan || IsEnableVPan) &&
         ((e.Button == PanButtons  && (PanModifierKeys  == Keys.None || ModifierKeys == PanModifierKeys)) ||
          (e.Button == PanButtons2 && (PanModifierKeys2 == Keys.None || ModifierKeys == PanModifierKeys2))))
      {
        _isPanning = true;
        _dragStartPt = mousePt;
        _currentPane = pane;
        //_zoomState = new ZoomState( _dragPane, ZoomState.StateType.Pan );
        ZoomStateSave(pane, ZoomState.StateType.Pan);
      }
      else if (pane != null && (IsEnableHZoom || IsEnableVZoom) &&
        ((e.Button == ZoomButtons  && (ZoomModifierKeys  == Keys.None || ModifierKeys == ZoomModifierKeys)) ||
        (e.Button  == ZoomButtons2 && (ZoomModifierKeys2 == Keys.None || ModifierKeys == ZoomModifierKeys2))))
      {
        _isZooming   = true;
        _dragStartPt = mousePt;
        _dragEndPt   = mousePt;
        _dragEndPt.Offset(1, 1);
        _currentPane = pane;
        ZoomStateSave(pane, ZoomState.StateType.Zoom);

        DisposeZoomBox();
        _zoomBand    = new ZedGraphZoomBand(this, e.Location);
        _zoomBand.Show();
        Focus();
      }
      //Revision: JCarpenter 10/06
      else if (IsEnableSelection && e.Button == SelectButtons &&
              ((SelectModifierKeys       == Keys.None || ModifierKeys == SelectModifierKeys) ||
               (SelectAppendModifierKeys == Keys.None || ModifierKeys == SelectAppendModifierKeys)))
      {
        _isSelecting = true;
        _dragStartPt = mousePt;
        _dragEndPt   = mousePt;
        _dragEndPt.Offset(1, 1);
        _currentPane    = pane;
      }
      else if ((IsEnableHEdit || IsEnableVEdit) &&
         (e.Button == EditButtons && (EditModifierKeys == Keys.None || ModifierKeys == EditModifierKeys)))
      {
        // find the point that was clicked, and make sure the point list is editable
        // and that it's a primary Y axis (the first Y or Y2 axis)
        if (pane.FindNearestPoint(mousePt, out _dragCurve, out _dragIndex) &&
              _dragCurve.Points is IPointListEdit)
        {
          _isEditing     = true;
          _currentPane      = pane;
          _dragStartPt   = mousePt;
          _dragStartPair = _dragCurve[_dragIndex];
        }
      }
    }

    /// <summary>
    /// Handle a KeyUp event
    /// </summary>
    /// <param name="sender">The <see cref="ZedGraphControl" /> in which the KeyUp occurred.</param>
    /// <param name="e">A <see cref="KeyEventArgs" /> instance.</param>
    protected void ZedGraphControl_KeyUp( object sender, KeyEventArgs e )
    {
      SetCursor();
    }

    /// <summary>
    /// Handle the Key Events so ZedGraph can Escape out of a panning or zooming operation.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ZedGraphControl_KeyDown( object sender, KeyEventArgs e )
    {
      SetCursor();

      if ( e.KeyCode == Keys.Escape )
      {
        if ( _isPanning )
          HandlePanCancel();
        if ( _isZooming )
          HandleZoomCancel();
        if ( _isEditing )
          HandleEditCancel();
        //if ( _isSelecting )
        // Esc always cancels the selection
        HandleSelectionCancel();

        _isZooming = false;
        _isPanning = false;
        _isEditing = false;
        _isSelecting = false;

        Refresh();
      }
    }

    /// <summary>
    /// Handle a MouseUp event in the <see cref="ZedGraphControl" />
    /// </summary>
    /// <param name="sender">A reference to the <see cref="ZedGraphControl" /></param>
    /// <param name="e">A <see cref="MouseEventArgs" /> instance</param>
    protected void ZedGraphControl_MouseUp( object sender, MouseEventArgs e )
    {
      // Provide Callback for MouseUp events
      if (_masterPane != null)
      {
        if ( MouseUpEvent?.Invoke( this, e ) ?? false )
          return;

        if (_currentPane is SplitterPane)
        {
          ((SplitterPane)_currentPane).OnMouseUp(this, e);
          return;
        }

        if ( _currentPane != null )
        {
          // If the MouseUp event occurs, the user is done dragging.
          if (_isZooming)
            HandleZoomFinish(sender, e);
          else if (_isPanning)
            HandlePanFinish();
          else if (_isEditing)
            HandleEditFinish();
          //Revision: JCarpenter 10/06
          else if (_isSelecting)
            HandleSelectionFinish(sender, e);
        }
      }

      if (_isGraphDragging)
        HandleGraphDragFinish(e);

      // Reset the rectangle.
      //dragStartPt = new Rectangle( 0, 0, 0, 0 );
      _currentPane        = null;
      _isZooming       = false;
      _isPanning       = false;
      _isEditing       = false;
      _isSelecting     = false;
      _isGraphDragging = false;

      Cursor.Current = Cursors.Default;
    }

    /// <summary>
    /// Make a string label that corresponds to a user scale value.
    /// </summary>
    /// <param name="axis">The axis from which to obtain the scale value.  This determines
    /// if it's a date value, linear, log, etc.</param>
    /// <param name="val">The value to be made into a label</param>
    /// <param name="iPt">The ordinal position of the value</param>
    /// <param name="isOverrideOrdinal">true to override the ordinal settings of the axis,
    /// and prefer the actual value instead.</param>
    /// <returns>The string label.</returns>
    protected string MakeValueLabel( Axis axis, double val, int iPt, bool isOverrideOrdinal )
    {
      if (axis == null) return "";

      if ( axis.Scale.IsDate || axis.Scale.Type == AxisType.DateAsOrdinal )
        return XDate.ToString( val, PointDateFormat );

      if (!axis.Scale.IsText || axis.Scale.TextLabels == null)
        return axis.Scale.IsAnyOrdinal && axis.Scale.Type != AxisType.LinearAsOrdinal
               && !isOverrideOrdinal
                 ? iPt.ToString(PointValueFormat)
                 : val.ToString(PointValueFormat);

      var i = iPt;
      if ( isOverrideOrdinal )
        i = (int)( val - 0.5 );

      return i >= 0 && i < axis.Scale.TextLabels.Length
               ? axis.Scale.TextLabels[i]
               : (i + 1).ToString();
    }

    /// <summary>
    /// protected method for handling MouseMove events to display tooltips over
    /// individual datapoints.
    /// </summary>
    /// <param name="sender">
    /// A reference to the control that has the MouseMove event.
    /// </param>
    /// <param name="e">
    /// A MouseEventArgs object.
    /// </param>
    protected void ZedGraphControl_MouseMove( object sender, MouseEventArgs e )
    {
      if (_masterPane == null) return;

      var mousePt = e.Location;

      // Provide Callback for MouseMove events
      if (MouseMoveEvent?.Invoke( this, e ) ?? false)
        return;

      //Point tempPt = this.PointToClient( Control.MousePosition );

      var pane = SetCursor( mousePt );

      if (_currentPane is SplitterPane)
      {
        ((SplitterPane)_currentPane).OnMouseMove(this, e);
        return;
      }

      // If the mouse is being dragged,
      // undraw and redraw the rectangle as the mouse moves.

      if (pane == null)
        return;

      if (_isZooming || _isSelecting)
        HandleZoomDrag(mousePt);
      else if (_isPanning)
        HandlePanDrag(mousePt);
      else if (_isEditing)
        HandleEditDrag(mousePt);
      else if (IsShowCursorValues)
        HandleCursorValues(mousePt);
      else if (IsShowPointValues)
        HandlePointValues(mousePt);
      //Revision: JCarpenter 10/06
      else if (_isGraphDragging)
        HandleGraphDrag(mousePt);
      else if (pane is GraphPane)
      {
        // Mouse moved over X or Y axis?
        if (!_mouseHoveredAxisRect.Contains(mousePt))
          using (var g = CreateGraphics())
          {
            /*
            if (pane == null && MasterPane != null)
            {
              // Mouse is not in chart area
              var p = MasterPane.FindPane(mousePt);
              pane = p as GraphPane;
            }
            */
            if (((GraphPane)pane).FindAxis(mousePt, g, out _mouseHoveredAxis,
                                           out _mouseHoveredYAxisIndex, out _mouseHoveredAxisRect))
            {
              if (_mouseHoveredAxis != null && ModifierKeys != Keys.None &&
                  ((PanModifierKeys == Keys.None &&
                    (ModifierKeys == Keys.Shift || ModifierKeys == Keys.Control)) ||
                     ModifierKeys == PanModifierKeys || ModifierKeys == ZoomModifierKeys))
                Cursor = _mouseHoveredAxis is IXAxis ? Cursors.SizeWE : Cursors.SizeNS;
              else if (IsShowPointValues)
                Cursor = Cursors.Cross;
              else
                Cursor = Cursors.Default;
            }
          }
      }

      InvalidateCrossHair(mousePt);

      _currentPane = pane;
    }

    private void InvalidateCrossHair(Point mousePt)
    {
      // Display crosshair
      if (!IsShowCrossHair)
        return;

      if (!_lastCrosshairPoint.IsEmpty)
      {
        if (CrossHairType == CrossHairType.MasterPane)
        {
          // Invalidate old cross-hair location
          Invalidate(new Rectangle(_lastCrosshairPoint.X - 5, 0, 10, ClientSize.Height));
          Invalidate(new Rectangle(0, _lastCrosshairPoint.Y - 5, ClientSize.Width, 10));
        }
        else if (_currentPane is GraphPane)
        {
          // Invalidate old cross-hair location
          var rect = ((GraphPane)_currentPane).Chart.Rect;
          Invalidate(new Rectangle(_lastCrosshairPoint.X - 5, (int)rect.Top, 10, (int)rect.Height));
          Invalidate(new Rectangle((int)rect.Left, _lastCrosshairPoint.Y - 5, (int)rect.Width, 10));
          Invalidate(_lastCrosshairXlabelRect);
          Invalidate(_lastCrosshairYlabelRect);
        }
      }

      _lastCrosshairPoint = mousePt;
    }

    #endregion

    #region Mouse Wheel Zoom Events

    /// <summary>
    /// Handle a MouseWheel event in the <see cref="ZedGraphControl" />
    /// </summary>
    /// <param name="sender">A reference to the <see cref="ZedGraphControl" /></param>
    /// <param name="e">A <see cref="MouseEventArgs" /> instance</param>
    protected void ZedGraphControl_MouseWheel( object sender, MouseEventArgs e )
    {
      if (_currentPane is SplitterPane)
        return;

      //if ((!_isEnableVZoom && !_isEnableHZoom) || !_isEnableWheelZoom || _masterPane == null ||
      //    (_zoomModifierKeys != Keys.None && ModifierKeys != _zoomModifierKeys))
      //  return;
      if ((!IsEnableVZoom && !IsEnableHZoom) || !IsEnableWheelZoom || _masterPane == null)
        return;

      if (e.Delta == 0) return;

      if (_currentPane == null && MasterPane != null)
      {
        if (_mouseHoveredAxis == null)
          _currentPane = MasterPane.FindChartRect(new PointF(e.X, e.Y));
        else
        {
          var p = MasterPane.FindPane(new PointF(e.X, e.Y));
          _currentPane = p as GraphPane;
        }
        if (_currentPane == null)
          return;
      }

      // If currently focused pane is a splitter - ignore
      var pane = _currentPane as GraphPane;
      if (pane == null)
        return;

      //------------------------------------------------------------------------
      // If panning is allowed or the mouse is on some axis, do the panning
      //------------------------------------------------------------------------
      if ((_mouseHoveredAxis != null || 
          (_currentPane.MouseWheelAction & MouseWheelActions.PanHorV) > 0) &&
          (PanModifierKeys == Keys.None || (ModifierKeys & PanModifierKeys) == PanModifierKeys))
      {
        _isPanning = true;
        _dragStartPt = new Point(e.X, e.Y);
        //_zoomState = ZoomStateSave(_dragPane, ZoomState.StateType.Pan);
        var frac = e.Delta * PanStep;

        var pt = _mouseHoveredAxis != null
          ? (_mouseHoveredAxis is IXAxis
            ? new Point(e.X + frac, e.Y)
            : new Point(e.X, e.Y + frac))
          : ((_currentPane.MouseWheelAction & MouseWheelActions.PanH) > 0
            ? new Point(e.X + frac, e.Y)
            : new Point(e.X, e.Y + frac));

        var singleAxis = (PanModifierKeys2 == Keys.None || (ModifierKeys & PanModifierKeys2) == PanModifierKeys2);

        HandlePanDrag(pt, singleAxis ? _mouseHoveredAxis : null, _mouseHoveredYAxisIndex);
        HandlePanFinish();

        if (_mouseHoveredAxis == null)
          _currentPane = null;

        _isPanning  = false;
        return;
      }

      //------------------------------------------------------------------------
      // If zomming is allowed or the mouse is on some axis, do the zoomming
      //------------------------------------------------------------------------
      if ((_mouseHoveredAxis != null ||
          (pane.MouseWheelAction & MouseWheelActions.Zoom) > 0) &&
         (ZoomModifierKeys == Keys.None || (ModifierKeys & ZoomModifierKeys) == ZoomModifierKeys))
      {
        var oldState = ZoomStateSave(pane, ZoomState.StateType.WheelZoom);

        var centerPoint = new PointF(e.X, e.Y);
        var zoomFraction = (1 + (e.Delta < 0 ? 1.0 : -1.0)*ZoomStepFraction);

        /* Alternative calc of zoom fraction:
        var direction = e.Delta < 1 ? -.05f : .05f;
        var scale = m_HoveredAxis.Scale;
        var increment = direction * (scale.Max - scale.Min);
        scale.Min += increment;
        scale.Max -= increment;
        */
        var hZoom = IsEnableHZoom &&
                    ((_mouseHoveredAxis != null && _mouseHoveredAxis is IXAxis) ||
                     (_mouseHoveredAxis == null && (pane.MouseWheelAction & MouseWheelActions.ZoomH) > 0));
        var vZoom = IsEnableVZoom &&
                    ((_mouseHoveredAxis != null && _mouseHoveredAxis is IYAxis) ||
                     (_mouseHoveredAxis == null && (pane.MouseWheelAction & MouseWheelActions.ZoomV) > 0));

        var singleAxis = (ZoomModifierKeys2 == Keys.None || (ModifierKeys & ZoomModifierKeys2) == ZoomModifierKeys2);

        ZoomPane(pane, zoomFraction, centerPoint, _mouseHoveredAxis == null && _isZoomOnMouseCenter,
                 false, hZoom, vZoom, singleAxis ? _mouseHoveredAxis : null, _mouseHoveredYAxisIndex);

        ApplyToAllPanes(pane);

        using (var g = this.CreateGraphics())
        {
          // always AxisChange() the dragPane
          pane.AxisChange(g);

          foreach (var tempPane in _masterPane.PaneList
                  .Where(p => p is GraphPane && p != pane && (_isSynchronizeXAxes || _isSynchronizeYAxes))
                  .Cast<GraphPane>())
            tempPane.AxisChange(g);
        }

        ZoomStatePush(pane);

        // Provide Callback to notify the user of zoom events
        ZoomEvent?.Invoke(this, oldState,
                          new ZoomState(pane, ZoomState.StateType.WheelZoom));
        Refresh();
      }
    }

    /// <summary>
    /// Zoom a specified pane in or out according to the specified zoom fraction.
    /// </summary>
    /// <remarks>
    /// The zoom will occur on the <see cref="XAxis" />, <see cref="YAxis" />, and
    /// <see cref="Y2Axis" /> only if the corresponding flag, <see cref="IsEnableHZoom" /> or
    /// <see cref="IsEnableVZoom" />, is true.  Note that if there are multiple Y or Y2 axes, all of
    /// them will be zoomed.
    /// </remarks>
    /// <param name="pane">The <see cref="GraphPane" /> instance to be zoomed.</param>
    /// <param name="zoomFraction">The fraction by which to zoom, less than 1 to zoom in, greater than
    /// 1 to zoom out.  For example, 0.9 will zoom in such that the scale is 90% of what it was
    /// originally.</param>
    /// <param name="centerPt">The screen position about which the zoom will be centered.  This
    /// value is only used if <see paramref="isZoomOnCenter" /> is true.
    /// </param>
    /// <param name="isZoomOnCenter">true to cause the zoom to be centered on the point
    /// <see paramref="centerPt" />, false to center on the <see cref="Chart.Rect" />.
    /// </param>
    /// <param name="isRefresh">true to force a refresh of the control, false to leave it unrefreshed</param>
    /// <param name="hZoom">Permit horizontal zoom</param>
    /// <param name="vZoom">Permit vertical zoom</param>
    /// <param name="axis">If not null zoom will be done on just given axis, otherwise on all axises</param>
    /// <param name="yAxisIndex">Index of Y axis (only valid if axis param is not null)</param>
    protected void ZoomPane( GraphPane pane, double zoomFraction, PointF centerPt,
          bool isZoomOnCenter, bool isRefresh, bool hZoom = true, bool vZoom = true,
          Axis axis = null, int yAxisIndex = 0)
    {
      ZoomStateSave(pane, ZoomState.StateType.Zoom);
      ZoomStatePush(pane);

      if (axis != null)
      {
        if (axis is IXAxis && hZoom || axis is IYAxis && vZoom)
        { 
          double x, y;
          pane.ReverseTransform(centerPt, axis is X2Axis, axis is Y2Axis, yAxisIndex, out x, out y);
          ZoomScale(axis, zoomFraction, x, isZoomOnCenter);
        }
      }
      else
      {
        double x;
        double x2;
        double[] y;
        double[] y2;

        pane.ReverseTransform( centerPt, out x, out x2, out y, out y2 );

        if ( hZoom )
        {
          ZoomScale( pane.XAxis, zoomFraction,  x,  isZoomOnCenter );
          ZoomScale( pane.X2Axis, zoomFraction, x2, isZoomOnCenter );
        }
        if ( vZoom )
        {
          for ( var i = 0; i < pane.YAxisList.Count; i++ )
            ZoomScale( pane.YAxisList[i], zoomFraction, y[i], isZoomOnCenter );
          for ( var i = 0; i < pane.Y2AxisList.Count; i++ )
            ZoomScale( pane.Y2AxisList[i], zoomFraction, y2[i], isZoomOnCenter );
        }
      }

      if (!isRefresh) return;

      using ( var g = this.CreateGraphics() )
        pane.AxisChange( g );

      SetScroll( hScrollBar1, pane.XAxis, _xScrollRange.Min, _xScrollRange.Max );
      SetScroll( vScrollBar1, pane.YAxis, YScrollRangeList[0].Min, YScrollRangeList[0].Max );

      Refresh();
    }

    /// <summary>
    /// Zoom a specified pane in or out according to the specified zoom fraction.
    /// </summary>
    /// <remarks>
    /// The zoom will occur on the <see cref="XAxis" />, <see cref="YAxis" />, and
    /// <see cref="Y2Axis" /> only if the corresponding flag, <see cref="IsEnableHZoom" /> or
    /// <see cref="IsEnableVZoom" />, is true.  Note that if there are multiple Y or Y2 axes, all of
    /// them will be zoomed.
    /// </remarks>
    /// <param name="pane">The <see cref="GraphPane" /> instance to be zoomed.</param>
    /// <param name="zoomFraction">The fraction by which to zoom, less than 1 to zoom in, greater than
    /// 1 to zoom out.  For example, 0.9 will zoom in such that the scale is 90% of what it was
    /// originally.</param>
    /// <param name="centerPt">The screen position about which the zoom will be centered.  This
    /// value is only used if <see paramref="isZoomOnCenter" /> is true.
    /// </param>
    /// <param name="isZoomOnCenter">true to cause the zoom to be centered on the point
    /// <see paramref="centerPt" />, false to center on the <see cref="Chart.Rect" />.
    /// </param>
    public void ZoomPane( GraphPane pane, double zoomFraction, PointF centerPt, bool isZoomOnCenter )
    {
      ZoomPane( pane, zoomFraction, centerPt, isZoomOnCenter, true );
    }


    /// <summary>
    /// Zoom the specified axis by the specified amount, with the center of the zoom at the
    /// (optionally) specified point.
    /// </summary>
    /// <remarks>
    /// This method is used for MouseWheel zoom operations</remarks>
    /// <param name="axis">The <see cref="Axis" /> to be zoomed.</param>
    /// <param name="zoomFraction">The zoom fraction, less than 1.0 to zoom in, greater than 1.0 to
    /// zoom out.  That is, a value of 0.9 will zoom in such that the scale length is 90% of what
    /// it previously was.</param>
    /// <param name="centerVal">The location for the center of the zoom.  This is only used if
    /// <see paramref="IsZoomOnMouseCenter" /> is true.</param>
    /// <param name="isZoomOnCenter">true if the zoom is to be centered at the
    /// <see paramref="centerVal" /> screen position, false for the zoom to be centered within
    /// the <see cref="Chart.Rect" />.
    /// </param>
    protected void ZoomScale( Axis axis, double zoomFraction, double centerVal, bool isZoomOnCenter )
    {
      if (axis == null || zoomFraction <= 0.01 || zoomFraction >= 100.0) return;
      var scale = axis.Scale;
      /*
        if ( axis.Scale.IsLog )
        {
          double ratio = Math.Sqrt( axis._scale._max / axis._scale._min * zoomFraction );

          if ( !isZoomOnCenter )
            centerVal = Math.Sqrt( axis._scale._max * axis._scale._min );

          axis._scale._min = centerVal / ratio;
          axis._scale._max = centerVal * ratio;
        }
        else
        {
        */
      var minLin = scale.MinLinearized;
      var maxLin = scale.MaxLinearized;
      //var range  = ( maxLin - minLin ) * zoomFraction / 2.0;
      var fact   = zoomFraction == 1.0 ? 0.0 : 1 / zoomFraction - 1;

      if ( !isZoomOnCenter )
        centerVal = ( maxLin + minLin ) / 2.0;

      //if (scale.IsLog)
      //{
      //    // do not zoom in when limit is reached
      //    if (range > 4 && zoomFraction > 1.0)
      //        return;
      //}

      var minSet = minLin - (minLin - centerVal) * fact;
      var maxSet = maxLin - (maxLin - centerVal) * fact;

      //if (minSet < _xScrollRange.Min) minSet = _xScrollRange.Min;
      //if (maxSet > _xScrollRange.Max) maxSet = _xScrollRange.Max;

      scale.MinLinearized = minSet;
      scale.MaxLinearized = maxSet;

      //  }

      scale.MinAuto = false;
      scale.MaxAuto = false;
    }

  #endregion

  #region Pan Events

    private void HandlePanDrag(Point mousePt, Axis axis = null, int yAxisIndex = 0)
    {
      var pane = _currentPane as GraphPane;

      if (pane == null)
        return;

      if (axis != null)
      {
        if (axis is IXAxis && IsEnableHPan)
        {
          double x1 = axis.ReverseTransform(pane, _dragStartPt.X);
          double x2 = axis.ReverseTransform(pane, mousePt.X);
          PanScale(axis, x1, x2);
          SetScroll(this.hScrollBar1, pane.XAxis, _xScrollRange.Min, _xScrollRange.Max);
        }
        if (axis is IXAxis && IsEnableHPan || axis is IYAxis && IsEnableVPan)
        {
          double y1 = axis.ReverseTransform(pane, _dragStartPt.Y);
          double y2 = axis.ReverseTransform(pane, mousePt.Y);
          PanScale(axis, y1, y2);
          SetScroll(this.vScrollBar1, pane.YAxis, YScrollRangeList[0].Min, YScrollRangeList[0].Max);
        }
      }
      else
      {
        double x1, x2, xx1, xx2;
        double[] y1, y2, yy1, yy2;
        //PointF endPoint = mousePt;
        //PointF startPoint = ( (Control)sender ).PointToClient( this.dragRect.Location );

        pane.ReverseTransform( _dragStartPt, out x1, out xx1, out y1, out yy1 );
        pane.ReverseTransform( mousePt, out x2, out xx2, out y2, out yy2 );

        if ( IsEnableHPan )
        {
          PanScale( pane.XAxis, x1, x2 );
          PanScale( pane.X2Axis, xx1, xx2 );
          SetScroll( hScrollBar1, pane.XAxis, _xScrollRange.Min, _xScrollRange.Max );
        }
        if ( IsEnableVPan )
        {
          for ( int i = 0; i < y1.Length; i++ )
            PanScale( pane.YAxisList[i], y1[i], y2[i] );
          for ( int i = 0; i < yy1.Length; i++ )
            PanScale(pane.Y2AxisList[i], yy1[i], yy2[i] );
          this.SetScroll( this.vScrollBar1, pane.YAxis, YScrollRangeList[0].Min,
            YScrollRangeList[0].Max );
        }

        ApplyToAllPanes(pane);
      }

      Refresh();

      _dragStartPt = mousePt;
    }

    private void HandlePanFinish()
    {
      var pane = _currentPane as GraphPane;

      // push the prior saved zoomstate, since the scale ranges have already been changed on
      // the fly during the panning operation
      if (_zoomState == null || !_zoomState.IsChanged(pane)) return;

      //_dragPane.ZoomStack.Push( _zoomState );
      ZoomStatePush( _currentPane );

      // Provide Callback to notify the user of pan events
      this.ZoomEvent?.Invoke( this, _zoomState,
                              new ZoomState( pane, ZoomState.StateType.Pan ) );
      _zoomState = null;
    }

    private void HandlePanCancel()
    {
      var pane = _currentPane as GraphPane;

      if (!_isPanning) return;
      if (_zoomState != null && _zoomState.IsChanged(pane))
      {
        ZoomStateRestore(pane);
        //_zoomState.ApplyState( _dragPane );
        //_zoomState = null;
      }
      _isPanning = false;
      Refresh();

      ZoomStateClear();
    }

    /// <summary>
    /// Handle a panning operation for the specified <see cref="Axis" />.
    /// </summary>
    /// <param name="axis">The <see cref="Axis" /> to be panned</param>
    /// <param name="startVal">The value where the pan started.  The scale range
    /// will be shifted by the difference between <see paramref="startVal" /> and
    /// <see paramref="endVal" />.
    /// </param>
    /// <param name="endVal">The value where the pan ended.  The scale range
    /// will be shifted by the difference between <see paramref="startVal" /> and
    /// <see paramref="endVal" />.
    /// </param>
    protected void PanScale( Axis axis, double startVal, double endVal )
    {
      if (axis == null) return;
      Scale scale = axis.Scale;
      double delta = scale.Linearize( startVal ) - scale.Linearize( endVal );

      /*
      double ddminx = scale._minLinearized - _xScrollRange.Min;
      if (_xScrollRange.Min != 0.0 && ddminx < -delta)
        delta = -ddminx;

      double ddmaxx = -scale._maxLinearized + _xScrollRange.Max;
      if (_xScrollRange.Max != 0.0 && ddmaxx < delta)
        delta = ddmaxx;
      */
      scale.MinLinearized += delta;
      scale.MaxLinearized += delta;

      scale.MinAuto = false;
      scale.MaxAuto = false;

      /*
                if ( axis.Type == AxisType.Log )
                {
                  axis._scale._min *= startVal / endVal;
                  axis._scale._max *= startVal / endVal;
                }
                else
                {
                  axis._scale._min += startVal - endVal;
                  axis._scale._max += startVal - endVal;
                }
        */
    }

  #endregion

  #region Edit Point Events

    private void HandleEditDrag( Point mousePt )
    {
      var pane = _currentPane as GraphPane;
      if (pane == null) return;

      // get the scale values that correspond to the current point
      double curX, curY;
      pane.ReverseTransform( mousePt, _dragCurve.IsX2Axis, _dragCurve.IsY2Axis,
          _dragCurve.YAxisIndex, out curX, out curY );
      double startX, startY;
      pane.ReverseTransform( _dragStartPt, _dragCurve.IsX2Axis, _dragCurve.IsY2Axis,
          _dragCurve.YAxisIndex, out startX, out startY );

      // calculate the new scale values for the point
      PointPair newPt = new PointPair( _dragStartPair );

      Scale xScale = _dragCurve.GetXAxis(pane).Scale;
      if ( IsEnableHEdit )
        newPt.X = xScale.DeLinearize( xScale.Linearize( newPt.X ) +
              xScale.Linearize( curX ) - xScale.Linearize( startX ) );

      Scale yScale = _dragCurve.GetYAxis(pane).Scale;
      if ( IsEnableVEdit )
        newPt.Y = yScale.DeLinearize( yScale.Linearize( newPt.Y ) +
              yScale.Linearize( curY ) - yScale.Linearize( startY ) );

      // save the data back to the point list
      IPointListEdit list = _dragCurve.Points as IPointListEdit;
      if ( list != null )
        list[_dragIndex] = newPt;

      // force a redraw
      Refresh();
    }

    private void HandleEditFinish()
    {
      var pane = _currentPane as GraphPane;
      if (pane == null) return;
      PointEditEvent?.Invoke( this, pane, _dragCurve, _dragIndex );
    }

    private void HandleEditCancel()
    {
      if (!_isEditing) return;
      var list = _dragCurve.Points as IPointListEdit;
      if ( list != null )
        list[_dragIndex] = _dragStartPair;
      _isEditing = false;
      Refresh();
    }

  #endregion

  #region Zoom Events
    /*
    private bool AllowZoomDrag()
    {
      return
        (_dragStartPt != _dragEndPt) &&
        (!_isEnableHZoom || Math.Abs(_dragStartPt.X - _dragEndPt.X) > 1) &&
        (!_isEnableVZoom || Math.Abs(_dragStartPt.Y - _dragEndPt.Y) > 1);
    }
    */

    private void HandleZoomDrag( Point mousePt )
    {
      // Check if the mouse has actually moved.
      var newDragEndPoint = Point.Round(BoundPointToRect(mousePt, _currentPane.Rect));
      if (_dragEndPt == newDragEndPoint)
        return;

      _dragEndPt = newDragEndPoint;
      if (_zoomBand == null || !_zoomBand.Visible) return;

      var pane = _currentPane as GraphPane;

      var coords = CalcZoomRect(pane, _dragStartPt, _dragEndPt);
      _zoomBand.Size = coords.Size;

      /*
      using (Graphics g = Graphics.FromHwnd(this.Handle))
      {
        // Hide the previous rectangle by calling the
        // DrawReversibleFrame method with the same parameters.
        Rectangle rect = this.CalcZoomRect(this._dragStartPt, this._dragEndPt);
        ReversibleFrame.Draw(g, this.BackColor, rect);

        // Bound the zoom to the ChartRect
        _dragEndPt = newDragEndPoint;
        rect = this.CalcZoomRect(this._dragStartPt, this._dragEndPt);

        // Draw the new rectangle by calling DrawReversibleFrame again.
        ReversibleFrame.Draw(g, this.BackColor, rect);
      }
      */
    }

    public double ZoomResolution { get; set; } = float.Epsilon;

    private void DisposeZoomBox()
    {
      if (_zoomBand == null || _zoomBand.Disposing) return;

      _zoomBand.Hide();
      _zoomBand.Dispose();
      _zoomBand = null;
    }

    private void HandleZoomFinish(object sender, MouseEventArgs e)
    {
      DisposeZoomBox();

      var pane = _currentPane as GraphPane;

      var mousePtF = BoundPointToRect(new Point(e.X, e.Y), pane.Chart._rect);

      // Only accept a drag if it covers at least 5 pixels in each direction
      //Point curPt = ( (Control)sender ).PointToScreen( Point.Round( mousePt ) );
      if ((Math.Abs(mousePtF.X - _dragStartPt.X) > 4 || !IsEnableHZoom) &&
          (Math.Abs(mousePtF.Y - _dragStartPt.Y) > 4 || !IsEnableVZoom))
      {
        // Draw the rectangle to be evaluated. Set a dashed frame style
        // using the FrameStyle enumeration.
        //ControlPaint.DrawReversibleFrame( this.dragRect,
        //  this.BackColor, FrameStyle.Dashed );

        double x1, x2, xx1, xx2;
        double[] y1, y2, yy1, yy2;
        //PointF startPoint = ( (Control)sender ).PointToClient( this.dragRect.Location );

        pane.ReverseTransform(_dragStartPt, out x1, out xx1, out y1, out yy1);
        pane.ReverseTransform(mousePtF,     out x2, out xx2, out y2, out yy2);

        var zoomLimitExceeded = false;

        if (IsEnableHZoom)
        {
          if (Math.Abs(x1 - x2) < ZoomResolution || Math.Abs(xx1 - xx2) < ZoomResolution)
            zoomLimitExceeded = true;
        }

        if (IsEnableVZoom && !zoomLimitExceeded)
        {
          zoomLimitExceeded =
            y1.Where((t, i)  => Math.Abs(t - y2[i])  < ZoomResolution).Any() ||
            yy1.Where((t, i) => Math.Abs(t - yy2[i]) < ZoomResolution).Any();
        }

        if (!zoomLimitExceeded)
        {
          ZoomStatePush(pane);
          //ZoomState oldState = _dragPane.ZoomStack.Push( _dragPane,
          //      ZoomState.StateType.Zoom );

          if (IsEnableHZoom)
          {
            pane.XAxis.Scale._min = Math.Min(x1, x2);
            pane.XAxis.Scale.MinAuto = false;
            pane.XAxis.Scale._max = Math.Max(x1, x2);
            pane.XAxis.Scale.MaxAuto = false;

            pane.X2Axis.Scale._min = Math.Min(xx1, xx2);
            pane.X2Axis.Scale.MinAuto = false;
            pane.X2Axis.Scale._max = Math.Max(xx1, xx2);
            pane.X2Axis.Scale.MaxAuto = false;
          }

          if (IsEnableVZoom)
          {
            for (int i = 0; i < y1.Length; i++)
            {
              pane.YAxisList[i].Scale._min = Math.Min(y1[i], y2[i]);
              pane.YAxisList[i].Scale._max = Math.Max(y1[i], y2[i]);
              pane.YAxisList[i].Scale.MinAuto = false;
              pane.YAxisList[i].Scale.MaxAuto = false;
            }
            for (int i = 0; i < yy1.Length; i++)
            {
              pane.Y2AxisList[i].Scale._min = Math.Min(yy1[i], yy2[i]);
              pane.Y2AxisList[i].Scale._max = Math.Max(yy1[i], yy2[i]);
              pane.Y2AxisList[i].Scale.MinAuto = false;
              pane.Y2AxisList[i].Scale.MaxAuto = false;
            }
          }

          SetScroll(hScrollBar1, pane.XAxis, _xScrollRange.Min, _xScrollRange.Max);
          SetScroll(vScrollBar1, pane.YAxis, YScrollRangeList[0].Min, YScrollRangeList[0].Max);

          ApplyToAllPanes(pane);

          // Provide Callback to notify the user of zoom events
          this.ZoomEvent?.Invoke(this, _zoomState, //oldState,
                                  new ZoomState(pane, ZoomState.StateType.Zoom));

          using (var g = this.CreateGraphics())
          {
            // always AxisChange() the dragPane
            pane.AxisChange(g);

            foreach (var p in _masterPane.PaneList
                    .Where(p => p != pane &&
                                p is GraphPane &&
                                (_isSynchronizeXAxes || _isSynchronizeYAxes))
                    .Cast<GraphPane>())
              p.AxisChange(g);
          }
        }
      }

      // refresh anyway
      Refresh();
    }

    private void HandleZoomCancel()
    {
      if ( _isZooming )
      {
        _isZooming = false;
        Refresh();

        ZoomStateClear();
      }

      DisposeZoomBox();
    }

    private PointF BoundPointToRect( Point mousePt, RectangleF rect )
    {
      var newPt = new PointF( mousePt.X, mousePt.Y );

      if ( mousePt.X < rect.X      ) newPt.X = rect.X;
      if ( mousePt.X > rect.Right  ) newPt.X = rect.Right;
      if ( mousePt.Y < rect.Y      ) newPt.Y = rect.Y;
      if ( mousePt.Y > rect.Bottom ) newPt.Y = rect.Bottom;

      return newPt;
    }

    /// <summary>
    /// Caclulates the <see cref="Rectangle"/> between two points for zooming.
    /// </summary>
    /// <param name="point1">The first point.</param>
    /// <param name="point2">The second point.</param>
    /// <returns>The rectangle between the two points.</returns>
    private Rectangle CalcZoomRect(GraphPane pane, Point point1, Point point2)
    {
      var size = new Size(point2.X - point1.X, point2.Y - point1.Y);
      var rect = new Rectangle(point1, size);

      var chartRect = Rectangle.Round(pane.Chart.Rect);

      var chartPt = chartRect.Location;

      if (!this.IsEnableVZoom)
      {
        rect.Y = chartPt.Y;
        rect.Height = chartRect.Height + 1;
      }
      else if (!this.IsEnableHZoom)
      {
        rect.X = chartPt.X;
        rect.Width = chartRect.Width + 1;
      }

      return rect;
    }

  #endregion

  #region Selection Events

    // Revision: JCarpenter 10/06
    /// <summary>
    /// Perform selection on curves within the drag pane, or under the mouse click.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleSelectionFinish( object sender, MouseEventArgs e )
    {
      if ( e.Button != SelectButtons )
      {
        Refresh();
        return;
      }

      var pane = _currentPane as GraphPane;

      if (pane == null)
        return;

      var mousePtF = BoundPointToRect( new Point( e.X, e.Y ), pane.Chart._rect );
      var mousePt  = BoundPointToRect( new Point( e.X, e.Y ), pane.Rect );
      //var curPt    = ( (Control)sender ).PointToScreen( Point.Round( mousePt ) );

      // Only accept a drag if it covers at least 5 pixels in each direction
      //Point curPt = ( (Control)sender ).PointToScreen( Point.Round( mousePt ) );
      if ((Math.Abs(mousePtF.X - _dragStartPt.X) > 4) &&
          (Math.Abs(mousePtF.Y - _dragStartPt.Y) > 4))
      {

        #region New Code to Select on Rubber Band

        double x1, x2, xx1, xx2;
        double[] y1, y2, yy1, yy2;
        //var startPoint = ( (Control)sender ).PointToClient( new Point( Convert.ToInt32( pane.Rect.X ), Convert.ToInt32( this._currentPane.Rect.Y ) ) );

        pane.ReverseTransform( _dragStartPt, out x1, out xx1, out y1, out yy1 );
        pane.ReverseTransform( mousePtF, out x2, out xx2, out y2, out yy2 );

        CurveList objects = new CurveList();

        double left = Math.Min( x1, x2 );
        double right = Math.Max( x1, x2 );

        double top = 0;
        double bottom = 0;

        for ( int i = 0; i < y1.Length; i++ )
        {
          bottom = Math.Min( y1[i], y2[i] );
          top = Math.Max( y1[i], y2[i] );
        }

        for ( int i = 0; i < yy1.Length; i++ )
        {
          bottom = Math.Min( bottom, yy2[i] );
          bottom = Math.Min( yy1[i], bottom );
          top = Math.Max( top, yy2[i] );
          top = Math.Max( yy1[i], top );
        }

        double w = right - left;
        double h = bottom - top;

        RectangleF rF = new RectangleF( (float)left, (float)top, (float)w, (float)h );

        pane.FindContainedObjects( rF, this.CreateGraphics(), out objects );

        if ( SelectModifierKeys == Keys.None || ModifierKeys == SelectAppendModifierKeys )
          Selection.AddToSelection( _masterPane, objects );
        else
          Selection.Select( _masterPane, objects );
        //        this.Select( objects );

        //Graphics g = this.CreateGraphics();
        //this._dragPane.AxisChange( g );
        //g.Dispose();

        #endregion
      }
      else   // It's a single-select
      {
        #region New Code to Single Select

        //Point mousePt = new Point( e.X, e.Y );

        int iPt;
        PaneBase pn;
        object nearestObj;

        using ( var g = this.CreateGraphics() )
        {
          if (MasterPane.FindNearestPaneObject( mousePt, g, out pn, out nearestObj, out iPt))
          {
            if ( nearestObj is CurveItem && iPt >= 0 )
            {
              if ( SelectAppendModifierKeys == Keys.None || ModifierKeys == SelectAppendModifierKeys )
                Selection.AddToSelection( _masterPane, nearestObj as CurveItem );
              else
                Selection.Select( _masterPane, nearestObj as CurveItem );
            }
            else
              Selection.ClearSelection( _masterPane );

            Refresh();
          }
          else
            Selection.ClearSelection( _masterPane );
        }
        #endregion New Code to Single Select
      }

      using ( var g = this.CreateGraphics() )
      {
        // always AxisChange() the dragPane
        pane.AxisChange( g );

        foreach ( var p in _masterPane.PaneList.Where(p => p is GraphPane).Cast<GraphPane>())
        {
          if ( p != pane && ( _isSynchronizeXAxes || _isSynchronizeYAxes ) )
            p.AxisChange( g );
        }
      }

      Refresh();
    }

    private void HandleSelectionCancel()
    {
      _isSelecting = false;

      Selection.ClearSelection( _masterPane );

      Refresh();
    }

  #endregion

  #region Graph Drag Events
    private void HandleGraphDrag(Point mousePt)
    {
      if (_graphDragState.Obj == null) return;
      GraphObj obj = _graphDragState.Obj;
      GraphPane pane = _graphDragState.Pane;

      if (_graphDragState.State == GraphDragState.DragState.Select)
      {
        obj.IsMoving = true;
        _graphDragState.State = GraphDragState.DragState.Move;
      }

      switch (_graphDragState.State)
      {
        case GraphDragState.DragState.Move:
          //obj.Location.X += (mousePt.X - _dragEndPt.X) / _graphDragState.Pane.Rect.Width;
          //obj.Location.Y += (mousePt.Y - _dragEndPt.Y) / _graphDragState.Pane.Rect.Height;
          obj.UpdateLocation(pane, mousePt.X - _dragEndPt.X, mousePt.Y - _dragEndPt.Y);
#if false
          // hack for no width/height polygon
          if (true || obj.Location.Width != 1)
          {
              // convert location to screen coordinate
              PointF ptPix1 = pane.GeneralTransform(obj.Location.X1, obj.Location.Y1,
                      obj.Location.CoordinateFrame);

              PointF ptPix2 = pane.GeneralTransform(obj.Location.X2, obj.Location.Y2,
                      obj.Location.CoordinateFrame);

              // calc new position
              ptPix1.X += (mousePt.X - _dragEndPt.X);
              ptPix1.Y += (mousePt.Y - _dragEndPt.Y);

              ptPix2.X += (mousePt.X - _dragEndPt.X);
              ptPix2.Y += (mousePt.Y - _dragEndPt.Y);

              // convert to user coordinate
              PointD pt1 = pane.GeneralReverseTransform(ptPix1, obj.Location.CoordinateFrame);
              PointD pt2 = pane.GeneralReverseTransform(ptPix2, obj.Location.CoordinateFrame);

              obj.Location.X = pt1.X;
              obj.Location.Y = pt1.Y;
              obj.Location.Width = pt2.X - pt1.X;
              obj.Location.Height = pt2.Y - pt1.Y;
          } 
          else
          {
              obj.Location.X += mousePt.X - _dragEndPt.X;
              obj.Location.Y += mousePt.Y - _dragEndPt.Y;
          }
#endif
          //_graphDragState.startPt = e.Location;
          _dragEndPt = mousePt;

          //this.Text = String.Format("{0} {1} -- {2} {3}",
          //    drag.obj.Location.X, selectedLocation.Y,
          //    drag.obj.Location.Location.X, selectedObj.Location.Y);
          this.GraphEvent?.Invoke(this, pane, obj, GraphEventArg.Moving);

          //this.SetToolTip(String.Format("{0} is moved to {1}/{2}", obj,
          //        mousePt, obj.Location.Rect),
          //        mousePt);
          break;

        case GraphDragState.DragState.Resize:
          obj.ResizeEdge(_dragIndex, mousePt, pane);

          this.GraphEvent?.Invoke(this, pane, obj, GraphEventArg.Resizing);
          break;

        default:
          int index;
          if (_graphDragState.Obj.FindNearestEdge(mousePt, pane, out index))
          {
            //this.Text = String.Format("edge is {0}", index);
            //this.Cursor = Cursors.SizeAll;
          }
          break;
      }

      //zedGraph.Invalidate();

      // force a redraw
      Refresh();

      //return true;
    }

    private void HandleGraphDragFinish(MouseEventArgs e)
    {
      if (_graphDragState.Obj == null) return;

      // do not modify current selected graph 
      _graphDragState.Obj.IsMoving = false;
      _graphDragState.State = GraphDragState.DragState.None;

      if ((e.X != _dragStartPt.X || e.Y != _dragStartPt.Y))
        this.GraphEvent?.Invoke(this, _graphDragState.Pane, _graphDragState.Obj,
                                GraphEventArg.End);

      // force a redraw
      //Refresh();
    }

  #endregion

  #region Cursor and PointValue methods

    private Point HandlePointValues(Point mousePt)
    {
      if (mousePt.Equals(_lastMousePt))
        return mousePt;

      using (var g = this.CreateGraphics())
      {
        int iPt;
        PaneBase foundPane;
        object nearestObj;

        if (_masterPane.FindNearestPaneObject(mousePt, g, out foundPane, out nearestObj, out iPt))
        {
          if (foundPane is GraphPane && nearestObj is CurveItem && iPt >= 0)
          {
            var pane = (GraphPane)foundPane;
            var curve = (CurveItem)nearestObj;
            var label = "";
            // Provide Callback for User to customize the tooltips
            if (PointValueEvent != null)
              label = PointValueEvent(this, pane, curve, iPt);
            else
            {
              if (curve is PieItem)
                label = ((PieItem)curve).Value.ToString(PointValueFormat);
              else
              {
                var pt = curve.Points[iPt];

                if (pt.Tag is string)
                  label = (string)pt.Tag;
                else
                {
                  double xVal, yVal, lowVal;
                  var valueHandler = new ValueHandler(pane, false);
                  if (curve is IBarItem && pane.BarSettings.Base != BarBase.X)
                    valueHandler.GetValues(curve, iPt, out yVal, out lowVal, out xVal);
                  else
                    valueHandler.GetValues(curve, iPt, out xVal, out lowVal, out yVal);

                  var xStr = MakeValueLabel(curve.GetXAxis(pane), xVal, iPt,
                    curve.IsOverrideOrdinal);
                  var yStr = MakeValueLabel(curve.GetYAxis(pane), yVal, iPt,
                    curve.IsOverrideOrdinal);

                  label = $"({xStr}, {yStr})";
                }
              }
            }

            SetToolTip(label, mousePt);
          }
          else
            DisableToolTip();
        }
        else
          DisableToolTip();
      }

      _lastMousePt = mousePt;

      return mousePt;
    }

    private Point HandleCursorValues(Point mousePt)
    {
      var pane = _masterPane.FindPane(mousePt) as GraphPane;
      if (pane != null && pane.Chart._rect.Contains(mousePt))
      {
        // Provide Callback for User to customize the tooltips
        var label = "";
        if (this.CursorValueEvent != null)
          label = this.CursorValueEvent(this, pane, mousePt);
        else
        {
          double x, x2, y, y2;
          pane.ReverseTransform(mousePt, out x, out x2, out y, out y2);
          string xStr = MakeValueLabel(pane.XAxis, x, -1, true);
          string yStr = MakeValueLabel(pane.YAxis, y, -1, true);
          string y2Str = MakeValueLabel(pane.Y2Axis, y2, -1, true);

          label = "( " + xStr + ", " + yStr + ", " + y2Str + " )";
        }

        SetToolTip(label, mousePt);
      }
      else
        DisableToolTip();

      return mousePt;
    }

    /// <summary>
    /// Set the cursor according to the current mouse location.
    /// </summary>
    protected void SetCursor()
    {
      SetCursor(this.PointToClient(Control.MousePosition));
    }

    /// <summary>
    /// Set the cursor according to the current mouse location.
    /// </summary>
    protected PaneBase SetCursor(Point mousePt)
    {
      if (_masterPane == null) return null;

      var focusPane = _masterPane.FindPane(mousePt);

      Cursor    cursor = null;
      GraphPane pane   = null;

      if (focusPane != null)
      {
        if (focusPane is SplitterPane)
        {
          cursor = ((SplitterPane)focusPane).Cursor;
        }
        else if (focusPane is GraphPane)
        {
          if (((GraphPane)focusPane).Chart._rect.Contains(mousePt))
            pane = (GraphPane)focusPane;
        }
      }

      if (pane != null && IsEnableGraphEdit /*&& _isGraphDragging*/)
      {
        int index;
        object obj;

        if ( // current obj is resizing or edge is selected
          _graphDragState.Obj != null
          && (_graphDragState.State == GraphDragState.DragState.Resize
              || _graphDragState.Obj.FindNearestEdge(mousePt, _graphDragState.Pane, out index)))
          cursor = Cursors.SizeAll;
        else if ( // current obj is moving or selected
          (_graphDragState.Obj != null
           && _graphDragState.State == GraphDragState.DragState.Move)
          || (pane.FindNearestObject(mousePt,
                                     this.CreateGraphics(), out obj, out index)
              && obj == _graphDragState.Obj))
          cursor = Cursors.Hand;
        else
          cursor = Cursors.Default;
      }

      if (cursor != null && cursor != Cursors.Default)
      {
        // do nothing, just for ...
      }
      else if ((IsEnableHPan || IsEnableVPan) &&
               (ModifierKeys == PanModifierKeys ||
                (PanModifierKeys2 != Keys.None && ModifierKeys == PanModifierKeys2) || _isPanning) &&
               (pane != null || _isPanning))
        cursor = Cursors.Hand;
      else if ((IsEnableVZoom || IsEnableHZoom) && (pane != null || _isZooming))
        cursor = Cursors.Cross;
      else if (IsEnableSelection && (pane != null || _isSelecting))
        cursor = Cursors.Cross;
      else
        cursor = Cursors.Default;

      //      else if ( isZoomMode || isPanMode )
      //        this.Cursor = Cursors.No;

      if (cursor != null)
        this.Cursor = cursor;

      return focusPane;
    }

  #endregion
  }
}
