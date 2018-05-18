using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TestRun.fonbet
{
    class AtLeastTwo : FonbetWebProgram
    {
        public static CustomProgram FabricateAtLeastTwo()
        {
            return new AtLeastTwo();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();

            LogStage("Проверка на продажу ставки выше исходного");
            if (WebElementExist(".//*[@class='coupon__sell-button _state_orange']"))
            {
                IWebElement lastprice = GetWebElement(".//*[@class='coupon__sell-button _state_orange']/span[2]", "Не отображается цена продажи на кнопке продажи");
                IWebElement firstprice = GetWebElement(".//*[@class='coupon__sell-button _state_orange']/../../../*[@class='coupon__info']//*[@title='Сумма пари']/i[2]", "Не отображается сумма пари в купоне");
                var lastPriceText = Convert.ToDouble(lastprice.Text);
                var firstPriceText = Convert.ToDouble(firstprice.Text);

                if (lastPriceText < firstPriceText)
                {
                    throw new Exception("Не корректная продажа ставки");
                }
            }

            SwitchPageToBets();

            LogStage("Выбор событий на систему 2/3");
            IList<IWebElement> grid = driver.FindElements(By.XPath(".//*[@class='table']/tbody//td[5]")); //все ставки из одного столбца грида событий
            grid[3].Click();
            grid[4].Click();
            grid[5].Click();
            ClickWebElement(".//*[@class='coupons']/div[1]//*[@class='coupon__info-item-inner']", "Кнопка выбора типа пари", "кнопки выбора типа пари");
            ClickWebElement(".//*[@id='popup']/li[2]", "Строка Система 2/3", "строки Систима 2/3");
            ClickWebElement(".//*[@class='coupons']/div[1]//*[@class='coupon__info-item-inner']/i", "Иконка Калькулятор", "иконка Калькулятор");

            LogStage("Проверка калькулятора");
            IWebElement possibleWin = GetWebElement(".//*[@class='calculator__head']/div[3]", "Не отображается возможный выигрыш в калькуляторе");
            IWebElement firstBetfirstEv = GetWebElement("//*[@class='calculator__table _right']/tbody/tr[1]/td[1]", "Не отображается первая ставка первого события");
            IWebElement firstBetSecEv = GetWebElement("//*[@class='calculator__table _right']/tbody/tr[2]/td[1]", "Не отображается первая ставка второго события");
            IWebElement betSumm = GetWebElement(".//*[@class='calculator__head']//*[@title='Сумма пари']", "Не отображается сумма пари");
            IWebElement combinationRate = GetWebElement("//*[@class='calculator__table _right']/tbody/tr[6]/td[1]", "Не отображается коэффициент комбинации");
            IWebElement winRate = GetWebElement("//*[@class='calculator__table _right']/tbody/tr[7]/td[1]", "Не отображается выигрыш комбинации");

            var possibleWinText = possibleWin.Text;
            var firstBetfirstEvText = firstBetfirstEv.Text;
            var firstBetSecEvText = firstBetSecEv.Text;

            var betSummText = Convert.ToDouble(betSumm.Text);
            var a = Math.Round(betSummText / 3, 2);
            var b = Convert.ToDouble(combinationRate.Text, CultureInfo.GetCultureInfo("en-US").NumberFormat);
            string input = winRate.Text;
            string pattern = "\\s+";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            string result = rgx.Replace(input, replacement);
            var sum = Convert.ToDouble(result);
            string finalResult = rgx.Replace(possibleWinText, replacement);
            var finalSum = Convert.ToDouble(finalResult);

            if (Convert.ToDouble(firstBetfirstEvText, CultureInfo.GetCultureInfo("en-US").NumberFormat) *
                Convert.ToDouble(firstBetSecEvText, CultureInfo.GetCultureInfo("en-US").NumberFormat) == Math.Round(
                    Convert.ToDouble(
                        driver.FindElement(By.XPath("//*[@class='calculator__table _right']/tbody/tr[6]/td[1]")).Text,
                        CultureInfo.GetCultureInfo("en-US").NumberFormat)))
            {
                if (Math.Round(Math.Round(betSummText / 3, 2) * Convert.ToDouble(
                                   driver.FindElement(
                                       By.XPath("//*[@class='calculator__table _right']/tbody/tr[6]/td[1]")).Text,
                                   CultureInfo.GetCultureInfo("en-US").NumberFormat)) == sum)
                {
                    if (Math.Round(Math.Round(betSummText / 3, 2) * Convert.ToDouble(
                                       driver.FindElement(
                                           By.XPath("//*[@class='calculator__table _right']/tbody/tr[6]/td[1]")).Text,
                                       CultureInfo.GetCultureInfo("en-US").NumberFormat) +
                                   Math.Round(betSummText / 3, 2) * Convert.ToDouble(
                                       driver.FindElement(
                                           By.XPath("//*[@class='calculator__table _right']/tbody/tr[6]/td[2]")).Text,
                                       CultureInfo.GetCultureInfo("en-US").NumberFormat) +
                                   Math.Round(betSummText / 3, 2) * Convert.ToDouble(
                                       driver.FindElement(
                                           By.XPath("//*[@class='calculator__table _right']/tbody/tr[6]/td[3]")).Text,
                                       CultureInfo.GetCultureInfo("en-US").NumberFormat)) == Convert.ToDouble(finalSum))
                    {
                        ClickWebElement("//*[@class='calculator__left-wrap']//tbody/tr[1]/td[3]//input",
                            "Чекбокс первого исхода", "чекбокса первого исхода");

                        IWebElement firstEvent = GetWebElement("//*[@class='calculator__left-wrap']//tbody/tr[1]",
                            "Не отображается первое событие в калькуляторе");
                        var firstEventClass = firstEvent.GetAttribute("class");
                        if (!firstEventClass.Contains("disabled"))
                            throw new Exception("Не работает чекбокс Исход");
                        if (!firstBetfirstEv.Text.Contains("0.00"))
                            throw new Exception("Не обнуляется кэф при снятии чекбокса");

                        IWebElement winCombo = GetWebElement("//*[@class='calculator__table _right']/tbody/tr[7]/td[3]", "Не отображается выигрыш комбинации");
                        var winComboValue = winCombo.Text;
                        var possibleWinValue = possibleWin.Text;
                        if (winComboValue != possibleWinValue)
                            throw new Exception("Выигрыш комбинации и возможный выигрыш не совпадают");
                    }
                    else
                    {
                        throw new Exception("Сломалась общаяя сумма(калькулятор)");
                    }
                }
                else
                {
                    throw new Exception("Сломался выйгрыш комбинации(калькулятор)");
                }
            }
            else{ throw new Exception("Калькулятор сломался(коэффициент комбинации)"); }
        }
    }
    class CorrectBetsShowing : FonbetWebProgram
    {
        public static CustomProgram FabricateCorrectBetsShowing()
        {
            return new CorrectBetsShowing();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            SwitchPageToBets();

            IList<IWebElement> rows = driver.FindElements(By.XPath(".//*[@class='table__row']")); //строки с событиями
            IList<IWebElement> btns = null;
            IWebElement row = null;
            IList<IWebElement> fora = null;

            LogStage("Выбирается событие с 10 возможными ставками");
            for (var i = 0; i < rows.Count; i++) //Выбираем строку события где есть 10 возможных ставок 
            {
                row = rows[i];
                btns = row.FindElements(By.XPath(".//*[@class='table__col _type_btn _type_normal']")); //коэффициенты
                fora = row.FindElements(By.XPath(".//*[@class='table__col _type_fora']")); //значения фор и тоталов
                // if (btns.Count && btns[0]. != row) continue;
                if (btns.Count == 10) break;
                fora = null;
                btns = null;
            }

            var foraText = fora[2].Text;

            string[] data = {
                "Поб 1",
                "Ничья",
                "Поб 2",
                "1X",
                "12",
                "X2",
                "1",
                "2",
                "> "+foraText+"",
                "< "+foraText+""
            };
            LogStage("Проверка соответсвтия ставки в гриде и ставки в купоне");
            for (var i = 0; i < data.Length; i++)
            {
                string x = data[i];
                btns[i].Click();
                IWebElement couponRate = GetWebElement("//*[@class='coupons']/div[1]//*[@class='coupon__info-head']/div[2]/div[2]/i[2]", "Не отображается кэф ставки в купоне");
                var couponRateValue = couponRate.Text;
                if(btns[i].Text != couponRateValue)
                    throw new Exception("Коэффициенты в гриде и в купоне отличаются");
                
                IWebElement couponResult = GetWebElement("//*[@class='coupons']/div[1]//tbody/tr/td[3]/span", "Не отображается исход ставки в купоне");
                var couponResultValue = couponResult.Text;
                if(couponResultValue != "" + x + "")
                    throw new Exception("Исход события в гриде и в купоне отличаются");
            }
        }
    }
}
