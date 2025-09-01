using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using System.Data.SqlClient;
using System.Data;
namespace Authentication.User
{
    public partial class DownloadEventReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string role = Session["Role"]?.ToString();
                if (string.IsNullOrEmpty(role) || (role != "Admin" && role != "Organizer"))
                    Response.Redirect("~/Login.aspx");

                LoadEvents();
            }
        }

        private void LoadEvents()
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            string userRole = Session["Role"].ToString();
            int userId = Convert.ToInt32(Session["UserID"]);

            string query = @"SELECT EventID, Title FROM Events";
            if (userRole == "Organizer")
                query += " WHERE OrganizerID = @UserID";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (userRole == "Organizer")
                    cmd.Parameters.AddWithValue("@UserID", userId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                ddlEvents.DataSource = reader;
                ddlEvents.DataValueField = "EventID";
                ddlEvents.DataTextField = "Title";
                ddlEvents.DataBind();
                ddlEvents.Items.Insert(0, new ListItem("-- Select Event --", ""));
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            string eventId = ddlEvents.SelectedValue;
            string reportType = ddlType.SelectedValue;

            if (string.IsNullOrEmpty(eventId) || string.IsNullOrEmpty(reportType))
            {
                lblMessage.Text = "Please select both Event and Report Type.";
                return;
            }

            DataTable dt = (reportType == "Participants") ? GetParticipantData(eventId) : GetVolunteerData(eventId);
            if (dt.Rows.Count > 0)
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add(dt, reportType + " Report");
                    ws.Columns().AdjustToContents();
                    ws.RangeUsed().Style.Font.FontName = "Calibri";
                    ws.RangeUsed().Style.Font.FontSize = 11;

                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", $"attachment;filename={reportType}_Report_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");

                    using (var memoryStream = new System.IO.MemoryStream())
                    {
                        wb.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            else
            {
                lblMessage.Text = "No data found for the selected report.";
            }
        }

        private DataTable GetParticipantData(string eventId)
        {
            DataTable dt = new DataTable();
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            string query = @"
SELECT 
    E.Title AS EventName,

    -- Registered User Info
    U.FullName AS RegisteredBy,
    U.Email AS RegisteredByEmail,
    P.Mobile,
    P.Gender,
    P.Qualification,

    -- Registration Info
    PR.ParticipantRole,
    PR.TeamName,
    PR.CollegeOrOrganization,
    PR.PaymentMode,
    PR.Status,
    PR.RegisteredAt,

    -- Team Member Details
    PTM.MemberName,
    PTM.MemberEmail,
    PTM.MemberPhone,

    -- Sub-Event Info (if any)
    PSC.SubCategoryName

FROM ParticipantRegistrations PR
INNER JOIN Users U ON PR.UserID = U.UserID
LEFT JOIN Participants P ON U.UserID = P.ParticipantID
LEFT JOIN ParticipantTeamMembers PTM ON PR.RegistrationID = PTM.RegistrationID
LEFT JOIN ParticipantSubEventRegistrations PSER ON PR.RegistrationID = PSER.RegistrationID
LEFT JOIN ParticipantSubCategories PSC ON PSER.SubCategoryID = PSC.SubCategoryID
INNER JOIN Events E ON PR.EventID = E.EventID
WHERE PR.EventID = @EventID
ORDER BY PR.RegistrationID, PTM.MemberID, PSC.SubCategoryName";


            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@EventID", eventId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        private DataTable GetVolunteerData(string eventId)
        {
            DataTable dt = new DataTable();
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            string query = @"
                SELECT U.FullName, U.Email, VP.Mobile, VP.Gender, VP.Skills, VA.Status AS ApplicationStatus,
                       VD.RoleTitle, VD.Description, VD.IsCompleted, VD.IsApprovedByOrganizer
                FROM Volunteers V
                INNER JOIN Users U ON V.UserID = U.UserID
                LEFT JOIN VolunteerProfiles VP ON U.UserID = VP.VolunteerID
                LEFT JOIN VolunteerApplications VA ON V.UserID = VA.UserID AND VA.EventID = V.EventID
                LEFT JOIN VolunteerDuties VD ON VD.VolunteerID = V.UserID AND VD.EventID = V.EventID
                WHERE V.EventID = @EventID";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@EventID", eventId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }
    }
}