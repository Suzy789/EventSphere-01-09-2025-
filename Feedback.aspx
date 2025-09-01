<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Feedback.aspx.cs" Inherits="Authentication.User.Feedback" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 <section class="container py-5">
        <div class="feedback-form mx-auto p-4 border shadow rounded bg-white text-dark" style="max-width: 800px;">

            <h2 class="text-center fw-bold mb-4">We Value Your Feedback</h2>
            <asp:Label ID="lblFeedbackMsg" runat="server" CssClass="fw-bold text-success d-block text-center mb-3" Visible="false" />

            <!-- Event Selection -->
            <div class="mb-3">
                <label for="ddlEvents" class="form-label fw-bold">Select Event You Attended:</label>
                <asp:DropDownList ID="ddlEvents" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlEvents_SelectedIndexChanged"/>
                <asp:RequiredFieldValidator ID="rfvEvent" runat="server" ControlToValidate="ddlEvents" InitialValue="" ErrorMessage="Please select an event" CssClass="text-danger" Display="Dynamic" />
            </div>

            <!-- Overall Rating -->
            <div class="mb-3">
                <label for="ddlOverallRating" class="form-label fw-bold">Overall, how satisfied were you with the event?</label>
                <asp:DropDownList ID="ddlOverallRating" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Select Rating" Value="" />
                    <asp:ListItem Text="1 - Very Dissatisfied" Value="1" />
                    <asp:ListItem Text="2 - Dissatisfied" Value="2" />
                    <asp:ListItem Text="3 - Neutral" Value="3" />
                    <asp:ListItem Text="4 - Satisfied" Value="4" />
                    <asp:ListItem Text="5 - Very Satisfied" Value="5" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvOverallRating" runat="server" ControlToValidate="ddlOverallRating" InitialValue="" ErrorMessage="Please rate the event" CssClass="text-danger" Display="Dynamic" />
            </div>

            <!-- Text Questions -->
            <div class="mb-3">
                <label for="txtEnjoyed" class="form-label fw-bold">What did you enjoy most about this event?</label>
                <asp:TextBox ID="txtEnjoyed" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" />
            </div>
            <div class="mb-3">
                <label for="txtDisliked" class="form-label fw-bold">What, if anything, did you dislike about the event?</label>
                <asp:TextBox ID="txtDisliked" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" />
            </div>

            <!-- Participant Panel -->
            <asp:Panel ID="pnlParticipantFeedback" runat="server" Visible="false">
                <h5 class="fw-bold mt-4">Sessions and Content</h5>
                <div class="mb-3">
                    <label class="form-label">How would you rate the quality of the sessions?</label>
                    <asp:DropDownList ID="ddlSessionQuality" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Select Rating" Value="" />
                        <asp:ListItem Text="1 - Very Dissatisfied" Value="1" />
                        <asp:ListItem Text="2 - Dissatisfied" Value="2" />
                        <asp:ListItem Text="3 - Neutral" Value="3" />
                        <asp:ListItem Text="4 - Satisfied" Value="4" />
                        <asp:ListItem Text="5 - Very Satisfied" Value="5" />
                    </asp:DropDownList>
                </div>
                <div class="mb-3">
                    <label class="form-label">What topics did you find most valuable?</label>
                    <asp:TextBox ID="txtValuableTopics" runat="server" CssClass="form-control" />
                </div>
                <div class="mb-3">
                    <label class="form-label">Were the speakers engaging and knowledgeable?</label>
                    <asp:DropDownList ID="ddlSpeakersRating" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Select Rating" Value="" />
                        <asp:ListItem Text="1 - Very Dissatisfied" Value="1" />
                        <asp:ListItem Text="2 - Dissatisfied" Value="2" />
                        <asp:ListItem Text="3 - Neutral" Value="3" />
                        <asp:ListItem Text="4 - Satisfied" Value="4" />
                        <asp:ListItem Text="5 - Very Satisfied" Value="5" />
                    </asp:DropDownList>
                </div>

                <!-- Logistics -->
                <h5 class="fw-bold mt-4">Logistics and Venue</h5>
                <div class="mb-3">
                    <label class="form-label">How satisfied were you with the venue?</label>
                    <asp:DropDownList ID="ddlVenue" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Select Rating" Value="" />
                        <asp:ListItem Text="1 - Very Dissatisfied" Value="1" />
                        <asp:ListItem Text="2 - Dissatisfied" Value="2" />
                        <asp:ListItem Text="3 - Neutral" Value="3" />
                        <asp:ListItem Text="4 - Satisfied" Value="4" />
                        <asp:ListItem Text="5 - Very Satisfied" Value="5" />
                    </asp:DropDownList>
                </div>
                <div class="mb-3">
                    <label class="form-label">Was the registration process smooth?</label>
                    <asp:DropDownList ID="ddlRegistration" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Select Rating" Value="" />
                        <asp:ListItem Text="1 - Very Dissatisfied" Value="1" />
                        <asp:ListItem Text="2 - Dissatisfied" Value="2" />
                        <asp:ListItem Text="3 - Neutral" Value="3" />
                        <asp:ListItem Text="4 - Satisfied" Value="4" />
                        <asp:ListItem Text="5 - Very Satisfied" Value="5" />
                    </asp:DropDownList>
                </div>

                <!-- Expectations -->
                <div class="mb-3">
                    <label class="form-label">Did the event meet your expectations?</label>
                    <asp:TextBox ID="txtExpectations" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
                </div>
            </asp:Panel>

            <!-- Volunteer Panel -->
            <asp:Panel ID="pnlVolunteerFeedback" runat="server" Visible="false">
                <h5 class="fw-bold mt-4">Volunteer Experience (if applicable)</h5>
                <div class="mb-3">
                    <label class="form-label">Rate the overall volunteer experience</label>
                    <asp:DropDownList ID="ddlVolunteerExperience" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Select Rating" Value="" />
                        <asp:ListItem Text="1 - Very Dissatisfied" Value="1" />
                        <asp:ListItem Text="2 - Dissatisfied" Value="2" />
                        <asp:ListItem Text="3 - Neutral" Value="3" />
                        <asp:ListItem Text="4 - Satisfied" Value="4" />
                        <asp:ListItem Text="5 - Very Satisfied" Value="5" />
                    </asp:DropDownList>
                </div>
                <div class="mb-3">
                    <label class="form-label">How helpful was the coordinator?</label>
                    <asp:DropDownList ID="ddlCoordinator" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Select Rating" Value="" />
                        <asp:ListItem Text="1 - Very Dissatisfied" Value="1" />
                        <asp:ListItem Text="2 - Dissatisfied" Value="2" />
                        <asp:ListItem Text="3 - Neutral" Value="3" />
                        <asp:ListItem Text="4 - Satisfied" Value="4" />
                        <asp:ListItem Text="5 - Very Satisfied" Value="5" />
                    </asp:DropDownList>
                </div>
            </asp:Panel>

            <!-- Demographics -->
            <h5 class="fw-bold mt-4">Additional Information (Optional)</h5>
            <div class="mb-3">
                <label class="form-label">Age Range</label>
                <asp:DropDownList ID="ddlAge" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Select Age Range" Value="" />
                    <asp:ListItem>Under 18</asp:ListItem>
                    <asp:ListItem>18–25</asp:ListItem>
                    <asp:ListItem>26–40</asp:ListItem>
                    <asp:ListItem>41–60</asp:ListItem>
                    <asp:ListItem>Above 60</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="mb-3">
                <label class="form-label">Gender</label>
                <asp:DropDownList ID="ddlGender" runat="server" CssClass="form-select">
                    <asp:ListItem Text="Select Gender" Value="" />
                    <asp:ListItem>Male</asp:ListItem>
                    <asp:ListItem>Female</asp:ListItem>
                    <asp:ListItem>Prefer not to say</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="mb-3">
                <label class="form-label">Your Email (optional)</label>
                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control" />
            </div>

            <!-- Communication -->
            <div class="mb-3">
                <label class="form-label">Did you receive adequate communication before, during, and after the event?</label>
                <asp:TextBox ID="txtCommunication" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control" />
            </div>

            <!-- Suggestions -->
            <div class="mb-3">
                <label class="form-label">What could be done differently next time to improve the experience?</label>
                <asp:TextBox ID="txtSuggestions" runat="server" TextMode="MultiLine" Rows="2" CssClass="form-control" />
            </div>

            <!-- Submit Buttons -->
            <div class="text-center mt-4">
                <div class="mt-4 d-flex justify-content-between">
                    <asp:Button ID="Button1" runat="server" CssClass="btn btn-secondary btn-lg" Text="← Back to Dashboard" PostBackUrl="~/User/UserDashboard.aspx" CausesValidation="false" />
                    <asp:Button ID="btnSubmitFeedback" runat="server" Text="Submit Feedback" CssClass="btn btn-warning px-5" OnClick="btnSubmitFeedback_Click" />
                </div>
            </div>
        </div>
    </section>

</asp:Content>
