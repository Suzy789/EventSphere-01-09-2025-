<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="MyBookmarkedEvents.aspx.cs" Inherits="Authentication.User.MyBookmarkedEvents" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div class="container py-4">
        <div class="bg-light p-4 rounded shadow-sm">
            <h2 class="text-center text-primary mb-4">My Bookmarked Events</h2>
           <asp:GridView ID="gvBookmarks" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered"  OnRowCommand="gvBookmarks_RowCommand">

                <Columns>
                    <asp:BoundField DataField="Title" HeaderText="Event Title" />
                    <asp:BoundField DataField="Date" HeaderText="Date" DataFormatString="{0:dd MMM yyyy}" />
                    <asp:BoundField DataField="Location" HeaderText="Location" />

                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <a href='<%# "EventDetails.aspx?eventid=" + Eval("EventID") %>' class="btn btn-primary btn-sm me-2">View</a>
                            <asp:LinkButton ID="btnRemove" runat="server" CssClass="btn btn-danger btn-sm"
                                CommandName="RemoveBookmark" CommandArgument='<%# Eval("EventID") %>'
                                OnClientClick="return confirm('Are you sure to remove this bookmark?');">Remove</asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-success fw-bold d-block mt-3" />
            <div class="text-center mt-4">
    <asp:Button ID="btnBack" runat="server" Text="Back to Dashboard" CssClass="btn btn-secondary" PostBackUrl="~/User/UserDashboard.aspx" />
</div>
        </div>
    </div>
</asp:Content>
