using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Qoden.Util
{
    /// <summary>
    /// <see cref="T:Qoden.Util.SingletonOperation`1"/> is async operation which can
    /// be run one at a time, like login operation.
    /// </summary>
    /// <remarks>
    /// <see cref="T:Qoden.Util.SingletonOperation`1"/> can be started from multiple threads.
    /// Only first caller starts real operation while others get already started operation as a result. 
    /// 
    /// Once operation finished it can be repeated.
    /// </remarks>
    public class SingletonOperation<T>
    {
        private Func<Task<T>> operation;
        private TaskCompletionSource<T> taskSource = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Qoden.Auth.SingletonOperation`1"/> class.
        /// </summary>
        /// <param name="operation">Operation to perform</param>
        public SingletonOperation(Func<Task<T>> operation)
        {
            this.operation = operation ?? throw new ArgumentNullException(nameof(operation));
        }

        /// <summary>
        /// Start single operation
        /// </summary>
        /// <returns>Async operation result</returns>
        public async Task<T> Start()
        {
            var t = Volatile.Read(ref taskSource);
            if (t != null)
            {
                return await t.Task;
            }

            var value = Interlocked.CompareExchange(ref taskSource, new TaskCompletionSource<T>(), null);

            if (value == null)
            {
                T result = default(T);
                try
                {
                    result = await operation();
                    taskSource.SetResult(result);
                }
                catch (OperationCanceledException e)
                {
                    taskSource.SetCanceled();
                }
                catch (Exception e)
                {
                    taskSource.SetException(e);
                }
                finally
                {
                    Volatile.Write(ref taskSource, null);
                }
                return result;
            }
            else
            {
                return await taskSource.Task;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Qoden.Auth.SingletonOperation`1"/> is started.
        /// </summary>
        /// <value><c>true</c> if started; otherwise, <c>false</c>.</value>
        public bool Started => taskSource?.Task != null;

        /// <summary>
        /// Gets running task.
        /// </summary>
        public Task<T> Task => taskSource?.Task;
    }
}
