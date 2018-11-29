using Presto.SDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace Presto.SWCamp.Lyrics
{
    class AlbumartManager
    {
        public BitmapImage AlbumArtImageSource { get; set; } = null;
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

            return string.Empty;
        }

        public void AlbumArtSearch()
        {
            bool isCorrectSearchAlbumArt = false;
            if (PrestoSDK.PrestoService.Player.CurrentMusic.Album.Picture == null)
            {
                string title = string.Empty;
                title = PrestoSDK.PrestoService.Player.CurrentMusic.Title;
                if(title==string.Empty)
                {
                    string CurrentMusic = PrestoSDK.PrestoService.Player.CurrentMusic.Path;
                    title = Path.GetFileNameWithoutExtension(CurrentMusic);
                    PrestoSDK.PrestoService.Player.CurrentMusic.Title = title;
                }
                if (title != string.Empty)
                {
                    string path = string.Empty;
                    string artist = string.Empty;
                    artist = PrestoSDK.PrestoService.Player.CurrentMusic.Artist.Name;
                    if (artist == string.Empty)
                    {
                        path = this.Run(title);
                    }
                    else
                    {
                        path = this.Run(title + " " + artist);
                    }
                    if (!(path == string.Empty || path == string.Empty))
                    {
                        isCorrectSearchAlbumArt = true;
                        AlbumArtImageSource = new BitmapImage(new Uri(path));
                    }
                }
            }
            else
            {
                isCorrectSearchAlbumArt = true;
                AlbumArtImageSource = new BitmapImage(new Uri(PrestoSDK.PrestoService.Player.CurrentMusic.Album.Picture));
            }
            if (!isCorrectSearchAlbumArt)
                AlbumArtImageSource = null;
        }
    }
}
