using System;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using Microsoft.Win32;
using System.IO;

namespace Launcher.AcceptanceTest
{
    public class BaseTest
    {
        public BaseTest(string browser)
        {
            this.browser = browser;
        }

        protected IWebDriver driver;
        private StringBuilder verificationErrors;
        protected string baseURL;
        protected string browser;

        [SetUp]
        public void SetupTest()
        {
            baseURL = "http://localhost:8080";
            verificationErrors = new StringBuilder();

            if (browser == "edge")
            {
                Assert.Ignore("Edge tests are disabled due to random timeout fails.");
            }
            if (browser == "edge" && !IsEdgeAvailable())
            {
                Assert.Ignore("You're not using Windows 10, so Microsoft Edge is unavailable. The test is being omitted.");
            }
            driver = WebDriverFactory.Create(this.browser);
        }

        [TearDown]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            Assert.AreEqual("", verificationErrors.ToString());
        }

        private bool IsEdgeAvailable()
        {
            var browserKeys = Registry.ClassesRoot.OpenSubKey(@"Local Settings\Software\Microsoft\Windows\CurrentVersion\AppModel\PackageRepository\Packages");
            if (browserKeys == null)
            {
                return false;
            }
            else
            {
                var elements = browserKeys.GetSubKeyNames();
                foreach (var element in elements)
                {
                    if (element.Contains("MicrosoftEdge"))
                    {
                        var keyPath = browserKeys.OpenSubKey(element);
                        var edgePath = keyPath.GetValue("Path");
                        if (edgePath != null && Directory.Exists(edgePath.ToString()))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;
        }
    }
}