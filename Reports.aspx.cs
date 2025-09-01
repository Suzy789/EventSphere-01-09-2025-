using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office.Word;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace Authentication.Admin
{
    public partial class Reports : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadEventsDropdown();
        }
        private void LoadEventsDropdown()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT EventID, Title FROM Events ORDER BY Date DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                ddlEvents.DataSource = cmd.ExecuteReader();
                ddlEvents.DataTextField = "Title";   // what user sees
                ddlEvents.DataValueField = "EventID"; // what backend uses
                ddlEvents.DataBind();
            }

            ddlEvents.Items.Insert(0, new ListItem("-- Select Event --", "0"));
        }
        protected void btnExportEvents_Click(object sender, EventArgs e)
        {
            ExportReport(GetEvents(), "EventsReport.xlsx", "Events");
        }

        protected void btnExportParticipants_Click(object sender, EventArgs e)
        {
            int eventId = int.Parse(ddlEvents.SelectedValue); // from dropdown
            ExportReport(GetParticipants(), "ParticipantsReport.xlsx", "Participants");
        }

        protected void btnExportVolunteers_Click(object sender, EventArgs e)
        {
            ExportReport(GetVolunteers(), "VolunteersReport.xlsx", "Volunteers");
        }

        protected void btnExportFullReport_Click(object sender, EventArgs e)
        {
            DataTable dtEvents = GetEvents();
            DataTable dtParticipants = GetParticipants();
            DataTable dtVolunteers = GetVolunteers();

            if (dtEvents.Rows.Count == 0 && dtParticipants.Rows.Count == 0 && dtVolunteers.Rows.Count == 0)
            {
                ShowError("No data found for the selected date range.");
                return;
            }

            // Create a merged DataTable
            DataTable merged = new DataTable("FullReport");

            // Add common columns
            merged.Columns.Add("ReportType"); // Event / Participant / Volunteer
            merged.Columns.Add("EventID");
            merged.Columns.Add("EventTitle");
            merged.Columns.Add("EventDate");
            merged.Columns.Add("Location");

            // Add participant-specific columns
            merged.Columns.Add("RegistrationID");
            merged.Columns.Add("RegisteredBy");
            merged.Columns.Add("RegisteredByEmail");
            merged.Columns.Add("Mobile");
            merged.Columns.Add("TeamName");
            merged.Columns.Add("MemberName");
            merged.Columns.Add("MemberEmail");
            merged.Columns.Add("MemberPhone");
            merged.Columns.Add("SubCategoryName");

            // Add volunteer-specific columns
            merged.Columns.Add("VolunteerID");
            merged.Columns.Add("VolunteerName");
            merged.Columns.Add("VolunteerEmail");
            merged.Columns.Add("ApplicationStatus");

            // Fill Events
            foreach (DataRow ev in dtEvents.Rows)
            {
                merged.Rows.Add("Event",
                    ev["EventID"], ev["EventTitle"], ev["Date"], ev["Location"],
                    null, null, null, null, null, null, null, null, null,
                    null, null, null);
            }

            // Fill Participants
            foreach (DataRow p in dtParticipants.Rows)
            {
                merged.Rows.Add("Participant",
                    null, p["Title"], null, null,
                    p["RegistrationID"], p["RegisteredBy"], p["Email"], p["Mobile"],
                    p["TeamName"], p["MemberName"], p["MemberEmail"], p["MemberPhone"], p["SubCategoryName"],
                    null, null, null, null);
            }

            // Fill Volunteers
            foreach (DataRow v in dtVolunteers.Rows)
            {
                merged.Rows.Add("Volunteer",
                    null, v["EventTitle"], v["EventDate"], null,
                    null, null, null, null, null, null, null, null, null,
                    v["VolunteerID"], v["FullName"], v["Email"], v["ApplicationStatus"]);
            }

            // Export single sheet
            ExportReport(merged, "FullReport.xlsx", "FullReport");
        }


        private DataTable GetEvents()
        {
            DateTime? startDate = ParseDate(txtStartDate.Text.Trim());
            DateTime? endDate = ParseDate(txtEndDate.Text.Trim());

            string query = @"
        SELECT 
    EventID,
    Title AS EventTitle,
    Date,
    Location,
    CategoryID
FROM Events
WHERE (@StartDate IS NULL OR Date >= @StartDate)
  AND (@EndDate IS NULL OR Date <= @EndDate)";

            return GetDataTable(query, startDate, endDate);
        }


        private DataTable GetParticipants()
        {
            DataTable dt = new DataTable();
            string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            string query = @"
        SELECT 
            E.Title,
            PR.RegistrationID,
            U.FullName AS RegisteredBy,
            U.Email,
            P.Mobile,
            PR.TeamName,
            PTM.MemberName,
            PTM.MemberEmail,
            PTM.MemberPhone,
            PSC.SubCategoryName
        FROM ParticipantRegistrations PR
        INNER JOIN Events E ON PR.EventID = E.EventID
        INNER JOIN Users U ON PR.UserID = U.UserID
        LEFT JOIN Participants P ON U.UserID = P.ParticipantID
        LEFT JOIN ParticipantTeamMembers PTM ON PR.RegistrationID = PTM.RegistrationID
        LEFT JOIN ParticipantSubEventRegistrations PSER ON PR.RegistrationID = PSER.RegistrationID
        LEFT JOIN ParticipantSubCategories PSC ON PSER.SubCategoryID = PSC.SubCategoryID
        ORDER BY E.Title, PR.RegistrationID, PTM.MemberID";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }




        private DataTable GetVolunteers()
        {
            DateTime? startDate = ParseDate(txtStartDate.Text.Trim());
            DateTime? endDate = ParseDate(txtEndDate.Text.Trim());

            string query = @"
                SELECT V.VolunteerID, U.FullName, U.Email, E.Title AS EventTitle, E.Date AS EventDate, VA.Status AS ApplicationStatus
                FROM Volunteers V
                INNER JOIN Users U ON V.UserID = U.UserID
                LEFT JOIN Events E ON V.EventID = E.EventID
                LEFT JOIN VolunteerApplications VA ON V.UserID = VA.UserID AND VA.EventID = V.EventID
                WHERE (@StartDate IS NULL OR E.Date >= @StartDate OR E.Date IS NULL)
                  AND (@EndDate IS NULL OR E.Date <= @EndDate OR E.Date IS NULL)
                ORDER BY E.Date DESC";

            return GetDataTable(query, startDate, endDate);
        }

        private DataTable GetDataTable(string query, DateTime? startDate, DateTime? endDate)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@StartDate", (object)startDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@EndDate", (object)endDate ?? DBNull.Value);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        private void ExportReport(DataTable dt, string filename, string sheetName)
        {
            if (dt.Rows.Count == 0)
            {
                ShowError($"No {sheetName} data found for the selected date range.");
                return;
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(sheetName);
                ws.Cell(1, 1).InsertTable(dt);
                ws.Columns().AdjustToContents();

                SendExcelToClient(wb, filename);
            }
        }

        private DateTime? ParseDate(string dateText)
        {
            if (DateTime.TryParse(dateText, out DateTime dt))
                return dt.Date;
            return null;
        }

        private void SendExcelToClient(XLWorkbook workbook, string filename)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", $"attachment;filename={filename}");

            using (var ms = new MemoryStream())
            {
                workbook.SaveAs(ms);
                ms.WriteTo(Response.OutputStream);
                ms.Close();
            }
            Response.Flush();
            Response.End();
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }
    }
}