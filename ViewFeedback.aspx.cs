using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Authentication.User
{
    public partial class ViewFeedback : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            

            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }
            if (!IsPostBack)
                LoadEvents();
        }

        private void LoadEvents()
        {

            int organizerId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT EventID, Title FROM Events WHERE OrganizerID = @OrganizerID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@OrganizerID", organizerId);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    ddlEvents.DataSource = reader;
                    ddlEvents.DataTextField = "Title";
                    ddlEvents.DataValueField = "EventID";
                    ddlEvents.DataBind();

                    reader.Close();
                }
            }

            ddlEvents.Items.Insert(0, "-- Select Event --");
        }


        protected void ddlEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEvents.SelectedIndex > 0)
                LoadFeedback();
            else
            {
                rptFeedback.DataSource = null;
                rptFeedback.DataBind();
                feedbackSummary.Visible = false;
                lblMsg.Visible = false;
            }
        }

        private void LoadFeedback()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT f.FeedbackID, f.EventID, f.UserID,
                        f.OverallRating, f.Enjoyed, f.Disliked, f.Suggestions,
                        f.SessionQuality, f.ValuableTopics, f.SpeakerRating,
                        f.VenueRating, f.RegistrationRating, f.Expectations,
                        f.VolunteerExperience, f.CoordinatorHelpfulness, f.Communication,
                        f.AgeRange, f.Gender, f.Email,
                        f.SubmittedAt, 
                        u.FullName AS UserFullName, u.Role AS UserRole
                 FROM Feedback f
                 JOIN Users u ON f.UserID = u.UserID
                 WHERE f.EventID = @EventID";



                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EventID", Convert.ToInt32(ddlEvents.SelectedValue));

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    rptFeedback.DataSource = dt;
                    rptFeedback.DataBind();

                    // If ratings exist, compute average
                    if (dt.Columns.Contains("OverallRating") && dt.Compute("COUNT(OverallRating)", "") is int count && count > 0)
                    {
                        double avgRating = Convert.ToDouble(dt.Compute("AVG(OverallRating)", string.Empty));
                        litAverageRating.Text = avgRating.ToString("0.0");
                        litStars.Text = GetStars(avgRating);
                        litTotalFeedback.Text = dt.Rows.Count.ToString();
                        feedbackSummary.Visible = true;
                    }
                    else
                    {
                        feedbackSummary.Visible = false;
                    }

                    lblMsg.Visible = false;
                }
                else
                {
                    rptFeedback.DataSource = null;
                    rptFeedback.DataBind();
                    feedbackSummary.Visible = false;
                    lblMsg.Text = "No feedback available.";
                    lblMsg.Visible = true;
                }
            }
        }

        // Generate star rating (★ ☆)
        public string GetStars(object ratingObj)
        {
            if (ratingObj == DBNull.Value) return "";

            double rating = Convert.ToDouble(ratingObj);
            int fullStars = (int)Math.Floor(rating);
            bool halfStar = (rating - fullStars) >= 0.5;
            int emptyStars = 5 - fullStars - (halfStar ? 1 : 0);

            StringBuilder sb = new StringBuilder();
            sb.Append(new string('★', fullStars));
            if (halfStar) sb.Append("☆");
            sb.Append(new string('☆', emptyStars));

            return $"<span class='text-warning fs-5'>{sb}</span>";
        }
    }
}
