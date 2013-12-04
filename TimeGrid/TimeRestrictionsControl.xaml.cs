using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace TimeGrid
{
    /// <summary>
    /// Interaction logic for TimeRestrictionsControl.xaml
    /// </summary>
    public partial class TimeRestrictionsControl
    {
        private const int DaysPerWeek = 7;
        private const int HoursPerDay = 24;
        readonly ToggleCell[,] _hourCells = new ToggleCell[DaysPerWeek,HoursPerDay];

        public TimeRestrictionsControl()
        {
            InitializeComponent();
            for (var day = 0; day < DaysPerWeek; day++)
            {
                for (var hour = 0; hour < HoursPerDay; hour++)
                {
                    _hourCells[day,hour] = new ToggleCell();
                    Grid.SetColumn(_hourCells[day,hour], hour + 1);
                    Grid.SetRow(_hourCells[day,hour], day + 1);
                    _hourCells[day, hour].PreviewMouseLeftButtonDown += TimeGrid_PreviewMouseLeftButtonDown;
                    _hourCells[day, hour].PreviewMouseLeftButtonUp += TimeGrid_PreviewMouseLeftButtonUp;
                    _hourCells[day, hour].PreviewMouseMove += TimeGrid_PreviewMouseMove;
                    WeekGrid.Children.Add(_hourCells[day, hour]);
                }
            }
            WeekGrid.DataContext = this;            
        }

        #region Drag Support
        private ToggleCell _dragSource;
        private Point? _mouseDragStart;
        private CellRegion _dragRegion;

        void TimeGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as ToggleCell;
            if (element == null) return;
            _dragSource = element;
            _dragSource.Selected = !_dragSource.Selected;
            _mouseDragStart = e.GetPosition(this);         
        }

        void TimeGrid_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = sender as ToggleCell;
            if (element == null) return;

            if (_dragSource != null)
            {
                if(_dragRegion != null) _dragRegion.ApplySelection(_hourCells, _dragSource.Selected);
                EndDragging();
            }
        }

        void TimeGrid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var target = sender as ToggleCell;
            if (_mouseDragStart != null && _dragSource != target) // Mouse is down
            {
                var newRegion = new CellRegion(_hourCells,_dragSource, target);
                if (_dragRegion != null)
                {
                    if (newRegion.TopLeft == _dragRegion.TopLeft && newRegion.BottomRight == _dragRegion.BottomRight) return;
                    _dragRegion.SetVisualStates(_hourCells, "NoDrag");
                }
                _dragRegion = newRegion;
                _dragRegion.SetVisualStates(_hourCells, _dragSource.Selected ? "DragOn" : "DragOff");
            }
        }

        void EndDragging()
        {
            _mouseDragStart = null;
            _dragSource = null;
            _dragRegion = null;
        }
        #endregion

        #region TimeRestrictions
        public static readonly DependencyProperty TimeRestrictionsProperty =
            DependencyProperty.Register("TimeRestrictions", typeof (WeeklyTimePeriod), typeof (TimeRestrictionsControl), new PropertyMetadata(TimeRestrictionsChanged));
        public WeeklyTimePeriod TimeRestrictions
        {
            get { return (WeeklyTimePeriod) GetValue(TimeRestrictionsProperty); }
            set { SetValue(TimeRestrictionsProperty, value); }
        }

        static void TimeRestrictionsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var thisObj = obj as TimeRestrictionsControl;
            if (thisObj == null) return;
            thisObj.BindCells();
        }
        #endregion

        #region MinCellSize
        public static readonly DependencyProperty MinCellSizeProperty =
            DependencyProperty.Register("MinCellSize", typeof (int), typeof (TimeRestrictionsControl), new PropertyMetadata(20));
        public int MinCellSize
        {
            get { return (int) GetValue(MinCellSizeProperty); }
            set { SetValue(MinCellSizeProperty, value); }
        }
        #endregion

        void BindCells()
        {
            const int firstHourOffset = 0;
            for (var day = 0; day < DaysPerWeek; day++)
            {
                for (var hour = 0; hour < HoursPerDay; hour++)
                {
                    var boundHour = day * HoursPerDay + ((hour + firstHourOffset) % 24);
                    var binding = new Binding("Enabled")
                    {
                        Mode= BindingMode.TwoWay,
                        Source = TimeRestrictions.Hours[boundHour]
                    };
                    BindingOperations.ClearBinding(_hourCells[day, hour], ToggleCell.SelectedProperty);
                    BindingOperations.SetBinding(_hourCells[day, hour],ToggleCell.SelectedProperty, binding);
                }
            }
        }

        class CellRegion
        {
            public CellRegion(ToggleCell[,] cells, ToggleCell first, ToggleCell second)
            {
                var firstx = Grid.GetColumn(first) - 1;
                var secondx = Grid.GetColumn(second) - 1;
                var firsty = Grid.GetRow(first) - 1;
                var secondy = Grid.GetRow(second) - 1;
                var minx = Math.Min(firstx, secondx);
                var miny = Math.Min(firsty, secondy);
                var maxx = Math.Max(firstx, secondx);
                var maxy = Math.Max(firsty, secondy);
                TopLeft = cells[miny, minx];
                BottomRight = cells[maxy, maxx];
            }

            public ToggleCell TopLeft { get; private set; }
            public ToggleCell BottomRight { get; private set; }

            public void SetVisualStates(ToggleCell[,] cells, string state)
            {
                ApplyActionToRegion(cells, (cell) => VisualStateManager.GoToState(cell, state, true));
            }

            public void ApplySelection(ToggleCell[,] cells, bool select)
            {
                ApplyActionToRegion(cells, (cell) =>
                {
                    VisualStateManager.GoToState(cell, "NoDrag", true);
                    VisualStateManager.GoToState(cell, "Normal", true);
                    cell.Selected = select;
                });
            }

            void ApplyActionToRegion(ToggleCell[,] cells, Action<ToggleCell> action)
            {
                var startx = Grid.GetColumn(TopLeft) - 1;
                var endx = Grid.GetColumn(BottomRight) - 1;
                var starty = Grid.GetRow(TopLeft) - 1;
                var endy = Grid.GetRow(BottomRight) - 1;
                for (var x = startx; x <= endx; x++)
                {
                    for (var y = starty; y <= endy; y++)
                    {
                        action(cells[y, x]);
                    }
                }
            }
        }
    }
}
