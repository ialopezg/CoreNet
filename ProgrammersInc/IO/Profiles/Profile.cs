using System;
using System.Collections.Specialized;
using System.Data;

namespace ProgrammersInc.IO
{
    /// <summary>
    /// Clase base para todos los perfiles en este namespace.
    /// </summary>
    /// <remarks>
    /// esta clase contiene métodos y propiedades comunes para todas las clases derivadas de
    /// <see cref="Profile"/>. Esta implementa la mayoria de propiedades y métodos de la interface
    /// base, así que las clase derivadas podrían no tenerlos.
    /// </remarks>
    public abstract class Profile : IProfile
    {
        #region Constructors
        /// <summary>
        /// Inicializa una nueva instancia de la clase estableciendo la propiedad
        /// <see cref="Name"/> a <see cref="DefaultName"/>.
        /// </summary>
        protected Profile()
        {
            name = DefaultName;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase estableciendo la propiedad
        /// <see cref="Name"/> a valor dado.
        /// </summary>
        /// <param name="name">Valor a establecerse en la propiedad <see cref="Name"/>.</param>
        protected Profile(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase en base a otro objeto <see cref="Profile"/>.
        /// </summary>
        /// <param name="profile">Perfil del que se tomaran las propiedades para esta instancia.</param>
        protected Profile(Profile profile)
        {
            this.name = profile.Name;
            this.readOnly = profile.ReadOnly;
            this.Changed = profile.Changed;
            this.Changing = profile.Changing;
        }
        #endregion

        #region Properties
        #region Public Properties
        string name;
        /// <summary>
        /// Obtiene o establece asociado a este perfil.
        /// </summary>
        /// <exception cref="NullReferenceException">El valor a asignar no puede ser nulo.</exception>
        /// <exception cref="InvalidOperationException">La propiedad <see cref="ReadOnly"/>,
        /// está establecida a <c>true</c>.</exception>
        /// <remarks>
        /// Usualmente esta propiedad define el nombre del archivo donde los datos serán almacenados.
        /// </remarks>
        public string Name
        {
            get { return this.name; }
            set 
            {
                VerifyNotReadOnly();
                if (name == value.Trim())
                    return;

                if (!RaiseChangeEvent(true, ProfileChangeType.Name, null, null, value))
                    return;

                name = value.Trim();
                RaiseChangeEvent(false, ProfileChangeType.Name, null, null, value);
            }
        }

        bool readOnly;
        /// <summary>
        /// Obtiene o establece un valor que indica si el perfil es sólo de lectura.
        /// </summary>
        /// <exception cref="InvalidOperationException">El valor de la propiedad ya es <c>true</c>.</exception>
        public bool ReadOnly
        {
            get { return readOnly; }
            set
            {
                this.VerifyNotReadOnly();
                if (readOnly == value)
                    return;

                if (RaiseChangeEvent(true, ProfileChangeType.ReadOnly, null, null, value))
                    return;

                readOnly = value;
                RaiseChangeEvent(false, ProfileChangeType.ReadOnly, null, null, value);
            }
        }
        #endregion

        #region Public Abstract Properties
        /// <summary>
        /// Obtiene el nombre por defecto asociado al perfil.
        /// </summary>
        /// <remarks>
        /// Este es usado para el nombre por defecto al perfil y tipicamente esta basedo en el
        /// nombre del ejecutable mas alguna extensión.
        /// </remarks>
        public abstract string DefaultName { get; }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Obtiene el nombre por defecto del archivo usado por este perfil, sin la extensión específca del
        /// perfil.
        /// </summary>
        /// <remarks>
        /// Esta propiedad es usada para las implementaciones basadas en archivos.
        /// <para>
        /// Para aplicaciones Windows, retornará la ruta completa del archivo sin la extensión.
        /// Para aplicaciones Web, retornará la ruta del archivo Web.Config, sin la extensión
        /// .config.
        /// </para>
        /// </remarks>
        protected string DefaultNameWithoutExtension
        {
            get
            {
                try
                {
                    string file = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
                    return file.Substring(0, file.LastIndexOf('.'));
                }
                catch
                {
                    return "profile"; // Si todo falla.
                }
            }
        }
        #endregion
        #endregion

        #region Methods Implementation
        #region Protected Methods
        /// <summary>
        /// Metodo que dispara los eventos <see cref="Changing"/> y <see cref="Changed"/>.
        /// </summary>
        /// <param name="changing"><c>true</c> si el evento lazado es <see cref="Changing"/>,
        /// y <c>false</c> si el evento es <see cref="Changed"/>.</param>
        /// <param name="changeType">El tipo de cambio a darse.</param>
        /// <param name="section">La sección que envuelve el cambio o null si no aplica.</param>
            /// <param name="entry">La entrada que envuelve el cambio o null si no aplica.</param>
        /// <param name="value">El valor del cambio o null si no aplica.</param>
        /// <returns>El valor devuelto es basado en el tipo de evento generado.</returns>
        protected bool RaiseChangeEvent(bool changing, ProfileChangeType changeType, string section, string entry, object value)
        {
            if (changing)
            {
                if (Changing == null)
                    return true;
                ProfileChangingArgs e = new ProfileChangingArgs(changeType, section, entry, value);
                OnChanging(e);
                return !e.Cancel;
            }

            if (Changed != null)
                OnChanged(new ProfileChangedArgs(changeType, section, entry, value));

            return true;
        }
        #endregion

        #region Public Abstract Methods
        /// <summary>
        /// Crea una copia de si mismo.
        /// </summary>
        /// <returns>Una copia de la instancia actual.</returns>
        public abstract object Clone();

        /// <summary>
        /// Obtiene todas las entradas dentro de una sección dada.
        /// </summary>
        /// <param name="section">El nombre de la sección que contiene las entradas.</param>
        /// <returns>La colección de nombre de entradas encontradas en la sección del archivo dada.</returns>
        public abstract StringCollection GetEntryNames(string section);

        /// <summary>
        /// Obtiene los nombre de todas las secciones del archivo.
        /// </summary>
        /// <returns>La colección de nombre de secciones encontradas en el archivo.</returns>
        public abstract StringCollection GetSectionNames();

        /// <summary>
        /// Obtiene el valor de una entrada dentro de una sección.
        /// </summary>
        /// <param name="section">Sección que contiene la entrada.</param>
        /// <param name="entry">Entrada a evaluar.</param>
        /// <returns>El valor de la entrada, null en caso de que la entrada no exista.</returns>
        public abstract object GetValue(string section, string entry);

        /// <summary>
        /// Borra una clave dentro de una sección del perfil actual.
        /// </summary>
        /// <param name="section">Sección que contiene la clave.</param>
        /// <param name="entry">Clave a borrarse.</param>
        public abstract void RemoveEntry(string section, string entry);

        /// <summary>
        /// Borra una sección del perfil actual.
        /// </summary>
        /// <param name="section">Sección a borrar.</param>
        public abstract void RemoveSection(string section);

        /// <summary>
        /// Escribe el valor de una entrada dentro de una seccion.
        /// </summary>
        /// <param name="section">Seccion que contendrá la entrada.</param>
        /// <param name="entry">Nombre de la entrada.</param>
        /// <param name="value">Valor de la entrada</param>
        public abstract void WriteValue(string section, string entry, object value);
        #endregion

        #region Protected Virtual Methods
        /// <summary>
        /// Crea una copia de si mismo y la convierte en solo lectura.
        /// </summary>
        /// <returns>La copia del objeto para la implementación de solo lectura.</returns>
        public virtual IReadOnlyProfile CloneToReadOnly()
        {
            Profile profile = (Profile)Clone();
            profile.ReadOnly = true;

            return profile;
        }

        /// <summary>
        /// Obtiene objeto DataSet. Convirtiendo cada una de las secciones en una tabla de datos,
        /// y las entradas entradas en las columnas de las tablas y valores las filas de cada una
        /// de las tablas.
        /// </summary>
        /// <returns>Si el archivo existe, el valor retornado será un objeto DataSet representando
        /// el perfil; en otro caso devolvera null. </returns>
        public virtual DataSet GetDataSet()
        {
            try
            {
                VerifyName();

                DataSet dataSet = new DataSet(name);
                foreach (string section in GetSectionNames())
                {
                    DataTable table = dataSet.Tables.Add(section);

                    StringCollection entries = GetEntryNames(section);
                    DataColumn[] columns = new DataColumn[entries.Count];
                    object[] values = new object[entries.Count];

                    int i = 0;
                    foreach (string entry in entries)
                    {
                        object value = GetValue(section, entry);
                        columns[i] = new DataColumn(entry, value.GetType());
                        values[i++] = value;
                    }

                    table.Columns.AddRange(columns);
                    table.Rows.Add(values);
                }

                return dataSet;
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Retorna el valor de una cadena de texto de una entrada que está dentro de una sección.
        /// </summary>
        /// <param name="section">Sección que contiene la entrada.</param>
        /// <param name="entry">Entrada a evaluar.</param>
        /// <param name="defaultValue">Valor por defecto en caso de que la entrada no exista.</param>
        /// <returns>El valor de la entrada convertida en texto, en caso de que la entrada no exista,
        /// retornará el valor por defecto dado.</returns>
        public virtual string GetValue(string section, string entry, string defaultValue)
        {
            object value = GetValue(section, entry);

            return (value == null) ? defaultValue : value.ToString();
        }

        /// <summary>
        /// Retorna el valor entero de una entrada que está dentro de una sección.
        /// </summary>
        /// <param name="section">Sección que contiene la entrada.</param>
        /// <param name="entry">Entrada a evaluar.</param>
        /// <param name="defaultValue">Valor por defecto en caso de que la entrada no exista.</param>
        /// <returns>El valor de la entrada convertida en entero, en caso de que la entrada no exista,
        /// retornará el valor por defecto dado.</returns>
        public virtual int GetValue(string section, string entry, int defaultValue)
        {
            try
            {
                object value = GetValue(section, entry);

                if (value == null)
                    return defaultValue;

                return Convert.ToInt32(value);
            }
            catch (Exception) { return 0; }
        }

        /// <summary>
        /// Retorna el valor double de una entrada que está dentro de una sección.
        /// </summary>
        /// <param name="section">Sección que contiene la entrada.</param>
        /// <param name="entry">Entrada a evaluar.</param>
        /// <param name="defaultValue">Valor por defecto en caso de que la entrada no exista.</param>
        /// <returns>El valor de la entrada convertida en double, en caso de que la entrada no exista,
        /// retornará el valor por defecto dado.</returns>
        public virtual double GetValue(string section, string entry, double defaultValue)
        {
            try
            {
                object value = GetValue(section, entry);

                if (value == null)
                    return defaultValue;

                return Convert.ToDouble(value);
            }
            catch (Exception) { return 0; }
        }

        /// <summary>
        /// Retorna el valor boolean de una entrada que está dentro de una sección.
        /// </summary>
        /// <param name="section">Sección que contiene la entrada.</param>
        /// <param name="entry">Entrada a evaluar.</param>
        /// <param name="defaultValue">Valor por defecto en caso de que la entrada no exista.</param>
        /// <returns>El valor de la entrada convertida en boolean, en caso de que la entrada no exista,
        /// retornará el valor por defecto dado.</returns>
        public virtual bool GetValue(string section, string entry, bool defaultValue)
        {
            try
            {
                object value = GetValue(section, entry);
                if (value == null)
                    return defaultValue;

                return Convert.ToBoolean(value);
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Verifica si una entrada existe.
        /// </summary>
        /// <param name="section">Seción que contiene la entrada.</param>
        /// <param name="entry">Entrada a evaluar.</param>
        /// <returns><c>true</c> si la entrada existe, en otro caso retornará <c>false</c>.</returns>
        public virtual bool HasEntry(string section, string entry)
        {
            StringCollection entries = GetEntryNames(section);

            if (entries == null)
                return false;

            VerifyAndAdjustEntry(ref entry);
            return entries.IndexOf(entry) >= 0;
        }

        /// <summary>
        /// Determina si una sección existe.
        /// </summary>
        /// <param name="section">Sección a evaluar.</param>
        /// <returns><c>true</c> si la sección existe, en otro caso retornará <c>false</c>.</returns>
        public virtual bool HasSection(string section)
        {
            StringCollection sections = this.GetSectionNames();

            if (sections == null)
                return false;

            VerifyAndAjustSection(ref section);
            return sections.IndexOf(section) >= 0;
        }

        /// <summary>
        /// Dispara el evento <see cref="Changed"/>.
        /// </summary>
        /// <param name="e">Los datos del evento.</param>
        protected virtual void OnChanged(ProfileChangedArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        /// <summary>
        /// Dispara el evento <see cref="Changing"/>.
        /// </summary>
        /// <param name="e">Los datos del evento.</param>
        protected virtual void OnChanging(ProfileChangingArgs e)
        {
            if (Changing == null)
                return;

            foreach (ProfileChangingHandler handler in Changing.GetInvocationList())
            {
                handler(this, e);

                if (e.Cancel)
                    break;
            }
        }

        /// <summary>
        /// Verifica si la entrada dada no está vacía o nula y quita todos los
        /// carácteres no imprimibles de está.
        /// </summary>
        /// <param name="entry">Nombre de la entrada a evaluar.</param>
        /// <exception cref="ArgumentNullException">La sección es nula o vacía.</exception>
        protected virtual void VerifyAndAdjustEntry(ref string entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");

            entry = entry.Trim();
        }

        /// <summary>
        /// Verifica si la seccion dada no está vacía o nula y quita todos los
        /// carácteres no imprimibles de está.
        /// </summary>
        /// <param name="section">Nombre de la sección a evaluar.</param>
        /// <exception cref="ArgumentNullException">La sección es nula o vacía.</exception>
        protected virtual void VerifyAndAjustSection(ref string section)
        {
            try
            {
                if (string.IsNullOrEmpty(section))
                    throw new ArgumentNullException("section");

                section = section.Trim();
            }
            catch (Exception) {  }
        }

        /// <summary>
        /// Verifica que el nombre de la propiedad <see cref="Profile.Name"/> sea diferente de null o vacía.
        /// </summary>
        /// <exception cref="InvalidOperationException">La propiedad es null o vacía.</exception>
        protected internal virtual void VerifyName()
        {
            if (string.IsNullOrEmpty(name))
                throw new InvalidOperationException("Peración no permitida porque la propiedad Name es null o vacía.");
        }

        /// <summary>
        /// Verifica si el valor de la propiedad <see cref="Profile.ReadOnly"/> es <c>false</c>.
        /// </summary>
        /// <exception cref="InvalidOperationException">El valor de la propiedad es <c>true</c>.</exception>
        /// <remarks>
        /// Este método es usado por las clases derivadas como la forma conveniente para validar las
        /// modificaciones que se le hagan al perfil.
        /// </remarks>
        protected internal virtual void VerifyNotReadOnly()
        {
            if (readOnly)
                throw new InvalidOperationException("La operación no está permitida porque la Propiedad ReadOnly se ha establecido a true.");
        }

        /// <summary>
        /// Escribe provenientes de la tablas contenidas de un DataSet a un perfil.
        /// </summary>
        /// <param name="dataSet">Ojbeto DataSet que contiene los datos a ser guardados.</param>
        public virtual void WriteDataSet(DataSet dataSet)
        {
            try
            {
                if (dataSet == null)
                    throw new ArgumentNullException("dataSet");

                foreach (DataTable table in dataSet.Tables)
                {
                    string section = table.TableName;
                    DataRowCollection rows = table.Rows;
                    if (rows.Count == 0)
                        continue;

                    foreach (DataColumn column in table.Columns)
                    {
                        string entry = column.ColumnName;
                        object value = rows[0][column];

                        WriteValue(section, entry, value);
                    }
                }
            }
            catch (Exception) { }
        }
        #endregion
        #endregion

        #region Events
        /// <summary>
        /// Evento que se dispara para notificar que se ha producido un cambio en el perfil.
        /// </summary>
        public event ProfileChangedHandler Changed;

        /// <summary>
        /// Evento que se dispara para notificar acerca de un cambio en el perfil.
        /// </summary>
        public event ProfileChangingHandler Changing;
        #endregion
    }

    /// <summary>
    /// Delegado para manejar el evento <see cref="Profile.Changing"/>.
    /// </summary>
    /// <param name="sender">Objeto que disparó el evento.</param>
    /// <param name="e">Datos del evento.</param>
    public delegate void ProfileChangingHandler(object sender, ProfileChangingArgs e);

    /// <summary>
    /// Delegado para manejar el evento <see cref="Profile.Changing"/>.
    /// </summary>
    /// <param name="sender">Objeto que disparó el evento.</param>
    /// <param name="e">Datos del evento.</param>
    public delegate void ProfileChangedHandler(object sender, ProfileChangedArgs e);
}