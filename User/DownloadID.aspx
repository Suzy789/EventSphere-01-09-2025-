<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="DownloadID.aspx.cs" Inherits="Authentication.User.DownloadID" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .id-container {
            margin: 50px auto;
            padding: 30px;
            background: #fff;
            width: 400px;
            text-align: center;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0,0,0,0.1);
            font-family: Arial, sans-serif;
        }
        .id-container h2 {
            color: #333;
            margin-bottom: 15px;
        }
        .id-container p {
            margin-bottom: 20px;
            color: #666;
        }
        .id-button {
            display: inline-block;
            padding: 10px 20px;
            background: #007bff;
            color: #fff;
            border-radius: 5px;
            border: none;
            cursor: pointer;
        }
        .id-button:hover {
            background: #0056b3;
        }
    </style>

    <div class="id-container">
        <h2>Your Volunteer ID Card</h2>
        <p>Click below to download your ID card.</p>
        <asp:Button ID="btnDownloadNow" runat="server" Text="Download ID"
            CssClass="id-button" OnClick="btnDownloadNow_Click" />
    </div>
</asp:Content>
