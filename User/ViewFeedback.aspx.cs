using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
namespace Authentication.User
{
    public partial class ViewFeedback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Organizer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadOrganizerEvents();
            }
        }

        private void LoadOrganizerEvents()
        {
            int organizerId = Convert.ToInt32(Session["UserID"]);
            string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT EventID, Title FROM Events WHERE OrganizerID = @OrganizerID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@OrganizerID", organizerId);

                conn.Open();
                ddlEvents.DataSource = cmd.ExecuteReader();
                ddlEvents.DataTextField = "Title";
                ddlEvents.DataValueField = "EventID";
                ddlEvents.DataBind();
                ddlEvents.Items.Insert(0, new ListItem("Select Event", ""));
            }
        }

        protected void ddlEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            rptFeedback.DataSource = null;
            rptFeedback.DataBind();
            lblMsg.Visible = false;

            if (ddlEvents.SelectedIndex <= 0)
                return;

            int eventId = Convert.ToInt32(ddlEvents.SelectedValue);
            LoadFeedback(eventId);
        }

        private void LoadFeedback(int eventId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                SELECT 
                    F.Message, 
                    F.SubmittedAt,
                    U.FullName AS UserFullName,
                    U.Role AS UserRole
                FROM Feedback F
                INNER JOIN Users U ON F.UserID = U.UserID
                WHERE F.EventID = @EventID
                ORDER BY F.SubmittedAt DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EventID", eventId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    rptFeedback.DataSource = reader;
                    rptFeedback.DataBind();
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "No feedback available for this event.";
                }
            }
        }
    }
}