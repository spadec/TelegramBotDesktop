Public Class Messages
    Public Property idMessage As Integer
    Public Property MessageText As String
    Public Property ActionTypeID As Integer
    Public Property ChatID As Integer
    Public Property LanguageID As Integer
    Public Property FromLogin As String
    Public Property SmallDate As DateTime
    Public Property ActionDiscription As String
    Public Overrides Function toString() As String
        Return String.Format("[{0}],{1}", FromLogin, SmallDate)
    End Function


End Class
