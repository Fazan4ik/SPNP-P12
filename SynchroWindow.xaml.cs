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
        private const int Months = 12;
        private static Random r = new Random();
        public SynchroWindow()
        {
            InitializeComponent();
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            sum = 100;
            LogtextBlock.Text = String.Empty;
            threadCount = Months;
            float randPercent, avgPercent = 0;
            for (int i = 0; i < threadCount; i++)
            {
               new Thread(AddPercent5).Start(new MonthData { Month = i + 1});
               /* randPercent = (float)Math.Round(r.NextDouble() * 20, 1);
                avgPercent += randPercent;
                new Thread(AddPercentHW).Start(new MonthData { Month = i + 1, Percent = randPercent });*/
            }
           // LogtextBlock.Text += $"Avg percent: {(avgPercent / Months):F2}\n";

        }

        #region CW1

        private Semaphore semaphore = new Semaphore(4, 4);
        private void AddPercent5(object? data)
        {
            var monthData = data as MonthData;
            semaphore.WaitOne();
            Thread.Sleep(1000);
            double localSum;
            localSum = sum = sum * 1.1;

            semaphore.Release();
            Dispatcher.Invoke(() =>
            {
                LogtextBlock.Text += $"{monthData?.Month} --- {localSum:F2}\n";
            });
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
        private object countLocker = new();


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
            public float Percent { get; set; }
        }
        #endregion

        #region HW1
        private object mainLocker = new(); 
        private void AddPercentHW(object? data)
        {
            var months = data as MonthData;
            Thread.Sleep(200);
            double localSum;
            lock (mainLocker) 
            { 
                localSum = sum += (sum * months!.Percent / 100); 
            }
            Dispatcher.Invoke(() =>
            {
                LogtextBlock.Text += $"{months?.Month}) --- {localSum:F2} --- (+{months?.Percent}%)\n";
            });
            bool isLast = false;  
            lock (mainLocker)  
            {
                threadCount--;
                Thread.Sleep(1);
                if (threadCount == 0)  
                {
                    isLast = true;
                }
            }
            if (isLast)
            {
                Dispatcher.Invoke(() =>
                {
                    LogtextBlock.Text += $"--------\n\nresult = {sum:F2}";
                });
            }
        }
        #endregion

    }
}
