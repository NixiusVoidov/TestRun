using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace TestRun.fonbet.cyprus
{
    class RegistrationCypr : FonbetWebProgram 
    {
        public static CustomProgram FabricateRegistrationCypr()
        {
            return new RegistrationCypr();
        }

        protected override bool NeedLogin()
        {
            return false;
        }
        public override void Run()
        {
            base.Run();

           
            if (!WebElementExist(".//*[@href='/#!/account/registration']"))
                throw new Exception("На данном сайте нет регистрации");

            LogStage("Переход на страницу регистрации");
            ClickWebElement(".//*[@href='/#!/account/registration']", "Кнопка Регистрации", "кнопки Регистрации");
            var required = driver.FindElements(By.XPath(".//*[@class='ui__required']"));
            if (required.Count != 11)
                throw new Exception("Число обязательных полей не равно 11");
            LogStage("Проверка createProcess по тестовому сценарию");
            CreateProcessRegistrationCypr("1", "rejected", "0", "10");
            CreateProcessRegistrationCypr("2", "rejected", "0", "11");
            CreateProcessRegistrationCypr("3", "rejected", "0", "12");
            CreateProcessRegistrationCypr("4", "rejected", "0", "4");
            CreateProcessRegistrationCypr("5", "rejected", "0", "1");
            CreateProcessRegistrationCypr("6",null, null, null);
            LogStage("Проверка sendcode по тестовому сценарию");
            SendKeysToWebElement(".//*[@class='ui__field-wrap-inner']//input", "6", "Поле Код смс", "поля  Код смс");
            var msg = GetWebElement(".//*[@id='registration-cyprus-error']", "Нет модуля с ошибкой");
            if (!(msg.GetAttribute("data-processstate").Equals("waitForVerificationCode") && msg.GetAttribute("data-errorcode").Equals("10")))
                throw new Exception("Неверная обработка ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Подтвердить", "кнопки Подтвердить");
            driver.FindElement(By.XPath(".//*[@class='ui__field-wrap-inner']//input")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field-wrap-inner']//input", "1234", "Поле Код смс", "поля  Код смс");

            SendKeysToWebElement(".//*[@class='registration__password-fields']/div[1]//input", "123qwe123", "Поле Пароль", "поля Пароль");
            SendKeysToWebElement(".//*[@class='registration__password-fields']/div[2]//input", "123qwe123", "Поле Подтверждение пароля", "поля Подтверждение пароля");
            ClickWebElement(".//*[@class='toolbar__item process-button']/button", "Кнопка Завершить регистрацию", "кнопки Завершить регистрацию");
            if(!WebElementExist(".//*[@class='account-error _type_success']"))
                throw new Exception("Регистрация НЕ прошла успешно");
        }

        private void CreateProcessRegistrationCypr(string postcode, string process, string code, string rejcode)
        {
            LogStage("Заполнение персональных данных");
            IList<IWebElement> checkbox = driver.FindElements(By.XPath(".//*[@class='ui__checkbox _type_checkbox']"));
            foreach (IWebElement element in checkbox)
                element.Click();

            SendKeysToWebElement(".//*[@class='registration__form-inner _size_middle']/div[1]/div[1]/div[1]//input", "Test","Поле Имя", "поля Имя");
            SendKeysToWebElement(".//*[@class='registration__form-inner _size_middle']/div[1]/div[1]/div[2]//input", "LastName","Поле Фамилия", "поля Фамилия");
            IsRegistrationDisabled(2);
            SendKeysToWebElement(".//*[@class='registration__form-inner _size_middle']/div[1]/div[1]/div[4]//input", "4213","Поле Номер паспорта", "поля Номер паспорта");
            IsRegistrationDisabled(3);
            driver.FindElement(By.XPath(".//*[@class='registration__form-inner _size_middle']/div[1]/div[1]/div[5]//input")).SendKeys(Keys.Home);
            SendKeysToWebElement(".//*[@class='registration__form-inner _size_middle']/div[1]/div[1]/div[5]//input", "11111999","Поле Дата рождения", "поля Дата рождения");
            IsRegistrationDisabled(4);
            SendKeysToWebElement(".//*[@class='registration__form-inner _size_middle']/div[1]/div[1]/div[6]//input","lol@ya.ru", "Поле Email", "поля Email");
            IsRegistrationDisabled(5);
            ClickWebElement(".//*[@class='registration__form-inner _size_middle']/div[1]/div[1]//a", "Выбор Обращения","выбора Обращения");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[1]", "Строка Господин", "строки Господин");
            IsRegistrationDisabled(6);
            SendKeysToWebElement(".//*[@class='registration__form-inner _size_middle']/div[1]/div[2]/div[2]//input", "Piter","Поле Город", "поля Город");
            IsRegistrationDisabled(7);
            SendKeysToWebElement(".//*[@class='registration__form-inner _size_middle']/div[1]/div[2]/div[4]//input","ul.Gogolya, 12", "Поле Адрес", "поля Адрес");
            IsRegistrationDisabled(8);
            SendKeysToWebElement(".//*[@class='registration__form-inner _size_middle']/div[1]/div[2]/div[5]//input", postcode,"Поле Почтовый индекс", "поля Почтовый индекс");
            IsRegistrationDisabled(9);
            SendKeysToWebElement(".//*[@class='registration__form-inner _size_middle']/div[1]/div[1]/label//input","35742425311", "Поле Номер телефона", "поля Номер телефона");
            IsRegistrationDisabled(10);
            SendKeysToWebElement(".//*[@class='registration__form-inner _size_middle']/div[2]//input", "1","Поле Код на картинке", "поля Код на картинке");
            ClickWebElement(".//*[@class='toolbar__item process-button']/button", "Кнопка Продолжить", "кнопки Продолжить");

            var msg = GetWebElement(".//*[@id='registration-cyprus-error']", "Нет модуля с ошибкой");
            if (!(msg.GetAttribute("data-processstate").Equals(process) && msg.GetAttribute("data-errorcode").Equals(code) &&
                  msg.GetAttribute("data-rejectioncode").Equals(rejcode)))
                throw new Exception("Неверная обработка ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Подтвердить", "кнопки Подтвердить");
        }

        private void IsRegistrationDisabled(int number)
        {
            if (!driver.FindElement(By.XPath(".//*[@class='toolbar__item process-button']/button")).GetAttribute("class")
                .Contains("disabled"))
                throw new Exception(String.Format("Можно продолжить регистрацию уже с {0} параметрами", number));
        }
    }
}
