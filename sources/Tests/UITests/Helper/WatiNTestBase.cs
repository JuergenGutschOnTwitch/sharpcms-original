using Gallio.Framework;
using Gallio.Model;
using MbUnit.Framework;
using WatiN.Core;
using WatiN.Core.Logging;

namespace UITests
{
    public class WatiNTestBase
    {
        protected IE ie;

        [SetUp]
        public void CreateBrowser()
        {
            ie = new IE();
            Logger.LogWriter = new WatiNStreamLogger();
        }

        [TearDown]
        public void DisposeBrowser()
        {
            if (TestContext.CurrentContext.Outcome.Status == TestStatus.Failed)
                TestHelper.Snapshot(ie, "Final screen when failure occurred.", TestLog.Failures);
            if (ie != null)
                ie.Dispose();
        }

        public void Login()
        {
            using (TestLog.BeginSection("Go to sharpcms adminpage"))
            {
                ie.GoTo("http://localhost:17267/cleansite/login.aspx");
                Assert.IsTrue(ie.ContainsText("Login"), "Unable to load adminpage! Ensure that the webserver is started");
            }

            using (TestLog.BeginSection("Enter the password and press login"))
            {
                ie.TextField(Find.ByName("data_login")).TypeText("admin");
                ie.TextField(Find.ByName("data_password")).TypeText("admin");
                ie.Button(Find.ByValue("Login")).Click();
                Assert.IsTrue(ie.ContainsText("Pages"), "Expected to find \"Pages\" on the page.");
            }
        }

        public void Logout()
        {
            using (TestLog.BeginSection("Lgout from sharpcms adminpage"))
            {
                ie.Link(Find.ByText("Log out")).Click();
                Assert.IsTrue(ie.ContainsText("Login"), "Expected to find \"Login \" on the page.");
            }
        }
    }
}
