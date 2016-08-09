using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Launcher.AcceptanceTest;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests
{
    [TestFixture("firefox")]
    [TestFixture("chrome")]
    [TestFixture("edge")]
    public class InactiveWorkspacesAreFrozen : BaseTest
    {
        private class OperationObject
        {
            public OperationObject(string op, string path, object value)
            {
                Op = op;
                Path = path;
                Value = value;
            }

            public string Op { get; set; }
            public string Path { get; set; }
            public object Value { get; set; }

            protected bool Equals(OperationObject other)
            {
                return string.Equals(Op, other.Op) && string.Equals(Path, other.Path) && Equals(Value, other.Value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((OperationObject) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (Op != null ? Op.GetHashCode() : 0);
                    hashCode = (hashCode*397) ^ (Path != null ? Path.GetHashCode() : 0);
                    hashCode = (hashCode*397) ^ (Value != null ? Value.GetHashCode() : 0);
                    return hashCode;
                }
            }

            public override string ToString()
            {
                return $"Op: {Op}, Path: {Path}, Value: {Value}";
            }
        }

        private WebDriverWait _wait;
        private By _helperOneSelector;
        private By _helperTwoSelector;
        private By _helperTwoInputSelector;
        private IJavaScriptExecutor _javaScriptExecutor;

        public InactiveWorkspacesAreFrozen(string browser) : base(browser)
        {
        }

        [SetUp]
        public void SetUp()
        {
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            _helperOneSelector = SelectorForMenuLink("/Launcher_AcceptanceHelperOne");
            _helperTwoSelector = SelectorForMenuLink("/Launcher_AcceptanceHelperTwo");
            _helperTwoInputSelector = By.Id("helper-two-input");
            _javaScriptExecutor = (IJavaScriptExecutor)driver;
            driver.Navigate().GoToUrl(baseURL + "/launcher");
        }

        [Test]
        public void InactiveWorkspacesAreFrozenTest()
        {
            // ARRANGE
            var puppetClient = driver.FindElement(By.XPath("//puppet-client"));
            _javaScriptExecutor.ExecuteScript(@"
window.seleniumPatchesReceived = [];
arguments[0].addEventListener('patchreceived', function(ev) {window.seleniumPatchesReceived.push(ev.detail.data);});", puppetClient);

            // go to helper one to make sure it's loaded
            var helperOneLink = _wait.Until(ExpectedConditions.ElementToBeClickable(_helperOneSelector));
            helperOneLink.Click();

            // go to helper two, where we actually do stuff
            var helperTwoLink = _wait.Until(ExpectedConditions.ElementToBeClickable(_helperTwoSelector));
            helperTwoLink.Click();
            // value in input is persisted in database, if we used same value in each test, second run (and next) would always pass
            var newvalue = "new-value-"+Guid.NewGuid();

            // ACT
            var input = _wait.Until(ExpectedConditions.ElementExists(_helperTwoInputSelector));
            input.Clear();
            input.SendKeys(newvalue);

            // ASSERT
            Predicate<OperationObject> isForbiddenOperation = o => 
                o.Op == "replace" 
                && o.Path.EndsWith("Launcher_AcceptanceHelperOne/SharedValue/Description$") 
                && Equals(o.Value, newvalue);
            // this checks that no element matches the forbidden predicate
            Assert.That(GetReceivedPatches(), Has.None.Matches(isForbiddenOperation));
        }

        private IEnumerable<OperationObject> GetReceivedPatches()
        {
            // contrary to what it states in docs (List<string>), selenium returns a ReadOnlyCollection<Object>
            IEnumerable<object> patchesRaw =
                (IEnumerable<object>) _javaScriptExecutor.ExecuteScript("return window.seleniumPatchesReceived;");
            return patchesRaw
                .Cast<string>()
                .Select(JsonConvert.DeserializeObject<List<OperationObject>>)
                .SelectMany(o => o);
        }

        private By SelectorForMenuLink(string menuLink)
        {
            return By.CssSelector($"#launcher-menu a[href*=\'{menuLink}\']");
        }
    }
}