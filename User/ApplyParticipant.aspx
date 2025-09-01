<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ApplyParticipant.aspx.cs" Inherits="Authentication.User.ApplyParticipant" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:HiddenField ID="hfEventID" runat="server" />
    <asp:HiddenField ID="hfLeaderUserID" runat="server" />

    <div class="container mt-4 mb-5">
        <h2 class="mb-4">Apply as Participant</h2>

        <!-- Event Name -->
        <div class="mb-3">
            <label>Event Name</label>
            <asp:TextBox ID="txtEventName" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
        </div>

       <!-- User Info -->
        <div class="row">
            <div class="col-md-6 mb-3">
                <label>Full Name</label>
                <asp:TextBox ID="txtFullName" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
            </div>
            <div class="col-md-6 mb-3">
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
            </div>
        </div>

        <div class="row">
            <div class="col-md-6 mb-3">
                <label>Phone</label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-md-6 mb-3">
                <label>College / Organization</label>
                <asp:TextBox ID="txtOrganization" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
        </div>

        <!-- Sub Events -->
        <div class="mb-3">
            <label>Select Sub Events / Categories</label>
            <asp:CheckBoxList ID="cblSubCategories" runat="server" 
                CssClass="list-unstyled" 
                AutoPostBack="true" 
                OnSelectedIndexChanged="cblSubCategories_SelectedIndexChanged">
            </asp:CheckBoxList>
        </div>

        <!-- Dynamic Team Panels -->
        <asp:PlaceHolder ID="phTeams" runat="server"></asp:PlaceHolder>

        <!-- Team Name -->
        <div class="mb-3">
            <label>Team Name (if applicable)</label>
            <asp:TextBox ID="txtTeamName" runat="server" CssClass="form-control"></asp:TextBox>
        </div>

        <!-- File Upload -->
        <div class="mb-3">
            <label>Supporting Document (optional)</label>
            <asp:FileUpload ID="fuSupportingDoc" runat="server" CssClass="form-control" />
        </div>

        <!-- Payment -->
        <div class="mb-3">
            <label>Payment Mode</label>
            <asp:DropDownList ID="ddlPaymentMode" runat="server" CssClass="form-select">
                <asp:ListItem Value="Online">Online</asp:ListItem>
                <asp:ListItem Value="Offline">Offline</asp:ListItem>
            </asp:DropDownList>
        </div>

        <!-- Terms -->
        <div class="form-check mb-3">
            <asp:CheckBox ID="chkAgree" runat="server" CssClass="form-check-input" />
            <label class="form-check-label" for="chkAgree">I agree to the Terms & Conditions</label>
        </div>

        <!-- Submit -->
        <asp:Button ID="btnSubmit" runat="server" Text="Register" CssClass="btn btn-primary"
            OnClick="btnSubmit_Click" />

        <!-- Message -->
        <div class="mt-3">
            <asp:Label ID="lblMessage" runat="server" Visible="false"></asp:Label>
        </div>
    </div>

</asp:Content>
