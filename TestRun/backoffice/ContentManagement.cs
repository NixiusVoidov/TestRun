using OpenQA.Selenium;
using System;
using System.Collections.Generic;

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
            LogStage("Добавление нового блога в Меню управление клиентом");
            ClickWebElement(".//*[@class='menu']//*[@href='#/explorer/content']", "Меню Управление клиентом", "меню Управление клиентом");
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
            waitTillElementisDisplayed(driver, ".//*[@class='ui__image-preview']", 5);

            ClickWebElement(".//*[@class='tabs__head tabs__slider']//a[1]", "Вкладка Общее", "вкладки Общее");
            ClickWebElement(".//*[@class='ui__checkbox-item']//input", "Чекбокс Опубликовано", "чекбокса Опубликовано");
            ClickWebElement(".//*[@class='form__buttons']/div[1]/button", "Кнопка Добавить", "кнопки Добавить");

            //сделать урыл и проверку на сайте

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
            if(driver.FindElement(By.XPath(".//*[@id='curtain']/div/div[2]//li[1]")).GetAttribute("class").Contains("state_selected"))
                throw new Exception("Клонирование не сработало");
            ClickWebElement(".//*[@id='curtain']/div/div[2]//li[1]", "1ый блог из списка", "1ого блога из списка");
            if(driver.FindElement(By.XPath(".//*[@id='curtain']/div/div[2]//li[1]//*[@class='curtain__news-title-inner']")).Text!=titleText)
                throw  new  Exception("При клонировании поменялись тайтлы");

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

            LogStage("Проверка фильтра Приложения");
            ContentApplicationsFilter();
            ClickWebElement(".//*[@class='curtain__list']/li[2]", "Строка Ставка дня", "строки Ставка дня");
            ClickWebElement(".//*[@class='curtain__list']/li[1]", "Строка Блог", "строки Блог");
            ContentCategoriessFilter();

            //доделать еще фильтр и проверить порядок чтоб Сашка сделал

         



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
