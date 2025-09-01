<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ViewVolunteersApplication.aspx.cs" Inherits="Authentication.User.ViewVolunteerApplication" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container py-4">

    <!-- Page Heading -->
    <div class="banner mb-4">
        <h1 class="mb-1">Volunteers for Event</h1>
        <p class="text-muted">Select an event to view its volunteer applications by category.</p>
    </div>

    <!-- Event Selection -->
    <div class="mb-4">
        <asp:Label ID="lblSelectEvent" runat="server" AssociatedControlID="ddlEvents"
            CssClass="form-label fw-bold" Text="Select Event:"></asp:Label>
        <asp:DropDownList ID="ddlEvents" runat="server" AutoPostBack="true"
            OnSelectedIndexChanged="ddlEvents_SelectedIndexChanged"
            CssClass="form-select">
        </asp:DropDownList>
    </div>

    <!-- Event Title -->
    <h4 id="lblEventTitle" runat="server" class="highlight mb-4"></h4>

    <!-- Categories & Volunteers -->
    <asp:Repeater ID="rptCategories" runat="server" OnItemDataBound="rptCategories_ItemDataBound">
        <ItemTemplate>
            <!-- Category Heading -->
            <h5 class="sub-heading mt-4"><%# Eval("CategoryName") %></h5>

            <!-- Volunteers Table -->
            <asp:Repeater ID="rptVolunteers" runat="server" OnItemCommand="rptVolunteers_ItemCommand">
                <HeaderTemplate>
                    <table class="table table-bordered table-striped">
                        <thead class="table-light">
                            <tr>
                                <th>Name</th>
                                <th>Email</th>
                                <th>Status</th>
                                <th style="width: 200px;">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("FullName") %></td>
                        <td><%# Eval("Email") %></td>
                        <td><%# Eval("Status") %></td>
                        <td>
                            <!-- Pending Actions -->
                            <asp:Panel runat="server" Visible='<%# Eval("Status").ToString() == "Pending" || Eval("Status").ToString() == "Pending Approval" %>'>
                                <asp:Button ID="btnApprove" runat="server" Text="Approve"
                                    CssClass="btn btn-success btn-sm me-1"
                                    CommandName="Approve" CommandArgument='<%# Eval("ApplicationID") %>' />
                                <asp:Button ID="btnReject" runat="server" Text="Reject"
                                    CssClass="btn btn-danger btn-sm"
                                    CommandName="Reject" CommandArgument='<%# Eval("ApplicationID") %>' />
                            </asp:Panel>

                            <!-- Approved -->
                            <asp:Label runat="server" CssClass="badge bg-success"
                                Visible='<%# Eval("Status").ToString() == "Approved" %>'
                                Text="Accepted"></asp:Label>

                            <!-- View Details (Always) -->
                            <asp:Button ID="btnViewDetails" runat="server" Text="View Details"
                                CssClass="btn btn-outline-primary btn-sm ms-1"
                                CommandName="ViewDetails" CommandArgument='<%# Eval("ApplicationID") %>' />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </ItemTemplate>
    </asp:Repeater>

    <!-- Messages -->
    <asp:Label ID="lblNoVolunteers" runat="server"
        Text="No volunteer applications yet."
        CssClass="alert alert-warning d-block mt-4" Visible="false"></asp:Label>

    <asp:Label ID="lblMessage" runat="server"
        CssClass="alert d-block mt-3" Visible="false"></asp:Label>

</div>
</asp:Content>
