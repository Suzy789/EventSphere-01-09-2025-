using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Authentication.User
{
	public partial class AssignedDuties : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				if (Session["UserID"] == null)
				{
					Response.Redirect("~/Login.aspx");
					return;
				}
				LoadAssignedDuties();
			}
		}

		/// <summary>
		/// Load all assigned duties for the logged-in volunteer.
		/// </summary>
		private void LoadAssignedDuties()
		{
			try
			{
				if (Session["UserID"] == null)
				{
					lblMessage.Text = "⚠️ You must be logged in to view assigned duties.";
					lblMessage.CssClass = "text-danger";
					lblMessage.Visible = true;
					gvDuties.Visible = false;
					return;
				}

				int volunteerId = Convert.ToInt32(Session["UserID"]);
				string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

				using (SqlConnection conn = new SqlConnection(connStr))
				{
					conn.Open();
					string sql = @"
                        SELECT 
                            d.DutyID,
                            d.EventID,
                            d.RoleTitle,
                            d.Description,
                            d.IsCompleted,
                            e.Title AS EventTitle,
                            va.ApplicationID
                        FROM VolunteerDuties d
                        INNER JOIN Events e ON d.EventID = e.EventID
                        INNER JOIN VolunteerApplications va 
                            ON va.EventID = d.EventID AND va.UserID = d.VolunteerID
                        WHERE d.VolunteerID=@VID AND d.IsApprovedByOrganizer=1
                        ORDER BY d.DutyID DESC";

					using (SqlCommand cmd = new SqlCommand(sql, conn))
					{
						cmd.Parameters.AddWithValue("@VID", volunteerId);

						SqlDataAdapter da = new SqlDataAdapter(cmd);
						DataTable dt = new DataTable();
						da.Fill(dt);

						gvDuties.DataSource = dt;
						gvDuties.DataBind();

						if (dt.Rows.Count == 0)
						{
							lblMessage.Text = "✅ You have no assigned duties yet.";
							lblMessage.CssClass = "text-info";
							lblMessage.Visible = true;
						}
						else
						{
							lblMessage.Visible = false;
						}
					}
				}
			}
			catch (Exception ex)
			{
				lblMessage.Text = "❌ Error loading duties: " + ex.Message;
				lblMessage.CssClass = "text-danger";
				lblMessage.Visible = true;
			}
		}

		/// <summary>
		/// Handle DownloadID button click from the GridView.
		/// </summary>
		protected void gvDuties_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "DownloadID")
			{
				try
				{
					int appId = Convert.ToInt32(e.CommandArgument);
					// ✅ Redirect with ApplicationID
					Response.Redirect("~/User/DownloadID.aspx?appid=" + appId);
				}
				catch (Exception ex)
				{
					lblMessage.Text = "❌ Error preparing download: " + ex.Message;
					lblMessage.CssClass = "text-danger";
					lblMessage.Visible = true;
				}
			}
		}
	}
}