
''' <summary>
''' Параметр (или результат) функции
''' </summary>
''' <remarks></remarks>
Public Class Parameter1
    Public Property Comment As String = ""
    Public Property Type As ParameterType1
    Public Property VariableIndex As Integer
    Public Property Value As New Value

    Public Function Clone() As Parameter1
        Dim res As New Parameter1
        res.Comment = Comment.Clone
        res.Type = Type
        res.VariableIndex = VariableIndex
        res.Value = New Value(Value)
        Return res
    End Function

    Public Overrides Function ToString() As String
        Dim result As String = ""
        If Comment.Length > 0 Then result += Comment + ":"
        If Type = ParameterType.typeValue Then
            result += Value.ToString
        End If
        If Type = ParameterType.typeVariable Then
            result += "[" + VariableIndex.ToString + "]"
        End If
        If Type = ParameterType.typeNone Then
            result += "!!!"
        End If
        Return result
    End Function

End Class