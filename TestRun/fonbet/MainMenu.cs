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
    //class Slider : FonbetWebProgram
    //{
    //    public static CustomProgram FabricateSlider()
    //    {
    //        return new Slider();
    //    }

    //    protected override bool NeedLogin()
    //    {
    //        return false;
    //    }

    //    public override void Run()
    //    {
    //        base.Run();

    //        if (!WebElementExist(".//*[@class='how2play']"))
    //            throw new Exception("Нет виджета HowToPlay");

    //        LogStage("Проверка работы всех разделов меню \"Я хочу заключать пари в интернете\"");
    //        ClickWebElement(".//*[@class='how2play__help-steps']/div[1]", "Кнопка как заключать пари в интернете", "кнопки как заключать пари в интернете");
    //        ClickWebElement(".//*[@class='how2play__stepout _visible']/div[1]", "Раздел удаленная идентификация", "раздела удаленная идентификация");
    //        if (!WebElementExist(".//*[@class='how2play__stepout _visible']/div[1]//*[@href='#!/account/registration/RemoteVerification2']"))
    //            throw new Exception("Нет ссылки на удаленную верификацию");

    //        Setup(browserName);
    //        driver.FindElement(By.XPath(".//*[@class='home-slider__switch-wrap']/div[1]")).Click();
    //        Assert.True(driver.FindElement(By.XPath(".//*[@class='home-slider__switch-wrap']/div[1]")).GetAttribute("class").Contains("selected"), "Проблемы со слайдером на главной странице");
    //        driver.FindElement(By.XPath(".//*[@class='home-slider__switch-wrap']/div[2]")).Click();
    //        Assert.True(driver.FindElement(By.XPath(".//*[@class='home-slider__switch-wrap']/div[2]")).GetAttribute("class").Contains("selected"), "Проблемы со слайдером на главной странице");
    //        driver.FindElement(By.XPath(".//*[@class='home-slider__bets']/div[1]")).Click();
    //        Assert.True(IsElementPresent(".//*[@class='authorization__inner']"), "Не появилось окно авторизации");
    //        driver.FindElementWait(By.XPath(".//*[@class='login-form__form']/div[1]/input"), 6).SendKeys("11");
    //        driver.FindElement(By.XPath(".//*[@class='login-form__form']/div[2]/input")).SendKeys("ueueue11");
    //        driver.FindElement(By.XPath(".//*[@class='login-form__form-row _right']/div[2]/button")).Click();
    //        // Thread.Sleep(1000);
    //        // driver.FindElementWait(By.XPath(".//*[@class='home-slider__switch-wrap']/div[2]"),8).Click();
    //        waitElementIsVisible(driver, By.XPath(".//*[@class='home-slider__bets']/div[1]"));
    //        driver.FindElement(By.XPath(".//*[@class='home-slider__bets']/div[1]")).Click();
    //        Assert.True(IsElementPresent(".//*[@class='coupons']/div[1]//*[@class='coupon__title']"), "Не появилась новая ставка");


    //        ClickWebElement(".//*[@class='how2play__stepout _visible']/div[2]", "Раздел карты Фонбет", "раздела карты Фонбет");
    //        if (!WebElementExist(".//*[@class='how2play__stepout _visible']/div[2]//*[@href='#!/account/registration/Bk']"))
    //            throw new Exception("Нет ссылки на регистрацию БК");

    //        ClickWebElement(".//*[@class='how2play__stepout _visible']/div[3]", "Раздел кошелек Киви", "раздела кошелек Киви");
    //        if (!WebElementExist(".//*[@class='how2play__stepout _visible']/div[3]//*[@href='#!/account/registration/Qiwi']"))
    //            throw new Exception("Нет ссылки на регистрацию Киви");

    //        ClickWebElement(".//*[@class='how2play__stepout _visible']/div[4]", "Раздел без карты и без киви", "раздела без карты и без киви");
    //        if (!WebElementExist(".//*[@class='how2play__stepout _visible']/div[4]//*[@href='#!/account/how2Register']"))
    //            throw new Exception("Нет ссылки на  ИНУЮ регистрацию");

    //        LogStage("Проверка работы всех разделов меню \"Я хочу заключать пари в клубах\"");
    //        ClickWebElement(".//*[@class='how2play__help-steps']/div[2]", "Кнопка как заключать пари в клубах", "кнопки как заключать пари в клубах");
    //        if (!WebElementExist("//*[@href='#!/products/addresses']"))
    //            throw new Exception("Нет ссылки на найти клубы");
    //        if (!WebElementExist("//*[@href='#!/pages/fonapps']"))
    //            throw new Exception("Нет ссылки на \"Как играть на ставкомате\"");
    //    }
    //}
}
