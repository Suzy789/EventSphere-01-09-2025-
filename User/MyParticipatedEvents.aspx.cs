using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Authentication.User
{
    public partial class MyParticipatedEvents : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserID"] == null)
                {
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                LoadParticipatedEvents();

            }
        }

        private void LoadParticipatedEvents()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        e.EventID,
                        e.Title,
                        e.Date,
                        e.Location,
                        pr.IsPresent,
                        CASE 
                            WHEN EXISTS (
                                SELECT 1 FROM Certificates c
                                WHERE c.EventID = e.EventID AND c.UserID = @UserID
                            ) THEN 1 ELSE 0
                        END AS IsCertificateAvailable
                    FROM ParticipantRegistrations pr
                    INNER JOIN Events e ON pr.EventID = e.EventID
                    WHERE pr.UserID = @UserID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    rptParticipatedEvents.DataSource = dt;
                    rptParticipatedEvents.DataBind();
                    lblNoEvents.Visible = (dt.Rows.Count == 0);
                }
            }
        }

        protected void rptParticipatedEvents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var row = (DataRowView)e.Item.DataItem;

                bool isPresent = row["IsPresent"] != DBNull.Value && Convert.ToBoolean(row["IsPresent"]);
                bool isCertAvail = row["IsCertificateAvailable"] != DBNull.Value && Convert.ToBoolean(row["IsCertificateAvailable"]);
                bool datePassed = row["Date"] != DBNull.Value && Convert.ToDateTime(row["Date"]).Date <= DateTime.Today;

                Button btn = (Button)e.Item.FindControl("btnCert");
                btn.Visible = isPresent && isCertAvail && datePassed;
            }
        }

        protected void rptParticipatedEvents_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
                if (e.CommandName == "DownloadCertificate")
                {
                    int eventId = Convert.ToInt32(e.CommandArgument);
                    int userId = Convert.ToInt32(Session["UserID"]);

                    // Redirect to Certificate.aspx (type = participant)
                    Response.Redirect($"~/User/Certificate.aspx?eventId={eventId}&type=participant");
                
            }
        }
    }
}
