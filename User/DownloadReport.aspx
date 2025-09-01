<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="DownloadReport.aspx.cs" Inherits="Authentication.User.DownloadReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="container mt-5">
        <h2 class="text-center mb-4 text-info">Download Event Performance Report</h2>
        <div class="text-center">
            <asp:Button ID="btnDownloadExcel" runat="server" CssClass="btn btn-primary" Text="Download Report as Excel"
                OnClick="btnDownloadExcel_Click" />
        </div>
        <asp:Label ID="lblMessage" runat="server" CssClass="text-success d-block mt-3 text-center"></asp:Label>
    </div>
</asp:Content>
