using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Reflection;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Opera;
using OpenQA.Selenium.IE;

namespace TestRun
{
    class WebTestProgramParameters : ProgramParameters
    {
        public string Browser { get; set; }
        public string HomeURL { get; set; }
    }

    class CustomWebTestProgram : CustomProgram
    {
        public string Browser = null;
        public string HomeURL = null;

        protected IWebDriver driver = null;

        public static CustomProgram FabricateCustomWebTestProgram()
        {
            return new CustomWebTestProgram();
        }

        public void ReadParameters(WebTestProgramParameters prm)
        {
            base.ReadParameters(prm);

            Browser = prm.Browser;
            HomeURL = prm.HomeURL;
        }

        public override void ReadParameters(TestTaskResponseBody prm)
        {
            base.ReadParameters(prm);
            Browser = prm.browser;
            HomeURL = prm.url;
        }

        public override void ReadParamsFromJson(string jsonText)
        {
            WebTestProgramParameters prm = JsonConvert.DeserializeObject<WebTestProgramParameters>(jsonText);
            ReadParameters(prm);            
        }

        public override void SetFromString(string paramName, string paramValue)
        {
            if (paramName == "Browser")
                Browser = paramValue;
            if (paramName == "HomeURL")
                HomeURL = paramValue;
            else
                base.SetFromString(paramName, paramValue);
        }

        public override void VerifyParameters()
        {
            base.VerifyParameters();
            if (Browser == null)
                throw new Exception("Браузер не указан");
            if (HomeURL == null)
                throw new Exception("Не указан стартовый URL");
        }

        public override void PrintParameters()
        {
            base.PrintParameters();
            Console.WriteLine("Браузер: \t\t\t{0}", Browser);
            Console.WriteLine("Стартовая страница: \t\t{0}", HomeURL);
        }

        public override void WriteParametersToReport()
        {
            base.WriteParametersToReport();
            Report.Conditions.Add("Browser", Browser);
            Report.Conditions.Add("HomeURL", HomeURL);
        }

        public override void BeforeRun()
        {
            base.BeforeRun();
            LogStartAction("Инициализация браузера");
            Console.WriteLine("");

            if (Browser.Equals("CHROME", StringComparison.InvariantCultureIgnoreCase))
            {
                driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            }
            else  if (Browser.Equals("SAFARI", StringComparison.InvariantCultureIgnoreCase))
            {
                driver = new SafariDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            }
            else if (Browser.Equals("FIREFOX", StringComparison.InvariantCultureIgnoreCase))
            {
                driver = new FirefoxDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            }
            else if (Browser.Equals("OPERA", StringComparison.InvariantCultureIgnoreCase))
            {
                driver = new OperaDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            }
            else if (Browser.Equals("IE", StringComparison.InvariantCultureIgnoreCase))
            {
                driver = new InternetExplorerDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            }
            else
                throw new Exception(String.Format("Неизвестный тип браузера: '{0}'", Browser));

            driver.Manage().Window.Maximize();

            LogStartAction("Переход браузера на стартовую страницу");

            driver.Navigate().GoToUrl(HomeURL);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            LogActionSuccess();
        }

        public override void AfterRun()
        {
            if (driver != null)
            {
                LogStartAction("Закрытие браузера");
                driver.Quit();
                LogActionSuccess();
            }
            base.AfterRun();
        }

        public override void OnError(Exception exception)
        {
            //((ITakesScreenshot)driver).GetScreenshot().SaveAsFile("lastError.png", ScreenshotImageFormat.Png);
            base.OnError(exception);

        }

        // Нахождение на странице веб-элемента. Не порождает исключение
        protected IWebElement FindWebElement(string xPath)
        {
            try
            {
                return driver.FindElement(By.XPath(xPath));
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected IWebElement GetWebElement(string xPath, string errorIfNotExists)
        {
            IWebElement element = FindWebElement(xPath);
            if (element == null)
                throw new Exception(errorIfNotExists);
            return element;
        }

        protected void ClickWebElement(string xPath, string elementCaptionInNominative, string elementCaptionInGenitive)
        {
            try
            {
                LogStartAction(String.Format("Клик {0}", elementCaptionInGenitive));
                IWebElement element = driver.FindElement(By.XPath(xPath));
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                //  wait.Until(drv => drv.FindElement(By.XPath(xPath)));
                wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(xPath)));
                element.Click();
                LogActionSuccess();

            }
            catch (NoSuchElementException)
            {
                throw new Exception(String.Format("{0} на странице не обнаружен(а)", elementCaptionInNominative));
            }
            catch (ElementNotVisibleException)
            {
                throw new Exception(String.Format("{0} не виден(а)", elementCaptionInNominative));
            }
            catch (StaleElementReferenceException)
            {
                throw new Exception(String.Format("{0} недоступен(на) для клика", elementCaptionInNominative));
            }
        }

        protected void ClickWebElementWithText(string className, string text, string elementCaptionInNominative,
            string elementCaptionInGenitive)
        {
            string checkValue = string.Format(".//*[@class='{0}']/*[text()='{1}']", className, text);
            string stringError = string.Format("{0} \"{1}\"", elementCaptionInNominative, text);
            string stringErrorTwo = string.Format("{0} \"{1}\"", elementCaptionInGenitive, text);
            ClickWebElement(checkValue, stringError, stringErrorTwo);
        }

        protected void SendKeysToWebElement(string xPath, string keys, string elementCaptionInNominative, string elementCaptionInGenitive)
        {
            try
            {
                LogStartAction(String.Format("Ввод \"{0}\" в {1}", keys, elementCaptionInNominative));
                IWebElement element = driver.FindElement(By.XPath(xPath));
                element.SendKeys(keys);
                
                LogActionSuccess();
            }
            catch (NoSuchElementException)
            {
                throw new Exception(String.Format("{0} на странице не обнаружен(а)", elementCaptionInNominative));
            }
            catch (ElementNotVisibleException)
            {
                throw new Exception(String.Format("{0} не виден(а)", elementCaptionInNominative));
            }
            catch (InvalidElementStateException)
            {
                throw new Exception(String.Format("Состояние элемента {0} не позволяет выполнить ввод", elementCaptionInGenitive));
            }
            catch (StaleElementReferenceException)
            {
                throw new Exception(String.Format("{0} недоступен(на) для ввода", elementCaptionInNominative));
            }
        }

        protected object ExecuteJavaScript(string javaScript, string errorMessage)
        {
            try
            {
                IJavaScriptExecutor executor = driver as IJavaScriptExecutor;
                return executor.ExecuteScript(javaScript);
            }
            catch (Exception)
            {
                throw new Exception(errorMessage);
            }
        }
    }
}
