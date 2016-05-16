using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Launcher.AcceptanceTest;

/*
This test checks if the workspace opens correctly when
a link to it is clicked
*/
namespace SeleniumTests {
    [TestFixture("firefox")]
    [TestFixture("chrome")]
    [TestFixture("edge")]
    public class WorkspaceOpens : BaseTest
    {
        private bool acceptNextAlert = true;
        
        public WorkspaceOpens(string browser) : base(browser) { }
        
        [Test]
        public void TheWorkspaceOpensTest() {
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
