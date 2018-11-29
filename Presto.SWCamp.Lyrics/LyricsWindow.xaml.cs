using Presto.SDK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using System.Windows.Threading;

namespace Presto.SWCamp.Lyrics
{
    /// <summary>
    /// LyricsWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricsWindow : Window
    {
        LyricsManager lyricsManager;
        AlbumartManager albumartManager;
        private bool isShow = false;
        public LyricsWindow()
        {
            InitializeComponent();
            PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged;
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height;

            lyricsManager = new LyricsManager();
            albumartManager = new AlbumartManager();

        }

        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {
            this.Show();
            lyricsManager.StreamChanged();
            //GC 강제 실행
            System.GC.Collect(2, GCCollectionMode.Forced);
            System.GC.WaitForFullGCComplete();            

            bool check = false;
            if (PrestoSDK.PrestoService.Player.CurrentMusic.Album.Picture == null)
            {
                if (PrestoSDK.PrestoService.Player.CurrentMusic.Title != null)
                {
                    string path = string.Empty;
                    if(PrestoSDK.PrestoService.Player.CurrentMusic.Artist.Name == null)
                    {
                        path = albumartManager.Run(PrestoSDK.PrestoService.Player.CurrentMusic.Title);
                    }
                    else
                    {
                        path = albumartManager.Run(PrestoSDK.PrestoService.Player.CurrentMusic.Title + " " + PrestoSDK.PrestoService.Player.CurrentMusic.Artist.Name);
                    }
                    if (!(path == null || path == string.Empty))
                    {
                        check = true;
                        albumArtImage.ImageSource = new BitmapImage(new Uri(path));
                    }
                }
            } else
            {
                check = true;
                albumArtImage.ImageSource = new BitmapImage(new Uri(PrestoSDK.PrestoService.Player.CurrentMusic.Album.Picture));
            }
            if (!check)
                albumArtImage.ImageSource = null;
        }
        
        private void Timer_Tick(object sender, EventArgs e)
        {
            double cur = PrestoSDK.PrestoService.Player.Position;
        }

        /* 창 드래그 */
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed) DragMove();
        }

        
        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            PrestoSDK.PrestoService.Player.PlayPrevious();
        }

        private void PlayOrPauseButton_Click(object sender, RoutedEventArgs e)
        {
            switch (PrestoSDK.PrestoService.Player.PlaybackState)
            {
                case Common.PlaybackState.Playing:
                  
                    PrestoSDK.PrestoService.Player.Pause();
                    break;
                case Common.PlaybackState.Paused:
                    PrestoSDK.PrestoService.Player.Resume();
                    break;
                case Common.PlaybackState.Stopped:
                     PrestoSDK.PrestoService.Player.Play();
                    break;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            PrestoSDK.PrestoService.Player.PlayNext();
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
        }

        /* 우상단 버튼 관리 
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void TopMostButton_Click(object sender, RoutedEventArgs e)
        {
            //항상 위에 표시 활성화
            if (this.Topmost == false)
            {
                topMostButton.Color = (Color)ColorConverter.ConvertFromString("#000000");
                this.Topmost = true;
            }
            else
            {
                topMostButton.Color = (Color)ColorConverter.ConvertFromString("#FF74C105");
                this.Topmost = false;
            }
        }
        */

    }
}
