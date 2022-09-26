using System;
using System.Data;

namespace ProgrammersInc.IO
{
    /// <summary>
    /// Interfaz implementada por clases de perfil en este namespace.
    /// Representa un perfil normal.
    /// </summary>
    public interface IProfile : IReadOnlyProfile
    {
        #region Properties
        /// <summary>
        /// Obtiene el nombre por defecto asociado al perfil.
        /// </summary>
        /// <remarks>
        /// Este es usado para el nombre por defecto al perfil y tipicamente esta basedo en el
        /// nombre del ejecutable mas alguna extensión.
        /// </remarks>
        string DefaultName { get; }

        /// <summary>
        /// Obtiene o establece el nombre asociado al perfil.
        /// </summary>
        /// <remarks>
        /// Este podria ser el nombre del archivo donde se almacerán los datos, o algo así.
        /// </remarks>
        new string Name { get; set; }

        /// <summary>
        /// Obtiene o establece si el perfil es sólo de lectura.
        /// </summary>
        bool ReadOnly { get; set; }
        #endregion
        
        #region Methods Implementation
        /// <summary>
        /// Crea una copia de si mismo y la convierte en solo lectura.
        /// </summary>
        /// <returns>La copia del objeto para la implementación de solo lectura.</returns>
        IReadOnlyProfile CloneToReadOnly();

        /// <summary>
        /// Borra una clave dentro de una sección del perfil actual.
        /// </summary>
        /// <param name="section">Sección que contiene la clave.</param>
        /// <param name="entry">Clave a borrarse.</param>
        void RemoveEntry(string section, string entry);

        /// <summary>
        /// Borra una sección del perfil actual.
        /// </summary>
        /// <param name="section">Sección a borrar.</param>
        void RemoveSection(string section);

        /// <summary>
        /// Escribe provenientes de la tablas contenidas de un DataSet a un perfil.
        /// </summary>
        /// <param name="dataSet">Ojbeto DataSet que contiene los datos a ser guardados.</param>
        void WriteDataSet(DataSet dataSet);

        /// <summary>
        /// Escribe el valor de una entrada dentro de una seccion.
        /// </summary>
        /// <param name="section">Seccion que contendrá la entrada.</param>
        /// <param name="entry">Nombre de la entrada.</param>
        /// <param name="value">Valor de la entrada</param>
        void WriteValue(string section, string entry, object value);
        #endregion

        #region Events
        /// <summary>
        /// Evento que se dispara para notificar que se ha producido un cambio en el perfil.
        /// </summary>
        event ProfileChangedHandler Changed;

        /// <summary>
        /// Evento que se dispara para notificar acerca de un cambio en el perfil.
        /// </summary>
        event ProfileChangingHandler Changing;
        #endregion
    }
}