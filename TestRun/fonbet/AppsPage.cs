using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TestRun.fonbet
{
    class AppChecking : FonbetWebProgram
    {
        public static CustomProgram FabricateAppChecking()
        {
            return new AppChecking();
        }

        public override void Run()
        {
            base.Run();

            LogStage("Переход в Приложения");
            ClickWebElement(".//*[@href='/#!/apps']", "Вкладка \"Приложения\"", "вкладки \"Приложения\"");
          

            var data = new HashSet<string>
            {
                {"ios"},
                {"android"},
                {"windows"}

            };
           
            foreach (var key in data)
            {
                LogStage("Проверка текстовых блоков");
                string titleApp = String.Format(".//*[@id='{0}']//h2", key);
                string titleAppText = driver.FindElement(By.XPath(titleApp)).Text.ToLower();
                if(!titleAppText.Contains("приложение для " + key + ""))
                    throw new Exception("Отсутствует заголовок приложение для " + key + "");

                LogStage("Проверка графических блоков");
                string appBlock = String.Format(".//*[@id='{0}']//*[@class='appPage__os-body-wrap']/div[1]", key);
                var appBlockClass = driver.FindElement(By.XPath(appBlock)).GetAttribute("class");
                if(appBlockClass.Contains("_hidden"))
                    throw new Exception("По умолчанию стоит не тот переключатель");

                LogStage("Проверка работы переключателя");
                string switcher = String.Format(".//*[@id='{0}']//*[@class='appPage__head-switch']/span[2]", key);
                if (WebElementExist(switcher)) //переключатель айфон/айпад или смартфон/планшет
                {
                    ClickWebElement(switcher, "Кнопка переключения устройства", "кнопки переключения устройства");

                    var appBlockValue = driver.FindElement(By.XPath(appBlock)).GetAttribute("class");
                    if (!appBlockValue.Contains("_hidden"))
                        throw new Exception("Переключатель не работает");
                }
            }
            var macTitle = GetWebElement(".//*[@id='macOS']//h2", "Нет тайтл для мака");
            string macTitleText = macTitle.Text.ToLower();
            if (!macTitleText.Contains("приложение для macos"))
                throw new Exception("Отсутствует заголовок приложение для приложение для macos");
        }
    }
}
