﻿Imports System.ServiceProcess

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TelegramBotService
    Inherits System.ServiceProcess.ServiceBase

    'UserService переопределяет метод Dispose для очистки списка компонентов.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    ' Главная точка входа процесса
    <MTAThread()> _
    <System.Diagnostics.DebuggerNonUserCode()> _
    Shared Sub Main()
        Dim ServicesToRun() As System.ServiceProcess.ServiceBase

        ' В одном процессе может выполняться несколько служб NT. Для добавления
        ' службы в процесс измените следующую строку,
        ' чтобы создавался второй объект службы. Например,
        '
        '   ServicesToRun = New System.ServiceProcess.ServiceBase () {New Service1, New MySecondUserService}
        '
        ServicesToRun = New System.ServiceProcess.ServiceBase() {New TelegramBotService}

        System.ServiceProcess.ServiceBase.Run(ServicesToRun)
    End Sub

    'Является обязательной для конструктора компонентов
    Private components As System.ComponentModel.IContainer

    ' Примечание: следующая процедура является обязательной для конструктора компонентов
    ' Для ее изменения используйте конструктор компонентов.  
    ' Не изменяйте ее в редакторе исходного кода.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.EventLog1 = New System.Diagnostics.EventLog()
        CType(Me.EventLog1, System.ComponentModel.ISupportInitialize).BeginInit()
        '
        'EventLog1
        '
        Me.EventLog1.Log = "TelegramBot"
        '
        'Service1
        '
        Me.ServiceName = "TelegramBotService"
        CType(Me.EventLog1, System.ComponentModel.ISupportInitialize).EndInit()

    End Sub

    Friend WithEvents EventLog1 As EventLog
End Class
