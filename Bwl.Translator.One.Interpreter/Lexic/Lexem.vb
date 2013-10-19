'лексема
Public Class Lexem
    Private _nameUpper, _name As String
    Public Property Type As LexType

    Public Property Name As String
        Set(value As String)
            _name = String.Copy(value)
            _nameUpper = _name.ToUpper
        End Set
        Get
            Return _name
        End Get
    End Property

    Public ReadOnly Property NameUpper As String
        Get
            Return _nameUpper
        End Get
    End Property

    Sub New()

    End Sub

    Public Sub CopyFrom(l As Lexem)
        Type = l.Type
        Name = l.Name
    End Sub

    Sub New(l As Lexem)
        CopyFrom(l)
    End Sub

    Sub New(lexemType As LexType)
        Type = lexemType
    End Sub

    Public Overrides Function ToString() As String
        Return Type.ToString + " " + name
    End Function

End Class