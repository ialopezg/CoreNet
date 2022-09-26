using System;
using System.IO;

namespace ProgrammersInc.Security
{
    /// <summary>
    /// Clase que permite codificar y descodificar datos en el Sistema Hexadecimal.
    /// </summary>
    public class HexEncoder : IEncoder
    {
        #region Variables Implementation
        /// <summary>
        /// Tabla de codificación.
        /// </summary>
        static readonly byte[] EncodingTable = 
            {
                (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4',
                (byte)'5', (byte)'6', (byte)'7', (byte)'8', (byte)'9',
                (byte)'a', (byte)'b', (byte)'c', (byte)'d', (byte)'e',
                (byte)'f' 
            };
        /// <summary>
        /// Tabla de descodificación.
        /// </summary>
        static readonly byte[] DecodingTable = new byte[128];
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa una instancia de la clase.
        /// </summary>
        static HexEncoder()
        {
            for (int i = 0; i < EncodingTable.Length; i++)
                DecodingTable[EncodingTable[i]] = (byte)i;

            DecodingTable['A'] = DecodingTable['a'];
            DecodingTable['B'] = DecodingTable['b'];
            DecodingTable['C'] = DecodingTable['c'];
            DecodingTable['D'] = DecodingTable['d'];
            DecodingTable['E'] = DecodingTable['e'];
            DecodingTable['F'] = DecodingTable['f'];
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Codifica una matriz de bytes al Sistema Hexadecimal.
        /// </summary>
        /// <param name="data">Matriz de dataos a encriptar.</param>
        /// <param name="off"></param>
        /// <param name="length">Tamaño de los datos a encriptar.</param>
        /// <param name="stream">Objeto <see cref="System.IO.Stream"/> a implementar
        /// para escribir en memoria el resultado de la encriptación.</param>
        /// <returns>Retorna el número de bytes producidos.</returns>
        public int Encode(byte[] data, int off, int length, Stream stream)
        {
            for (int i = off; i < (off + length); i++)
            {
                int v = data[i];
                stream.WriteByte(EncodingTable[v >> 4]);
                stream.WriteByte(EncodingTable[v & 0xf]);
            }

            return length * 2;
        }

        /// <summary>
        /// Decodificada los datos de entrada codificados en Sistema Hexadecimal.
        /// Se asume que los datos de entrada son datos validos.
        /// </summary>
        /// <param name="data">Datos a decodificar.</param>
        /// <param name="off">Posición inicial de lectura de los datos.</param>
        /// <param name="length">Tamaño de los datos a encriptar.</param>
        /// <param name="stream">Objeto <see cref="System.IO.Stream"/> a implementar
        /// para escribir en memoria el resultado de la descodificación.</param>
        /// <returns>Retorna el número de bytes producidos.</returns>
        public int Decode(byte[] data, int off, int length, Stream stream)
        {
            byte b1, b2;
            int outLen = 0;
            int end = off + length;

            while (end > off)
            {
                if (!Ignore((char)data[end - 1]))
                    break;

                end--;
            }

            int i = off;
            while (i < end)
            {
                while (i < end && Ignore((char)data[i]))
                {
                    i++;
                }

                b1 = DecodingTable[data[i++]];

                while (i < end && Ignore((char)data[i]))
                {
                    i++;
                }

                b2 = DecodingTable[data[i++]];

                stream.WriteByte((byte)((b1 << 4) | b2));

                outLen++;
            }

            return outLen;
        }

        /// <summary>
        /// Decodificada una cadena de texto codificada en Sistema Hexadecimal.
        /// Ecribiendo los resultado en el <see cref="System.IO.Stream"/> dado.
        /// </summary>
        /// <param name="data">Datos a decodificar.</param>
        /// <param name="stream">Objeto <see cref="System.IO.Stream"/> a implementar
        /// para escribir en memoria el resultado de la descodificación.</param>
        /// <returns>Retorna el número de bytes producidos.</returns>
        public int DecodeString(string data, Stream stream)
        {
            byte b1, b2;
            int length = 0;

            int end = data.Length;

            while (end > 0)
            {
                if (!Ignore(data[end - 1]))
                    break;

                end--;
            }

            int i = 0;
            while (i < end)
            {
                while (i < end && Ignore(data[i]))
                {
                    i++;
                }

                b1 = DecodingTable[data[i++]];

                while (i < end && Ignore(data[i]))
                {
                    i++;
                }

                b2 = DecodingTable[data[i++]];

                stream.WriteByte((byte)((b1 << 4) | b2));

                length++;
            }

            return length;
        }
        #endregion

        #region Private Methods
        bool Ignore(char c)
        {
            return (c == '\n' || c == '\r' || c == '\t' || c == ' ');
        }
        #endregion
    }
}