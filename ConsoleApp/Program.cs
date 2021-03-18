using System;
using System.Diagnostics;

namespace ConsoleApp
{
    class Program
    {
        static string exePath = AppDomain.CurrentDomain.BaseDirectory + "WinFormsApp.exe";

        static void Main(string[] args)
        {
            ProcessStartInfo info = new ProcessStartInfo(exePath);
            info.UseShellExecute = false;
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            // info.CreateNoWindow = true;
            info.ErrorDialog = false;
            // info.WindowStyle = ProcessWindowStyle.Hidden;

            Process process = Process.Start(info);
            process.WaitForExit();
        }
    }
}
