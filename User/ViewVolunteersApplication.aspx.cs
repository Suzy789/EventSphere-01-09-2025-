using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Authentication.User
{
	public partial class ViewVolunteerApplication : System.Web.UI.Page
	{
		private readonly string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

		// Keep EventID across postbacks
		private int EventId
		{
			get { return ViewState["EventID"] != null ? Convert.ToInt32(ViewState["EventID"]) : 0; }
			set { ViewState["EventID"] = value; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["UserID"] == null)
			{
				// Redirect to login page if not logged in
				Response.Redirect("~/Login.aspx");
				return;
			}
			if (!IsPostBack)
			{
				LoadEventsDropdown();
			}
		}

		// Load all events into dropdown
		private void LoadEventsDropdown()
		{
			using (SqlConnection con = new SqlConnection(connectionString))
			using (SqlCommand cmd = new SqlCommand("SELECT EventID, Title FROM Events ORDER BY Title", con))
			{
				con.Open();
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					ddlEvents.DataSource = reader;
					ddlEvents.DataTextField = "Title";
					ddlEvents.DataValueField = "EventID";
					ddlEvents.DataBind();
				}
			}

			ddlEvents.Items.Insert(0, new ListItem("-- Select Event --", "0"));
		}

		// When dropdown changes
		protected void ddlEvents_SelectedIndexChanged(object sender, EventArgs e)
		{
			EventId = Convert.ToInt32(ddlEvents.SelectedValue);

			if (EventId > 0)
			{
				LoadEventTitle();
				LoadCategories();
			}
			else
			{
				lblEventTitle.InnerText = "";
				rptCategories.DataSource = null;
				rptCategories.DataBind();
				lblNoVolunteers.Visible = false;
			}
		}

		private void LoadEventTitle()
		{
			using (SqlConnection con = new SqlConnection(connectionString))
			using (SqlCommand cmd = new SqlCommand("SELECT Title FROM Events WHERE EventID = @EventID", con))
			{
				cmd.Parameters.AddWithValue("@EventID", EventId);
				con.Open();
				object result = cmd.ExecuteScalar();
				lblEventTitle.InnerText = result != null ? "Event: " + result.ToString() : "Event not found";
			}
		}

		private void LoadCategories()
		{
			using (SqlConnection con = new SqlConnection(connectionString))
			using (SqlCommand cmd = new SqlCommand(@"
                SELECT CategoryID, CategoryName, RequiredVolunteers, AllocatedVolunteers
                FROM VolunteerCategories
                WHERE EventID = @EventID", con))
			{
				cmd.Parameters.AddWithValue("@EventID", EventId);

				DataTable categoryTable = new DataTable();
				new SqlDataAdapter(cmd).Fill(categoryTable);

				if (categoryTable.Rows.Count > 0)
				{
					rptCategories.DataSource = categoryTable;
					rptCategories.DataBind();
					lblNoVolunteers.Visible = false;
				}
				else
				{
					ShowWarning("No volunteer categories found for this event.");
					rptCategories.DataSource = null;
					rptCategories.DataBind();
				}
			}
		}

		protected void rptCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				DataRowView categoryRow = (DataRowView)e.Item.DataItem;
				int categoryId = Convert.ToInt32(categoryRow["CategoryID"]);
				Repeater rptVolunteers = (Repeater)e.Item.FindControl("rptVolunteers");

				using (SqlConnection con = new SqlConnection(connectionString))
				using (SqlCommand cmd = new SqlCommand(@"
                    SELECT va.ApplicationID, u.FullName, u.Email, va.Status
                    FROM VolunteerApplications va
                    INNER JOIN Users u ON va.UserID = u.UserID
                    WHERE va.EventID = @EventID AND va.CategoryID = @CategoryID", con))
				{
					cmd.Parameters.AddWithValue("@EventID", EventId);
					cmd.Parameters.AddWithValue("@CategoryID", categoryId);

					DataTable volunteerTable = new DataTable();
					new SqlDataAdapter(cmd).Fill(volunteerTable);

					rptVolunteers.DataSource = volunteerTable;
					rptVolunteers.DataBind();
				}
			}
		}

		protected void rptVolunteers_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			int applicationId = Convert.ToInt32(e.CommandArgument);

			using (SqlConnection con = new SqlConnection(connectionString))
			{
				con.Open();
				int categoryId = GetCategoryIdForApplication(con, applicationId);
				if (categoryId == 0) { ShowError("Category not found."); return; }

				(int requiredSlots, int allocatedSlots) = GetSlotInfo(con, categoryId);

				if (e.CommandName == "Approve")
				{
					if (allocatedSlots >= requiredSlots)
					{
						ShowError("No more slots available for this category.");
						return;
					}

					int volunteerId = GetVolunteerIdForApplication(con, applicationId);
					if (!EventExists(con, EventId)) { ShowError("Event not found."); return; }
					if (!VolunteerExists(con, volunteerId)) { ShowError("Volunteer not found."); return; }
					if (!CategoryExists(con, categoryId)) { ShowError("Category not found."); return; }

					UpdateApplicationStatus(con, applicationId, "Approved");
					IncrementAllocatedSlots(con, categoryId);
					InsertVolunteerDuty(con, EventId, volunteerId, categoryId, "General Volunteer", "Assigned automatically upon approval");
					ShowSuccess("Volunteer approved.");
				}
				else if (e.CommandName == "Reject")
				{
					UpdateApplicationStatus(con, applicationId, "Rejected");
					ShowError("Volunteer rejected.");
				}
				else if (e.CommandName == "ViewDetails")
				{
					Response.Redirect($"VolunteerDetails.aspx?appid={applicationId}");
					return;
				}
			}

			LoadCategories();
		}

		// ==== Helper Methods ====
		private bool EventExists(SqlConnection con, int eventId) =>
			ExecuteCountQuery(con, "SELECT COUNT(*) FROM Events WHERE EventID = @EventID", "@EventID", eventId) > 0;

		private bool VolunteerExists(SqlConnection con, int volunteerId) =>
			ExecuteCountQuery(con, "SELECT COUNT(*) FROM Users WHERE UserID = @UserID", "@UserID", volunteerId) > 0;

		private bool CategoryExists(SqlConnection con, int categoryId) =>
			ExecuteCountQuery(con, "SELECT COUNT(*) FROM VolunteerCategories WHERE CategoryID = @CategoryID", "@CategoryID", categoryId) > 0;

		private int ExecuteCountQuery(SqlConnection con, string sql, string paramName, int paramValue)
		{
			using (SqlCommand cmd = new SqlCommand(sql, con))
			{
				cmd.Parameters.AddWithValue(paramName, paramValue);
				return (int)cmd.ExecuteScalar();
			}
		}

		private int GetVolunteerIdForApplication(SqlConnection con, int applicationId)
		{
			using (SqlCommand cmd = new SqlCommand("SELECT UserID FROM VolunteerApplications WHERE ApplicationID = @AppID", con))
			{
				cmd.Parameters.AddWithValue("@AppID", applicationId);
				return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
			}
		}

		private int GetCategoryIdForApplication(SqlConnection con, int applicationId)
		{
			using (SqlCommand cmd = new SqlCommand("SELECT CategoryID FROM VolunteerApplications WHERE ApplicationID = @AppID", con))
			{
				cmd.Parameters.AddWithValue("@AppID", applicationId);
				return Convert.ToInt32(cmd.ExecuteScalar() ?? 0);
			}
		}

		private (int required, int allocated) GetSlotInfo(SqlConnection con, int categoryId)
		{
			using (SqlCommand cmd = new SqlCommand("SELECT RequiredVolunteers, AllocatedVolunteers FROM VolunteerCategories WHERE CategoryID = @CategoryID", con))
			{
				cmd.Parameters.AddWithValue("@CategoryID", categoryId);
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						return (
							Convert.ToInt32(reader["RequiredVolunteers"]),
							Convert.ToInt32(reader["AllocatedVolunteers"])
						);
					}
				}
			}
			return (0, 0);
		}

		private void UpdateApplicationStatus(SqlConnection con, int applicationId, string status)
		{
			using (SqlCommand cmd = new SqlCommand("UPDATE VolunteerApplications SET Status = @Status WHERE ApplicationID = @AppID", con))
			{
				cmd.Parameters.AddWithValue("@Status", status);
				cmd.Parameters.AddWithValue("@AppID", applicationId);
				cmd.ExecuteNonQuery();
			}
		}

		private void IncrementAllocatedSlots(SqlConnection con, int categoryId)
		{
			using (SqlCommand cmd = new SqlCommand("UPDATE VolunteerCategories SET AllocatedVolunteers = AllocatedVolunteers + 1 WHERE CategoryID = @CategoryID", con))
			{
				cmd.Parameters.AddWithValue("@CategoryID", categoryId);
				cmd.ExecuteNonQuery();
			}
		}

		private void InsertVolunteerDuty(SqlConnection con, int eventId, int volunteerId, int categoryId, string roleTitle, string description)
		{
            using (SqlCommand cmd = new SqlCommand(@"
        INSERT INTO VolunteerDuties 
            (EventID, VolunteerID, CategoryID, RoleTitle, Description, IsApprovedByOrganizer, IsCompleted)
        VALUES 
            (@EventID, @VolunteerID, @CategoryID, @RoleTitle, @Description, 1, 0)", con))
            {
                cmd.Parameters.AddWithValue("@EventID", eventId);
                cmd.Parameters.AddWithValue("@VolunteerID", volunteerId);
                cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                cmd.Parameters.AddWithValue("@RoleTitle", roleTitle);
                cmd.Parameters.AddWithValue("@Description", description ?? (object)DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

		// ==== UI Message Methods ====
		private void ShowSuccess(string message)
		{
			lblMessage.CssClass = "alert alert-success";
			lblMessage.Text = message;
			lblMessage.Visible = true;
		}

		private void ShowError(string message)
		{
			lblMessage.CssClass = "alert alert-danger";
			lblMessage.Text = message;
			lblMessage.Visible = true;
		}

		private void ShowWarning(string message)
		{
			lblNoVolunteers.Text = message;
			lblNoVolunteers.Visible = true;
		}
	}
}
