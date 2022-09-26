using System;
using System.Collections;
using System.Collections.Generic;

namespace ProgrammersInc.Utility.Collections
{
	/// <summary>
	/// Clase que provoca eventos cuando los elementos son agregados/eliminados.
	/// </summary>
	public class EventingList<T>: IList<T>
    {
        #region Methods
        /// <summary>
        /// Agrega el elemento dado al final de esta colecci�n.
        /// </summary>
        /// <param name="item">Elemento a ser agregado.</param>
        public void Add(T item)
        {
            OnPreDataChanged(new EventInfo(EventType.Added, item));
            items.Add(item);
            OnDataChanged(new EventInfo(EventType.Added, item));
        }

        /// <summary>
        /// Agrega la colecci�n de elementos dados al final de esta colecci�n.
        /// </summary>
        /// <param name="items">Elementos a agregarse.</param>
        public void AddRange(T[] items)
        {
            OnPreDataChanged(new EventInfo(EventType.Added, items));
            this.items.AddRange(items);
            OnDataChanged(new EventInfo(EventType.Added, items));
        }

        /// <summary>
        /// Quita todos los elementos de esta colecci�n.
        /// </summary>
        public void Clear()
        {
            if (items.Count > 0)
            {
                T[] itemsIn = this.items.ToArray();
                OnPreDataChanged(new EventInfo(EventType.Deleted, itemsIn));
                this.items.Clear();
                OnDataChanged(new EventInfo(EventType.Deleted, itemsIn));
            }
        }

        /// <summary>
        /// Determina si el elemento dado se encuentra en esta colecci�n.
        /// </summary>
        /// <param name="item">Elemento a buscarse.</param>
        /// <returns><c>true</c> si el elemento es encontrado, en otro caso <c>false</c>.</returns>
        public bool Contains(T item)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// Copia todos los elementos de esta instancia a partir del �ndice de base cero dado.
        /// </summary>
        /// <param name="array">Array de destino de los elementos.</param>
        /// <param name="arrayIndex">Indice de base cero de donde se iniciar� la copia de los
        /// elementos.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Devuelve un enumerador que recorre en iteraci�n la colecci�n.
        /// </summary>
        /// <returns>Un enumerador que recorre en iteraci�n la colecci�n.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        /// <summary>
        /// Busca el elemento dado y devuelve el �ndice de base cero de la primera aparici�on del elemento
        /// en esta colecci�n.
        /// </summary>
        /// <param name="item">Elemento a buscar.</param>
        /// <returns>El �ndice de base cero de la primera aparici�on del elemento
        /// en esta colecci�n.</returns>
        public int IndexOf(T item)
        {
            return items.IndexOf(item);
        }

        /// <summary>
        /// Inserta el elemento espec�ficado en el �ndice de base cero dado.
        /// </summary>
        /// <param name="index">Indice donde se almacenar� el elemento.</param>
        /// <param name="item">Elemento a insertar.</param>
        public void Insert(int index, T item)
        {
            OnPreDataChanged(new EventInfo(EventType.Added, item));
            items.Insert(index, item);
            OnDataChanged(new EventInfo(EventType.Added, item));
        }

        /// <summary>
        /// Mueve el elemento de �ndice de origen al �ndice de destino dados.
        /// </summary>
        /// <param name="from">Indice de origen.</param>
        /// <param name="to">Indice de destino.</param>
        public void MoveItem(int from, int to)
        {
            if (from < 0 || from >= items.Count)
                throw new ArgumentOutOfRangeException("from");
            if (to < 0 || to >= items.Count)
                throw new ArgumentOutOfRangeException("to");

            T temp = items[from];
            if (to > from)
                to--;
            items.RemoveAt(from);
            items.Insert(to, temp);
        }

        /// <summary>
        /// Provoca el evento <see cref="DataChanged"/>.
        /// </summary>
        /// <param name="eventInfo">Datos del evento.</param>
        protected virtual void OnDataChanged(EventInfo eventInfo)
        {
            if (DataChanged != null)
                DataChanged(this, eventInfo);
        }

        /// <summary>
        /// Provoca el evento <see cref="PreDataChanged"/>.
        /// </summary>
        /// <param name="eventInfo">Datos del evento.</param>
        protected virtual void OnPreDataChanged(EventInfo eventInfo)
        {
            if (PreDataChanged != null)
                PreDataChanged(this, eventInfo);
        }

        /// <summary>
        /// Quita el elemento dado de esta colecci�n.
        /// </summary>
        /// <param name="item">Elemento a ser borrado.</param>
        /// <returns><c>true</c> si la funci�n fue exitosa, en otro caso <c>false</c>.</returns>
        public bool Remove(T item)
        {
            int i = items.IndexOf(item);
            if (i != -1)
            {
                RemoveAt(i);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Quita el elemento ubicado en el �ndice de base cero dado.
        /// </summary>
        /// <param name="index">Indice del elemento a borrar.</param>
        public void RemoveAt(int index)
        {
            T item = items[index];
            OnPreDataChanged(new EventInfo(EventType.Deleted, item));
            items.RemoveAt(index);
            OnDataChanged(new EventInfo(EventType.Deleted, item));
        }

        /// <summary>
        /// Elimina un rango de elementos ubicados en la posici�n inicial y que coincida con la cantidad
        /// total de elementos a suprimirse.
        /// </summary>
        /// <param name="start">Posici�n inicial.</param>
        /// <param name="count">Cantidad de elementos a elminarse.</param>
        public void RemoveRange(int start, int count)
        {
            T[] items = this.items.GetRange(start, count).ToArray();
            OnPreDataChanged(new EventInfo(EventType.Deleted, items));
            this.items.RemoveRange(start, count);
            OnDataChanged(new EventInfo(EventType.Deleted, items));
        }

        /// <summary>
        /// Borra la colecci�n de elementos dados.
        /// </summary>
        /// <param name="items">Elementos a borrarse.</param>
        public void RemoveRange(T[] items)
        {
            if (items.Length == 0)
                return;

            OnPreDataChanged(new EventInfo(EventType.Deleted, items));

            Dictionary<T, int> itemIndexes = new Dictionary<T, int>(this.items.Count);
            int i = 0;
            foreach (T item in items)
                itemIndexes[item] = i++;
            List<int> removeList = new List<int>();
            foreach (T item in items)
            {
                int index;
                if (itemIndexes.TryGetValue(item, out index))
                {
                    itemIndexes.Remove(item); // only allow to be deleted once
                    removeList.Add(index);
                }
            }
            if (removeList.Count == this.items.Count)
                this.items.Clear();
            else
            {
                removeList.Sort();
                int start = -1;
                int count = 0;
                for (i = removeList.Count - 1; i >= 0; i--)
                {
                    int removeAt = removeList[i];
                    if (removeAt + 1 == start)
                    {
                        start = removeAt;
                        count++;
                    }
                    else
                    {
                        if (start != -1)
                            this.items.RemoveRange(start, count);
                        start = removeAt;
                        count = 1;
                    }
                }
                if (start != -1 && count > 0)
                    this.items.RemoveRange(start, count);
            }
            OnDataChanged(new EventInfo(EventType.Deleted, items));
        }

        /// <summary>
        /// Devuelve un array de los elementos de esta colecci�n.
        /// </summary>
        /// <returns>Un array de los elementos de esta colecci�n.</returns>
        public T[] ToArray()
        {
            return items.ToArray();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Obtiene el n�mero de elementos realmente contenido en esta instancia.
        /// </summary>
        public int Count
        {
            get { return items.Count; }
        }

        /// <summary>
        /// Obtiene un valor indicando si esta caloecci�n es de s�lo lectura.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Obtiene el elemento ubicado en el �ndice de base cero dado.
        /// </summary>
        /// <param name="index">Indice del elemento requerido.</param>
        /// <returns>El elemento ubicado en el �ndice de base cero dado.</returns>
        public T this[int index]
        {
            get { return items[index]; }
            set
            {
                if (!items[index].Equals(value))
                {
                    T itemToRemove = items[index];

                    OnPreDataChanged(new EventInfo(EventType.Deleted, itemToRemove));
                    OnPreDataChanged(new EventInfo(EventType.Added, value));

                    items[index] = value;

                    OnDataChanged(new EventInfo(EventType.Deleted, itemToRemove));
                    OnDataChanged(new EventInfo(EventType.Added, value));
                }
            }
        }

        /// <summary>
        /// Obtiene los elementos de esta instancia.
        /// </summary>
        protected List<T> UnderlyingList
        {
            get { return items; }
        }
        #endregion

        #region Fields
        List<T> items = new List<T>();
        #endregion

        #region Events
        /// <summary>
        /// Ocurre cuando los datos de esta instancia han cambiado.
        /// </summary>
        public event EventHandler<EventInfo> DataChanged;


        /// <summary>
        /// Preprocesa los datos antes de ser cambiados.
        /// </summary>
        public event EventHandler<EventInfo> PreDataChanged;
        #endregion

        #region EventType Enum
        /// <summary>
        /// Representa los tipos de evetnos soportados.
        /// </summary>
        public enum EventType
        {
            /// <summary>
            /// Elemento borrado.
            /// </summary>
            Deleted,
            /// <summary>
            /// Elemento agregado.
            /// </summary>
            Added
        };
        #endregion

        #region EventInfo Class
        /// <summary>
        /// Contiene los datos para los eventos de agregado o borrado de elementos.
        /// </summary>
        public class EventInfo : EventArgs
        {
            #region Constructors
            /// <summary>
            /// Inicializa una nueva instancia de la clase <see cref="EventInfo"/>.
            /// </summary>
            /// <param name="eventType">Tipo de evento.</param>
            /// <param name="item">Elemento que provoc� el evento.</param>
            public EventInfo(EventType eventType, T item)
            {
                EventType = eventType;
                Items = new T[] { item };
            }

            /// <summary>
            /// Inicializa una nueva instancia de la clase <see cref="EventInfo"/>.
            /// </summary>
            /// <param name="eventType">Tipo de evento.</param>
            /// <param name="item">Elementos que provocar�n el evento.</param>
            public EventInfo(EventType eventType, T[] items)
            {
                EventType = eventType;
                Items = items;
            }
            #endregion

            #region Fields
            /// <summary>
            /// Tipo de evento.
            /// </summary>
            public readonly EventType EventType;
            /// <summary>
            /// Elementos afectados.
            /// </summary>
            public readonly T[] Items;
            #endregion
        }
        #endregion
    }
}
