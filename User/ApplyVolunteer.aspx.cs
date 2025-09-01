using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Authentication.User
{
	public partial class ApplyVolunteer : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			// When the page first loads (not when clicking submit)
			if (!IsPostBack)
			{
				// Check if user is logged in
				if (Session["UserID"] == null)
				{
					Response.Redirect("~/Login.aspx");
					return;
				}

				// Get event id from URL
				string eventIdStr = Request.QueryString["eventid"];
				if (string.IsNullOrEmpty(eventIdStr))
				{
					lblMessage.Text = "Invalid event!";
					lblMessage.CssClass = "message error-message";
					lblMessage.Visible = true;
					btnSubmit.Enabled = false;
					return;
				}

				int eventId = Convert.ToInt32(eventIdStr);
				int userId = Convert.ToInt32(Session["UserID"]);

				// Load all info
				LoadUserDetails(userId);
				LoadEventDetails(eventId);
				LoadVolunteerCategories(eventId);
			}
		}

		// Show categories from database
		private void LoadVolunteerCategories(int eventId)
		{
			ddlRole.Items.Clear();
			ddlRole.Items.Add(new ListItem("Select a role", "")); // first option

			string conStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
			SqlConnection con = new SqlConnection(conStr);
			string query = "SELECT CategoryID, CategoryName, (RequiredVolunteers - ISNULL(AllocatedVolunteers,0)) AS Remaining " +
						   "FROM VolunteerCategories WHERE EventID=@EventID AND (RequiredVolunteers - ISNULL(AllocatedVolunteers,0))>0";
			SqlCommand cmd = new SqlCommand(query, con);
			cmd.Parameters.AddWithValue("@EventID", eventId);

			con.Open();
			SqlDataReader dr = cmd.ExecuteReader();
			while (dr.Read())
			{
				// Example: Registration Desk (Remaining: 5)
				string text = dr["CategoryName"].ToString() + " (Remaining: " + dr["Remaining"].ToString() + ")";
				string value = dr["CategoryID"].ToString();
				ddlRole.Items.Add(new ListItem(text, value));
			}
			dr.Close();
			con.Close();
		}

		// Load user's name & email
		private void LoadUserDetails(int userId)
		{
			string conStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
			SqlConnection con = new SqlConnection(conStr);
			string query = "SELECT FullName, Email FROM Users WHERE UserID=@UserID";
			SqlCommand cmd = new SqlCommand(query, con);
			cmd.Parameters.AddWithValue("@UserID", userId);

			con.Open();
			SqlDataReader dr = cmd.ExecuteReader();
			if (dr.Read())
			{
				txtFullName.Text = dr["FullName"].ToString();
				txtEmail.Text = dr["Email"].ToString();
			}
			dr.Close();
			con.Close();
		}

		// Load event name
		private void LoadEventDetails(int eventId)
		{
			string conStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
			SqlConnection con = new SqlConnection(conStr);
			string query = "SELECT Title FROM Events WHERE EventID=@EventID";
			SqlCommand cmd = new SqlCommand(query, con);
			cmd.Parameters.AddWithValue("@EventID", eventId);

			con.Open();
			SqlDataReader dr = cmd.ExecuteReader();
			if (dr.Read())
			{
				txtEventName.Text = dr["Title"].ToString();
			}
			dr.Close();
			con.Close();
		}
		private string GenerateAttendanceCode(int length = 6)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			var random = new Random();
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}


		// When user clicks Submit button
		protected void btnSubmit_Click(object sender, EventArgs e)
		{
			// Simple validations
			if (!chkAgree.Checked)
			{
				ShowMessage("You must agree to the guidelines.", false);
				return;
			}
			if (ddlRole.SelectedValue == "")
			{
				ShowMessage("Please select a role.", false);
				return;
			}
			if (txtEmergencyContact.Text.Trim() == "")
			{
				ShowMessage("Please enter emergency contact.", false);
				return;
			}

			int eventId = Convert.ToInt32(Request.QueryString["eventid"]);
			int userId = Convert.ToInt32(Session["UserID"]);
			int categoryId = Convert.ToInt32(ddlRole.SelectedValue);
			// File upload folder
			string folderPath = Server.MapPath("~/Uploads/Volunteers/");
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			// Save files
			string idProofPath = SaveFile(fuIDProof, folderPath);
			string photoPath = SaveFile(fuPhoto, folderPath);
			string attendanceCode = GenerateAttendanceCode();

			// Save to database
			string conStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
			SqlConnection con = new SqlConnection(conStr);
			con.Open();

			// Check duplicate
			string checkQuery = "SELECT COUNT(*) FROM VolunteerApplications WHERE EventID=@EID AND UserID=@UID";
			SqlCommand checkCmd = new SqlCommand(checkQuery, con);
			checkCmd.Parameters.AddWithValue("@EID", eventId);
			checkCmd.Parameters.AddWithValue("@UID", userId);
			int already = (int)checkCmd.ExecuteScalar();
			if (already > 0)
			{
				ShowMessage("You already applied for this event.", false);
				con.Close();
				return;
			}

			// Insert new row
			string insert = @"INSERT INTO VolunteerApplications 
            (EventID,UserID,CategoryID,VolunteeredBefore,ExperienceDescription,
             EmergencyContact,IDProofPath,PhotoPath,AgreedToGuidelines,ConsentForCertificate,AttendanceCode) 
            VALUES (@EID,@UID,@CID,@VB,@Exp,@Contact,@ID,@Photo,@Agree,@Consent,@Code)";
			using (SqlCommand cmd = new SqlCommand(insert, con))
			{
				cmd.Parameters.AddWithValue("@EID", eventId);
				cmd.Parameters.AddWithValue("@UID", userId);
				cmd.Parameters.AddWithValue("@CID", categoryId);
				cmd.Parameters.AddWithValue("@VB", rblVolunteeredBefore.SelectedValue == "Yes");
				cmd.Parameters.AddWithValue("@Exp", txtExperience.Text.Trim());
				cmd.Parameters.AddWithValue("@Contact", txtEmergencyContact.Text.Trim());
				cmd.Parameters.AddWithValue("@ID", (object)SaveFile(fuIDProof, Server.MapPath("~/Uploads/Volunteers/")) ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@Photo", (object)SaveFile(fuPhoto, Server.MapPath("~/Uploads/Volunteers/")) ?? DBNull.Value);
				cmd.Parameters.AddWithValue("@Agree", chkAgree.Checked);
				cmd.Parameters.AddWithValue("@Consent", chkCertificateConsent.Checked);
				cmd.Parameters.AddWithValue("@Code", attendanceCode);

				cmd.ExecuteNonQuery();
				con.Close();
			}

			ShowMessage("Application submitted successfully!", true);
			btnSubmit.Enabled = false;
		}

		// Save file and return path
		private string SaveFile(FileUpload fu, string folder)
		{
			if (fu.HasFile)
			{
				string fileName = Guid.NewGuid().ToString() + "_" + fu.FileName;
				fu.SaveAs(Path.Combine(folder, fileName));
				return "Uploads/Volunteers/" + fileName;
			}
			return null;
		}

		// Show message in label
		private void ShowMessage(string msg, bool success)
		{
			lblMessage.Text = msg;
			lblMessage.CssClass = success ? "message success-message" : "message error-message";
			lblMessage.Visible = true;
		}
	}
}
