<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ViewFeedback.aspx.cs" Inherits="Authentication.User.ViewFeedback" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container mt-4">
        <h2 class="text-center text-primary">View Feedback</h2>

        <!-- Event Dropdown -->
        <asp:DropDownList ID="ddlEvents" runat="server" AutoPostBack="true"
            CssClass="form-select my-3" OnSelectedIndexChanged="ddlEvents_SelectedIndexChanged" />

        <!-- Feedback Summary -->
        <div id="feedbackSummary" runat="server" visible="false" class="mb-4 text-center">
            <h4 class="text-dark">Average Rating</h4>
            <asp:Literal ID="litStars" runat="server"></asp:Literal>
            <h5 class="mt-2"><asp:Literal ID="litAverageRating" runat="server"></asp:Literal> / 5</h5>
            <span class="badge bg-primary">
                <asp:Literal ID="litTotalFeedback" runat="server"></asp:Literal> feedback(s)
            </span>
        </div>

        <!-- Feedback List -->
        <asp:Repeater ID="rptFeedback" runat="server">
            <ItemTemplate>
                <div class="card shadow-sm mb-3 p-3">
                    <h5><%# Eval("UserFullName") %> 
                        <span class="badge bg-secondary"><%# Eval("UserRole") %></span>
                    </h5>
                    <div>
                        <strong>Overall Rating:</strong>
                        <%# GetStars(Eval("OverallRating")) %>
                        (<%# Eval("OverallRating") %>/5)
                    </div>
                    <p><strong>Enjoyed:</strong> <%# Eval("Enjoyed") %></p>
                    <p><strong>Disliked:</strong> <%# Eval("Disliked") %></p>
                    <p><strong>Suggestions:</strong> <%# Eval("Suggestions") %></p>
                    <small class="text-muted">Submitted on <%# Eval("SubmittedAt", "{0:dd-MMM-yyyy hh:mm tt}") %></small>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <!-- No Feedback Message -->
        <asp:Label ID="lblMsg" runat="server" CssClass="text-danger fw-bold mt-3 d-block" Visible="false" />
    </div>
</asp:Content>
