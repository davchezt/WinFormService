using EngineIOSharp.Common.Enum;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using Tiny.RestClient;

namespace DaVcheztService
{
    public partial class MainService : ServiceBase
    {
        String ApiUrl = "https://api.entrepreneurday.id";

        TinyRestClient client;
        SocketIOClient socket;

        Timer timer = new Timer(); // name space(using System.Timers;)
        public MainService()
        {
            InitializeComponent();

            // Socket.IO
            SocketConnect();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);

            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 300000; // number in milisecinds 5 minutes
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            socket.Close();
            socket.Dispose();

            WriteToFile("Service is stopped at " + DateTime.Now);
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            // GET
            DoGetRequest("v1/user/me");
            // POST JSON
            // DoPostRequest("v1/user/login");
            // POST FormData
            DoFormPostRequest("v1/user/login");

            WriteToFile("Service is recall at " + DateTime.Now);
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to. 
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

        public void SocketConnect()
        {
            SocketIOClientOption option = new SocketIOClientOption(EngineIOScheme.https, "militant-socket-server.herokuapp.com", 443);
            // SocketIOClientOption option = new SocketIOClientOption(EngineIOScheme.http, "localhost", 8080);
            socket = new SocketIOClient(option);
            socket.On("connection", () =>
            {
                WriteToFile("Socket Connected!");

                socket.Emit("subscribe", "test");
                /*socket.Emit("ping", new Dictionary<String, String> {
                    { "anu", "una" }
                });*/
            });

            socket.On("disconnect", () =>
            {
                WriteToFile("Socket Disconnected!");
            });

            socket.On("error", (JToken[] Data) => // Type of argument is JToken[].
            {
                if (Data != null && Data.Length > 0 && Data[0] != null)
                {
                    WriteToFile("Socket Error: " + Data[0]);
                }
                else
                {
                    WriteToFile("Socket Error: Unkown Error");
                }
            });

            socket.On("message", (Data) => // Argument can be used without type.
            {
                if (Data != null && Data.Length > 0 && Data[0] != null)
                {
                    WriteToFile("Socket Message: " + Data[0]);
                }
            });

            /*socket.On("ping", (Data) => // Argument can be used without type.
            {
                if (Data != null && Data.Length > 0 && Data[0] != null)
                {
                    WriteToFile("Message : " + Data[0]);
                }
            });*/

            socket.On("user", (Data) => // Argument can be used without type.
            {
                if (Data != null && Data.Length > 0 && Data[0] != null)
                {
                    WriteToFile("Socket Message: " + Data[0]);
                }
            });

            /*socket.On("CustomEvent", CustomEventHandler); // Handler can be method.
            socket.On(9001, ItsOverNineThousands); // Type of event is JToken. So, it can be a number.
            socket.Off(9001, ItsOverNineThousands);*/ // Remove 9001 event handler.

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {
                socket.Connect();
            }
            catch (Exception ex)
            {
                WriteToFile($"{ex.Message}");
            }
        }

        private void DoGetRequest(String RouterPath = null)
        {
            Task.Run(action: async () =>
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                client = new TinyRestClient(new HttpClient(), ApiUrl);
                client.Settings.DefaultHeaders.Add("Accept", "application/json");
                client.Settings.DefaultHeaders.Add("User-Agent", "Admin Client 1.0");
                client.Settings.DefaultHeaders.AddBearer("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiMiJ9.PbGhl-f91Mc537CmVSrCHnah0qB-ze7c3orXaKwshZk");
                // client.Settings.Formatters.OfType<JsonFormatter>().First().UseKebabCase();

                try
                {
                    // String response = await client.GetRequest(RouterPath).ExecuteAsStringAsync();
                    // JObject json = JObject.Parse(response);

                    JObject output = await client.GetRequest(RouterPath).ExecuteAsync<JObject>();
                    WriteToFile(output.ToString());
                }
                catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    WriteToFile($"{ex.Message} {ex.ReasonPhrase}");
                }
                catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.InternalServerError)
                {
                    WriteToFile($"{ex.Message} {ex.ReasonPhrase}");
                }
                catch (Exception ex)
                {
                    WriteToFile($"{ex.Message}");
                }
            });
        }

        private void DoPostRequest(String RouterPath = null)
        {
            Task.Run(action: async () =>
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                client = new TinyRestClient(new HttpClient(), ApiUrl);
                client.Settings.DefaultHeaders.Add("Accept", "application/json");
                client.Settings.DefaultHeaders.Add("User-Agent", "Admin Client 1.0");

                try
                {
                    Dictionary<String, String> form = new Dictionary<String, String>() {
                        { "username", "davchezt" },
                        { "password", "4Bahagia4" },
                        { "login", "true" }
                    };
                    string jsonText = JsonConvert.SerializeObject(form, Formatting.Indented);
                    // JObject jsonObject = JObject.Parse(jsonText);

                    JObject output = await client.PostRequest(RouterPath)
                        .AddStringContent(jsonText, "application/json")
                        .ExecuteAsync<JObject>();

                    WriteToFile(output.ToString());
                }
                catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    WriteToFile($"{ex.Message} {ex.ReasonPhrase}");
                }
                catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.InternalServerError)
                {
                    WriteToFile($"{ex.Message} {ex.ReasonPhrase}");
                }
                catch (Exception ex)
                {
                    WriteToFile($"{ex.Message}");
                }
            });
        }

        private void DoFormPostRequest(String RouterPath = null)
        {
            Task.Run(action: async () =>
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                client = new TinyRestClient(new HttpClient(), ApiUrl);
                client.Settings.DefaultHeaders.Add("Accept", "application/json");
                client.Settings.DefaultHeaders.Add("User-Agent", "Admin Client 1.0");
                try
                {
                    Dictionary<String, String> FormData = new Dictionary<String, String>() {
                        { "username", "davchezt" },
                        { "password", "4Bahagia4" },
                        { "login", "true" }
                    };

                    JObject output = await client.PostRequest(RouterPath)
                        .AddFormParameters(FormData)
                        .ExecuteAsync<JObject>();

                    WriteToFile(output.ToString());
                }
                catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    WriteToFile($"{ex.Message} {ex.ReasonPhrase}");
                }
                catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.InternalServerError)
                {
                    WriteToFile($"{ex.Message} {ex.ReasonPhrase}");
                }
            });
        }
    }
}
