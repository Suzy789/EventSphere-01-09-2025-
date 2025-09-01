<%@ Page Title="Event Details" Language="C#" MasterPageFile="~/MasterPage.Master"
    CodeBehind="ApplyVolunteer.aspx.cs"
    Inherits="Authentication.User.ApplyVolunteer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
       <h2 style="color: #e14658; text-align: center; font-size: 30px; margin-bottom: 20px;">Volunteer Application</h2>
<div class="form-container">

        <asp:Label ID="lblMessage" runat="server" CssClass="message success-message" Visible="false" />

        <div class="form-group">
            <label>Event:</label>
            <asp:TextBox ID="txtEventName" runat="server" CssClass="inputbox" ReadOnly="true" />
        </div>

        <div class="form-group">
            <label>User Email:</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="inputbox" ReadOnly="true" />
        </div>

        <div class="form-group">
            <label>Full Name:</label>
            <asp:TextBox ID="txtFullName" runat="server" CssClass="inputbox" ReadOnly="true" />
        </div>

        <div class="form-group">
            <label>Phone Number:</label>
            <asp:TextBox ID="txtPhone" runat="server" CssClass="inputbox" />
        </div>

       <div class="form-group">
    <label>Preferred Volunteer Role:</label>
    <asp:DropDownList ID="ddlRole" runat="server" CssClass="inputbox">
    </asp:DropDownList>
</div>

               <div class="form-group" id="divOtherRole" runat="server" visible="false">
            <label>Please specify your role:</label>
            <asp:TextBox ID="txtOtherRole" runat="server" CssClass="inputbox" />
        </div>

        <div class="form-group">
            <label>Have you volunteered before?</label>
            <asp:RadioButtonList ID="rblVolunteeredBefore" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem Text="Yes" Value="Yes" />
                <asp:ListItem Text="No" Value="No" Selected="True" />
            </asp:RadioButtonList>
        </div>

        <div class="form-group">
            <label>If Yes, describe your experience (optional):</label>
            <asp:TextBox ID="txtExperience" runat="server" CssClass="inputbox" TextMode="MultiLine" Rows="3" />
        </div>

        <div class="form-group">
            <label>Emergency Contact Number:</label>
            <asp:TextBox ID="txtEmergencyContact" runat="server" CssClass="inputbox" />
        </div>

        <div class="form-group">
            <label>Upload ID Proof:</label>
            <asp:FileUpload ID="fuIDProof" runat="server" CssClass="inputbox" />
        </div>

        <div class="form-group">
            <label>Upload Recent Photo:</label>
            <asp:FileUpload ID="fuPhoto" runat="server" CssClass="inputbox" />
        </div>

        <div class="form-group">
            <asp:CheckBox ID="chkAgree" runat="server" />
            <label for="chkAgree" style="display:inline;"> I agree to the Event's Volunteer Guidelines & Code of Conduct.</label>
        </div>

        <div class="form-group">
            <asp:CheckBox ID="chkCertificateConsent" runat="server" />
            <label for="chkCertificateConsent" style="display:inline;"> I understand that certificates will be issued only based on attendance.</label>
        </div>

        <asp:Button ID="btnSubmit" runat="server" Text="Submit Application" CssClass="submit-button" OnClick="btnSubmit_Click" />
        <asp:Button ID="btnBack" runat="server" Text="Back to Browse Events" CssClass="submit-button" PostBackUrl="BrowseEvents.aspx" />

    </div>
</asp:Content>
