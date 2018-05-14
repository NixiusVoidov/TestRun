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

            LogStage("Переход в Личный кабинет");
            ClickWebElement(".//*[@class='header__login-head']/div[1]/span", "ФИО в шапке", "ФИО в шапке");
            ClickWebElement(".//*[@id='popup']/li[1]", "Кнопка Личный кабинет", "кнопки Личный кабинет");

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
                LogStage(String.Format("Проверка фильтра \"{0}\"", item.Value));
                ClickWebElement(".//*[@href='#!/account/" + item.Key + "']", "Меню \"" + item.Value + "\"", "меню \"" + item.Value + "\"");
                if (driver.Title != item.Value)
                    throw new Exception(String.Format("Страница не содержит title \"{0}\" ", item.Value));
            }

            LogStage("Проверка фильтра \"Тип пари\"");
            ClickWebElement(".//*[@class='account-calendar__row'][1]/div[1]", "Дата первого видимого дня календаря", "даты первого видимого дня календаря"); //подгружаю события минимум за прошедший месяц
            ClickWebElement(".//*[@class='ui__checkbox-text']/*[text()='Линия']", "Чекбокс Линия", "чекбокса Линия");
            IWebElement betTypeGrid = GetWebElement(".//*[@class='wrap'][1]//*[@class='operation-row _odd']/div[4]", "Не отображается Тип пари в гриде");
            var betTypeGridText = betTypeGrid.Text;
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
                var nameTofind = string.Format(".//*[@class='account-filter']/div[2]//*[@class='ui__checkboxes']/div[{0}]//*[@class='ui__checkbox-text']/span", i);
                var checkbox = driver.FindElement(By.XPath(nameTofind));
                checkbox.Click();
            }
            LogStage("Проверка работы всех фильтров в столбце \"Результат\"");
            for (var i = 1; i <= all.Count; i++)
            {
                var nameTofind = string.Format(".//*[@class='account-filter']/div[2]//*[@class='ui__checkboxes']/div[{0}]//*[@class='ui__checkbox-text']/span", i);
                var dataText = driver.FindElement(By.XPath(nameTofind)).Text;
                driver.FindElement(By.XPath(".//*[@class='ui__checkbox-text']/*[text()='" + dataText + "']")).Click();
                if (!WebElementExist(".//*[@class='page-account__empty-list-text']"))
                {
                    IWebElement betresultGrid = GetWebElement(".//*[@class=\'wrap\'][1]//*[@class=\"operation-row _odd\"]/div[6]/span[1]", "Не отображается Результат пари в гриде");
                    var betresultGridText = betresultGrid.Text;
                    if (!betresultGridText.Equals(dataText))
                        throw new Exception("Не работают фильтры Результата пари");
                }
                ClickWebElement(".//*[@class='ui__checkbox-text']/*[text()='" + dataText + "']", "Чекбокс \"" + dataText + "\"", "чекбокс \"" + dataText + "\"");
            }

            LogStage("Проверка развертки конкретного события");
            ClickWebElement(".//*[@class='ui__checkbox-text']/*[text()='Выигрыш']", "Чекбокс Выигрыш", "чекбокса Выигрыш");
            ClickWebElement(".//*[@class='wrap'][1]//*[@class='operation-row _odd']/div[7]", "Стрелка разворота события", "стрелки разворота события");
            IWebElement betValue = GetWebElement(".//*[@class='bet-details _odd']//table//*[text()='Выигрыш']", "Не отображается Результат пари в развернутом гриде");
            var betValueText = betValue.Text;
            if (!betValueText.Equals("Выигрыш"))
                throw new Exception("Не верный результат в развернутом пари");

            LogStage("Проверка возможности скрыть бланас");
            ClickWebElement(".//*[@class='header__login-head']/div[1]/span", "ФИО в шапке", "ФИО в шапке");
            ClickWebElement(".//*[@id='popup']/li[4]", "Кнопка Скрыть баланс", "кнопки Скрыть баланс");
            if (WebElementExist("//*[@href='/#!/account/deposit']"))
                throw new Exception("Не работают скрытие Баланса аккаунта");
        }
    }
}
