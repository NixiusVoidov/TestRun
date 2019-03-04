using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;

namespace TestRun.fonbet
{
    class FavoritesTree : FonbetWebProgram
    {
        public static CustomProgram FabricateFavoritesTreeTop()
        {
            return new FavoritesTree();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            SwitchPageToBets();
            OpenBetsEventFilter();

            LogStage("Проверка работы звездочки в футболе");
            IWebElement footballFilter = GetWebElement(".//*[@href='#!/bets/football']/../span", "Не найден футбол в фильтре");
            var beforeFilterClickClass = footballFilter.GetAttribute("class");
            ClickWebElement(".//*[@href='#!/bets/football']/../span", "Звездочка у футбола в фильтре событий", "звездочки у футбола в фильтре событий");
            var afterFilterClickClass = footballFilter.GetAttribute("class");
            if (beforeFilterClickClass == afterFilterClickClass)
            {
                throw new Exception("Функция избранных видов событий в фильтре не работают");
            }

            LogStage("Проверка работы суперзвездочки в футболе и черного списка турнира");
            ClickWebElement(".//*[@href='#!/bets/football']", "Звездочка у футбола в фильтре событий", "звездочки у футбола в фильтре событий");
            ClickWebElement(".//*[@class='line-header__menu--GWd-F']/div[3]", "Конкретный турнир по футболу", "конкретного турнира по футболу");

            var allStars = driver.FindElements(By.XPath(".//*[@id='popup']/li/span"));
            for (var i = 0; i < allStars.Count; i++)
            {
                if (!allStars[i].GetAttribute("class").Contains("icon_on"))
                    throw new Exception("Не все турниры выбраны суперзвездой");
            }
            IWebElement footbalTournamentStar = GetWebElement(".//*[@id='popup']/li[1]/span", "Не найдена звезда в фильтре турнира по футболу");
            footbalTournamentStar.Click();
            if (!footbalTournamentStar.GetAttribute("class").Contains("icon_black-list"))
                throw new Exception("Турнир не попал в черный список");

            LogStage("Проверка что черный список снял звезды с матчей турнира");
            ClickWebElement(".//*[@class='line-header__menu--GWd-F']/div[3]", "Конкретный турнир по футболу", "конкретного турнира по футболу");
            var firstTourStars = driver.FindElements(By.XPath(".//*[@class='table']/tbody[1]//*[@class='table__star']/i"));
            for (var i = 0; i < firstTourStars.Count; i++)
            {
                if (firstTourStars[i].GetAttribute("class").Contains("state_checked"))
                    throw new Exception("Черный список не убирает звездочки с матчей");
            }

            LogStage("Проверка работы полузвездочки в футболе и белого списка в турнире");
            OpenBetsEventFilter();
            IWebElement footballFilterStart = GetWebElement(".//*[@href='#!/bets/football']/../span", "не найден футбол в фильтре");
            footballFilterStart.Click();
            ClickWebElement(".//*[@class='line-header__menu--GWd-F']/div[3]", "Конкретный турнир по футболу", "конкретного турнира по футболу");
            driver.FindElement(By.XPath(".//*[@id='popup']/li[1]/span")).Click();
            if(!driver.FindElement(By.XPath(".//*[@id='popup']/li[1]/span")).GetAttribute("class").Contains("icon_white-list"))
                throw new Exception("Турнир не попал в белый список");
            OpenBetsEventFilter();
            if(!driver.FindElement(By.XPath(".//*[@href='#!/bets/football']/../span")).GetAttribute("class").Contains("icon_semi"))
                throw new Exception("У футбола нет полузвездочки");

            LogStage("Проверка что футбол попал в избранное");
            ClickWebElement(".//*[@href='#!/bets/favorites']", "Переход к избранному в фильтре", "перехода к избранному в фильтре");
            IWebElement tournamentTitle = GetWebElement(".//*[@class='table__title-text']", "не найден заголовок турнира");
            if (!tournamentTitle.Text.ToUpper().Contains("ФУТБОЛ"))
                throw new Exception("Нет заголовка в футбольных турнирах");

            LogStage("Проверка работы АнонсЛайф");
            OpenBetsEventFilter();
            ClickWebElement(".//*[@href='#!/bets/announcements']", "Анонс Лайв в фильтре событий", "по анонсу лайф в фильтре событий");
            string sortByTime = GetWebElement("//div[contains(@class, 'line-header__announce-filter')]", "Не найдена кнопка сортировать по времени").Text;
            ClickWebElement("//div[contains(@class, 'line-header__announce-filter')]", "Фильтр сортировки по времени/соревнованию", "Фильтра сортировки по времени/соревнованию");
            string sortByCompetiton = GetWebElement("//div[contains(@class, 'line-header__announce-filter')]", "Не найдена кнопка сортировать по соревнованию").Text;
            if (sortByTime == sortByCompetiton)
                throw new Exception("Не работает кнопка фильтрации по времени/совернованию");

            LogStage("Проверка работы скрола в меню фильтра");
            OpenBetsEventFilter();
            CheckScrollinFilterTopMenu(580, 480);

        }
    }

    class FavoritesTreeLeft : FonbetWebProgram
    {
        public static CustomProgram FabricateFavoritesTreeLeft()
        {
            return new FavoritesTreeLeft();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            SwitchPageToBets();
            SwitchToLeftTypeMenu();



            LogStage("Проверка работы звездочки в футболе");
            IWebElement footballFilter = GetWebElement(".//*[@href='#!/bets/football']/div/div[3]", "Не найден футбол в фильтре");
            var beforeFilterClickClass = footballFilter.GetAttribute("class");
            ClickWebElement(".//*[@href='#!/bets/football']/div/div[3]", "Звездочка у футбола в фильтре событий", "звездочки у футбола в фильтре событий");
            var afterFilterClickClass = footballFilter.GetAttribute("class");
            if (beforeFilterClickClass == afterFilterClickClass)
            {
                throw new Exception("Функция избранных видов событий в фильтре не работают");
            }

            LogStage("Проверка работы суперзвездочки в футболе и черного списка турнира");
            ClickWebElement(".//*[@href='#!/bets/football']", "Звездочка у футбола в фильтре событий", "звездочки у футбола в фильтре событий");

            IWebElement footbalTournamentStar = GetWebElement(".//*[@class='list-view-new__table-body']//tr[8]//a/div/div[last()]", "Не найдена звезда в фильтре турнира по футболу");
            footbalTournamentStar.Click();
            if (!footbalTournamentStar.GetAttribute("class").Contains("state_blackList"))
                throw new Exception("Турнир не попал в черный список");

            LogStage("Проверка что черный список снял звезды с матчей турнира");
            var firstTourStars = driver.FindElements(By.XPath(".//*[@class='table']/tbody[3]//*[@class='table__star']/i"));
            for (var i = 0; i < firstTourStars.Count; i++)
            {
                if (firstTourStars[i].GetAttribute("class").Contains("state_checked"))
                    throw new Exception("Черный список не убирает звездочки с матчей");
            }

            LogStage("Проверка работы полузвездочки в футболе и белого списка в турнире");
            IWebElement footballFilterStart = GetWebElement(".//*[@href='#!/bets/football']/div/div[3]", "не найден футбол в фильтре");
            footballFilterStart.Click();
            ClickWebElement(".//*[@class='list-view-new__table-body']//tr[8]//a/div/div[last()]", "Конкретный турнир по футболу", "конкретного турнира по футболу");
            if (!driver.FindElement(By.XPath(".//*[@class='list-view-new__table-body']//tr[8]//a/div/div[last()]")).GetAttribute("class").Contains("state_whiteList"))
                throw new Exception("Турнир не попал в белый список");
            if (!driver.FindElement(By.XPath(".//*[@href='#!/bets/football']/div/div[3]")).GetAttribute("class").Contains("state_semi"))
                throw new Exception("У футбола нет полузвездочки");

            LogStage("Проверка что футбол попал в избранное");
            ClickWebElement(".//*[@href='#!/bets/favorites']", "Переход к избранному в фильтре", "перехода к избранному в фильтре");
            waitTillElementContains(driver, ".//*[@class='list-view-new__table-body']/tr[2]//td/a/div", "state_disabled");
            IWebElement tournamentTitle = GetWebElement(".//*[@class='table__title-text']", "не найден заголовок турнира");
            if (!tournamentTitle.Text.ToUpper().Contains("ФУТБОЛ"))
                throw new Exception("Нет заголовка в футбольных турнирах");

            LogStage("Проверка работы АнонсЛайф");
            ClickWebElement(".//*[@href='#!/bets/announcements']", "Анонс Лайв в фильтре событий", "по анонсу лайф в фильтре событий");
            string sortByCompetiton = GetWebElement(".//*[@class='event-v-list__announce-filter-btn']", "Не найдена кнопка сортировать по соревнованию").Text;
            ClickWebElement(".//*[@class='event-v-list__announce-filter-btn']", "Фильтр сортировки по времени/соревнованию", "фильтра сортировки по времени/соревнованию");
            ClickWebElement(".//*[@class='event-v-list__announce-popup-menu']/li[2]", "Фильтр сортировки по времени", "фильтра сортировки по времени");
            string sortByTime = GetWebElement(".//*[@class='event-v-list__announce-filter-btn']", "Не найдена кнопка сортировать по соревнованию").Text;
            if (sortByTime == sortByCompetiton)
                throw new Exception("Не работает кнопка фильтрации по времени/совернованию");

            LogStage("Проверка работы скрола в меню фильтра");
            var windowSize = new System.Drawing.Size(580, 480);
            driver.Manage().Window.Size = windowSize;
            ExecuteJavaScript("return document.getElementsByClassName(\"list-view-new__scroll-box\")[\"0\"].scrollHeight>document.getElementsByClassName(\"list-view-new__scroll-box\")[\"0\"].clientHeight;",
                "Не работает кнопка фильтрации по аремени/совернованию");
        }
    }

    class FavoritesExpandTree : FonbetWebProgram
    {
        public static CustomProgram FabricateFavoritesExpandTree()
        {
            return new FavoritesExpandTree();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            SwitchPageToBets();

            LogStage("Выбор событий в 12 ближ часов");

            ClickWebElement(".//*[@class='line-header__menu--GWd-F']/div[1]", "Меню времени в фильтре", "меню времени в фильтре");
            ClickWebElement(".//*[@id='popup']/li[6]", "Значение ближайших 12 часов", "значения ближайших 12 часов");
            if (WebElementExist(".//*[@class='table__empty table__flex-container']"))
                throw new Exception("Нет событий чтобы выполнить тест");

            LogStage("Развернуть все дочерние события");
            ClickWebElement(".//*[@class='line-header__head--1D5p_']/div[1]", "Разворот меню фильтра", "разворота меню фильтра");
            ClickWebElement(".//*[@id='popup']/li[2]", "Графа разворота всех дочерних событий", "графы разоворота всех дочерних событий");
            if (!WebElementExist(".//*[@class='table__col _pos_first _indent_2']"))
                throw new Exception("Не работает развертка дочерних событий");

            LogStage("Свернуть все дочерние события");
            ClickWebElement(".//*[@class='line-header__head--1D5p_']/div[1]", "Разворот меню фильтра", "разворота меню фильтра");
            ClickWebElement(".//*[@id='popup']/li[3]", "Графа сворачивания всех дочерних событий", "графы сворачивания всех дочерних событий");

            LogStage("Развернуть все дополнительные пари");
            ClickWebElement(".//*[@class='line-header__head--1D5p_']/div[1]", "Разворот меню фильтра", "разворота меню фильтра");
            ClickWebElement(".//*[@id='popup']/li[4]", "Графа разворота всех дополнительных пари", "графы разоворота всех дополнительных пари");
            if (!WebElementExist(".//*[@class='table__match-title _type_with-details _indent_1 _state_expanded']"))
                throw new Exception("Не работает развертка дополнительных пари");

            LogStage("Свернуть все дополнительные пари");
            ClickWebElement(".//*[@class='line-header__head--1D5p_']/div[1]", "Разворот меню фильтра", "разворота меню фильтра");
            ClickWebElement(".//*[@id='popup']/li[5]", "Графа сворачивания всех дополнительных пари", "графы сворачивания всех дополнительных пари");

        }
    }

    class TimeTree : FonbetWebProgram
    {
        public static CustomProgram FabricateTimeTree()
        {
            return new TimeTree();
        }


        public override void Run()
        {
            base.Run();


            MakeDefaultSettings();
            SwitchPageToBets();

            Dictionary<int, string> times = new Dictionary<int, string>()
            {
                { 60, "Ближайший час" },
                { 120, "Ближайшие 2 часа" },
                { 240, "Ближайшие 4 часа" },
                { 360, "Ближайшие 6 часов" },
                { 720, "Ближайшие 12 часов" },
                { 1440, "Ближайший день" },
                { 2880, "Ближайшие 2 дня" }
            };

            foreach (KeyValuePair<int, string> item in times)
            {
                LogStage(String.Format("Проверка фильтра \"{0}\"", item.Value));
                TimeFilterChecker(item.Key, item.Value);
                ClickWebElement("//*[@class='line-header__menu--GWd-F']//div[contains(@class, 'type_sport')]", "Дропдаун весь спорт", "Дропдауна Весь спорт");
                if (!WebElementExist("//*[@id='popup']"))
                    throw new Exception("Не открылось меню весь спорт");

            }

            LogStage("Проверка работы скрола в меню фильтра");
            ClickWebElement("//div[contains(@class,'line-header__menu')]/div[1]", "Меню времени в фильтре", "меню времени в фильтре");
            CheckScrollinFilterTopMenu(480, 350);

        }
    }
}
