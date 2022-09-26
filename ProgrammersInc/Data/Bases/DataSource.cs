using System;
using System.Data;
using System.Data.Common;
using System.ComponentModel;

namespace ProgrammersInc.Data
{
    /// <summary>
    /// Clase base que implementan todas las clases de origen de datos en este namespace.
    /// </summary>
    public abstract class DataSource : IDataSource
    {
        #region Variables Implementation
        bool disposed = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Data.DataSource"/>.
        /// </summary>
        public DataSource() { }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Data.DataSource"/>.
        /// </summary>
        /// <param name="provider">Objeto que se usará para crear un proveedor de las
        /// clases e origen de datos.</param>
        /// <param name="connectionString">La cadena que se utilizará para abrir la conexión.</param>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> es null.</exception>
        /// /// <exception cref="ArgumentNullException"><paramref name="connectionString"/> es null.</exception>
        public DataSource(DbProviderFactory provider, string connectionString)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            this.factory = provider;
            this.connection = provider.CreateConnection();
            this.connection.ConnectionString = connectionString;
            this.SetCommand();
        }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Data.DataSource"/>.
        /// </summary>
        /// <param name="providerName">Nombre invariable de un proveedor.</param>
        /// <param name="connectionString">La cadena que se utilizará para abrir la conexión.</param>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> es null.</exception>
        public DataSource(string providerName, string connectionString)
            : this(DbProviderFactories.GetFactory(providerName), connectionString) { }
        #endregion

        #region Destructors
        /// <summary>
        /// Destruye todos los recursos que se esten usando y libera la memoria implementada.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destruye la instancia acual.
        /// </summary>
        ~DataSource()
        {
            Dispose(false);
        }
        #endregion

        #region Properties
        DbCommand command = null;
        /// <summary>
        /// Obtiene el objeto que representará una instrucción SQL o un
        /// procedimiento almacenado que se va a ejecutar en un origen de datos.
        /// </summary>
        protected DbCommand Command
        {
            get { return command; }
        }

        DbConnection connection = null;
        /// <summary>
        /// Obtiene o establece el objeto que representará la conexión a una base de datos.
        /// </summary>
        protected DbConnection Connection
        {
            get { return this.connection; }
            set { this.connection = value; }
        }

        string connectionString;
        /// <summary>
        /// Obtiene o establece la cadena de conexión que se utilizará para
        /// realizar la conexión a la base de datos.
        /// </summary>
        protected string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }
	

        DbProviderFactory factory;
        /// <summary>
        /// Obtiene o establece el objeto que se usará para crear un proveedor de las
        /// clases e origen de datos.
        /// </summary>
        protected DbProviderFactory Factory
        {
            get { return factory; }
            set { this.factory = value; }
        }

        DbTransaction transaction = null;
        /// <summary>
        /// Obtiene o establece el objeto que realizará las transacciones en el
        /// origen de datos actual.
        /// </summary>
        protected DbTransaction Transaction
        {
            get { return transaction; }
            set { transaction = value; }
        }

        bool transactionIsSet = false;
        /// <summary>
        /// Obtiene o establece si se ha establecido una transacción a esta instancia.
        /// </summary>
        public bool TransactionIsSet
        {
            get { return this.transactionIsSet; }
            set { this.transactionIsSet = value; }
        }
        #endregion

        #region Methods Implementation
        #region Protected Methods
        /// <summary>
        /// Crea el comando que se asociará a la conexión actual.
        /// </summary>
        protected void SetCommand()
        {
            this.command = this.connection.CreateCommand();
            this.command.Connection = this.connection;
            if (ServerType == ServerType.MICROSOFT_SQL_SERVER)
                this.command.CommandType = CommandType.StoredProcedure;
            else
                this.command.CommandType = CommandType.Text;
        }

        /// <summary>
        /// Obtiene el tipo de servidor como actua esta instancia.
        /// </summary>
        public abstract ServerType ServerType { get; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Agrega un parámetro del tipo <see cref="System.Boolean"/>.
        /// </summary>
        /// <param name="name">Nombre del parámetro.</param>
        /// <param name="value">valor del parámetro.</param>
        public void AddParameter(string name, bool value)
        {
            if (this.command.Parameters.IndexOf(name) == -1)
            {
                DbParameter param = this.command.CreateParameter();
                param.ParameterName = name;
                param.DbType = DbType.Boolean;
                param.Direction = ParameterDirection.Input;
                param.Value = value;
                this.command.Parameters.Add(param);
            }
            else
                this.command.Parameters[name].Value = value;
        }

        /// <summary>
        /// Agrega un parámetro del tipo <see cref="System.Byte"/>.
        /// </summary>
        /// <param name="name">Nombre del parámetro.</param>
        /// <param name="value">valor del parámetro.</param>
        public void AddParameter(string name, byte value)
        {
            if (this.command.Parameters.IndexOf(name) == -1)
            {
                DbParameter param = this.command.CreateParameter();
                param.ParameterName = name;
                param.DbType = DbType.Byte;
                param.Direction = ParameterDirection.Input;
                param.Value = value;
                this.command.Parameters.Add(param);
            }
            else
                this.command.Parameters[name].Value = value;
        }

        /// <summary>
        /// Agrega un parámetro del tipo array de <see cref="System.Byte"/>.
        /// </summary>
        /// <param name="name">Nombre del parámetro.</param>
        /// <param name="value">valor del parámetro.</param>
        public void AddParameter(string name, byte[] value)
        {
            if (this.command.Parameters.IndexOf(name) == -1)
            {
                DbParameter param = this.command.CreateParameter();
                param.ParameterName = name;
                param.DbType = DbType.Binary;
                param.Direction = ParameterDirection.Input;
                param.Value = value;
                this.command.Parameters.Add(param);
            }
            else
                this.command.Parameters[name].Value = value;
        }

        /// <summary>
        /// Agrega un parámetro del tipo <see cref="System.DateTime"/>.
        /// </summary>
        /// <param name="name">Nombre del parámetro.</param>
        /// <param name="value">valor del parámetro.</param>
        public void AddParameter(string name, DateTime value)
        {
            if (this.command.Parameters.IndexOf(name) == -1)
            {
                DbParameter param = this.command.CreateParameter();
                param.ParameterName = name;
                param.DbType = DbType.DateTime;
                param.Direction = ParameterDirection.Input;
                param.Value = value;
                this.command.Parameters.Add(param);
            }
            else
                this.command.Parameters[name].Value = value;
        }

        /// <summary>
        /// Agrega un parámetro del tipo <see cref="System.Decimal"/>.
        /// </summary>
        /// <param name="name">Nombre del parámetro.</param>
        /// <param name="value">valor del parámetro.</param>
        public void AddParameter(string name, decimal value)
        {
            if (this.command.Parameters.IndexOf(name) == -1)
            {
                DbParameter param = this.command.CreateParameter();
                param.ParameterName = name;
                param.DbType = DbType.Currency;
                param.Direction = ParameterDirection.Input;
                param.Value = value;
                this.command.Parameters.Add(param);
            }
            else
                this.command.Parameters[name].Value = value;
        }

        /// <summary>
        /// Agrega un parámetro del tipo <see cref="System.Double"/>.
        /// </summary>
        /// <param name="name">Nombre del parámetro.</param>
        /// <param name="value">valor del parámetro.</param>
        public void AddParameter(string name, double value)
        {
            if (this.command.Parameters.IndexOf(name) == -1)
            {
                DbParameter param = this.command.CreateParameter();
                param.ParameterName = name;
                param.DbType = DbType.Double;
                param.Direction = ParameterDirection.Input;
                param.Value = value;
                this.command.Parameters.Add(param);
            }
            else
                this.command.Parameters[name].Value = value;
        }

        /// <summary>
        /// Agrega un parámetro del tipo <see cref="System.Single"/>.
        /// </summary>
        /// <param name="name">Nombre del parámetro.</param>
        /// <param name="value">valor del parámetro.</param>
        public void AddParameter(string name, float value)
        {
            if (this.command.Parameters.IndexOf(name) == -1)
            {
                DbParameter param = this.command.CreateParameter();
                param.ParameterName = name;
                param.DbType = DbType.Single;
                param.Direction = ParameterDirection.Input;
                param.Value = value;
                this.command.Parameters.Add(param);
            }
            else
                this.command.Parameters[name].Value = value;
        }

        /// <summary>
        /// Agrega un parámetro del tipo <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="name">Nombre del parámetro.</param>
        /// <param name="value">valor del parámetro.</param>
        public void AddParameter(string name, int value)
        {
            if (this.command.Parameters.IndexOf(name) == -1)
            {
                DbParameter param = this.command.CreateParameter();
                param.ParameterName = name;
                param.DbType = DbType.Int32;
                param.Direction = ParameterDirection.Input;
                param.Value = value;
                this.command.Parameters.Add(param);
            }
            else
                this.command.Parameters[name].Value = value;
        }

        /// <summary>
        /// Agrega un parámetro del tipo <see cref="System.Int64"/>.
        /// </summary>
        /// <param name="name">Nombre del parámetro.</param>
        /// <param name="value">valor del parámetro.</param>
        public void AddParameter(string name, long value)
        {
            if (this.command.Parameters.IndexOf(name) == -1)
            {
                DbParameter param = this.command.CreateParameter();
                param.ParameterName = name;
                param.DbType = DbType.Int64;
                param.Direction = ParameterDirection.Input;
                param.Value = value;
                this.command.Parameters.Add(param);
            }
            else
                this.command.Parameters[name].Value = value;
        }

        /// <summary>
        /// Agrega un parámetro del tipo <see cref="System.Object"/>.
        /// </summary>
        /// <param name="name">Nombre del parámetro.</param>
        /// <param name="value">valor del parámetro.</param>
        public void AddParameter(string name, object value)
        {
            if (this.command.Parameters.IndexOf(name) == -1)
            {
                DbParameter param = this.command.CreateParameter();
                param.ParameterName = name;
                param.DbType = DbType.Object;
                param.Direction = ParameterDirection.Input;
                if (value != null)
                    param.Value = value;
                else
                    param.Value = System.DBNull.Value;
                this.command.Parameters.Add(param);
            }
            else
                if (value != null)
                    this.command.Parameters[name].Value = value;
                else
                    this.command.Parameters[name].Value = System.DBNull.Value;
        }

        /// <summary>
        /// Agrega un parámetro del tipo <see cref="System.Int16"/>.
        /// </summary>
        /// <param name="name">Nombre del parámetro.</param>
        /// <param name="value">valor del parámetro.</param>
        public void AddParameter(string name, short value)
        {
            if (this.command.Parameters.IndexOf(name) == -1)
            {
                DbParameter param = this.command.CreateParameter();
                param.ParameterName = name;
                param.DbType = DbType.Int16;
                param.Direction = ParameterDirection.Input;
                param.Value = value;
                this.command.Parameters.Add(param);
            }
            else
                this.command.Parameters[name].Value = value;
        }

        /// <summary>
        /// Agrega un parámetro del tipo <see cref="System.String"/>.
        /// </summary>
        /// <param name="name">Nombre del parámetro.</param>
        /// <param name="value">valor del parámetro.</param>
        public void AddParameter(string name, string value)
        {
            if (command.Parameters.IndexOf(name) == -1)
            {
                DbParameter param = this.command.CreateParameter();
                param.ParameterName = name;
                param.DbType = DbType.String;
                param.Direction = ParameterDirection.Input;
                if (value != null)
                {
                    if (value.Length > 0)
                    {
                        param.Size = value.Length;
                        param.Value = value;
                    }
                    else
                    {
                        param.Size = 1;
                        param.Value = System.DBNull.Value;
                    }
                }
                this.command.Parameters.Add(param);
            }
            else
                this.command.Parameters[name].Value = value;
        }

        /// <summary>
        /// Confirma la transacción a l servicio de datos actual.
        /// </summary>
        public void ConfirmTransaction()
        {
            if (this.transactionIsSet == false)
                throw new InvalidOperationException("TransactionIsSet no se ha establecido a true.");

            this.transaction.Commit();
        }

        /// <summary>
        /// Deshace una transacción en un estado pendiente.
        /// </summary>
        public void UndoTransaction()
        {
            if (this.transactionIsSet == false)
                throw new InvalidOperationException("TransactionIsSet no se ha establecido a true.");

            this.transaction.Rollback();
        }
        #endregion

        #region Private Methods
        void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (transaction != null)
                        transaction.Dispose();
                    if (connection.State != ConnectionState.Open)
                        connection.Close();
                    connection.Dispose();
                    if (command != null)
                        command.Dispose();
                    if (factory != null)
                        factory = null;
                }
            }
        }
        #endregion
        #endregion
    }
}