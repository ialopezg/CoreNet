using System;
using System.Collections.Generic;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Representa una colección de elementos <see cref="SummaryBoxItem"/>.
    /// </summary>
    public class SummaryBoxItemCollection : List<SummaryBoxItem>
    {
        #region Constructors
        /// <summary>
        /// Crea una nueva intancia de la clase <see cref="SummaryBoxItemCollection"/>.
        /// </summary>
        /// <param name="owner">Propietario de esta colección de elementos.</param>
        public SummaryBoxItemCollection(SummaryBox owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            this.owner = owner;
        }
        #endregion

        #region Properties
        SummaryBox owner;
        /// <summary>
        /// Obtiene el propietario de esta colección de elementos.
        /// </summary>
        public SummaryBox Owner
        {
            get { return owner; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Agrega un elemento al final de esta colección.
        /// </summary>
        /// <param name="item">Elemento a agregarse.</param>
        public new void Add(SummaryBoxItem item)
        {
            if (!(base.Contains(item)))
                base.Add(item);

            owner.CalculateItems();
            owner.Invalidate();
        }

        /// <summary>
        /// Agrega una colección de elementos al final de esta colección.
        /// </summary>
        /// <param name="items">Elementos a agregarse.</param>
        public void AddRange(SummaryBoxItem[] items)
        {
            foreach (SummaryBoxItem item in items)
                Add(item);
        }

        /// <summary>
        /// Borra la primera coincidencia con el elemento dado de esta colección.
        /// </summary>
        /// <param name="item">Elemento a borrarse.</param>
        /// <returns><c>true</c> si el elemento fue removido, en otro caso <c>false</c>.</returns>
        public new bool Remove(SummaryBoxItem item)
        {
            bool result = base.Remove(item);
            owner.CalculateItems();

            return result;
        }

        /// <summary>
        /// Borra un elemento ubicado en la posición dada de esta colección.
        /// </summary>
        /// <param name="index">Posición del elemento.</param>
        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);

            owner.CalculateItems();
        }

        /// <summary>
        /// Devuelve la cantidad de elementos presentes en esta colección.
        /// </summary>
        /// <returns>La cantidad de elementos presentes en esta colección.</returns>
        public override string ToString()
        {
            return base.Count.ToString();
        }
        #endregion
    }
}