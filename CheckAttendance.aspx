<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CheckAttendance.aspx.cs" Inherits="Authentication.User.CheckAttendance" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="container py-4">
        <h2 class="mb-4">📋 Check Attendance</h2>

        <!-- Attendance Cards -->
        <asp:Repeater ID="rptAttendance" runat="server">
            <ItemTemplate>
                <div class="card mb-3 shadow-sm">
                    <div class="card-body">
                        <h5 class="card-title"><%# Eval("Title") %></h5>
                        <p class="card-text">
                            Date: <%# Eval("Date", "{0:dd MMM yyyy}") %><br />
                            Status: 
                            <%# Eval("AttendanceStatus").ToString() == "Present" ? "<span class='text-success fw-bold'>Present</span>" : Eval("AttendanceStatus").ToString() == "Absent" ? "<span class='text-danger fw-bold'>Absent</span>" : "<span class='text-secondary fw-bold'>Pending</span>" %>
                        </p>
                        <p class="card-text">
                            Marked At: <%# Eval("MarkedAt") != DBNull.Value ? string.Format("{0:dd MMM yyyy hh:mm tt}", Eval("MarkedAt")) : "-" %>
                        </p>
                        <asp:Button ID="btnReportIssue" runat="server" Text="Report Issue"
                            CommandArgument='<%# Eval("EventID") %>'
                            CssClass="btn btn-warning btn-sm"
                            OnClick="btnReportIssue_Click" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <!-- Report Issue Panel -->
        <asp:Panel ID="pnlReportIssue" runat="server" CssClass="card shadow-sm p-3 mb-3" Visible="false">
            <h4>📝 Report Attendance Issue</h4>
            <asp:HiddenField ID="hfEventID" runat="server" />
            <div class="mb-3">
                <label for="txtIssue" class="form-label">Describe your issue:</label>
                <asp:TextBox ID="txtIssue" runat="server" TextMode="MultiLine" CssClass="form-control" Rows="4" Placeholder="Enter details here..."></asp:TextBox>
            </div>
            <asp:Button ID="btnSubmitIssue" runat="server" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmitIssue_Click" />
            <asp:Button ID="btnCancelIssue" runat="server" Text="Cancel" CssClass="btn btn-secondary ms-2" OnClick="btnCancelIssue_Click" />
            <asp:Label ID="lblIssueMessage" runat="server" CssClass="text-success mt-2"></asp:Label>
        </asp:Panel>

        <asp:Label ID="lblMessage" runat="server" CssClass="text-success mt-3"></asp:Label>
    </div>

</asp:Content>
