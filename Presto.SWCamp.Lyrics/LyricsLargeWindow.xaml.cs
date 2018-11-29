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
        AlbumartManager albumartManager;
        private bool isThisWindowShow = false;
        private bool isAutoLyricsIndexChange = false;

        public LyricsLargeWindow()
        {
            InitializeComponent();
            PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged;
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height;
            lyricsManager = new LyricsManager();
            albumartManager = new AlbumartManager();
            
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        // 재생중인 음악이 바뀌면
        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {
            // 큰 플레이어가 실행중이고, 숨겨진 상태라면
            if(isThisWindowShow == true)
            {
                this.Show();
            }

            // 현재 바뀐 음악에 대한 가사 처리
            lyricsManager.StreamChanged();

            // 재바인딩
            lyricsList.ItemsSource = lyricsManager.GetLyricsData();

            // 해당 음악에 앨범아트 존재하지 않으면 검색하여 출력
            

            //GC 강제 실행 (메모리 부분)
            System.GC.Collect(2, GCCollectionMode.Forced);
            System.GC.WaitForFullGCComplete();

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double cur = PrestoSDK.PrestoService.Player.Position;

            // 재생중일때만 가사 위치 변경
            if (cur > 0 && PrestoSDK.PrestoService.Player.PlaybackState == Common.PlaybackState.Playing)
            {
                int listBoxIndex = lyricsManager.GetCurrentLyricsIndex(cur);
                // 유효한 범위가 아니라면 종료
                if (listBoxIndex < 0) return;
                if(lyricsList.SelectedIndex != listBoxIndex)
                {
                    //Timer에 의해 ListBox Item이 변경되었으므로
                    isAutoLyricsIndexChange = true;
                    lyricsList.SelectedIndex = listBoxIndex;
                }
            }
            
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed) DragMove();
        }

        private void LyricsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = lyricsList.SelectedIndex;

            //자동 스크롤
            lyricsList.ScrollIntoView(lyricsList.Items[selectedIndex]);
            

            if(isAutoLyricsIndexChange == true)
            {
                isAutoLyricsIndexChange = false;
                return;
            }

            //사용자가 클릭하여 가사위치가 변경되었다면 구간 점프
            double position = lyricsManager.GetSelectPosition(selectedIndex);
            PrestoSDK.PrestoService.Player.Position = position;
        }
    }
}
