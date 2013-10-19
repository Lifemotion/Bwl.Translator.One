<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TestForm
    Inherits System.Windows.Forms.Form

    'Форма переопределяет dispose для очистки списка компонентов.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Является обязательной для конструктора форм Windows Forms
    Private components As System.ComponentModel.IContainer

    'Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
    'Для ее изменения используйте конструктор форм Windows Form.  
    'Не изменяйте ее в редакторе исходного кода.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.source = New System.Windows.Forms.TextBox()
        Me.run = New System.Windows.Forms.Button()
        Me.result = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'source
        '
        Me.source.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.source.Location = New System.Drawing.Point(8, 9)
        Me.source.Multiline = True
        Me.source.Name = "source"
        Me.source.Size = New System.Drawing.Size(608, 181)
        Me.source.TabIndex = 0
        Me.source.Text = "source"
        '
        'run
        '
        Me.run.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.run.Location = New System.Drawing.Point(8, 199)
        Me.run.Name = "run"
        Me.run.Size = New System.Drawing.Size(96, 30)
        Me.run.TabIndex = 1
        Me.run.Text = "Run"
        Me.run.UseVisualStyleBackColor = True
        '
        'result
        '
        Me.result.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.result.Location = New System.Drawing.Point(8, 237)
        Me.result.Multiline = True
        Me.result.Name = "result"
        Me.result.Size = New System.Drawing.Size(608, 171)
        Me.result.TabIndex = 2
        '
        'TestForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(627, 419)
        Me.Controls.Add(Me.result)
        Me.Controls.Add(Me.run)
        Me.Controls.Add(Me.source)
        Me.Name = "TestForm"
        Me.Text = "Bwl Interpreter Test"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents source As System.Windows.Forms.TextBox
    Friend WithEvents run As System.Windows.Forms.Button
    Friend WithEvents result As System.Windows.Forms.TextBox

End Class
