using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Authentication.User
{
	public partial class MyVolunteerEvents : System.Web.UI.Page
	{
		private readonly string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				if (Session["UserID"] == null)
				{
					Response.Redirect("~/Login.aspx");
					return;
				}

				LoadVolunteerEvents();
			}
		}

		private void LoadVolunteerEvents()
		{
			string userId = Session["UserID"].ToString();

			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				string query = @"
                    SELECT 
                        e.EventID,
                        e.Title,
                        e.Date,
                        e.Location,
                        va.Status,
                        va.UserID AS VolunteerID,
                        vc.CategoryName,
                        ISNULL(vd.IsCompleted, 0) AS IsCompleted
                    FROM VolunteerApplications va
                    INNER JOIN Events e ON va.EventID = e.EventID
                    LEFT JOIN VolunteerDuties vd ON va.EventID = vd.EventID AND va.UserID = vd.VolunteerID
                    LEFT JOIN VolunteerCategories vc ON va.CategoryID = vc.CategoryID
                    WHERE va.UserID = @UserID";

				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@UserID", userId);

					DataTable dt = new DataTable();
					using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
					{
						adapter.Fill(dt);
					}

					rptMyEvents.DataSource = dt;
					rptMyEvents.DataBind();
					lblNoEvents.Visible = (dt.Rows.Count == 0);
				}
			}
		}

		protected void rptMyEvents_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "DownloadCertificate":
					HandleCertificateDownload(e.CommandArgument.ToString());
					break;

				case "GiveFeedback":
					HandleFeedbackRedirect(e.CommandArgument.ToString());
					break;
			}
		}

		private void HandleCertificateDownload(string argument)
		{
			// Split the command argument into volunteerId and eventId
			string[] args = argument.Split('|');

			if (args.Length == 2)
			{
				int volunteerId;
				int eventId;

				if (int.TryParse(args[0], out volunteerId) && int.TryParse(args[1], out eventId))
				{
					using (SqlConnection conn = new SqlConnection(connectionString))
					{
						conn.Open();

						// Step 1: Get the role (CategoryName) for this volunteer
						string role = "Volunteer"; // default
						string getRoleQuery = "SELECT vc.CategoryName " +
											  "FROM VolunteerApplications va " +
											  "INNER JOIN VolunteerCategories vc ON va.CategoryID = vc.CategoryID " +
											  "WHERE va.UserID = @UserID AND va.EventID = @EventID";

						using (SqlCommand cmdRole = new SqlCommand(getRoleQuery, conn))
						{
							cmdRole.Parameters.AddWithValue("@UserID", volunteerId);
							cmdRole.Parameters.AddWithValue("@EventID", eventId);

							object result = cmdRole.ExecuteScalar();
							if (result != null)
							{
								role = result.ToString();
							}
						}

						// Step 2: Check if certificate already exists
						string checkCertQuery = "SELECT COUNT(*) FROM Certificates WHERE UserID = @UserID AND EventID = @EventID";
						bool certExists = false;

						using (SqlCommand cmdCheck = new SqlCommand(checkCertQuery, conn))
						{
							cmdCheck.Parameters.AddWithValue("@UserID", volunteerId);
							cmdCheck.Parameters.AddWithValue("@EventID", eventId);

							int count = Convert.ToInt32(cmdCheck.ExecuteScalar());
							if (count > 0)
							{
								certExists = true;
							}
						}

						// Step 3: If not exists, insert into Certificates table
						if (!certExists)
						{
							string insertCertQuery = "INSERT INTO Certificates (UserID, EventID, Role) " +
													 "VALUES (@UserID, @EventID, @Role)";

							using (SqlCommand cmdInsert = new SqlCommand(insertCertQuery, conn))
							{
								cmdInsert.Parameters.AddWithValue("@UserID", volunteerId);
								cmdInsert.Parameters.AddWithValue("@EventID", eventId);
								cmdInsert.Parameters.AddWithValue("@Role", role);
								cmdInsert.ExecuteNonQuery();
							}
						}
					}

					// Step 4: Redirect to the certificate page
					Response.Redirect($"~/User/Certificate.aspx?volunteerId={volunteerId}&eventId={eventId}");
				}
			}
		}


		private void HandleFeedbackRedirect(string argument)
		{
			if (int.TryParse(argument, out int eventId))
			{
				Response.Redirect($"~/User/Feedback.aspx?eventId={eventId}");
			}
		}
	}
}
