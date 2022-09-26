namespace ProgrammersInc.Data
{
    /// <summary>
    /// Describe el tipo de conexión con un origen de datos.
    /// </summary>
    public enum ServerType
    {
        /// <summary>
        /// Soporte para bases de datos Microsoft Office 97 - 2003.
        /// </summary>
        MICROSOFT_ACCESS = 0,
        /// <summary>
        /// Soporte para bases de datos Microsoft Office 2007.
        /// </summary>
        MICROSOFT_ACCESS_2007 = 1,
        /// <summary>
        /// Soporte para bases de datos Microsoft SQL SERVER.
        /// </summary>
        MICROSOFT_SQL_SERVER = 2
    };
}