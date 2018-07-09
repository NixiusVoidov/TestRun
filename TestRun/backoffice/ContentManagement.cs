using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Threading;
using OpenQA.Selenium.Remote;

namespace TestRun.backoffice
{
    class ContentBlog : BackOfficeProgram
    {
        public static CustomProgram FabricateContentBlog()
        {
            return new ContentBlog();
        }

        public override void Run()
        {
            base.Run();
           
            LogStage("Удаление тестовых записей");
            ClickWebElement(".//*[@class='menu']//*[@href='#/explorer/content']", "Меню Управление клиентом", "меню Управление клиентом");
            ClickWebElement(".//*[@class='curtain__list']/li[1]", "Строка Блог", "Строки Блог");
            IEnumerable<IWebElement> testData = driver.FindElements(By.XPath(".//*[@class='curtain _state_expanded']/div[2]//li//*[@class='curtain__news-title-inner']")).Take(10);
            var dataArray = testData.Where(n => n.Text.Contains("Тестовый заголовок")).ToArray();
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            for (int i =0; i< dataArray.Length-1; i++)
            {
                waitTillElementisDisplayed(driver, ".//*[@class='form__title']", 2);
                js.ExecuteScript("arguments[0].click()", dataArray[i]);
                    ClickWebElement(".//*[@id='js-toolbar']/div/div[7]//button", "Кнопка Удалить", "кнопки Удалить");
                    waitTillElementisDisplayed(driver, ".//*[@class='modal__foot']/div[2]/a", 2);
                    ClickWebElement(".//*[@class='modal__foot']/div[2]/a", "Кнопка Ок всплывающего окна", "кнопки Ок всплывающего окна");
                
            }

            LogStage("Добавление нового блога в Меню управление клиентом");
            ClickWebElement(".//*[@class='menu']//*[@href='#/explorer/content']", "Меню Управление клиентом", "меню Управление клиентом");
            ClickWebElement(".//*[@id='js-toolbar']/div[1]/div[1]", "Кнопка Добавить", "кнопки Добавить");
            ClickWebElement(".//*[@class='toolbar__drop-down _state_visible']/div[1]", "Строка Блог", "строки Блог");
            SetupVisualSettings();
            ClickWebElement(".//*[@class='role-form__inner']/div[2]//*[@class='ui__list-node right-list__row'][3]//input", "Чекбокс Новости Фонбет", "чекбокса Новости Фонбет");
            LogStage("Заполнение вкладки Содержимое");
            ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[3]", "Вкладка Содержимое", "вкладки Содержимое");
            SendKeysToWebElement(".//*[@class='form__row']/label[2]//input", "Тестовый заголовок", "Поле Заголовок", "поля Заголовок");
            SendKeysToWebElement(".//*[@class='form__row']/label[3]//textarea", "Тестовый Анонс", "Поле Анонс", "поля Анонс");
            SendKeysToWebElement(".//*[@class='form__row']/label[4]//textarea", "Тестовый Текст новости", "Поле Текст новости", "поля Текст новости");
            SendKeysToWebElement(".//*[@class='form__row _gaps-top_20']/div[2]//input", "/Test/XYZ/ABC/52130392.jpg", "Поле Маленькое изображение", "поля Маленькое изображение");
            SendKeysToWebElement(".//*[@class='form__row _gaps-top_20']/div[3]//input", "/Test/XYZ/ABC/1457029630_aprilia-rsv4-rr-01-1.jpg", "Поле Большое изображение", "поля Большое изображение");
            ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[1]", "Вкладка Общее", "вкладки Общее");
            ClickWebElement(".//*[@class='ui__checkbox-item']//input", "Чекбокс Опубликовано", "чекбокса Опубликовано");
            ClickWebElement(".//*[@class='form__buttons']/div[1]/button", "Кнопка Добавить", "кнопки Добавить");

            SwitchToPreView();
            if (!WebElementExist(".//*[@class='content-list__big-image-inner']"))
                throw new Exception("Не отображается большая картинка");
            ClickWebElement(".//*[@class='content-page__categories']/li[2]", "Вкладка Спорт с Фонбет", "вкладки Спорт с Фонбет");
            if (!WebElementExist(".//*[@class='content-list']/article[1]//*[@class='content-list__image']"))
                throw new Exception("Не отображается маленькая картинка");
        }


    }
    class ContentGeneralTab : BackOfficeProgram
    {
        public static CustomProgram FabricateContentGeneralTab()
        {
            return new ContentGeneralTab();
        }

        public override void Run()
        {
            base.Run();
            LogStage("Выбор нового блога в Меню управление клиентом");
            ClickWebElement(".//*[@class='menu']//*[@href='#/explorer/content']", "Меню Управление клиентом", "меню Управление клиентом");
            ClickWebElement(".//*[@class='curtain__list']/li[1]", "Строка Блог", "строки Блог");
            ClickWebElement(".//*[@id='curtain']/div/div[2]//li[1]", "1ый блог из списка", "1ого блога из списка");
            IWebElement title =GetWebElement(".//*[@id='curtain']/div/div[2]//li[1]//*[@class='curtain__news-title-inner']", "Нет тайтла");
            string titleText = title.Text;

            LogStage("Проверка функции Клонирование");

            ClickWebElement(".//*[@id='js-toolbar']/div/div[3]//button", "Кнопка Клонировать", "кнопки Клонировать");
            ClickWebElement(".//*[@class='form__buttons']/div[1]/button", "Кнопка Добавить", "кнопки Добавить");
            if (driver.FindElement(By.XPath(".//*[@id='curtain']/div/div[2]//li[1]")).GetAttribute("class").Contains("state_selected"))
                throw new Exception("Клонирование не сработало");
            ClickWebElement(".//*[@id='curtain']/div/div[2]//li[1]", "1ый блог из списка", "1ого блога из списка");
            if (driver.FindElement(By.XPath(".//*[@id='curtain']/div/div[2]//li[1]//*[@class='curtain__news-title-inner']")).Text != titleText)
                throw new Exception("При клонировании поменялись тайтлы");

            LogStage("Проверка функции Удаление");
            var count = driver.FindElements(By.XPath(".//*[@id='curtain']/div/div[2]//li")).Count;
            ClickWebElement(".//*[@id='js-toolbar']/div/div[7]//button", "Кнопка Удалить", "кнопки Удалить");
            ClickWebElement(".//*[@class='modal__foot']/div[2]/a", "Кнопка Ок всплывающего окна", "кнопки Ок всплывающего окна");
            if (driver.FindElements(By.XPath(".//*[@id='curtain']/div/div[2]//li")).Count == count)
                throw new Exception("При удалении число блогов не изменилось");

            LogStage("Проверка функции Показывать только активные");
            var countМValue = driver.FindElements(By.XPath(".//*[@id='curtain']/div/div[2]//li")).Count;
            ClickWebElement(".//*[@id='js-toolbar']/div[2]/div/div[1]", "Кнопка Показывать только активные", "кнопки Показывать только активные");

            if (driver.FindElements(By.XPath(".//*[@id='curtain']/div/div[2]//li")).Count == countМValue)
                throw new Exception("\"Показать только активные\" не работает");

            LogStage("Проверка фильтров");
            ContentApplicationsFilter();
            ClickWebElement(".//*[@class='curtain__list']/li[2]", "Строка Ставка дня", "строки Ставка дня");
            ClickWebElement(".//*[@class='curtain__list']/li[1]", "Строка Блог", "строки Блог");
            ContentCategoriesFilter();
            ContentMarketsFilter();

            LogStage("Проверка работы кнопки Подробная информация");
            ClickWebElement("//*[@id='js-toolbar']/div[2]/div/div[last()]//button", "Кнопка Подробная информация", "кнопка Подробная информация");
            if(!WebElementExist("//*[@class='curtain__sub-title _type_news']"))
                throw new Exception("Не работает кнопка Подробной информации");

        }
       
    }
    class ContentBetOfTheDay : BackOfficeProgram
    {
        public static CustomProgram FabricateContentBetOfTheDay()
        {
            return new ContentBetOfTheDay();
        }

        public override void Run()
        {
            base.Run();

            RemoveDuplicates(".//*[@class='curtain__list']/li[2]", "Ставка дня", "Англия — Германия", ".//*[@class='curtain _state_expanded']/div[2]//li//*[@class='curtain__news-title-inner']");

            LogStage("Добавление новой Ставки дня в Меню управление клиентом");
            ClickWebElement(".//*[@class='menu']//*[@href='#/explorer/content']", "Меню Управление клиентом", "меню Управление клиентом");
            ShowOnlyActive();
            ClickWebElement(".//*[@id='js-toolbar']/div[1]/div[1]", "Кнопка Добавить", "кнопки Добавить");
            ClickWebElement(".//*[@class='toolbar__drop-down _state_visible']/div[2]", "Строка Ставка дня", "строки Ставка дня");

            SetupVisualSettings();

            LogStage("Заполнение вкладки Изображение");
            ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[3]", "Вкладка Изображение", "вкладки Изображение");
            SendKeysToWebElement(".//*[@class='tabs__content-inner _state_visible']/div[1]//input", "/Content/BetsOfDay/soccer.jpg", "Поле Изображение", "поля Изображение");
            driver.FindElement(By.XPath(".//*[@class='tabs__content-inner _state_visible']/div[1]//input")).SendKeys(Keys.Backspace);
            Thread.Sleep(500);
            driver.FindElement(By.XPath(".//*[@class='tabs__content-inner _state_visible']/div[1]//input")).SendKeys("g");
            SendKeysToWebElement(".//*[@class='tabs__content-inner _state_visible']/label//input", "Тестовое фото", "Поле Подпись к фото", "поля Подпись к фото");
            SendKeysToWebElement(".//*[@class='tabs__content-inner _state_visible']/div[2]//input", "/Content/CompetitionLogo/AFL_Shield.png", "Поле Логотип(поверх)", "поля Логотип(поверх)");
            driver.FindElement(By.XPath(".//*[@class='tabs__content-inner _state_visible']/div[2]//input")).SendKeys(Keys.Backspace);
            Thread.Sleep(500);
            driver.FindElement(By.XPath(".//*[@class='tabs__content-inner _state_visible']/div[2]//input")).SendKeys("g");

            SendKeysToWebElement(".//*[@class='tabs__content-inner _state_visible']/div[4]/div[1]//input", "/Content/TeamLogo/barcelona.svg", "Поле Логотип 1", "поля Логотип 1");
            driver.FindElement(By.XPath(".//*[@class='tabs__content-inner _state_visible']/div[4]/div[1]//input")).SendKeys(Keys.Backspace);
            Thread.Sleep(500);
            driver.FindElement(By.XPath(".//*[@class='tabs__content-inner _state_visible']/div[4]/div[1]//input")).SendKeys("g");

            SendKeysToWebElement(".//*[@class='tabs__content-inner _state_visible']/div[4]/div[2]//input", "/Content/TeamLogo/psg.png", "Поле Логотип 2", "поля Логотип 2");
            driver.FindElement(By.XPath(".//*[@class='tabs__content-inner _state_visible']/div[4]/div[2]//input")).SendKeys(Keys.Backspace);
            Thread.Sleep(500);
            driver.FindElement(By.XPath(".//*[@class='tabs__content-inner _state_visible']/div[4]/div[2]//input")).SendKeys("g");

            LogStage("Выбор события");
            ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[1]", "Вкладка Общее", "вкладки Общее");
            ClickWebElement(".//*[@id='popup-event']//a", "Вкладка Событие", "вкладки Событие");
            waitTillElementisDisplayed(driver, ".//*[@id='popup-event']//a/../div[last()]//span", 5);
            Thread.Sleep(500);
            ClickWebElement(".//*[@id='popup-event']//a/../div[last()]//span", "Разворот события", "разворота события");
            waitTillElementisDisplayed(driver,".//*[@id='popup-event']//a/../div[last()]//*[@class='ui-dropdown__items']/div[3]/span",5);
            ClickWebElement(".//*[@id='popup-event']//a/../div[last()]//*[@class='ui-dropdown__items']/div[3]/span", "Разворот события Чемпионат мира", "разворота события Чемпионат мира");
            ClickWebElement(".//*[@id='popup-event']//a/../div[last()]//*[@class='ui-dropdown__items']/div[4]/span", "Конечное событие Англия-Германия", "конечного события Англия-Германия");

            LogStage("Выбор Котировки");
            ClickWebElement(".//*[@id='popup-Fon.Ora.Factor']//a", "Котировка 1", "котировки 1");
            waitTillElementisDisplayed(driver, ".//*[@class='ui-dropdown__items']/div[1]", 5);
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[1]", "Строка Основные ставки", "строки Основные ставки");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[2]", "Строка Итоги", "строки Итоги");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[3]", "Строка Поб1", "строки Поб1");
            ClickWebElement(".//*[@class='form__buttons']/div[1]/button", "Кнопка Добавить", "кнопки Добавить");
            waitTillElementisDisplayed(driver, "//*[@class='form__row _gaps-bottom_150']/div[5]/div/div[1]//a", 5);
            ClickWebElement(".//*[@class='form__row _gaps-bottom_150']/div[5]/div/div[1]//a", "Котировка 2", "котировки 2");
            ClickWebElement(".//*[@class='form__row _gaps-bottom_150']/div[5]/div/div[1]//a/../div[2]//*[@class='ui-dropdown__item'][1]", "Строка Ничья", "строки Ничья");
            ClickWebElement(".//*[@class='form__row _gaps-bottom_150']/div[6]/div/div[1]//a", "Котировка 3", "котировки 3");
            ClickWebElement(".//*[@class='form__row _gaps-bottom_150']/div[6]/div/div[1]//a/../div[2]//*[@class='ui-dropdown__item'][1]", "Строка Поб2", "строки Поб2");
            ClickWebElement(".//*[@class='ui__checkbox-item']//input", "Чекбокс Опубликовано", "чекбокса Опубликовано");
            ClickWebElement(".//*[@class='form__buttons']/div[1]/button", "Кнопка Добавить", "кнопки Добавить");
            Thread.Sleep(2000);
            SwitchToPreView();
            LogStage("Проверка что все элементы ставки дня отображаются на сайте");
            if (!WebElementExist(".//*[@href='#!/bets/football/12532/9639759']"))
                throw new Exception("Не отображается баннер ставки дня");
            if (!WebElementExist(".//*[@class='home-slider__photo-caption']"))
                throw new Exception("Не отображается подпись к фото");
            if (!WebElementExist(".//*[@class='home-slider__extra-logo']"))
                throw new Exception("Не отображается экстра лого");
            if (driver.FindElements(By.XPath(".//*[@class='home-slider__logo-wrap']/div")).Count!=2)
                throw new Exception("Не отображаются лого команд");
            driver.SwitchTo().Window(driver.WindowHandles[0]);
            ClickWebElement(".//*[@class='ui__checkbox-item']//input", "Чекбокс Опубликовано", "чекбокса Опубликовано");
            ClickWebElement(".//*[@class='form__buttons']/div[1]/button", "Кнопка Сохранить", "кнопки Сохранить");
        }

    }

    class ContentBanner : BackOfficeProgram
    {
        public static CustomProgram FabricateContentBanner()
        {
            return new ContentBanner();
        }

        public override void Run()
        {
            base.Run();
            RemoveDuplicates(".//*[@class='curtain__list']/li[3]", "Баннер", "Тестовый Заголовок", ".//*[@class='curtain _state_expanded']/div[2]//li//*[@class='curtain__news-caption-inner']");

            LogStage("Добавление нового Баннера в Меню управление клиентом");
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            ClickWebElement(".//*[@class='menu']//*[@href='#/explorer/content']", "Меню Управление клиентом", "меню Управление клиентом");
            ClickWebElement(".//*[@id='js-toolbar']/div[1]/div[1]", "Кнопка Добавить", "кнопки Добавить");
            ClickWebElement(".//*[@class='toolbar__drop-down _state_visible']/div[3]", "Строка Баннер", "строки Баннер");
            SetupVisualSettings();

            LogStage("Выбор Содержимого");
            ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[3]", "Вкладка Содержимое", "вкладки Содержимое");

            SendKeysToWebElement(".//*[@class='tabs__content']/div/div[3]/div/div/label[1]//input", "Тестовый Заголовок", "Поле Заголовок", "поля Заголовок");
            SendKeysToWebElement(".//*[@class='tabs__content']/div/div[3]/div/div/div[1]//input", "/Content/Banners/RightBanners/First-deposit_red.jpg", "Поле Мульти-язычное изображение", "поля Мульти-язычное изображение");
            driver.FindElement(By.XPath(".//*[@class='tabs__content']/div/div[3]/div/div/div[1]//input")).SendKeys(Keys.Backspace);
            Thread.Sleep(500);
            driver.FindElement(By.XPath(".//*[@class='tabs__content']/div/div[3]/div/div/div[1]//input")).SendKeys("g");
            Thread.Sleep(500);
            ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[1]", "Вкладка Общее", "вкладки Общее");
            ClickWebElement(".//*[@class='ui__checkbox-item']//input", "Чекбокс Опубликовано", "чекбокса Опубликовано");
            ClickWebElement(".//*[@class='form__buttons']/div[1]/button", "Кнопка Добавить", "кнопки Добавить");

            LogStage("Переход в Макеты");
            ClickWebElement(".//*[@href='#/']", "Лого на главную страницу", "лого на главную страницу");
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@href='#/explorer/contentLayout']")));

            ClickWebElement(".//*[@class='curtain__list']/li[1]", "Строка Главная", "строки Главная");
            ClickWebElement(".//*[@class='curtain _state_expanded']/div[2]//li[2]", "Строка Сайдбар с купонами", "строки Сайдбар с купонами");
            ClickWebElement(".//*[@class='curtain _state_expanded']/div[3]//li[7]", "Строка Зона-7", "строки Зона-7");
            waitTillElementisDisplayed(driver, ".//*[@id='js-toolbar']/div[1]/div[1]/button", 2);

            LogStage("Удаление слайдеров, если есть");

            if (WebElementExist(".//*[@class='curtain__col _pos_4']//ul/li"))
            {
                ClickWebElement(".//*[@class='curtain__col _pos_4']//ul//li[1]", "1ая строка баннера", "1ой строки баннера");
                IList<IWebElement> sliderData = driver.FindElements(By.XPath(".//*[@class='curtain__col _pos_4 _state_focused']//ul/li//*[@class='curtain__news-title-inner']"));
                for (int i = sliderData.Count - 1; i >= 0; i--)
                {
                    ClickWebElement(".//*[@id='js-toolbar']/div/div[7]//button", "Кнопка Удалить", "кнопки Удалить");
                    waitTillElementisDisplayed(driver, ".//*[@class='modal__foot']/div[2]/a", 2);
                    ClickWebElement(".//*[@class='modal__foot']/div[2]/a", "Кнопка Ок всплывающего окна", "кнопки Ок всплывающего окна");
                }
            }

            ClickWebElement(".//*[@id='js-toolbar']/div[1]/div[1]/button", "Кнопка Добавить элемент макета", "кнопки Добавить элемент макета");
            LogStage("Настройка Баннера");
            SetupVisualSettings();

            LogStage("Настройка отображения и общего");

            ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[3]", "Вкладка Настройки отображения", "вкладки Настройки отображения");
            ClickWebElement(".//*[@class='role-form__inner']/div[last()]//input", "Галка Показывать на всех страницах", "галки Показывать на всех страницах");
            ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[1]", "Вкладка Общее", "вкладки Общее");
            ClickWebElement(".//*[@class='ui__checkbox-item']//input", "Чекбокс Опубликовано", "чекбокса Опубликовано");
            ClickWebElement("//*[@id='popup-Content.Service.LayoutType']", "Меню Тип макета", "меню Тип макета");
            ClickWebElement("//*[@id='popup-Content.Service.LayoutType']//*[@class='ui-dropdown__items']//div[1]", "Строка Баннер", "строки Баннер");
            ClickWebElement(".//*[@id='popup-Content.Banner']", "Меню Объект", "меню Объект");
            IList < IWebElement > elements = driver.FindElements(By.XPath("//*[@id='popup-Content.Banner']//*[@class='ui-dropdown__items']/div/span"));
            foreach (var value in elements)
            {
                if (value.Text.Contains("Тестовый Заголовок"))
                {
                    value.Click();
                    break;
                }
            }
            ClickWebElement(".//*[@class='form__buttons']/div[1]/button", "Кнопка Добавить", "кнопки Добавить");

            LogStage("Проверка что баннер виден на сайте");
            driver.Navigate().GoToUrl("http://fonred5051.dvt24.com/#!/");
            // Смена языка при необходимости
            IWebElement langSetElement = FindWebElement(".//*[@class='header__lang-set']");
            if (langSetElement != null)
            {
                LogStage("Смена языка на русский");
                ClickWebElement(".//*[@class='header__lang-set']", "Кнопка выбора языка", "кнопки выбора языка");
                ClickWebElement(".//*[@class='header__lang-item']//*[text()='Русский']", "Кнопка выбора русского языка", "кнопки выбора русского языка");
            }

            ExecuteJavaScript("window.location.reload()", "Страницы не открылась");
            if (!WebElementExist(".//*[@src='//origin-test.bkfon-resource.ru/Content/Banners/RightBanners/First-deposit_red.jpg']"))
                throw new Exception("Не отображается баннер на главной");
            ClickWebElement(".//*[@href='/#!/bets']", "Вкладка Линия", "вкладки Линия");
            if (!WebElementExist(".//*[@src='//origin-test.bkfon-resource.ru/Content/Banners/RightBanners/First-deposit_red.jpg']"))
                throw new Exception("Не отображается баннер в линии");
            ClickWebElement(".//*[@href='/#!/live']", "Вкладка Лайве", "вкладки Лайве");
            if (!WebElementExist(".//*[@src='//origin-test.bkfon-resource.ru/Content/Banners/RightBanners/First-deposit_red.jpg']"))
                throw new Exception("Не отображается баннер в лайве");
        }
    }
    class ContentBannerLifeTime : BackOfficeProgram
    {
        public static CustomProgram FabricateContentBannerLifeTime()
        {
            return new ContentBannerLifeTime();
        }

        public override void Run()
        {
            base.Run();
            LogStage("Установка времени отображения баннера");
            ClickWebElement(".//*[@class='menu']//*[@href='#/explorer/content']", "Меню Управление клиентом", "меню Управление клиентом");
            ClickWebElement(".//*[@class='curtain__list']/li[3]", "Строка Баннер", "строки Баннер");
            ClickWebElement(".//*[@class='curtain _state_expanded']/div[2]//*[@class='curtain__list']/li[1]", "Строка 1ого баннера в списке", "строки 1ого баннера в списке");
            if (!driver.FindElement(By.XPath(".//*[@class='ui__checkbox-item']//input")).GetAttribute("class")
                .Contains("state_checked"))
            {
                ClickWebElement(".//*[@class='ui__checkbox-item']//input", "Чекбокс Опубликовано", "чекбокса Опубликовано");
                ClickWebElement(".//*[@class='form__buttons']/div[1]/button", "Кнопка Сохранить", "кнопки Сохранить");
            }
            Thread.Sleep(1000);
            ClickWebElement(".//*[@class='form__fields']/label[2]//time/i/a[2]", "Кнопка сброса даты", "кнопки сброса даты");
            ClickWebElement(".//*[@class='form__fields']/label[2]//time/i[3]/a[1]", "Иконка Установить текущую дату", "иконки тановить текущую дату");
            driver.FindElement(By.XPath(".//*[@class='form__fields']/label[2]//time/span[4]/span[2]")).Click();
            driver.FindElement(By.XPath(".//*[@class='form__fields']/label[2]//time/i[3]/i/a[1]")).Click();
            ClickWebElement(".//*[@class='form__buttons']/div[1]/button", "Кнопка Сохранить", "кнопки Сохранить");

            Thread.Sleep(65000);
            ClickWebElement(".//*[@id='js-toolbar']/div/div[5]//button", "Кнопка Обновить", "кнопки обновить");
            var indicator = GetWebElement(".//*[@class='curtain _state_expanded']/div[2]//*[@class='curtain__list']/li[1]/div/span[1]","Нет кружка индикатора");
            var indicatorClass = indicator.GetAttribute("style");
            if (indicatorClass.Contains("green"))
                throw new Exception("Не снимается банер после окончания срока");
           
        }
    }
    class ContentWinnersClub : BackOfficeProgram
    {
        public static CustomProgram FabricateContentWinnersClub()
        {
            return new ContentWinnersClub();
        }

        public override void Run()
        {
            base.Run();
            RemoveDuplicates(".//*[@class='curtain__list']/li[5]", "Клуб победителей", "ВОУ ВОУ ВОУ", ".//*[@class='curtain _state_expanded']/div[2]//li//*[@class='curtain__news-title-inner']");

            LogStage("Добавление новой Ставки дня в Меню управление клиентом");
            ClickWebElement(".//*[@class='menu']//*[@href='#/explorer/content']", "Меню Управление клиентом", "меню Управление клиентом");
            ShowOnlyActive();
            ClickWebElement(".//*[@id='js-toolbar']/div[1]/div[1]", "Кнопка Добавить", "кнопки Добавить");
            ClickWebElement(".//*[@class='toolbar__drop-down _state_visible']/div[6]", "Строка Выигрыш недели", "строки Выигрыш недели");

            SetupVisualSettings();

            LogStage("Заполнение вкладки Содержимое");
            ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[3]", "Вкладка Изображение", "вкладки Изображение");
            SendKeysToWebElement(".//*[@class='form__row']/label[2]//input", "Большие Вяземы", "Поле Регион", "поля Регион");
            SendKeysToWebElement(".//*[@class='form__row']/label[3]//textarea", "Внимание Внимание", "Поле Анонс", "поля Анонс");
            SendKeysToWebElement(".//*[@class='form__row']/label[4]//textarea", "У нас есть победитель", "Поле Текст", "поля Текст");
            SendKeysToWebElement(".//*[@class='form__row']/div/div[2]//input", "/Content/RegistrationDocumentInstruction/date-chel-1.jpg", "Поле Маленькое изображ", "поля Маленькое изображ");
            driver.FindElement(By.XPath(".//*[@class='form__row']/div/div[2]//input")).SendKeys(Keys.Backspace);
            Thread.Sleep(500);
            driver.FindElement(By.XPath(".//*[@class='form__row']/div/div[2]//input")).SendKeys("g");
            SendKeysToWebElement(".//*[@class='form__row']/div/div[3][@class='form__row']//input", "/Content/RegistrationDocumentInstruction/p_621746.jpg", "Поле Большое изображ", "поля Большое изображ");
            driver.FindElement(By.XPath(".//*[@class='form__row']/div/div[3][@class='form__row']//input")).SendKeys(Keys.Backspace);
            Thread.Sleep(500);
            driver.FindElement(By.XPath(".//*[@class='form__row']/div/div[3][@class='form__row']//input")).SendKeys("g");

            LogStage("Заполнение вкладки Общее");
            ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[1]", "Вкладка Общее", "вкладки Общее");
            SendKeysToWebElement(".//*[@class='role-form__inner']/label[1]//input", "ВОУ ВОУ ВОУ", "Поле Заголовок", "поля Заголовок");
            SendKeysToWebElement(".//*[@class='role-form__inner']/label[2]//input", "100500 тысяч японских йен", "Поле Сумма выигрыша", "поля Сумма выигрыша");
            ClickWebElement(".//*[@class='ui__checkbox-item']//input", "Чекбокс Опубликовано", "чекбокса Опубликовано");
            ClickWebElement(".//*[@class='form__buttons']/div[1]/button", "Кнопка Добавить", "кнопки Добавить");

           // SwitchToPreView();

            LogStage("Проверка что все элементы ставки дня отображаются на сайте");
            driver.Navigate().GoToUrl("http://fonred5051.dvt24.com/#!/");
            // Смена языка при необходимости
            IWebElement langSetElement = FindWebElement(".//*[@class='header__lang-set']");
            if (langSetElement != null)
            {
                LogStage("Смена языка на русский");
                ClickWebElement(".//*[@class='header__lang-set']", "Кнопка выбора языка", "кнопки выбора языка");
                ClickWebElement(".//*[@class='header__lang-item']//*[text()='Русский']", "Кнопка выбора русского языка", "кнопки выбора русского языка");
            }

            waitTillElementisDisplayed(driver, ".//*[text()='100500 тысяч японских йен']", 5);
            ClickWebElement(".//*[@class='top-win__head']/a", "Меню Клуб Победителей", "меню Клуб Победителей");
            if (!WebElementExist(".//*[@class='content-list__image']"))
                throw new Exception("Не отображается маленькое изображене");
            waitTillElementisDisplayed(driver, ".//*[@class='content-list']/article[1]/h2/a", 2);
            ClickWebElement(".//*[@class='content-list']/article[1]/h2/a", "Ссылка на разворот 1ого события", "ссылки на разворот 1ого события");
            if (!WebElementExist(".//*[@class='content-list__big-image-inner']"))
                throw new Exception("Не отображается большое изображение");
        }
    }
    class СontentCompetitonLogos : BackOfficeProgram
    {
        public static CustomProgram FabricateСontentCompetitonLogos()
        {
            return new СontentCompetitonLogos();
        }

        public override void Run()
        {
            base.Run();
           
            LogStage("Переход в Логотипы соревнований");
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].click()", driver.FindElement(By.XPath(".//*[@href='#/explorer/contentCompetitionLogo']")));

            LogStage("Создание нового логотипа");
            ClickWebElement(".//*[@class='curtain__list']/li[1]", "Строка Футбол", "строки Футбол");
            ClickWebElement(".//*[@id='js-toolbar']/div[1]/div[1]", "Кнопка Создать логотип", "кнопки Создать логотип");
            ClickWebElement(".//*[@class='role-form__inner']/div[1]//a", "Дропдаун Выбор соревнований", "дропдауна Выбор соревнований");
            ClickWebElement(".//*[@class='ui-dropdown__items']//div[1]", "Строка нужного соревнования", "строки нужного соревнования");
            var competionID = GetWebElement("//*[@class='role-form__inner']/label[1]//input", "Нет строки с ID");
            string competitonValue = competionID.GetAttribute("value");
            ClickWebElement(".//*[@class='ui__checkbox-item']//input", "Чекбокс Опубликовано", "чекбокса Опубликовано");

            SendKeysToWebElement(".//*[@class='form__row'][1]/label//input", "/Content/CompetitionLogo/fide-rating-logo.png", "Поле Маленький логотип", "поля Маленький логотип");
            driver.FindElement(By.XPath(".//*[@class='form__row'][1]/label//input")).SendKeys(Keys.Backspace);
            Thread.Sleep(500);
            driver.FindElement(By.XPath(".//*[@class='form__row'][1]/label//input")).SendKeys("g");
            SendKeysToWebElement(".//*[@class='form__row'][2]/label//input", "/Content/CompetitionLogo/NHL_logo_lr.png", "Поле Монохромный логотип", "поля Монохромный логотип");
            driver.FindElement(By.XPath(".//*[@class='form__row'][2]/label//input")).SendKeys(Keys.Backspace);
            Thread.Sleep(500);
            driver.FindElement(By.XPath(".//*[@class='form__row'][2]/label//input")).SendKeys("g");
            SendKeysToWebElement(".//*[@class='form__row'][3]/label//input", "/Content/Logo/Competition/australiaOpen.svg", "Поле Большой логотип", "поля Большой логотип");
            driver.FindElement(By.XPath(".//*[@class='form__row'][3]/label//input")).SendKeys(Keys.Backspace);
            Thread.Sleep(500);
            driver.FindElement(By.XPath(".//*[@class='form__row'][3]/label//input")).SendKeys("g");
            SetupVisualSettings();
            ClickWebElement(".//*[@class='form__buttons']/div[1]/button", "Кнопка Добавить", "кнопки Добавить");

            LogStage("Проверка что все элементы ставки дня отображаются на сайте");
            
            js.ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            driver.Navigate().GoToUrl("http://fonred5051.dvt24.com/#!/");
            // Смена языка при необходимости
            IWebElement langSetElement = FindWebElement(".//*[@class='header__lang-set']");
            if (langSetElement != null)
            {
                LogStage("Смена языка на русский");
                ClickWebElement(".//*[@class='header__lang-set']", "Кнопка выбора языка", "кнопки выбора языка");
                ClickWebElement(".//*[@class='header__lang-item']//*[text()='Русский']", "Кнопка выбора русского языка", "кнопки выбора русского языка");
            }
            
            LogStage("Установка настроек по умолчанию");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            
            LogStage("Перевод меню в отображение слева");
            ClickWebElement(".//*[@class='header-ui__checkbox _type_radio'][@value='2']", "Радиобатон отображения меню слева", "радиобатона отображения меню слева");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");
            

            LogStage("Переход в линию");
            ClickWebElement(".//*[@href='/#!/bets']", "Вкладка \"Линия\"", "вкладки \"Линия\"");
            ClickWebElement(".//*[@class='list-view-new__table-body']/tr[4]/td/div", "Разворот спорта футбол", "разворота спорта футбол");

           
            if (!WebElementExist(".//*[@href='#!/bets/football/"+ competitonValue+"']//*[@class='event-v-list__item-cell event-v-list__cell-image icon']"))
                throw new Exception("Не отображается маленький логотип");
            ClickWebElement(".//*[@href='#!/bets/football/" + competitonValue + "']", "Строка с маленьким логотип", "строки с маленьким логотипа");
            if (!WebElementExist(".//*[@class='table__flag-icon icon _type_flag']"))
                throw new Exception("Не отображается монохромный логотип");
            ClickWebElement(".//*[@class='table']//tr[last()]/td[2]//a", "Строка ивентвью", "строки ивентвью");
            if (!WebElementExist("//div[contains(@style,'australia')]"))
                throw new Exception("Не отображается лого в ивентвью");

            driver.SwitchTo().Window(driver.WindowHandles[0]);
            ClickWebElement(".//*[@id='js-toolbar']/div/div[5]//button", "Кнопка Удалить", "кнопки Удалить");
            waitTillElementisDisplayed(driver, ".//*[@class='modal__foot']/div[2]/a", 2);
            ClickWebElement(".//*[@class='modal__foot']/div[2]/a", "Кнопка Ок всплывающего окна", "кнопки Ок всплывающего окна");
        }
    }
}
