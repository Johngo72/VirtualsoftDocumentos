namespace SicaVS
{
    public class InfValidacion
    {
        public int idRegistro { get; set; }
        public string documentoCliente { get; set; } = string.Empty;
        public string tokenEmpresa { get; set; } = string.Empty;        
        public string datos { get; set; } = "NA";
    }
    public class InfRegistro
    {
        public int Id { get; set; }
        public string DocumentoCliente{ get; set; } = string.Empty;
        public string ClaveRegistro { get; set; } = string.Empty;
        public int IdRegistro { get; set; }
        public string TokenEmpresa { get; set; } = string.Empty;
        public string datos { get; set; } = "NA";
        public string EndPoint { get; set; } = "NA";
        public string TipoEnPoint { get; set; } = "";
        public int EstadoConexion { get; set; }
        public string MensajeConexion { get; set; } = "OK";
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
