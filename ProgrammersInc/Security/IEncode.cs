using System;
using System.IO;

namespace ProgrammersInc.Security
{
    /// <summary>
    /// Codifica y decodifica matrices de bytes (tipicamente del formato
    /// de codificaci�n ASCII de 7-bit).
    /// </summary>
    public interface IEncoder
    {
        /// <summary>
        /// Codifica una matriz de bytes.
        /// </summary>
        /// <param name="data">Matriz de datos a encriptar.</param>
        /// <param name="off">Posici�n inicial de lectura de los datos.</param>
        /// <param name="length">Tama�o de los datos a encriptar.</param>
        /// <param name="stream">Objeto <see cref="System.IO.Stream"/> a implementar
        /// para escribir en memoria el resultado de la encriptaci�n.</param>
        /// <returns>Retorna el n�mero de bytes producidos.</returns>
        int Encode(byte[] data, int off, int length, Stream stream);

        /// <summary>
        /// Descodifica una matriz de bytes codificados en Sistema Hexadecimal.
        /// </summary>
        /// <param name="data">Matriz de datos a descodificar.</param>
        /// <param name="off">Posici�n inicial de lectura de los datos.</param>
        /// <param name="lenght">Tama�o de los datos a encriptar.</param>
        /// <param name="stream">Objeto <see cref="System.IO.Stream"/> a implementar
        /// para escribir en memoria el resultado de la descodificaci�n.</param>
        /// <returns>Retorna el n�mero de bytes producidos.</returns>
        int Decode(byte[] data, int off, int lenght, Stream stream);

        /// <summary>
        /// Decodificada una cadena de texto codificada en Sistema Hexadecimal.
        /// Ecribiendo los resultado en el <see cref="System.IO.Stream"/> dado.
        /// </summary>
        /// <param name="data">Datos a decodificar.</param>
        /// <param name="stream">Objeto <see cref="System.IO.Stream"/> a implementar
        /// para escribir en memoria el resultado de la descodificaci�n.</param>
        /// <returns>Retorna el n�mero de bytes producidos.</returns>
        int DecodeString(string data, Stream stream);
    }
}