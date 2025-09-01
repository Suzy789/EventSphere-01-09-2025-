<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="VolunteerStatus.aspx.cs" Inherits="Authentication.User.VolunteerStatus" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="container py-5">
      <h2 class="mb-4 text-center">My Volunteering Status</h2>

      <!-- Summary Cards -->
      <div class="row text-white mb-4">
          <div class="col-md-3 mb-3">
              <div class="card bg-success rounded-4 shadow text-center p-4">
                  <h5>Approved</h5>
                  <asp:Label ID="lblApproved" runat="server" CssClass="fs-3 fw-bold"></asp:Label>
              </div>
          </div>
          <div class="col-md-3 mb-3">
              <div class="card bg-warning rounded-4 shadow text-center p-4">
                  <h5>Pending</h5>
                  <asp:Label ID="lblPending" runat="server" CssClass="fs-3 fw-bold"></asp:Label>
              </div>
          </div>
          <div class="col-md-3 mb-3">
              <div class="card bg-danger rounded-4 shadow text-center p-4">
                  <h5>Rejected</h5>
                  <asp:Label ID="lblRejected" runat="server" CssClass="fs-3 fw-bold"></asp:Label>
              </div>
          </div>
          <div class="col-md-3 mb-3">
              <div class="card bg-secondary rounded-4 shadow text-center p-4">
                  <h5>Cancelled</h5>
                  <asp:Label ID="lblCancelled" runat="server" CssClass="fs-3 fw-bold"></asp:Label>
              </div>
          </div>
      </div>

      <!-- Progress Bar -->
      <div class="mb-4">
          <label class="fw-semibold">Application Progress</label>
          <div class="progress" style="height: 30px;">
              <asp:Literal ID="ltProgressBar" runat="server"></asp:Literal>
          </div>
      </div>

      <!-- Applications Table -->
      <asp:GridView ID="gvApplications" runat="server" CssClass="table table-bordered table-hover mt-4" AutoGenerateColumns="False" EmptyDataText="No volunteering applications found.">
          <Columns>
              <asp:BoundField HeaderText="Event" DataField="Title" />
              <asp:BoundField HeaderText="Category" DataField="CategoryName" />
              <asp:BoundField HeaderText="Role" DataField="RoleTitle" />
              <asp:BoundField HeaderText="Status" DataField="Status" />
           </Columns>
      </asp:GridView>
  </div>
</asp:Content>
