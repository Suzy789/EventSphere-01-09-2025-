<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="MyOrganizedEvents.aspx.cs" Inherits="Authentication.User.MyOrganizedEvent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  
    <!-- Banner -->
   <div class="banner mb-4">
       <h1 class="mb-1">My Organized Events</h1>
       <p class="text-muted">Manage and track the events you’ve created</p>
   </div>

        <asp:Repeater ID="rptOrganizedEvents" runat="server" OnItemCommand="rptOrganizedEvents_ItemCommand">
           <HeaderTemplate>
            <div style="display:flex; flex-wrap:wrap; gap:15px;">
        </HeaderTemplate>
<ItemTemplate>
    <div style="width:48%; border:1px solid #ddd; border-radius:8px; overflow:hidden; box-shadow:0 2px 6px rgba(0,0,0,0.1);">

        <!-- Event Image (updated) -->
        <asp:Image ID="imgEventThumb" runat="server"
                   Width="100%" Height="150"
                   Style="object-fit:cover;"
                   AlternateText="Event Image"
                   ImageUrl='<%# string.IsNullOrEmpty(Eval("EventBanner") as string) ? "~/images/default-event.jpg" : Eval("EventBanner").ToString()  %>' />

        <!-- Event Info -->
        <div class="card">
            <h3><%# Eval("Title") %></h3>
            <p><strong>Date:</strong> <%# Eval("Date", "{0:dd MMM yyyy}") %></p>
            <p><strong>Location:</strong> <%# Eval("Location") %></p>
            <p><strong>Description:</strong> <%# Eval("Description") %></p>
            <p><strong>Created On:</strong> <%# Eval("CreatedAt", "{0:dd MMM yyyy}") %></p>

            <a href='ViewVolunteers.aspx?eventid=<%# Eval("EventID") %>' class="submit-button">View Volunteers</a>

            <!-- Add Category Button -->
            <asp:Button ID="btnShowCategoryForm" runat="server" CommandName="ShowCategoryForm" CommandArgument='<%# Eval("EventID") %>' Text="Add Volunteer Category" CssClass="btn btn-warning mt-2" />

            <!-- Category Form Panel -->
            <asp:Panel ID="pnlCategoryForm" runat="server" Visible="false" CssClass="category-form mt-3">
                <asp:Label Text="Category Name" runat="server" CssClass="form-label" />
                <asp:TextBox ID="txtCategoryName" runat="server" CssClass="form-control mb-2" />

                <asp:Label Text="Required Volunteers" runat="server" CssClass="form-label" />
                <asp:TextBox ID="txtRequiredCount" runat="server" CssClass="form-control mb-2" TextMode="Number" />

                <asp:Button ID="btnSaveCategory" runat="server" CommandName="SaveCategory" CommandArgument='<%# Eval("EventID") %>' Text="Save Category" CssClass="btn btn-success" />
            </asp:Panel>
        </div>
    </div>
</ItemTemplate>


            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>

        <asp:Label ID="lblNoEvents" runat="server" Text="You haven’t organized any events yet." Visible="false" CssClass="error-message" />
        <asp:Button ID="Button1" runat="server" Text="Back to Dashboard" CssClass="submit-button" PostBackUrl="~/User/UserDashboard.aspx" />


      

   

</asp:Content>
