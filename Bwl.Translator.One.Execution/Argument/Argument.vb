
''' <summary>
''' Параметр (или результат) функции
''' </summary>
''' <remarks></remarks>
Public Class Argument
    Public Property Comment As String = ""
    Public Property Type As ArgumentType
    Public Property VariableName As String = ""
    Public Property Value As New Value

    Public Function Clone() As Argument
        Dim res As New Argument
        res.Comment = Comment.Clone
        res.VariableName = VariableName.Clone
        res.Type = Type
        res.Value = Value.Clone
        Return res
    End Function

    Public Overrides Function ToString() As String
        Dim result As String = ""
        If Comment.Length > 0 Then result += Comment + ":"
        Select Case Type
            Case ArgumentType.typeNone
                result += "!!!"
            Case ArgumentType.typeVariable
                result += "[" + VariableName.ToString + "]"
            Case ArgumentType.typeValue
                result += Value.ToString
        End Select
        Return result
    End Function

End Class