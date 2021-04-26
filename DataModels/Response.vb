Public Class Response
    Public Property Ok As Boolean
    Public Property result As Result
End Class
Public Class Result
    Public Property message_id As Integer
    Public Property from As From
    Public Property chat As Mchat
    Public Property [Date] As Integer
    Public Property text As String
End Class
Public Class From
    Public Property id As Integer
    Public Property is_bot As Boolean
    Public Property first_name As String
    Public Property username As String
End Class
Public Class Mchat
    Public Property id As Integer
    Public Property first_name As String
    Public Property last_name As String
    Public Property username As String
    Public Property type As String
End Class