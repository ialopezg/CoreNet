using System;

namespace ProgrammersInc.IO
{
    /// <summary>
    /// Clase que contiene los datos del manejador del evento <see cref="Profile.Changing"/>.
    /// </summary>
    public class ProfileChangingArgs : ProfileChangedArgs
    {
        #region Constructors
        /// <summary>
        /// Crea una nueva instancia que contiene los datos del evento.
        /// </summary>
        /// <param name="changeType">El nombre del cambio a realizar en el perfil.</param>
        /// <param name="section">El nombre de la seción envuelta en el cambio, o null.</param>
        /// <param name="entry">El nombre de la entrada envuelta en el cambio, o null.</param>
        /// <param name="value">El nuevo valor para la propiedad.</param>
        public ProfileChangingArgs(ProfileChangeType changeType, string section, string entry, object value)
            : base(changeType, section, entry, value) { }
        #endregion

        #region Properties Implementation
        bool cancel;
        /// <summary>
        /// Obtiene o establece un valor que indicará si el cambió realizado se cancelará o no.
        /// </summary>
        /// <remarks>El valor por defecto es <c>false</c> lo que significa que la peración es permitida.</remarks>
        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }
        #endregion
    }   
}