using Presto.Common;
using Presto.SDK;

[assembly: PrestoTitle("Lyrics")]
[assembly: PrestoAuthor("SW Camp")]
[assembly: PrestoDescription("가사 플러그인 입니다.")]

namespace Presto.SWCamp.Lyrics
{
    public class PluginEntry : PrestoPlugin
    {
        private LyricsWindow _lyrics;
        private LyricsLargeWindow _lyricsLarge;

        public override void OnLoad()
        {
            _lyrics = new LyricsWindow();
            _lyricsLarge = new LyricsLargeWindow();
            _lyrics.Show();
            _lyricsLarge.Show();
        }

        public override void OnUnload()
        {
            _lyrics.Close();
            _lyricsLarge.Close();
        }
    }
}
