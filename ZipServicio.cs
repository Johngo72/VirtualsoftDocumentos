using System.IO.Compression;

namespace SicaVS
{
    public class ZipServicio
    {
        public void CrearZipConArchivos(string zipFilePath, string[] filePaths)
        {
            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }
            using (ZipArchive zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (string filePath in filePaths)
                {
                    if (File.Exists(filePath))
                    {
                        zip.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
                    }
                    else
                    {
                        throw new FileNotFoundException($"El archivo '{filePath}' no existe.");
                    }
                }
            }
        }
    }
}
