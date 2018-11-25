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
        private enum LRCFormat { Artist, Album, Title, Author, PlayeTime, LRCMaker, Lyrics, None };
        private const string artistPattern = @"^\[ar:.*\]$";
        private const string albumPattern = @"^\[al:.*\]$";
        private const string titlePattern = @"^\[ti:.*\]$";
        private const string authorPattern = @"^\[au:.*\]$";
        private const string lengthPattern = @"^\[length:.*\]$";
        private const string byPattern = @"^\[by:.*\]$";
        private const string syncPattern = @"\[[0-9]*:[0-9]*\.[0-9]*\].*";
        private Lyrics lyrics;
        public string CurrentMusic { get; set; } = null;
                
        public void StreamChanged()
        {
            lyrics = new Lyrics();
            try
            {
                CurrentMusic = PrestoSDK.PrestoService.Player.CurrentMusic.Path;
                string lyricsFileName = Path.GetFileNameWithoutExtension(CurrentMusic) + ".lrc";
                string parentPath = Path.GetDirectoryName(CurrentMusic);
                string[] lines = File.ReadAllLines(Path.Combine(parentPath, lyricsFileName));
                this.LyricsParsing(lines);
            }
            catch
            {
                string artist = PrestoSDK.PrestoService.Player.CurrentMusic.Artist.Name;
                string album = PrestoSDK.PrestoService.Player.CurrentMusic.Album.Name;
                string bitrate = PrestoSDK.PrestoService.Player.CurrentMusic.Bitrate.ToString();
                if (artist == null) artist = "알수없는 음악가";
                if (album == null) album = "알 수 없는 앨범";
                if (bitrate == "0") bitrate = "알수없음";
                lyrics.Lines.Add(new KeyValuePair<double, string>(0,artist + "/" + album + "/" + bitrate + "kbps"
                    + "\n가사를 불러올 수 없습니다."));
            }
        }

        // 포맷 파싱
        private void LyricsParsing(string[] lines)
        {
            double time = 0;
            foreach(string data in lines)
            {
                LRCFormat type = this.FindMatcingType(data.ToLower());
                switch (type)
                {
                    case LRCFormat.Title: lyrics.Title = data; break;
                    case LRCFormat.Artist: lyrics.Artist = data; break;
                    case LRCFormat.Album: lyrics.Album = data; break;
                    case LRCFormat.Author: lyrics.Author = data; break;
                    case LRCFormat.LRCMaker: lyrics.By = data; break;
                    case LRCFormat.PlayeTime: /*lyrics.Length = data; */ break;
                    case LRCFormat.Lyrics:
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
                    //텍스트만 존재할경우 이전가사와 합침
                    case LRCFormat.None:
                        if (lyrics.Lines.Count <= 0) break;
                        string newData = lyrics.Lines.Last().Value + data;
                        time = lyrics.Lines.Last().Key;
                        lyrics.Lines.RemoveAt(lyrics.Lines.Count - 1);
                        lyrics.Lines.Add(new KeyValuePair<double, string>(time, newData));
                        break;
                }
            }
            lyrics.Lines.Sort((x, y) => x.Key.CompareTo(y.Key));

            //중복된 시간을 가진 가사가 여러개 있을경우 하나로 합침
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

        //해당 데이터가 어떤 데이터인지 판별
        private LRCFormat FindMatcingType(string inputLine)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(inputLine, artistPattern))
                return LRCFormat.Artist;
            if (System.Text.RegularExpressions.Regex.IsMatch(inputLine, albumPattern))
                return LRCFormat.Album;
            if (System.Text.RegularExpressions.Regex.IsMatch(inputLine, titlePattern))
                return LRCFormat.Title;
            if (System.Text.RegularExpressions.Regex.IsMatch(inputLine, authorPattern))
                return LRCFormat.Author;
            if (System.Text.RegularExpressions.Regex.IsMatch(inputLine, lengthPattern))
                return LRCFormat.PlayeTime;
            if (System.Text.RegularExpressions.Regex.IsMatch(inputLine, byPattern))
                return LRCFormat.LRCMaker;
            if (System.Text.RegularExpressions.Regex.IsMatch(inputLine, syncPattern))
                return LRCFormat.Lyrics;
            
            return LRCFormat.None;
        }

        public string GetCurrentLyric(double position)
        {
            string preparing = "가사 준비중입니다.";
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

            //근접한 위치를 찾을 수 없을경우 가사파일 정보를 출력
            if (closeLyrics == -1)
            {
                string infoData = "곡명: " + lyrics.Title + "(" + lyrics.Album+ ")\n" +
                    "작사가: " + lyrics.Author + "\n" +
                    "가사 만든이: " + lyrics.By;
                return infoData;
            }

            return lyrics.Lines[closeLyrics].Value;
        }
    }
}
