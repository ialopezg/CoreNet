using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Xml;

namespace ProgrammersInc.Utility.Assemblies
{
    /// <summary>
    /// Clase que permite la manipulación de recursos dentro de un ensamblado.
    /// </summary>
    public sealed class ManifestResources
    {
        #region Constructors
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ManifestResources"/>.
        /// </summary>
        /// <param name="baseNamespace">Nombre base sobre el que trabajará esta instancia.</param>
        public ManifestResources(string baseNamespace)
            : this(baseNamespace, Assembly.GetCallingAssembly()) { }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ManifestResources"/>.
        /// </summary>
        /// <param name="baseNamespace">Nombre base sobre el que trabajará esta instancia.</param>
        /// <param name="assembly">Ensamblado que contiene el recurso.</param>
        public ManifestResources(string baseNamespace, Assembly assembly)
        {
            if (baseNamespace == null)
                throw new ArgumentNullException("baseNamespace");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            this.baseNamespace = baseNamespace;
            this.assembly = assembly;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Devuelve el ícono ubicado en la ruta especificada.
        /// </summary>
        /// <param name="path">Ruta a evaluar.</param>
        /// <returns>El ícono ubicado en la ruta especificada.</returns>
        public Icon GetIcon(string path)
        {
            using (Stream stream = GetStream(path))
            {
                return new Icon(stream);
            }
        }

        /// <summary>
        /// Devuelve la imagen ubicado en la ruta especificada.
        /// </summary>
        /// <param name="path">Ruta a evaluar.</param>
        /// <returns>La imagen ubicada en la ruta especificada.</returns>
        public Image GetImage(string path)
        {
            using (Stream stream = GetStream(path))
            {
                return Image.FromStream(stream);
            }
        }

        /// <summary>
        /// Devuelve un objeto Stream de la ruta dada.
        /// </summary>
        /// <param name="path">Ruta a evaluar.</param>
        /// <returns>Un objeto Stream de la ruta dada.</returns>
        public Stream GetStream(string path)
        {
            return assembly.GetManifestResourceStream(baseNamespace + "." + path);
        }

        /// <summary>
        /// Devuelve el texto ubicado en la ruta especificada.
        /// </summary>
        /// <param name="path">Ruta a evaluar.</param>
        /// <returns>El texto ubicado en la ruta especificada.</returns>
        public string GetString(string path)
        {
            using (Stream stream = GetStream(path))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Devuelve el recurso como un objeto XML.
        /// </summary>
        /// <param name="path">Ruta base para la acción.</param>
        /// <returns>El recurso como un objeto XML.</returns>
        public XmlDocument GetXmlDocument(string path)
        {
            XmlDocument xmlDoc = new XmlDocument();

            using (Stream stream = GetStream(path))
            {
                if (stream == null)
                    throw new ArgumentException(string.Format("Resource '{0}' not found.", path), "path");
                xmlDoc.Load(stream);
            }

            return xmlDoc;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Obtiene los nombre de los recursos en el recurso asociado a esta instancia.
        /// </summary>
        public string[] ResourceNames
        {
            get { return assembly.GetManifestResourceNames(); }
        }
        #endregion

        #region Fields
        string baseNamespace;
        Assembly assembly;
        #endregion
    }
}
