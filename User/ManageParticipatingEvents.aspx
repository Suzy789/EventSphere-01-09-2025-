<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ManageParticipatingEvents.aspx.cs" Inherits="Authentication.User.ManageParticipatingEvents" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <div class="container py-4">
        <div class="bg-light p-4 rounded shadow-sm">
            <h2 class="text-center text-primary mb-3">Manage Participating Events</h2>
            <p class="text-center text-muted mb-4">Cancel or edit upcoming event registrations.</p>

            <asp:GridView ID="gvUpcomingEvents" runat="server" AutoGenerateColumns="False"
                CssClass="table table-bordered table-striped text-center"
                OnRowCommand="gvUpcomingEvents_RowCommand">
                <Columns>
                    <asp:BoundField DataField="EventTitle" HeaderText="Event Title" />
                    <asp:BoundField DataField="EventDate" HeaderText="Event Date" DataFormatString="{0:dd MMM yyyy}" />
                    <asp:BoundField DataField="Location" HeaderText="Location" />
                    <asp:BoundField DataField="RegisteredAt" HeaderText="Registered On" DataFormatString="{0:dd MMM yyyy}" />
                    <asp:TemplateField HeaderText="Actions">
                        <ItemTemplate>
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-danger btn-sm"
                                CommandName="CancelRegistration" CommandArgument='<%# Eval("RegistrationID") %>' />
                            <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn btn-warning btn-sm ms-2"
                                CommandName="EditRegistration" CommandArgument='<%# Eval("RegistrationID") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-success fw-bold mt-3 d-block"></asp:Label>
        </div>

<!-- Edit Form -->
<asp:Panel ID="pnlEditForm" runat="server" CssClass="mt-4 p-4 bg-white border rounded shadow text-dark" Visible="false">
    <h4 class="text-primary mb-3">Edit Registration Details</h4>
    <asp:HiddenField ID="hfEditRegistrationID" runat="server" />

    <!-- Show Event Name as read-only -->
    <div class="mb-3">
        <label class="form-label fw-bold">Event Name</label>
        <asp:Label ID="lblEventTitle" runat="server" CssClass="form-control bg-light" />
    </div>

    <!-- Role / SubCategory -->
    <div class="mb-3">
        <label class="form-label">Role / Category</label>
        <asp:DropDownList ID="ddlSubCategory" runat="server" CssClass="form-control" AppendDataBoundItems="true">
            <asp:ListItem Text="-- Select --" Value="" />
        </asp:DropDownList>
    </div>

    <div class="row mb-3">
        <div class="col-md-6">
            <label class="form-label">Team Name</label>
            <asp:TextBox ID="txtTeamName" runat="server" CssClass="form-control" Placeholder="Enter Team Name" />
        </div>
        <div class="col-md-6">
            <label class="form-label">Number of Team Members</label>
            <asp:TextBox ID="txtTeamSize" runat="server" CssClass="form-control" TextMode="Number" Placeholder="Enter number" />
        </div>
    </div>

    <div class="mb-3">
        <label class="form-label">College / Organization</label>
        <asp:TextBox ID="txtCollege" runat="server" CssClass="form-control" Placeholder="Enter college or organization name" />
    </div>

    <div class="mb-3">
        <label class="form-label">Payment Mode</label>
        <asp:DropDownList ID="ddlPaymentMode" runat="server" CssClass="form-control">
            <asp:ListItem Text="-- Select Payment Mode --" Value="" />
            <asp:ListItem Text="Online" Value="Online" />
            <asp:ListItem Text="Offline" Value="Offline" />
            <asp:ListItem Text="Not Applicable" Value="NA" />
        </asp:DropDownList>
    </div>

    <!-- Save button -->
          <asp:Button ID="btnUpdate" runat="server" Text="Update Registration" CssClass="btn btn-success" OnClick="btnUpdate_Click" />
            <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="btn btn-secondary ms-2" OnClick="btnCancelEdit_Click" />
</asp:Panel>


      
     
    </div>
</asp:Content>
