using System;
using System.IO;

namespace Voyager
{
    public sealed class Logger : ILogger
    {
        static Logger()
        {
        }

        public static Logger Instance { get; } = new Logger();

        private StreamWriter _logFile;

        public void OpenFile()
        {
            var date = DateTime.Now.ToString("MM.dd.yyyy hh.mm.ss");
            var path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            var fileName = path  + "\\" + date + ".log";

            _logFile = new StreamWriter(fileName);
        }

        public void CloseFile()
        {
            _logFile.Dispose();
        }

        public void AppendLog(string text)
        {
            _logFile.Write(text);
        }
    }
}
