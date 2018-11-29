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

namespace Presto.SWCamp.Lyrics
{
    /// <summary>
    /// LyricsLargeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricsLargeWindow : Window
    {
        public LyricsLargeWindow()
        {
            InitializeComponent();
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed) DragMove();
        }
    }
}
