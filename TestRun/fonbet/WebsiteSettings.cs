using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;


namespace TestRun.fonbet
{
    class BetsSettings : FonbetWebProgram
    {
        public static CustomProgram FabricateBetsSettings() => new BetsSettings();

        public override void Run()
        {
            base.Run();

            LogStage("Установка настроек по умолчанию");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__restore-btn']", "Кнопка восстановления настроек по умолчанию", "кнопки восстановления настроек по умолчанию");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");

            LogStartAction("Проверка работы ставки пари с предыдущей ставки");
            //Выбор ставки из грида
            SwitchPageToBets();
            Thread.Sleep(1500);
            IList<IWebElement> events = driver.FindElements(By.XPath(".//*[@class='table']/tbody//td[5][@class='table__col _type_btn _type_normal']"));
            events[0].Click();
            WaitTillElementisDisplayed(driver, "//div[contains(@class, 'sum-panel')]//input", 5);
            SendKeysToWebElement("//div[contains(@class, 'sum-panel')]//input", "99", "поле ввода значения ставки", "поля ввода значения ставки");
            Thread.Sleep(1000);
            ClickWebElement("//div[contains(@class, 'place-button')]/div[1]", "Кнопка сделать ставку", "кнопки сделать ставку");
            Thread.Sleep(2000);
            for (int i = 2; i < events.Count; i++)
            {
                var a = events[i].Text;
                double kef = Convert.ToDouble(a, CultureInfo.InvariantCulture);
                var kef2 = Math.Round(kef);
                if (kef2 <= 3 && (!events[i].GetAttribute("class").Contains("_state_blocked")))
                {
                    events[i].Click();
                    break;
                }
            }
            // events[14].Click();
            Thread.Sleep(1000);
            if (driver.FindElement(By.XPath("//div[contains(@class, 'sum-panel')]//input")).GetAttribute("value") != "99")
                throw new Exception("Сумма предыдущего пари не подставляется");
            ClickWebElement("//div[contains(@class,'coupon__header-clear')]", "Крестик закрыть купон", "крестика закрыть купон");

            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__content']/div[3]/div[1]//input", "Чекбокс Подставлять сумму предыщудего пари", "чекбокса Подставлять сумму предыщудего пари");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");
            events[6].Click();
            if (!driver.FindElement(By.XPath("//div[contains(@class, 'sum-panel')]//input")).GetAttribute("class").Contains("state_error"))
                throw new Exception("Поле в сумме не пустое");

            LogStage("Установка быстрого пари в 100 руб");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__restore-btn']", "Кнопка восстановления настроек по умолчанию", "кнопки восстановления настроек по умолчанию");
            Thread.Sleep(1500);
            ClickWebElement(".//*[@class='settings__section'][1]/div/div[1]//*[@class='header-ui__checkbox-label']/input", "Чекбокс быстрое пари", "чекбокса быстрое пари");
            Thread.Sleep(1500);
            SendKeysToWebElement(".//*[@class='settings__section'][1]/div[1]/div/div[2]//input", "100", "поле ввода значения быстрой ставки", "поля ввода значения быстрой ставки");

            LogStage("Добавление любимого пари");
            ClickWebElement("//*[@value='showPercent']", "Радиобатон любимого пари в % от баланса", "радиобатона любимого пари в % от баланса");
            ClickWebElement(".//*[@class='settings__row _type_normal _type_columns']//*[@class='settings__fields-action _type_add']", "Кнопка добавления нового поля для быстрой ставки", "кнопки добавления нового поля для быстрой ставки");
            SendKeysToWebElement(".//*[@class='settings__row _type_normal _type_columns']/div/div[4]//input", "50", "новое поле ввода для любимого пари", "нового поля ввода для любимого пари");

            LogStage("Разрешение на прием любимого пари");
            ClickWebElement("//*[@class='settings__section'][1]/div/div[3]//input", "Чекбокс принимать пари с измененными коэффициентами", "чекбокса принимать пари с измененными коэффициентами");
            ClickWebElement("//*[@class='settings__section'][1]/div/div[5]//input", "Чекбокс принимать пари с измененными тоталами / форами", "чекбокса принимать пари с измененными тоталами / форами");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");

            LogStartAction("Проверка работы быстрой ставки");
            IWebElement fastBet = GetWebElement("//div[contains(@class, 'coupons-toolbar__item--1jILz _type_one-click')]/div/div[2]", "Не найдено поле с суммой быстрой ставки");
            var fastBetClass = fastBet.GetAttribute("title");
            if (fastBetClass != "100")
                throw new Exception("Поле с суммой быстрой ставки содержит неверное значение");
            IWebElement fastButton = GetWebElement("//div[contains(@class, 'coupons-toolbar__item--1jILz _type_one-click')]/div/div[1]", "Не найдена кнопка с быстрой ставкой");
            var fastButtonClass = fastButton.GetAttribute("class");
            if (!fastButtonClass.Contains("on"))
                throw new Exception("Быстрое пари выключено");
            LogActionSuccess();

            LogStage("Проверка работы приема любого пари");
            IWebElement betsOdd = GetWebElement("//div[contains(@class, 'coupons-toolbar__item--1jILz _type_switchers')]/div/div[1]", "Не найден элемент Принимать пари с измененными коэф.");
            var betsOddClass = betsOdd.GetAttribute("class");
            if (!betsOddClass.Contains("state_any"))
                throw new Exception("Не работает прием пари с изменными коэффициентами");
            IWebElement betsTotal = GetWebElement("//div[contains(@class, 'coupons-toolbar__item--1jILz _type_switchers')]/div/div[2]", "Не найден элемент Принимать пари с измененными коэф.");
            var betsTotalClass = betsTotal.GetAttribute("class");
            if (!betsTotalClass.Contains("state_on"))
                throw new Exception("Не работает прием пари с изменными тоталами / форами");

            Thread.Sleep(1000);
            ClickWebElement("//div[contains(@class, 'coupons-toolbar__item--1jILz _type_one-click')]/div", "Кнопка быстрой ставки", "кнопки быстрой ставки");

            //Выбор ставки из грида с кэф<2
            IList<IWebElement> grid = driver.FindElements(By.XPath(".//*[@class='table']/tbody//td[10]"));
            for (int i = 4; i < grid.Count; i++)
            {
                var a = grid[i].Text;
                double kef = Convert.ToDouble(a, CultureInfo.GetCultureInfo("en-US").NumberFormat);
                var kef2 = Math.Round(kef);
                if (kef2 <= 2 && (!grid[i].GetAttribute("class").Contains("_state_blocked")))
                {
                    grid[i].Click();
                    break;
                }
            }

            
           
            string max = driver.FindElement(By.XPath("//div[contains(@class,'min-max')]/span[3]")).Text;
            var result = Convert.ToDouble(Regex.Replace(max, @"[^\d]+", ""));
            

            if (ClientBalance < result)
            {
                LogStage("Проверка ввода суммы больше чем баланс счета");

                LogStartAction("Сумма ставки равна максимальной");
                ClickWebElement("//div[contains(@class,'min-max')]/span[3]", "Значение максимальной ставки", "значения максимальной ставки");
                ClickWebElement("//div[contains(@class, 'place-button')]/div[1]", "Кнопка заключить пари", "кнопки заключить пари");
                if (!WebElementExist("//div[contains(@class, 'modal-window__user-area _no-funds')]"))
                    throw new Exception("Нет ошибки о нехватке средств");
                ClickWebElement("//div[contains(@class,'modal-window__button-area')]/a[2]", "Кнопка Нет", "кнопки Нет");
                LogActionSuccess();
            }

            LogStartAction("Сумма ставки больше максимальной");
           // var maxBet = Convert.ToDouble(driver.FindElement(By.XPath("//div[contains(@class,'min-max')]/span[3]")).Text);
            SendKeysToWebElement("//div[contains(@class, 'sum-panel')]//input", Convert.ToString(result * 2), "поле ввода значения ставки", "поля ввода значения ставки");
            IWebElement button = GetWebElement("//div[contains(@class, 'place-button')]/div[1]", "Нет кнопки заключить пари");
            if (!button.GetAttribute("class").Contains("disabled"))
                throw new Exception("Кнопка ставки не блокируется");
            LogActionSuccess();
            ClickWebElement("//div[contains(@class,'coupon__header-clear')]", "Кнопка закрытия окна нового пари", "кнопки закрытия окна нового пари");
        }
    }

    class CashOutAndDialogsSettings : FonbetWebProgram
    {
        public static CustomProgram FabricateCashOutAndDialogsSettings() => new CashOutAndDialogsSettings();

        public override void Run()
        {
            base.Run();

            LogStage("Установка настроек по умолчанию");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__restore-btn']", "Кнопка восстановления настроек по умолчанию", "кнопки восстановления настроек по умолчанию");

            LogStage("Установка быстрого пари");
            ClickWebElement(".//*[@class='settings__section'][1]/div/div[1]//*[@class='header-ui__checkbox-label']/input", "Чекбокс быстрое пари", "чекбокса быстрое пари");

            LogStage("Редактирование Продажи пари");
            ClickWebElement("//*[@class='settings__section'][2]/div[3]//input", "Чекбокс изменение суммы продажи", "чекбокса изменения суммы продажи");

            LogStage("Редактирование меню Диалоги");
            ClickWebElement("//*[@class='settings__section'][4]/div[1]//input", "Чекбокс 'Не спрашивать подтверждение для быстрого пари'", "чекбокса 'Не спрашивать подтверждение для быстрого пари'");
            ClickWebElement("//*[@class='settings__section'][4]/div[2]//input", "Чекбокс 'Не спрашивать подтверждение для продажи'", "чекбокса 'Не спрашивать подтверждение для продажи'");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");

            LogStage("Проверка работы настроек Продажи пари");
            ClickWebElement(".//*[@class='coupons-toolbar--3wv_Z']/div[4]", "Вкладка 'На продажу' в меню купонов", "вкладки 'На продажу' в меню купонов");
            if (WebElementExist(".//*[@class='coupon__content-empty']"))
            {
                LogHint("Нет купонов на продажу, добавляем купоны");
                //Выбор ставки из грида с кэф<2
                IList<IWebElement> grid = driver.FindElements(By.XPath(".//*[@class='line__table-wrap']/div[2]//*[@class='table']/tbody//td[5]"));
                grid[5].Click();
                ClickWebElement(".//*[@class='coupons-toolbar--3wv_Z']/div[4]", "Вкладка 'На продажу' в меню купонов", "вкладки 'На продажу' в меню купонов");
                Thread.Sleep(5000);
            }
            IWebElement sellSwitch = GetWebElement(".//*[@class='coupon__sell-button-area--3BXdB']/a[2]", "Нет тумблера рядом с кнопкой продажи пари");
            var sellSwitchClass = sellSwitch.GetAttribute("class");
            if (!sellSwitchClass.Contains("coupon__sell-switch"))
                throw new Exception("Не работает чекбокс изменение суммы продажи");

            LogStage("Проверка работы настроек Диалогов");
            ClickWebElement(".//*[@href='/#!/bets']", "Вкладка \"Линия\"", "вкладки \"Линия\"");
            WaitForPageLoad();
            IList<IWebElement> bet = driver.FindElements(By.XPath(".//*[@class='table']/tbody//td[5]"));
            bet[4].Click();
            if (WebElementExist(".//*[@class='modal-window__button-area']"))
                throw new Exception("Не работает чекбокс 'Не спрашивать подтверждение для быстрого пари'");
            ClickWebElement(".//*[@class='coupons-toolbar--3wv_Z']/div[4]", "Вкладка 'На продажу' в меню купонов", "вкладки 'На продажу' в меню купонов");
            ClickWebElement(".//*[@class='coupons__list-inner']//article[1]//*[@class='coupon__sell-button-area--3BXdB'][1]/a[1]", "Кнопка 'Продать пари'", "кнопки 'Продать пари'");
            if (WebElementExist(".//*[@class='modal-window__button-area']"))
                throw new Exception("Не работает чекбокс 'Не спрашивать подтверждение для продажи'");
        }

    }

    class View : FonbetWebProgram
    {
        public static CustomProgram FabricateView() => new View();

        public override void Run()
        {
            base.Run();

            LogStage("Установка настроек по умолчанию");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__restore-btn']", "Кнопка восстановления настроек по умолчанию", "кнопки восстановления настроек по умолчанию");
            Thread.Sleep(1000);
            // LogStage("Установка чекбоксов меню Вид");
            LogStartAction("Установка чекбоксов меню Вид");
            for (var i = 1; i <= 12; i++)
            {
                if (i == 4 || i==7||i == 8 || i == 9)
                {
                    continue;
                }
                string nameTofind = string.Format("//*[@class='settings__section'][5]/div/div[{0}]//input", i);
                var element = driver.FindElement(By.XPath(nameTofind));
                element.Click();
            }
            LogActionSuccess();

            LogStartAction("Проверка компактного режима отображения подвала сайта");
            IWebElement footerSidebar = GetWebElement(".//*[@id='footerContainer']//footer/div[1]", "Не сворачивается футер");
            var footerSidebarClass = footerSidebar.GetAttribute("class");
            if (!footerSidebarClass.Contains("compact"))
                throw new Exception("Не работает компактный режим отображения подвала сайта");

            LogStage("Перевод меню в отображение слева");
            ClickWebElement(".//*[@class='settings__section']//span[text()='Слева']/../input", "Радиобатон отображения меню слева", "радиобатона отображения меню слева");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");
            SwitchPageToBets();
            LogStartAction("Проверка отображения номеров");
            if (!WebElementExist(".//*[@class='table__event-number']"))
                throw new Exception("Не работает отображение номеров событий");
            LogActionSuccess();

            LogStartAction("Проверка скрытия купонов на вкладке \"Недавняя\"");
            if (WebElementExist(".//*[@class='coupon__content']"))
                throw new Exception("Не работает скрытие принятых купонов");
            LogActionSuccess();

            LogStartAction("Проверка отображения статусов купонов цветом");
            ClickWebElement("//*[@class='coupons-toolbar--3wv_Z _mode_compact--2egXJ']/div[1]", "Иконка на гамбургер", "иконки гамбургера");
            ClickWebElement("//*[@id='popupLineMenu']/li[2]", "Строка отображение недавних купонов", "Строка отображения недавних купонов");
            IWebElement couponLable = GetWebElement("//div[contains(@class,'coupon__info-head')]/div[3]", "Нет результата купона");
            var couponLableClass = couponLable.GetAttribute("class");
            if (!couponLableClass.Contains("style_colored"))
                throw new Exception("Не работает отображения статусов купонов цветом");
            LogActionSuccess();

            LogStartAction("Проверка режима ширины ленты купонов");
            IWebElement couponWide = GetWebElement(".//*[@id='coupons__inner']", "Пропала лента купонов");
            var couponWideClass = couponWide.GetAttribute("class");
            if (couponWideClass.Contains("type_wide"))
                throw new Exception("Не работает узкая лента купонов");

            LogStage("Проверка компактного отображения меню в личном кабинете");
            ClickWebElement(".//*[@class='header__login-item'][1]", "Имя пользователя в шапке", "имени пользователя в шапке");
            IWebElement accountSidebar = GetWebElement(".//*[@class='page-account__content']/div[1]", "Пропала лента купонов");
            var accountSidebarClass = accountSidebar.GetAttribute("class");
            if (!accountSidebarClass.Contains("compact"))
                throw new Exception("Не работает компактный режим отображения меню в личном кабинете");

            


            SwitchPageToBets();
            LogStage("Проверка автосворачивания элементов в меню событий");
            ClickWebElement(".//*[@class='list-view-new__table-body']/tr[5]//a", "Строка ФУТБОЛ в меню спорта слева", "строки ФУТБОЛ в меню спорта слева");
            IWebElement footballArrow = GetWebElement(".//*[@class='list-view-new__table-body']/tr[5]//*[@class='event-v-list__cell-overlay']/div", "Нет стрекли разворота вида спорта");
            var footballArrowClass = footballArrow.GetAttribute("class");
            if (!footballArrowClass.Contains("state_opened"))
                throw new Exception("Не работает автосворачивание элементов в меню событий");
            ClickWebElement(".//*[@href='#!/bets/hockey']", "Строка Хоккей в меню спорта слева", "строки Хоккей в меню спорта слева");
            IWebElement footballslide = GetWebElement(".//*[@class='list-view-new__table-body']/tr[5]//*[@class='event-v-list__cell-overlay']/div", "Нет стрекли разворота вида спорта");
            var footballslideClass = footballslide.GetAttribute("class");
            if (footballslideClass.Contains("state_opened"))
                throw new Exception("Не работает автосворачивание элементов в меню событий");

            LogStage("Проверка автосворачивания купонов");
            ClickWebElement(".//*[@class='list-view-new__table-body']/tr[6]//*[@class='event-v-list__cell-overlay']/div", "Стрелка сворачивания строки Хоккей", "стрелки сворачивания строки Хоккей");
            ClickWebElement(".//*[@href='#!/bets/0']", "Строка Все события", "строки Все события");
            IList<IWebElement> allBets = driver.FindElements(By.XPath(".//*[@class='table__body']/tr[6]/td[3]"));
            allBets[3].Click();
            allBets[4].Click();
            allBets[5].Click();
            ClickWebElement("//div[contains(@class,'place-button')]/div[1]", "Кнопка заключить пари", "кнопки заключить пари");
            // WaitTillElementisDisplayed(driver, ".//*[@class='coupons__list-inner']/div[1]/article[1]/div[1]/div[1]", 15);
            Thread.Sleep(1000);
            IWebElement couponArrow = GetWebElement(".//*[@class='coupons__list-inner']/div/article[1]/div[1]/div[1]", "Нет стрелки разворота у купона");
            var couponArrowClass = couponArrow.GetAttribute("class");
            if (!couponArrowClass.Contains("expanded"))
                throw new Exception("Не работает автосворачивание купонов");
        }

    }
    class PlayerProtection : FonbetWebProgram
    {
        public static CustomProgram FabricatePlayerProtection()
        {
            return new PlayerProtection();
        }

        public override void Run()
        {
            base.Run();

            LogStage("Сброс настроек по умолчанию и установка самоограничения в 1 минуту");
            ClickWebElement(".//*[@href='/#!/account/deposit']", "Кнопка Личный кабинет", "кнопки Личный кабинет");
            ClickWebElement("//*[@href='#!/account/responsible']", "Вкладка Самоогрничения", "вкладки Самоогрничения");
            if (!driver.FindElement(By.XPath("//*[@class='ui__checkbox-label']/..")).GetAttribute("class").Contains("state_checked")){
                ClickWebElement("//*[@class='ui__checkbox-label']/input", "Чекбокс Включить ограничения сессии", "чекбокса Включить ограничения сессии");
            }
            SendKeysToWebElement("//*[@class='ui__field-inner']/input", "1", "Поле Самоограничения", "поля Самоограничения");
            ClickWebElement("//*[@class='change-password__form-row']//button", "Кнопка Сохранить", "кнопки Сохранить");

            LogStage("Ожидание минуты");
            Thread.Sleep(62000);
            LogStage("Проверка продолжения сессии");
            ClickWebElement(".//*[@class='session-dialog__buttons']/div[2]", "Кнопка Продолжить в диалоговом окне", "кнопки  Продолжить в диалоговом окне");
            LogStage("Ожидание минуты");
            Thread.Sleep(62000);
            LogStage("Проверка выхода из сессии");
            ClickWebElement(".//*[@class='session-dialog__buttons']/div[1]", "Кнопка Выход в диалоговом окне", "кнопки  Выход в диалоговом окне");
            WaitTillElementisDisplayed(driver, ".//*[@class='header__login-head']/a", 5);
            IWebElement loginStatus = GetWebElement(".//*[@class='header__login-head']/a", "Нет кнопки Войти");
            string loginStatusText = loginStatus.Text.ToUpper();
            if (!loginStatusText.Contains("ВОЙТИ"))
                throw new Exception("Кнопка называется иначе чем Войти");
        }
    }

    class ViewWithoutLogin : FonbetWebProgram
    {
        public static CustomProgram FabricateViewWithoutLogin()
        {
            return new ViewWithoutLogin();
        }

        protected override bool NeedLogin()
        {
            return false;
        }

        public override void Run()
        {
            base.Run();

            LogStage("Установка настроек вида и отображение меню слева");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__rows']/div[1]//input", "Чекбокс отображать номера событий", "чекбокса отображать номера событий");
            ClickWebElement(".//*[@class='settings__rows']/div[2]//input", "Чекбокс компактный режим отображения подвала", "чекбокса компактный режим отображения подвала");
            ClickWebElement(".//*[@class='settings__rows']/div[3]//input", "Чекбокс автоматическое сворачивание элементов", "чекбокса автоматическое сворачивание элементов");
            ClickWebElement(".//*[@class='settings__section']//span[text()='Слева']/../input", "Радиобатон отображения меню слева", "радиобатона отображения меню слева");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");

            SwitchPageToBets();
            LogStartAction("Проверка отображения номеров");
            if (!WebElementExist(".//*[@class='table__event-number']"))
                throw new Exception("Не работает отображение номеров событий");

            LogStartAction("Проверка сворачивание футера");
            IWebElement footerSidebar = GetWebElement(".//*[@id='footerContainer']//footer/div[1]", "Нет футера");
            var footerSidebarClass = footerSidebar.GetAttribute("class");
            if (!footerSidebarClass.Contains("compact"))
                throw new Exception("Не работает компактный режим отображения подвала сайта");

            LogStage("Проверка автосворачивания элементов в меню событий");
            ClickWebElement(".//*[@class='list-view-new__table-body']/tr[4]//a", "Строка ФУТБОЛ в меню спорта слева", "строки ФУТБОЛ в меню спорта слева");
            IWebElement footballArrow = GetWebElement("//*[@href='#!/bets/football']/../div", "Нет стрекли разворота вида спорта");
            ClickWebElement(".//*[@href='#!/bets/hockey']", "Строка Хоккей в меню спорта слева", "строки Хоккей в меню спорта слева");
            var footballArrowClass = footballArrow.GetAttribute("class");
            if (footballArrowClass.Contains("state_opened"))
                throw new Exception("Не работает автосворачивание элементов в меню событий");

            LogStartAction("Проверка меню слева");
            IWebElement menuBar = GetWebElement(".//*[@class='line-filter-layout__menu--3YfDq']/div", "Нет фильтр меню");
            var menuBarClass = menuBar.GetAttribute("class");
            if (!menuBarClass.Contains("menu-left"))
                throw new Exception("Не работает отображение меню слева");
        }

    }

    class CouponGridInterface : FonbetWebProgram
    {
        public static CustomProgram FabricateCouponGridInterface()
        {
            return new CouponGridInterface();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            SwitchPageToBets();
            LogStage("Проверка узкой ленты купонов");
            ClickOnHamburger();
            ClickWebElement(".//*[@id='popupLineMenu']/li[1]", "Кнопка узкая лента купонов", "кнопки узкая ленты купонов");
            IWebElement couponMenu = GetWebElement(".//*[@id='coupons__inner']", "Нет отображения всех купонов");
            var couponMenuClass = couponMenu.GetAttribute("class");
            if (couponMenuClass.Contains("wide"))
                throw new Exception("Не работает узкая лента купонов");


            LogStage("Проверка функции Скрыть все недавние купоны");
            ClickOnHamburger();
            ClickWebElement(".//*[@id='popupLineMenu']/li[1]", "Кнопка узкая лента купонов", "кнопки узкая ленты купонов");
            ClickOnHamburger();
            ClickWebElement(".//*[@id='popupLineMenu']/li[2]", "Кнопка Скрыть все недавние купоны", "кнопки Скрыть все недавние купоны");
            if (!WebElementExist("//div[contains(@class,'coupon__content-empty')]"))
                throw new Exception("Не работает скрытие всех купонов");
            ClickOnHamburger();
            ClickWebElement(".//*[@id='popupLineMenu']/li[2]", "Кнопка Показать все недавние купоны", "кнопки Показать все недавние купоны");

            LogStage("Проверка функции Развернуть все купоны");
            ClickOnHamburger();
            ClickWebElement(".//*[@id='popupLineMenu']/li[3]", "Кнопка Развернуть все купоны", "кнопки Развернуть все купоны");

            LogStartAction("Проверка кол-ва стрелочек у купонов");
            IList<IWebElement> betType = driver.FindElements(By.XPath(".//*[@class='coupon__info-item-inner']//*[@class='coupon__info-text']"));

            int betCount = 0;
            foreach (IWebElement element in betType)
            {
                string typeBet = element.Text;
                if (typeBet != "Одинар")
                    betCount++;
            }
            IList<IWebElement> countArrow = driver.FindElements(By.XPath("//div[contains(@class,'coupon__head _style')]/i"));
            if (betCount != countArrow.Count)
                throw new Exception("Не корректное кол-во стрелок у купонов");
            LogActionSuccess();

            IWebElement arrowMenu = GetWebElement(".//*[@class='coupon__show-hide--8g8S1']", "Нет стрелок у купонов");
            var arrowMenuClass = arrowMenu.GetAttribute("class");
            if (arrowMenuClass.Contains("expanded"))
                throw new Exception("Не работает Развернуть все купоны");

            LogStage("Проверка функции Свернуть все купоны");
            ClickOnHamburger();
            ClickWebElement(".//*[@id='popupLineMenu']/li[4]", "Кнопка Свернуть все купоны", "кнопки Свернуть все купоны");
            IWebElement menuArrow = GetWebElement("//div[contains(@class, 'coupon__show-hide--8g8S1')]", "Нет стрелок у купонов");
            var menuArrowClass = menuArrow.GetAttribute("class");
            if (!menuArrowClass.Contains("expanded"))
                throw new Exception("Не работает Свернуть все купоны");

            LogStage("Проверка вкладки Нерасчитанные");
            ClickOnHamburger();

            ClickWebElement(".//*[@id='popupLineMenu']/li[3]", "Кнопка Развернуть все купоны",
                "кнопки Развернуть все купоны");
            ClickWebElement("//*[@class='coupons-toolbar--3wv_Z']/div[3]", "Меню \"Нерасчитанные\"",
                "меню \"Нерасчитанные\"");
            if (WebElementExist("//div[contains(@class, 'coupon__info-label')]"))
            {
                throw new Exception("В фильтр \"Нерассчитанные\" попали расчитанные");
            }

            else LogHint("Нерасчитанных купонов нет");

            LogStage("Проверка вкладки На продажу");
            if (WebElementExist(".//*[@class='coupon--367FJ _type_list--2aBkW']"))
            {
                ClickWebElement("//*[@class='coupons-toolbar--3wv_Z']/div[4]", "Меню \"На продажу\"", "меню \"На продажу\"");
                IList<IWebElement> grid = driver.FindElements(By.XPath(".//*[@class='coupon--367FJ _type_list--2aBkW']")); //общее кол-во купонов
                IList<IWebElement> gridSell = driver.FindElements(By.XPath(".//*[@class='coupon__sell-button-area--3BXdB']")); //купоны которые можно продать
                if (grid.Count != gridSell.Count)
                    throw new Exception("Проблемы с фильтром \"На продажу\"");
            }
            else LogHint("Купонов на продажу нет");

            LogStage("Проверка открытия/закрытия купона целиком");
            if (!WebElementExist(".//*[@class='coupon__content-empty--kYoEt']")) // если купоны существуют
            {
                ClickWebElement(".//*[@class='coupon--367FJ _type_list--2aBkW'][1]/div[1]/a", "Кнопка Копировать купон", "кнопки Копировать купон");
                ClickWebElement("//div[contains(@class, 'new-coupon__header-clear')]", "Кнопка закрытия купона целиком", "кнопки закрытия купона целиком"); //крестик у купона
                if (WebElementExist("//div[contains(@class, 'place-button')]"))
                    throw new Exception("Не работает кнопка закрытия купона целиком");

                LogStage("Проверка открытия/закрытия купона по событиям");
                ClickWebElement(".//*[@class='coupon--367FJ _type_list--2aBkW'][1]/div[1]/a", "Кнопка Копировать купон", "кнопки Копировать купон");
                IList<IWebElement> all = driver.FindElements(By.XPath("//div[contains(@class,'stake-clear')]")); //крестики у события
                for (int i = 0; i < all.Count; i++)
                {
                    driver.FindElement(By.XPath("//div[contains(@class,'stake-clear')]")).Click();
                }
                if (WebElementExist("//div[contains(@class, 'new-coupon__header-clear')]")) //кнопка закрыть
                    throw new Exception("Не работает кнопка закрытия купона по событиям");
            }

            LogStartAction("Проверка быстрой ставки и копирования купонов");
            ExecuteJavaScript("return (function(){var e = document.getElementsByClassName('coupons-toolbar__icon--10Se9 _icon_one-click-switch--1Atcg _state_off--YWLR4')[0];var hasOff = e.classList.contains('off');e.click();var hasOn = e.classList.contains('on');return hasOn == hasOff;})()", "Быстрая ставка не работает");
            if (WebElementExist(".//*[@class='coupon--367FJ _type_list--2aBkW'][1]/div[1]/a"))
                throw new Exception("Кнопка копирования ставки не исчезает при быстрой ставке");
            if (!WebElementExist("//div[contains(@class,'_icon_one-click-sum')]"))
                throw new Exception("Сумма быстрой ставки не отображается");

            LogStage("Проверка приема пари с измен. коэфф");
            IWebElement rateSwitch = GetWebElement("//*[@class='coupons-toolbar__item--1jILz _type_switchers--3ZRs4']/div/div[1]", "Принимать пари с увеличенными коэффициентами");
            var rateSwitchTitle = rateSwitch.GetAttribute("title");
            ClickWebElement("//*[@class='coupons-toolbar__item--1jILz _type_switchers--3ZRs4']/div/div[1]", "Кнопка \"Принимать пари с увеличенными коэффициентами\"", "кнопки \"Принимать пари с увеличенными коэффициентами\"");
            if (rateSwitchTitle == rateSwitch.GetAttribute("title"))
                throw new Exception("Не работает прием пари с увелич коэфф.");
            ClickWebElement("//*[@class='coupons-toolbar__item--1jILz _type_switchers--3ZRs4']/div/div[1]", "Кнопка \"Принимать пари с увеличенными коэффициентами\"", "кнопки \"Принимать пари с увеличенными коэффициентами\"");
            if (rateSwitchTitle == rateSwitch.GetAttribute("title"))
                throw new Exception("Не работает прием пари с увелич коэфф.");
            ClickWebElement("//*[@class='coupons-toolbar__item--1jILz _type_switchers--3ZRs4']/div/div[1]", "Кнопка \"Принимать пари с увеличенными коэффициентами\"", "кнопки \"Принимать пари с увеличенными коэффициентами\"");
            if (rateSwitchTitle != rateSwitch.GetAttribute("title"))
                throw new Exception("Не работает прием пари с увелич коэфф.");

            LogStage("Проверка пари с изменными тоталами/форами");
            IWebElement totalSwitch = GetWebElement("//*[@class='coupons-toolbar__item--1jILz _type_switchers--3ZRs4']/div/div[2]", "Принимать пари с увеличенными коэффициентами");
            var totalSwitchTitle = totalSwitch.GetAttribute("title");
            ClickWebElement("//*[@class='coupons-toolbar__item--1jILz _type_switchers--3ZRs4']/div/div[2]", "Кнопка \"Принимать пари с измененными тоталами/форой\"", "кнопки \"Принимать пари с измененными тоталами/форой\"");
            if (totalSwitchTitle == totalSwitch.GetAttribute("title"))
                throw new Exception("Не работает прием пари с измененными тоталами/форой");
            ClickWebElement("//*[@class='coupons-toolbar__item--1jILz _type_switchers--3ZRs4']/div/div[2]", "Кнопка \"Принимать пари с измененными тоталами/форой\"", "кнопки \"Принимать пари с измененными тоталами/форой\"");
            if (totalSwitchTitle != totalSwitch.GetAttribute("title"))
                throw new Exception("Не работает прием пари с измененными тоталами/форой");
        }

        private void ClickOnHamburger()
        {
            ClickWebElement("//div[contains(@class, 'coupons-toolbar--3wv_Z')]/div[1]", "Меню отображения списка ставок", "меню отображения списка ставок");
        }
    }
    class CouponSettings : FonbetWebProgram
    {
        public static CustomProgram FabricateCouponSettings() => new CouponSettings();

        public override void Run()
        {
            base.Run();

            LogStage("Установка настроек сайта по умолчанию");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__restore-btn']", "Кнопка восстановления настроек по умолчанию", "кнопки восстановления настроек по умолчанию");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");

            //LogStage("Установка настроек по умолчанию");
            //ClickWebElement("//*[@class='coupons-toolbar--3wv_Z']/div[last()]/div", "Меню настроек", "меню настройки");
            //ClickWebElement("//*[@class='settings__buttons--3FfTP']/div[1]", "Кнопка восстановления настроек по умолчанию", "кнопки восстановления настроек по умолчанию");
            //ClickWebElement("//*[@class='settings__buttons--3FfTP']/div[2]", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");

            LogStartAction("Проверка работы ставки пари с предыдущей ставки");
            //Выбор ставки из грида
            SwitchPageToBets();
            Thread.Sleep(1500);
            IList<IWebElement> events = driver.FindElements(By.XPath(".//*[@class='table']/tbody//td[5][@class='table__col _type_btn _type_normal']"));
            events[0].Click();
            WaitTillElementisDisplayed(driver, "//div[contains(@class, 'sum-panel')]//input", 5);
            SendKeysToWebElement("//div[contains(@class, 'sum-panel')]//input", "55", "поле ввода значения ставки", "поля ввода значения ставки");
            Thread.Sleep(1000);
            ClickWebElement("//div[contains(@class, 'place-button')]/div[1]", "Кнопка сделать ставку", "кнопки сделать ставку");
            Thread.Sleep(2000);
            for (int i = 2; i < events.Count; i++)
            {
                var a = events[i].Text;
                double kef = Convert.ToDouble(a, CultureInfo.InvariantCulture);
                var kef2 = Math.Round(kef);
                if (kef2 <= 3 && (!events[i].GetAttribute("class").Contains("_state_blocked")))
                {
                    events[i].Click();
                    break;
                }
            }
            // events[14].Click();
            Thread.Sleep(1000);
            if (driver.FindElement(By.XPath("//div[contains(@class, 'sum-panel')]//input")).GetAttribute("value") != "55")
                throw new Exception("Сумма предыдущего пари не подставляется");
            ClickWebElement("//div[contains(@class,'coupon__header-clear')]", "Крестик закрыть купон", "крестика закрыть купон");

            ClickWebElement("//*[@class='coupons-toolbar--3wv_Z']/div[last()]/div", "Меню настроек", "меню настройки");
            ClickWebElement("//*[@class='settings__content--3IFWX']/div[3]/div/div[1]//input", "Чекбокс Подставлять сумму предыщудего пари", "чекбокса Подставлять сумму предыщудего пари");
            ClickWebElement("//*[@class='settings__buttons--3FfTP']/div[2]", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");
            events[6].Click();
            if (!driver.FindElement(By.XPath("//div[contains(@class, 'sum-panel')]//input")).GetAttribute("class").Contains("state_error"))
                throw new Exception("Поле в сумме не пустое");

            LogStage("Установка пари в один клик в 100 руб");
            ClickWebElement("//*[@class='coupons-toolbar--3wv_Z']/div[last()]/div", "Меню настроек", "меню настройки");
            ClickWebElement("//*[@class='settings__buttons--3FfTP']/div[1]", "Кнопка восстановления настроек по умолчанию", "кнопки восстановления настроек по умолчанию");
            Thread.Sleep(1500);
            ClickWebElement("//*[@class='settings__content--3IFWX']/div[1]/div/div[1]/div[1]//input", "Чекбокс пари в один клик", "чекбокса пари в один клик");
            Thread.Sleep(1500);
            SendKeysToWebElement(".//*[@class='settings__content--3IFWX']/div[1]/div/div[1]/div[2]//input", "100", "поле ввода значения быстрой ставки", "поля ввода значения быстрой ставки");
            ///////////////
            LogStage("Добавление любимого пари");
            ClickWebElement("//*[@value='showPercent']", "Радиобатон любимого пари в % от баланса", "радиобатона любимого пари в % от баланса");
            ClickWebElement("//*[@class='settings__content--3IFWX']/div[3]/div/div[4]/div[2]//i", "Кнопка добавления нового поля для быстрой ставки", "кнопки добавления нового поля для быстрой ставки");
            SendKeysToWebElement("//*[@class='settings__content--3IFWX']/div[3]/div/div[4]/div[last()-1]//input", "50", "новое поле ввода для любимого пари", "нового поля ввода для любимого пари");

            LogStage("Разрешение на прием любимого пари");
            ClickWebElement("//*[@class='settings__content--3IFWX']/div[1]/div/div[3]//input", "Чекбокс принимать пари с измененными коэффициентами", "чекбокса принимать пари с измененными коэффициентами");
            ClickWebElement("//*[@class='settings__content--3IFWX']/div[1]/div/div[5]//input", "Чекбокс принимать пари с измененными тоталами / форами", "чекбокса принимать пари с измененными тоталами / форами");
            ClickWebElement("//*[@class='settings__buttons--3FfTP']/div[2]", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");

            LogStartAction("Проверка работы быстрой ставки");
            IWebElement fastBet = GetWebElement("//div[contains(@class, 'coupons-toolbar__item--1jILz _type_one-click')]/div/div[2]", "Не найдено поле с суммой быстрой ставки");
            var fastBetClass = fastBet.GetAttribute("title");
            if (fastBetClass != "100")
                throw new Exception("Поле с суммой быстрой ставки содержит неверное значение");
            IWebElement fastButton = GetWebElement("//div[contains(@class, 'coupons-toolbar__item--1jILz _type_one-click')]/div/div[1]", "Не найдена кнопка с быстрой ставкой");
            var fastButtonClass = fastButton.GetAttribute("class");
            if (!fastButtonClass.Contains("on"))
                throw new Exception("Быстрое пари выключено");
            LogActionSuccess();

            LogStage("Проверка работы приема любого пари");
            IWebElement betsOdd = GetWebElement("//div[contains(@class, 'coupons-toolbar__item--1jILz _type_switchers')]/div/div[1]", "Не найден элемент Принимать пари с измененными коэф.");
            var betsOddClass = betsOdd.GetAttribute("class");
            if (!betsOddClass.Contains("state_any"))
                throw new Exception("Не работает прием пари с изменными коэффициентами");
            IWebElement betsTotal = GetWebElement("//div[contains(@class, 'coupons-toolbar__item--1jILz _type_switchers')]/div/div[2]", "Не найден элемент Принимать пари с измененными коэф.");
            var betsTotalClass = betsTotal.GetAttribute("class");
            if (!betsTotalClass.Contains("state_on"))
                throw new Exception("Не работает прием пари с изменными тоталами / форами");

            Thread.Sleep(1000);
            ClickWebElement("//div[contains(@class, 'coupons-toolbar__item--1jILz _type_one-click')]/div", "Кнопка быстрой ставки", "кнопки быстрой ставки");

            //Выбор ставки из грида с кэф<2
            IList<IWebElement> grid = driver.FindElements(By.XPath(".//*[@class='table']/tbody//td[10]"));
            for (int i = 2; i < grid.Count; i++)
            {
                var a = grid[i].Text;
                double kef = Convert.ToDouble(a, CultureInfo.GetCultureInfo("en-US").NumberFormat);
                var kef2 = Math.Round(kef);
                if (kef2 <= 2 && (!grid[i].GetAttribute("class").Contains("_state_blocked")))
                {
                    grid[i].Click();
                    break;
                }
            }

            // //Проверка рачета ставки
            //  IWebElement newBetValue = GetWebElement(".//*[@class='coupon__foot-stakes']/a[4]", "Не найдена добавленная кнопка с ставкой в 1%");
            // double betValue = Convert.ToDouble(newBetValue.Text);
            // IWebElement accounBalance = GetWebElement(".//*[@class='header__login-item']//*[@class='header__login-balance']", "Не отображается баланс счета");
            // string input = accounBalance.Text;
            // string pattern = "\\s+";
            // string replacement = "";
            // Regex rgx = new Regex(pattern);
            // string result = rgx.Replace(input, replacement);
            // double balance = Convert.ToDouble(result, CultureInfo.GetCultureInfo("en-US").NumberFormat);
            // //Отнимаю 1 когда баланс не четный, т/к деление тогда получается дробным
            // if (betValue % 2 == 0)
            // {
            //     if (Math.Round(balance / 2) != betValue)
            //     {
            //         throw new Exception("Не корректные расчеты быстрой ставки");
            //     }
            // }
            // else
            // {
            //     if (Math.Round(balance / 2) != betValue - 1)
            //     {
            //         throw new Exception("Не корректные расчеты быстрой ставки");
            //     }
            // }

            string max = driver.FindElement(By.XPath("//div[contains(@class,'min-max')]/span[3]")).Text;
            var result = Convert.ToDouble(Regex.Replace(max, @"[^\d]+", ""));


            if (ClientBalance < result)
            {
                LogStage("Проверка ввода суммы больше чем баланс счета");

                LogStartAction("Сумма ставки равна максимальной");
                ClickWebElement("//div[contains(@class,'min-max')]/span[3]", "Значение максимальной ставки", "значения максимальной ставки");
                ClickWebElement("//div[contains(@class, 'place-button')]/div[1]", "Кнопка заключить пари", "кнопки заключить пари");
                if (!WebElementExist("//div[contains(@class, 'modal-window__user-area _no-funds')]"))
                    throw new Exception("Нет ошибки о нехватке средств");
                ClickWebElement("//div[contains(@class,'modal-window__button-area')]/a[2]", "Кнопка Нет", "кнопки Нет");
                LogActionSuccess();
            }

            LogStartAction("Сумма ставки больше максимальной");
            // var maxBet = Convert.ToDouble(driver.FindElement(By.XPath("//div[contains(@class,'min-max')]/span[3]")).Text);
            SendKeysToWebElement("//div[contains(@class, 'sum-panel')]//input", Convert.ToString(result * 2), "поле ввода значения ставки", "поля ввода значения ставки");
            IWebElement button = GetWebElement("//div[contains(@class, 'place-button')]/div[1]", "Нет кнопки заключить пари");
            if (!button.GetAttribute("class").Contains("disabled"))
                throw new Exception("Кнопка ставки не блокируется");
            LogActionSuccess();
            ClickWebElement("//div[contains(@class,'coupon__header-clear')]", "Кнопка закрытия окна нового пари", "кнопки закрытия окна нового пари");
        }
    }

}
