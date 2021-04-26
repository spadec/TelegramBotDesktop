Imports System.Configuration
Imports System.Data.SqlClient

Public Class DataBase
    ReadOnly sql As New SQL_connection(GetConnectionStringByName("db:TelegramBot"))
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
    ''' <summary>
    ''' Проверяет есть ли новые сообщения в БД
    ''' </summary>
    ''' <returns></returns>
    Public Function CheckNewMessage()
        sql.Connect()
        Dim query As New SqlCommand("select top 1 idMessage from Messages where isNew=1", sql.Connection)

        Dim result As Object = query.ExecuteScalar()
        sql.Connection.Close()
        If result <> Nothing Then
            Return result
        End If
        Return Nothing
    End Function
    ''' <summary>
    ''' Получает список новых сообщений с группировкой по чату
    ''' </summary>
    ''' <returns></returns>
    Public Function GetNewMessages()
        sql.Connect()
        Dim Results As New List(Of Messages)()
        Dim query As New SqlCommand("select MAX(m.FromLogin) as login,MAX(m.Date) as date,m.ChatID from Messages m 
                                    Where m.isNew=1 group by m.ChatID", sql.Connection)
        Dim reader = query.ExecuteReader()
        While reader.Read()
            Dim message As New Messages With {
                .FromLogin = reader.GetString(0),
                .SmallDate = reader.GetDateTime(1),
                .ChatID = reader.GetInt32(2)}
            Results.Add(message)
        End While
        sql.Connection.Close()
        Return Results
    End Function
    ''' <summary>
    ''' Получает список новых сообщений с группировкой по чату
    ''' </summary>
    ''' <returns></returns>
    Public Function GetChatsByPeriod(ByVal period As Integer)
        sql.Connect()
        Dim Results As New List(Of Messages)()
        Dim query As SqlCommand
        Select Case period
            Case 0
                query = New SqlCommand("select MAX(m.FromLogin) as login,MAX(m.Date) as date,m.ChatID from Messages m 
                                    Where m.Date >= dateadd(day,-1,getdate()) group by m.ChatID", sql.Connection)
            Case 1
                query = New SqlCommand("select MAX(m.FromLogin) as login,MAX(m.Date) as date,m.ChatID from Messages m 
                                    Where m.Date >= dateadd(week,-1,getdate()) group by m.ChatID", sql.Connection)
            Case 2
                query = New SqlCommand("select MAX(m.FromLogin) as login,MAX(m.Date) as date,m.ChatID from Messages m 
                                    Where m.Date >= dateadd(month,-1,getdate()) group by m.ChatID", sql.Connection)
            Case 3
                query = New SqlCommand("select MAX(m.FromLogin) as login,MAX(m.Date) as date,m.ChatID from Messages m 
                                    Where m.Date >= dateadd(year,-1,getdate()) group by m.ChatID", sql.Connection)
            Case 4
                query = New SqlCommand("select MAX(m.FromLogin) as login,MAX(m.Date) as date,m.ChatID from Messages m 
                                    Where m.Date >= dateadd(year,-1,getdate()) group by m.ChatID", sql.Connection)
        End Select
        Dim reader = query.ExecuteReader()
        While reader.Read()
            Dim message As New Messages With {
                .FromLogin = reader.GetString(0),
                .SmallDate = reader.GetDateTime(1),
                .ChatID = reader.GetInt32(2)}
            Results.Add(message)
        End While
        sql.Connection.Close()
        Return Results
    End Function
    ''' <summary>
    ''' Получает сообщения чата, потом можно ограничить кол-во сообщений по дате
    ''' Исключим те сообщения которые посылаются автоматическим функциям бота
    ''' </summary>
    ''' <param name="ChatID">Идентификатор Чата от API телеграмма</param>
    ''' <returns></returns>
    Public Function GetMessagesFromChat(ByVal ChatID As Integer)
        sql.Connect()
        Dim query As New SqlCommand("select m.idMessage, m.MessageText,m.Date,m.FromLogin,m.ChatID,a.Description From Messages m 
                                        inner join ActionTypes a on m.ActionTypeID = a.idActionType
                                        Where m.ChatID = @ChatID AND a.idActionType <> 4 ", sql.Connection)
        Dim Results As New List(Of Messages)()
        query.Parameters.AddWithValue("@ChatID", ChatID)
        Dim reader = query.ExecuteReader()
        While reader.Read()
            Dim message As New Messages With {
                .idMessage = reader.GetInt32(0),
                .MessageText = reader.GetString(1),
                .SmallDate = reader.GetDateTime(2),
                .FromLogin = reader.GetString(3),
                .ChatID = reader.GetInt32(4),
                .ActionDiscription = reader.GetString(5)
            }
            Results.Add(message)
        End While
        sql.Connection.Close()
        If Results.Count > 0 Then
            Return Results
        End If
        Return Nothing
    End Function
    ''' <summary>
    ''' Отмечаем что новые сообщения прочитаны
    ''' </summary>
    ''' <param name="ChatID"></param>
    Public Sub UpdatNewChatMessages(ByVal ChatID As Integer)
        sql.Connect()
        Dim query As New SqlCommand("update Messages Set isNew=0 Where ChatID = @id", sql.Connection)
        query.Parameters.AddWithValue("@id", ChatID)
        query.ExecuteNonQuery()
        sql.Connection.Close()
    End Sub
    ''' <summary>
    ''' Записывает ответ оператора на сообщение пользователя
    ''' </summary>
    ''' <param name="MessageID"></param>
    Public Sub SetAnswerByMessage(ByVal MessageText As String, ByVal MessageID As Integer)
        sql.Connect()
        Dim query As New SqlCommand("Insert into Answers (AnswerText,Date,MessageID) Values (@AnswerText,getdate(),@MessageID)", sql.Connection)
        query.Parameters.AddWithValue("@AnswerText", MessageText)
        query.Parameters.AddWithValue("@MessageID", MessageID)
        query.ExecuteNonQuery()
        sql.Connection.Close()
    End Sub
    ''' <summary>
    ''' Список ответов оператора на сообщение пользователя
    ''' </summary>
    ''' <param name="MessageID"></param>
    ''' <returns></returns>
    Public Function GetAnswersByMessage(ByVal MessageID As Integer) As List(Of Answers)
        sql.Connect()
        Dim Results As New List(Of Answers)()
        Dim query As New SqlCommand("Select * From Answers Where MessageID=@MessageID", sql.Connection)
        query.Parameters.AddWithValue("@MessageID", MessageID)
        Dim reader = query.ExecuteReader()
        While reader.Read()
            Dim Answer As New Answers With {
                .idAnswer = reader.GetInt32(0),
                .AnswerText = reader.GetString(1),
                .SmallDate = reader.GetDateTime(2),
                .MessageID = reader.GetInt32(3)
            }
            Results.Add(Answer)
        End While
        sql.Connection.Close()
        Return Results
    End Function
    Public Function CheckNewMessagesFromChat(ByVal ChatID As Integer)
        sql.Connect()
        Dim query As New SqlCommand("select top 1 idMessage from Messages where isNew=1 AND ChatID=@ChatID", sql.Connection)
        query.Parameters.AddWithValue("@ChatID", ChatID)
        Dim result As Object = query.ExecuteScalar()
        sql.Connection.Close()
        If result <> Nothing Then
            Return result
        End If
        Return Nothing
    End Function
End Class
