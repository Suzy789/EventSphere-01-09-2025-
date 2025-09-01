<%@ Page Title="Volunteer Details" Language="C#" MasterPageFile="~/MasterPage.Master"
    AutoEventWireup="true" CodeBehind="VolunteerDetails.aspx.cs" Inherits="Authentication.User.VolunteerDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container mt-5">
        <h2 class="mb-4 text-center">Volunteer Application Details</h2>

        <!-- Volunteer Details Card -->
        <div class="card shadow-lg border-0 rounded-4">
            <div class="row g-0">
                <!-- Photo -->
                <div class="col-md-4 text-center p-4">
                    <asp:Image ID="imgVolunteer" runat="server" CssClass="img-fluid rounded-circle shadow"
                        AlternateText="Volunteer Photo" Style="width: 200px; height: 200px; object-fit: cover;" />
                </div>

                <!-- Details -->
                <div class="col-md-8">
                    <div class="card-body">
                        <dl class="row mb-0">
                            <dt class="col-sm-4">Volunteer Name:</dt>
                            <dd class="col-sm-8"><asp:Label ID="lblVolunteerName" runat="server" /></dd>

                            <dt class="col-sm-4">Event Name:</dt>
                            <dd class="col-sm-8"><asp:Label ID="lblEventName" runat="server" /></dd>

                            <dt class="col-sm-4">Category:</dt>
                            <dd class="col-sm-8"><asp:Label ID="lblCategory" runat="server" /></dd>

                            <dt class="col-sm-4">Application Status:</dt>
                            <dd class="col-sm-8">
                                <asp:Label ID="lblStatus" runat="server" CssClass="fw-bold" />
                            </dd>

                            <dt class="col-sm-4">Start Date:</dt>
                            <dd class="col-sm-8"><asp:Label ID="lblStartDate" runat="server" /></dd>

                            <dt class="col-sm-4">End Date:</dt>
                            <dd class="col-sm-8"><asp:Label ID="lblEndDate" runat="server" /></dd>

                            <dt class="col-sm-4">Location:</dt>
                            <dd class="col-sm-8"><asp:Label ID="lblLocation" runat="server" /></dd>
                        </dl>
                    </div>
                </div>
            </div>
        </div>

        <!-- Back Button -->
        <div class="text-center mt-4">
            <a href="ManageVolunteeringEvents.aspx" class="btn btn-secondary px-4">
                <i class="bi bi-arrow-left"></i> Back to Events
            </a>
        </div>
    </div>

</asp:Content>
