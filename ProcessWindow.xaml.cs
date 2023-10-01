using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for ProcessWindow.xaml
    /// </summary>
    public partial class ProcessWindow : Window
    {
        private static Mutex? mutex;
        private const String mutexName = "SPNP_MUTEX";

        public ProcessWindow()
        {
            // CheckPreviousLaunch();
            InitializeComponent();
        }


        private void CheckPreviousLaunch()
        {
            try
            {
                mutex = Mutex.OpenExisting(mutexName);
            }
            catch { }
            if (mutex != null)
            {
                if (!mutex.WaitOne(1))
                {
                    String message = "Запущено інший екземпляр вікна";
                    MessageBox.Show(message);
                    throw new ApplicationException(message);
                }
            }
            else
            {
                mutex = new Mutex(true, mutexName);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            mutex?.ReleaseMutex();
            /* mutex?.ReleaseMutex();
             mutex?.Dispose();*/
        }


        private void ShowProcesses_Click(object sender, RoutedEventArgs e)
        {
            Process[] processes = Process.GetProcesses();
            // ProcTextBlock.Text = "";
            String prevName = "";
            TreeViewItem? item = null;
            ProcTreeView.Items.Clear();
            foreach (Process process in processes.OrderBy(p => p.ProcessName))
            {
                // ProcTextBlock.Text += String.Format("{0} {1}\n",process.Id,process.ProcessName);
                if (prevName != process.ProcessName)
                {
                    prevName = process.ProcessName;
                    item = new TreeViewItem() { Header = prevName };
                    ProcTreeView.Items.Add(item);
                }
                var subItem = new TreeViewItem()
                {
                    Header = String.Format("{0} {1}\n", process.Id, process.ProcessName),
                    Tag = process
                };
                subItem.MouseDoubleClick += TreeViewItem_MouseDoubleClick;
                item?.Items.Add(subItem);


            }
        }

        private void TreeViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeViewItem item)
            {
                String message = "";
                if (item.Tag is Process process)
                {
                    foreach (ProcessThread thread in process.Threads)
                    {
                        message += thread.Id + "\r\n";
                    }
                }
                else
                {
                    message = "No process in tag";
                }
                MessageBox.Show(message);
            }
        }


        private Process? notepadProcess;
        private void StartNotePad_Click(object sender, RoutedEventArgs e)
        {
            notepadProcess ??= Process.Start("notepad.exe");
        }

        private void StopNotePad_Click(object sender, RoutedEventArgs e)
        {
            notepadProcess?.CloseMainWindow();
            notepadProcess?.Kill(true);
            notepadProcess?.WaitForExit();
            notepadProcess?.Dispose();
            notepadProcess = null;
        }

        private void StartEdit_Click(object sender, RoutedEventArgs e)
        {
            String dir = AppContext.BaseDirectory;
            int binPosition = dir.IndexOf("bin");
            String projectRoot = dir[..binPosition];
            // MessageBox.Show(projectRoot);
            notepadProcess ??= Process.Start(
                "notepad.exe",
                $"{projectRoot}ProcessWindow.xaml.cs");
        }

        Process? browserProcess;
        private void StartBrowser_Click(object sender, RoutedEventArgs e)
        {
            String filename = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            if (System.IO.File.Exists(filename))
            {
                browserProcess ??= Process.Start(filename, "-url youtube.com");
            }
            else
            {
                MessageBox.Show("Браузер Google не встановленний");
            }

        }

        private Process? calculatorProcess;

        private void StartCalculator_Click(object sender, RoutedEventArgs e)
        {
            calculatorProcess ??= Process.Start("calc");
        }

        private void StopCalculator_Click(object sender, RoutedEventArgs e)
        {
            var temp = Process.GetProcessesByName("CalculatorApp");
            foreach (var item in temp)
            {
                item.Kill();
            }
            calculatorProcess = null;
        }

    }
}
