using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Authentication.User
{
	public partial class EditVolunteerPreference : System.Web.UI.Page
	{
		private readonly string _connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["UserID"] == null)
			{
				Response.Redirect("~/Login.aspx");
				return;
			}

			if (!IsPostBack)
			{
				int applicationId = GetApplicationIdFromQuery();
				if (applicationId == 0) return;

				// Load EventID & CategoryID from application
				int categoryId, eventId;
				if (!GetApplicationDetails(applicationId, out categoryId, out eventId))
				{
					ShowMessage("Application not found.", "alert-danger");
					return;
				}

				LoadRoles(eventId, categoryId);
				LoadEventDetails(eventId, categoryId);
			}
		}

		private int GetApplicationIdFromQuery()
		{
			if (!int.TryParse(Request.QueryString["ApplicationID"], out int applicationId))
			{
				ShowMessage("Invalid or missing Application ID.", "alert-danger");
				return 0;
			}
			return applicationId;
		}

		private bool GetApplicationDetails(int applicationId, out int categoryId, out int eventId)
		{
			categoryId = 0;
			eventId = 0;

			using (SqlConnection con = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand("SELECT CategoryID, EventID FROM VolunteerApplications WHERE ApplicationID = @ApplicationID", con))
			{
				cmd.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = applicationId;
				con.Open();
				SqlDataReader dr = cmd.ExecuteReader();
				if (dr.Read())
				{
					categoryId = Convert.ToInt32(dr["CategoryID"]);
					eventId = Convert.ToInt32(dr["EventID"]);
					return true;
				}
			}
			return false;
		}

		private void LoadRoles(int eventId, int currentCategoryId)
		{
			ddlRoles.Items.Clear();

			using (SqlConnection con = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand(@"
                SELECT CategoryID, CategoryName, AllocatedVolunteers, RequiredVolunteers
                FROM VolunteerCategories
                WHERE EventID = @EventID
                ORDER BY CategoryName", con))
			{
				cmd.Parameters.Add("@EventID", SqlDbType.Int).Value = eventId;
				con.Open();
				SqlDataReader dr = cmd.ExecuteReader();
				while (dr.Read())
				{
					var item = new ListItem(dr["CategoryName"].ToString(), dr["CategoryID"].ToString());

					int allocated = Convert.ToInt32(dr["AllocatedVolunteers"]);
					int required = Convert.ToInt32(dr["RequiredVolunteers"]);
					bool isFull = allocated >= required && Convert.ToInt32(dr["CategoryID"]) != currentCategoryId;

					if (isFull)
					{
						item.Text += " (Full)";
						item.Attributes["disabled"] = "disabled";
					}

					ddlRoles.Items.Add(item);
				}
			}

			ddlRoles.SelectedValue = currentCategoryId.ToString();
		}

		private void LoadEventDetails(int eventId, int categoryId)
		{
			using (SqlConnection con = new SqlConnection(_connectionString))
			using (SqlCommand cmd = new SqlCommand(@"
                SELECT e.Title, c.CategoryName
                FROM Events e
                INNER JOIN VolunteerCategories c ON e.EventID = c.EventID
                WHERE c.CategoryID = @CategoryID", con))
			{
				cmd.Parameters.Add("@CategoryID", SqlDbType.Int).Value = categoryId;
				con.Open();
				SqlDataReader dr = cmd.ExecuteReader();
				if (dr.Read())
				{
					lblEventTitle.Text = $"{dr["Title"]} - Current Role: {dr["CategoryName"]}";
				}
			}
		}

		protected void btnSave_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(ddlRoles.SelectedValue))
			{
				ShowMessage("Please select a role.", "alert-warning");
				return;
			}

			int newCategoryId = int.Parse(ddlRoles.SelectedValue);
			int applicationId = GetApplicationIdFromQuery();
			if (applicationId == 0) return;

			int oldCategoryId, eventId;
			if (!GetApplicationDetails(applicationId, out oldCategoryId, out eventId)) return;

			if (oldCategoryId == newCategoryId)
			{
				ShowMessage("No changes detected.", "alert-info");
				return;
			}

			using (SqlConnection con = new SqlConnection(_connectionString))
			{
				con.Open();
				SqlTransaction transaction = con.BeginTransaction();

				try
				{
					// Decrease old category count
					using (SqlCommand cmd = new SqlCommand(@"
                        UPDATE VolunteerCategories
                        SET AllocatedVolunteers = AllocatedVolunteers - 1
                        WHERE CategoryID = @OldCategoryID AND AllocatedVolunteers > 0", con, transaction))
					{
						cmd.Parameters.Add("@OldCategoryID", SqlDbType.Int).Value = oldCategoryId;
						cmd.ExecuteNonQuery();
					}

					// Increase new category count
					int rowsAffected;
					using (SqlCommand cmd = new SqlCommand(@"
                        UPDATE VolunteerCategories
                        SET AllocatedVolunteers = AllocatedVolunteers + 1
                        WHERE CategoryID = @NewCategoryID
                        AND AllocatedVolunteers < RequiredVolunteers", con, transaction))
					{
						cmd.Parameters.Add("@NewCategoryID", SqlDbType.Int).Value = newCategoryId;
						rowsAffected = cmd.ExecuteNonQuery();
					}

					if (rowsAffected == 0)
					{
						transaction.Rollback();
						ShowMessage("Unable to update. The selected category is already full.", "alert-danger");
						return;
					}

					// Update application record and reset status to Pending
					using (SqlCommand cmd = new SqlCommand(@"
                        UPDATE VolunteerApplications
                        SET CategoryID = @NewCategoryID, Status = 'Pending'
                        WHERE ApplicationID = @ApplicationID", con, transaction))
					{
						cmd.Parameters.Add("@NewCategoryID", SqlDbType.Int).Value = newCategoryId;
						cmd.Parameters.Add("@ApplicationID", SqlDbType.Int).Value = applicationId;
						cmd.ExecuteNonQuery();
					}

					transaction.Commit();
					ShowMessage("Volunteer preference updated successfully! Your application is now pending approval.", "alert-success");
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					ShowMessage("An error occurred: " + ex.Message, "alert-danger");
				}
			}
		}

		private void ShowMessage(string message, string cssClass)
		{
			pnlMessage.CssClass = "alert " + cssClass;
			lblMessage.Text = message;
			pnlMessage.Visible = true;
		}
	}
}
