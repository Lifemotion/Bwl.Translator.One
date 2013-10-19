Imports System.Text

Public Class Executor

    Private _vars As New ValuesStorage

    Sub New()
      
    End Sub

    Private Sub CallFunction(ByRef out As StringBuilder, ByRef errs As StringBuilder, ByVal instr As Instruction)

    End Sub

    Private Sub CallInternalFunction(ByRef out As StringBuilder, ByRef errs As StringBuilder, ByVal instr As Instruction, funcName As String)
        Try
            Dim funcFound As Boolean
            'WRITE
            If funcName = "WRITE" Or funcName = "WRITELINE" Then
                'возврат - 0
                Dim result = GetVariable(instr.Args(1).VariableName)
                result.Type = DataType.dataInteger
                result.ValueInteger = 0

                'выводим аргументы
                For i As Integer = 2 To (instr.Args.Count) - 1
                    Dim val1 = ResolveToValue(instr.Args(i))
                    out.Append(val1.AsString)
                Next i

                If funcName = "WRITELINE" Then out.AppendLine()
                funcFound = True
            End If

            'АРИФМЕТИЧЕСКИЕ И ЛОГИЧЕСКИЕ ОПЕРАЦИИ
            If (funcName = "ADD") OrElse (funcName = "SB") OrElse (funcName = "MUL") OrElse (funcName = "DIV") OrElse (funcName = "AND") OrElse (funcName = "OR") Then
                funcFound = True
                Dim arg1 As Integer = ResolveToValue(instr.Args(2)).ValueInteger
                Dim arg2 As Integer = ResolveToValue(instr.Args(3)).ValueInteger
                Dim result As Integer
                If funcName = "ADD" Then
                    result = arg1 + arg2
                End If
                If funcName = "SB" Then
                    result = arg1 - arg2
                End If
                If funcName = "MUL" Then
                    result = arg1 * arg2
                End If
                If funcName = "DIV" Then
                    result = arg1 \ arg2
                End If
                If funcName = "AND" Then
                    result = ((arg1 > 0) AndAlso (arg2 > 0))
                End If
                If funcName = "OR" Then
                    result = ((arg1 > 0) OrElse (arg2 > 0))
                End If

                Dim resultValue = GetVariable(instr.Args(1).VariableName)
                resultValue.Type = DataType.dataInteger
                resultValue.ValueInteger = result
            End If

            If (funcName = "EQUAL") OrElse (funcName = "LOWER") OrElse (funcName = "GREATER") Then
                funcFound = True
                Dim val1 = ResolveToValue(instr.Args(2))
                Dim val2 = ResolveToValue(instr.Args(3))

                If val1.Type = val2.Type Then
                    Dim result As Integer = 0
                    If (funcName = "EQUAL") AndAlso val1.AsString = val2.AsString Then
                        result = 1
                    End If
                    If (funcName = "LOWER") AndAlso val1.AsString < val2.AsString Then
                        result = 1
                    End If
                    If (funcName = "GREATER") AndAlso val1.AsString > val2.AsString Then
                        result = 1
                    End If
                    Dim resultValue = GetVariable(instr.Args(1).VariableName)
                    resultValue.Type = DataType.dataInteger
                    resultValue.ValueInteger = result
                Else
                    errs.AppendLine("Сравниваются данные разных типов.")
                End If
            End If

            If (funcName = "INC") OrElse (funcName = "DEC") OrElse (funcName = "NO") Then
                funcFound = True
                Dim result As Integer
                Dim arg1 = ResolveToValue(instr.Args(2))

                If funcName = "INC" Then
                    If instr.Args(2).Type = ArgumentType.typeVariable Then
                        arg1.ValueInteger += 1
                    End If
                    result = arg1.ValueInteger + 1
                End If

                If funcName = "DEC" Then
                    If instr.Args(2).Type = ArgumentType.typeVariable Then
                        arg1.ValueInteger -= 1
                    End If
                    result = arg1.ValueInteger - 1
                End If

                If funcName = "NO" Then
                    Select Case arg1.ValueInteger
                        Case 0
                            result = 1
                        Case Else
                            result = -result
                    End Select
                End If
                Dim resultValue = GetVariable(instr.Args(1).VariableName)
                resultValue.Type = DataType.dataInteger
                resultValue.ValueInteger = result
            End If

            If funcName = "VAL" Then
                funcFound = True
                Dim arg1 = ResolveToValue(instr.Args(2))
                Dim result As Integer = CInt(Val(arg1.AsString))
                Dim resultValue = GetVariable(instr.Args(1).VariableName)
                resultValue.Type = DataType.dataInteger
                resultValue.ValueInteger = result
            End If

            If Not funcFound Then
                RaiseEvent UnknownFunctionCall(funcFound, funcName, instr, out)
                'TODO 1
            End If

            If Not funcFound Then
                Dim exc As New RunTimeError
                exc.Text = "Не найдена функция: " + funcName
            End If

        Catch ex As TranslatorError
            Throw ex
        Catch ex As Exception
            Dim exc As New RunTimeError
            exc.Text = ex.Message
            Throw exc
        End Try
    End Sub

    Public Event UnknownFunctionCall(ByRef found As Boolean, funcName As String, instruction As Instruction, out As StringBuilder)

    Private Function GetVariable(name As String) As Value
        Dim var = _vars.Find(name)
        If var Is Nothing Then
            Throw New Exception("Var not exist")
        End If
        Return var
    End Function

    Private Function ResolveToValue(ByRef arg As Argument) As Value
        Select Case arg.Type
            Case ArgumentType.typeVariable
                Dim var = GetVariable(arg.VariableName)
                Return var
            Case ArgumentType.typeValue
                Return arg.Value
            Case Else
                Throw New Exception("")
        End Select
    End Function

    Private Function GetParamDataType(ByRef param As Argument) As DataType
        Return ResolveToValue(param).Type
    End Function

    'запускаем программу
    Public Sub Run(program() As Instruction, out As StringBuilder)

        Dim errs As New StringBuilder
        'исполняем программу на ПЯ
        Dim cs As Integer = 0
        Dim working As Boolean = True
        'поток для
        Do While (cs < program.Length) AndAlso working
            Dim instr As Instruction = program(cs)

            'инструкция вызова функции
            If instr.Action = Instruction.ActionType.callProgram Then
                CallFunction(out, errs, instr)
            End If

            'инструкция вызова функции
            If instr.Action = Instruction.ActionType.callByName Then
                Dim funcName As String = instr.Args(0).Value.ValueString
                CallInternalFunction(out, errs, instr, funcName)
            End If

            'инструкция добавления переменной
            If instr.Action = Instruction.ActionType.addVariable Then
                Dim var As New Value
                Dim arg = instr.Args(0)
                var.Name = arg.VariableName
                var.Type = arg.Value.Type
                _vars.Add(var)
            End If

            'инструкция присваивания переменной
            If instr.Action = Instruction.ActionType.setVariable Then
                Dim var = GetVariable(instr.Args(0).VariableName)
                Dim arg = ResolveToValue(instr.Args(1))

                If var.Type = DataType.dataInteger Then
                    var.ValueInteger = arg.ValueInteger
                End If

                If var.Type = DataType.dataString Then
                    var.ValueString = arg.ValueString
                End If
                'TODO типы данных
            End If

            'инструкция условного перехода
            If instr.Action = Instruction.ActionType.jumpIfNot Then
                'значение-условие
                Dim arg = ResolveToValue(instr.Args(0))
                'если значение ложно, переходим
                If arg.ValueInteger <= 0 Then
                    Dim v = instr.Args(1).Value.ValueInteger
                    cs += v
                End If
            End If

            'инструкция безусловного перехода
            If instr.Action = Instruction.ActionType.jumpAlways Then
                Dim v = instr.Args(0).Value.ValueInteger
                cs = cs + v
            End If

            'инкрементируем счетчик
            cs += 1

            'если были ошибки
            If errs.Length > 0 Then
                working = False
                Dim exc = New RunTimeError
                exc.Text = errs.ToString
                Throw exc
            End If
        Loop
    End Sub
End Class
