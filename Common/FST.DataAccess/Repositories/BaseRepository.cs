using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FST.DataAccess.Repositories
{
    public abstract class BaseRepository
    {
        private static readonly Dispatcher _dbDispatcher;

        protected ApplicationDBContext Context { get; private set; }

        static BaseRepository()
        {
            Dispatcher dispatcher = null;
            var dispatcherReadyEvent = new ManualResetEvent(false);
            new Thread(new ThreadStart(() =>
            {
                dispatcher = Dispatcher.CurrentDispatcher;
                dispatcherReadyEvent.Set();
                Dispatcher.Run();
            })).Start();

            dispatcherReadyEvent.WaitOne();
            _dbDispatcher = dispatcher;
        }

        public BaseRepository(ApplicationDBContext context)
        {
            Context = context;
        }

        protected void ThreadSafeExecute(Action action)
        {
            _dbDispatcher.Invoke(() => action());
        }

        protected async Task ThreadSafeTaskExecute(Func<Task> action)
        {
            await _dbDispatcher.Invoke(async () => await action());
        }

        protected T ThreadSafeExecute<T>(Func<T> action)
        {
            return _dbDispatcher.Invoke(() => action());
        }

        protected async Task<T> ThreadSafeTaskExecute<T>(Func<Task<T>> action)
        {
            return await _dbDispatcher.Invoke(async () => await action());
        }
    }
}
