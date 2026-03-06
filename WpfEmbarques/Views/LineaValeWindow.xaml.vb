Imports System.Collections.ObjectModel
Imports System.Windows.Input
Imports WpfEmbarques.ValidacionesGlobales
Public Class LineaValeWindow
    Public Property LineaActual As LineaVale
    Public Property EsEdicion As Boolean

    Public Property ListaProductos As List(Of Producto)
    Public Property ListaTipoCaja As List(Of TipoCaja)
    Public Property ListaTipoPalet As List(Of TipoPalet)
    Public Property idCabecera As Long


    Public Sub New(Optional linea As LineaVale = Nothing, Optional Editar As Boolean = False)
        InitializeComponent()

        If linea Is Nothing Then
            linea = New LineaVale()
        End If
        LineaActual = linea
        EsEdicion = Editar
        idCabecera = linea.idCab_lva
        Debug.WriteLine("Hash recibido en vista padre: " & LineaActual.GetHashCode())

        CargarCombos()
        If EsEdicion Then
            CargarDatosEnPantalla()
        End If
    End Sub
    Private Sub CargarDatosEnPantalla()
        txtCProd.Text = LineaActual.prod_lva
        txtPneto.Text = If(LineaActual.pNeto_lva.HasValue, LineaActual.pNeto_lva.ToString(), "")
        txtCajas.Text = If(LineaActual.cajas_lva.HasValue, LineaActual.cajas_lva.ToString(), "")
        txtNPalets.Text = If(LineaActual.nPalet_lva.HasValue, LineaActual.nPalet_lva.ToString(), "")
        txtTotKilos.Text = If(LineaActual.totkil_lva.HasValue, LineaActual.nPalet_lva.ToString(), "")
        cmbProductos.SelectedValue = LineaActual.prod_lva
        cmbTipCaja.SelectedValue = LineaActual.tipCaja_lva
        cmbTipPalet.SelectedValue = LineaActual.tipPalet_lva
        Debug.WriteLine("Valor seleccionado en cmbTipCaja: " & cmbTipCaja.SelectedValue)
    End Sub

    Private Sub CargarCombos()
        Dim servicio As New ServicioGlobal()
        ListaTipoCaja = servicio.ObtenerTipoCaja()
        ListaTipoPalet = servicio.ObtenerTipoPalet()

        Dim prodService As New ProductoService()
        ListaProductos = prodService.ObtenerProductos()

        cmbProductos.ItemsSource = ListaProductos
        cmbProductos.DisplayMemberPath = "des_prod"
        cmbProductos.SelectedValuePath = "cod_prod"

        cmbTipCaja.ItemsSource = ListaTipoCaja
        cmbTipCaja.DisplayMemberPath = "TipoCaja_tCaja"
        cmbTipCaja.SelectedValuePath = "idCodigo_tCaja"

        cmbTipPalet.ItemsSource = ListaTipoPalet
        cmbTipPalet.DisplayMemberPath = "TipoPalet_tPal"
        cmbTipPalet.SelectedValuePath = "idCodigo_tPal"
        Debug.WriteLine("TipCaja disponibles:")
        For Each item In cmbTipCaja.Items
            ' Por ejemplo, si es de tipo TipoCaja
            Dim tipoCaja As TipoCaja = TryCast(item, TipoCaja)
            If tipoCaja IsNot Nothing Then
                Debug.WriteLine("ID: " & tipoCaja.idCodigo_tCaja & " - Descripción: " & tipoCaja.TipoCaja_tCaja)
            End If
        Next

    End Sub

    Private Async Sub BtnGuardar_Click(sender As Object, e As RoutedEventArgs)

        If Not ValidarYRecogerDatos() Then
            Exit Sub
        End If

        Try
            Dim servicio As New LineasValeService()

            If EsEdicion Then

                servicio.ActualizarLinea(LineaActual)
            Else
                ' Generamos nuevo id si hace falta
                LineaActual.idLinea_lva = Await servicio.ObtenerNuevoIdLinea(LineaActual.idCab_lva)
                servicio.InsertarLinea(LineaActual)
            End If

            Me.DialogResult = True
            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Error al guardar la línea: " & ex.Message)
        End Try
    End Sub



    Private Sub BtnCancelar_Click(sender As Object, e As RoutedEventArgs)
        Me.DialogResult = False
        Me.Close()
    End Sub

    Private Sub cmbProductos_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cmbProductos.SelectionChanged
        Dim combo As ComboBox = CType(sender, ComboBox)
        Dim productoSeleccionado As Producto = TryCast(combo.SelectedItem, Producto)

        If productoSeleccionado IsNot Nothing Then
            txtCProd.Text = productoSeleccionado.cod_prod
            txtPneto.Text = productoSeleccionado.peso.ToString()
            CalcularTotKilos(cmbProductos)

            ' Si estás editando directamente LineaActual también puedes hacer:
            'LineaActual.prod_lva = productoSeleccionado.cod_prod
            '"LineaActual.pNeto_lva = productoSeleccionado.peso
        End If
    End Sub
    Private Function ValidarYRecogerDatos() As Boolean
        ' Validación directa sobre los campos de la pantalla
        Try
            If String.IsNullOrWhiteSpace(txtCProd.Text) Then
                MessageBox.Show("Debes seleccionar un producto.")
                Return False
            End If

            Dim pneto As Integer
            If Not Integer.TryParse(txtPneto.Text, pneto) Then
                MessageBox.Show("Peso Neto inválido.")
                Return False
            End If

            Dim cajas As Integer
            If Not Integer.TryParse(txtCajas.Text, cajas) Then
                MessageBox.Show("Número de cajas inválido.")
                Return False
            End If

            Dim npalet As Integer
            If Not Integer.TryParse(txtNPalets.Text, npalet) Then
                npalet = 0 ' O puedes mostrar un warning
            End If

            ' Asignación directa a LineaActual
            LineaActual.idCab_lva = idCabecera
            LineaActual.prod_lva = txtCProd.Text
            LineaActual.pNeto_lva = pneto
            LineaActual.cajas_lva = cajas
            Debug.WriteLine("Hash de LineaActual: " & LineaActual.GetHashCode())
            LineaActual.pNeto_lva = pneto
            Debug.WriteLine("Asignado pNeto: " & pneto.ToString())
            Debug.WriteLine("LineaActual.pNeto_lva = " & If(LineaActual.pNeto_lva.HasValue, LineaActual.pNeto_lva.Value.ToString(), "NULO"))
            LineaActual.nPalet_lva = npalet
            LineaActual.tipCaja_lva = If(cmbTipCaja.SelectedValue IsNot Nothing, CStr(cmbTipCaja.SelectedValue), Nothing)
            LineaActual.tipPalet_lva = If(cmbTipPalet.SelectedValue IsNot Nothing, CStr(cmbTipPalet.SelectedValue), Nothing)

            ' Puedes calcular totkil aquí si lo deseas
            LineaActual.totkil_lva = pneto * cajas

            Return True
        Catch ex As Exception
            MessageBox.Show("Error asignando datos " & ex.Message)
            Return False
        End Try
    End Function

    Private Sub txtCajas_LostFocus(sender As Object, e As RoutedEventArgs) Handles txtCajas.LostFocus

        CalcularTotKilos(txtCajas)

        txtNPalets.Focus()
    End Sub
    Private Sub CalcularTotKilos(sender As Object)
        Dim pneto As Integer
        Dim cajas As Integer

        If Integer.TryParse(txtPneto.Text, pneto) AndAlso Integer.TryParse(txtCajas.Text, cajas) Then
            Dim total As Integer = pneto * cajas
            txtTotKilos.Text = total.ToString()
            LineaActual.totkil_lva = total
        Else
            txtTotKilos.Text = ""
            LineaActual.totkil_lva = Nothing
        End If

        ' Si el que pierde el foco es txtCajas, mueve el foco a txtNPalets

    End Sub


    Private Sub LineaValeWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        AsociarComportamientoComboBox(Me)
    End Sub

    Private Sub txtCProd_LostFocus(sender As Object, e As RoutedEventArgs) Handles txtCProd.LostFocus
        Dim codigoIngresado As String = txtCProd.Text.Trim()
        Dim productoEncontrado = ListaProductos.FirstOrDefault(Function(p) p.cod_prod.Equals(codigoIngresado, StringComparison.OrdinalIgnoreCase))

        If productoEncontrado IsNot Nothing Then
            cmbProductos.SelectedValue = productoEncontrado.cod_prod
        Else
            cmbProductos.SelectedIndex = -1
            MessageBox.Show("Producto no encontrado", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning)

        End If
    End Sub
End Class
