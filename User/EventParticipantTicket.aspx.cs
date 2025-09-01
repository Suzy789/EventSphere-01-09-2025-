using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Web.UI;
using QRCoder;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
                       pr.ParticipantRole, pr.IsCancelled, pr.AttendanceCode
                FROM ParticipantRegistrations pr
                INNER JOIN Events e ON pr.EventID = e.EventID
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
                            con.Open();
                            updateCmd.ExecuteNonQuery();
                            con.Close();
                        }
                    }

                    // Encode ticket info directly in QR
                    string qrText = $"Event Ticket\n" +
                  $"Event: {row["Title"]}\n" +
                  $"Date: {Convert.ToDateTime(row["EventDate"]):yyyy-MM-dd}\n" +
                  $"Location: {row["Location"]}\n" +
                  $"Code: {row["AttendanceCode"]}";
                    row["QRCodeUrl"] = GenerateQRCodeBase64(qrText);
                }

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
                SELECT e.Title, e.Date, e.Location, pr.ParticipantRole, pr.AttendanceCode
                FROM ParticipantRegistrations pr
                INNER JOIN Events e ON pr.EventID = e.EventID
                WHERE pr.RegistrationID=@RegID";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@RegID", registrationId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string title = reader["Title"].ToString();
                    DateTime date = Convert.ToDateTime(reader["Date"]);
                    string location = reader["Location"].ToString();
                    
                    string code = reader["AttendanceCode"].ToString();

                    GeneratePdfTicket(title, date, location, code);

                }
            }
        }

        private void GeneratePdfTicket(string title, DateTime date, string location, string code)

        {

            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(new iTextSharp.text.Rectangle(300f, 450f));

                doc.SetMargins(15f, 15f, 15f, 15f);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Border
                PdfContentByte cb = writer.DirectContent;
                cb.SetColorStroke(new BaseColor(0, 102, 204));
                cb.SetLineWidth(2f);
                cb.Rectangle(doc.LeftMargin, doc.BottomMargin,
                    doc.PageSize.Width - doc.LeftMargin - doc.RightMargin,
                    doc.PageSize.Height - doc.TopMargin - doc.BottomMargin);
                cb.Stroke();

                // Heading
                var headingFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, new BaseColor(0, 102, 204));
                Paragraph heading = new Paragraph("🎫 Event Ticket", headingFont) { Alignment = Element.ALIGN_CENTER };
                doc.Add(heading);
                doc.Add(new Paragraph("\n"));

                // Info Table
                PdfPTable table = new PdfPTable(2) { WidthPercentage = 100 };
                table.SetWidths(new float[] { 35f, 65f });
                var labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                var valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);

                AddRow(table, "Event:", title, labelFont, valueFont);
                AddRow(table, "Date:", date.ToString("yyyy-MM-dd"), labelFont, valueFont);
                AddRow(table, "Location:", location, labelFont, valueFont);
              
                AddRow(table, "Code:", code, labelFont, valueFont);

                doc.Add(table);
                doc.Add(new Paragraph("\n"));

                // QR Code
                var qrImg = GenerateQRCodeImage($"🎫 Event Ticket\nEvent: {title}\nDate: {date:yyyy-MM-dd}\nLocation: {location}\nCode: {code}");


                qrImg.ScaleAbsolute(120f, 120f);
                qrImg.Alignment = Element.ALIGN_CENTER;
                doc.Add(qrImg);

                // Footer
                var noteFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10, BaseColor.GRAY);
                Paragraph note = new Paragraph("✔ Show this ticket for entry and attendance.", noteFont) { Alignment = Element.ALIGN_CENTER };
                doc.Add(note);

                doc.Close();

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", $"attachment;filename=Ticket_{title}.pdf");
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
        }

        private void AddRow(PdfPTable table, string label, string value, iTextSharp.text.Font labelFont, iTextSharp.text.Font valueFont)

        {
            PdfPCell c1 = new PdfPCell(new Phrase(label, labelFont)) { Border = iTextSharp.text.Rectangle.NO_BORDER, Padding = 4f };
            PdfPCell c2 = new PdfPCell(new Phrase(value, valueFont)) { Border = iTextSharp.text.Rectangle.NO_BORDER, Padding = 4f };

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