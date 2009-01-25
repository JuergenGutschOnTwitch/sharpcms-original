using Gallio.Framework;
using Gallio.Model.Logging;
using WatiN.Core;

namespace UITests
{
    public class TestHelper
    {

        public static void Snapshot(IE ie, string caption)
        {
            Snapshot(ie, caption, TestLog.Default);
        }

        public static void Snapshot(IE ie, string caption, TestLogStreamWriter logStreamWriter)
        {
            using (logStreamWriter.BeginSection(caption))
            {
                logStreamWriter.Write("Url: ");
                using (logStreamWriter.BeginMarker(Marker.Link(ie.Url)))
                    logStreamWriter.Write(ie.Url);
                logStreamWriter.WriteLine();

                logStreamWriter.EmbedImage(caption + ".png", new CaptureWebPage(ie).CaptureWebPageImage(false, false, 100));
            }
        }
    }
}
