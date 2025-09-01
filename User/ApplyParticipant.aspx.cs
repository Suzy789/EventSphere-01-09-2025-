using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Authentication.User
{
    public partial class ApplyParticipant : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        // Persist leader info in ViewState so it's available on every postback
        private string LeaderName
        {
            get => (string)(ViewState["LeaderName"] ?? string.Empty);
            set => ViewState["LeaderName"] = value;
        }
        private string LeaderEmail
        {
            get => (string)(ViewState["LeaderEmail"] ?? string.Empty);
            set => ViewState["LeaderEmail"] = value;
        }
        private string LeaderMobile
        {
            get => (string)(ViewState["LeaderMobile"] ?? string.Empty);
            set => ViewState["LeaderMobile"] = value;
        }
        private string LeaderInstitute
        {
            get => (string)(ViewState["LeaderInstitute"] ?? string.Empty);
            set => ViewState["LeaderInstitute"] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Rebuild dynamic team panels on every postback so controls exist for values to bind
            if (IsPostBack)
            {
                phTeams.Controls.Clear();

                foreach (ListItem item in cblSubCategories.Items)
                {
                    if (item.Selected)
                    {
                        int subCatId = Convert.ToInt32(item.Value);
                        int teamSize = GetTeamSize(subCatId);
                        BuildTeamPanel(subCatId, item.Text, teamSize);
                    }
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                int eventId = Convert.ToInt32(Request.QueryString["EventID"]);
                hfEventID.Value = eventId.ToString();
                hfLeaderUserID.Value = Session["UserID"].ToString();

                LoadEventDetails(eventId);
                LoadUserAndParticipantDetails(Convert.ToInt32(Session["UserID"]));
                LoadSubCategories(eventId);
            }
        }

        private void LoadEventDetails(int eventId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT Title FROM Events WHERE EventID=@EventID", con))
            {
                cmd.Parameters.AddWithValue("@EventID", eventId);
                con.Open();
                txtEventName.Text = Convert.ToString(cmd.ExecuteScalar());
            }
        }

        private void LoadUserAndParticipantDetails(int userId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // From Users
                using (SqlCommand cmd = new SqlCommand("SELECT FullName, Email FROM Users WHERE UserID=@UID", con))
                {
                    cmd.Parameters.AddWithValue("@UID", userId);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            LeaderName = Convert.ToString(dr["FullName"]);
                            LeaderEmail = Convert.ToString(dr["Email"]);
                        }
                    }
                }

                // From Participants (profile) -> Mobile, InstituteName
                using (SqlCommand cmd = new SqlCommand("SELECT Mobile, InstituteName FROM Participants WHERE ParticipantID=@PID", con))
                {
                    cmd.Parameters.AddWithValue("@PID", userId);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            LeaderMobile = Convert.ToString(dr["Mobile"]);
                            LeaderInstitute = Convert.ToString(dr["InstituteName"]);
                        }
                    }
                }
            }

            // Prefill the visible read-only leader info at the top of the page
            txtFullName.Text = LeaderName;
            txtEmail.Text = LeaderEmail;
            txtPhone.Text = LeaderMobile;          // show/update-able if you want
            txtOrganization.Text = LeaderInstitute;
        }

        private void LoadSubCategories(int eventId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT SubCategoryID, SubCategoryName FROM ParticipantSubCategories WHERE EventID=@EID ORDER BY SubCategoryName", con))
            {
                cmd.Parameters.AddWithValue("@EID", eventId);
                con.Open();
                cblSubCategories.DataSource = cmd.ExecuteReader();
                cblSubCategories.DataTextField = "SubCategoryName";
                cblSubCategories.DataValueField = "SubCategoryID";
                cblSubCategories.DataBind();
            }
        }

        protected void cblSubCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            // When boxes change, rebuild panels so the team forms appear/disappear
            phTeams.Controls.Clear();

            foreach (ListItem item in cblSubCategories.Items)
            {
                if (item.Selected)
                {
                    int subCatId = Convert.ToInt32(item.Value);
                    int teamSize = GetTeamSize(subCatId);
                    BuildTeamPanel(subCatId, item.Text, teamSize);
                }
            }
        }

        private int GetTeamSize(int subCatId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT TeamSize FROM ParticipantSubCategories WHERE SubCategoryID=@SID", con))
            {
                cmd.Parameters.AddWithValue("@SID", subCatId);
                con.Open();
                object result = cmd.ExecuteScalar();
                return result == null || result == DBNull.Value ? 1 : Convert.ToInt32(result);
            }
        }

        private void BuildTeamPanel(int subCatId, string subCatName, int teamSize)
        {
            var panel = new Panel { CssClass = "mb-4 border p-3 rounded bg-light" };
            panel.Controls.Add(new Literal { Text = $"<h5 class='mb-3'>{subCatName} - Team Members</h5>" });

            for (int i = 0; i < teamSize; i++)
            {
                string prefix = $"sc_{subCatId}_tm_{i}";

                var lbl = new Literal { Text = $"<label class='fw-bold d-block'>Member {i + 1}</label>" };
                var txtName = new TextBox { ID = prefix + "_Name", CssClass = "form-control mb-2" };
                txtName.Attributes["placeholder"] = "Member Name";

                var txtEmailBox = new TextBox { ID = prefix + "_Email", CssClass = "form-control mb-2" };
                txtEmailBox.Attributes["placeholder"] = "Member Email";

                var txtPhoneBox = new TextBox { ID = prefix + "_Phone", CssClass = "form-control mb-3" };
                txtPhoneBox.Attributes["placeholder"] = "Member Phone";

                // Prefill **Member 1** (leader) with persisted ViewState values
                if (i == 0)
                {
                    // Prefer the current header input values if user edited them; else ViewState
                    var headerName = string.IsNullOrWhiteSpace(txtFullName.Text) ? LeaderName : txtFullName.Text;
                    var headerEmail = string.IsNullOrWhiteSpace(txtEmail.Text) ? LeaderEmail : txtEmail.Text;
                    var headerPhone = string.IsNullOrWhiteSpace(txtPhone.Text) ? LeaderMobile : txtPhone.Text;

                    txtName.Text = headerName;
                    txtEmailBox.Text = headerEmail;
                    txtPhoneBox.Text = headerPhone;
                }

                panel.Controls.Add(lbl);
                panel.Controls.Add(txtName);
                panel.Controls.Add(txtEmailBox);
                panel.Controls.Add(txtPhoneBox);
            }

            phTeams.Controls.Add(panel);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!chkAgree.Checked)
            {
                ShowMessage("Please agree to the terms & conditions.", true);
                return;
            }

            // At least one sub-event must be selected
            bool anySelected = false;
            foreach (ListItem it in cblSubCategories.Items) if (it.Selected) { anySelected = true; break; }
            if (!anySelected)
            {
                ShowMessage("Select at least one sub-event/category.", true);
                return;
            }

            int eventId = Convert.ToInt32(hfEventID.Value);
            int userId = Convert.ToInt32(hfLeaderUserID.Value);
            string docPath = null;

            // Save supporting document if provided
            if (fuSupportingDoc.HasFile)
            {
                string ext = Path.GetExtension(fuSupportingDoc.FileName).ToLowerInvariant();
                if (ext != ".pdf" && ext != ".docx")
                {
                    ShowMessage("Only .pdf or .docx files are allowed.", true);
                    return;
                }
                string fileName = Guid.NewGuid() + ext;
                fuSupportingDoc.SaveAs(Server.MapPath("~/Uploads/") + fileName);
                docPath = fileName;
            }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                var tx = con.BeginTransaction();

                try
                {
                    // One-time Attendance Code (leader/registration)
                    string attendanceCode = GenerateAttendanceCode(eventId, con, tx);

                    // Compute total members across selected sub-events
                    int totalMembers = 0;
                    foreach (ListItem item in cblSubCategories.Items)
                        if (item.Selected) totalMembers += GetTeamSize(Convert.ToInt32(item.Value));

                    // Insert registration
                    int regId;
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO ParticipantRegistrations
                        (EventID, UserID, TeamName, CollegeOrOrganization, SupportingDocumentPath,
                         PaymentMode, IsAgreedToTerms, Status, AttendanceCode, NumberOfTeamMembers)
                        VALUES (@EID,@UID,@TName,@Org,@Doc,@Pay,@Agree,'Confirmed',@Code,@TeamMembers);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);", con, tx))
                    {
                        cmd.Parameters.AddWithValue("@EID", eventId);
                        cmd.Parameters.AddWithValue("@UID", userId);
                        cmd.Parameters.AddWithValue("@TName", (object)(txtTeamName.Text ?? string.Empty));
                        cmd.Parameters.AddWithValue("@Org", (object)(txtOrganization.Text ?? string.Empty));
                        cmd.Parameters.AddWithValue("@Doc", (object)(docPath ?? (object)DBNull.Value));
                        cmd.Parameters.AddWithValue("@Pay", ddlPaymentMode.SelectedValue);
                        cmd.Parameters.AddWithValue("@Agree", chkAgree.Checked);
                        cmd.Parameters.AddWithValue("@Code", attendanceCode);
                        cmd.Parameters.AddWithValue("@TeamMembers", totalMembers);

                        regId = (int)cmd.ExecuteScalar();
                    }

                    // Insert selected sub-events and their team members
                    foreach (ListItem item in cblSubCategories.Items)
                    {
                        if (!item.Selected) continue;

                        int subCatId = Convert.ToInt32(item.Value);
                        int teamSize = GetTeamSize(subCatId);

                        using (SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO ParticipantSubEventRegistrations (RegistrationID, SubCategoryID)
                            VALUES (@RID,@SID);", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@RID", regId);
                            cmd.Parameters.AddWithValue("@SID", subCatId);
                            cmd.ExecuteNonQuery();
                        }

                        // Insert team members for this sub-event
                        for (int i = 0; i < teamSize; i++)
                        {
                            string prefix = $"sc_{subCatId}_tm_{i}";
                            string name = GetValue(prefix + "_Name");
                            string email = GetValue(prefix + "_Email");
                            string phone = GetValue(prefix + "_Phone");

                            // Basic validation to avoid blank rows
                            if (string.IsNullOrWhiteSpace(name) ||
                                string.IsNullOrWhiteSpace(email) ||
                                string.IsNullOrWhiteSpace(phone))
                            {
                                tx.Rollback();
                                ShowMessage($"Team member {i + 1} in \"{item.Text}\" is incomplete. Please fill Name, Email, Phone.", true);
                                return;
                            }

                            using (SqlCommand cmd = new SqlCommand(@"
                                INSERT INTO ParticipantTeamMembers
                                (RegistrationID, MemberName, MemberEmail, MemberPhone)
                                VALUES (@RID,@Name,@Email,@Phone);", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@RID", regId);
                                cmd.Parameters.AddWithValue("@Name", name);
                                cmd.Parameters.AddWithValue("@Email", email);
                                cmd.Parameters.AddWithValue("@Phone", phone);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    tx.Commit();
                    ShowMessage($"✅ Registration successful! Attendance Code (Leader only): <b>{attendanceCode}</b>", false);
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    ShowMessage("Error: " + ex.Message, true);
                }
            }
        }

        private string GenerateAttendanceCode(int eventId, SqlConnection con, SqlTransaction tx)
        {
            var rnd = new Random();
            while (true)
            {
                string code = "EVT" + eventId + "-" + rnd.Next(1000, 9999);
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM ParticipantRegistrations WHERE EventID=@EID AND AttendanceCode=@Code", con, tx))
                {
                    cmd.Parameters.AddWithValue("@EID", eventId);
                    cmd.Parameters.AddWithValue("@Code", code);
                    int count = (int)cmd.ExecuteScalar();
                    if (count == 0) return code;
                }
            }
        }

        // Safely reads dynamic textbox value by ID from the PlaceHolder; falls back to Request.Form
        private string GetValue(string id)
        {
            // Controls are rebuilt in OnInit, so this should work
            var txt = phTeams.FindControl(id) as TextBox;
            if (txt != null) return txt.Text?.Trim() ?? string.Empty;

            // Fallback (in case of naming container differences)
            string formKey = FindFormKeyForControlID(id);
            if (!string.IsNullOrEmpty(formKey) && Request.Form[formKey] != null)
                return Request.Form[formKey].Trim();

            return string.Empty;
        }

        // Try to guess the form key when control is inside naming containers
        private string FindFormKeyForControlID(string shortId)
        {
            foreach (string key in Request.Form.Keys)
            {
                if (key != null && key.EndsWith("$" + shortId, StringComparison.OrdinalIgnoreCase))
                    return key;
            }
            return null;
        }

        private void ShowMessage(string msg, bool error)
        {
            lblMessage.Text = msg;
            lblMessage.CssClass = error ? "text-danger" : "text-success";
            lblMessage.Visible = true;
        }
    }
}
