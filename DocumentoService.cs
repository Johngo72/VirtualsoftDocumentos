namespace SicaVS
{
    public class DocumentoService
    {
        private readonly string baseDirectory;

        public DocumentoService(string baseDirectory)
        {
            if (string.IsNullOrEmpty(baseDirectory))
            {
                throw new ArgumentException("La base del directorio no puede ser nula o vacía", nameof(baseDirectory));
            }
            this.baseDirectory = baseDirectory;
        }

        public string GenerarRutaDocumento(string nitCliente, string tipoDocumento, string idDocumento,int swFormato)
        {
            // Validar los parámetros de entrada
            if (string.IsNullOrEmpty(nitCliente))
            {
                throw new ArgumentException("El NIT del cliente no puede ser nulo o vacío", nameof(nitCliente));
            }
            
            if (string.IsNullOrEmpty(tipoDocumento))
            {
                throw new ArgumentException("El tipo de documento debe ser 1 (Factura), 2 (Nota de crédito) o 3 (Documento soporte)", nameof(tipoDocumento));
            }

            if (string.IsNullOrEmpty(idDocumento))
            {
                throw new ArgumentException("El Id del documento debe ser un número positivo)", nameof(idDocumento));
            }

            // Determinar el tipo de documento
            string tipoDoc;
            switch (tipoDocumento)
            {
                case "01":
                    tipoDoc = "factura";
                    break;
                case "02":
                    tipoDoc = "notacredito";
                    break;
                case "03":
                    tipoDoc = "documentosoporte";
                    break;
                default:
                    throw new InvalidOperationException("Tipo de documento no válido");
            }

            string extension;
            switch (swFormato)
            {
                case 1:
                    extension = "pdf";
                    break;
                case 2:
                    extension = "xml";
                    break;
                case 3:
                    extension = "zip";
                    break;
                default:
                    throw new InvalidOperationException("Tipo de archivo no válido");
            }


            // Construir la ruta de la carpeta del cliente
            string clienteFolder = Path.Combine(baseDirectory, "Cliente" + nitCliente);

            // Validar si la carpeta del cliente existe y crearla si no existe
            if (!Directory.Exists(clienteFolder))
            {
                Directory.CreateDirectory(clienteFolder);
            }

            // Construir la ruta completa del documento
            //string rutaDocumento = Path.Combine(clienteFolder, tipoDoc, idDocumento.ToString());

            // Asegurarse de que la carpeta del tipo de documento también exista
            string tipoDocFolder = Path.Combine(clienteFolder, tipoDoc);
            if (!Directory.Exists(tipoDocFolder))
            {
                Directory.CreateDirectory(tipoDocFolder);
            }

            string nombreArchivo = $"{idDocumento}";

            // Construir la ruta del directorio del archivo
            string archivoFolder = Path.Combine(tipoDocFolder, nombreArchivo);
            if (!Directory.Exists(archivoFolder))
            {
                Directory.CreateDirectory(archivoFolder);
            }


            string rutaDocumento = Path.Combine(archivoFolder, $"{nombreArchivo}.{extension}");


           // string nombreArchivo = $"{idDocumento}.{extension}";

            // Construir la ruta completa del archivo
            //string rutaDocumento = Path.Combine(tipoDocFolder, nombreArchivo);

            return rutaDocumento;
        }
    }
}
