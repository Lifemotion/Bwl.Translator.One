
''' <summary>
''' Параметр (или результат) функции
''' </summary>
''' <remarks></remarks>
Public Class FunctionArgument
    Public Property Comment As String = ""
    Public Property Type As PossibleDataType

    Public Function Clone() As FunctionArgument
        Dim res As New FunctionArgument
        res.Comment = String.Copy(Comment)
        res.Type = Type
        Return res
    End Function

    Public Overrides Function ToString() As String
        Dim result As String = ""
        If Comment.Length > 0 Then result += Comment
        Return result
    End Function

End Class