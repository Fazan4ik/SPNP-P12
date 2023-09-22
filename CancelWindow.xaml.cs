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
    /// Interaction logic for CancelWindow.xaml
    /// </summary>
    public partial class CancelWindow : Window
    {
        private CancellationTokenSource _cancellationTokenSource;
        private int activeTaskCount;
        private readonly object countLocker = new();

        public CancelWindow()
        {
            InitializeComponent();
            _cancellationTokenSource = null!;
        }

        #region CW1
        private async void StartBtn1_Click(object sender, RoutedEventArgs e)
        {
            /*RunProgress(ProgressBar1);
            RunProgress(ProgressBar2, 4);
            RunProgress(ProgressBar3, 2);*/

            /*await RunProgressWaitable(ProgressBar1);
            await RunProgressWaitable(ProgressBar2, 4);
            await RunProgressWaitable(ProgressBar3, 2);*/
            Clear();
            _cancellationTokenSource = new CancellationTokenSource();
            RunProgressCancellable(ProgressBar1, _cancellationTokenSource.Token);
            RunProgressCancellable(ProgressBar2, _cancellationTokenSource.Token, 4);
            RunProgressCancellable(ProgressBar3, _cancellationTokenSource.Token, 2);
        }

        #region HW1
        private async void StartBtn2_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            _cancellationTokenSource = new CancellationTokenSource();
            await RunProgressWaitable(ProgressBar1, _cancellationTokenSource.Token);
            await RunProgressWaitable(ProgressBar2, _cancellationTokenSource.Token, 4);
            await RunProgressWaitable(ProgressBar3, _cancellationTokenSource.Token, 2);
        }
        #endregion

        private void StopBtn1_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();

        }
        public void Clear()
        {
            ProgressBar1.Value = 0;
            ProgressBar2.Value = 0;
            ProgressBar3.Value = 0;
        }

        private async void RunProgress(ProgressBar progressBar, int time = 3)
        {
            progressBar.Value = 0;
            for (int i = 0; i < 10; i++)
            {
                progressBar.Value += 10;
                await Task.Delay(1000 * time / 10);
            }
        }
        private async Task RunProgressWaitable(ProgressBar progressBar, CancellationToken token, int time = 3)
        {
            progressBar.Foreground = Brushes.ForestGreen;
            progressBar.Value = 0;
            for (int i = 0; i < 10; i++)
            {
                progressBar.Value += 10;
                await Task.Delay(1000 * time / 10);
            }
        }
        private async Task RunProgressCancellable(
                            ProgressBar progressBar,
                            CancellationToken cancellationToken, int time = 3)
        {
            progressBar.Value = 0;
            lock (countLocker) { activeTaskCount++; }
            progressBar.Foreground = Brushes.ForestGreen;
            try
            {
                activeTaskCount++;
                for (int i = 0; i < 10; i++)
                {
                    progressBar.Value += 10;
                    await Task.Delay(1000 * time / 10);
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                if (progressBar.Value < 100)
                {
                    progressBar.Foreground = Brushes.Pink;
                    while (progressBar.Value > 0)
                    {
                        progressBar.Value -= 10;
                        await Task.Delay(500);
                    }
                }
                return;
            }
            finally
            {
                progressBar.Foreground = Brushes.Black;
                bool isLast;
                lock (countLocker)
                {
                    activeTaskCount--;
                    isLast = activeTaskCount == 0;

                }
                if (isLast)
                {
                    MessageBox.Show("Done");
                }
            }

        }
        #endregion


    }
}
