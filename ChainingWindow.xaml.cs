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

        private void StartBtn1_Click(object sender, RoutedEventArgs e)
        {
            var task10 =
            showProgress(ProgressBar1)
                .ContinueWith(task => showProgress(ProgressBar2)
                .ContinueWith(task => showProgress(ProgressBar3)))
                ;

            showProgress(ProgressBar11)
                .ContinueWith(task => showProgress(ProgressBar22)
                .ContinueWith(task => showProgress(ProgressBar33)))
                ;
        }

        private void StopBtn1_Click(object sender, RoutedEventArgs e)
        {

        }

        private async Task showProgress(ProgressBar progressBar)
        {
            int delay = 100;
            if (progressBar == ProgressBar1) delay = 300;
            if (progressBar == ProgressBar2) delay = 200;
            if (progressBar == ProgressBar3) delay = 100;
            if (progressBar == ProgressBar11) delay = 100;
            if (progressBar == ProgressBar22) delay = 200;
            if (progressBar == ProgressBar33) delay = 300;



            for (int i = 0; i <= 10; i++)
            {
                await Task.Delay(delay);
                Dispatcher.Invoke(() => progressBar.Value = i * 10);
            }
        }

        private async void StartBtn2_Click(object sender, RoutedEventArgs e)
        {
            var task1 = showProgress(ProgressBar1);
            var task2 = showProgress(ProgressBar2);
            await task1; var task11 = showProgress(ProgressBar11);
            await task2; var task22 = showProgress(ProgressBar22);
        }

        private void StopBtn2_Click(object sender, RoutedEventArgs e)
        {

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
