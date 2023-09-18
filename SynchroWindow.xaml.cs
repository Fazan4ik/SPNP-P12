using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPNP_P12
{
    /// <summary>
    /// Interaction logic for SynchroWindow.xaml
    /// </summary>
    public partial class SynchroWindow : Window
    {
        private double sum;
        private int threadCount;
        public SynchroWindow()
        {
            InitializeComponent();
        }
        #region 1    
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            sum = 100;
            LogtextBlock.Text = String.Empty;
            threadCount = 12;
            for(int i = 0; i < threadCount; i++)
            {
                new Thread(AddPercent4).Start(new MonthData { Month = i + 1});
            }
        }

        private void AddPercent()
        {
            Thread.Sleep(200);
            double localSum = sum;
            localSum *= 1.1;
            sum = localSum;
            Dispatcher.Invoke(() =>
            {
                LogtextBlock.Text += $"{sum}\n";
            });
        }

        private void AddPercent1()
        {
            double localSum = sum;
            Thread.Sleep(200);
            localSum *= 1.1;
            sum = localSum;
            Dispatcher.Invoke(() =>
            {
                LogtextBlock.Text += $"{sum}\n";
            });
        }


        private object sumLocker = new();

        private void AddPercent2()
        {
            lock (sumLocker)
            {
                double localSum = sum;
                Thread.Sleep(200);
                localSum *= 1.1;
                sum = localSum;
                Dispatcher.Invoke(() =>
                {
                    LogtextBlock.Text += $"{sum}\n";
                });
            }
        }

        private void AddPercent3()
        {
            Thread.Sleep(200);
            double localSum;
            lock (sumLocker)
            {
                localSum = 
                    sum = sum * 1.1;
            }
            Dispatcher.Invoke(() =>
            {
                LogtextBlock.Text += $"{localSum}\n";
            });
        }

        private void AddPercent4(object? data)
        {
            var monthData = data as MonthData;
            // Thread.Sleep(200);
            double localSum;
            lock (sumLocker)
            {
                localSum =
                    sum = sum * 1.1;
            }
            Dispatcher.Invoke(() =>
            {
                LogtextBlock.Text += $"{monthData?.Month} --- {localSum}\n";
            });
            threadCount--;
            if (threadCount == 0)
            {
                Dispatcher.Invoke(() =>
                {
                    LogtextBlock.Text += $"--------\n\nresult = {sum}";
                });
            }
        }
        class MonthData
        {
            public int Month { get; set; }
        }

        #endregion
    }
}
