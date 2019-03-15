using System.Collections.Generic;

namespace Qoden.Util
{
    /// <summary>
    /// Provides methods to merge updates into <see cref="IncrementalStore{T}"/>. Strategy also implements
    /// <see cref="IComparer{T}"/> to compare updates to each other. Only updates "greater" than store value applied
    /// to the store.  
    /// </summary>
    /// <typeparam name="T">Type of value stored in <see cref="IncrementalStore{T}"/></typeparam>
    public interface IIncrementalStoreUpdateStrategy<T> : IComparer<T>
    {
        /// <summary>
        /// Merges two store values together. <paramref name="storeValue"/> 
        /// </summary>
        /// <param name="storeValue">Value saved in the store.</param>
        /// <param name="update">Update to the store value.</param>
        /// <returns></returns>
        T Add(T storeValue, T update);

        /// <summary>
        /// Identity or zero element of the store. Empty store starts with this element as a value.
        /// </summary>
        T Identity { get; }
    }
}