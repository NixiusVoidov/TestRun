using System;
using System.Collections.Generic;
using System.Linq;
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
}
