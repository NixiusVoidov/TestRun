using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace TestRun.fonbet
{
    class DepositQiwi : FonbetWebProgram
    {
        public static CustomProgram FabricateDepositQiwi()
        {
            return new DepositQiwi();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();

            LogStage("Переход в Личный кабинет");
            ClickWebElement(".//*[@class='header__login-head']/div[1]/span", "ФИО в шапке", "ФИО в шапке");
            ClickWebElement(".//*[@id='popup']/li[1]", "Кнопка Личный кабинет", "кнопки Личный кабинет");

            LogStage("Переход в меню \"Запросы\" ");
            ClickWebElement(".//*[@href='#!/account/requests']", "Меню \"Запросы\"", "меню \"Запросы\"");
            if (driver.Title != "Запросы")
                throw new Exception("Страница не содержит title \"Запросы\"");

            LogStage("Создание нового запроса");
            ClickWebElement(".//*[@class='toolbar__item _left']", "Кнопка Новый запрос", "кнопки Новый запрос");
            ClickWebElement(".//*[@class='ui__field-inner']", "Меню Тип запроса", "меню тип запроса");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[1]", "Строка Проблема с пополнением", "строки Проблема с пополнением");
            ClickWebElement(".//*[@class='account-form__window _icon_img']//label[2]", "Меню Тема запроса", "меню Тема запроса");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[1]", "Строка Qiwi", "строки Qiwi");
            ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");
            if (!WebElementExist(".//*[@class='account-form__message _kind-error _style-box']"))
                throw new Exception("В форме нет обязательных полей");

            LogStage("Заполнение и создание нового запроса");
            SendKeysToWebElement(".//*[@name='THEDESCRIPTIONOFTHEPROBLEM']","Test","Поле описания проблемы", "поля описания проблемы");
            SendKeysToWebElement(".//*[@name='AMOUNTRUB']", "2409", "Поле Суммы(руб)", "поля Суммы(руб)");
            SendKeysToWebElement(".//*[@class='ui__label']/input", "C:\\Users\\User\\Downloads\\саша.jpg", "Поле Прикрепления файла", "поля Прикрепления файла");
            ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");
            if (!WebElementExist(".//*[@class='account-form__message _kind-top-notice _style-box']"))
                throw new Exception("Нет сообщения о создании заявки");
            IWebElement createResult= GetWebElement(".//*[@class='account-form__message _kind-top-notice _style-box']", "Нет сообщения о создании заявки");
            var createResultText = createResult.Text;
            var createResultTextConvert = Regex.Replace(createResultText, @"[^\d]+", ""); // Вычленение номера заявки из общего сообщения о создании заявки
            ClickWebElement(".//*[@class='toolbar__item account-form__button']/a//span", "Кнопка Закрыть", "кнопки Закрыть");

            LogStage("Проверка работы фильтра по статусу");
            ClickWebElement(".//*[@class='account-requests__form']//*[@class='ui__label']", "Кнопка разворота меню фильтра", "кнопки разворота меню фильтра");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[2]", "Строка Отвеченный", "строки Отвеченный");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[3]", "Строка В Облработке", "строки В Облработке");
            ClickWebElement(".//*[@class='ui__field-inner']//*[@class='toolbar__icon _caret-up']", "Стрелка фильтра по статусу", "стрелки фильтра по статусу");
            IList<IWebElement> gridNumber = driver.FindElements(By.XPath(".//*[@class='wrap']")); //все строки
            if (gridNumber.Count <2)
                throw new Exception("Не работает фильтр по статусу");

            LogStage("Проверка работы фильтра по номеру");
            ClickWebElement(".//*[@class='account-requests__form']//*[@class='ui__label']", "Кнопка разворота меню фильтра", "кнопки разворота меню фильтра");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[2]", "Строка Отвеченный", "строки Отвеченный");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[3]", "Строка В Облработке", "строки В Облработке");
            SendKeysToWebElement(".//*[@class='account-requests__form']//*[@class='ui__field-inner']//input", createResultTextConvert, "Поле Номер запроса", "поля Номер запроса");
            IList<IWebElement> myRequest = driver.FindElements(By.XPath(".//*[@class='wrap']/div/div[1]"));
            if (myRequest.Count != 1)
                throw new Exception("Есть два одинаковых номера запроса");
            IWebElement numberCell = GetWebElement(".//*[@class='wrap']/div/div[1]", "Нет поля номера заявки");
            var numberCellText = numberCell.Text;
            if (createResultTextConvert != numberCellText)
                throw new Exception("Не работает фильтр по номеру");
            ClickWebElement(".//*[@class='toolbar__icon icon _clear']", "Кнопка Очистить фильтр по номеру", "кнопки Очистить фильтр по номеру");
           
            LogStage("Закрытие готовой заявки");
            ClickWebElement(".//*[@class='requests-list__data']/div[1]", "Строка с последней созданной заявкой", "строки с последней созданной заявкой");
            ClickWebElement(".//*[@class='request-details']//*[@class='toolbar__item']", "Кнопка закрыть заявку", "кнопки закрыть заявку");
            IWebElement requestCell = GetWebElement(".//*[@class='requests-list__data']/div[1]/div", "Нет строк с заявками");
            var requestCellClass = requestCell.GetAttribute("class");
            if (requestCellClass.Contains("new"))
                throw new Exception("Не работает закрытие заявки");
            IWebElement requestStatus = GetWebElement(".//*[@class='requests-list__data']/div[1]//*[@class='column column-4']", "Нет колонки Статус");
            var requestStatusText = requestStatus.Text;
            if (requestStatusText != "Отвеченный")
                throw new Exception("Не поменялся статус заявки после ее закрытия");

            LogStage("Проверка переоткрытия заявки");
            ClickWebElement(".//*[@class='requests-list__data']/div[1]", "Строка с последней созданной заявкой", "строки с последней созданной заявкой");
            SendKeysToWebElement(".//*[@class='ui__field _message']", "Test", "Поле для ввода нового сообщения", "поля для ввода нового сообщения");
            ClickWebElement(".//*[@class='request-details__form-wrap']//button", "Кнопка отправки нового сообщения", "кнопки отправки нового сообщения");
            IWebElement newStatus = GetWebElement(".//*[@class='requests-list__data']/div[1]//*[@class='column column-4']", "Нет колонки Статус");
            var newStatussText = newStatus.Text;
            if (newStatussText != "Неотвеченный")
                throw new Exception("Не поменялся статус заявки после ее переоткрытия");
           
            LogStage("Проверка что фаил скачивается из заявки");
            ClickWebElement(".//*[@class='requests-list__data']/div[1]", "Строка с последней созданной заявкой", "строки с последней созданной заявкой");
            ClickWebElement(".//*[@class='request-details']//*[@class='toolbar__item']", "Кнопка закрыть заявку", "кнопки закрыть заявку");
            ClickWebElement(".//*[@class='request-details__request-file']/span", "Кнопка скачать прикрепленный фаил", "кнопки скачать прикрепленный фаил");
            if (CheckFileDownloaded("rccimg_0000000011_d180d396.jpg") == false)
                throw new Exception("Фаил из заявки не скачался");
        }
    }
}
