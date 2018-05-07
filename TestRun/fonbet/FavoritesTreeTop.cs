using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TestRun.fonbet
{
    class FavoritesTreeTop : FonbetWebProgram
    {
        public static  CustomProgram FabricateFavoritesTreeTop()
        {
            return new FavoritesTreeTop();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();

            SwitchPageToBets();

            OpenBetsEventFilter();

            IWebElement footballFilter = GetWebElement(".//*[@href='#!/bets/football']/../span", "Не найден футбол в фильтре");
            var beforeFilterClickClass = footballFilter.GetAttribute("class");
            ClickWebElement(".//*[@href='#!/bets/football']/../span", "Звездочка у футбола в фильтре событий", "звездочки у футбола в фильтре событий");
            var afterFilterClickClass = footballFilter.GetAttribute("class");
            if (beforeFilterClickClass == afterFilterClickClass) { 
                throw new Exception("Функция избранных видов событий в фильтре не работают");}

            ClickWebElement(".//*[@href='#!/bets/football']","Выбор футбола", "выбора футбола");
            ClickWebElement(".//*[@class='events__filter _type_segment']", "Выбор конкретного турнира по футболу", "выбора конкретного турнира по футболу");
            ClickWebElement(".//*[@id='popup']/li[1]/span", "Звездочка конкретного турнира по футболу в фильтре", "звездочки конкретного турнира по футболу в фильтре");
            IWebElement footbalTournamentStar = GetWebElement(".//*[@id='popup']/li[1]/span", "не найдена звезда в фильтре турнира по футболу");
            if (footbalTournamentStar.GetAttribute("class") !="events__filter-star")
                throw new Exception("Звездочки в дочерних событиях футбола не работают");

            ClickOnSportType();
            IWebElement footballFilterWithSemistar = GetWebElement(".//*[@href='#!/bets/football']/../span", "не найден футбол в фильтре");
            if (footballFilterWithSemistar.GetAttribute("class") != "events__filter-star _icon_semi")
                throw new Exception("Полузвездочки в меню футбола не работают");

            ClickWebElement(".//*[@href='#!/bets/favorites']", "Переход к избранному в фильтре", "перехода к избранному в фильтре");
            IWebElement tournamentTitle = GetWebElement(".//*[@class='table__title-text']", "не найден заголовок турнира");
            if (!tournamentTitle.Text.Contains("ФУТБОЛ"))
                throw new Exception("Нет заголовка в футбольных турнирах");

            ClickOnSportType();
            ClickWebElement(".//*[@href='#!/bets/announcements']", "Ананос Лайв в фильтре событий", "по анонсу лайф в фильтре событий");

            var sortByTime = GetWebElement(".//*[@class='events__announce-filter']", "Не найдена кнопка сортировать по времени").Text;
            ClickWebElement(".//*[@class='events__announce-filter']", "В фильтр сортировки по времени/соревнованию", "по фильтру сортировки по времени/соревнованию");
            var sortByCompetiton = GetWebElement(".//*[@class='events__announce-filter']", "Не найдена кнопка сортировать по соревнованию").Text;
            if (sortByTime == sortByCompetiton)
                throw new Exception("Не работает кнопка фильтрации по аремени/совернованию");

            ClickOnSportType();
            ScrollbarMethod(580,480);
            
        }
    }
}
