using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SicaVS.Helper;
using SicaVS.Modelos;
using System.Data;
using System.Xml.Serialization;
namespace SicaVS.Controllers
{    
    [ApiController]
    [Route("dcdian")]
    public class DocumentoElectronicos : Controller
    {
        private readonly JwtHelper _jwtHelper;
        public IConfiguration _configutation;
        private readonly string _cngStr;
        public DocumentoElectronicos(IConfiguration configuration,JwtHelper jwtHelper)
        {
            _jwtHelper= jwtHelper;
            _configutation = configuration;
            _cngStr = configuration.GetConnectionString("cnStr");
        }

        [Authorize]
        [HttpPost]
        [Route("enviar")]
        public dynamic enviarDocumento(InfDocumento DocInf)
        {
            string token = Request.Headers.Where(x => x.Key == "Authorization").FirstOrDefault().Value;
            var claims = _jwtHelper.InfToken(token);                     
            InfRegistro DatosConexion =new InfRegistro();
            DatosConexion = _jwtHelper.GetClaimValidacion(claims);
            if (DatosConexion.EstadoConexion == 0)
            {
                return new
                {
                    success = false,
                    message = "Datos de validación incorrectos",
                    result = "No registrar información valida "
                };
            }

            //Traer datos
            InvoiceType invoice = new InvoiceType();
            // Asignar valores a las propiedades de InvoiceType
            invoice.ID = new IDType { Value = DocInf.CodigoDocumento };
            invoice.IssueDate = new IssueDateType { Value = DateTime.Now };
            invoice.DocumentCurrencyCode = new DocumentCurrencyCodeType { Value = "USD" };

            invoice.TotalFactura = new TotalFacturaType { Value = 300000 };

            // Crear y asignar las líneas de la factura
            InvoiceLineType line1 = new InvoiceLineType
            {
                ID = new IDType { Value = "1" },
                LineExtensionAmount = new LineExtensionAmountType { Value = 100.00M }
            };

            InvoiceLineType line2 = new InvoiceLineType
            {
                ID = new IDType { Value = "2" },
                LineExtensionAmount = new LineExtensionAmountType { Value = 200.00M }
            };

            string baseDirectory = @"C:\Clientes";
            DocumentoService documentoService = new DocumentoService(baseDirectory);
            string archivoXml = "";
            string archivoPdf = "";
            string zipFilePath = "";
            try
            {
                archivoXml = documentoService.GenerarRutaDocumento(DocInf.DocumentoContribuyente, DocInf.DocumentoTipo, DocInf.CodigoDocumento, 2);
                archivoPdf = documentoService.GenerarRutaDocumento(DocInf.DocumentoContribuyente, DocInf.DocumentoTipo, DocInf.CodigoDocumento, 1);
                zipFilePath = documentoService.GenerarRutaDocumento(DocInf.DocumentoContribuyente, DocInf.DocumentoTipo, DocInf.CodigoDocumento, 3);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            // Asignar las líneas de la factura a la propiedad InvoiceLine de InvoiceType
            invoice.InvoiceLine = new InvoiceLineType[] { line1, line2 };
            SaveToXml(invoice, archivoXml);
            SaveToXml(invoice, archivoPdf);
            ZipServicio documentoZip = new ZipServicio();
            try
            {
                documentoZip.CrearZipConArchivos(zipFilePath, new string[] { archivoXml, archivoPdf });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return new
            {
                success = true,
                message = "Documento enviado correctamente",
                result = DocInf.CodigoDocumento
            };
        }

        [HttpPost]
        [Route("registrar")]
        public dynamic registrarCliente([FromBody] InfValidacion ValInf)
        {
            string jwtToken = "";
            string tempToken = "";
            int SwRegistrarToken = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(_cngStr))
                {
                    connection.Open();
                    string query = "SELECT Id, DocumentoEmisor, TokenEmpresa, BaseDatos,TokenRegistro FROM ResolucionesClientes WHERE Id = @Id AND TokenEmpresa = @TokenEmpresa";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", ValInf.idRegistro);
                        command.Parameters.AddWithValue("@TokenEmpresa", ValInf.tokenEmpresa);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                InfRegistro resolucion = new InfRegistro
                                {
                                    Id= reader.GetInt32(0),
                                    DocumentoEmisor = reader.GetString(1),
                                    TokenEmpresa = reader.GetString(2),
                                    Basedatos = reader.GetString(3),
                                };
                                jwtToken = reader.GetString(4);
                                if (jwtToken.Length<5){jwtToken = _jwtHelper.GenerarToken(resolucion); SwRegistrarToken = 1; }                           
                            }
                            else
                            {
                                return new
                                {
                                    success = true,
                                    message = "No se encontró la resolución con los parámetros proporcionados.",
                                    result = "Validación incorrecta"
                                };
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return new
                {
                    success = false,
                    message = "Error interno.",
                    result = "Consultar con el proveedor"
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    success = false,
                    message = "Error interno.",
                    result = "Consultar con el proveedor"
                };
            }
            if(SwRegistrarToken==1) {
                using (SqlConnection connection = new SqlConnection(_cngStr))
                {
                    connection.Open();
                    string query = "UPDATE ResolucionesClientes SET TokenRegistro = @TokenRegistro WHERE Id = @Id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", ValInf.idRegistro);
                        command.Parameters.AddWithValue("@TokenRegistro", jwtToken);
                        int result = command.ExecuteNonQuery();
                    }
                }
            }

            return new
            {
                success = true,
                message = "exito",
                result = jwtToken
            };

        }
                
        [HttpGet]
        [Route("estado")]
        public dynamic consultarEstado(InfEstado EstadoInf)
        {           
            return new
            {
                success = true,
                message = "información devuelta correctamente",
                result = EstadoInf
            };
        }
        [HttpGet]
        [Route("pdf")]
        public dynamic consultarPdf(InfEstado EstadoInf)
        {            
            return new
            {
                success = true,
                message = "cliente eliminado",
                result = EstadoInf
            };
        }
        [HttpGet]
        [Route("xml")]
        public dynamic consultarXml(InfEstado EstadoInf)
        {            
            return new
            {
                success = true,
                message = "cliente eliminado",
                result = EstadoInf
            };
        }
        [HttpGet]
        [Route("adjuntos")]
        public dynamic consultarAnjuntos(InfEstado EstadoInf)
        {
            return new
            {
                success = true,
                message = "cliente eliminado",
                result = EstadoInf
            };
        }

        [HttpPost]
        [Route("enviarcorreo")]
        public dynamic enviarCorreo(InfEstado EstadoInf)
        {
           
            return new
            {
                success = true,
                message = "cliente eliminado",
                result = EstadoInf
            };
        }
        public static void SaveToXml(InvoiceType invoice, string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(InvoiceType));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, invoice);
            }
        }
    }
}
