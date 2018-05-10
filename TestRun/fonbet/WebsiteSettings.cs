using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;


namespace TestRun.fonbet
{
    class BetsSettings : FonbetWebProgram
    {
        public static  CustomProgram FabricateBetsSettings()
        {
            return new BetsSettings();
        }

        public override void Run()
        {
            base.Run();

            LogStage("Установка настроек по умолчанию");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__restore-btn']", "Кнопка восстановления настроек по умолчанию", "кнопки восстановления настроек по умолчанию");

            LogStage("Установка быстрого пари в 100 руб");
            ClickWebElement(".//*[@class='settings__section'][1]/div/div[1]//*[@class='header-ui__checkbox-label']/input", "Чекбокс быстрое пари", "чекбокса быстрое пари");
            driver.FindElement(By.XPath(".//*[@class='settings__section'][1]/div/div//*[@class='settings__row']//input")).Clear(); //Очистить поле для ввода суммы быстрой ставки
            SendKeysToWebElement(".//*[@class='settings__section'][1]/div/div//*[@class='settings__row']//input","100","поле ввода значения быстрой ставки", "поля ввода значения быстрой ставки");

            LogStage("Добавление любимого пари");
            ClickWebElement("//*[@class='settings__section'][1]/div/div//*[@value='showPercent']", "Радиобатон любимого пари в % от баланса", "радиобатона любимого пари в % от баланса");
            ClickWebElement(".//*[@class='settings__row _type_normal _type_columns']//*[@class='settings__fields-action _type_add']", "Кнопка добавления нового поля для быстрой ставки", "кнопки добавления нового поля для быстрой ставки");
            SendKeysToWebElement(".//*[@class='settings__row _type_normal _type_columns']/div/div[4]//input", "50", "новое поле ввода для любимого пари", "нового поля ввода для любимого пари");

            LogStage("Разрешение на прием любимого пари");
            ClickWebElement("//*[@class='settings__section'][1]/div/div[7]//input", "Чекбокс принимать пари с измененными коэффициентами", "чекбокса принимать пари с измененными коэффициентами");
            ClickWebElement("//*[@class='settings__section'][1]/div/div[9]//input", "Чекбокс принимать пари с измененными тоталами / форами", "чекбокса принимать пари с измененными тоталами / форами");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");

            LogStage("Проверка работы быстрой ставки");
            IWebElement fastBet = GetWebElement(".//*[@class='oneClickSum on']", "Не найдено поле с суммой быстрой ставки");
            var fastBetClass = fastBet.GetAttribute("title");
            if (fastBetClass!="100")
               throw new Exception("Поле с суммой быстрой ставки содержит неверное значение");
            IWebElement fastButton = GetWebElement(".//*[@class='oneClickSwitch on']", "Не найдена кнопка с быстрой ставкой");
            var fastButtonClass = fastButton.GetAttribute("class");
            if (!fastButtonClass.Contains("on"))
                throw new Exception("Быстрое пари выключено");

            LogStage("Проверка работы приема любого пари");
            IWebElement betsOdd = GetWebElement(".//*[@class='coupons-toolbar__item _type_switchers']/div/div[1]", "Не найден элемент Принимать пари с измененными коэф.");
            var betsOddClass = betsOdd.GetAttribute("class");
            if (!betsOddClass.Contains("orange any"))
               throw new Exception("Не работает прием пари с изменными коэффициентами");
            IWebElement betsTotal = GetWebElement(".//*[@class='oneClickSwitch on']", "Не найдена кнопка с быстрой ставкой");
            var betsTotalClass = betsTotal.GetAttribute("class");
            if (!betsTotalClass.Contains("on"))
                throw new Exception("Не работает прием пари с изменными тоталами / форами");

            LogStage("Проверка расчета значения ставки в 50% от депозита");
            ClickWebElement(".//*[@class='oneClickSwitch on']", "Кнопка быстрой ставки", "кнопки быстрой ставки");
            ClickWebElement(".//*[@class='line__inner'][2]/a", "Кнопка перехода в меню Линия с панели слева", "кнопки перехода в меню Линия с панели слева");
            //Выбор ставки из грида
            IList<IWebElement> grid = driver.FindElements(By.XPath(".//*[@class='table']/tbody//td[5]"));
            grid[3].Click();
            //Проверка рачета ставки
             IWebElement newBetValue = GetWebElement(".//*[@class='coupon__foot-stakes']/a[4]", "Не найдена добавленная кнопка с ставкой в 1%");
            var betValue = Convert.ToDouble(newBetValue.Text);
            IWebElement accounBalance = GetWebElement(".//*[@class='header__login-item']//*[@class='header__login-balance']", "Не отображается баланс счета");
            string input = accounBalance.Text;
            string pattern = "\\s+";
            string replacement = "";
            Regex rgx = new Regex(pattern);
            string result = rgx.Replace(input, replacement);
            var balance = Convert.ToDouble(result, CultureInfo.GetCultureInfo("en-US").NumberFormat);
            //Отнимаю 1 когда баланс не четный, т/к деление тогда получается дробным
            if (betValue % 2 == 0)
            {
                if (Math.Round(balance / 2) != betValue)
                {
                    throw new Exception("Не корректные расчеты быстрой ставки");
                }
            }
            else
            {
                if (Math.Round(balance / 2) != betValue - 1)
                {
                    throw new Exception("Не корректные расчеты быстрой ставки");
                }
            }


            LogStage("Проверка ввода суммы больше чем баланс счета");
            SendKeysToWebElement(".//*[@class='coupon__foot-sum']/input", "9999999999", "поле ввода значения ставки", "поля ввода значения ставки");
            IWebElement placeBet = GetWebElement(".//*[@class='coupon__foot']/a", "Нет кнопки заключить пари");
            var placeBetClass = placeBet.GetAttribute("class");
            if (!placeBetClass.Contains("state_disabled"))
                throw new Exception("Кнопка не блокируется если сумма ставки выше максимальной");
            ClickWebElement(".//*[@class='coupons__list-inner']//article[1]/div[1]/a[1]", "Кнопка закрытия окна нового пари", "кнопки закрытия окна нового пари");
        }
        
    }

    class CashOutAndDialogsSettings : FonbetWebProgram
    {
        public static CustomProgram FabricateCashOutAndDialogsSettings()
        {
            return new CashOutAndDialogsSettings();
        }

        public override void Run()
        {
            base.Run();

            LogStage("Установка настроек по умолчанию");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__restore-btn']", "Кнопка восстановления настроек по умолчанию", "кнопки восстановления настроек по умолчанию");

            LogStage("Установка быстрого пари");
            ClickWebElement(".//*[@class='settings__section'][1]/div/div[1]//*[@class='header-ui__checkbox-label']/input", "Чекбокс быстрое пари", "чекбокса быстрое пари");
            

            LogStage("Редактирование Продажи пари");
            ClickWebElement("//*[@class='settings__section'][2]/div[1]//input", "Чекбокс изменение суммы продажи", "чекбокса изменения суммы продажи");
            ClickWebElement("//*[@class='settings__section'][2]/div[3]//input", "Чекбокс показывать продажу на всех вкладках", "чекбокса проказывать продажу на всех вкладках");
           
            LogStage("Редактирование меню Диалоги");
            ClickWebElement("//*[@class='settings__section'][3]/div[1]//input", "Чекбокс 'Не спрашивать подтверждение для быстрого пари'", "чекбокса 'Не спрашивать подтверждение для быстрого пари'");
            ClickWebElement("//*[@class='settings__section'][3]/div[2]//input", "Чекбокс 'Не спрашивать подтверждение для продажи'", "чекбокса 'Не спрашивать подтверждение для продажи'");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");


            LogStage("Проверка работы настроек Продажи пари");
            if (WebElementExist(".//*[@class='coupon__sell-button-area']"))
                throw new Exception("Не работает чекбокс показывать продажу на всех вкладках");
            ClickWebElement(".//*[@class='coupons-toolbar']//*[@title='На продажу']", "Вкладка 'На продажу' в меню купонов", "вкладки 'На продажу' в меню купонов");
            if (!(WebElementExist(".//*[@class='coupon__sell-button-area']") || WebElementExist(".//*[@class='coupon__content-empty']")))
                throw new Exception("Не корректно отрабатывает вкладка меню 'На продажу'");
            IWebElement sellSwitch = GetWebElement(".//*[@class='coupon__sell-button-area']/a[2]", "Нет тумблера рядом с кнопкой продажи пари");
            var sellSwitchClass = sellSwitch.GetAttribute("class");
            if (!sellSwitchClass.Contains("coupon__sell-switch _all"))
                throw new Exception("Не работает чекбокс изменение суммы продажи");

            LogStage("Проверка работы настроек Диалогов");
            SwitchPageToBets();
            IList<IWebElement> bet = driver.FindElements(By.XPath(".//*[@class='table']/tbody//td[5]"));
            bet[4].Click();
            if (WebElementExist(".//*[@class='modal-window__button-area']"))
                throw new Exception("Не работает чекбокс 'Не спрашивать подтверждение для быстрого пари'");
            ClickWebElement(".//*[@class='coupons-toolbar']//*[@title='На продажу']", "Вкладка 'На продажу' в меню купонов", "вкладки 'На продажу' в меню купонов");
            ClickWebElement(".//*[@class='coupons__list-inner']//article[1]//*[@class='coupon__sell-button-area'][1]/a[1]", "Кнопка 'Продать пари'", "кнопки 'Продать пари'");
            if (WebElementExist(".//*[@class='modal-window__button-area']"))
                throw new Exception("Не работает чекбокс 'Не спрашивать подтверждение для продажи'");
        }

    }
}
