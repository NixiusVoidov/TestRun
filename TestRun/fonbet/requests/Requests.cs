

using OpenQA.Selenium;

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
            CreateNewRequest("Проблема с пополнением", "Проблема с пополнением", "QIWI Кошелек", "Qiwi");
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
            CreateNewRequest("Проблема с пополнением", "Проблема с пополнением", "Банковская карта", "Банковская карта");
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
            CreateNewRequest("Проблема с пополнением", "Проблема с пополнением", "Мобильный телефон", "Мобильный телефон");
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
            CreateNewRequest("Проблема с пополнением", "Проблема с пополнением", "Яндекс.Деньги", "Яндекс.Деньги");
            FillAndCreateFormBuilder(14);
            CheckRequestFilter("Яндекс");
        }
    }
    class DepositApple : FonbetWebProgram
    {
        public static CustomProgram FabricateDepositApple()
        {
            return new DepositApple();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("Проблема с пополнением", "Проблема с пополнением", "Apple Pay", "Apple Pay");
            FillAndCreateFormBuilder(14);
            CheckRequestFilter("Apple Pay");
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
            CreateNewRequest("Техническая поддержка", "Техническая поддержка", "Вопросы по работе сайта", "Вопросы по работе сайта");
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
            CreateNewRequest("Техническая поддержка", "Техническая поддержка", "Замечания и предложения", "Замечания и предложения");
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
            CreateNewRequest("Техническая поддержка", "Техническая поддержка", "Вопросы по работе мобильного приложения", "Вопросы по работе мобильного приложения");
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
            CreateNewRequest("Вопрос по расчету", "Вопросы по расчету", "Пари Лайв", "Пари Лайв");
            FillAndCreateFormBuilder(31);
            CheckRequestFilter("Лайв");
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
            ClickWebElement(".//*[@class='toolbar__item _left']", "Кнопка Новый запрос", "кнопки Новый запрос");
            WaitTillElementisDisplayed(driver, ".//*[@class='ui__field-inner']", 10);
            ClickWebElement(".//*[@class='ui__field-inner']", "Меню Тип запроса", "меню тип запроса");
            WaitTillElementisDisplayed(driver, ".//span[text()='Вопрос по расчету']", 10);
            driver.FindElement(By.XPath(".//span[text()='Вопрос по расчету']")).Click();
            ClickWebElement(".//*[@class='account-form__window _icon_img']//label[2]", "Меню Тема запроса", "меню Тема запроса");
            if (WebElementExist(".//span[text()='Ставки']"))
            {
                ClickWebElement("//*[@class='account-form__close-btn']","крестик закрытия темы","крестика закрытия темы");
                CreateNewRequest("Вопрос по расчету", "Вопрос по расчету", "Ставки", "Ставки");
                FillAndCreateFormBuilder(32);
                CheckRequestFilter("Пари");
            }
            else
            {
                ClickWebElement("//*[@class='account-form__close-btn']", "крестик закрытия темы", "крестика закрытия темы");
                CreateNewRequest("Вопрос по расчету", "Вопрос по расчету", "Пари", "Пари");
                FillAndCreateFormBuilder(32);
                CheckRequestFilter("Пари"); 
            }
            
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
            CreateNewRequest("Повышенный максимум", "Повышенный максимум", "Повысить максимум", "Повысить максимум");
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
            CreateNewRequest("Проблема с выплатой", "Проблема с выплатой", "QIWI Кошелек", "Qiwi");
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
            CreateNewRequest("Проблема с выплатой", "Проблема с выплатой", "Банковская карта", "Банковская карта");
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
            CreateNewRequest("Проблема с выплатой", "Проблема с выплатой", "Мобильный телефон", "Мобильный телефон");
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
            CreateNewRequest("Проблема с выплатой", "Проблема с выплатой", "Яндекс.Деньги", "Яндекс деньги");
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
            CreateNewRequest("Телефонный сервис", "Телефонный сервис", "Вопрос по расчету", "Вопрос по расчету");
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
            CreateNewRequest("Телефонный сервис", "Телефонный сервис", "Вопросы по телефонному сервису", "Вопросы по телефонному сервису");
            FillAndCreateFormBuilder(62);
            CheckRequestFilter("сервису");
        }
    }
    class SaleAdministration : FonbetWebProgram
    {
        public static CustomProgram FabricateSaleAdministration()
        {
            return new SaleAdministration();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            ClickOnAccount();
            OpenRequests();
            CreateNewRequest("Прочие вопросы", "Прочие вопросы", "Вопросы по акциям", "Вопросы по акциям");
            FillAndCreateFormBuilder(71);
            CheckRequestFilter("по акциям");
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
            CreateNewRequest("Прочие вопросы", "Прочие вопросы", "Вопросы к администрации", "Вопросы к администрации");
            FillAndCreateFormBuilder(72);
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
            CreateNewRequest("Прочие вопросы", "Прочие вопросы", "Замечания и предложения", "Замечания и предложения");
            FillAndCreateFormBuilder(73);
            CheckRequestFilter("Замечания");
        }
    }
}
