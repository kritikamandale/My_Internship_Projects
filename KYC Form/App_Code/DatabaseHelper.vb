Imports System
Imports System.Configuration
Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls

Public Class DatabaseHelper

    ' Get connection string from Web.config
    Private Shared Function GetConnectionString() As String
        Return ConfigurationManager.ConnectionStrings("KYCConnection").ConnectionString
    End Function

    ' Bind dropdown list from database
    Public Shared Sub BindDropDown(ByVal ddl As DropDownList, ByVal query As String, ByVal textField As String, ByVal valueField As String)
        Using conn As New SqlConnection(GetConnectionString())
            Using cmd As New SqlCommand(query, conn)
                Using sda As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    sda.Fill(dt)

                    ddl.DataSource = dt
                    ddl.DataTextField = textField
                    ddl.DataValueField = valueField
                    ddl.DataBind()

                    ' Add default "Select" option at the beginning
                    ddl.Items.Insert(0, New ListItem("-- Select --", ""))
                End Using
            End Using
        End Using
    End Sub

    ' Execute non-query (INSERT, UPDATE, DELETE)
    Public Shared Function ExecuteNonQuery(ByVal query As String, Optional ByVal parameters() As SqlParameter = Nothing) As Integer
        Using conn As New SqlConnection(GetConnectionString())
            Using cmd As New SqlCommand(query, conn)
                If parameters IsNot Nothing Then
                    cmd.Parameters.AddRange(parameters)
                End If

                conn.Open()
                Return cmd.ExecuteNonQuery()
            End Using
        End Using
    End Function

    ' Execute scalar (get single value)
    Public Shared Function ExecuteScalar(ByVal query As String, Optional ByVal parameters() As SqlParameter = Nothing) As Object
        Using conn As New SqlConnection(GetConnectionString())
            Using cmd As New SqlCommand(query, conn)
                If parameters IsNot Nothing Then
                    cmd.Parameters.AddRange(parameters)
                End If

                conn.Open()
                Return cmd.ExecuteScalar()
            End Using
        End Using
    End Function

    ' Execute reader and return DataTable
    Public Shared Function ExecuteQuery(ByVal query As String, Optional ByVal parameters() As SqlParameter = Nothing) As DataTable
        Using conn As New SqlConnection(GetConnectionString())
            Using cmd As New SqlCommand(query, conn)
                If parameters IsNot Nothing Then
                    cmd.Parameters.AddRange(parameters)
                End If

                Using sda As New SqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    sda.Fill(dt)
                    Return dt
                End Using
            End Using
        End Using
    End Function

    ' Helper method to create SQL parameter
    Public Shared Function CreateParameter(ByVal name As String, ByVal value As Object) As SqlParameter
        If value Is Nothing Then
            value = DBNull.Value
        End If
        Return New SqlParameter(name, value)
    End Function

    ' Helper method to get integer value from dropdown (returns null if empty)
    Public Shared Function GetDropDownValue(ByVal ddl As DropDownList) As Integer?
        If String.IsNullOrEmpty(ddl.SelectedValue) Then
            Return Nothing
        End If
        Return Convert.ToInt32(ddl.SelectedValue)
    End Function

    ' Helper method to get string value safely
    Public Shared Function GetStringValue(ByVal value As String) As String
        If String.IsNullOrWhiteSpace(value) Then
            Return Nothing
        Else
            Return value.Trim()
        End If
    End Function

End Class
