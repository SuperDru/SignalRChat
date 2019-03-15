using System;
using System.Collections.Generic;

namespace Qoden.Util
{
    /// <summary>
    /// Encapsulates typical steps required to load some object from server and keep it in sync:
    /// <list type="number">
    /// <item>
    ///     <description>Connect to updates stream and collect them into temporary buffer.</description>
    /// </item>
    /// <item>
    ///     <description>Load snapshot and apply collected updates to it.</description>
    /// </item>
    /// <item>
    ///     <description>Keep listening to updates and apply them as needed.</description>
    /// </item>
    /// </list>
    /// </summary>
    ///
    /// <remarks>
    /// Instance of <see cref="IIncrementalStoreUpdateStrategy{T}"/> provides methods to find out if update needs to be applied and to apply updates.
    /// </remarks>
    /// 
    /// <typeparam name="T">Type of data structure which is being incrementally updated</typeparam>
    public class IncrementalStore<T>
    {
        private readonly IIncrementalStoreUpdateStrategy<T> _updateStrategy;
        private bool _isReady;
        private readonly List<T> _updates = new List<T>();

        /// <summary>
        /// Create an empty incremental store.
        /// </summary>
        /// <param name="updateStrategy">Update strategy instance to compare and merge updates</param>
        public IncrementalStore(IIncrementalStoreUpdateStrategy<T> updateStrategy)
        {
            _updateStrategy = updateStrategy ?? throw new ArgumentNullException(nameof(updateStrategy));
            Value = updateStrategy.Identity;
            _isReady = false;
        }

        /// <summary>
        /// Incremental store value.
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// True if incremental store has value.
        /// </summary>
        public bool IsReady => _isReady;

        /// <summary>
        /// Reset store to initial state.
        /// </summary>
        public void Reset()
        {
            Value = _updateStrategy.Identity;
            _isReady = false;
        }

        /// <summary>
        /// Send snapshot to the store, apply stored increments on top of it and transition to ready state.
        /// </summary>
        /// <param name="snapshot">Snapshot value</param>
        public void OnSnapshot(T snapshot)
        {
            Value = snapshot;
            foreach (var update in _updates)
            {
                if (_updateStrategy.Compare(Value, update) < 0)
                {
                    Value = _updateStrategy.Add(Value, update);
                }
            }
            
            _updates.Clear();
            _isReady = true;
        }

        /// <summary>
        /// Send increment to the store, either apply it right away or store until <see cref="OnSnapshot"/> is called.
        /// </summary>
        /// <param name="increment">Increment value</param>
        public void OnIncrement(T increment)
        {
            if (!_isReady)
            {
                _updates.Add(increment);
            }
            else if (_updateStrategy.Compare(Value, increment) < 0)
            {
                Value = _updateStrategy.Add(Value, increment);
            }
        }
    }
}