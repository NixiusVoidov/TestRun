using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;


namespace TestRun.fonbet
{
    class BetsSettings : FonbetWebProgram
    {
        public static  CustomProgram FabricateBetsSettings()
        {
            return new BetsSettings();
        }

        public override void Run()
        {
            base.Run();

            LogStage("Установка настроек по умолчанию");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__restore-btn']", "Кнопка восстановления настроек по умолчанию", "кнопки восстановления настроек по умолчанию");

            LogStage("Установка быстрого пари в 100 руб");
            ClickWebElement(".//*[@class='settings__section'][1]/div/div[1]//*[@class='header-ui__checkbox-label']/input", "Чекбокс быстрое пари", "чекбокса быстрое пари");
            driver.FindElement(By.XPath(".//*[@class='settings__section'][1]/div/div//*[@class='settings__row']//input")).Clear(); //Очистить поле для ввода суммы быстрой ставки
            SendKeysToWebElement(".//*[@class='settings__section'][1]/div/div//*[@class='settings__row']//input","100","поле ввода значения быстрой ставки", "поля ввода значения быстрой ставки");

            LogStage("Добавление любимого пари");
            ClickWebElement("//*[@class='settings__section'][1]/div/div//*[@value='showPercent']", "Радиобатон любимого пари в % от баланса", "радиобатона любимого пари в % от баланса");
            ClickWebElement(".//*[@class='settings__row _type_normal _type_columns']//*[@class='settings__fields-action _type_add']", "Кнопка добавления нового поля для быстрой ставки", "кнопки добавления нового поля для быстрой ставки");
            SendKeysToWebElement(".//*[@class='settings__row _type_normal _type_columns']/div/div[4]//input", "50", "новое поле ввода для любимого пари", "нового поля ввода для любимого пари");

            LogStage("Разрешение на прием любимого пари");
            ClickWebElement("//*[@class='settings__section'][1]/div/div[7]//input", "Чекбокс принимать пари с измененными коэффициентами", "чекбокса принимать пари с измененными коэффициентами");
            ClickWebElement("//*[@class='settings__section'][1]/div/div[9]//input", "Чекбокс принимать пари с измененными тоталами / форами", "чекбокса принимать пари с измененными тоталами / форами");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");

            LogStage("Проверка работы быстрой ставки");
            IWebElement fastBet = GetWebElement(".//*[@class='oneClickSum on']", "Не найдено поле с суммой быстрой ставки");
            var fastBetClass = fastBet.GetAttribute("title");
            if (fastBetClass!="100")
               throw new Exception("Поле с суммой быстрой ставки содержит неверное значение");
            IWebElement fastButton = GetWebElement(".//*[@class='oneClickSwitch on']", "Не найдена кнопка с быстрой ставкой");
            var fastButtonClass = fastButton.GetAttribute("class");
            if (!fastButtonClass.Contains("on"))
                throw new Exception("Быстрое пари выключено");

            LogStage("Проверка работы приема любого пари");
            IWebElement betsOdd = GetWebElement(".//*[@class='coupons-toolbar__item _type_switchers']/div/div[1]", "Не найден элемент Принимать пари с измененными коэф.");
            var betsOddClass = betsOdd.GetAttribute("class");
            if (!betsOddClass.Contains("orange any"))
               throw new Exception("Не работает прием пари с изменными коэффициентами");
            IWebElement betsTotal = GetWebElement(".//*[@class='oneClickSwitch on']", "Не найдена кнопка с быстрой ставкой");
            var betsTotalClass = betsTotal.GetAttribute("class");
            if (!betsTotalClass.Contains("on"))
                throw new Exception("Не работает прием пари с изменными тоталами / форами");

            LogStage("Проверка расчета значения ставки в 50% от депозита");
            ClickWebElement(".//*[@class='oneClickSwitch on']", "Кнопка быстрой ставки", "кнопки быстрой ставки");
            ClickWebElement(".//*[@class='line__inner'][2]/a", "Кнопка перехода в меню Линия с панели слева", "кнопки перехода в меню Линия с панели слева");
            //Выбор ставки из грида
            IList<IWebElement> grid = driver.FindElements(By.XPath(".//*[@class='table']/tbody//td[5]"));
            grid[3].Click();
            //Проверка рачета ставки
             IWebElement newBetValue = GetWebElement(".//*[@class='coupon__foot-stakes']/a[4]", "Не найдена добавленная кнопка с ставкой в 1%");
            var betValue = Convert.ToDouble(newBetValue.Text);
            IWebElement accounBalance = GetWebElement(".//*[@class='header__login-item']//*[@class='header__login-balance']", "Не отображается баланс счета");
            string input = accounBalance.Text;
            string pattern = "\\s+";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            string result = rgx.Replace(input, replacement);
            var balance = Convert.ToDouble(result, CultureInfo.GetCultureInfo("en-US").NumberFormat);
            //Отнимаю 1 когда баланс не четный, т/к деление тогда получается дробным
            if (betValue % 2 == 0)
            {
                if (Math.Round(balance / 2) != betValue)
                {
                    throw new Exception("Не корректные расчеты быстрой ставки");
                }
            }
            else
            {
                if (Math.Round(balance / 2) != betValue - 1)
                {
                    throw new Exception("Не корректные расчеты быстрой ставки");
                }
            }

            LogStage("Проверка ввода суммы больше чем баланс счета");
            SendKeysToWebElement(".//*[@class='coupon__foot-sum']/input", "9999999999", "поле ввода значения ставки", "поля ввода значения ставки");
            IWebElement placeBet = GetWebElement(".//*[@class='coupon__foot']/a", "Нет кнопки заключить пари");
            var placeBetClass = placeBet.GetAttribute("class");
            if (!placeBetClass.Contains("state_disabled"))
                throw new Exception("Кнопка не блокируется если сумма ставки выше максимальной");
            ClickWebElement(".//*[@class='coupons__list-inner']//article[1]/div[1]/a[1]", "Кнопка закрытия окна нового пари", "кнопки закрытия окна нового пари");
        }
        
    }

    class CashOutAndDialogsSettings : FonbetWebProgram
    {
        public static CustomProgram FabricateCashOutAndDialogsSettings()
        {
            return new CashOutAndDialogsSettings();
        }

        public override void Run()
        {
            base.Run();

            LogStage("Установка настроек по умолчанию");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__restore-btn']", "Кнопка восстановления настроек по умолчанию", "кнопки восстановления настроек по умолчанию");

            LogStage("Установка быстрого пари");
            ClickWebElement(".//*[@class='settings__section'][1]/div/div[1]//*[@class='header-ui__checkbox-label']/input", "Чекбокс быстрое пари", "чекбокса быстрое пари");

            LogStage("Редактирование Продажи пари");
            ClickWebElement("//*[@class='settings__section'][2]/div[1]//input", "Чекбокс изменение суммы продажи", "чекбокса изменения суммы продажи");
            ClickWebElement("//*[@class='settings__section'][2]/div[3]//input", "Чекбокс показывать продажу на всех вкладках", "чекбокса проказывать продажу на всех вкладках");
           
            LogStage("Редактирование меню Диалоги");
            ClickWebElement("//*[@class='settings__section'][3]/div[1]//input", "Чекбокс 'Не спрашивать подтверждение для быстрого пари'", "чекбокса 'Не спрашивать подтверждение для быстрого пари'");
            ClickWebElement("//*[@class='settings__section'][3]/div[2]//input", "Чекбокс 'Не спрашивать подтверждение для продажи'", "чекбокса 'Не спрашивать подтверждение для продажи'");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");

            LogStage("Проверка работы настроек Продажи пари");
            if (WebElementExist(".//*[@class='coupon__sell-button-area']"))
                throw new Exception("Не работает чекбокс показывать продажу на всех вкладках");
            ClickWebElement(".//*[@class='coupons-toolbar']//*[@title='На продажу']", "Вкладка 'На продажу' в меню купонов", "вкладки 'На продажу' в меню купонов");
            if (!(WebElementExist(".//*[@class='coupon__sell-button-area']") || WebElementExist(".//*[@class='coupon__content-empty']")))
                throw new Exception("Не корректно отрабатывает вкладка меню 'На продажу'");
            IWebElement sellSwitch = GetWebElement(".//*[@class='coupon__sell-button-area']/a[2]", "Нет тумблера рядом с кнопкой продажи пари");
            var sellSwitchClass = sellSwitch.GetAttribute("class");
            if (!sellSwitchClass.Contains("coupon__sell-switch _all"))
                throw new Exception("Не работает чекбокс изменение суммы продажи");

            LogStage("Проверка работы настроек Диалогов");
            SwitchPageToBets();
            IList<IWebElement> bet = driver.FindElements(By.XPath(".//*[@class='table']/tbody//td[5]"));
            bet[4].Click();
            if (WebElementExist(".//*[@class='modal-window__button-area']"))
                throw new Exception("Не работает чекбокс 'Не спрашивать подтверждение для быстрого пари'");
            ClickWebElement(".//*[@class='coupons-toolbar']//*[@title='На продажу']", "Вкладка 'На продажу' в меню купонов", "вкладки 'На продажу' в меню купонов");
            ClickWebElement(".//*[@class='coupons__list-inner']//article[1]//*[@class='coupon__sell-button-area'][1]/a[1]", "Кнопка 'Продать пари'", "кнопки 'Продать пари'");
            if (WebElementExist(".//*[@class='modal-window__button-area']"))
                throw new Exception("Не работает чекбокс 'Не спрашивать подтверждение для продажи'");
        }

    }

    class View : FonbetWebProgram
    {
        public static CustomProgram FabricateView()
        {
            return new View();
        }

        public override void Run()
        {
            base.Run();

            LogStage("Установка настроек по умолчанию");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__restore-btn']", "Кнопка восстановления настроек по умолчанию", "кнопки восстановления настроек по умолчанию");

            LogStage("Установка чекбоксов меню Вид");
            for (var i = 1; i <= 7; i++)
            {
                var nameTofind = string.Format("//*[@class='settings__section'][5]/div/div[{0}]//input", i);
                var element = driver.FindElement(By.XPath(nameTofind));
                element.Click();
            }

            LogStage("Перевод меню в отображение слева");
            ClickWebElement(".//*[@class='settings__section']//span[text()='слева']/../input", "Радиобатон отображения меню слева", "радиобатона отображения меню слева");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");

            LogStage("Проверка отображения номеров");
            if (!WebElementExist(".//*[@class='table__event-number']"))
                throw new Exception("Не работает отображение номеров событий");

            LogStage("Проверка отображения статусов купонов цветом");
            IWebElement couponLable = GetWebElement(".//*[@class='coupon__info-head']/div[3]", "Нет результата купона");
            var couponLableClass = couponLable.GetAttribute("class");
            if (couponLableClass.Contains("style_colored"))
                throw new Exception("Не работает отображения статусов купонов цветом");

            LogStage("Проверка компактного режима отображения купонов");
            IWebElement couponWide = GetWebElement(".//*[@class='page__right']/div[1]", "Пропала лента купонов");
            var couponWideClass = couponWide.GetAttribute("class");
            if (!couponWideClass.Contains("type_compact"))
                throw new Exception("Не работает компактный режим отображения купонов");

            LogStage("Проверка компактного отображения меню в личном кабинете");
            ClickWebElement(".//*[@class='header__login-item'][1]", "Имя пользователя в шапке", "имени пользователя в шапке");
            ClickWebElement(".//*[@href='/#!/account']", "Строка личный кабинет пользователя", "строки личный кабинет пользователя");
            IWebElement accountSidebar = GetWebElement(".//*[@class='page-account__content']/div[1]", "Пропала лента купонов");
            var accountSidebarClass = accountSidebar.GetAttribute("class");
            if (!accountSidebarClass.Contains("compact"))
                throw new Exception("Не работает компактный режим отображения меню в личном кабинете");

            LogStage("Проверка компактного режима отображения подвала сайта");
            IWebElement footerSidebar = GetWebElement(".//*[@id='footerContainer']//footer/div[1]", "Не сворачивается футер");
            var footerSidebarClass = footerSidebar.GetAttribute("class");
            if (!footerSidebarClass.Contains("compact"))
                throw new Exception("Не работает компактный режим отображения подвала сайта");

            LogStage("Проверка автосворачивания элементов в меню событий");
            SwitchPageToBets();
            ClickWebElement(".//*[@class='list-view-new__table-body']/tr[4]//a", "Строка ФУТБОЛ в меню спорта слева", "строки ФУТБОЛ в меню спорта слева");
            IWebElement footballArrow = GetWebElement(".//*[@class='list-view-new__table-body']/tr[4]//*[@class='event-v-list__cell-overlay']/div", "Нет стрекли разворота вида спорта");
            var footballArrowClass = footballArrow.GetAttribute("class");
            if (!footballArrowClass.Contains("state_opened"))
                throw new Exception("Не работает автосворачивание элементов в меню событий");
            ClickWebElement(".//*[@href='#!/bets/hockey']", "Строка Хоккей в меню спорта слева", "строки Хоккей в меню спорта слева");
            IWebElement footballslide = GetWebElement(".//*[@class='list-view-new__table-body']/tr[4]//*[@class='event-v-list__cell-overlay']/div", "Нет стрекли разворота вида спорта");
            var footballslideClass = footballslide.GetAttribute("class");
            if (footballslideClass.Contains("state_opened"))
                throw new Exception("Не работает автосворачивание элементов в меню событий");

            LogStage("Проверка автосворачивания купонов");
            ClickWebElement(".//*[@class='list-view-new__table-body']/tr[5]//*[@class='event-v-list__cell-overlay']/div", "Стрелка сворачивания строки Хоккей", "стрелки сворачивания строки Хоккей");
            ClickWebElement(".//*[@href='#!/bets/0']", "Строка Все события", "строки Все события");
            IList<IWebElement> allBets = driver.FindElements(By.XPath(".//*[@class='table__body']/tr[5]/td[3]"));
            allBets[3].Click();
            allBets[4].Click();
            allBets[5].Click();
            ClickWebElement(".//*[@class='coupons']/div[1]//*[@class='coupon__foot-btn']", "Кнопка заключить пари", "кнопки заключить пари");
            IWebElement couponArrow = GetWebElement(".//*[@class='coupons__list-inner']/div[1]/article[1]/div/i", "Нет стрелки разворота у купона");
            var couponArrowClass = couponArrow.GetAttribute("class");
            if (!couponArrowClass.Contains("expanded"))
                throw new Exception("Не работает автосворачивание купонов");
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
            ClickWebElement(".//*[@class='settings__section']//span[text()='слева']/../input", "Радиобатон отображения меню слева", "радиобатона отображения меню слева");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");

            LogStage("Проверка отображения номеров");
            SwitchPageToBets();
            if (!WebElementExist(".//*[@class='table__event-number']"))
                throw new Exception("Не работает отображение номеров событий");

            LogStage("Проверка сворачивание футера");
            IWebElement footerSidebar = GetWebElement(".//*[@id='footerContainer']//footer/div[1]", "Не сворачивается футер");
            var footerSidebarClass = footerSidebar.GetAttribute("class");
            if (!footerSidebarClass.Contains("compact"))
                throw new Exception("Не работает компактный режим отображения подвала сайта");

            LogStage("Проверка меню слева");
            IWebElement menuBar = GetWebElement(".//*[@class='page__main']/div/div[1]", "Нет фильтр меню");
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

            LogStage("Проверка узкой ленты купонов");
            ClickWebElement(".//*[@class='coupons-toolbar']/div[1]", "Меню отображения списка ставок", "меню отображения списка ставок");
            ClickWebElement(".//*[@id='popupLineMenu']/li[1]", "Кнопка узкая лента купонов", "кнопки узкая ленты купонов");
            IWebElement couponMenu = GetWebElement(".//*[@class='page__right']/div[1]", "Нет отображения всех купонов");
            var couponMenuClass = couponMenu.GetAttribute("class");
            if (couponMenuClass.Contains("wide"))
                throw new Exception("Не работает узкая лента купонов");

            LogStage("Проверка функции Развернуть все купоны");
            ClickWebElement(".//*[@class='coupons-toolbar']/div[1]", "Меню отображения списка ставок", "меню отображения списка ставок");
            ClickWebElement(".//*[@id='popupLineMenu']/li[1]", "Кнопка узкая лента купонов", "кнопки узкая ленты купонов");
            ClickWebElement(".//*[@class='coupons-toolbar']/div[1]", "Меню отображения списка ставок", "меню отображения списка ставок");
            ClickWebElement(".//*[@id='popupLineMenu']/li[2]", "Кнопка Развернуть все купоны", "кнопки Развернуть все купоны");

            LogStartAction("Проверка кол-ва стрелочек у купонов");
            IList<IWebElement> betType = driver.FindElements(By.XPath(".//*[@class='coupon__info-item-inner']//*[@class='coupon__info-text']"));

            var betCount = 0;
            foreach (IWebElement element in betType)
            {
                string typeBet = element.Text;
                if (typeBet!="Одинар")
                    betCount++;
            }
            IList<IWebElement> countArrow = driver.FindElements(By.XPath(".//*[@class='coupon__head _style_gray']/i"));
            if(betCount != countArrow.Count)
                throw new Exception("Не кореектное кол-во стрелок у купонов");
            LogActionSuccess();

            IWebElement arrowMenu = GetWebElement(".//*[@class='coupon__head _style_gray']/i", "Нет стрелок у купонов");
            var arrowMenuClass = arrowMenu.GetAttribute("class");
            if (arrowMenuClass.Contains("expanded"))
                throw new Exception("Не работает Развернуть все купоны");

            LogStage("Проверка функции Свернуть все купоны");
            ClickWebElement(".//*[@class='coupons-toolbar']/div[1]", "Меню отображения списка ставок", "меню отображения списка ставок");
            ClickWebElement(".//*[@id='popupLineMenu']/li[3]", "Кнопка Свернуть все купоны", "кнопки Свернуть все купоны");
            IWebElement menuArrow = GetWebElement(".//*[@class='coupon__head _style_gray']/i", "Нет стрелок у купонов");
            var menuArrowClass = menuArrow.GetAttribute("class");
            if (!menuArrowClass.Contains("expanded"))
                throw new Exception("Не работает Свернуть все купоны");

            LogStage("Проверка вкладки Нерасчитанные");
            ClickWebElement(".//*[@class='coupons-toolbar']/div[1]", "Меню отображения списка ставок", "меню отображения списка ставок");
            ClickWebElement(".//*[@id='popupLineMenu']/li[2]", "Кнопка Развернуть все купоны", "кнопки Развернуть все купоны");
            ClickWebElement(".//*[@class='coupons-toolbar']/div[3]", "Меню \"Нерасчитанные\"", "меню \"Нерасчитанные\"");
            IList<IWebElement> list = driver.FindElements(By.XPath(".//*[@class='coupon _type_list']")); //общее кол-во купонов
            IList<IWebElement> listAccept = driver.FindElements(By.XPath(".//*[@class='coupon__info-head']/div[3]")); //Ставка принято

            if (list.Count != listAccept.Count)
                throw new Exception("Проблемы с фильтром \"Нерассчитанные\"");

            LogStage("Проверка вкладки На продажу");
            ClickWebElement(".//*[@class='coupons-toolbar']/div[4]", "Меню \"На продажу\"", "меню \"На продажу\"");
            IList<IWebElement> grid = driver.FindElements(By.XPath(".//*[@class='coupon _type_list']")); //общее кол-во купонов
            IList<IWebElement> gridSell = driver.FindElements(By.XPath(".//*[@class='coupon__sell-button-area']")); //купоны которые можно продать
            if (grid.Count != gridSell.Count)
                throw new Exception("Проблемы с фильтром \"На продажу\"");

            LogStage("Проверка открытия/закрытия купона целиком");
            if (!WebElementExist(".//*[@class='coupon__content-empty']")) // если купоны существуют
            {
                ClickWebElement(".//*[@class='coupons__list-inner']//article[1]/div[1]/a", "Кнопка Копировать купон", "кнопки Копировать купон");
                ClickWebElement(".//*[@class='coupons']/div[1]//*[@class='coupon__head _new_coupon']/a", "Кнопка закрытия купона целиком", "кнопки закрытия купона целиком");
                if (WebElementExist(".//*[@class='coupon__foot-btn']"))
                    throw new Exception("Не работает кнопка закрытия купона целиком");

                LogStage("Проверка открытия/закрытия купона по событиям");
                ClickWebElement(".//*[@class='coupons__list-inner']//article[1]/div[1]/a", "Кнопка Копировать купон", "кнопки Копировать купон");
                IList<IWebElement> all = driver.FindElements(By.XPath("//*[@class='coupons__list-inner'][1]//article[1]/div[2]//tbody//td[1]"));
                foreach (IWebElement element in all)
                {
                    element.Click();
                }
                if (WebElementExist(".//*[@class='coupon__foot-btn']"))
                    throw new Exception("Не работает кнопка закрытия купона по событиям");
            }

            LogStage("Проверка быстрой ставки и копирования купонов"); 
            ExecuteJavaScript("return (function(){var e = document.getElementsByClassName('oneClickSwitch off')[0];var hasOff = e.classList.contains('off');e.click();var hasOn = e.classList.contains('on');return hasOn == hasOff;})()", "Быстрая ставка не работает");
            if (WebElementExist(".//*[@class='coupons__list-inner']//article/div[1]/a"))
                throw new Exception("Кнопка копирования ставки не исчезает при быстрой ставке");
            if (!WebElementExist(".//*[@class='oneClickSum on']"))
                throw new Exception("Сумма быстрой ставки не отображается");

            LogStage("Проверка приема пари с измен. коэфф");
            IWebElement rateSwitch = GetWebElement(".//*[@class='coupons-toolbar__item _type_switchers']/div/div[1]", "Принимать пари с увеличенными коэффициентами");
            var rateSwitchTitle = rateSwitch.GetAttribute("title");
            ClickWebElement(".//*[@class='coupons-toolbar__item _type_switchers']/div/div[1]", "Кнопка \"Принимать пари с увеличенными коэффициентами\"", "кнопки \"Принимать пари с увеличенными коэффициентами\"");
            if(rateSwitchTitle==rateSwitch.GetAttribute("title"))
                throw new Exception("Не работает прием пари с увелич коэфф.");
            ClickWebElement(".//*[@class='coupons-toolbar__item _type_switchers']/div/div[1]", "Кнопка \"Принимать пари с увеличенными коэффициентами\"", "кнопки \"Принимать пари с увеличенными коэффициентами\"");
            if (rateSwitchTitle == rateSwitch.GetAttribute("title"))
                throw new Exception("Не работает прием пари с увелич коэфф.");
            ClickWebElement(".//*[@class='coupons-toolbar__item _type_switchers']/div/div[1]", "Кнопка \"Принимать пари с увеличенными коэффициентами\"", "кнопки \"Принимать пари с увеличенными коэффициентами\"");
            if (rateSwitchTitle != rateSwitch.GetAttribute("title"))
                throw new Exception("Не работает прием пари с увелич коэфф.");

            LogStage("Проверка пари с изменными тоталами/форами");
            IWebElement totalSwitch = GetWebElement(".//*[@class='coupons-toolbar__item _type_switchers']/div/div[2]", "Принимать пари с увеличенными коэффициентами");
            var totalSwitchTitle = totalSwitch.GetAttribute("title");
            ClickWebElement(".//*[@class='coupons-toolbar__item _type_switchers']/div/div[2]", "Кнопка \"Принимать пари с измененными тоталами/форой\"", "кнопки \"Принимать пари с измененными тоталами/форой\"");
            if (totalSwitchTitle == totalSwitch.GetAttribute("title"))
                throw new Exception("Не работает прием пари с измененными тоталами/форой");
            ClickWebElement(".//*[@class='coupons-toolbar__item _type_switchers']/div/div[2]", "Кнопка \"Принимать пари с измененными тоталами/форой\"", "кнопки \"Принимать пари с измененными тоталами/форой\"");
            if (totalSwitchTitle != totalSwitch.GetAttribute("title"))
                throw new Exception("Не работает прием пари с измененными тоталами/форой");
        }

    }
}
