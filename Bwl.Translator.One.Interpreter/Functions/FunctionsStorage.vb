Public Class FunctionsStorage
    Private _items As New List(Of FunctionDescriptor)
    Public Function AddFunction() As Integer
        Dim func As New FunctionDescriptor

        _items.Add(func)
        Return _items.Count - 1
    End Function

    Public Function FindFunction(ByVal name As String) As Integer
        For i As Integer = 0 To _items.Count - 1
            If _items(i).name = name Then
                Return (i)
            End If
        Next i
        Return -1
    End Function

    Public Sub Add(func As FunctionDescriptor)
        _items.Add(func)
    End Sub

    Default Public Property Item(index As Integer) As FunctionDescriptor
        Get
            Return _items(index)
        End Get
        Set(value As FunctionDescriptor)
            _items(index) = value
        End Set
    End Property

End Class
