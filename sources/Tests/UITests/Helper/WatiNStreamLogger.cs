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
    }
}
