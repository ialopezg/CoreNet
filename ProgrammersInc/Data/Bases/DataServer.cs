using System;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;

namespace ProgrammersInc.Data
{
    /// <summary>
    /// Clase base que define los métodos para la conexion a origenes de datos.
    /// </summary>
    public abstract class DataServer : DataSource
    {
        #region Variables Implementation
        bool SetPrepare = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Data.DataServer"/>.
        /// </summary>
        public DataServer()
            : base() { }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Data.DataServer"/>.
        /// </summary>
        /// <param name="provider">Objeto que se usará para crear un proveedor de las
        /// clases e origen de datos.</param>
        /// <param name="connectionString">La cadena que se utilizará para abrir la conexión.</param>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> es null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="connectionString"/> es null.</exception>
        public DataServer(DbProviderFactory provider, string connectionString)
            : base(provider, connectionString) { }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="ProgrammersInc.Data.DataServer"/>.
        /// </summary>
        /// <param name="providerName">Nombre invariable de un proveedor.</param>
        /// <param name="connectionString">La cadena que se utilizará para abrir la conexión.</param>
        /// <exception cref="ArgumentNullException"><paramref name="provider"/> es null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="connectionString"/> es null.</exception>
        public DataServer(string providerName, string connectionString)
            : base(providerName, connectionString) { }
        #endregion

        #region Methods Implementation
        #region Public
        /// <summary>
        /// Ejecuta una instrucción T-SQL en un objeto de conexión.
        /// </summary>
        /// <returns>Número de filas afectadas.</returns>
        public int ExecuteQuerry()
        {
            int result;
            if (this.Connection.State.Equals(ConnectionState.Closed))
                this.Connection.Open();

            if (this.TransactionIsSet == true)
            {
                if (this.Transaction == null)
                    this.Transaction = Connection.BeginTransaction();

                if (this.Command == null)
                    this.Command.Transaction = this.Transaction;
            }

            try
            {
                if (this.SetPrepare == true)
                    this.Command.Prepare();

                result = (int)(this.Command.ExecuteNonQuery());
            }
            catch (DbException) { result = -1; }
            finally
            {
                this.Command.Parameters.Clear();
                if (this.TransactionIsSet == false)
                    this.Connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Devuelve true si se han encontrado registros, en otro caso retornará false.
        /// </summary>
        /// <returns>Si hay registros o no en el catálogo actual.</returns>
        public bool ExistsRecords()
        {
            if (this.RecordsCount() > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Obtiene el valor de un campo.
        /// </summary>
        /// <returns>El valor del campo.</returns>
        public object FieldValue()
        {
            this.Connection.Open();
            object result = this.Command.ExecuteScalar();
            Connection.Close();
            if (result != null)
                return result;
            else
                return string.Empty;
        }

        /// <summary>
        /// Devuelve una cache de memoria interna de datos.
        /// </summary>
        /// <returns>Una cache de memoria interna de datos.</returns>
        public DataSet GetDataSet()
        {
            try
            {
                DbDataAdapter adapter = this.Factory.CreateDataAdapter();
                adapter.SelectCommand = this.Command;
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                return ds;
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Devuelve una fila de datos en un <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// <returns>Una fila de datos en un <see cref="System.Data.DataTable"/>.</returns>
        public DataRow GetDataRow()
        {
            DataTable dt = GetDataTable();
            if (dt.Rows.Count > 0)
                return dt.Rows[0];
            else
                return null;
        }

        /// <summary>
        /// Devuelve una tabla de datos en memoria.
        /// </summary>
        /// <returns>Una tabla de datos en memoria.</returns>
        public DataTable GetDataTable()
        {
            DataSet ds = GetDataSet();
            if (ds.Tables.Count > 0)
                return ds.Tables[0];
            else
                return null;
        }

        /// <summary>
        /// Obtiene la siguiente secuencia para la tabla de datos específicada por
        /// el parámetro <paramref name="tableName"/>.
        /// </summary>
        /// <param name="tableName">Nombre de la tabla del que se requiere la secuencia.</param>
        /// <returns></returns>
        public int GetNextSequenceNumber(string tableName)
        {
            this.PrepareQuerry("SELECT CurrentValue FROM Sequences WHERE TableName = ?", CommandType.Text);
            base.AddParameter("TableName", tableName);
            int value = (int)FieldValue();
            this.SetCommand();
            this.PrepareQuerry("UPDATE Sequences SET CurrentValue = CurrentValue + 1 WHERE TableName = ?", CommandType.Text);
            base.AddParameter("TableName", tableName);
            this.ExecuteQuerry();

            return value;
        }

        /// <summary>
        /// Llena un objeto de listado "<see cref="System.Windows.Forms.ComboBox"/>", con los datos 
        /// resultantes de una sesión de datos.
        /// </summary>
        /// <param name="control">Control a llenar.</param>
        public void PopulateList(ref ComboBox control)
        {
            try
            {
                DataTable dt = this.GetDataTable();
                control.Items.Clear();
                foreach (DataRow dr in dt.Rows)
                    control.Items.Add(dr[0].ToString());
            }
            catch (Exception) { control.Items.Clear(); }
        }

        /// <summary>
        /// Rellena un objeto de listado "<see cref="System.Windows.Forms.ListBox"/>", con los datos 
        /// resultantes de una sesión de datos.
        /// </summary>
        /// <param name="control">Control a llenar.</param>
        public void PopulateList(ref ListBox control)
        {
            try
            {
                DataTable dt = this.GetDataTable();
                control.Items.Clear();
                foreach (DataRow dr in dt.Rows)
                    control.Items.Add(dr[0].ToString());
            }
            catch (Exception) { control.Items.Clear(); }
        }

        /// <summary>
        /// Prepara el objeto command para ejecuta una instrucción SQL, Procedimiento
        /// almacenado o el acceso a una tabla de datos.
        /// </summary>
        /// <param name="commandText">Instrucción SQL, Procedimient almacenado o
        /// Nombre de la tabla a ejecutarse.</param>
        /// <param name="commandType">Tipo de comando a ejecutar.</param>
        public void PrepareQuerry(string commandText, CommandType commandType)
        {
            if (this.Command.CommandText.Length == 0)
            {
                this.SetPrepare = true;
                this.Command.CommandType = commandType;
                this.Command.CommandText = commandText;
            }
            else
            {
                if (this.Command.CommandText == commandText)
                {
                    this.SetPrepare = true;
                    this.SetCommand();
                    this.Command.CommandType = commandType;
                    this.Command.CommandText = commandText;
                }
                else
                    this.SetPrepare = false;
            }
        }

        /// <summary>
        /// Devuelve el número total de registros.
        /// </summary>
        /// <returns>Número total de registros.</returns>
        public int RecordsCount()
        {
            try
            {
                this.Connection.Open();
                return (int)this.Command.ExecuteNonQuery();
            }
            catch (Exception) { return 0; }
            finally 
            {
                if (this.Connection.State == ConnectionState.Open)
                    this.Connection.Close();
            }
        }

        /// <summary>
        /// Realiza una prueba de conexión al origen de datos asociado a la instancia actual.
        /// </summary>
        /// <returns><c>true</c> si el Servidor responde, de lo contrario <c>false</c>.</returns>
        public bool TestConnection()
        {
            try
            {
                if (Connection.State == ConnectionState.Closed)
                    Connection.Open();

                return (Connection.State == ConnectionState.Open) ? true : false;
            }
            catch (Exception) { return false; }
            finally
            {
                if (Connection.State.Equals(ConnectionState.Open))
                    Connection.Close();
            }
        }
        #endregion
        #endregion
    }
}