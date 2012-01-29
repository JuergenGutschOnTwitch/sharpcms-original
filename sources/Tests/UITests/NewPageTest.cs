using Gallio.Framework;
using MbUnit.Framework;
using UITests.Helper;
using WatiN.Core;

namespace UITests
{
    /// <summary>
    /// This is the Logintest to the sharpcms backend
    /// </summary>
    [TestFixture]
    public class PagesTest : WatiNTestBase
    {
        [Test]
        public void CreatePage()
        {
            Login();
            
            using (TestLog.BeginSection("Go to sharpcms adminpage and press logout"))
            {
                Ie.Link(Find.ByText("Pages")).Click();
                Assert.IsTrue(Ie.ContainsText("German"), "Expected to find \"German\" on the page.");
                Ie.Link(Find.ByText("German")).Click();
                Assert.IsTrue(Ie.ContainsText("Page data "), "Expected to find \"Page data \" on the page.");
            }

            Logout();
        }
    }
}
