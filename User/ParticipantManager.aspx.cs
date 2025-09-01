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
    public partial class ParticipantManager : System.Web.UI.Page
    {
        string connstr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }
                LoadEvents();
            }
        }
        protected void chkPresent_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            GridViewRow row = (GridViewRow)chk.NamingContainer;

            int registrationId = Convert.ToInt32(gvParticipants.DataKeys[row.RowIndex].Value);

            using (SqlConnection con = new SqlConnection(connstr))
            {
                string query = "UPDATE ParticipantRegistrations SET IsPresent=@IsPresent WHERE RegistrationID=@RegistrationID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IsPresent", 1);            // mark only once
                    cmd.Parameters.AddWithValue("@RegistrationID", registrationId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            chk.Enabled = false;  // <— prevent further toggles

            // reload the grid so certificate button gets enabled
            int eventId = Convert.ToInt32(ddlEvents.SelectedValue);
            LoadParticipants(eventId);

            lblMessage.Text = "✅ Attendance marked";
            lblMessage.CssClass = "alert alert-success";
            lblMessage.Visible = true;
        }

        private bool HasRegistrationDeskVolunteer(int eventId)
        {
            using (SqlConnection con = new SqlConnection(connstr))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT COUNT(*)
        FROM VolunteerApplications va
        INNER JOIN VolunteerCategories vc ON va.CategoryID = vc.CategoryID
        WHERE va.EventID = @EventID AND vc.CategoryName = 'Registration Desk' AND va.Status='Approved'", con))
            {
                cmd.Parameters.AddWithValue("@EventID", eventId);
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private void LoadEvents()
        {
            int organizerId = Convert.ToInt32(Session["UserID"]);
            string query = "SELECT EventID, Title FROM Events WHERE OrganizerID = @OrganizerID";

            using (SqlConnection con = new SqlConnection(connstr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@OrganizerID", organizerId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ddlEvents.DataSource = reader;
                ddlEvents.DataTextField = "Title";
                ddlEvents.DataValueField = "EventID";
                ddlEvents.DataBind();
                ddlEvents.Items.Insert(0, new ListItem("-- Select Event --", ""));
            }
        }

        protected void ddlEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlEvents.SelectedValue))
            {
                int eventId = Convert.ToInt32(ddlEvents.SelectedValue);
                LoadParticipants(eventId);
                pnlParticipants.Visible = true;
                lblMessage.Visible = false;
            }
            else
            {
                pnlParticipants.Visible = false;
            }
        }

        private void LoadParticipants(int eventId)
        {
            string query = @"
    SELECT pr.RegistrationID,
           pr.UserID,
           u.FullName,
           u.Email,
           pr.AttendanceCode,
           ISNULL(pr.IsPresent,0) AS IsPresent,
           e.Date AS EventDate
    FROM ParticipantRegistrations pr
    INNER JOIN Users u ON pr.UserID = u.UserID
    INNER JOIN Events e ON pr.EventID = e.EventID
    WHERE pr.EventID = @EventID";
            

            using (SqlConnection con = new SqlConnection(connstr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@EventID", eventId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvParticipants.DataSource = dt;
                gvParticipants.DataKeyNames = new string[] { "RegistrationID", "UserID", "IsPresent", "EventDate" };

                gvParticipants.DataBind();
            }

            // Enable certificate button only if attendance is marked AND event date is today or earlier
            // enable/disable Issue Certificate buttons
            foreach (GridViewRow row in gvParticipants.Rows)
            {
                bool isPresent = Convert.ToBoolean(gvParticipants.DataKeys[row.RowIndex]["IsPresent"]);
                DateTime eventDate = Convert.ToDateTime(gvParticipants.DataKeys[row.RowIndex]["EventDate"]);

                Button btnCert = (Button)row.FindControl("btnIssueCertificate");
                CheckBox chkPresent = (CheckBox)row.FindControl("chkPresent");

                bool hasRegVolunteer = HasRegistrationDeskVolunteer(eventId);

                // enable certificate
                if (btnCert != null)
                {
                    if (hasRegVolunteer)
                        btnCert.Enabled = isPresent;  // ✅ enable immediately when attendance is marked by the reg desk
                    else
                        btnCert.Enabled = isPresent && eventDate <= DateTime.Today; // fallback if org marks
                }

                // enable/disable checkbox for organizer attendance marking 
                if (chkPresent != null)
                    chkPresent.Enabled = !hasRegVolunteer;
            }

        }

        // Manual Certificate Issuance by Organizer
        protected void btnIssueCertificate_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32((sender as Button).CommandArgument);
            int eventId = Convert.ToInt32(ddlEvents.SelectedValue);

            using (SqlConnection con = new SqlConnection(connstr))
            {
                con.Open();

                // Check if attendance is marked using IsPresent
                string attendanceCheck = @"
                    SELECT IsPresent 
                    FROM ParticipantRegistrations 
                    WHERE EventID=@EventID AND UserID=@UserID";

                bool attendanceMarked = false;
                using (SqlCommand cmdCheck = new SqlCommand(attendanceCheck, con))
                {
                    cmdCheck.Parameters.AddWithValue("@EventID", eventId);
                    cmdCheck.Parameters.AddWithValue("@UserID", userId);
                    attendanceMarked = Convert.ToBoolean(cmdCheck.ExecuteScalar());
                }

                if (!attendanceMarked)
                {
                    lblMessage.Text = $"⚠️ Cannot issue certificate. Attendance not marked yet.";
                    lblMessage.CssClass = "alert alert-warning";
                    lblMessage.Visible = true;
                    return;
                }

                // Check if certificate already exists
                string checkQuery = @"
                    SELECT COUNT(*) 
                    FROM Certificates 
                    WHERE EventID=@EventID AND UserID=@UserID AND Role='Participant'";

                using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                {
                    checkCmd.Parameters.AddWithValue("@EventID", eventId);
                    checkCmd.Parameters.AddWithValue("@UserID", userId);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        string insertQuery = @"
                            INSERT INTO Certificates (EventID, UserID, IssuedOn, Role)
                            VALUES (@EventID, @UserID, GETDATE(), 'Participant')";

                        using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                        {
                            insertCmd.Parameters.AddWithValue("@EventID", eventId);
                            insertCmd.Parameters.AddWithValue("@UserID", userId);
                            insertCmd.ExecuteNonQuery();
                        }

                        // Update notification flag
                        string updateNotification = @"
                            UPDATE ParticipantRegistrations
                            SET IsNotificationShown = 1
                            WHERE EventID=@EventID AND UserID=@UserID";

                        using (SqlCommand updateCmd = new SqlCommand(updateNotification, con))
                        {
                            updateCmd.Parameters.AddWithValue("@EventID", eventId);
                            updateCmd.Parameters.AddWithValue("@UserID", userId);
                            updateCmd.ExecuteNonQuery();
                        }

                        lblMessage.Text = $"✅ Certificate issued for user ID {userId}.";
                        lblMessage.CssClass = "alert alert-success";
                        lblMessage.Visible = true;
                    }
                    else
                    {
                        lblMessage.Text = $"ℹ️ Certificate already exists for user ID {userId}.";
                        lblMessage.CssClass = "alert alert-info";
                        lblMessage.Visible = true;
                    }
                }
                con.Close();
            }

            LoadParticipants(Convert.ToInt32(ddlEvents.SelectedValue));
        }
    }
}