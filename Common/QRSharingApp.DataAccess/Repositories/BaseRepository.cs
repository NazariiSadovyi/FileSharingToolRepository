using System;
using System.Threading;
using System.Threading.Tasks;

namespace QRSharingApp.DataAccess.Repositories
{
    public abstract class BaseRepository
    {
        private static readonly object _locker = new object();
        protected ApplicationDBContext Context { get; private set; }
        public static bool IsThreadSafeAnabled = true;

        public BaseRepository(ApplicationDBContext context)
        {
            Context = context;
        }

        protected void ThreadSafeExecute(Action action)
        {
            if (IsThreadSafeAnabled)
            {
                Monitor.Enter(_locker);
            }

            action();

            if (IsThreadSafeAnabled)
            {
                Monitor.Exit(_locker);
            }
        }

        protected async Task ThreadSafeTaskExecute(Func<Task> action)
        {
            if (IsThreadSafeAnabled)
            {
                Monitor.Enter(_locker);
            }

            await action();

            if (IsThreadSafeAnabled)
            {
                Monitor.Exit(_locker);
            }
        }

        protected T ThreadSafeExecute<T>(Func<T> action)
        {
            if (IsThreadSafeAnabled)
            {
                Monitor.Enter(_locker);
            }

            var result = action();

            if (IsThreadSafeAnabled)
            {
                Monitor.Exit(_locker);
            }

            return result;
        }

        protected async Task<T> ThreadSafeTaskExecute<T>(Func<Task<T>> action)
        {
            if (IsThreadSafeAnabled)
            {
                Monitor.Enter(_locker);
            }

            var result = await action();

            if (IsThreadSafeAnabled)
            {
                Monitor.Exit(_locker);
            }

            return result;
        }
    }
}
