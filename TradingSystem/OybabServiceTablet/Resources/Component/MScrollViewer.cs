using Oybab.Res.View.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Oybab.ServiceTablet.Resources.Component
{
    internal sealed class MScrollViewer : ScrollViewer
    {
        private ScrollBar _scrollbar;
        public MScrollViewer()
        {
            this.ApplyTemplate();
            _scrollbar = this.Template.FindName("PART_VerticalScrollBar", this) as ScrollBar;
            _scrollbar.Tag = "";
            _timer2.Interval = new TimeSpan(0, 0, 0, 1, 500);
            _timer2.Tick += _timer2_Tick;
        }

        private void _timer2_Tick(object sender, EventArgs e)
        {
            if (IsRun)
            {
                if (IsDisplay && _scrollbar.Tag.ToString() != "0")
                {
                    IsDisplay = false;
                    _scrollbar.Tag = "0";
                }
            }


            _timer2.Stop();

            IsRun = false;
           
        }

        internal void Stop()
        {
            if (_dragScrollTimer != null)
            {
                _dragScrollTimer.Tick -= TickDragScroll;
                _dragScrollTimer.Stop();
                _dragScrollTimer = null;
            }


            if (!IsRun)
            {
                IsRun = true;
                _timer2.Start();
               
            }
        }


        private double _friction = DEFAULT_FRICTION;
        public double Friction
        {
            get
            {
                return _friction;
            }
            set
            {
                _friction = Math.Min(Math.Max(value, MINIMUM_FRICTION), MAXIMUM_FRICTION);
            }
        }

        private double _previousHorizontaloffset;
        private double _previousverticaloffset;
        private Point _previousPreviousPoint;
        private Point _previousPoint;
        private Point _currentPoint;

        private bool _mouseDown = false;
        private bool _isDragging = false;

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            CancelDrag(PreviousVelocity(true));
        }
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            IsNeedStop = false;
            INeedStopCount = 0;
            MouseDownTime = DateTime.Now;



            IsIgnore = false;

            if (IsRun)
            {
                IsRun = false;
                _timer2.Stop();
            }


            if (!IsDisplay)
            {
                IsDisplay = true;

                if (_scrollbar.Tag.ToString() != "1")
                {
                    _scrollbar.Tag = "1";
                }
            }


            IsTryFixedMomentum = false;
            PointList.Clear();
            _currentPoint = FirstPoint = _previousPoint = _previousPreviousPoint = e.GetPosition(this);
            PointList.Add(_currentPoint);
            Momentum = new Vector(0, 0);
            BeginDrag();

            if (_dragScrollTimer != null)
            {
                e.Handled = true;
            }
        }

        private Point FirstPoint;
        private bool IsIgnore;
        private bool IsDisplay;
        private DispatcherTimer _timer2 = new DispatcherTimer();
        private bool IsRun;


        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {

            if (_mouseDown)
            {
                _currentPoint = e.GetPosition(this);


                if (!IsIgnore)
                {
                    double first = 0;
                    double current = 0;

                    if (this.HorizontalScrollBarVisibility == ScrollBarVisibility.Hidden)
                    {
                        first = Math.Abs(FirstPoint.X);
                        current = Math.Abs(_currentPoint.X);
                    }
                    else
                    {
                        first = Math.Abs(FirstPoint.Y);
                        current = Math.Abs(_currentPoint.Y);
                    }


                    if (Math.Abs(first - current) > 30)
                    {
                        IsIgnore = true;
                    }

                }
            }


            if (_mouseDown && !_isDragging)
            {
                _isDragging = true;
                DragScroll();

            }
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            CancelDrag(PreviousVelocity(true));

            if (IsIgnore)
            {
                ButtonBase element = e.OriginalSource as ButtonBase;

                if (null != element && element.IsPressed)
                {
                    ViewModelBase ViewModel = element.CommandParameter as ViewModelBase;
                    ViewModel.IsIgnore = true;
                }

            }
            else if ((DateTime.Now - MouseDownTime).TotalSeconds >= 1)
            {
                ButtonBase element = e.OriginalSource as ButtonBase;

                if (null != element && element.IsPressed)
                {
                    ViewModelBase ViewModel = element.CommandParameter as ViewModelBase;
                    ViewModel.IsLong = true;
                }
            }

        }


        private DateTime MouseDownTime = DateTime.Now;

        private void BeginDrag()
        {
            _mouseDown = true;
        }

        private void CancelDrag(Vector velocityToUse)
        {
            if (_isDragging)
                Momentum = velocityToUse;
            _isDragging = false;
            _mouseDown = false;
        }

        private void DragScroll()
        {
            if (_dragScrollTimer == null)
            {
                _dragScrollTimer = new DispatcherTimer(DispatcherPriority.Render);
                _dragScrollTimer.Tick += TickDragScroll;
                _dragScrollTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)DRAG_POLLING_INTERVAL);
                _dragScrollTimer.Start();
            }
        }




        private bool IsTryFixedMomentum = false;
        private void TickDragScroll(object sender, EventArgs e)
        {
            if (_isDragging)
            {


                GeneralTransform generalTransform = this.TransformToVisual(this);
                Point childToParentCoordinates = generalTransform.Transform(new Point(0, 0));
                Rect bounds = new Rect(childToParentCoordinates, this.RenderSize);

                if (bounds.Contains(_currentPoint))
                {
                    PerformScroll(PreviousVelocity(false));
                }





                if (!_mouseDown)
                {
                    CancelDrag(Velocity);
                }
                PointList.Add(_currentPoint);


                return;


            }
            else if (Momentum.Length > 0 && !IsNeedStop)
            {
                Momentum *= (1.0 - _friction / 4.0);
                PerformScroll(Momentum);

                if (!IsTryFixedMomentum)
                    IsTryFixedMomentum = true;

            }
           
            else
            {

               
                if (_dragScrollTimer != null)
                {
                    _dragScrollTimer.Tick -= TickDragScroll;
                    _dragScrollTimer.Stop();
                    _dragScrollTimer = null;
                }


                if (!IsRun)
                {
                    IsRun = true;
                    _timer2.Start();
                }



            }
        }

        private int _lastHorizontalOffset = 0;
        private int _lastVerticalOffset = 0;
        private int INeedStopCount = 0;
        private bool IsNeedStop = false;
        private void PerformScroll(Vector displacement)
        {
            var verticalOffset = Math.Max(0.0, VerticalOffset - displacement.Y);

            ScrollToVerticalOffset(verticalOffset);

            var horizontalOffset = Math.Max(0.0, HorizontalOffset - displacement.X);

            ScrollToHorizontalOffset(horizontalOffset);

            int _tempHorizontalOffset = (int)horizontalOffset;
            int _tempVerticalOffset = (int)verticalOffset;
            if (_lastHorizontalOffset == _tempHorizontalOffset && _lastVerticalOffset == _tempVerticalOffset)
            {
                if (INeedStopCount >= 50)
                {
                    IsNeedStop = true;
                }
                else
                {
                    ++INeedStopCount;
                }
            }
            else
            {
                INeedStopCount = 0;
                _lastHorizontalOffset = _tempHorizontalOffset;
                _lastVerticalOffset = _tempVerticalOffset;
            }

            _previousHorizontaloffset = this.HorizontalOffset;
            _previousverticaloffset = this.VerticalOffset;

            
        }
       

        private const double DRAG_POLLING_INTERVAL = 10; // milliseconds
        private DispatcherTimer _dragScrollTimer = null;
        private const double DEFAULT_FRICTION = 0.2;
        private const double MINIMUM_FRICTION = 0.0;
        private const double MAXIMUM_FRICTION = 1.0;


        private Vector Momentum { get; set; }
        private Vector Velocity
        {
            get
            {
               
                if (PointList.Count >= 1)
                {
                    _previousPoint = PointList.ElementAtOrDefault(PointList.Count() - 1);
                }

                return new Vector(_currentPoint.X - _previousPoint.X, _currentPoint.Y - _previousPoint.Y);
            }
        }

        private Vector PreviousVelocity(bool IsDistinct)
        {
            List<Point> pointList = null;
            if (IsDistinct)
            {
                pointList = PointList.Skip(Math.Max(0, PointList.Count() - 10)).Distinct().ToList();
            }
            else
            {
                pointList = PointList.Skip(Math.Max(0, PointList.Count() - 10)).ToList();
            }
            if (pointList.Count >= 1)
            {
                _previousPoint = pointList.ElementAtOrDefault(pointList.Count() - 1);
            }

            if (pointList.Count >= 2)
            {
                _previousPreviousPoint = pointList.ElementAtOrDefault(pointList.Count() - 2);
            }

            return new Vector(_previousPoint.X - _previousPreviousPoint.X, _previousPoint.Y - _previousPreviousPoint.Y);

        }


        List<Point> PointList = new List<Point>();



        private sealed class Vector
        {
            public double Length { get { return Math.Sqrt(X * X + Y * Y); } }
            public Vector(double x, double y)
            {
                X = x;
                Y = y;
            }

            public static Vector operator *(Vector vector, double scalar)
            {
                return new Vector(vector.X * scalar, vector.Y * scalar);
            }

            public double X { get; set; }
            public double Y { get; set; }

        }
    }
}
