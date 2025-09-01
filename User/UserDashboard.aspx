<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master"
    AutoEventWireup="true"
    CodeBehind="UserDashboard.aspx.cs"
    Inherits="Authentication.User.UserDashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">  <!-- Dashboard Banner -->
    <!-- 🌟 Dashboard Banner -->
<div class="banner text-center py-4">
    <h1 class="fw-bold">Welcome to EventSphere!</h1>
    <p class="text-muted">Manage your events, volunteering, and participation in one place.</p>
</div>

<!-- 📊 Quick Stats Section -->
<div class="grid grid-3">
    <!-- Organizer Stats -->
    <div class="card" runat="server" id="phEventsCreated" visible="false">
        <h2><asp:Literal ID="litEventsCreated" runat="server" /></h2>
        <p>Events Created</p>
    </div>
    <div class="card" runat="server" id="phParticipantsRegistered" visible="false">
        <h2><asp:Literal ID="litParticipantsRegistered" runat="server" /></h2>
        <p>Participants Registered</p>
    </div>

    <!-- Volunteer Stats -->
    <div class="card" runat="server" id="phEventsVolunteered" visible="false">
        <h2><asp:Literal ID="litEventsVolunteered" runat="server" /></h2>
        <p>Events Volunteered</p>
    </div>
    <div class="card" runat="server" id="phDutiesAssigned" visible="false">
        <h2><asp:Literal ID="litDutiesAssigned" runat="server" /></h2>
        <p>Duties Assigned</p>
    </div>

    <!-- Participant Stats -->
    <div class="card" runat="server" id="phParticipationCount" visible="false">
        <h2><asp:Literal ID="litParticipationCount" runat="server" /></h2>
        <p>Events Participated</p>
    </div>

    <!-- Common (Volunteer + Participant) -->
    <div class="card" runat="server" id="phCertificatesEarned" visible="false">
        <h2><asp:Literal ID="litCertificatesEarned" runat="server" /></h2>
        <p>Certificates Earned</p>
    </div>
</div>

<!-- ⚡ Quick Actions Section -->
<h2 class="mt-5">Quick Actions</h2>
<div class="grid grid-2">

   <!-- 🟢 Organizer Dashboard -->
<!-- 🔷 Event Management -->
<div class="card" runat="server" id="phCreateEvent" visible="false">
    <h3>Create New Event</h3>
    <p>Host and configure a new event with details and volunteer categories.</p>
    <a href="CreateEvent.aspx" class="submit-button">Create</a>
</div>

<div class="card" runat="server" id="phMyOrganizedEvents" visible="false">
    <h3>My Organized Events</h3>
    <p>Manage and monitor events you’ve created.</p>
    <a href="MyOrganizedEvents.aspx" class="submit-button">Manage</a>
</div>

<div class="card" runat="server" id="phBrowseEventsOrganizer" visible="false">
    <h3>Browse Events</h3>
    <p>Explore events from others or view public listings.</p>
    <a href="BrowseEvents.aspx" class="submit-button">Explore</a>
</div>

<!-- 🔷 Volunteer Coordination -->
<div class="card" runat="server" id="Div1" visible="false">
    <h3>Volunteer Applications</h3>
    <p>Review, approve/reject volunteer requests, and assign categories.</p>
    <a href="VolunteerApplications.aspx" class="submit-button">Review</a>
</div>

<div class="card" runat="server" id="phApprovedVolunteerList" visible="false">
    <h3>Approved Volunteer List</h3>
    <p>View volunteers approved under each category.</p>
    <a href="ApprovedVolunteers.aspx" class="submit-button">View</a>
</div>

<!-- 🔷 Participant Management -->
<div class="card" runat="server" id="Div2" visible="false">
    <h3>Participant Manager</h3>
    <p>View and manage participant registrations for your events.</p>
    <a href="ParticipantManager.aspx" class="submit-button">Manage</a>
</div>

<div class="card" runat="server" id="Div3" visible="false">
    <h3>Participant Report</h3>
    <p>Download event-wise participant lists with attendance and feedback.</p>
    <a href="ParticipantReport.aspx" class="submit-button">Download</a>
</div>

<!-- 🔷 Attendance & Reports -->
<div class="card" runat="server" id="phAttendanceManager" visible="false">
    <h3>Attendance Manager</h3>
    <p>Mark volunteer attendance for certificate eligibility.</p>
    <a href="AttendanceManager.aspx" class="submit-button">Mark</a>
</div>

<!--<div class="card" runat="server" id="phDownloadReport" visible="false">
    <h3>Download Event Report</h3>
    <p>Generate detailed reports of event performance.</p>
    <a href="DownloadEventReport.aspx" class="submit-button">Generate</a>
</div>-->

<!-- 🔷 Feedback & Communication -->
<div class="card" runat="server" id="Div4" visible="false">
    <h3>View Feedback</h3>
    <p>See feedback from participants and volunteers.</p>
    <a href="ViewFeedback.aspx" class="submit-button">View</a>
</div>

    <!-- 🔷 Volunteer Coordination -->
    <div class="card" runat="server" id="phVolunteerApplications" visible="false">
        <h3>Volunteer Applications</h3>
        <p>Review, approve/reject volunteer requests, and assign roles.</p>
        <a href="ViewVolunteersApplication.aspx" class="submit-button">Review</a>
    </div>
    <div class="card" runat="server" id="phApprovedVolunteers" visible="false">
        <h3>Approved Volunteer List</h3>
        <p>View volunteers approved under each category.</p>
        <a href="ApprovedVolunteerList.aspx" class="submit-button">View</a>
    </div>

    <!-- 🔷 Participant Management -->
    <div class="card" runat="server" id="phParticipantManager" visible="false">
        <h3>Participant Manager</h3>
        <p>View and manage participant registrations for your events.</p>
        <a href="ParticipantManager.aspx" class="submit-button">Manage</a>
    </div>
    <div class="card" runat="server" id="phParticipantReport" visible="false">
        <h3>Participant Report</h3>
        <p>Download participant lists with attendance and feedback.</p>
        <a href="ParticipantReport.aspx" class="submit-button">Download</a>
    </div>

    <!-- 🔷 Attendance & Reports -->
    <div class="card" runat="server" id="phMarkAttendance" visible="false">
        <h3>Attendance Manager</h3>
        <p>Mark volunteer attendance for certificate eligibility.</p>
        <a href="MarkVolunteerAttendance.aspx" class="submit-button">Mark</a>
    </div>
    <div class="card" runat="server" id="phEventReport" visible="false">
        <h3>Download Event Report</h3>
        <p>Generate detailed event performance reports.</p>
        <a href="DownloadReport.aspx" class="submit-button">Generate</a>
    </div>

    <div class="card" runat="server" id="phEventDetailsExport" visible="false">
    <h3>Export Participant/Volunteer Details</h3>
    <p>Download Excel reports of registered participants or approved volunteers for your events.</p>
    <a href="DownloadEventReport.aspx" class="submit-button">Download</a>
</div>
    <!-- 🔷 Feedback & Communication -->
    <div class="card" runat="server" id="phViewFeedback" visible="false">
        <h3>View Feedback</h3>
        <p>See feedback from participants and volunteers.</p>
        <a href="ViewFeedback.aspx" class="submit-button">View</a>
    </div>

    <!-- Volunteer Dashboard Cards -->
    <!-- 🔷 Event Participation -->
    <div class="card" runat="server" id="phBrowseEvents" visible="false">
        <h3>Browse Events</h3>
        <p>Explore available volunteering opportunities and apply.</p>
        <a href="BrowseEvents.aspx" class="submit-button">Explore</a>
    </div>
    <div class="card" runat="server" id="phMyVolunteerEvents" visible="false">
        <h3>My Volunteer Events</h3>
        <p>Track your volunteering and download certificates.</p>
        <a href="MyVolunteerEvents.aspx" class="submit-button">Track</a>
    </div>
    <div class="card" runat="server" id="phMarkParticipantAttendance" visible="false">
    <h3>Mark Participant Attendance</h3>
    <p>Scan QR codes or manage attendance for events assigned to you as Registration Desk volunteer.</p>
    <a href="VolunteerParticipantAttendance.aspx" class="submit-button">Mark Attendance</a>
</div>

    <!-- 🔷 Volunteering Operations -->
    <div class="card" runat="server" id="phAssignedDuties" visible="false">
        <h3>Assigned Duties</h3>
        <p>Check your assigned tasks and download ID card.</p>
        <a href="AssignedDuties.aspx" class="submit-button">View</a>
    </div>
    <div class="card" runat="server" id="phCheckAttendance" visible="false">
        <h3>Check Attendance</h3>
        <p>Confirm your event participation status.</p>
        <a href="CheckAttendance.aspx" class="submit-button">Submit</a>
    </div>
    <div class="card" runat="server" id="phVolunteerStatus" visible="false">
        <h3>Volunteer Status</h3>
        <p>Track your application status and approvals.</p>
        <a href="VolunteerStatus.aspx" class="submit-button">Check</a>
    </div>
    <div class="card" runat="server" id="phManageVolunteerEvents" visible="false">
        <h3>Manage Registered Events</h3>
        <p>Cancel or modify your volunteer registrations.</p>
        <a href="ManageVolunteeringEvents.aspx" class="submit-button">Manage</a>
    </div>

    <!-- 🔷 Engagement -->
    <div class="card" runat="server" id="phFeedbackVolunteer" visible="false">
        <h3>Give Feedback</h3>
        <p>Share your experience as a volunteer.</p>
        <a href="Feedback.aspx" class="submit-button">Submit</a>
    </div>

   <!-- Participant Dashboard Cards -->
<!-- 🔷 Event Discovery -->
<div class="card" runat="server" id="phBrowseEventsParticipant" visible="false">
    <h3>Browse Events</h3>
    <p>Explore and register for upcoming events.</p>
    <a href="BrowseEvents.aspx" class="submit-button">Browse</a>
</div>

<div class="card" runat="server" id="phBookmarkedEvents" visible="false">
    <h3>My Bookmarked Events</h3>
    <p>Save interesting events to revisit and register later.</p>
    <a href="MyBookmarkedEvents.aspx" class="submit-button">View Bookmarks</a>
</div>

<!-- 🔷 My Events -->
<div class="card" runat="server" id="phParticipationHistory" visible="false">
    <h3>My Participated Events</h3>
    <p>View your event history and download certificates.</p>
    <a href="MyParticipatedEvents.aspx" class="submit-button">History</a>
</div>
    <div class="card" runat="server" id="phEventTickets" visible="false">
    <h3>My Event Tickets</h3>
    <p>View and show QR codes for events you’ve registered for.</p>
    <a href="EventParticipantTicket.aspx" class="submit-button">View Tickets</a>
</div>
<div class="card" runat="server" id="phManageParticipation" visible="false">
    <h3>Manage Participating Events</h3>
    <p>Cancel or update your event registrations.</p>
    <a href="ManageParticipatingEvents.aspx" class="submit-button">Edit</a>
</div>

<!-- 🔷 Profile & Engagement (Common) -->
<div class="card">
    <h3>My Profile</h3>
    <p>View or update your personal information.</p>
    <a href="MyProfile.aspx" class="submit-button">Profile</a>
</div>

<div class="card" runat="server" id="phFeedbackParticipant" visible="false">
    <h3>Give Feedback</h3>
    <p>Share your feedback for attended events.</p>
    <a href="Feedback.aspx" class="submit-button">Submit</a>
</div>

<div class="card" runat="server" id="phInviteFriends" visible="false">
    <h3>Invite Friends</h3>
    <p>Copy event links and share with your friends to join.</p>
    <a href="InviteFriends.aspx" class="submit-button">Invite</a>
</div>
</div>

<!-- 📅 Upcoming Events Display -->
<div class="mt-5">
    <h2>Your Upcoming Events</h2>
    <div class="grid grid-2">
        <asp:Literal ID="litUpcomingEvents" runat="server" />
    </div>
</div>

<!-- 🎉 Volunteer Approval Modal -->
<div class="modal fade" id="approvedModal" tabindex="-1" aria-labelledby="approvedModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header bg-success text-white">
                <h5 class="modal-title" id="approvedModalLabel">🎉 Application Approved</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <p>Your volunteer application has been <strong>approved</strong>! 🎉</p>
                <p>You can now download your ID pass.</p>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnDownloadID" runat="server" Text="Download ID Pass"
                    CssClass="btn btn-primary" OnClick="btnDownloadID_Click" />
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
            </div>
        </div>
    </div>
</div>

<!-- 📦 Bootstrap Bundle JS -->
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
<asp:Literal ID="litModalScript" runat="server" />

</asp:Content>
