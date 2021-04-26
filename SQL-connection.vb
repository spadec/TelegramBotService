Imports System.Data.SqlClient
Public Class SQL_connection
    Public Connection As New SqlConnection
    Public Sub New(ByVal ConnString As String)
        Connection.ConnectionString = ConnString
    End Sub
    Public Function Connect() As Boolean
        Try
            With Connection
                If .State <> System.Data.ConnectionState.Open Then .Open()
                Return (.State = System.Data.ConnectionState.Open)
            End With
        Catch ex As Exception
            Return False
        End Try
    End Function
End Class