<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ViewApplications.aspx.cs" Inherits="Authentication.User.ViewApplications" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="container mt-4">
    <h3 class="mb-4">My Organized Events</h3>

    <asp:Label ID="lblNoEvent" runat="server" CssClass="text-danger" Visible="false"
        Text="You haven't organized any events yet."></asp:Label>

  <asp:Repeater ID="rptMyEvents" runat="server" OnItemCommand="rptMyEvents_ItemCommand">

    <HeaderTemplate>
        <table class="table table-bordered table-striped">
            <thead class="table-dark">
                <tr>
                    <th>Title</th>
                    <th>Date</th>
                    <th>Location</th>
                    <th>Status</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>

    <ItemTemplate>
        <!-- Event Row -->
        <tr>
            <td><%# Eval("Title") %></td>
            <td><%# Eval("Date", "{0:dd-MMM-yyyy}") %></td>
            <td><%# Eval("Location") %></td>
            <td><%# Eval("Status") %></td>
            <td>
                <asp:HiddenField ID="hfEventId" runat="server" Value='<%# Eval("EventID") %>' />

                <asp:Button ID="btnEdit" runat="server" Text="Edit" 
                    CommandName="EditEvent" CommandArgument='<%# Eval("EventID") %>' 
                    CssClass="btn btn-sm btn-primary me-1" />

                <asp:Button ID="btnDelete" runat="server" Text="Delete" 
                    CommandName="DeleteEvent" CommandArgument='<%# Eval("EventID") %>' 
                    CssClass="btn btn-sm btn-danger me-1"
                    OnClientClick="return confirm('Are you sure you want to delete this event?');" />

                <asp:Button ID="btnIssues" runat="server" Text="View Issues" 
                    CommandName="ToggleIssues" CommandArgument='<%# Eval("EventID") %>' 
                    CssClass="btn btn-sm btn-warning me-1" />

                <asp:Button ID="btnSubCategories" runat="server" Text="Manage Sub-Categories" 
                    CommandName="ToggleSubCategories" CommandArgument='<%# Eval("EventID") %>' 
                    CssClass="btn btn-sm btn-success" />
            </td>
        </tr>

        <!-- Inline Edit Panel for Event -->
        <tr>
            <td colspan="5">
                <asp:Panel ID="pnlEditEvent" runat="server" Visible="false" CssClass="p-3 bg-light border rounded">
                    <h6>Edit Event</h6>

                    <asp:HiddenField ID="hfEditEventId" runat="server" />

                    <div class="mb-2">
                        <label>Title:</label>
                        <asp:TextBox ID="txtEditTitle" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-2">
                        <label>Date:</label>
                        <asp:TextBox ID="txtEditDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="mb-2">
                        <label>Location:</label>
                        <asp:TextBox ID="txtEditLocation" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="mb-2">
                        <label>Status:</label>
                        <asp:DropDownList ID="ddlEditStatus" runat="server" CssClass="form-select">
                            <asp:ListItem Text="Upcoming" Value="Upcoming"></asp:ListItem>
                            <asp:ListItem Text="Ongoing" Value="Ongoing"></asp:ListItem>
                            <asp:ListItem Text="Completed" Value="Completed"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <asp:Button ID="btnUpdateEvent" runat="server" Text="Save Changes"
                        CssClass="btn btn-success btn-sm"
                        CommandName="UpdateEvent" CommandArgument='<%# Eval("EventID") %>' />

                    <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel"
                        CssClass="btn btn-secondary btn-sm ms-2"
                        CommandName="CancelEdit" />
                </asp:Panel>
            </td>
        </tr>

        <!-- Attendance Issues -->
        <tr>
            <td colspan="5">
                <asp:Panel ID="pnlIssues" runat="server" Visible="false" CssClass="p-3 bg-light border rounded">
                    <h6 class="mb-2">Attendance Issues</h6>
                    <asp:Label ID="lblNoIssues" runat="server" CssClass="text-muted" Visible="false" 
                        Text="No issues reported for this event."></asp:Label>

                    <asp:Repeater ID="rptIssues" runat="server">
                        <HeaderTemplate>
                            <table class="table table-sm table-bordered">
                                <thead class="table-secondary">
                                    <tr>
                                        <th>Volunteer Name</th>
                                        <th>Issue</th>
                                        <th>Reported At</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
    <tr>
        <td><%# Eval("VolunteerName") %></td>
        <td><%# Eval("IssueDescription") %></td>
        <td><%# Eval("ReportedAt", "{0:dd-MMM-yyyy HH:mm}") %></td>
    </tr>
</ItemTemplate>

                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </asp:Panel>
            </td>
        </tr>
<!-- Sub-Categories -->
<tr>
    <td colspan="5">
        <!-- Sub-Categories Panel -->
        <asp:Panel ID="pnlSubCategories" runat="server" Visible="false" CssClass="p-3 bg-light border rounded">
            <h6 class="mb-2">Event Sub-Categories</h6>
            <asp:Label ID="lblNoSubCategories" runat="server" CssClass="text-muted" Visible="false" 
                Text="No sub-categories created for this event."></asp:Label>

            <asp:Repeater ID="rptSubCategories" runat="server" OnItemCommand="rptSubCategories_ItemCommand">
                <HeaderTemplate>
                    <table class="table table-sm table-bordered">
                        <thead class="table-secondary">
                            <tr>
                                <th>Sub-Category Name</th>
                                <th>Team Size</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>

                <ItemTemplate>
                    <tr>
                        <td><%# Eval("SubCategoryName") %></td>
                        <td><%# Eval("TeamSize") %></td>
                        <td>
                            <asp:Button ID="btnEditSubCat" runat="server" Text="Edit"
                                CommandName="EditSubCategory" CommandArgument='<%# Eval("SubCategoryID") %>'
                                CssClass="btn btn-sm btn-primary me-1" />

                            <asp:Button ID="btnDeleteSubCat" runat="server" Text="Delete"
                                CommandName="DeleteSubCategory" CommandArgument='<%# Eval("SubCategoryID") %>'
                                CssClass="btn btn-sm btn-danger"
                                OnClientClick="return confirm('Delete this sub-category?');" />
                        </td>
                    </tr>

                    <!-- Inline Edit Panel for Sub-Category -->
                    <tr>
                        <td colspan="3">
                            <asp:Panel ID="pnlEditSubCategory" runat="server" Visible="false" CssClass="p-3 bg-light border rounded mt-2">
                                <h6>Edit Sub-Category</h6>

                                <asp:HiddenField ID="hfEditSubCatId" runat="server" />

                                <div class="mb-2">
                                    <label>Sub-Category Name:</label>
                                    <asp:TextBox ID="txtEditSubCatName" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                                <div class="mb-2">
                                    <label>Team Size:</label>
                                    <asp:TextBox ID="txtEditTeamSize" runat="server" CssClass="form-control" TextMode="Number"></asp:TextBox>
                                </div>

                                <asp:Button ID="btnUpdateSubCat" runat="server" Text="Save Changes"
                                    CssClass="btn btn-success btn-sm"
                                    CommandName="UpdateSubCategory" CommandArgument='<%# Eval("SubCategoryID") %>' />

                                <asp:Button ID="btnCancelSubCatEdit" runat="server" Text="Cancel"
                                    CssClass="btn btn-secondary btn-sm ms-2"
                                    CommandName="CancelSubCategoryEdit" />
                            </asp:Panel>
                        </td>
                    </tr>
                </ItemTemplate>

                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
            </asp:Repeater>

            <!-- Add Sub-Category Panel (inline, hidden initially) -->
            <asp:Panel ID="pnlAddSubCategory" runat="server" Visible="false" CssClass="p-3 bg-light border rounded mt-2">
                <asp:HiddenField ID="hfParentEventId_Add" runat="server" />
                <div class="mb-2">
                    <label>Sub-Category Name:</label>
                    <asp:TextBox ID="txtNewSubCatName" runat="server" CssClass="form-control" Placeholder="Sub-Category Name"></asp:TextBox>
                </div>
                <div class="mb-2">
                    <label>Team Size:</label>
                    <asp:TextBox ID="txtNewTeamSize" runat="server" CssClass="form-control" TextMode="Number" Placeholder="Team Size"></asp:TextBox>
                </div>
                <asp:Button ID="btnSaveSubCategory" runat="server" Text="Save"
                    CommandName="SaveSubCategory" CssClass="btn btn-primary btn-sm" />
                <asp:Button ID="btnCancelAddSubCategory" runat="server" Text="Cancel"
                    CommandName="CancelAddSubCategory" CssClass="btn btn-secondary btn-sm ms-2" />
            </asp:Panel>

            <!-- Button to open add panel -->
            <asp:Button ID="btnAddSubCategory" runat="server" Text="Add New Sub-Category"
                CommandName="ShowAddSubCategory" CommandArgument='<%# Eval("EventID") %>'
                CssClass="btn btn-sm btn-success mt-2" />
        </asp:Panel>
    </td>
</tr>

    </td>
</tr>

    </ItemTemplate>

    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
</asp:Repeater>

</div>
</asp:Content>



