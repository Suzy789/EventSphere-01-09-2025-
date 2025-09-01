<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="AssignedDuties.aspx.cs" Inherits="Authentication.User.AssignedDuties" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <h2 class="page-title">📋 My Assigned Duties</h2>

    <asp:Label ID="lblMessage" runat="server" Visible="false" />

  <asp:GridView ID="gvDuties" runat="server" AutoGenerateColumns="False"
    CssClass="table table-bordered" OnRowCommand="gvDuties_RowCommand">
    <Columns>
        <asp:BoundField DataField="EventTitle" HeaderText="Event" />
        <asp:BoundField DataField="RoleTitle" HeaderText="Role" />
        <asp:BoundField DataField="Description" HeaderText="Description" />
        <asp:TemplateField HeaderText="Download ID">
            <ItemTemplate>
                <asp:LinkButton ID="lnkDownload" runat="server"
                    CommandName="DownloadID"
                    CommandArgument='<%# Eval("ApplicationID") %>'
                    CssClass="btn btn-primary btn-sm">
                    Download ID
                </asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

<asp:Label ID="Label1" runat="server" Visible="false"></asp:Label>
</asp:Content>
