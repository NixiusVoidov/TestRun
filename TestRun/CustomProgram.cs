using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TestRun
{
    delegate CustomProgram FabricateProgram();

    public class ProgramParameters
    {
        public string AgentId { get; set; }
        public string Actor { get; set; }
        public string ResultURI { get; set; }
        
    }

    public enum ProgramStepReportType { Step, Stage, Warning, Hint}


    public class ProgramStepReport
    {
        public DateTime Time { get; set; }
        public string Text { get; set; }
        public ProgramStepReportType Type { get; set; }
        public Boolean? Success = null;
        public int? Index = null;
        public String Error = null;
    }

    public class ProgramReport
    {
        public string AgentId = null;
        public string Actor = null;
        public string ProgramName = null;
        public DateTime? StartTime = null;
        public DateTime? FinishTime = null;
        public Dictionary<string, string> Conditions = new Dictionary<string, string>();
        public List<ProgramStepReport> Steps = new List<ProgramStepReport>();
        public Boolean? Success = null;
        public string ErrorText = null;
    }

    class CustomProgram
    {
        public string ResultURI = "";
        public string AgentId = "";
        public string Actor = "";

        private static Dictionary<string, FabricateProgram> programList = new Dictionary<string, FabricateProgram>();

        private bool ActionStarted = false;
        private int ActionCounter = 0;

        public ProgramReport Report = new ProgramReport();
        protected ProgramStepReport CurrentStep = null;

        public static void Register(string name, FabricateProgram program)
        {
            programList.Add(name, program);
        }

        public static CustomProgram FabricateCustomProgram()
        {
            return new CustomProgram();
        }

        public static FabricateProgram FindProgram(string name)
        {
            if (programList.ContainsKey(name))
                return programList[name];
            else
                return null;
        }

        public CustomProgram()
        {

        }

        protected void ReadParameters(ProgramParameters parameters)
        {
            ResultURI = parameters.ResultURI;
            AgentId = parameters.AgentId;
            Actor = parameters.Actor;
        }

        public virtual void ReadParamsFromJson(string jsonText)
        {
            ProgramParameters prm = JsonConvert.DeserializeObject<ProgramParameters>(jsonText);

            ReadParameters(prm);
        }

        public virtual void SetFromString(string paramName, string paramValue)
        {
            if (paramName.Equals("ResultURI"))
                ResultURI = paramValue;
        }

        public virtual void VerifyParameters()
        {
            if (ResultURI == null)
                throw new Exception("Неуказан URI отправки результатов.");
        }

        public virtual void PrintParameters()
        {
            Console.WriteLine("Идентификатор агента: \t\t{0}", AgentId ?? "[не указан]");
            Console.WriteLine("Участник: \t\t\t{0}", Actor ?? "[не указан]");
            Console.WriteLine("URI отправки результата: \t{0}", ResultURI ?? "[неопределен]");
        }

        public virtual void WriteParametersToReport()
        {
            Report.AgentId = AgentId;
            Report.Actor = Actor;
            Report.Conditions.Add("ResultURI", ResultURI);
        }

        public virtual void BeforeRun()
        {
            
        }

        public virtual void Run()
        {
            
        }

        public virtual void AfterRun()
        {
            
        }

        public virtual void OnError(Exception exception)
        {
            Console.CursorLeft = Console.WindowWidth - 7;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("FAIL");
            Console.ResetColor();
            Console.WriteLine("]");
            ActionStarted = false;
            if (CurrentStep != null)
            {
                CurrentStep.Success = false;
                CurrentStep.Error = exception.Message;
                Report.Steps.Add(CurrentStep);
                CurrentStep = null;
            }
        }

        protected void LogStartAction(string actionCaption)
        {
            if (ActionStarted)
                LogActionSuccess();
            Console.Write(" ({0})\t {1}", ++ActionCounter, actionCaption);
            ActionStarted = true;

            CurrentStep = new ProgramStepReport
            {
                Time = DateTime.UtcNow,
                Type = ProgramStepReportType.Step,
                Text = actionCaption,
                Index = ActionCounter
            };

            Console.CursorLeft = Console.WindowWidth - 5;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.Write("[??]");
            Console.ResetColor();
        }

        protected void LogActionSuccess()
        {
            if (ActionStarted)
            {
                Console.CursorLeft = Console.WindowWidth - 5;
                Console.Write("[");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("OK");
                Console.ResetColor();
                Console.WriteLine("]");
                ActionStarted = false;
                if (CurrentStep != null)
                {
                    CurrentStep.Success = true;
                    Report.Steps.Add(CurrentStep);
                    CurrentStep = null;
                }
            }
        }

        protected void LogStage(string stageCaption)
        {
            if (ActionStarted)
                LogActionSuccess();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(String.Format("\t {0}", stageCaption));
            Console.ResetColor();

            ProgramStepReport step = new ProgramStepReport
            {
                Text = stageCaption,
                Time = DateTime.UtcNow,
                Type = ProgramStepReportType.Stage
            };
            Report.Steps.Add(step);
        }

        protected void LogHint(string hintCaption)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(String.Format("\t {0}", hintCaption));
            Console.ResetColor();
            ProgramStepReport step = new ProgramStepReport
            {
                Text = hintCaption,
                Time = DateTime.UtcNow,
                Type = ProgramStepReportType.Hint
            };
            Report.Steps.Add(step);
        }

        protected void LogWarning(string warningCaption)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(String.Format("\t {0}", warningCaption));
            Console.ResetColor();
            ProgramStepReport step = new ProgramStepReport
            {
                Text = warningCaption,
                Time = DateTime.UtcNow,
                Type = ProgramStepReportType.Warning
            };
            Report.Steps.Add(step);
        }

        public string ReportText()
        {
            return JsonConvert.SerializeObject(Report);
        }
    }

}
