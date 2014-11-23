Public Class LexicAnalyzer
    'состояние КА ЛА
    Private Enum State
        Start
        Verb
        Number
        Str
        Comment
        MathSymbol
    End Enum

    'лексический анализ
    Public Shared Function AnalyzeLine(ByRef line As String) As Lexem()

        Dim _lex As New List(Of Lexem)()
        'реализуем конечный автомат для разбиения на лексемы
        Dim state As State = State.Start
        'перебираем символы
        Dim pos As Integer = 0
        Dim tmp As String = ""
        'строка
        line = line & " "
        Dim lineC As String = String.Copy(line)
        'заготовки лексем
        '   Dim lex As New Lexem()
        '   lex.name = ""
        'ошибки
        Do While pos < line.Length
            Dim accepted As Boolean = False
            'текущий символ
            Dim chr As Char = lineC(pos)
            'перебор таблицы состояний и символов
            Select Case state
                Case State.Start
                    Select Case chr
                        Case " "c, ","c, ControlChars.Tab
                            accepted = True
                        Case "-"c
                            tmp &= chr
                            state = state.Number
                            accepted = True
                        Case "("c
                            tmp = ""
                            Dim lex As New Lexem()
                            lex.Type = LexType.lexOB
                            lex.Name = ""
                            _lex.Add(lex)
                            accepted = True
                        Case "+"c
                            tmp = ""
                            Dim lex As New Lexem()
                            lex.Type = LexType.lexMathOperation
                            lex.Name = "+"
                            _lex.Add(lex)
                            accepted = True
                        Case "-"c
                            tmp = ""
                            Dim lex As New Lexem()
                            lex.Type = LexType.lexMathOperation
                            lex.Name = "-"
                            _lex.Add(lex)
                            accepted = True
                        Case "*"c
                            tmp = ""
                            Dim lex As New Lexem()
                            lex.Type = LexType.lexMathOperation
                            lex.Name = "*"
                            _lex.Add(lex)
                            accepted = True
                        Case "/"c
                            tmp = ""
                            Dim lex As New Lexem()
                            lex.Type = LexType.lexMathOperation
                            lex.Name = "/"
                            _lex.Add(lex)
                            accepted = True

                        Case ")"c
                            tmp = ""
                            Dim lex As New Lexem()
                            lex.Type = LexType.lexCB
                            lex.Name = ""
                            _lex.Add(lex)
                            accepted = True
                        Case "="c
                            tmp = ""
                            Dim lex As New Lexem()
                            lex.Type = LexType.lexEq
                            lex.Name = ""
                            _lex.Add(lex)
                            accepted = True
                        Case """"c
                            tmp = ""
                            state = state.Str
                            accepted = True
                        Case "'"c
                            state = state.Comment
                            accepted = True
                    End Select

                    If IsAbc(chr) Then
                        tmp &= chr
                        state = State.Verb
                        accepted = True
                    End If

                    If IsDigit(chr) Then
                        tmp &= chr
                        state = State.Number
                        accepted = True
                    End If

                Case State.Verb
                    If IsAbc(chr) Then
                        tmp &= chr
                        accepted = True
                    End If
                    If IsDigit(chr) Then
                        tmp &= chr
                        accepted = True
                    End If
                    If IsDelimiter(chr) Then
                        Dim lex As New Lexem()
                        lex.Type = LexType.lexV
                        lex.Name = tmp
                        tmp = ""
                        _lex.Add(lex)
                        state = State.Start
                        pos -= 1
                        accepted = True
                    End If
                    If chr = "," Then
                        pos += 1
                    End If
                    If chr = "'" Then
                        state = State.Comment
                        accepted = True
                    End If
                Case State.Number
                    If IsDigit(chr) Then
                        tmp &= chr
                        accepted = True
                    End If
                    If IsDelimiter(chr) Then
                        Dim lex As New Lexem()
                        lex.Type = LexType.lexN
                        lex.Name = tmp
                        tmp = ""
                        _lex.Add(lex)
                        state = State.Start
                        pos -= 1
                        accepted = True
                    End If
                    If chr = "," Then
                        pos += 1
                    End If
                    If chr = "'" Then
                        state = State.Comment
                        accepted = True
                    End If
                Case State.Str
                    If chr = """" Then
                        Dim lex As New Lexem()
                        lex.Type = LexType.lexS
                        lex.Name = tmp
                        tmp = ""
                        _lex.Add(lex)
                        state = State.Start
                        accepted = True
                        chr = lineC(pos + 1)
                        If chr = "," Then
                            pos += 1
                        End If
                    Else
                        tmp &= chr
                        accepted = True
                    End If
                Case State.Comment
                    accepted = True
            End Select
            If Not accepted Then
                'очередной символ не был принят, прекращаем разбор
                Dim exc As New LogicAnalyzerError
                exc.Line = line
                exc.ColumnNumber = pos + 1
                exc.Text = "Недопустимый символ."
                Throw exc
            End If
            pos += 1
        Loop

        _lex.Add(New Lexem(LexType.lexEndLine))

        'успешное завершение
        For i As Integer = 0 To _lex.Count - 1

            If _lex(i).NameUpper = "DIM" Then
                _lex(i).Type = LexType.lexDim
            End If
            If _lex(i).NameUpper = "AS" Then
                _lex(i).Type = LexType.lexAs
            End If
            If _lex(i).NameUpper = "IF" Then
                _lex(i).Type = LexType.lexIf
            End If
            If _lex(i).NameUpper = "SUB" Then
                _lex(i).Type = LexType.lexSub
            End If
            If _lex(i).NameUpper = "END" Then
                _lex(i).Type = LexType.lexEnd
            End If
            If _lex(i).NameUpper = "INTEGER" Then
                _lex(i).Type = LexType.lexInteger
            End If
            If _lex(i).NameUpper = "STRING" Then
                _lex(i).Type = LexType.lexString
            End If
            If _lex(i).NameUpper = "FUNCTION" Then
                _lex(i).Type = LexType.lexFunction
            End If
            If _lex(i).NameUpper = "WHILE" Then
                _lex(i).Type = LexType.lexWhile
            End If
            If _lex(i).NameUpper = "THEN" Then
                _lex(i).Type = LexType.lexThen
            End If
        Next i

        Return _lex.ToArray

    End Function

    'входит ли символ в множество c символов языка
    Private Shared Function IsAbc(ByVal t As Char) As Boolean
        Select Case t
            Case "a"c To "z"c, "A"c To "Z"c, "_"c, "."c
                Return True
            Case Else
                Return False
        End Select
    End Function

    'является ли символ цифрой
    Private Shared Function IsDigit(ByVal t As Char) As Boolean
        Select Case t
            Case "0"c To "9"c
                Return True
            Case Else
                Return False
        End Select
    End Function

    ' является ли символ разделителем
    Private Shared Function IsDelimiter(ByVal t As Char) As Boolean
        Select Case t
            Case " "c, ","c, ")"c, "("c, "="c, "'"c, "+"c, "-"c, "*"c, "/"c, "\"c
                Return True
            Case Else
                Return False
        End Select
    End Function


End Class
