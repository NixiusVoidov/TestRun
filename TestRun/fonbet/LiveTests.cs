using System;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace TestRun.fonbet
{
    class BroadCastCheck : FonbetWebProgram
    {
        public static CustomProgram FabricateBroadCastCheck()
        {
            return new BroadCastCheck();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();

            LogStage("Переход в Лайф");
            ClickWebElement(".//*[@href='/#!/live']", "Вкладка \"Лайф\"", "вкладки \"Лайф\"");

            LogStage("Выбор трансляции");
            OpenBetsEventFilter();
            ClickWebElement("//*[@href='#!/live/broadcast']", "Меню \"Трансляции\"", "меню \"Трансляции\"");
            IList<IWebElement> grid = driver.FindElements(By.XPath("//*[@class='icon _type_normal _size_17 _icon_channel-external']")); //все элементы с иконками трансляций 1ого типа
            grid[1].Click();

            LogStage("Проверка что трансляция открывается в новом окне");
            var popup = driver.WindowHandles[1];
            if(string.IsNullOrEmpty(popup))
                throw new Exception("Не открылась запись в новом окне");
            driver.SwitchTo().Window(driver.WindowHandles[0]); 
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            if(!(driver.Title.Contains("YouTube") || driver.Title.Contains("Twitch")))
                throw new Exception("Ссылка ведет не ютуб или не твич");
            driver.SwitchTo().Window(driver.WindowHandles[1]).Close();
            driver.SwitchTo().Window(driver.WindowHandles[0]);

            LogStage("Проверка встроенной трансляции");
            IList<IWebElement> tv = driver.FindElements(By.XPath("//*[@class='table__channels'][2]/div")); //все элементы с иконками трансляций 2ого типа
            tv[1].Click();
            if (!WebElementExist(".//*[@class='tv']"))
                throw new Exception("Не работает встронный экран в фрейме с купонами");
            ClickWebElement(".//*[@class='tv__action _type_close']", "Кнопка закрытия встроенной трансляции", "кнопки закрытия встроенной трансляции");

            LogStage("Проверка клика на заблокированное событие");
            if (WebElementExist(".//*[@class='table__col _type_btn _state_blocked']"))
            {
                IList<IWebElement> list = driver.FindElements(By.XPath(".//*[@class='table__col _type_btn _state_blocked']")); //все заблокированные события
                list[0].Click();
                if(WebElementExist(".//*[@class='coupons']/div[1]//*[@class='coupon__title']"))
                    throw new Exception("Можно кликнуть на заблокированное событие");
             }
        }
    }
}
