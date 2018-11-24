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
        Dictionary<double, string> lyricsData;
        List<KeyValuePair<double, string>> lyrics;
        public LyricsWindow()
        {
            InitializeComponent();
            PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged;
        }

        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {
            lyricsData = new Dictionary<double, string>();
            lyrics = new List<KeyValuePair<double, string>>();
            //가사 데이터 읽어오기
            var currentMusic = PrestoSDK.PrestoService.Player.CurrentMusic.Path;
            var lyricsFileName = Path.GetFileNameWithoutExtension(currentMusic) + ".lrc";
            var parentPath = Path.GetDirectoryName(currentMusic);

            string[] lines = File.ReadAllLines(Path.Combine(parentPath, lyricsFileName));
            //가사 파싱
            foreach (var line in lines)
            {

                int idx = line.IndexOf("]");
                string[] data = {
                    line.Substring(0,idx),
                    line.Substring(idx+1)
                };
                double time = 0;
                
                try
                {
                    time = TimeSpan.ParseExact(data[0].Substring(1).Trim(), @"mm\:ss\.ff", CultureInfo.InvariantCulture).TotalMilliseconds;
                }
                catch
                {
                    continue;
                }
                string lyric = data[1];
                if(lyricsData.ContainsKey(time) == true)
                {
                    if (lyric.Length != 0)
                    {
                        lyric = lyricsData[time] + "\n" + lyric;
                        lyricsData.Remove(time);
                    }
                }
                lyricsData.Add(time, lyric);
            }

            foreach(KeyValuePair<double, string> data in lyricsData)
            {
                lyrics.Add(new KeyValuePair<double, string>(data.Key, data.Value));
            }

            lyrics.Sort((x,y)=>x.Value.CompareTo(y.Value));

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double cur = Presto.SDK.PrestoSDK.PrestoService.Player.Position;
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
        }
    }
}
