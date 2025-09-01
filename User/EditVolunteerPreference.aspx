<%@ Page Title="Edit Volunteering Preference" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="EditVolunteerPreference.aspx.cs" Inherits="Authentication.User.EditVolunteerPreference" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <div class="container mt-5">
        <h2 class="text-primary mb-4">Edit Volunteering Preference</h2>

        <!-- Message Panel -->
        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="alert alert-info" role="alert">
            <asp:Label ID="lblMessage" runat="server" CssClass="fw-bold"></asp:Label>
        </asp:Panel>

        <!-- Edit Form -->
        <asp:Panel ID="pnlEdit" runat="server" Visible="true" CssClass="card p-4 shadow-sm">
            <asp:Label ID="lblEventTitle" runat="server" CssClass="fw-bold mb-3 d-block"></asp:Label>

            <div class="mb-3">
                <label for="ddlRoles" class="form-label fw-bold">Select Role</label>
                <asp:DropDownList ID="ddlRoles" runat="server" CssClass="form-select"></asp:DropDownList>
            </div>

            <asp:Button ID="btnSave" runat="server" Text="Save Preference"
                OnClick="btnSave_Click" CssClass="btn btn-primary" />
        </asp:Panel>
    </div>

</asp:Content>
