using System;
using System.IO;
using System.ServiceProcess;

namespace DaVcheztService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new MainService()
                };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                Start(args);

                string LogText = "Press any key to stop...";
                Console.WriteLine(LogText);
                WriteToFile(LogText);

                Console.ReadKey(true);

                Stop();
            }
        }

        public static void Start(string[] args)
        {
            string LogText = "Service Started";
            Console.WriteLine(LogText);

            WriteToFile(LogText);
        }

        public static void Stop()
        {
            string LogText = "Service Stoped";
            Console.WriteLine(LogText);

            WriteToFile(LogText);
        }

        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceConsoleLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
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
    }
}
