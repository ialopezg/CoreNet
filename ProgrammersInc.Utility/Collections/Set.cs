using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace ProgrammersInc.Utility.Collections
{
    /// <summary>
    /// Expone un enumerado, que admite una iteración simple de un tipo dado.
    /// </summary>
    /// <typeparam name="T">Tipo de datos a procesarse.</typeparam>
    [DebuggerDisplay("Count={values.Count}")]
    public sealed class Set<T> : IEnumerable<T>
    {
        #region Constructors
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Set"/>.
        /// </summary>
        public Set()
        {
            values = new Dictionary<T, int>();
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Set"/>, con soporte de comparación
        /// entre los elementos.
        /// </summary>
        /// <param name="comparer">Objeto que comparará los elementos.</param>
        public Set(IEqualityComparer<T> comparer)
        {
            values = new Dictionary<T, int>(comparer);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="Set"/>, estableciendo una
        /// colecci[on de elementos como elementos iniciales de esta colección.
        /// </summary>
        /// <param name="ts">Elementos iniciales de la colección.</param>
        public Set(IEnumerable<T> ts)
            : this()
        {
            foreach (T t in ts)
                if (!Contains(t))
                    Add(t);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Agrega un elemento al final de esta colección.
        /// </summary>
        /// <param name="t">Elemento a agregarse.</param>
        public void Add(T t)
        {
            values.Add(t, 0);
        }

        /// <summary>
        /// Agrega una colección de elementos al final de esta colección.
        /// </summary>
        /// <param name="ts">Elementos a agregarse.</param>
        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
                Add(item);
        }

        /// <summary>
        /// Quita todos los elementos de esta colección.
        /// </summary>
        public void Clear()
        {
            values.Clear();
        }

        /// <summary>
        /// Determina si esta colección contiene el elemento dado.
        /// </summary>
        /// <param name="t">Elemento a evaluarse.</param>
        /// <returns><c>true</c> si el elemento es encontrado, en otro caso <c>false</c>.</returns>
        public bool Contains(T item)
        {
            return values.ContainsKey(item);
        }

        /// <summary>
        /// Determina las diferencias entre dos colecciones de elementos.
        /// </summary>
        /// <param name="first">Primer colección.</param>
        /// <param name="second">Segunda colección.</param>
        /// <param name="onlyInFirst">Diferencias de la primera únicamente.</param>
        /// <param name="inBoth">Diferencia en ambos.</param>
        /// <param name="onlyInSecond">Diferencias de la segunda únicamente.</param>
        public static void Differences(Set<T> first, Set<T> second, out T[] onlyInFirst, out T[] inBoth, out T[] onlyInSecond)
        {
            if (first == null)
                throw new ArgumentNullException("first");
            if (second == null)
                throw new ArgumentNullException("second");

            List<T> listOnlyInFirst = new List<T>();
            List<T> listInBoth = new List<T>();
            List<T> listOnlyInSecond = new List<T>();

            foreach (T t in first)
            {
                if (second.Contains(t))
                    listInBoth.Add(t);
                else
                    listOnlyInFirst.Add(t);
            }
            foreach (T t in second)
                if (!first.Contains(t))
                    listOnlyInSecond.Add(t);

            onlyInFirst = listOnlyInFirst.ToArray();
            inBoth = listInBoth.ToArray();
            onlyInSecond = listOnlyInSecond.ToArray();
        }

        /// <summary>
        /// Obtiene la enumeración de los elementos de esta colección.
        /// </summary>
        /// <returns>La enumeración de los elementos de esta colección.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (KeyValuePair<T, int> kvp in values)
                yield return kvp.Key;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Insercta los elementos dados con las colecciones de elementos dados.
        /// </summary>
        /// <param name="sets">Elementos a intersectar.</param>
        /// <returns>Los elementos dados con las colecciones de elementos dados.</returns>
        public static Set<T> Intersect(params Set<T>[] sets)
        {
            if (sets == null)
                throw new ArgumentNullException("sets");
            if (sets.Length == 0)
                return new Set<T>();

            if (sets[0] == null)
                throw new ArgumentNullException("sets[0]");

            Set<T> counted = sets[0].ShallowCopy();

            for (int i = 1; i < sets.Length; ++i)
            {
                Set<T> set = sets[i];

                if (set == null)
                    throw new ArgumentNullException(string.Format("sets[{0}]", i));

                foreach (T t in set)
                {
                    int count;

                    if (counted.values.TryGetValue(t, out count))
                        counted.values[t] = count + 1;
                }
            }

            Set<T> intersection = new Set<T>();
            int c = sets.Length - 1;

            foreach (KeyValuePair<T, int> kvp in counted.values)
            {
                if (kvp.Value == c)
                    intersection.Add(kvp.Key);
            }

            return intersection;
        }

        /// <summary>
        /// Quita el elemento dado de esta colección.
        /// </summary>
        /// <param name="t">Elemento a ser quitado.</param>
        public void Remove(T t)
        {
            values.Remove(t);
        }

        /// <summary>
        /// Crea una copia de los elementos de esta coleción.
        /// </summary>
        /// <returns>Una copia de los elementos de esta coleción.</returns>
        public Set<T> ShallowCopy()
        {
            Set<T> copy = new Set<T>();

            foreach (T t in this)
                copy.Add(t);

            return copy;
        }

        /// <summary>
        /// Devuelve un array conteniendo los elementos de esta colección.
        /// </summary>
        /// <returns>Un array conteniendo los elementos de esta colección.</returns>
        public T[] ToArray()
        {
            T[] ts = new T[Count];
            int pos = 0;

            foreach (T t in this)
            {
                ts[pos] = t;
                ++pos;
            }

            return ts;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Obtiene el número de elementos de esta colección.
        /// </summary>
        public int Count
        {
            get { return values.Count; }
        }
        #endregion

        #region Fields
        private Dictionary<T, int> values;
        #endregion
    }
}
