Public Class Configuracion
    Public Property id_conf As String           ' nvarchar(4)
    Public Property idCoop_conf As String       ' nvarchar(2)
    Public Property idEmpa_conf As String       ' nvarchar(2)
    Public Property NomEmp_conf As String       ' nvarchar(100)
    Public Property Dire_Conf As String         ' nvarchar(100)
    Public Property Direc2_conf As String       ' nvarchar(100)
    Public Property isla_conf As String         ' nvarchar(2)
    Public Property SoloPlatanos_conf As Boolean? ' bit (nullable)
    Public Property global_conf As Boolean?
End Class
