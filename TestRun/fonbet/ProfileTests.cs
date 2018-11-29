using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace TestRun.fonbet
{
    class ProfileHistoryBet : FonbetWebProgram
    {
        public static CustomProgram FabricateProfileHistoryBet()
        {
            return new ProfileHistoryBet();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();

            LogStage("Проверка открытия всех вкладок меню");
            Dictionary<string,string> menu = new Dictionary<string,string>()
            {
                {"profile", "Мой профиль" },
                {"deposit","Пополнение счёта" },
                {"withdrawal","Получение выигрыша" },
                {"history","История" },
            };
            foreach (KeyValuePair<string, string> item in menu)
            {
                LogStage(String.Format("Проверка меню \"{0}\"", item.Value));
                ClickWebElement(".//*[@href='#!/account/" + item.Key + "']", "Меню \"" + item.Value + "\"", "меню \"" + item.Value + "\"");
                if (driver.Title != item.Value)
                    throw new Exception(String.Format("Страница не содержит title \"{0}\" ", item.Value));
            }

            LogStage("Проверка фильтра \"Тип пари\"");
            ClickWebElement(".//*[@class='account-calendar__row'][1]/div[1]", "Дата первого видимого дня календаря", "даты первого видимого дня календаря"); //подгружаю события минимум за прошедший месяц
            ClickWebElement(".//*[@class='ui__checkbox-text']/*[text()='Линия']", "Чекбокс Линия", "чекбокса Линия");
            IWebElement betTypeGrid = GetWebElement(".//*[@class='wrap'][1]//*[@class='operation-row _odd']/div[4]", "Не отображается Тип пари в гриде");
            string betTypeGridText = betTypeGrid.Text;
            if (!(betTypeGridText.Equals("Фрибет") || betTypeGridText.Equals("Суперэкспресс")))
                throw new Exception("Добавился новый тип пари, кроме фрибета и линии");
            ClickWebElement(".//*[@class='ui__checkbox-text']/*[text()='Линия']", "Чекбокс Линия", "чекбокса Линия");
            var bets = driver.FindElements(By.XPath(".//*[@class='account-filter']/div[1]//input"));
            for (int i = 0; i < bets.Count; i++)
                bets[i].Click();
            if (!WebElementExist(".//*[@class='page-account__empty-list-text']"))
                throw new Exception("Добавился новый тип пари, кроме фрибета, суперэкспресса или линии, либо фильтры не работают");
            for (int i = 0; i < bets.Count; i++)
                bets[i].Click();


            LogStage("Снятие всех фильтров в столбце \"Результат\"");
            IList<IWebElement> all = driver.FindElements(By.XPath(".//*[@class='account-filter']/div[2]//*[@class='ui__checkboxes']/div")); //Все чекбоксы в столбце Результат
            for (var i = 1; i <= all.Count; i++)
            {
                string nameTofind = string.Format(".//*[@class='account-filter']/div[2]//*[@class='ui__checkboxes']/div[{0}]//*[@class='ui__checkbox-text']/span", i);
                var checkbox = driver.FindElement(By.XPath(nameTofind));
                checkbox.Click();
            }
            LogStage("Проверка работы всех фильтров в столбце \"Результат\"");
            for (var i = 1; i <= all.Count; i++)
            {
                string nameTofind = string.Format(".//*[@class='account-filter']/div[2]//*[@class='ui__checkboxes']/div[{0}]//*[@class='ui__checkbox-text']/span", i);
                string dataText = driver.FindElement(By.XPath(nameTofind)).Text;
                ClickWebElementWithText("ui__checkbox-text", dataText, "Чекбокс", "чекбокса");
          
                if (!WebElementExist(".//*[@class='page-account__empty-list-text']"))
                {
                    WaitTillElementisDisplayed(driver, ".//*[@class='wrap'][1]//*[@class='operation-row _odd']/div[6]/span[1]", 120);
                    IWebElement betresultGrid = GetWebElement(".//*[@class='wrap'][1]//*[@class='operation-row _odd']/div[6]/span[1]", "Не отображается Результат пари в гриде");
                    string betresultGridText = betresultGrid.Text;
                    if (!betresultGridText.Equals(dataText))
                        throw new Exception("Не работают фильтры Результата пари");
                }
                ClickWebElementWithText("ui__checkbox-text", dataText, "Чекбокс", "чекбокса");
               
            }

            LogStage("Проверка развертки конкретного события");
            ClickWebElement(".//*[@class='ui__checkbox-text']/*[text()='Выигрыш']", "Чекбокс Выигрыш", "чекбокса Выигрыш");
            WaitTillElementisDisplayed(driver, ".//*[@class='wrap'][1]//*[@class='operation-row _odd']/div[6]/span[1]", 120);
            ClickWebElement(".//*[@class='wrap'][1]//*[@class='operation-row _odd']/div[7]", "Стрелка разворота события", "стрелки разворота события");
            IWebElement betValue = GetWebElement(".//*[@class='bet-details _odd']//table//*[text()='Выигрыш']", "Не отображается Результат пари в развернутом гриде");
            string betValueText = betValue.Text;
            if (!betValueText.Equals("Выигрыш"))
                throw new Exception("Не верный результат в развернутом пари");

            LogStage("Проверка свертки/развертки календаря");
            ClickWebElement(".//*[@class='account-history-bets__right']/div[1]//*[@class='account-block__head']/div", "Стрелка разворота календаря", "стрелки разворота календаря");
            IWebElement calendr = GetWebElement(".//*[@class='account-history-bets__right']/div[1]//*[@class='account-block__head']/div", "Не отображается стрелка у календаря");
            var calendrClass = calendr.GetAttribute("class");
            if (calendrClass.Contains("expanded"))
                throw new Exception("Не работает свертка Календаря");

            LogStage("Проверка свертки/развертки фильтра");
            ClickWebElement(".//*[@class='account-history-bets__right']/div[2]//*[@class='account-block__head']/div", "Стрелка разворота фильтра", "стрелки разворота фильтра");
            IWebElement filter = GetWebElement(".//*[@class='account-history-bets__right']/div[2]//*[@class='account-block__head']/div", "Не отображается стрелка у календаря");
            var filterClass = filter.GetAttribute("class");
            if (filterClass.Contains("expanded"))
                throw new Exception("Не работает свертка Фильтра");

            LogStage("Проверка возможности скрыть бланас");
            ClickWebElement(".//*[@class='header__login-head']/div[1]/span", "ФИО в шапке", "ФИО в шапке");
            ClickWebElement(".//*[@id='popup']/li[4]", "Кнопка Скрыть баланс", "кнопки Скрыть баланс");
            if (WebElementExist("//*[@href='/#!/account/deposit']"))
                throw new Exception("Не работают скрытие Баланса аккаунта");
        }
    }

    class ProfileHistoryOperation : FonbetWebProgram
    {
        public static CustomProgram FabricateProfileHistoryOperation()
        {
            return new ProfileHistoryOperation();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();

            LogStage("Проверка разворачивания/сворачивания меню в личном кабинете");
            if (WebElementExist(".//*[@class='account-menu__icon _switcher']"))
            {
                ClickWebElement(".//*[@class='account-menu__icon _switcher']", "Стрелка сворачивания меню в личном кабинете", "стрелка сворачивания меню в личном кабинете");
                if (WebElementExist(".//*[@class='account-menu__user-name']"))
                    throw new Exception("Не работает сворачивание меню в личном кабинете");
                ClickWebElement(".//*[@class='account-menu__link _switcher']", "Стрелка разворота меню в личном кабинете", "стрелка разворота меню в личном кабинете");
            }
            else
            {
                ClickWebElement(".//*[@class='account-menu__link _switcher']", "Стрелка разворота меню в личном кабинете", "стрелки разворота меню в личном кабинете");
            };

            LogStage("Переход во вкладку \"Операции\"");
            ClickWebElement(".//*[@href='#!/account/history']", "Меню История", "меню История");
            Thread.Sleep(2000);
            ClickWebElement(".//*[@class='account-calendar__row'][1]/div[1]", "Дата первого видимого дня календаря", "даты первого видимого дня календаря"); //подгружаю события минимум за прошедший месяц
            Thread.Sleep(2000);
            waitTillElementContains(driver, ".//*[@class='account-calendar__row'][1]/div[1]", "loaded");
            ClickWebElement(".//*[@href='#!/account/history/operations']", "Вкладка Операции", "вкладки Операции");

            LogStage("Снятие всех фильтров в столбце \"Тип операции\"");
            IList<IWebElement> all = driver.FindElements(By.XPath(".//*[@class='ui__checkboxes']/div")); //Все доступные типы операций в фильтре
            for (var i = 1; i <= all.Count; i++)
            {
                string nameTofind = string.Format(".//*[@class='ui__checkboxes']/div[{0}]//*[@class='ui__checkbox-text']/span", i);
                var element = driver.FindElement(By.XPath(nameTofind));
                element.Click();
                Thread.Sleep(500);
            }

            LogStage("Проверка работы всех фильтров в столбце \"Тип операции\"");
            for (var i = 1; i <= all.Count; i++)
            {
                string nameTofind = string.Format(".//*[@class='ui__checkboxes']/div[{0}]//*[@class='ui__checkbox-text']/span", i);
                var element = driver.FindElement(By.XPath(nameTofind)).Text;
               


                ClickWebElementWithText("ui__checkbox-text", element, "Чекбокс", "чекбокса");
                if (!WebElementExist(".//*[@class='page-account__empty-list-text']"))
                {
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(50));
                    wait.Until(ExpectedConditions.ElementIsVisible(By.XPath(".//*[@class='operation-row _odd']/div[4]")));
                    IWebElement betoperationGrid = GetWebElement(".//*[@class='operation-row _odd']/div[4]", "Не отображается Операции в гриде");
                    string betoperationGridText = betoperationGrid.Text;
                    if (!betoperationGridText.Equals(element))
                        throw new Exception("Не работают фильтры Типа операций");
                }
                Thread.Sleep(200);
                ClickWebElementWithText("ui__checkbox-text", element, "Чекбокс", "чекбокса");
            }

            LogStage("Проверка развертки конкретной операции");
            ClickWebElement(".//*[@class='ui__checkbox-text']/*[text()='Заключено пари']", "Чекбокс заключено пари", "чекбокса заключено пари");
            WaitTillElementisDisplayed(driver, ".//*[@class='wrap'][1]//*[@class='operation-row _odd']/div[7]", 30);
            ClickWebElement(".//*[@class='wrap'][1]//*[@class='operation-row _odd']/div[7]", "Стрелка разворота операции", "Стрелка разворота операции");
            IWebElement betMark = GetWebElement(".//*[@class='bet-details _odd']//table//*[text()='Коэффициент']", "Не отображается коэффициент в развернутом гриде");
            string betMarkText = betMark.Text;
            if (!betMarkText.Equals("Коэффициент"))
                throw new Exception("Нет коэфф в развернутой операции");
        }
    }
    class AuthorizationChecker : FonbetWebProgram
    {
        public static CustomProgram FabricateAuthorizationChecker()
        {
            return new AuthorizationChecker();
        }

        protected override bool NeedLogin()
        {
            return false;
        }

        public override void Run()
        {
            base.Run();

            LogStage("Проверка плейсхолдера Номер счета");
            ClickWebElement(".//*[@class='header__login-head']/a", "Кнопка Войти", "кнопки Войти");
            SendKeysToWebElement(".//*[@class='login-form__form']/div[1]/input","11","Поле логина", "поля логина");
            IWebElement loginPlace = GetWebElement(".//*[@class='login-form__types-container']/div[2]", "Нет поля логина");
            var loginPlaceClass = loginPlace.GetAttribute("class");
            if (!loginPlaceClass.Contains("active"))
                throw new Exception("Не переключается на placeholder с номером счета");

            LogStage("Проверка плейсхолдера Почты");
            ClearBeforeInput(".//*[@class='login-form__form']/div[1]/input");
            SendKeysToWebElement(".//*[@class='login-form__form']/div[1]/input", "ya@ya.ru", "Поле логина", "поля логина");
            IWebElement mailPlace = GetWebElement(".//*[@class='login-form__types-container']/div[3]", "Нет поля логина");
            var mailPlaceClass = mailPlace.GetAttribute("class");
            if (!mailPlaceClass.Contains("active"))
                throw new Exception("Не переключается на placeholder с почтой");

            LogStage("Проверка плейсхолдера Телефона");
            ClearBeforeInput(".//*[@class='login-form__form']/div[1]/input");
            SendKeysToWebElement(".//*[@class='login-form__form']/div[1]/input", "+79991234567", "Поле логина", "поля логина");
            IWebElement phonePlace = GetWebElement(".//*[@class='login-form__types-container']/div[1]", "Нет поля логина");
            var phonePlaceClass = phonePlace.GetAttribute("class");
            if (!phonePlaceClass.Contains("active"))
                throw new Exception("Не переключается на placeholder с телефоном");

            LogStage("Проверка ошибки входа");
            SendKeysToWebElement(".//*[@class='login-form__form']/div[2]/input", "gg", "Поле пароля", "поля паоля");
            ClickWebElement(".//*[@class='login-form']//*[@class='toolbar__item']", "Кнопка логина", "кнопки логина");
            if (!WebElementExist(".//*[@class='login-form__error']"))
                throw new Exception("Не высвечивается сообщение о некорретном логине/пароле");
        }
    }
    class PwdRecovery : FonbetWebProgram
    {
        public static CustomProgram FabricatePwdRecovery()
        {
            return new PwdRecovery();
        }

        protected override bool NeedLogin()
        {
            return false;
        }

        public override void Run()
        {
            base.Run();
           
            RejectPwdChecker("000000001", "0","12");
            RejectPwdChecker("000000002", "0","10");
            RejectPwdChecker("000000003", "0","4");
            RejectPwdChecker("000000004", "0","1");

            LogStage("Переход на страницу восстановление пароля");
            ClickWebElement(".//*[@href='/#!/account/restore-password']", "Кнопка Забыли пароль", "кнопки забыли пароль");

            LogStage("Проверка sendCode по тестовому сценарию на error 10");
            WaitTillElementisDisplayed(driver, "//*[@class='toolbar__item']//button", 5);
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[2]//input", "000000005", "Поле номер телефона", "поля номер телефона");
            Thread.Sleep(500);
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[4]//input", "1", "Поле капча", "поля капчи");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            WaitTillElementisDisplayed(driver, ".//*[@class='toolbar__item']//button", 5);
            Thread.Sleep(500);
            SendKeysToWebElement(".//*[@class='ui__field']", "123123", "Поле Код подтверждения", "поля Код подтверждения");
            Thread.Sleep(500);
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            WaitTillElementisDisplayed(driver, ".//*[@id='restore-password-error']", 15);
            var errorMessage = GetWebElement(".//*[@id='restore-password-error']", "Нет сообщения об ошибке");
            if (!errorMessage.GetAttribute("data-errorcode").Equals("10"))
                throw new Exception("Неверный текст ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");

            LogStage("Проверка sendPassword на reject");
            ClearBeforeInput(".//*[@class='ui__field']");
            SendKeysToWebElement(".//*[@class='ui__field']", "123456", "Поле Код подтверждения", "поля Код подтверждения");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            WaitTillElementisDisplayed(driver, ".//*[@class='change-password__form-inner']/div/div[1]//input", 5);
            Thread.Sleep(500);
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[1]//input", "1234567Q", "Поле Новый пароль", "поля Новый пароль");
            Thread.Sleep(500);
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[2]//input", "1234567Q", "Поле Повторите новый пароль", "поля Повторите новый пароль");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            var errorText = GetWebElement(".//*[@id='restore-password-error']", "Нет текста ошибки");
            if (!errorText.GetAttribute("data-errorcode").Equals("0") && errorText.GetAttribute("data-rejectioncode").Equals("1"))
                throw new Exception("Неверный текст ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");

            LogStage("Проверка sendPassword на complete");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[2]//input", "000000005", "Поле номер телефона", "поля номер телефона");
            Thread.Sleep(500);
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[4]//input", "1", "Поле капча", "поля капчи");
            Thread.Sleep(500);
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            Thread.Sleep(500);
            SendKeysToWebElement(".//*[@class='ui__field']", "123456", "Поле Код подтверждения", "поля Код подтверждения");
            Thread.Sleep(500);
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            Thread.Sleep(500);
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[1]//input", "!23qweQWE", "Поле Новый пароль", "поля Новый пароль");
            Thread.Sleep(500);
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[2]//input", "!23qweQWE", "Поле Повторите новый пароль", "поля Повторите новый пароль");
            Thread.Sleep(500);
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            var message = GetWebElement(".//*[@class='account-error__title']", "Нет title ошибки");
            if (!WebElementExist(".//*[@class='account__content']//span"))
                throw new Exception("Нет кнопки \"Войти на сайт\"");
            if (!message.Text.Contains("Пароль успешно изменен."))
                throw new Exception("Тестовое восстановление пароля не удалось");

        }
    }
    class EmailConfirm : FonbetWebProgram
    {
        public static CustomProgram FabricateEmailConfirm()
        {
            return new EmailConfirm();
        }

        public override void Run()
        {
            base.Run();

            ClickOnAccount();
            ClickWebElement(".//*[@href='#!/account/profile/change-email']", "Вкладка Смена email", "вкладки смена email");
           
            CreateProcessemailChecker("1@dev.dev","rejected","0","14");
            CreateProcessemailChecker("2@dev.dev", "rejected", "0", "13");
            CreateProcessemailChecker("3@dev.dev", "rejected", "0", "12");

            LogStage("Проверка sendCode по тестовому сценарию");
            Thread.Sleep(1000);
            ExecuteJavaScript("window.location.reload()", "Дж скрипт тупит");
            Thread.Sleep(1000);
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input", "4@dev.dev", "Поле email", "поля email");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
            SendEmailCodeChecker("1235", "waitForCode","10", null);
            Thread.Sleep(1000);
            SendEmailCodeChecker("9999","rejected","0","1");
            ClearBeforeInput(".//*[@class='ui__field-inner']/input");
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input", "4@dev.dev", "Поле email", "поля email");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
            ClearBeforeInput(".//*[@class='ui__field-inner']/input");
            Thread.Sleep(1000);
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input", "1234", "Поле ввода кода", "поля ввода кода");
            Thread.Sleep(1000);
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки отправить");
            WaitTillElementisDisplayed(driver, ".//*[@classid='account-error__btn-inner']//span", 5);
           if(!WebElementExist(".//*[@class='account-error _type_success _style_bordered']"))
                throw new Exception("Ошибка на финальном шаге. Нет поздравительного сообщения");
            ClickWebElement(".//*[@classid='account-error__btn-inner']//span", "Кнопка Вернуться к профилю", "кнопки Вернуться к профилю");
            var mainTab = GetWebElement(".//*[@class='account-tabs']/a[1]", "Нет вкладки Основные данные");
            var mainTabClass = mainTab.GetAttribute("class");
            if (!mainTabClass.Contains("state_active"))
                throw new Exception("Не произошло возвращения на главную страницу");
        }
    }

    class RegistrationV4 : FonbetWebProgram 
    {
        public static CustomProgram FabricateRegistrationV4()
        {
            return new RegistrationV4();
        }

        protected override bool NeedLogin()
        {
            return false;
        }
        public override void Run()
        {
            base.Run();

            if (!WebElementExist(".//*[@href='/#!/account/registration']"))
                throw new Exception("На данном сайте нет супер-регистрации");

            LogStage("Переход на страницу регистрации");
            ClickWebElement(".//*[@href='/#!/account/registration']", "Кнопка Регистрации", "кнопки Регистрации");
            var required = driver.FindElements(By.XPath(".//*[@class='ui__required']"));
            if (required.Count != 4)
                throw new Exception("Число обязательных полей не равно 4");

            LogStage("Заполнение персональных данных");
            SendKeysToWebElement(".//*[@class='reg-v4__form-column--31oQE']/div[1]//input", "Тест", "Поле Имя", "поля Имя");
            ClickWebElement(".//*[@id='checkbox2']", "Чекбокс Подтверждения правил", "чекбокса Подтверждения правил");
            IWebElement button = GetWebElement(".//*[@class='reg-v4__form-row--1HvrA _form-buttons--3mZsY']//button", "Нет кнопки отправить форму");
            if (!button.GetAttribute("class").Contains("state_disabled"))
                throw new Exception("Возможно продолжить только с Именем ");
            LogStage("Заполнение пароля");
            SendKeysToWebElement(".//*[@class='reg-v4__form-column--31oQE']/div[5]//input", "123qwe123", "Поле Пароль", "поля Пароль");
            if (!button.GetAttribute("class").Contains("state_disabled"))
                throw new Exception("Возможно продолжить без email и номера телефона");
            LogStage("Заполнение контактной информации");
            SendKeysToWebElement(".//*[@class='reg-v4__form-column--31oQE']/div[3]//input", "4000100@mail.ru", "Поле email", "поля email");
            if (!button.GetAttribute("class").Contains("state_disabled"))
                throw new Exception("Возможно продолжить без телефона");
            Thread.Sleep(500);
            SendKeysToWebElement(".//*[@class='reg-v4__form-column--31oQE']/div[2]//input", Keys.Home+ "000000001", "Поле Номер телефона", "поля Номер телефона");
            ClearBeforeInput(".//*[@class='reg-v4__form-column--31oQE']/div[3]//input");
            driver.FindElement(By.XPath(".//*[@class='reg-v4__form-column--31oQE']/div[3]//input")).SendKeys("a");
            driver.FindElement(By.XPath(".//*[@class='reg-v4__form-column--31oQE']/div[3]//input")).SendKeys(Keys.Backspace);
            if (!button.GetAttribute("class").Contains("state_disabled"))
                throw new Exception("Возможно продолжить без почты");
            SendKeysToWebElement(".//*[@class='reg-v4__form-column--31oQE']/div[3]//input", "4000100@mail.ru", "Поле email", "поля email");
            Thread.Sleep(500);
            ClickWebElement(".//*[@class='reg-v4__form-row--1HvrA _form-buttons--3mZsY']//button", "Кпонка Продолжить", "кпонки Продолжить");
            var errorMessage = GetWebElement(".//*[@id='reg-v4-cupis-error']", "Нет модуля с ошибкой");
            if (!errorMessage.GetAttribute("data-rejectioncode").Equals("4") && !errorMessage.GetAttribute("data-processstate").Equals("rejected") && !errorMessage.GetAttribute("data-errorcode").Equals("0"))
                throw new Exception("Неверная обработка ошибки");
            ClickWebElement(".//*[@class='reg-v4__error-buttons--1X_YP']//a", "Кнопка Повторить", "кнопки Повторить");

            CreateProcessRegistration("000000002", null, "2", null);
            CreateProcessRegistration("000000003", null, null, null);
            CreateProcessRegistration("000000004", "rejected", "0", "11");
            CreateProcessRegistration("000000005", "rejected", "0", "13");
            CreateProcessRegistration("000000006", "rejected", "0", "10");
            CreateProcessRegistration("000000007", "rejected", "0", "1");

            ClearBeforeInput(".//*[@class='reg-v4__form-column--31oQE']/div[2]//input");
            SendKeysToWebElement(".//*[@class='reg-v4__form-column--31oQE']/div[2]//input", "000000008", "Поле Номер телефона", "поля Номер телефона");
            ClickWebElement(".//*[@class='reg-v4__form-row--1HvrA _form-buttons--3mZsY']//button", "Кпонка Продолжить", "кпонки Продолжить");
            SendSmsCodeRegistration("1", "waitForSmsCode", "10", null);
            SendSmsCodeRegistration("2", "waitForSmsCode", "11", null);
            SendSmsCodeRegistration("3", "rejected", "10", "12");
            SendSmsCodeRegistration("4", "rejected", "0", "10");
            SendSmsCodeRegistration("5", "rejected", "0", "14");
            SendSmsCodeRegistration("6", "rejected", "0", "15");
            SendSmsCodeRegistration("7", "rejected", "0", "16");
            SendSmsCodeRegistration("8", null, null, null);
            SendSmsCodeRegistration("0", null, null, null);
            SendSmsCodeRegistration("9", null, null, null);
        }
    }

    class VerificationCupisQiwi : FonbetWebProgram //захожу пот 13 учеткой на 5051  и смотрю чтобы была галка в админке
    {
        public static CustomProgram FabricateVerificationCupisQiwi()
        {
            return new VerificationCupisQiwi();
        }

        public override void Run()
        {
            base.Run();
            VerificationStatusCheck();

            LogStage("Проверка createProcess по тестовому сценарию");
            if (WebElementExist(".//*[@class='verification__notice-types-wrap']//*[@class='toolbar__btn-text']"))
            {
                ClickWebElement(".//*[@class='verification__notice-types-wrap']//*[@class='toolbar__btn-text']", "Кнопка Продолжить верификацию",
                    "кнопки Продолжить верификацию");
                ClickWebElement(".//*[@class='verification__form-row']/div[2]/a", "Кнопка Отменить",
                   "кнопки Отменить");
                ClickWebElement(".//*[@class='confirm__foot--3H8gD']/div[2]/a", "Кнопка Подтверждения отмены",
                  "кнопки Подтверждения отмены");
            }

            ClickWebElement(".//*[@class='verification__tab']/div[3]", "Кнопка Верификации по киви",
                "кнопки Верификации по киви");
            IWebElement inputData = GetWebElement(".//*[@class='ui__field-wrap-inner']//input", "Нет поля для ввода");
            Thread.Sleep(1500);
            ClearBeforeInput(".//*[@class='ui__field-wrap-inner']//input");
            SendKeysToWebElement(".//*[@class='ui__field-wrap-inner']//input", "000000002", "Поле Номер телефона",
                "поля Номер телефона");
            ClickWebElement(".//*[@id='rulesAgree']", "Чекбокс Соглашения с правилами",
                "чекбокс Соглашения с правилами");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
            var errorMessage = GetWebElement(".//*[@id='verification-qiwi-error']", "Нет текста ошибки");
            if (!(errorMessage.GetAttribute("data-errorcode").Equals("0") && errorMessage.GetAttribute("data-processstate").Equals("rejected") && errorMessage.GetAttribute("data-rejectioncode").Equals("11")))
                throw new Exception("Неверная обработка ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");

            CreateProcessVerificationQiwi("3", "rejected", "0", "4");
            CreateProcessVerificationQiwi("4", null, "2", null);
            CreateProcessVerificationQiwi("5", "rejected", "0", "10");
            CreateProcessVerificationQiwi("6", "rejected", "0", "12");
            CreateProcessVerificationQiwi("7", "rejected", "0", "15");
            CreateProcessVerificationQiwi("8", "rejected", "0", "1");

            driver.FindElement(By.XPath(".//*[@class='ui__field-wrap-inner']//input")).SendKeys(Keys.Backspace);
            SendKeysToWebElement(".//*[@class='ui__field-wrap-inner']//input", "9", "Поле Номер телефона",
                "поля Номер телефона");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");

            SendSmsVerificationQiwi("2", "rejected", "0", "10");
            SendSmsVerificationQiwi("3", "rejected", "0", "14");
            SendSmsVerificationQiwi("4", "rejected", "0", "13");
            SendSmsVerificationQiwi("5", "rejected", "0", "15");
            SendSmsVerificationQiwi("6", "rejected", "0", "1");

            ClearBeforeInput(".//*[@class='ui__field-wrap-inner']//input");
            SendKeysToWebElement(".//*[@class='ui__field-wrap-inner']//input", "7", "Поле Номер телефона", "поля Номер телефона");
            Thread.Sleep(1000);
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");

            SendPasportVerificationQiwi("2222222222", null, "2", null);
            SendPasportVerificationQiwi("3333333333", "waitForPassport", "10", null);
            SendPasportVerificationQiwi("4444444444", "rejected", "0", "17");
            SendPasportVerificationQiwi("5555555555", "rejected", "0", "1");
            SendPasportVerificationQiwi("6666666666", null, null, null);
        }

    }
    class VerificationCupisBk : FonbetWebProgram //захожу пот 13 учеткой на 5051  и смотрю чтобы была галка в админке
    {
        public static CustomProgram FabricateVerificationCupisBk()
        {
            return new VerificationCupisBk();
        }

        public override void Run()
        {
            base.Run();
            VerificationStatusCheck();

            LogStage("Проверка createProcess по тестовому сценарию");
            if (WebElementExist(".//*[@class='verification__notice-types-wrap']//span")) //если процесс уже существует
            {
                ClickWebElement(".//*[@class='verification__notice-types-wrap']//span", "Кнопка Продолжить",
                    "кнопки Продолжить");
                if (WebElementExist(".//*[@class='toolbar__btn-text'][text()='Отменить']"))
                {
                    ClickWebElement(".//*[@class='toolbar__btn-text'][text()='Отменить']", "Кнопка Отменить", "кнопки Отменить");
                    ClickWebElement("//div[contains(@class,'confirm__foot--3H8gD')]/div[2]//a", "Кнопка Да", "кнопки Да");
                }
                else ExecuteJavaScript("app.accountManager.cancelWaitingVerificationProcess();", "Не убился процесс идентификации");
            }

            ClickWebElement(".//*[@class='verification__tab']/div[2]", "Кнопка Верификации по БК",
                "кнопки Верификации по БК");
            IWebElement inputData = GetWebElement(".//*[@class='verification__form-inner']/div/div[2]//input", "Нет поля номера карты фонбет");

            ClearBeforeInput(".//*[@class='verification__form-inner']/div/div[2]//input");
            SendKeysToWebElement(".//*[@class='verification__form-inner']/div/div[2]//input", "0000FFFF0002", "Поле Номера карты фонбет","поля Номера карты фонбет");
            Thread.Sleep(500);
            SendKeysToWebElement(".//*[@class='verification__form-inner']/div/div[3]/label[1]//input", Keys.Home + "2222222222", "Поле Серия и номер паспорта", "поля Серия и номер паспорта");
            Thread.Sleep(500);
            SendKeysToWebElement(".//*[@class='verification__form-inner']/div/div[3]/label[2]//input", Keys.Home + "11112011", "Поле Дата выдачи", "поля Дата выдачи");
            ClickWebElement(".//*[@id='rulesAgree']", "Чекбокс Соглашения с правилами", "чекбокс Соглашения с правилами");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");


            var errorMessage = GetWebElement(".//*[@id='verification-bk-error']", "Нет текста ошибки");
            if (!(errorMessage.GetAttribute("data-errorcode").Equals("0") && errorMessage.GetAttribute("data-processstate").Equals("rejected") && errorMessage.GetAttribute("data-rejectioncode").Equals("11")))
                throw new Exception("Неверная обработка ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");


            CreateProcessVerificationBk("3", "rejected", "0", "12");
            CreateProcessVerificationBk("4", "rejected", "0", "13");
            CreateProcessVerificationBk("5", "rejected", "0", "18");
            CreateProcessVerificationBk("6", "rejected", "0", "4");
            CreateProcessVerificationBk("7", null, "2", null);
            CreateProcessVerificationBk("8", "rejected", "0", "14");
            CreateProcessVerificationBk("9", "rejected", "0", "10");
            Thread.Sleep(1000);
            driver.FindElement(By.XPath(".//*[@class='verification__form-inner']/div/div[2]//input")).SendKeys(Keys.Backspace);
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");

            SendSmsVerificationBK("2", null, "2", null);
            SendSmsVerificationBK("3", "rejected", "0", "15");
            SendSmsVerificationBK("4", "rejected", "0", "17");
            SendSmsVerificationBK("5", "rejected", "0", "19");
            SendSmsVerificationBK("6", "rejected", "0", "10");
            SendSmsVerificationBK("7", "rejected", "0", "16");
            SendSmsVerificationBK("8", "rejected", "0", "1");
            SendSmsVerificationBK("0", null, null, null);
            SendSmsVerificationBK("9", null, null, null);
        }
    }

    class RemoteVerification : FonbetWebProgram //захожу пот 13 учеткой на 5051  и смотрю чтобы была галка в админке
    {
        public static CustomProgram FabricateRemoteVerification()
        {
            return new RemoteVerification();
        }

        public override void Run()
        {
            base.Run();
            VerificationStatusCheck();

            LogStage("Проверка createProcess по тестовому сценарию");
            string[,] arrayProcess = new string[4, 4] { { "2", "0","rejected","11" }, { "3", "0","rejected","4"},
                    {"4", "2",null,null }, {"5", "0","rejected","10" } };
            for (int i = 0; i < 4; i++)
            {
                driver.Navigate().GoToUrl("http://fonred5051.dvt24.com/?test=" + arrayProcess[i, 0] + "#!/account/verification/remote-verification");
                if (WebElementExist("//*[@class='verification__form-row _gap-30']/div[2]"))
                {
                    ClickWebElement("//*[@class='verification__form-row _gap-30']/div[2]", "Кнопка Отменить", "кнопки Отменить");
                    ClickWebElement("//*[@class='confirm__foot--3H8gD']/div[2]/a", "Кнопка Да", "кнопки Да");
                }
                else
                    ClickWebElement("//*[@class='verification__tab']/div[1]", "Кнопка Удаленной идентификации", "кнопки Удаленной идентификации");
                MakeDisplayInhert("//*[@class='verification__form-lock-container']/div[1]//input");

                SendKeysToWebElement("//*[@class='verification__form-lock-container']/div[1]//input", "C:\\Users\\User\\Downloads\\саша.jpg", "Поле Главный разворот страницы", "поля Главный разворот страницы");
                LogStage("Проверка на валидность аттача");
                MakeDisplayInhert("//*[@class='verification__form-lock-container']/div[2]//input");
                SendKeysToWebElement("//*[@class='verification__form-lock-container']/div[2]//input", "C:\\Users\\User\\Downloads\\auto-tests.zip", "Поле Разворот с регистрацией", "поля Разворот с регистрацией");
                if (!driver.FindElement(By.XPath("//*[@class='verification__form-lock-container']/div[2]//*[@class='upload-file__inner--ds5yK']/div[2]")).GetAttribute("class").Contains("error"))
                    throw new Exception("Получилось прикрепить зип фаил");
                ClickWebElement("//*[@class='verification__form-lock-container']/div[2]//*[@class='upload-file__inner--ds5yK']/i[2]", "Крестик удаления файла", "крестика удаления файла");
                SendKeysToWebElement("//*[@class='verification__form-lock-container']/div[2]//input", "C:\\Users\\User\\Documents\\24H soft.png", "Поле Разворот с регистрацией", "поля Разворот с регистрацией");
                ClickWebElement("//*[@class='toolbar__item _reg-button']", "Кнопка Далее", "кнопки Далее");
                WaitTillElementisDisplayed(driver, ".//*[@id='rv-error']", 10);
                var message = GetWebElement(".//*[@id='rv-error']", "Нет модуля с ошибкой");
                if (arrayProcess[i, 2] == null && arrayProcess[i, 3] == null)
                {
                    if (!(message.GetAttribute("data-errorcode").Equals(arrayProcess[i, 1])))
                        throw new Exception("Неверная обработка ошибки");
                    ClickWebElement(".//*[@class='verification__error-buttons']/div", "Кнопка Повторить", "кнопки Повторить");
                }
                else
                {
                    if (!(message.GetAttribute("data-errorcode").Equals(arrayProcess[i, 1]) && message.GetAttribute("data-processstate").Equals(arrayProcess[i, 2]) && message.GetAttribute("data-rejectioncode").Equals(arrayProcess[i, 3])))
                        throw new Exception("Неверная обработка ошибки");
                    ClickWebElement(".//*[@class='verification__error-buttons']/div", "Кнопка Повторить", "кнопки Повторить");
                }
            }
            driver.Navigate().GoToUrl("http://fonred5051.dvt24.com/?test=6#!/account/verification/remote-verification");
            ClickWebElement("//*[@class='verification__tab']/div[1]", "Кнопка Удаленной идентификации", "кнопки Удаленной идентификации");
            MakeDisplayInhert("//*[@class='verification__form-lock-container']/div[1]//input");
            SendKeysToWebElement("//*[@class='verification__form-lock-container']/div[1]//input", "C:\\Users\\User\\Downloads\\саша.jpg", "Поле Главный разворот страницы", "поля Главный разворот страницы");
            MakeDisplayInhert("//*[@class='verification__form-lock-container']/div[2]//input");
            SendKeysToWebElement("//*[@class='verification__form-lock-container']/div[2]//input", "C:\\Users\\User\\Documents\\24H soft.png", "Поле Разворот с регистрацией", "поля Разворот с регистрацией");


            LogStage("Проверка SendSmsCode по тестовому сценарию");

            string[,] arraySms = new string[5, 4] { { "2", "0","rejected","14" }, { "3", "0","rejected","15"},
                    {"4", "0","rejected","16" }, {"5", "0","rejected","17" }, {"6", "0","rejected","19" } };
            for (int i = 0; i < 5; i++)
            {
                WaitTillElementisDisplayed(driver, "//*[@class='toolbar__item _reg-button']", 10);
                Thread.Sleep(1000);
                ClickWebElement("//*[@class='toolbar__item _reg-button']", "Кнопка Далее", "кнопки Далее");
                WaitTillElementisDisplayed(driver, ".//*[@class='ui__field verification__sms-field']", 10);
                SendKeysToWebElement(".//*[@class='ui__field verification__sms-field']", arraySms[i, 0], "Поле SMS кода", "поля SMS кода");
                Thread.Sleep(1000);
                ClickWebElement(".//*[@class='toolbar__item _reg-button']/button//span", "Кнопка Отправить", "кнопки Отправить");
                if (arraySms[i, 0] == "6")
                {
                    if (!WebElementExist("//*[@class='verification__form-row']"))
                        throw new Exception("Не появился процесс по киви");
                    WaitTillElementisDisplayed(driver, "//*[@class='verification__form-row']/div[2]/a", 8);
                    ClickWebElement("//*[@class='verification__form-row']/div[2]/a", "Кнопка Отменить", "кнопки Отменить");
                    ClickWebElement("//*[@class='confirm__foot--3H8gD']/div[2]/a", "Кнопка Да", "кнопки Да");
                }
                else
                {
                    WaitTillElementisDisplayed(driver, ".//*[@id='rv-error']", 10);
                    var message = GetWebElement(".//*[@id='rv-error']", "Нет модуля с ошибкой");
                    if (!(message.GetAttribute("data-errorcode").Equals(arraySms[i, 1]) && message.GetAttribute("data-processstate").Equals(arraySms[i, 2]) && message.GetAttribute("data-rejectioncode").Equals(arraySms[i, 3])))
                        throw new Exception("Неверная обработка ошибки, когда логин равен " + arraySms[i, 0] + "");
                    ClickWebElement(".//*[@class='verification__error-buttons']/div", "Кнопка Повторить", "кнопки Повторить");
                }
            }

            LogStage("Проверка selectTimeSlot по тестовому сценарию");

            driver.Navigate().GoToUrl("http://fonred5051.dvt24.com/?test=6#!/account/verification/remote-verification");
            ClickWebElement("//*[@class='verification__tab']/div[1]", "Кнопка Удаленной идентификации", "кнопки Удаленной идентификации");
            MakeDisplayInhert("//*[@class='verification__form-lock-container']/div[1]//input");
            SendKeysToWebElement("//*[@class='verification__form-lock-container']/div[1]//input", "C:\\Users\\User\\Downloads\\саша.jpg", "Поле Главный разворот страницы", "поля Главный разворот страницы");
            MakeDisplayInhert("//*[@class='verification__form-lock-container']/div[2]//input");
            SendKeysToWebElement("//*[@class='verification__form-lock-container']/div[2]//input", "C:\\Users\\User\\Documents\\24H soft.png", "Поле Разворот с регистрацией", "поля Разворот с регистрацией");
            ClickWebElement("//*[@class='toolbar__item _reg-button']", "Кнопка Далее", "кнопки Далее");
            WaitTillElementisDisplayed(driver, ".//*[@class='ui__field verification__sms-field']", 10);
            SendKeysToWebElement(".//*[@class='ui__field verification__sms-field']", "7", "Поле SMS кода", "поля SMS кода");
            Thread.Sleep(1000);
            ClickWebElement(".//*[@class='toolbar__item _reg-button']/button", "Кнопка Отправить", "кнопки Отправить");
            SelectSkypeLogin("22222");
            var error = GetWebElement(".//*[@id='rv-error']", "Нет модуля с ошибкой");
            if (!(error.GetAttribute("data-errorcode").Equals("10") && error.GetAttribute("data-processstate").Equals("waitForSelectCallTime")))
                throw new Exception("Неверная обработка ошибки, когда логин=2");
            ClickWebElement("//*[@class='verification__error-buttons']/div[1]", "Кнопка Повторить", "кнопки Повторить");
            SelectSkypeLogin("33333");
            Thread.Sleep(20000);
            if (!driver.FindElement(By.XPath("//*[@class='verification__call-comment']")).Text.Contains("все операторы были заняты"))
                throw new Exception("Неверная обработка ошибки, когда логин=3");
            SelectSkypeLogin("44444");
            Thread.Sleep(20000);
            if (!driver.FindElement(By.XPath("//*[@class='verification__call-comment']")).Text.Contains("не смогли до вас дозвониться"))
                throw new Exception("Неверная обработка ошибки, когда логин=4");
            SelectSkypeLogin("55555");
            Thread.Sleep(20000);
            var msg = GetWebElement(".//*[@id='rv-error']", "Нет модуля с ошибкой");
            if (!(msg.GetAttribute("data-errorcode").Equals("0") && msg.GetAttribute("data-processstate").Equals("rejected") && msg.GetAttribute("data-rejectioncode").Equals("18")))
                throw new Exception("Неверная обработка ошибки, когда логин=5");
            ClickWebElement("//*[@class='verification__error-buttons']/div[1]", "Кнопка Повторить", "кнопки Повторить");
            ClickWebElement("//*[@class='toolbar__item _reg-button']", "Кнопка Далее", "кнопки Далее");
            WaitTillElementisDisplayed(driver, ".//*[@class='ui__field verification__sms-field']", 10);
            SendKeysToWebElement(".//*[@class='ui__field verification__sms-field']", "7", "Поле SMS кода", "поля SMS кода");
            Thread.Sleep(1000);
            ClickWebElement(".//*[@class='toolbar__item _reg-button']/button", "Кнопка Отправить", "кнопки Отправить");
            SelectSkypeLogin("77777");
            //Thread.Sleep(20000);
            WaitTillElementisDisplayed(driver, "//*[@class='verification__qiwi-instruction-wrap']", 30);
            if (!driver.FindElement(By.XPath("//*[@class='account-header__title']")).Text.Contains("завершения идентификации остался один шаг"))
                throw new Exception("Неверная обработка ошибки, когда логин=7");
        }

        private void MakeDisplayInhert(string xpath)
        {
            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
            IWebElement element = driver.FindElement(By.XPath(xpath));
            jse.ExecuteScript("arguments[0].setAttribute('style', 'display: inherit')", element);
        }

        private void SelectSkypeLogin(string s)
        {
            WaitTillElementisDisplayed(driver, "//*[@class='verification__time-slot-wrap']", 10);
            ClearBeforeInput("//*[@class='ui__field-inner']/input");
            SendKeysToWebElement("//*[@class='ui__field-inner']/input", s, "Поле Логина", "поля Логина");
            Thread.Sleep(1000);
            ClickWebElement("//*[@class='verification__form-row _gap-30']/div[1]", "Кнопка Отправить", "кнопки Отправить");
        }
    }

        class ChangePhoneCupis : FonbetWebProgram
    {
        public static CustomProgram FabricateChangePhoneCupis()
        {
            return new ChangePhoneCupis();
        }


        public override void Run()
        {
            base.Run();

            ClickOnAccount();
            WaitTillElementisDisplayed(driver, ".//*[@href='#!/account/profile/change-phone']", 5);
            ClickWebElement(".//*[@href='#!/account/profile/change-phone']", "Вкладка Смена номера телефона", "вкладки Смена номера телефона");
            LogStage("Проверка createProcess по тестовому сценарию");
            CreateProcessPhoneChange("000000000",null,null,null);
            CreateProcessPhoneChange("000000001", null, null, null);
            CreateProcessPhoneChange("000000002", "rejected", "0","4");
            CreateProcessPhoneChange("000000003", "rejected", "0", "11");
            CreateProcessPhoneChange("000000004", null, "2", null);
           
        }
    }
}

