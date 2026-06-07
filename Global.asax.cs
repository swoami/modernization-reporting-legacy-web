using System;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using Microsoft.Win32;

namespace Reporting.Legacy.Web
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            var mode = ConfigurationManager.AppSettings["ReportingMode"] ?? "ClassicBatch";
            var workstation = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\SoftwareOne\EcomReporting", "ReportWorkstation", "unregistered");
            TryWriteEvent("Reporting.Legacy.Web started in " + mode + " mode on " + workstation);
        }

        private static void TryWriteEvent(string message)
        {
            try
            {
                EventLog.WriteEntry("Reporting.Legacy.Web", message, EventLogEntryType.Information);
            }
            catch
            {
                // Intentionally ignored: local developer machines may not have the legacy EventLog source registered.
            }
        }
    }
}
