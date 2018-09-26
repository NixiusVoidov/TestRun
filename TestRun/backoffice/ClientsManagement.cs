using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace TestRun.backoffice
{
    class OperatorsSchedule : BackOfficeProgram
    {
        public static CustomProgram FabricateOperatorsSchedule()
        {
            return new OperatorsSchedule();
        }

        public override void Run()
        {
            base.Run();
            ClickWebElement(".//*[@href='#/explorer/operatorsSchedule']//i", "Меню расписание операторов", "меню расписание операторов");
            LogStage("Проверка Календарей на валидность");
            var sheduleCount = driver.FindElements(By.XPath(".//*[@class='curtain__list']/li")).Count;
                ClickWebElement(".//*[@id='js-toolbar']/div[1]/div[1]/button", "Кнопка Добавить расписание", "кнопки Добавить расписание");
            ClickWebElement(".//*[@class='form__fields']/label[1]//time/span/span[2]", "Поле минут во времени начала работы", "поля минут во времени начала работы");
            ClickWebElement(".//*[@class='form__fields']/label[1]//time//*[@class='ui-datetime__arrows']/a[1]", "Стрелка увеличения значения", "стрелки увеличения значения");
            if(driver.FindElement(By.XPath(".//*[@class='form__fields']/label[1]//time/span/span[2]")).Text!="30")
                throw new Exception("Шаг в расписании не равен 30 мин");
            ClickWebElement(".//*[@class='form__fields']/label[2]//time/span/span[1]", "Поле часов во времени окончания работы", "поля часов во времени окончания работы");
            ClickWebElement(".//*[@class='form__fields']/label[2]//time//*[@class='ui-datetime__arrows']/a[2]", "Стрелка уменьшения значения", "стрелки уменьшения значения");
            if(driver.FindElements(By.XPath(".//*[@class='ui__error']")).Count!=2)
                throw new Exception("Кол-во предупреждающих сообщений об ошибках не равно 2");

            ClickWebElement(".//*[@class='form__fields']/label[2]//time//*[@class='ui-datetime__action fa fa-times']", "Крестик сброса времени окончания", "крестика сброса времени окончания");
            if (driver.FindElements(By.XPath(".//*[@class='ui__error']")).Count != 1)
                throw new Exception("Кол-во предупреждающих сообщений об ошибках не равно 1");

            ClickWebElement(".//*[@class='form__fields']/label[2]//time/i/a[1]", "Кнопка календаря", "кнопки календаря");
            Thread.Sleep(1000);
            ClickWebElement(".//*[@class='ui-calendar__col _type_today']", "Дата сегодня в календаре", "даты сегодня в календаре");
            ClickWebElement(".//*[@class='form__fields']/label[2]//time/span/span[1]", "Поле часов во времени окончания работы", "поля часов во времени окончания работы");
            ClickWebElement(".//*[@class='form__fields']/label[2]//time//*[@class='ui-datetime__arrows']/a[2]", "Стрелка уменьшения значения", "стрелки уменьшения значения");

            SendKeysToWebElement(".//*[@class='ui__field-inner']//input", "1", "Кол-во операторов", "кол-ва операторов");
            ClickWebElement(".//*[@class='form__buttons']/div[1]", "Кнопка Добавить", "кнопки Добавить");
            if(driver.FindElements(By.XPath(".//*[@class='curtain__list']/li")).Count==sheduleCount)
                throw new Exception("Расписание не добавилось");
            DeleteButton(); 
        }

    }

    class FreebetImport : BackOfficeProgram
    {
        public static CustomProgram FabricateFreebetImport()
        {
            return new FreebetImport();
        }

        public override void Run()
        {
            base.Run();
            ClickWebElement(".//*[@href='#/freeBetImport']//i", "Меню расписание операторов", "меню расписание операторов");
            LogStage("Проверка правильной обработки csv");

            SendKeysToWebElement(".//*[@class='ui-uploader _fileinput_invisible']", "C:\\Users\\User\\Documents\\FreeBets\\FreeBets 5.csv", "Поле Прикрепления файла", "поля Прикрепления файла");
            if(!driver.FindElement(By.XPath(".//*[@class='ui__field-inner']/textarea")).Text.Equals("FreeBets 5.csv"))
                throw new Exception("В комментарий не подставилось название файла");
           
            if (!driver.FindElement(By.XPath(".//*[@class='list-view__table-body']/tr[1]/td[3]/span")).Text.Equals("30"))
                throw new Exception("Фрибет не равен 30 рублям");
            if (!driver.FindElement(By.XPath(".//*[@class='list-view__table-body']/tr[4]/td[5]/div")).Text.Equals("Некорректный формат данных."))
                throw new Exception("Не прошла проверка на корректные данные строка 4");
            if (!driver.FindElement(By.XPath(".//*[@class='list-view__table-body']/tr[7]/td[5]/div")).Text.Equals("Некорректный формат данных."))
                throw new Exception("Не прошла проверка на корректные данные строка 7");
            if(!driver.FindElement(By.XPath(".//*[@class='toolbar__item free-bet-info__btn-switcher--2iaHy']/a")).GetAttribute("class").Contains("state_disabled"))
                throw new Exception("Поле идентификатор акции необязательное");
            SendKeysToWebElement(".//*[@class='free-bet-info__form--1O1T4']/label//input", "testPromoId", "Поле идентификатор акции", "поля идентификатор акции");
            ClickWebElement(".//*[@class='toolbar__item free-bet-info__btn-switcher--2iaHy']/a", "Кнопка Запустить импорт", "кнопки Запустить импорт");
            ClickWebElement("//*[@class='head__logo-highlight']", "Логотип бэкофиса", "логотипа бэкофиса");
            ClickWebElement("//*[@href='#/actionlog']/span/i", "Журнал действий", "журнала действий");
            var rows = driver.FindElements(By.XPath("//*[@class='table__col-item _col_jsonattributes']/span/span"));
            rows[1].Click();
            LogStartAction("Проверка фрибета в логах ");
            if (!driver.FindElement(By.XPath(".//*[@class='ui-modalJSON__modal']/pre")).Text.Contains("\"currency\": \"1\""))
                throw new Exception("В логе валюта не равна 1");
            if (!driver.FindElement(By.XPath(".//*[@class='ui-modalJSON__modal']/pre")).Text.Contains("\"value\": \"3000\""))
                throw new Exception("В логе валюта не равна 1");
            if (!driver.FindElement(By.XPath(".//*[@class='ui-modalJSON__modal']/pre")).Text.Contains("\"expireTime\": \"2018-07-19 20:59:59\""))
                throw new Exception("В логе валюта не равна 1");
            if (!driver.FindElement(By.XPath(".//*[@class='ui-modalJSON__modal']/pre")).Text.Contains("\"promoId\": \"testPromoId\""))
                throw new Exception("В логе валюта не равна 1");
            LogActionSuccess();
        }

    }

}


