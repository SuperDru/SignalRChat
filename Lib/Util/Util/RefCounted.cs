using System;
using System.Diagnostics;
using System.Threading;

namespace Qoden.Util
{
    /// <summary>
    /// Base class for reference counted objects. Object starts with ref count equals to 1.
    /// </summary>
    public abstract class RefCounted : IDisposable
    {
        private int _refCount = 1;

        /// <summary>
        /// Increment ref counter.
        /// </summary>
        public void AddRef()
        {
            Debug.Assert(_refCount > 0);
            Interlocked.Increment(ref _refCount);
        }

        /// <summary>
        /// Decrement ref counter and dispose object when counter reaches zero.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.Decrement(ref _refCount) == 0)
            {
                Dispose(true);
            }
        }

        protected abstract void Dispose(bool disposing);
    }
}