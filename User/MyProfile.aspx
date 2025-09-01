<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="MyProfile.aspx.cs" Inherits="Authentication.User.MyProfile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<h2 style="color: #e14658; text-align: center; font-size: 30px; margin-bottom: 20px;">My Profile</h2>

<h2>My Profile</h2>

    <div class="form-container">
        <asp:Panel ID="pnlMessage" runat="server" Visible="false">
            <asp:Label ID="lblMessage" runat="server" CssClass="message"></asp:Label>
        </asp:Panel>

        <!-- Display User Info -->
        <div class="form-group">
            <label>Full Name:</label>
            <asp:Label ID="lblFullName" runat="server" CssClass="form-control"></asp:Label>
        </div>

        <div class="form-group">
            <label>Email:</label>
            <asp:Label ID="lblEmail" runat="server" CssClass="form-control"></asp:Label>
        </div>

        <div class="form-group">
            <label>Role:</label>
            <asp:Label ID="lblRole" runat="server" CssClass="form-control text-danger fw-bold"></asp:Label>
        </div>

        <!-- Volunteer Profile Panel -->
        <asp:Panel ID="pnlVolunteer" runat="server" Visible="false">
            <div class="form-group">
                <label>Mobile:</label>
                <asp:TextBox ID="txtMobileV" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvMobileV" runat="server" ControlToValidate="txtMobileV"
    ErrorMessage="Mobile is required." ForeColor="Red" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label>Date of Birth:</label>
                <asp:TextBox ID="txtDOBV" runat="server" TextMode="Date" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvDOBV" runat="server" ControlToValidate="txtDOBV"
    ErrorMessage="Date of Birth is required." ForeColor="Red" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label>Gender:</label>
                <asp:DropDownList ID="ddlGenderV" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Select" Value="" />
                    <asp:ListItem Text="Male" />
                    <asp:ListItem Text="Female" />
                    <asp:ListItem Text="Other" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvGenderV" runat="server" ControlToValidate="ddlGenderV"
    InitialValue="" ErrorMessage="Please select Gender." ForeColor="Red" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label>Skills:</label>
                <asp:TextBox ID="txtSkillsV" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Languages Known:</label>
                <asp:TextBox ID="txtLanguagesV" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Qualification:</label>
                <asp:TextBox ID="txtQualificationV" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Current Address:</label>
                <asp:TextBox ID="txtAddressV" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
           <asp:RequiredFieldValidator ID="rfvAddressV" runat="server" ControlToValidate="txtAddressV"
    ErrorMessage="Address is required." ForeColor="Red" Display="Dynamic" />
                </div>
            <div class="form-group">
                <label>City:</label>
                <asp:TextBox ID="txtCityV" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>State:</label>
                <asp:TextBox ID="txtStateV" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Country:</label>
                <asp:TextBox ID="txtCountryV" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Pin Code:</label>
                <asp:TextBox ID="txtPinV" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Preferred Mode:</label>
                <asp:DropDownList ID="ddlModeV" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Select" Value="" />
                    <asp:ListItem Text="Online" />
                    <asp:ListItem Text="Offline" />
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label>Preferred Event Types:</label>
                <asp:CheckBoxList ID="cblEventsV" runat="server" CssClass="form-control" RepeatDirection="Horizontal" RepeatLayout="Flow" >
                       <asp:ListItem Text="Conference" />
                <asp:ListItem Text="Seminar" />
                <asp:ListItem Text="Workshop" />
                <asp:ListItem Text="Cultural Event" />
                <asp:ListItem Text="Hackathon" />
                <asp:ListItem Text="Exhibition" />
                <asp:ListItem Text="Sports" />
    </asp:CheckBoxList>
           </div>
            <div class="form-group">
                <label>ID Proof (optional):</label>
                <asp:FileUpload ID="fuIDProofV" runat="server" CssClass="form-control" />
            </div>
        </asp:Panel>

        <!-- Organizer Profile Panel -->
        <asp:Panel ID="pnlOrganizer" runat="server" Visible="false">
            <div class="form-group">
                <label>Organization Name:</label>
                <asp:TextBox ID="txtOrgName" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvOrgName" runat="server" ControlToValidate="txtOrgName"
    ErrorMessage="Organization name is required." ForeColor="Red" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label>Organization Type:</label>
                <asp:TextBox ID="txtOrgType" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Designation:</label>
                <asp:TextBox ID="txtDesignation" runat="server" CssClass="form-control" />


<asp:RequiredFieldValidator ID="rfvDesignation" runat="server" ControlToValidate="txtDesignation"
    ErrorMessage="Designation is required." ForeColor="Red" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label>Office Address:</label>
                <asp:TextBox ID="txtOfficeAddress" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
            </div>
            <div class="form-group">
                <label>City:</label>
                <asp:TextBox ID="txtCityO" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>State:</label>
                <asp:TextBox ID="txtStateO" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Country:</label>
                <asp:TextBox ID="txtCountryO" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Pin Code:</label>
                <asp:TextBox ID="txtPinO" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Organization Phone:</label>
                <asp:TextBox ID="txtOrgPhone" runat="server" CssClass="form-control" />
                                <asp:RequiredFieldValidator ID="rfvOrgPhone" runat="server" ControlToValidate="txtOrgPhone"
    ErrorMessage="Phone number is required." ForeColor="Red" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label>Preferred Event Types:</label>
<asp:CheckBoxList ID="cblOrgEvents" runat="server" CssClass="form-control" RepeatDirection="Horizontal" RepeatLayout="Flow" >
                       <asp:ListItem Text="Conference" />
                <asp:ListItem Text="Seminar" />
                <asp:ListItem Text="Workshop" />
                <asp:ListItem Text="Cultural Event" />
                <asp:ListItem Text="Hackathon" />
                <asp:ListItem Text="Exhibition" />
                <asp:ListItem Text="Sports" />
    </asp:CheckBoxList>
            </div>
            <div class="form-group">
                <label>Website:</label>
                <asp:TextBox ID="txtWebsite" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>LinkedIn:</label>
                <asp:TextBox ID="txtLinkedIn" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Facebook:</label>
                <asp:TextBox ID="txtFacebook" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Organization Logo (optional):</label>
                <asp:FileUpload ID="fuOrgLogo" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>ID Proof (optional):</label>
                <asp:FileUpload ID="fuOrgIDProof" runat="server" CssClass="form-control" />
            </div>
        </asp:Panel>

        <!-- Participant Profile Panel -->
        <asp:Panel ID="pnlParticipant" runat="server" Visible="false">
            <div class="form-group">
                <label>Mobile:</label>
                <asp:TextBox ID="txtMobileP" runat="server" CssClass="form-control" />
                
<asp:RequiredFieldValidator ID="rfvMobileP" runat="server" ControlToValidate="txtMobileP"
    ErrorMessage="Mobile is required." ForeColor="Red" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label>Date of Birth:</label>
                <asp:TextBox ID="txtDOBP" runat="server" TextMode="Date" CssClass="form-control" />
           
<asp:RequiredFieldValidator ID="rfvDOBP" runat="server" ControlToValidate="txtDOBP"
    ErrorMessage="DOB is required." ForeColor="Red" Display="Dynamic" />
                </div>
            <div class="form-group">
                <label>Gender:</label>
                <asp:DropDownList ID="ddlGenderP" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Select" Value="" />
                    <asp:ListItem Text="Male" />
                    <asp:ListItem Text="Female" />
                    <asp:ListItem Text="Other" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvGenderP" runat="server" ControlToValidate="ddlGenderP"
    InitialValue="" ErrorMessage="Select Gender." ForeColor="Red" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label>Languages:</label>
                <asp:TextBox ID="txtLanguagesP" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Qualification:</label>
                <asp:TextBox ID="txtQualificationP" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Current Address:</label>
                <asp:TextBox ID="txtAddressP" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2" />
            </div>
            <div class="form-group">
                <label>City:</label>
                <asp:TextBox ID="txtCityP" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>State:</label>
                <asp:TextBox ID="txtStateP" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Country:</label>
                <asp:TextBox ID="txtCountryP" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Pin Code:</label>
                <asp:TextBox ID="txtPinP" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Preferred Mode:</label>
                <asp:DropDownList ID="ddlModeP" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Select" Value="" />
                    <asp:ListItem Text="Online" />
                    <asp:ListItem Text="Offline" />
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <label>Institute Name:</label>
                <asp:TextBox ID="txtInstitute" runat="server" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvInstitute" runat="server" ControlToValidate="txtInstitute"
    ErrorMessage="Institute name is required." ForeColor="Red" Display="Dynamic" />
            </div>
            <div class="form-group">
                <label>Course / Department:</label>
                <asp:TextBox ID="txtCourseDept" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Year of Study:</label>
                <asp:TextBox ID="txtYearStudy" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Preferred Event Types:</label>
                <asp:CheckBoxList ID="cblEventsP" runat="server" CssClass="form-control" RepeatDirection="Horizontal" RepeatLayout="Flow" >
                       <asp:ListItem Text="Conference" />
                <asp:ListItem Text="Seminar" />
                <asp:ListItem Text="Workshop" />
                <asp:ListItem Text="Cultural Event" />
                <asp:ListItem Text="Hackathon" />
                <asp:ListItem Text="Exhibition" />
                <asp:ListItem Text="Sports" />
    </asp:CheckBoxList>
            </div>
            <div class="form-group">
                <label>ID Proof (optional):</label>
                <asp:FileUpload ID="fuIDProofP" runat="server" CssClass="form-control" />
            </div>
        </asp:Panel>

        <!-- Save Button -->
        <asp:Button ID="btnSave" runat="server" Text="Save Profile" CssClass="submit-button" OnClick="btnSave_Click" />
    <asp:Button ID="btnBack" runat="server" Text="Back to Dashboard" CssClass="submit-button" PostBackUrl="~/User/UserDashboard.aspx" />
    
    
    </div>
  

</asp:Content>












