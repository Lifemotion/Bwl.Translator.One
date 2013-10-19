
''' <summary>
''' Параметр (или результат) функции
''' </summary>
''' <remarks></remarks>
Public Class Argument
    Public Property Comment As String = ""
    Public Property Type As DataType
    Public Property IsPointer As Boolean
    Public Property Value As String = ""

    Public Function Clone() As Argument
        Dim res As New Argument
        res.Comment = Comment.Clone
        res.Type = Type
        res.IsPointer = IsPointer
        res.Value = Value.Clone
        Return res
    End Function

    Public Overrides Function ToString() As String
        Dim result As String = ""
        If Comment.Length > 0 Then result += Comment + ":"
        If IsPointer Then result += "["
        Select Case Type
            Case DataType.dataInteger
                result += "#" + Value.ToString
            Case DataType.dataString
                result += "'" + Value.ToString + "'"
        End Select
        If IsPointer Then result += "}"
        Return result
    End Function

End Class