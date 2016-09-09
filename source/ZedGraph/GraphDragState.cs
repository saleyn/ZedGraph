using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZedGraph
{
    public class GraphDragState
    {
        public enum DragState
        {
            None,
            Select,
            Move,
            Resize,
        };

        private DragState _state;
        private GraphObj _obj;
        private GraphPane _pane;

        public DragState State
        {
            get { return _state; }
            set { _state = value; }
        }

        public GraphObj Obj
        {
            get { return _obj; }
            set { _obj = value; }
        }

        public GraphPane Pane
        {
            get { return _pane; }
            set { _pane = value; }
        }

        public GraphDragState()
        {
            Reset();
        }

        public void Reset()
        {
            if (_obj != null)
            {
                _obj.IsSelected = false;
                _obj.IsMoving = false;
            }

            _obj = null;
            _state = DragState.None;
            _pane = null;
        }
    }
}
