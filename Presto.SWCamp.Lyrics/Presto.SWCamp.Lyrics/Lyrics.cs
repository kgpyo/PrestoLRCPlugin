using Presto.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presto.SWCamp.Lyrics
{
    class Lyrics
    {
        public enum LRCFormat { Artist, Album, Title, Author, PlayeTime, LRCMaker, Lyrics, None };
        public const string artistPattern = @"^\[ar:.*\]$";
        public const string albumPattern = @"^\[al:.*\]$";
        public const string titlePattern = @"^\[ti:.*\]$";
        public const string authorPattern = @"^\[au:.*\]$";
        public const string lengthPattern = @"^\[length:.*\]$";
        public const string byPattern = @"^\[by:.*\]$";
        public const string syncPattern = @"\[[0-9]*:[0-9]*\.[0-9]*\].*";
        public List<KeyValuePair<double, string>> Lines = new List<KeyValuePair<double, string>>();    //가사 데이터
        public int Sync { get; set; }                       //싱크 데이터
        public string Title { get; set; } = "정보없음";     //제목
        public string Artist { get; set; } = "정보없음";    //가수
        public string Album { get; set; } = "정보없음";     //앨범
        public string Author { get; set; } = "정보없음";    //작사가
        public int Length { get; set; } = 0;                //음악의 길이
        public string By { get; set; } = "정보없음";        //LRC 작성자
    }
}
