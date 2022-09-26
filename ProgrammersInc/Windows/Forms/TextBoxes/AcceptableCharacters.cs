namespace System.Windows.Forms
{
    /// <summary>
    /// Representa los tipos de caracteres soportados por el control <see cref="FilterTextBox"/>.
    /// </summary>
    public enum AcceptableCharacters
    {
        /// <summary>
        /// Todos los caracteres.
        /// </summary>
        All = 0,
        /// <summary>
        /// Unicamente digitos.
        /// </summary>
        DigitOnly = 1,
        /// <summary>
        /// Unicamente letras.
        /// </summary>
        LetterOnly = 2,
        /// <summary>
        /// Letras o numeros.
        /// </summary>
        LetterOrDigit = 4
    }
}