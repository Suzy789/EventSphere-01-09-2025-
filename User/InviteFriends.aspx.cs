using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
namespace Authentication.User
{
    public partial class InviteFriends : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                // Redirect to login or show an error
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack)
            {
                LoadUserEvents();
            }
        }

        private void LoadUserEvents()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT e.EventID, e.Title AS EventTitle, e.Date AS EventDate
FROM ParticipantRegistrations pr
INNER JOIN Events e ON pr.EventID = e.EventID
WHERE pr.UserID = @UserID AND pr.Status = 'Confirmed'
ORDER BY e.Date DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Add EventURL column
                dt.Columns.Add("EventURL", typeof(string));
                foreach (DataRow row in dt.Rows)
                {
                    string eventId = row["EventID"].ToString();
                    string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    row["EventURL"] = baseUrl + "/User/EventDetails.aspx?eventid=" + eventId;
                }

                gvUserEvents.DataSource = dt;
                gvUserEvents.DataBind();
            }
        }
    }

}