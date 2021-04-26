Imports System.Configuration
Imports System.Net
Imports Newtonsoft.Json
Public Class SendMessages
    Private ReadOnly BotToken As String
    Private Response As Response
    Public Sub New()
        BotToken = GetSettingByKey("TelegramBotToken")
    End Sub
    Private Shared Function GetSettingByKey(ByVal key As String) As String
        Dim returnValue As String = Nothing
        Dim config As Configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        Dim appSettings As AppSettingsSection = CType(config.GetSection("appSettings"), AppSettingsSection)
        ' If found, return the connection string.
        If Not appSettings.Settings Is Nothing Then
            returnValue = appSettings.Settings(key).Value
        End If
        Return returnValue
    End Function
    Public Function SendMessage(ByVal ChatID As Integer, ByVal MessageText As String) As Boolean
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Dim request As String = "https://api.telegram.org/bot" & BotToken & "/sendMessage?chat_id=" & ChatID & "&" & "text=" & MessageText
        Dim webClient As New System.Net.WebClient
        Dim result As String = webClient.DownloadString(request)
        Dim obj = JsonConvert.DeserializeObject(Of Response)(result)
        If obj.Ok = True Then
            Return True
        End If
        Return False
    End Function
End Class
