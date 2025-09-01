<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="VolunteerParticipantAttendance.aspx.cs" Inherits="Authentication.User.VolunteerParticipantAttendance" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container my-5 p-4 bg-white shadow rounded">
    <h2 class="text-center text-primary fw-bold mb-4">📝 Participant Attendance</h2>

    <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-info text-center" Visible="false"></asp:Label>

    <!-- Event Selection -->
    <div class="mb-4">
        <label class="form-label fw-semibold">Select Event:</label>
        <asp:DropDownList ID="ddlEvents" runat="server" AutoPostBack="true"
            CssClass="form-select" OnSelectedIndexChanged="ddlEvents_SelectedIndexChanged">
        </asp:DropDownList>
    </div>

    <!-- QR Code / Manual Input -->
    <div class="mb-3">
        <label class="form-label fw-semibold">Scan or Enter Participant QR Code:</label>
        <div class="input-group">
            <asp:TextBox ID="txtQRCode" runat="server" CssClass="form-control" Placeholder="Enter QR Code"></asp:TextBox>
            <button type="button" class="btn btn-success" onclick="submitQRCode()">Mark Attendance</button>
        </div>
    </div>

    <!-- Participant List -->
    <asp:Panel ID="pnlParticipants" runat="server" Visible="false">
        <asp:GridView ID="gvParticipants" runat="server" CssClass="table table-bordered table-hover mt-4"
            AutoGenerateColumns="False" DataKeyNames="RegistrationID">
            <Columns>
                <asp:BoundField DataField="FullName" HeaderText="Participant Name" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="AttendanceCode" HeaderText="Attendance QR" />
                <asp:TemplateField HeaderText="Present?">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkPresent" runat="server"
    Checked='<%# Convert.ToBoolean(Eval("IsPresent")) %>'
    AutoPostBack="true"
    OnCheckedChanged="chkPresent_CheckedChanged" />

                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </asp:Panel>
</div>

<script>
    function submitQRCode() {
        __doPostBack('MarkQRCode', '');
    }
</script>

</asp:Content>
