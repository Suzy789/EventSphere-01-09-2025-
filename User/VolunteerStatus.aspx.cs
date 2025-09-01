using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Authentication.User
{
	public partial class VolunteerStatus : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				if (Session["UserID"] != null)
				{
					LoadVolunteerStatus();
				}
				else
				{
					Response.Redirect("~/Login.aspx");
				}
			}
		}

		private void LoadVolunteerStatus()
		{
			int userId = Convert.ToInt32(Session["UserID"]);
			string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

			using (SqlConnection con = new SqlConnection(connStr))
			{
				string query = @"
                    SELECT 
                        E.Title,
                        VC.CategoryName,
                        VD.RoleTitle,
                        VA.Status
                    FROM VolunteerApplications VA
                    INNER JOIN VolunteerCategories VC ON VA.CategoryID = VC.CategoryID
                    INNER JOIN Events E ON VA.EventID = E.EventID
                    LEFT JOIN VolunteerDuties VD ON 
                        VA.UserID = VD.VolunteerID AND 
                        VA.CategoryID = VD.CategoryID AND 
                        VA.EventID = VD.EventID
                    WHERE VA.UserID = @UserID
                    ORDER BY VA.AppliedAt DESC";

				SqlDataAdapter da = new SqlDataAdapter(query, con);
				da.SelectCommand.Parameters.AddWithValue("@UserID", userId);

				DataTable dt = new DataTable();
				da.Fill(dt);

				// Bind to GridView
				gvApplications.DataSource = dt;
				gvApplications.DataBind();

				// Summary Counts
				int approved = dt.Select("Status = 'Approved'").Length;
				int pending = dt.Select("Status = 'Pending'").Length;
				int rejected = dt.Select("Status = 'Rejected'").Length;
				int cancelled = dt.Select("Status = 'Cancelled'").Length;

				lblApproved.Text = approved.ToString();
				lblPending.Text = pending.ToString();
				lblRejected.Text = rejected.ToString();
				lblCancelled.Text = cancelled.ToString();

				// Progress Bar
				int total = dt.Rows.Count;
				string progressHtml = "";

				if (total > 0)
				{
					int approvedPercent = (approved * 100) / total;
					int pendingPercent = (pending * 100) / total;
					int rejectedPercent = (rejected * 100) / total;
					int cancelledPercent = (cancelled * 100) / total;

					progressHtml = $@"
                        <div class='progress-bar bg-success' role='progressbar' style='width: {approvedPercent}%;' aria-valuenow='{approvedPercent}' aria-valuemin='0' aria-valuemax='100'>{approvedPercent}% Approved</div>
                        <div class='progress-bar bg-warning text-dark' role='progressbar' style='width: {pendingPercent}%;' aria-valuenow='{pendingPercent}' aria-valuemin='0' aria-valuemax='100'>{pendingPercent}% Pending</div>
                        <div class='progress-bar bg-danger' role='progressbar' style='width: {rejectedPercent}%;' aria-valuenow='{rejectedPercent}' aria-valuemin='0' aria-valuemax='100'>{rejectedPercent}% Rejected</div>
                        <div class='progress-bar bg-secondary' role='progressbar' style='width: {cancelledPercent}%;' aria-valuenow='{cancelledPercent}' aria-valuemin='0' aria-valuemax='100'>{cancelledPercent}% Cancelled</div>";
				}
				else
				{
					progressHtml = "<div class='progress-bar bg-secondary' role='progressbar' style='width: 100%;'>No Applications Yet</div>";
				}

				ltProgressBar.Text = progressHtml;
			}
		}
	}
}
