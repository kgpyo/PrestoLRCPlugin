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
        public static List<string> Patterns = new List<string>()
        {
            {"^\\[ar:^.*s\\]$" },
            {"^\\[al:^.*s\\]$" },
            {"^\\[ti:^.*s\\]$" },
            {"^\\[au:^.*s\\]$" },
            {"^\\[length:^.*s\\]$" },
            {"^\\[by:^.*s\\]$" },
            {"^\\[[0-9]+:[0-9]+.[0-9]+\\]^.*s" }
        };
        public List<KeyValuePair<double, string>> Lines;    //가사 데이터
        public int Sink { get; set; }                       //싱크 데이터
        public string Title { get; set; }                   //제목
        public string Artist { get; set; }                  //가수
        public string Album { get; set; }                   //앨범
        public string Author { get; set; }                  //작사가
        public int Length { get; set; }                     //음악의 길이
        public string By { get; set; }                      //LRC 작성자
    }
}
