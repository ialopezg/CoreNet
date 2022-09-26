using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace ProgrammersInc
{
    /// <summary>
    /// Clase que define metodos y propiedades para el acceso a informacion dentro de un archivo xml..
    /// </summary>
    public class Properties
    {
        Dictionary<string, object> properties = new Dictionary<string, object>();

        #region Properties
        /// <summary>
        /// Obtiene el número total de elementos.
        /// </summary>
        public int Count
        {
            get { return properties.Count; }
        }

        /// <summary>
        /// Obtiene el listado de los nombres de las propiedades.
        /// </summary>
        public string[] Elements
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (KeyValuePair<string, object> property in properties)
                    ret.Add(property.Key);
                return ret.ToArray();
            }
        }

        /// <summary>
        /// Obtiene un item de la lista determinado por la clave dada.
        /// </summary>
        public string this[string property]
        {
            get { return Convert.ToString(Get(property)); }
            set { Set(property, value); }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Obtiene si dentro de la colección actual se encuentra la clave dada por el
        /// parámetro <paramref name="property"/>.
        /// </summary>
        /// <param name="property">Nombre de la propiedad a consultar.</param>
        /// <returns></returns>
        public bool Contains(string property)
        {
            return properties.ContainsKey(property);
        }

        /// <summary>
        /// Obtiene el valor de una propiedad dada.
        /// </summary>
        /// <param name="property">Nombre de la propiedad requerida.</param>
        /// <returns>Un valor <see cref="System.String"/> que representara el valor de la propiedad deseada.</returns>
        public object Get(string property)
        {
            if (!properties.ContainsKey(property))
                return null;

            return properties[property];
        }

        /// <summary>
        /// Obtiene el valor valor para una propiedad, si esta no se encuentra en la colección
        /// se agregará y devolverá el valor por defecto.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">Nombre de la propiedad.</param>
        /// <param name="defaultValue">Valor por defecto de la propiedad.</param>
        /// <returns>Un objeto T con los datos de la propiedad.</returns>
        public T Get<T>(string property, T defaultValue)
        {
            if (!properties.ContainsKey(property))
            {
                properties.Add(property, defaultValue);
                return defaultValue;
            }
            object o = properties[property];

            if (o is string && typeof(T) != typeof(string))
            {
                TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
                try
                {
                    o = c.ConvertFromInvariantString(o.ToString());
                }
                catch (Exception)
                {
                    o = defaultValue;
                }
                properties[property] = o;
            }
            else if (o is ArrayList && typeof(T).IsArray)
            {
                ArrayList list = (ArrayList)o;
                Type elementType = typeof(T).GetElementType();
                Array arr = System.Array.CreateInstance(elementType, list.Count);
                TypeConverter c = TypeDescriptor.GetConverter(elementType);
                try
                {
                    for (int i = 0; i < arr.Length; ++i)
                        if (list[i] != null)
                            arr.SetValue(c.ConvertFromInvariantString(list[i].ToString()), i);
                    o = arr;
                }
                catch (Exception)
                {
                    o = defaultValue;
                }
                properties[property] = o;
            }
            else if (!(o is string) && typeof(T) == typeof(string))
            {
                TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
                if (c.CanConvertTo(typeof(string)))
                    o = c.ConvertToInvariantString(o);
                else
                    o = o.ToString();
            }

            try
            {
                return (T)o;
            }
            catch (NullReferenceException)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Lee las propiedades desde un archivo xml cargado en un lector XML.
        /// </summary>
        /// <param name="reader">Lector con los datos.</param>
        /// <param name="endElement">Elemento final.</param>
        public void ReadProperties(XmlReader reader, string endElement)
        {
            if (reader.IsEmptyElement)
                return;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == endElement)
                            return;
                        break;
                    case XmlNodeType.Element:
                        string propertyName = reader.LocalName;
                        if (propertyName == "Properties")
                        {
                            propertyName = reader.GetAttribute(0);
                            Properties p = new Properties();
                            p.ReadProperties(reader, "Properties");
                            properties[propertyName] = p;
                        }
                        else if (propertyName == "Array")
                        {
                            propertyName = reader.GetAttribute(0);
                            properties[propertyName] = ReadArray(reader);
                        }
                        else
                            properties[propertyName] = reader.HasAttributes ? reader.GetAttribute(0) : null;
                        break;
                }
            }
        }

        /// <summary>
        /// Elimina la propiedad dada por el parámetro <paramref name="property"/> de la colección de propiedades.
        /// </summary>
        /// <param name="property">Propiedad a eliminar.</param>
        /// <returns>true si la acción se ejecuto satisfactoriamente, en otro caso false.</returns>
        public bool Remove(string property)
        {
            return properties.Remove(property);
        }

        /// <summary>
        /// Guarda los datos de todas las propiedades activas.
        /// </summary>
        /// <param name="fileName">Archivo donde se guardaran los datos.</param>
        public void Save(string fileName)
        {
            using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartElement("Properties");
                WriteProperties(writer);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Establece el valores para una propiedad o colección de propiedades.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">Nombre de la propiedad.</param>
        /// <param name="value">Valor de la propiedad.</param>
        public void Set<T>(string property, T value)
        {
            T oldValue = default(T);
            if (!properties.ContainsKey(property))
                properties.Add(property, value);
            else
            {
                oldValue = Get<T>(property, value);
                properties[property] = value;
            }
            OnPropertyChanged(new PropertyChangedEventArgs(this, property, oldValue, value));
        }

        /// <summary>
        /// Retorna una clase <see cref="System.String"/> representando la clase <see cref="Properties"/>.
        /// </summary>
        /// <returns>Un cadena de texto con el valor de la propiedad actual.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[Properties:{");
            foreach (KeyValuePair<string, object> entry in properties)
            {
                sb.Append(entry.Key);
                sb.Append("=");
                sb.Append(entry.Value);
                sb.Append(",");
            }
            sb.Append("}]");
            return sb.ToString();
        }

        /// <summary>
        /// Escribe los datos de todas las propiedades activas en un escritor XML.
        /// </summary>
        /// <param name="writer">Escritor de archivos XML donde se guardaran las propiedades.</param>
        public void WriteProperties(XmlTextWriter writer)
        {
            foreach (KeyValuePair<string, object> entry in properties)
            {
                object val = entry.Value;
                if (val is Properties)
                {
                    writer.WriteStartElement("Properties");
                    writer.WriteAttributeString("name", entry.Key);
                    ((Properties)val).WriteProperties(writer);
                    writer.WriteEndElement();
                }
                else if (val is Array || val is ArrayList)
                {
                    writer.WriteStartElement("Array");
                    writer.WriteAttributeString("name", entry.Key);
                    foreach (object o in (IEnumerable)val)
                    {
                        writer.WriteStartElement("Element");
                        WriteValue(writer, o);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                else
                {
                    writer.WriteStartElement(entry.Key);
                    WriteValue(writer, val);
                    writer.WriteEndElement();
                }
            }
        }
        #endregion

        #region Privates Methods
        void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        ArrayList ReadArray(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return new ArrayList(0);
            ArrayList l = new ArrayList();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == "Array")
                        {
                            return l;
                        }
                        break;
                    case XmlNodeType.Element:
                        l.Add(reader.HasAttributes ? reader.GetAttribute(0) : null);
                        break;
                }
            }
            return l;
        }

        void WriteValue(XmlWriter writer, object val)
        {
            if (val != null)
            {
                if (val is string)
                    writer.WriteAttributeString("value", val.ToString());
                else
                {
                    TypeConverter c = TypeDescriptor.GetConverter(val.GetType());
                    writer.WriteAttributeString("value", c.ConvertToInvariantString(val));
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// evento que se dispara cuando el contenido de una propiedad ha sido modificado.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}