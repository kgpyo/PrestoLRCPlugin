﻿using Presto.SDK;
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
        string[] str = new string[4];
        public LyricsWindow()
        {
            InitializeComponent();
            PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged;
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height;

            lyricsManager = new LyricsManager();

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(300)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {
            this.Show();
            lyricsManager.StreamChanged();
            
            //GC 강제 실행
            System.GC.Collect(2, GCCollectionMode.Forced);
            System.GC.WaitForFullGCComplete();

            if (PrestoSDK.PrestoService.Player.CurrentMusic.Title == null)
            {
                string fileName = Path.GetFileNameWithoutExtension(lyricsManager.CurrentMusic);
                PrestoSDK.PrestoService.Player.CurrentMusic.Title = fileName;
            }
        }
        
        private void Timer_Tick(object sender, EventArgs e)
        {
            
            double cur = PrestoSDK.PrestoService.Player.Position;
            str[0] = "";
            str = lyricsManager.GetCurrentLyric(cur);

            if (str[0] == "가사 준비중입니다.")
            {
                textLyrics.Text = str[0];
            }
            else if(str[3] == "1")
            {
                string[] arr = str[0].Split('\n'); //곡 정보를 스플릿 하여 배열에 저장
                textLyrics.Inlines.Clear();
                textLyrics.Inlines.Add(new Run { Text = arr[0] + "\n" });
                textLyrics.Inlines.Add(new Run { Text = arr[1] + "\n" });
                textLyrics.Inlines.Add(new Run { Text = arr[2] });
            }
            else if(str[3] == "2")
            {                
                textLyrics.Inlines.Clear();
                textLyrics.Inlines.Add(new Run { Text = str[0] + "\n" });
                textLyrics.Inlines.Add(new Run { Text = str[1] , FontWeight = FontWeights.Bold });           
            }
            else if(str[3] == "3")
            {           
                textLyrics.Inlines.Clear();
                textLyrics.Inlines.Add(new Run { Text = str[0] + "\n" });
                textLyrics.Inlines.Add(new Run { Text = str[1] + "\n", FontWeight = FontWeights.Bold });
                textLyrics.Inlines.Add(new Run { Text = str[2]});
            }
            else if(str[3] == "4")
            {
                textLyrics.Inlines.Clear();
                textLyrics.Inlines.Add(new Run { Text = str[0] + "\n", FontWeight = FontWeights.Bold });
                textLyrics.Inlines.Add(new Run { Text = str[1]});
            }

            
            
        }

        /* 창 드래그 */
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton != MouseButtonState.Pressed) DragMove();
        }

        /* 앨범아트 버튼 관리 */
        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            PrestoSDK.PrestoService.Player.PlayPrevious();
        }

        private void PlayOrPauseButton_Click(object sender, RoutedEventArgs e)
        {
            switch (PrestoSDK.PrestoService.Player.PlaybackState)
            {
                case Common.PlaybackState.Playing:
                    playStatus.Content = "||";
                    PrestoSDK.PrestoService.Player.Pause();
                    break;
                case Common.PlaybackState.Paused:
                    playStatus.Content = "";
                    PrestoSDK.PrestoService.Player.Resume();
                    break;
                case Common.PlaybackState.Stopped:
                    playStatus.Content = "";
                    PrestoSDK.PrestoService.Player.Play();
                    break;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            PrestoSDK.PrestoService.Player.PlayNext();
        }

        /* 우상단 버튼 관리 */
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
    }
}
