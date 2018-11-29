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
        public string Run(string keyword)
        {
            const string searchUrl = "http://www.maniadb.com/api/search/{0}/?sr=song&display=1&key=gyeongpoy@naver.com&v=0.5";
            string search = String.Format(searchUrl, keyword);

            try
            {
                using (XmlTextReader reader = new XmlTextReader(search))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            if (reader.Name == "image")
                            {
                                string id = reader["image"];
                                reader.Read();
                                return reader.Value.ToString();
                            }
                        }
                    }
                }
            }
            catch
            {

            }

            return "null";
        }
    }
}
