

namespace TestRun.fonbet.requests
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
    class DepositYandex : FonbetWebProgram
    {
        public static CustomProgram FabricateDepositYandex()
        {
            return new DepositYandex();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("1", "Проблема с пополнением", "4", "Яндекс.Деньги");
            FillAndCreateFormBuilder(14);
            CheckRequestFilter("Яндекс");
        }
    }
    class SupportSite : FonbetWebProgram
    {
        public static CustomProgram FabricateSupportSite()
        {
            return new SupportSite();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("2", "Техническая поддержка", "1", "Вопросы по работе сайте");
            FillAndCreateFormBuilder(21);
            CheckRequestFilter("Вопросы по работе");
        }
    }
    class SupportSuggestion : FonbetWebProgram
    {
        public static CustomProgram FabricateSupportSuggestion()
        {
            return new SupportSuggestion();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("2", "Техническая поддержка", "2", "Замечания и предложения");
            FillAndCreateFormBuilder(22);
            CheckRequestFilter("Замечания");
        }
    }
    class SupportMobile : FonbetWebProgram
    {
        public static CustomProgram FabricateSupportMobile()
        {
            return new SupportMobile();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("2", "Техническая поддержка", "3", "Вопросы по работе мобильного приложения");
            FillAndCreateFormBuilder(23);
            CheckRequestFilter("мобильного приложения");
        }
    }
    class CalculationsBetLife : FonbetWebProgram
    {
        public static CustomProgram FabricateCalculationsBetLife()
        {
            return new CalculationsBetLife();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("3", "Вопросы по расчету", "1", "Пари Лайф");
            FillAndCreateFormBuilder(31);
            CheckRequestFilter("Лайф");
        }
    }
    class CalculationsBet : FonbetWebProgram
    {
        public static CustomProgram FabricateCalculationsBet()
        {
            return new CalculationsBet();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("3", "Вопросы по расчету", "2", "Пари");
            FillAndCreateFormBuilder(32);
            CheckRequestFilter("Пари");
        }
    }
    class IncreasedMax : FonbetWebProgram
    {
        public static CustomProgram FabricateIncreasedMax()
        {
            return new IncreasedMax();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("4", "Повышенный максимум", "1", "Повысить максимум");
            FillAndCreateFormBuilder(41);
            CheckRequestFilter("Повысить");
        }
    }
    class PaymentQiwi : FonbetWebProgram
    {
        public static CustomProgram FabricatePaymentQiwi()
        {
            return new PaymentQiwi();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("5", "Проблема с выплатой", "1", "Qiwi");
            FillAndCreateFormBuilder(5);
            CheckRequestFilter("QIWI");
        }
    }
    class PaymentCard : FonbetWebProgram
    {
        public static CustomProgram FabricatePaymentCard()
        {
            return new PaymentCard();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("5", "Проблема с выплатой", "2", "Банковская карта");
            FillAndCreateFormBuilder(5);
            CheckRequestFilter("Банковская");
        }
    }
    class PaymentMobile : FonbetWebProgram
    {
        public static CustomProgram FabricatePaymentMobile()
        {
            return new PaymentMobile();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("5", "Проблема с выплатой", "3", "Мобильный телефон");
            FillAndCreateFormBuilder(5);
            CheckRequestFilter("Мобильный");
        }
    }
    class PaymentYandex : FonbetWebProgram
    {
        public static CustomProgram FabricatePaymentYandex()
        {
            return new PaymentYandex();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("5", "Проблема с выплатой", "4", "Яндекс деньги");
            FillAndCreateFormBuilder(5);
            CheckRequestFilter("Яндекс");
        }
    }
    class PhoneCalculation : FonbetWebProgram
    {
        public static CustomProgram FabricatePhoneCalculation()
        {
            return new PhoneCalculation();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("6", "Телефонный сервис", "1", "Вопрос по расчету");
            FillAndCreateFormBuilder(61);
            CheckRequestFilter("расчету");
        }
    }
    class PhoneService : FonbetWebProgram
    {
        public static CustomProgram FabricatePhoneService()
        {
            return new PhoneService();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("6", "Телефонный сервис", "2", "Вопросы по телефонному сервису");
            FillAndCreateFormBuilder(62);
            CheckRequestFilter("сервису");
        }
    }
    class OtherAdministration : FonbetWebProgram
    {
        public static CustomProgram FabricateOtherAdministration()
        {
            return new OtherAdministration();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("7", "Прочие вопросы", "1", "Вопросы к администрации");
            FillAndCreateFormBuilder(71);
            CheckRequestFilter("администрации");
        }
    }
    class OtherSuggestion : FonbetWebProgram
    {
        public static CustomProgram FabricateOtherSuggestion()
        {
            return new OtherSuggestion();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("7", "Прочие вопросы", "2", "Замечания и предложения");
            FillAndCreateFormBuilder(72);
            CheckRequestFilter("Замечания");
        }
    }
}
