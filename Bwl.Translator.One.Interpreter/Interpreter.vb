Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports System.Text

'интерпретатор ПЯ
Public Class Interpreter
    Private _externalFunctions As New FunctionsStorage


    Sub New()
        ExternalPredefinedFunctions.CopyTo(_externalFunctions)
    End Sub
    Private _syntaxAnalyzer As New SyntaxAnalyzer(_externalFunctions)

    'добавляем и транслируем строку программы
    Public Sub InterpreteLine(ByVal line As String, debugLineNumber As Integer)
        Dim lexems = LexicAnalyzer.AnalyzeLine(line)
        _syntaxAnalyzer.AddCodeLine(lexems, debugLineNumber)
    End Sub

    'добавляем и транслируем программу
    Public Sub Interprete(ByVal text As String)
        _syntaxAnalyzer.UnknownnExternalsAllowed = True
        Dim lines = Split(text, vbCrLf)
        Dim i = 0
        For Each line In lines
            i += 1
            Try
                InterpreteLine(line, i)
            Catch ex As TranslateTimeError
                ex.LineNumber = i
                ex.Line = line
                Throw ex
            End Try
        Next
    End Sub

    Public ReadOnly Property Instructions As Instruction()
        Get
            Return _syntaxAnalyzer.Instructions
        End Get
    End Property



End Class







