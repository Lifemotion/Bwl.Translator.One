'тип данных
Public Enum PossibleDataType
    pdtAny
    pdtInteger
    pdtString
End Enum
Public Class DataTypeConverter
    Shared Function ToFD(v As DataType) As PossibleDataType
        Select Case v
            Case DataType.dataInteger
                Return PossibleDataType.pdtInteger
            Case DataType.dataString
                Return PossibleDataType.pdtString
            Case Else
                Throw New Exception
        End Select
    End Function

    Shared Function FromFD(v As PossibleDataType) As DataType
        Select Case v
            Case PossibleDataType.pdtInteger
                Return DataType.dataInteger
            Case PossibleDataType.pdtString
                Return DataType.dataString
            Case Else
                Throw New Exception
        End Select
    End Function

End Class