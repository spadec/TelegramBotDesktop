Imports System.ComponentModel
Imports System.Timers
Public Class Chat
    Public ChatID As Integer
    Public LastMessageID As Integer
    Dim Sql As New DataBase
    Dim aTimer As Timer

    Private Sub Chat_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        RichTextBox1.Clear()
        UpdateChat()
            aTimer = New Timer(5000)
            AddHandler aTimer.Elapsed, AddressOf Me.OnTimer
        aTimer.Start()
    End Sub
    Public Sub New(ByVal ChatID As Integer)

        ' Этот вызов является обязательным для конструктора.
        InitializeComponent()
        Me.ChatID = ChatID
        ' Добавить код инициализации после вызова InitializeComponent().

    End Sub
    Private Sub OnTimer(sender As Object, e As Timers.ElapsedEventArgs)
        ' TODO: Insert monitoring activities here.
        Try
            Invoke(Sub()
                       Dim isNew = Sql.CheckNewMessagesFromChat(ChatID)
                       If isNew IsNot Nothing Then
                           UpdateChat()
                       End If
                   End Sub)

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Public Sub UpdateChat()
        RichTextBox1.Visible = False
        ClearChat()
        Dim Chat As List(Of Messages) = Sql.GetMessagesFromChat(ChatID)
        Me.Text = "Чат с потребителем " & Chat(0).FromLogin
        For i As Integer = 0 To Chat.Count - 1
            Dim str As String = ""
            str &= "Действие в Боте: " & Chat(i).ActionDiscription & vbCrLf
            str &= Chat(i).FromLogin & " (" & Chat(i).SmallDate.ToString & ")" & vbCrLf
            str &= Chat(i).MessageText & vbCrLf
            'Dim selectionIndex = RichTextBox1.SelectionStart
            'RichTextBox1.Text = RichTextBox1.Text.Insert(selectionIndex, str)
            'RichTextBox1.SelectionStart = selectionIndex + str.Length
            RichTextBox1.SelectionFont = New Font("Arial", 10)
            RichTextBox1.SelectionColor = Color.Blue
            RichTextBox1.SelectedText = str + ControlChars.Cr
            RenderAnswer(Chat(i).idMessage)
            LastMessageID = Chat(i).idMessage
        Next
        RichTextBox1.ScrollToCaret()
        RichTextBox1.Visible = True
    End Sub
    Public Sub RenderAnswer(ByVal MessageID As Integer)
        Dim Answers As List(Of Answers) = Sql.GetAnswersByMessage(MessageID)
        If Answers.Count > 0 Then
            For i As Integer = 0 To Answers.Count - 1
                Dim str As String = ""
                str &= "Мой ответ: " & Answers(i).SmallDate & vbCrLf
                str &= Answers(i).AnswerText & vbCrLf
                RichTextBox1.SelectionFont = New Font("Arial", 8)
                RichTextBox1.SelectionColor = Color.Red
                RichTextBox1.SelectedText = str + ControlChars.Cr
            Next
        Else
        End If
    End Sub
    Public Sub ClearChat()
        RichTextBox1.ResetText()
    End Sub

    Private Sub Send_Click(sender As Object, e As EventArgs) Handles Send.Click
        Dim send As New SendMessages
        Loading.Visible = True
        TextAnswer.Cursor = Cursors.WaitCursor
        Me.Cursor = Cursors.WaitCursor
        Try
            If TextAnswer.Text.Length > 0 Then
                Dim result = send.SendMessage(ChatID, TextAnswer.Text)
                If result = True Then
                    Sql.SetAnswerByMessage(TextAnswer.Text, LastMessageID)
                    UpdateChat()
                    Sql.UpdatNewChatMessages(ChatID)
                    Loading.Visible = False
                    TextAnswer.Cursor = Cursors.Default
                    Me.Cursor = Cursors.Default
                End If
            Else
                MsgBox("Введите ответ")
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        TextAnswer.Text = ""
    End Sub

    Private Sub Chat_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        aTimer.Stop()
    End Sub
End Class