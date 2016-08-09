using System;
using System.Threading;
using Launcher.AcceptanceTest;
using NUnit.Framework;
using OpenQA.Selenium;

namespace SeleniumTests
{
    [TestFixture("firefox")]
    [TestFixture("chrome")]
    [TestFixture("edge")]
    public class WorkspaceSwitches : BaseTest
    {
        public WorkspaceSwitches(string browser) : base(browser)
        {
        }

        /// <summary>
        /// This test checks if the workspace switches correctly by:
        ///- loading a workspace "Launcher_AcceptanceHelperOne"
        ///- switching to a workspace "Launcher_AcceptanceHelperTwo"
        ///- switching back to the workspace "Launcher_AcceptanceHelperOne"
        /// </summary>
        [Test]
        public void TheWorkspaceSwitchesTest()
        {
            driver.Navigate().GoToUrl(baseURL + "/launcher");
            for (var second = 0;; second++)
            {
                if (second >= 60) Assert.Fail("timeout");
                try
                {
                    if (IsElementPresent(By.CssSelector(CssSelectorForMenuLink("/Launcher_AcceptanceHelperOne"))))
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                }
                Thread.Sleep(1000);
            }

            ClickOnElement(driver.FindElement(By.CssSelector(CssSelectorForMenuLink("/Launcher_AcceptanceHelperOne"))));

            for (var second = 0;; second++)
            {
                if (second >= 60) Assert.Fail("timeout");
                try
                {
                    if ("This is Launcher Acceptance Helper One" == driver.FindElement(By.CssSelector("h1")).Text)
                        break;
                }
                catch (Exception)
                {
                }
                Thread.Sleep(1000);
            }

            ClickOnElement(driver.FindElement(By.CssSelector(CssSelectorForMenuLink("/Launcher_AcceptanceHelperTwo"))));

            for (var second = 0;; second++)
            {
                if (second >= 60) Assert.Fail("timeout");
                try
                {
                    if ("This is Launcher Acceptance Helper Two" == driver.FindElement(By.CssSelector("h2")).Text)
                        break;
                }
                catch (Exception)
                {
                }
                Thread.Sleep(1000);
            }

            Assert.IsTrue(IsElementPresent(By.TagName("h1")));
            Assert.IsFalse(driver.FindElement(By.TagName("h1")).Displayed);

            ClickOnElement(driver.FindElement(By.CssSelector(CssSelectorForMenuLink("/Launcher_AcceptanceHelperOne"))));

            Assert.AreEqual("This is Launcher Acceptance Helper One", driver.FindElement(By.CssSelector("h1")).Text);
            Assert.IsTrue(IsElementPresent(By.TagName("h2")));
            Assert.IsFalse(driver.FindElement(By.TagName("h2")).Displayed);
        }

        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private string CssSelectorForMenuLink(string menuLink)
        {
            return $"#launcher-menu a[href*=\'{menuLink}\']";
        }

        private void ClickOnElement(IWebElement element)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
        }
    }
}