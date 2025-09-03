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
            <!-- Event title with subcategory -->
            <h4 class="text-primary fw-bold">
                <%# !string.IsNullOrEmpty(Eval("SubCategoryName")?.ToString()) 
                        ? Eval("Title") + " — " + Eval("SubCategoryName") 
                        : Eval("Title") + " — General" %>
            </h4>

            <p><strong>Date:</strong> <%# Eval("EventDate", "{0:dd MMM yyyy}") %> 
               at <%# Eval("EventDate", "{0:hh:mm tt}") %></p>
            <p><strong>Location:</strong> <%# Eval("Location") %></p>
            <p><strong>Participant:</strong> <%# Eval("FullName") %></p>
            <p><strong>Code:</strong> <%# Eval("AttendanceCode") %></p>

            <div class="text-center my-3">
                <asp:Image ID="Image1" runat="server" 
                           ImageUrl='<%# Eval("QRCodeUrl") %>' 
                           CssClass="qr-code border rounded shadow" />
            </div>

            <div class="text-center">
                <asp:Button ID="btnDownloadTicket" runat="server" 
                            Text="📥 Download Ticket PDF" 
                            CssClass="btn btn-primary fw-bold px-4" 
                            CommandArgument='<%# Eval("RegistrationID") %>' 
                            OnClick="btnDownloadTicket_Click" />
            </div>

            <!-- Footer -->
            <p class="text-muted small text-center mt-3">
                Please bring this ticket and a valid ID for entry.
            </p>
        </div>
    </ItemTemplate>
</asp:Repeater>


</div>

<style>
    .ticket { background-color: #f9f9f9; }
    .qr-code { width: 150px; height: 150px; }
</style>



</asp:Content>
