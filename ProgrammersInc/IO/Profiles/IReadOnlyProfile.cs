using System;
using System.Collections.Specialized;
using System.Data;

namespace ProgrammersInc.IO
{
    /// <summary>
    /// Interfaz base para todos los perfiles en este namespace.
    /// Representa un perfil de solo lectura.
    /// </summary>
    public interface IReadOnlyProfile : ICloneable
    {
        #region Properties
        /// <summary>
        /// Obtiene el nombre asociado al perfil.
        /// </summary>
        /// <remarks>
        /// Este podria ser el nombre del archivo donde se almacer�n los datos, o algo as�.
        /// </remarks>
        string Name { get; }
        #endregion

        #region Methods Implementation
        /// <summary>
        /// Obtiene todas las entradas dentro de una secci�n dada.
        /// </summary>
        /// <param name="section">El nombre de la secci�n que contiene las entradas.</param>
        /// <returns>La colecci�n de nombre de entradas encontradas en la secci�n del archivo dada.</returns>
        StringCollection GetEntryNames(string section);

        /// <summary>
        /// Obtiene objeto DataSet. Convirtiendo cada una de las secciones en una tabla de datos,
        /// y las entradas entradas en las columnas de las tablas y valores las filas de cada una
        /// de las tablas.
        /// </summary>
        /// <returns>Si el archivo existe, el valor retornado ser� un objeto DataSet representando
        /// el perfil; en otro caso devolvera null. </returns>
        DataSet GetDataSet();

        /// <summary>
        /// Obtiene los nombre de todas las secciones del archivo.
        /// </summary>
        /// <returns>La colecci�n de nombre de secciones encontradas en el archivo.</returns>
        StringCollection GetSectionNames();

        /// <summary>
        /// Obtiene el valor de una entrada dentro de una secci�n.
        /// </summary>
        /// <param name="section">Secci�n que contiene la entrada.</param>
        /// <param name="entry">Entrada a evaluar.</param>
        /// <returns>El valor de la entrada, null en caso de que la entrada no exista.</returns>
        object GetValue(string section, string entry);

        /// <summary>
        /// Retorna el valor de una cadena de texto de una entrada que est� dentro de una secci�n.
        /// </summary>
        /// <param name="section">Secci�n que contiene la entrada.</param>
        /// <param name="entry">Entrada a evaluar.</param>
        /// <param name="defaultValue">Valor por defecto en caso de que la entrada no exista.</param>
        /// <returns>El valor de la entrada convertida en texto, en caso de que la entrada no exista,
        /// retornar� el valor por defecto dado.</returns>
        string GetValue(string section, string entry, string defaultValue);

        /// <summary>
        /// Retorna el valor entero de una entrada que est� dentro de una secci�n.
        /// </summary>
        /// <param name="section">Secci�n que contiene la entrada.</param>
        /// <param name="entry">Entrada a evaluar.</param>
        /// <param name="defaultValue">Valor por defecto en caso de que la entrada no exista.</param>
        /// <returns>El valor de la entrada convertida en entero, en caso de que la entrada no exista,
        /// retornar� el valor por defecto dado.</returns>
        int GetValue(string section, string entry, int defaultValue);

        /// <summary>
        /// Retorna el valor double de una entrada que est� dentro de una secci�n.
        /// </summary>
        /// <param name="section">Secci�n que contiene la entrada.</param>
        /// <param name="entry">Entrada a evaluar.</param>
        /// <param name="defaultValue">Valor por defecto en caso de que la entrada no exista.</param>
        /// <returns>El valor de la entrada convertida en double, en caso de que la entrada no exista,
        /// retornar� el valor por defecto dado.</returns>
        double GetValue(string section, string entry, double defaultValue);

        /// <summary>
        /// Retorna el valor boolean de una entrada que est� dentro de una secci�n.
        /// </summary>
        /// <param name="section">Secci�n que contiene la entrada.</param>
        /// <param name="entry">Entrada a evaluar.</param>
        /// <param name="defaultValue">Valor por defecto en caso de que la entrada no exista.</param>
        /// <returns>El valor de la entrada convertida en boolean, en caso de que la entrada no exista,
        /// retornar� el valor por defecto dado.</returns>
        bool GetValue(string section, string entry, bool defaultValue);

        /// <summary>
        /// Verifica si una entrada existe.
        /// </summary>
        /// <param name="section">Seci�n que contiene la entrada.</param>
        /// <param name="entry">Entrada a evaluar.</param>
        /// <returns><c>true</c> si la entrada existe, en otro caso retornar� <c>false</c>.</returns>
        bool HasEntry(string section, string entry);

        /// <summary>
        /// Determina si una secci�n existe.
        /// </summary>
        /// <param name="section">Secci�n a evaluar.</param>
        /// <returns><c>true</c> si la secci�n existe, en otro caso retornar� <c>false</c>.</returns>
        bool HasSection(string section);
        #endregion
    }
}