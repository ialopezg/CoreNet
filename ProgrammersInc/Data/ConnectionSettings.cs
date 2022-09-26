using System;
using System.IO;
using ProgrammersInc.IO;

namespace ProgrammersInc.Data
{
    /// <summary>
    /// Define las propiedades de una conexión a un servidor de datos.
    /// </summary>
    public class ConnectionSettings
    {
        #region Constants
        // SQL Server Connections Strings
        const string SQL_CONNECTIONSTRING_STANDAR = "Data Source={0};Initial Catalog={1};User Id={2};Password={3};";
        const string SQL_CONNECTIONSTRING_TRUSTED = "Data Source={0};Initial Catalog={1};Integrated Security=SSPI;";
        const string SQL_CONNECTIONSTRIN_VIA_IP = "Data Source={0},{1};Network Library=DBMSSOCN;Initial Catalog={2};User ID={3};Password={4};";

        // Microsoft Access 97 - 2003 Connection Strings
        const string MSA_CONNECTIONSTRING1 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};User Id={1};Password=;";
        const string MSA_CONNECTIONSTRING2 = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Jet OLEDB:Database Password={1};";

        // Microsoft Access 2007 Connection Strings
        const string MSA2K7_CONNECTIONSTRING_STANDAR = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Persist Security Info=False;";
        const string MSA2K7_CONNECTIONSTRING_WITH_SECURITY = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Jet OLEDB:Database Password={1};";
        #endregion

        #region Constructors
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ConnectionSettings"/>.
        /// </summary>
        public ConnectionSettings() { }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ConnectionSettings"/>.
        /// </summary>
        /// <param name="serverType">Tipo de servidor.</param>
        /// <param name="serverName">Nombre del servidor.</param>
        /// <param name="databaseName">Nombre de la base de datos.</param>
        /// <param name="userName">Nombre de usuario.</param>
        /// <param name="password">Contraseña del usuario.</param>
        public ConnectionSettings(ServerType serverType, string serverName, string databaseName, string userName, string password)
        {
            this.serverType = serverType;
            this.serverName = serverName;
            this.databaseName = databaseName;
            this.userName = userName;
            this.password = password;
            this.byIPAccess = true;

            ParseConnectionString();
        }
        #endregion

        #region Properties Implementation
        bool byIPAccess;
        /// <summary>
        /// Específica un valor indicando que la conexión se hará mediante el acceso IP.
        /// </summary>
        public bool ByIPAccess
        {
            get { return byIPAccess; }
        }
	
        ServerType serverType;
        /// <summary>
        /// Obtiene el tipo de servidor que se usará en la conexión.
        /// </summary>
        public ServerType ServerType
        {
            get { return serverType; }
        }

        string serverName;
        /// <summary>
        /// Nombre del servidor a ser usado.
        /// </summary>
        public string ServerName
        {
            get { return serverName; }
        }

        string  databaseName;
        /// <summary>
        /// Nombre de la base de datos.
        /// </summary>
        public string  DatabaseName
        {
            get { return databaseName; }
        }

        string userName;
        /// <summary>
        /// Obtiene el nombre de usuario.
        /// </summary>
        public string UserName
        {
            get { return userName; }
        }

        string password;
        /// <summary>
        /// Obtiene la contraseña del usuario.
        /// </summary>
        public string Password
        {
            get { return password; }
        }

        string connectionString;
        /// <summary>
        /// Obtiene la cadena de conexión a usarse.
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString; }
        }
        #endregion
        
        #region Methods Implementation
        public bool LoadPropertiesFromStream(Ini iniFile, string connectionName)
        {
            try
            {
                if (iniFile == null)
                    throw new ArgumentNullException("iniFile");

                string section = string.Empty;
                if (iniFile.GetSectionNames().Contains(connectionName))
                {
                    section = connectionName;

                    serverType = (ServerType)iniFile.GetValue(section, "ServerType", 0);
                    switch (serverType)
                    {
                        case ServerType.MICROSOFT_ACCESS:
                            {
                                databaseName = iniFile.GetValue(section, "DataSource", string.Empty);
                                password = iniFile.GetValue(section, "DataSource", string.Empty);
                            }
                            break;
                        case ServerType.MICROSOFT_ACCESS_2007:
                            {
                                databaseName = iniFile.GetValue(section, "DataSource", string.Empty);
                                password = iniFile.GetValue(section, "DataSource", string.Empty);
                            }
                            break;
                        case ServerType.MICROSOFT_SQL_SERVER:
                            {
                                serverName = iniFile.GetValue(section, "ServerName", string.Empty);
                                databaseName = iniFile.GetValue(section, "InitialCatalog", string.Empty);
                                userName = iniFile.GetValue(section, "UserID", string.Empty);
                                password = iniFile.GetValue(section, "Password", string.Empty);
                            }
                            break;
                    }
                    ParseConnectionString();

                    return true;
                }
                else
                    throw new InvalidOperationException("La conexión especificada no existe...");
            }
            catch (Exception) { return false; }
        }

        void ParseConnectionString()
        {
            if (this.serverType == ServerType.MICROSOFT_SQL_SERVER)
            {
                if (string.IsNullOrEmpty(serverName))
                    throw new ArgumentNullException("serverName");
                if (string.IsNullOrEmpty(databaseName))
                    throw new ArgumentNullException("databaseName");

                if (!byIPAccess || !string.IsNullOrEmpty(userName))
                    connectionString = string.Format(SQL_CONNECTIONSTRING_STANDAR, serverName, databaseName, userName, password);
                else if (!byIPAccess || string.IsNullOrEmpty(userName))
                    connectionString = string.Format(SQL_CONNECTIONSTRING_TRUSTED, serverName, databaseName);
                else if (byIPAccess || !string.IsNullOrEmpty(userName))
                    connectionString = string.Format(SQL_CONNECTIONSTRING_TRUSTED, "1433", serverName, databaseName, userName, password);
            }
            else if (serverType == ServerType.MICROSOFT_ACCESS)
            {
                if (string.IsNullOrEmpty(databaseName))
                    throw new ArgumentNullException("databaseName");

                if (string.IsNullOrEmpty(password))
                    connectionString = string.Format(MSA_CONNECTIONSTRING1, databaseName);
                else if (!string.IsNullOrEmpty(password))
                    connectionString = string.Format(MSA_CONNECTIONSTRING2, databaseName, password);
            }
            else if (serverType == ServerType.MICROSOFT_ACCESS_2007)
            {
                if (string.IsNullOrEmpty(databaseName))
                    throw new ArgumentNullException("databaseName");

                if (string.IsNullOrEmpty(password))
                    connectionString = string.Format(MSA2K7_CONNECTIONSTRING_STANDAR, databaseName);
                else if (!string.IsNullOrEmpty(password))
                    connectionString = string.Format(MSA2K7_CONNECTIONSTRING_WITH_SECURITY, databaseName, password);
            }
        }

        /// <summary>
        /// Verifica si el servidor de datos asociado a la cadena de conexión puede escuchar perticiones de datos.
        /// </summary>
        /// <returns>Si el servidor es accesible retornará <c>true</c>, en otro caso retornará <c>false</c>.</returns>
        public bool ServerIsAlive()
        {
            Database server = new Database(this);

            return server.TestConnection();
        }
        #endregion
    }
}