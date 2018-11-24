using Presto.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presto.SWCamp.Lyrics
{
    class LyricsManager
    {
        private Lyrics lyrics;
        public LyricsManager()
        {
        }
        
        public void StreamChanged()
        {
            lyrics = new Lyrics();
            try
            {
                string currentMusic = PrestoSDK.PrestoService.Player.CurrentMusic.Path;
                string lyricsFileName = Path.GetFileNameWithoutExtension(currentMusic) + ".lrc";
                string parentPath = Path.GetDirectoryName(currentMusic);
                string[] lines = File.ReadAllLines(Path.Combine(parentPath, lyricsFileName));
                this.LyricsParsing(lines);
            }
            catch (Exception ex)
            {
                lyrics.Lines.Add(new KeyValuePair<double, string>(0,"처리중 문제가 발생하였습니다.\n" + ex.ToString()));
            }
        }

        // 포맷 파싱
        private void LyricsParsing(string[] lines)
        {
            double time = 0;
            foreach(string data in lines)
            {
                Lyrics.LRCFormat type = this.FindMatcingType(data.ToLower());
                switch (type)
                {
                    case Lyrics.LRCFormat.Title:
                        lyrics.Title = data;
                        break;
                    case Lyrics.LRCFormat.Artist:
                        lyrics.Artist = data;
                        break;
                    case Lyrics.LRCFormat.Album:
                        lyrics.Album = data;
                        break;
                    case Lyrics.LRCFormat.Author:
                        lyrics.Author = data;
                        break;
                    case Lyrics.LRCFormat.LRCMaker:
                        lyrics.By = data;
                        break;
                    case Lyrics.LRCFormat.PlayeTime:
                        //lyrics.Length = data;
                        break;
                    case Lyrics.LRCFormat.Lyrics:
                        int parseIdx = data.IndexOf("]");
                        string timeData = data.Substring(1, parseIdx - 1);
                        string lyricsData = data.Substring(parseIdx + 1);

                        //시간영역 m,s,f
                        string[] timeFormat = {
                            timeData.Split(':')[0],
                            timeData.Split(':')[1].Split('.')[0],
                            timeData.Split(':')[1].Split('.')[1]
                        };
                        time = int.Parse(timeFormat[0]) * 1000 * 60 + int.Parse(timeFormat[1]) * 1000 + int.Parse(timeFormat[2]);
                        lyrics.Lines.Add(new KeyValuePair<double, string>(time, lyricsData));

                        break;
                    case Lyrics.LRCFormat.None:
                        if (lyrics.Lines.Count <= 0) break;
                        string newData = lyrics.Lines.Last().Value + data;
                        time = lyrics.Lines.Last().Key;
                        lyrics.Lines.RemoveAt(lyrics.Lines.Count - 1);
                        lyrics.Lines.Add(new KeyValuePair<double, string>(time, newData));
                        break;
                }
            }
            lyrics.Lines.Sort((x, y) => x.Key.CompareTo(y.Key));

            for (int i = 1; i < lyrics.Lines.Count; i++)
            {
                KeyValuePair<double, string> cur = lyrics.Lines[i];
                KeyValuePair<double, string> prev = lyrics.Lines[i - 1];
                if (cur.Key == prev.Key)
                {
                    lyrics.Lines[i - 1] = new KeyValuePair<double, string>(cur.Key, prev.Value + "\r\n" + cur.Value);
                    lyrics.Lines.RemoveAt(i);
                    i--;
                }
            }
        }

        private Lyrics.LRCFormat FindMatcingType(string inputLine)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(inputLine, Lyrics.artistPattern))
                return Lyrics.LRCFormat.Artist;
            if (System.Text.RegularExpressions.Regex.IsMatch(inputLine, Lyrics.albumPattern))
                return Lyrics.LRCFormat.Album;
            if (System.Text.RegularExpressions.Regex.IsMatch(inputLine, Lyrics.titlePattern))
                return Lyrics.LRCFormat.Title;
            if (System.Text.RegularExpressions.Regex.IsMatch(inputLine, Lyrics.authorPattern))
                return Lyrics.LRCFormat.Author;
            if (System.Text.RegularExpressions.Regex.IsMatch(inputLine, Lyrics.lengthPattern))
                return Lyrics.LRCFormat.PlayeTime;
            if (System.Text.RegularExpressions.Regex.IsMatch(inputLine, Lyrics.byPattern))
                return Lyrics.LRCFormat.LRCMaker;
            if (System.Text.RegularExpressions.Regex.IsMatch(inputLine, Lyrics.syncPattern))
                return Lyrics.LRCFormat.Lyrics;
            
            return Lyrics.LRCFormat.None;
        }

        public string GetCurrentLyric(double position)
        {
            string preparing = "준비중";
            if (lyrics == null || lyrics.Lines.Count <= 0 || position < 0)
                return preparing;
            int lyricsLength = lyrics.Lines.Count;
            int left = 0, right = lyricsLength-1, mid = 0, closeLyrics = -1;
            while(left<=right)
            {
                mid = (left + right);
                if(lyrics.Lines[mid].Key <= position)
                {
                    closeLyrics = mid;
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }
            if (closeLyrics == -1)
            {
                string infoData = "Album: " + lyrics.Album + "\n" +
                    "Title: " + lyrics.Title + "\n" +
                    "Author: " + lyrics.Author + "\n" +
                    "By: " + lyrics.By;
                return infoData;
            }

            return lyrics.Lines[closeLyrics].Value;
        }
    }
}
