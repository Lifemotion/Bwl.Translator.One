Public Class VarDescsStorage
    Private _list As New List(Of VarDescriptor)

    Public Sub Add(item As VarDescriptor)
        _list.Add(item)
    End Sub

    Public Sub Remove(item As VarDescriptor)
        _list.Remove(item)
    End Sub

    Public Function Count() As Integer
        Return _list.Count
    End Function

    Public Function Find(name As String) As VarDescriptor
        For Each item In _list
            If item.Name.ToLower = name.ToLower Then
                Return item
            End If
        Next
        Return Nothing
    End Function

End Class
