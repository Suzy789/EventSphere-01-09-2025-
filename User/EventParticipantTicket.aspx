<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="EventParticipantTicket.aspx.cs" Inherits="Authentication.User.EventParticipantTicket" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .qr-code { 
    width: 150px; 
    height: 150px; 
}
        .ticket {
    background-color: #f9f9f9;
    border-radius: 8px;
    box-shadow: 0 0 8px rgba(0,0,0,0.1);
    padding: 20px;
}
    </style>
    <div class="container my-5 p-4 bg-white shadow rounded">
    <h2 class="text-center text-primary fw-bold mb-4">🎫 My Event Tickets</h2>
    
    <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-info text-center" Visible="false"></asp:Label>

    <asp:Repeater ID="rptTickets" runat="server">
        <ItemTemplate>
            <div class="ticket card mb-4 p-3 border border-primary">
                <h4><%# Eval("Title") %></h4>
                <p><strong>Date:</strong> <%# Eval("EventDate", "{0:yyyy-MM-dd}") %></p>
                <p><strong>Location:</strong> <%# Eval("Location") %></p>
                <p><strong>Attendance QR Code:</strong></p>
                <div class="text-center mb-3">
    <asp:Image ID="Image1" runat="server" ImageUrl='<%# Eval("QRCodeUrl") %>' CssClass="qr-code" />
</div>
                <br />
                <asp:Button ID="btnDownloadTicket" runat="server" Text="📥 Download Ticket" CssClass="btn btn-primary" CommandArgument='<%# Eval("RegistrationID") %>' OnClick="btnDownloadTicket_Click" />
            </div>
        </ItemTemplate>
    </asp:Repeater>

</div>

<style>
    .ticket { background-color: #f9f9f9; }
    .qr-code { width: 150px; height: 150px; }
</style>


</asp:Content>
