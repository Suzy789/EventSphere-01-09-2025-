using DocumentFormat.OpenXml.Bibliography;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Windows.Controls;

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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                string query = @"
;WITH Combined AS (
    -- Main participant (Pra1)
    SELECT 
        u.FullName AS PersonName,
        u.UserID,
        NULL AS MemberID,
        e.Title AS EventTitle,
        FORMAT(e.Date, 'dd MMMM yyyy') AS EventDate,
        e.Location,
        sc.SubCategoryName,
        CASE WHEN c.CertificateID IS NOT NULL THEN 1 ELSE 0 END AS IsCompleted
    FROM ParticipantRegistrations pr
    INNER JOIN Events e ON pr.EventID = e.EventID
    INNER JOIN Users u ON pr.UserID = u.UserID
    INNER JOIN ParticipantSubEventRegistrations psr ON psr.RegistrationID = pr.RegistrationID -- ✅ force subcategory
    INNER JOIN ParticipantSubCategories sc ON sc.SubCategoryID = psr.SubCategoryID
    LEFT JOIN Certificates c 
        ON c.EventID = pr.EventID AND c.UserID = u.UserID AND c.Role = 'Participant'
    WHERE pr.EventID = @EventID AND pr.UserID = @UserID

    UNION ALL

    -- Team members (exclude logged-in user if somehow stored here)
    SELECT 
        ptm.MemberName AS PersonName,
        NULL AS UserID,
        ptm.MemberID,
        e.Title AS EventTitle,
        FORMAT(e.Date, 'dd MMMM yyyy') AS EventDate,
        e.Location,
        sc.SubCategoryName,
        CASE WHEN c.CertificateID IS NOT NULL THEN 1 ELSE 0 END AS IsCompleted
    FROM ParticipantRegistrations pr
    INNER JOIN Events e ON pr.EventID = e.EventID
    INNER JOIN ParticipantTeamMembers ptm ON ptm.RegistrationID = pr.RegistrationID
    LEFT JOIN ParticipantSubCategories sc 
    ON sc.SubCategoryID = ISNULL(ptm.SubCategoryID, (
        SELECT TOP 1 psr.SubCategoryID
        FROM ParticipantSubEventRegistrations psr
        WHERE psr.RegistrationID = pr.RegistrationID
    ))
    LEFT JOIN Certificates c 
        ON c.EventID = pr.EventID AND c.MemberID = ptm.MemberID AND c.Role = 'Participant'
    WHERE pr.EventID = @EventID AND pr.UserID = @UserID
      AND ptm.MemberName <> (SELECT FullName FROM Users WHERE UserID = @UserID) -- ✅ avoid duplicate Eric
)
-- ✅ must SELECT from CTE
SELECT PersonName, UserID, MemberID, EventTitle, EventDate, Location, SubCategoryName, IsCompleted
FROM Combined;";



                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EventID", eventId);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                // ✅ add these two
                HashSet<string> issued = new HashSet<string>();
                bool anyCompleted = false;

                Document doc = new Document(PageSize.A4.Rotate(), 40, 40, 60, 60);
                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter.GetInstance(doc, ms);
                    doc.Open();

                    bool firstPage = true;
                    while (reader.Read())
                    {
                        string fullName = reader["PersonName"].ToString();
                        string eventTitle = reader["EventTitle"].ToString();
                        string eventDate = reader["EventDate"].ToString();
                        string location = reader["Location"].ToString();
                        string subCategory = reader["SubCategoryName"]?.ToString();
                        bool isCompleted = Convert.ToBoolean(reader["IsCompleted"]);

                        // ✅ enforce Pra1's own subcategory
                        if (reader["UserID"] != DBNull.Value && Convert.ToInt32(reader["UserID"]) == userId)
                        {
                            subCategory = reader["SubCategoryName"]?.ToString();
                        }

                        // Prevent duplicates
                        string key = fullName + "|" + (subCategory ?? "");
                        if (issued.Contains(key)) continue;
                        issued.Add(key);

                        if (!isCompleted)
                        {
                            SaveCertificateRecord(eventId, reader);
                            isCompleted = true;
                        }

                        anyCompleted = true;

                        if (!firstPage) doc.NewPage();   // ✅ only between pages
                        AddCertificatePage(doc, fullName, eventTitle, eventDate, location, "Participant", subCategory);
                        firstPage = false;
                    }

                    doc.Close();

                    if (!anyCompleted)
                    {
                        Response.Write("<script>alert('⚠️ Certificate is not yet available.'); window.location='MyParticipatedEvents.aspx';</script>");
                        return;
                    }

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=Participant_Certificates.pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite(ms.ToArray());
                    Response.End();
                }
            }
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
        private void SaveCertificateRecord(int eventId, SqlDataReader reader)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            {
                string insert = @"INSERT INTO Certificates (EventID, UserID, MemberID, Role, IssuedOn)
                          VALUES (@EventID, @UserID, @MemberID, 'Participant', GETDATE())";

                SqlCommand cmd = new SqlCommand(insert, conn);
                cmd.Parameters.AddWithValue("@EventID", eventId);

                if (reader["UserID"] != DBNull.Value)
                {
                    cmd.Parameters.AddWithValue("@UserID", Convert.ToInt32(reader["UserID"]));
                    cmd.Parameters.AddWithValue("@MemberID", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@UserID", DBNull.Value);
                    cmd.Parameters.AddWithValue("@MemberID", Convert.ToInt32(reader["MemberID"]));
                }

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void AddCertificatePage(Document doc, string fullName, string title, string date, string location, string role, string subCategory = "")
        {
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
            Font eventFont = FontFactory.GetFont("Times-Bold", 18, Font.BOLD, BaseColor.BLACK);

            string eventDisplay = !string.IsNullOrEmpty(subCategory)
     ? $"{title} — {subCategory}"
     : $"{title} — General";

            string bodyText = role == "Participant"
                ? "In recognition of participation in the event:"
                : "For outstanding service in the event:";

            doc.Add(new Paragraph(eventDisplay, eventFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingBefore = 10f,
                SpacingAfter = 15f
            });

            bodyText += $"\n\nheld on {date} at {location}.";

            if (role == "Participant")
                bodyText += "\n\nYour presence and contribution are appreciated.";
            else
                bodyText += $"\n\nRole Undertaken: {role}\n\nYour contribution is deeply valued.";

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
        }

        private void GeneratePDFCertificate(string fullName, string title, string date, string location, string role, string subCategory = "")
        {
            Document doc = new Document(PageSize.A4.Rotate(), 40, 40, 60, 60);
            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                AddCertificatePage(doc, fullName, title, date, location, role, subCategory);

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

