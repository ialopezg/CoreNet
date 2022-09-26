using System;
using System.Collections.Specialized;
using System.Windows.Forms;

using Microsoft.Win32;

namespace ProgrammersInc.IO
{
    /// <summary>
    /// Clase de perfil que utiliza el registro de Microsoft Windows para recuperar y guardar
    /// información.
    /// </summary>
    public class Registry : Profile
    {
        #region Constructor
        /// <summary>
        /// Crea una nueva instancia de la clase.
        /// </summary>
        public Registry() { }

        /// <summary>
        /// Crea una nueva instancia de la clase, estableciendo la clave raíz a
        /// <paramref name="rootKey"/> y la propiedad <see cref="Profile.Name"/> a
        /// <paramref name="subKeyName"/>.
        /// </summary>
        /// <param name="rootKey">Clave donde se realizarán las operaciones del
        /// perfil actual.</param>
        /// <param name="subKeyName">Nombre que se asociará al perfil.</param>
        public Registry(RegistryKey rootKey, string subKeyName)
            : base("")
        {
            if (rootKey == null)
                throw new ArgumentNullException("rootKey");
            if(string.IsNullOrEmpty(subKeyName))
                throw new ArgumentNullException("subKeyName");

            this.rootKey = rootKey;
            this.Name = subKeyName;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase en base a otro objeto <see cref="Registry"/>.
        /// </summary>
        /// <param name="reg">Perfil del que se tomaran las propiedades para esta instancia.</param>
        public Registry(Registry reg)
            : base(reg)
        {
            this.rootKey = reg.RootKey;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Obtiene el nombre por defecto asociado al perfil.
        /// </summary>
        /// <remarks>
        /// Este es usado para el nombre por defecto al perfil y tipicamente esta basedo en el
        /// nombre del ejecutable mas alguna extensión.
        /// </remarks>
        public override string DefaultName
        {
            get
            {
                if (Application.CompanyName == "" || Application.ProductName == "")
                    throw new InvalidOperationException("Application.CompanyName and/or Application.ProductName are empty and they're needed for the DefaultName.");

                return "Software\\" + Application.CompanyName + "\\" + Application.ProductName;
            }
        }

        RegistryKey rootKey = Microsoft.Win32.Registry.CurrentUser;
        /// <summary>
        /// Obbtiene o establece la entrada clave raíz que usará el perfil para las operaciones
        /// de guardado, eliminación y modificación.
        /// </summary>
        public RegistryKey RootKey
        {
            get { return this.rootKey; }
            set
            {
                VerifyNotReadOnly();
                
                if (rootKey == value)
                    return;

                if (!RaiseChangeEvent(true, ProfileChangeType.Other, null, "RootKey", value))
                    return;

                this.rootKey = value;

                RaiseChangeEvent(false, ProfileChangeType.Other, null, "RootKey", value);
            }
        }
        #endregion

        #region Methods Implementation
        #region Public Override Methods
        /// <summary>
        /// Crea una copia de si mismo.
        /// </summary>
        /// <returns>Una copia de la instancia actual.</returns>
        public override object Clone()
        {
            return new Registry(this);
        }

        /// <summary>
        /// Obtiene todas las entradas dentro de una sección dada.
        /// </summary>
        /// <param name="section">El nombre de la sección que contiene las entradas.</param>
        /// <returns>La colección de nombre de entradas encontradas en la sección del archivo dada.</returns>
        public override StringCollection GetEntryNames(string section)
        {
            try
            {
                VerifyAndAjustSection(ref section);

                using (RegistryKey subKey = GetSubKey(section, false, false))
                {
                    if (subKey == null)
                        return null;

                    StringCollection entries = new StringCollection();
                    entries.AddRange(subKey.GetSubKeyNames());

                    return entries;
                }
            }
            catch (Exception) { return new StringCollection(); }
        }

        /// <summary>
        /// Obtiene los nombre de todas las secciones del archivo.
        /// </summary>
        /// <returns>La colección de nombre de secciones encontradas en el archivo.</returns>
        public override StringCollection GetSectionNames()
        {
            try
            {
                VerifyName();

                using (RegistryKey key = rootKey.OpenSubKey(Name))
                {
                    if (key == null)
                        return null;

                    StringCollection sections = new StringCollection();
                    sections.AddRange(key.GetSubKeyNames());

                    return sections;
                }
            }
            catch (Exception) { return new StringCollection(); }
        }

        /// <summary>
        /// Obtiene el valor de una entrada dentro de una sección.
        /// </summary>
        /// <param name="section">Sección que contiene la entrada.</param>
        /// <param name="entry">Entrada a evaluar.</param>
        /// <returns>El valor de la entrada, null en caso de que la entrada no exista.</returns>
        public override object GetValue(string section, string entry)
        {
            VerifyAndAjustSection(ref section);
            VerifyAndAdjustEntry(ref entry);

            using (RegistryKey subKey = GetSubKey(section, false, false))
                return (subKey == null ? null : subKey.GetValue(entry));
        }

        /// <summary>
        /// Borra una clave dentro de una sección del perfil actual.
        /// </summary>
        /// <param name="section">Sección que contiene la clave.</param>
        /// <param name="entry">Clave a borrarse.</param>
        public override void RemoveEntry(string section, string entry)
        {
            try
            {
                VerifyNotReadOnly();
                VerifyAndAjustSection(ref section);
                VerifyAndAdjustEntry(ref entry);

                using (RegistryKey subKey = GetSubKey(section, false, true))
                {
                    if (subKey != null && subKey.GetValue(entry) != null)
                    {
                        if (!RaiseChangeEvent(true, ProfileChangeType.RemoveEntry, section, entry, null))
                            return;

                        subKey.DeleteValue(entry, false);

                        RaiseChangeEvent(false, ProfileChangeType.RemoveEntry, section, entry, null);
                    }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Borra una sección del perfil actual.
        /// </summary>
        /// <param name="section">Sección a borrar.</param>
        public override void RemoveSection(string section)
        {
            try
            {
                VerifyNotReadOnly();
                VerifyName();
                VerifyAndAjustSection(ref section);

                using (RegistryKey key = rootKey.OpenSubKey(Name, true))
                {
                    if (key != null && HasSection(section))
                    {
                        if (!RaiseChangeEvent(true, ProfileChangeType.RemoveSection, section, null, null))
                            return;

                        key.DeleteSubKeyTree(section);
                        RaiseChangeEvent(false, ProfileChangeType.RemoveSection, section, null, null);
                    }
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Escribe el valor de una entrada dentro de una seccion.
        /// </summary>
        /// <param name="section">Seccion que contendrá la entrada.</param>
        /// <param name="entry">Nombre de la entrada.</param>
        /// <param name="value">Valor de la entrada</param>
        public override void WriteValue(string section, string entry, object value)
        {
            try
            {
                if (value == null)
                {
                    RemoveEntry(section, entry);
                    return;
                }

                VerifyNotReadOnly();
                VerifyAndAjustSection(ref section);
                VerifyAndAdjustEntry(ref entry);

                if (!RaiseChangeEvent(true, ProfileChangeType.WriteValue, section, entry, value))
                    return;

                using (RegistryKey subKey = GetSubKey(section, true, true))
                    subKey.SetValue(entry, value);

                RaiseChangeEvent(false, ProfileChangeType.WriteValue, section, entry, value);
            }
            catch (Exception) { }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Obtiene una clave del Registro para la clave dada.
        /// </summary>
        /// <param name="section">Sección a evaluar.</param>
        /// <param name="create">Específica un valor que indica si la clave será creada.</param>
        /// <param name="writable">Indica un valor que específica si la clave podrá ser modificada.</param>
        /// <returns>Una clave del Registro para la clave dada.</returns>
        protected RegistryKey GetSubKey(string section, bool create, bool writable)
        {
            VerifyName();

            string keyName = Name + "\\" + section;

            if (create)
                return rootKey.CreateSubKey(keyName);

            return rootKey.OpenSubKey(keyName, writable);
        }
        #endregion
        #endregion
    }
}
