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
        public LyricsWindow()
        {
            InitializeComponent();
            PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged;
            lyricsManager = new LyricsManager();

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {
            lyricsManager.StreamChanged();
        }
        
        private void Timer_Tick(object sender, EventArgs e)
        {
            double cur = Presto.SDK.PrestoSDK.PrestoService.Player.Position;
            textLyrics.Text = lyricsManager.GetCurrentLyric(cur);
            /*
            //lyrics.BinarySearch(cur, IComparer<KeyValuePair<double, string>>());
            //이분탐색
            int left = 0, right = lyrics.Count - 1, mid = 0, ans = 0;
            while(left<=right)
            {
                mid = (left + right) / 2;
                if(lyrics[mid].Key <= cur)
                {
                    ans = mid;
                    left = mid + 1;
                } else
                {
                    right = mid - 1;
                }
            }
            textLyrics.Text = lyrics[ans].Value;
            */
            /*
            for (int i=0;i<lyrics.Count;i+=1)
            {
                double time = lyrics[i].Key;
                string text = lyrics[i].Value;
                if(i+1<lyrics.Count)
                {
                    if (time <= cur && cur < lyrics[i + 1].Key)
                    {
                        textLyrics.Text = text;
                        break;
                    }
                }
                else
                {
                    textLyrics.Text = text;
                }
            }
            */
        }
    }
}
