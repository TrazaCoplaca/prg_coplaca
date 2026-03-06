Imports System.ComponentModel

Public Class LineaVale
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub OnPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

    ' Propiedades normales
    Private _idCoop_lva As String
    Public Property idCoop_lva As String
        Get
            Return _idCoop_lva
        End Get
        Set(value As String)
            If _idCoop_lva <> value Then
                _idCoop_lva = value
                OnPropertyChanged(NameOf(idCoop_lva))
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
                OnPropertyChanged(NameOf(idEmpa_lva))
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
                OnPropertyChanged(NameOf(idCab_lva))
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
                OnPropertyChanged(NameOf(idLinea_lva))
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
                OnPropertyChanged(NameOf(prod_lva))
            End If
        End Set
    End Property
    Private _tipProducto_lva As String
    Public Property tipProducto_lva As String
        Get
            Return _tipProducto_lva
        End Get
        Set(value As String)
            If _tipProducto_lva <> value Then
                _tipProducto_lva = value
                OnPropertyChanged(NameOf(_tipProducto_lva))
            End If
        End Set
    End Property
    ' Propiedades Nullable (¡aquí va la magia!)
    Private _pNeto_lva As Integer?
    Public Property pNeto_lva As Integer?
        Get
            Return _pNeto_lva
        End Get
        Set(value As Integer?)
            If Not Nullable.Equals(_pNeto_lva, value) Then
                _pNeto_lva = value
                OnPropertyChanged(NameOf(pNeto_lva))
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
                OnPropertyChanged(NameOf(cajas_lva))
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
                OnPropertyChanged(NameOf(totkil_lva))
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
                OnPropertyChanged(NameOf(nPalet_lva))
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
                OnPropertyChanged(NameOf(tipPalet_lva))
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
                OnPropertyChanged(NameOf(tipCaja_lva))
            End If
        End Set
    End Property

    Private _traspasado As Integer?
    Public Property traspasado_lva As Integer?
        Get
            Return _traspasado
        End Get
        Set(value As Integer?)
            If Not Nullable.Equals(_traspasado, value) Then
                _traspasado = value
                OnPropertyChanged(NameOf(traspasado_lva))
            End If
        End Set
    End Property

    ' Estado (no nullable)
    Private _estado_lva As String
    Public Property estado_lva As String
        Get
            Return _estado_lva
        End Get
        Set(value As String)
            If _estado_lva <> value Then
                _estado_lva = value
                OnPropertyChanged(NameOf(estado_lva))
            End If
        End Set
    End Property
End Class
