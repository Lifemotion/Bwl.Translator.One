Imports System.Text

Public Class SyntaxAnalyzer

    Private _funcPointer As Integer
    Private _externalFunctions As FunctionsStorage
    Private _programFunctions As FunctionsStorage
    Private _vars As New VarDescsStorage
    Private _blocks As New List(Of Block)()
    Private _prg As New List(Of Instruction)
    Private _unknownFunction As New FunctionDescriptor
    Private _tmpVarCount As Integer = 1

    'переменные и функции
    Sub New(externalFunctions As FunctionsStorage)
        _externalFunctions = externalFunctions
        _programFunctions = New FunctionsStorage
        _funcPointer = -1
        _unknownFunction.name = "UnknownExternal"
        _unknownFunction.freeParams = True
    End Sub


    'синтаксический анилиз и генерация промежуточного кода
    Public Sub AddCodeLine(_lex As Lexem(), debugLineNumber As Integer)
        'номер лексемы
        Dim pos As Integer = 0
        'лексема
        Dim lex As New Lexem()
        'состояние КА
        Dim state As State = state.synStart
        'стек(магазин) лексем
        Dim stack As New List(Of StackElement)()
        'SynStackElem elem;
        'ошибки
        Dim errs As New StringBuilder
        Dim result As String = ""
        Dim funcCount As Integer = 0
        'счетчик операций ПЯ
        Dim instrCount As Integer = 0
        Do While pos < _lex.Length
            'очередная лексема
            lex = _lex(pos)
            Dim accepted As Boolean = False
            Select Case state
                'начало трансляции
                Case state.synStart
                    If lex.Type = LexType.lexEndLine Then
                        accepted = True
                    End If
                    If lex.Type = LexType.lexDim Then
                        accepted = True
                        state = state.synDim
                    End If
                    If lex.Type = LexType.lexV Then
                        accepted = True
                        state = state.synR
                        pos -= 1
                    End If
                    If lex.Type = LexType.lexFunction Then
                        If _funcPointer = -1 Then
                            accepted = True
                            state = state.synSub
                        Else
                            errs.AppendLine("Функции не могут быть вложенными!")
                        End If
                    End If
                    If lex.Type = LexType.lexSub Then
                        If _funcPointer = -1 Then
                            accepted = True
                            state = state.synSub
                        Else
                            errs.AppendLine("Функции не могут быть вложенными!")

                        End If
                    End If
                    If lex.Type = LexType.lexEnd Then
                        accepted = True
                        state = state.synEnd
                    End If
                    If lex.Type = LexType.lexIf Then
                        stack.Add(GlobalMembersInterpreter.NewSSE("IF", StackElementType.ssIf))
                        accepted = True
                        state = state.synR
                    End If
                    If lex.Type = LexType.lexWhile Then
                        stack.Add(GlobalMembersInterpreter.NewSSE("WHILE", StackElementType.ssWhile))
                        accepted = True
                        state = state.synR
                    End If
                    'DIM - объявление переменной
                Case state.synDim
                    If lex.Type = LexType.lexV Then
                        'нет ли уже такой переменной

                        'нету
                        If _vars.Find(lex.Name) Is Nothing Then
                            accepted = True
                            stack.Add(GlobalMembersInterpreter.NewSSE(lex.NameUpper))
                            'есть
                        Else
                            errs.AppendLine("Переменная " + lex.NameUpper + " уже объявлена!")
                        End If
                    End If

                    If lex.Type = LexType.lexAs Then
                        accepted = True
                        state = state.synDimAs
                    End If

                Case state.synDimAs
                    Dim type As DataType
                    If lex.Type = LexType.lexInteger Then
                        accepted = True
                        type = DataType.dataInteger
                    End If
                    If lex.Type = LexType.lexString Then
                        accepted = True
                        type = DataType.dataString
                    End If
                    If accepted Then
                        'тип указан правильно, добавляем переменные
                        Do While stack.Count > 0
                            Dim varInfo = stack(stack.Count - 1)
                            Dim addingVar = New VarDescriptor
                            addingVar.Name = varInfo.name
                            addingVar.Type = type
                            stack.RemoveAt(stack.Count - 1)
                            _vars.Add(addingVar)
                            'команда добавления переменно
                            Dim instr As New Instruction
                            instr.DebugFromLine = debugLineNumber
                            instr.Action = Instruction.ActionType.addVariable
                            Dim arg1 As New Argument
                            arg1.Type = type
                            arg1.Value = varInfo.name
                            instr.Args.Add(arg1)
                            _prg.Add(instr)
                        Loop
                        state = state.synEndLine
                    Else
                        'тип указан неправильно
                        errs.AppendLine("Неправильно указан тип переменной.")
                    End If
                    'строка началась с идентификатора
                Case state.synR
                    If lex.Type = LexType.lexThen Then
                        accepted = True
                        state = state.synThen
                    End If
                    If lex.Type = LexType.lexV Then
                        accepted = True
                        stack.Add(GlobalMembersInterpreter.NewSSE(lex.NameUpper))
                    End If
                    ' ( - помечаем строку как вызов функции
                    If lex.Type = LexType.lexOB Then
                        Dim prev As StackElement = stack(stack.Count - 1)
                        stack.RemoveAt(stack.Count - 1)
                        If prev.type = StackElementType.ssName Then
                            accepted = True
                            stack.Add(GlobalMembersInterpreter.NewSSE(prev.name, StackElementType.ssCall))
                        End If
                    End If
                    ' = - помечаем строку как присваивание, если найдется указанная переменная
                    If lex.Type = LexType.lexEq Then
                        If stack.Count = 1 Then
                            Dim prev As StackElement = stack(stack.Count - 1)
                            stack.RemoveAt(stack.Count - 1)
                            If prev.type = StackElementType.ssName OrElse stack.Count = 0 Then
                                If _vars.Find(prev.name) IsNot Nothing Then
                                    accepted = True
                                    stack.Add(GlobalMembersInterpreter.NewSSE(prev.name, StackElementType.ssLet))
                                Else
                                    errs.AppendLine("Переменная не найдена" + lex.NameUpper)
                                End If
                            Else
                                errs.AppendLine("Левая часть присваивания неверна!")
                            End If
                        Else
                            errs.AppendLine("Левая часть присваивания неверна!")
                        End If
                    End If

                    'число - заносим как параметр в стек
                    If lex.Type = LexType.lexN Then
                        accepted = True
                        Dim elem As StackElement = GlobalMembersInterpreter.NewSSE("", StackElementType.ssParam)
                        elem.param.Type = DataType.dataInteger
                        elem.param.Value = CInt(Val(lex.Name)).ToString
                        stack.Add(elem)
                    End If
                    'строковая константа - заносим как параметр в стек
                    If lex.Type = LexType.lexS Then
                        accepted = True
                        Dim elem As StackElement = GlobalMembersInterpreter.NewSSE("", StackElementType.ssParam)
                        elem.param.Type = DataType.dataString
                        elem.param.Value = lex.Name
                        stack.Add(elem)
                    End If
                    ' ) - закрываем функцию
                    If lex.Type = LexType.lexCB Then
                        Dim params As New List(Of StackElement)()
                        Dim elem As New StackElement()
                        Dim walk As Boolean = True
                        Dim correct As Boolean = False
                        Do While walk
                            If stack.Count > 0 Then
                                elem = stack(stack.Count - 1)
                                stack.RemoveAt(stack.Count - 1)
                                'если элемент в стеке - параметр
                                If elem.type = StackElementType.ssParam Then

                                    params.Insert(0, elem)
                                    'если элемент - вызов функции
                                ElseIf elem.type = StackElementType.ssCall Then
                                    correct = True
                                    walk = False
                                    'если элемент - имя переменной
                                ElseIf elem.type = StackElementType.ssName Then
                                    If _vars.Find(elem.name) IsNot Nothing Then
                                        ' Dim varIndex As Integer = _vars.FindVar(elem.name)
                                        Dim param As New StackElement()
                                        param.type = StackElementType.ssParam
                                        param.name = "" & elem.name
                                        param.param.Type = DataType.dataString
                                        param.param.IsPointer = True
                                        param.param.Value = elem.name

                                        params.Insert(0, param)
                                    Else
                                        walk = False
                                        errs.AppendLine("Переменная не определена: " + elem.name)
                                    End If
                                Else
                                    walk = False
                                    errs.AppendLine("Какая-то ерунда вместо значения: " + elem.name)
                                End If
                            Else
                                walk = False
                                errs.AppendLine("Что же закрывает скобка?")
                            End If
                        Loop
                        If correct Then
                            'параметры корректны и найден вызов функции
                            Dim funcInd1 As Integer = _externalFunctions.FindFunction(elem.name)
                            Dim funcInd2 As Integer = _programFunctions.FindFunction(elem.name)
                            If funcInd1 > -1 Or funcInd2 > -1 Or UnknownnExternalsAllowed Then
                                'функция существует
                                Dim func As New FunctionDescriptor(_unknownFunction)
                                Dim funcExternal As Boolean = funcInd1 > -1
                                If funcInd1 > -1 Then func = (_externalFunctions(funcInd1))
                                If funcInd2 > -1 Then func = (_programFunctions(funcInd2))

                                Dim paramsCorrect As Boolean = True
                                If Not (func.FreeParams) Then
                                    If params.Count = func.Args.Count Then
                                        For i As Integer = 0 To params.Count - 1
                                            'переданный параметр - значение
                                            If Not params(i).param.IsPointer Then
                                                If Not (DataTypeConverter.FromFD(func.Args(i).Type) = params(i).param.Type) Then
                                                    errs.AppendLine("Неверный тип параметра #" + (i + 1).ToString + ": " + func.Args(i).Comment)
                                                    paramsCorrect = False
                                                End If
                                            Else
                                                'переданный параметр - переменная
                                                If Not (DataTypeConverter.FromFD(func.Args(i).Type) = _vars.Find(params(i).param.Value).Type) Then
                                                    errs.AppendLine("Неверный тип параметра #" + (i + 1).ToString + ": " + func.Args(i).Comment)
                                                    paramsCorrect = False
                                                End If
                                            End If
                                        Next i
                                    Else
                                        errs.AppendLine("Функция " + elem.name + " принимает " + func.Args.Count.ToString + " парам.")
                                    End If
                                Else
                                    'параметры частично или полностью свободны
                                    If (func.FreeParamsCount > -1) AndAlso (func.FreeParamsCount <> params.Count) Then
                                        errs.AppendLine("Функция " + elem.name + " принимает " + func.FreeParamsCount.ToString + " парам.")
                                    End If
                                    If func.FreeParamsType <> PossibleDataType.pdtAny Then
                                        For i As Integer = 0 To params.Count - 1
                                            If Not params(i).param.IsPointer Then
                                                If Not (DataTypeConverter.FromFD(func.FreeParamsType) = params(i).param.Type) Then
                                                    errs.AppendLine("Неверный тип параметра #" + (i + 1).ToString)
                                                    paramsCorrect = False
                                                End If
                                            Else
                                                'переданный параметр - переменная
                                                If Not (DataTypeConverter.FromFD(func.FreeParamsType) = _vars.Find(params(i).param.Value).Type) Then
                                                    errs.AppendLine("Неверный тип параметра #" + (i + 1).ToString)
                                                    paramsCorrect = False
                                                End If
                                            End If
                                        Next i
                                    End If

                                End If

                                'параметры функции проверены
                                If paramsCorrect Then
                                    'вводим переменную для результата
                                    Dim var = New VarDescriptor
                                    _vars.Add(var)

                                    var.Name = "!" + _tmpVarCount.ToString
                                    _tmpVarCount += 1
                                    If func.Result.Type = PossibleDataType.pdtAny Then
                                        func.Result.Type = PossibleDataType.pdtInteger
                                    End If

                                    var.Type = DataTypeConverter.FromFD(func.Result.Type)

                                    'команда добавления переменно
                                    Dim instr1 As New Instruction
                                    instr1.Action = Instruction.ActionType.addVariable
                                    instr1.DebugFromLine = debugLineNumber
                                    Dim arg1 As New Argument
                                    arg1.Type = var.Type
                                    arg1.Value = var.Name
                                    instr1.Args.Add(arg1)
                                    _prg.Add(instr1)

                                    'суём переменную результата в стек
                                    Dim resVar As StackElement = GlobalMembersInterpreter.NewSSE("Result_" & elem.name, StackElementType.ssParam)
                                    resVar.param.Comment = "RET" '& elem.name
                                    resVar.param.Value = var.Name
                                    resVar.param.Type = DataType.dataString
                                    resVar.param.IsPointer = True
                                    stack.Add(resVar)
                                    'заносим вызов функции в виде инструкции ПЯ
                                    Dim instr As New Instruction()
                                    instr.DebugFromLine = debugLineNumber
                                    instr.Action = Instruction.ActionType.callByName
                                    'первый параметр-имя функции
                                    Dim funcN As New Argument()
                                    funcN.Type = DataType.dataString
                                    funcN.Value = elem.name
                                    '  funcN.Value.ValueInteger = funcInd1
                                    ' funcN.Comment = elem.name
                                    instr.Args.Add(funcN)
                                    'второй параметр - результат возврата
                                    Dim ret As New Argument()
                                    ret.Type = DataType.dataString
                                    ret.IsPointer = True

                                    ret.Value = var.Name
                                    instr.Args.Add(ret)
                                    'третий и далее - параметры функции
                                    For i As Integer = 0 To params.Count - 1
                                        Dim param As New Argument()
                                        param = params(i).param
                                        '  param.Comment = params(i).name
                                        instr.Args.Add(param)
                                    Next i
                                    instrCount += 1
                                    _prg.Add(instr)
                                    accepted = True
                                    'state=synEndLine;
                                End If
                            Else
                                errs.AppendLine("Функция не найдена: " + elem.name)
                            End If
                        End If
                    End If
                    If lex.Type = LexType.lexEndLine Then
                        accepted = True
                        pos -= 1
                        state = state.synEndLine
                    End If
                    'состояние конца строки лексем - закрываем открытые операции
                Case state.synEndLine
                    'снимаем верхний параметер стека, если есть
                    Dim value As New StackElement()
                    If stack.Count > 0 Then
                        value = stack(stack.Count - 1)
                        stack.RemoveAt(stack.Count - 1)
                        'верхний параметр может быть только значением, а после него в стеке должно остаться не более одного элемента
                        If (value.type = StackElementType.ssParam) AndAlso (stack.Count <= 1) Then
                            If stack.Count = 0 Then
                                accepted = True
                            End If
                            If stack.Count = 1 Then
                                Dim op As StackElement = stack(stack.Count - 1)
                                stack.RemoveAt(stack.Count - 1)
                                'операция присваивания
                                If op.type = StackElementType.ssLet Then
                                    Dim var = _vars.Find(op.name)
                                    If GetParamDataType(value.param) <> var.Type Then
                                        'типы не соответсвуют
                                        errs.AppendLine("Тип переменной " + (var.Name) + " не соответсвует типу значения.")
                                    Else
                                        'присваиваем (на ПЯ)
                                        Dim instr As New Instruction()
                                        instr.DebugFromLine = debugLineNumber
                                        instr.Action = Instruction.ActionType.setVariable
                                        'первый параметр - имя переменной - приемника
                                        Dim dest As New Argument()
                                        dest.Type = DataType.dataString
                                        dest.IsPointer = True
                                        dest.Value = op.name
                                        ' dest.Comment = op.name
                                        instr.Args.Add(dest)
                                        'второй параметр - параметр (результат)
                                        instr.Args.Add(value.param)
                                        instrCount += 1
                                        _prg.Add(instr)
                                        accepted = True
                                    End If
                                End If
                                'условный блок
                                If (op.type = StackElementType.ssIf) OrElse (op.type = StackElementType.ssWhile) Then
                                    If GetParamDataType(value.param) = DataType.dataInteger Then
                                        'все верно, добавляем условный переход
                                        Dim instr As New Instruction()
                                        instr.DebugFromLine = debugLineNumber
                                        instr.Action = Instruction.ActionType.jumpIfNot
                                        'первый параметр - параметр условия
                                        'TODO :: The following line was determined to be a copy constructor call - this should be verified and a copy constructor should be created if it does not yet exist:
                                        'ORIGINAL LINE: Parameter ifr=value.param;
                                        Dim ifr = value.param.Clone
                                        '  ifr.Comment = "If"
                                        instr.Args.Add(ifr)
                                        'второй параметр - смещение до конца блока
                                        'оно будет заполнено позднее
                                        Dim dest As New Argument()
                                        dest.Comment = "Offset"
                                        dest.Type = DataType.dataInteger
                                        dest.Value = "0"
                                        instr.Args.Add(dest)
                                        'третий парамектр - количество операций в этой строке
                                        Dim count As New Argument()
                                        count.Comment = "InstrCount"
                                        count.Value = instrCount.ToString
                                        count.Type = DataType.dataInteger
                                        instr.Args.Add(count)
                                        instrCount += 1
                                        _prg.Add(instr)
                                        accepted = True
                                        Dim block As New Block()
                                        block.startAddress = _prg.Count - 1
                                        If op.type = StackElementType.ssIf Then
                                            block.type = BlockType.blockIf
                                        End If
                                        If op.type = StackElementType.ssWhile Then
                                            block.type = BlockType.blockWhile
                                        End If
                                        _blocks.Add(block)
                                    Else
                                        errs.AppendLine("Конструкция If..End If принимает занчения типа Integer.")
                                    End If
                                End If
                            End If
                        Else
                            errs.AppendLine("Не удалось разобрать строку. Неожиданный элемент в строке. " + value.name)
                        End If
                    Else
                        'если стек пуст, делать, соответственно, нечего
                        accepted = True
                    End If
                    Exit Select
                Case state.synSub
                    If lex.Type = LexType.lexV Then
                        stack.Add(GlobalMembersInterpreter.NewSSE(lex.NameUpper))
                        state = state.synSubV
                        accepted = True
                    End If
                Case state.synSubV
                    If lex.Type = LexType.lexOB Then
                        state = state.synSubOB
                        accepted = True
                    End If
                Case state.synSubOB
                    If lex.Type = LexType.lexCB Then
                        state = state.synSubCB
                        accepted = True
                    Else
                        If (pos + 2 < _lex.Length) AndAlso (_lex(pos).Type = LexType.lexV) AndAlso (_lex(pos + 1).Type = LexType.lexAs) AndAlso ((_lex(pos + 2).Type = LexType.lexInteger) OrElse (_lex(pos + 2).Type = LexType.lexString)) Then
                            'параметр в верном формате
                            Dim arg As New StackElement()
                            arg.type = StackElementType.ssParam
                            arg.param.Comment = _lex(pos).NameUpper
                            If _lex(pos + 2).Type = LexType.lexString Then
                                arg.param.Type = DataType.dataString
                            End If
                            If _lex(pos + 2).Type = LexType.lexInteger Then
                                arg.param.Type = DataType.dataInteger
                            End If
                            'добавляем в стек

                            stack.Insert(0, arg)
                            'stack.push_back(arg);
                            'инкрементируем
                            pos += 2
                            accepted = True
                        Else
                            errs.AppendLine("Параметр функции " + lex.NameUpper + " задан неверно.")
                        End If
                    End If
                Case state.synSubCB
                    'скобка закрыта, дальше или конец строки, или As с параметром
                    Dim correct As Boolean = False
                    Dim type As DataType = DataType.dataInteger
                    If pos = _lex.Length - 1 Then
                        correct = True
                    Else
                        If (pos + 2 = _lex.Length - 1) AndAlso (_lex(pos).Type = LexType.lexAs) Then
                            If _lex(pos + 1).Type = LexType.lexInteger Then
                                correct = True
                            End If
                            If _lex(pos + 1).Type = LexType.lexString Then
                                correct = True
                                type = DataType.dataString
                            End If
                        End If
                    End If
                    If correct Then
                        'получаем имя функции
                        Dim funcName As String = stack(stack.Count - 1).name
                        stack.RemoveAt(stack.Count - 1)
                        'добавляем объявление функции
                        Dim num1 As Integer = _externalFunctions.FindFunction(funcName)
                        Dim num2 As Integer = _programFunctions.FindFunction(funcName)
                        If num1 = -1 And num2 = -1 Then
                            'такой функции еще нет
                            Dim num = _programFunctions.AddFunction()
                            'добавляем параметры
                            _programFunctions(num).Name = funcName
                            _programFunctions(num).Result.Type = DataTypeConverter.ToFD(type)
                            'добавляем инструкцию
                            Dim instr As New Instruction()
                            instr.DebugFromLine = debugLineNumber
                            instr.Action = Instruction.ActionType.functionEntryPoint
                            Dim func As New Argument()
                            '  func.Value.ValueInteger = num
                            instr.Args.Add(func)
                            instrCount += 1
                            _prg.Add(instr)
                            _programFunctions(num).EntryPoint = _prg.Count - 1
                            _funcPointer = num
                            Do While stack.Count > 0
                                Dim arg As Argument = stack(stack.Count - 1).param
                                stack.RemoveAt(stack.Count - 1)
                                ' _programFunctions(num).Args.Add(arg)
                                ' Dim var As Integer = _vars.AddVar()
                                ' _vars(var).name = arg.name
                            Loop
                            Dim block As New Block()
                            block.startAddress = _prg.Count - 1
                            block.type = BlockType.blockSub
                            _blocks.Add(block)
                            accepted = True
                        Else
                            errs.AppendLine("Такая функция уже объявлена!")
                        End If
                    Else
                        errs.AppendLine("Возврат функции задан неверно.")
                    End If
                Case state.synEnd
                    If (lex.Type = LexType.lexSub) OrElse (lex.Type = LexType.lexFunction) Then
                        'завершающая строка блока функции - ищем соответсвующую ей начальную строку
                        If (_funcPointer <> -1) AndAlso (_blocks(_blocks.Count - 1).type = BlockType.blockSub) Then
                            'добавляем инструкцию
                            Dim instr As New Instruction()
                            instr.DebugFromLine = debugLineNumber
                            instr.Action = Instruction.ActionType.functionReturn
                            Dim func As New Argument()
                            func.Value = _funcPointer.ToString
                            instr.Args.Add(func)
                            instrCount += 1
                            _prg.Add(instr)
                            'выходим из режима блока функции
                            _funcPointer = -1
                            _blocks.RemoveAt(_blocks.Count - 1)
                            accepted = True
                            state = state.synEndLine
                        Else
                            errs.AppendLine("Завершение функции без ее начала.")
                        End If
                    End If
                    'завершающая строка условного блока
                    If lex.Type = LexType.lexIf Then
                        If (_blocks.Count > 0) AndAlso (_blocks(_blocks.Count - 1).type = BlockType.blockIf) Then
                            'добавляем инструкцию
                            Dim instr As New Instruction()
                            instr.DebugFromLine = debugLineNumber
                            instr.Action = Instruction.ActionType.idleLabel

                            instrCount += 1
                            _prg.Add(instr)
                            'ищем блок If и указываем в нем смещение
                            Dim addr As Integer = _blocks(_blocks.Count - 1).startAddress
                            Dim offset As Integer = _prg.Count - 2 - addr
                            'второй параметр - смещение
                            _prg(addr).Args(1).Value = offset.ToString
                            'удаляем метку блока из стека
                            _blocks.RemoveAt(_blocks.Count - 1)
                            accepted = True
                            state = state.synEndLine
                        Else
                            errs.AppendLine("End If без If .. Then.")
                        End If
                    End If
                    'завершающая строка блока цикла
                    If lex.Type = LexType.lexWhile Then
                        If (_blocks.Count > 0) AndAlso (_blocks(_blocks.Count - 1).type = BlockType.blockWhile) Then
                            Dim addr As Integer = _blocks(_blocks.Count - 1).startAddress
                            Dim offset3 As Integer = CInt(_prg(addr).Args(2).Value)
                            'добавляем инструкцию перехода на начало цикла
                            Dim instr1 As New Instruction()
                            instr1.DebugFromLine = debugLineNumber
                            instr1.Action = Instruction.ActionType.jumpAlways
                            Dim offset As New Argument()
                            offset.Comment = "Offset"
                            offset.Type = DataType.dataInteger
                            offset.Value = (addr - _prg.Count - 1 - offset3).ToString
                            instr1.Args.Add(offset)
                            instrCount += 1
                            _prg.Add(instr1)
                            'добавляем инструкцию метки
                            Dim instr2 As New Instruction()
                            instr2.DebugFromLine = debugLineNumber
                            instr2.Action = Instruction.ActionType.idleLabel
                            Dim name As New Argument()
                            name.Comment = "End While Label"
                            '  instr2.params.Add(name)
                            instrCount += 1
                            _prg.Add(instr2)
                            'ищем блок If и указываем в нем смещение до метки
                            Dim offset2 As Integer = _prg.Count - 2 - addr
                            'второй параметр - смещение
                            _prg(addr).Args(1).Value = offset2.ToString
                            'удаляем метку блока из стека
                            _blocks.RemoveAt(_blocks.Count - 1)
                            accepted = True
                            state = state.synEndLine
                        Else
                            errs.AppendLine("End While без While.")
                        End If
                    End If
                Case state.synThen
                    'перешли сюда после обнаружения Then
                    pos -= 1
                    accepted = True
                    state = state.synEndLine
            End Select

            'были ошибки разбора
            If Not accepted Then
                Dim exc As New SyntaxAnalyzerError
                exc.Text = errs.ToString
                Throw exc
            End If
            pos += 1
        Loop
    End Sub

    Public ReadOnly Property Instructions As Instruction()
        Get
            Return _prg.ToArray
        End Get
    End Property

    Public Property UnknownnExternalsAllowed As Boolean = False

    Private Function GetParamDataType(ByRef param As Argument) As DataType
        If param.IsPointer Then
            Return _vars.Find(param.Value).Type
        Else
            Return param.Type
        End If

    End Function
End Class
