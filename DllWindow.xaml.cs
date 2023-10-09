using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Interaction logic for DllWindow.xaml
    /// </summary>
    public partial class DllWindow : Window
    {

        [DllImport("user32.dll")]
        public
            static
            extern
            int MessageBoxA(
                IntPtr hWnd,
                String lpText,
                String lpCaption,
                uint uType
            );

        public DllWindow()
        {
            InitializeComponent();
        }

        private void AlertButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxA(
                IntPtr.Zero,
                "Повідомелння",
                "Заголовок",
                0x40
                );
        }

        [DllImport("Kernel32.dll", EntryPoint = "Beep")]
        public static extern bool Sound(uint frequency, uint duration);

        private async Task PlaySoundAsync(uint frequency, uint duration)
        {
            await Task.Run(() => Sound(frequency, duration));
        }

        private async void SoundButton_Click(object sender, RoutedEventArgs e)
        {
            await PlaySoundAsync(440, 1000);
        }

        private async void SoundButton2_Click(object sender, RoutedEventArgs e)
        {
            await PlaySoundAsync(540, 1000);
        }

        private void SoundButton3_Click(object sender, RoutedEventArgs e)
        {
            Sound(640, 1000);
        }

        private void SoundButton4_Click(object sender, RoutedEventArgs e)
        {
            Sound(740, 1000);
        }



        /*  HANDLE CreateThread(
      [in, optional]  LPSECURITY_ATTRIBUTES   lpThreadAttributes,
      [in]            SIZE_T                  dwStackSize,
      [in]            LPTHREAD_START_ROUTINE  lpStartAddress,
      [in, optional]  __drv_aliasesMem LPVOID lpParameter,
      [in]            DWORD                   dwCreationFlags,
      [out, optional] LPDWORD                 lpThreadId
    );
*/
        public delegate void ThreadMethod();

        [DllImport("Kernel32.dll", EntryPoint = "CreateThread")]
        public static extern
            IntPtr NewThread(
                IntPtr lpThreadAttributes,
                uint dwStackSize,
                ThreadMethod lpStartAddress,
                IntPtr lpParameter,
                uint dwCreationFlags,
                IntPtr lpThreadId
            );
        public void ErrorMessage()
        {
            MessageBoxA(
                IntPtr.Zero,
                "Повідомелння про помилку",
                "Місце виникнення",
                0x14
                );
            methodHandle.Free();

        }


        GCHandle methodHandle;
        private void ThreadButton_Click(object sender, RoutedEventArgs e)
        {
            var method = new ThreadMethod(ErrorMessage);
            methodHandle = GCHandle.Alloc(method);
            NewThread(IntPtr.Zero, 0, method, IntPtr.Zero, 0, IntPtr.Zero);
        }

    }
}
