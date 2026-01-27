Imports System
Imports System.Data.SqlClient
Imports System.IO
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections.Generic
Imports System.Web.Script.Serialization

Partial Class index
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadDropDowns()
        End If
    End Sub

    Private Sub LoadDropDowns()
        Try
            ' Bind all dropdowns from database
            DatabaseHelper.BindDropDown(ddlAccountType, "SELECT Id, TypeName FROM AccountTypes WHERE IsActive = 1", "TypeName", "Id")
            DatabaseHelper.BindDropDown(ddlCustomerType, "SELECT Id, TypeName FROM CustomerTypes WHERE IsActive = 1", "TypeName", "Id")
            DatabaseHelper.BindDropDown(ddlBranch, "SELECT Id, BranchName FROM Branches WHERE IsActive = 1", "BranchName", "Id")
            DatabaseHelper.BindDropDown(ddlGender, "SELECT Id, GenderName FROM Genders WHERE IsActive = 1", "GenderName", "Id")
            DatabaseHelper.BindDropDown(ddlMaritalStatus, "SELECT Id, StatusName FROM MaritalStatuses WHERE IsActive = 1", "StatusName", "Id")
            DatabaseHelper.BindDropDown(ddlNationality, "SELECT Id, NationalityName FROM Nationalities WHERE IsActive = 1", "NationalityName", "Id")
            DatabaseHelper.BindDropDown(ddlResidentialStatus, "SELECT Id, StatusName FROM ResidentialStatuses WHERE IsActive = 1", "StatusName", "Id")
            DatabaseHelper.BindDropDown(ddlAddressType, "SELECT Id, TypeName FROM AddressTypes WHERE IsActive = 1", "TypeName", "Id")
            DatabaseHelper.BindDropDown(ddlOccupationType, "SELECT Id, TypeName FROM OccupationTypes WHERE IsActive = 1", "TypeName", "Id")
            DatabaseHelper.BindDropDown(ddlIncomeRange, "SELECT Id, RangeText FROM IncomeRanges WHERE IsActive = 1", "RangeText", "Id")
            DatabaseHelper.BindDropDown(ddlFundSource, "SELECT Id, SourceName FROM FundSources WHERE IsActive = 1", "SourceName", "Id")

            ' Check if editing
            If Request.QueryString("id") IsNot Nothing Then
                Dim id As Integer
                If Integer.TryParse(Request.QueryString("id"), id) Then
                    LoadSubmissionData(id)
                End If
            End If
        Catch ex As Exception
            ' Log error or show message
            Response.Write("<script>alert('Error loading dropdowns: " & ex.Message.Replace("'", "\'") & "');</script>")
        End Try
    End Sub

    Private Sub LoadSubmissionData(ByVal id As Integer)
        Try
            Dim query As String = "SELECT * FROM KYCSubmissions WHERE Id = @Id"
            Dim parameters() As SqlParameter = {New SqlParameter("@Id", id)}
            Dim dt As System.Data.DataTable = DatabaseHelper.ExecuteQuery(query, parameters)

            If dt.Rows.Count > 0 Then
                Dim row As System.Data.DataRow = dt.Rows(0)

                ' Set hidden field and button text
                hfSubmissionId.Value = id.ToString()
                btnSubmit.Text = "Update Submission"

                ' Serialize data for JS population
                Dim rowData As New Dictionary(Of String, Object)()
                For Each col As System.Data.DataColumn In dt.Columns
                    rowData.Add(col.ColumnName, row(col))
                Next

                Dim serializer As New JavaScriptSerializer()
                Dim jsonCallback As String = "populateKycForm(" & serializer.Serialize(rowData) & ");"

                ' Register startup script
                ClientScript.RegisterStartupScript(Me.GetType(), "populateForm", jsonCallback, True)
            End If
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine("Error loading submission: " & ex.Message)
        End Try
    End Sub

    Protected Sub SubmitForm(ByVal sender As Object, ByVal e As EventArgs) Handles btnSubmit.Click
        Try
            ' Create uploads folder if it doesn't exist
            Dim uploadsPath As String = Server.MapPath("~/Uploads")
            If Not Directory.Exists(uploadsPath) Then
                Directory.CreateDirectory(uploadsPath)
            End If

            ' Handle file uploads
            Dim aadhaarPath As String = SaveUploadedFile(Request.Files("aadhaarUpload"), uploadsPath)
            Dim panPath As String = SaveUploadedFile(Request.Files("panUpload"), uploadsPath)
            Dim passportDlPath As String = SaveUploadedFile(Request.Files("passportDlUpload"), uploadsPath)
            Dim addressProofPath As String = SaveUploadedFile(Request.Files("addressProofUpload"), uploadsPath)
            Dim signaturePath As String = SaveUploadedFile(Request.Files("signatureUpload"), uploadsPath)

            ' Determine if Update or Insert
            Dim isUpdate As Boolean = Not String.IsNullOrEmpty(hfSubmissionId.Value)
            Dim submissionId As Integer = 0
            If isUpdate Then
                submissionId = Integer.Parse(hfSubmissionId.Value)
            End If

            Dim query As String
            If isUpdate Then
                query = "UPDATE KYCSubmissions SET " &
                        "AccountTypeId = @AccountTypeId, CustomerTypeId = @CustomerTypeId, BranchId = @BranchId, ApplicationDate = @ApplicationDate, " &
                        "Email = @Email, Mobile = @Mobile, AlternateMobile = @AlternateMobile, " &
                        "AadhaarNumber = @AadhaarNumber, AadhaarName = @AadhaarName, AadhaarDob = @AadhaarDob, GenderId = @GenderId, " &
                        "FirstName = @FirstName, MiddleName = @MiddleName, LastName = @LastName, FatherName = @FatherName, MotherName = @MotherName, SpouseName = @SpouseName, MaritalStatusId = @MaritalStatusId, " &
                        "NationalityId = @NationalityId, Religion = @Religion, ResidentialStatusId = @ResidentialStatusId, PlaceOfBirth = @PlaceOfBirth, CountryOfBirth = @CountryOfBirth, " &
                        "Street = @Street, Area = @Area, Location = @Location, PostOffice = @PostOffice, City = @City, State = @State, Country = @Country, Pincode = @Pincode, AddressTypeId = @AddressTypeId, " &
                        "IsPermanentAddressSame = @IsPermanentAddressSame, PermanentAddress = @PermanentAddress, " &
                        "OccupationTypeId = @OccupationTypeId, EmployerName = @EmployerName, NatureOfBusiness = @NatureOfBusiness, Designation = @Designation, IncomeRangeId = @IncomeRangeId, FundSourceId = @FundSourceId, " &
                        "PanNumber = @PanNumber, PanHolderName = @PanHolderName, DlNumber = @DlNumber, DlDob = @DlDob, DlName = @DlName, " &
                        "AadhaarFilePath = ISNULL(@AadhaarFilePath, AadhaarFilePath), " &
                        "PanFilePath = ISNULL(@PanFilePath, PanFilePath), " &
                        "PassportDlFilePath = ISNULL(@PassportDlFilePath, PassportDlFilePath), " &
                        "AddressProofFilePath = ISNULL(@AddressProofFilePath, AddressProofFilePath), " &
                        "SignatureFilePath = ISNULL(@SignatureFilePath, SignatureFilePath) " &
                        "WHERE Id = @Id"
            Else
                query = "INSERT INTO KYCSubmissions (" &
                        "AccountTypeId, CustomerTypeId, BranchId, ApplicationDate, " &
                        "Email, Mobile, AlternateMobile, " &
                        "AadhaarNumber, AadhaarName, AadhaarDob, GenderId, " &
                        "FirstName, MiddleName, LastName, FatherName, MotherName, SpouseName, MaritalStatusId, " &
                        "NationalityId, Religion, ResidentialStatusId, PlaceOfBirth, CountryOfBirth, " &
                        "Street, Area, Location, PostOffice, City, State, Country, Pincode, AddressTypeId, " &
                        "IsPermanentAddressSame, PermanentAddress, " &
                        "OccupationTypeId, EmployerName, NatureOfBusiness, Designation, IncomeRangeId, FundSourceId, " &
                        "PanNumber, PanHolderName, DlNumber, DlDob, DlName, " &
                        "AadhaarFilePath, PanFilePath, PassportDlFilePath, AddressProofFilePath, SignatureFilePath" &
                        ") VALUES (" &
                        "@AccountTypeId, @CustomerTypeId, @BranchId, @ApplicationDate, " &
                        "@Email, @Mobile, @AlternateMobile, " &
                        "@AadhaarNumber, @AadhaarName, @AadhaarDob, @GenderId, " &
                        "@FirstName, @MiddleName, @LastName, @FatherName, @MotherName, @SpouseName, @MaritalStatusId, " &
                        "@NationalityId, @Religion, @ResidentialStatusId, @PlaceOfBirth, @CountryOfBirth, " &
                        "@Street, @Area, @Location, @PostOffice, @City, @State, @Country, @Pincode, @AddressTypeId, " &
                        "@IsPermanentAddressSame, @PermanentAddress, " &
                        "@OccupationTypeId, @EmployerName, @NatureOfBusiness, @Designation, @IncomeRangeId, @FundSourceId, " &
                        "@PanNumber, @PanHolderName, @DlNumber, @DlDob, @DlName, " &
                        "@AadhaarFilePath, @PanFilePath, @PassportDlFilePath, @AddressProofFilePath, @SignatureFilePath" &
                        ")"
            End If

            Dim parameters As New List(Of SqlParameter)
            parameters.Add(DatabaseHelper.CreateParameter("@AccountTypeId", DatabaseHelper.GetDropDownValue(ddlAccountType)))
            parameters.Add(DatabaseHelper.CreateParameter("@CustomerTypeId", DatabaseHelper.GetDropDownValue(ddlCustomerType)))
            parameters.Add(DatabaseHelper.CreateParameter("@BranchId", DatabaseHelper.GetDropDownValue(ddlBranch)))
            parameters.Add(DatabaseHelper.CreateParameter("@ApplicationDate", Request.Form("applicationDate")))
            parameters.Add(DatabaseHelper.CreateParameter("@Email", Request.Form("email")))
            parameters.Add(DatabaseHelper.CreateParameter("@Mobile", Request.Form("mobile")))
            parameters.Add(DatabaseHelper.CreateParameter("@AlternateMobile", Request.Form("alternateMobile")))
            parameters.Add(DatabaseHelper.CreateParameter("@AadhaarNumber", Request.Form("aadhaar")))
            parameters.Add(DatabaseHelper.CreateParameter("@AadhaarName", Request.Form("aadhaarName")))
            parameters.Add(DatabaseHelper.CreateParameter("@AadhaarDob", Request.Form("aadhaarDob")))
            parameters.Add(DatabaseHelper.CreateParameter("@GenderId", DatabaseHelper.GetDropDownValue(ddlGender)))
            parameters.Add(DatabaseHelper.CreateParameter("@FirstName", Request.Form("firstName")))
            parameters.Add(DatabaseHelper.CreateParameter("@MiddleName", Request.Form("middleName")))
            parameters.Add(DatabaseHelper.CreateParameter("@LastName", Request.Form("lastName")))
            parameters.Add(DatabaseHelper.CreateParameter("@FatherName", Request.Form("fatherName")))
            parameters.Add(DatabaseHelper.CreateParameter("@MotherName", Request.Form("motherName")))
            parameters.Add(DatabaseHelper.CreateParameter("@SpouseName", Request.Form("spouseName")))
            parameters.Add(DatabaseHelper.CreateParameter("@MaritalStatusId", DatabaseHelper.GetDropDownValue(ddlMaritalStatus)))
            parameters.Add(DatabaseHelper.CreateParameter("@NationalityId", DatabaseHelper.GetDropDownValue(ddlNationality)))
            parameters.Add(DatabaseHelper.CreateParameter("@Religion", Request.Form("religion")))
            parameters.Add(DatabaseHelper.CreateParameter("@ResidentialStatusId", DatabaseHelper.GetDropDownValue(ddlResidentialStatus)))
            parameters.Add(DatabaseHelper.CreateParameter("@PlaceOfBirth", Request.Form("placeOfBirth")))
            parameters.Add(DatabaseHelper.CreateParameter("@CountryOfBirth", Request.Form("countryOfBirth")))
            parameters.Add(DatabaseHelper.CreateParameter("@Street", Request.Form("street")))
            parameters.Add(DatabaseHelper.CreateParameter("@Area", Request.Form("area")))
            parameters.Add(DatabaseHelper.CreateParameter("@Location", Request.Form("location")))
            parameters.Add(DatabaseHelper.CreateParameter("@PostOffice", Request.Form("postOffice")))
            parameters.Add(DatabaseHelper.CreateParameter("@City", Request.Form("city")))
            parameters.Add(DatabaseHelper.CreateParameter("@State", Request.Form("state")))
            parameters.Add(DatabaseHelper.CreateParameter("@Country", Request.Form("country")))
            parameters.Add(DatabaseHelper.CreateParameter("@Pincode", Request.Form("pincode")))
            parameters.Add(DatabaseHelper.CreateParameter("@AddressTypeId", DatabaseHelper.GetDropDownValue(ddlAddressType)))
            parameters.Add(DatabaseHelper.CreateParameter("@IsPermanentAddressSame", Request.Form("sameAddress") = "yes"))
            parameters.Add(DatabaseHelper.CreateParameter("@PermanentAddress", Request.Form("permanentAddress")))
            parameters.Add(DatabaseHelper.CreateParameter("@OccupationTypeId", DatabaseHelper.GetDropDownValue(ddlOccupationType)))
            parameters.Add(DatabaseHelper.CreateParameter("@EmployerName", Request.Form("employerName")))
            parameters.Add(DatabaseHelper.CreateParameter("@NatureOfBusiness", Request.Form("natureOfBusiness")))
            parameters.Add(DatabaseHelper.CreateParameter("@Designation", Request.Form("designation")))
            parameters.Add(DatabaseHelper.CreateParameter("@IncomeRangeId", DatabaseHelper.GetDropDownValue(ddlIncomeRange)))
            parameters.Add(DatabaseHelper.CreateParameter("@FundSourceId", DatabaseHelper.GetDropDownValue(ddlFundSource)))
            parameters.Add(DatabaseHelper.CreateParameter("@PanNumber", Request.Form("panNumber")))
            parameters.Add(DatabaseHelper.CreateParameter("@PanHolderName", Request.Form("panHolderName")))
            parameters.Add(DatabaseHelper.CreateParameter("@DlNumber", Request.Form("dlNumber")))
            parameters.Add(DatabaseHelper.CreateParameter("@DlDob", Request.Form("dlDob")))
            parameters.Add(DatabaseHelper.CreateParameter("@DlName", Request.Form("dlName")))
            parameters.Add(DatabaseHelper.CreateParameter("@AadhaarFilePath", aadhaarPath))
            parameters.Add(DatabaseHelper.CreateParameter("@PanFilePath", panPath))
            parameters.Add(DatabaseHelper.CreateParameter("@PassportDlFilePath", passportDlPath))
            parameters.Add(DatabaseHelper.CreateParameter("@AddressProofFilePath", addressProofPath))
            parameters.Add(DatabaseHelper.CreateParameter("@SignatureFilePath", signaturePath))

            If isUpdate Then
                parameters.Add(New SqlParameter("@Id", submissionId))
            End If

            ' Execute Query
            Dim result As Integer = DatabaseHelper.ExecuteNonQuery(query, parameters.ToArray())

            If result > 0 Then
                Dim msg As String = If(isUpdate, "Submission updated successfully!", "Form submitted successfully to SQL Server!")
                Response.Write("<script>alert('" & msg & "'); window.location='admin-dashboard.aspx';</script>")
            Else
                Dim action As String = If(isUpdate, "updated", "inserted")
                Response.Write("<script>alert('Error: No rows were " & action & ".');</script>")
            End If
        Catch ex As Exception
            ' Log the full error
            System.Diagnostics.Debug.WriteLine(String.Format("ERROR in SubmitForm: {0}", ex.Message))

            ' Show detailed error to user
            Response.Write("<script>alert('Error: " & ex.Message.Replace("'", "\'") & "');</script>")
        End Try
    End Sub

    Private Function SaveUploadedFile(ByVal file As HttpPostedFile, ByVal uploadsPath As String) As String
        If file IsNot Nothing AndAlso file.ContentLength > 0 Then
            Dim fileName As String = Path.GetFileName(file.FileName)
            Dim uniqueFileName As String = Guid.NewGuid().ToString() & "_" & fileName
            Dim filePath As String = Path.Combine(uploadsPath, uniqueFileName)
            file.SaveAs(filePath)
            Return "~/Uploads/" & uniqueFileName
        End If
        Return Nothing
    End Function

End Class
