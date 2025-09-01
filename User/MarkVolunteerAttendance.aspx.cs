using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Authentication.User
{
	public partial class MarkAttendance : System.Web.UI.Page
	{
		private readonly string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["UserID"] == null)
			{
				Response.Redirect("~/Login.aspx");
				return;
			}

			if (!IsPostBack)
			{
				LoadEvents();
			}
		}

		private void LoadEvents()
		{
			using (SqlConnection conn = new SqlConnection(connStr))
			using (SqlCommand cmd = new SqlCommand(@"
                SELECT DISTINCT e.EventID, e.Title
                FROM Events e
                JOIN VolunteerApplications va ON e.EventID = va.EventID
                WHERE e.OrganizerID = @OrganizerID AND va.Status = 'Approved'
                ORDER BY e.Title", conn))
			{
				cmd.Parameters.Add("@OrganizerID", SqlDbType.Int).Value = Session["UserID"];

				conn.Open();
				ddlEvents.DataSource = cmd.ExecuteReader();
				ddlEvents.DataTextField = "Title";
				ddlEvents.DataValueField = "EventID";
				ddlEvents.DataBind();
			}

			ddlEvents.Items.Insert(0, new ListItem("-- Select Event --", ""));
		}

		protected void ddlEvents_SelectedIndexChanged(object sender, EventArgs e)
		{
			gvVolunteers.Visible = false;
			gvVolunteers.DataSource = null;
			gvVolunteers.DataBind();

			gvMarkedVolunteers.DataSource = null;
			gvMarkedVolunteers.DataBind();

			lblMessage.Text = "";
		}

		protected void btnVerify_Click(object sender, EventArgs e)
		{
			lblMessage.Text = "";

			if (string.IsNullOrEmpty(ddlEvents.SelectedValue))
			{
				lblMessage.Text = "⚠️ Please select an event.";
				return;
			}

			if (string.IsNullOrWhiteSpace(txtCode.Text))
			{
				lblMessage.Text = "⚠️ Please enter the attendance code.";
				return;
			}

			int eventId = int.Parse(ddlEvents.SelectedValue);
			string enteredCode = txtCode.Text.Trim();

			if (!IsCodeValid(eventId, enteredCode))
			{
				lblMessage.Text = "❌ Invalid attendance code.";
				return;
			}

			LoadVolunteers(eventId, enteredCode);
			LoadMarkedVolunteers(eventId, enteredCode);
		}

		private bool IsCodeValid(int eventId, string code)
		{
			using (SqlConnection conn = new SqlConnection(connStr))
			using (SqlCommand cmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM VolunteerApplications 
                WHERE EventID = @EventID AND AttendanceCode = @Code", conn))
			{
				cmd.Parameters.Add("@EventID", SqlDbType.Int).Value = eventId;
				cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 50).Value = code;

				conn.Open();
				return (int)cmd.ExecuteScalar() > 0;
			}
		}

		private void LoadVolunteers(int eventId, string code)
		{
			using (SqlConnection conn = new SqlConnection(connStr))
			using (SqlCommand cmd = new SqlCommand(@"
                SELECT u.UserID AS VolunteerID, u.FullName, u.Email
                FROM VolunteerApplications va
                JOIN Users u ON va.UserID = u.UserID
                WHERE va.EventID = @EventID 
                  AND va.Status = 'Approved'
                  AND va.AttendanceCode = @Code
                  AND NOT EXISTS (
                        SELECT 1 FROM VolunteerAttendance a
                        WHERE a.VolunteerID = u.UserID AND a.EventID = va.EventID
                  )
                ORDER BY u.FullName", conn))
			{
				cmd.Parameters.Add("@EventID", SqlDbType.Int).Value = eventId;
				cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 50).Value = code;

				conn.Open();
				DataTable dt = new DataTable();
				dt.Load(cmd.ExecuteReader());

				gvVolunteers.Visible = dt.Rows.Count > 0;
				gvVolunteers.DataSource = dt;
				gvVolunteers.DataBind();

				if (dt.Rows.Count == 0)
					lblMessage.Text = "ℹ️ All volunteers with this code have been marked present.";
			}
		}

		private void LoadMarkedVolunteers(int eventId, string code)
		{
			using (SqlConnection conn = new SqlConnection(connStr))
			using (SqlCommand cmd = new SqlCommand(@"
                SELECT u.FullName, u.Email, a.MarkedAt
                FROM VolunteerAttendance a
                JOIN Users u ON a.VolunteerID = u.UserID
                JOIN VolunteerApplications va ON va.UserID = u.UserID AND va.EventID = a.EventID
                WHERE a.EventID = @EventID
                  AND va.AttendanceCode = @Code
                ORDER BY a.MarkedAt DESC", conn))
			{
				cmd.Parameters.Add("@EventID", SqlDbType.Int).Value = eventId;
				cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 50).Value = code;

				conn.Open();
				DataTable dt = new DataTable();
				dt.Load(cmd.ExecuteReader());

				gvMarkedVolunteers.DataSource = dt;
				gvMarkedVolunteers.DataBind();
			}
		}

		protected void gvVolunteers_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName != "Mark")
				return;

			int eventId = int.Parse(ddlEvents.SelectedValue);
			string code = txtCode.Text.Trim();
			int volunteerId = int.Parse(e.CommandArgument.ToString());

			using (SqlConnection conn = new SqlConnection(connStr))
			{
				conn.Open();

				// Prevent duplicate marking
				using (SqlCommand checkCmd = new SqlCommand(@"
                    SELECT COUNT(*) FROM VolunteerAttendance 
                    WHERE EventID = @EventID AND VolunteerID = @VolunteerID", conn))
				{
					checkCmd.Parameters.Add("@EventID", SqlDbType.Int).Value = eventId;
					checkCmd.Parameters.Add("@VolunteerID", SqlDbType.Int).Value = volunteerId;

					if ((int)checkCmd.ExecuteScalar() > 0)
					{
						lblMessage.Text = "⚠️ This volunteer is already marked present.";
						return;
					}
				}

				// Insert attendance
				using (SqlCommand insertCmd = new SqlCommand(@"
                    INSERT INTO VolunteerAttendance (EventID, VolunteerID, MarkedAt) 
                    VALUES (@EventID, @VolunteerID, @MarkedAt)", conn))
				{
					insertCmd.Parameters.Add("@EventID", SqlDbType.Int).Value = eventId;
					insertCmd.Parameters.Add("@VolunteerID", SqlDbType.Int).Value = volunteerId;
					insertCmd.Parameters.Add("@MarkedAt", SqlDbType.DateTime).Value = DateTime.Now;
					insertCmd.ExecuteNonQuery();
				}

				// Mark duties completed
				using (SqlCommand updateCmd = new SqlCommand(@"
                    UPDATE VolunteerDuties 
                    SET IsCompleted = 1 
                    WHERE EventID = @EventID AND VolunteerID = @VolunteerID", conn))
				{
					updateCmd.Parameters.Add("@EventID", SqlDbType.Int).Value = eventId;
					updateCmd.Parameters.Add("@VolunteerID", SqlDbType.Int).Value = volunteerId;
					updateCmd.ExecuteNonQuery();
				}
			}

			LoadVolunteers(eventId, code);
			LoadMarkedVolunteers(eventId, code);
		}
	}
}
