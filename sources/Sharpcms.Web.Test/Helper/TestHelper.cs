using Gallio.Common.Markup;
using Gallio.Framework;
using WatiN.Core;
using WatiN.Core.UtilityClasses;

namespace Sharpcms.Web.Test.Helper
{
    public static class TestHelper
    {
        public static void Snapshot(IE ie, string caption)
        {
            Snapshot(ie, caption, TestLog.Default);
        }

        public static void Snapshot(IE ie, string caption, MarkupStreamWriter logStreamWriter)
        {
            using (logStreamWriter.BeginSection(caption))
            {
                logStreamWriter.Write("Url: ");
                using (logStreamWriter.BeginMarker(Marker.Link(ie.Url)))
                {
                    logStreamWriter.Write(ie.Url);
                }

                logStreamWriter.WriteLine();

                logStreamWriter.EmbedImage(caption + ".png", new CaptureWebPage(ie).CaptureWebPageImage(false, false, 100));
            }
        }
    }
}