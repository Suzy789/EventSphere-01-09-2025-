using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace Authentication.User
{
    public partial class CheckAttendance : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
                Response.Redirect("~/Login.aspx");

            if (!IsPostBack)
                LoadAttendance();
        }

        private void LoadAttendance()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT e.EventID, e.Title, e.Date,
                       CASE 
                           WHEN a.AttendanceID IS NOT NULL THEN 'Present'
                           WHEN va.Status='Approved' THEN 'Absent'
                           ELSE 'Pending'
                       END AS AttendanceStatus,
                       a.MarkedAt
                FROM VolunteerApplications va
                JOIN Events e ON va.EventID = e.EventID
                LEFT JOIN VolunteerAttendance a ON a.EventID = e.EventID AND a.VolunteerID = va.UserID
                WHERE va.UserID = @UserID
                ORDER BY e.Date DESC", conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                conn.Open();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                rptAttendance.DataSource = dt;
                rptAttendance.DataBind();
            }

            pnlReportIssue.Visible = false;
            lblMessage.Text = "";
        }

        protected void btnReportIssue_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int eventId = Convert.ToInt32(btn.CommandArgument);

            hfEventID.Value = eventId.ToString();
            pnlReportIssue.Visible = true;
            lblIssueMessage.Text = "";
            txtIssue.Text = "";
        }

        protected void btnCancelIssue_Click(object sender, EventArgs e)
        {
            pnlReportIssue.Visible = false;
        }

        protected void btnSubmitIssue_Click(object sender, EventArgs e)
        {
            int eventId = Convert.ToInt32(hfEventID.Value);
            int userId = Convert.ToInt32(Session["UserID"]);
            string issueText = txtIssue.Text.Trim();

            if (string.IsNullOrEmpty(issueText))
            {
                lblIssueMessage.Text = "⚠️ Please describe your issue.";
                return;
            }

            try
            {
                // Insert into separate issues table
                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO VolunteerAttendanceIssues (EventID, VolunteerID, IssueDescription)
                    VALUES (@EventID, @VolunteerID, @Issue)", conn))
                {
                    cmd.Parameters.AddWithValue("@EventID", eventId);
                    cmd.Parameters.AddWithValue("@VolunteerID", userId);
                    cmd.Parameters.AddWithValue("@Issue", issueText);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblIssueMessage.Text = "✅ Your issue has been submitted. The organizer will review it.";
                pnlReportIssue.Visible = false;

                LoadAttendance();
            }
            catch (SqlException ex)
            {
                lblIssueMessage.Text = "❌ Error submitting issue: " + ex.Message;
            }
        }
    }
}

