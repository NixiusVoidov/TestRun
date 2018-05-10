using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace TestRun.fonbet
{
    class SuperExpress : FonbetWebProgram
    {
        public static CustomProgram FabricateSuperExpress()
        {
            return new SuperExpress();
        }

        public override void Run()
        {
            base.Run();

            LogStage("Установка настроек по умолчанию");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__restore-btn']", "Кнопка восстановления настроек по умолчанию", "кнопки восстановления настроек по умолчанию");

            LogStage("Переход в Суперэкспресс");
            ClickWebElement(".//*[@href='/#!/superexpress']", "Вкладка \"Суперэкспресс\"", "вкладки \"Суперэкспресс\"");
            if (!WebElementExist(".//*[@class='main-panel__block-left']"))
                throw new Exception("Пропал левый информационный блок в суперэкспрессе");
            IWebElement makeBet = GetWebElement(".//*[@class='new-coupon__box-footer']/a", "Не найдена кнопка с заключить пари");
            var makeBetClass = makeBet.GetAttribute("class");
            if (!makeBetClass.Contains("_state_disabled"))
                throw new Exception("Возможно заключить пари без суммы ставки");

            LogStage("Проверяем случайный выбор");
            ClickWebElement(".//*[@class='matrix-form__header-actions']/div[2]", "Кнопка случайный выбор", "кнопки случайный выбор");
            IWebElement defaultBet = GetWebElement(".//*[@class='new-coupon__box-sum']/span[1]", "Не указан нижний порог ставки");
            var defaultBetValue = defaultBet.Text;
            if (!defaultBetValue.Equals("50"))
                throw new Exception("Минимальная ставка по умолчанию не равна 50");
            MarkedBoxCounter(15, " _marked");
            IWebElement optionCount = GetWebElement(".//*[@class='new-coupon__label-value _value']", "Не указано число вариантов");
            var optionCountValue = optionCount.Text;
            if (!optionCountValue.Equals("1"))
                throw new Exception("Случайный выбор не выдает  1 возможное значение");

            LogStage("Устанавливаем 2 возможных варианта");
            ChooseTwoResults();
            var optionCountValues = optionCount.Text;
            if (!optionCountValues.Equals("2"))
                throw new Exception("Не работает счетчик числа вариантов");
            var BetValue = defaultBet.Text;
            if (!BetValue.Equals("100"))
                throw new Exception("Не увеличивается минимальная сумма ставки при увеличении числа вариантов");
            IWebElement doBet = GetWebElement(".//*[@class='new-coupon__box-footer']/a", "Не найдена кнопка с заключить пари");
            var doBetClass = doBet.GetAttribute("class");
            if (!doBetClass.Contains("_state_disabled"))
                throw new Exception("Возможно заключить пари на сумму меньше минимальной");

            LogStage("Проверяем кнопку Очистить");
            ClickWebElement(".//*[@class='matrix-form__header-actions']/div[1]", "Кнопка очистки выбранных событий", "кнопки очистки выбранных событий");
            MarkedBoxCounter(45, null);

            LogStage("Проверяем возможность ставки");
            ClickWebElement(".//*[@class='matrix-form__header-actions']/div[2]", "Кнопка случайный выбор", "кнопки случайный выбор");
            ClickWebElement(".//*[@class='new-coupon__button _kind_place']", "Кнопка заключить пари", "кнопки заключить пари");
            if(!WebElementExist(".//*[@class='coupon-item placed-coupon']"))
                throw new Exception("Не появился купон с поставленным пари");

            LogStage("Проверяем копирование купона");
            ChooseTwoResults();
            ClickWebElement(".//*[@class='placed-coupon__box-header']/span[2]", "Кнопка копирования купона", "кнопки копирования купона");
            MarkedBoxCounter(15, " _marked");

            LogStage("Проверяем закрытие купона");
            ClickWebElement(".//*[@class='placed-coupon__box-header']/span[1]", "Кнопка закрытия купона", "кнопки закрытия купона");
            if (WebElementExist(".//*[@class='coupon-item placed-coupon']"))
                throw new Exception("Закрытие купона не работает");
        }

    }
}
