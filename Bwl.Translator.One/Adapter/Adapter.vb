Public Class Adapter
    Public Shared Sub WriteLine(ParamArray args())
        Write(args)
        Console.WriteLine()
    End Sub
    Public Shared Sub Write(ParamArray args())
        For Each arg In args
            Console.Write(arg)
        Next
    End Sub
    Public Shared Function Equal(arg1, arg2)
        If arg1 = arg2 Then Return 1 Else Return 0
    End Function
    Public Shared Function Lower(arg1, arg2)
        If arg1 < arg2 Then Return 1 Else Return 0
    End Function
    Public Shared Function Greater(arg1, arg2)
        If arg1 > arg2 Then Return 1 Else Return 0
    End Function
    Public Shared Function Add(arg1, arg2)
        Return arg1 + arg2
    End Function
    Public Shared Function Sb(arg1, arg2)
        Return arg1 - arg2
    End Function
    Public Shared Function Div(arg1, arg2)
        Return arg1 \ arg2
    End Function
    Public Shared Function Mul(arg1, arg2)
        Return arg1 * arg2
    End Function

    Public Shared Sub Inc(ByRef arg1)
        arg1 += 1
    End Sub

    Public Shared Sub Dec(ByRef arg1)
        arg1 -= 1
    End Sub

    Public Shared Function No(arg1)
        If arg1 = 0 Then Return 1 Else Return -arg1
    End Function


End Class
