using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Authentication.User
{
    public partial class CreateEvent : System.Web.UI.Page
    {
        // ===== Page Load =====
        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                if (Session["UserID"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                }
                else
                {
                    LoadCategories();
                    // Default visibility for dates
                    divSingleDate.Visible = true;
                    divStartDate.Visible = false;
                    divEndDate.Visible = false;
                }
            }
        }

        // ===== Category Dropdown Load =====
        private void LoadCategories()
        {
            string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT CategoryID, CategoryName FROM EventCategories", conn);
                conn.Open();
                ddlCategory.DataSource = cmd.ExecuteReader();
                ddlCategory.DataTextField = "CategoryName";
                ddlCategory.DataValueField = "CategoryID";
                ddlCategory.DataBind();
                ddlCategory.Items.Insert(0, new ListItem("--Select Category--", ""));
            }
        }

        // ===== Handle Number of Days Toggle =====
        protected void txtNumDays_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtNumDays.Text.Trim(), out int numDays))
            {
                if (numDays <= 1)
                {
                    divSingleDate.Visible = true;
                    divStartDate.Visible = false;
                    divEndDate.Visible = false;
                }
                else
                {
                    divSingleDate.Visible = false;
                    divStartDate.Visible = true;
                    divEndDate.Visible = true;
                }
            }
        }

        // ===== Sub-events Table (stored in ViewState) =====
        private DataTable SubEventsTable
        {
            get
            {
                if (ViewState["SubEvents"] == null)
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("SubCategoryName");
                    dt.Columns.Add("TeamSize", typeof(int));
                    ViewState["SubEvents"] = dt;
                }
                return (DataTable)ViewState["SubEvents"];
            }
            set { ViewState["SubEvents"] = value; }
        }

        protected void btnAddSubEvent_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSubEventName.Text) && !string.IsNullOrEmpty(txtSubTeamSize.Text))
            {
                DataTable dt = SubEventsTable;
                dt.Rows.Add(txtSubEventName.Text, int.Parse(txtSubTeamSize.Text));
                SubEventsTable = dt;

                gvSubEvents.DataSource = dt;
                gvSubEvents.DataBind();

                txtSubEventName.Text = "";
                txtSubTeamSize.Text = "";
            }
        }

        // ===== Create Event Button =====
        protected void btnCreateEvent_Click(object sender, EventArgs e)
        {
            if (!chkTerms.Checked)
            {
                ShowError("You must accept the terms and conditions.");
                return;
            }

            // Gather form values
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();
            string location = txtLocation.Text.Trim();
            string skillsRequired = txtSkills.Text.Trim();
            string categoryId = ddlCategory.SelectedValue;
            string mode = ddlMode.SelectedValue;
            string status = ddlStatus.SelectedValue;
            string contactEmail = txtEmail.Text.Trim();
            string contactPhone = txtPhone.Text.Trim();

            bool allowVolunteers = chkAllowVolunteers.Checked;
            bool allowParticipants = chkAllowParticipants.Checked;

            int.TryParse(txtMaxVolunteers.Text.Trim(), out int maxVolunteers);
            int.TryParse(txtMaxParticipants.Text.Trim(), out int maxParticipants);
            decimal.TryParse(txtFee.Text.Trim(), out decimal registrationFee);

            // Handle dates
            DateTime startDate, endDate;

            if (int.TryParse(txtNumDays.Text.Trim(), out int numDays) && numDays > 1)
            {
                if (!DateTime.TryParse(txtStartDate.Text.Trim(), out startDate) ||
                    !DateTime.TryParse(txtEndDate.Text.Trim(), out endDate))
                {
                    ShowError("Invalid start or end date.");
                    return;
                }
            }
            else
            {
                if (!DateTime.TryParse(txtDate.Text.Trim(), out startDate))
                {
                    ShowError("Invalid event date.");
                    return;
                }
                endDate = startDate; // single-day event
            }

            // Time values
            TimeSpan.TryParse(txtStartTime.Text.Trim(), out TimeSpan startTime);
            TimeSpan.TryParse(txtEndTime.Text.Trim(), out TimeSpan endTime);

            if (!DateTime.TryParse(txtDeadline.Text.Trim(), out DateTime deadline))
            {
                ShowError("Invalid registration deadline.");
                return;
            }

            // Banner upload
            string bannerPath = "";
            if (fuBanner.HasFile)
            {
                string folderPath = Server.MapPath("~/Assets/EventBanners/");
                if (!System.IO.Directory.Exists(folderPath))
                    System.IO.Directory.CreateDirectory(folderPath);

                string fileName = Guid.NewGuid().ToString() + "_" + System.IO.Path.GetFileName(fuBanner.FileName);
                string savePath = System.IO.Path.Combine(folderPath, fileName);
                fuBanner.SaveAs(savePath);
                bannerPath = "~/Assets/EventBanners/" + fileName;
            }

            // Organizer ID
            int organizerId = Convert.ToInt32(Session["UserID"]);
            string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
INSERT INTO Events 
(Title, Description, Location, Date, EndDate, StartTime, EndTime, OrganizerID, CreatedAt, CategoryID, SkillsRequired,
 MaxVolunteers, MaxParticipants, IsVolunteerOpen, IsParticipantOpen, EventBanner, RegistrationFee, EventMode, RegistrationDeadline, 
 Status, ContactEmail, ContactPhone, TermsAccepted)
OUTPUT INSERTED.EventID
VALUES 
(@Title, @Description, @Location, @Date, @EndDate, @StartTime, @EndTime, @OrganizerID, GETDATE(), @CategoryID, @SkillsRequired,
 @MaxVolunteers, @MaxParticipants, @IsVolunteerOpen, @IsParticipantOpen, @EventBanner, @RegistrationFee, @EventMode, @RegistrationDeadline,
 @Status, @ContactEmail, @ContactPhone, @TermsAccepted)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Title", title);
                cmd.Parameters.AddWithValue("@Description", description);
                cmd.Parameters.AddWithValue("@Location", location);
                cmd.Parameters.AddWithValue("@Date", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);
                cmd.Parameters.AddWithValue("@StartTime", startTime);
                cmd.Parameters.AddWithValue("@EndTime", endTime);
                cmd.Parameters.AddWithValue("@OrganizerID", organizerId);
                cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                cmd.Parameters.AddWithValue("@SkillsRequired", skillsRequired);
                cmd.Parameters.AddWithValue("@MaxVolunteers", maxVolunteers);
                cmd.Parameters.AddWithValue("@MaxParticipants", maxParticipants);
                cmd.Parameters.AddWithValue("@IsVolunteerOpen", allowVolunteers);
                cmd.Parameters.AddWithValue("@IsParticipantOpen", allowParticipants);
                cmd.Parameters.AddWithValue("@EventBanner", bannerPath);
                cmd.Parameters.AddWithValue("@RegistrationFee", registrationFee);
                cmd.Parameters.AddWithValue("@EventMode", mode);
                cmd.Parameters.AddWithValue("@RegistrationDeadline", deadline);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@ContactEmail", contactEmail);
                cmd.Parameters.AddWithValue("@ContactPhone", contactPhone);
                cmd.Parameters.AddWithValue("@TermsAccepted", true);

                conn.Open();
                object eventIdObj = cmd.ExecuteScalar();

                if (eventIdObj != null && eventIdObj != DBNull.Value)
                {
                    int eventId = Convert.ToInt32(eventIdObj);

                    // Insert sub-events
                    foreach (DataRow row in SubEventsTable.Rows)
                    {
                        string subEventName = row["SubCategoryName"].ToString();
                        int teamSize = Convert.ToInt32(row["TeamSize"]);

                        SqlCommand insertSubCmd = new SqlCommand(
                            "INSERT INTO ParticipantSubCategories (EventID, SubCategoryName, TeamSize) VALUES (@EventID, @SubCategoryName, @TeamSize)",
                            conn);
                        insertSubCmd.Parameters.AddWithValue("@EventID", eventId);
                        insertSubCmd.Parameters.AddWithValue("@SubCategoryName", subEventName);
                        insertSubCmd.Parameters.AddWithValue("@TeamSize", teamSize);
                        insertSubCmd.ExecuteNonQuery();
                    }

                    ShowSuccess("Event created successfully!");
                    ClearForm();
                }
                else
                {
                    ShowError("Failed to retrieve newly created Event ID.");
                }
            }
        }

        // ===== Helpers =====
        private void ShowError(string message)
        {
            lblStatus.CssClass = "text-danger";
            lblStatus.Text = message;
            lblStatus.Visible = true;
        }

        private void ShowSuccess(string message)
        {
            lblStatus.CssClass = "text-success";
            lblStatus.Text = message;
            lblStatus.Visible = true;
        }

        private void ClearForm()
        {
            txtTitle.Text = "";
            txtDescription.Text = "";
            txtLocation.Text = "";
            txtDate.Text = "";
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            txtSkills.Text = "";
            txtMaxVolunteers.Text = "";
            txtMaxParticipants.Text = "";
            txtStartTime.Text = "";
            txtEndTime.Text = "";
            txtDeadline.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtFee.Text = "0.00";
            txtNumDays.Text = "";
            ddlCategory.SelectedIndex = 0;
            ddlMode.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;
            chkAllowVolunteers.Checked = false;
            chkAllowParticipants.Checked = true;
            chkTerms.Checked = false;
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedText = ddlCategory.SelectedItem.Text.ToLower();
            if (selectedText.Contains("ngo"))
            {
                txtFee.Text = "0.00";
                txtFee.Enabled = false;
            }
            else
            {
                txtFee.Enabled = true;
            }
        }

        protected void cvAudience_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = chkAllowVolunteers.Checked || chkAllowParticipants.Checked;
        }
        protected void chkAllowParticipants_CheckedChanged(object sender, EventArgs e)
        {
            ToggleParticipantCategoryVisibility();
        }

        private void ToggleParticipantCategoryVisibility()
        {
            // Show sub-event panel only if participants are allowed
            trParticipantCategories.Visible = chkAllowParticipants.Checked;
        }

        protected void rfvParticipantCategories_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // If participants are allowed, ensure at least one sub-event exists
            if (chkAllowParticipants.Checked)
                args.IsValid = SubEventsTable.Rows.Count > 0;
            else
                args.IsValid = true;
        }

    }
}
