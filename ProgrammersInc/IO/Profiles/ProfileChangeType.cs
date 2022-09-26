namespace ProgrammersInc.IO
{
    /// <summary>
    /// Define los distinto tipos de cambios que se pueden dar en un perfil.
    /// </summary>
    public enum ProfileChangeType
    {
        /// <summary>
        /// Cambio referido a la propiedad <see cref="Profile.Name"/>.
        /// </summary>
        Name = 0,
        /// <summary>
        /// Cambio referido a la propiedad <see cref="Profile.ReadOnly"/>.
        /// </summary>
        ReadOnly = 1,
        /// <summary>
        /// Cambio referido al método <see cref="Profile.RemoveEntry"/>.
        /// </summary>
        RemoveEntry = 2,
        /// <summary>
        /// Cambio referido al método <see cref="Profile.RemoveSection"/>.
        /// </summary>
        RemoveSection = 4,
        /// <summary>
        /// Cambio referido al método <see cref="Profile.WriteValue"/>.
        /// </summary>
        WriteValue = 8,
        /// <summary>
        /// Otro.
        /// </summary>
        Other = 16
    }
}