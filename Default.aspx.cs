using System;
using System.Configuration;
using System.Web.UI;

namespace Reporting.Legacy.Web
{
    public partial class Default : Page
    {
        protected global::System.Web.UI.WebControls.Literal OperatorName;
        protected global::System.Web.UI.WebControls.Literal ReportingMode;
        protected global::System.Web.UI.WebControls.Literal ArchiveRoot;
        protected global::System.Web.UI.WebControls.HyperLink MonthlySalesLink;

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["LastReportingPage"] = "Default";
            OperatorName.Text = string.IsNullOrWhiteSpace(Context.User.Identity.Name)
                ? "Local demo operator"
                : Context.User.Identity.Name;
            ReportingMode.Text = ConfigurationManager.AppSettings["ReportingMode"];
            ArchiveRoot.Text = ConfigurationManager.AppSettings["ReportArchiveRoot"];
        }
    }
}
