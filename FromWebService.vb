Imports System.Xml
Imports System.IO
Imports System.Web
Public Class FromWebService
    Protected Shared wbs As New WebService.Service()
    ''' <summary>
    ''' Возвращает список строк с данными счетов пользователя
    ''' </summary>
    ''' <param name="id">Номер лицевого счета</param>
    ''' <returns></returns>
    Public Function GetConsumer(id As String) As List(Of String)
        Dim i As Integer = 1
        Dim res As List(Of String) = New List(Of String)
        Dim data As String
        Dim str As List(Of String) = New List(Of String)
        Dim quot As String = Chr(34)
        Try
            data = wbs.FindConsumer(id, 11)
            Using reader As XmlReader = XmlReader.Create(New StringReader(data))
                While reader.ReadToFollowing("Data")
                    str.Add(DateString)
                    str.Add(CutName(reader.GetAttribute("Name1")))
                    str.Add(reader.GetAttribute("Address1"))
                    str.Add(" ")
                    While (reader.GetAttribute("idConsumer" & i.ToString)) IsNot Nothing
                        Dim amount As Double = reader.GetAttribute("AmountBalance" & i.ToString)
                        System.Math.Round(amount, 2)
                        str.Add(reader.GetAttribute("AccName" & i.ToString).Replace("&quot;", quot) & ": " & amount.ToString)
                        i += 1
                    End While
                End While
            End Using
        Catch ex As Exception
            str.Add("ErrNumber: " & Err.Number.ToString())
            str.Add("Description: " & Err.Description)
        End Try
        Return str
    End Function
    ''' <summary>
    ''' Скрывает часть ФИО
    ''' </summary>
    ''' <param name="name"></param>
    ''' <returns></returns>
    Public Function CutName(ByVal name As String)
        Dim arr() As String = Split(name)
        Dim sorname As String = ""
        Dim name1 As String = ""
        Dim thirdname As String = ""
        If arr(0) IsNot Nothing Then
            sorname = Left(arr(0), 4) & "*** "
        End If
        If arr(1) IsNot Nothing Then
            name1 = Left(arr(1), 1) & "."
        End If
        If arr(2) IsNot Nothing Then
            thirdname = Left(arr(2), 1) & "."
        End If
        Return sorname & " " & name1 & " " & thirdname
    End Function
End Class
