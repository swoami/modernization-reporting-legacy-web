<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MonthlySales.aspx.cs" Inherits="Reporting.Legacy.Web.Reports.MonthlySales" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Monthly Sales Report</title>
    <style>
        body { font-family: Segoe UI, Arial, sans-serif; margin: 32px; color: #1f2937; }
        table { border-collapse: collapse; min-width: 760px; }
        th, td { border: 1px solid #cbd5e1; padding: 8px 10px; text-align: left; }
        th { background: #e2e8f0; }
        .warning { color: #92400e; margin-bottom: 16px; }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Monthly Sales Report</h1>
        <asp:Label ID="Message" runat="server" CssClass="warning" />
        <asp:GridView ID="MonthlySalesGrid" runat="server" AutoGenerateColumns="true" />
        <p>
            <asp:Button ID="ExportButton" runat="server" Text="Export Orders CSV" OnClick="ExportButton_Click" />
        </p>
    </form>
</body>
</html>
