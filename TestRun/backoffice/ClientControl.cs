using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;

namespace TestRun.backoffice
{
    class СlientControl : BackOfficeProgram
    {
        public static CustomProgram FabricateСlientControl()
        {
            return new СlientControl();
        }

        public override void Run()
        {
            base.Run();
            ClickWebElement(".//*[@href='#/clientManager']/span[text()='Поиск клиентов']","Меню поиска клиентов", "меню поиска клиентов");
            LogStage("Проверка фильтра Идентификатор клиента");
            SendKeysToWebElement(".//*[text()='Идентификатор клиента']/../div//input", "11", "Поле Идентификатор клиента", "поля Идентификатор клиента");
            Thread.Sleep(500);
            ClickWebElement(".//*[@class='clients__btn-inner']//button", "Кнопка Найти","кнопки Найти");
            waitTillElementisDisplayed(driver, ".//*[@class='clients__result-stats']", 5);
            IWebElement result = GetWebElement(".//*[@class='clients__result-stats']", "Нет результата поиска");
            if (!result.Text.Contains("Найден 1 Клиент"))
              throw new Exception("В поисковой выдаче больше одного клиента");

            LogStage("Проверка фильтра суперклиента");
            ClickWebElement(".//*[text()='Критерий поиска']/../div", "Меню Критерий поиска", "меню Критерий поиска");
            ClickWebElement(".//*[@class='ui-dropdown__caption'][text()='По идентификатору суперклиента']", "По идентификатору суперклиента", "По идентификатору суперклиента");
            Thread.Sleep(500);
            ClearBeforeInput(".//*[text()='Идентификатор суперклиента']/../div//input");
            SendKeysToWebElement(".//*[text()='Идентификатор суперклиента']/../div//input", "345", "Поле Идентификатор суперклиента", "поля Идентификатор суперклиента");
            ClickWebElement(".//*[@class='clients__btn-inner']//button", "Кнопка Найти", "кнопки Найти");
            String newText = driver.FindElement(By.XPath(".//*[@class='clients__result-stats']")).Text;
            if (!newText.Contains("Клиенты не найдены"))
                throw new Exception("В поисковой выдаче клиенты есть");

            LogStage("Проверка фильтра ФИО");
            ClickWebElement(".//*[text()='Критерий поиска']/../div", "Меню Критерий поиска", "меню Критерий поиска");
            ClickWebElement(".//*[@class='ui-dropdown__caption'][text()='По Ф.И.О.']", "Значение По Ф.И.О.", "значения По Ф.И.О.");
            Thread.Sleep(500);
            ClearBeforeInput(".//*[text()='Ф.И.О.']/../div//input");
            SendKeysToWebElement(".//*[text()='Ф.И.О.']/../div//input", "Тестовый", "Поле ФИО", "поля ФИО");
            ClickWebElement(".//*[@class='clients__btn-inner']//button", "Кнопка Найти", "кнопки Найти");
            String fioText = driver.FindElement(By.XPath(".//*[@class='clients__result-stats']")).Text;
            if (!fioText.Contains("Найден"))
                throw new Exception("В поисковой выдаче клиентов нет");

            GeneralTab();
            AdditionalTab();
            OperationTab();
            FreeBet();

        }

    }
}
