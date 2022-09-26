using System;
using System.Data;
using System.Data.Common;

namespace ProgrammersInc.Data
{
    /// <summary>
    /// Clase que define los métodos y propiedades para el acceso a un servicio de datos del tipo
    /// Microsoft SQL Server.
    /// </summary>
    public class Database : DataServer
    {
        #region Constants
        const string CONNECTIONSTRING_STANDAR = "Data Source={0};Initial Catalog={1};User Id={2};Password={3};";
        const string CONNECTIONSTRING_TRUSTED = "Data Source={0};Initial Catalog={1};Integrated Security=SSPI;";
        const string CONNECTIONSTRIN_VIA_IP = "Data Source={0},{1};Network Library=DBMSSOCN;Initial Catalog={2};User ID={3};Password={4};";
        #endregion

        #region Constructors
        /// <summary>
        /// Crea una instantia de la clase para ser implementada en conexiones del tipo  Microsoft SQL Server.
        /// </summary>
        /// <param name="serverName">Nombre del servidor.</param>
        /// <param name="catalogName">Nombre del catálogo inicial.</param>
        public Database(string serverName, string catalogName)
            : this(serverName, catalogName, string.Empty, string.Empty) { }

        /// <summary>
        /// Crea una instantia de la clase para ser implementada en conexiones del tipo Microsoft SQL Server.
        /// </summary>
        /// <param name="ipAddress">Dirección IP del servidor de datos.</param>
        /// <param name="port">Puerto a usar para escuchar datos.</param>
        /// <param name="catalogName">Nombre del catálogo inicial.</param>
        /// <param name="userName">Nombre de Usuario.</param>
        /// <param name="password">Contraseña del usuario.</param>
        public Database(string ipAddress, string port, string catalogName, string userName, string password)
            : this(ipAddress + "," + port, catalogName, userName, password) { }

        /// <summary>
        /// Crea una instantia de la clase para ser implementada en conexiones del tipo  Microsoft SQL Server.
        /// </summary>
        /// <param name="serverName">Nombre del servidor.</param>
        /// <param name="catalogName">Nombre del catálogo inicial.</param>
        /// <param name="userName">Nombre de Usuario.</param>
        /// <param name="password">Contraseña del usuario.</param>
        public Database(string serverName, string catalogName, string userName, string password)
            : base()
        {
            this.serverType = ServerType.MICROSOFT_SQL_SERVER;
            this.Factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            this.Connection = this.Factory.CreateConnection();
            if (string.IsNullOrEmpty(userName))
                this.ConnectionString = string.Format(CONNECTIONSTRING_TRUSTED, serverName, catalogName);
            else if (!string.IsNullOrEmpty(userName))
                this.ConnectionString = string.Format(CONNECTIONSTRING_STANDAR, serverName, catalogName, userName, password);
            this.Connection.ConnectionString = this.ConnectionString;
            this.SetCommand();
        }

        public Database(ConnectionSettings settings)
            : base()
        {
            this.serverType = settings.ServerType;
            switch (serverType)
            {
                case ServerType.MICROSOFT_SQL_SERVER:
                    this.Factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
                    break;
                case ServerType.MICROSOFT_ACCESS:
                case ServerType.MICROSOFT_ACCESS_2007:
                    this.Factory = DbProviderFactories.GetFactory("System.Data.OleDb");
                    break;
            }
            this.Connection = this.Factory.CreateConnection();
            this.Connection.ConnectionString = settings.ConnectionString;
            this.SetCommand();
        }
        #endregion

        #region Properties
        ServerType serverType;
        /// <summary>
        /// Obtiene el tipo de servidor como actua esta instancia.
        /// </summary>
        public override ServerType ServerType
        {
            get { return serverType; }
        }
        #endregion
    }
}