using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

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
            ClickWebElement(".//*[@href='/#!/live']", "Вкладка \"Лайв\"", "вкладки \"Лайв\"");

            LogStage("Выбор трансляции");
            ClickWebElement("//div[contains(@class,'line-header__menu')]/div", "Фильтр событий", "фильтра событий");
            ClickWebElement("//*[@href='#!/live/broadcast']", "Меню \"Трансляции\"", "меню \"Трансляции\"");
            IList<IWebElement> grid = driver.FindElements(By.XPath("//*[@class='icon _type_normal _size_17 _icon_channel-external']")); //все элементы с иконками трансляций 1ого типа
            grid[1].Click();

            LogStage("Проверка что трансляция открывается в новом окне");
            var popup = driver.WindowHandles[1];
            if (string.IsNullOrEmpty(popup))
                throw new Exception("Не открылась запись в новом окне");
            driver.SwitchTo().Window(driver.WindowHandles[0]);
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            if (!(driver.Title.Contains("YouTube") || driver.Title.Contains("Twitch")))
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
                if (WebElementExist(".//*[@class='coupons']/div[1]//*[@class='coupon__title']"))
                    throw new Exception("Можно кликнуть на заблокированное событие");
            }

            LogStage("Проверка трансляции неавторизованным пользователем");
            ClickWebElement("//*[@class='header__item header__login']/div/div[1]", "Меню АККАУНТ", "меню АККАУНТ");
            ClickWebElement(".//*[@id='popup']/li[last()]", "Кнопка Выход", "кнопки Выход");
            IList<IWebElement> tv2 = driver.FindElements(By.XPath("//*[@class='table__channels'][2]/div")); //все элементы с иконками трансляций 2ого типа
            tv2[1].Click();
            if (!WebElementExist(".//*[@class='authorization__text']"))
                throw new Exception("Не появилось окно авторизации для просмотра видео");
           

        }
    }


    class EventViewLive : FonbetWebProgram
    {
        public static CustomProgram FabricateEventViewLive()
        {
            return new EventViewLive();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();

            LogStage("Переход в Лайв");
            ClickWebElement(".//*[@href='/#!/live']", "Вкладка \"Лайв\"", "вкладки \"Лайв\"");
            ClickWebElement(".//*[@class='line-header__head--1D5p_']/div/span", "сендвич \"меню\"", "сендвича \"меню\"");
            ClickWebElement(".//*[@id='popup']/li[1]", "Меню слева", "Меню слева");

            LogStartAction("Открываем Eventview");
            IList<IWebElement> columns = driver.FindElements(By.XPath(".//span[contains(@class, 'flag')]/ancestor::tbody[contains(@class, 'table__body')]//div[contains(@class, '_state_expanded')]/ancestor::tr[contains(@class, 'table__row')]//a[contains(@class, 'table__match-title-text')]")); // Все строки  c флагом и стрелкой
            columns[0].Click();

            LogStartAction("Проверяем на флаги, таймер и комментарии");
            if (!WebElementExist(".//*[@class='ev-scoreboard__event-logo--3hMJr']"))
            {
                if (!WebElementExist(".//*[@class='ev-scoreboard__event-logo--3hMJr _no-timer--2jUtY']")) // при отсутствии времени
                    throw new Exception("Не оборажается флаг турнира в ивент вью");
                else
                    throw new Exception("Не оборажается флаг турнира в ивент вью");
            }
            if (WebElementExist(".//*[@class='ev-scoreboard__event-logo--3hMJr']"))
            {
                if (!WebElementExist(".//*[@class='ev-scoreboard__timer-text--3Ulee']"))
                    throw new Exception("Не оборажается таймер события в ивент вью");
            }
           
            if (!WebElementExist(".//*[@class='ev-scoreboard__comment--3u2eF']"))
                throw new Exception("Не оборажается поле комментария в ивент вью");
            LogActionSuccess();

            LogStartAction("Проверяем переключение табов");
            var tabs = driver.FindElements(By.XPath(".//*[@class='ev-tabs__item--NcTxT']"));
            tabs[0].Click();
           
            if(!tabs[0].GetAttribute("class").Contains("state_select"))
                throw new Exception("Непереключается таб");
            LogActionSuccess();

            LogStartAction("Проверяем видео фрейм и избранное");
            IWebElement star = GetWebElement(".//*[@class='ev-scoreboard__favorite-icon--27rCl']", "Звездочка в событии");
            ClickWebElement(".//*[@class='ev-scoreboard__favorite-icon--27rCl']", "Звездочка в ивентвью", "звездочки в ивентвью");
            if (!star.GetAttribute("class").Contains("state_on"))
                throw new Exception("Звездочка не нажимается");
            ClickWebElement(".//*[@href='#!/live/favorites']", "Меню Избранное", "меню Избранное");
            IList<IWebElement> events = driver.FindElements(By.XPath(".//*[@class='table']/tbody"));
            if(events.Count!=1)
                throw new Exception("Событие не добавилось в избранное");
            ClickWebElement(".//*[@href='#!/live/broadcast']", "Меню Трансляция", "меню Трансляция");
            ClickWebElement(".//*[@class='table__match-title-text']", "Событие из грида", "события из грида");
            ClickWebElement("//div[contains(@class, 'ev-scoreboard__channel--2wTx7')]", "Кнопка трансляции", "кнопки трансляции");
            if (!WebElementExist(".//*[@class='tv']"))
                throw new Exception("Не открылся телевизор");
            ClickWebElement("//span[contains(@class, 'icon_betradar--')]", "Кнопка СпортРадар", "кнопки СпортРадар");
            var popup = driver.WindowHandles[1];
            if (string.IsNullOrEmpty(popup))
                throw new Exception("Не открылся спортрадар в новом окне");
            driver.SwitchTo().Window(driver.WindowHandles[0]);
            LogActionSuccess();

            LogStartAction("Проверяем возврат на предыдущий шаг");
            ClickWebElement(".//*[@class='ev-scoreboard__back-button--4V1iz']", "Кнопка \"Назад\"", "кнопки \"Назад\"");
            string url = driver.Url;
            if (!url.Contains("/#!/live/broadcast"))
                throw new Exception("Не рабоатет кнопка назад из ивентвью");
            LogActionSuccess();

        }
    }
}
