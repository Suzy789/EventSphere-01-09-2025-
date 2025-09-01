<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="InviteFriends.aspx.cs" Inherits="Authentication.User.InviteFriends" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <div class="container py-4">
        <div class="bg-light p-4 rounded shadow-sm">
            <h2 class="text-center text-primary mb-4">Invite Friends (Offline Friendly)</h2>
            <p class="text-center text-muted">Share event details by copying, printing, downloading, or emailing.</p>

            <asp:GridView ID="gvUserEvents" runat="server" AutoGenerateColumns="False"
                CssClass="table table-bordered table-striped text-center align-middle">
                <Columns>
                    <asp:BoundField DataField="EventTitle" HeaderText="Event Title" />
                    <asp:BoundField DataField="EventDate" HeaderText="Date" DataFormatString="{0:dd MMM yyyy}" />

                    <asp:TemplateField HeaderText="Invite Options">
                        <ItemTemplate>
                            <!-- Event URL textbox -->
                            <div class="input-group mb-2">
                                <asp:TextBox ID="txtEventURL" runat="server"
                                    CssClass="form-control text-center"
                                    Text='<%# Eval("EventURL") %>' ReadOnly="true" />
                                <button type="button" class="btn btn-outline-primary"
                                    onclick="copyToClipboard('<%# Eval("EventURL") %>')">
                                    Copy
                                </button>
                            </div>

                            <!-- Action buttons -->
                            <div class="d-flex flex-wrap justify-content-center gap-2">
                                <!-- Email -->
                                <asp:HyperLink ID="lnkEmail" runat="server"
                                    CssClass="btn btn-success btn-sm"
                                    Text="Email Invite"
                                    NavigateUrl='<%# "mailto:?subject=Join this Event&body=Check out this event: " + Eval("EventURL") %>' />

                                <!-- Print -->
                                <asp:Button ID="btnPrint" runat="server"
                                    Text="Print"
                                    CssClass="btn btn-dark btn-sm"
                                    OnClientClick='<%# "printEvent(\"" + Eval("EventTitle") + "\", \"" + Eval("EventURL") + "\"); return false;" %>' />

                                <!-- Download -->
                                <asp:Button ID="btnDownload" runat="server"
                                    Text="Download Info"
                                    CssClass="btn btn-secondary btn-sm"
                                    OnClientClick='<%# "downloadTxtFile(\"" + Eval("EventTitle") + "\", \"" + Eval("EventURL") + "\"); return false;" %>' />
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-success mt-3 fw-bold d-block text-center" />
        </div>
    </div>

    <script type="text/javascript">
        // Copy to clipboard (works in IE11 and modern browsers)
        function copyToClipboard(text) {
            var tempInput = document.createElement("input");
            tempInput.value = text;
            document.body.appendChild(tempInput);
            tempInput.select();
            try {
                document.execCommand("copy");
                alert("Link copied: " + text);
            } catch (err) {
                alert("Please copy manually: " + text);
            }
            document.body.removeChild(tempInput);
        }

        // Print event details (IE & modern friendly)
        function printEvent(title, url) {
            var content = "Event: " + title + "\nJoin: " + url;
            var printWindow = window.open('', '', 'height=600,width=400');
            if (!printWindow) {
                alert("Popup blocked. Please allow popups for this site.");
                return;
            }
            printWindow.document.write("<pre>" + content + "</pre>");
            printWindow.document.close();
            printWindow.focus();
            printWindow.print();
        }

        // Download as .txt (with IE11 fallback)
        function downloadTxtFile(title, url) {
            var content = "Event: " + title + "\r\nLink: " + url;

            if (window.navigator && window.navigator.msSaveOrOpenBlob) {
                // IE10+ fallback
                var blob = new Blob([content], { type: 'text/plain' });
                window.navigator.msSaveOrOpenBlob(blob, title.replace(/\s+/g, '_') + "_Invite.txt");
            } else {
                // Modern browsers
                var blob = new Blob([content], { type: 'text/plain' });
                var a = document.createElement("a");
                a.href = window.URL.createObjectURL(blob);
                a.download = title.replace(/\s+/g, '_') + "_Invite.txt";
                a.click();
            }
        }
    </script>
</asp:Content>
