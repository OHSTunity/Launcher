using System;
using System.Text;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;

/*
This test checks if the workspace switches correctly by:
- loading a workspace "Launcher_AcceptanceHelperOne"
- switching to a workspace "Launcher_AcceptanceHelperTwo"
- switching back to the workspace "Launcher_AcceptanceHelperOne"
*/
namespace SeleniumTests {
    [TestFixture("firefox")]
    [TestFixture("chrome")]
    [TestFixture("edge")]
    public class WorkspaceSwitches {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;
        private string browser;

        public WorkspaceSwitches(string browser) {
            this.browser = browser;
        }

        [SetUp]
        public void SetupTest() {
            driver = WebDriverFactory.Create(this.browser);
            baseURL = "http://localhost:8080";
            verificationErrors = new StringBuilder();
        }

        [TearDown]
        public void TeardownTest() {
            try {
                driver.Quit();
            }
            catch (Exception) {
                // Ignore errors if unable to close the browser
            }
            Assert.AreEqual("", verificationErrors.ToString());
        }

        [Test]
        public void TheWorkspaceSwitchesTest() {
            driver.Navigate().GoToUrl(baseURL + "/launcher");
            for (int second = 0; ; second++) {
                if (second >= 60) Assert.Fail("timeout");
                try {
                    if (IsElementPresent(By.CssSelector(CssSelectorForMenuLink("/Launcher_AcceptanceHelperOne")))) break;
                }
                catch (Exception) { }
                Thread.Sleep(1000);
            }

            ClickOnElement(driver.FindElement(By.CssSelector(CssSelectorForMenuLink("/Launcher_AcceptanceHelperOne"))));

            for (int second = 0; ; second++) {
                if (second >= 60) Assert.Fail("timeout");
                try {
                    if ("This is Launcher Acceptance Helper One" == driver.FindElement(By.CssSelector("h1")).Text) break;
                }
                catch (Exception) { }
                Thread.Sleep(1000);
            }

            ClickOnElement(driver.FindElement(By.CssSelector(CssSelectorForMenuLink("/Launcher_AcceptanceHelperTwo"))));

            for (int second = 0; ; second++) {
                if (second >= 60) Assert.Fail("timeout");
                try {
                    if ("This is Launcher Acceptance Helper Two" == driver.FindElement(By.CssSelector("h2")).Text) break;
                }
                catch (Exception) { }
                Thread.Sleep(1000);
            }

            Assert.IsTrue(IsElementPresent(By.TagName("h1")));
            Assert.IsFalse(driver.FindElement(By.TagName("h1")).Displayed);

            ClickOnElement(driver.FindElement(By.CssSelector(CssSelectorForMenuLink("/Launcher_AcceptanceHelperOne"))));

            Assert.AreEqual("This is Launcher Acceptance Helper One", driver.FindElement(By.CssSelector("h1")).Text);
            Assert.IsTrue(IsElementPresent(By.TagName("h2")));
            Assert.IsFalse(driver.FindElement(By.TagName("h2")).Displayed);
        }
        private bool IsElementPresent(By by) {
            try {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException) {
                return false;
            }
        }

        private bool IsAlertPresent() {
            try {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException) {
                return false;
            }
        }

        private string CssSelectorForMenuLink(string menuLink) {
            return "#launcher-menu a[href*='" + menuLink + "']";
        }

        private void ClickOnElement(IWebElement element) {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            js.ExecuteScript("arguments[0].click();", element);
        }

        private string CloseAlertAndGetItsText() {
            try {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert) {
                    alert.Accept();
                }
                else {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally {
                acceptNextAlert = true;
            }
        }
    }
}
