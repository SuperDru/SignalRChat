using System;
using System.Linq;
using System.Threading;

namespace Qoden.Util
{
    public static class Arrays
    {
        public static void InterlockedAdd<T>(ref T[] items, T item)
        {
            T[] to;
            T[] from;
            do
            {
                from = Volatile.Read(ref items);
                to = new T[from.Length + 1];
                Array.Copy(from, to, from.Length);
                to[from.Length] = item;
            } while (Interlocked.CompareExchange(ref items, to, from) != from);
        }

        public static bool InterlockedRemove<T>(ref T[] items, T item) where T : class
        {
            T[] from;
            T[] to;
            do
            {
                from = Volatile.Read(ref items);

                var count = from.Count(value => ReferenceEquals(value, item)); // Specifically uses identity
                if (count == 0)
                    return false;

                var oldSize = from.Length;
                to = new T[oldSize - count];

                for (int i = 0, pos = 0; i < oldSize; i++)
                {
                    var testSequence = from[i];
                    if (!ReferenceEquals(item, testSequence))
                    {
                        to[pos++] = testSequence;
                    }
                }
            } while (Interlocked.CompareExchange(ref items, to, from) != from);

            return true;
        }
    }
}