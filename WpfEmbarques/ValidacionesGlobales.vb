
Imports System.Security.Cryptography
Imports System.Text
Module ValidacionesGlobales
    Public Sub SoloNumeros_Pasting(sender As Object, e As DataObjectPastingEventArgs)
        If e.DataObject.GetDataPresent(GetType(String)) Then
            Dim texto = CStr(e.DataObject.GetData(GetType(String)))
            If Not System.Text.RegularExpressions.Regex.IsMatch(texto, "^\d+$") Then
                e.CancelCommand()
            End If
        Else
            e.CancelCommand()
        End If
    End Sub

    Public Sub SoloDecimales_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        Dim textBox = TryCast(sender, TextBox)
        Dim textoCompleto = textBox.Text.Insert(textBox.SelectionStart, e.Text)

        ' Aceptamos coma o punto como separador decimal (dependiendo del sistema)
        Dim decimalSeparator As String = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator

        Dim regex = New System.Text.RegularExpressions.Regex("^\d*(" & System.Text.RegularExpressions.Regex.Escape(decimalSeparator) & ")?\d*$")
        e.Handled = Not regex.IsMatch(textoCompleto)
    End Sub

    Public Sub SoloDecimales_Pasting(sender As Object, e As DataObjectPastingEventArgs)
        Dim decimalSeparator As String = Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator

        If e.DataObject.GetDataPresent(GetType(String)) Then
            Dim texto = CStr(e.DataObject.GetData(GetType(String)))
            Dim regex = New System.Text.RegularExpressions.Regex("^\d*(" & System.Text.RegularExpressions.Regex.Escape(decimalSeparator) & ")?\d*$")
            If Not regex.IsMatch(texto) Then
                e.CancelCommand()
            End If
        Else
            e.CancelCommand()
        End If
    End Sub
    Public Sub AsociarComportamientoComboBox(obj As DependencyObject)
        For i As Integer = 0 To VisualTreeHelper.GetChildrenCount(obj) - 1
            Dim child = VisualTreeHelper.GetChild(obj, i)

            If TypeOf child Is ComboBox Then
                AddHandler CType(child, ComboBox).PreviewMouseDown, AddressOf AbrirComboAlHacerClick
            Else
                AsociarComportamientoComboBox(child) ' Recursivo para buscar más hijos
            End If
        Next
    End Sub

    Public Sub AbrirComboAlHacerClick(sender As Object, e As MouseButtonEventArgs)
        Dim combo = TryCast(sender, ComboBox)
        If combo IsNot Nothing AndAlso Not combo.IsDropDownOpen Then
            combo.IsDropDownOpen = True
            e.Handled = True
        End If
    End Sub
    Public Function CalcularHash(ByVal input As String) As String
        Using sha256 As SHA256 = SHA256.Create()
            Dim bytes As Byte() = sha256.ComputeHash(Encoding.UTF8.GetBytes(input))
            Dim builder As New StringBuilder()
            For Each b As Byte In bytes
                builder.Append(b.ToString("x2"))
            Next
            Return builder.ToString()
        End Using
    End Function
End Module
