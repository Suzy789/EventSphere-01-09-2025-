using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing; // For Bitmap
using System.IO;
using System.Web.UI;
using QRCoder;
using iTextSharp.text; // For PDF
using iTextSharp.text.pdf;
using System.Web;

// --- alias fix: avoid Rectangle conflict ---
using PdfRectangle = iTextSharp.text.Rectangle;

namespace Authentication.User
{
    public partial class EventParticipantTicket : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                LoadTickets();
            }
        }

        private void LoadTickets()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            string query = @"
                SELECT pr.RegistrationID, e.Title, e.Date AS EventDate, e.Location,
       pr.ParticipantRole, pr.IsCancelled, pr.AttendanceCode, u.FullName,
       sc.SubCategoryName
FROM ParticipantRegistrations pr
INNER JOIN Events e ON pr.EventID = e.EventID
INNER JOIN Users u ON pr.UserID = u.UserID
LEFT JOIN ParticipantSubEventRegistrations psr ON psr.RegistrationID = pr.RegistrationID
LEFT JOIN ParticipantSubCategories sc ON sc.SubCategoryID = psr.SubCategoryID
WHERE pr.UserID=@UserID AND pr.IsCancelled=0
     AND CAST(e.Date AS DATE) >= CAST(GETDATE() AS DATE)
ORDER BY e.Date ASC";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (!dt.Columns.Contains("QRCodeUrl"))
                    dt.Columns.Add("QRCodeUrl", typeof(string));

                con.Open();
                foreach (DataRow row in dt.Rows)
                {
                    string code = row["AttendanceCode"].ToString();
                    if (string.IsNullOrEmpty(code))
                    {
                        code = GenerateRandomCode(8);
                        row["AttendanceCode"] = code;

                        string updateQuery = "UPDATE ParticipantRegistrations SET AttendanceCode=@Code WHERE RegistrationID=@RegID";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                        {
                            updateCmd.Parameters.AddWithValue("@Code", code);
                            updateCmd.Parameters.AddWithValue("@RegID", row["RegistrationID"]);
                            updateCmd.ExecuteNonQuery();
                        }
                    }

                    // Encode ticket info for QR
                    string subCategory = row["SubCategoryName"]?.ToString();
                    string displayTitle = !string.IsNullOrEmpty(subCategory)
                        ? $"{row["Title"]} — {subCategory}"
                        : $"{row["Title"]} — General";

                    // Encode QR with subcategory included
                    string qrText =
                        $"Event={displayTitle};" +
                        $"Date={Convert.ToDateTime(row["EventDate"]).ToString("dd MMM yyyy hh:mm tt")};" +
                        $"Venue={row["Location"]};" +
                        $"Participant={row["FullName"]};" +
                        $"Code={row["AttendanceCode"]}";

                    row["QRCodeUrl"] = GenerateQRCodeBase64(qrText);

                }
                con.Close();

                rptTickets.DataSource = dt;
                rptTickets.DataBind();
            }
        }

        private string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] codeChars = new char[length];
            for (int i = 0; i < length; i++)
                codeChars[i] = chars[random.Next(chars.Length)];
            return new string(codeChars);
        }

        private string GenerateQRCodeBase64(string text)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new QRCode(qrCodeData))
            using (Bitmap bitmap = qrCode.GetGraphic(20))
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
            }
        }

        protected void btnDownloadTicket_Click(object sender, EventArgs e)
        {
            int registrationId = Convert.ToInt32((sender as System.Web.UI.WebControls.Button).CommandArgument);

            string query = @"
               SELECT e.Title, e.Date, e.Location, pr.ParticipantRole, pr.AttendanceCode, 
       u.FullName, sc.SubCategoryName
FROM ParticipantRegistrations pr
INNER JOIN Events e ON pr.EventID = e.EventID
INNER JOIN Users u ON pr.UserID = u.UserID
LEFT JOIN ParticipantSubEventRegistrations psr ON psr.RegistrationID = pr.RegistrationID
LEFT JOIN ParticipantSubCategories sc ON sc.SubCategoryID = psr.SubCategoryID
WHERE pr.RegistrationID=@RegID
";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@RegID", registrationId);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string title = reader["Title"].ToString();
                        DateTime date = Convert.ToDateTime(reader["Date"]);
                        string location = reader["Location"].ToString();
                        string code = reader["AttendanceCode"].ToString();
                        string participant = reader["FullName"].ToString();
                        string subCategory = reader["SubCategoryName"]?.ToString();
                        GeneratePdfTicket(title, date, location, participant, code, subCategory);
                    }
                }
            }
        }

        private void GeneratePdfTicket(string title, DateTime date, string location, string participant, string code, string subCategory = "")
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(new PdfRectangle(420f, 260f));
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Background
                PdfContentByte canvas = writer.DirectContentUnder;
                BaseColor bgColor = new BaseColor(245, 250, 255);
                canvas.SetColorFill(bgColor);
                canvas.Rectangle(0, 0, doc.PageSize.Width, doc.PageSize.Height);
                canvas.Fill();

                // Header bar
                BaseColor headerColor = new BaseColor(0, 51, 102);
                canvas.SetColorFill(headerColor);
                canvas.Rectangle(0, doc.PageSize.Height - 40, doc.PageSize.Width, 40);
                canvas.Fill();

                // Header text
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.BLACK);
                Paragraph header = new Paragraph("🎟 EVENT TICKET", headerFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingBefore = -28
                };
                doc.Add(header);

                // Branding top right
                var brandFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_RIGHT,
                    new Phrase("EventSphere", brandFont),
                    doc.PageSize.Width - 10, doc.PageSize.Height - 25, 0);

                // Layout
                PdfPTable layout = new PdfPTable(2);
                layout.WidthPercentage = 100;
                layout.SetWidths(new float[] { 65, 35 });
                layout.SpacingBefore = 20f;

                // LEFT: Info
                PdfPTable infoTable = new PdfPTable(2);
                infoTable.WidthPercentage = 100;
                infoTable.SetWidths(new float[] { 30, 70 });

                var labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, new BaseColor(0, 51, 102));
                var valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);

                // 👇 Now we use displayTitle with subCategory
                string displayTitle = !string.IsNullOrEmpty(subCategory)
                    ? $"{title} — {subCategory}"
                    : $"{title} — General";

                AddRow(infoTable, "Event", displayTitle, labelFont, valueFont);
                AddRow(infoTable, "Date", date.ToString("dd MMM yyyy"), labelFont, valueFont);
                AddRow(infoTable, "Time", date.ToString("hh:mm tt"), labelFont, valueFont);
                AddRow(infoTable, "Venue", location, labelFont, valueFont);
                AddRow(infoTable, "Name", participant, labelFont, valueFont);
                AddRow(infoTable, "Code", code, labelFont, valueFont);

                PdfPCell leftCell = new PdfPCell(infoTable) { Border = PdfRectangle.NO_BORDER };
                layout.AddCell(leftCell);

                // RIGHT: QR Code
                string qrText = $"Event={displayTitle};Date={date:dd MMM yyyy hh:mm tt};Venue={location};Participant={participant};Code={code}";
                var qrImg = GenerateQRCodeImage(qrText);

                qrImg.ScaleAbsolute(100f, 100f);

                PdfPCell qrCell = new PdfPCell(qrImg)
                {
                    Border = PdfRectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                layout.AddCell(qrCell);

                doc.Add(layout);

                // Footer disclaimer
                var disclaimerFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, BaseColor.GRAY);
                Paragraph disclaimer = new Paragraph("Disclaimer: This ticket is issued by EventSphere. Please present a valid ID with this ticket at entry.", disclaimerFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingBefore = 15
                };
                doc.Add(disclaimer);

                doc.Close();

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", $"attachment;filename=Ticket_{title.Replace(" ", "_")}.pdf");
                Response.BinaryWrite(ms.ToArray());
                Response.Flush();
                Response.SuppressContent = true;
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }


        private void AddRow(PdfPTable table, string label, string value, iTextSharp.text.Font labelFont, iTextSharp.text.Font valueFont)
        {
            PdfPCell c1 = new PdfPCell(new Phrase(label, labelFont)) { Border = PdfRectangle.NO_BORDER, Padding = 3f };
            PdfPCell c2 = new PdfPCell(new Phrase(value, valueFont)) { Border = PdfRectangle.NO_BORDER, Padding = 3f };
            table.AddCell(c1);
            table.AddCell(c2);
        }

        private iTextSharp.text.Image GenerateQRCodeImage(string text)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new QRCode(qrCodeData))
            using (Bitmap bitmap = qrCode.GetGraphic(20))
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return iTextSharp.text.Image.GetInstance(ms.ToArray());
            }
        }
    }
}

