using System;
using System.Collections.Generic;
using OpenQA.Selenium;

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
                {"deposit","Пополнение баланса" },
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
            if (!betTypeGridText.Equals("Фрибет"))
                throw new Exception("Добавился новый тип пари, кроме фрибета и линии");
            ClickWebElement(".//*[@class='ui__checkbox-text']/*[text()='Фрибет']", "Чекбокс Фрибет", "чекбокса Фрибет");
            if (!WebElementExist(".//*[@class='page-account__empty-list-text']"))
                throw new Exception("Добавился новый тип пари, кроме фрибета и линии, либо фильтры не работают");
            ClickWebElement(".//*[@class='ui__checkbox-text']/*[text()='Линия']", "Чекбокс Линия", "чекбокса Линия");
            ClickWebElement(".//*[@class='ui__checkbox-text']/*[text()='Фрибет']", "Чекбокс Фрибет", "чекбокса Фрибет");


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
                    IWebElement betresultGrid = GetWebElement(".//*[@class=\'wrap\'][1]//*[@class=\"operation-row _odd\"]/div[6]/span[1]", "Не отображается Результат пари в гриде");
                    string betresultGridText = betresultGrid.Text;
                    if (!betresultGridText.Equals(dataText))
                        throw new Exception("Не работают фильтры Результата пари");
                }
                ClickWebElementWithText("ui__checkbox-text", dataText, "Чекбокс", "чекбокса");
               
            }

            LogStage("Проверка развертки конкретного события");
            ClickWebElement(".//*[@class='ui__checkbox-text']/*[text()='Выигрыш']", "Чекбокс Выигрыш", "чекбокса Выигрыш");
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
            ClickWebElement(".//*[@class='account-calendar__row'][1]/div[1]", "Дата первого видимого дня календаря", "даты первого видимого дня календаря"); //подгружаю события минимум за прошедший месяц
            ClickWebElement(".//*[@href='#!/account/history/operations']", "Вкладка Операции", "вкладки Операции");

            LogStage("Снятие всех фильтров в столбце \"Тип операции\"");
            IList<IWebElement> all = driver.FindElements(By.XPath(".//*[@class='ui__checkboxes']/div")); //Все доступные типы операций в фильтре
            for (var i = 1; i <= all.Count; i++)
            {
                string nameTofind = string.Format(".//*[@class='ui__checkboxes']/div[{0}]//*[@class='ui__checkbox-text']/span", i);
                var element = driver.FindElement(By.XPath(nameTofind));
                element.Click();
            }

            LogStage("Проверка работы всех фильтров в столбце \"Тип операции\"");
            for (var i = 1; i <= all.Count; i++)
            {
                string nameTofind = string.Format(".//*[@class='ui__checkboxes']/div[{0}]//*[@class='ui__checkbox-text']/span", i);
                var element = driver.FindElement(By.XPath(nameTofind)).Text;

                ClickWebElementWithText("ui__checkbox-text", element, "Чекбокс", "чекбокса");
                if (!WebElementExist(".//*[@class='page-account__empty-list-text']"))
                {
                    IWebElement betoperationGrid = GetWebElement(".//*[@class='operation-row _odd']/div[4]", "Не отображается Операции в гриде");
                    string betoperationGridText = betoperationGrid.Text;
                    if (!betoperationGridText.Equals(element))
                        throw new Exception("Не работают фильтры Типа операций");
                }
                ClickWebElementWithText("ui__checkbox-text", element, "Чекбокс", "чекбокса");
            }

            LogStage("Проверка развертки конкретной операции");
            ClickWebElement(".//*[@class='ui__checkbox-text']/*[text()='Заключено пари']", "Чекбокс заключено пари", "чекбокса заключено пари");
            ClickWebElement(".//*[@class='wrap'][1]//*[@class='operation-row _odd']/div[7]", "Стрелка разворота операции", "Стрелка разворота операции");
            IWebElement betMark = GetWebElement(".//*[@class='bet-details _odd']//table//*[text()='Коэфф']", "Не отображается коэффициент в развернутом гриде");
            string betMarkText = betMark.Text;
            if (!betMarkText.Equals("Коэфф"))
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
            IWebElement loginPlace = GetWebElement(".//*[@class='login-form__form']/div[1]/input", "Нет поля логина");
            var loginPlaceClass = loginPlace.GetAttribute("placeholder");
            if (!loginPlaceClass.Contains("Номер счёта"))
                throw new Exception("Не переключается на placeholder с номером счета");

            LogStage("Проверка плейсхолдера Почты");
            driver.FindElement(By.XPath(".//*[@class='login-form__form']/div[1]/input")).Clear();
            SendKeysToWebElement(".//*[@class='login-form__form']/div[1]/input", "ya@ya.ru", "Поле логина", "поля логина");
            IWebElement mailPlace = GetWebElement(".//*[@class='login-form__form']/div[1]/input", "Нет поля логина");
            var mailPlaceClass = mailPlace.GetAttribute("placeholder");
            if (!mailPlaceClass.Contains("example@domain.com"))
                throw new Exception("Не переключается на placeholder с почтой");

            LogStage("Проверка плейсхолдера Телефона");
            driver.FindElement(By.XPath(".//*[@class='login-form__form']/div[1]/input")).Clear();
            SendKeysToWebElement(".//*[@class='login-form__form']/div[1]/input", "+79991234567", "Поле логина", "поля логина");
            IWebElement phonePlace = GetWebElement(".//*[@class='login-form__form']/div[1]/input", "Нет поля логина");
            var phonePlaceClass = phonePlace.GetAttribute("placeholder");
            if (!phonePlaceClass.Contains("+7 (999) 999-99-99"))
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

            RejectPwdChecker("12", "000000001");
            RejectPwdChecker("10", "000000002");
            RejectPwdChecker("4", "000000003");
            RejectPwdChecker("1", "000000004");

            LogStage("Переход на страницу восстановление пароля");
            ClickWebElement(".//*[@href='/#!/account/restore-password']", "Кнопка Забыли пароль", "кнопки забыли пароль");

            LogStage("Проверка sendCode по тестовому сценарию на error 10");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[2]//input", "000000005", "Поле номер телефона", "поля номер телефона");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[3]//input", "11", "Поле капча", "поля капчи");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            SendKeysToWebElement(".//*[@class='ui__field']", "123123", "Поле Код подтверждения", "поля Код подтверждения");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            var errorMessage = GetWebElement(".//*[@class='account-error__text']", "Нет текста ошибки");
            if (!errorMessage.Text.Contains("Неверный код подтверждения"))
                throw new Exception("Неверный текст ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");

            LogStage("Проверка sendPassword на reject");
            driver.FindElement(By.XPath(".//*[@class='ui__field']")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field']", "123456", "Поле Код подтверждения", "поля Код подтверждения");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[1]//input", "1234567Q", "Поле Новый пароль", "поля Новый пароль");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[2]//input", "1234567Q", "Поле Повторите новый пароль", "поля Повторите новый пароль");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            var errorText = GetWebElement(".//*[@class='account-error__text']", "Нет текста ошибки");
            if (!errorText.Text.Contains("В процессе регистрации произошла неожиданная ошибка"))
                throw new Exception("Неверный текст ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");

            LogStage("Проверка sendPassword на complete");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[2]//input", "000000005", "Поле номер телефона", "поля номер телефона");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[3]//input", "11", "Поле капча", "поля капчи");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            SendKeysToWebElement(".//*[@class='ui__field']", "123456", "Поле Код подтверждения", "поля Код подтверждения");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[1]//input", "!23qweQWE", "Поле Новый пароль", "поля Новый пароль");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[2]//input", "!23qweQWE", "Поле Повторите новый пароль", "поля Повторите новый пароль");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            var message = GetWebElement(".//*[@class='account-error__title']", "Нет title ошибки");
            if (!WebElementExist(".//*[@class='account__content']//span"))
                throw new Exception("Нет кнопки \"Войти на сайт\"");
            if (!message.Text.Contains("Пароль успешно изменён"))
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

            CreateProcessemailChecker("14", "1@dev.dev");
            CreateProcessemailChecker("13", "2@dev.dev");
            CreateProcessemailChecker("12", "3@dev.dev");

            LogStage("Проверка sendCode по тестовому сценарию");
            driver.FindElement(By.XPath(".//*[@class='ui__field-inner']/input")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input", "4@dev.dev", "Поле email", "поля email");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
            SendEmailCodeChecker("10", "1235");
            SendEmailCodeChecker("1", "9999");
            driver.FindElement(By.XPath(".//*[@class='ui__field-inner']/input")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input", "4@dev.dev", "Поле email", "поля email");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
            driver.FindElement(By.XPath(".//*[@class='ui__field-inner']/input")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input", "1234", "Поле email", "поля email");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки отправить");
            var errorMessage = GetWebElement(".//*[@class='account-error__text']", "Нет текста ошибки");
            if (!errorMessage.Text.Contains("E-mail успешно подверждён"))
                throw new Exception("Неверный текст ошибки");
            ClickWebElement(".//*[@classid='account-error__btn-inner']//span", "Кнопка Вернуться к профилю", "кнопки Вернуться к профилю");
            var mainTab = GetWebElement(".//*[@class='account-tabs']/a[1]", "Нет вкладки Основные данные");
            var mainTabClass = mainTab.GetAttribute("class");
            if (!mainTabClass.Contains("state_active"))
                throw new Exception("Не произошло возвращения на главную страницу");
        }
    }

    class RegistrationV4 : FonbetWebProgram //нужно доделать
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

            if(!WebElementExist(".//*[@href='/#!/account/registration/Reg4']"))
                throw new Exception("На данном сайте не супер-регистрации");

            LogStage("Переход на страницу регистрации");
            ClickWebElement(".//*[@href='/#!/account/registration/Reg4']", "Кнопка Регистрации", "кнопки Регистрации");
            var required = driver.FindElements(By.XPath(".//*[@class='ui__required']"));
            if (required.Count != 6)
                throw new Exception("Число обязательных полей не равно 6");

            LogStage("Заполнение персональных данных");
            SendKeysToWebElement(".//*[@class='registration-v4__step-wrap']/div[2]/label[1]//input", "Это", "Поле Фамилия", "поля Фамилия");
            SendKeysToWebElement(".//*[@class='registration-v4__step-wrap']/div[2]/label[2]//input", "Тест", "Поле Имя", "поля Имя");
            LogStage("Заполнение контактной информации");
            SendKeysToWebElement(".//*[@class='registration-v4__step-wrap']/div[4]/label[1]//input", "test@mail.ru", "Поле email", "поля email");
            SendKeysToWebElement(".//*[@class='registration-v4__step-wrap']/div[4]/label[2]//input", "000000001", "Поле Номер телефона", "поля Номер телефона");
            LogStage("Заполнение пароля");
            SendKeysToWebElement(".//*[@class='registration-v4__step-wrap']/div[6]/label[1]//input", "123qwe123", "Поле Пароль", "поля Пароль");
            SendKeysToWebElement(".//*[@class='registration-v4__step-wrap']/div[6]/label[2]//input", "123qwe123", "Поле Подтверждение пароля", "поля Подтверждение пароля");
            ClickWebElement(".//*[@id='checkbox2']", "Чекбокс Подтверждения правил", "чекбокса Подтверждения правил");
            ClickWebElement(".//*[@class='registration-v4__form-row _form-buttons']//button", "Кпонка Продолжить", "кпонки Продолжить");
            var errorMessage = GetWebElement(".//*[@class='account-error__text']", "Нет текста ошибки");
            if (!errorMessage.Text.Contains("Предыдущий процесс не завершён"))
                throw new Exception("Неверный текст ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Закрыть", "кнопки Закрыть");

            CreateProcessRegistration("000000002");
            CreateProcessRegistration("000000003");
            CreateProcessRegistration("000000004");
            CreateProcessRegistration("000000005");
            CreateProcessRegistration("000000006");
            CreateProcessRegistration("000000007");

            driver.Navigate().GoToUrl("http://fonred5000.dvt24.com/?test=1#!/account/registration/Reg4");
            ClickWebElement(".//*[@class='registration-v4__form-row _form-buttons']//button", "Кпонка Продолжить", "кпонки Продолжить");
            SendSmsCodeRegistration("1");
            SendSmsCodeRegistration("2");
            SendSmsCodeRegistration("3");
            SendSmsCodeRegistration("4");
            SendSmsCodeRegistration("5");
            SendSmsCodeRegistration("6");
            SendSmsCodeRegistration("7");
            SendSmsCodeRegistration("8");
            SendSmsCodeRegistration("9");
            SendSmsCodeRegistration("0");

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
            ClickOnAccount();

            LogStage("Проверка на статус верификации");
            if (!WebElementExist(".//*[@class='verification__notice-types-wrap']"))
            {
                LogStage("Переход в админку");
                driver.Navigate().GoToUrl("http://fonbackoffice.dvt24.com/");

                LogStage("Логирование в админке");
                SendKeysToWebElement(".//*[@id='username']", "mashkov", "Поле Пользователь", "поля Пользователь");
                SendKeysToWebElement(".//*[@id='password']", "ueueue11", "Поле Пароль", "поля Пароль");
                ClickWebElement(".//*[@class='login__btn']", "Кнопка Вход", "кнопки Вход");

                LogStage("Проверка аккаунта на верификацию");
                ClickWebElement(".//*[@class='home__items']//*[@href='#/clientManager']", "Меню Поиск клиентов",
                    "меню Поиск клиентов");
                SendKeysToWebElement(".//*[@class='clients__fields']/label//input", "13", "Поле Идентификатор клиента",
                    "поля Идентификатор клиента");
                ClickWebElement(".//*[@class='clients__btn-inner']//button", "Кнпока Найти", "кнопки Найти");
                ClickWebElement(".//*[@class='tabs__head tabs__slider']/span/a[2]", "Вкладка Расширенная информация",
                    "вкладки Расширенная информация");
                ClickWebElement(".//*[@class='form__col-wide']/div[1]//i", "Иконка Редактировать",
                    "иконки Редактировать");
                IList<IWebElement> status = driver.FindElements(By.XPath(".//*[@class='form__col-wide']/form//input"));
                foreach (IWebElement element in status)
                {
                    string statusClass = element.GetAttribute("class");

                    if (statusClass.Contains("checked"))
                        element.Click();
                }

                SendKeysToWebElement(".//*[@class='ui__field-inner']/textarea", "autotest", "Поле Комментарий",
                    "поля Комментарий");
                ClickWebElement(".//*[text()='Сохранить']", "Кнопка Сохранить", "кнопки Сохранить");
                driver.Navigate().GoToUrl("http://fonred5051.dvt24.com/");
                ClickOnAccount();
                // return;
            }

            LogStage("Проверка createProcess по тестовому сценарию");
            ClickWebElement(".//*[@href='#!/account/verification/qiwi']", "Кнопка Верификации по киви",
                "кнопки Верификации по киви");
            driver.FindElement(By.XPath(".//*[@class='ui__field-wrap-inner']//input")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field-wrap-inner']//input", "79000000002", "Поле Номер телефона",
                "поля Номер телефона");
            ClickWebElement(".//*[@id='rulesAgree']", "Чекбокс Соглашения с правилами",
                "чекбокс Соглашения с правилами");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
            var errorMessage = GetWebElement(".//*[@class='account-error__text']", "Нет текста ошибки");
            if (!errorMessage.Text.Contains("Счёт уже верифицирован"))
                throw new Exception("Неверный текст ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");

            CreateProcessVerificationQiwi("79000000003");
            CreateProcessVerificationQiwi("79000000004");
            CreateProcessVerificationQiwi("79000000005");
            CreateProcessVerificationQiwi("79000000006");
            CreateProcessVerificationQiwi("79000000007");
            CreateProcessVerificationQiwi("79000000008");

            driver.FindElement(By.XPath(".//*[@class='ui__field-wrap-inner']//input")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field-wrap-inner']//input", "79000000009", "Поле Номер телефона",
                "поля Номер телефона");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");

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
            ClickWebElement(".//*[@href='#!/account/profile/change-phone']", "Вкладка Смена номера телефона", "вкладки Смена номера телефона");
            LogStage("Проверка createProcess по тестовому сценарию");
            driver.FindElement(By.XPath(".//*[@class='ui__field-inner']/input")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input", "000000000", "Поле Номер телефона", "поля Номер телефона");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
            if (!WebElementExist(".//*[@class='ui__field-inner']/input"))
                throw new Exception("Нет поля для ввода кода подтверждения");
            ClickWebElement(".//*[@href='#!/account/profile/main']", "Вкладка Основная", "вкладки Основная");
            ClickWebElement(".//*[@href='#!/account/profile/change-phone']", "Вкладка Смена номера телефона", "вкладки Смена номера телефона");
            CreateProcessPhoneChange("000000002");
            CreateProcessPhoneChange("000000003");
            CreateProcessPhoneChange("000000004");
            ClickWebElement(".//*[@href='#!/account/profile/change-phone']", "Вкладка Смена номера телефона", "вкладки Смена номера телефона");
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input", "000000005", "Поле Номер телефона", "поля Номер телефона");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");

            LogStage("Проверка sendSmsCode по тестовому сценарию");
            SendSmsPhoneChange("2");
            SendSmsPhoneChange("3");
            SendSmsPhoneChange("4");
            SendSmsPhoneChange("1");
        }
    }
}
