using System;
using System.Collections.Generic;
using System.Threading;
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

            MakeDefaultSettings();

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
            string defaultBetValue = defaultBet.Text;
            if (!defaultBetValue.Equals("100"))
                throw new Exception("Минимальная ставка по умолчанию не равна 100");
            MarkedBoxCounter(15, " _marked");
            IWebElement optionCount = GetWebElement(".//*[@class='new-coupon__label-value _value']", "Не указано число вариантов");
            string optionCountValue = optionCount.Text;
            if (!optionCountValue.Equals("1"))
                throw new Exception("Случайный выбор не выдает  1 возможное значение");

            LogStage("Устанавливаем 2 возможных варианта");
            ChooseTwoResults();
            string optionCountValues = optionCount.Text;
            if (!optionCountValues.Equals("2"))
                throw new Exception("Не работает счетчик числа вариантов");
            string BetValue = defaultBet.Text;
            if (!BetValue.Equals("200"))
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
            ClickWebElement(".//*[@class='new-bet__sum-value _kind_min']", "Минимальня ставка", "минимальной ставки");
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
    class SuperExpressBatch : FonbetWebProgram
    {
        public static CustomProgram FabricateSuperExpressBatch()
        {
            return new SuperExpressBatch();
        }

        public override void Run()
        {
            base.Run();

            MakeDefaultSettings();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            LogStage("Переход в Суперэкспресс");
            ClickWebElement(".//*[@href='/#!/superexpress']", "Вкладка \"Суперэкспресс\"", "вкладки \"Суперэкспресс\"");
            ClickWebElement(".//*[@class='matrix-form__header-switcher']/div[1]", "Кнопка Пакет", "кнопки Пакет");

            LogStage("Проверка кнопок из шапки");
            ClickWebElement(".//*[@class='header-action__item']/div", "Кнопка Пример", "кнопки Пример");
            if (driver.FindElements(By.XPath(".//*[@class='list-view__table-body']/tr")).Count < 40)
                throw new Exception("Не сработала кнопка Пример");
            ClickWebElement(".//*[@class='matrix-form__header-actions']/div[2]", "Кнопка Очистить", "кнопки Очистить");
            if (!driver.FindElement(By.XPath(".//*[@class='list-view__body']//span")).GetAttribute("class").Contains("state_empty"))
                throw new Exception("Не сработала кнопка Очистить");

            LogStage("Проверка табличного вида");
            ClickWebElement(".//*[@class='matrix-form__header-actions']/div[1]", "Кнопка Табличный вид", "кнопки Табличный вид");
            if (!WebElementExist(".//*[@class='batch-form__add-new-variant matrix-form__button-tap']"))
                throw new Exception("Не сработала кнопка Табличный вид");
            ClickWebElement("//*[@class='batch-form__add-new-variant matrix-form__button-tap']", "Кнопка Добавить вариант", "кнопки Добавить вариант");
            Thread.Sleep(1500);
            IList<IWebElement> firstString = driver.FindElements(By.XPath(".//*[@class='batch-table__variant-selector']"));
            foreach (IWebElement element in firstString)
            {
                element.Click();
            }
            ClickWebElement("//*[@class='batch-form__add-new-variant matrix-form__button-tap']", "Кнопка Добавить вариант", "кнопки Добавить вариант");
            if (!driver.FindElement(By.XPath(".//*[@class='new-coupon__box-footer']/a")).GetAttribute("class").Contains("state_disabled"))
                throw new Exception("Возможно заключить пари с пустой строкой");

            IList<IWebElement> secondString = driver.FindElements(By.XPath("//*[@class='list-view__table-body']/tr[2]//*[@class='batch-table__variant-selector']"));
            foreach (IWebElement element in secondString)
            {
                element.Click();
                element.Click();
            }
            CheckBatchBet();

            LogStage("Проверка ручного ввода");
            ClickWebElement(".//*[@class='matrix-form__header-actions']/div[1]", "Кнопка Ввод текста", "кнопки Ввод текста");
            ClickWebElement(".//*[@class='matrix-form__header-actions']/div[2]", "Кнопка Очистить", "кнопки Очистить");
            ClickWebElement(".//*[@class='list-view__body']", "Поле ввода ставок", "поля ввода ставок");
            SendKeysToWebElement(".//*[@class='batch-editor__text-input _state_error']", "100; 1-(X); 2-(X); 3-(X); 4-(X); 5-(X); 6-(X); 7-(2); 8-(1); 9-(X); 10-(1); 11-(2); 12-(1); 13-(2); 14-(X); 15-(1).", "Ставка первого события", "ставки первого события");
            SendKeysToWebElement(".//*[@class='batch-editor__text-input']", Keys.Enter, "Переход к след строке", "перехода к след строке");
            SendKeysToWebElement(".//*[@class='list-view__table-body']/tr[2]//input", "100; 1-(1); 2-(2); 3-(X); 4-(1); 5-(1); 6-(X); 7-(2); 8-(2); 9-(X); 10-(1); 11-(2); 12-(1); 13-(2); 14-(X); 15-(1).", "Ставка второго события", "ставки второго события");
            CheckBatchBet();
        }

        private void CheckBatchBet()
        {
            LogStage("Проверка что ставка поставилась правильно");
            ClickWebElement(".//*[@class='new-coupon__box-footer']/a", "Кнопка Заключить пари", "кнопки Заключить пари");
            if (WebElementExist(".//*[@class='new-coupon__error-message']"))
            {
                var error = driver.FindElement(By.XPath(".//*[@class='new-coupon__error-message']")).Text;
                throw new Exception(error);
            }
            if (driver.FindElement(By.XPath(".//*[@class='placed-coupon__box-caption']/span[1]")).Text.ToUpper() != "ПАКЕТ ПАРИ")
                throw new Exception("В заголовке купона не указано Пакет пари");
            if (driver.FindElement(By.XPath(".//*[@class='placed-batch__count-value']")).Text != "2")
                throw new Exception("Кол-во пари в купоне не верно");
        }
    }
}
