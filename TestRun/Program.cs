using System;
using System.IO;
using System.Net;
using System.Text;

namespace TestRun
{
    class Program
    {
        static void RegisterPrograms()
        {
            CustomProgram.Register("custom", CustomProgram.FabricateCustomProgram);
            CustomProgram.Register("webbrowser", CustomWebTestProgram.FabricateCustomWebTestProgram);
            CustomProgram.Register("fonbet", FonbetWebProgram.FabricateFonbetWebProgram);

            CustomProgram.Register("fonbet_favtreetop", fonbet.FavoritesTree.FabricateFavoritesTreeTop);
            CustomProgram.Register("fonbet_favtreeleft", fonbet.FavoritesTreeLeft.FabricateFavoritesTreeLeft);
            CustomProgram.Register("fonbet_favexpand", fonbet.FavoritesExpandTree.FabricateFavoritesExpandTree);
            CustomProgram.Register("fonbet_timetree", fonbet.TimeTree.FabricateTimeTree);
            CustomProgram.Register("fonbet_betssettings", fonbet.BetsSettings.FabricateBetsSettings);
            CustomProgram.Register("fonbet_cashoutanddialog", fonbet.CashOutAndDialogsSettings.FabricateCashOutAndDialogsSettings);
            CustomProgram.Register("fonbet_view", fonbet.View.FabricateView);
            CustomProgram.Register("fonbet_viewwologin", fonbet.ViewWithoutLogin.FabricateViewWithoutLogin);
            CustomProgram.Register("fonbet_superexpress", fonbet.SuperExpress.FabricateSuperExpress);
            CustomProgram.Register("fonbet_coupongrid", fonbet.CouponGridInterface.FabricateCouponGridInterface);
            CustomProgram.Register("fonbet_results", fonbet.ResultsTab.FabricateResultsTab);
            CustomProgram.Register("fonbet_historybet", fonbet.ProfileHistoryBet.FabricateProfileHistoryBet);
            CustomProgram.Register("fonbet_operationbet", fonbet.ProfileHistoryOperation.FabricateProfileHistoryOperation);
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


        static void SendResult(string URL, string resultText)
        {
            WebRequest request = WebRequest.Create(URL);
            request.Method = "POST";
            byte[] postBody = Encoding.UTF8.GetBytes(resultText);
            request.ContentType = "application/json";
            request.ContentLength = postBody.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(postBody, 0, postBody.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            response.Close();

        }

        static void Main(string[] args)
        {
            RegisterPrograms();
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
                            SendResult(program.ResultURI, report);
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
