Partial Class SyntaxAnalyzer
    ''' <summary>
    ''' Состояние синт. анализатора
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum State
        synStart
        synDim
        synDimAs
        synEndLine
        synR
        synEnd
        synSub
        synSubV
        synSubOB
        synSubCB
        synThen
    End Enum
End Class