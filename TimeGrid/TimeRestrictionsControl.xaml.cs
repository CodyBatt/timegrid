using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


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
                    WeekGrid.Children.Add(_hourCells[day, hour]);
                }
            }
            WeekGrid.DataContext = this;
        }

        void TimeGrid_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Toggle the state of the ToggleCell that was clicked.
            // Get the cell with
            var element = sender as UIElement;
            if (element == null) return;
            var row = Grid.GetRow(element);
            var col = Grid.GetColumn(element);
            // Set enabled to false
            _hourCells[row - 1, col - 1].Selected = !_hourCells[row - 1, col - 1].Selected;
        }
        
        public static readonly DependencyProperty TimeRestrictionsProperty =
            DependencyProperty.Register("TimeRestrictions", typeof (WeeklyTimePeriod), typeof (TimeRestrictionsControl), new PropertyMetadata(TimeRestrictionsChanged));
        public WeeklyTimePeriod TimeRestrictions
        {
            get { return (WeeklyTimePeriod) GetValue(TimeRestrictionsProperty); }
            set { SetValue(TimeRestrictionsProperty, value); }
        }

        public static readonly DependencyProperty MinCellSizeProperty =
            DependencyProperty.Register("MinCellSize", typeof (int), typeof (TimeRestrictionsControl), new PropertyMetadata(20));
        public int MinCellSize
        {
            get { return (int) GetValue(MinCellSizeProperty); }
            set { SetValue(MinCellSizeProperty, value); }
        }

        static void TimeRestrictionsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var thisObj = obj as TimeRestrictionsControl;
            if (thisObj == null) return;
            thisObj.BindCells();
        }

        void BindCells()
        {
            // Set the datacontext of the Border to the 
            // hourlytimeperiod from the TimeRestrictions.Hours property
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
    }

}
