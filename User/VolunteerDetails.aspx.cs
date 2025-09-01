using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace Authentication.User
{
	public partial class VolunteerDetails : System.Web.UI.Page
	{
		private readonly string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

		protected void Page_Load(object sender, EventArgs e)
		{
			// Ensure user is logged in
			if (Session["UserID"] == null)
			{
				Response.Redirect("~/Login.aspx");
				return;
			}

			if (!IsPostBack)
			{
				// Get ApplicationID from query string
				if (Request.QueryString["appid"] != null &&
					int.TryParse(Request.QueryString["appid"], out int applicationId))
				{
					LoadVolunteerDetails(applicationId);
				}
				else
				{
					Response.Redirect("ManageVolunteeringEvents.aspx");
				}
			}
		}

		private void LoadVolunteerDetails(int applicationId)
		{
			string query = @"
                SELECT 
                    u.FullName AS VolunteerName,
                    va.PhotoPath,
                    e.Title AS EventTitle,
                    c.CategoryName,
                    va.Status,
                    e.Date,
                    e.StartTime,
                    e.EndTime,
                    e.Location
                FROM VolunteerApplications va
                INNER JOIN Users u ON va.UserID = u.UserID
                INNER JOIN Events e ON va.EventID = e.EventID
                INNER JOIN VolunteerCategories c ON va.CategoryID = c.CategoryID
                WHERE va.ApplicationID = @ApplicationID";

			using (SqlConnection conn = new SqlConnection(connectionString))
			using (SqlCommand cmd = new SqlCommand(query, conn))
			{
				cmd.Parameters.AddWithValue("@ApplicationID", applicationId);
				conn.Open();

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					if (reader.Read())
					{
						// Volunteer and event details
						lblVolunteerName.Text = reader["VolunteerName"].ToString();
						lblEventName.Text = reader["EventTitle"].ToString();
						lblCategory.Text = reader["CategoryName"].ToString();
						lblStatus.Text = reader["Status"].ToString();

						// Date formatting
						DateTime eventDate = Convert.ToDateTime(reader["Date"]);
						lblStartDate.Text = $"{eventDate:dd MMM yyyy} {reader["StartTime"]}";
						lblEndDate.Text = $"{eventDate:dd MMM yyyy} {reader["EndTime"]}";

						lblLocation.Text = reader["Location"].ToString();

						// Handle image
						string photoPath = reader["PhotoPath"] != DBNull.Value
							? reader["PhotoPath"].ToString()
							: "";
						imgVolunteer.ImageUrl = GetImageUrl(photoPath);

						// Status color
						if (lblStatus.Text.Equals("Approved", StringComparison.OrdinalIgnoreCase))
							lblStatus.CssClass += " text-success";
						else if (lblStatus.Text.Equals("Pending", StringComparison.OrdinalIgnoreCase))
							lblStatus.CssClass += " text-warning";
						else
							lblStatus.CssClass += " text-danger";
					}
					else
					{
						Response.Redirect("ManageVolunteeringEvents.aspx");
					}
				}
			}
		}

		/// <summary>
		/// Returns a valid browser URL for the volunteer photo or a default image if missing.
		/// </summary>
		private string GetImageUrl(string dbPath)
		{
			// Default if empty
			if (string.IsNullOrWhiteSpace(dbPath))
				return ResolveUrl("~/images/default-user.png");

			// Only keep the file name
			string fileName = Path.GetFileName(dbPath);

			// Build the expected virtual path
			string virtualPath = "~/Uploads/" + fileName;
			string physicalPath = Server.MapPath(virtualPath);

			// If file doesn't exist, use default
			if (!File.Exists(physicalPath))
				return ResolveUrl("~/images/default-user.png");

			// Return relative URL
			return ResolveUrl(virtualPath);
		}
	}
}
