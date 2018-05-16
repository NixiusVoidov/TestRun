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
            FillAndCreateFormBuilder(11);
            CheckRequestFilter("Qiwi");
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
            FillAndCreateFormBuilder(12);
            CheckRequestFilter("Банковская");
        }
    }
    class DepositMobile : FonbetWebProgram
    {
        public static CustomProgram FabricateDepositMobile()
        {
            return new DepositMobile();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("1", "Проблема с пополнением", "3", "Мобильный телефон");
            FillAndCreateFormBuilder(13);
            CheckRequestFilter("Мобильный");
        }
    }
}
