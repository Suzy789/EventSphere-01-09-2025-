<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="CreateEvent.aspx.cs" Inherits="Authentication.User.CreateEvent" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container py-4">
    <div class="p-4 bg-light text-dark rounded shadow-sm">
        <h2 class="text-center text-primary mb-4">Create New Event</h2>

        <!-- Validation Summary -->
        <asp:ValidationSummary ID="vsSummary" runat="server" CssClass="alert alert-danger" 
            HeaderText="Please fix the following:" />

        <div class="row">

            <!-- ===== 1. Event Basics ===== -->
            <div class="col-md-12 mb-3">
                <label for="txtTitle" class="form-label fw-bold">Event Title</label>
                <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvTitle" runat="server" ControlToValidate="txtTitle"
                    ErrorMessage="Title is required." CssClass="text-danger" Display="Dynamic" />
            </div>

            <div class="col-md-12 mb-3">
                <label for="txtDescription" class="form-label fw-bold">Event Description</label>
                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvDesc" runat="server" ControlToValidate="txtDescription"
                    ErrorMessage="Description is required." CssClass="text-danger" Display="Dynamic" />
            </div>

            <div class="col-md-6 mb-3">
                <label for="ddlCategory" class="form-label fw-bold">Category</label>
                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-select" AutoPostBack="true" 
                    OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged" />
                <asp:RequiredFieldValidator ID="rfvCategory" runat="server" ControlToValidate="ddlCategory"
                    InitialValue="" ErrorMessage="Please select a category." CssClass="text-danger" Display="Dynamic" />
            </div>

            <div class="col-md-6 mb-3">
                <label for="ddlMode" class="form-label fw-bold">Event Mode</label>
                <asp:DropDownList ID="ddlMode" runat="server" CssClass="form-select">
                    <asp:ListItem Text="--Select--" Value="" />
                    <asp:ListItem Text="Online" Value="Online" />
                    <asp:ListItem Text="Offline" Value="Offline" />
                    <asp:ListItem Text="Hybrid" Value="Hybrid" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvMode" runat="server" ControlToValidate="ddlMode"
                    InitialValue="" ErrorMessage="Select event mode." CssClass="text-danger" Display="Dynamic" />
            </div>

            <div class="col-md-6 mb-3">
                <label for="ddlStatus" class="form-label fw-bold">Event Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                    <asp:ListItem Text="--Select--" Value="" />
                    <asp:ListItem Text="Upcoming" Value="Upcoming" />
                    <asp:ListItem Text="Ongoing" Value="Ongoing" />
                    <asp:ListItem Text="Completed" Value="Completed" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvStatus" runat="server" ControlToValidate="ddlStatus"
                    InitialValue="" ErrorMessage="Select event status." CssClass="text-danger" Display="Dynamic" />
            </div>

            <!-- ===== 2. Date & Time ===== -->
            <div class="col-md-6 mb-3">
                <label for="txtNumDays" class="form-label fw-bold">Number of Days</label>
                <asp:TextBox ID="txtNumDays" runat="server" CssClass="form-control" AutoPostBack="true"
                    OnTextChanged="txtNumDays_TextChanged" />
                <asp:RegularExpressionValidator ID="revNumDays" runat="server" 
                    ControlToValidate="txtNumDays" ValidationExpression="^\d+$"
                    ErrorMessage="Enter valid number of days" CssClass="text-danger" Display="Dynamic" />
            </div>

            <div id="divSingleDate" runat="server" class="col-md-6 mb-3">
                <label for="txtDate" class="form-label fw-bold">Date</label>
                <asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-control" />
            </div>
            <div id="divStartDate" runat="server" class="col-md-6 mb-3" visible="false">
                <label for="txtStartDate" class="form-label fw-bold">Start Date</label>
                <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date" CssClass="form-control" />
            </div>
            <div id="divEndDate" runat="server" class="col-md-6 mb-3" visible="false">
                <label for="txtEndDate" class="form-label fw-bold">End Date</label>
                <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date" CssClass="form-control" />
            </div>

            <div class="col-md-6 mb-3">
                <label for="txtStartTime" class="form-label fw-bold">Start Time</label>
                <asp:TextBox ID="txtStartTime" runat="server" CssClass="form-control" TextMode="Time" />
            </div>
            <div class="col-md-6 mb-3">
                <label for="txtEndTime" class="form-label fw-bold">End Time</label>
                <asp:TextBox ID="txtEndTime" runat="server" CssClass="form-control" TextMode="Time" />
            </div>

            <!-- ===== 3. Location ===== -->
            <div class="col-md-6 mb-3">
                <label for="txtLocation" class="form-label fw-bold">Location</label>
                <asp:TextBox ID="txtLocation" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvLocation" runat="server" ControlToValidate="txtLocation"
                    ErrorMessage="Location is required." CssClass="text-danger" Display="Dynamic" />
            </div>

            <!-- ===== 4. Audience & Capacity ===== -->
            <div class="col-md-6 mb-3">
                <label for="txtSkills" class="form-label fw-bold">Skills Required</label>
                <asp:TextBox ID="txtSkills" runat="server" CssClass="form-control" />
            </div>

            <div class="col-md-6 mb-3">
                <label for="txtMaxVolunteers" class="form-label fw-bold">Max Volunteers</label>
                <asp:TextBox ID="txtMaxVolunteers" runat="server" CssClass="form-control" />
                <asp:RegularExpressionValidator ID="revMaxVol" runat="server" ControlToValidate="txtMaxVolunteers"
                    ValidationExpression="^\d+$" ErrorMessage="Enter valid number" CssClass="text-danger" Display="Dynamic" />
            </div>

            <div class="col-md-6 mb-3">
                <label for="txtMaxParticipants" class="form-label fw-bold">Max Participants</label>
                <asp:TextBox ID="txtMaxParticipants" runat="server" CssClass="form-control" />
                <asp:RegularExpressionValidator ID="revMaxPart" runat="server" ControlToValidate="txtMaxParticipants"
                    ValidationExpression="^\d+$" ErrorMessage="Enter valid number" CssClass="text-danger" Display="Dynamic" />
            </div>

            <!-- Audience Checkboxes -->
            <div class="col-md-6 mb-3 form-check">
                <asp:CheckBox ID="chkAllowVolunteers" runat="server" CssClass="form-check-input" />
                <label class="form-check-label" for="chkAllowVolunteers">Allow Volunteers</label>
            </div>
            <div class="col-md-6 mb-3 form-check">
                <asp:CheckBox ID="chkAllowParticipants" runat="server" CssClass="form-check-input"
                    AutoPostBack="true" OnCheckedChanged="chkAllowParticipants_CheckedChanged" />
                <label class="form-check-label" for="chkAllowParticipants">Allow Participants</label>
            </div>
            <asp:CustomValidator ID="cvAudience" runat="server"
                ErrorMessage="Select at least one audience (volunteers or participants)."
                CssClass="text-danger" Display="Dynamic" OnServerValidate="cvAudience_ServerValidate" />

            <!-- Sub Events Panel -->
            <asp:Panel ID="trParticipantCategories" runat="server" CssClass="col-md-12 mb-3" Visible="false">
                <h5 class="text-primary">Add Sub Events (Poster, Quiz, etc.)</h5>
                <div class="row g-2 mb-2 align-items-end">
                    <div class="col-md-5">
                        <asp:TextBox ID="txtSubEventName" runat="server" CssClass="form-control" Placeholder="Sub Event Name" />
                    </div>
                    <div class="col-md-3">
                        <asp:TextBox ID="txtSubTeamSize" runat="server" CssClass="form-control" Placeholder="Team Size" />
                        <asp:RegularExpressionValidator ID="revSubTeamSize" runat="server" 
                            ControlToValidate="txtSubTeamSize" ValidationExpression="^\d+$"
                            ErrorMessage="Enter valid team size" CssClass="text-danger" Display="Dynamic" />
                    </div>
                    <div class="col-md-4">
                        <asp:Button ID="btnAddSubEvent" runat="server" Text="➕ Add Sub Event" 
                            CssClass="btn btn-success w-100" OnClick="btnAddSubEvent_Click" CausesValidation="false" />
                    </div>
                </div>
                <asp:GridView ID="gvSubEvents" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered">
                    <Columns>
                        <asp:BoundField DataField="SubCategoryName" HeaderText="Sub Event Name" />
                        <asp:BoundField DataField="TeamSize" HeaderText="Team Size" />
                    </Columns>
                </asp:GridView>
                <asp:CustomValidator ID="rfvParticipantCategories" runat="server"
                    ErrorMessage="Add at least one sub-event for participants."
                    CssClass="text-danger" Display="Dynamic"
                    OnServerValidate="rfvParticipantCategories_ServerValidate" />
            </asp:Panel>

            <!-- ===== 5. Registration Details ===== -->
            <div class="col-md-6 mb-3">
                <label for="txtDeadline" class="form-label fw-bold">Registration Deadline</label>
                <asp:TextBox ID="txtDeadline" runat="server" CssClass="form-control" TextMode="Date" />
                <asp:RequiredFieldValidator ID="rfvDeadline" runat="server" ControlToValidate="txtDeadline"
                    ErrorMessage="Deadline is required." CssClass="text-danger" Display="Dynamic" />
            </div>

            <div class="col-md-6 mb-3">
                <label for="txtFee" class="form-label fw-bold">Registration Fee (₹)</label>
                <asp:TextBox ID="txtFee" runat="server" CssClass="form-control" Text="0.00" />
                <asp:RequiredFieldValidator ID="rfvFee" runat="server" ControlToValidate="txtFee"
                    ErrorMessage="Fee is required." CssClass="text-danger" Display="Dynamic" />
                <asp:RegularExpressionValidator ID="revFee" runat="server" ControlToValidate="txtFee"
                    ValidationExpression="^\d+(\.\d{1,2})?$"
                    ErrorMessage="Enter a valid amount (e.g., 0.00)" CssClass="text-danger" Display="Dynamic" />
            </div>

            <!-- ===== 6. Contact Information ===== -->
            <div class="col-md-6 mb-3">
                <label for="txtEmail" class="form-label fw-bold">Contact Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                    ErrorMessage="Email required." CssClass="text-danger" Display="Dynamic" />
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail"
                    ValidationExpression="^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$"
                    ErrorMessage="Invalid email." CssClass="text-danger" Display="Dynamic" />
            </div>

            <div class="col-md-6 mb-3">
                <label for="txtPhone" class="form-label fw-bold">Contact Phone</label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvPhone" runat="server" ControlToValidate="txtPhone"
                    ErrorMessage="Phone required." CssClass="text-danger" Display="Dynamic" />
                <asp:RegularExpressionValidator ID="revPhone" runat="server" ControlToValidate="txtPhone"
                    ValidationExpression="^\d{10}$" ErrorMessage="Enter 10-digit number." CssClass="text-danger" Display="Dynamic" />
            </div>

            <!-- ===== 7. Banner Upload ===== -->
            <div class="col-md-6 mb-3">
                <label class="form-label fw-bold">Event Banner</label>
                <asp:FileUpload ID="fuBanner" runat="server" CssClass="form-control" />
            </div>

            <!-- ===== 8. Terms & Submit ===== -->
            <div class="col-md-12 mb-3 form-check">
                <asp:CheckBox ID="chkTerms" runat="server" CssClass="form-check-input" />
                <label class="form-check-label" for="chkTerms">I agree to the terms and conditions</label>
            </div>

            <div class="col-md-12 text-center mb-2">
                <asp:Button ID="btnCreateEvent" runat="server" Text="Create Event" CssClass="btn btn-primary px-4 me-2" 
                    OnClick="btnCreateEvent_Click" />
                <asp:Button ID="btnBack" runat="server" Text="Back to Dashboard" PostBackUrl="~/User/UserDashboard.aspx" 
                    CssClass="btn btn-outline-secondary" CausesValidation="False" />
            </div>

            <div class="col-md-12 text-center">
                <asp:Label ID="lblStatus" runat="server" CssClass="text-danger fw-bold" Visible="false" />
            </div>

        </div>
    </div>
</div>
</asp:Content>
