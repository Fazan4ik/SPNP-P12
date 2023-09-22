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
    /// Interaction logic for TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        public TaskWindow()
        {
            InitializeComponent();
        }

        #region CW_19_09_2023
        private void DemoButton1_Click(object sender, RoutedEventArgs e)
        {
            Task task = new Task(demo1);
            task.Start();

            Task task2 = Task.Run(demo1);
        }
        private void demo1()
        {
            Dispatcher.Invoke(() => LogTextBlock.Text += "demo1 starts\n");
            Thread.Sleep(1000);
            Dispatcher.Invoke(() => LogTextBlock.Text += "demo1 finishes\n");
        }

        private async void DemoButton2_Click(object sender, RoutedEventArgs e)
        {
            /*Task<String> task = demo2();
            String str = await demo2();
            LogTextBlock.Text += $"demo2 result: {str}\n";*/

            Task<String> task1 = demo2();
            String res = $"demo2-1 result: {await task1}\n";
            LogTextBlock.Text += res;
            Task<String> task2 = demo2();
            res = $"demo2-2 result: {await task2}\n";
            LogTextBlock.Text += res;


        }
        private async Task<String> demo2()
        {
            LogTextBlock.Text += $"demo2 starts\n";
            await Task.Delay(1000);
            return "Done";
        }
        #endregion

       
    }
}
