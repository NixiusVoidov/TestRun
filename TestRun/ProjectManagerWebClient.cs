using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace TestRun
{
    class ProjectManagerWebClientSettings
    {
        public string Agent { get; set; }
        public string Actor { get; set; }
        public string URL { get; set; }
    }

    // Тело запроса задачи на тестирования от сервера ПМ
    public class GetTestTaskRequestData
    {
        public string agent { get; set; }
        public string actor { get; set; }
    }

    // Тело запроса подтверждения/дискарда от выполнения тест-задачи
    public class SendConfirmTestTaskRequestData
    {
        public string agent { get; set; }
        public string actor { get; set; }
        public bool confirm { get; set; }
        public string message { get; set; }
        public string task { get; set; }
    }

    public class ProjectManagerServerResponse
    {
        public string kind { get; set; }
    }

    public class TestTaskResponseBody
    {
        public string program { get; set; }
        public string browser { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public string url { get; set; }
        public Int64 task { get; set; }
    }

    public class ErrorResponseBody
    {
        public int errorCode { get; set; }
        public string errorText { get; set; }    
        public string errorValue { get; set; }
        public int logicErrorCode { get; set; }
    }

    public class ErrorResponse : ProjectManagerServerResponse
    {
        public ErrorResponseBody response = new ErrorResponseBody();
    }

    public class TestTaskResponse : ProjectManagerServerResponse
    {
        public TestTaskResponseBody response = new TestTaskResponseBody();
    }
    
    class ProjectManagerWebClient   
    {
        protected static ProjectManagerWebClientSettings Settings = new ProjectManagerWebClientSettings();

        public static void LoadSettings(string fileName)
        {
            string jsonText = File.ReadAllText(fileName, System.Text.Encoding.UTF8);
            Settings = JsonConvert.DeserializeObject<ProjectManagerWebClientSettings>(jsonText);
        }

        public static void ApplyParamsToProgram(CustomProgram program)
        {
            program.Actor = Settings.Actor;
            program.AgentId = Settings.Agent;
        }

        static protected string PerformPostRequest(string URL, string postText)
        {
            WebRequest request = WebRequest.Create(URL);
            request.Method = "POST";
            byte[] postBody = Encoding.UTF8.GetBytes(postText);
            request.ContentType = "application/json";
            request.ContentLength = postBody.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(postBody, 0, postBody.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();

            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string responseText = reader.ReadToEnd();
            response.Close();
            return responseText;
        }

        static void LogWebError(string operationName, ErrorResponse error)
        {
            Console.WriteLine("{0} - Ошибка с кодом {1} - {2} [{3}]", operationName, error.response.errorCode, error.response.errorText, error.response.errorValue);
        }

        static void CheckWebError(string operationName, ProjectManagerServerResponse response)
        {
            if (response.kind == "error")
                LogWebError(operationName, (ErrorResponse)response);
        }
        
        static protected ProjectManagerServerResponse PerformRequest(string URL, object data)
        {
            string responseText = PerformPostRequest(URL, JsonConvert.SerializeObject(data));
            ProjectManagerServerResponse response = JsonConvert.DeserializeObject<ProjectManagerServerResponse>(responseText);
            if (response.kind == "error")
                return JsonConvert.DeserializeObject<ErrorResponse>(responseText);
            if (response.kind == "testTask")
                return JsonConvert.DeserializeObject<TestTaskResponse>(responseText);
            return response;
        }

        static public string ConfirmTaskUrl()
        {
            return Settings.URL + "api/projectManager/sendConfirmTestTask";
        }

        static public string RequestTestTaskURL()
        {
            return Settings.URL + "api/projectManager/getTestTask";
        }

        static public string SendReportURL()
        {
            return Settings.URL + "atsapi/sendResult";
        }

        static public bool ConfirmTask(Int64 taskId)
        {
            SendConfirmTestTaskRequestData requestData = new SendConfirmTestTaskRequestData();
            requestData.actor = Settings.Actor;
            requestData.agent = Settings.Agent;
            requestData.task = taskId.ToString();
            requestData.confirm = true;
            try
            {
                ProjectManagerServerResponse response = PerformRequest(ConfirmTaskUrl(), requestData);
                CheckWebError("Подтверждение выполнения задачи", response);
                return response.kind == "accepted";
            }
            catch (Exception)
            {
                return false;
            }
        }

        static public void RejectTestTask(Int64 taskId, string message)
        {
            SendConfirmTestTaskRequestData requestData = new SendConfirmTestTaskRequestData();
            requestData.actor = Settings.Actor;
            requestData.agent = Settings.Agent;
            requestData.task = taskId.ToString();
            requestData.confirm = false;
            requestData.message = message;
            ProjectManagerServerResponse response = PerformRequest(ConfirmTaskUrl(), requestData);
            CheckWebError("Отклонение выполнения задачи", response);
        }

        static public TestTaskResponseBody RequestTestTask()
        {
            GetTestTaskRequestData requestData = new GetTestTaskRequestData();
            requestData.agent = Settings.Agent;
            requestData.actor = Settings.Actor;

            ProjectManagerServerResponse response = PerformRequest(RequestTestTaskURL(), requestData);

            CheckWebError("Запрос задачи тестирования", response);

            if (response.kind == "error")
            {
                throw new Exception(((ErrorResponse)response).response.errorText);
            }

            if (response.kind == "testTask")
                return ((TestTaskResponse)response).response;

            return null;
        }

        static public void SendReport(ProgramReport report)
        {
            string responseText = PerformPostRequest(SendReportURL(), JsonConvert.SerializeObject(report));
            Console.WriteLine("Результат отправки отчета: \n {0}", responseText);
        }
    }


}
