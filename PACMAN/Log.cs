using System;
using System.IO;
using System.Reflection;

namespace PACMAN
{
    static class Log
    {
        private static readonly string Path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\log.txt";
        public static DateTime Now;
        private static StreamWriter _writer;
        public static void AddLog(string logMessage)
        {
            TaskManager.Add(null, delegate
            {
                using (_writer = File.AppendText(Path))
                {
                    _writer.Write("{0}", Math.Round((DateTime.Now - Now).TotalMilliseconds));
                    Now = DateTime.Now;
                    _writer.WriteLine(" :\t{0}", logMessage);
                }
            });
        }
    }
}
