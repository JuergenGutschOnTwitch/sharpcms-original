using Gallio.Framework;
using WatiN.Core.Interfaces;

namespace UITests
{
    public class WatiNStreamLogger : ILogWriter
    {
        public void LogAction(string message)
        {
            TestLog.WriteLine(message);
        }
    }
}
