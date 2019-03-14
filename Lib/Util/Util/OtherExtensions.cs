using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Qoden.Util
{
    /// <summary>
    /// Contains various extension methods.
    /// </summary>
    public static class OtherExtensions
    {
        private static ILogger Logger { get; } = Log.ForContext(typeof(OtherExtensions));

        /// <summary>
        /// Represent <see cref="CancellationToken"/> as <see cref="Task"/>.
        /// </summary>
        public static async Task AsTask(this CancellationToken token)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (token.Register(() => tcs.TrySetCanceled(token), useSynchronizationContext: false))
                await tcs.Task;
        }

        /// <summary>
        /// Safely forgot task.
        /// </summary>
        public static void ForgotTask(this Task task, ILogger logger = null)
        {
            if (logger == null)
            {
                logger = Logger;
            }

            task.ContinueWith(x =>
            {
                if (x.Exception != null)
                    logger.Error(x.Exception, "Some error occurred in the forgotten task.");
            }).ConfigureAwait(false);
        }
    }
}