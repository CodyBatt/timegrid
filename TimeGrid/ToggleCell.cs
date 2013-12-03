using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TimeGrid
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TimeRestrictionsControl"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:TimeRestrictionsControl;assembly=TimeRestrictionsControl"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ToggleCell/>
    ///
    /// </summary>
    
    [TemplatePart(Name="PART_CellBorder", Type = typeof(Border))]
    public class ToggleCell : Control
    {
        static ToggleCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleCell), new FrameworkPropertyMetadata(typeof(ToggleCell)));
        }

        Border CellBorder { get; set; }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CellBorder = Template.FindName("PART_CellBorder",this) as Border;
            MouseEnter += TimeCell_MouseEnter;
            MouseLeave += TimeCell_MouseLeave;
        }

        void TimeCell_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        void TimeCell_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOver", true);
        }

        public static readonly DependencyProperty SelectedProperty =
            DependencyProperty.Register("Selected", typeof (bool), typeof (ToggleCell), new PropertyMetadata(SelectedChanged));

        public bool Selected
        {
            get { return (bool) GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        static void SelectedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var thisObj = obj as ToggleCell;
            if (thisObj == null) return;
            thisObj.UpdateVisual();
        }

        public static readonly DependencyProperty OnBackgroundBrushProperty =
            DependencyProperty.Register("OnBackgroundBrush", typeof (Brush), typeof (ToggleCell), new PropertyMetadata(new SolidColorBrush(Colors.Green)));

        public Brush OnBackgroundBrush
        {
            get { return (Brush) GetValue(OnBackgroundBrushProperty); }
            set { SetValue(OnBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty OffBackgroundBrushProperty =
            DependencyProperty.Register("OffBackgroundBrush", typeof (Brush), typeof (ToggleCell), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public Brush OffBackgroundBrush
        {
            get { return (Brush) GetValue(OffBackgroundBrushProperty); }
            set { SetValue(OffBackgroundBrushProperty, value); }
        }

        void UpdateVisual()
        {
            CellBorder.Background = Selected ? OnBackgroundBrush : OffBackgroundBrush;
        }

    }
}
