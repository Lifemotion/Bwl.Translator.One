Imports bwl.Translator.One.Adapter
Public Module Test
    Public Sub Main()
        WriteLine("Test program")
        Dim A, B As Integer
        A = -2
        B = Val("5")
        Dim C, E As Integer
        C = Add(A, B)
        Inc(C)
        Inc(C)
        Dec(C)
        E = Div(Mul(Sb(B, A), 5), 7)
        WriteLine("4=", C)
        Write("3=", Add(A, B))
        WriteLine()
        WriteLine("5=", E)
        Dim D As String
        D = "cat"
        WriteLine("cat=", D)
        Write("dog=")
        If Equal(D, "cat") Then
            Write("do")
        End If
        If No(Equal(D, "catz")) Then
            Write("g")
        End If
        If Equal(D, "cats") Then
            Write("dog")
        End If
        WriteLine()
        Dim f As Integer
        While Lower(f, 10000)
            Inc(f)
        End While
        WriteLine("10000=", f)
    End Sub

End Module
