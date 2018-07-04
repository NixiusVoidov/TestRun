using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


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
            ClickWebElement(".//*[@class='header__login-head']/div[1]/span", "ФИО в шапке", "ФИО в шапке");
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

        public override void BeforeRun()
        {
            base.BeforeRun();
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
