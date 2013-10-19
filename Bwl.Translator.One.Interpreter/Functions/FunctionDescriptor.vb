'функция
Public Class FunctionDescriptor
    Public Property Name As String = ""
    Public Property Args As New List(Of FunctionArgument)
    Public Property Result As New FunctionArgument
    Public Property EntryPoint As Integer = -1
    Public Property FreeParams As Boolean = False
    Public Property FreeParamsCount As Integer = -1
    Public Property FreeParamsType As PossibleDataType = PossibleDataType.pdtAny

    Sub New()
        Me.result.Comment = "Result"
        Me.Result.Type = PossibleDataType.pdtAny
    End Sub

    Public Sub CopyFrom(v As FunctionDescriptor)
        Name = String.Copy(v.Name)
        Args.Clear()
        Args.AddRange(v.Args)
        result = v.result.Clone
        entryPoint = v.entryPoint
        freeParams = v.freeParams
        freeParamsCount = v.freeParamsCount
        freeParamsType = v.freeParamsType
    End Sub

    Sub New(v As FunctionDescriptor)
        CopyFrom(v)
    End Sub
End Class