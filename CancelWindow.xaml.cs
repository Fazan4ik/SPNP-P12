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


/*private void StartBtnPB_Click(object sender, RoutedEventArgs e)
{
    Clear();
    _cancellationTokenSource = new CancellationTokenSource();
    await RunProgressWaitable(ProgressBar10, _cancellationTokenSource.Token);
    await RunProgressWaitable(ProgressBar11, _cancellationTokenSource.Token, 4);
    await RunProgressWaitable(ProgressBar12, _cancellationTokenSource.Token, 2);
}*/



namespace SPNP_P12
{
    /// <summary>
    /// Interaction logic for CancelWindow.xaml
    /// </summary>
    public partial class CancelWindow : Window
    {
        private CancellationTokenSource cancellationTokenSource = null!;
        private int taskCountActive;
        private readonly object countLocker = new();

        public CancelWindow()
        {
            InitializeComponent();
        }


        #region CW1
        private void StartBtn1_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource = new CancellationTokenSource();
            taskCountActive = 0;
            RunProgressCancellable(progressBar10, cancellationTokenSource.Token);
            RunProgressCancellable(progressBar11, cancellationTokenSource.Token, 4);
            RunProgressCancellable(progressBar12, cancellationTokenSource.Token, 2);
        }
        private void StopBtn1_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }
        private async void RunProgressCancellable(ProgressBar progressBar, CancellationToken token, int time = 3)
        {
            progressBar.Value = 0;
            lock (countLocker) { taskCountActive++; }
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    progressBar.Value += 10;
                    await Task.Delay(1000 * time / 10);
                    token.ThrowIfCancellationRequested();
                }
            }
            catch (OperationCanceledException)
            {
                if (progressBar.Value < 100)
                {
                    for (int i = (int)progressBar.Value / 10; i > 0; i--)
                    {
                        progressBar.Foreground = Brushes.Yellow;
                        progressBar.Value -= 10;
                        await Task.Delay(1000 * time / 10);
                    }
                }
            }
            finally
            {
                progressBar.Foreground = Brushes.Green;
                bool isLast;
                lock (countLocker)
                {
                    isLast = (--taskCountActive) == 0;
                }
                if (isLast)
                {
                    MessageBox.Show("Кінець");
                }
            }
        }
        #endregion
        #region HW2
        private async void StartBtn2_Click(object sender, RoutedEventArgs e)
        {
            ClearProgressBar(progressBar20, progressBar21, progressBar22);
            cancellationTokenSource = new CancellationTokenSource();
            await RunProgressWaitable(progressBar20, cancellationTokenSource.Token);
            await RunProgressWaitable(progressBar21, cancellationTokenSource.Token, 4);
            await RunProgressWaitable(progressBar22, cancellationTokenSource.Token, 2);
        }
        private void StopBtn2_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }
        private async Task RunProgressWaitable(ProgressBar progressBar, CancellationToken token, int time = 3)
        {
            progressBar.Value = 0;
            for (int i = 0; i < 10; i++)
            {
                progressBar.Value += 10;
                await Task.Delay(1000 * time / 10);

                if (token.IsCancellationRequested)
                {
                    break;
                }
            }
        }
        #endregion
        #region HW2
        private async void StartBtn3_Click(object sender, RoutedEventArgs e)
        {
            ClearProgressBar(progressBar30, progressBar31, progressBar32);
            cancellationTokenSource = new CancellationTokenSource();
            await Task.Run(() => RunProgress(progressBar30, cancellationTokenSource.Token));
            await Task.Run(() => RunProgress(progressBar31, cancellationTokenSource.Token, 4));
            await Task.Run(() => RunProgress(progressBar32, cancellationTokenSource.Token, 2));
        }
        private void StopBtn3_Click(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource?.Cancel();
        }
        private async void RunProgress(ProgressBar progressBar, CancellationToken token, int time = 3)
        {
            for (int i = 0; i < 10; i++)
            {
                Dispatcher.Invoke(() => progressBar.Value += 10);
                await Task.Delay(1000 * time / 10);

                if (token.IsCancellationRequested)
                {
                    break;
                }
            }
        }
        #endregion
        private void ClearProgressBar(params ProgressBar[] progressBars)
        {
            foreach (var pb in progressBars)
            {
                pb.Value = 0;
            }
        }
    }
}
