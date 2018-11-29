using Presto.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Presto.SWCamp.Lyrics
{
    class LyricsManager
    {
        private enum LRCFormat { Artist, Album, Title, Author, PlayeTime, LRCMaker, Lyrics, None };
        private readonly Regex artistPattern = new Regex(@"^\[ar:(.+)\]$");
        private readonly Regex albumPattern = new Regex(@"^\[al:(.+)\]$");
        private readonly Regex titlePattern = new Regex(@"^\[ti:(.+)\]$");
        private readonly Regex authorPattern = new Regex(@"^\[au:(.+)\]$");
        private readonly Regex lengthPattern = new Regex(@"^\[length:(.+)\]$");
        private readonly Regex byPattern = new Regex(@"^\[by:(.+)\]$");
        private readonly Regex syncPattern = new Regex(@"\[([0-9]+):([0-9]+)\.([0-9]+)\](.*)");
        private AlsongLRCManager alsongLRCManager = new AlsongLRCManager();
        private Lyrics lyrics;
        private List<double> timeList = null; // 이진탐색을 위해 사용하는 리스트
        public string CurrentMusic { get; set; } = null;
        public int LyricsListIndex { get; set; } = 0;
        
            
        public void StreamChanged()
        {
            lyrics = null;//GCC
            lyrics = new Lyrics();
            string artist = string.Empty;
            string album = string.Empty;
            string bitrate = string.Empty;
            string title = string.Empty;
            if (PrestoSDK.PrestoService.Player.CurrentMusic.Artist.Name != null)
                artist = PrestoSDK.PrestoService.Player.CurrentMusic.Artist.Name;
            if (PrestoSDK.PrestoService.Player.CurrentMusic.Album.Name != null)
            album = PrestoSDK.PrestoService.Player.CurrentMusic.Album.Name;
            if (PrestoSDK.PrestoService.Player.CurrentMusic.Title != null)
                title = PrestoSDK.PrestoService.Player.CurrentMusic.Title.ToString();
            bitrate = PrestoSDK.PrestoService.Player.CurrentMusic.Bitrate.ToString();
            try
            {
                CurrentMusic = PrestoSDK.PrestoService.Player.CurrentMusic.Path;
                string lyricsFileName = Path.GetFileNameWithoutExtension(CurrentMusic) + ".lrc";
                string parentPath = Path.GetDirectoryName(CurrentMusic);
                string path = Path.Combine(parentPath, lyricsFileName);
                FileInfo file = new FileInfo(path);
                
                if(title == string.Empty)
                {
                    title = Path.GetFileNameWithoutExtension(this.CurrentMusic);
                    PrestoSDK.PrestoService.Player.CurrentMusic.Title = title;
                }
                if (file.Exists == true && this.LyricsListIndex == 0)
                {
                    string[] lines = File.ReadAllLines(path, Encoding.Default);
                    this.LyricsParsing(lines);
                }
                else
                {
                    string lrcData = alsongLRCManager.GetLRCData(title, artist, LyricsListIndex);
                    string[] lines = lrcData.Split('\n');
                    this.LyricsParsing(lines);
                }
            }
            catch
            {
                if (artist == string.Empty) artist = lyrics.Artist;
                if (album == string.Empty) album = lyrics.Album;
                lyrics.Lines.Add(0,artist + "/" + album + "/" + bitrate + "kbps"
                    + "\n가사를 불러올 수 없습니다.");
            }
            timeList = lyrics.Lines.Keys.ToList<double>();
        }

        // 포맷 파싱
        private void LyricsParsing(string[] lines)
        {
            double time = 0;
            foreach (string data in lines)
            {
                LRCFormat type = this.FindMatcingType(data.ToLower());
                switch (type)
                {
                    case LRCFormat.Title: lyrics.Title = titlePattern.Match(data).Groups[1].ToString().Trim(); break;
                    case LRCFormat.Artist: lyrics.Artist = artistPattern.Match(data).Groups[1].ToString().Trim(); break;
                    case LRCFormat.Album: lyrics.Album = albumPattern.Match(data).Groups[1].ToString().Trim(); break;
                    case LRCFormat.Author: lyrics.Author = authorPattern.Match(data).Groups[1].ToString().Trim(); break;
                    case LRCFormat.LRCMaker: lyrics.By = byPattern.Match(data).Groups[1].ToString().Trim(); break;
                    case LRCFormat.PlayeTime: lyrics.Length = int.Parse(lengthPattern.Match(data).Groups[1].ToString().Trim()); break;
                    case LRCFormat.Lyrics:
                        GroupCollection matchingData = syncPattern.Match(data).Groups;
                        string lyricsData = matchingData[4].ToString().Trim();

                        //시간영역 m,s,f
                        string[] timeFormat = {
                            matchingData[1].ToString().Trim(),
                            matchingData[2].ToString().Trim(),
                            matchingData[3].ToString().Trim()
                        };
                        time = int.Parse(timeFormat[0]) * 1000 * 60 + int.Parse(timeFormat[1]) * 1000 + int.Parse(timeFormat[2]);

                        if(lyrics.Lines.ContainsKey(time) == true)
                        {
                            lyricsData = lyrics.Lines[time] + "\n" + lyricsData;
                            lyrics.Lines.Remove(time);
                        }
                        if (lyricsData == string.Empty) break;
                        lyrics.Lines.Add(time, lyricsData);
                        
                        break;
                    //텍스트만 존재할경우 이전가사와 합침
                    case LRCFormat.None:
                        if (lyrics.Lines.Count <= 0 || data.Trim() == string.Empty) break;
                        if (lyrics.Lines.ContainsKey(time) != true) break;
                        string newData = lyrics.Lines[time] + "\n" + data;
                        lyrics.Lines.Remove(time);
                        lyrics.Lines.Add(time, newData);
                        break;
                }
            }

            //LRC 가사 포맷정보 담기
            string infoData = "곡명: " + lyrics.Title + "(" + lyrics.Album + ")\n" +
                    "작사가: " + lyrics.Author + "\n" +
                    "가사 만든이: " + lyrics.By;
            if(lyrics.Lines.ContainsKey(0) == true)
            {
                infoData += "\n" + lyrics.Lines[0];
                lyrics.Lines.Remove(0);
            }
            lyrics.Lines.Add(0, infoData);
        }

        //해당 데이터가 어떤 데이터인지 판별
        private LRCFormat FindMatcingType(string inputLine)
        {
            if (artistPattern.IsMatch(inputLine) == true)
                return LRCFormat.Artist;
            if (albumPattern.IsMatch(inputLine) == true)
                return LRCFormat.Album;
            if (titlePattern.IsMatch(inputLine) == true)
                return LRCFormat.Title;
            if (authorPattern.IsMatch(inputLine) == true)
                return LRCFormat.Author;
            if (lengthPattern.IsMatch(inputLine)==true)
                return LRCFormat.PlayeTime;
            if (byPattern.IsMatch(inputLine) == true)
                return LRCFormat.LRCMaker;
            if (syncPattern.IsMatch(inputLine) == true)
                return LRCFormat.Lyrics;
            
            return LRCFormat.None;
        }

        public int GetCurrentLyricsIndex(double position)
        {
            if (this.lyrics == null) return 0;

            int closeLyrics = -1;

            // 사용자가 지정한 오프셋을 더해서 가사 위치찾기
            closeLyrics = timeList.BinarySearch(position + lyrics.Offset);

            if (closeLyrics >= 0) return -1;
            return Math.Max(0, ~closeLyrics - 1);
        }

        public string GetCurrentLyric(double position)
        {
            string preparing = "가사 준비중입니다.";
            
            if (lyrics == null || lyrics.Lines.Count <= 0 || position < 0)
                return preparing;
            int lyricsIndex = this.GetCurrentLyricsIndex(position);
            if (lyricsIndex < 0) return preparing;
            return lyrics.Lines[timeList[lyricsIndex]];
        }

        public double GetSelectPosition(int index)
        {
            double position = this.timeList[index] - this.lyrics.Offset;
            if (position < 0)
                position = 0;
            if (position > PrestoSDK.PrestoService.Player.Length)
                position = PrestoSDK.PrestoService.Player.Length - 1;
            return position;
        }

        public SortedDictionary<double, string> GetLyricsData()
        {
            if (this.lyrics != null)
                return this.lyrics.Lines;
            else return new SortedDictionary<double, string>();
        }

        public void SetOffset(double offset)
        {
            if (this.lyrics == null) return;
            this.lyrics.Offset += offset;
        }

        public double GetOffset()
        {
            if (this.lyrics == null) return 0;
            return this.lyrics.Offset;
        }
    }
}
