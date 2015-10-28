using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace PACMAN
{
    static class Log
    {
        public static DateTime now;
        private static StreamWriter _writer;
        public static void addLog(string logMessage)
        {
            TaskManager.Add(null, delegate
            {
                using (_writer = File.AppendText("log.txt"))
                {
                    _writer.Write("{0}", Math.Round((DateTime.Now - now).TotalMilliseconds));
                    Log.now = DateTime.Now;
                    _writer.WriteLine(" :\t{0}", logMessage);
                }
            });
        }
    }
}
