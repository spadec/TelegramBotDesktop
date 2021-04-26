Public Class FullList
    Public ChatID As Integer
    Public LastMessageID As Integer
    Dim Sql As New DataBase
    Private Sub FullList_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        ListBox1.Items.Clear()
        loadChats(ComboBox1.SelectedIndex)
    End Sub
    Protected Sub loadChats(ByVal period)
        Try
            Dim NewMessageList As List(Of Messages) = Sql.GetChatsByPeriod(period)
            For i As Integer = 0 To NewMessageList.Count - 1
                ListBox1.Items.Add(NewMessageList(i))
            Next
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Private Sub ListBox1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListBox1.MouseDoubleClick
        If ListBox1.SelectedItem IsNot Nothing Then
            Dim ChatForm As New Chat((ListBox1.SelectedItem).ChatID)
            ChatForm.Show()
        End If
    End Sub
End Class