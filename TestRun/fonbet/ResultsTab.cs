﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;

namespace TestRun.fonbet
{
    class ResultsTab : FonbetWebProgram
    {
        public static CustomProgram FabricateResultsTab()
        {
            return new ResultsTab();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();

            LogStage("Переход в Результаты");
            ClickWebElement(".//*[@href='/#!/results']", "Вкладка \"Результаты\"", "вкладки \"Результаты\"");

            LogStage("Проверка сортировки по времени");
            IList<IWebElement> gridNumber = driver.FindElements(By.XPath(".//*[@class='table__time']/span[2]")); //поля времени событий
            ClickWebElement("//*[@class='results__filter-item']//*[@name='sortMode']", "Радиобатон сортировать по времени", "радиобатона сортировать по времени");
            driver.FindElement(By.XPath(".//*[@class='results__filter-item']//*[@name='sortMode']")).Click(); //радиобатон сортировать по времени
            IList<IWebElement> gridTime = driver.FindElements(By.XPath(".//*[@class='table__time']/span[2]"));
            var sortedList = gridTime.OrderBy(t => t.Text);
            if (sortedList == gridNumber)
                throw new Exception("Фильтр по времени не работает");

            LogStage("Проверка сортировки по номеру");
            IList<IWebElement> gridTimeNumber = driver.FindElements(By.XPath(".//*[@class='table__match-title']/span")); //номер события
            ClickWebElement("//*[@class='results__filter-item']//*[@name='sortMode']", "Радиобатон сортировать по номеру", "радиобатона сортировать по номеру");
            IList<IWebElement> gridNumberTime = driver.FindElements(By.XPath(".//*[@class='table__time']/span[2]"));
            var sorted = gridNumberTime.OrderBy(t => t.Text);
            if (gridTimeNumber == sorted)
                throw new Exception("Фильтр по номеру не работает");

            LogStage("Проверка чекбокса Только текущие");
            ClickWebElement(".//*[@class='all_menu results__menu']/div[2]/div[2]//input", "Чекбокс \"Только текущие\"", "чекбокса \"Только текущие\"");
            IList<IWebElement> live = driver.FindElements(By.XPath(".//*[@class='table__label _style_green']")); //все элементы с припиской ЛАЙФ
            IList<IWebElement> allgrid = driver.FindElements(By.XPath(".//*[@class='table__time']/span")); // все элементы на странице
            if (live.Count != allgrid.Count)
                throw new Exception("Чекбокс только текущие -  не работает");

            LogStage("Проверка поиска");
            ClickWebElement(".//*[@class='all_menu results__menu']/div[2]/div[2]//input", "Чекбокс \"Только текущие\"", "чекбокса \"Только текущие\"");
            IList<IWebElement> numberGrid = driver.FindElements(By.XPath(".//*[@class='table__match-title']/span")); //номера событий
            SendKeysToWebElement(".//*[@placeholder='Поиск']", numberGrid[20].Text, "Меню поиск", "меню поиска");
            ClickWebElement("//*[@class='results__filter-item']//*[@name='sortMode']", "Радиобатон сортировать по номеру", "радиобатона сортировать по номеру");
            WaitTillElementisDisplayed(driver, ".//*[@class='results_table']", 5);
            if (driver.FindElements(By.XPath(".//*[@class=\'table__match-title\']")).Count != 1) 
                throw new Exception("Фильтр нашел 2 и более значений");

            LogStage("Проверка логаута");
            ClickWebElement(".//*[@class='header__login-head']/div[1]", "ФИО в шапке", "ФИО в шапке");
            ClickWebElement(".//*[@id='popup']/li[last()]", "Кнопка Выйти из аккаунта", "кнопки выйти из аккаунта");
            Thread.Sleep(1500);
            IWebElement loginButton = GetWebElement(".//*[@class='header__login-head']/a", "Нет кнопки логина");
            string bloginButtonClass = loginButton.GetAttribute("class");
            if (!bloginButtonClass.Contains("header__link"))
                throw new Exception("Логаут не сработал");
        }
    }
}
