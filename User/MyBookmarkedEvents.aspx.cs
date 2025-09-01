using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace Authentication.User
{
    public partial class MyBookmarkedEvents : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["Role"]?.ToString() != "Participant")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadBookmarks();
            }
        }

        private void LoadBookmarks()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT e.EventID, e.Title, e.Date, e.Location
                    FROM BookmarkedEvents b
                    INNER JOIN Events e ON b.EventID = e.EventID
                    WHERE b.UserID = @UserID
                    ORDER BY b.BookmarkedAt DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvBookmarks.DataSource = dt;
                gvBookmarks.DataBind();

                lblMessage.Text = dt.Rows.Count == 0 ? "No bookmarks yet." : "";
            }
        }

        protected void gvBookmarks_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoveBookmark")
            {
                int eventId = Convert.ToInt32(e.CommandArgument);
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "DELETE FROM BookmarkedEvents WHERE UserID = @UserID AND EventID = @EventID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);
                    cmd.Parameters.AddWithValue("@EventID", eventId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMessage.Text = "Bookmark removed successfully.";
                LoadBookmarks();
            }
        }
    }
}