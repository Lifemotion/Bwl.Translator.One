Imports System.Text

Public Class TestForm

    Private Sub run_Click(sender As System.Object, e As System.EventArgs) Handles run.Click
        Dim interpreter As New One.Interpreter.Interpreter
        Dim executor As New Executor()
        AddHandler executor.UnknownFunctionCall, AddressOf ExternalCall

        Try
            interpreter.Interprete(source.Text)
            Dim out As New StringBuilder
            Dim prg = interpreter.Instructions
            Dim start = Now
            executor.Run(prg, out)
            result.Text = out.ToString
            result.Text += vbCrLf + (Now - start).TotalMilliseconds.ToString("0.0") + " мсек"
        Catch ex As TranslatorError
            result.Text += "Ошибка " + ex.Position + " " + ex.Text + vbCrLf
            result.Text += ex.Line
        End Try
    End Sub

    Public Sub New()

        ' Этот вызов является обязательным для конструктора.
        InitializeComponent()
        source.Text = IO.File.ReadAllText("test.txt")
        ' Добавьте все инициализирующие действия после вызова InitializeComponent().

    End Sub

    Private Sub TestForm_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub ExternalCall(ByRef found As Boolean, funcName As String, instruction As Instruction, out As StringBuilder)
        If funcName.ToLower = "msgbox" Then MsgBox(instruction.Args(2).Value)
        instruction.Args(1).Value = "3"
        found = True
    End Sub

End Class
