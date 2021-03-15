using EngineIOSharp.Common.Enum;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tiny.RestClient;
using TrayApp.Properties;

namespace TrayApp
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "{aaf9b8d3-736e-40ac-935b-f44043ec2691}");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string[] launchArgs = Environment.GetCommandLineArgs();

            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                try
                {
                    string args = String.Empty;
                    if (launchArgs.Length > 1)
                    {
                        args = launchArgs[1].Trim();
                    }
                    // GET
                    Helper.DoGetRequest("v1/user/me");
                    // POST JSON
                    // DoPostRequest("v1/user/login");
                    // POST FormData
                    Helper.DoFormPostRequest("v1/user/login");

                    Application.Run(new MyCustomApplicationContext());
                }
                catch (UriFormatException) { }
            }
            else
            {
                string args = String.Empty;
                if (launchArgs.Length > 1)
                {
                    args = launchArgs[1].Trim();
                }

                MessageBox.Show("App is running" + args);
            }
            mutex.ReleaseMutex();
        }

        private static void SetStartup(bool save)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (save)
                rk.SetValue("WinFormsApp", Application.ExecutablePath);
            else
                rk.DeleteValue("WinFormsApp", false);

        }
    }

    public class MyCustomApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        public MyCustomApplicationContext()
        {
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Show", Open);
            trayMenu.MenuItems.Add("Exit", Exit);

            // Initialize Tray Icon
            trayIcon = new NotifyIcon();
            trayIcon.Text = "Tray App";
            trayIcon.Icon = Resources.AppIcon;
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;

            Helper.SocketConnect();
            Helper.OpenApp(String.Empty);
        }

        private void Open(object sender, EventArgs e)
        {
            Helper.OpenApp(String.Empty);
        }

        private void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            Helper.SocketDisconnect();
            Application.Exit();
        }
    }

    public static class Helper
    {
        static string ApiUrl = "https://api.entrepreneurday.id";
        static string exePath = AppDomain.CurrentDomain.BaseDirectory + "WinFormsApp.exe";

        static TinyRestClient client;
        static SocketIOClient socket;

        public static void SocketConnect()
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

                    if (File.Exists(exePath))
                    {
                        string exeConsolePath = AppDomain.CurrentDomain.BaseDirectory + "ConsoleApp.exe";

                        WriteToFile("Execute: " + exePath + " " + Data[0]["event"].ToString());
                        OpenApp(Data[0]["event"].ToString());
                    }
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

        public static void OpenApp(string launchArgs)
        {
            ProcessStartInfo info = new ProcessStartInfo(exePath);

            if (launchArgs != String.Empty)
            {
                info = new ProcessStartInfo(exePath, launchArgs);
            }
            info.UseShellExecute = false;
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.CreateNoWindow = true;
            info.ErrorDialog = false;
            info.WindowStyle = ProcessWindowStyle.Hidden;

            Process process = Process.Start(info);
        }

        public static void SocketDisconnect()
        {
            socket.Close();
            socket.Dispose();
        }

        public static void DoGetRequest(String RouterPath = null)
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

        public static void DoPostRequest(String RouterPath = null)
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

        public static void DoFormPostRequest(String RouterPath = null)
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

        public static void WriteToFile(string Message)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\HelperLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
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
            catch { }
        }
    }
}
