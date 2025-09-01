<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/AdminMasterPage.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="Authentication.Admin.Reports" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   
    <h2>Admin Reports Download</h2>

    <asp:Label ID="lblError" runat="server" CssClass="text-danger mb-3"></asp:Label>

    <div class="mb-3">
        <label>Start Date:</label>
        <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" placeholder="YYYY-MM-DD" />
    </div>
    <div class="mb-3">
        <label>End Date:</label>
        <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" placeholder="YYYY-MM-DD" />
    </div>

    <div class="mb-3">
        <label>Select Event:</label>
        <asp:DropDownList ID="ddlEvents" runat="server" CssClass="form-control" />
    </div>

    <asp:Button ID="btnExportEvents" runat="server" Text="Export Events Report" CssClass="btn btn-primary mb-2" OnClick="btnExportEvents_Click" />
    <asp:Button ID="btnExportParticipants" runat="server" Text="Export Participants Report" CssClass="btn btn-primary mb-2" OnClick="btnExportParticipants_Click" />
    <asp:Button ID="btnExportVolunteers" runat="server" Text="Export Volunteers Report" CssClass="btn btn-primary mb-2" OnClick="btnExportVolunteers_Click" />
    <asp:Button ID="btnExportFullReport" runat="server" Text="Export Full Report" CssClass="btn btn-success" OnClick="btnExportFullReport_Click" />
</asp:Content>
