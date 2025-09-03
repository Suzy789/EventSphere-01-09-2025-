using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

namespace Authentication.User
{
    public partial class ViewApplications : System.Web.UI.Page
    {
        private string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["Role"].ToString() != "Organizer")
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
                LoadMyEvents();
        }

        private void LoadMyEvents()
        {
            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT EventID, Title, Date, Location, Status 
                                 FROM Events WHERE OrganizerID = @UserID 
                                 ORDER BY Date ASC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserID", userId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptMyEvents.DataSource = dt;
                rptMyEvents.DataBind();

                lblNoEvent.Visible = (dt.Rows.Count == 0);
            }
        }

        protected void rptMyEvents_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int eventId = 0;

            if (e.CommandName != "CancelEdit" &&
                e.CommandName != "CancelAddSubCategory")
            {
                // Try parsing safely
                if (!int.TryParse(Convert.ToString(e.CommandArgument), out eventId))
                {
                    // If parsing fails, try from hidden field
                    HiddenField hf = (HiddenField)e.Item.FindControl("hfEditEventId");
                    if (hf != null && !string.IsNullOrEmpty(hf.Value))
                        int.TryParse(hf.Value, out eventId);
                }
            }

            if (e.CommandName == "EditEvent")
            {
                Panel pnlEdit = (Panel)e.Item.FindControl("pnlEditEvent");
                pnlEdit.Visible = true;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "SELECT Title, Date, Location, Status FROM Events WHERE EventID = @EventID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EventID", eventId);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        ((HiddenField)e.Item.FindControl("hfEditEventId")).Value = eventId.ToString();
                        ((TextBox)e.Item.FindControl("txtEditTitle")).Text = dr["Title"].ToString();
                        ((TextBox)e.Item.FindControl("txtEditDate")).Text = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                        ((TextBox)e.Item.FindControl("txtEditLocation")).Text = dr["Location"].ToString();
                        ((DropDownList)e.Item.FindControl("ddlEditStatus")).SelectedValue = dr["Status"].ToString();
                    }
                }
            }
            else if (e.CommandName == "CancelEdit")
            {
                Panel pnlEdit = (Panel)e.Item.FindControl("pnlEditEvent");
                pnlEdit.Visible = false; // no parsing, just hide
            }
           

    else if (e.CommandName == "DeleteEvent")
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "DELETE FROM Events WHERE EventID = @EventID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EventID", eventId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadMyEvents();
            }
            else if (e.CommandName == "ToggleIssues")
            {
                Panel pnlIssues = (Panel)e.Item.FindControl("pnlIssues");
                pnlIssues.Visible = !pnlIssues.Visible;
                if (pnlIssues.Visible) LoadIssues(eventId, e.Item);
            }
            else if (e.CommandName == "ToggleSubCategories")
            {
                Panel pnlSubCategories = (Panel)e.Item.FindControl("pnlSubCategories");
                pnlSubCategories.Visible = !pnlSubCategories.Visible;
                if (pnlSubCategories.Visible) LoadSubCategories(eventId, e.Item);
            }
            else if (e.CommandName == "ShowAddSubCategory")
            {
                eventId = Convert.ToInt32(e.CommandArgument);  // reuse instead of redeclare

                Panel pnlSubCategories = (Panel)e.Item.FindControl("pnlSubCategories");
                Panel pnlAddSubCategory = (Panel)pnlSubCategories.FindControl("pnlAddSubCategory");
                HiddenField hfParentEventId_Add = (HiddenField)pnlAddSubCategory.FindControl("hfParentEventId_Add");

                hfParentEventId_Add.Value = eventId.ToString();
                pnlAddSubCategory.Visible = true;
            }

            else if (e.CommandName == "SaveSubCategory")
            {
                Panel pnlAddSubCategory = (Panel)e.Item.FindControl("pnlAddSubCategory");
                TextBox txtNewSubCatName = (TextBox)pnlAddSubCategory.FindControl("txtNewSubCatName");
                TextBox txtNewTeamSize = (TextBox)pnlAddSubCategory.FindControl("txtNewTeamSize");
                HiddenField hfParentEventId_Add = (HiddenField)pnlAddSubCategory.FindControl("hfParentEventId_Add");

                if (!int.TryParse(hfParentEventId_Add.Value, out eventId))
                {
                    // show error instead of crashing
                    return;
                }

                int teamSize = 0;
                int.TryParse(txtNewTeamSize.Text, out teamSize);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = @"INSERT INTO ParticipantSubCategories (EventID, SubCategoryName, TeamSize)
                         VALUES (@EventID, @Name, @TeamSize)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@EventID", eventId);
                    cmd.Parameters.AddWithValue("@Name", txtNewSubCatName.Text.Trim());
                    cmd.Parameters.AddWithValue("@TeamSize", teamSize);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                pnlAddSubCategory.Visible = false;

                // Refresh parent repeater so UI updates immediately
                LoadMyEvents();

                // Optionally: reopen the subcategories panel for that event
                Panel pnlSubCategories = (Panel)e.Item.FindControl("pnlSubCategories");
                if (pnlSubCategories != null)
                {
                    pnlSubCategories.Visible = true;
                    LoadSubCategories(eventId, e.Item);
                }

            }

            else if (e.CommandName == "CancelAddSubCategory")
            {
                Panel pnlAddSubCategory = (Panel)e.Item.FindControl("pnlAddSubCategory");
                pnlAddSubCategory.Visible = false;
            }

        }

        private void LoadSubCategories(int eventId, RepeaterItem item)
        {
            Panel pnlSubCategories = (Panel)item.FindControl("pnlSubCategories");
            if (pnlSubCategories == null) return; // safety

            Repeater rptSubCategories = (Repeater)pnlSubCategories.FindControl("rptSubCategories");
            Label lblNoSubCategories = (Label)pnlSubCategories.FindControl("lblNoSubCategories");

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"SELECT SubCategoryID, SubCategoryName, TeamSize
                         FROM ParticipantSubCategories
                         WHERE EventID = @EventID
                         ORDER BY SubCategoryName ASC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EventID", eventId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (rptSubCategories != null)
                {
                    rptSubCategories.DataSource = dt;
                    rptSubCategories.DataBind();
                }

                if (lblNoSubCategories != null)
                {
                    lblNoSubCategories.Visible = (dt.Rows.Count == 0);
                }
            }
        }

        protected void rptSubCategories_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "EditSubCategory")
            {
                int subCategoryId = Convert.ToInt32(e.CommandArgument);

                string query = "SELECT SubCategoryName, TeamSize FROM ParticipantSubCategories WHERE SubCategoryID = @SubCategoryID";
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@SubCategoryID", subCategoryId);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        HiddenField hfEditSubCatId = (HiddenField)e.Item.FindControl("hfEditSubCatId");
                        TextBox txtEditSubCatName = (TextBox)e.Item.FindControl("txtEditSubCatName");
                        TextBox txtEditTeamSize = (TextBox)e.Item.FindControl("txtEditTeamSize");
                        Panel pnlEditSubCategory = (Panel)e.Item.FindControl("pnlEditSubCategory");

                        hfEditSubCatId.Value = subCategoryId.ToString();
                        txtEditSubCatName.Text = dr["SubCategoryName"].ToString();
                        txtEditTeamSize.Text = dr["TeamSize"].ToString();
                        pnlEditSubCategory.Visible = true;
                    }
                }
            }
            else if (e.CommandName == "DeleteSubCategory")
            {
                int subCategoryId = Convert.ToInt32(e.CommandArgument);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "DELETE FROM ParticipantSubCategories WHERE SubCategoryID = @SubCategoryID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@SubCategoryID", subCategoryId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                // Hide the edit panel if visible
                Panel pnlEditSubCategory = (Panel)e.Item.FindControl("pnlEditSubCategory");
                if (pnlEditSubCategory != null)
                {
                    pnlEditSubCategory.Visible = false;
                }

                // First reload parent events (so repeater structure stays consistent)
                LoadMyEvents();

                // Then reload the sub-categories for that parent event
                HiddenField hfParentEventId = (HiddenField)e.Item.NamingContainer.FindControl("hfParentEventId");
                if (hfParentEventId != null)
                {
                    int parentEventId = Convert.ToInt32(hfParentEventId.Value);
                    LoadSubCategories(parentEventId, (RepeaterItem)e.Item.NamingContainer);
                }
            }

            else if (e.CommandName == "UpdateSubCategory")
            {
                HiddenField hfEditSubCatId = (HiddenField)e.Item.FindControl("hfEditSubCatId");
                TextBox txtEditSubCatName = (TextBox)e.Item.FindControl("txtEditSubCatName");
                TextBox txtEditTeamSize = (TextBox)e.Item.FindControl("txtEditTeamSize");

                int subCategoryId = Convert.ToInt32(hfEditSubCatId.Value);

                string query = "UPDATE ParticipantSubCategories SET SubCategoryName=@Name, TeamSize=@TeamSize WHERE SubCategoryID=@SubCategoryID";
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Name", txtEditSubCatName.Text.Trim());
                    cmd.Parameters.AddWithValue("@TeamSize", Convert.ToInt32(txtEditTeamSize.Text));
                    cmd.Parameters.AddWithValue("@SubCategoryID", subCategoryId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                // Hide the edit panel after update
                Panel pnlEditSubCategory = (Panel)e.Item.FindControl("pnlEditSubCategory");
                pnlEditSubCategory.Visible = false;
                LoadMyEvents();

                // Reload using parent EventID
                HiddenField hfParentEventId = (HiddenField)e.Item.NamingContainer.FindControl("hfParentEventId");
                if (hfParentEventId != null)
                {
                    int parentEventId = Convert.ToInt32(hfParentEventId.Value);
                    LoadSubCategories(parentEventId, (RepeaterItem)e.Item.NamingContainer);
                }
            }

            else if (e.CommandName == "CancelSubCategoryEdit")
            {
                Panel pnlEditSubCategory = (Panel)e.Item.FindControl("pnlEditSubCategory");
                pnlEditSubCategory.Visible = false;

                // Reload fresh data to discard edits
                HiddenField hfParentEventId = (HiddenField)e.Item.NamingContainer.FindControl("hfParentEventId");
                if (hfParentEventId != null)
                {
                    int parentEventId = Convert.ToInt32(hfParentEventId.Value);
                    LoadSubCategories(parentEventId, (RepeaterItem)e.Item.NamingContainer);
                }
            }
        }


        private void LoadIssues(int eventId, RepeaterItem item)
        {
            Panel pnlIssues = (Panel)item.FindControl("pnlIssues");
            if (pnlIssues == null) return;

            Repeater rptIssues = (Repeater)pnlIssues.FindControl("rptIssues");
            Label lblNoIssues = (Label)pnlIssues.FindControl("lblNoIssues");

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
    SELECT ai.IssueDescription, 
           ai.ReportedAt, 
           u.FullName AS VolunteerName
    FROM VolunteerAttendanceIssues ai
    INNER JOIN Users u ON ai.VolunteerID = u.UserID
    WHERE ai.EventID = @EventID
    ORDER BY ai.ReportedAt DESC";


                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@EventID", eventId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (rptIssues != null)
                {
                    rptIssues.DataSource = dt;
                    rptIssues.DataBind();
                }

                if (lblNoIssues != null)
                {
                    lblNoIssues.Visible = (dt.Rows.Count == 0);
                }
            }
        }

        private int GetEventIdBySubCategory(int subCategoryId)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT EventID FROM ParticipantSubCategories WHERE SubCategoryID=@SubCategoryID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@SubCategoryID", subCategoryId);
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
}

