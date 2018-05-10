using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;


namespace TestRun
{
    class FonbetWebProgramParameters : WebTestProgramParameters
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    class FonbetWebProgram : CustomWebTestProgram
    {
        public string Login = null;
        public string Password = null;
        protected string ClientName = null;
        protected double ClientBalance = 0.0;

        public static CustomProgram FabricateFonbetWebProgram()
        {
            return new FonbetWebProgram();
        }

        public void ReadParameters(FonbetWebProgramParameters parameters)
        {
            base.ReadParameters(parameters);
            Login = parameters.Login;
            Password = parameters.Password;
        }

        public override void ReadParamsFromJson(string jsonText)
        {
            FonbetWebProgramParameters prm = JsonConvert.DeserializeObject<FonbetWebProgramParameters>(jsonText);
            ReadParameters(prm);
        }

        public override void SetFromString(string paramName, string paramValue)
        {
            if (paramName == "Login")
                Login = paramValue;
            else if (paramName == "Password")
                Password = paramValue;
            else
                base.SetFromString(paramName, paramValue);
        }

        public override void PrintParameters()
        {
            base.PrintParameters();
            Console.WriteLine("Логин: \t\t\t\t{0}", Login);
        }

        public override void WriteParametersToReport()
        {
            base.WriteParametersToReport();
            Report.Conditions.Add("Login", Login);
        }

        protected virtual bool NeedLogin()
        {
            return true;
        }

        protected void DoLogin()
        {
            LogStage(String.Format("Логин под \"{0}\"", Login));
            ClickWebElement(".//*[@class='header__login-head']/a", "Панель логина", "панели логина");
            SendKeysToWebElement(".//*[@class='login-form__form']/div[1]/input", Login, "поле логина", "поля логина");
            SendKeysToWebElement(".//*[@class='login-form__form']/div[2]/input", Password, "поле пароля", "поля пароля");
            ClickWebElement(".//*[@class='login-form__form-row _right']/div[2]/button", "Кнопка логина", "кнопки логина");

            LogStartAction("Ожидание входа");
            IWebElement errorElement = FindWebElement(".//*[@class='login-form__error']");
            if (errorElement != null)
                throw new Exception(String.Format("Ошибка логина: {0}", errorElement.Text));
            LogActionSuccess();
        }


        protected void UpdateLoginInfo()
        {
            IWebElement loginCaptionElement = FindWebElement(".//*[@class='header__login-label _style_white']");
            if (loginCaptionElement != null)
            {
                ClientName = loginCaptionElement.Text;
                LogHint(String.Format("Клиент: {0}", ClientName));
            }
            else
            {
                LogWarning("Информации о клиенте на странице не обнаружено.");
                ClientName = null;
            }

            IWebElement balanceElement = FindWebElement(".//*[@class='header__login-balance']");
            if (balanceElement != null)
            {
                string balanceText = balanceElement.Text.Replace(" ", "").Replace(".", ",");
                
                if (Double.TryParse(balanceText, out ClientBalance))
                {
                    LogHint(String.Format("Баланс: {0:F2} ", ClientBalance));
                }
                else
                {
                    ClientBalance = 0.0;
                    LogWarning(String.Format("Неверный формат значения баланса клиента: {0}", balanceElement.Text));
                }
            }
            else
                LogWarning("Информации о балансе клиента на странице не обнаружено.");
        }
        // Метод переходит на вкладку Линия
        protected void SwitchPageToBets()
        {
            LogStage("Переход в линию");
            ClickWebElement(".//*[@href='/#!/bets']", "Вкладка \"Линия\"", "вкладки \"Линия\"");
        }

        // Метод проверяет что при сужении окна бразуера появляется скролл у фильтра в верхнем меню, выдает ошибку если это не так
        protected void CheckScrollinFilterTopMenu(int x, int y)
        {
            var windowSize = new System.Drawing.Size(x, y);
            driver.Manage().Window.Size = windowSize;
            ExecuteJavaScript("return document.getElementById(\"popup\").scrollHeight>document.getElementById(\"popup\").clientHeight;", "Не работает скролл в фильтре верхнего меню");
        }

        // Метод кликает на фильтр выбора спорта
        protected void ClickOnSportType()
        {
            LogStage("Открытие меню с видами спорта");
            ClickWebElement(".//*[@class='events__filter _type_sport']", "Фильтр выбора спорта", "фильтра выбора спорта");
        }
        // Метод устанавливает настройки вебсайта по-умолчанию.
        protected void MakeDefaultSettings()
        {
            LogStartAction("Установка настроек по умолчанию");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__restore-btn']", "Кнопка восстановления настроек по умолчанию", "кнопки восстановления настроек по умолчанию");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");
            LogActionSuccess();
        }
         // Метод открывает фильтр событий
        protected void OpenBetsEventFilter()
        {
            LogStage("Открытие фильтра событий");
            ClickWebElement(".//*[@class='events__filter _type_sport']", "Фильтр событий", "фильтра событий");
        }
        // Метод переключает меню в режим отображения слева
        protected void SwitchToLeftTypeMenu()
        {
            LogStage("Переключение в меню 'слева'");
            ClickWebElement(".//*[@class='page__line-header']//*[@class='events__head _page_line']/div[1]", "Кнопка разворот меню фильтра", "кнопка разворота меню фильтра");
            ClickWebElement(".//*[@id='popup']/li[1]", "Меню СЛЕВА", "меню слева");
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
         // Метод принимает на вход число минут и название строки в фильтре времени и проверяет что в результат выдачи попадают только те события, которые удовлетворяют кол-ву минут, переданных в параметр
        protected void TimeFilterChecker(int timeValue, string chooseData)
        {
            ClickWebElement(".//*[@class='events__filter _type_time']", "Меню времени в фильтре", "меню времени в фильтре");
            ClickWebElement(String.Format(".//*[@class='events__filter-item']//*[text()='{0}']", chooseData), String.Format("Значение \"{0}\"", chooseData), String.Format("значения \"{0}\"", chooseData));
            IList<IWebElement> all = driver.FindElements(By.XPath(".//*[@class='table__time']"));
            foreach (IWebElement element in all)
            {
                string[] timeSplit = element.Text.Split(' ');

                if ((timeSplit.Length == 3) || (timeSplit.Length == 4))
                {
                    string[] hourSplit = timeSplit.Last().Split(':');
                    int[] numbers = hourSplit.Select(int.Parse).ToArray();
                    var timeSpan = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, numbers[0], numbers[1], 0) - DateTime.Now);
                    if (timeSpan.Minutes > timeValue) 
                        throw new Exception("Фильтры по времени не работают");
                }
                else
                {
                    throw new Exception("В массиве больше элементов чем должно быть");
                }
            }
        }

        // Метод впринимает кол-во отмеченных событий в суперэкспрессе
        protected void MarkedBoxCounter(int value, string mark)
        {
            IList<IWebElement> all = driver.FindElements(By.XPath(String.Format(".//*[@class='matrix-form__mark-box{0}']", mark)));
            if (all.Count != value)
            {
                throw new Exception("Что-то не так с выбором/очисткой полей возможных вариантов");
            }
        }

        // Метод выбирает два исхода в 1ой строчке, независимо от того какого уже там событие выбрано
        protected void ChooseTwoResults()
        {
            if (driver.FindElement(By.XPath(".//*[@class='bet-table']//tr[2]//td[4]/div")).GetAttribute("class")
                .Equals("matrix-form__mark-box"))
            {
                driver.FindElement(By.XPath(".//*[@class='bet-table']//tr[2]//td[4]/div")).Click();
            }
            else if (driver.FindElement(By.XPath(".//*[@class='bet-table']//tr[2]//td[5]/div")).GetAttribute("class")
                .Equals("matrix-form__mark-box"))
            {
                driver.FindElement(By.XPath(".//*[@class='bet-table']//tr[2]//td[5]/div")).Click();
            }
            else
            {
                driver.FindElement(By.XPath(".//*[@class='bet-table']//tr[2]//td[6]/div")).Click();
            }
        }

        public override void BeforeRun()
        {
            base.BeforeRun();
            // Смена языка при необходимости
            IWebElement langSetElement = FindWebElement(".//*[@class='header__lang-set']");
            if ((langSetElement != null) && driver.Title.Contains("Home"))
            {
                LogStage("Смена языка на русский");
                ClickWebElement(".//*[@class='header__lang-set']", "Кнопка выбора языка", "кнопки выбора языка");
                ClickWebElement(".//*[@class='header__lang-item']//*[text()='Русский']", "Кнопка выбора русского языка", "кнопки выбора русского языка");
            }

            if (NeedLogin())
            {
                DoLogin();
                UpdateLoginInfo();
            }
        }
    }
}
