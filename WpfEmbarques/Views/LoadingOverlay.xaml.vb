Imports System.Windows.Threading

Public Class LoadingOverlay
    Private dotCount As Integer = 0
    Private WithEvents timer As DispatcherTimer

    Public Sub New()
        InitializeComponent()
        timer = New DispatcherTimer()
        timer.Interval = TimeSpan.FromMilliseconds(500)
    End Sub

    Public Sub Show(message As String)
        MessageTextBlock.Text = message
        Me.Visibility = Visibility.Visible
        dotCount = 0
        timer.Start()
    End Sub

    Public Sub Hide()
        timer.Stop()
        Me.Visibility = Visibility.Collapsed
    End Sub

    Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles timer.Tick
        dotCount = (dotCount + 1) Mod 4
        Dim dots As String = New String("."c, dotCount)
        MessageTextBlock.Text = MessageTextBlock.Text.Split("."c)(0) & dots
    End Sub
End Class
