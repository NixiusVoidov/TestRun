using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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

        protected void SwitchPageToBets()
        {
            LogStage("Переход в линию");
            ClickWebElement(".//*[@href='/#!/bets']", "Вкладка \"Линия\"", "вкладки \"Линия\"");
        }

        protected void OpenBetsEventFilter()
        {
            LogStage("Открытие фильтра событий");
            ClickWebElement(".//*[@class='events__filter _type_sport']", "Фильтр событий", "фильтра событий");
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

            if (Login.Length > 0)
            {
                DoLogin();
                UpdateLoginInfo();
            }
        }
    }
}
