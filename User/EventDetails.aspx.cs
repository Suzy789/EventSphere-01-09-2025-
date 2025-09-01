using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Authentication.User
{
	public partial class EventDetails : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
    {
        // ✅ Check session
        if (Session["UserID"] == null || Session["Role"] == null)
        {
            Response.Redirect("~/Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        // ✅ Check querystring
        if (string.IsNullOrEmpty(Request.QueryString["eventid"]))
        {
            lblMessage.Text = "Invalid event.";
            lblMessage.CssClass = "text-danger";
            lblMessage.Visible = true;
            return;
        }

        int eventId = Convert.ToInt32(Request.QueryString["eventid"]);
        int userId = Convert.ToInt32(Session["UserID"]);
        string role = Session["Role"].ToString();

        // ✅ Load event
        LoadEventDetails(eventId);

        // ✅ Role-based UI
        if (role == "Organizer" && IsEventOwnedByUser(eventId, userId))
        {
            btnEdit.Visible = true;
            btnDelete.Visible = true;
            pnlVolunteerCategories.Visible = false;
            pnlParticipant.Visible = false;
            btnBackToDashboardOrganizer.Visible = true;
        }
        else if (role == "Volunteer")
        {
            pnlVolunteerCategories.Visible = true;
            pnlParticipant.Visible = false;
            btnBackToDashboardOrganizer.Visible = false;
            LoadVolunteerCategories(eventId);
        }
        else if (role == "Participant")
        {
            pnlParticipant.Visible = true;
            pnlVolunteerCategories.Visible = false;
            btnBackToDashboardOrganizer.Visible = false;
        }
        else
        {
            pnlVolunteerCategories.Visible = false;
            pnlParticipant.Visible = false;
            btnBackToDashboardOrganizer.Visible = false;
        }
    }
		}

		#region Load Event
		private void LoadEventDetails(int eventId)
		{
			try
			{
				string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

				string query = @"
SELECT e.Title, e.Description, e.Date, e.StartTime, e.EndTime, e.Location,
       ec.CategoryName, e.EventMode, e.RegistrationDeadline, e.Status,
       e.ContactEmail, e.ContactPhone, e.RegistrationFee, e.TermsAccepted,
       u.FullName AS Organizer, e.EventBanner
FROM Events e
JOIN Users u ON e.OrganizerID = u.UserID
LEFT JOIN EventCategories ec ON e.CategoryID = ec.CategoryID
WHERE e.EventID = @EventID";

				using (SqlConnection conn = new SqlConnection(connStr))
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@EventID", eventId);
					conn.Open();

					SqlDataReader reader = cmd.ExecuteReader();
					if (reader.Read())
					{
						// Populate labels
						lblTitle.Text = reader["Title"].ToString();
						lblDescription.Text = reader["Description"].ToString();
						lblDate.Text = Convert.ToDateTime(reader["Date"]).ToString("yyyy-MM-dd");
						lblStartTime.Text = reader["StartTime"] != DBNull.Value ? reader["StartTime"].ToString() : "-";
						lblEndTime.Text = reader["EndTime"] != DBNull.Value ? reader["EndTime"].ToString() : "-";
						lblLocation.Text = reader["Location"].ToString();
						lblCategory.Text = reader["CategoryName"]?.ToString() ?? "-";
						lblMode.Text = reader["EventMode"]?.ToString() ?? "-";
						lblDeadline.Text = reader["RegistrationDeadline"] != DBNull.Value ? Convert.ToDateTime(reader["RegistrationDeadline"]).ToString("dd MMM yyyy") : "-";
						lblStatus.Text = reader["Status"]?.ToString() ?? "-";
						lblEmail.Text = reader["ContactEmail"]?.ToString() ?? "-";
						lblPhone.Text = reader["ContactPhone"]?.ToString() ?? "-";
						lblFee.Text = reader["RegistrationFee"] != DBNull.Value ? Convert.ToDecimal(reader["RegistrationFee"]).ToString("₹0.00") : "Free";
						lblTerms.Text = (reader["TermsAccepted"] != DBNull.Value && Convert.ToBoolean(reader["TermsAccepted"])) ? "Yes" : "No";
						lblOrganizer.Text = reader["Organizer"].ToString();


						// Banner
						string bannerPath = reader["EventBanner"]?.ToString();
						imgBanner.ImageUrl = bannerPath;
						imgBanner.Visible = !string.IsNullOrEmpty(bannerPath);
					}
					else
					{
						lblMessage.Text = "Event not found.";
						lblMessage.Visible = true;
					}

					reader.Close();
				}
			}
			catch (Exception ex)
			{
				lblMessage.Text = "Error loading event: " + ex.Message;
				lblMessage.CssClass = "text-danger";
				lblMessage.Visible = true;
			}
		}
		#endregion

		#region Check Ownership
		private bool IsEventOwnedByUser(int eventId, int userId)
		{
			try
			{
				string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
				using (SqlConnection conn = new SqlConnection(connStr))
				using (SqlCommand cmd = new SqlCommand(
					"SELECT COUNT(*) FROM Events WHERE EventID=@EID AND OrganizerID=@UID", conn))
				{
					cmd.Parameters.AddWithValue("@EID", eventId);
					cmd.Parameters.AddWithValue("@UID", userId);
					conn.Open();
					return (int)cmd.ExecuteScalar() > 0;
				}
			}
			catch
			{
				return false;
			}
		}
		#endregion

		#region Load Volunteer Categories
		private void LoadVolunteerCategories(int eventId)
		{
			try
			{
				string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
				string query = @"
            SELECT 
                vc.CategoryID,
                vc.CategoryName,
                vc.RequiredVolunteers,
                ISNULL((
                    SELECT COUNT(*) 
                    FROM VolunteerApplications va
                    WHERE va.CategoryID = vc.CategoryID AND va.EventID = @EID
                ), 0) AS AllocatedVolunteers,
                (vc.RequiredVolunteers - ISNULL((
                    SELECT COUNT(*) 
                    FROM VolunteerApplications va
                    WHERE va.CategoryID = vc.CategoryID AND va.EventID = @EID
                ), 0)) AS RemainingVolunteers
            FROM VolunteerCategories vc
            WHERE vc.EventID = @EID";

				using (SqlConnection conn = new SqlConnection(connStr))
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddWithValue("@EID", eventId);
					conn.Open();
					rptVolunteerCategories.DataSource = cmd.ExecuteReader();
					rptVolunteerCategories.DataBind();
				}
			}
			catch (Exception ex)
			{
				lblMessage.Text = "Error loading volunteer categories: " + ex.Message;
				lblMessage.CssClass = "text-danger";
				lblMessage.Visible = true;
			}
		}

		#endregion


		#region Participant Registration
		protected void btnRegisterParticipant_Click(object sender, EventArgs e)
		{
           
           
                Response.Redirect($"ApplyParticipant.aspx?eventid={Request.QueryString["eventid"]}");

        }
        #endregion


        #region Other Buttons
        protected void btnEdit_Click(object sender, EventArgs e)
        {
            ToggleEditMode(true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                int eventId = Convert.ToInt32(Request.QueryString["eventid"]);
                string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Events WHERE EventID=@EID", conn);
                    cmd.Parameters.AddWithValue("@EID", eventId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                Response.Redirect("BrowseEvents.aspx");
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error deleting event: " + ex.Message;
                lblMessage.CssClass = "text-danger";
                lblMessage.Visible = true;
            }
        }



        protected void btnApplyVolunteerRedirect_Click(object sender, EventArgs e)
        {
            // Redirect with event id
            Response.Redirect($"ApplyVolunteer.aspx?eventid={Request.QueryString["eventid"]}");
        }
        #endregion
        private void ToggleEditMode(bool isEdit)
        {
            lblTitle.Visible = lblDescription.Visible = lblDate.Visible =
            lblStartTime.Visible = lblEndTime.Visible = lblLocation.Visible =
            lblEmail.Visible = lblPhone.Visible = lblFee.Visible = !isEdit;

            txtTitle.Visible = txtDescription.Visible = txtDate.Visible =
            txtStartTime.Visible = txtEndTime.Visible = txtLocation.Visible =
            txtEmail.Visible = txtPhone.Visible = txtFee.Visible = isEdit;

            btnEdit.Visible = !isEdit;
            btnUpdate.Visible = isEdit;
            btnCancel.Visible = isEdit;
            btnDelete.Visible = !isEdit;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ToggleEditMode(false);
            LoadEventDetails(Convert.ToInt32(Request.QueryString["eventid"]));
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                int eventId = Convert.ToInt32(Request.QueryString["eventid"]);
                string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(@"
            UPDATE Events SET 
                Title = @Title,
                Description = @Description,
                Date = @Date,
                StartTime = @StartTime,
                EndTime = @EndTime,
                Location = @Location,
                ContactEmail = @Email,
                ContactPhone = @Phone,
                RegistrationFee = @Fee
            WHERE EventID = @EventID", conn))
                {
                    cmd.Parameters.AddWithValue("@Title", txtTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@Description", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@Date", txtDate.Text.Trim());
                    cmd.Parameters.AddWithValue("@StartTime", txtStartTime.Text.Trim());
                    cmd.Parameters.AddWithValue("@EndTime", txtEndTime.Text.Trim());
                    cmd.Parameters.AddWithValue("@Location", txtLocation.Text.Trim());
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@Phone", txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@Fee", decimal.TryParse(txtFee.Text.Trim(), out decimal fee) ? fee : 0);
                    cmd.Parameters.AddWithValue("@EventID", eventId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                ToggleEditMode(false);
                LoadEventDetails(eventId);

                lblMessage.Text = "Event updated successfully.";
                lblMessage.CssClass = "text-success";
                lblMessage.Visible = true;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error updating event: " + ex.Message;
                lblMessage.CssClass = "text-danger";
                lblMessage.Visible = true;
            }
        }
    }

}
