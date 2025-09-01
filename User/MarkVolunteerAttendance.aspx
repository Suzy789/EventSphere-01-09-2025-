<%@ Page Title="Mark Attendance" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="MarkAttendance.aspx.cs" Inherits="Authentication.User.MarkAttendance" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">
        <h2 class="mb-4">📋 Manage Volunteer Attendance</h2>

        <!-- Event Selection -->
        <asp:DropDownList ID="ddlEvents" runat="server" AutoPostBack="true"
            OnSelectedIndexChanged="ddlEvents_SelectedIndexChanged"
            CssClass="form-select w-50 mb-3" />

        <!-- Attendance Code Input -->
        <div class="input-group w-50 mb-3">
            <asp:TextBox ID="txtCode" runat="server" CssClass="form-control" Placeholder="Enter Attendance Code" />
            <asp:Button ID="btnVerify" runat="server" Text="Verify & Load Volunteers"
                CssClass="btn btn-primary" OnClick="btnVerify_Click" />
        </div>

        <!-- Status Message -->
        <asp:Label ID="lblMessage" runat="server" CssClass="d-block mb-3 fw-bold" ForeColor="Red" />

        <!-- Volunteers Not Yet Marked -->
        <asp:GridView ID="gvVolunteers" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"
            OnRowCommand="gvVolunteers_RowCommand" Visible="false">
            <Columns>
                <asp:BoundField HeaderText="Name" DataField="FullName" />
                <asp:BoundField HeaderText="Email" DataField="Email" />
                <asp:TemplateField HeaderText="Mark Attendance">
                    <ItemTemplate>
                        <asp:Button ID="btnMark" runat="server" Text="Mark" CommandName="Mark"
                            CommandArgument='<%# Eval("VolunteerID") %>'
                            CssClass="btn btn-success btn-sm" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <!-- Volunteers Already Marked -->
        <h4 class="mt-5">✅ Volunteers with Marked Attendance</h4>
        <asp:GridView ID="gvMarkedVolunteers" runat="server" AutoGenerateColumns="False"
            CssClass="table table-bordered">
            <Columns>
                <asp:BoundField HeaderText="Name" DataField="FullName" />
                <asp:BoundField HeaderText="Email" DataField="Email" />
                <asp:BoundField HeaderText="Marked At" DataField="MarkedAt" DataFormatString="{0:dd MMM yyyy hh:mm tt}" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
