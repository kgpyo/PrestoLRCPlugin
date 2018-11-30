using Presto.SDK;
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
using System.Windows.Threading;

namespace Presto.SWCamp.Lyrics
{
    /// <summary>
    /// LyricsLargeWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricsLargeWindow : Window
    {
        LyricsManager lyricsManager;
        public bool IsThisWindowShow { get; set;  } = false;
        private bool isAutoLyricsIndexChange = false;
        public Window ParentWindow { get; set; } = null;
        public LyricsLargeWindow(Window parent)
        {
            InitializeComponent();
            this.ParentWindow = parent;
            PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged;
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height;
            this.Topmost = true;

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += Timer_Tick;
            timer.Start();

        }

        // 재생중인 음악이 바뀌면
        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {
            // 큰 플레이어가 실행중이고, 숨겨진 상태라면
            if (IsThisWindowShow == true)
            {
                this.Show();
            }
            lyricsManager = null;
            lyricsManager = new LyricsManager();
            lyricsManager.LyricsListIndex = 0;
            // 현재 바뀐 음악에 대한 가사 처리
            lyricsManager.StreamChanged();

            // 재바인딩
            lyricsList.ItemsSource = lyricsManager.GetLyricsData();

            isAutoLyricsIndexChange = true;
            lyricsList.SelectedIndex = 0;
            lyricsList.ScrollIntoView(lyricsList.Items[0]);
        }

        public void SetAlbumArtImage(BitmapImage AlbumArtImageSource)
        {
            albumArtImage.ImageSource = AlbumArtImageSource;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (lyricsManager == null) return;

            double cur = PrestoSDK.PrestoService.Player.Position;

            // 재생중일때만 가사 위치 변경, 오프셋 반영
            if (cur > 0 && PrestoSDK.PrestoService.Player.PlaybackState == Common.PlaybackState.Playing)
            {
                int listBoxIndex = lyricsManager.GetCurrentLyricsIndex(cur);
                // 유효한 범위가 아니라면 종료
                if (listBoxIndex < 0) return;
                if (lyricsList.SelectedIndex != listBoxIndex)
                {
                    //Timer에 의해 ListBox Item이 변경되었으므로
                    isAutoLyricsIndexChange = true;
                    lyricsList.SelectedIndex = listBoxIndex;
                }

                //Offset값
                offsetValue.Text = lyricsManager.GetOffset().ToString();
            }
            else
            {
                isAutoLyricsIndexChange = true;
                lyricsList.SelectedIndex = 0;
            }
        }

        private void LyricsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lyricsManager == null) return;
            int selectedIndex = lyricsList.SelectedIndex;
            // 유효한 범위가 아니라면 실행하지 않음
            if (!(selectedIndex >=0 && selectedIndex<lyricsList.Items.Count))
                return;

            //자동 스크롤
            //자동으로 가운데 정렬이 안되기 때무에 임의로 2칸 당김
            //세줄가사이상이면 밀릴수 있으므로 출력하지 않음
            int lineCount = lyricsList.Items[selectedIndex].ToString().Split('\n').Length;
            if (selectedIndex < lyricsList.Items.Count - 3 && lineCount == 2)
                lyricsList.ScrollIntoView(lyricsList.Items[selectedIndex + 2]);
            else if(selectedIndex<lyricsList.Items.Count - 5 && lineCount <= 1)
                lyricsList.ScrollIntoView(lyricsList.Items[selectedIndex + 4]);
            else lyricsList.ScrollIntoView(lyricsList.Items[selectedIndex]);


            if (isAutoLyricsIndexChange == true)
            {
                isAutoLyricsIndexChange = false;
                return;
            }

            //사용자가 클릭하여 가사위치가 변경되었다면 구간 점프
            double position = lyricsManager.GetSelectPosition(selectedIndex);
            PrestoSDK.PrestoService.Player.Position = position;
        }

        /* 윈도우 창 */
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed) DragMove();
        }

        /* 버튼 구역 */
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

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            PrestoSDK.PrestoService.Player.PlayPrevious();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            PrestoSDK.PrestoService.Player.PlayNext();
        }

        private void VloumeDownButton_Click(object sender, RoutedEventArgs e)
        {
            float volume = PrestoSDK.PrestoService.Player.Volume;
            if (volume > 0f)
                volume -= 0.05f;
            if (volume < 0) volume = 0f;
            PrestoSDK.PrestoService.Player.Volume = volume;
        }

        private void VloumeUpButton_Click(object sender, RoutedEventArgs e)
        {
            float volume = PrestoSDK.PrestoService.Player.Volume;
            if (volume < 1f)
                volume += 0.05f;
            if (volume > 1) volume = 1f;
            PrestoSDK.PrestoService.Player.Volume = volume;
        }

        private void LoopButton_Click(object sender, RoutedEventArgs e)
        {
            switch(PrestoSDK.PrestoService.Player.RepeatMode)
            {
                case Common.RepeatMode.None:
                    PrestoSDK.PrestoService.Player.RepeatMode = Common.RepeatMode.All;
                    break;
                case Common.RepeatMode.All:
                    PrestoSDK.PrestoService.Player.RepeatMode = Common.RepeatMode.One;
                    break;
                case Common.RepeatMode.One:
                    PrestoSDK.PrestoService.Player.RepeatMode = Common.RepeatMode.None;
                    break; ;
            }
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            if(PrestoSDK.PrestoService.Player.ShuffleMode == Common.ShuffleMode.None)
            {
                PrestoSDK.PrestoService.Player.ShuffleMode = Common.ShuffleMode.Random;
            }
            else
            {
                PrestoSDK.PrestoService.Player.ShuffleMode = Common.ShuffleMode.None;
            }
        }

        private void HideButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            IsThisWindowShow = false;
            this.Hide();
            ((LyricsWindow)this.ParentWindow).IsThisWindowShow = true;
            ((LyricsWindow)this.ParentWindow).Show();
        }

        private void OffsetMinusButton_Click(object sender, RoutedEventArgs e)
        {
            if (lyricsManager == null) return;
            //500밀리세컨드 간격 조절
            lyricsManager.SetOffset(-500);
        }

        private void OffsetPlusButton_Click(object sender, RoutedEventArgs e)
        {
            if (lyricsManager == null) return;
            lyricsManager.SetOffset(500);
        }

        private void NextLyricsDataButon_Click(object sender, RoutedEventArgs e)
        {
            if (lyricsManager == null) return;
            lyricsManager.LyricsListIndex += 1;
            lyricsManager.StreamChanged();
            lyricsList.ItemsSource = lyricsManager.GetLyricsData();
        }

        private void PreviousLyricsDataButton_Click(object sender, RoutedEventArgs e)
        {
            if (lyricsManager == null) return;
            if (lyricsManager.LyricsListIndex > 0)
            {
                lyricsManager.LyricsListIndex -= 1;
                lyricsManager.StreamChanged();
                lyricsList.ItemsSource = lyricsManager.GetLyricsData();
            }
        }
    }
}
