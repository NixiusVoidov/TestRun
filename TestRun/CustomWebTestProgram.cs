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
using OpenQA.Selenium.Remote;
using System.Threading;

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
        //        FirefoxOptions profile = new FirefoxOptions();
        //        profile.SetPreference("browser.download.folderList", 2);
        //        profile.SetPreference("browser.download.manager.showWhenStarting", false);
        //        profile.SetPreference("browser.helperApps.neverAsk.openFile", "text/csv,application/x-msexcel,application/excel,application/x-excel,application/vnd.ms-excel,image/png,image/jpeg,text/html,text/plain,application/msword,application/xml");
        //        profile.SetPreference("browser.helperApps.neverAsk.saveToDisk",
        //"text/csv,application/x-msexcel,application/excel,application/x-excel,application/vnd.ms-excel,image/png,image/jpeg,text/html,text/plain,application/msword,application/xml,application/json");
        //        profile.SetPreference("webdriver.log.file", "C:\\Users\\User\\Downloads\\firefox.log");

                //profile.SetPreference("browser.helperApps.alwaysAsk.force", false);
                //profile.SetPreference("browser.download.manager.alertOnEXEOpen", false);
                //profile.SetPreference("browser.download.manager.focusWhenStarting", false);
                //profile.SetPreference("browser.download.manager.useWindow", false);
                //profile.SetPreference("browser.download.manager.showAlertOnComplete", false);
                //profile.SetPreference("browser.download.manager.closeWhenDone", false);
                //Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
               // driver = new FirefoxDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), profile);
                driver = new FirefoxDriver();


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
                if (Browser.Equals("SAFARI", StringComparison.InvariantCultureIgnoreCase))
                {
                    driver.Close();
                    Thread.Sleep(2000);
                    driver.Quit();
                }
                else  driver.Quit();
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
                PerformanceProfiler.Profiler.Start("FindWebElement");
                IWebElement element = driver.FindElement(By.XPath(xPath));
                PerformanceProfiler.Profiler.Stage("driver.FindElement(ByXPath)");
                return element;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Метод проверяет существование элемента в DOM
        protected bool WebElementExist(string element)
        {
            try
            {
                driver.FindElement(By.XPath(string.Format(element)));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        protected static bool WaitTillElementisDisplayed(IWebDriver driver, string xpath, int timeoutInSeconds)
        {
            bool elementDisplayed = false;

            for (int i = 0; i < timeoutInSeconds; i++)
            {
                try
                {
                    if (timeoutInSeconds > 0)
                    {
                        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                        wait.Until(drv => drv.FindElement(By.XPath(xpath)));

                    }
                    elementDisplayed = driver.FindElement(By.XPath(xpath)).Displayed;
                }
                catch
                {
                    throw new Exception("Элемент не появился за " + timeoutInSeconds + " секунд");
                }
            }
            return elementDisplayed;
        }


        protected void ClearBeforeInput(string xpath)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            Thread.Sleep(1000);
            driver.FindElement(By.XPath(xpath)).Click();
            driver.FindElement(By.XPath(xpath)).Clear();
            if (Browser.Equals("SAFARI", StringComparison.InvariantCultureIgnoreCase))
            {
                driver.FindElement(By.XPath(xpath)).SendKeys(Keys.Command + "a");
            }
               else driver.FindElement(By.XPath(xpath)).SendKeys(Keys.Control + "a");
            driver.FindElement(By.XPath(xpath)).SendKeys(Keys.Delete);
            Thread.Sleep(1000);
            wait.Until(drv => drv.FindElement(By.XPath(xpath))).GetAttribute("value").Equals("");
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
            PerformanceProfiler.Profiler.Start("ClickWebElement");
            try
            {
                By by = By.XPath(xPath);
                PerformanceProfiler.Profiler.Stage("By.XPath");
                LogStartAction(String.Format("Клик {0}", elementCaptionInGenitive));
                PerformanceProfiler.Profiler.Stage("LogStartAction");
                IWebElement element = driver.FindElement(by);
                PerformanceProfiler.Profiler.Stage("driver.FindElement");
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                wait.Until(ExpectedConditions.ElementToBeClickable(by));
                PerformanceProfiler.Profiler.Stage("wait web driver");
                element.Click();
                PerformanceProfiler.Profiler.Stage("Element.Click");
                LogActionSuccess();
                PerformanceProfiler.Profiler.Stage("LogActionSuccess");

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
