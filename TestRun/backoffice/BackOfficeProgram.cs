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

        public static CustomProgram FabricateBackOfficeProgram()
        {
            return new BackOfficeProgram();
        }

        public void ReadParameters(BackOfficeProgramParameters parameters)
        {
            base.ReadParameters(parameters);
            Login = parameters.Login;
            Password = parameters.Password;
        }

        public override void ReadParameters(TestTaskResponseBody prm)
        {
            base.ReadParameters(prm);
            Login = prm.user;
            Password = prm.password;
        }

        public override void ReadParamsFromJson(string jsonText)
        {
            BackOfficeProgramParameters prm = JsonConvert.DeserializeObject<BackOfficeProgramParameters>(jsonText);
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

        protected void SetupVisualSettings()
        {
            LogStage("Установка области видимости");

            ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[text()='Область видимости']", "Вкладка Область видимости", "вкладки Область видимости");
            var website = GetWebElement(".//*[@class='role-form__inner']/div[1]/div[1]//input", "Нет чекбокса Веб-сайт");
            var websiteClass = website.GetAttribute("class");
            if (!websiteClass.Contains("state_checked"))
                ClickWebElement(".//*[@class='role-form__inner']/div[1]//*[@class='ui__list-node right-list__row'][1]//input", "Чекбокс Веб-сайт", "чекбокса Веб-сайт");
            ClickWebElement("//*[@class='ui__list role-list__body _style-height-auto'][last()]/div[2]//input", "Чекбокс Fonbet русский", "чекбокса Fonbet русский");
            ClickWebElement("//*[@class='ui__list role-list__body _style-height-auto'][last()]/div[3]//input", "Чекбокс Fonbet английский", "чекбокса Fonbet английский");
            ClickWebElement("//*[@class='ui__list role-list__body _style-height-auto'][last()]/div[5]//input", "Чекбокс ЦУПИС", "чекбокса ЦУПИС");
        }

        public override void BeforeRun()
        {
            base.BeforeRun();
            DoLoginBackoffice();
        }

        protected void DeleteButton()
        {
            ClickWebElement(".//*[@id='js-toolbar']/div//*[text()='Удалить']", "Кнопка Удалить", "кнопки Удалить");
            WaitTillElementisDisplayed(driver, ".//*[@class='modal__foot']/div[2]/a", 5);
            ClickWebElement(".//*[@class='modal__foot']/div[2]/a", "Кнопка Ок всплывающего окна", "кнопки Ок всплывающего окна");
        }

        protected void WaitForPageLoad()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until((wdriver) => (driver as IJavaScriptExecutor).ExecuteScript("return document.readyState").Equals("complete"));

        }

        protected void SwitchToWebsiteNewWindow(string url)
        {
            LogStage("Проверка что все элементы ставки дня отображаются на сайте");
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl(url);
            // Смена языка при необходимости
            IWebElement langSetElement = FindWebElement(".//*[@class='header__lang-set']");
            if ((langSetElement != null) && (!driver.FindElement(By.XPath(".//*[@class='header__lang-set']/a/i")).GetAttribute("class").Contains("icon_ru")))
            {
                LogStage("Смена языка на русский");
                ClickWebElement(".//*[@class='header__lang-set']", "Кнопка выбора языка", "кнопки выбора языка");
                ClickWebElement(".//*[@class='header__lang-item']//*[text()='Русский']", "Кнопка выбора русского языка", "кнопки выбора русского языка");
            }
        }
        protected void TextBeforeAndAfterQuestions(int xpathPart, string describe, string position)
        {
            LogStage("Создание нового текста " + describe + "");
            ClickWebElement(".//*[@id='js-toolbar']/div[1]/div[2]", "Кнопка Создать вопрос", "кнопки Создать вопрос");
            SetupVisualSettings();
            ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[1]", "Вкладка Общее", "вкладки Общее");
            ClickWebElement(".//*[@class='tabs__content']//*[@class='ui-dropdown__fields']//a", "Дропдаун Тип параграфа",
                "дропдауна Тип параграфа");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[" + xpathPart + "]", "Строка Текст " + describe + "",
                "строки Текст " + describe + "");
            SendKeysToWebElement(".//*[@class='role-form__inner']/label[2]//textarea", "Тестовый Ответ", "Поле Ответ",
                "поля Ответ");
            LogStage("Добавление ссылки");
            ClickWebElement(".//*[@class='form-table']//i", "Кнопка добавить ссылку", "кнопки добавить ссылку");
            SendKeysToWebElement(".//*[@class='form-table__edit-form']/div/label[1]//input", "Яндекс", "Поле Заголовок",
                "поля Заголовок");
            SendKeysToWebElement(".//*[@class='form-table__edit-form']/div/label[2]//input", "https://ya.ru", "Поле URL",
                "поля URL");
            ClickWebElement(".//*[@class='form__row']/div[1]/a", "Кнопка Применить", "кнопки Применить");
            ClickWebElement(".//*[@class='form__buttons']/div[1]/button", "Кнопка Сохранить", "кнопки Сохранить");

            SwitchToWebsiteNewWindow("http://fonred5051.dvt24.com/#!/faq");
            WaitForPageLoad();
            ExecuteJavaScript("window.location.reload()", "Дж скрипт тупит");
            WaitTillElementisDisplayed(driver, ".//*[@class='faq__top-text-inner']/*[@class='faq__top-text']", 5);
            if (!WebElementExist(".//*[@class='faq__" + position + "-text-inner']/*[@class='faq__" + position + "-text']"))
                throw new Exception("Не появился текст перед вопросом");
            if (!WebElementExist(".//*[@class='faq__" + position + "-text-inner']/*[@class='faq__links']"))
                throw new Exception("Не появились ссылки перед вопросом");
            driver.Close();
            driver.SwitchTo().Window(driver.WindowHandles[0]);
            DeleteButton();
        }
        protected void SwitchToPreView()
        {
            LogStage("Проверка отображения картинки");
            ClickWebElement(".//*[@class='form__preview-links-item'][last()]/a", "Ссылка на созданный блог", "ссылки на созданый блог");
            LogStage("Проверка что новость открывается в новом окне");
            var popup = driver.WindowHandles[1];
            if (string.IsNullOrEmpty(popup))
                throw new Exception("Не открылась запись в новом окне");
            driver.SwitchTo().Window(driver.WindowHandles[0]);
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            // Смена языка при необходимости
            IWebElement langSetElement = FindWebElement(".//*[@class='header__lang-set']");
            if (langSetElement != null)
            {
                LogStage("Смена языка на русский");
                ClickWebElement(".//*[@class='header__lang-set']", "Кнопка выбора языка", "кнопки выбора языка");
                ClickWebElement(".//*[@class='header__lang-item']//*[text()='Русский']", "Кнопка выбора русского языка", "кнопки выбора русского языка");
            }
        }

        protected void RemoveDuplicates(string xpath, string content, string eventName, string listXpath)
        {
            LogStage("Удаление дублей");
            ClickWebElement(".//*[@class='menu']//*[@href='#/explorer/content']", "Меню Управление клиентом", "меню Управление клиентом");
            ClickWebElement(xpath, "Строка "+ content, "строки " + content);
            ShowOnlyActive();
            IList<IWebElement> testData = driver.FindElements(By.XPath(listXpath));
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            for (int i = testData.Count - 1;i>=0; i--)
            {
                js.ExecuteScript("arguments[0].click()", testData[i]);
                if (testData[i].Text.Contains(eventName))
                {
                    ClickWebElement(".//*[@id='js-toolbar']/div/div[7]//button", "Кнопка Удалить", "кнопки Удалить");
                    WaitTillElementisDisplayed(driver, ".//*[@class='modal__foot']/div[2]/a", 2);
                    ClickWebElement(".//*[@class='modal__foot']/div[2]/a", "Кнопка Ок всплывающего окна", "кнопки Ок всплывающего окна");
                }
            }
        }

        protected void ContentApplicationsFilter()
        {
            for(int i=1;i<10;i++)
            {
                
                ClickWebElement(".//*[@id='js-toolbar']/div[2]/div/div[2]", "Фильтр Приложения", "фильтра Приложения");
                ClickWebElement(".//*[@class='ui-dropdown__items']/div["+i+"]", "Строка приложения", "строки  приложения");
                ClickWebElement(".//*[@id='curtain']/div/div[2]//li[1]", "Строка из выпадающего списка", "строки из выпадающего списка");
                ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[2]", "Вкладка Область видимости", "вкладки Область видимости");
                IWebElement checkbox = GetWebElement(".//*[@class='role-form__inner']/div[1]//*[@class='ui__list-node right-list__row'][" +i + "]//input", "Нет чекбокса");
                var checkboxClass = checkbox.GetAttribute("class");
                if (!checkboxClass.Contains("checked"))
                    throw new Exception("Не работает фильтр приложений");
               
            }
            ClickWebElement(".//*[@id='js-toolbar']/div[2]/div/div[2]//i", "Крестик Сбросить фильтр", "крестика Сбросить фильтр");
        }
        protected void ContentCategoriesFilter()
        {
            
            for(int i=1; i<5;i++)
            {
                ClickWebElement(".//*[@id='js-toolbar']/div[2]/div/div[3]", "Фильтр категории", "фильтра категории");
                ClickWebElement(".//*[@class='ui-dropdown__items']/div["+i+"]", "Строка категории", "строки категории");
                ClickWebElement(".//*[@id='curtain']/div/div[2]//li[1]", "Строка из выпадающего списка", "строка из выпадающего списка");
                ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[2]", "Вкладка Область видимости", "вкладки Область видимости");
                IWebElement checkbox = GetWebElement(".//*[@class='role-form__inner']/div[2]//*[@class='ui__list-node right-list__row'][" + i + "]//input", "Нет чекбокса");
                var checkboxClass = checkbox.GetAttribute("class");
                if (!checkboxClass.Contains("checked"))
                    throw new Exception("Не работает фильтр категории");
                ClickWebElement(".//*[@id='js-toolbar']/div[2]/div/div[3]//i", "Крестик Сбросить фильтр", "крестика Сбросить фильтр");
            }
        }
        protected void ContentMarketsFilter()
        {
           
           
            for (int i = 0; i < 7; i++)
            {
                ClickWebElement(".//*[@id='js-toolbar']/div[2]/div/div[4]", "Фильтр Рынки", "фильтра Рынки");
                var filterElement = driver
                    .FindElements(By.XPath(".//*[@class='ui-dropdown__items']/div"))
                    .Where(n => !n.GetAttribute("class").Contains("_type_group "))
                    .ToArray();
                filterElement[i].Click();
                string decription= String.Format("Строка '{0}' из выпадающего списка", i);
                string decriptionTwo = String.Format("строки '{0}' из выпадающего списка", i);
                ClickWebElement(".//*[@id='curtain']/div/div[2]//li[1]", decription, decriptionTwo);
                ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[2]", "Вкладка Область видимости", "вкладки Область видимости");
                IWebElement checkbox = GetWebElement(".//*[@class='role-form__inner']/div[3]//*[@class='ui__list-node right-list__row'][" + (i+1) + "]//input", "Нет чекбокса");
                var checkboxClass = checkbox.GetAttribute("class");
                if (!checkboxClass.Contains("checked"))
                    throw new Exception("Не работает фильтр рынки");
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
            Thread.Sleep(1000);
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
            Thread.Sleep(500);
            ClickWebElement(".//*[@class='fa fa-external-link-square']", "Ссылка на номер события", "ссылки на номер события");

            var imgage = driver.FindElement(By.XPath(".//*[@class='ui-lightbox__modal']"));
            Thread.Sleep(1000);
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
            WaitTillElementisDisplayed(driver, ".//*[@class='toolbar__item']//*[text()='Добавить']", 5);
            ClickWebElement(".//*[@class='toolbar__item']//*[text()='Добавить']", "Кнопка Добавить", "кнопка Добавить");
            SendKeysToWebElement(".//*[text()='СУММА']/..//input", "500", "Поле Сумма фрибета", "поля Сумма фрибета");
            SendKeysToWebElement(".//*[text()='Комментарий']/..//textarea", "test comment", "Поле Комменнтарий", "поля Сумма фрибета");
            ClickWebElement(".//*[text()='Добавить']", "Кнопка Добавить", "кнопка Добавить");
            ElementIsClickable(By.XPath(".//*[@class='cl']/div[1]//*[text()='удалить']"));
           
            ClickWebElement(".//*[@class='cl']/div[1]//*[text()='удалить']", "Кнопка Удалить", "кнопка Удалить");
            Thread.Sleep(500);
            SendKeysToWebElement(".//*[text()='Комментарий']/..//textarea", "Don`t need", "Поле Комменнтарий", "поля Сумма фрибета");
            Thread.Sleep(500);
            ClickWebElement(".//*[@class='cl__cell-value cl__cell-edit']//a", "Кнопка Удалить", "кнопка Удалить");
            var beforeCount = driver.FindElements(By.XPath(".//*[@class='cl__body cl__row']")).Count;
            ClickWebElement(".//*[text()='Показать удаленные']", "Кнопка Показать удаленные", "кнопка Показать удаленные");
            var afterCount = driver.FindElements(By.XPath(".//*[@class='cl__body cl__row']")).Count;
            if(beforeCount==afterCount)
                throw new Exception("Не работает кнопка показать удаленные");

        }

        protected void ShowOnlyActive()
        {
            WaitTillElementisDisplayed(driver, ".//*[@id='js-toolbar']/div[2]/div/div[1]", 5);
            var stateChecked = GetWebElement(".//*[@id='js-toolbar']/div[2]/div/div[1]//button", "Нет кнопки показать только актвиные");
            var stateCheckedClass = stateChecked.GetAttribute("class");
            if (!stateCheckedClass.Contains("state_checked"))
                ClickWebElement(".//*[@id='js-toolbar']/div[2]/div/div[1]", "Кнопка Показывать только активные", "кнопки Показывать только активные");
        }

        public static Func<IWebDriver, IWebElement> ElementIsClickable(By locator)
        {
            return driver =>
            {
                var element = driver.FindElement(locator);
                return (element != null && element.Displayed && element.Enabled) ? element : null;
            };
        }
    }
    
}
