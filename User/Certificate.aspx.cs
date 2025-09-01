using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Authentication.User
{
	public partial class Certificate : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                string type = Request.QueryString["type"]?.ToLower();
                if (type == "participant")
                {
                    litHeading.Text = "<h2 class='form-title text-center text-primary fw-bold mb-4'>🎖 Certificate of Participation</h2>";
                }
                else
                {
                    litHeading.Text = "<h2 class='form-title text-center text-primary fw-bold mb-4'>🎖 Certificate of Volunteer Appreciation</h2>";
                }
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            int eventId = Convert.ToInt32(Request.QueryString["eventid"]);
            string type = Request.QueryString["type"]?.ToLower();
            int userId = Convert.ToInt32(Session["UserID"]);

            if (type == "participant")
                GenerateParticipantCertificate(eventId, userId);
            else
                GenerateVolunteerCertificate(eventId, userId);
        }

        // ========================== PARTICIPANT =============================

        private void GenerateParticipantCertificate(int eventId, int userId)
        {
            string fullName = "", eventTitle = "", eventDate = "", location = "";
            bool isCompleted = false;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                string query = @"
                SELECT u.FullName, e.Title, FORMAT(e.Date, 'dd MMMM yyyy') AS EventDate, e.Location,
                       CASE WHEN c.CertificateID IS NOT NULL THEN 1 ELSE 0 END AS IsCompleted
                FROM ParticipantRegistrations pr
                INNER JOIN Users u ON pr.UserID = u.UserID
                INNER JOIN Events e ON pr.EventID = e.EventID
                LEFT JOIN Certificates c ON c.EventID = pr.EventID AND c.UserID = pr.UserID AND c.Role = 'Participant'
                WHERE pr.EventID = @EventID AND pr.UserID = @UserID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EventID", eventId);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    fullName = reader["FullName"].ToString();
                    eventTitle = reader["Title"].ToString();
                    eventDate = reader["EventDate"].ToString();
                    location = reader["Location"].ToString();
                    isCompleted = Convert.ToBoolean(reader["IsCompleted"]);
                }
            }

            if (!isCompleted)
            {
                Response.Write("<script>alert('⚠️ Certificate is not yet available.'); window.location='MyParticipatedEvents.aspx';</script>");
                return;
            }

            GeneratePDFCertificate(fullName, eventTitle, eventDate, location, "Participant");
        }

        // ========================== VOLUNTEER =============================

        private void GenerateVolunteerCertificate(int eventId, int userId)
        {
            string fullName = "", eventTitle = "", eventDate = "", location = "", role = "";
            bool isCompleted = false;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                string query = @"
                SELECT u.FullName, e.Title, FORMAT(e.Date, 'dd MMMM yyyy') AS EventDate, e.Location,
                       vc.CategoryName AS Role,
                       ISNULL(vd.IsCompleted, 0) AS IsCompleted
                FROM VolunteerApplications va
                INNER JOIN Users u ON va.UserID = u.UserID
                INNER JOIN Events e ON va.EventID = e.EventID
                LEFT JOIN VolunteerDuties vd ON vd.EventID = va.EventID AND vd.VolunteerID = va.UserID
                LEFT JOIN VolunteerCategories vc ON vc.CategoryID = va.CategoryID
                WHERE va.EventID = @EventID AND va.UserID = @UserID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EventID", eventId);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    fullName = reader["FullName"].ToString();
                    eventTitle = reader["Title"].ToString();
                    eventDate = reader["EventDate"].ToString();
                    location = reader["Location"].ToString();
                    role = reader["Role"].ToString();
                    isCompleted = Convert.ToBoolean(reader["IsCompleted"]);
                }
            }

            if (!isCompleted)
            {
                Response.Write("<script>alert('⚠️ Certificate is only available after duty completion.'); window.location='MyVolunteerEvents.aspx';</script>");
                return;
            }

            GeneratePDFCertificate(fullName, eventTitle, eventDate, location, role);
        }

        // ========================== PDF GENERATOR =============================

        private void GeneratePDFCertificate(string fullName, string title, string date, string location, string role)
        {
            Document doc = new Document(PageSize.A4.Rotate(), 40, 40, 60, 60);
            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // Border
                Rectangle border = new Rectangle(doc.PageSize)
                {
                    Left = doc.LeftMargin - 10,
                    Right = doc.PageSize.Width - doc.RightMargin + 10,
                    Top = doc.PageSize.Height - doc.TopMargin + 10,
                    Bottom = doc.BottomMargin - 10,
                    BorderWidth = 4,
                    BorderColor = new BaseColor(41, 128, 185),
                    Border = Rectangle.BOX
                };
                doc.Add(border);

                // Header
                Font headingFont = FontFactory.GetFont("Helvetica", 28, Font.BOLD, new BaseColor(41, 128, 185));
                doc.Add(new Paragraph("EventSphere", headingFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                });

                string certTitle = role == "Participant" ? "Certificate of Participation" : "Certificate of Volunteer Appreciation";
                Font titleFont = FontFactory.GetFont("Helvetica", 22, Font.BOLD);
                doc.Add(new Paragraph(certTitle, titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10f
                });

                Font subtitleFont = FontFactory.GetFont("Helvetica", 14, Font.ITALIC, BaseColor.DARK_GRAY);
                doc.Add(new Paragraph("Presented with gratitude by EventSphere", subtitleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 25f
                });

                Font nameFont = FontFactory.GetFont("Times-Bold", 24, Font.BOLD);
                doc.Add(new Paragraph("This certificate is awarded to", FontFactory.GetFont("Times-Roman", 16, Font.NORMAL))
                {
                    Alignment = Element.ALIGN_CENTER
                });
                doc.Add(new Paragraph(fullName, nameFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                });

                Font bodyFont = FontFactory.GetFont("Helvetica", 13, Font.NORMAL);
                string bodyText = role == "Participant"
                    ? $"In recognition of participation in the event:\n\n“{title}”\n\nheld on {date} at {location}.\n\nYour presence and contribution are appreciated."
                    : $"For outstanding service in the event:\n\n“{title}”\n\nheld on {date} at {location}.\n\nRole Undertaken: {role}\n\nYour contribution is deeply valued.";

                doc.Add(new Paragraph(bodyText, bodyFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingBefore = 10f,
                    SpacingAfter = 30f
                });

                PdfPTable footerTable = new PdfPTable(2)
                {
                    WidthPercentage = 80,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                footerTable.SetWidths(new float[] { 1f, 1f });

                Font footerFont = FontFactory.GetFont("Helvetica", 12, Font.ITALIC, BaseColor.GRAY);
                footerTable.AddCell(new PdfPCell(new Phrase("_______________________\nOrganizer Signature", footerFont))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER
                });
                footerTable.AddCell(new PdfPCell(new Phrase("Issued by EventSphere", footerFont))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_CENTER
                });

                doc.Add(footerTable);
                doc.Close();

                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", $"attachment;filename={role}_Certificate.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
        }
    }
}
