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

            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("1","Проблема с пополнением","1","Qiwi");

            LogStage("Заполнение и создание нового запроса");
            SendKeysToWebElement(".//*[@name='THEDESCRIPTIONOFTHEPROBLEM']","Test","Поле описания проблемы", "поля описания проблемы");
            SendKeysToWebElement(".//*[@name='AMOUNTRUB']", "2409", "Поле Суммы(руб)", "поля Суммы(руб)");
            SendKeysToWebElement(".//*[@class='ui__label']/input", "C:\\Users\\User\\Downloads\\саша.jpg", "Поле Прикрепления файла", "поля Прикрепления файла");
            ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");

            CheckRequestFilter("QIWI");
        }
    }
    class DepositCard: FonbetWebProgram
    {
        public static CustomProgram FabricateDepositCard()
        {
            return new DepositCard();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();

            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("1", "Проблема с пополнением", "2", "Банковская карта");

            LogStage("Заполнение и создание нового запроса");
            SendKeysToWebElement(".//*[@name='THEDESCRIPTIONOFTHEPROBLEM']", "Test", "Поле описания проблемы", "поля описания проблемы");
            SendKeysToWebElement(".//*[@name='AMOUNTRUB']", "2409", "Поле Суммы(руб)", "поля Суммы(руб)");
            SendKeysToWebElement(".//*[@class='ui__label']/input", "C:\\Users\\User\\Downloads\\саша.jpg", "Поле Прикрепления файла", "поля Прикрепления файла");
            ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");

            CheckRequestFilter("Банковская");
        }
    }
}
