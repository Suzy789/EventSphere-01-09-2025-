<%@ Page Title="Manage Volunteering Events" Language="C#" MasterPageFile="~/MasterPage.Master"
    AutoEventWireup="true" CodeBehind="ManageVolunteeringEvents.aspx.cs"
    Inherits="Authentication.User.ManageVolunteering" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-5">
        <h2 class="mb-4 text-primary fw-bold">My Volunteering Events</h2>

        <!-- Script for showing modal -->
        <asp:Literal ID="litModalScript" runat="server" />

        <!-- No events message -->
        <asp:Panel ID="pnlNoEvents" runat="server" Visible="false">
            <div class="alert alert-info shadow-sm rounded-3">
                You have no volunteering events.
            </div>
        </asp:Panel>

        <!-- Events List -->
        <asp:Repeater ID="rptVolunteeringEvents" runat="server" OnItemCommand="rptVolunteeringEvents_ItemCommand">
            <ItemTemplate>
                <div class="card mb-3 shadow-sm border-0 rounded-4">
                    <div class="card-body">
                        <h5 class="card-title fw-semibold"><%# Eval("Title") %></h5>
                        <p class="card-text mb-1"><strong>Date:</strong> <%# Eval("Date", "{0:MMMM dd, yyyy}") %></p>
                        <p class="card-text mb-1"><strong>Location:</strong> <%# Eval("Location") %></p>
                        <p class="card-text mb-2"><strong>Your Role:</strong> <%# Eval("RoleTitle") %></p>

                        <!-- Status Badge -->
                        <span class='badge <%# 
                            Eval("Status").ToString() == "Approved" ? "bg-success" : 
                            Eval("Status").ToString() == "Pending" ? "bg-warning text-dark" : 
                            "bg-secondary" %>'>
                            <%# Eval("Status") %>
                        </span>

                        <!-- Days Remaining Badge -->
                        <span class='badge <%# 
                            Convert.ToInt32(Eval("DaysRemaining")) == 0 ? "bg-primary" :
                            Convert.ToInt32(Eval("DaysRemaining")) > 0 ? "bg-info text-dark" : 
                            "bg-dark" %>'>
                            <%# Convert.ToInt32(Eval("DaysRemaining")) == 0 ? "Happening Today!" :
                                Convert.ToInt32(Eval("DaysRemaining")) > 0 ? Eval("DaysRemaining") + " days remaining" : 
                                "Completed" %>
                        </span>

                        <div class="mt-3 d-flex justify-content-end gap-2 flex-wrap">
                            <!-- Show Edit Preference only if Pending -->
                            <asp:HyperLink 
                                ID="btnEditPreference" 
                                runat="server"
                                CssClass="btn btn-outline-primary btn-sm"
                                NavigateUrl='<%# $"~/User/EditVolunteerPreference.aspx?eventId={Eval("EventID")}&applicationId={Eval("ApplicationID")}" %>'
                                Visible='<%# Eval("Status").ToString() == "Pending" && Convert.ToInt32(Eval("DaysRemaining")) >= 0 %>'>
                                Edit Preference
                            </asp:HyperLink>

                            <!-- Cancel Button for both Pending & Approved -->
                            <asp:LinkButton 
                                ID="btnCancel" 
                                runat="server"
                                CommandName="CancelRegistration"
                                CommandArgument='<%# Eval("EventID") %>'
                                CssClass="btn btn-danger btn-sm"
                                Visible='<%# (Eval("Status").ToString() == "Approved" || Eval("Status").ToString() == "Pending") && Convert.ToInt32(Eval("DaysRemaining")) >= 0 %>'
                                OnClientClick="return confirm('Are you sure you want to cancel your registration?');">
                                Cancel Registration
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <!-- Event Day Modal -->
    <div class="modal fade" id="eventDayModal" tabindex="-1" aria-labelledby="eventDayModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content shadow-lg rounded-4">
                <div class="modal-header bg-primary text-white rounded-top-4">
                    <h5 class="modal-title" id="eventDayModalLabel">Reminder</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body text-center">
                    🎉 One of your volunteering events is happening today! Please make sure to check in on time.
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-outline-primary" data-bs-dismiss="modal">OK</button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
