Imports System.Net
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports System.Timers
Public Class TelegramBotService
    Private eventId As Integer = 1
    Private Bot As MyTelegramBot = New MyTelegramBot()
    Public Enum ServiceState
        SERVICE_STOPPED = 1
        SERVICE_START_PENDING = 2
        SERVICE_STOP_PENDING = 3
        SERVICE_RUNNING = 4
        SERVICE_CONTINUE_PENDING = 5
        SERVICE_PAUSE_PENDING = 6
        SERVICE_PAUSED = 7
    End Enum

    <StructLayout(LayoutKind.Sequential)>
    Public Structure ServiceStatus
        Public dwServiceType As Long
        Public dwCurrentState As ServiceState
        Public dwControlsAccepted As Long
        Public dwWin32ExitCode As Long
        Public dwServiceSpecificExitCode As Long
        Public dwCheckPoint As Long
        Public dwWaitHint As Long
    End Structure
    Declare Auto Function SetServiceStatus Lib "advapi32.dll" (ByVal handle As IntPtr, ByRef serviceStatus As ServiceStatus) As Boolean
    ' To access the constructor in Visual Basic, select New from the
    ' method name drop-down list. 
    Public Sub New()
        MyBase.New()
        InitializeComponent()
        Me.EventLog1 = New System.Diagnostics.EventLog
        If Not System.Diagnostics.EventLog.SourceExists("TelegramBot") Then
            System.Diagnostics.EventLog.CreateEventSource("TelegramBot",
        "TelegramBot")
        End If
        EventLog1.Source = "TelegramBot"
        EventLog1.Log = "TelegramBot"
    End Sub
    Private Sub OnTimer(sender As Object, e As Timers.ElapsedEventArgs)
        ' TODO: Insert monitoring activities here.
        Dim webClient As New System.Net.WebClient
        Try
            Dim result As String = webClient.DownloadString("https://api.telegram.org/bot1333211420:AAETlNljgZCoYz1XHIdqbVvhxYUGlllpMBo/getMe")
            EventLog1.WriteEntry("Monitoring the System" & result, EventLogEntryType.Information, eventId)
        Catch ex As Exception
            Dim result As String = ex.Message
            EventLog1.WriteEntry("Monitoring the System" & result, EventLogEntryType.Error, eventId)
        End Try
        eventId = eventId + 1
    End Sub
    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Update the service state to Start Pending.
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
        Dim serviceStatus As ServiceStatus = New ServiceStatus()
        serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING
        serviceStatus.dwWaitHint = 100000
        SetServiceStatus(Me.ServiceHandle, serviceStatus)
        EventLog1.WriteEntry("In OnStart")
        Dim timer As Timer = New Timer()
        timer.Interval = 60000 ' 60 seconds
        AddHandler timer.Elapsed, AddressOf Me.OnTimer
        Bot.Main()
        serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING
        SetServiceStatus(Me.ServiceHandle, serviceStatus)
    End Sub
    Protected Overrides Sub OnContinue()
        'Me.Bot.StartReceiving()
        EventLog1.WriteEntry("In OnContinue.")
    End Sub
    Protected Overrides Sub OnStop()
        'Me.Bot.StopReceiving()
        EventLog1.WriteEntry("In OnStop.")
        Bot.BotStop()
    End Sub
End Class
