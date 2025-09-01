using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.IO;

namespace Authentication.User
{
    public partial class MyProfile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (Session["UserID"] == null || Session["Role"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadProfile();
                ShowPanelByRole(Session["Role"].ToString());
            }
        }

        private void ShowPanelByRole(string role)
        {
            pnlOrganizer.Visible = role == "Organizer";
            pnlVolunteer.Visible = role == "Volunteer";
            pnlParticipant.Visible = role == "Participant";

            lblRole.Text = role;
        }

        private void LoadProfile()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            string role = Session["Role"].ToString();
            string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Load common user info
                SqlCommand userCmd = new SqlCommand("SELECT FullName, Email FROM Users WHERE UserID = @UserID", con);
                userCmd.Parameters.AddWithValue("@UserID", userId);
                SqlDataReader userReader = userCmd.ExecuteReader();

                if (userReader.Read())
                {
                    lblFullName.Text = userReader["FullName"].ToString();
                    lblEmail.Text = userReader["Email"].ToString();
                }
                userReader.Close();

                SqlCommand cmd = null;
                SqlDataReader reader = null;

                if (role == "Organizer")
                {
                    cmd = new SqlCommand("SELECT * FROM Organizers WHERE OrganizerID = @UserID", con);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtOrgName.Text = reader["OrgName"]?.ToString();
                        txtOrgType.Text = reader["OrgType"]?.ToString();
                        txtDesignation.Text = reader["Designation"]?.ToString();
                        txtWebsite.Text = reader["OrgWebsite"]?.ToString();
                        txtLinkedIn.Text = reader["LinkedIn"]?.ToString();
                        txtFacebook.Text = reader["Facebook"]?.ToString();
                        txtOfficeAddress.Text = reader["OfficeAddress"]?.ToString();
                        txtCityO.Text = reader["City"]?.ToString();
                        txtStateO.Text = reader["State"]?.ToString();
                        txtCountryO.Text = reader["Country"]?.ToString();
                        txtPinO.Text = reader["PinCode"]?.ToString();
                        txtOrgPhone.Text = reader["OrgPhone"]?.ToString();

                        string selectedEvents = reader["OrgEvents"]?.ToString();
                        if (!string.IsNullOrEmpty(selectedEvents))
                        {
                            foreach (string evt in selectedEvents.Split(','))
                            {
                                ListItem item = cblOrgEvents.Items.FindByText(evt.Trim());
                                if (item != null) item.Selected = true;
                            }
                        }
                    }
                    reader.Close();
                }
                else if (role == "Volunteer")
                {
                    cmd = new SqlCommand("SELECT * FROM VolunteerProfiles WHERE VolunteerID = @UserID", con);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtMobileV.Text = reader["Mobile"]?.ToString();
                        txtDOBV.Text = Convert.ToDateTime(reader["DOB"]).ToString("yyyy-MM-dd");
                        ddlGenderV.SelectedValue = reader["Gender"]?.ToString();
                        txtSkillsV.Text = reader["Skills"]?.ToString();
                        txtLanguagesV.Text = reader["Languages"]?.ToString();
                        txtQualificationV.Text = reader["Qualification"]?.ToString();
                        txtAddressV.Text = reader["CurrentAddress"]?.ToString();
                        txtCityV.Text = reader["City"]?.ToString();
                        txtStateV.Text = reader["State"]?.ToString();
                        txtCountryV.Text = reader["Country"]?.ToString();
                        txtPinV.Text = reader["PinCode"]?.ToString();
                        ddlModeV.SelectedValue = reader["PreferredMode"]?.ToString();

                        string preferred = reader["PreferredEvents"]?.ToString();
                        if (!string.IsNullOrEmpty(preferred))
                        {
                            foreach (string evt in preferred.Split(','))
                            {
                                ListItem item = cblEventsV.Items.FindByText(evt.Trim());
                                if (item != null) item.Selected = true;
                            }
                        }
                    }
                    reader.Close();
                }
                else if (role == "Participant")
                {
                    cmd = new SqlCommand("SELECT * FROM Participants WHERE ParticipantID = @UserID", con);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        txtMobileP.Text = reader["Mobile"]?.ToString();
                        txtDOBP.Text = Convert.ToDateTime(reader["DOB"]).ToString("yyyy-MM-dd");
                        ddlGenderP.SelectedValue = reader["Gender"]?.ToString();
                        txtLanguagesP.Text = reader["Languages"]?.ToString();
                        txtQualificationP.Text = reader["Qualification"]?.ToString();
                        txtAddressP.Text = reader["CurrentAddress"]?.ToString();
                        txtCityP.Text = reader["City"]?.ToString();
                        txtStateP.Text = reader["State"]?.ToString();
                        txtCountryP.Text = reader["Country"]?.ToString();
                        txtPinP.Text = reader["PinCode"]?.ToString();
                        ddlModeP.SelectedValue = reader["PreferredMode"]?.ToString();
                        txtInstitute.Text = reader["InstituteName"]?.ToString();
                        txtCourseDept.Text = reader["CourseDept"]?.ToString();
                        txtYearStudy.Text = reader["YearOfStudy"]?.ToString();
                        string preferred = reader["PreferredEvents"]?.ToString();
                        if (!string.IsNullOrEmpty(preferred))
                        {
                            foreach (string evt in preferred.Split(','))
                            {
                                ListItem item = cblEventsP.Items.FindByText(evt.Trim());
                                if (item != null) item.Selected = true;
                            }
                        }
                    }
                
                    reader.Close();
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            string role = Session["Role"].ToString();
            string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlCommand cmdCheck = new SqlCommand();
                cmdCheck.Connection = con;

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                if (role == "Organizer")
                {
                    string orgLogo = SaveFile(fuOrgLogo);
                    string orgID = SaveFile(fuOrgIDProof);

                    cmdCheck.CommandText = "SELECT COUNT(*) FROM Organizers WHERE OrganizerID=@UserID";
                    cmdCheck.Parameters.AddWithValue("@UserID", userId);
                    int count = (int)cmdCheck.ExecuteScalar();

                    if (count == 0)
                    {
                        // INSERT
                        cmd.CommandText = @"INSERT INTO Organizers (OrganizerID, OrgName, OrgType, Designation, OfficeAddress, City, State, Country, PinCode,
                    OrgPhone, OrgWebsite, LinkedIn, Facebook, OrgEvents, OrgLogoPath, OrgIDProofPath)
                    VALUES (@UserID, @OrgName, @OrgType, @Designation, @OfficeAddress, @City, @State, @Country, @PinCode,
                    @OrgPhone, @OrgWebsite, @LinkedIn, @Facebook, @OrgEvents, @OrgLogoPath, @OrgIDProofPath)";
                    }
                    else
                    {
                        // UPDATE
                        cmd.CommandText = @"UPDATE Organizers SET OrgName=@OrgName, OrgType=@OrgType, Designation=@Designation,
                    OfficeAddress=@OfficeAddress, City=@City, State=@State, Country=@Country, PinCode=@PinCode,
                    OrgPhone=@OrgPhone, OrgWebsite=@OrgWebsite, LinkedIn=@LinkedIn, Facebook=@Facebook,
                    OrgEvents=@OrgEvents, OrgLogoPath=@OrgLogoPath, OrgIDProofPath=@OrgIDProofPath
                    WHERE OrganizerID=@UserID";
                    }

                    cmd.Parameters.AddWithValue("@OrgName", txtOrgName.Text.Trim());
                    cmd.Parameters.AddWithValue("@OrgType", txtOrgType.Text.Trim());
                    cmd.Parameters.AddWithValue("@Designation", txtDesignation.Text.Trim());
                    cmd.Parameters.AddWithValue("@OfficeAddress", txtOfficeAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@City", txtCityO.Text.Trim());
                    cmd.Parameters.AddWithValue("@State", txtStateO.Text.Trim());
                    cmd.Parameters.AddWithValue("@Country", txtCountryO.Text.Trim());
                    cmd.Parameters.AddWithValue("@PinCode", txtPinO.Text.Trim());
                    cmd.Parameters.AddWithValue("@OrgPhone", txtOrgPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@OrgWebsite", txtWebsite.Text.Trim());
                    cmd.Parameters.AddWithValue("@LinkedIn", txtLinkedIn.Text.Trim());
                    cmd.Parameters.AddWithValue("@Facebook", txtFacebook.Text.Trim());
                    cmd.Parameters.AddWithValue("@OrgEvents", string.Join(",", cblOrgEvents.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Text)));
                    cmd.Parameters.AddWithValue("@OrgLogoPath", orgLogo);
                    cmd.Parameters.AddWithValue("@OrgIDProofPath", orgID);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                }
                else if (role == "Volunteer")
                {
                    string idProof = SaveFile(fuIDProofV);

                    cmdCheck.CommandText = "SELECT COUNT(*) FROM VolunteerProfiles WHERE VolunteerID=@UserID";
                    cmdCheck.Parameters.Clear();
                    cmdCheck.Parameters.AddWithValue("@UserID", userId);
                    int count = (int)cmdCheck.ExecuteScalar();

                    if (count == 0)
                    {
                        cmd.CommandText = @"INSERT INTO VolunteerProfiles (VolunteerID, Mobile, DOB, Gender, Skills, Languages, Qualification, CurrentAddress, City,
                    State, Country, PinCode, PreferredMode, PreferredEvents, IDProofPath)
                    VALUES (@UserID, @Mobile, @DOB, @Gender, @Skills, @Languages, @Qualification, @CurrentAddress, @City,
                    @State, @Country, @PinCode, @PreferredMode, @PreferredEvents, @IDProofPath)";
                    }
                    else
                    {
                        cmd.CommandText = @"UPDATE VolunteerProfiles SET Mobile=@Mobile, DOB=@DOB, Gender=@Gender, Skills=@Skills,
                    Languages=@Languages, Qualification=@Qualification, CurrentAddress=@CurrentAddress, City=@City,
                    State=@State, Country=@Country, PinCode=@PinCode, PreferredMode=@PreferredMode,
                    PreferredEvents=@PreferredEvents, IDProofPath=@IDProofPath
                    WHERE VolunteerID=@UserID";
                    }

                    cmd.Parameters.AddWithValue("@Mobile", txtMobileV.Text.Trim());
                    cmd.Parameters.AddWithValue("@DOB", txtDOBV.Text.Trim());
                    cmd.Parameters.AddWithValue("@Gender", ddlGenderV.SelectedValue);
                    cmd.Parameters.AddWithValue("@Skills", txtSkillsV.Text.Trim());
                    cmd.Parameters.AddWithValue("@Languages", txtLanguagesV.Text.Trim());
                    cmd.Parameters.AddWithValue("@Qualification", txtQualificationV.Text.Trim());
                    cmd.Parameters.AddWithValue("@CurrentAddress", txtAddressV.Text.Trim());
                    cmd.Parameters.AddWithValue("@City", txtCityV.Text.Trim());
                    cmd.Parameters.AddWithValue("@State", txtStateV.Text.Trim());
                    cmd.Parameters.AddWithValue("@Country", txtCountryV.Text.Trim());
                    cmd.Parameters.AddWithValue("@PinCode", txtPinV.Text.Trim());
                    cmd.Parameters.AddWithValue("@PreferredMode", ddlModeV.SelectedValue);
                    cmd.Parameters.AddWithValue("@PreferredEvents", string.Join(",", cblEventsV.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Text)));
                    cmd.Parameters.AddWithValue("@IDProofPath", idProof);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                }
                else if (role == "Participant")
                {
                    string idProof = SaveFile(fuIDProofP);

                    cmdCheck.CommandText = "SELECT COUNT(*) FROM Participants WHERE ParticipantID=@UserID";
                    cmdCheck.Parameters.Clear();
                    cmdCheck.Parameters.AddWithValue("@UserID", userId);
                    int count = (int)cmdCheck.ExecuteScalar();

                    if (count == 0)
                    {
                        cmd.CommandText = @"INSERT INTO Participants (ParticipantID, Mobile, DOB, Gender, Languages, Qualification, CurrentAddress, City, State, Country,
                    PinCode, PreferredMode, InstituteName, CourseDept, YearOfStudy, PreferredEvents, IDProofPath)
                    VALUES (@UserID, @Mobile, @DOB, @Gender, @Languages, @Qualification, @CurrentAddress, @City, @State, @Country,
                    @PinCode, @PreferredMode, @InstituteName, @CourseDept, @YearOfStudy, @PreferredEvents, @IDProofPath)";
                    }
                    else
                    {
                        cmd.CommandText = @"UPDATE Participants SET Mobile=@Mobile, DOB=@DOB, Gender=@Gender, Languages=@Languages,
                    Qualification=@Qualification, CurrentAddress=@CurrentAddress, City=@City, State=@State, Country=@Country,
                    PinCode=@PinCode, PreferredMode=@PreferredMode, InstituteName=@InstituteName,
                    CourseDept=@CourseDept, YearOfStudy=@YearOfStudy, PreferredEvents=@PreferredEvents, IDProofPath=@IDProofPath
                    WHERE ParticipantID=@UserID";
                    }

                    cmd.Parameters.AddWithValue("@Mobile", txtMobileP.Text.Trim());
                    cmd.Parameters.AddWithValue("@DOB", txtDOBP.Text.Trim());
                    cmd.Parameters.AddWithValue("@Gender", ddlGenderP.SelectedValue);
                    cmd.Parameters.AddWithValue("@Languages", txtLanguagesP.Text.Trim());
                    cmd.Parameters.AddWithValue("@Qualification", txtQualificationP.Text.Trim());
                    cmd.Parameters.AddWithValue("@CurrentAddress", txtAddressP.Text.Trim());
                    cmd.Parameters.AddWithValue("@City", txtCityP.Text.Trim());
                    cmd.Parameters.AddWithValue("@State", txtStateP.Text.Trim());
                    cmd.Parameters.AddWithValue("@Country", txtCountryP.Text.Trim());
                    cmd.Parameters.AddWithValue("@PinCode", txtPinP.Text.Trim());
                    cmd.Parameters.AddWithValue("@PreferredMode", ddlModeP.SelectedValue);
                    cmd.Parameters.AddWithValue("@InstituteName", txtInstitute.Text.Trim());
                    cmd.Parameters.AddWithValue("@CourseDept", txtCourseDept.Text.Trim());
                    cmd.Parameters.AddWithValue("@YearOfStudy", txtYearStudy.Text.Trim());
                    cmd.Parameters.AddWithValue("@PreferredEvents", string.Join(",", cblEventsP.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Text)));
                    cmd.Parameters.AddWithValue("@IDProofPath", idProof);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                }

                int rows = cmd.ExecuteNonQuery();
                if (rows == 0)
                {
                    lblMessage.Text = "Update failed. No data was changed.";
                }
                else
                {
                    lblMessage.Text = "Profile saved successfully.";
                }
                pnlMessage.Visible = true;
            }
        }


        private string SaveFile(FileUpload uploader)
        {
            if (uploader != null && uploader.HasFile)
            {
                string folder = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(uploader.FileName);
                string fullPath = Path.Combine(folder, fileName);
                uploader.SaveAs(fullPath);
                return "~/Uploads/" + fileName;
            }
            return string.Empty;
        }
    }
}