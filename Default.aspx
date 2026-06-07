<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Reporting.Legacy.Web.Default" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Legacy Reporting Console</title>
    <style>
        body { font-family: Segoe UI, Arial, sans-serif; margin: 32px; color: #1f2937; }
        .panel { border: 1px solid #cbd5e1; padding: 20px; max-width: 820px; }
        code { background: #f1f5f9; padding: 2px 4px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="panel">
            <h1>Legacy Reporting Console</h1>
            <p>Back-office order and invoice reporting for the warehouse supply store.</p>
            <p><strong>Operator:</strong> <asp:Literal ID="OperatorName" runat="server" /></p>
            <p><strong>Mode:</strong> <asp:Literal ID="ReportingMode" runat="server" /></p>
            <p><strong>Archive root:</strong> <code><asp:Literal ID="ArchiveRoot" runat="server" /></code></p>
            <p>
                <asp:HyperLink ID="MonthlySalesLink" runat="server" NavigateUrl="~/Reports/MonthlySales.aspx">
                    Open monthly sales report
                </asp:HyperLink>
            </p>
        </div>
    </form>
</body>
</html>
