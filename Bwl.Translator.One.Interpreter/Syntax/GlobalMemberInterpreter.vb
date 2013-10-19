Partial Class SyntaxAnalyzer
    Private Class GlobalMembersInterpreter
        Public Shared Function NewSSE(ByVal name As String) As StackElement
            Dim elem As New StackElement()
            elem.type = StackElementType.ssName
            elem.name = name
            Return elem
        End Function

        Public Shared Function NewSSE(ByVal name As String, ByVal type As StackElementType) As StackElement
            Dim elem As New StackElement()
            elem.type = type
            elem.name = name
            Return elem
        End Function
    End Class
End Class