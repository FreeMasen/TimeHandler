using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace TimeHandler
{

    public class UserConfig : IEnumerable<Tuple<string, string>>
    {

        public ReportConfig ReportConfig = new ReportConfig();
        public JiraConfig JiraConfig = new JiraConfig();
        public PomorodoConfig PomorodoConfig = new PomorodoConfig();
        public UserConfig()
        {
            this.ReportConfig = new ReportConfig();
            this.JiraConfig = new JiraConfig();
            this.PomorodoConfig = new PomorodoConfig();
        }
        public UserConfig(
            string reportPath,
            string jiraUser,
            string jiraAuth,
            int pomWork,
            int pomShortBrk,
            int pomLongBrk
        )
        {
            this.ReportConfig = new ReportConfig
            {
                StoragePath = reportPath
            };
            this.JiraConfig = new JiraConfig
            {
                JiraAuthToken = jiraAuth,
                JiraUserName = jiraUser,
            };
            this.PomorodoConfig = new PomorodoConfig
            {
                LongBreakLength = pomLongBrk,
                ShortBreakLength = pomShortBrk,
                WorkLength = pomWork,
            };
        }

        IEnumerator<Tuple<string, string>> IEnumerable<Tuple<string, string>>.GetEnumerator()
        {
            return new ConfigEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ConfigEnumerator(this);
        }
    }

    public class ReportConfig
    {
        public string StoragePath = TrayContext.MY_DOCUMENTS_PATH;
    }

    public class JiraConfig
    {
        public string JiraUserName;
        public string JiraAuthToken;
    }
    public class PomorodoConfig
    {
        public int WorkLength = 25;
        public int ShortBreakLength = 5;
        public int LongBreakLength = 15;
    }

    public class ConfigEnumerator : IEnumerator<Tuple<String, String>>
    {
        private readonly UserConfig Inner;
        private int index = -1;

        public ConfigEnumerator(UserConfig inner)
        {
            Inner = inner;
        }

        Tuple<string, string> IEnumerator<Tuple<string, string>>.Current => GetCurrent();

        object IEnumerator.Current => GetCurrent();
        private Tuple<String, String> GetCurrent()
        {
            switch (index)
            {
                case 0:
                    return new Tuple<string, string>("ReportPath", Inner.ReportConfig.StoragePath);
                case 1:
                    return new Tuple<string, string>("JiraUserName", Inner.JiraConfig.JiraUserName);
                case 2:
                    return new Tuple<string, string>("JiraAuthToken", Inner.JiraConfig.JiraAuthToken);
                case 3:
                    return new Tuple<string, string>("PomodoroWorkLength", Inner.PomorodoConfig.WorkLength.ToString());
                case 4:
                    return new Tuple<string, string>("PomodoroShortBreakLength", Inner.PomorodoConfig.ShortBreakLength.ToString());
                case 5:
                    return new Tuple<string, string>("PomodoroLongBreakLength", Inner.PomorodoConfig.ShortBreakLength.ToString());
                default:
                    return null;
            }
        }
        bool IEnumerator.MoveNext()
        {
            index += 1;
            if (index > 5)
            {
                return false;
            }
            return true;
        }

        void IEnumerator.Reset()
        {
            index = -1;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ConfigEnumerator() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
