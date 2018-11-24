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
            lyrics = new Lyrics();
        }
        
        public void StreamChanged()
        {
            try
            {
                string currentMusic = PrestoSDK.PrestoService.Player.CurrentMusic.Path;
                string lyricsFileName = Path.GetFileNameWithoutExtension(currentMusic) + ".lrc";
                string parentPath = Path.GetDirectoryName(currentMusic);
                string[] lines = File.ReadAllLines(Path.Combine(parentPath, lyricsFileName));
                this.LyricsParsing(lines);
            }
            catch
            {
                lyrics.Lines.Add(new KeyValuePair<double, string>(0,"처리중 문제가 발생하였습니다."));
            }
        }

        // 포맷 파싱
        private void LyricsParsing(string[] lines)
        {
            foreach(string data in lines)
            {
                Lyrics.LRCFormat type = this.FindMatcingType(data.ToLower());
                switch(type)
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
                        int idx = data.IndexOf("]");
                        string timeData = data.Substring(1, idx);
                        string lyricsData = data.Substring(idx + 1);
                        //double time = int.Parse(timeFormat[0]) * 1000 * 60 + int.Parse(timeFormat[1]) * 1000 + int.Parse(timeFormat[2]);
                        //lyrics.Lines.Add(new KeyValuePair<double, string>(time, ddd[1]));
                        break;
                    case Lyrics.LRCFormat.None:
                        break;
                }
            }
        }

        private Lyrics.LRCFormat FindMatcingType(string inputLine)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(Lyrics.Patterns[0], inputLine))
                return Lyrics.LRCFormat.Artist;
            if (System.Text.RegularExpressions.Regex.IsMatch(Lyrics.Patterns[1], inputLine))
                return Lyrics.LRCFormat.Album;
            if (System.Text.RegularExpressions.Regex.IsMatch(Lyrics.Patterns[2], inputLine))
                return Lyrics.LRCFormat.Title;
            if (System.Text.RegularExpressions.Regex.IsMatch(Lyrics.Patterns[3], inputLine))
                return Lyrics.LRCFormat.Author;
            if (System.Text.RegularExpressions.Regex.IsMatch(Lyrics.Patterns[4], inputLine))
                return Lyrics.LRCFormat.PlayeTime;
            if (System.Text.RegularExpressions.Regex.IsMatch(Lyrics.Patterns[5], inputLine))
                return Lyrics.LRCFormat.LRCMaker;
            if (System.Text.RegularExpressions.Regex.IsMatch(Lyrics.Patterns[6], inputLine))
                return Lyrics.LRCFormat.Lyrics;
            
            return Lyrics.LRCFormat.None;
        }

        public string GetCurrentLyric(double position)
        {
            string preparing = "전주중";
            int lyricsLength = lyrics.Lines.Count;
            int left = 0, right = lyricsLength, mid = 0, closeLyrics = -1;
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
                return preparing;

            return lyrics.Lines[closeLyrics].Value;
        }
    }
}
