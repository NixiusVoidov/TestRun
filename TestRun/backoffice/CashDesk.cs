using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TestRun.backoffice
{
    class CashDeskKind : BackOfficeProgram
    {
        public static CustomProgram FabricateCashDeskKind()
        {
            return new CashDeskKind();
        }

        public override void Run()
        {
            base.Run();

            ClickWebElement(".//*[@href='#/explorer/cashDeskKind']//i", "Тип кассы", "типа кассы");
            LogStage("Добавление нового типа кассы");
            ClickWebElement(".//*[@id='js-toolbar']/div[1]/div[1]/button", "Кнопка Добавить тип кассы", "кнопки Добавить тип кассы");
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input", "АвтотестКасса", "Тип операций", "типа операций");
            ClickWebElement(".//*[@class='toolbar__item form-table__add-button']", "Кнопка добавить операцию", "кнопки добавить операцию");
            SendKeysToWebElement(".//*[@class='form-table__edit-form']/div/label[1]//input", "Тест", "Строка Название", "строки Название");

            ClickWebElement("//*[@class='ui__field-wrap']/div/a", "Разворот Области действия", "разворота Области действия");
            ClickWebElement(".//*[@class='ui-dropdown__items-inner']/div/div[3]", "Строка BkfonInternet", "строки BkfonInternet");
            var operations = driver.FindElements(By.XPath("//*[@class='ui__checkbox-label']"));
            foreach(var n in operations)
            {
                n.Click();
            }
            ClickWebElement("//*[@class='toolbar__btn-text'][text()='Применить']", "Кнопка Применить", "кнопки Применить");
            ClickWebElement("//*[@class='toolbar__btn-text'][text()='Добавить']", "Кнопка Добавить", "кнопки Добавить");

           
        }
    }
}


