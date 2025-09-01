using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Authentication.User
{
	public partial class ViewVolunteer : System.Web.UI.Page
	{
		private readonly string _connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

		// Store EventID in ViewState
		private int EventId
		{
			get => ViewState["EventID"] != null ? (int)ViewState["EventID"] : 0;
			set => ViewState["EventID"] = value;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				if (int.TryParse(Request.QueryString["eventid"], out int eid))
				{
					EventId = eid;
					LoadEventTitle();
					LoadCategories();
				}
				else
				{
					ShowWarning("Invalid Event ID.");
				}
			}
		}

		private void LoadEventTitle()
		{
			using (SqlConnection con = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("SELECT Title FROM Events WHERE EventID = @EventID", con))
			{
				cmd.Parameters.AddWithValue("@EventID", EventId);
				con.Open();
				object result = cmd.ExecuteScalar();
				lblEventTitle.InnerText = result != null ? "Event: " + result : "Event not found";
			}
		}

		private void LoadCategories()
		{
			using (SqlConnection con = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand(@"
                SELECT CategoryID, CategoryName, RequiredVolunteers, AllocatedVolunteers
                FROM VolunteerCategories
                WHERE EventID = @EventID", con))
			{
				cmd.Parameters.AddWithValue("@EventID", EventId);
				DataTable categoriesTable = new DataTable();
				new SqlDataAdapter(cmd).Fill(categoriesTable);

				if (categoriesTable.Rows.Count > 0)
				{
					rptCategories.DataSource = categoriesTable;
					rptCategories.DataBind();
				}
				else
				{
					ShowWarning("No volunteer categories found for this event.");
				}
			}
		}

		protected void rptCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				DataRowView row = (DataRowView)e.Item.DataItem;
				int categoryId = Convert.ToInt32(row["CategoryID"]);

				Repeater rptVolunteers = (Repeater)e.Item.FindControl("rptVolunteers");

				using (SqlConnection con = new SqlConnection(_connectionString))
				using (SqlCommand cmd = new SqlCommand(@"
                    SELECT va.ApplicationID, u.FullName, u.Email, va.Status
                    FROM VolunteerApplications va
                    INNER JOIN Users u ON va.UserID = u.UserID
                    WHERE va.EventID = @EventID AND va.CategoryID = @CategoryID", con))
				{
					cmd.Parameters.AddWithValue("@EventID", EventId);
					cmd.Parameters.AddWithValue("@CategoryID", categoryId);

					DataTable volunteersTable = new DataTable();
					new SqlDataAdapter(cmd).Fill(volunteersTable);

					rptVolunteers.DataSource = volunteersTable;
					rptVolunteers.DataBind();
				}
			}
		}

		protected void rptVolunteers_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			int applicationId = Convert.ToInt32(e.CommandArgument);

			using (SqlConnection con = new SqlConnection(_connectionString))
			{
				con.Open();

				int categoryId = GetCategoryIdForApplication(con, applicationId);
				if (categoryId == 0)
				{
					ShowError("❌ Category not found.");
					return;
				}

				(int requiredSlots, int allocatedSlots) = GetSlotInfo(con, categoryId);

				switch (e.CommandName)
				{
					case "Approve":
						if (allocatedSlots >= requiredSlots)
						{
							ShowError("⚠️ No more slots available for this category.");
							return;
						}

						int volunteerId = GetVolunteerIdForApplication(con, applicationId);

						UpdateApplicationStatus(con, applicationId, "Approved");
						IncrementAllocatedSlots(con, categoryId);
						InsertVolunteerDuty(con, EventId, volunteerId, categoryId, "General Volunteer");
						ShowSuccess("✅ Volunteer approved and duty assigned.");
						break;

					case "Reject":
						UpdateApplicationStatus(con, applicationId, "Rejected");
						ShowError("❌ Volunteer rejected.");
						break;

					case "ViewDetails":
						Response.Redirect($"VolunteerDetails.aspx?appid={applicationId}");
						return;
				}
			}

			LoadCategories(); // Refresh UI
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
						return (Convert.ToInt32(reader["RequiredVolunteers"]), Convert.ToInt32(reader["AllocatedVolunteers"]));
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

		// ✅ Updated to accept role & description
		/// <summary>
		/// Inserts a volunteer duty record when a volunteer is approved.
		/// RoleTitle is fetched from VolunteerCategories; falls back to defaultRoleTitle if not found.
		/// Sets IsApprovedByOrganizer = 1 and IsCompleted = 0.
		/// </summary>
		private void InsertVolunteerDuty(SqlConnection con, int eventId, int volunteerId, int categoryId, string defaultRoleTitle)
		{
			if (con == null) throw new ArgumentNullException(nameof(con));

			// 1️⃣ Get RoleTitle from VolunteerCategories (CategoryName used as RoleTitle)
			string roleTitle;
			using (SqlCommand cmdRole = new SqlCommand(
				"SELECT CategoryName FROM VolunteerCategories WHERE CategoryID = @CatID", con))
			{
				cmdRole.Parameters.Add("@CatID", SqlDbType.Int).Value = categoryId;
				object result = cmdRole.ExecuteScalar();
				roleTitle = result != null ? result.ToString() : defaultRoleTitle;
			}

			// 2️⃣ Description - Professional & clear
			string description = "Assigned automatically upon approval by organizer";

			// 3️⃣ Insert Duty
			using (SqlCommand cmdInsert = new SqlCommand(@"
        INSERT INTO VolunteerDuties
            (EventID, VolunteerID, CategoryID, RoleTitle, Description, IsApprovedByOrganizer, IsCompleted)
        VALUES
            (@EventID, @VolunteerID, @CategoryID, @RoleTitle, @Description, 1, 0);", con))
			{
				cmdInsert.Parameters.Add("@EventID", SqlDbType.Int).Value = eventId;
				cmdInsert.Parameters.Add("@VolunteerID", SqlDbType.Int).Value = volunteerId;
				cmdInsert.Parameters.Add("@CategoryID", SqlDbType.Int).Value = categoryId;
				cmdInsert.Parameters.Add("@RoleTitle", SqlDbType.NVarChar, 200).Value = roleTitle;
				cmdInsert.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = description;

				cmdInsert.ExecuteNonQuery();
			}
		}

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
