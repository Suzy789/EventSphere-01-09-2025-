using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Authentication.User
{
	public partial class DownloadID : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request.QueryString["appid"] == null)
			{
				Response.Write("❌ Invalid request.");
				Context.ApplicationInstance.CompleteRequest();
				return;
			}
			ViewState["AppID"] = Convert.ToInt32(Request.QueryString["appid"]);
		}

		protected void btnDownloadNow_Click(object sender, EventArgs e)
		{
			if (ViewState["AppID"] == null) return;
			int appId = (int)ViewState["AppID"];

			// Fetch details
			string fullName, eventTitle, duty, photoPath, attendanceCode;
			FetchVolunteerData(appId, out fullName, out eventTitle, out duty, out photoPath, out attendanceCode);

			if (string.IsNullOrEmpty(fullName))
			{
				Response.Write("❌ Volunteer application not found.");
				return;
			}

			using (MemoryStream ms = new MemoryStream())
			{
				Document doc = new Document(new Rectangle(300f, 450f));
				doc.SetMargins(15f, 15f, 15f, 15f);
				PdfWriter writer = PdfWriter.GetInstance(doc, ms);
				doc.Open();

				// Draw border
				PdfContentByte cb = writer.DirectContent;
				cb.SetColorStroke(new BaseColor(0, 102, 204));
				cb.SetLineWidth(2f);
				cb.Rectangle(doc.LeftMargin, doc.BottomMargin,
							 doc.PageSize.Width - doc.LeftMargin - doc.RightMargin,
							 doc.PageSize.Height - doc.TopMargin - doc.BottomMargin);
				cb.Stroke();

				// Heading
				var headingFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, new BaseColor(0, 102, 204));
				Paragraph heading = new Paragraph("EventSphere", headingFont) { Alignment = Element.ALIGN_CENTER };
				doc.Add(heading);

				var subFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
				Paragraph sub = new Paragraph("VOLUNTEER ID CARD", subFont) { Alignment = Element.ALIGN_CENTER };
				doc.Add(sub);
				doc.Add(new Paragraph("\n"));

				// Photo
				if (!string.IsNullOrEmpty(photoPath))
				{
					string absPhoto = Server.MapPath("~/" + photoPath.TrimStart('/'));
					if (File.Exists(absPhoto))
					{
						iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(absPhoto);
						img.ScaleAbsolute(90f, 90f);
						img.Alignment = Element.ALIGN_CENTER;
						doc.Add(img);
						doc.Add(new Paragraph("\n"));
					}
					else
					{
						Paragraph noPhoto = new Paragraph("[Photo not found]", subFont) { Alignment = Element.ALIGN_CENTER };
						doc.Add(noPhoto);
						doc.Add(new Paragraph("\n"));
					}
				}

				// Info Table
				PdfPTable table = new PdfPTable(2) { WidthPercentage = 100 };
				table.SetWidths(new float[] { 35f, 65f });
				var labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
				var valueFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);

				AddRow(table, "Name:", fullName, labelFont, valueFont);
				AddRow(table, "Event:", eventTitle, labelFont, valueFont);
				AddRow(table, "Role:", duty, labelFont, valueFont);
				AddRow(table, "Code:", attendanceCode, labelFont, valueFont);

				doc.Add(table);
				doc.Add(new Paragraph("\n"));

				// Footer
				var noteFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10, BaseColor.GRAY);
				Paragraph note = new Paragraph("✔ Show this card for entry and attendance.", noteFont)
				{ Alignment = Element.ALIGN_CENTER };
				doc.Add(note);

				doc.Close();

				// Output PDF
				Response.Clear();
				Response.ContentType = "application/pdf";
				Response.AddHeader("Content-Disposition", $"attachment;filename=VolunteerID_{appId}.pdf");
				Response.BinaryWrite(ms.ToArray());
				Context.ApplicationInstance.CompleteRequest();
			}
		}

		private void FetchVolunteerData(int appId, out string fullName, out string eventTitle, out string duty, out string photoPath, out string attendanceCode)
		{
			fullName = eventTitle = duty = photoPath = attendanceCode = "";

			string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
			using (SqlConnection conn = new SqlConnection(connStr))
			{
				conn.Open();
				string query = @"
                    SELECT 
                        u.FullName,
                        e.Title AS EventTitle,
                        c.CategoryName,
                        ISNULL(va.PhotoPath,'') AS PhotoPath,
                        ISNULL(va.AttendanceCode,'') AS AttendanceCode
                    FROM VolunteerApplications va
                    INNER JOIN Users u ON va.UserID = u.UserID
                    INNER JOIN Events e ON va.EventID = e.EventID
                    INNER JOIN VolunteerCategories c ON va.CategoryID = c.CategoryID AND c.EventID = va.EventID
                    WHERE va.ApplicationID = @AppID";

				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@AppID", appId);
					using (SqlDataReader dr = cmd.ExecuteReader())
					{
						if (dr.Read())
						{
							fullName = dr["FullName"].ToString();
							eventTitle = dr["EventTitle"].ToString();
							duty = dr["CategoryName"].ToString();
							photoPath = dr["PhotoPath"].ToString();
							attendanceCode = dr["AttendanceCode"].ToString();
						}
					}
				}
			}
		}

		private void AddRow(PdfPTable table, string label, string value, Font labelFont, Font valueFont)
		{
			PdfPCell c1 = new PdfPCell(new Phrase(label, labelFont))
			{
				Border = Rectangle.NO_BORDER,
				Padding = 4f
			};
			table.AddCell(c1);

			PdfPCell c2 = new PdfPCell(new Phrase(value, valueFont))
			{
				Border = Rectangle.NO_BORDER,
				Padding = 4f
			};
			table.AddCell(c2);
		}
	}
}
