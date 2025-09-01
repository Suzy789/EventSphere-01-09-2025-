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
    public partial class Feedback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadAttendedEvents();
                pnlParticipantFeedback.Visible = false;
                pnlVolunteerFeedback.Visible = false;
                lblFeedbackMsg.Visible = false;
            }
        }

        private void LoadAttendedEvents()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"
            SELECT DISTINCT 
                E.EventID,
                E.Title + ' [Participant]' AS DisplayTitle
            FROM Events E
            INNER JOIN ParticipantRegistrations PR ON E.EventID = PR.EventID
            WHERE PR.UserID = @UserID AND PR.Status = 'Confirmed'

            UNION

            SELECT DISTINCT 
                E.EventID,
                E.Title + ' [Volunteer]' AS DisplayTitle
            FROM Events E
            INNER JOIN VolunteerApplications VA ON E.EventID = VA.EventID
            WHERE VA.UserID = @UserID AND VA.Status = 'Approved'
        ";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userId);

                conn.Open();
                ddlEvents.DataSource = cmd.ExecuteReader();
                ddlEvents.DataTextField = "DisplayTitle";
                ddlEvents.DataValueField = "EventID";
                ddlEvents.DataBind();

                ddlEvents.Items.Insert(0, new ListItem("Select Event", ""));
            }
        }


        protected void ddlEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlParticipantFeedback.Visible = false;
            pnlVolunteerFeedback.Visible = false;

            if (ddlEvents.SelectedIndex <= 0)
                return;

            int userId = Convert.ToInt32(Session["UserID"]);
            int eventId = Convert.ToInt32(ddlEvents.SelectedValue);
            string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string participantQuery = "SELECT COUNT(*) FROM ParticipantRegistrations WHERE UserID = @UserID AND EventID = @EventID AND Status = 'Confirmed'";
                SqlCommand participantCmd = new SqlCommand(participantQuery, conn);
                participantCmd.Parameters.AddWithValue("@UserID", userId);
                participantCmd.Parameters.AddWithValue("@EventID", eventId);
                int isParticipant = (int)participantCmd.ExecuteScalar();

                string volunteerQuery = "SELECT COUNT(*) FROM VolunteerApplications WHERE UserID = @UserID AND EventID = @EventID AND Status = 'Approved'";
                SqlCommand volunteerCmd = new SqlCommand(volunteerQuery, conn);
                volunteerCmd.Parameters.AddWithValue("@UserID", userId);
                volunteerCmd.Parameters.AddWithValue("@EventID", eventId);
                int isVolunteer = (int)volunteerCmd.ExecuteScalar();

                pnlParticipantFeedback.Visible = isParticipant > 0;
                pnlVolunteerFeedback.Visible = isVolunteer > 0;
            }
        }

        protected void btnSubmitFeedback_Click(object sender, EventArgs e)
        {
            if (ddlEvents.SelectedIndex <= 0)
                return;

            int userId = Convert.ToInt32(Session["UserID"]);
            int eventId = Convert.ToInt32(ddlEvents.SelectedValue);

            // Common fields
            string overallRating = ddlOverallRating.SelectedValue;
            string enjoyed = txtEnjoyed.Text.Trim();
            string disliked = txtDisliked.Text.Trim();
            string communication = txtCommunication.Text.Trim();
            string suggestions = txtSuggestions.Text.Trim();
            string age = ddlAge.SelectedValue;
            string gender = ddlGender.SelectedValue;
            string email = txtEmail.Text.Trim();

            string message = $"Overall Rating: {overallRating}\nEnjoyed: {enjoyed}\nDisliked: {disliked}";

            // Participant-specific
            if (pnlParticipantFeedback.Visible)
            {
                message += "\n\n[Participant Feedback]";
                message += $"\nSession Quality: {ddlSessionQuality.SelectedValue}";
                message += $"\nValuable Topics: {txtValuableTopics.Text.Trim()}";
                message += $"\nSpeaker Rating: {ddlSpeakersRating.SelectedValue}";
                message += $"\nVenue Rating: {ddlVenue.SelectedValue}";
                message += $"\nRegistration Process: {ddlRegistration.SelectedValue}";
                message += $"\nExpectations Met: {txtExpectations.Text.Trim()}";
            }

            // Volunteer-specific
            if (pnlVolunteerFeedback.Visible)
            {
                message += "\n\n[Volunteer Feedback]";
                message += $"\nVolunteer Experience: {ddlVolunteerExperience.SelectedValue}";
                message += $"\nCoordinator Helpfulness: {ddlCoordinator.SelectedValue}";
            }

            // Extra info
            message += $"\n\n[Additional Info]";
            message += $"\nCommunication: {communication}";
            message += $"\nSuggestions: {suggestions}";
            message += $"\nAge: {age}, Gender: {gender}, Email: {email}";

            string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string insertQuery = "INSERT INTO Feedback (EventID, UserID, Message, SubmittedAt) VALUES (@EventID, @UserID, @Message, GETDATE())";
                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@EventID", eventId);
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@Message", message);

                conn.Open();
                cmd.ExecuteNonQuery();

                lblFeedbackMsg.Visible = true;
                lblFeedbackMsg.Text = "Thank you for your feedback!";
                lblFeedbackMsg.CssClass = "text-success fw-bold";

                // Optionally clear fields
                ClearFields();
            }
        }

        private void ClearFields()
        {
            ddlEvents.SelectedIndex = 0;
            ddlOverallRating.SelectedIndex = 0;
            txtEnjoyed.Text = "";
            txtDisliked.Text = "";
            ddlSessionQuality.SelectedIndex = 0;
            txtValuableTopics.Text = "";
            ddlSpeakersRating.SelectedIndex = 0;
            ddlVenue.SelectedIndex = 0;
            ddlRegistration.SelectedIndex = 0;
            txtExpectations.Text = "";
            ddlVolunteerExperience.SelectedIndex = 0;
            ddlCoordinator.SelectedIndex = 0;
            ddlAge.SelectedIndex = 0;
            ddlGender.SelectedIndex = 0;
            txtEmail.Text = "";
            txtCommunication.Text = "";
            txtSuggestions.Text = "";
            pnlParticipantFeedback.Visible = false;
            pnlVolunteerFeedback.Visible = false;
        }
    }
}