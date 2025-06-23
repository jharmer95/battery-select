using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BatterySelect
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (PowerQuery.IsAcPower())
            {
                Console.WriteLine("AC power detected.");
                IconBattery.Source = new BitmapImage(new Uri("icons/plug_icon.png", UriKind.Relative));
            }
            else
            {
                Console.WriteLine("DC power detected.");
                IconBattery.Source = new BitmapImage(new Uri("icons/battery_icon.png", UriKind.Relative));
            }

            switch (PowerQuery.CurrentMode)
            {
                case PowerQuery.PowerMode.Efficiency:
                    BtnEfficiency.Background = Brushes.LightGreen;
                    BtnBalanced.Background = Brushes.LightGray;
                    BtnPerformance.Background = Brushes.LightGray;
                    break;

                case PowerQuery.PowerMode.Balanced:
                    BtnEfficiency.Background = Brushes.LightGray;
                    BtnBalanced.Background = Brushes.LightBlue;
                    BtnPerformance.Background = Brushes.LightGray;
                    break;

                case PowerQuery.PowerMode.Performance:
                    BtnEfficiency.Background = Brushes.LightGray;
                    BtnBalanced.Background = Brushes.LightGray;
                    BtnPerformance.Background = Brushes.LightCoral;
                    break;

                default:
                    throw new InvalidOperationException("Unknown PowerMode");
            }
        }

        private void BtnEfficiency_Click(object sender, RoutedEventArgs e)
        {
            if (PowerQuery.CurrentMode == PowerQuery.PowerMode.Efficiency)
            {
                Console.WriteLine("Already in Efficiency mode.");
                return;
            }

            Console.Write("Switching to Efficiency mode... ");
            PowerQuery.CurrentMode = PowerQuery.PowerMode.Efficiency;
            UpdateDisplay();
            Console.WriteLine("Done!");
        }

        private void BtnBalanced_Click(object sender, RoutedEventArgs e)
        {
            if (PowerQuery.CurrentMode == PowerQuery.PowerMode.Balanced)
            {
                Console.WriteLine("Already in Balanced mode.");
                return;
            }

            Console.Write("Switching to Balanced mode... ");
            PowerQuery.CurrentMode = PowerQuery.PowerMode.Balanced;
            UpdateDisplay();
            Console.WriteLine("Done!");
        }

        private void BtnPerformance_Click(object sender, RoutedEventArgs e)
        {
            if (PowerQuery.CurrentMode == PowerQuery.PowerMode.Performance)
            {
                Console.WriteLine("Already in Performance mode.");
                return;
            }

            Console.Write("Switching to Performance mode... ");
            PowerQuery.CurrentMode = PowerQuery.PowerMode.Performance;
            UpdateDisplay();
            Console.WriteLine("Done!");
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateDisplay();
        }
    }
}