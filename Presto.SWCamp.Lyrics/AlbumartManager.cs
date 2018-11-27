using Presto.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Presto.SWCamp.Lyrics
{
    class AlbumartManager
    {
        public void Run()
        {
            const string searchUrl = "http://www.maniadb.com/api/search/{0}/?sr=song&display=1&key=gyeongpoy@naver.com&v=0.5";
            string keyword = Console.ReadLine();
            string search = String.Format(searchUrl, keyword);
            bool check = false;
            using (XmlTextReader reader = new XmlTextReader(search))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name == "image")
                        {
                            string id = reader["image"];
                            check = true;
                            reader.Read();
                            //new BitmapImage(new Uri(reader.Value));
                            break;
                        }
                    }
                }
            }
        }
    }
}
