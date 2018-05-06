using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestRun.fonbet
{
    class FavoritesTree : FonbetWebProgram
    {
        public static  CustomProgram FabricateFavoritesTree()
        {
            return new FavoritesTree();
        }

        public override void Run()
        {
            base.Run();
            SwitchPageToBets();

            OpenBetsEventFilter();

            IWebElement footballFilter = GetWebElement(".//*[@href='#!/bets/football']/../span", "Не найден футбол в фильтре");
            var beforeFilterClickClass = footballFilter.GetAttribute("class");
            ClickWebElement(".//*[@href='#!/bets/football']/../span", "Звездочка у футбола в фильтре событий", "звездочки у футбола в фильтре событий");
            var afterFilterClickClass = footballFilter.GetAttribute("class");
            if (beforeFilterClickClass == afterFilterClickClass)
                throw new Exception("Функция избранных видов событий в фильтре не работают");
        }
    }
}
