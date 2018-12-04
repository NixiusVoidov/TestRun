using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace TestRun.fonbet
{
    class HowToPlay : FonbetWebProgram
    {
        public static CustomProgram FabricateHowToPlay()
        {
            return new HowToPlay();
        }

        protected override bool NeedLogin()
        {
            return false;
        }

        public override void Run()
        {
            base.Run();
            ExecuteJavaScript("window.location.reload()", "Дж скрипт тупит");
            Thread.Sleep(4000);
            if (!WebElementExist(".//*[@class='how2play']"))
               throw new Exception("Нет виджета HowToPlay");

            LogStage("Проверка работы всех разделов меню \"Я хочу заключать пари в интернете\"");
            ClickWebElement(".//*[@class='how2play__help-steps']/div[1]","Кнопка как заключать пари в интернете", "кнопки как заключать пари в интернете");
            if(!WebElementExist(".//*[@class='how2play__stepout _visible']/div[1]//*[@href='#!/account/registration']"))
                throw new Exception("Нет ссылки на быструю регистрацию");

            //ClickWebElement(".//*[@class='how2play__stepout _visible']/div[2]", "Раздел карты Фонбет","раздела карты Фонбет");
            //if (!WebElementExist(".//*[@class='how2play__stepout _visible']/div[2]//*[@href='#!/account/registration/Bk']"))
            //    throw new Exception("Нет ссылки на регистрацию БК");

            //ClickWebElement(".//*[@class='how2play__stepout _visible']/div[3]", "Раздел кошелек Киви", "раздела кошелек Киви");
            //if (!WebElementExist(".//*[@class='how2play__stepout _visible']/div[3]//*[@href='#!/account/registration/Qiwi']"))
            //    throw new Exception("Нет ссылки на регистрацию Киви");

            //ClickWebElement(".//*[@class='how2play__stepout _visible']/div[4]", "Раздел без карты и без киви", "раздела без карты и без киви");
            //if (!WebElementExist(".//*[@class='how2play__stepout _visible']/div[4]//*[@href='#!/account/how2Register']"))
            //    throw new Exception("Нет ссылки на  ИНУЮ регистрацию");

            LogStage("Проверка работы всех разделов меню \"Я хочу заключать пари в клубах\"");
            ClickWebElement(".//*[@class='how2play__help-steps']/div[2]", "Кнопка как заключать пари в клубах", "кнопки как заключать пари в клубах");
            if (!WebElementExist("//*[@href='#!/products/addresses']"))
                throw new Exception("Нет ссылки на найти клубы");
            if (!WebElementExist("//*[@href='#!/pages/fonapps']"))
                throw new Exception("Нет ссылки на \"Как играть на ставкомате\"");
        }
    }
    class Slider : FonbetWebProgram
    {
        public static CustomProgram FabricateSlider()
        {
            return new Slider();
        }

        protected override bool NeedLogin()
        {
            return false;
        }

        public override void Run()
        {
            ExecuteJavaScript("window.location.reload()", "Дж скрипт тупит");
            Thread.Sleep(4000);
            IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;
            jse.ExecuteScript("document.getElementsByClassName('home-slider__switch-wrap')[0].style.opacity = 1");
            if (!WebElementExist(".//*[@class='home-slider__switch-wrap']/div[1]"))
                throw new Exception("На главной странице нет слайдера");

            LogStage("Проверка переключения слайдера по кнопкам");
            IWebElement slider = driver.FindElement(By.XPath(".//*[@class='home-slider']"));
            ClickWebElement(".//*[@class='home-slider__switch-wrap']/div[1]", "Кнопка первой страницы слайдера", "кнопки первой страницы слайдера");
            var sliderButton = GetWebElement(".//*[@class='home-slider__switch-wrap']/div[1]", "Нет кнопки у слайдера");
            var sliderButtonClass = sliderButton.GetAttribute("class");
            if (!sliderButtonClass.Contains("selected"))
                throw new Exception("Проблемы со слайдером на главной странице");
            ClickWebElement(".//*[@class='home-slider__switch-wrap']/div[2]", "Кнопка второй страницы слайдера", "кнопки второй страницы слайдера");
            var sliderSecondButton = GetWebElement(".//*[@class='home-slider__switch-wrap']/div[2]", "У слайдера только одна кнопка");
            var sliderSecondButtonClass = sliderSecondButton.GetAttribute("class");
            if (!sliderSecondButtonClass.Contains("selected"))
                throw new Exception("Проблемы со слайдером на главной странице");

            LogStage("Проверка ставки через слайдер");
            ClickWebElement(".//*[@class='home-slider__bets']/div[1]", "Модуль ставки на странице слайдера", "модуля ставки на странице сладера");
            if (!WebElementExist(".//*[@class='authorization__inner']"))
                throw new Exception("Не появилось окно авторизации");
            SendKeysToWebElement(".//*[@class='login-form__form']/div[1]/input", Login, "поле логина", "поля логина");
            SendKeysToWebElement(".//*[@class='login-form__form']/div[2]/input", Password, "поле пароля", "поля пароля");
            ClickWebElement(".//*[@class='login-form__form-row _right']/div[2]/button", "Кнопка логина", "кнопки логина");
            Thread.Sleep(4000);
            jse.ExecuteScript("document.getElementsByClassName('home-slider__switch-wrap')[0].style.opacity = 1");
            ClickWebElement(".//*[@class='home-slider__switch-wrap']/div[2]", "Кнопка второй страницы слайдера", "кнопки второй страницы слайдера");
            ClickWebElement(".//*[@class='home-slider__bets']/div[1]", "Модуль ставки на странице слайдера", "модуля ставки на странице сладера");
            if (!WebElementExist(".//*[@class='coupons']/div[1]//*[@class='coupon__title']"))
                throw new Exception("Не появилась новая ставка");

        }
       
    }
   
class NewsAndWinnerClub : FonbetWebProgram
    {
        public static CustomProgram FabricateNewsAndWinnerClub()
        {
            return new NewsAndWinnerClub();
        }

        protected override bool NeedLogin()
        {
            return false;
        }

        public override void Run()
        {
            base.Run();


            LogStage("Проверка наличия блока новостей");
            if (!WebElementExist(".//*[@class='home-news__items']"))
                throw new Exception("Нет блока новостей на главной странице сайта");
            //waitTillElementisDisplayed(driver, ".//*[@class='home-news__items']/article[1]/h1", 5);
            //Thread.Sleep(2000);
            LogStage("Проверка отображения и корретной работы блока новостей");
            ClickWebElement(".//*[@class='home-news__items']/article[1]/h1/a", "1ый топик из ленты новостей", "1ого топика из ленты новостей");
            Thread.Sleep(1500);
            WaitTillElementisDisplayed(driver, ".//*[@id='popupModal']", 5);
            ClickWebElement(".//*[@class='news-modal__close']", "Кнопка закрытия popup", "кнопки закрытия popup");

            ClickWebElement(".//*[@class='home-news__source-box']/span[2]", "Вкладка \"Прогнозы\"", "вкладки \"Прогнозы\"");
            var prediction = GetWebElement(".//*[@class='home-news__source-box']/span[2]", "Нет вкладки \"Прогнозы\"");
            var predictionClass = prediction.GetAttribute("class");
            if (!predictionClass.Contains("_selected"))
                throw new Exception("Не переключается на вкладку Прогнозы в шапке новостей");
            ClickWebElement(".//*[@class='home-news__items']/article[2]//a", "2ой топик из ленты новостей", "2ого топика из ленты новостей");
            WaitTillElementisDisplayed(driver, ".//*[@id='popupModal']", 5);
            ClickWebElement(".//*[@class='news-modal__close']", "Кнопка закрытия popup", "кнопки закрытия popup");

            ClickWebElement(".//*[@class='home-news__source-box']/span[3]", "Вкладка \"Спорт с Фонтбет\"", "вкладки \"Спорт с Фонтбет\"");
            var sport = GetWebElement(".//*[@class='home-news__source-box']/span[3]", "Нет вкладки \"Прогнозы\"");
            var sportClass = sport.GetAttribute("class");
            if (!sportClass.Contains("_selected"))
                throw new Exception("Не переключается на вкладку Прогнозы в шапке новостей");
            ClickWebElement(".//*[@class='home-news__items']/article[3]//a", "3ий топик из ленты новостей", "3ого топика из ленты новостей");
            WaitTillElementisDisplayed(driver, ".//*[@id='popupModal']", 5);
            ClickWebElement(".//*[@class='news-modal__close']", "Кнопка закрытия popup", "кнопки закрытия popup");
            Thread.Sleep(1000);
            ClickWebElement(".//*[@class='top-win__items']/article[1]//a", "Топик в модуле \"Клуб победителей\"", "топика в модуле \"Клуб победителей\"");
            WaitTillElementisDisplayed(driver, ".//*[@id='popupModal']", 5);
            ClickWebElement(".//*[@class='news-modal__close']", "Кнопка закрытия popup", "кнопки закрытия popup");

            ClickWebElement(".//*[@href='#!/news/fnl']", "Ссылка на ВСЕ новости", "ссылки на ВСЕ новости");
            var titleNews = GetWebElement(".//*[@class='content-page__title']", "Нет тайтала на странице \"Новости\"");
            var titleNewsText = titleNews.Text;
            if (!titleNewsText.Contains("Новости"))
                throw new Exception("Тайтл не соответсвует странице");
        }
    }

    class CompressionMenu : FonbetWebProgram
    {
        public static CustomProgram FabricateCompressionMenu()
        {
            return new CompressionMenu();
        }

        protected override bool NeedLogin()
        {
            return false;
        }

        public override void Run()
        {
            base.Run();
            var windowSize = new System.Drawing.Size(1400,1100);
            driver.Manage().Window.Size = windowSize;
            LogStage("Проверка меню на компактность");
            if (!WebElementExist(".//*[@class='menu__item _type_more']"))
                throw new Exception("Не работает компактное меню");
            ClickWebElement(".//*[@class='menu__item _type_more']", "Многоточие в меню", "многоточия в меню");
            if (!driver.FindElement(By.XPath(".//*[@class='menu__item _type_more']/a/span")).GetAttribute("class").Contains("expanded"))
                throw new Exception("Не разворачивается компактное меню");
        }
    }
   
    
    class TopThreeEvents : FonbetWebProgram
    {
        public static CustomProgram FabricateTopThreeEvents()
        {
            return new TopThreeEvents();
        }
        //protected override bool NeedLogin()
        //{
        //    return false;
        //}

        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElements(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("error!");
                return false;
            }
        }

        public override void Run()
        {
            base.Run();
            MakeDefaultSettings();
            LogStage("Проверка лайф событий");
            ClickWebElement("//*[@href='/#']", "Переход на главную", "перехода на главную");
            ClickWebElement("//div[contains(@class, 'top-event-selector__live')]", "Иконка Лайв", "иконки Лайв");
            if (WebElementExist("//div[contains(@class,'top-event-list__smart--')]"))
            driver.FindElements(By.XPath("//div[contains(@class,'top-event-list__smart--')]"))[0].Click();

            var scores = driver.FindElements(By.XPath("//div[contains(@class, 'top-event-item__score-container')]"));
            var events = driver.FindElements(By.XPath("//i[contains(@class,'icon_star')]"));
            if (scores.Count != events.Count)
                throw new Exception("Мы не на лайве");
            LogStage("Проверка попадание в избранное");
            LogStartAction("Выбираем событие из лайва");
            events[1].Click();
            ClickWebElement("//div[contains(@class, 'top-event-selector__live')]", "Иконка Лайв", "иконки Лайв");
            LogStartAction("Выбираем событие из линии");
            ClickWebElement("//*[@data-sport='1']/i", "Иконка Футбола", "иконки Футбола");
            var lines = driver.FindElements(By.XPath("//*[@data-place='line']"));
            lines[0].FindElement(By.XPath("div/i")).Click();
            // var stars = driver.FindElements(By.XPath("//i[contains(@class,'icon_star')]"));
            // LogStartAction("Выбираем событие из линии");
            // stars[0].Click();
            LogStartAction("Проверяем избранное");
            SwitchPageToBets();
            ClickWebElement("//div[contains(@class,'line-header__menu')]/div[2]", "Меню Все события", "меню Все события");
            ClickWebElement("//*[@href='#!/bets/favorites']", "Меню Избранное", "меню Избранное");
            if (driver.FindElements(By.XPath("//i[contains(@class,'icon_star')]")).Count != 1)
                throw new Exception("В избранном линии больше чем 1 событие");
            SwitchPageToLive();
            ClickWebElement("//div[contains(@class,'line-header__menu')]/div", "Меню Все события", "меню Все события");
            Thread.Sleep(1000);
            ClickWebElement("//*[@href='#!/live/favorites']", "Меню Избранное", "меню Избранное");
            if (driver.FindElements(By.XPath("//i[contains(@class,'icon_star')]")).Count != 1)
                throw new Exception("В избранном лайва больше чем 1 событие");
            LogActionSuccess();

            LogStage("Проверка контейнеров по чемпионатам");
            ClickWebElement("//*[@href='/#']", "Кнопка Фонбет", "кнопки Фонбет");
            ClickWebElement("//*[@data-sport='1']/i", "Кнопка Футбол", "кнопки Футбол");
            var containers = driver.FindElements(By.XPath("//div[contains(@class,'header__comp-btn-container')]"));
            containers[0].Click();
            if (driver.FindElements(By.XPath("//div[contains(@class,'header__comp-btn-container')]")).Count != 1)
                throw new Exception("Не выбирается контейнер");
            ClickWebElement("//div[contains(@class,'header__comp-btn-container')]/span[2]", "Крестик закрыть контейнер", "крестика закрыть контейнер");
            var separators = driver.FindElements(By.XPath("//span[contains(@class,'top-event-separator')]"));
            separators[0].Click();
            if (driver.FindElements(By.XPath("//div[contains(@class,'header__comp-btn-container')]")).Count != 1)
                throw new Exception("Турниров больше одного");

            LogStartAction("Проверяем eventView");
            var eventsName = driver.FindElements(By.XPath("//div[contains(@class,'top-event-item__title-container')]/a"));
            eventsName[0].Click();
            if (!WebElementExist("//span[contains(@class,'back-button')]"))
                throw new Exception("Не открыл eventView");
            ClickWebElement("//span[contains(@class,'back-button')]", "Стрелка назад в eventView", "стрелки назад в eventView");
            LogActionSuccess();
            var champNameBefore = driver.FindElement(By.XPath("//span[contains(@class,'top-event-header__comp-btn-caption')]")).Text;
            ClickWebElement("//span[contains(@class,'top-event-header__comp-btn-caption')]", "Подпись чемпионата", "подписи чемпионата");
            ClickWebElement("//div[contains(@class,'top-event-header__dropdown')]/span[3]", "Чемпионат из дропдауна", "чемпионата из дропдауна");
            var champNameAfter = driver.FindElement(By.XPath("//span[contains(@class,'top-event-header__comp-btn-caption')]")).Text;
            if (champNameAfter == champNameBefore)
                throw new Exception("Не переключился чемпионат");

            LogStartAction("Проверяем Подборки");
            ClickWebElement("//*[@data-sport='-1']/i", "Кнопка Подборки", "кнопки Подборки");
            if(WebElementExist("//div[contains(@class,'top-event-selector__smart-icon-container')]"))
                driver.FindElement(By.XPath("//div[contains(@class,'top-event-selector__smart-icon-container')]/span")).Click();
            if (!WebElementExist("//div[contains(@class,'top-event-list__smart-container')]"))
                throw new Exception("На текущий момент нет подборок");
            Thread.Sleep(1000);
            ClickWebElement("//div[contains(@class,'top-event-list__smart-container')]/div[1]/div", "Первая подборка в списке", "первой подборки в списке");
            // Thread.Sleep(1000);
            ClickWebElement("//div[contains(@class,'top-event-selector__smart-container')]/div/span", "Крестик у выбранной подборки", "крестика у выбранной подборки");
            if (!WebElementExist("//div[contains(@class,'top-event-list__smart-container')]"))
                throw new Exception("Подборка не закрывается");

        }
    }

    class SeachBar : FonbetWebProgram
    {
        public static CustomProgram FabricateSeachBar()
        {
            return new SeachBar();
        }

        protected override bool NeedLogin()
        {
            return false;
        }

        public override void Run()
        {
            ClickWebElement("//*[@class='search__link']", "Кнопка Поиска в шапке", "кнопки Поиска в шапке");

            LogStage("Проверка фильтров команд");
            SendKeysToWebElement("//*[@id='search-component']/input", "Ф", "Поле поиска", "поля поиска");
            var filterType =
                driver.FindElements(
                    By.XPath("//*[@id='search-filter']//div[contains(@class,'search-filter__item-header')]"));
            if (filterType.Count != 3)
            {
                throw new Exception("Кол-во фильтров не равно 3ем");
            }

            SendKeysToWebElement("//*[@id='search-component']/input", "у", "Поле поиска", "поля поиска");
            Thread.Sleep(1000);
            if (driver.FindElements(
                    By.XPath("//*[@id='search-filter']//div[contains(@class,'search-filter__item-header')]")).Count !=
                2)
            {
                throw new Exception("Кол-во фильтров не равно 2м");
            }

            ClickWebElement("//*[@id='search-filter']/div[1]/*[@id='search-filter__item']", "1ая строка вида спорт",
                "1ой строки вида спорта");
            SendKeysToWebElement("//*[@id='search-component']/input", "л", "Поле поиска", "поля поиска");
            Thread.Sleep(1000);
            ClickWebElement("//*[@id='search-filter']/div[1]/*[@id='search-filter__item']", "1ая строка соревнования",
                "1ой строки соревнования");

            var eventNames = driver.FindElements(By.XPath("//a[contains(@class, 'search-result__event-name')]"));
            string[] values = eventNames[0].Text.Split('—');
            SendKeysToWebElement("//*[@id='search-component']/input", values[0], "Поле поиска", "поля поиска");
            Thread.Sleep(1000);
            if (driver.FindElements(
                    By.XPath("//*[@id='search-filter']//div[contains(@class,'search-filter__item-header')]")).Count !=
                1)
            {
                throw new Exception("Кол-во фильтров не равно 1у");
            }

            ClickWebElement("//*[@id='search-filter']/div[1]/*[@id='search-filter__item']",
                "1ая строка фильтра по команде", "1ой строки  фильтра по команде");

            LogStage("Проверка удаления фильтров и закртытия поиска");
            var searchTagDelete =
                driver.FindElements(
                    By.XPath("//div[contains(@class,'search__tag-list')]//*[@id='search__tag-delete']"));
            searchTagDelete[2].Click();
            Thread.Sleep(1000);
            if (driver.FindElements(By.XPath("//div[contains(@class,'search__tag-list')]//*[@id='search__tag-delete']"))
                    .Count != 2)
                throw new Exception("Не удалился серч тэг");

            ClickWebElement(".//*[@id='search__input-delete']", "Крестик закрывающий поиск",
                "крестика закрывающий поиск");
            if (WebElementExist("//*[@id='search-container']"))
                throw new Exception("Поиск не закрылся");

            ClickWebElement("//*[@class='search__link']", "Кнопка Поиска в шапке", "кнопки Поиска в шапке");
            WaitTillElementisDisplayed(driver, "//*[@id='search-component']/input", 15);
            SendKeysToWebElement("//*[@id='search-component']/input", Keys.Escape, "Поле поиска", "поля поиска");
            if (WebElementExist("//*[@id='search-container']"))
                throw new Exception("Поиск не закрылся");

            LogStage("Проверка на дублирование команд");
            ClickWebElement("//*[@class='search__link']", "Кнопка Поиска в шапке", "кнопки Поиска в шапке");
            SendKeysToWebElement("//*[@id='search-component']/input", Keys.Escape, "Поле поиска", "поля поиска");
            ClickWebElement("//*[@class='search__link']", "Кнопка Поиска в шапке", "кнопки Поиска в шапке");
            SendKeysToWebElement("//*[@id='search-component']/input", "сп", "Поле поиска", "поля поиска");

            var allSports = driver.FindElements(By.XPath("//*[@class='search-filter__sport-header--QEFQi']"));
            for (var i = 1; i <= allSports.Count; i++)
            {
                string dataSport = string.Format("//*[@id='search-filter']//*[@data-sport={0}]", i);
                var eventValues = driver.FindElements(By.XPath(dataSport));
                List<string> myCollection = new List<string>();
                foreach (var team in eventValues)
                {
                    myCollection.Add(team.Text);
                }

                if (myCollection.Count != myCollection.Distinct().Count()) //проверка на дубликаты
                {
                    throw new Exception("В поиске выдает две одинаковые команды");
                } 
            }

            LogStage("Проверка на дублирование фильтров");
            SendKeysToWebElement("//*[@id='search-component']/input", "артак", "Поле поиска", "поля поиска");
            Thread.Sleep(1000);
            var allevents = driver.FindElements(By.XPath("//*[@id='search-filter__item']"));
            List<string> myArr = new List<string>();
            foreach (var a in allevents)
            {
                myArr.Add(a.Text);
            }
           string maxLenthTeam = myArr.OrderByDescending(s => s.Length).First();
            ClearBeforeInput("//*[@id='search-component']/input");
            SendKeysToWebElement("//*[@id='search-component']/input", maxLenthTeam, "Поле поиска", "поля поиска");
            ClickWebElement("//*[@id='search-filter']/div[1]/*[@id='search-filter__item']",
                "1ая строка фильтра по команде", "1ой строки  фильтра по команде");
            SendKeysToWebElement("//*[@id='search-component']/input", maxLenthTeam, "Поле поиска", "поля поиска");
            Thread.Sleep(1000);
            if(WebElementExist("//*[@id='search-filter']"))
                throw new Exception("Идет дублирование фильтров");

            LogStage("Проверка поиска по номеру события и отображение лайв события");
            ClickWebElement("//*[@id='settings-popup']","Кнопка настроек сайта", "кнопки Настроек сайта");
            ClickWebElement("//*[@class='settings__rows']/div[1]//input", "Чекбокс отображать номера событий", "чекбокса отображать номера событий");
            ClickWebElement("//*[@class='settings__head']/a", "Крестик закрыть меню настроек", "крестика закрыть меню настроек");
            SwitchPageToLive();

            var eventNumber = driver.FindElement(By.XPath("//table/tbody[1]/tr[2]/td[2]/div[1]/span")).Text;
            var eventName = driver.FindElement(By.XPath("//table/tbody[1]/tr[2]/td[2]/div[1]/a")).Text;
            var eventScore = driver.FindElement(By.XPath("//table/tbody[1]/tr[2]/td[2]/div[2]/*[@class='table__score']/span")).Text;
            var eventFlag = driver.FindElement(By.XPath("//table/tbody[1]/tr[1]/th[1]/div/span[2]")).GetAttribute("style");
            ClickWebElement("//*[@class='search__link']", "Кнопка Поиска в шапке", "кнопки Поиска в шапке");
            WaitTillElementisDisplayed(driver, "//*[@id='search-component']/input", 15);
            SendKeysToWebElement("//*[@id='search-component']/input", eventNumber, "Поле поиска", "поля поиска");
            if(driver.FindElement(By.XPath("//*[@class='search-result__item--lp1SU']/div[1]//span")).GetAttribute("style")!=eventFlag)
                throw new Exception("Неправильный флаг в выдаче");
            if (driver.FindElement(By.XPath("//*[@class='search-result__item--lp1SU']/div[2]/a")).Text != eventName)
                throw new Exception("Неправильное имя события");
            if (driver.FindElement(By.XPath("//*[@class='search-result__item--lp1SU']/div[2]//div[contains(@class,'search-result__event-score')]")).Text.Replace(" ", string.Empty) != eventScore)
                throw new Exception("Неправильный счет в поиске");

            LogStage("Проверка eventView");
            ClickWebElement("//*[@class='search-result--3VqTJ']//a",
                "Строка с событием", "строки с событием");
            if(!WebElementExist(".//*[@class='ev-scoreboard__event-logo--3hMJr']"))
                throw new Exception("Не открылся eventView");
            ClickWebElement(".//*[@class='ev-scoreboard__back-button--4V1iz']", "Кнопка \"Назад\"", "кнопки \"Назад\"");

            LogStage("Проверка eng выдачи");


            ClickWebElement("//*[@class='header__lang-set']//i", "Иконка выбора языка", "иконки выбора языка");
            ClickWebElement("//*[@class='header__lang-list _state_visible']/a[2]", "Иконка англ языка", "иконки англ языка");
            ClickWebElement("//*[@class='search__link']", "Кнопка Поиска в шапке", "кнопки Поиска в шапке");
            SendKeysToWebElement("//*[@id='search-component']/input", "foot", "Поле поиска", "поля поиска");
            if(driver.FindElement(By.XPath("//*[@id='search-filter__item']")).Text!="Football")
                throw new Exception("Нет выдачи на англ");
        }
    }


}
