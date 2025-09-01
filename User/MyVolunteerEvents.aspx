<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="MyVolunteerEvents.aspx.cs" Inherits="Authentication.User.MyVolunteerEvents" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Page Heading -->
    <div class="text-center mb-5">
        <h2 class="fw-bold text-primary">
            <i class="bi bi-award-fill me-2"></i> My Volunteer Events
        </h2>
        <p class="text-muted">Track your volunteering journey and access certificates.</p>
    </div>

    <!-- No Events Message -->
    <asp:Label ID="lblNoEvents" runat="server" 
        Text="You haven’t registered for any events yet." 
        Visible="false" CssClass="alert alert-warning fw-semibold shadow-sm border-0 text-center"></asp:Label>

    <!-- Events Grid -->
    <div class="row g-4">
        <asp:Repeater ID="rptMyEvents" runat="server" OnItemCommand="rptMyEvents_ItemCommand">
            <ItemTemplate>
                <div class="col-md-6">
                    <div class="card shadow-sm border-0 rounded-3 h-100">
                        <div class="card-body">
                            <h4 class="fw-bold text-dark mb-3">
                                <i class="bi bi-calendar-event me-2 text-primary"></i> <%# Eval("Title") %>
                            </h4>

                            <p class="mb-1"><strong class="text-secondary">Volunteer ID:</strong> <span class="text-dark"><%# Eval("VolunteerID") %></span></p>
                            <p class="mb-1"><strong class="text-secondary">Date:</strong> <span class="text-dark"><%# Eval("Date", "{0:dd MMM yyyy}") %></span></p>
                            <p class="mb-1"><strong class="text-secondary">Location:</strong> <span class="text-dark"><%# Eval("Location") %></span></p>
                            <p class="mb-1"><strong class="text-secondary">Status:</strong> <span class="badge bg-info text-dark"><%# Eval("Status") %></span></p>
                            <p class="mb-3"><strong class="text-secondary">Role:</strong> <span class="badge bg-secondary"><%# Eval("CategoryName") %></span></p>

                            <div>
                                <asp:Button ID="btnCert" runat="server" Text="&#127891; Download Certificate"
                                    CssClass="btn btn-success btn-sm me-2"
                                    CommandName="DownloadCertificate"
                                    CommandArgument='<%# Eval("VolunteerID") + "|" + Eval("EventID") %>'
                                    Visible='<%# Convert.ToBoolean(Eval("IsCompleted")) && Convert.ToDateTime(Eval("Date")).Date <= DateTime.Today %>' />

                                <asp:Button ID="btnFeedback" runat="server" Text="💬 Give Feedback"
                                    CssClass="btn btn-primary btn-sm"
                                    CommandName="GiveFeedback"
                                    CommandArgument='<%# Eval("EventID") %>' />
                            </div>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

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
                        &#127891; Your certificate will be available to download on the event day after attendance verification.
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

</asp:Content>
