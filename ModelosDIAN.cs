namespace SicaVS
{
    public class InfValidacion
    {
        public int idRegistro { get; set; }
        public string documentoEmisor { get; set; } = string.Empty;
        public string tokenEmpresa { get; set; } = string.Empty;        
        public string datos { get; set; } = "NA";
    }
    public class InfRegistro
    {
        public int Id { get; set; }
        public string DocumentoEmisor { get; set; } = string.Empty;
        public string TokenEmpresa { get; set; } = string.Empty;
        public string Basedatos { get; set; } = "NA";
        public int EstadoConexion { get; set; }
        public string MensajeConexion { get; set; } = "OK";
        public string ResololucionDIAN{ get; set; } = string.Empty;
        public string Pefijo { get; set; } = "";
        public string FechaInicioResolucion { get; set; } = "";
        public string FechaFinResolucion { get; set; } = "";
        public string RangoInicial { get; set; } = string.Empty;
        public string RangoFinal { get; set; } = string.Empty;
        public string CorreoEnvioFacturas { get; set; } = "NA";
        public string CorreoToken { get; set; } = "";
        public string ServidorCorreo { get; set; } = "";
        public string PuertoServidorCorreo { get; set; } = "";
        public string SSL { get; set; } = "";
        public string SoftwareID { get; set; } = "";
        public string ClaveTecnica { get; set; } = "";
        public string SoftwareDianPing { get; set; } = "";
        public string TestSetID { get; set; } = "";
        public string TaxLevel { get; set; } = "";
        public string FechaFinCerficado { get; set; } = "";
        public string TokenCertificado { get; set; } = string.Empty;
    }
    public class InfEstado
    {
        public string? CodigoDocumento{ get; set; }
        public string? TipoOperacion { get; set; }
    }
    public class InfDocumento
    {
        public string? CodigoDocumento { get; set; }
        public string? DocumentoTipo { get; set; }
        public string? DocumentoContribuyente{ get; set; }
        public string? DireccionContribuyente { get; set; }
        public string? DocumentoCliente{ get; set; }
        public string? DireccionCliente { get; set; }
        public string? CiudadCliente { get; set; }
    }

}
