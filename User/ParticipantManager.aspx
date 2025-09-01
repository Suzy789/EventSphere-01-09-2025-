<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ParticipantManager.aspx.cs" Inherits="Authentication.User.ParticipantManager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container my-5 p-4 bg-white shadow rounded">

      <div class="container my-5 p-4 bg-white shadow rounded">
    <h2 class="text-center text-primary fw-bold mb-4">📋 Manage Participant Attendance</h2>

    <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-info d-block text-center" Visible="false" />

    <!-- Event Dropdown -->
    <div class="mb-4">
        <label class="form-label fw-semibold">Select an Event:</label>
        <asp:DropDownList ID="ddlEvents" runat="server" AutoPostBack="true"
            CssClass="form-select"
            OnSelectedIndexChanged="ddlEvents_SelectedIndexChanged">
        </asp:DropDownList>
    </div>

    <!-- Participant Grid (Read-Only Attendance) -->
    <asp:Panel ID="pnlParticipants" runat="server" Visible="false">
        <asp:GridView ID="gvParticipants" runat="server" CssClass="table table-bordered table-hover mt-4"
            AutoGenerateColumns="False" DataKeyNames="RegistrationID, IsPresent, EventDate,UserID">
            <Columns>
                <asp:BoundField DataField="FullName" HeaderText="Participant Name" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="AttendanceCode" HeaderText="Attendance Code / QR" />
                <asp:TemplateField HeaderText="Present?">
                    <ItemTemplate>
                       <asp:CheckBox ID="chkPresent" runat="server"
    Checked='<%# Convert.ToBoolean(Eval("IsPresent")) %>'
    AutoPostBack="true"
    OnCheckedChanged="chkPresent_CheckedChanged" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Certificate">
                    <ItemTemplate>
                        <asp:Button ID="btnIssueCertificate" runat="server" Text="Issue"
                            CssClass="btn btn-sm btn-primary"
                            CommandArgument='<%# Eval("UserID") %>'
                            OnClick="btnIssueCertificate_Click" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel>
</div>

</asp:Content>
