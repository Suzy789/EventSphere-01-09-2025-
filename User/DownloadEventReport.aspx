<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="DownloadEventReport.aspx.cs" Inherits="Authentication.User.DownloadEventReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div class="container mt-5">
        <h2 class="text-center text-primary mb-4">Generate Event Reports</h2>

        <div class="row justify-content-center">
            <div class="col-md-8 p-4 bg-light border rounded shadow-sm">
                <div class="mb-3">
                    <label for="ddlEvents" class="form-label">Select Event:</label>
                    <asp:DropDownList ID="ddlEvents" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>

                <div class="mb-3">
                    <label for="ddlType" class="form-label">Report Type:</label>
                    <asp:DropDownList ID="ddlType" runat="server" CssClass="form-select">
                        <asp:ListItem Text="-- Select --" Value="" />
                        <asp:ListItem Text="Participant Details" Value="Participants" />
                        <asp:ListItem Text="Volunteer Details" Value="Volunteers" />
                    </asp:DropDownList>
                </div>

                <div class="text-center">
                    <asp:Button ID="btnDownload" runat="server" CssClass="btn btn-success px-4"
                        Text="Download Excel Report" OnClick="btnDownload_Click" />
                </div>

                <asp:Label ID="lblMessage" runat="server" CssClass="text-danger mt-3 d-block text-center"></asp:Label>
            </div>
        </div>
    </div>
</asp:Content>
