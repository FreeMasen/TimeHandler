using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace TimeHandler
{
    class Logger
    {
        private static DateTime CurrentDate = DateTime.Now.Date;
#if DEBUG
        private static string Path = $"{TrayContext.APP_DATA_PATH}\\{DateTime.Now.Date:yyyy-MM-dd}.debug.log";
#else
        private static string Path = $"{TrayContext.APP_DATA_PATH}\\{DateTime.Now.Date:yyyy-MM-dd}.log";
#endif
        private static bool Cleaned = false;
        public static void Log(string message, bool error = false)
        {
#if DEBUG
            DebugLog(message, error);
#else
            ProdLog(message, error);
#endif
        }

        static void DebugLog(string message, bool error = false)
        {
            var st = new StackTrace();
            var caller = st.GetFrame(2);
            var method = caller.GetMethod();
            var mName = method.Name;
            var mClass = method.DeclaringType;
            var kind = error ? "ERROR" : "INFO";
            var msg = $"{DateTime.Now:y-M-dTH:mm:ss.ffff},{kind},{mClass}.{mName}\"{message}\"";
            Console.WriteLine(msg, error);
            //FileLog(message, error);
        }

        static void ProdLog(string message, bool error = false)
        {
            try
            {
                Data.InsertLog(DateTime.Now, error ? "ERROR" : "INFO", message);
            }
            catch (Exception ex)
            {

                FileLog(ex.Message, true);
            }
            finally
            {
                FileLog(message, error);
            }
        }
        private static void FileLog(string message, bool error)
        {
            if (CurrentDate < DateTime.Now.Date)
            {
                UpdatePath();
            }
            if (!Cleaned)
            {
                Clean();
                Cleaned = true;
            }
            try
            {
                using (var s = File.AppendText(Path))
                {
                    if (error)
                    {
                        s.WriteLine($"\"{DateTime.Now:y-M-dTH:mm:ss.ffff}\",\"ERROR\",\"{message}\"");
                    }
                    else
                    {
                        s.WriteLine($"\"{DateTime.Now:y-M-dTH:mm:ss.ffff}\",\"INFO\" ,\"{message}\"");
                    }
                }
            }
            catch (Exception)
            {
                //not much we can do here...
            }
        }
        private static void UpdatePath()
        {
            CurrentDate = DateTime.Now.Date;
            Path = $"{TrayContext.APP_DATA_PATH}\\{CurrentDate:yyyy-MM-dd}.log";
            Clean();
        }
        private static void Clean()
        {
            var pathBase = TrayContext.APP_DATA_PATH;
            foreach (var f in Directory.EnumerateFiles(pathBase))
            {
                var fn = f.Substring(pathBase.Length + 1);
                if (fn.EndsWith(".log"))
                {
                    var dtPart = fn.Substring(0, fn.Length - 4);
                    if (DateTime.TryParse(dtPart, out var dt))
                    {
                        try
                        {
                            if ((CurrentDate - dt).TotalDays > 7)
                            {
                                File.Delete(f);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log(ex.Message);
                        }
                    }
                }
            }
            try
            {
                Data.CleanLogs();
            }
            catch (Exception)
            {
                //??
            }
        }
    }

}
