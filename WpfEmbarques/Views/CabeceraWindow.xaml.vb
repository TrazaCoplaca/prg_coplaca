Imports System.Collections.ObjectModel
Imports System.Data.Odbc
Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Wordprocessing
Imports InformesQuestPdf
Imports InformesQuestPdf.Models
Imports InformeVale
Imports Syncfusion.Office
Imports WpfEmbarques.ServicioEnvioCorreo

Public Class CabeceraWindow
    Public Property CabeceraDto As CabValeDto = Nothing
    Public Property ListaProductos As List(Of Producto)
    Public Property ListaTipoPalet As List(Of TipoPalet)
    Public Property ListaTipoCaja As List(Of TipoCaja)
    Public Property ListaTransportista As List(Of Transportista)
    Public Property ListaCargador As List(Of Cargador)
    Public Property ListaNaviera As List(Of Naviera)
    Public Property ListaBuque As List(Of Buque)
    Public Property ListaDestino As List(Of Destino)
    Public Property ListarLineas As ObservableCollection(Of VistaLineasVale)
    Public Property ListaIsla As List(Of Isla)
    Private Property observaciones As String

    Private reporsitorio As New CabValeService

    Private DatosGrabados As Boolean = False

    Public Sub New(Optional CabValeDto As CabValeDto = Nothing)
        Dim TemperaturaDefecto = "+12ºC"
        InitializeComponent()
        'Me.DataContext = Me
        dataGridLineas.IsEnabled = False
        CargarComboNavieras(0)
        CargarComboBuques()
        CargarComboCargador()
        CargarComboTransportista()
        CargarComboDestino()
        CargarComboProductos()
        CargarComboTipoCaja()
        CargarComboTipoPalet()
        CargarComboISla()
        Dim buscaCabVale As CabVale = Nothing

        If CabValeDto IsNot Nothing Then
            DatosGrabados = True
            Dim repositorio As New CabValeService() ' Asegúrate de tener el servicio instanciado
            Dim embarque As CabVale = repositorio.ObtenUnaCabecera(CabValeDto.idVale_cva)

            If embarque IsNot Nothing Then

                buscaCabVale = embarque
                CabeceraDto = CabValeDto
                ' Aquí puedes rellenar los campos de tu formulario si hace falta
                txtId.Text = buscaCabVale.idVale_cva.ToString()
                dpFecha.SelectedDate = buscaCabVale.Fecha_cva
                txtMatricula.Text = buscaCabVale.Matr_cva
                txtMatTra.Text = buscaCabVale.Mattrac_cva
                txtSemana.Text = buscaCabVale.Sem_cva
                TxtOrden.Text = buscaCabVale.SemEmb_cva
                txtTemp.Text = buscaCabVale.Temp_cva
                txtNterm.Text = buscaCabVale.NTerm_cva
                txtId.IsEnabled = False
                cmbTransportista.SelectedValue = If(buscaCabVale.Trans_cva.HasValue, buscaCabVale.Trans_cva.Value, 0)
                cmbCargador.SelectedValue = If(buscaCabVale.Cargador_cva.HasValue, buscaCabVale.Cargador_cva.Value, 0)
                cmbNaviera.SelectedValue = If(buscaCabVale.Naviera_cva.HasValue, buscaCabVale.Naviera_cva.Value, 0)
                cmbBuque.SelectedValue = If(buscaCabVale.Buque_cva.HasValue, buscaCabVale.Buque_cva.Value, 0)
                cmbDestino.SelectedValue = If(buscaCabVale.Destino_cva, buscaCabVale.Destino_cva, 0)
                chkFrutSemAnt.IsChecked = If(buscaCabVale.FrutSemAnt_cva, buscaCabVale.FrutSemAnt_cva, False)
                observaciones = buscaCabVale.observaciones_cva.ToString()
                cmbIsla.SelectedValue = If(buscaCabVale.isla_cva.HasValue, buscaCabVale.isla_cva.Value, 0)
                txtOrdCarga.Text = If(buscaCabVale.OrdExp_cva IsNot Nothing, buscaCabVale.OrdExp_cva, "")

                CargarLineasDeCabecera(buscaCabVale.idVale_cva)

            Else
                MessageBox.Show("No se encontró el embarque con el ID especificado.", "Atención", MessageBoxButton.OK, MessageBoxImage.Warning)
            End If
        Else
            DatosGrabados = False
            txtId.Text = reporsitorio.ObtenerSiguienteIdVale(SesionUsuario.CoopActual.Substring(2, 2), SesionUsuario.CoopActual.Substring(0, 2))
            dpFecha.Text = Date.Today
            txtSemana.Text = DameSemana(dpFecha.ToString)
            TxtOrden.Text = reporsitorio.ObtenerNumeroEmbarqueSemana(SesionUsuario.CoopActual.Substring(2, 2), SesionUsuario.CoopActual.Substring(0, 2), DameSemana(dpFecha.Text))
            txtTemp.Text = TemperaturaDefecto
        End If
        ControlCaracteresMaximo()
    End Sub
    Private Sub ControlCaracteresMaximo()
        txtId.MaxLength = 15
        txtMatricula.MaxLength = 15
        txtMatTra.MaxLength = 15
        txtNterm.MaxLength = 15
        txtOrdCarga.MaxLength = 10
        txtSemana.MaxLength = 2
        txtTemp.MaxLength = 10
        TxtOrden.MaxLength = 2


    End Sub
    Private Sub BtnGuardar_Click(sender As Object, e As RoutedEventArgs)
        If GuardarDatos() Then
            MessageBox.Show("Cabecera guardada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information)
            dataGridLineas.IsEnabled = True
        End If

    End Sub
    Private Function GuardarDatos() As Boolean
        Try
            If Not ValidarCampos() Then
                MessageBox.Show("Todos los campos son obligatorios", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning)
                Return False
            Else
                ' Creamos el objeto cabecera
                EnviarDatosAbase()

                Return True
            End If
        Catch ex As Exception
            MessageBox.Show("Error al guardar la cabecera: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            Return False
        End Try
    End Function
    Private Sub EnviarDatosAbase()
        Try
            Dim nuevaCabecera As New CabVale With {
                       .idVale_cva = CLng(txtId.Text),
                       .idCoop_cva = SesionUsuario.CoopActual.Substring(2, 2), ' O puedes usar un TextBox si es editable
                       .idEmpa_cva = SesionUsuario.CoopActual.Substring(0, 2), ' O recuperar de combo si procede
                       .Sem_cva = txtSemana.Text,
                       .Fecha_cva = dpFecha.SelectedDate,
                       .Matr_cva = txtMatricula.Text,
                       .Trans_cva = If(cmbTransportista.SelectedValue IsNot Nothing, CInt(cmbTransportista.SelectedValue), Nothing),
                       .Mattrac_cva = txtMatTra.Text,
                       .Temp_cva = txtTemp.Text,
                       .NTerm_cva = txtNterm.Text,
                       .SemEmb_cva = TxtOrden.Text,
                       .Origen_cva = "", ' Completar si tienes este dato
                       .Cargador_cva = If(cmbCargador.SelectedValue IsNot Nothing, CInt(cmbCargador.SelectedValue), Nothing),
                       .Naviera_cva = If(cmbNaviera.SelectedValue IsNot Nothing, CInt(cmbNaviera.SelectedValue), Nothing),
                       .Buque_cva = If(cmbBuque.SelectedValue IsNot Nothing, CInt(cmbBuque.SelectedValue), Nothing),
                       .Destino_cva = If(cmbDestino.SelectedItem IsNot Nothing, CInt(cmbDestino.SelectedValue), Nothing),
                        .FrutSemAnt_cva = If(chkFrutSemAnt.IsChecked.HasValue, chkFrutSemAnt.IsChecked.Value, False),
                       .observaciones_cva = observaciones,
                       .isla_cva = If(cmbIsla.SelectedValue IsNot Nothing, CInt(cmbIsla.SelectedValue), Nothing),
                       .OrdExp_cva = txtOrdCarga.Text.ToString()
                     }


            ' Guardamos
            Dim servicio As New CabValeService()
            'If Not DatosGrabados Then
            Dim comprobarCabeceera As CabVale = servicio.ObtenUnaCabecera(nuevaCabecera.idVale_cva)
            'If CabeceraDto Is Nothing Then

            If comprobarCabeceera Is Nothing Then
                servicio.InsertarCabecera(nuevaCabecera)
                DatosGrabados = True
                'BcEmbarqueService.EnviarEmbarque(nuevaCabecera, GetAccessToken())
                ' BcEmbarqueService.EnviarDatosCabecera(nuevaCabecera, GetAccessToken)
            Else
                servicio.ActualizarCabecera(nuevaCabecera)
                'BcEmbarqueService.EnviarEmbarque(nuevaCabecera, GetAccessToken())
                'BcEmbarqueService.EnviarDatosCabecera(nuevaCabecera, GetAccessToken)
            End If
        Catch ex As Exception
            MessageBox.Show("Error al guardar la cabecera: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

    End Sub
    Private Sub EnviarDatosBCCoplaca()
        Dim servicio As New CabValeService
        Dim srvLineasVale As New LineasValeService

        Dim comprobarCabecera As CabVale = servicio.ObtenUnaCabecera(txtId.Text)
        Dim comprobarLinea As New LineaVale
        If comprobarCabecera IsNot Nothing Then
            Using BcEmbarqueService.EnviarDatosCabecera(comprobarCabecera, GetAccessToken)

            End Using
            comprobarLinea.idCab_lva = txtId.Text
            Using BcEmbarqueService.EliminarLineasBc(comprobarLinea, GetAccessToken)

            End Using
            Dim lineas As List(Of LineaVale) = srvLineasVale.ObtenerLineasVale(comprobarCabecera.idCoop_cva, comprobarCabecera.idEmpa_cva, txtId.Text)
            If lineas IsNot Nothing Then
                BcEmbarqueService.EnviarDatosLineaBC(lineas, GetAccessToken)



            End If
        End If

    End Sub
    Private Function ValidarCampos() As Boolean
        If String.IsNullOrWhiteSpace(txtId.Text) Then Return False
        If String.IsNullOrWhiteSpace(txtMatricula.Text) Then Return False
        If String.IsNullOrWhiteSpace(txtMatTra.Text) Then Return False
        If String.IsNullOrWhiteSpace(txtSemana.Text) Then Return False
        If String.IsNullOrWhiteSpace(txtTemp.Text) Then Return False
        If String.IsNullOrWhiteSpace(txtNterm.Text) Then Return False
        If dpFecha.SelectedDate Is Nothing Then Return False
        If cmbTransportista.SelectedValue Is Nothing Then Return False
        If cmbCargador.SelectedValue Is Nothing Then Return False
        If cmbNaviera.SelectedValue Is Nothing Then Return False
        If cmbBuque.SelectedValue Is Nothing Then Return False
        If cmbDestino.SelectedValue Is Nothing Then Return False
        If cmbIsla.SelectedValue Is Nothing Then Return False

        Return True
    End Function

    Private Sub CargarLineasDeCabecera(idCabecera As Long)

        Dim servicio As New LineasValeService()
        Dim listaOriginal = servicio.ObtenerLineasDeVista(idCabecera)

        ListarLineas = New ObservableCollection(Of VistaLineasVale)(listaOriginal)
        dataGridLineas.ItemsSource = ListarLineas
        dataGridLineas.IsEnabled = True
    End Sub
    Private Sub BtnCancelar_Click(sender As Object, e As RoutedEventArgs)
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub cmbCargador_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cmbCargador.SelectionChanged

    End Sub

    Private Sub BtnAddLinea_Click(sender As Object, e As RoutedEventArgs)
        If GuardarDatos() Then
            Dim nuevaLinea As New LineaVale With {
         .idCab_lva = CLng(txtId.Text),
         .idCoop_lva = Mid(SesionUsuario.CoopActual, 3, 2),
         .idEmpa_lva = Mid(SesionUsuario.CoopActual, 1, 2)
     }

            Dim ventana As New LineaValeWindow(nuevaLinea, False)
            ventana.idCabecera = txtId.Text
            If ventana.ShowDialog() = True Then
                ' Refrescamos todo el grid directamente desde la base de datos
                CargarLineasDeCabecera(nuevaLinea.idCab_lva)
            End If
        End If
    End Sub

    Private Sub BtnEditLinea_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub BtnDeleteLinea_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub CabeceraWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        AsociarComportamientoComboBox(Me)
    End Sub


    Private Sub dpFecha_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs) Handles dpFecha.SelectedDateChanged
        If dpFecha.SelectedDate.HasValue Then
            txtSemana.Text = DameSemana(dpFecha.Text)
        End If
    End Sub
    Private Function DameSemana(Fecha As DateTime) As Integer
        Dim semana As Integer = Globalization.CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(Fecha, Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
        DameSemana = semana.ToString
    End Function

    Private Sub dpFecha_Initialized(sender As Object, e As EventArgs) Handles dpFecha.Initialized

    End Sub

    Private Sub txtTemp_PreviewTextInput(sender As Object, e As TextCompositionEventArgs) Handles txtTemp.PreviewTextInput
        Dim textoActual = CType(sender, TextBox).Text
        Dim textoPropuesto = textoActual.Insert(CType(sender, TextBox).SelectionStart, e.Text)

        Dim resultado As Double
        e.Handled = Not Double.TryParse(textoPropuesto.Replace(",", "."),
                                        Globalization.NumberStyles.Float,
                                        Globalization.CultureInfo.InvariantCulture,
                                        resultado)
    End Sub
    Private Sub TxtTemp_Pasting(sender As Object, e As DataObjectPastingEventArgs)
        If e.DataObject.GetDataPresent(GetType(String)) Then
            Dim textoPegado = CStr(e.DataObject.GetData(GetType(String)))
            Dim resultado As Double
            If Not Double.TryParse(textoPegado.Replace(",", "."),
                                   Globalization.NumberStyles.Float,
                                   Globalization.CultureInfo.InvariantCulture,
                                   resultado) Then
                e.CancelCommand()
            End If
        Else
            e.CancelCommand()
        End If
    End Sub

    Private Sub txtTemp_LostFocus(sender As Object, e As RoutedEventArgs) Handles txtTemp.LostFocus
        Dim texto As String = txtTemp.Text.Trim()

        ' Intentamos convertirlo a número
        Dim numero As Double
        If Double.TryParse(texto.Replace("°C", "").Trim().Replace(",", "."), Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, numero) Then
            ' Si es válido, lo mostramos con formato "número °C"
            txtTemp.Text = numero.ToString("+0.##", Globalization.CultureInfo.InvariantCulture) & " °C"
        Else
            ' Si no es válido, puedes dejarlo vacío o mostrar advertencia
            txtTemp.Text = ""
            ' MessageBox.Show("Introduce una temperatura válida", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning)
        End If
    End Sub


    Private Function ConvertirVistaEnLinea(vista As VistaLineasVale, idCabecera As Long) As LineaVale
        If vista Is Nothing Then Return Nothing

        Debug.WriteLine($"[Vista] prod: {vista.prod_lva}, pNeto: {vista.pNeto_lva}, cajas: {vista.cajas_lva}, tipCaja: {vista.tipCaja_lva}, tipPalet: {vista.tipPalet_lva}")

        Dim prod = vista.prod_lva
        Dim pneto = If(vista.pNeto_lva.HasValue, vista.pNeto_lva.Value, 0)
        Dim cajas = If(vista.cajas_lva.HasValue, vista.cajas_lva.Value, 0)
        Dim totkilos = If(vista.totkil_lva.HasValue, vista.totkil_lva.Value, 0)
        Dim tipCaja = vista.tipCaja_lva.ToString
        Dim tipPalet = vista.tipPalet_lva.ToString()
        Dim nPalet = If(vista.nPalet_lva.HasValue, vista.nPalet_lva.Value, 0)
        Dim estado = If(String.IsNullOrWhiteSpace(vista.estado_lva), "", vista.estado_lva)
        Dim idLinea = vista.idLinea_lva


        Console.WriteLine($"→ Vista: prod={prod}, pNeto={pneto}, cajas={cajas}, tipCaja={tipCaja}, tipPalet={tipPalet}, idLinea={idLinea}")

        Dim linea As New LineaVale With {
                    .idCoop_lva = Mid(SesionUsuario.CoopActual, 3, 2),
                    .idEmpa_lva = Mid(SesionUsuario.CoopActual, 1, 2),
                    .idCab_lva = idCabecera,
                    .idLinea_lva = idLinea,
                    .prod_lva = prod,
                    .pNeto_lva = pneto,
                    .cajas_lva = cajas,
                    .totkil_lva = totkilos,
                    .estado_lva = estado,
                    .nPalet_lva = nPalet,
                    .tipCaja_lva = tipCaja,
                    .tipPalet_lva = tipPalet,
                    .traspasado_lva = False
                }

        Return linea

    End Function

    Private Sub BtnEditarLinea_Click(sender As Object, e As RoutedEventArgs)
        ' Obtenemos la línea seleccionada desde el DataGrid
        Dim vistaSeleccionada As VistaLineasVale = TryCast(dataGridLineas.SelectedItem, VistaLineasVale)
        If vistaSeleccionada Is Nothing Then
            MessageBox.Show("Selecciona una línea para editar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning)
            Return
        End If

        ' Cargamos la línea real desde la BD
        Dim servicio As New LineasValeService()
        Dim lineaCompleta As LineaVale = servicio.CargarUnaLinea(vistaSeleccionada.idCab_lva, vistaSeleccionada.idLinea_lva)

        If lineaCompleta IsNot Nothing Then
            Dim ventana As New LineaValeWindow(lineaCompleta, True)
            If ventana.ShowDialog() = True Then
                CargarLineasDeCabecera(If(String.IsNullOrEmpty(txtId?.Text), 0, txtId.Text))
            End If
        Else
            MessageBox.Show("No se pudo cargar la línea seleccionada.", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End If
    End Sub

    Private Sub BtnEliminarLinea_Click(sender As Object, e As RoutedEventArgs)
        Dim vistaSeleccionada As VistaLineasVale = TryCast(dataGridLineas.SelectedItem, VistaLineasVale)
        If vistaSeleccionada Is Nothing Then
            MessageBox.Show("Selecciona una línea para eliminar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning)
            Return
        End If

        Dim resultado = MessageBox.Show("¿Estás seguro de que quieres eliminar la línea seleccionada?", "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question)
        If resultado = MessageBoxResult.Yes Then
            Try
                Dim servicio As New LineasValeService()
                servicio.EliminarLinea(vistaSeleccionada.idCab_lva, vistaSeleccionada.idLinea_lva)
                CargarLineasDeCabecera(vistaSeleccionada.idCab_lva)
                MessageBox.Show("Línea eliminada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information)
            Catch ex As Exception
                MessageBox.Show("Error al eliminar la línea: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            End Try
        End If
    End Sub



    Private Function FindVisualChild(Of T As DependencyObject)(parent As DependencyObject) As T
        For i As Integer = 0 To VisualTreeHelper.GetChildrenCount(parent) - 1
            Dim child = VisualTreeHelper.GetChild(parent, i)
            If TypeOf child Is T Then
                Return CType(child, T)
            Else
                Dim result = FindVisualChild(Of T)(child)
                If result IsNot Nothing Then Return result
            End If
        Next
        Return Nothing
    End Function
    Private Sub CargarComboProductos()
        Dim servicio As New ProductoService()
        ListaProductos = servicio.ObtenerProductos()



    End Sub
    Private Sub CargarComboTipoPalet()
        Dim servicio As New ServicioGlobal()
        ListaTipoPalet = servicio.ObtenerTipoPalet()


    End Sub
    Private Sub CargarComboTipoCaja()
        Dim servicio As New ServicioGlobal()
        ListaTipoCaja = servicio.ObtenerTipoCaja()


    End Sub
    Private Sub CargarComboNavieras(buque As Long)

        Dim servicio As New ServicioGlobal()
        Dim navieras = servicio.ObtenerNavieras(buque)

        cmbNaviera.ItemsSource = navieras
        cmbNaviera.DisplayMemberPath = "Des_nav"
        cmbNaviera.SelectedValuePath = "id_nav"
    End Sub
    Private Sub CargarComboISla()

        Dim servicio As New ServicioGlobal()
        Dim Isla = servicio.ObtenerIsla()

        cmbIsla.ItemsSource = Isla
        cmbIsla.DisplayMemberPath = "des_Isla"
        cmbIsla.SelectedValuePath = "id_Isla"
    End Sub
    Private Sub CargarComboBuques()
        Dim servicio As New ServicioGlobal()
        Dim buques = servicio.ObtenerBuques()
        cmbBuque.ItemsSource = buques
        cmbBuque.DisplayMemberPath = "Des_buq"
        cmbBuque.SelectedValuePath = "id_buq"
        CargarComboNavieras(cmbBuque.SelectedItem)

    End Sub
    Private Sub CargarComboCargador()
        Dim servicio As New ServicioGlobal()
        Dim cargador = servicio.ObtenerCargador()
        cmbCargador.ItemsSource = cargador
        cmbCargador.DisplayMemberPath = "Nomb_carg"
        cmbCargador.SelectedValuePath = "id_carg"

    End Sub
    Private Sub CargarComboTransportista()

        Dim servicio As New ServicioGlobal()
        ListaTransportista = servicio.ObtenerTransportista()
        cmbTransportista.ItemsSource = ListaTransportista


    End Sub
    Private Sub CargarComboDestino()
        Dim servicio As New ServicioGlobal()
        Dim Destino = servicio.ObtenerDestino()
        cmbDestino.ItemsSource = Destino
        cmbDestino.DisplayMemberPath = "Des_des"
        cmbDestino.SelectedValuePath = "id_des"

    End Sub

    Private Sub BtnObservaciones_Click(sender As Object, e As RoutedEventArgs)
        Dim ventana As New ObsCabValeWindow(observaciones)
        If ventana.ShowDialog = True Then
            observaciones = ventana.Observaciones
        End If

    End Sub

    Private Sub BtnImprimir_Click(sender As Object, e As RoutedEventArgs)
        EnviarDatosAbase()
        'EnviarDatosBCCoplaca()
        If MsgBox("Desea Enviar los datos a coplaca", vbYesNo) = vbYes Then
            CargarDatosParaImprimir(True)
            Dim srv As New CabEmbarquePruebaService(ConexionDb.connectionString)
            srv.TraspasarDesdeCabVale(txtId.Text, Mid(SesionUsuario.CoopActual, 3, 2), Mid(SesionUsuario.idEmpa, 1, 2))
            Dim srvLin As New LineasEmbarquePruebaService(ConexionDb.connectionString)
            srvLin.TraspasarLineas(txtId.Text, Mid(SesionUsuario.CoopActual, 3, 2), Mid(SesionUsuario.idEmpa, 1, 2))
        Else
            CargarDatosParaImprimir(False)
        End If
        If MsgBox("Desea enviar la prueba directa", vbYesNo) = vbYes Then
            EnviarPruebaDirecta()
        End If
    End Sub

    Private Sub CargarDatosParaImprimir(enviarMail As Boolean)
        Dim datosVarios As New ServicioGlobal
        Dim datosImprimir As New CabValeService
        Dim configuracion As New ConfiguracionService
        Dim _datosImprimir = datosImprimir.ObtenUnaCabecera(txtId.Text)
        Dim _cargador As Cargador = datosVarios.ObtenDatosCargador(cmbCargador.SelectedValue)
        Dim _transportista As Transportista = datosVarios.ObtenNifTransportista(cmbTransportista.SelectedValue)
        Dim _cooperativa As Configuracion = configuracion.ObtenerUnaConfiguracion(SesionUsuario.idEmpa & SesionUsuario.IdCoop)

        Try
            ' Obtener datos de la cabecera desde los controles del formulario
            Dim cabecera As New Dictionary(Of String, String) From {
                {"Grupo", _cooperativa.NomEmp_conf},
                {"Direccion1", _cooperativa.Dire_Conf},
                {"Direccion2", _cooperativa.Direc2_conf},
                {"ID", txtId.Text}, ' comienza la cabecera de datos parte derecha
                {"Matricula", If(txtMatricula.Text IsNot Nothing, txtMatricula.Text, "")},
                {"Transportista", If(cmbTransportista.Text IsNot Nothing, cmbTransportista.Text, "")},
                {"NifTransportista", If(_transportista.Nif_trans IsNot Nothing, _transportista.Nif_trans, "")},
                {"MatriculaTract", If(txtMatTra.Text IsNot Nothing, txtMatTra.Text, "")},
                {"Temperatura", If(txtTemp.Text IsNot Nothing, txtTemp.Text, "")},
                {"Termografo", If(txtNterm.Text IsNot Nothing, txtNterm.Text, "")},
                {"Destino", If(cmbDestino.Text IsNot Nothing, cmbDestino.Text, "")},
                {"Fecha", If(dpFecha.SelectedDate.HasValue, dpFecha.SelectedDate.Value.ToShortDateString(), "")},
                {"Semana", txtSemana.Text},
                {"Orden", TxtOrden.Text},
                {"Cargador", If(cmbCargador.Text IsNot Nothing, cmbCargador.Text, "")},
                {"NifCargador", If(_cargador.nif_carg IsNot Nothing, _cargador.nif_carg, "")},
                {"DirCargador", If(_cargador.Direcc_carg IsNot Nothing, _cargador.Direcc_carg, "")},
                {"CpoCargador", If(_cargador.Cpostal_carg IsNot Nothing, _cargador.Cpostal_carg, "")},
                {"Naviera", If(cmbNaviera.Text IsNot Nothing, cmbNaviera.Text, "")},
                {"Buque", If(cmbBuque.Text IsNot Nothing, cmbBuque.Text, "")},
                {"Receptor", "EUROBANAN,S.L."},
                {"Observaciones", If(observaciones IsNot Nothing, observaciones, "")},
                {"Isla", If(cmbIsla.Text IsNot Nothing, cmbIsla.Text, "")},
                {"Temp2", If(_datosImprimir.Temp2_cva IsNot Nothing, _datosImprimir.Temp2_cva, "")},
                {"nTerm2", If(_datosImprimir.nTerm1_cva IsNot Nothing, _datosImprimir.nTerm1_cva, "")},
                {"nTerm3", If(_datosImprimir.nTerm2_cva IsNot Nothing, _datosImprimir.nTerm2_cva, "")},
                {"OrdExp", If(_datosImprimir.OrdExp_cva IsNot Nothing, _datosImprimir.OrdExp_cva, "")}
            }

            ' Convertir líneas del DataGrid a una lista de diccionarios
            Dim lineas As New List(Of Dictionary(Of String, String))
            Dim _Producto As New ProductoService()
            '  Dim resultado As String = String.Join(",", "123".ToCharArray())


            For Each item As VistaLineasVale In ListarLineas
                _Producto.ObtenerUnProducto(item.prod_lva.ToString)

                lineas.Add(New Dictionary(Of String, String) From {
                    {"Producto", item.prod_lva},
                    {"TipProducto", _Producto.ObtenerUnProducto(item.prod_lva.ToString).tipo_prod},
                    {"Descripcion", item.desProd_lva.ToString()},
                    {"PesoNeto", item.pNeto_lva.ToString()},
                    {"Cajas", item.cajas_lva.ToString()},
                    {"TotalKilos", item.totkil_lva.ToString()},
                    {"NumPalet", item.nPalet_lva.ToString()},
                    {"tipoPalet", item.desPalet_lva.ToString()},
                    {"tipoCaja", item.desCaja_lva.ToString()},
                    {"Asterisco", String.Join(",", _Producto.ObtenerAsterisco(item.prod_lva.ToString).ToString().ToCharArray())}
                    })
            Next

            Dim SrvDatoInteres As New DatoInteresRepository

            Dim LineasDatosInteres As List(Of ClasDatiInteres) = SrvDatoInteres.ObteneListaDatoInteres(_datosImprimir.idVale_cva, _datosImprimir.idCoop_cva, _datosImprimir.idEmpa_cva)






            Dim nombreArchivoPdf = $"InformeVale_{txtId.Text}.pdf"

            Dim rutaPdf = ObtenerRutaTemporal(nombreArchivoPdf)

            Dim informe As New InformeVale()
            GenerarInformeValePDF(cabecera, lineas, LineasDatosInteres, rutaPdf)
            If enviarMail Then
                Dim enviaCorreo As New ServicioEnvioCorreo
                'enviaCorreo.EnviarPdfPorCorreo_GmailMulti(rutaPdf, _cooperativa.NomEmp_conf, "empaquetados@coplaca.org", "jgug sbnj qpxl chgm", New List(Of String) From {"trazacan@gmail.com"})
                enviaCorreo.EnviarPdfDesdeConfig(rutaPdf, _cooperativa.NomEmp_conf, txtId.Text, txtMatricula.Text)

                'MessageBox.Show("Informe generado correctamente en: " & rutaDestino, "Impresión", MessageBoxButton.OK, MessageBoxImage.Information)º
            End If
        Catch ex As Exception
            MessageBox.Show("Error al generar el informe: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
        ' EnviarDatosServidor()
    End Sub
    Private Sub EnviarPruebaDirecta()
        Dim datosVarios As New ServicioGlobal
        Dim datosImprimir As New CabValeService
        Dim configuracion As New ConfiguracionService
        Dim _datosImprimir = datosImprimir.ObtenUnaCabecera(txtId.Text)
        Dim _cargador As Cargador = datosVarios.ObtenDatosCargador(cmbCargador.SelectedValue)
        Dim _transportista As Transportista = datosVarios.ObtenNifTransportista(cmbTransportista.SelectedValue)
        Dim _cooperativa As Configuracion = configuracion.ObtenerUnaConfiguracion(SesionUsuario.idEmpa & SesionUsuario.IdCoop)

        Try
            ' Obtener datos de la cabecera desde los controles del formulario
            Dim cabecera As New Dictionary(Of String, String) From {
                {"Grupo", _cooperativa.NomEmp_conf},
                {"Direccion1", _cooperativa.Dire_Conf},
                {"Direccion2", _cooperativa.Direc2_conf},
                {"ID", txtId.Text}, ' comienza la cabecera de datos parte derecha
                {"Matricula", If(txtMatricula.Text IsNot Nothing, txtMatricula.Text, "")},
                {"Transportista", If(cmbTransportista.Text IsNot Nothing, cmbTransportista.Text, "")},
                {"NifTransportista", If(_transportista.Nif_trans IsNot Nothing, _transportista.Nif_trans, "")},
                {"MatriculaTract", If(txtMatTra.Text IsNot Nothing, txtMatTra.Text, "")},
                {"Temperatura", If(txtTemp.Text IsNot Nothing, txtTemp.Text, "")},
                {"Termografo", If(txtNterm.Text IsNot Nothing, txtNterm.Text, "")},
                {"Destino", If(cmbDestino.Text IsNot Nothing, cmbDestino.Text, "")},
                {"Fecha", If(dpFecha.SelectedDate.HasValue, dpFecha.SelectedDate.Value.ToShortDateString(), "")},
                {"Semana", txtSemana.Text},
                {"Orden", TxtOrden.Text},
                {"Cargador", If(cmbCargador.Text IsNot Nothing, cmbCargador.Text, "")},
                {"NifCargador", If(_cargador.nif_carg IsNot Nothing, _cargador.nif_carg, "")},
                {"DirCargador", If(_cargador.Direcc_carg IsNot Nothing, _cargador.Direcc_carg, "")},
                {"CpoCargador", If(_cargador.Cpostal_carg IsNot Nothing, _cargador.Cpostal_carg, "")},
                {"Naviera", If(cmbNaviera.Text IsNot Nothing, cmbNaviera.Text, "")},
                {"Buque", If(cmbBuque.Text IsNot Nothing, cmbBuque.Text, "")},
                {"Receptor", "EUROBANAN,S.L."},
                {"Observaciones", If(observaciones IsNot Nothing, observaciones, "")},
                {"Isla", If(cmbIsla.Text IsNot Nothing, cmbIsla.Text, "")},
                {"Temp2", If(_datosImprimir.Temp2_cva IsNot Nothing, _datosImprimir.Temp2_cva, "")},
                {"nTerm2", If(_datosImprimir.nTerm1_cva IsNot Nothing, _datosImprimir.nTerm1_cva, "")},
                {"nTerm3", If(_datosImprimir.nTerm2_cva IsNot Nothing, _datosImprimir.nTerm2_cva, "")},
                {"OrdExp", If(_datosImprimir.OrdExp_cva IsNot Nothing, _datosImprimir.OrdExp_cva, "")}
            }

            ' Convertir líneas del DataGrid a una lista de diccionarios
            Dim lineas As New List(Of Dictionary(Of String, String))
            Dim _Producto As New ProductoService()
            '  Dim resultado As String = String.Join(",", "123".ToCharArray())


            For Each item As VistaLineasVale In ListarLineas
                _Producto.ObtenerUnProducto(item.prod_lva.ToString)

                lineas.Add(New Dictionary(Of String, String) From {
                    {"Producto", item.prod_lva},
                    {"TipProducto", _Producto.ObtenerUnProducto(item.prod_lva.ToString).tipo_prod},
                    {"Descripcion", item.desProd_lva.ToString()},
                    {"PesoNeto", item.pNeto_lva.ToString()},
                    {"Cajas", item.cajas_lva.ToString()},
                    {"TotalKilos", item.totkil_lva.ToString()},
                    {"NumPalet", item.nPalet_lva.ToString()},
                    {"tipoPalet", item.desPalet_lva.ToString()},
                    {"tipoCaja", item.desCaja_lva.ToString()},
                    {"Asterisco", String.Join(",", _Producto.ObtenerAsterisco(item.prod_lva.ToString).ToString().ToCharArray())}
                    })
            Next

            Dim SrvDatoInteres As New DatoInteresRepository

            Dim LineasDatosInteres As List(Of ClasDatiInteres) = SrvDatoInteres.ObteneListaDatoInteres(_datosImprimir.idVale_cva, _datosImprimir.idCoop_cva, _datosImprimir.idEmpa_cva)






            Dim nombreArchivoPdf = $"InformeVale_{txtId.Text}.pdf"

            Dim rutaPdf = ObtenerRutaTemporal(nombreArchivoPdf)

            Dim informe As New InformeVale()
            GenerarInformeValePDF(cabecera, lineas, LineasDatosInteres, rutaPdf)

        Catch ex As Exception
            MessageBox.Show("Error al enviar el embarque a COPLACA: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

    End Sub
    Public Function ObtenerRutaTemporal(nombreArchivo As String) As String
        ' Carpeta temporal del sistema + carpeta de tu app
        Dim carpetaTemp As String = Path.Combine(Path.GetTempPath(), "CargaCamionesTemp")

        ' Crear la carpeta si no existe
        If Not Directory.Exists(carpetaTemp) Then
            Directory.CreateDirectory(carpetaTemp)
        End If

        ' Devolver ruta completa del archivo
        Return Path.Combine(carpetaTemp, nombreArchivo)
    End Function

    Private Sub GenerarInformeValePDF(cabeceraDict As Dictionary(Of String, String), lineasDict As List(Of Dictionary(Of String, String)), lineasDatointeres As List(Of ClasDatiInteres), rutaPdf As String)
        Try
            ' Convertir diccionario a DatosCabecera
            Dim datosCabecera As New DatosCabecera With {
                .Grupo = cabeceraDict("Grupo"),
                .Direccion1 = cabeceraDict("Direccion1"),
                .Direccion2 = cabeceraDict("Direccion2"),
                .ID = cabeceraDict("ID"),
                .Matricula = cabeceraDict("Matricula"),
                .Transportista = cabeceraDict("Transportista"),
                .NifTrans = cabeceraDict("NifTransportista"),
                .MatTractora = cabeceraDict("MatriculaTract"),
                .Temperatura = cabeceraDict("Temperatura"),
                .Termografo = cabeceraDict("Termografo"),
                .Destino = cabeceraDict("Destino"),
                .Fecha = cabeceraDict("Fecha"),
                .Semana = cabeceraDict("Semana"),
                .Cargador = cabeceraDict("Cargador"),
                .NifCargador = cabeceraDict("NifCargador"),
                .Direccion = cabeceraDict("DirCargador"),
                .Poblacion = cabeceraDict("CpoCargador"),
                .Naviera = cabeceraDict("Naviera"),
                .Buque = cabeceraDict("Buque"),
                .Receptor = cabeceraDict("Receptor"),
                .DestinoBarco = "Muelle CÁDIZ",
                .Observaciones = cabeceraDict("Observaciones"),
                .Orden = cabeceraDict("Orden"),
                .isla = cabeceraDict("Isla"),
                .Temp2 = cabeceraDict("Temp2"),
                .nTerm2 = cabeceraDict("nTerm2"),
                .nTerm3 = cabeceraDict("nTerm3"),
                .OrdExp = cabeceraDict("OrdExp")
                }

            ' Convertir lista de diccionarios a lista de LineaEmbarque
            Dim lineasVale As New List(Of LineaEmbarque)
            For Each linea In lineasDict
                lineasVale.Add(New LineaEmbarque With {
                .Producto = linea("Producto"),
                .Descripcion = linea("Descripcion"),
                .PesoNeto = linea("PesoNeto"),
                .Cajas = linea("Cajas"),
                .NumPalet = linea("NumPalet"),
                .TotalKilos = linea("TotalKilos"),
                .desCaja = linea("tipoCaja"),
                .desPalet = linea("tipoPalet"),
                .asterisco = linea("Asterisco"),
                .TipoProd = linea("TipProducto")
            })
            Next
            Dim listaConvertida As New List(Of LineaDatosInteres)
            For Each d In lineasDatointeres
                listaConvertida.Add(New LineaDatosInteres With {
                .IdVale = d.idVale_din,
                .IdCoop = d.idCoop_din,
                .IdEmpa = d.idEmpa_din,
                .Nlin = d.Nlin_din,
                .Producto = d.Producto_din,
                .Temp = d.Temp_din,
                .Termografo = d.Termografo_din
                })
            Next


            ' Llamar al generador QuestPDF
            Dim informe As New InformeValeDatosInteres()
            informe.Generar(datosCabecera, lineasVale, listaConvertida, rutaPdf)

        Catch ex As Exception
            MessageBox.Show("Error al generar el PDF: " & ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub cmbNaviera_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cmbNaviera.SelectionChanged

    End Sub

    Private Sub txtTemp_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtTemp.TextChanged

    End Sub

    Private Sub TxtBiTemp_PreviewTextInput(sender As Object, e As TextCompositionEventArgs)
        Dim textoActual = CType(sender, TextBox).Text
        Dim textoPropuesto = textoActual.Insert(CType(sender, TextBox).SelectionStart, e.Text)

        Dim resultado As Double
        e.Handled = Not Double.TryParse(textoPropuesto.Replace(",", "."),
                                        Globalization.NumberStyles.Float,
                                        Globalization.CultureInfo.InvariantCulture,
                                        resultado)
    End Sub

    Private Sub TxtBiTemp_Pasting(sender As Object, e As DataObjectPastingEventArgs)
        If e.DataObject.GetDataPresent(GetType(String)) Then
            Dim textoPegado = CStr(e.DataObject.GetData(GetType(String)))
            Dim resultado As Double
            If Not Double.TryParse(textoPegado.Replace(",", "."),
                                   Globalization.NumberStyles.Float,
                                   Globalization.CultureInfo.InvariantCulture,
                                   resultado) Then
                e.CancelCommand()
            End If
        Else
            e.CancelCommand()
        End If


    End Sub




    Private Sub txtNterm_LostFocus(sender As Object, e As RoutedEventArgs) Handles txtNterm.LostFocus

    End Sub



    Private Sub txtMatricula_LostFocus(sender As Object, e As RoutedEventArgs) Handles txtMatricula.LostFocus
        txtMatricula.Text = txtMatricula.Text.ToUpper()
    End Sub



    Private Sub txtMatTra_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtMatTra.TextChanged

    End Sub

    Private Sub txtMatTra_LostFocus(sender As Object, e As RoutedEventArgs) Handles txtMatTra.LostFocus
        txtMatTra.Text = txtMatTra.Text.ToUpper()
    End Sub

    Private Sub BtnDatosDeInteres(sender As Object, e As RoutedEventArgs)
        Dim ventana As New DatosDeInteresView(txtId.Text, SesionUsuario.CoopActual.Substring(2, 2), SesionUsuario.CoopActual.Substring(0, 2))
        ventana.Owner = Me
        ventana.ShowDialog()
    End Sub

    Private Sub CargarOrden_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub cmbBuque_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cmbBuque.SelectionChanged
        Dim obtenernaviera As New ServicioGlobal
        Dim valor As Long = obtenernaviera.ObtenerNavieraPorBuque(cmbBuque.SelectedValue)
        CargarComboNavieras(valor)
        cmbNaviera.SelectedValue = valor
    End Sub
End Class
