'инструкция ПЯ
Public Class Instruction
    Public Enum ActionType
        idle = -1
        idleLabel = -2

        callByName = 1
        callProgram = 2

        addVariable = 11
        removeVariable = 12
        setVariable = 13

        jumpIfNot = 21
        jumpAlways = 22

        functionEntryPoint = 31
        functionReturn = 32
    End Enum
    Public Property Action As ActionType
    Public Property Args As New List(Of Argument)

    Public Overrides Function ToString() As String
        Dim result = Action.ToString
        For Each prm In Args
            result += " " + prm.ToString
        Next
        Return result
    End Function
End Class