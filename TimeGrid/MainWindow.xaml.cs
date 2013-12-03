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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Restrictions = new WeeklyTimePeriod(new bool[7, 24]
            {
                {
                    true, false, true, false, true, false, true, false, true, false, true, false, true, false, true, false,
                    true, false, true, false, true, false, true, false,
                },
                {
                    false, true, false, true, false, true, false, true, false, true, false, true, false, true, false, true,
                    false, true, false, true, false, true, false, true,
                },
                {
                    true, false, true, false, true, false, true, false, true, false, true, false, true, false, true, false,
                    true, false, true, false, true, false, true, false,
                },
                {
                    false, true, false, true, false, true, false, true, false, true, false, true, false, true, false, true,
                    false, true, false, true, false, true, false, true,
                },
                {
                    true, false, true, false, true, false, true, false, true, false, true, false, true, false, true, false,
                    true, false, true, false, true, false, true, false,
                },
                {
                    false, true, false, true, false, true, false, true, false, true, false, true, false, true, false, true,
                    false, true, false, true, false, true, false, true,
                },
                {
                    true, false, true, false, true, false, true, false, true, false, true, false, true, false, true, false,
                    true, false, true, false, true, false, true, false,
                },
            });

            DataContext = this;
        }

        public static readonly DependencyProperty RestrictionsProperty =
            DependencyProperty.Register("Restrictions", typeof (WeeklyTimePeriod), typeof (MainWindow), new PropertyMetadata(default(WeeklyTimePeriod)));

        public WeeklyTimePeriod Restrictions
        {
            get { return (WeeklyTimePeriod) GetValue(RestrictionsProperty); }
            set { SetValue(RestrictionsProperty, value); }
        }

        private void DumpClicked(object sender, RoutedEventArgs e)
        {
            Console.AppendText(Restrictions.ToString());
            Console.AppendText(Environment.NewLine);
        }
    }
}
