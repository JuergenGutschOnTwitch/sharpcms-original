using Gallio.Framework;
using WatiN.Core.Interfaces;

namespace UITests.Helper
{
    public class WatiNStreamLogger : ILogWriter
    {
        public void LogAction(string message)
        {
            TestLog.WriteLine(message);
        }

        public void LogDebug(string message)
        {
            throw new System.NotImplementedException();
        }

        public bool HandlesLogAction { get; private set; }
        public bool HandlesLogDebug { get; private set; }
    }
}
