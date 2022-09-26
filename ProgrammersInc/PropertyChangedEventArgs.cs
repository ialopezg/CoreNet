using System;
using System.Collections.Generic;

namespace ProgrammersInc
{
    /// <summary>
    /// Manejador que se implementa para manejar el evento <see cref="Properties.PropertyChanged"/>.
    /// </summary>
    /// <param name="sender">Objeto que solicita el manejador del evento.</param>
    /// <param name="e">Datos del evento.</param>
    public delegate void PropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);

    /// <summary>
    /// Clase que contiene los datos del evento.
    /// </summary>
    public class PropertyChangedEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Crea una nueva instancia de la clase que contiene los datos del evento.
        /// </summary>
        /// <param name="properties">Colección de Propiedades donde se generó el evento.</param>
        /// <param name="key">Clave donde se generó el evento.</param>
        /// <param name="oldValue">Valor anterior de la clave.</param>
        /// <param name="newValue">Nuevo valor para la clave.</param>
        public PropertyChangedEventArgs(Properties properties, string key, object oldValue, object newValue)
        {
            this.properties = properties;
            this.key = key;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
        #endregion

        #region Properties
        Properties properties;
        /// <returns>
        /// Colección de propiedades que generó el cambio.
        /// </returns>
        public Properties Properties
        {
            get { return properties; }
        }

        string key;
        /// <returns>
        /// Clave que genero el cambio.
        /// </returns>
        public string Key
        {
            get { return key; }
        }

        object newValue;
        /// <returns>
        /// El nuevo valor de la propiedad.
        /// </returns>
        public object NewValue
        {
            get { return newValue; }
        }

        object oldValue;
        /// <returns>
        /// El valor anterior de la propiedad.
        /// </returns>
        public object OldValue
        {
            get { return oldValue; }
        }
        #endregion
    }
}