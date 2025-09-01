using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
namespace Authentication.User
{
    public partial class ManageParticipatingEvents : System.Web.UI.Page
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
                LoadUpcomingEvents();

            }

        }


        private void LoadUpcomingEvents()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
SELECT pr.RegistrationID, e.Title AS EventTitle, e.Date AS EventDate, e.Location, pr.RegisteredAt
FROM ParticipantRegistrations pr
INNER JOIN Events e ON pr.EventID = e.EventID
WHERE pr.UserID = @UserID 
    AND CAST(e.Date AS DATE) >= CAST(GETDATE() AS DATE)
    AND pr.Status != 'Cancelled'
ORDER BY e.Date ASC";



                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvUpcomingEvents.DataSource = dt;
                gvUpcomingEvents.DataBind();
            }
        }

        private void LoadSubCategoriesByEvent(int eventId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT SubCategoryID, SubCategoryName FROM ParticipantSubCategories WHERE EventID = @EventID", con))
                {
                    cmd.Parameters.AddWithValue("@EventID", eventId);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        ddlSubCategory.DataSource = reader;
                        ddlSubCategory.DataTextField = "SubCategoryName";
                        ddlSubCategory.DataValueField = "SubCategoryID";
                        ddlSubCategory.DataBind();
                    }
                }
            }

            // Optional: add default item
            ddlSubCategory.Items.Insert(0, new ListItem("-- Select Role --", ""));
        }


        protected void gvUpcomingEvents_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int regId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "CancelRegistration")
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = "UPDATE ParticipantRegistrations SET Status = 'Cancelled' WHERE RegistrationID = @RegistrationID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@RegistrationID", regId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                lblMessage.Text = "Registration cancelled successfully.";
                pnlEditForm.Visible = false;
                LoadUpcomingEvents();
            }
            else if (e.CommandName == "EditRegistration")
            {
                hfEditRegistrationID.Value = regId.ToString();

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string query = @"SELECT pr.SubCategoryID, pr.TeamName, pr.NumberOfTeamMembers, pr.CollegeOrOrganization, pr.PaymentMode,
                        pr.EventID, e.Title AS EventTitle
                 FROM ParticipantRegistrations pr
                 INNER JOIN Events e ON pr.EventID = e.EventID
                 WHERE pr.RegistrationID = @RegistrationID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@RegistrationID", regId);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        int eventId = Convert.ToInt32(reader["EventID"]);
                        lblEventTitle.Text = reader["EventTitle"].ToString(); // Event name (read-only)

                        LoadSubCategoriesByEvent(eventId); // ✅ populate dropdown first

                        // ✅ then assign values
                        ddlSubCategory.SelectedValue = reader["SubCategoryID"].ToString();
                        txtTeamName.Text = reader["TeamName"].ToString();
                        txtTeamSize.Text = reader["NumberOfTeamMembers"].ToString();
                        txtCollege.Text = reader["CollegeOrOrganization"].ToString();
                        ddlPaymentMode.SelectedValue = reader["PaymentMode"].ToString();

                        pnlEditForm.Visible = true;
                    }

                }
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            int regId = Convert.ToInt32(hfEditRegistrationID.Value);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
                    UPDATE ParticipantRegistrations
                    SET SubCategoryID = @SubCategoryID,
                        TeamName = @TeamName,
                        NumberOfTeamMembers = @TeamSize,
                        CollegeOrOrganization = @College,
                        PaymentMode = @PaymentMode
                    WHERE RegistrationID = @RegistrationID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@SubCategoryID", ddlSubCategory.SelectedValue);
                cmd.Parameters.AddWithValue("@TeamName", txtTeamName.Text.Trim());
                cmd.Parameters.AddWithValue("@TeamSize", string.IsNullOrEmpty(txtTeamSize.Text) ? (object)DBNull.Value : Convert.ToInt32(txtTeamSize.Text));
                cmd.Parameters.AddWithValue("@College", txtCollege.Text.Trim());
                cmd.Parameters.AddWithValue("@PaymentMode", ddlPaymentMode.SelectedValue);
                cmd.Parameters.AddWithValue("@RegistrationID", regId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            lblMessage.Text = "Registration updated successfully.";
            pnlEditForm.Visible = false;
            LoadUpcomingEvents();
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            pnlEditForm.Visible = false;
        }
    }
}
