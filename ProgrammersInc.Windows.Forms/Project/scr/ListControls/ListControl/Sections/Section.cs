using System;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Representa un elemento visible dentro de un control <see cref="SuperList"/>.
    /// </summary>
    public abstract class Section : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="Section"/>.
        /// </summary>
        /// <param name="host">Control que contendrá este elemento.</param>
        public Section(ISectionHost host)
        {
            if (host == null)
                throw new ArgumentNullException("host");

            this.host = host;
        }
        #endregion

        #region Properties
        private ISectionHost host;
        /// <summary>
        /// Devuelve que control que contendrá el elemento actual.
        /// </summary>
        public ISectionHost Host
        {
            get { return this.host; }
        }
	
        #endregion

        #region Methods Implementation
        public virtual void Dispose()
        {
 
        }
        #endregion
    }
}