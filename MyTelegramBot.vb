Imports System.Net
Imports System.Configuration
Imports Telegram.Bot
Imports Telegram.Bot.Args
Imports Telegram.Bot.Types
Imports Telegram.Bot.Types.Enums
Imports Telegram.Bot.Types.ReplyMarkups

Public Class MyTelegramBot
    Dim wp As WebProxy = New WebProxy(GetSettingByKey("Proxy"))
    Protected Friend ReadOnly Bot As TelegramBotClient = New TelegramBotClient(GetSettingByKey("TelegramBotToken"), wp) 'token from Telegram
    Public Shared lang As String = "kz" 'по умолчанию казахский 1333211420:AAETlNljgZCoYz1XHIdqbVvhxYUGlllpMBo
    Public Shared actions = {"testimony", "debt", "plans", "pay", "feedback", "info", "init"} 'callback функции на действия пользователя
    Public Sub Main()
        AddHandler Me.Bot.OnMessage, AddressOf BotOnMessageRecived
        AddHandler Me.Bot.OnCallbackQuery, AddressOf BotOnCallbackQueryRecevied
        Me.Bot.StartReceiving()
    End Sub
    Public Sub BotStop()
        Me.Bot.StopReceiving()
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

    ''' <summary>
    ''' Инициализация Бота, меню выбора языков
    ''' </summary>
    ''' <param name="FromId"></param>
    Protected Async Sub InitBot(FromId As Integer)
        Dim startText As String = "Қызмет тілін таңдаңыз: / Выберите язык обслуживания:"
        Dim s1 As String = GetEmojiFlag("kz")
        Dim s2 As String = GetEmojiFlag("ru")
        Dim buttons() = {InlineKeyboardButton.WithCallbackData(s1, "kz"), InlineKeyboardButton.WithCallbackData(s2, "ru")}
        Dim InlineKeyboard = New InlineKeyboardMarkup(buttons)
        Await Me.Bot.SendTextMessageAsync(FromId, startText, replyMarkup:=InlineKeyboard)
    End Sub
    ''' <summary>
    ''' Обработка callback'ов в ответ на действия пользователся
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub BotOnCallbackQueryRecevied(sender As Object, e As CallbackQueryEventArgs)
        Dim CallbackData As String = e.CallbackQuery.Data
        Dim cl As New CallbacksHandlers
        Select Case CallbackData
            Case "kz"
                Dim sql As New TelegramBotToSQL,
                    ChatId = e.CallbackQuery.From.Id
                sql.UpdateChatLang(ChatId, "kz")
                sql.UpdateAction(ChatId, "lang")
                Dim menu = {"⏲ Есептегіштің көрсеткіштерін жіберіңіз", "💰 Қарызды тексеріңіз", "🔖 Тарифтер", "₸ Оплатить", "📨 Оставить обращение", "📑 Информация к сведению", "🔙 Тілді өзгерту"}
                cl.ShowMainMenu("Мәзір пунктін таңдаңыз", e, menu, actions)
            Case "ru"
                Dim sql As New TelegramBotToSQL,
                    ChatId = e.CallbackQuery.From.Id
                sql.UpdateChatLang(ChatId, "ru")
                sql.UpdateAction(ChatId, "lang")
                Dim menu = {"⏲ Передать показания", "💰 Проверить задолженность", "🔖 Тарифы", "₸ Оплатить", "📨 Оставить обращение", "📑 Информация к сведению", "🔙 Сменить язык"}
                cl.ShowMainMenu("Выберите пункт меню", e, menu, actions)
            Case "debt"
                cl.CallDebt(e)
            Case "testimony"
                cl.CallTestimony(e)
            Case "plans"
                cl.CallPlans(e)
            Case "ElectroPlans"
                cl.ElectroPlans(e)
            Case "ElectroPlansEntity"
                Try
                    cl.ElectroPlansEntity(e)
                Catch ex As Exception

                End Try
            Case "ElectroPlansIndividual"
                cl.ElectroPlansIndividual(e)
            Case "ThermalPlans"
                cl.ThermalPlans(e)
            Case "ThermalPlansEntity"
                cl.ThermalPlansEntity(e)
            Case "ThermalPlansIndividual"
                cl.ThermalPlansIndividual(e)
            Case "pay"
                cl.CallPay(e)
            Case "KassaPay"
                cl.KassaPay(e)
            Case "OnlinePay"
                cl.OnlinePay(e)
            Case "feedback"
                cl.CallFeedBack(e)
            Case "info"
                cl.CallInfo(e)
            Case "electro"
                cl.ElectroReading(e)
            Case "GVS"
                cl.GVSReading(e)
            Case "init"
                InitBot(e.CallbackQuery.From.Id)
        End Select
        'Await Bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, buttonText)
    End Sub
    ''' <summary>
    ''' Событие когда бот получает сообщение от пользователя
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Async Sub BotOnMessageRecived(sender As Object, e As MessageEventArgs)
        Dim message = e.Message,
                ChatId = message.From.Id,
                name As String = message.From.FirstName & " " & message.From.LastName
        If message.Type <> MessageType.Text Then
            Return
        End If
        Dim MessageDB As New Messages
        'Console.WriteLine("Пользователь {0} отправил сообщение {1}", name, message.Text)
        Dim sql As New TelegramBotToSQL
        Dim LastAction As String = sql.IsSetChatID(ChatId)
        If message.Text = "/start" Then
            InitBot(message.From.Id)
            If LastAction <> Nothing Then
                sql.InitUpdate(ChatId)
            Else
                sql.SetNewChatID(ChatId, name, "init")
            End If
        Else
            If LastAction <> Nothing Then
                MessageDB.MessageText = message.Text
                MessageDB.ChatID = message.From.Id
                MessageDB.SmallDate = Date.Now
                MessageDB.FromLogin = name
                Dim ids = sql.SetMessageTypeAndLang(message.From.Id)
                MessageDB.ActionTypeID = ids(0)
                MessageDB.LanguageID = ids(1)
                MessageDB.IsNew = 1
                Select Case LastAction
                    Case "init"
                        InitBot(message.From.Id)
                    Case "lang"
                        InitBot(message.From.Id)
                    Case "plans"
                        Dim lang = sql.GetChatLang(ChatId)
                        If lang = "kz" Then
                            Await Me.Bot.SendTextMessageAsync(message.From.Id, "")
                        ElseIf lang = "ru" Then
                            Await Me.Bot.SendTextMessageAsync(message.From.Id, "жоғарыдағы сілтемеден тарифтік сипаттамаларды жүктеп алыңыз")
                        Else
                            Await Me.Bot.SendTextMessageAsync(message.From.Id, "Команда не распознана / Пәрмен танылмады")
                        End If
                    Case "debt"
                        MessageDB.IsNew = 0
                        Dim lang = sql.GetChatLang(ChatId)
                        Dim regexp As New System.Text.RegularExpressions.Regex("\d{8}")
                        Dim str = regexp.IsMatch(message.Text)
                        Dim title As String = Nothing
                        Dim back As String = Nothing
                        Dim response As String = Nothing
                        Dim i As Integer
                        If str = True Then
                            Dim wbs As FromWebService = New FromWebService()
                            Dim data = wbs.GetConsumer(message.Text)
                            If data(0) IsNot Nothing Then
                                For i = 0 To data.Count - 1
                                    response &= data(i) & vbCrLf
                                Next
                                If lang = "kz" Then
                                    back = "🔙 Артқа"
                                End If
                                If lang = "ru" Then
                                    back = "🔙 Вернутся"
                                End If
                            Else
                                If lang = "kz" Then
                                    response = "Табылмады. Жеке кабинетіңізді дұрыс енгізгеніңізді тексеріңіз"
                                    back = "🔙 Артқа"
                                End If
                                If lang = "ru" Then
                                    response = "Не найденно. Проверте правильно ли вы ввели лицевой счет"
                                    back = "🔙 Вернутся"
                                End If
                            End If
                            Dim button() = {InlineKeyboardButton.WithCallbackData(back, lang)}
                            Dim InlineKeyboard = New InlineKeyboardMarkup(button)
                            Await Me.Bot.SendTextMessageAsync(message.From.Id, response, replyMarkup:=InlineKeyboard)
                        Else
                            If lang = "ru" Then
                                title = "Вы не верно ввели лицевой счет, попробуйте еще раз"
                            End If
                            If lang = "kz" Then
                                title = "Жеке кабинетіңізді қате енгіздіңіз, қайталап көріңіз"
                            End If
                            Await Me.Bot.SendTextMessageAsync(message.From.Id, title)
                        End If
                    Case "testimony"
                End Select
                sql.SetMessageInDB(MessageDB)
            End If
        End If
        ' Await Me.Bot.GetUpdatesAsync()
    End Sub
    ''' <summary>
    ''' Кодирует символы флагов емоджи
    ''' </summary>
    ''' <param name="lang">строка идентификатора языка</param>
    ''' <returns></returns>
    Public Function GetEmojiFlag(ByVal lang As String)
        Dim s1 As String = Nothing
        If lang = "ru" Then
            Dim letterC As Integer = &H1F1F7 'r
            Dim letterD As Integer = &H1F1FA 'u
            s1 = [Char].ConvertFromUtf32(letterC) & [Char].ConvertFromUtf32(letterD) & " Русский"
        End If
        If lang = "kz" Then
            Dim letterA As Integer = &H1F1F0 'k
            Dim letterB As Integer = &H1F1FF 'z
            s1 = [Char].ConvertFromUtf32(letterA) & [Char].ConvertFromUtf32(letterB) & " Қазақ"
        End If
        Return s1
    End Function
End Class