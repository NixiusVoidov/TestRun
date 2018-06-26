using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

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
            // IList<IWebElement> testData = driver.FindElements(By.XPath(".//*[@class='curtain__list']//*[text()='Тестовый заголовок']")); 
            IEnumerable<IWebElement> testData = driver.FindElements(By.XPath(".//*[@class='curtain _state_expanded']/div[2]//li")).Take(10);
            var dataArray = testData.Where(n => n.Text.Contains("Тестовый заголовок")).ToArray();

            while (dataArray.Count() > 0)
            {
                dataArray[0].Click();
                ClickWebElement(".//*[@id='js-toolbar']/div/div[7]//button", "Кнопка Удалить", "кнопки Удалить");
                ClickWebElement(".//*[@class='modal__foot']/div[2]/a", "Кнопка Ок всплывающего окна", "кнопки Ок всплывающего окна");
            }

            LogStage("Добавление нового блога в Меню управление клиентом");
           // ClickWebElement(".//*[@class='menu']//*[@href='#/explorer/content']", "Меню Управление клиентом", "меню Управление клиентом");
            ClickWebElement(".//*[@id='js-toolbar']/div[1]/div[1]", "Кнопка Добавить", "кнопки Добавить");
            ClickWebElement(".//*[@class='toolbar__drop-down _state_visible']/div[1]", "Строка Блог", "строки Блог");
            LogStage("Установка области видимости");
            ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[2]", "Вкладка Область видимости", "вкладки Область видимости");
            ClickWebElement(".//*[@class='role-form__inner']/div[3]//*[@class='ui__list-node right-list__row'][1]//input", "Чекбокс Fonbet русский", "чекбокса Fonbet русский");
            ClickWebElement(".//*[@class='role-form__inner']/div[3]//*[@class='ui__list-node right-list__row'][2]//input", "Чекбокс Fonbet английский", "чекбокса Fonbet английский");
            ClickWebElement(".//*[@class='role-form__inner']/div[3]//*[@class='ui__list-node right-list__row'][3]//input", "Чекбокс ЦУПИС", "чекбокса ЦУПИС");
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

            LogStage("Проверка отображения картинки");
            ClickWebElement(".//*[@class='role-form__inner']/div[last()]/a", "Ссылка на созданный блог", "ссылки на созданый блог");

            LogStage("Проверка что новость открывается в новом окне");
            var popup = driver.WindowHandles[1];
            if (string.IsNullOrEmpty(popup))
                throw new Exception("Не открылась запись в новом окне");
            driver.SwitchTo().Window(driver.WindowHandles[0]);
            driver.SwitchTo().Window(driver.WindowHandles[1]);
            // Смена языка при необходимости
            IWebElement langSetElement = FindWebElement(".//*[@class='header__lang-set']");
            if (langSetElement != null)
            {
                LogStage("Смена языка на русский");
                ClickWebElement(".//*[@class='header__lang-set']", "Кнопка выбора языка", "кнопки выбора языка");
                ClickWebElement(".//*[@class='header__lang-item']//*[text()='Русский']", "Кнопка выбора русского языка", "кнопки выбора русского языка");
            }
            if(!WebElementExist(".//*[@class='content-list__big-image-inner']"))
                throw new Exception("Не отображается большая картинка");
            ClickWebElement(".//*[@class='content-page__categories']/li[2]", "Вкладка Спорт с Фонбет", "вкладки Спорт с Фонбет");
            if (!WebElementExist(" .//*[@class='content-list']/article[1]//*[@class='content-list__image']"))
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
            LogStage("Добавление новой Ставки дня в Меню управление клиентом");
            ClickWebElement(".//*[@class='menu']//*[@href='#/explorer/content']", "Меню Управление клиентом", "меню Управление клиентом");
            ClickWebElement(".//*[@id='js-toolbar']/div[1]/div[1]", "Кнопка Добавить", "кнопки Добавить");
            ClickWebElement(".//*[@class='toolbar__drop-down _state_visible']/div[2]", "Строка Ставка дня", "строки Ставка дня");


            LogStage("Выбор события");
            ClickWebElement(".//*[@id='popup-event']//a", "Вкладка Событие", "вкладки Событие");
            ClickWebElement(".//*[@id='popup-event']//a/../div[last()]//span", "Разворот события", "разворота события");
            ClickWebElement(".//*[@id='popup-event']//a/../div[last()]//*[@class='ui-dropdown__items']/div[2]/span", "Разворот события", "разворота события");
            ClickWebElement(".//*[@id='popup-event']//a/../div[last()]//*[@class='ui-dropdown__items']/div[3]/span", "Конечное событие", "конечного события");

            //ClickWebElement(".//*[@class='role-form__inner']/div[3]//*[@class='ui__list-node right-list__row'][2]//input", "Чекбокс Fonbet английский", "чекбокса Fonbet английский");
            //ClickWebElement(".//*[@class='role-form__inner']/div[3]//*[@class='ui__list-node right-list__row'][3]//input", "Чекбокс ЦУПИС", "чекбокса ЦУПИС");
            //ClickWebElement(".//*[@class='role-form__inner']/div[2]//*[@class='ui__list-node right-list__row'][3]//input", "Чекбокс Новости Фонбет", "чекбокса Новости Фонбет");
            //LogStage("Заполнение вкладки Содержимое");
            //ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[3]", "Вкладка Содержимое", "вкладки Содержимое");
            //SendKeysToWebElement(".//*[@class='form__row']/label[2]//input", "Тестовый заголовок", "Поле Заголовок", "поля Заголовок");
            //SendKeysToWebElement(".//*[@class='form__row']/label[3]//textarea", "Тестовый Анонс", "Поле Анонс", "поля Анонс");
            //SendKeysToWebElement(".//*[@class='form__row']/label[4]//textarea", "Тестовый Текст новости", "Поле Текст новости", "поля Текст новости");
            //SendKeysToWebElement(".//*[@class='form__row _gaps-top_20']/div[2]//input", "/Test/XYZ/ABC/52130392.jpg", "Поле Маленькое изображение", "поля Маленькое изображение");
            //waitTillElementisDisplayed(driver, ".//*[@class='ui__image-preview']", 5);

            //ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[1]", "Вкладка Общее", "вкладки Общее");
            //ClickWebElement(".//*[@class='ui__checkbox-item']//input", "Чекбокс Опубликовано", "чекбокса Опубликовано");
            //ClickWebElement(".//*[@class='form__buttons']/div[1]/button", "Кнопка Добавить", "кнопки Добавить");

            //сделать урыл и проверку на сайте

        }


    }
}
