using System;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.ComponentModel;

namespace ProgrammersInc.IO
{
    /// <summary>
    /// Clase que representa un archivo Ini.
    /// </summary>
    public class Ini : Profile
    {
        #region Constructors
        /// <summary>
        /// Crea una nueva instancia de la clase, estableciendo la propiedad <see cref="Profile.Name"/>
        /// a <see cref="Profile.DefaultName"/>.
        /// </summary>
        public Ini() { }

        /// <summary>
        /// Crea una nueva instancia de la clase, estableciendo la propiedad <see cref="Profile.Name"/>
        /// al nombre de archivo dado.
        /// </summary>
        /// <param name="fileName">El nombre del archivo INI, con el que se inicializará la propiedad
        /// <see cref="Profile.Name"/>.</param>
        public Ini(string fileName)
            : base(fileName) { }

        /// <summary>
        /// Crea una nueva instancia de la clase, en base a otro objeto <see cref="Ini"/>.
        /// </summary>
        /// <param name="ini">Objeto Ini, del que se tomarán los valores para todas las propiedades
        /// de la clase.</param>
        public Ini(Ini ini)
            : base(ini) { }
        #endregion

        #region Properties Implementation
        #region Public Override Properties
        /// <summary>
        /// Obtiene el nombre por defecto del archivo INI.
        /// </summary>
        public override string DefaultName
        {
            get { return DefaultNameWithoutExtension + ".ini"; }
        }
        #endregion
        #endregion

        #region Methods Implementation
        #region Public Override Methods
        /// <summary>
        /// Crea una copia de si mismo.
        /// </summary>
        /// <returns>Una copia de la instancia actual.</returns>
        public override object Clone()
        {
            return new Ini(this);
        }

        /// <summary>
        /// Obtiene todas las entradas dentro de una sección dada.
        /// </summary>
        /// <param name="section">El nombre de la sección que contiene las entradas.</param>
        /// <returns>La colección de nombre de entradas encontradas en la sección del archivo dada.</returns>
        /// <exception cref="InvalidOperationException">La sección no existe en el archivo INI.</exception>
        public override StringCollection GetEntryNames(string section)
        {
            try
            {
                if (!HasSection(section))
                    throw new InvalidOperationException();

                VerifyAndAjustSection(ref section);

                StringCollection entries = new StringCollection();
                for (int maxSize = 500; true; maxSize *= 2)
                {
                    byte[] bytes = new byte[maxSize];
                    int size = GetPrivateProfileString(section, 0, "", bytes, maxSize, this.Name);

                    if (size < maxSize - 2)
                    {
                        string result = Encoding.ASCII.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                        if (string.IsNullOrEmpty(result))
                            return entries;

                        entries.AddRange(result.Split(new char[] { '\0' }));
                        return entries;
                    }
                }
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Obtiene los nombre de todas las secciones del archivo.
        /// </summary>
        /// <returns>La colección de nombre de secciones encontradas en el archivo.</returns>
        /// <exception cref="FileNotFoundException">El archivo asociado a la instancia no existe.</exception>
        public override StringCollection GetSectionNames()
        {
            try
            {
                if (!File.Exists(this.Name))
                    throw new FileNotFoundException("El archivo especificado no existe.", this.Name);

                StringCollection sections = new StringCollection();
                for (int maxSize = 500; true; maxSize *= 2)
                {
                    byte[] bytes = new byte[maxSize];
                    int size = GetPrivateProfileString(0, "", "", bytes, maxSize, this.Name);

                    if (size < maxSize - 2)
                    {
                        string result = Encoding.ASCII.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                        if (string.IsNullOrEmpty(result))
                           return sections;

                       sections.AddRange(result.Split(new char[] { '\0' }));
                       return sections;
                    }
                }
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Obtiene el valor de una entrada dentro de una sección.
        /// </summary>
        /// <param name="section">Sección que contiene la entrada.</param>
        /// <param name="entry">Entrada a evaluar.</param>
        /// <returns>El valor de la entrada, null en caso de que la entrada no exista.</returns>
        public override object GetValue(string section, string entry)
        {
            try
            {
                VerifyName();
                VerifyAndAjustSection(ref section);
                VerifyAndAdjustEntry(ref entry);

                for (int maxSize = 250; true; maxSize *= 2)
                {
                    StringBuilder result = new StringBuilder(maxSize);
                    int size = GetPrivateProfileString(section, entry, "", result, maxSize, this.Name);

                    if (size < maxSize - 1)
                    {
                        if (size == 0 && !HasEntry(section, entry))
                            return null;
                        return result.ToString();
                    }
                }
            }
            catch (Exception) { return null; }
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
                if (!HasEntry(section, entry))
                    return;
                VerifyNotReadOnly();
                VerifyName();
                VerifyAndAjustSection(ref section);
                VerifyAndAdjustEntry(ref entry);

                if (!RaiseChangeEvent(true, ProfileChangeType.RemoveEntry, section, entry, null))
                    return;

                if (WritePrivateProfileString(section, entry, 0, this.Name) == 0)
                    throw new Win32Exception();

                RaiseChangeEvent(false, ProfileChangeType.RemoveEntry, section, entry, null);
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
                if (!HasSection(section))
                    return;

                VerifyNotReadOnly();
                VerifyName();
                VerifyAndAjustSection(ref section);

                if (!RaiseChangeEvent(true, ProfileChangeType.RemoveSection, section, null, null))
                    return;

                if (WritePrivateProfileString(section, 0, string.Empty, this.Name) == 0)
                    throw new Win32Exception();

                RaiseChangeEvent(false, ProfileChangeType.RemoveSection, section, null, null);
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
                VerifyName();
                VerifyAndAjustSection(ref section);
                VerifyAndAdjustEntry(ref entry);

                if (!RaiseChangeEvent(true, ProfileChangeType.WriteValue, section, entry, value))
                    return;

                if (WritePrivateProfileString(section, entry, value.ToString(), this.Name) == 0)
                    throw new Win32Exception();

                RaiseChangeEvent(false, ProfileChangeType.WriteValue, section, entry, value);
            }
            catch (Exception) { }
        }
        #endregion

        #region Windows API Methods
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, int key, string defaultValue, [MarshalAs(UnmanagedType.LPArray)] byte[] result, int size, string fileName);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(int section, string key, string defaultValue, [MarshalAs(UnmanagedType.LPArray)] byte[] result, int size, string fileName);

        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder result, int size, string fileName);

        [DllImport("kernel32", SetLastError = true)]
        static extern int WritePrivateProfileString(string section, string key, string value, string fileName);

        [DllImport("kernel32", SetLastError = true)]
        static extern int WritePrivateProfileString(string section, string key, int value, string fileName);

        [DllImport("kernel32", SetLastError = true)]
        static extern int WritePrivateProfileString(string section, int key, string value, string fileName);
        #endregion
        #endregion
    }
}