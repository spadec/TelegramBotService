Imports System.Data.SqlClient
Imports System.Configuration
Public Class TelegramBotToSQL
    Protected ReadOnly SqlConn As New SQL_connection(GetConnectionStringByName("db:TelegramBot"))
    Protected Shared Function GetConnectionStringByName(ByVal name As String) As String
        Dim returnValue As String = Nothing
        Dim settings As ConnectionStringSettings =
        ConfigurationManager.ConnectionStrings(name)
        ' If found, return the connection string.
        If Not settings Is Nothing Then
            returnValue = settings.ConnectionString
        End If
        Return returnValue
    End Function
    Public Sub SetNewChatID(ByVal ChatID As Integer, ByVal login As String, ByVal LastAction As String)
        Me.SqlConn.Connect()
        Dim query As New SqlCommand("insert into actions (id_chat,login,date,last_action) VALUES (" & ChatID & ",'" & login & "'," & "getdate(),'" & LastAction & "')", Me.SqlConn.Connection)
        query.ExecuteNonQuery()
        Me.SqlConn.Connection.Close()
    End Sub
    Public Sub UpdateChatLang(ByVal ChatID As Integer, ByVal lang As String)
        Me.SqlConn.Connect()
        Dim query As New SqlCommand("UPDATE actions SET chat_lang = @lang WHERE id_chat = @id", Me.SqlConn.Connection)
        query.Parameters.AddWithValue("@id", ChatID)
        query.Parameters.AddWithValue("@lang", lang)
        query.ExecuteNonQuery()
        Me.SqlConn.Connection.Close()
    End Sub
    Public Function IsSetChatID(ByVal ChatID As Integer)
        Me.SqlConn.Connect()
        Dim query As New SqlCommand("select last_action FROM actions WHERE id_chat = @id ", Me.SqlConn.Connection)
        query.Parameters.AddWithValue("@id", ChatID)
        Dim result As Object = query.ExecuteScalar()
        If result <> Nothing Then
            Return result
        End If
        Return Nothing
        Me.SqlConn.Connection.Close()
    End Function
    Public Sub InitUpdate(ByVal ChatID As Integer)
        Me.SqlConn.Connect()
        Dim query As New SqlCommand("UPDATE actions SET date = getdate(),last_action = @action WHERE id_chat = @id", Me.SqlConn.Connection)
        query.Parameters.AddWithValue("@action", "init")
        query.Parameters.AddWithValue("@id", ChatID)
        query.ExecuteNonQuery()
        Me.SqlConn.Connection.Close()
    End Sub
    Public Function GetChatLang(ByVal ChatID As Integer)
        Me.SqlConn.Connect()
        Dim query As New SqlCommand("select chat_lang FROM actions WHERE id_chat = @id ", Me.SqlConn.Connection)
        query.Parameters.AddWithValue("@id", ChatID)
        Dim result As Object = query.ExecuteScalar()
        If result <> Nothing Then
            Return result
        End If
        Return Nothing
        Me.SqlConn.Connection.Close()
    End Function
    Public Sub UpdateAction(ChatID, action)
        Me.SqlConn.Connect()
        Dim query As New SqlCommand("UPDATE actions SET last_action = @action, date = getdate()  WHERE id_chat = @id", Me.SqlConn.Connection)
        query.Parameters.AddWithValue("@id", ChatID)
        query.Parameters.AddWithValue("@action", action)
        query.ExecuteNonQuery()
        Me.SqlConn.Connection.Close()
    End Sub
    Public Function GetPayProviders()
        Me.SqlConn.Connect()
        Dim query As New SqlCommand("select Url,ImgUrl from OnlinePayProviders", Me.SqlConn.Connection)
        Dim results As List(Of PayProviders) = New List(Of PayProviders)()
        Dim reader = query.ExecuteReader()
        While reader.Read()
            Dim providers As New PayProviders
            providers.Url = reader.GetString(0)
            providers.ImgUrl = reader.GetString(1)
            results.Add(providers)
        End While
        Me.SqlConn.Connection.Close()
        Return results
    End Function
    Public Function GetContent(lang As String, ActionTypeID As Integer)
        Me.SqlConn.Connect()
        Dim query As New SqlCommand("Select text FROM Contents c inner join ActionTypes a on c.ActionTypeID=a.idActionType inner join Languages l on c.LanguageID=l.idLanguage WHERE l.ShortName = @lang AND a.idActionType= @ActionTypeID", Me.SqlConn.Connection)
        query.Parameters.AddWithValue("@lang", lang)
        query.Parameters.AddWithValue("@ActionTypeID", ActionTypeID)
        Dim result = query.ExecuteScalar()
        Return result
        Me.SqlConn.Connection.Close()
    End Function
    Public Sub SetMessageInDB(ByVal Message As Messages)
        Me.SqlConn.Connect()
        Dim query As New SqlCommand("Insert Into Messages (ChatID,MessageText,FromLogin,Date,ActionTypeID,LanguageID, isNew) VALUES (@ChatID,@MessageText,@FromLogin,getdate(),@ActionTypeID,@LanguageID, @IsNew)", Me.SqlConn.Connection)
        query.Parameters.AddWithValue("@MessageText", Message.MessageText)
        query.Parameters.AddWithValue("@FromLogin", Message.FromLogin)
        query.Parameters.AddWithValue("@ChatID", Message.ChatID)
        query.Parameters.AddWithValue("@ActionTypeID", Message.ActionTypeID)
        query.Parameters.AddWithValue("@LanguageID", Message.LanguageID)
        query.Parameters.AddWithValue("@IsNew", Message.IsNew)

        query.ExecuteNonQuery()
        Me.SqlConn.Connection.Close()
    End Sub
    Public Function SetMessageTypeAndLang(chatid As Integer)
        Me.SqlConn.Connect()
        Dim query As New SqlCommand("Select t.idActionType, l.idLanguage From actions a inner join ActionTypes t on a.last_action = t.Type inner join Languages l on a.chat_lang = l.ShortName Where a.id_chat = @ChatID", Me.SqlConn.Connection)
        query.Parameters.AddWithValue("@ChatID", chatid)
        Dim reader = query.ExecuteReader()
        Dim results As New List(Of Integer)
        While reader.Read()
            results.Add(reader.GetInt32(0))
            results.Add(reader.GetInt32(1))
        End While
        Me.SqlConn.Connection.Close()
        Return results
    End Function
    Public Sub DisConnect()
        Me.SqlConn.Connection.Close()
    End Sub

End Class