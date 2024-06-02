using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SicaVS.Helper;
using SicaVS.Modelos;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
namespace SicaVS.Controllers
{    
    [ApiController]
    [Route("dcdian")]
    public class DocumentoElectronicos : Controller
    {
        private readonly JwtHelper _jwtHelper;
        public IConfiguration _configutation;
        public DocumentoElectronicos(IConfiguration configuration,JwtHelper jwtHelper)
        {
            _jwtHelper= jwtHelper;
            _configutation = configuration;
        }

        // [Authorize]
        [HttpPost]
        [Route("enviar")]
        public dynamic enviarDocumento(InfDocumento DocInf)
        {
            string token = Request.Headers.Where(x => x.Key == "Authorization").FirstOrDefault().Value;
            var claims = _jwtHelper.InfToken(token);                     
            InfRegistro DatosConexion =new InfRegistro();
            DatosConexion = _jwtHelper.GetClaimModelo(claims);
            
            //string connectionString = "Data Source = 66.70.229.26; Initial Catalog = Sica_Uramax; User ID = sa; pwd=Sica2014";
            string connectionString = "Data Source=66.70.229.26;Initial Catalog=Sica_Uramax;User ID=sa;Password=Sica2014;TrustServerCertificate=True;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM ResolucionesClientes where IdEstado=1";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandTimeout = 60; // Tiempo de espera del comando en segundos
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable dataTable = new DataTable();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                //MessageBox.Show("Error en la consulta de la base de datos: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                //MessageBox.Show("Ocurrió un error: " + ex.Message);
            }



            InvoiceType invoice = new InvoiceType();
            // Asignar valores a las propiedades de InvoiceType
            invoice.ID = new IDType { Value = DocInf.CodigoDocumento };
            invoice.IssueDate = new IssueDateType { Value = DateTime.Now };
            invoice.DocumentCurrencyCode = new DocumentCurrencyCodeType { Value = "USD" };
            
            invoice.TotalFactura = new TotalFacturaType { Value = 300000 };
            InvoiceHelper invoiceHelper = new InvoiceHelper();
            InvoiceFact item = new InvoiceFact() {
                Nzona = "01",
                Frtfte = "Frt",
                Frtfteiva = "0.5",
                factind = 1,
                Fechaini = "2021/05/10",
                Fechafin = "2021/06/11",
                PrefijoFact = "1212441"

            };
            invoiceHelper.CreateInvoice(1212124, item, ref invoice);
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
            string cnStr = "Data Source=66.70.229.26;Initial Catalog=Sica_Uramax;User ID=sa;Password=Sica2014;TrustServerCertificate=True;";
            string jwtToken = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(cnStr))
                {
                    connection.Open();
                    string query = "SELECT Id, DocumentoCliente, TokenEmpresa, BaseDatos FROM ResolucionesClientes WHERE Id = @Id AND TokenEmpresa = @TokenEmpresa";
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
                                    IdRegistro = reader.GetInt32(0),
                                    DocumentoCliente = reader.GetString(1),
                                    TokenEmpresa = reader.GetString(2),
                                    datos = reader.GetString(3)
                                };
                                jwtToken = _jwtHelper.GenerarToken(resolucion);
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
            using (SqlConnection connection = new SqlConnection(cnStr))
            {
                connection.Open();
                string query = "UPDATE ResolucionesClientes SET TokenRegistro = @TokenRegistro WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", ValInf.idRegistro);
                    command.Parameters.AddWithValue("@TokenRegistro", jwtToken);
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return new
                        {
                            success = true,
                            message = "exito",
                            result = jwtToken
                        };
                    }
                    else
                    {
                        return new
                        {
                            success = false,
                            message = "error interno",
                            result = ""
                        };
                    }
                }
            }
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
