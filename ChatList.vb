Imports System.ComponentModel
Imports System.Configuration
Imports System.Timers
Public Class Chats
    Private ReadOnly contextMenu1 As System.Windows.Forms.ContextMenu
    Friend WithEvents ExitItem As System.Windows.Forms.MenuItem
    Friend WithEvents JustItem As System.Windows.Forms.MenuItem
    Private flag As Boolean = False
    Private ReadOnly sql As New DataBase()
    Dim index As Integer
    Public Sub New()

        ' Этот вызов является обязательным для конструктора.
        InitializeComponent()
        ' Добавить код инициализации после вызова InitializeComponent().
        Me.contextMenu1 = New System.Windows.Forms.ContextMenu
        Me.ExitItem = New System.Windows.Forms.MenuItem
        Me.JustItem = New System.Windows.Forms.MenuItem
        ' Initialize contextMenu1
        Me.contextMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() _
                            {Me.ExitItem, Me.JustItem})

        ' Initialize menuItem1
        Me.ExitItem.Index = 1
        Me.ExitItem.Text = "E&xit"
        Me.JustItem.Index = 0
        Me.JustItem.Text = "О Программе"
        ' The ContextMenu property sets the menu that will
        ' appear when the systray icon is right clicked.
        NotifyIcon1.ContextMenu = Me.contextMenu1

    End Sub
    Protected Friend Shared Function GetSettingByKey(ByVal key As String) As String
        Dim returnValue As String = Nothing
        Dim config As Configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        Dim appSettings As AppSettingsSection = CType(config.GetSection("appSettings"), AppSettingsSection)
        ' If found, return the connection string.
        If Not appSettings.Settings Is Nothing Then
            returnValue = appSettings.Settings(key).Value
        End If
        Return returnValue
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckNewMessages()
        Dim aTimer As Timer
        aTimer = New Timer(5000)
        AddHandler aTimer.Elapsed, AddressOf Me.OnTimer
        aTimer.Start()
    End Sub
    Private Sub NotifyIcon1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotifyIcon1.DoubleClick
        Me.Show()
        Me.WindowState = FormWindowState.Normal
    End Sub
    Private Sub OnTimer(sender As Object, e As Timers.ElapsedEventArgs)
        ' TODO: Insert monitoring activities here.
        Try
            CheckNewMessages()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub
    Public Sub CheckNewMessages()
        Dim isNewMessageExist = sql.CheckNewMessage()
        If isNewMessageExist IsNot Nothing Then
            Dim NewMessageList As List(Of Messages) = sql.GetNewMessages()
            Dim Now1 As DateTime = Now()
            Invoke(Sub()
                       NotifyIcon1.BalloonTipTitle = "Есть не прочитанные сообщения"
                       NotifyIcon1.BalloonTipText = "У Вас " & NewMessageList.Count & " непрочитанных сообщений"
                       NotifyIcon1.ShowBalloonTip(10000)
                   End Sub)
            Invoke(Sub() NotifyIcon1.Icon = New Icon("email.ico"))
            Invoke(Sub() ListBox1.Items.Clear())

            For i As Integer = 0 To NewMessageList.Count - 1
                index = i
                Invoke(Sub() ListBox1.Items.Add(NewMessageList(index)))
            Next
        Else
            Invoke(Sub() NotifyIcon1.Icon = New Icon("skelogo.ico"))
            Invoke(Sub() ListBox1.Items.Clear())
        End If
    End Sub
    Private Sub Form1_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.SizeChanged
        If Me.WindowState = FormWindowState.Minimized Then
            Me.Hide()
        End If
    End Sub
    Private Sub ExitItem_Click(Sender As Object, e As EventArgs) Handles ExitItem.Click
        ' Close the form, which closes the application.
        flag = True
        Me.Close()
    End Sub
    Private Sub JustItem_Click(Sender As Object, e As EventArgs) Handles JustItem.Click
        ' Close the form, which closes the application.
        AboutBox1.Show()
    End Sub
    Protected Overrides Sub OnClosing(e As CancelEventArgs)
        If flag Then
            MyClosingForm(e)
        Else
            e.Cancel = True
            Me.WindowState = FormWindowState.Minimized
        End If
    End Sub
    Protected Sub MyClosingForm(e As CancelEventArgs)
        e.Cancel = False
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged

    End Sub

    Private Sub ListBox1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListBox1.MouseDoubleClick
        If ListBox1.SelectedItem IsNot Nothing Then
            Dim ChatForm As New Chat((ListBox1.SelectedItem).ChatID)
            ChatForm.Show()
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        FullList.Show()
    End Sub

    Private Sub NotifyIcon1_BalloonTipClicked(sender As Object, e As EventArgs) Handles NotifyIcon1.BalloonTipClicked
        Me.Show()
        Me.WindowState = FormWindowState.Normal
    End Sub

End Class
