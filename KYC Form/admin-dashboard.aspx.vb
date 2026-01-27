Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Collections.Generic
Imports System.Web.Script.Serialization
Imports System.Web.Services
Imports System.Web.Script.Services

Partial Class admin_dashboard
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Try
                Using conn As New SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings("KYCConnection").ConnectionString)
                    ' System.Web.HttpContext.Current.Response.Write("<!-- Connected to: " & conn.DataSource & ", DB: " & conn.Database & " -->")
                End Using
            Catch ex As Exception
                ' System.Web.HttpContext.Current.Response.Write("<!-- Connection Error: " & ex.Message & " -->")
            End Try

            UpdateStats()
        End If
    End Sub

    Private Sub UpdateStats()
        Try
            Dim query As String = "SELECT Count(*) FROM KYCSubmissions"
            Dim dt As DataTable = DatabaseHelper.ExecuteQuery(query)
            If dt.Rows.Count > 0 Then
                ViewState("TotalCount") = dt.Rows(0)(0)
            End If

            Dim todayQuery As String = "SELECT Count(*) FROM KYCSubmissions WHERE CAST(SubmissionDate AS DATE) = CAST(GETDATE() AS DATE)"
            Dim dtToday As DataTable = DatabaseHelper.ExecuteQuery(todayQuery)
            If dtToday.Rows.Count > 0 Then
                ViewState("TodayCount") = dtToday.Rows(0)(0)
            End If
        Catch
        End Try
    End Sub

    Public Function GetSubmissionsJSON() As String
        Try
            Dim query As String = "SELECT k.Id, k.SubmissionDate, k.FirstName, k.MiddleName, k.LastName, k.Email, k.Mobile, k.AadhaarNumber, at.TypeName AS AccountType, k.Status FROM KYCSubmissions k LEFT JOIN AccountTypes at ON k.AccountTypeId = at.Id ORDER BY k.SubmissionDate DESC"
            Dim dt As DataTable = DatabaseHelper.ExecuteQuery(query)

            If dt Is Nothing OrElse dt.Rows.Count = 0 Then
                Return "[]"
            End If

            Dim rows As New List(Of Dictionary(Of String, Object))()

            For Each dr As DataRow In dt.Rows
                Dim row As New Dictionary(Of String, Object)()
                row.Add("id", dr("Id"))
                Dim dateStr As String = ""
                If Not IsDBNull(dr("SubmissionDate")) Then
                    Try
                        dateStr = Convert.ToDateTime(dr("SubmissionDate")).ToString("yyyy-MM-ddTHH:mm:ss")
                    Catch
                    End Try
                End If
                row.Add("submissionDate", dateStr)
                Dim fname As String = If(Not IsDBNull(dr("FirstName")), dr("FirstName").ToString(), "")
                Dim mname As String = If(Not IsDBNull(dr("MiddleName")), dr("MiddleName").ToString(), "")
                Dim lname As String = If(Not IsDBNull(dr("LastName")), dr("LastName").ToString(), "")
                Dim fullName As String = (fname & " " & mname).Trim() & " " & lname
                row.Add("fullName", fullName.Trim())
                row.Add("email", If(Not IsDBNull(dr("Email")), dr("Email").ToString(), ""))
                row.Add("mobile", If(Not IsDBNull(dr("Mobile")), dr("Mobile").ToString(), ""))
                row.Add("aadhaar", If(Not IsDBNull(dr("AadhaarNumber")), dr("AadhaarNumber").ToString(), ""))
                row.Add("accountType", If(Not IsDBNull(dr("AccountType")), dr("AccountType").ToString(), ""))
                row.Add("status", If(Not IsDBNull(dr("Status")), dr("Status").ToString(), "Pending"))
                rows.Add(row)
            Next

            Dim serializer As New JavaScriptSerializer()
            serializer.MaxJsonLength = Integer.MaxValue
            Return serializer.Serialize(rows)
        Catch ex As Exception
            Return "[{""error"": ""Server Error: " & ex.Message.Replace("""", "'") & """}]"
        End Try
    End Function

    Public Function GetTotalCount() As Integer
        Return If(ViewState("TotalCount") IsNot Nothing, CInt(ViewState("TotalCount")), 0)
    End Function

    Public Function GetTodayCount() As Integer
        Return If(ViewState("TodayCount") IsNot Nothing, CInt(ViewState("TodayCount")), 0)
    End Function

    <WebMethod()>
    Public Shared Function UpdateStatus(ByVal id As Integer, ByVal status As String) As String
        Try
            Dim query As String = "UPDATE KYCSubmissions SET Status = @Status WHERE Id = @Id"
            Dim parameters() As SqlParameter = {New SqlParameter("@Status", status), New SqlParameter("@Id", id)}
            DatabaseHelper.ExecuteNonQuery(query, parameters)
            Return "Success"
        Catch ex As Exception
            Return "Error: " & ex.Message
        End Try
    End Function

    <WebMethod()>
    Public Shared Function DeleteSubmission(ByVal id As Integer) As String
        Try
            Dim query As String = "DELETE FROM KYCSubmissions WHERE Id = @Id"
            Dim parameters() As SqlParameter = {New SqlParameter("@Id", id)}
            DatabaseHelper.ExecuteNonQuery(query, parameters)
            Return "Success"
        Catch ex As Exception
            Return "Error: " & ex.Message
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetSubmissionDetails(ByVal id As Integer) As String
        Try
            Dim query As String = "SELECT k.*, at.TypeName as AccountType, ct.TypeName as CustomerType, br.BranchName, g.GenderName, ms.StatusName as MaritalStatus, n.NationalityName as Nationality, rs.StatusName as ResidentialStatus, adt.TypeName as AddressType, ot.TypeName as Occupation, ir.RangeText as IncomeRange, fs.SourceName as FundSource FROM KYCSubmissions k LEFT JOIN AccountTypes at ON k.AccountTypeId = at.Id LEFT JOIN CustomerTypes ct ON k.CustomerTypeId = ct.Id LEFT JOIN Branches br ON k.BranchId = br.Id LEFT JOIN Genders g ON k.GenderId = g.Id LEFT JOIN MaritalStatuses ms ON k.MaritalStatusId = ms.Id LEFT JOIN Nationalities n ON k.NationalityId = n.Id LEFT JOIN ResidentialStatuses rs ON k.ResidentialStatusId = rs.Id LEFT JOIN AddressTypes adt ON k.AddressTypeId = adt.Id LEFT JOIN OccupationTypes ot ON k.OccupationTypeId = ot.Id LEFT JOIN IncomeRanges ir ON k.IncomeRangeId = ir.Id LEFT JOIN FundSources fs ON k.FundSourceId = fs.Id WHERE k.Id = @Id"
            Dim parameters() As SqlParameter = {New SqlParameter("@Id", id)}
            Dim dt As DataTable = DatabaseHelper.ExecuteQuery(query, parameters)

            If dt.Rows.Count > 0 Then
                Dim row As DataRow = dt.Rows(0)
                Dim serializer As New JavaScriptSerializer()
                Dim rowData As New Dictionary(Of String, Object)()
                For Each col As DataColumn In dt.Columns
                    Dim value As Object = row(col)
                    If IsDBNull(value) Then
                        rowData.Add(col.ColumnName, "")
                    ElseIf TypeOf value Is DateTime Then
                        rowData.Add(col.ColumnName, DirectCast(value, DateTime).ToString("yyyy-MM-dd"))
                    Else
                        rowData.Add(col.ColumnName, value.ToString())
                    End If
                Next
                Return serializer.Serialize(rowData)
            End If
            Return "{}"
        Catch ex As Exception
            Return "{""error"": ""Error loading details: " & ex.Message.Replace("""", "'") & """}"
        End Try
    End Function

End Class
