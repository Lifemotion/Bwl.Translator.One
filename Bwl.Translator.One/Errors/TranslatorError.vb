Public Class TranslatorError
    Inherits Exception
    Public Property LineNumber As Integer
    Public Property ColumnNumber As Integer
    Public Property Line As String
    Public Property Text As String
    Public ReadOnly Property Position As String
        Get
            Return "(" + LineNumber.ToString + ", " + ColumnNumber.ToString + ")"
        End Get
    End Property
End Class
