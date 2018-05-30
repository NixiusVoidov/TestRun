using System;
using OpenQA.Selenium;

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

            if (!WebElementExist(".//*[@class='how2play']"))
               throw new Exception("Нет виджета HowToPlay");

            LogStage("Проверка работы всех разделов меню \"Я хочу заключать пари в интернете\"");
            ClickWebElement(".//*[@class='how2play__help-steps']/div[1]","Кнопка как заключать пари в интернете", "кнопки как заключать пари в интернете");
            ClickWebElement(".//*[@class='how2play__stepout _visible']/div[1]","Раздел удаленная идентификация","раздела удаленная идентификация");
            if(!WebElementExist(".//*[@class='how2play__stepout _visible']/div[1]//*[@href='#!/account/registration/RemoteVerification2']"))
                throw new Exception("Нет ссылки на удаленную верификацию");

            ClickWebElement(".//*[@class='how2play__stepout _visible']/div[2]", "Раздел карты Фонбет","раздела карты Фонбет");
            if (!WebElementExist(".//*[@class='how2play__stepout _visible']/div[2]//*[@href='#!/account/registration/Bk']"))
                throw new Exception("Нет ссылки на регистрацию БК");

            ClickWebElement(".//*[@class='how2play__stepout _visible']/div[3]", "Раздел кошелек Киви", "раздела кошелек Киви");
            if (!WebElementExist(".//*[@class='how2play__stepout _visible']/div[3]//*[@href='#!/account/registration/Qiwi']"))
                throw new Exception("Нет ссылки на регистрацию Киви");

            ClickWebElement(".//*[@class='how2play__stepout _visible']/div[4]", "Раздел без карты и без киви", "раздела без карты и без киви");
            if (!WebElementExist(".//*[@class='how2play__stepout _visible']/div[4]//*[@href='#!/account/how2Register']"))
                throw new Exception("Нет ссылки на  ИНУЮ регистрацию");

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
            base.Run();

            if (!WebElementExist(".//*[@class='home-slider__switch-wrap']/div[1]"))
                throw new Exception("На главной странице нет слайдера");

            LogStage("Проверка переключения слайдера по кнопкам");
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

            LogStage("Проверка отображения и корретной работы блока новостей");
            ClickWebElement(".//*[@class='home-news__items']/article[1]/h1", "1ый топик из ленты новостей", "1ого топика из ленты новостей");
            if (!WebElementExist(".//*[@id='popupModal']"))
                throw new Exception("Не открылся попап с новостью");
            ClickWebElement(".//*[@class='news-modal__close']", "Кнопка закрытия popup", "кнопки закрытия popup");

            ClickWebElement(".//*[@class='home-news__source-box']/span[2]", "Вкладка \"Прогнозы\"", "вкладки \"Прогнозы\"");
            var prediction = GetWebElement(".//*[@class='home-news__source-box']/span[2]", "Нет вкладки \"Прогнозы\"");
            var predictionClass = prediction.GetAttribute("class");
            if (!predictionClass.Contains("_selected"))
                throw new Exception("Не переключается на вкладку Прогнозы в шапке новостей");
            ClickWebElement(".//*[@class='home-news__items']/article[2]//a", "2ой топик из ленты новостей", "2ого топика из ленты новостей");
            if (!WebElementExist(".//*[@id='popupModal']"))
                throw new Exception("Не открылся попап с новостью");
            ClickWebElement(".//*[@class='news-modal__close']", "Кнопка закрытия popup", "кнопки закрытия popup");

            ClickWebElement(".//*[@class='home-news__source-box']/span[3]", "Вкладка \"Спорт с Фонтбет\"", "вкладки \"Спорт с Фонтбет\"");
            var sport = GetWebElement(".//*[@class='home-news__source-box']/span[3]", "Нет вкладки \"Прогнозы\"");
            var sportClass = sport.GetAttribute("class");
            if (!sportClass.Contains("_selected"))
                throw new Exception("Не переключается на вкладку Прогнозы в шапке новостей");
            ClickWebElement(".//*[@class='home-news__items']/article[3]//a", "3ий топик из ленты новостей", "3ого топика из ленты новостей");
            if (!WebElementExist(".//*[@id='popupModal']"))
                throw new Exception("Не открылся попап с новостью");
            ClickWebElement(".//*[@class='news-modal__close']", "Кнопка закрытия popup", "кнопки закрытия popup");

            ClickWebElement(".//*[@class='top-win__items']/article[1]//a", "Топик в модуле \"Клуб победителей\"", "топика в модуле \"Клуб победителей\"");
            if (!WebElementExist(".//*[@id='popupModal']"))
                throw new Exception("Не открылся попап с новостью");
            ClickWebElement(".//*[@class='news-modal__close']", "Кнопка закрытия popup", "кнопки закрытия popup");

            ClickWebElement(".//*[@href='#!/news/fnl']", "Ссылка на ВСЕ новости", "ссылки на ВСЕ новости");
            var titleNews = GetWebElement(".//*[@class='content-page__title']", "Нет тайтала на странице \"Новости\"");
            var titleNewsText = titleNews.Text;
            if (!titleNewsText.Contains("Новости"))
                throw new Exception("Тайтл не соответсвует странице");
        }
    }

    class PwdRecovery : FonbetWebProgram
    {
        public static CustomProgram FabricatePwdRecovery()
        {
            return new PwdRecovery();
        }

        protected override bool NeedLogin()
        {
            return false;
        }

        public override void Run()
        {
            base.Run();

            RejectPwdChecker("12", "000000001");
            RejectPwdChecker("10", "000000002");
            RejectPwdChecker("4", "000000003");
            RejectPwdChecker("1", "000000004");

            LogStage("Переход на страницу восстановление пароля");
            ClickWebElement(".//*[@href='/#!/account/restore-password']", "Кнопка Забыли пароль", "кнопки забыли пароль");

            LogStage("Проверка sendCode по тестовому сценарию на error 10");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[2]//input", "000000005", "Поле номер телефона", "поля номер телефона");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[3]//input", "11", "Поле капча", "поля капчи");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            SendKeysToWebElement(".//*[@class='ui__field']", "123123", "Поле Код подтверждения", "поля Код подтверждения");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            var errorMessage = GetWebElement(".//*[@class='account-error__text']", "Нет текста ошибки");
            if (!errorMessage.Text.Contains("Неверный код подтверждения"))
                throw new Exception("Неверный текст ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");

            LogStage("Проверка sendPassword на reject");
            driver.FindElement(By.XPath(".//*[@class='ui__field']")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field']", "123456", "Поле Код подтверждения", "поля Код подтверждения");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[1]//input", "1234567Q", "Поле Новый пароль", "поля Новый пароль");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[2]//input", "1234567Q", "Поле Повторите новый пароль", "поля Повторите новый пароль");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            var errorText = GetWebElement(".//*[@class='account-error__text']", "Нет текста ошибки");
            if (!errorText.Text.Contains("В процессе регистрации произошла неожиданная ошибка"))
                throw new Exception("Неверный текст ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");

            LogStage("Проверка sendPassword на complete");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[2]//input", "000000005", "Поле номер телефона", "поля номер телефона");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[3]//input", "11", "Поле капча", "поля капчи");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            SendKeysToWebElement(".//*[@class='ui__field']", "123456", "Поле Код подтверждения", "поля Код подтверждения");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[1]//input", "!23qweQWE", "Поле Новый пароль", "поля Новый пароль");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[2]//input", "!23qweQWE", "Поле Повторите новый пароль", "поля Повторите новый пароль");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки отправить");
            var message = GetWebElement(".//*[@class='account-error__title']", "Нет title ошибки");
            if(!WebElementExist(".//*[@class='account__content']//span"))
                throw new Exception("Нет кнопки \"Войти на сайт\"");
            if (!message.Text.Contains("Пароль успешно изменён"))
                throw new Exception("Тестовое восстановление пароля не удалось");

        }
    }
    class EmailConfirm : FonbetWebProgram
    {
        public static CustomProgram FabricateEmailConfirm()
        {
            return new EmailConfirm();
        }

        public override void Run()
        {
            base.Run();

            ClickOnAccount();
            ClickWebElement(".//*[@href='#!/account/profile/change-email']", "Вкладка Смена email", "вкладки смена email");

            CreateProcesslChecker("14", "1@dev.dev");
            CreateProcesslChecker("13", "2@dev.dev");
            CreateProcesslChecker("12", "3@dev.dev");

            LogStage("Проверка sendCode по тестовому сценарию");
            driver.FindElement(By.XPath(".//*[@class='ui__field-inner']/input")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input", "4@dev.dev", "Поле email", "поля email");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
            SendEmailCodeChecker("10", "1235");
            SendEmailCodeChecker("1", "9999");
            driver.FindElement(By.XPath(".//*[@class='ui__field-inner']/input")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input", "4@dev.dev", "Поле email", "поля email");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
            driver.FindElement(By.XPath(".//*[@class='ui__field-inner']/input")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input","1234", "Поле email", "поля email");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки отправить");
            var errorMessage = GetWebElement(".//*[@class='account-error__text']", "Нет текста ошибки");
            if (!errorMessage.Text.Contains("E-mail успешно подверждён"))
                throw new Exception("Неверный текст ошибки");
            ClickWebElement(".//*[@classid='account-error__btn-inner']//span", "Кнопка Вернуться к профилю", "кнопки Вернуться к профилю");
            var mainTab = GetWebElement(".//*[@class='account-tabs']/a[1]", "Нет вкладки Основные данные");
            var mainTabClass = mainTab.GetAttribute("class");
            if (!mainTabClass.Contains("state_active"))
                throw new Exception("Не произошло возвращения на главную страницу");
        }
    }
}
