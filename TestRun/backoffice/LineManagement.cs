using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace TestRun.backoffice
{
    class PopularEventList : BackOfficeProgram
    {
        public static CustomProgram FabricatePopularEventList()
        {
            return new PopularEventList();
        }

        public override void Run()
        {
            base.Run();
            SwitchToWebsiteNewWindow("http://fonred5051.dvt24.com/#!/");
            LogStage("Установка настроек по умолчанию");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__rows']/div[1]//input", "Чекбокс отображать номера событий", "чекбокса отображать номера событий");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");
            LogActionSuccess();
            LogStage("Переход в Лайв");
            ClickWebElement(".//*[@href='/#!/live']", "Вкладка \"Лайв\"", "вкладки \"Лайв\"");
            var titleList = driver.FindElements(By.XPath(".//*[@class='table__title-text']"));
            string competitions = titleList[6].GetAttribute("title"); //соревнование

            var teamList = driver.FindElements(By.XPath("//*[@class='table__match-title-text']"));
            string[] team = teamList[6].Text.Split('—'); //команда
            var events = teamList[7].Text; //событие

            driver.SwitchTo().Window(driver.WindowHandles[0]);


            ClickWebElement(".//*[@href='#/explorer/popularEventsList']//i", "Список популярных событий", "списка популярных событий");
            LogStage("Проверка Календарей на валидность");

            ClickWebElement(".//*[@id='js-toolbar']/div[1]/div[1]/button", "Кнопка Добавить список", "кнопки Добавить список");
            //ClickWebElement("//*[@id='ML_dr16g2leu6w']//span/a[1]//i", "Знак плюс добавления языка в названии списка", "знака плюс добавления языка в названии списка");
            //ClickWebElement("//*[@class='multilang-edit__langs-row']/span/span[1]", "Российский флаг", "Российского флага");
            SendKeysToWebElement(".//*[@class='form__row']/div/div[1]/label//input", "ЧМ-24HSoft", "Название списка", "Названия списка");
            ClickWebElement(".//*[@class='form__row']/div/div[1]/div//input", "Чекбокс все области действия", "чекбокса все области действия");
            LogStartAction("Выбор соревнования");
            ClickWebElement("//*[@class='tabs__head tabs__slider']/span/a[2]", "Вкладка Событие", "Вкладки Событие");
            SendKeysToWebElement(".//*[@class='uxtabs__content--2w4aR']/div[1]//*[@placeholder='Поиск']", competitions, "Строка Поиска", "строки Поиска");
            ClickWebElement("//*[@class='toolbar-icon--16Khq fa fa-square']", "Чекбокс фильтровать", "чекбокса фильтровать");
            WaitTillElementisDisplayed(driver, ".//*[@class='fa fa-square-o']", 10);
            ClickWebElement("//*[@class='uxtabs__content-inner--27p9K state_visible--1XH6e']//*[@class='toolbar--UITRS']/div[1]", "Кнопка Показать", "кнопки Показать");
            ClickWebElement("//*[@class='drop-down__menu--wpyeM _state_visible--3asOr']/div[4]", "Кнопка Развернуть все", "кнопки Развернуть все");
            driver.FindElements(By.XPath(".//*[@class='fa fa-square-o']")).Last().Click();
            LogActionSuccess();
            Thread.Sleep(1500);
            LogStartAction("Выбор команды");
            ClickWebElement("//*[@class='uxtabs__head-items--3iObv']/div[2]", "Вкладка Команды", "Вкладки Команды");
            ClickWebElement("//*[@class='tabs__head tabs__slider']/span/a[2]", "Вкладка Событие", "Вкладки Событие");
            SendKeysToWebElement(".//*[@class='uxtabs__content--2w4aR']/div[2]//*[@placeholder='Поиск']", team[0], "Строка Поиска", "строки Поиска");
            ClickWebElement("//*[@class='toolbar-icon--16Khq fa fa-square']", "Чекбокс фильтровать", "чекбокса фильтровать");
            WaitTillElementisDisplayed(driver, ".//*[@class='fa fa-square-o']", 10);
            ClickWebElement("//*[@class='uxtabs__content-inner--27p9K state_visible--1XH6e']//*[@class='toolbar--UITRS']/div[1]", "Кнопка Показать", "кнопки Показать");
            ClickWebElement("//*[@class='drop-down__menu--wpyeM _state_visible--3asOr']/div[4]", "Кнопка Развернуть все", "кнопки Развернуть все");
            //ClickWebElement("//*[@class='uxtabs__content-inner--27p9K state_visible--1XH6e']//*[@class='virtual-list__list--3TeJI']/div[2]//*[@class='tree-grid__cell__text--zkh8_']", "1ая строка тригрида", "1ой строки тригрида");
            //ClickWebElement("//*[@class='uxtabs__content-inner--27p9K state_visible--1XH6e']//*[@class='virtual-list__list--3TeJI']/div[2]//*[@class='tree-grid__cell__text--zkh8_']", "1ая строка тригрида", "1ой строки тригрида");
        }
    }

}


