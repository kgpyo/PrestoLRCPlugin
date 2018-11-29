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
        AlbumartManager albumartManager;
        private LyricsLargeWindow _lyricsLarge = null;
        public bool IsThisWindowShow { get; set; } = false;
        public LyricsWindow()
        {
            InitializeComponent();
            PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged;
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height;
            this.Topmost = true;
            albumartManager = new AlbumartManager();

        }

        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {
            //GC 강제 실행
            System.GC.Collect(2, GCCollectionMode.Forced);
            System.GC.WaitForFullGCComplete();

            if (IsThisWindowShow == true || _lyricsLarge.IsActive == false)
            {
                this.IsThisWindowShow = true;
                this.Show();
            }

            bool isCorrectSearchAlbumArt = false;
            if (PrestoSDK.PrestoService.Player.CurrentMusic.Album.Picture == null)
            {
                string title = string.Empty;
                title = PrestoSDK.PrestoService.Player.CurrentMusic.Title;
                if (title == null)
                {
                    string CurrentMusic = PrestoSDK.PrestoService.Player.CurrentMusic.Path;
                    title = Path.GetFileNameWithoutExtension(CurrentMusic);
                    PrestoSDK.PrestoService.Player.CurrentMusic.Title = title;
                }
                if(title != string.Empty)
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
                        isCorrectSearchAlbumArt = true;
                        albumArtImage.ImageSource = new BitmapImage(new Uri(path));
                    }
                }
            } else
            {
                isCorrectSearchAlbumArt = true;
                albumArtImage.ImageSource = new BitmapImage(new Uri(PrestoSDK.PrestoService.Player.CurrentMusic.Album.Picture));
            }
            if (!isCorrectSearchAlbumArt)
                albumArtImage.ImageSource = null;
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
            IsThisWindowShow = false;
            _lyricsLarge = new LyricsLargeWindow();
            _lyricsLarge.IsThisWindowShow = true;
            _lyricsLarge.Show();
        }

    }
}
