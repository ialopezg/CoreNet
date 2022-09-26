using System;
using System.Data.Common;

namespace ProgrammersInc.Data
{
    /// <summary>
    /// Interfaz base para todos los tipos de origenes de datos en este namespace.
    /// </summary>
    public interface IDataSource : IDisposable
    {
        #region Properties
        /// <summary>
        /// Obtiene el tipo de servidor como actua esta instancia.
        /// </summary>
        ServerType ServerType { get; }
        #endregion

        #region Methods Implementation
        /// <summary>
        /// Agrega un par�meto del tipo <see cref="System.Boolean"/>.
        /// </summary>
        /// <param name="name">Nombre del par�metro.</param>
        /// <param name="value">Valor del par�metro.</param>
        void AddParameter(string name, bool value);

        /// <summary>
        /// Agrega un par�meto del tipo <see cref="System.Byte"/>.
        /// </summary>
        /// <param name="name">Nombre del par�metro.</param>
        /// <param name="value">Valor del par�metro.</param>
        void AddParameter(string name, byte value);

        /// <summary>
        /// Agrega un par�meto del tipo array de <see cref="System.Byte"/> (byte[]).
        /// </summary>
        /// <param name="name">Nombre del par�metro.</param>
        /// <param name="value">Valor del par�metro.</param>
        void AddParameter(string name, byte[] value);

        /// <summary>
        /// Agrega un par�meto del tipo <see cref="System.DateTime"/>.
        /// </summary>
        /// <param name="name">Nombre del par�metro.</param>
        /// <param name="value">Valor del par�metro.</param>
        void AddParameter(string name, DateTime value);

        /// <summary>
        /// Agrega un par�meto del tipo <see cref="System.Decimal"/>.
        /// </summary>
        /// <param name="name">Nombre del par�metro.</param>
        /// <param name="value">Valor del par�metro.</param>
        void AddParameter(string name, decimal value);

        /// <summary>
        /// Agrega un par�meto del tipo <see cref="System.Double"/>.
        /// </summary>
        /// <param name="name">Nombre del par�metro.</param>
        /// <param name="value">Valor del par�metro.</param>
        void AddParameter(string name, double value);

        /// <summary>
        /// Agrega un par�meto del tipo <see cref="System.Single"/>.
        /// </summary>
        /// <param name="name">Nombre del par�metro.</param>
        /// <param name="value">Valor del par�metro.</param>
        void AddParameter(string name, float value);

        /// <summary>
        /// Agrega un par�meto del tipo <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="name">Nombre del par�metro.</param>
        /// <param name="value">Valor del par�metro.</param>
        void AddParameter(string name, int value);

        /// <summary>
        /// Agrega un par�meto del tipo <see cref="System.Int64"/>.
        /// </summary>
        /// <param name="name">Nombre del par�metro.</param>
        /// <param name="value">Valor del par�metro.</param>
        void AddParameter(string name, long value);

        /// <summary>
        /// Agrega un par�meto del tipo <see cref="System.Object"/>.
        /// </summary>
        /// <param name="name">Nombre del par�metro.</param>
        /// <param name="value">Valor del par�metro.</param>
        void AddParameter(string name, object value);

        /// <summary>
        /// Agrega un par�meto del tipo <see cref="System.Int16"/>.
        /// </summary>
        /// <param name="name">Nombre del par�metro.</param>
        /// <param name="value">Valor del par�metro.</param>
        void AddParameter(string name, short value);

        /// <summary>
        /// Agrega un par�meto del tipo <see cref="System.String"/>.
        /// </summary>
        /// <param name="name">Nombre del par�metro.</param>
        /// <param name="value">Valor del par�metro.</param>
        void AddParameter(string name, string value);

        /// <summary>
        /// Confirma la transacci�n a l servicio de datos actual.
        /// </summary>
        void ConfirmTransaction();

        /// <summary>
        /// Deshace una transacci�n en un estado pendiente.
        /// </summary>
        void UndoTransaction();
        #endregion
    }
}