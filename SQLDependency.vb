Imports System.Data.SqlClient
Imports System.Configuration
Public Class MySQLDependency
    Dim connectionString As String
    Dim queueName As String
    Public Sub New(connStr As String, queue As String)
        connectionString = connStr
        queueName = queue
    End Sub
    Public Sub Initialization()
        ' Create a dependency connection.
        SqlDependency.Start(connectionString)
    End Sub

    Sub SomeMethod()
        Dim conn = New SQL_connection(connectionString)
        conn.Connect()
        Dim connection = conn.Connection
        ' Assume connection is an open SqlConnection.
        ' Create a new SqlCommand object.
        Using command As New SqlCommand(
          "SELECT MessageText, FromLogin FROM dbo.Messages",
          connection)

            ' Create a dependency and associate it with the SqlCommand.
            Dim dependency As New SqlDependency(command)
            ' Maintain the refernce in a class member.
            ' Subscribe to the SqlDependency event.
            AddHandler dependency.OnChange, AddressOf OnDependencyChange

            ' Execute the command.
            Using reader = command.ExecuteReader()
                ' Process the DataReader.
            End Using
        End Using
    End Sub

    ' Handler method
    Sub OnDependencyChange(ByVal sender As Object,
        ByVal e As SqlNotificationEventArgs)
        MsgBox(e.Source.ToString)
        ' Handle the event (for example, invalidate this cache entry).
    End Sub

    Sub Termination()
        ' Release the dependency
        SqlDependency.Stop(connectionString)
    End Sub
End Class
