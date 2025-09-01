<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ViewFeedback.aspx.cs" Inherits="Authentication.User.ViewFeedback" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="container mt-4">
        <h2 class="text-center text-primary">View Feedback</h2>

        <!-- Event Dropdown -->
        <asp:DropDownList ID="ddlEvents" runat="server" AutoPostBack="true"
            CssClass="form-select my-3" OnSelectedIndexChanged="ddlEvents_SelectedIndexChanged" />

        <!-- Feedback List -->
        <asp:Repeater ID="rptFeedback" runat="server">
            <HeaderTemplate>
                <div class="list-group">
            </HeaderTemplate>
            <ItemTemplate>
                <div class="list-group-item shadow-sm mb-2">
                    <h5 class="mb-1"><%# Eval("UserFullName") %> (<%# Eval("UserRole") %>)</h5>
                    <small class="text-muted"><%# Eval("SubmittedAt", "{0:dd MMM yyyy hh:mm tt}") %></small>
                    <p class="mt-2"><%# Eval("Message").ToString().Replace("\n", "<br/>") %></p>
                </div>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>

        <asp:Label ID="lblMsg" runat="server" CssClass="text-danger fw-bold mt-3 d-block" Visible="false" />
    </div></asp:Content>
