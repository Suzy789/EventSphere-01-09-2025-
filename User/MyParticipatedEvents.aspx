<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="MyParticipatedEvents.aspx.cs" Inherits="Authentication.User.MyParticipatedEvents" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<!-- Certificate Info Modal -->
<div class="modal fade" id="certModal" tabindex="-1" aria-labelledby="certModalLabel" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg rounded-4">
            <div class="modal-header bg-primary text-white border-0">
                <h5 class="modal-title" id="certModalLabel">
                    <i class="bi bi-award-fill me-2"></i> Certificate Claim Information
                </h5>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body text-center p-4">
                <p class="fw-semibold fs-5 text-dark">
                    🎓 Your certificate will be available to download on the event day after attendance verification.
                </p>
            </div>
            <div class="modal-footer border-0 justify-content-center">
                <button type="button" class="btn btn-outline-primary rounded-pill px-4" data-bs-dismiss="modal">
                    Got it!
                </button>
            </div>
        </div>
    </div>
</div>

<!-- Show modal on page load -->
<script>
    document.addEventListener("DOMContentLoaded", function () {
        var certModal = new bootstrap.Modal(document.getElementById('certModal'));
        certModal.show();
    });
</script>

<!-- Page Heading -->
    <div class="text-center mb-5">
        <h2 class="fw-bold text-primary">
            <i class="bi bi-award-fill me-2"></i> My Participated Events
        </h2>
        <p class="text-muted">Track your participation and access certificates.</p>
    </div>

    <!-- No Events Message -->
    <asp:Label ID="lblNoEvents" runat="server" 
        Text="You haven’t participated in any events yet." 
        Visible="false" CssClass="alert alert-warning fw-semibold shadow-sm border-0 text-center"></asp:Label>

    <!-- Events Grid -->
    <div class="row g-4">
    <asp:Repeater ID="rptParticipatedEvents" runat="server"
    OnItemDataBound="rptParticipatedEvents_ItemDataBound"
    OnItemCommand="rptParticipatedEvents_ItemCommand">



            <ItemTemplate>
                <div class="col-md-6">
                    <div class="card shadow-sm border-0 rounded-3 h-100">
                        <div class="card-body">
                            <h4 class="fw-bold text-dark mb-3">
                                <i class="bi bi-calendar-event me-2 text-primary"></i> <%# Eval("Title") %>
                            </h4>

                            <p class="mb-1"><strong class="text-secondary">Event ID:</strong> <span class="text-dark"><%# Eval("EventID") %></span></p>
                            <p class="mb-1"><strong class="text-secondary">Date:</strong> <span class="text-dark"><%# Eval("Date", "{0:dd MMM yyyy}") %></span></p>
                            <p class="mb-1"><strong class="text-secondary">Location:</strong> <span class="text-dark"><%# Eval("Location") %></span></p>

                            <div>
                               <asp:Button ID="btnCert" runat="server" Text="🎓 Download Certificate"
    CssClass="btn btn-success btn-sm me-2"
    CommandName="DownloadCertificate"
    CommandArgument='<%# Eval("EventID") %>' />
                            </div>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    
     

</asp:Content>
