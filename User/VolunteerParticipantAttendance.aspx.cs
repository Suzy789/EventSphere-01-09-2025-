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
    public partial class VolunteerParticipantAttendance : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] == null || Session["Role"].ToString() != "Volunteer")
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                LoadEvents();
            }
        }
        private bool HasRegistrationDeskVolunteer(int eventId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT COUNT(*) 
        FROM VolunteerApplications va
        INNER JOIN VolunteerCategories vc ON va.CategoryID = vc.CategoryID
        WHERE va.EventID = @EID AND vc.CategoryName='Registration Desk' AND va.Status='Approved'", con))
            {
                cmd.Parameters.AddWithValue("@EID", eventId);
                con.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private void LoadEvents()
        {
            int volunteerId = Convert.ToInt32(Session["UserID"]);

            string query = @"
             SELECT DISTINCT e.EventID, e.Title
FROM Events e
INNER JOIN VolunteerApplications va ON e.EventID = va.EventID
INNER JOIN VolunteerCategories vc ON va.CategoryID = vc.CategoryID
WHERE va.UserID = @VolunteerID
  AND vc.CategoryName = 'Registration Desk'
  AND va.Status = 'Approved'";


            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@VolunteerID", volunteerId);
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

                // ✅ Disable QR textbox and grid checkboxes if there is no Registration Desk volunteer
                bool hasRegVol = HasRegistrationDeskVolunteer(eventId);
                txtQRCode.Enabled = hasRegVol;

                foreach (GridViewRow row in gvParticipants.Rows)
                {
                    CheckBox chk = row.FindControl("chkPresent") as CheckBox;
                    if (chk != null) chk.Enabled = hasRegVol;
                }
            }
            else
            {
                pnlParticipants.Visible = false;
            }
        }


        private void LoadParticipants(int eventId)
        {
            string query = @"
                SELECT pr.RegistrationID, u.FullName, u.Email, pr.AttendanceCode, pr.IsPresent
                FROM ParticipantRegistrations pr
                INNER JOIN Users u ON pr.UserID = u.UserID
                WHERE pr.EventID=@EventID AND pr.Status='Confirmed' AND pr.IsCancelled=0";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@EventID", eventId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvParticipants.DataSource = dt;
                gvParticipants.DataBind();
            }
        }

        protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
        {
            base.RaisePostBackEvent(sourceControl, eventArgument);

            if (Request["__EVENTTARGET"] == "MarkQRCode")
            {
                MarkAttendanceByQRCode();
            }
        }
        protected void chkPresent_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            GridViewRow row = (GridViewRow)chk.NamingContainer;

            int registrationId = Convert.ToInt32(gvParticipants.DataKeys[row.RowIndex].Value);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "UPDATE ParticipantRegistrations SET IsPresent=@IsPresent WHERE RegistrationID=@RegistrationID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@IsPresent", chk.Checked ? 1 : 0);
                    cmd.Parameters.AddWithValue("@RegistrationID", registrationId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            lblMessage.Text = chk.Checked ? "✅ Attendance marked" : "⚠️ Attendance unmarked";
            lblMessage.CssClass = chk.Checked ? "alert alert-success" : "alert alert-warning";
            lblMessage.Visible = true;
        }

        private void MarkAttendanceByQRCode()
        {
            string qrCode = txtQRCode.Text.Trim();
            int eventId = Convert.ToInt32(ddlEvents.SelectedValue);

            if (string.IsNullOrEmpty(qrCode))
            {
                lblMessage.Text = "⚠️ Please enter a QR code.";
                lblMessage.CssClass = "alert alert-warning";
                lblMessage.Visible = true;
                return;
            }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Check if the QR code exists and attendance not marked
                string checkQuery = @"
            SELECT COUNT(*) 
            FROM ParticipantRegistrations
            WHERE EventID=@EventID AND AttendanceCode=@Code AND IsPresent=0";

                using (SqlCommand cmd = new SqlCommand(checkQuery, con))
                {
                    cmd.Parameters.AddWithValue("@EventID", eventId);
                    cmd.Parameters.AddWithValue("@Code", qrCode);
                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        // Mark attendance
                        string updateQuery = @"
                    UPDATE ParticipantRegistrations
                    SET IsPresent=1
                    WHERE EventID=@EventID AND AttendanceCode=@Code";

                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, con))
                        {
                            updateCmd.Parameters.AddWithValue("@EventID", eventId);
                            updateCmd.Parameters.AddWithValue("@Code", qrCode);
                            int rows = updateCmd.ExecuteNonQuery();

                            if (rows > 0)
                            {
                                lblMessage.Text = "✅ Attendance marked successfully.";
                                lblMessage.CssClass = "alert alert-success";
                                lblMessage.Visible = true;
                            }
                            else
                            {
                                lblMessage.Text = "⚠️ Attendance could not be updated. Try again.";
                                lblMessage.CssClass = "alert alert-danger";
                                lblMessage.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        lblMessage.Text = "⚠️ Invalid or already used QR code.";
                        lblMessage.CssClass = "alert alert-danger";
                        lblMessage.Visible = true;
                    }
                }
            }

            LoadParticipants(eventId);
            txtQRCode.Text = "";
        }
    

        
            }
        }
    

