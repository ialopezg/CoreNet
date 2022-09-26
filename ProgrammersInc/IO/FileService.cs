using System;
using System.IO;

namespace ProgrammersInc.IO
{
    /// <summary>
    /// Clase  que define metodos para el tratamiento de archivos.
    /// </summary>
    public sealed class FileService
    {
        /// <summary>
        /// Obtiene el nombre de un archivo sin incluir el path.
        /// </summary>
        /// <param name="filename">Ruta completa del archivo a evaluar.</param>
        /// <returns>Un System.String que representara el nombre del archivo.</returns>
        public static string GetFileName(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException("FileName");

            return new FileInfo(filename).Name;
        }

        /// <summary>
        /// Convierte un archivo en un array de bytes.
        /// </summary>
        /// <param name="fileName">Ruta del archivo a convertir.</param>
        /// <returns>Un array de bytes representando el archivo codificado.</returns>
        public static byte[] FileToBytes(string fileName)
        {
            try
            {
                if (!File.Exists(fileName))
                    throw new ArgumentNullException("fileName");

                FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read);
                byte[] result = new byte[fileStream.Length - 1];
                fileStream.Read(result, 0, (int)fileStream.Length - 1);
                fileStream.Close();

                return result;
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Convierte un objeto stream a una secuencia de bytes.
        /// </summary>
        /// <param name="stream">Objeto a convertir.</param>
        /// <returns>La secuencia de bytes.</returns>
        public static byte[] FileToBytes(Stream stream)
        {
            try
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                byte[] result = new byte[stream.Length - 1];
                stream.Read(result, 0, (int)stream.Length - 1);
                stream.Close();

                return result;
            }
            catch (Exception) { return null; }
        }
    }
}