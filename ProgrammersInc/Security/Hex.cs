using System;
using System.IO;

namespace ProgrammersInc.Security
{
    /// <summary>
    /// Clase utilizada para codificar y decodificar datos
    /// encriptados en Sistema Hexadecimal.
    /// </summary>
    public sealed class Hex
    {
        static readonly IEncoder encoder = new HexEncoder();

        #region Public Methods
        /// <summary>
        /// Codifica los datos de entrada produciendo una matr�z de bytes
        /// codificados en Sistema Hexadecimal.
        /// </summary>
        /// <param name="data">Datos a convertir.</param>
        /// <returns>Una matr�z de bytes codificado en Sistema Hexadecimal.</returns>
        public static byte[] Encode(byte[] data)
        {
            return Encode(data, 0, data.Length);
        }

        /// <summary>
        /// Codifica los datos de entrada produciendo una matr�z de bytes
        /// codificados en Sistema Hexadecimal y lo escribe en un Stream dado.
        /// </summary>
        /// <param name="data">Datos a convertir.</param>
        /// <param name="stream">Objeto <see cref="System.IO.Stream"/> a implementar
        /// para escribir el resultado de la encriptaci�n.</param>
        /// <returns>Retorna el n�mero de bytes producidos.</returns>
        public static int Encode(byte[] data, Stream stream)
        {
            return Encode(data, 0, data.Length, stream);
        }

        /// <summary>
        /// Codifica los datos de entrada produciendo una matr�z de bytes
        /// codificados en Sistema Hexadecimal.
        /// </summary>
        /// <param name="data">Datos a convertir.</param>
        /// <param name="off">Punto inicial de conversi�n.</param>
        /// <param name="length">Tama�o de los datos a encriptar.</param>
        /// <returns>Una matr�z de bytes codificado en Sistema Hexadecimal.</returns>
        public static byte[] Encode(byte[] data, int off, int length)
        {
            MemoryStream memoryStream = new MemoryStream(length * 2);

            Encode(data, off, length, memoryStream);

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Codifica los datos de entrada produciendo una matr�z de bytes
        /// codificados en Sistema Hexadecimal.
        /// </summary>
        /// <param name="data">Datos a convertir.</param>
        /// <param name="off">Punto inicial de conversi�n.</param>
        /// <param name="length">Tama�o de los datos a encriptar.</param>
        /// <param name="stream">Objeto <see cref="System.IO.Stream"/> a implementar
        /// para escribir el resultado de la encriptaci�n.</param>
        /// <returns>Una matr�z de bytes codificado en Sistema Hexadecimal.</returns>
        public static int Encode(byte[] data, int off, int length, Stream stream)
        {
            return encoder.Encode(data, off, length, stream);
        }

        /// <summary>
        /// Decodificada los datos de entrada codificados en Sistema Hexadecimal.
        /// Se asume que los datos de entrada son datos validos.
        /// </summary>
        /// <param name="data">Datos a decodificar.</param>
        /// <returns>Una matr�z de bytes representando los datos decodificados.</returns>
        public static byte[] Decode(byte[] data)
        {
            MemoryStream memoryStream = new MemoryStream((data.Length + 1) / 2);

            encoder.Decode(data, 0, data.Length, memoryStream);

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Decodificada una cadena de texto codificada en Sistema Hexadecimal.
        /// Los espacios en blanco ser�n ignorados.
        /// </summary>
        /// <param name="data">Datos a decodificar.</param>
        /// <returns>Una matr�z de bytes representando los datos decodificados.</returns>
        public static byte[] Decode(string data)
        {
            MemoryStream memoryStream = new MemoryStream((data.Length + 1) / 2);

            encoder.DecodeString(data, memoryStream);

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Decodificada una cadena de texto codificada en Sistema Hexadecimal.
        /// Los resultados ser�n escritos en el <see cref="System.IO.Stream"/> dado.
        /// Los espacios en blanco ser�n ignorados.
        /// </summary>
        /// <param name="data">Datos a decodificar.</param>
        /// <param name="stream">Objeto <see cref="System.IO.Stream"/> a implementar
        /// para escribir el resultado de la descodificaci�n.</param>
        /// <returns>Una matr�z de bytes representando los datos decodificados.</returns>
        public static int Decode(string data, Stream stream)
        {
            return encoder.DecodeString(data, stream);
        }
        #endregion
    }
}