using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;

namespace USFTRestToCSVService
{
    class ExceptionHandler
    {
        #region Parameters
        static string EventLogSource
        {
            get
            {
                return ConfigurationManager.AppSettings["eventlogsource"] ?? "USFTRestToCSV";
            }
        }
        #endregion
        internal static void Output(string msg, Exception exc)
        {
            EventLog.WriteEntry(EventLogSource, msg + FormatForOutput(exc), EventLogEntryType.Error);
        }

        internal static string FormatForOutput(Exception exc)
        {
            var ret = string.Format("\r\n{0}\r\n{1}\r\n{2}", exc.Message, exc.Source, exc.StackTrace);
            if (exc.InnerException != null)
                return ret + "\r\nInner Exception:" + FormatForOutput(exc);
            return ret;
        }
    }
}
