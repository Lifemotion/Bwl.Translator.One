'значение, хранит тип значения и ссылки на значения разных типов
Public Class VarDescriptor
    Public Property Type As DataType
    Public Property Name As String = ""

    Sub New()

    End Sub

    Sub New(v As VarDescriptor)
        CopyFrom(v)
    End Sub

    Public Sub CopyFrom(v As VarDescriptor)
        Type = v.Type
        Name = String.Copy(v.Name)
    End Sub

    Public Function Clone() As VarDescriptor
        Dim res As New VarDescriptor(Me)
        Return res
    End Function


    Public Overrides Function ToString() As String
        Dim result As String = Name + " " + Type.ToString
        Return result
    End Function
End Class
