using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPNP_P12
{
    /// <summary>
    /// Interaction logic for ThreadingWindow.xaml
    /// </summary>
    public partial class ThreadingWindow : Window
    {
        private static Mutex? mutex;
        private static string mutexName = "TW_MUTEX";

        public ThreadingWindow()
        {
            CheckPreviousLaunch();
            InitializeComponent();
        }
        private void CheckPreviousLaunch()
        {
            try { mutex = Mutex.OpenExisting(mutexName); } catch { } 

            if (mutex is null)
            {
                mutex = new Mutex(true, mutexName);
            }
            else if (!mutex.WaitOne(1))
            {
                MessageBox.Show("Экземпляр окна уже запущен!");
                throw new ApplicationException();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mutex?.ReleaseMutex();
        }


        #region 1
        private void StartButton1_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                ProgressBar1.Value = i * 10;
                Thread.Sleep(300);
            }
            ProgressBar1.Value = 100;
        }

        private void StopButton1_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 2
        private void StartButton2_Click(object sender, RoutedEventArgs e)
        {
            new Thread(IncrementProgress2).Start();
        }

        private void StopButton2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void IncrementProgress2()
        {
            for (int i = 0; i < 10; i++)
            {
                ProgressBar2.Value = i * 10;
                Thread.Sleep(300);
            }
            ProgressBar2.Value = 100;
        }
        #endregion
        #region 3   
        private bool isStopped3;
        private void StartButton3_Click(object sender, RoutedEventArgs e)
        {
            new Thread(IncrementProgress3).Start();
            isStopped3 = false;

        }

        private void StopButton3_Click(object sender, RoutedEventArgs e)
        {
            isStopped3 = true;
        }

        private void IncrementProgress3()
        {
            for (int i = 0; i <= 10 && !isStopped3; i++)
            {
                this.Dispatcher.Invoke(
                    () => ProgressBar3.Value = i * 10
                );
                Thread.Sleep(300);
            }
        }
        #endregion

        #region 4
        private bool isStopped4 { get; set; }
        private Thread? thread4;
        private void StartButton4_Click(object sender, RoutedEventArgs e)
        {
            if (thread4 == null)
            {
                isStopped4 = false;
                thread4 = new Thread(IncrementProgress4);
                thread4.Start();

                StartButton4.IsEnabled = false;
                StopButton4.IsEnabled = true;
            }

        }
        private void StopButton4_Click(object sender, RoutedEventArgs e)
        {
            stopHandle();
        }
        private void stopHandle()
        {
            isStopped4 = true;
            thread4 = null;
            StartButton4.IsEnabled = true;
            StopButton4.IsEnabled = false;

        }
        private void IncrementProgress4()
        {
            for (int i = 0; i <= 10 && !isStopped4; i++)
            {
                this.Dispatcher.Invoke(
                    () => ProgressBar4.Value = i * 10
                );
                Thread.Sleep(300);
            }
            thread4 = null;
            /* this.Dispatcher.Invoke(() =>
             {
                 StartButton4.IsEnabled = true;
                 StopButton4.IsEnabled = false;
             });*/
            this.Dispatcher.Invoke(stopHandle);
        }
        #endregion

        #region 5
        private Thread? thread5;
        CancellationTokenSource cts;
        private void StartButton5_Click(object sender, RoutedEventArgs e)
        {
            int workTime = Convert.ToInt32(WorktimeTextBox.Text);
            thread5 = new Thread(IncrementProgress5);
            cts = new();
            thread5.Start(new ThreadData5 { WorkTime = workTime, CancelToken = cts.Token });
        }

        private void StopButton5_Click(object sender, RoutedEventArgs e)
        {
            cts?.Cancel();
        }

        private void IncrementProgress5(object? parameter)
        {
            if(parameter is ThreadData5 data)
            {
                for (int i = 0; i <= 100; i++)
                {
                    this.Dispatcher.Invoke(
                        () => ProgressBar5.Value = i 
                    );
                    Thread.Sleep(100 * data.WorkTime);
                    if (data.CancelToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Thread 5 started with invalid argument");
            }
        }

        class ThreadData5
        {
            public int WorkTime { get; set; }

            public CancellationToken CancelToken { get; set; }
        }

        #endregion

        #region HW

        private Thread? hwThread1, hwThread2, hwThread3;
        private CancellationTokenSource? hwCts = null;
        private Random r = new Random();

       


        private void BtnHwStart_Click(object sender, RoutedEventArgs e)
        {
            if (hwCts is not null)
            {
                hwCts.Cancel();
            }

            int workTime1 = r.Next(1, 10);
            int workTime2 = r.Next(1, 10);
            int workTime3 = r.Next(1, 10);

            hwTextBox1.Text = workTime1.ToString();
            hwTextBox2.Text = workTime2.ToString();
            hwTextBox3.Text = workTime3.ToString();

            hwCts = new CancellationTokenSource();
            hwThread1 = new Thread(IncrementHWProgressBar1);
            hwThread2 = new Thread(IncrementHWProgressBar1);
            hwThread3 = new Thread(IncrementHWProgressBar1);

            hwThread1.Start(new ThreadProgress
            {
                WorkTime = workTime1,
                ProgressBar = hwProgressBar1,
                CancellToken = hwCts.Token
            });
            hwThread2.Start(new ThreadProgress
            {
                WorkTime = workTime2,
                ProgressBar = hwProgressBar2,
                CancellToken = hwCts.Token
            });
            hwThread3.Start(new ThreadProgress
            {
                WorkTime = workTime3,
                ProgressBar = hwProgressBar3,
                CancellToken = hwCts.Token
            });
        }

        private void BtnHwStop_Click(object sender, RoutedEventArgs e)
        {
            hwCts?.Cancel();

        }

        private void IncrementHWProgressBar1(object? parameter)
        {
            if (parameter is ThreadProgress threadProgress)
            {
                try
                {
                    for (int i = 0; i <= 10; i++)
                    {
                        Dispatcher.Invoke(() => threadProgress.ProgressBar.Value = i * 10);
                        Thread.Sleep(100 * threadProgress.WorkTime);
                        threadProgress.CancellToken.ThrowIfCancellationRequested();
                    }
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        class ThreadProgress
        {
            public int WorkTime { get; set; }
            public ProgressBar ProgressBar { get; set; } = null!;
            public CancellationToken CancellToken { get; set; }
        }
        #endregion
    }
}
