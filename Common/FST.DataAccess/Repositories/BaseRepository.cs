using System;
using System.Threading;
using System.Threading.Tasks;

namespace FST.DataAccess.Repositories
{
    public abstract class BaseRepository
    {
        private static readonly object _locker = new object();
        protected ApplicationDBContext Context { get; private set; }

        public BaseRepository(ApplicationDBContext context)
        {
            Context = context;
        }

        protected void ThreadSafeExecute(Action action)
        {
            Monitor.Enter(_locker);
            action();
            Monitor.Exit(_locker);
        }

        protected async Task ThreadSafeTaskExecute(Func<Task> action)
        {
            Monitor.Enter(_locker);
            await action();
            Monitor.Exit(_locker);
        }

        protected T ThreadSafeExecute<T>(Func<T> action)
        {
            Monitor.Enter(_locker);
            var result = action();
            Monitor.Exit(_locker);
            return result;
        }

        protected async Task<T> ThreadSafeTaskExecute<T>(Func<Task<T>> action)
        {
            Monitor.Enter(_locker);
            var result = await action();
            Monitor.Exit(_locker);
            return result;
        }
    }
}
