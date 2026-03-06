Imports System.ComponentModel

Public Class VistaLineasVale
    Implements INotifyPropertyChanged

    Private _idCoop_lva As String
    Public Property idCoop_lva As String
        Get
            Return _idCoop_lva
        End Get
        Set(value As String)
            If _idCoop_lva <> value Then
                _idCoop_lva = value
                RaisePropertyChanged(NameOf(idCoop_lva))
            End If
        End Set
    End Property

    Private _idEmpa_lva As String
    Public Property idEmpa_lva As String
        Get
            Return _idEmpa_lva
        End Get
        Set(value As String)
            If _idEmpa_lva <> value Then
                _idEmpa_lva = value
                RaisePropertyChanged(NameOf(idEmpa_lva))
            End If
        End Set
    End Property

    Private _idCab_lva As Long
    Public Property idCab_lva As Long
        Get
            Return _idCab_lva
        End Get
        Set(value As Long)
            If _idCab_lva <> value Then
                _idCab_lva = value
                RaisePropertyChanged(NameOf(idCab_lva))
            End If
        End Set
    End Property

    Private _idLinea_lva As Integer
    Public Property idLinea_lva As Integer
        Get
            Return _idLinea_lva
        End Get
        Set(value As Integer)
            If _idLinea_lva <> value Then
                _idLinea_lva = value
                RaisePropertyChanged(NameOf(idLinea_lva))
            End If
        End Set
    End Property

    Private _prod_lva As String
    Public Property prod_lva As String
        Get
            Return _prod_lva
        End Get
        Set(value As String)
            If _prod_lva <> value Then
                _prod_lva = value
                RaisePropertyChanged(NameOf(prod_lva))
            End If
        End Set
    End Property

    Private _desProd_lva As String
    Public Property desProd_lva As String
        Get
            Return _desProd_lva
        End Get
        Set(value As String)
            If _desProd_lva <> value Then
                _desProd_lva = value
                RaisePropertyChanged(NameOf(desProd_lva))
            End If
        End Set
    End Property

    Private _pNeto_lva As Integer?
    Public Property pNeto_lva As Integer?
        Get
            Return _pNeto_lva
        End Get
        Set(value As Integer?)
            If Not Nullable.Equals(_pNeto_lva, value) Then
                _pNeto_lva = value
                RaisePropertyChanged(NameOf(pNeto_lva))
                CalcularTotKilos()
            End If
        End Set
    End Property

    Private _cajas_lva As Integer?
    Public Property cajas_lva As Integer?
        Get
            Return _cajas_lva
        End Get
        Set(value As Integer?)
            If Not Nullable.Equals(_cajas_lva, value) Then
                _cajas_lva = value
                RaisePropertyChanged(NameOf(cajas_lva))
                CalcularTotKilos()
            End If
        End Set
    End Property

    Private _totkil_lva As Integer?
    Public Property totkil_lva As Integer?
        Get
            Return _totkil_lva
        End Get
        Set(value As Integer?)
            If Not Nullable.Equals(_totkil_lva, value) Then
                _totkil_lva = value
                RaisePropertyChanged(NameOf(totkil_lva))
            End If
        End Set
    End Property

    Private _estado_lva As String
    Public Property estado_lva As String
        Get
            Return _estado_lva
        End Get
        Set(value As String)
            If _estado_lva <> value Then
                _estado_lva = value
                RaisePropertyChanged(NameOf(estado_lva))
            End If
        End Set
    End Property

    Private _nPalet_lva As Integer?
    Public Property nPalet_lva As Integer?
        Get
            Return _nPalet_lva
        End Get
        Set(value As Integer?)
            If Not Nullable.Equals(_nPalet_lva, value) Then
                _nPalet_lva = value
                RaisePropertyChanged(NameOf(nPalet_lva))
            End If
        End Set
    End Property

    Private _tipPalet_lva As String
    Public Property tipPalet_lva As String
        Get
            Return _tipPalet_lva
        End Get
        Set(value As String)
            If Not Nullable.Equals(_tipPalet_lva, value) Then
                _tipPalet_lva = value
                RaisePropertyChanged(NameOf(tipPalet_lva))
            End If
        End Set
    End Property

    Private _tipCaja_lva As String
    Public Property tipCaja_lva As String
        Get
            Return _tipCaja_lva
        End Get
        Set(value As String)
            If Not Nullable.Equals(_tipCaja_lva, value) Then
                _tipCaja_lva = value
                RaisePropertyChanged(NameOf(tipCaja_lva))
            End If
        End Set
    End Property

    Private _desPalet_lva As String
    Public Property desPalet_lva As String
        Get
            Return _desPalet_lva
        End Get
        Set(value As String)
            If _desPalet_lva <> value Then
                _desPalet_lva = value
                RaisePropertyChanged(NameOf(desPalet_lva))
            End If
        End Set
    End Property

    Private _desCaja_lva As String
    Public Property desCaja_lva As String
        Get
            Return _desCaja_lva
        End Get
        Set(value As String)
            If _desCaja_lva <> value Then
                _desCaja_lva = value
                RaisePropertyChanged(NameOf(desCaja_lva))
            End If
        End Set
    End Property

    Private _KilosCaja As Long?
    Public Property KilosCaja As Long?
        Get
            Return _KilosCaja
        End Get
        Set(value As Long?)
            If Not Nullable.Equals(_KilosCaja, value) Then
                _KilosCaja = value
                RaisePropertyChanged(NameOf(KilosCaja))
            End If
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private Sub RaisePropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    Private Sub CalcularTotKilos()
        If pNeto_lva.HasValue AndAlso cajas_lva.HasValue Then
            totkil_lva = pNeto_lva.Value * cajas_lva.Value
        Else
            totkil_lva = Nothing
        End If
    End Sub

End Class
