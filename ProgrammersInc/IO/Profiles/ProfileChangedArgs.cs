using System;

namespace ProgrammersInc.IO
{
    /// <summary>
    /// Clase que contiene los datos para el manejador del evento <see cref="Profile.Changed"/>.
    /// </summary>
    /// <remarks>
    /// Esta clase provee toda la información relacionada a un cambio hecho al perfil.
    /// También es usada como clase base para la clase <see cref="ProfileChangingArgs"/>, la cual
    /// es pasada como parámetro del manejador del evento <see cref="Profile.Changing"/>
    /// </remarks>
    public class ProfileChangedArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Crea una nueva instancia de la clase, inicializando todos sus propiedades a los valores dados.
        /// </summary>
        /// <param name="changeType">El nombre del cambio a realizar en el perfil.</param>
        /// <param name="section">El nombre de la seción envuelta en el cambio, o null.</param>
        /// <param name="entry">El nombre de la entrada envuelta en el cambio, o null.</param>
        /// <param name="value">El nuevo valor para la propiedad.</param>
        public ProfileChangedArgs(ProfileChangeType changeType, string section, string entry, object value)
        {
            this.changeType = changeType;
            this.section = section;
            this.entry = entry;
            this.value = value;
        }
        #endregion

        #region Properties
        readonly ProfileChangeType changeType;
        /// <summary>
        /// Obtiene el tipo de cambio que generó el evento.
        /// </summary>
        public ProfileChangeType ChangeType
        {
            get { return changeType; }
        }

        readonly string section;
        /// <summary>
        /// Obtiene la sección que envuelve el cambio, o null si no aplica.
        /// </summary>
        public string Section
        {
            get { return section; }
        }

        readonly string entry;
        /// <summary>
        /// Obtiene el nombre de la entrada envuelta en el cambio, o null si no aplica.
        /// </summary>
        /// <remarks>
        /// Si la propiedad <see cref="ChangeType"/> es establecida a <see cref="ProfileChangeType.Other"/>,
        /// esta propiedad contendrá el nombre de la propiedad o método que cambió.
        /// </remarks>
        public string Entry
        {
            get { return entry; }
        }

        readonly object value;
        /// <summary>
        /// Obtiene el nuevo valor para la entrada, propiedad o método, basado en el valor de la
        /// propiedad <see cref="ChangeType"/>.
        /// </summary>
        public object Value
        {
            get { return value; }
        }
        #endregion
    }
}