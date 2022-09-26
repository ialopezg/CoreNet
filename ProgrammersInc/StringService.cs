namespace ProgrammersInc
{
    /// <summary>
    /// Clase que define m�todos personalizados para el tratamiento de cadenas de texto.
    /// </summary>
    public sealed class StringService
    {
        /// <summary>
        /// Capitaliza la primer letra de un cadena de texto dada.
        /// </summary>
        /// <param name="stringText">Cadena de texto a tratar.</param>
        /// <returns>La cadena de texto formateada.</returns>
        public static string FirstCharCapitalized(string stringText)
        {
            if (stringText.Length == 0)
                return stringText;

            char originalFirst = stringText[0];
            char upperFirst = char.ToUpper(originalFirst);

            if (originalFirst == upperFirst)
                return stringText;

            return upperFirst + stringText.Substring(1);
        }

        /// <summary>
        /// Devuelve una cadena que contiene un n�mero especificado de caracteres a partir del lado izquierdo de una cadena.
        /// </summary>
        /// <param name="str">str: Par�metro necesario. Expresi�n de tipo String de la que se devuelven los caracteres situados m�s a la izquierda.</param>
        /// <param name="length">Length: Par�metro necesario. Expresi�n de tipo Integer. Expresi�n num�rica que indica el n�mero de caracteres que se van a devolver. Si es 0, se devuelve una cadena de longitud cero (""). Si es mayor o igual que el n�mero de caracteres en str, se devuelve la cadena completa.</param>
        /// <returns>Devuelve una cadena que contiene un n�mero especificado de caracteres a partir del lado izquierdo de una cadena.</returns>
        public static string Left(string str, int length)
        {
            string result = str.Substring(0, length);

            return result;
        }

        /// <summary>
        /// Recorta una cadena en base a los parametros especificados
        /// </summary>
        /// <param name="str">Cadena a recortar</param>
        /// <param name="startIndex">Caracter donde se iniciara el recorte</param>
        /// <param name="length">Tama�o de la cadena resultante</param>
        /// <returns>Una sub cadena recortada de la cadena original en base 
        /// a los parametros especificados</returns>
        public static string Mid(string str, int startIndex, int length)
        {
            string result = str.Substring(startIndex, length);

            return result;
        }

        /// <summary>
        /// Recorta una cadena en base a los parametros especificados
        /// </summary>
        /// <param name="str">Cadena a recortar</param>
        /// <param name="startIndex">Caracter donde se iniciara el recorte</param>
        /// <returns>Una sub cadena recortada de la cadena original en base 
        /// a los parametros especificados</returns>
        public static string Mid(string str, int startIndex)
        {
            string result = str.Substring(startIndex);

            return result;
        }

        /// <summary>
        /// Devuelve una cadena que contiene un n�mero especificado de caracteres a partir del lado derecho de una cadena.
        /// </summary>
        /// <param name="str">str: Par�metro necesario. Expresi�n de tipo String de la que se devuelven los caracteres situados m�s a la derecha.</param>
        /// <param name="length">Length: Par�metro necesario. Integer. Expresi�n num�rica que indica el n�mero de caracteres que se van a devolver. Si es 0, se devuelve una cadena de longitud cero (""). Si es mayor o igual que el n�mero de caracteres en str, se devuelve la cadena completa</param>
        /// <returns>Devuelve una cadena que contiene un n�mero especificado de caracteres a partir del lado derecho de una cadena.</returns>
        public static string Right(string str, int length)
        {
            string result = str.Substring(str.Length - length, length);

            return result;
        }
    }
}