using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace TestRun
{

    class Program
    {

        static void RegisterPrograms()
        {
            CustomProgram.Register("custom", CustomProgram.FabricateCustomProgram);
            CustomProgram.Register("webbrowser", CustomWebTestProgram.FabricateCustomWebTestProgram);
            CustomProgram.Register("fonbet", FonbetWebProgram.FabricateFonbetWebProgram);

            CustomProgram.Register("fonbet_favtreetop", fonbet.FavoritesTree.FabricateFavoritesTreeTop);   //Проверка работы избранных событий, если фильтр меню событий сверху
            CustomProgram.Register("fonbet_favtreeleft", fonbet.FavoritesTreeLeft.FabricateFavoritesTreeLeft); // Проверка работы избранных событий, если фильтр меню событий слева
            CustomProgram.Register("fonbet_favexpand", fonbet.FavoritesExpandTree.FabricateFavoritesExpandTree); //Проверка разворачивания/сворачивания всех дочерних событий и доп. пари
            CustomProgram.Register("fonbet_timetree", fonbet.TimeTree.FabricateTimeTree); //Проверка фильтра времени в меню событий
            CustomProgram.Register("fonbet_betssettings", fonbet.BetsSettings.FabricateBetsSettings); //Проверка всего функционала из модуля "Пари" в настройках сайта
            CustomProgram.Register("fonbet_cashoutanddialog", fonbet.CashOutAndDialogsSettings.FabricateCashOutAndDialogsSettings); //Проверка всего функционала из модуля "Продажа пари" и "Диалоги" в настройках сайта
            CustomProgram.Register("fonbet_view", fonbet.View.FabricateView); //Проверка всего функционала из модуля "Вид" в настройках сайта
            CustomProgram.Register("fonbet_viewwologin", fonbet.ViewWithoutLogin.FabricateViewWithoutLogin); //Проверка всего функционала из модуля "Вид" в настройках сайта без авторизации пользователя
            CustomProgram.Register("fonbet_superexpress", fonbet.SuperExpress.FabricateSuperExpress); //Проверка всего функционала во вкладке "Суперэкспресс"
            CustomProgram.Register("fonbet_coupongrid", fonbet.CouponGridInterface.FabricateCouponGridInterface); //Проверка работы панели фильтров из модуля ленты купонов (Меню, Все недавние, Нерасчитанные, На продажу)
            CustomProgram.Register("fonbet_results", fonbet.ResultsTab.FabricateResultsTab); //Проверка работы всего функицонала во вкладке "Результат"
            CustomProgram.Register("fonbet_historybet", fonbet.ProfileHistoryBet.FabricateProfileHistoryBet); //Проверка работы вкладки История в Личном кабинете пользователя
            CustomProgram.Register("fonbet_operationbet", fonbet.ProfileHistoryOperation.FabricateProfileHistoryOperation); //Проверка работы вкладки Операции в Личном кабинете пользователя
            CustomProgram.Register("fonbet_auth", fonbet.AuthorizationChecker.FabricateAuthorizationChecker); //Проверка всех трех типов авторизации(номер счета, номер телефона, почта)
            CustomProgram.Register("fonbet_playerprotection", fonbet.PlayerProtection.FabricatePlayerProtection); //Проверка модуля "Самоограничения" в настройках сайта 
            CustomProgram.Register("request_depositqiwi", fonbet.requests.DepositQiwi.FabricateDepositQiwi); //Проверка полного цикла создания и работы с запросом на тему "Депозит-Киви" из личного кабинета пользователя
            CustomProgram.Register("request_depositcard", fonbet.requests.DepositCard.FabricateDepositCard); //Проверка полного цикла создания и работы с запросом на тему "Депозит-Карта" из личного кабинета пользователя
            CustomProgram.Register("request_depositmobile", fonbet.requests.DepositMobile.FabricateDepositMobile); //Проверка полного цикла создания и работы с запросом на тему "Депозит-Мобильный телефон" из личного кабинета пользователя
            CustomProgram.Register("request_deposityandex", fonbet.requests.DepositYandex.FabricateDepositYandex); //Проверка полного цикла создания и работы с запросом на тему "Депозит-Янлекс Деньги" из личного кабинета пользователя
            CustomProgram.Register("request_supportsite", fonbet.requests.SupportSite.FabricateSupportSite); //Проверка полного цикла создания и работы с запросом на тему "Техподдержка-Сайт" из личного кабинета пользователя
            CustomProgram.Register("request_supportsuggestion", fonbet.requests.SupportSuggestion.FabricateSupportSuggestion); //Проверка полного цикла создания и работы с запросом на тему "Техподдержка-Вопросы и предложениия" из личного кабинета пользователя
            CustomProgram.Register("request_supportmobile", fonbet.requests.SupportMobile.FabricateSupportMobile); //Проверка полного цикла создания и работы с запросом на тему "Техподдержка-Вопрос по мобильному приложению" из личного кабинета пользователя
            CustomProgram.Register("request_calculationsbetlife", fonbet.requests.CalculationsBetLife.FabricateCalculationsBetLife); //Проверка полного цикла создания и работы с запросом на тему "Вопрос по расчету-Пари Лайф" из личного кабинета пользователя
            CustomProgram.Register("request_calculationsbet", fonbet.requests.CalculationsBet.FabricateCalculationsBet); //Проверка полного цикла создания и работы с запросом на тему "Вопрос по расчету-Пари" из личного кабинета пользователя
            CustomProgram.Register("request_increasedmax", fonbet.requests.IncreasedMax.FabricateIncreasedMax); //Проверка полного цикла создания и работы с запросом на тему "Повышенный максимум" из личного кабинета пользователя
            CustomProgram.Register("request_paymentqiwi", fonbet.requests.PaymentQiwi.FabricatePaymentQiwi); //Проверка полного цикла создания и работы с запросом на тему "Проблемы с выплатой - Киви" из личного кабинета пользователя
            CustomProgram.Register("request_paymentcard", fonbet.requests.PaymentCard.FabricatePaymentCard); //Проверка полного цикла создания и работы с запросом на тему "Проблемы с выплатой - Карта" из личного кабинета пользователя
            CustomProgram.Register("request_paymentmobile", fonbet.requests.PaymentMobile.FabricatePaymentMobile); //Проверка полного цикла создания и работы с запросом на тему "Проблемы с выплатой - Мобильный" из личного кабинета пользователя
            CustomProgram.Register("request_paymentyandex", fonbet.requests.PaymentYandex.FabricatePaymentYandex); //Проверка полного цикла создания и работы с запросом на тему "Проблемы с выплатой - Яндекс Деньги" из личного кабинета пользователя
            CustomProgram.Register("request_phonecalculation", fonbet.requests.PhoneCalculation.FabricatePhoneCalculation); //Проверка полного цикла создания и работы с запросом на тему "Телефонный сервис - Вопрос по расчету" из личного кабинета пользователя
            CustomProgram.Register("request_phoneservice", fonbet.requests.PhoneService.FabricatePhoneService); //Проверка полного цикла создания и работы с запросом на тему "Телефонный сервис - Вопрос по тел сервису" из личного кабинета пользователя
            CustomProgram.Register("request_otheradministration", fonbet.requests.OtherAdministration.FabricateOtherAdministration); //Проверка полного цикла создания и работы с запросом на тему "Прочие вопросы - Вопросы к администрации" из личного кабинета пользователя
            CustomProgram.Register("request_othersuggestion", fonbet.requests.OtherSuggestion.FabricateOtherSuggestion); //Проверка полного цикла создания и работы с запросом на тему "Прочие вопросы - Замечания и предложения" из личного кабинета пользователя
            CustomProgram.Register("fonbet_broadcast", fonbet.BroadCastCheck.FabricateBroadCastCheck); //Проверка отображения всех видов трансляций лайф событий 
            CustomProgram.Register("fonbet_atleasttwo", fonbet.AtLeastTwo.FabricateAtLeastTwo); //Проверка правильности работы ставки "Система 2/3"
            CustomProgram.Register("fonbet_correctbetshowing", fonbet.CorrectBetsShowing.FabricateCorrectBetsShowing); //Проверка соответствия отображения типа ставки в таблице событий и в ленте купонов (например поб 1, Ничья, Тотал..)
            CustomProgram.Register("fonbet_appcheking", fonbet.AppChecking.FabricateAppChecking); //Проверка правильного отображения и работы функционала во вкладке "Приложения"
            CustomProgram.Register("fonbet_freebet", fonbet.FreeBet.FabricateFreeBet); //Проврека правильности работы со ставкой "Фрибет"
            CustomProgram.Register("fonbet_howtoplay", fonbet.HowToPlay.FabricateHowToPlay); //Проверка работы функционала модуля "Как делать ставки" на главной странице (видно только для неавторизованных пользователей)
            CustomProgram.Register("fonbet_slider", fonbet.Slider.FabricateSlider); //Проверка работы функционала слайдера на главной странице (видно только для неавторизованных пользователей)
            CustomProgram.Register("fonbet_newsandwinners", fonbet.NewsAndWinnerClub.FabricateNewsAndWinnerClub); //Проверка работы функционала модуля Новости на главной странице (видно только для неавторизованных пользователей)
            CustomProgram.Register("fonbet_pwdrecovery", fonbet.PwdRecovery.FabricatePwdRecovery); //Проверка работы функционала восстановление пароля по тестовому сценарию
            CustomProgram.Register("fonbet_emailconfirm", fonbet.EmailConfirm.FabricateEmailConfirm); //Проверка работы процесса подтверждения email по тестовому сценарию
            CustomProgram.Register("fonbet_registrationv4", fonbet.RegistrationV4.FabricateRegistrationV4); //Проверка работы процесса супер-регистрации(v4) по тестовому сценарию
            CustomProgram.Register("fonbet_verificationcupisqiwi", fonbet.VerificationCupisQiwi.FabricateVerificationCupisQiwi); //Проверка работы процесса верификации через КИВИ по тестовому сценарию
            CustomProgram.Register("fonbet_changephonecupis", fonbet.ChangePhoneCupis.FabricateChangePhoneCupis); //Проверка работы процесса изменения номера телефона по тестовому сценарию
            CustomProgram.Register("fonbet_verificationcupisbk", fonbet.VerificationCupisBk.FabricateVerificationCupisBk); //Проверка работы процесса верификации через BK по тестовому сценарию
            CustomProgram.Register("cyprus_registration", fonbet.cyprus.RegistrationCypr.FabricateRegistrationCypr); //Проверка работы процесса регистрации кипра по тестовому сценарию


            CustomProgram.Register("backoffice_clientcontrol", backoffice.СlientControl.FabricateСlientControl);
            CustomProgram.Register("backoffice_contentblog", backoffice.ContentBlog.FabricateContentBlog);
            CustomProgram.Register("backoffice_contentgeneraltab", backoffice.ContentGeneralTab.FabricateContentGeneralTab);
            //CustomProgram.Register("backoffice_contentbetday", backoffice.ContentBetOfTheDay.FabricateContentBetOfTheDay);
            CustomProgram.Register("backoffice_contentbanner", backoffice.ContentBanner.FabricateContentBanner);
            CustomProgram.Register("backoffice_contentbannerlifetime", backoffice.ContentBannerLifeTime.FabricateContentBannerLifeTime);

        }

        static void ApplyParams(CustomProgram program, string[] args)
        {
            // Читаем настройки по-умолчанию из json-файла
            string jsonText = File.ReadAllText(@"default.settings", System.Text.Encoding.UTF8);
            program.ReadParamsFromJson(jsonText);

            // Читаем настройки из параметров командной строки.
            for(int paramIndex = 1; paramIndex < args.Length; paramIndex++)
            {
                string str = args[paramIndex];
                string[] values = str.Split('=');
                if (values.Length == 2)
                {
                    string key = values[0];
                    string value = values[1];
                    if (value.Length == 0)
                        value = null;
                    program.SetFromString(key, value);
                }
            }

            // Логируем параметры выполнения
            Console.WriteLine(new String('_', Console.WindowWidth));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Параметры выполнения программы:");
            Console.ResetColor();
            program.PrintParameters();
            Console.Write(new String('_', Console.WindowWidth));
        }


        static void TryExecuteTask(TestTaskResponseBody data)
        {
            FabricateProgram programFabric = CustomProgram.FindProgram(data.program);
            try
            {
                if (programFabric == null)
                    throw new Exception(String.Format("Программа {0} не найдена", data.program));
                CustomProgram program = programFabric();

                // Читаем настройки по-умолчанию
                string jsonText = File.ReadAllText(@"default.settings", System.Text.Encoding.UTF8);
                program.ReadParamsFromJson(jsonText);

                // Читаем параметры из тест-задачи
                program.ReadParameters(data);
                ProjectManagerWebClient.ApplyParamsToProgram(program);

                // Предварительная инициализация
                program.VerifyParameters();
                program.WriteParametersToReport();
                // Если на этом этапе все ок - отправляем серверу ПМ подтсверждение - что автотестировщик переходит в режим выполнения теста
                if (ProjectManagerWebClient.ConfirmTask(data.task))
                {
                    // Если подтверждение принято - начинаем тестирование
                    try
                    {
                        program.Report.ProgramName = data.program;
                        program.Report.StartTime = DateTime.UtcNow;

                        program.BeforeRun();
                        program.Run();
                        program.Report.Success = true;
                    }
                    catch (Exception e)
                    {
                        
                        program.Report.Success = false;
                        program.Report.ErrorText = e.Message;
                        program.OnError(e);
                    }
                    finally
                    {
                        try
                        {
                            program.AfterRun();
                        }
                        finally
                        {
                            program.Report.FinishTime = DateTime.UtcNow;
                            ProjectManagerWebClient.SendReport(program.Report);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ProjectManagerWebClient.RejectTestTask(data.task, e.Message);
            }
        }

        static void RequestTestTask()
        {
            try
            {
                TestTaskResponseBody responsedTask = ProjectManagerWebClient.RequestTestTask();
                if (responsedTask == null)
                    Thread.Sleep(20000);
                else
                    TryExecuteTask(responsedTask);
            }
            catch (Exception)
            {
                Thread.Sleep(60000);
            }
        }

        static void MainLooped(string[] args)
        {
            ProjectManagerWebClient.LoadSettings("auto.settings");
            try
            {
                while (true)
                {
                    RequestTestTask();
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.Write("\n\n [!] ");
                Console.ResetColor();
                Console.WriteLine(" Ошибка выполнения - {0}", e.Message);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        static void Main(string[] args)
        {
            RegisterPrograms();
            if ((args.Length == 1) && (args[0] == "auto"))
            {
                MainLooped(args);
            }
            else
            try
            {
                // Понять какой тест запускать
                if (args.Length == 0)
                    throw new Exception("Неуказана программа первым параметром командной строки.");

                string programName = args[0];

                Console.Write("\nИнициализация программы {0}...", programName);

                FabricateProgram programFabric = CustomProgram.FindProgram(programName);
                if (programFabric == null)
                    throw new Exception(String.Format("Программа {0} неопределена.", programName));
                // Создаем тест
                CustomProgram program = programFabric();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[OK]");
                Console.ResetColor();
                // Передать параметры
                ApplyParams(program, args);

                program.VerifyParameters();
                program.WriteParametersToReport();

                // Запустить тест
                try
                {
                    program.Report.ProgramName = programName;
                    program.Report.StartTime = DateTime.UtcNow;

                    Console.WriteLine(" - Предпусковая подготовка...");
                    program.BeforeRun();
                    Console.WriteLine(" - Выполнение...");
                    program.Run();
                    program.Report.Success = true;
                    // Передать результат
                    
                }
                catch (Exception exception)
                {
                    program.Report.Success = false;
                    program.Report.ErrorText = exception.Message;
                    program.OnError(exception);
                    throw;
                }
                finally
                {
                    Console.WriteLine(" - Завершение...");
                    try
                    {
                        program.AfterRun();
                    }
                    finally
                    {
                        program.Report.FinishTime = DateTime.UtcNow;
                        string report = program.ReportText();
                        try
                        {
                            Console.WriteLine("Отправка отчета");
                            ProjectManagerWebClient.SendReport(program.Report);
                        } catch (Exception)
                        {
                            // stub
                        }
                        Console.WriteLine("Сохранение отчета в файл lastresult.json");
                        File.WriteAllText(@"lastReport.json", report);
                    }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.Write("\n\n [!] ");
                Console.ResetColor();
                Console.WriteLine(" Ошибка выполнения - {0}", e.Message);
            }
            finally
            {
                Console.ResetColor(); 
            }
        }
    }
}
