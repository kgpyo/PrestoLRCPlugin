using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Presto.SWCamp.Lyrics
{
    class AlsongLRCManager
    {
        public void Test()
        {
            string url = "http://lyrics.alsong.co.kr/alsongwebservice/service1.asmx";

            string data = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<SOAP-ENV:Envelope " +
                    "xmlns:SOAP-ENV =\"http://www.w3.org/2003/05/soap-envelope\" " +
                    "xmlns:SOAP-ENC=\"http://www.w3.org/2003/05/soap-encoding\" " +
                    "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                    "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                    "xmlns:ns2=\"ALSongWebServer/Service1Soap\" " +
                    "xmlns:ns1=\"ALSongWebServer\" " +
                    "xmlns:ns3=\"ALSongWebServer/Service1Soap12\">" +
                    "<SOAP-ENV:Body>" +
                        "<ns1:GetResembleLyric2>" +
                        "<ns1:stQuery>" +
                            "<ns1:strTitle>" + "lucky strike" + "</ns1:strTitle>" +
                            "<ns1:strArtistName>" + "maroon5" + "</ns1:strArtistName>" +
                            "<ns1:nCurPage>0</ns1:nCurPage>" +
                        "</ns1:stQuery>" +
                    "</ns1:GetResembleLyric2>" +
                    "</SOAP-ENV:Body>" +
                "</SOAP-ENV:Envelope>";
            Console.WriteLine(data);
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(data);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.UserAgent = "gSOAP/2.7";
            request.ContentType = "application/soap+xml; charset=UTF-8";

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();

            HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
            StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
            string respone = streamReader.ReadToEnd();

            Console.Write(respone);
        }
    }
}
