using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Presto.SWCamp.Lyrics
{
    class AlsongLRCManager
    {
        //sring -> StringBuilder 변경
        private StringBuilder data = new StringBuilder();
        
        public AlsongLRCManager()
        {
            data.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            data.Append("<SOAP-ENV:Envelope ");
            data.Append("xmlns:SOAP-ENV =\"http://www.w3.org/2003/05/soap-envelope\" ");
            data.Append("xmlns:SOAP-ENC=\"http://www.w3.org/2003/05/soap-encoding\" ");
            data.Append("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" ");
            data.Append("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" ");
            data.Append("xmlns:ns2=\"ALSongWebServer/Service1Soap\" ");
            data.Append("xmlns:ns1=\"ALSongWebServer\" ");
            data.Append("xmlns:ns3=\"ALSongWebServer/Service1Soap12\">");
            data.Append("<SOAP-ENV:Body>");
            data.Append("<ns1:GetResembleLyric2>");
            data.Append("<ns1:stQuery>");
            data.Append("<ns1:strTitle>{0}</ns1:strTitle>");
            data.Append("<ns1:strArtistName>{1}</ns1:strArtistName>");
            data.Append("<ns1:nCurPage>0</ns1:nCurPage>");
            data.Append("</ns1:stQuery>");
            data.Append("</ns1:GetResembleLyric2>");
            data.Append("</SOAP-ENV:Body>");
            data.Append("</SOAP-ENV:Envelope>");
        }
        public string GetLRCData(string title, string artist, int index)
        {
            return HtmlParsingToLRC(RequestLRCToAlsong(title, artist), index);
        }
        public string RequestLRCToAlsong(string title, string artist)
        {
            string url = "http://lyrics.alsong.co.kr/alsongwebservice/service1.asmx";
            string response = String.Empty;
            Console.WriteLine(String.Format(data.ToString(), title, artist));
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(String.Format(data.ToString(), title, artist));
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.UserAgent = "gSOAP/2.7";
            request.ContentType = "application/soap+xml; charset=UTF-8";

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            using (HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse())
            {
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                response = streamReader.ReadToEnd();
            }

            return response;

        }

        public string HtmlParsingToLRC(string data, int index)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(data);
            XmlNodeList xmlList = xml.GetElementsByTagName("ST_GET_RESEMBLELYRIC2_RETURN");

            if (xmlList.Count <= index) return "[00:00.00]데이터가 존재하지 않습니다";

            string x = xmlList[index]["strLyric"].InnerText; // xmlList상태로 변경하려니 인터넷 자료는 커녕 MS사에도 없길래 
            x = x.Replace("<br>", "\n");          // string 을만들어서 때려박아서 수정

            StringBuilder resultData = new StringBuilder();
            resultData.AppendFormat("[ar:{0}]\n", xmlList[0]["strArtistName"].InnerText);
            resultData.AppendFormat("[al:{0}]\n", xmlList[0]["strAlbumName"].InnerText);
            resultData.AppendFormat("[by:{0}]\n", xmlList[0]["strRegisterFirstName"].InnerText);
            resultData.AppendFormat("[ti:{0}]\n", xmlList[0]["strRegisterName"].InnerText);
            resultData.Append(x);
            return resultData.ToString();
        }
    }
}
