Imports Telegram.Bot.Args
Imports Telegram.Bot.Types.Enums
Imports Telegram.Bot.Types.ReplyMarkups
Public Class CallbacksHandlers
    Inherits MyTelegramBot
    Dim sql As New TelegramBotToSQL()
    ''' <summary>
    ''' Показывает главное меню после выбора языка
    ''' </summary>
    ''' <param name="MainTitle">Заголовок меню</param>
    ''' <param name="e">Аргументы событий на действия пользователя</param>
    ''' <param name="MenuTitles">Массив с заголовками кнопок</param>
    Protected Friend Async Sub ShowMainMenu(MainTitle As String, e As CallbackQueryEventArgs, MenuTitles() As String, actions() As String)
        Dim MultiList As New List(Of List(Of InlineKeyboardButton))
        Dim i As Integer
        For i = 0 To MenuTitles.Length - 1
            MultiList.Add(New List(Of InlineKeyboardButton))
            MultiList(i).Add(InlineKeyboardButton.WithCallbackData(MenuTitles(i), actions(i)))
        Next
        Dim menu = New InlineKeyboardMarkup(MultiList)
        Await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, MainTitle, replyMarkup:=menu)
    End Sub
    ''' <summary>
    ''' Коллбэк на действие "Информация к сведению"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Async Sub CallInfo(e As CallbackQueryEventArgs)
        Dim title As String = Nothing,
        ChatId = e.CallbackQuery.From.Id,
        lang = sql.GetChatLang(ChatId)
        Try
            title = sql.GetContent(lang, 6)
            Await Me.Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, title, parseMode:=ParseMode.Markdown)
        Catch ex As Exception

        End Try
        sql.UpdateAction(ChatId, "info")
    End Sub
    ''' <summary>
    ''' Коллбэк на действие "Тарифы"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Sub CallPlans(e As CallbackQueryEventArgs)
        Dim title As String = Nothing,
        ChatId = e.CallbackQuery.From.Id,
        lang = sql.GetChatLang(ChatId),
        menu() As String = Nothing,
        longChatID As Long = e.CallbackQuery.From.Id,
        actions() As String = {"ElectroPlans", "ThermalPlans", lang}
        If lang = "ru" Then
            menu = {"📌 По электрической энергии", "📌 По тепловой энергии", "🔙 Вернутся"}
            title = "Тарифы"
        End If
        If lang = "kz" Then
            menu = {"📌 По электрической энергии", "📌 По тепловой энергии", "🔙 Вернутся"}
            title = "Тарифы"
        End If
        sql.UpdateAction(ChatId, "plans")
        RenderButtonsInColumn(ChatId, title, menu, actions)
    End Sub
    ''' <summary>
    ''' Коллбэк на действие "Тарифы по электрической энергии"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Sub ElectroPlans(e As CallbackQueryEventArgs)
        Dim title As String = Nothing,
        ChatId = e.CallbackQuery.From.Id,
        lang = sql.GetChatLang(ChatId),
        menu() As String = Nothing,
        actions() As String = {"ElectroPlansEntity", "ElectroPlansIndividual", lang}
        If lang = "ru" Then
            menu = {"📌 Юридические лица", "📌 Бытовые потребители", "🔙 Вернутся"}
            title = "Тарифы по электрической энергии"
        End If
        If lang = "kz" Then
            menu = {"📌 Юридические лица", "📌 Бытовые потребители", "🔙 Вернутся"}
            title = "Тарифы по электрической энергии"
        End If
        sql.UpdateAction(ChatId, "ElectroPlans")
        RenderButtonsInColumn(ChatId, title, menu, actions)
    End Sub
    ''' <summary>
    ''' Коллбэк на действие "Тарифы по электроэнергии для юредических лиц"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Async Sub ElectroPlansEntity(e As CallbackQueryEventArgs)
        Dim ChatId = e.CallbackQuery.From.Id,
        lang = sql.GetChatLang(ChatId),
        title As String = Nothing
        sql.UpdateAction(ChatId, "ElectroPlansEntity")
        Try
            title = sql.GetContent(lang, 8)
            Await Me.Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, title, parseMode:=ParseMode.Markdown)
        Catch ex As Exception

        End Try
        ElectroPlans(e)
    End Sub
    ''' <summary>
    ''' Коллбэк на действие "Тарифы по электроэнергии для физических лиц"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Async Sub ElectroPlansIndividual(e As CallbackQueryEventArgs)
        Dim ChatId = e.CallbackQuery.From.Id,
        lang = sql.GetChatLang(ChatId),
        title As String = Nothing
        Try
            title = sql.GetContent(lang, 9)
            Await Me.Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, title, parseMode:=ParseMode.Markdown)
        Catch ex As Exception

        End Try
        sql.UpdateAction(ChatId, "ElectroPlansIndividual")
        ElectroPlans(e)
    End Sub
    ''' <summary>
    ''' Коллбэк на действие "Тарифы по теплоэнергии"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Sub ThermalPlans(e As CallbackQueryEventArgs)
        Dim title As String = Nothing,
        ChatId = e.CallbackQuery.From.Id,
        lang = sql.GetChatLang(ChatId),
        menu() As String = Nothing,
        actions() As String = {"ThermalPlansEntity", "ThermalPlansIndividual", lang}
        If lang = "ru" Then
            menu = {"📌 Юридические лица", "📌 Бытовые потребители", "🔙 Вернутся"}
            title = "Тарифы по тепловой энергии"
        End If
        If lang = "kz" Then
            menu = {"📌 Юридические лица", "📌 Бытовые потребители", "🔙 Вернутся"}
            title = "Тарифы по тепловой энергии"
        End If
        sql.UpdateAction(ChatId, "ThermalPlans")
        RenderButtonsInColumn(ChatId, title, menu, actions)
    End Sub
    ''' <summary>
    ''' Коллбэк на действие "Тарифы по теплоэнергии для Потребителей"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Async Sub ThermalPlansIndividual(e As CallbackQueryEventArgs)
        Dim title As String = Nothing,
        ChatId = e.CallbackQuery.From.Id,
        lang = sql.GetChatLang(ChatId)
        Try
            title = sql.GetContent(lang, 11)
            Await Me.Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, title, parseMode:=ParseMode.Markdown)
        Catch ex As Exception

        End Try
        sql.UpdateAction(ChatId, "ThermalPlansIndividual")
        ThermalPlans(e)
    End Sub
    ''' <summary>
    ''' Коллбэк на действие "Тарифы по теплоэнергии для Юридических лиц"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Async Sub ThermalPlansEntity(e As CallbackQueryEventArgs)
        Dim title As String = Nothing,
       ChatId = e.CallbackQuery.From.Id,
       lang = sql.GetChatLang(ChatId)
        sql.UpdateAction(ChatId, "ThermalPlansEntity")
        Try
            title = sql.GetContent(lang, 12)
            Await Bot.SendTextMessageAsync(ChatId, title, parseMode:=ParseMode.Markdown)
        Catch ex As Exception

        End Try
        ThermalPlans(e)
    End Sub
    ''' <summary>
    ''' Коллбэк на действие "Оставить обращение"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Async Sub CallFeedBack(e As CallbackQueryEventArgs)
        Dim title As String = Nothing,
            ChatId = e.CallbackQuery.From.Id,
            lang = sql.GetChatLang(ChatId)
        sql.UpdateAction(ChatId, "feedback")
        Try
            title = sql.GetContent(lang, 13)
            Await Bot.SendTextMessageAsync(ChatId, title)
        Catch ex As Exception

        End Try
    End Sub
    ''' <summary>
    ''' Коллбэк по кнопке "Оплатить"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Sub CallPay(e As CallbackQueryEventArgs)
        Dim title As String = Nothing,
        ChatId = e.CallbackQuery.From.Id,
        lang = sql.GetChatLang(ChatId),
        menu() As String = Nothing,
        actions() As String = {"KassaPay", "OnlinePay", lang}
        If lang = "ru" Then
            menu = {"🏦 Оплатить в кассах ТОО", "💻 Оплатить онлайн", "🔙 Вернутся"}
            title = "Оплатить"
        End If
        If lang = "kz" Then
            menu = {"🏦 Оплатить в кассах ТОО", "💻 Оплатить онлайн", "🔙 Вернутся"}
            title = "Оплатить"
        End If
        sql.UpdateAction(ChatId, "pay")
        RenderButtonsInColumn(ChatId, title, menu, actions)
    End Sub
    ''' <summary>
    ''' Коллбэк по кнопке "Оплатить в кассах ТОО"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Async Sub KassaPay(e As CallbackQueryEventArgs)
        Dim title As String = Nothing,
        ChatId = e.CallbackQuery.From.Id,
        lang = sql.GetChatLang(ChatId)
        sql.UpdateAction(ChatId, "KassaPay")
        Try
            title = sql.GetContent(lang, 15)
            Await Bot.SendTextMessageAsync(ChatId, title, parseMode:=ParseMode.Markdown)
        Catch ex As Exception

        End Try
        CallPay(e)
    End Sub
    ''' <summary>
    ''' Колбэкк по кнопке "Оплатить Онлайн"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Async Sub OnlinePay(e As CallbackQueryEventArgs)
        Dim title As String = Nothing,
        ChatId = e.CallbackQuery.From.Id,
        lang = sql.GetChatLang(ChatId)
        sql.UpdateAction(ChatId, "OnlinePay")
        Dim providers As List(Of PayProviders)
        providers = sql.GetPayProviders()
        Try
            For i As Integer = 0 To providers.Count - 1
                Dim str As String = "[" & providers(i).Url & "](" & providers(i).Url & ")"
                Await Bot.SendPhotoAsync(ChatId, photo:=providers(i).ImgUrl, caption:=str, parseMode:=ParseMode.Markdown)
            Next
        Catch ex As Exception
            CallPay(e)
        End Try
        CallPay(e)
    End Sub
    ''' <summary>
    ''' Коллбэк на действие "Передать показания"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Async Sub CallTestimony(e As CallbackQueryEventArgs)
        Dim title As String = "",
                    ChatId = e.CallbackQuery.From.Id,
                    lang = sql.GetChatLang(ChatId),
                    TextElectro As String = Nothing,
                    TextButtonGVS As String = Nothing,
                    back As String = Nothing
        If lang = "ru" Then
            title = "Передать показания"
            TextElectro = "💡 электроэнергия"
            TextButtonGVS = "🛁 ГВС"
            back = "🔙 Вернутся"
        End If
        If lang = "kz" Then
            title = "Передать показания"
            TextElectro = "💡 электроэнергия"
            TextButtonGVS = "🛁 ГВС"
            back = "🔙 Артқа"
        End If
        sql.UpdateAction(ChatId, "testimony")
        Dim MultiList As New List(Of List(Of InlineKeyboardButton))
        MultiList.Add(New List(Of InlineKeyboardButton))
        MultiList(0).Add(InlineKeyboardButton.WithCallbackData(TextElectro, "electro"))
        MultiList.Add(New List(Of InlineKeyboardButton))
        MultiList(1).Add(InlineKeyboardButton.WithCallbackData(TextButtonGVS, "GVS"))
        MultiList.Add(New List(Of InlineKeyboardButton))
        MultiList(2).Add(InlineKeyboardButton.WithCallbackData(back, lang))
        Dim InlineKeyboard = New InlineKeyboardMarkup(MultiList)
        Await Me.Bot.SendTextMessageAsync(ChatId, title, replyMarkup:=InlineKeyboard)
    End Sub
    ''' <summary>
    ''' Коллбэк на действие "Передать показания по электроэнергии"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Async Sub ElectroReading(e As CallbackQueryEventArgs)
        Dim ChatId = e.CallbackQuery.From.Id,
            lang = sql.GetChatLang(ChatId)
        sql.UpdateAction(ChatId, "electro")
        Try
            Dim title = sql.GetContent(lang, 17)
            Await Me.Bot.SendTextMessageAsync(ChatId, title)
        Catch ex As Exception

        End Try
    End Sub
    ''' <summary>
    ''' Коллбэк на действие "Передать показания по ГВС"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Async Sub GVSReading(e As CallbackQueryEventArgs)
        Dim ChatId = e.CallbackQuery.From.Id,
        lang = sql.GetChatLang(ChatId)
        sql.UpdateAction(ChatId, "GVS")
        Try
            Dim title = sql.GetContent(lang, 18)
            Await Me.Bot.SendTextMessageAsync(ChatId, title)
        Catch ex As Exception

        End Try
    End Sub
    ''' <summary>
    ''' Коллбэк на действие "Проверить задолженность"
    ''' </summary>
    ''' <param name="e"></param>
    Protected Friend Async Sub CallDebt(e As CallbackQueryEventArgs)
        Dim ChatId = e.CallbackQuery.From.Id,
        lang = sql.GetChatLang(ChatId),
        title As String = ""
        If lang = "ru" Then
            title = "введите номер лицевого счета"
        End If
        If lang = "kz" Then
            title = "жеке шотыңыздың нөмірін енгізіңіз"
        End If
        sql.UpdateAction(ChatId, "debt")
        Await Bot.SendTextMessageAsync(e.CallbackQuery.From.Id, title)
    End Sub
    ''' <summary>
    ''' Кнопки меню в одну колонку
    ''' </summary>
    ''' <param name="e"></param>
    ''' <param name="title"></param>
    ''' <param name="menu"></param>
    ''' <param name="actions"></param>
    Protected Friend Async Sub RenderButtonsInColumn(e As Integer, title As String, menu() As String, actions() As String)
        Dim MultiList As New List(Of List(Of InlineKeyboardButton))
        For i As Integer = 0 To menu.Length - 1
            MultiList.Add(New List(Of InlineKeyboardButton))
            MultiList(i).Add(InlineKeyboardButton.WithCallbackData(menu(i), actions(i)))
        Next
        Dim InlineKeyboard = New InlineKeyboardMarkup(MultiList)
        Await Me.Bot.SendTextMessageAsync(e, title, replyMarkup:=InlineKeyboard)
    End Sub
End Class
