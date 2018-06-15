using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace TestRun.backoffice
{
    class BackOfficeProgramParameters : WebTestProgramParameters
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
    class BackOfficeProgram: CustomWebTestProgram
    {
        public string Login = null;
        public string Password = null;
        protected string ClientName = null;
        protected double ClientBalance = 0.0;

        public static CustomProgram FabricateBackOfficeProgram()
        {
            return new BackOfficeProgram();
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
        
        protected void DoLoginBackoffice()
        {
            LogStage(String.Format("Логин под \"{0}\"", Login));
            SendKeysToWebElement(".//*[@id='username']", Login, "поле логина", "поля логина");
            SendKeysToWebElement(".//*[@id='password']", Password, "поле пароля", "поля пароля");
            ClickWebElement(".//*[@class='login__btn']", "Кнопка логина", "кнопки логина");

            LogStartAction("Ожидание входа");
            IWebElement errorElement = FindWebElement(".//*[@class='login-form__error']");
            if (errorElement != null)
                throw new Exception(String.Format("Ошибка логина: {0}", errorElement.Text));
            LogActionSuccess();
        }

        public override void BeforeRun()
        {
            base.BeforeRun();
            DoLoginBackoffice();
        }

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

        protected void GeneralTab()
        {
            LogStage("Проверка работы Общей вкладки");
            ClickWebElement(".//*[@class='tabs__content-inner _state_visible']//i", "Иконка редактировать", "иконки редактировать");
            ClickWebElement(".//*[@class='ui__list']/div[7]//input", "Чекбокс запрещены лайф ставки", "чекбокса запрещены лайф ставки");
            ClickWebElement(".//*[@class='ui__list']/div[8]//input", "Чекбокс запрещены моб приложения", "чекбокса запрещены моб приложения");
            SendKeysToWebElement(".//*[@class='ui__label']//textarea", "AutoTestGeneralInfo", "Поле Комментарий к изменениями", "поля Комментарий к изменениями");
            ClickWebElement(".//*[@class='tabs__content-inner _state_visible']//*[text()='Сохранить']", "Кнопка Сохранить", "кнопки Сохранить");

        }
        protected void AdditionalTab()
        {
            LogStage("Проверка работы вкладки Доп информация");
            ClickWebElement(".//*[@class='tabs__nav'][1]", "Вкладка расширенная информация", "вкладки расширенная информация");
            if (WebElementExist(".//*[text()='Низкая степень риска']"))
            {
                ClickWebElement(".//*[@class='tabs__content-inner _state_visible']//*[text()='Редактировать']", "Кнопка Редактировать", "кнопки Редактировать");
                ClickWebElement(".//*[text()='Степень риска']/..//a", "Выпадающий список Степень риска", "выпадающего списка Степень риска");
                ClickWebElement(".//*[@class='ui-dropdown__items']/div[3]", "Строка средней степени риска", "строки средней степени риска");
                SendKeysToWebElement(".//*[@class='tabs__content-inner _state_visible']//textarea", "AutoTestAdditionalInfo", "Поле Комментарий к изменениями", "поля Комментарий к изменениями");
                ClickWebElement(".//*[button]//*[text()='Сохранить']", "Кнопка Сохранить", "кнопки Сохранить");
                return;
            }
            ClickWebElement(".//*[@class='tabs__content-inner _state_visible']//*[text()='Редактировать']", "Кнопка Редактировать", "кнопки Редактировать");
            ClickWebElement(".//*[text()='Степень риска']/..//a", "Выпадающий список Степень риска", "выпадающего списка Степень риска");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[4]", "Строка низкой степени риска", "строки низкой степени риска");
            SendKeysToWebElement(".//*[@class='tabs__content-inner _state_visible']//textarea", "AutoTestAdditionalInfo", "Поле Комментарий к изменениями", "поля Комментарий к изменениями");
            ClickWebElement(".//*[button]//*[text()='Сохранить']", "Кнопка Сохранить", "кнопки Сохранить");
        }
        protected void OperationTab()
        {
            LogStage("Проверка работы вкладки Операции");
            ClickWebElement(".//*[@class='tabs__nav'][2]", "Вкладка Операции", "вкладки Операции");
            ClickWebElement(".//*[@class='toolbar__item']//*[text()='Загрузить ещё']", "Кнопка Загрузить еще", "кнопки Загрузить еще");
            IWebElement countSearch= GetWebElement(".//*[@class='clients__operations-toolbar-row']/span/span","Не отображается число операций");
            if(!countSearch.Text.Contains("400"))
                throw new Exception("Не работает кнопка Загрузить еще");

            string test1 = driver.FindElement(By.XPath(".//*[@class='list-view__table-body']/tr[1]/td")).Text;
            ClickWebElement(".//*[@class='toolbar__icon fa fa-calendar']", "Иконка Календарь", "иконки Календарь");
            ClickWebElement(".//*[@id='calendar-popup']/div/table/tbody/tr[1]/td[2]/a", "Число в календаре", "число в календаре");
            ClickWebElement(".//*[@id='operations-calendar']//*[@class='toolbar__item']", "Поиск в календаре", "поиск в календаре");
            string test2 = driver.FindElement(By.XPath(".//*[@class='list-view__table-body']/tr[1]/td")).Text;
            if(test2==test1)
               throw new Exception("Не работает календарь");


            ClickWebElement(".//*[@class='ui__field-inner']//*[text()='Тип операции']", "Фильтр Тип операции", "фильтр Тип операции");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[1]", "Строка Сделана ставка", "строка Сделана ставка");
            ClickWebElement(".//*[@class='ui__field-inner']//*[text()='Номер операции']", "Фильтр Тип операции", "фильтр Тип операции");
            ClickWebElement(".//*[@class='ui-dropdown _state_expanded']//tr[1]", "Первый номер из фильтра", "первый номер из фильтра");
            ClickWebElement(".//*[@class='fa fa-external-link-square']", "Ссылка на номер события", "ссылки на номер события");

            var imgage = driver.FindElement(By.XPath(".//*[@class='ui-lightbox__modal']"));

            var action = new Actions(driver);
            action
                .MoveToElement(imgage)
                .MoveByOffset(250, 250)
                .Build()
                .Perform();

            action.Click().Perform();
        }

        protected void FreeBet()
        {
            LogStage("Проверка работы вкладки Фрибет");
            ClickWebElement(".//*[@class='tabs__nav'][4]", "Фильтр Фрибеты", "фильтр Фрибеты");
            ClickWebElement(".//*[@class='toolbar__item']//*[text()='Добавить']", "Кнопка Добавить", "кнопка Добавить");
            SendKeysToWebElement(".//*[text()='СУММА']/..//input", "500", "Поле Сумма фрибета", "поля Сумма фрибета");
            SendKeysToWebElement(".//*[text()='Комментарий']/..//textarea", "test comment", "Поле Комменнтарий", "поля Сумма фрибета");
            ClickWebElement(".//*[text()='Добавить']", "Кнопка Добавить", "кнопка Добавить");
            ClickWebElement(".//*[@class='cl']/div[1]//*[text()='удалить']", "Кнопка Удалить", "кнопка Удалить");
            SendKeysToWebElement(".//*[text()='Комментарий']/..//textarea", "Don`t need", "Поле Комменнтарий", "поля Сумма фрибета");
            ClickWebElement(".//*[@class='cl']/div[1]//*[text()='удалить']", "Кнопка Удалить", "кнопка Удалить");
            var beforeCount = driver.FindElements(By.XPath(".//*[@class='cl__body cl__row']")).Count;
            ClickWebElement(".//*[text()='Показать удаленные']", "Кнопка Показать удаленные", "кнопка Показать удаленные");
            var afterCount = driver.FindElements(By.XPath(".//*[@class='cl__body cl__row']")).Count;
            if(beforeCount==afterCount)
                throw new Exception("Не работает кнопка показать удаленные");

        }
    }
    
}
