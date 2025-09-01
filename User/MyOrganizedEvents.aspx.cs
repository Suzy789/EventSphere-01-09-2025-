using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Authentication.User
{
	public partial class MyOrganizedEvent : System.Web.UI.Page
	{
		private readonly string _connStr = ConfigurationManager.ConnectionStrings["constr"]?.ConnectionString;

		protected void Page_Load(object sender, EventArgs e)
		{
			if (Session["UserID"] == null)
			{
				Response.Redirect("~/Login.aspx");
				return;
			}

			if (!IsPostBack)
			{
				//LoadBanner();
				LoadEvents();
			}
		}

		/*private void LoadBanner()
		{
			using (SqlConnection conn = new SqlConnection(_connStr))
			using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 EventBanner FROM Events WHERE OrganizerID=@OrganizerID", conn))
			{
				cmd.Parameters.AddWithValue("@OrganizerID", Session["UserID"]);
				conn.Open();
				object bannerPath = cmd.ExecuteScalar();

				imgPageBanner.ImageUrl = (bannerPath != null && bannerPath != DBNull.Value && !string.IsNullOrEmpty(bannerPath.ToString()))
					? bannerPath.ToString()
					: "~/images/default-banner.jpg";
			}
		}*/

		private void LoadEvents()
		{
			using (SqlConnection conn = new SqlConnection(_connStr))
			using (SqlCommand cmd = new SqlCommand(@"
                SELECT EventID, Title, Date, Location, Description, CreatedAt, EventBanner
                FROM Events 
                WHERE OrganizerID = @OrganizerID 
                ORDER BY Date ASC", conn))
			{
				cmd.Parameters.AddWithValue("@OrganizerID", Session["UserID"]);

				DataTable dt = new DataTable();
				using (SqlDataAdapter da = new SqlDataAdapter(cmd))
				{
					da.Fill(dt);
				}

				rptOrganizedEvents.DataSource = dt;
				rptOrganizedEvents.DataBind();
				lblNoEvents.Visible = dt.Rows.Count == 0;
			}
		}

		protected void rptOrganizedEvents_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			if (e.CommandName == "ShowCategoryForm")
			{
				Panel pnl = (Panel)e.Item.FindControl("pnlCategoryForm");
				if (pnl != null)
					pnl.Visible = true;
			}
			else if (e.CommandName == "SaveCategory")
			{
				TextBox txtCategoryName = (TextBox)e.Item.FindControl("txtCategoryName");
				TextBox txtRequiredCount = (TextBox)e.Item.FindControl("txtRequiredCount");

				if (!int.TryParse(txtRequiredCount.Text.Trim(), out int requiredCount))
					return;

				using (SqlConnection conn = new SqlConnection(_connStr))
				using (SqlCommand cmd = new SqlCommand(@"INSERT INTO VolunteerCategories 
                        (EventID, CategoryName, RequiredVolunteers) 
                        VALUES (@EventID, @CategoryName, @RequiredVolunteers)", conn))
				{
					cmd.Parameters.AddWithValue("@EventID", Convert.ToInt32(e.CommandArgument));
					cmd.Parameters.AddWithValue("@CategoryName", txtCategoryName.Text.Trim());
					cmd.Parameters.AddWithValue("@RequiredVolunteers", requiredCount);

					conn.Open();
					cmd.ExecuteNonQuery();
				}

				LoadEvents();
			}
		}
	}
}
