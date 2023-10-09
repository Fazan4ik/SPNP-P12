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
    /// Interaction logic for ChainingWindow.xaml
    /// </summary>
    public partial class ChainingWindow : Window
    {
        public ChainingWindow()
        {
            InitializeComponent();
        }


        private CancellationTokenSource cts = null!;
        private void StartBtn1_Click(object sender, RoutedEventArgs e)
        {
            if (cts is null || cts.IsCancellationRequested)
                cts = new CancellationTokenSource();
            else
                return;

            var task10 =
                ShowProgress(progressBar10, cts.Token)
                .ContinueWith(task10 =>
                    ShowProgress(progressBar11, cts.Token)
                   .ContinueWith(task11 =>
                        ShowProgress(progressBar12, cts.Token)));  

            var task20 =
                ShowProgress(progressBar20, cts.Token)  
                .ContinueWith(task20 =>
                    ShowProgress(progressBar21, cts.Token)
                   .ContinueWith(task21 =>
                        ShowProgress(progressBar22, cts.Token)));
        }

        private void StopBtn1_Click(object sender, RoutedEventArgs e)
        {
            cts?.Cancel();
        }

        private async void StartBtn2_Click(object sender, RoutedEventArgs e)
        {
            if (cts is null || cts.IsCancellationRequested)  
                cts = new CancellationTokenSource();
            else
                return;
            var task10 = ShowProgress(progressBar10, cts.Token);
            var task20 = ShowProgress(progressBar20, cts.Token);
            await task10; var task11 = ShowProgress(progressBar11, cts.Token);
            await task20; var task21 = ShowProgress(progressBar21, cts.Token);
            await task11; var task12 = ShowProgress(progressBar12, cts.Token);
            await task21; var task22 = ShowProgress(progressBar22, cts.Token);
        }

        private void StopBtn2_Click(object sender, RoutedEventArgs e)
        {
            cts?.Cancel();
        }

        private async Task ShowProgress(ProgressBar progressBar, CancellationToken token)
        {
            int delay = 100;
            if (progressBar == progressBar10) delay = 300;
            if (progressBar == progressBar11) delay = 200;
            if (progressBar == progressBar12) delay = 100;

            if (progressBar == progressBar20) delay = 100;
            if (progressBar == progressBar21) delay = 200;
            if (progressBar == progressBar22) delay = 300;

            for (int i = 0; i <= 10; i++)
            {
                await Task.Delay(delay);
                Dispatcher.Invoke(() => progressBar.Value = i * 10);
                if (token.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        private async void StartBtn3_Click(object sender, RoutedEventArgs e)
        {
            String str = "";
            var text = await AddHello(str)
                .ContinueWith(task =>
                {
                    String res = task.Result;
                    Dispatcher.Invoke(() => LogTextBlock.Text = res);
                    return AddWorld(res);  // taskW                    
                })
                .Unwrap()  // зняти одну "обгортку" Task<>, без неї task2 - Task<taskW> = Task<Task<String>>
                .ContinueWith(task2 =>  // а з нею - task2 - Task<String> (без однієї "обгортки")
                {
                    String res = task2.Result;
                    Dispatcher.Invoke(() => LogTextBlock.Text = res);
                    return AddExclamation(res);
                })
                .Unwrap()
                .ContinueWith(task =>
                    Dispatcher.Invoke(() => LogTextBlock.Text = task.Result));
            MessageBox.Show(text);
        }

        private void StopBtn3_Click(object sender, RoutedEventArgs e)
        {

        }

        async Task<String> AddHello(String str)
        {
            await Task.Delay(1000);
            return str + " Hello ";
        }
        async Task<String> AddWorld(String str)
        {
            await Task.Delay(1000);
            return str + " World ";
        }
        async Task<String> AddExclamation(String str)
        {
            await Task.Delay(1000);
            return str + " !!! ";
        }

    }
}
