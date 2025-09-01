using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using ClosedXML.Excel;
namespace Authentication.User
{
    public partial class DownloadReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Allow both Admin and Organizer to access
                string role = Session["Role"]?.ToString();
                if (string.IsNullOrEmpty(role) || (role != "Admin" && role != "Organizer"))
                {
                    Response.Redirect("~/Login.aspx");
                }
            }
        }

        protected void btnDownloadExcel_Click(object sender, EventArgs e)
        {
            DataTable reportTable = GetEventReportData();

            if (reportTable.Rows.Count > 0)
            {
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add(reportTable, "Event Report");

                    ws.Columns().AdjustToContents();
                    ws.RangeUsed().Style.Font.FontName = "Calibri";
                    ws.RangeUsed().Style.Font.FontSize = 11;

                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=EventReport_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xlsx");

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
                lblMessage.Text = "No data found to generate the report.";
            }
        }

        private DataTable GetEventReportData()
        {
            DataTable dt = new DataTable();
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
   SELECT 
    E.EventID,
    E.Title AS EventTitle,
    E.Date,
    E.Location,
    EC.CategoryName AS Category,
    E.EventMode,
    E.Status,
    (
        SELECT COUNT(*) 
        FROM ParticipantRegistrations PR 
        WHERE PR.EventID = E.EventID
    ) AS TotalParticipants,
    (
        SELECT COUNT(*) 
        FROM VolunteerApplications VA 
        WHERE VA.EventID = E.EventID AND VA.Status = 'Approved'
    ) AS TotalVolunteers,
    (
        SELECT COUNT(*) 
        FROM ParticipantTeamMembers PTM
        INNER JOIN ParticipantSubCategories PSC ON PTM.SubCategoryID = PSC.SubCategoryID
        WHERE PSC.EventID = E.EventID
    ) AS TotalTeamMembers
FROM Events E
LEFT JOIN EventCategories EC ON E.CategoryID = EC.CategoryID
ORDER BY E.Date DESC";


                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            return dt;
        }
    }
}