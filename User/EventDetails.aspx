<%@ Page Title="Event Details" Language="C#" MasterPageFile="~/MasterPage.Master"
    AutoEventWireup="true" CodeBehind="EventDetails.aspx.cs"
    Inherits="Authentication.User.EventDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

<div class="container my-5">
    <div class="text-center mb-4">
        <h1 class="display-5 fw-bold">Event Details</h1>
        <p class="text-muted">Learn more about this event and join as a volunteer or participant.</p>
    </div>

    <!-- Event Banner -->
    <asp:Image ID="imgBanner" runat="server" CssClass="img-fluid rounded shadow mb-4"
               Style="max-height:300px; object-fit:cover;" Visible="false" />

     <!-- Event Info with Edit Support -->
<table class="table table-bordered">
    <tr><th>Title</th>
        <td>
            <asp:Label ID="lblTitle" runat="server" />
            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" Visible="false" />
        </td>
    </tr>
    <tr><th>Description</th>
        <td>
            <asp:Label ID="lblDescription" runat="server" />
            <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" Visible="false" />
        </td>
    </tr>
    <tr><th>Date</th>
        <td>
            <asp:Label ID="lblDate" runat="server" />
            <asp:TextBox ID="txtDate" runat="server" CssClass="form-control" TextMode="Date" Visible="false" />
        </td>
    </tr>
    <tr><th>Start Time</th>
        <td>
            <asp:Label ID="lblStartTime" runat="server" />
            <asp:TextBox ID="txtStartTime" runat="server" CssClass="form-control" Visible="false" />
        </td>
    </tr>
    <tr><th>End Time</th>
        <td>
            <asp:Label ID="lblEndTime" runat="server" />
            <asp:TextBox ID="txtEndTime" runat="server" CssClass="form-control" Visible="false" />
        </td>
    </tr>
    <tr><th>Location</th>
        <td>
            <asp:Label ID="lblLocation" runat="server" />
            <asp:TextBox ID="txtLocation" runat="server" CssClass="form-control" Visible="false" />
        </td>
    </tr>
    <tr><th>Contact Email</th>
        <td>
            <asp:Label ID="lblEmail" runat="server" />
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" Visible="false" />
        </td>
    </tr>
    <tr><th>Contact Phone</th>
        <td>
            <asp:Label ID="lblPhone" runat="server" />
            <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" Visible="false" />
        </td>
    </tr>
    <tr><th>Registration Fee</th>
        <td>
            <asp:Label ID="lblFee" runat="server" />
            <asp:TextBox ID="txtFee" runat="server" CssClass="form-control" Visible="false" />
        </td>
    </tr>
    <tr><th>Category</th><td><asp:Label ID="lblCategory" runat="server" /></td></tr>
    <tr><th>Mode</th><td><asp:Label ID="lblMode" runat="server" /></td></tr>
    <tr><th>Registration Deadline</th><td><asp:Label ID="lblDeadline" runat="server" /></td></tr>
    <tr><th>Status</th><td><asp:Label ID="lblStatus" runat="server" /></td></tr>
    <tr><th>Terms Accepted</th><td><asp:Label ID="lblTerms" runat="server" /></td></tr>
    <tr><th>Organizer</th><td><asp:Label ID="lblOrganizer" runat="server" /></td></tr>
</table>



   <!-- Volunteer Categories -->
<asp:Panel ID="pnlVolunteerCategories" runat="server" Visible="false" CssClass="mb-4">
    <h4 class="mb-3">Available Volunteer Categories</h4>
    <asp:Repeater ID="rptVolunteerCategories" runat="server">
        <HeaderTemplate>
            <table class="table table-striped align-middle shadow-sm rounded">
                <thead class="table-dark">
                    <tr>
                        <th>Category</th>
                        <th>Required</th>
                        <th>Allocated</th>
                        <th>Remaining</th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Eval("CategoryName") %></td>
                <td><%# Eval("RequiredVolunteers") %></td>
                <td><%# Eval("AllocatedVolunteers") %></td>
                <td><%# Eval("RemainingVolunteers") %></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>

    <!-- Actions -->
    <div class="mt-4 d-flex justify-content-between">
        <asp:Button ID="btnBackToDashboard" runat="server" CssClass="btn btn-secondary btn-lg"
            Text="← Back to Dashboard" PostBackUrl="~/User/UserDashboard.aspx" />

        <asp:Button ID="btnApplyVolunteerRedirect" runat="server" CssClass="btn btn-success btn-lg"
            Text="Apply as Volunteer" OnClick="btnApplyVolunteerRedirect_Click" />
    </div>
</asp:Panel>


    <!-- Participant Registration -->

    <asp:Panel ID="pnlParticipant" runat="server" Visible="false" CssClass="mb-4">
        <div class="mt-4 d-flex justify-content-between">
    <asp:Button ID="btnback" runat="server" CssClass="btn btn-secondary btn-lg"
        Text="← Back to Dashboard" PostBackUrl="~/User/UserDashboard.aspx" />
    <asp:Button ID="btnRegisterParticipant" runat="server" Text="Register as Participant" CssClass="btn btn-success" OnClick="btnRegisterParticipant_Click" />
 </div>
    </asp:Panel>

    <!-- Organizer Controls -->
    <div class="mt-4 d-flex flex-wrap justify-content-between align-items-center gap-2">

 <div class="d-flex flex-wrap gap-2">
    <asp:Button ID="btnEdit" runat="server" Text="Edit Event" CssClass="btn btn-warning me-2"
        OnClick="btnEdit_Click" Visible="false" />
    <asp:Button ID="btnUpdate" runat="server" Text="Save Changes" CssClass="btn btn-success me-2"
        OnClick="btnUpdate_Click" Visible="false" />
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-secondary me-2"
        OnClick="btnCancel_Click" Visible="false" />
    <asp:Button ID="btnDelete" runat="server" Text="Delete Event" CssClass="btn btn-danger"
        OnClick="btnDelete_Click" Visible="false"
        OnClientClick="return confirm('Are you sure you want to delete this event?');" />
      <div class="mt-4 d-flex justify-content-between">
          </div>
          <!-- Right: Back Button -->
    <asp:Button ID="btnBackToDashboardOrganizer" runat="server" CssClass="btn btn-outline-primary"
        Text="← Back to Dashboard" PostBackUrl="~/User/UserDashboard.aspx" />
</div>

    <!-- Status Message -->
    <asp:Label ID="lblMessage" runat="server" CssClass="mt-3 d-block" Visible="false"></asp:Label>
</div>
</asp:Content>
