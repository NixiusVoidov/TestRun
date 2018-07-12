using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;


namespace TestRun
{
    class FonbetWebProgramParameters : WebTestProgramParameters
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    class FonbetWebProgram : CustomWebTestProgram
    {
        public string Login = null;
        public string Password = null;
        protected string ClientName = null;
        protected double ClientBalance = 0.0;

        public static CustomProgram FabricateFonbetWebProgram()
        {
            return new FonbetWebProgram();
        }

        public void ReadParameters(FonbetWebProgramParameters parameters)
        {
            base.ReadParameters(parameters);
            Login = parameters.Login;
            Password = parameters.Password;
        }

        public override void ReadParameters(TestTaskResponseBody prm)
        {
            base.ReadParameters(prm);
            Login = prm.user;
            Password = prm.password;
        }

        public override void ReadParamsFromJson(string jsonText)
        {
            FonbetWebProgramParameters prm = JsonConvert.DeserializeObject<FonbetWebProgramParameters>(jsonText);
            ReadParameters(prm);
        }

        public override void SetFromString(string paramName, string paramValue)
        {
            if (paramName == "Login")
                Login = paramValue;
            else if (paramName == "Password")
                Password = paramValue;
            else
                base.SetFromString(paramName, paramValue);
        }

        public override void PrintParameters()
        {
            base.PrintParameters();
            Console.WriteLine("Логин: \t\t\t\t{0}", Login);
        }

        public override void WriteParametersToReport()
        {
            base.WriteParametersToReport();
            Report.Conditions.Add("Login", Login);
        }

        protected virtual bool NeedLogin()
        {
            return true;
        }

        protected void DoLogin()
        {
            LogStage(String.Format("Логин под \"{0}\"", Login));
            ClickWebElement(".//*[@class='header__login-head']/a", "Панель логина", "панели логина");
            SendKeysToWebElement(".//*[@class='login-form__form']/div[1]/input", Login, "поле логина", "поля логина");
            SendKeysToWebElement(".//*[@class='login-form__form']/div[2]/input", Password, "поле пароля", "поля пароля");
            ClickWebElement(".//*[@class='login-form__form-row _right']/div[2]/button", "Кнопка логина", "кнопки логина");

            LogStartAction("Ожидание входа");
            IWebElement errorElement = FindWebElement(".//*[@class='login-form__error']");
            if (errorElement != null)
                throw new Exception(String.Format("Ошибка логина: {0}", errorElement.Text));
            if(WebElementExist(".//*[@class='ident-instruction--3wvHY']"))
                ClickWebElement(".//*[@class='ident-instruction__foot--2-EbX']/div[1]//a", "Кнопка закрыть", "кнопки закрыть");
            LogActionSuccess();
        }

        protected void UpdateLoginInfo()
        {
            IWebElement loginCaptionElement = FindWebElement(".//*[@class='header__login-label _style_white']");
            if (loginCaptionElement != null)
            {
                ClientName = loginCaptionElement.Text;
                LogHint(String.Format("Клиент: {0}", ClientName));
            }
            else
            {
                LogWarning("Информации о клиенте на странице не обнаружено.");
                ClientName = null;
            }

            IWebElement balanceElement = FindWebElement(".//*[@class='header__login-balance']");
            if (balanceElement != null)
            {
                string balanceText = balanceElement.Text.Replace(" ", "").Replace(".", ",");
                
                if (Double.TryParse(balanceText, out ClientBalance))
                {
                    LogHint(String.Format("Баланс: {0:F2} ", ClientBalance));
                }
                else
                {
                    ClientBalance = 0.0;
                    LogWarning(String.Format("Неверный формат значения баланса клиента: {0}", balanceElement.Text));
                }
            }
            else
                LogWarning("Информации о балансе клиента на странице не обнаружено.");
        }
        // Метод переходит на вкладку Линия
        protected void SwitchPageToBets()
        {
            LogStage("Переход в линию");
            ClickWebElement(".//*[@href='/#!/bets']", "Вкладка \"Линия\"", "вкладки \"Линия\"");
        }

       

        // Метод проверяет что при сужении окна бразуера появляется скролл у фильтра в верхнем меню, выдает ошибку если это не так
        protected void CheckScrollinFilterTopMenu(int x, int y)
        {
            var windowSize = new System.Drawing.Size(x, y);
            driver.Manage().Window.Size = windowSize;
            ExecuteJavaScript("return document.getElementById(\"popup\").scrollHeight>document.getElementById(\"popup\").clientHeight;", "Не работает скролл в фильтре верхнего меню");
        }

        // Метод кликает на фильтр выбора спорта
        protected void ClickOnSportType()
        {
            LogStage("Открытие меню с видами спорта");
            ClickWebElement(".//*[@class='events__filter _type_sport']", "Фильтр выбора спорта", "фильтра выбора спорта");
        }

        // Метод переходит в личный кабинет
        protected void ClickOnAccount()
        {
            LogStage("Переход в Личный кабинет");
            ClickWebElement(".//*[@class='header__item header__login']/div/div[1]/span", "ФИО в шапке", "ФИО в шапке");
            ClickWebElement(".//*[@id='popup']/li[1]", "Кнопка Личный кабинет", "кнопки Личный кабинет");
        }

        // Метод устанавливает настройки вебсайта по-умолчанию.
        protected void MakeDefaultSettings()
        {
            LogStage("Установка настроек по умолчанию");
            ClickWebElement(".//*[@id='settings-popup']", "Меню настроек", "меню настройки");
            ClickWebElement(".//*[@class='settings__restore-btn']", "Кнопка восстановления настроек по умолчанию", "кнопки восстановления настроек по умолчанию");
            ClickWebElement(".//*[@class='settings__head']/a", "Кнопка закрытия меню  настроек", "кнопки закрытия меню  настроек");
            LogActionSuccess();
        }
         // Метод открывает фильтр событий
        protected void OpenBetsEventFilter()
        {
            LogStage("Открытие фильтра событий");
            ClickWebElement(".//*[@class='events__filter _type_sport']", "Фильтр событий", "фильтра событий");
        }

        protected void OpenRequests()
        {
            LogStage("Переход в меню \"Запросы\" ");
            ClickWebElement(".//*[@href='#!/account/requests']", "Меню \"Запросы\"", "меню \"Запросы\"");
            if (driver.Title != "Запросы")
                throw new Exception("Страница не содержит title \"Запросы\"");
        }
        // Метод переключает меню в режим отображения слева
        protected void SwitchToLeftTypeMenu()
        {
            LogStage("Переключение в меню 'слева'");
            ClickWebElement(".//*[@class='page__line-header']//*[@class='events__head _page_line']/div[1]", "Кнопка разворот меню фильтра", "кнопка разворота меню фильтра");
            ClickWebElement(".//*[@id='popup']/li[1]", "Меню СЛЕВА", "меню слева");
        }
        // Метод проверяет существование элемента в DOM
        protected bool WebElementExist(string element)
        {
            try
            {
                driver.FindElement(By.XPath(string.Format(element)));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
       
        protected void WaitForPageLoad()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until((wdriver) => (driver as IJavaScriptExecutor).ExecuteScript("return document.readyState").Equals("complete"));
           
        }
       


        // Метод принимает на вход число минут и название строки в фильтре времени и проверяет что в результат выдачи попадают только те события, которые удовлетворяют кол-ву минут, переданных в параметр
        protected void TimeFilterChecker(int timeValue, string chooseData)
        {
            ClickWebElement(".//*[@class='events__filter _type_time']", "Меню времени в фильтре", "меню времени в фильтре");
            ClickWebElement(String.Format(".//*[@class='events__filter-item']//*[text()='{0}']", chooseData), String.Format("Значение \"{0}\"", chooseData), String.Format("значения \"{0}\"", chooseData));
            IList<IWebElement> all = driver.FindElements(By.XPath(".//*[@class='table__time']"));
            foreach (IWebElement element in all)
            {
                string[] timeSplit = element.Text.Split(' ');

                if ((timeSplit.Length == 3) || (timeSplit.Length == 4))
                {
                    string[] hourSplit = timeSplit.Last().Split(':');
                    int[] numbers = hourSplit.Select(int.Parse).ToArray();
                    var timeSpan = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, numbers[0], numbers[1], 0) - DateTime.Now);
                    if (timeSpan.Minutes > timeValue) 
                        throw new Exception("Фильтры по времени не работают");
                }
                else
                {
                    throw new Exception("В массиве больше элементов чем должно быть");
                }
            }
        }
        // Метод проверяет что фаил из параметра скачался на компьютер
        protected static bool CheckFileDownloaded(string filename)
        {
            bool exist = false;
            string Path = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
            string[] filePaths = Directory.GetFiles(Path);
            foreach (string p in filePaths)
            {
                if (p.Contains(filename))
                {
                    exist = true;
                    File.Delete(p);
                    break;
                }
            }
            return exist;
        }
        //Метод заполняет форму запроса в зависимостиот типа и темы
        protected void FillAndCreateFormBuilder(int inputValue) // парметр это число из 2х цифр - 1ая цифра то номер строки меню при выборе типа запроса, 2ая цифра - номер строки меню при выборе темы запроса
        {
            LogStage("Заполнение и создание нового запроса");
            //qiwi, ,банковская карта, яндекс деньги , 5 все
            if (inputValue == 11 || inputValue == 12 || inputValue == 14 || inputValue == 5)
            {
                SendKeysToWebElement(".//*[@name='THEDESCRIPTIONOFTHEPROBLEM']", "Test", "Поле описания проблемы", "поля описания проблемы");
                SendKeysToWebElement(".//*[@name='AMOUNTRUB']", "2409", "Поле Суммы(руб)", "поля Суммы(руб)");
                SendKeysToWebElement(".//*[@class='ui__label']/input", "C:\\Users\\User\\Downloads\\саша.jpg","Поле Прикрепления файла", "поля Прикрепления файла");
                ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");
                return;
            }

            if (inputValue == 13)
            {
                SendKeysToWebElement(".//*[@name='AMOUNTRUB']", "2409", "Поле Суммы(руб)", "поля Суммы(руб)");
                SendKeysToWebElement(".//*[@name='PROBLEMDESCRIPTION']", "Test", "Поле описания проблемы", "поля описания проблемы"); //1 - мобильный телефон
                SendKeysToWebElement(".//*[@class='ui__label']/input", "C:\\Users\\User\\Downloads\\саша.jpg", "Поле Прикрепления файла", "поля Прикрепления файла");
                ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");
                return;
            }

            if (inputValue == 21)
            {
                SendKeysToWebElement(".//*[@name='QUESTION']", "Test", "Поле Вопрос", "поля Вопрос"); //2-1 вопрос по работе сайта 
                SendKeysToWebElement(".//*[@class='ui__label']/input", "C:\\Users\\User\\Downloads\\саша.jpg","Поле Прикрепления файла", "поля Прикрепления файла");
                ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");
                return;
            }

            if (inputValue == 22)
            {
                SendKeysToWebElement(".//*[@name='COMMENT']", "Test", "Поле Замечание", "поля Замечание"); //2-2 замечания и предложения
                SendKeysToWebElement(".//*[@name='SUGGESTION']", "Test", "Поле Предложение", "поля Предложение");
                SendKeysToWebElement(".//*[@class='ui__label']/input", "C:\\Users\\User\\Downloads\\саша.jpg","Поле Прикрепления файла", "поля Прикрепления файла");
                ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");
                return;
            }

            if (inputValue == 23)
            {
                SendKeysToWebElement(".//*[@name='TYPEOFAPPLICATION']", "Test", "Поле Тип приложения", "поля Тип приложения"); //2-3 вопрос по работе моб приложения
                SendKeysToWebElement(".//*[@name='QUESTION']", "Test", "Поле Вопрос", "поля Вопрос");
                SendKeysToWebElement(".//*[@class='ui__label']/input", "C:\\Users\\User\\Downloads\\саша.jpg", "Поле Прикрепления файла", "поля Прикрепления файла");
                ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");
                return;
            }

            if (inputValue == 31)
            {
                SendKeysToWebElement(".//*[@name='BETNUMBER']", "523", "Поле Номер ставки", "поля Номер ставки"); //3-1 пари Лайф
                ClickWebElement(".//*[@class='ui-datetime__actions']", "Иконка календарь", "иконки календаря");
                ClickWebElement(".//*[@class='ui-calendar__body']/tr[1]/td[1]", "Поле даты в календаре", "поля даты в календаре");
                SendKeysToWebElement(".//*[@name='QUESTION']", "Test Question", "Поле Содержание вопроса", "поля Содержание вопроса");
                SendKeysToWebElement(".//*[@name='REQUEST']", "Test Request", "Поле Просьба к администрации", "поля Просьба к администрации");
                ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");
                return;
            }

            if (inputValue == 32 || inputValue == 62)
            {
                SendKeysToWebElement(".//*[@name='QUESTION']", "Test Question", "Поле Содержание вопроса", "поля Содержание вопроса"); //3-2 пари 6-2
                SendKeysToWebElement(".//*[@name='REQUEST']", "Test Request", "Поле Просьба к администрации", "поля Просьба к администрации");
                ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");
                return;
            }

            if (inputValue == 41)
            {
                ClickWebElement(".//*[@class='ui-datetime__actions']", "Иконка календарь", "иконки календаря"); //4 повысить максимум
                ClickWebElement(".//*[@class='ui-calendar__body']/tr[1]/td[1]", "Поле даты в календаре", "поля даты в календаре");
                SendKeysToWebElement(".//*[@name='TOURNAMENTANDEVENT']", "Test tournament", "Поле Чемпионат и событие", "поля Чемпионат и событие");
                SendKeysToWebElement(".//*[@name='DESIREDSTAKE']", "12432", "Поле Сумма ставки", "поля Сумма ставки");
                SendKeysToWebElement(".//*[@name='COMMENTS']", "Test comment", "Поле Комментарий", "поля Комментарий");
                ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");
                return;
            }

            if (inputValue == 61)
            {
                SendKeysToWebElement(".//*[@name='BETNUMBER']", "523", "Поле Номер ставки", "поля Номер ставки"); //6-1
                ClickWebElement(".//*[@class='ui-datetime__actions']", "Иконка календарь", "иконки календаря");
                ClickWebElement(".//*[@class='ui-calendar__body']/tr[1]/td[1]", "Поле даты в календаре", "поля даты в календаре");
                SendKeysToWebElement(".//*[@name='OPERATORNUMBER']", "11", "Поле Номер оператора", "поля Номер оператора");
                SendKeysToWebElement(".//*[@name='QUESTION']", "Test Question", "Поле Содержание вопроса", "поля Содержание вопроса");
                SendKeysToWebElement(".//*[@name='REQUEST']", "Test Request", "Поле Просьба к администрации", "поля Просьба к администрации");
                ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");
                return;
            }

            if (inputValue == 71)
            {
                SendKeysToWebElement(".//*[@name='QUESTION']", "Test", "Поле Вопрос", "поля Вопрос"); //7-1
                ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");
                return;
            }

            if (inputValue == 72)
            {
                SendKeysToWebElement(".//*[@name='COMMENT']", "Test", "Поле Замечание", "поля Замечание"); //7-2 
                SendKeysToWebElement(".//*[@name='SUGGESTION']", "Test", "Поле Предложение", "поля Предложение");
                ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");
                return;
            }
        }
        //Метод создает новый запрос в зависимости от типа и темы 
        protected void CreateNewRequest(string firstClassValue, string firstError, string secondClassValue, string secondError)
        {
            LogStage("Создание нового запроса");
            string firstMenuValue = string.Format(".//*[@class='ui-dropdown__items']/div[{0}]", firstClassValue);
            string secondMenuValue = string.Format(".//*[@class='ui-dropdown__items']/div[{0}]", secondClassValue);
            string firstErrorValue = string.Format("Строка \"{0}\"", firstError);
            string firstErrorValueTwo = string.Format("строки \"{0}\"", firstError);
            string secondErrorValue = string.Format("Строка \"{0}\"", secondError);
            string secondErrorValueTwo = string.Format("cтроки \"{0}\"", secondError);

            ClickWebElement(".//*[@class='toolbar__item _left']", "Кнопка Новый запрос", "кнопки Новый запрос");
            ClickWebElement(".//*[@class='ui__field-inner']", "Меню Тип запроса", "меню тип запроса");
            ClickWebElement(firstMenuValue, firstErrorValue, firstErrorValueTwo);
            ClickWebElement(".//*[@class='account-form__window _icon_img']//label[2]", "Меню Тема запроса", "меню Тема запроса");
            ClickWebElement(secondMenuValue, secondErrorValue, secondErrorValueTwo);
            ClickWebElement(".//*[@class='toolbar__item account-form__button']/a/div", "Кнопка Подтвердить", "кнопки Подтвердить");
            if (!WebElementExist(".//*[@class='account-form__message _kind-error _style-box']"))
                throw new Exception("В форме нет обязательных полей");
        }

        //Метод проверяет все фильтры при работе с запросом
        protected void CheckRequestFilter(string requestName)
        {
            LogStage("Проверка сообщения о создании заявки");
            if (!WebElementExist(".//*[@class='account-form__message _kind-top-notice _style-box']"))
                throw new Exception("Нет сообщения о создании заявки");
            IWebElement createResult = GetWebElement(".//*[@class='account-form__message _kind-top-notice _style-box']", "Нет сообщения о создании заявки");
            string createResultText = createResult.Text;
            var createResultTextConvert = Regex.Replace(createResultText, @"[^\d]+", ""); // Вычленение номера заявки из общего сообщения о создании заявки
            ClickWebElement(".//*[@class='toolbar__item account-form__button']/a//span", "Кнопка Закрыть", "кнопки Закрыть");

            LogStage("Проверка работы фильтра по статусу");
            ClickWebElement(".//*[@class='account-requests__form']//*[@class='ui__label']", "Кнопка разворота меню фильтра", "кнопки разворота меню фильтра");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[2]", "Строка Отвеченный", "строки Отвеченный");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[3]", "Строка В Облработке", "строки В Облработке");
            ClickWebElement(".//*[@class='ui__field-inner']//*[@class='toolbar__icon _caret-up']", "Стрелка фильтра по статусу", "стрелки фильтра по статусу");
            IList<IWebElement> gridNumber = driver.FindElements(By.XPath(".//*[@class='wrap']")); //все строки
            if (gridNumber.Count < 2)
                throw new Exception("Не работает фильтр по статусу");

            LogStage("Проверка работы фильтра по номеру");
            ClickWebElement(".//*[@class='account-requests__form']//*[@class='ui__label']", "Кнопка разворота меню фильтра", "кнопки разворота меню фильтра");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[2]", "Строка Отвеченный", "строки Отвеченный");
            ClickWebElement(".//*[@class='ui-dropdown__items']/div[3]", "Строка В Обработке", "строки В Обработке");
            SendKeysToWebElement(".//*[@class='account-requests__form']//*[@class='ui__field-inner']//input", createResultTextConvert, "Поле Номер запроса", "поля Номер запроса");
            IList<IWebElement> myRequest = driver.FindElements(By.XPath(".//*[@class='wrap']/div/div[1]"));
            if (myRequest.Count != 1)
                throw new Exception("Есть два одинаковых номера запроса");
            IWebElement numberCell = GetWebElement(".//*[@class='wrap']/div/div[1]", "Нет поля номера заявки");
            string numberCellText = numberCell.Text;
            if (createResultTextConvert != numberCellText)
                throw new Exception("Не работает фильтр по номеру");
            ClickWebElement(".//*[@class='toolbar__icon icon _clear']", "Кнопка Очистить фильтр по номеру", "кнопки Очистить фильтр по номеру");

            LogStage("Закрытие готовой заявки");
            ClickWebElement(".//*[@class='requests-list__data']/div[1]", "Строка с последней созданной заявкой", "строки с последней созданной заявкой");
            ClickWebElement(".//*[@class='request-details']//*[@class='toolbar__item']", "Кнопка закрыть заявку", "кнопки закрыть заявку");
            IWebElement requestCell = GetWebElement(".//*[@class='requests-list__data']/div[1]/div", "Нет строк с заявками");
            var requestCellClass = requestCell.GetAttribute("class");
            if (requestCellClass.Contains("new"))
                throw new Exception("Не работает закрытие заявки");
            IWebElement requestStatus = GetWebElement(".//*[@class='requests-list__data']/div[1]//*[@class='column column-4']", "Нет колонки Статус");
            string requestStatusText = requestStatus.Text;
            if (requestStatusText != "Отвеченный")
                throw new Exception("Не поменялся статус заявки после ее закрытия");

            LogStage("Проверка переоткрытия заявки");
            ClickWebElement(".//*[@class='requests-list__data']/div[1]", "Строка с последней созданной заявкой", "строки с последней созданной заявкой");
            SendKeysToWebElement(".//*[@class='ui__field _message']", "Test", "Поле для ввода нового сообщения", "поля для ввода нового сообщения");
            ClickWebElement(".//*[@class='request-details__form-wrap']//button", "Кнопка отправки нового сообщения", "кнопки отправки нового сообщения");
            IWebElement newStatus = GetWebElement(".//*[@class='requests-list__data']/div[1]//*[@class='column column-4']", "Нет колонки Статус");
            string newStatussText = newStatus.Text;
            if (newStatussText != "Неотвеченный")
                throw new Exception("Не поменялся статус заявки после ее переоткрытия");
            IWebElement theme = GetWebElement(".//*[@class='requests-list__data']/div[1]//*[@class='column column-3']", "Нет колонки Тема");
            string themeText = theme.Text;
            if (!themeText.Contains(requestName))
                throw new Exception("Тема не связана с "+ requestName + "");

            LogStage("Проверка что фаил скачивается из заявки");
            ClickWebElement(".//*[@class='requests-list__data']/div[1]", "Строка с последней созданной заявкой", "строки с последней созданной заявкой");
            ClickWebElement(".//*[@class='request-details']//*[@class='toolbar__item']", "Кнопка закрыть заявку", "кнопки закрыть заявку");
            ClickWebElement(".//*[@class='request-details__request-file']/span", "Кнопка скачать прикрепленный фаил", "кнопки скачать прикрепленный фаил");
            if (CheckFileDownloaded("rccimg_0000000011_d180d396.jpg") == false)
                throw new Exception("Фаил из заявки не скачался");
        }
        // Метод принимает кол-во отмеченных событий в суперэкспрессе
        protected void MarkedBoxCounter(int value, string mark)
        {
            IList<IWebElement> all = driver.FindElements(By.XPath(String.Format(".//*[@class='matrix-form__mark-box{0}']", mark)));
            if (all.Count != value)
            {
                throw new Exception("Что-то не так с выбором/очисткой полей возможных вариантов");
            }
        }

        // Метод выбирает два исхода в 1ой строчке, независимо от того какого уже там событие выбрано
        protected void ChooseTwoResults()
        {
            if (driver.FindElement(By.XPath(".//*[@class='bet-table']//tr[2]//td[4]/div")).GetAttribute("class")
                .Equals("matrix-form__mark-box"))
            {
                driver.FindElement(By.XPath(".//*[@class='bet-table']//tr[2]//td[4]/div")).Click();
            }
            else if (driver.FindElement(By.XPath(".//*[@class='bet-table']//tr[2]//td[5]/div")).GetAttribute("class")
                .Equals("matrix-form__mark-box"))
            {
                driver.FindElement(By.XPath(".//*[@class='bet-table']//tr[2]//td[5]/div")).Click();
            }
            else
            {
                driver.FindElement(By.XPath(".//*[@class='bet-table']//tr[2]//td[6]/div")).Click();
            }
        }
          // Метод принимает на вход  ожидаемый номер ошибки и нномер телефона и проверяет правильность работы функции восстановления пароля по тестовому сценарию на тестовых данных
        protected void RejectPwdChecker(string rejectValue, string phoneValue)
        {
            string errorText = "";
            if (rejectValue == "12")
                errorText = "Исчерпано количество попыток восстановления пароля";
            if (rejectValue == "10")
                errorText = "Клиент с указанными данными не найден";
            if (rejectValue == "4")
                errorText = "Предыдущий процесс не завершён";
            if (rejectValue == "1")
                errorText = "В процессе регистрации произошла неожиданная ошибка";

            LogStage("Переход на страницу восстановление пароля");
            ClickWebElement(".//*[@href='/#!/account/restore-password']", "Кнопка Забыли пароль", "кнопки забыли пароль");

            LogStage("Проверка createProcessWithCaptcha по тестовому сценарию на rejected " + rejectValue + "");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[2]//input", phoneValue , "Поле номер телефона", "поля номер телефона");
            SendKeysToWebElement(".//*[@class='change-password__form-inner']/div/div[3]//input", "11", "Поле капча", "поля капчи");
            ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки Отправить");
            var errorMessage = GetWebElement(".//*[@class='account-error__text']", "Нет текста ошибки");
            if (!errorMessage.Text.Contains(errorText))
                throw new Exception("Неверный код ошибки");
            ClickWebElement(".//*[@class='account__heading-close']", "Крестик Закрыть окно", "Крестика Закрыть оно");
        }

        protected void VerificationStatusCheck()
        {
            ClickOnAccount();
            LogStage("Проверка на статус верификации");
            if (!WebElementExist(".//*[@class='verification__notice-types-wrap']"))
            {
                LogStage("Переход в админку");
                driver.Navigate().GoToUrl("http://fonbackoffice.dvt24.com/");

                LogStage("Логирование в админке");
                SendKeysToWebElement(".//*[@id='username']", "mashkov", "Поле Пользователь", "поля Пользователь");
                SendKeysToWebElement(".//*[@id='password']", "ueueue11", "Поле Пароль", "поля Пароль");
                ClickWebElement(".//*[@class='login__btn']", "Кнопка Вход", "кнопки Вход");

                LogStage("Проверка аккаунта на верификацию");
                ClickWebElement(".//*[@class='home__items']//*[@href='#/clientManager']", "Меню Поиск клиентов",
                    "меню Поиск клиентов");
                SendKeysToWebElement(".//*[@class='clients__fields']/label//input", "13", "Поле Идентификатор клиента",
                    "поля Идентификатор клиента");
                ClickWebElement(".//*[@class='clients__btn-inner']//button", "Кнпока Найти", "кнопки Найти");
                ClickWebElement(".//*[@class='tabs__head tabs__slider']/span/a[2]", "Вкладка Расширенная информация",
                    "вкладки Расширенная информация");
                ClickWebElement(".//*[@class='form__col-wide']/div[1]//i", "Иконка Редактировать",
                    "иконки Редактировать");
                IList<IWebElement> status = driver.FindElements(By.XPath(".//*[@class='form__col-wide']/form//input"));
                foreach (IWebElement element in status)
                {
                    string statusClass = element.GetAttribute("class");

                    if (statusClass.Contains("checked"))
                        element.Click();
                }
                SendKeysToWebElement(".//*[@class='ui__field-inner']/textarea", "autotest", "Поле Комментарий",
                    "поля Комментарий");
                ClickWebElement(".//*[text()='Сохранить']", "Кнопка Сохранить", "кнопки Сохранить");
                driver.Navigate().GoToUrl("http://fonred5051.dvt24.com/");
                ClickOnAccount();
            }
        }
        // Метод принимает на вход  ожидаемый номер ошибки и почту и проверяет правильность работы функции подтверждения email по тестовому сценарию на тестовых данных
        protected void CreateProcessemailChecker(string rejectValue, string emailValue)
        {
            string errorText = "";
            if (rejectValue == "14")
                errorText = "Исчерпан лимит на количество изменений адреса электронной почты";
            if (rejectValue == "13")
                errorText = "Адрес электронной почты уже подтверждён";
            if (rejectValue == "12")
                errorText = "Адрес электронной почты занят";

            LogStage("Проверка creatProcess по тестовому сценарию на rejected " + rejectValue + "");
            driver.FindElement(By.XPath(".//*[@class='ui__field-inner']/input")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input", emailValue, "Поле email", "поля email");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
            var errorMessage = GetWebElement(".//*[@class='account-error__text']", "Нет текста ошибки");
            if (!errorMessage.Text.Contains(errorText))
                throw new Exception("Неверный текст ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");
        }

        // Метод принимает на вход  ожидаемый номер ошибки и почту и проверяет правильность работы функции подтверждения email по тестовому сценарию на тестовых данных
        protected void SendEmailCodeChecker(string value, string emailValue)
        {
            string errorText = "";
            if (value == "10")
                errorText = "Неправильный код подтверждения";
            if (value == "1")
                errorText = "Внутренняя ошибка";
            driver.FindElement(By.XPath(".//*[@class='ui__field-inner']/input")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input", emailValue, "Поле email", "поля email");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки отправить");
            var errorMessage = GetWebElement(".//*[@class='account-error__text']", "Нет текста ошибки");
            if (!errorMessage.Text.Contains(errorText))
                throw new Exception("Неверный текст ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");
        }

        protected void FillRegistrationForm()
        {
        ClickWebElement(".//*[@href='/#!/account/registration/Reg4']", "Кнопка Регистрации", "кнопки Регистрации");
        SendKeysToWebElement(".//*[@class='registration-v4__step-wrap']/div[2]/label[1]//input", "Это", "Поле Фамилия", "поля Фамилия");
        SendKeysToWebElement(".//*[@class='registration-v4__step-wrap']/div[2]/label[2]//input", "Тест", "Поле Имя", "поля Имя");
        SendKeysToWebElement(".//*[@class='registration-v4__step-wrap']/div[6]/label[1]//input", "123qwe123", "Поле Пароль", "поля Пароль");
        SendKeysToWebElement(".//*[@class='registration-v4__step-wrap']/div[6]/label[2]//input", "123qwe123", "Поле Подтверждение пароля", "поля Подтверждение пароля");
        SendKeysToWebElement(".//*[@class='registration-v4__step-wrap']/div[4]/label[1]//input", "4000100@mail.ru", "Поле email", "поля email");
        SendKeysToWebElement(".//*[@class='registration-v4__step-wrap']/div[4]/label[2]//input", "000000008", "Поле Телефон", "поля Телефон");
        ClickWebElement(".//*[@id='checkbox2']", "Чекбокс Подтверждения правил", "чекбокса Подтверждения правил");
    }

        // Метод принимает на вход  ожидаемый номер телефона и  проверяет правильность работы createProcess по тестовому сценарию на тестовых данных для супер-регистрации
        protected void CreateProcessRegistration(string phoneValue, string process, string code, string rejcode)
        {
            if (phoneValue == "000000003")
            {
                //FillRegistrationForm();
                //driver.Navigate().GoToUrl("http://fonred5000.dvt24.com/?test=1#!/account/registration/Reg4");
                //WaitForElementByXpath(".//*[@class='registration-v4__step-wrap']/div[4]/label[2]//input");
                driver.FindElement(By.XPath(".//*[@class='registration-v4__step-wrap']/div[4]/label[2]//input")).Clear();
                SendKeysToWebElement(".//*[@class='registration-v4__step-wrap']/div[4]/label[2]//input", phoneValue, "Поле Номер телефона", "поля Номер телефона");
                ClickWebElement(".//*[@class='registration-v4__form-row _form-buttons']//button", "Кпонка Продолжить", "кпонки Продолжить");
                if (!WebElementExist(".//*[@class='registration-v4__captcha-wrap']"))
                    throw new Exception("Нет капчи");
                ClickWebElement(".//*[@class='account__heading-close']", "Крестик окна", "крестика окна");
                return;
            }

            FillRegistrationForm();
            // driver.Navigate().GoToUrl("http://fonred5000.dvt24.com/?test=1#!/account/registration/Reg4");
            driver.FindElement(By.XPath(".//*[@class='registration-v4__step-wrap']/div[4]/label[2]//input")).Clear();
            SendKeysToWebElement(".//*[@class='registration-v4__step-wrap']/div[4]/label[2]//input", phoneValue, "Поле Номер телефона", "поля Номер телефона");
            ClickWebElement(".//*[@class='registration-v4__form-row _form-buttons']//button", "Кпонка Продолжить", "кпонки Продолжить");
            var errorMessage = GetWebElement(".//*[@id='registration-v4-error']", "Нет модуля с ошибкой");
            if (phoneValue == "000000002")
            {
                if (!(errorMessage.GetAttribute("data-errorcode").Equals(code)))
                    throw new Exception("Неверная обработка ошибки");
               
            }
            else
            if (phoneValue == "000000004")
            {
                if (!(errorMessage.GetAttribute("data-errorcode").Equals(code) && errorMessage.GetAttribute("data-processstate").Equals(process)))
                    throw new Exception("Неверная обработка ошибки");
            }
           else  if (!(errorMessage.GetAttribute("data-errorcode").Equals(code) && errorMessage.GetAttribute("data-processstate").Equals(process) && errorMessage.GetAttribute("data-rejectioncode").Equals(rejcode)))
                throw new Exception("Неверная обработка ошибки");
            
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Закрыть", "кнопки Закрыть");
        }

        // Метод принимает на вход смс код с  телефона и  проверяет правильность работы SendSmsCode по тестовому сценарию на тестовых данных для супер-регистрации
        protected void SendSmsCodeRegistration(string smsValue, string process, string code, string rejcode)
        {
            if (smsValue == "8")
            {
                LogStage("Проверка sendSmsCode по тестовому сценарию");
               
                if (!WebElementExist(".//*[@class='registration-v4__form-inner']/div[2]//input"))
                {
                    FillRegistrationForm();
                   // driver.Navigate().GoToUrl("http://fonred5000.dvt24.com/?test=1#!/account/registration/Reg4");
                    ClickWebElement(".//*[@class='registration-v4__form-row _form-buttons']//button", "Кпонка Продолжить", "кпонки Продолжить");
                }
               
                SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", smsValue, "Поле СМС код", "поля СМС код");
                ClickWebElement(".//*[@class='toolbar__item process-button']/button", "Кнопка Отправить", "Кнопка Отправить");
               

                if (!driver.FindElement(By.XPath(".//*[@class='registration-v4__verification-info']")).Text.Contains("Qiwi"))
                    throw new Exception("Смс не ведет на киви верификацию");
                LogStage("Проверка перехода в личный кабинет");
                ClickWebElement(".//*[@class='registration-v4__form-inner']//*[@class='toolbar__item']//span", "Кнопка Перейти в личный кабинет", "кнопки Перейти в личный кабинет");
                if (!WebElementExist(".//*[@class='confirm__inner---LYRu']"))
                    throw new Exception("Непоявилось подтверждение");
                ClickWebElement(".//*[@class='confirm__inner---LYRu']/div[3]/div[2]/a", "Кнопка Ок", "кнопки Ок");
                if(!driver.FindElement(By.XPath(".//*[@class='account-menu__wrap']/a[1]")).GetAttribute("class").Contains("active"))
                    throw new Exception("Не перешел в мой профиль");
                if (!WebElementExist(".//*[@class='verification__notice-item'][text()='Продолжить идентификацию']"))
                    throw new Exception("Нет кнопки продолжить идентификацию из ЛК");
                ClickWebElement(".//*[@class='page-account']//*[@class='account-menu__icon _exit']", "Кнопка Выход", "кнопки Выход");

                sendPasportRegistrationQiwi("22222", null, "2", null);
                sendPasportRegistrationQiwi("33333", "waitForPassport", "10", null);
                sendPasportRegistrationQiwi("44444", "rejected", "0", "17");
                sendPasportRegistrationQiwi("55555", "rejected", "0", "1");
                sendPasportRegistrationQiwi("66666", null, null, null);
                return;
            }

            if (smsValue == "9")
            {
                LogStage("Проверка sendSmsCode по тестовому сценарию");
                
                if (!WebElementExist(".//*[@class='registration-v4__form-inner']/div[2]//input"))
                {
                    FillRegistrationForm();
                    //driver.Navigate().GoToUrl("http://fonred5000.dvt24.com/?test=1#!/account/registration/Reg4");
                    ClickWebElement(".//*[@class='registration-v4__form-row _form-buttons']//button", "Кпонка Продолжить", "кпонки Продолжить");
                }
                driver.FindElement(By.XPath(".//*[@class='registration-v4__form-inner']/div[2]//input")).Clear();
                SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", smsValue, "Поле СМС код", "поля СМС код");
                ClickWebElement(".//*[@class='toolbar__item process-button']/button", "Кнопка Отправить", "Кнопка Отправить");


                if (driver.FindElement(By.XPath(".//*[@class='registration-v4__verification-info']")).Text.Contains("Qiwi"))
                    throw new Exception("Смс ведет на киви верификацию");
                LogStage("Проверка перехода в личный кабинет");
                ClickWebElement(".//*[@class='registration-v4__form-inner']//*[@class='toolbar__item']//span", "Кнопка Перейти в личный кабинет", "кнопки Перейти в личный кабинет");
                if (!WebElementExist(".//*[@class='confirm__inner---LYRu']"))
                    throw new Exception("Непоявилось подтверждение");
                ClickWebElement(".//*[@class='confirm__inner---LYRu']/div[3]/div[2]/a", "Кнопка Ок", "кнопки Ок");
                if (!driver.FindElement(By.XPath(".//*[@class='account-menu__wrap']/a[1]")).GetAttribute("class").Contains("active"))
                    throw new Exception("Не перешел в мой профиль");
                if (!WebElementExist(".//*[@class='verification__notice-item'][text()='Продолжить идентификацию']"))
                    throw new Exception("Нет кнопки продолжить идентификацию из ЛК");
                ClickWebElement(".//*[@class='page-account']//*[@class='account-menu__icon _exit']", "Кнопка Выход", "кнопки Выход");

                sendPasportRegistrationBk("22222", "waitForPassport", "10", null);
                sendPasportRegistrationBk("33333", "rejected", "0", "18");
                sendPasportRegistrationBk("66666", null, null, null);
                return;
            }

            if (smsValue == "0")
            {
                if (!WebElementExist(".//*[@class='registration-v4__form-inner']/div[2]//input"))
                {
                    FillRegistrationForm();
                    //driver.Navigate().GoToUrl("http://fonred5000.dvt24.com/?test=1#!/account/registration/Reg4");
                    ClickWebElement(".//*[@class='registration-v4__form-row _form-buttons']//button", "Кпонка Продолжить", "кпонки Продолжить");
                }
                driver.FindElement(By.XPath(".//*[@class='registration-v4__form-inner']/div[2]//input")).Clear();
                SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", smsValue, "Поле СМС код", "поля СМС код");
                ClickWebElement(".//*[@class='toolbar__item process-button']/button", "Кнопка Отправить", "Кнопка Отправить");
                if (!WebElementExist(".//*[@class='account-error _type_success']"))
                    throw new Exception("Верификация  не прошла");
                ClickWebElement(".//*[@class='account__heading-title']/a", "Крестик формы регистрации", "крестика формы регистрации");
                return;
            }
                

            LogStage("Проверка sendSmsCode по тестовому сценарию");
            if (!WebElementExist(".//*[@class='registration-v4__form-inner']/div[2]//input"))
            {
                FillRegistrationForm();
                // driver.Navigate().GoToUrl("http://fonred5000.dvt24.com/?test=1#!/account/registration/Reg4");
                ClickWebElement(".//*[@class='registration-v4__form-row _form-buttons']//button", "Кпонка Продолжить", "кпонки Продолжить");
            }
            driver.FindElement(By.XPath(".//*[@class='registration-v4__form-inner']/div[2]//input")).Clear();
            SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", smsValue, "Поле СМС код", "поля СМС код");
            ClickWebElement(".//*[@class='toolbar__item process-button']/button", "Кнопка Отправить", "Кнопка Отправить");


            var messageData = GetWebElement(".//*[@id='registration-v4-error']", "Нет модуля с ошибкой");
            if (smsValue == "1" || smsValue == "2")
            {
                if (!(messageData.GetAttribute("data-errorcode").Equals(code) && messageData.GetAttribute("data-processstate").Equals(process)))
                    throw new Exception("Неверная обработка ошибки");
            }
            else 
            {
                if (!(messageData.GetAttribute("data-errorcode").Equals(code) && messageData.GetAttribute("data-processstate").Equals(process) && messageData.GetAttribute("data-rejectioncode").Equals(rejcode)))
                    throw new Exception("Неверная обработка ошибки");
            }

            if (smsValue != "0")
                ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Закрыть/Повторить", "кнопки Закрыть/Повторить");
            if (smsValue == "5" || smsValue == "6" || smsValue == "7")
            {
               
                var body = GetWebElement(".//body", "Нет тела");
                if (!body.GetAttribute("class").Contains("restorePassword"))
                    throw new Exception("Не перешли на восстановление пароля");
                ClickWebElement(".//*[@class='account__heading-title']/a", "Крестик формы восстановления пароля", "крестика формы восстановления пароля");
            }

        }

        protected void sendPasportRegistrationQiwi(string pasportnumber, string process, string code, string rejcode)
        {
            
            if (pasportnumber == "22222")
            {
                // driver.Navigate().GoToUrl("http://fonred5000.dvt24.com/?test=1#!/account/registration/Reg4");
                FillRegistrationForm();
                ClickWebElement(".//*[@class='registration-v4__form-row _form-buttons']//button", "Кпонка Продолжить","кпонки Продолжить");
                SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", "8", "Поле СМС код","поля СМС код");
                ClickWebElement(".//*[@class='toolbar__item process-button']/button", "Кнопка Отправить", "Кнопка Отправить");
                LogStage("Проверка sendPasport qiwi по тестовому сценарию");
                SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", pasportnumber, "Поле Серия и номер","поля Серия и номер");
                SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[3]//input", "01012000", "Поле Дата выдачи","поля Дата выдачи");
                ClickWebElement(".//*[@id='checkbox2']", "Чекбокс Подтверждения правил", "чекбокса Подтверждения правил");
                ClickWebElement(".//*[@class='toolbar__item process-button']", "Кнопка Отправить данные по киви","кнопки Отправить данные по киви");
                var messageData = GetWebElement(".//*[@id='verification-qiwi-error']", "Нет модуля с ошибкой");
                if (!(messageData.GetAttribute("data-errorcode").Equals(code)))
                    throw new Exception("Неверная обработка ошибки");
                ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Закрыть/Повторить", "кнопки Закрыть/Повторить");
                return;
            }
            if (pasportnumber == "33333")
            {
                LogStage("Проверка sendPasport qiwi по тестовому сценарию");
                driver.FindElement(By.XPath(".//*[@class='registration-v4__form-inner']/div[2]//input")).Clear();
                SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", pasportnumber, "Поле Серия и номер", "поля Серия и номер");    
                ClickWebElement(".//*[@class='toolbar__item process-button']", "Кнопка Отправить данные по киви", "кнопки Отправить данные по киви");
                var messageData = GetWebElement(".//*[@id='verification-qiwi-error']", "Нет модуля с ошибкой");
                if (!(messageData.GetAttribute("data-errorcode").Equals(code) && messageData.GetAttribute("data-processstate").Equals(process)))
                throw new Exception("Неверная обработка ошибки");
                ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Закрыть/Повторить", "кнопки Закрыть/Повторить");
                return;
            }
            if (pasportnumber == "44444")
            {
                LogStage("Проверка sendPasport qiwi по тестовому сценарию");
                driver.FindElement(By.XPath(".//*[@class='registration-v4__form-inner']/div[2]//input")).Clear();
                SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", pasportnumber, "Поле Серия и номер", "поля Серия и номер");
                ClickWebElement(".//*[@class='toolbar__item process-button']", "Кнопка Отправить данные по киви", "кнопки Отправить данные по киви");
                var messageData = GetWebElement(".//*[@id='verification-qiwi-error']", "Нет модуля с ошибкой");
                if (!(messageData.GetAttribute("data-errorcode").Equals(code) && messageData.GetAttribute("data-processstate").Equals(process) && messageData.GetAttribute("data-rejectioncode").Equals(rejcode)))
                    throw new Exception("Неверная обработка ошибки");
                ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Закрыть/Повторить", "кнопки Закрыть/Повторить");
                if (!driver.FindElement(By.XPath(".//*[@class='account-menu__wrap']/a[1]")).GetAttribute("class").Contains("active"))
                    throw new Exception("Не перешел в мой профиль");
                if (!WebElementExist(".//*[@class='verification__notice-item'][text()='Продолжить идентификацию']"))
                    throw new Exception("Нет кнопки продолжить идентификацию из ЛК");
                ClickWebElement(".//*[@class='page-account']//*[@class='account-menu__icon _exit']", "Кнопка Выход", "кнопки Выход");
                return;
            }
            if (pasportnumber == "55555")
            {
                FillRegistrationForm();
                ClickWebElement(".//*[@class='registration-v4__form-row _form-buttons']//button", "Кпонка Продолжить", "кпонки Продолжить");
                SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", "8", "Поле СМС код", "поля СМС код");
                ClickWebElement(".//*[@class='toolbar__item process-button']/button", "Кнопка Отправить", "Кнопка Отправить");
                LogStage("Проверка sendPasport qiwi по тестовому сценарию");
                SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", pasportnumber, "Поле Серия и номер", "поля Серия и номер");
                SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[3]//input", "01012000", "Поле Дата выдачи", "поля Дата выдачи");
                ClickWebElement(".//*[@id='checkbox2']", "Чекбокс Подтверждения правил", "чекбокса Подтверждения правил");
                ClickWebElement(".//*[@class='toolbar__item process-button']", "Кнопка Отправить данные по киви", "кнопки Отправить данные по киви");
                var messageData = GetWebElement(".//*[@id='verification-qiwi-error']", "Нет модуля с ошибкой");
                if (!(messageData.GetAttribute("data-errorcode").Equals(code) && messageData.GetAttribute("data-processstate").Equals(process) && messageData.GetAttribute("data-rejectioncode").Equals(rejcode)))
                    throw new Exception("Неверная обработка ошибки");
                ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Закрыть/Повторить", "кнопки Закрыть/Повторить");
                if (!driver.FindElement(By.XPath(".//*[@class='account-menu__wrap']/a[1]")).GetAttribute("class").Contains("active"))
                    throw new Exception("Не перешел в мой профиль");
                if (!WebElementExist(".//*[@class='verification__notice-item'][text()='Продолжить идентификацию']"))
                    throw new Exception("Нет кнопки продолжить идентификацию из ЛК");
                ClickWebElement(".//*[@class='page-account']//*[@class='account-menu__icon _exit']", "Кнопка Выход", "кнопки Выход");
                return;
            }

            FillRegistrationForm();
            ClickWebElement(".//*[@class='registration-v4__form-row _form-buttons']//button", "Кпонка Продолжить", "кпонки Продолжить");
            SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", "8", "Поле СМС код", "поля СМС код");
            ClickWebElement(".//*[@class='toolbar__item process-button']/button", "Кнопка Отправить", "Кнопка Отправить");
            LogStage("Проверка sendPasport qiwi по тестовому сценарию");
            SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", "66666", "Поле Серия и номер", "поля Серия и номер");
            SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[3]//input", "01012000", "Поле Дата выдачи", "поля Дата выдачи");
            ClickWebElement(".//*[@id='checkbox2']", "Чекбокс Подтверждения правил", "чекбокса Подтверждения правил");
            ClickWebElement(".//*[@class='toolbar__item process-button']", "Кнопка Отправить данные по киви", "кнопки Отправить данные по киви");
            if (!WebElementExist(".//*[@class='account-error _type_success']"))
                throw new Exception("Верификация по киви не прошла");
            ClickWebElement(".//*[@class='account-error__actions-inner']//span", "Кнопка Закрыть/Повторить", "кнопки Закрыть/Повторить");
            if (!driver.FindElement(By.XPath(".//*[@class='account-menu__wrap']/a[1]")).GetAttribute("class").Contains("active"))
                throw new Exception("Не перешел в мой профиль");
            if (!WebElementExist(".//*[@class='verification__notice-item'][text()='Продолжить идентификацию']"))
                throw new Exception("Нет кнопки продолжить идентификацию из ЛК");
            ClickWebElement(".//*[@class='page-account']//*[@class='account-menu__icon _exit']", "Кнопка Выход", "кнопки Выход");


        }
        private void sendPasportRegistrationBk(string pasportnumber, string process, string code, string rejcode)
        {

            if (pasportnumber == "22222")
            {
                LogStage("Проверка sendPasport бк по тестовому сценарию");
                // driver.Navigate().GoToUrl("http://fonred5000.dvt24.com/?test=1#!/account/registration/Reg4");
                FillRegistrationForm();
                ClickWebElement(".//*[@class='registration-v4__form-row _form-buttons']//button", "Кпонка Продолжить", "кпонки Продолжить");
                SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", "9", "Поле СМС код", "поля СМС код");
                ClickWebElement(".//*[@class='toolbar__item process-button']/button", "Кнопка Отправить", "Кнопка Отправить");
                LogStage("Проверка sendPasport qiwi по тестовому сценарию");
                SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", pasportnumber, "Поле Серия и номер", "поля Серия и номер");
                SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]/label[2]//input", "01012000", "Поле Дата выдачи", "поля Дата выдачи");
                ClickWebElement(".//*[@id='checkbox2']", "Чекбокс Подтверждения правил", "чекбокса Подтверждения правил");
                ClickWebElement(".//*[@class='toolbar__item process-button']", "Кнопка Отправить данные по бк", "кнопки Отправить данные по бк");
                var messageData = GetWebElement(".//*[@id='verification-bk-error']", "Нет модуля с ошибкой");
                if (!(messageData.GetAttribute("data-errorcode").Equals(code) && messageData.GetAttribute("data-processstate").Equals(process)))
                    throw new Exception("Неверная обработка ошибки");
                ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Закрыть/Повторить", "кнопки Закрыть/Повторить");
                return;
            }
            if (pasportnumber == "33333")
            {
                LogStage("Проверка sendPasport бк по тестовому сценарию");
                driver.FindElement(By.XPath(".//*[@class='registration-v4__form-inner']/div[2]//input")).Clear();
                SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", pasportnumber, "Поле Серия и номер", "поля Серия и номер");
                ClickWebElement(".//*[@class='toolbar__item process-button']", "Кнопка Отправить данные по бк", "кнопки Отправить данные по бк");
                var messageData = GetWebElement(".//*[@id='verification-bk-error']", "Нет модуля с ошибкой");
                if (!(messageData.GetAttribute("data-errorcode").Equals(code) && messageData.GetAttribute("data-processstate").Equals(process) && messageData.GetAttribute("data-rejectioncode").Equals(rejcode)))
                    throw new Exception("Неверная обработка ошибки");
                ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Закрыть/Повторить", "кнопки Закрыть/Повторить");
                if (!driver.FindElement(By.XPath(".//*[@class='account-menu__wrap']/a[1]")).GetAttribute("class").Contains("active"))
                    throw new Exception("Не перешел в мой профиль");
                if (!WebElementExist(".//*[@class='verification__notice-item'][text()='Продолжить идентификацию']"))
                    throw new Exception("Нет кнопки продолжить идентификацию из ЛК");
                ClickWebElement(".//*[@class='page-account']//*[@class='account-menu__icon _exit']", "Кнопка Выход", "кнопки Выход");
                return;
            }
            FillRegistrationForm();
            ClickWebElement(".//*[@class='registration-v4__form-row _form-buttons']//button", "Кпонка Продолжить", "кпонки Продолжить");
            SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", "9", "Поле СМС код", "поля СМС код");
            ClickWebElement(".//*[@class='toolbar__item process-button']/button", "Кнопка Отправить", "Кнопка Отправить");
            LogStage("Проверка sendPasport бк по тестовому сценарию");
            SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]//input", "66666", "Поле Серия и номер", "поля Серия и номер");
            SendKeysToWebElement(".//*[@class='registration-v4__form-inner']/div[2]/label[2]//input", "01012000", "Поле Дата выдачи", "поля Дата выдачи");
            ClickWebElement(".//*[@id='checkbox2']", "Чекбокс Подтверждения правил", "чекбокса Подтверждения правил");
            ClickWebElement(".//*[@class='toolbar__item process-button']", "Кнопка Отправить данные по бк", "кнопки Отправить данные по бк");
            if (!WebElementExist(".//*[@class='account-error _type_success']"))
                throw new Exception("Верификация по бк не прошла");
            ClickWebElement(".//*[@class='account-error__actions-inner']//span", "Кнопка Закрыть/Повторить", "кнопки Закрыть/Повторить");
            if (!driver.FindElement(By.XPath(".//*[@class='account-menu__wrap']/a[1]")).GetAttribute("class").Contains("active"))
                throw new Exception("Не перешел в мой профиль");
            if (WebElementExist(".//*[@class='verification__notice-types-wrap']/a[2]"))
                throw new Exception("Нет кнопки отменить процесс");
            ClickWebElement(".//*[@class='page-account']//*[@class='account-menu__icon _exit']", "Кнопка Выход", "кнопки Выход");

        }

        // Метод принимает на вход  ожидаемый номер телефона и  проверяет правильность работы createProcess по тестовому сценарию на тестовых данных для идентификации bk
        protected void CreateProcessVerificationBk(string phoneValue, string process, string code, string rejcode)
        {
            if (phoneValue == "7")
            {
                LogStage("Проверка createProcess по тестовому сценарию");
                driver.FindElement(By.XPath(".//*[@class='verification__form-inner']/div/div[2]//input")).SendKeys(Keys.Backspace);
                SendKeysToWebElement(".//*[@class='verification__form-inner']/div/div[2]//input", phoneValue, "Поле Номера карты фонбет", "поля Номера карты фонбет");
                ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
                var message = GetWebElement(".//*[@id='verification-bk-error']", "Нет модуля с ошибкой");
                if (!(message.GetAttribute("data-errorcode").Equals(code)))
                    throw new Exception("Неверная обработка ошибки");
                ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");
                return;
            }

            LogStage("Проверка createProcess по тестовому сценарию");

            driver.FindElement(By.XPath(".//*[@class='verification__form-inner']/div/div[2]//input")).SendKeys(Keys.Backspace);
            SendKeysToWebElement(".//*[@class='verification__form-inner']/div/div[2]//input", phoneValue, "Поле Номера карты фонбет", "поля Номера карты фонбет");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
            var messageData = GetWebElement(".//*[@id='verification-bk-error']", "Нет модуля с ошибкой");
            if (!(messageData.GetAttribute("data-errorcode").Equals(code) && messageData.GetAttribute("data-processstate").Equals(process) && messageData.GetAttribute("data-rejectioncode").Equals(rejcode)))
                throw new Exception("Неверная обработка ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");
        }
        protected static bool waitTillElementisDisplayed(IWebDriver driver, string xpath, int timeoutInSeconds)
        {
            bool elementDisplayed = false;

            for (int i = 0; i < timeoutInSeconds; i++)
            {
                try
                {
                    if (timeoutInSeconds > 0)
                    {
                        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                        wait.Until(drv => drv.FindElement(By.XPath(xpath)));
                    }
                    elementDisplayed = driver.FindElement(By.XPath(xpath)).Displayed;
                }
                catch
                { }
            }
            return elementDisplayed;

        }
        protected void SendSmsVerificationBK(string smsValue, string process, string code, string rejcode)
        {
            LogStage("Проверка SendSmsCode по тестовому сценарию");
            if (smsValue == "2")
            {
                driver.FindElement(By.XPath(".//*[@class='ui__field-wrap-inner']//input")).Clear();
                SendKeysToWebElement(".//*[@class='ui__field-wrap-inner']//input", smsValue, "Поле Код смс", "поля Код смс");
                Thread.Sleep(1000);
                ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
                waitTillElementisDisplayed(driver, ".//*[@id='verification-bk-error']", 5);
                var message = GetWebElement(".//*[@id='verification-bk-error']", "Нет модуля с ошибкой");
                if (!(message.GetAttribute("data-errorcode").Equals(code)))
                    throw new Exception("Неверная обработка ошибки");
                waitTillElementisDisplayed(driver, ".//*[@class='account-error__actions']//span", 5);
                ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");
                return;
            }
           
            driver.FindElement(By.XPath(".//*[@class='ui__field-wrap-inner']//input")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field-wrap-inner']//input", smsValue, "Поле Код смс", "поля Код смс");
            Thread.Sleep(1000);
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
            if (smsValue == "9")
            {
                LogStage("Проверка waitForPassport по тестовому сценарию");
                ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки Отправить");
                var message = GetWebElement(".//*[@id='verification-bk-error']", "Нет модуля с ошибкой");
                if (!(message.GetAttribute("data-errorcode").Equals("10") && message.GetAttribute("data-processstate").Equals("waitForPassport")))
                    throw new Exception("Неверная обработка ошибки");
                ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Подтвердить", "кнопки Подтвердить");

                driver.FindElement(By.XPath(".//*[@class='verification__form-row']/label[1]//input")).SendKeys(Keys.Home);
                SendKeysToWebElement(".//*[@class='verification__form-row']/label[1]//input", "3", "Поле Серия и номер паспорта", "поля Серия и номер паспорта");
                ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки Отправить");
                waitTillElementisDisplayed(driver, ".//*[@class='account-error__actions']//span", 5);
                var msg = GetWebElement(".//*[@id='verification-bk-error']", "Нет модуля с ошибкой");
                if (!(msg.GetAttribute("data-errorcode").Equals("0") && msg.GetAttribute("data-processstate").Equals("rejected") && msg.GetAttribute("data-rejectioncode").Equals("18")))
                    throw new Exception("Неверная обработка ошибки");
                ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Подтвердить", "кнопки Подтвердить");

                ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
                SendKeysToWebElement(".//*[@class='ui__field-wrap-inner']//input", smsValue, "Поле Код смс", "поля Код смс");
                Thread.Sleep(500);
                ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
                driver.FindElement(By.XPath(".//*[@class='verification__form-row']/label[1]//input")).SendKeys(Keys.Home);
                SendKeysToWebElement(".//*[@class='verification__form-row']/label[1]//input", "1", "Поле Серия и номер паспорта", "поля Серия и номер паспорта");
                ClickWebElement(".//*[@class='toolbar__item']//button", "Кнопка Отправить", "кнопки Отправить");
                waitTillElementisDisplayed(driver, ".//*[@classid='account-error__btn-inner']//a", 9);
                if (!driver.FindElement(By.XPath(".//*[@class='verification']/div")).GetAttribute("class").Contains("success"))
                    throw new Exception("Неверная обработка ошибки");
                ClickWebElement(".//*[@classid='account-error__btn-inner']//a", "Кнопка Подтвердить", "кнопки Подтвердить");
                return;
            }
            if (smsValue == "0")
            {
                
                waitTillElementisDisplayed(driver, ".//*[@class='verification']//span", 10);
                if (!driver.FindElement(By.XPath(".//*[@class='verification']/div")).GetAttribute("class").Contains("success"))
                    throw new Exception("Неверная обработка ошибки");
                ClickWebElement(".//*[@class='verification']//span", "Кнопка Закрыть", "кнопки Закрыть");
                ClickWebElement(".//*[@class='verification__notice-types-wrap']/a", "Кнопка Отменить процесс", "кнопки Отменить процесс");
                ClickWebElement(".//*[@href='#!/account/verification/bk']", "Кнопка Верификации по БК","кнопки Верификации по БК");
                SendKeysToWebElement(".//*[@class='verification__form-inner']/div/div[2]//input", "0000FFFF000", "Поле Номера карты фонбет", "поля Номера карты фонбет");
                Thread.Sleep(500);
                SendKeysToWebElement(".//*[@class='verification__form-inner']/div/div[3]/label[1]//input", "2222222222", "Поле Серия и номер паспорта", "поля Серия и номер паспорта");
                Thread.Sleep(500);
                SendKeysToWebElement(".//*[@class='verification__form-inner']/div/div[3]/label[2]//input", "11112011", "Поле Дата выдачи", "поля Дата выдачи");
                ClickWebElement(".//*[@id='rulesAgree']", "Чекбокс Соглашения с правилами", "чекбокс Соглашения с правилами");
                ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
                return;
            }
            waitTillElementisDisplayed(driver, ".//*[@id='verification-bk-error']", 9);
            var messageData = GetWebElement(".//*[@id='verification-bk-error']", "Нет модуля с ошибкой");
            if (!(messageData.GetAttribute("data-errorcode").Equals(code) && messageData.GetAttribute("data-processstate").Equals(process) && messageData.GetAttribute("data-rejectioncode").Equals(rejcode)))
                throw new Exception("Неверная обработка ошибки");
            waitTillElementisDisplayed(driver, ".//*[@class='account-error__actions']//span", 5);
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");
            waitTillElementisDisplayed(driver, ".//*[@class='toolbar__item']/button", 5);
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
        }
        // Метод принимает на вход  ожидаемый номер телефона и  проверяет правильность работы createProcess по тестовому сценарию на тестовых данных для идентификации киви
        protected void CreateProcessVerificationQiwi(string phoneValue, string process, string code, string rejcode)
        {        
                if (phoneValue == "4")
                {
                    LogStage("Проверка createProcess по тестовому сценарию");
                    driver.FindElement(By.XPath(".//*[@class='ui__field-wrap-inner']//input")).SendKeys(Keys.Backspace);
                    SendKeysToWebElement(".//*[@class='ui__field-wrap-inner']//input", phoneValue, "Поле Номер телефона", "поля Номер телефона");
                    ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
                    var message = GetWebElement(".//*[@id='verification-qiwi-error']", "Нет модуля с ошибкой");
                    if (!(message.GetAttribute("data-errorcode").Equals(code)))
                        throw new Exception("Неверная обработка ошибки");
                    ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");
                    return;
                }
                
            LogStage("Проверка createProcess по тестовому сценарию");

            driver.FindElement(By.XPath(".//*[@class='ui__field-wrap-inner']//input")).SendKeys(Keys.Backspace);
            SendKeysToWebElement(".//*[@class='ui__field-wrap-inner']//input", phoneValue, "Поле Номер телефона","поля Номер телефона");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
            var messageData = GetWebElement(".//*[@id='verification-qiwi-error']", "Нет модуля с ошибкой");
            if (!(messageData.GetAttribute("data-errorcode").Equals(code) && messageData.GetAttribute("data-processstate").Equals(process) && messageData.GetAttribute("data-rejectioncode").Equals(rejcode)))
                throw new Exception("Неверная обработка ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");
        }

        protected void SendSmsVerificationQiwi(string smsValue, string process, string code, string rejcode)
        {
            LogStage("Проверка SendSmsCode по тестовому сценарию");
            driver.FindElement(By.XPath(".//*[@class='ui__field-wrap-inner']//input")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field-wrap-inner']//input", smsValue, "Поле Номер телефона", "поля Номер телефона");
            Thread.Sleep(1000);
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
            waitTillElementisDisplayed(driver, ".//*[@class='account-error__actions']//span", 5);
            var messageData = GetWebElement(".//*[@id='verification-qiwi-error']", "Нет модуля с ошибкой");
            if (!(messageData.GetAttribute("data-errorcode").Equals(code) && messageData.GetAttribute("data-processstate").Equals(process) && messageData.GetAttribute("data-rejectioncode").Equals(rejcode)))
                throw new Exception("Неверная обработка ошибки");
            waitTillElementisDisplayed(driver, ".//*[@class='account-error__actions']//span", 5);
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Повторить", "кнопки Повторить");
            waitTillElementisDisplayed(driver, ".//*[@class='toolbar__item']/button", 5);
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
        }
        protected void SendPasportVerificationQiwi(string pasportValue, string process, string code, string rejcode)
        {
            LogStage("Проверка sendPassport по тестовому сценарию");
         
            
            if (pasportValue == "2222222222")
            {
                waitTillElementisDisplayed(driver, ".//*[@class='verification__form-row']/label[2]//input", 5);
                SendKeysToWebElement(".//*[@class='verification__form-row']/label[1]//input", pasportValue, "Поле Серия и Номер паспорта", "поля Серия и Номер паспорта");
                Thread.Sleep(800);
                SendKeysToWebElement(".//*[@class='verification__form-row']/label[2]//input", "111120000", "Поле Дата выдачи", "поля Дата выдачи");
                ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
                waitTillElementisDisplayed(driver, ".//*[@class='account-error__actions']//span", 5);
                var messageData = GetWebElement(".//*[@id='verification-qiwi-error']", "Нет модуля с ошибкой");
                if (!(messageData.GetAttribute("data-errorcode").Equals(code)))
                    throw new Exception("Неверная обработка ошибки");
                ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Закрыть", "кнопки Закрыть");
            }
             else if (pasportValue == "3333333333")
            {
                driver.FindElement(By.XPath(".//*[@class='verification__form-row']/label[1]//input")).SendKeys(Keys.Home);
                SendKeysToWebElement(".//*[@class='verification__form-row']/label[1]//input", "3", "Поле Серия и Номер паспорта", "поля Серия и Номер паспорта");
                ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
                waitTillElementisDisplayed(driver, ".//*[@class='account-error__actions']//span", 5);
                var messageData = GetWebElement(".//*[@id='verification-qiwi-error']", "Нет модуля с ошибкой");
                if (!(messageData.GetAttribute("data-errorcode").Equals(code) && messageData.GetAttribute("data-processstate").Equals(process)))
                    throw new Exception("Неверная обработка ошибки");
                ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Закрыть", "кнопки Закрыть");
            }

            else if (pasportValue == "4444444444")
            {
                
                driver.FindElement(By.XPath(".//*[@class='verification__form-row']/label[1]//input")).SendKeys(Keys.Home);
                SendKeysToWebElement(".//*[@class='verification__form-row']/label[1]//input", "4", "Поле Серия и Номер паспорта", "поля Серия и Номер паспорта");
                ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
                waitTillElementisDisplayed(driver, ".//*[@class='account-error__actions']//span", 5);
                var messageData = GetWebElement(".//*[@id='verification-qiwi-error']", "Нет модуля с ошибкой");
                if (!(messageData.GetAttribute("data-errorcode").Equals(code) && messageData.GetAttribute("data-processstate").Equals(process) && messageData.GetAttribute("data-rejectioncode").Equals(rejcode)))
                    throw new Exception("Неверная обработка ошибки");
                ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Закрыть", "кнопки Закрыть");
            }
            else
            {
                ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
                waitTillElementisDisplayed(driver, ".//*[@class='toolbar__item']/button", 5);
                SendKeysToWebElement(".//*[@class='ui__field-wrap-inner']//input", "7", "Поле Номер телефона", "поля Номер телефона");
                Thread.Sleep(800);
                ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Подтвердить", "кнопки Подтвердить");
                driver.FindElement(By.XPath(".//*[@class='verification__form-row']/label[1]//input")).SendKeys(Keys.Home);
                SendKeysToWebElement(".//*[@class='verification__form-row']/label[1]//input", pasportValue, "Поле Серия и Номер паспорта", "поля Серия и Номер паспорта");
                ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
                
                if (pasportValue == "5555555555")
                {
                    waitTillElementisDisplayed(driver, ".//*[@class='account-error__actions']//span", 5);
                    var messageData = GetWebElement(".//*[@id='verification-qiwi-error']", "Нет модуля с ошибкой");
                    if (!(messageData.GetAttribute("data-errorcode").Equals(code) && messageData.GetAttribute("data-processstate").Equals(process) && messageData.GetAttribute("data-rejectioncode").Equals(rejcode)))
                        throw new Exception("Неверная обработка ошибки");
                }
                else
                {
                    waitTillElementisDisplayed(driver, ".//*[@class='account-error__actions']//span", 5);
                    if (!driver.FindElement(By.XPath(".//*[@class='verification']/div")).GetAttribute("class").Contains("success"))
                    throw new Exception("Неверная обработка ошибки");
                    ClickWebElement(".//*[@classid='account-error__actions']//span", "Кнопка Закрыть", "кнопки Закрыть");
                    return;
                }
                
                ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Закрыть", "кнопки Закрыть");
            }

           
        }
        protected void CreateProcessPhoneChange(string phoneValue)
        {
            string errorText = "";
            if (phoneValue == "000000002")
                errorText = "Предыдущий процесс не завершён";
            if (phoneValue == "000000003")
                errorText = "Указанный номер телефона уже установлен";
            if (phoneValue == "000000004")
                errorText = "В процессе подтверждения номера телефона произошла неожиданная ошибка. Приносим Вам свои извинения за эту неприятную ситуацию";

            driver.FindElement(By.XPath(".//*[@class='ui__field-inner']/input")).Clear();
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input", phoneValue, "Поле Номер телефона", "поля Номер телефона");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
            var errorMessage = GetWebElement(".//*[@class='account-error__text']", "Нет текста ошибки");
            if (!errorMessage.Text.Contains(errorText))
                throw new Exception("Неверный текст ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Закрыть/Повторить", "кнопки Закрыть/Повторить");
        }

        protected void SendSmsPhoneChange(string smsValue)
        {
            string errorText = "";
            if (smsValue == "2")
                errorText = "Паспортные данные нового номера телефона не совпадают с полученными от ЦУПИС";
            if (smsValue == "3")
                errorText = "Введён неверный код из смс";
            if (smsValue == "4")
                errorText = "Внутренняя ошибка";
            if (smsValue == "1")
            {

                WaitForPageLoad();
                SendKeysToWebElement(".//*[@class='ui__field-inner']/input", smsValue, "Поле Код смс", "поля Код смс");
                ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
                var message = GetWebElement(".//*[@class='account-error__text']", "Нет текста ошибки");
                if (!message.Text.Contains("Номер телефона успешно подверждён."))
                    throw new Exception("Неверный текст ошибки");
                ClickWebElement(".//*[@class='account-profile__form-inner']//*[@class='toolbar__item']/a","Кнопка Вернуться к профилю", "кнопки Вернуться к профилю");
                if(!WebElementExist(".//*[@class='account-profile__block']"))
                    throw new Exception("Не вернулись к профилю");
                return;
            }

            WaitForPageLoad();
            SendKeysToWebElement(".//*[@class='ui__field-inner']/input", smsValue, "Поле Код смс", "поля Код смс");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
            var errorMessage = GetWebElement(".//*[@class='account-error__text']", "Нет текста ошибки");
            if (!errorMessage.Text.Contains(errorText))
                throw new Exception("Неверный текст ошибки");
            ClickWebElement(".//*[@class='account-error__actions']//span", "Кнопка Закрыть/Повторить", "кнопки Закрыть/Повторить");
            ClickWebElement(".//*[@class='toolbar__item']/button", "Кнопка Отправить", "кнопки Отправить");
        }

        public override void BeforeRun()
        {
            base.BeforeRun();
            // Подтверждение куки при необходимости
            if (WebElementExist(".//*[@class='modal-window']"))
                ClickWebElement(".//*[@class='modal-window']/div[2]/a", "Кнопка Согласен с куки", "кнопки Согласен с куки");


            // Смена языка при необходимости
            IWebElement langSetElement = FindWebElement(".//*[@class='header__lang-set']");
            if ((langSetElement != null) && driver.Title.Contains("Home"))
            {
                LogStage("Смена языка на русский");
                ClickWebElement(".//*[@class='header__lang-set']", "Кнопка выбора языка", "кнопки выбора языка");
                ClickWebElement(".//*[@class='header__lang-item']//*[text()='Русский']", "Кнопка выбора русского языка", "кнопки выбора русского языка");
            }

            if (NeedLogin())
            {
                DoLogin();
                UpdateLoginInfo();
            }

        }
    }
}
