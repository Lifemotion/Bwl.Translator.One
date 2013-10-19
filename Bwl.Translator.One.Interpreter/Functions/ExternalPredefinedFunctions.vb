Public Class ExternalPredefinedFunctions
    Private Shared _funcs As New List(Of FunctionDescriptor)

    Private Shared Function AddFunction() As Integer
        Dim func As New FunctionDescriptor
        func.entryPoint = -1
        func.result.Comment = "Result"
        func.Result.Type = PossibleDataType.pdtInteger
        func.freeParams = False
        func.freeParamsCount = -1
        func.FreeParamsType = PossibleDataType.pdtAny
        _funcs.Add(func)
        Return _funcs.Count - 1
    End Function

    Shared Sub New()
        Dim pos As Integer
        'функция вывода на страницу
        pos = AddFunction()
        _funcs(pos).name = "WRITE"
        _funcs(pos).freeParams = True
        pos = AddFunction()
        _funcs(pos).name = "WRITELINE"
        _funcs(pos).freeParams = True
        'сложение чисел
        pos = AddFunction()
        _funcs(pos).name = "ADD"
        Dim param As New FunctionArgument()
        Dim result As New FunctionArgument()
        param.Comment = "AddParam"
        param.Type = PossibleDataType.pdtInteger
        result = param.Clone
        result.Comment = "Result"
        _funcs(pos).Args.Add(param.Clone)
        _funcs(pos).Args.Add(param.Clone)
        _funcs(pos).result = result.Clone
        'вычитание
        pos = AddFunction()
        param.Comment = "SubParam"
        _funcs(pos).name = "SB"
        _funcs(pos).Args.Add(param.Clone)
        _funcs(pos).Args.Add(param.Clone)
        _funcs(pos).result = result.Clone
        'умножение
        pos = AddFunction()
        param.Comment = "MulParam"
        _funcs(pos).name = "MUL"
        _funcs(pos).Args.Add(param.Clone)
        _funcs(pos).Args.Add(param.Clone)
        _funcs(pos).result = result.Clone
        'деление
        pos = AddFunction()
        param.Comment = "DivParam"
        _funcs(pos).name = "DIV"
        _funcs(pos).Args.Add(param.Clone)
        _funcs(pos).Args.Add(param.Clone)
        _funcs(pos).result = result.Clone
        'инкремент
        pos = AddFunction()
        param.Comment = "IncParam"
        _funcs(pos).name = "INC"
        _funcs(pos).Args.Add(param.Clone)
        _funcs(pos).result = result.Clone
        pos = AddFunction()
        param.Comment = "DecParam"
        _funcs(pos).name = "DEC"
        _funcs(pos).Args.Add(param.Clone)
        _funcs(pos).result = result.Clone
        pos = AddFunction()
        param.Comment = "NotParam"
        _funcs(pos).name = "NO"
        _funcs(pos).Args.Add(param.Clone)
        _funcs(pos).result = result.Clone
        'сравнения
        pos = AddFunction()
        param.Comment = "CmpParam"
        _funcs(pos).name = "EQUAL"
        _funcs(pos).freeParams = True
        _funcs(pos).freeParamsCount = 2
        _funcs(pos).result = result.Clone
        pos = AddFunction()
        _funcs(pos).name = "LOWER"
        _funcs(pos).freeParams = True
        _funcs(pos).freeParamsCount = 2
        _funcs(pos).result = result.Clone
        pos = AddFunction()
        _funcs(pos).name = "GREATER"
        _funcs(pos).freeParams = True
        _funcs(pos).freeParamsCount = 2
        _funcs(pos).result = result.Clone
        'логические операции
        pos = AddFunction()
        param.Comment = "LogicParam"
        _funcs(pos).name = "AND"
        _funcs(pos).Args.Add(param.Clone)
        _funcs(pos).Args.Add(param.Clone)
        _funcs(pos).result = result.Clone
        pos = AddFunction()
        _funcs(pos).name = "OR"
        _funcs(pos).Args.Add(param.Clone)
        _funcs(pos).Args.Add(param.Clone)
        _funcs(pos).result = result.Clone
        'параметры запроса
        Dim strParam As New FunctionArgument()
        strParam.Comment = "StrParam"
        strParam.Type = PossibleDataType.pdtString
        pos = AddFunction()
        _funcs(pos).name = "GETPARAM"
        _funcs(pos).Args.Add(strParam.Clone)
        _funcs(pos).result = result.Clone
        'строку в число
        pos = AddFunction()
        strParam.Comment = "StrParam"
        strParam.Type = PossibleDataType.pdtString
        _funcs(pos).name = "VAL"
        _funcs(pos).Args.Add(strParam.Clone)
    End Sub

    Public Shared Sub CopyTo(funcStorage As FunctionsStorage)
        For Each f In _funcs
            funcStorage.Add(f)
        Next
    End Sub

End Class
