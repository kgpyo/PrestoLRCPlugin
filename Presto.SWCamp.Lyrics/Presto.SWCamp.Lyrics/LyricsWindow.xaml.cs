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
        List<KeyValuePair<double, string>> lyrics;
        public LyricsWindow()
        {
            InitializeComponent();
            PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged;
        }

        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {
            lyrics = new List<KeyValuePair<double, string>>();
            //가사 데이터 읽어오기
            var currentMusic = PrestoSDK.PrestoService.Player.CurrentMusic.Path;
            var lyricsFileName = Path.GetFileNameWithoutExtension(currentMusic) + ".lrc";
            var parentPath = Path.GetDirectoryName(currentMusic);

            string[] lines = File.ReadAllLines(Path.Combine(parentPath, lyricsFileName));

            foreach (var line in lines)
            {
                string[] data = line.Split(']');
                double time = 0;
                try
                {
                    time = TimeSpan.ParseExact(data[0].Substring(1).Trim(), @"mm\:ss\.ff", CultureInfo.InvariantCulture).TotalMilliseconds;
                }
                catch
                {
                    continue;
                }
                lyrics.Add(new KeyValuePair<double, string>(time, data[1]));
            }

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
