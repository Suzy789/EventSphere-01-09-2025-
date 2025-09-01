<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="BrowseEvents.aspx.cs" Inherits="Authentication.User.BrowseEvents" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<h2 style="text-align:center; color:#e14658;">Browse Events</h2>

<!-- Filter Section -->
<div class="filter-box">
    <div class="filter-content">
        <div class="filter-group">
            <label for="ddlCategory">Category:</label>
            <asp:DropDownList ID="ddlCategory" runat="server" CssClass="inputbox" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed" />
        </div>
        <div class="filter-group">
            <label for="txtLocation">Location:</label>
            <asp:TextBox ID="txtLocation" runat="server" CssClass="inputbox" />
        </div>
        <div>
          <label for="ddlStatus">Status:</label>
    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="inputbox" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed">
        <asp:ListItem Text="All" Value="All" />
        <asp:ListItem Text="Upcoming" Value="Upcoming" />
        <asp:ListItem Text="Ongoing" Value="Ongoing" />
        <asp:ListItem Text="Completed" Value="Completed" />
    </asp:DropDownList>
            </div>
        
            <asp:Button ID="btnFilter" runat="server" Text="Apply Filters" CssClass="submit-button" OnClick="Filter_Changed" />
            <asp:Button ID="btnClear" runat="server" Text="Clear Filters" CssClass="submit-button" OnClick="btnClear_Click" />
        
    </div>
</div>


<!-- Event Cards -->
<asp:Repeater ID="rptEvents" runat="server" OnItemCommand="rptEvents_ItemCommand">
    <HeaderTemplate><div class="grid grid-2" style="margin-top: 20px;"></HeaderTemplate>

    <ItemTemplate>
    <div class="card">
        <h3><%# Eval("Title") %></h3>
        <p><strong>Date:</strong> <%# Eval("Date", "{0:dd MMM yyyy}") %></p>
        <p><strong>Location:</strong> <%# Eval("Location") %></p>
        <p><%# Eval("Description") %></p>

        <asp:Button runat="server" Text="View Details" CommandName="ViewDetails"
            CommandArgument='<%# Eval("EventID") %>' CssClass="submit-button" />

        <asp:PlaceHolder ID="phRegister" runat="server" Visible='<%# Session["Role"] != null && Session["Role"].ToString() != "Organizer" %>'>
            <asp:Button runat="server" Text='<%# GetButtonText(Eval("EventID")) %>' CssClass="submit-button"
                CommandName="Register" CommandArgument='<%# Eval("EventID") %>' />
        </asp:PlaceHolder>
<!-- ✅ Show Bookmark button only to Participants -->
        <asp:PlaceHolder ID="phBookmark" runat="server" Visible='<%# Session["Role"] != null && Session["Role"].ToString() == "Participant" %>'>
            <asp:Button runat="server" Text="Bookmark" CommandName="Bookmark"
                CommandArgument='<%# Eval("EventID") %>' CssClass="submit-button" />
        </asp:PlaceHolder>
        </div>
</ItemTemplate>


    <FooterTemplate></div></FooterTemplate>
</asp:Repeater>


<!-- No Results Message -->
<asp:Label ID="lblNoEvents" runat="server" Text="No events available for the selected filters." Visible="false" CssClass="error-message" />

<!-- Back Button -->
<div style="margin-top: 30px; text-align: center;">
    <asp:Button ID="btnBack" runat="server" Text="Back to Dashboard" CssClass="submit-button" PostBackUrl="~/User/UserDashboard.aspx" />
</div>
</asp:Content>
