using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Authentication.User
{
	public partial class ManageVolunteering : System.Web.UI.Page
	{
		private readonly string _connectionString = ConfigurationManager.ConnectionStrings["constr"]?.ConnectionString;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["UserID"] == null)
			{
				Response.Redirect("~/Login.aspx");
				return;
			}

			if (!IsPostBack)
			{
				LoadVolunteeringEvents();
			}
		}

		private void LoadVolunteeringEvents()
		{
			if (string.IsNullOrWhiteSpace(_connectionString))
				throw new Exception("Connection string 'constr' is not defined in Web.config.");

			int userId = Convert.ToInt32(Session["UserID"]);

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				string query = @"
                    SELECT 
                        VA.ApplicationID,    
                        E.EventID, 
                        E.Title, 
                        E.Date,
                        E.Location, 
                        VC.CategoryName AS RoleTitle,
                        VA.Status,
                        DATEDIFF(DAY, CAST(GETDATE() AS DATE), E.Date) AS DaysRemaining
                    FROM VolunteerApplications VA
                    INNER JOIN Events E ON VA.EventID = E.EventID
                    INNER JOIN VolunteerCategories VC 
                        ON VC.EventID = VA.EventID AND VC.CategoryID = VA.CategoryID
                    WHERE 
                        VA.UserID = @UserID 
                    ORDER BY E.Date ASC";

				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@UserID", userId);

					using (SqlDataAdapter da = new SqlDataAdapter(cmd))
					{
						DataTable dt = new DataTable();
						da.Fill(dt);

						rptVolunteeringEvents.DataSource = dt;
						rptVolunteeringEvents.DataBind();

						pnlNoEvents.Visible = dt.Rows.Count == 0;

						// Show modal if any event is happening today
						foreach (DataRow row in dt.Rows)
						{
							if (Convert.ToInt32(row["DaysRemaining"]) == 0)
							{
								litModalScript.Text = @"<script>
                                    var myModal = new bootstrap.Modal(document.getElementById('eventDayModal'));
                                    window.onload = function() { myModal.show(); };
                                </script>";
								break;
							}
						}
					}
				}
			}
		}

		protected void rptVolunteeringEvents_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName == "CancelRegistration")
			{
				CancelRegistration(Convert.ToInt32(e.CommandArgument));
			}
		}

		private void CancelRegistration(int eventId)
		{
			if (string.IsNullOrWhiteSpace(_connectionString))
				throw new Exception("Connection string 'constr' is not defined in Web.config.");

			int userId = Convert.ToInt32(Session["UserID"]);
			int categoryId = 0;

			using (SqlConnection conn = new SqlConnection(_connectionString))
			{
				conn.Open();

				// 1️⃣ Get CategoryID for this application
				string getCategoryQuery = @"
            SELECT CategoryID 
            FROM VolunteerApplications
            WHERE UserID = @UserID 
              AND EventID = @EventID
              AND Status IN ('Approved', 'Pending')";

				using (SqlCommand cmdGetCat = new SqlCommand(getCategoryQuery, conn))
				{
					cmdGetCat.Parameters.AddWithValue("@UserID", userId);
					cmdGetCat.Parameters.AddWithValue("@EventID", eventId);

					object result = cmdGetCat.ExecuteScalar();
					if (result != null)
						categoryId = Convert.ToInt32(result);
				}

				// 2️⃣ Cancel the application
				string cancelQuery = @"
            UPDATE VolunteerApplications
            SET Status = 'Cancelled'
            WHERE UserID = @UserID
              AND EventID = @EventID
              AND Status IN ('Approved', 'Pending')";

				using (SqlCommand cmdCancel = new SqlCommand(cancelQuery, conn))
				{
					cmdCancel.Parameters.AddWithValue("@UserID", userId);
					cmdCancel.Parameters.AddWithValue("@EventID", eventId);
					cmdCancel.ExecuteNonQuery();
				}

				// 3️⃣ Update VolunteerCategories allocated count
				if (categoryId > 0)
				{
					string updateCategoryQuery = @"
                UPDATE VolunteerCategories
                SET AllocatedVolunteers = AllocatedVolunteers - 1
                WHERE CategoryID = @CategoryID AND AllocatedVolunteers > 0";

					using (SqlCommand cmdUpdateCat = new SqlCommand(updateCategoryQuery, conn))
					{
						cmdUpdateCat.Parameters.AddWithValue("@CategoryID", categoryId);
						cmdUpdateCat.ExecuteNonQuery();
					}
				}

				// 4️⃣ Remove assigned duties
				string deleteDutyQuery = @"
            DELETE FROM VolunteerDuties
            WHERE VolunteerID = @UserID AND EventID = @EventID";

				using (SqlCommand cmdDeleteDuty = new SqlCommand(deleteDutyQuery, conn))
				{
					cmdDeleteDuty.Parameters.AddWithValue("@UserID", userId);
					cmdDeleteDuty.Parameters.AddWithValue("@EventID", eventId);
					cmdDeleteDuty.ExecuteNonQuery();
				}
			}

			// Reload updated events
			LoadVolunteeringEvents();
		}
	}
	}
