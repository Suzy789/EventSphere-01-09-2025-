<%@ Page Title="Certificate of Volunteering" Language="C#" MasterPageFile="~/MasterPage.Master" 
    AutoEventWireup="true" CodeBehind="Certificate.aspx.cs" Inherits="Authentication.User.Certificate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 <div class="container my-5 p-4 shadow bg-white rounded">
        <asp:Literal ID="litHeading" runat="server" />

        <p class="text-center text-muted mb-4">
            Click the button below to download your certificate in PDF format.
        </p>

        <div class="text-center">
            <asp:Button ID="btnDownload" runat="server" CssClass="btn btn-success btn-lg"
                Text="🎓 Download Certificate"
                OnClick="btnDownload_Click" />
        </div>
    </div>
</asp:Content>
