using System;
using System.IO;
using TeklaAI.Core.Interfaces;

namespace TeklaAI.Core.Utilities
{
    /// <summary>
    /// Simple file and console logger implementation
    /// </summary>
    public class Logger : ILogger
    {
        private static Logger _instance;
        private static readonly object _lock = new object();
        private readonly string _logFilePath;
        private readonly bool _writeToConsole;

        private Logger(string logDirectory = null, bool writeToConsole = true)
        {
            _writeToConsole = writeToConsole;

            // Default log directory: MyDocuments/TeklaAI/Logs
            if (string.IsNullOrEmpty(logDirectory))
            {
                string myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                logDirectory = Path.Combine(myDocs, "TeklaAI", "Logs");
            }

            // Create directory if it doesn't exist
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Log file name: TeklaAI_2024-12-02.log
            string fileName = $"TeklaAI_{DateTime.Now:yyyy-MM-dd}.log";
            _logFilePath = Path.Combine(logDirectory, fileName);
        }

        /// <summary>
        /// Get singleton instance of Logger
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new Logger();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Initialize logger with custom settings (call once at app start)
        /// </summary>
        public static void Initialize(string logDirectory = null, bool writeToConsole = true)
        {
            lock (_lock)
            {
                _instance = new Logger(logDirectory, writeToConsole);
            }
        }

        public void Info(string message)
        {
            WriteLog("INFO", message);
        }

        public void Warning(string message)
        {
            WriteLog("WARNING", message);
        }

        public void Error(string message, Exception ex = null)
        {
            string fullMessage = message;
            if (ex != null)
            {
                fullMessage += $"\nException: {ex.GetType().Name}\nMessage: {ex.Message}\nStack Trace:\n{ex.StackTrace}";
            }
            WriteLog("ERROR", fullMessage);
        }

        public void Success(string message)
        {
            WriteLog("SUCCESS", message);
        }

        public void Debug(string message)
        {
            WriteLog("DEBUG", message);
        }

        private void WriteLog(string level, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logEntry = $"[{timestamp}] [{level}] {message}";

            // Write to file
            lock (_lock)
            {
                try
                {
                    File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    // If file write fails, at least write to console
                    Console.WriteLine($"LOGGING ERROR: {ex.Message}");
                }
            }

            // Write to console with color coding
            if (_writeToConsole)
            {
                ConsoleColor originalColor = Console.ForegroundColor;

                switch (level)
                {
                    case "ERROR":
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case "WARNING":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case "SUCCESS":
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case "DEBUG":
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }

                Console.WriteLine(logEntry);
                Console.ForegroundColor = originalColor;
            }
        }

        /// <summary>
        /// Get the current log file path
        /// </summary>
        public string GetLogFilePath()
        {
            return _logFilePath;
        }
    }
}