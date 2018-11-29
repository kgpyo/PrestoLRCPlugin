﻿using Presto.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presto.SWCamp.Lyrics
{
    class Lyrics
    {
        //list -> sortedDictonary 변경
        public SortedDictionary<double, string> Lines = new SortedDictionary<double, string>();    //가사 데이터
        public double Offset { get; set; }                  //싱크조절
        public string Title { get; set; } = "정보없음";     //제목
        public string Artist { get; set; } = "정보없음";    //가수
        public string Album { get; set; } = "정보없음";     //앨범
        public string Author { get; set; } = "정보없음";    //작사가
        public int Length { get; set; } = 0;                //음악의 길이
        public string By { get; set; } = "정보없음";        //LRC 작성자
    }
}
