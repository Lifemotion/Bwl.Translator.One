'значение, хранит тип значения и ссылки на значения разных типов
Public Class Var
    Public Property Type As DataType
    Public Property Value As String = ""
    Public Property Name As String = ""

    Sub New()

    End Sub

    Sub New(v As Var)
        CopyFrom(v)
    End Sub

    Public Sub CopyFrom(v As Var)
        Type = v.Type
        Value = v.Value.Clone
        Name = v.Name.Clone
    End Sub

    Public Function Clone() As Var
        Dim res As New Var(Me)
        Return res
    End Function

    Public Overrides Function ToString() As String
        Dim result As String = ""
        Select Case Type
            Case DataType.dataString
                result += "$" + Value
            Case DataType.dataInteger
                result += "#" + Value
        End Select
        Return result
    End Function
End Class
