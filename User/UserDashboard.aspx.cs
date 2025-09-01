using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Authentication.User
{
    public partial class UserDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
			if (!IsPostBack)
			{
				if (Session["UserID"] == null || Session["Role"] == null)
				{
					Response.Redirect("~/Login.aspx");
					return;
				}

				SetCardVisibility();
				//LoadDashboardStats();
				//LoadUpcomingEvents();

				int userId = Convert.ToInt32(Session["UserID"]);
				CheckApprovedApplication(userId);
			}
		}

		private void SetCardVisibility()
		{
			string role = Session["Role"]?.ToString();

			// Organizer Section
			bool isOrganizer = role == "Organizer";
			phCreateEvent.Visible = isOrganizer;
			phMyOrganizedEvents.Visible = isOrganizer;
			phVolunteerApplications.Visible = isOrganizer;
			phApprovedVolunteerList.Visible = isOrganizer;
			phParticipantManager.Visible = isOrganizer;
			phMarkAttendance.Visible = isOrganizer;
			phEventReport.Visible = isOrganizer;
			phViewFeedback.Visible = isOrganizer;
			phEventsCreated.Visible = isOrganizer;
			phParticipantsRegistered.Visible = isOrganizer;
			phEventDetailsExport.Visible = isOrganizer;

			// Volunteer Section
			bool isVolunteer = role == "Volunteer";
			phManageVolunteerEvents.Visible = isVolunteer;
			phMarkParticipantAttendance.Visible = isVolunteer;
			phAssignedDuties.Visible = isVolunteer;
			phCheckAttendance.Visible = isVolunteer;
			phVolunteerStatus.Visible = isVolunteer;
			phFeedbackVolunteer.Visible = isVolunteer;
			phEventsVolunteered.Visible = isVolunteer;
			phDutiesAssigned.Visible = isVolunteer;
			

			// Participant Section
			bool isParticipant = role == "Participant";
			phParticipationCount.Visible = isParticipant;
			phParticipationHistory.Visible = isParticipant;
			phEventTickets.Visible = isParticipant;
			phFeedbackParticipant.Visible = isParticipant;
			phBookmarkedEvents.Visible = isParticipant;
			phInviteFriends.Visible = isParticipant;
			phManageParticipation.Visible = isParticipant;

			// Shared (Volunteer + Participant)
			bool isVolunteerOrParticipant = isVolunteer || isParticipant;
			phMyVolunteerEvents.Visible = isVolunteerOrParticipant;
			phCertificatesEarned.Visible = isVolunteerOrParticipant;

			// Shared across all roles
			phBrowseEvents.Visible = true;
		}

		private void LoadDashboardStats()
		{
			int userId = Convert.ToInt32(Session["UserID"]);
			string role = Session["Role"].ToString();
			string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

			using (SqlConnection conn = new SqlConnection(connStr))
			{
				conn.Open();

				if (role == "Participant" || role == "Volunteer")
				{
					SqlCommand cmdCertificates = new SqlCommand("SELECT COUNT(*) FROM Certificates WHERE UserID = @UserID", conn);
					cmdCertificates.Parameters.AddWithValue("@UserID", userId);
					litCertificatesEarned.Text = cmdCertificates.ExecuteScalar().ToString();
				}

				if (role == "Organizer")
				{
					SqlCommand cmdCreated = new SqlCommand("SELECT COUNT(*) FROM Events WHERE OrganizerID = @UserID", conn);
					cmdCreated.Parameters.AddWithValue("@UserID", userId);
					litEventsCreated.Text = cmdCreated.ExecuteScalar().ToString();

					SqlCommand cmdParticipants = new SqlCommand(@"
                        SELECT COUNT(DISTINCT VA.UserID)
                        FROM VolunteerApplications VA
                        INNER JOIN Events E ON VA.EventID = E.EventID
                        INNER JOIN Users U ON VA.UserID = U.UserID
                        WHERE E.OrganizerID = @UserID AND U.Role = 'Participant'", conn);
					cmdParticipants.Parameters.AddWithValue("@UserID", userId);
					litParticipantsRegistered.Text = cmdParticipants.ExecuteScalar().ToString();
				}

				if (role == "Volunteer")
				{
					SqlCommand cmdDuties = new SqlCommand("SELECT COUNT(*) FROM VolunteerDuties WHERE VolunteerID = @UserID", conn);
					cmdDuties.Parameters.AddWithValue("@UserID", userId);
					litDutiesAssigned.Text = cmdDuties.ExecuteScalar().ToString();

					SqlCommand cmdEventsVolunteered = new SqlCommand("SELECT COUNT(DISTINCT EventID) FROM VolunteerApplications WHERE UserID = @UserID AND Status = 'Approved'", conn);
					cmdEventsVolunteered.Parameters.AddWithValue("@UserID", userId);
					litEventsVolunteered.Text = cmdEventsVolunteered.ExecuteScalar().ToString();
				}

				if (role == "Participant")
				{
					SqlCommand cmdParticipation = new SqlCommand("SELECT COUNT(DISTINCT EventID) FROM ParticipantRegistrations WHERE UserID = @UserID", conn);
					cmdParticipation.Parameters.AddWithValue("@UserID", userId);
					litParticipationCount.Text = cmdParticipation.ExecuteScalar().ToString();
				}
			}
		}

		private void CheckApprovedApplication(int userId)
		{
			try
			{
				string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
				using (SqlConnection conn = new SqlConnection(connStr))
				{
					conn.Open();
					string query = @"
                        SELECT TOP 1 ApplicationID
                        FROM VolunteerApplications
                        WHERE UserID=@UID AND Status='Approved' AND IsNotificationShown=0";

					SqlCommand cmd = new SqlCommand(query, conn);
					cmd.Parameters.AddWithValue("@UID", userId);

					object result = cmd.ExecuteScalar();
					if (result != null)
					{
						int applicationId = Convert.ToInt32(result);
						ViewState["ApprovedApplicationID"] = applicationId;

						SqlCommand updateCmd = new SqlCommand("UPDATE VolunteerApplications SET IsNotificationShown=1 WHERE ApplicationID=@AID", conn);
						updateCmd.Parameters.AddWithValue("@AID", applicationId);
						updateCmd.ExecuteNonQuery();

						ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowApprovedModal", "var myModal = new bootstrap.Modal(document.getElementById('approvedModal')); myModal.show();", true);
					}
				}
			}
			catch
			{
				// Log error
			}
		}
		
		protected void btnDownloadID_Click(object sender, EventArgs e)
		{
			if (ViewState["ApprovedApplicationID"] != null)
			{
				int appId = Convert.ToInt32(ViewState["ApprovedApplicationID"]);
				Response.Redirect("~/User/DownloadID.aspx?appid=" + appId);
			}
		}
		
		private void LoadUpcomingEvents()
		{
			int userId = Convert.ToInt32(Session["UserID"]);
			string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

			using (SqlConnection conn = new SqlConnection(connStr))
			{
				conn.Open();
				string query = @"
                    SELECT DISTINCT E.EventID, E.Title, E.Date, E.Location
                    FROM Events E
                    LEFT JOIN VolunteerApplications VA ON E.EventID = VA.EventID AND VA.Status = 'Approved'
                    WHERE (VA.UserID = @UserID OR E.OrganizerID = @UserID)
                      AND E.Date >= GETDATE()
                    ORDER BY E.Date ASC";

				SqlCommand cmd = new SqlCommand(query, conn);
				cmd.Parameters.AddWithValue("@UserID", userId);

				SqlDataReader reader = cmd.ExecuteReader();
				StringBuilder html = new StringBuilder();

				while (reader.Read())
				{
					string title = reader["Title"].ToString();
					string location = reader["Location"].ToString();
					string eventId = reader["EventID"].ToString();
					DateTime date = Convert.ToDateTime(reader["Date"]);

					html.Append("<div class='card'>");
					html.Append($"<h3>{title}</h3>");
					html.Append($"<p><strong>Date:</strong> {date:dd MMM yyyy}</p>");
					html.Append($"<p><strong>Location:</strong> {location}</p>");
					html.Append($"<a href='EventDetails.aspx?eventid={eventId}' class='submit-button' style='margin-top:10px;'>Details</a>");
					html.Append("</div>");
				}

				litUpcomingEvents.Text = html.ToString();
			}
		}
	}
}







