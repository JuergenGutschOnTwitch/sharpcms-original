using Gallio.Framework;
using Gallio.Model;
using MbUnit.Framework;
using WatiN.Core;
using WatiN.Core.Logging;

namespace Sharpcms.Web.Test.Helper
{
    public class WatiNTestBase
    {
        protected IE Ie;

        [SetUp]
        public void CreateBrowser()
        {
            Ie = new IE();
            Logger.LogWriter = new WatiNStreamLogger();
        }

        [TearDown]
        public void DisposeBrowser()
        {
            if (TestContext.CurrentContext.Outcome.Status == TestStatus.Failed)
            {
                TestHelper.Snapshot(Ie, "Final screen when failure occurred.", TestLog.Failures);
            }

            if (Ie != null)
            {
                Ie.Dispose();
            }
        }

        protected void Login()
        {
            using (TestLog.BeginSection("Go to sharpcms adminpage"))
            {
                Ie.GoTo("http://localhost:17267/cleansite/login.aspx");
                Assert.IsTrue(Ie.ContainsText("Login"), "Unable to load adminpage! Ensure that the webserver is started");
            }

            using (TestLog.BeginSection("Enter the password and press login"))
            {
                Ie.TextField(Find.ByName("data_login")).TypeText("admin");
                Ie.TextField(Find.ByName("data_password")).TypeText("admin");
                Ie.Button(Find.ByValue("Login")).Click();
                Assert.IsTrue(Ie.ContainsText("Pages"), "Expected to find \"Pages\" on the page.");
            }
        }

        protected void Logout()
        {
            using (TestLog.BeginSection("Lgout from sharpcms adminpage"))
            {
                Ie.Link(Find.ByText("Log out")).Click();
                Assert.IsTrue(Ie.ContainsText("Login"), "Expected to find \"Login \" on the page.");
            }
        }
    }
}