using org.apache.zookeeper.recipes.@lock;
using ZookeeperTester.Zoo.Application;

namespace ZookeeperTester.Zoo
{
    internal class LockerService : ILockerService
    {
        private record LockerCallback(Func<Task> LockAcquiredAction, Func<Task> LockReleasedAction) : LockListener
        {
            public Task lockAcquired() => LockAcquiredAction.Invoke();

            public Task lockReleased() => LockReleasedAction.Invoke();
        }

        private readonly IZooKeeperClient zooKeeper;

        private WriteLock? writeLock;

        public async Task UnlockAsync()
        {
            if (writeLock is not null)
                await writeLock.unlock();
        }

        public LockerService(IZooKeeperClient zooKeeper) => this.zooKeeper = zooKeeper;

        public async Task<bool> LockAsync(string key, CancellationToken cancellationToken = default)
        {
            writeLock = new(zooKeeper.Instance, $"/{key}", null);

            var _lock = new ManualResetEvent(false);

            writeLock.setLockListener(new LockerCallback(() =>
            {
                _lock.Set();
                return Task.CompletedTask;
            }, () => Task.CompletedTask));

            await writeLock.Lock();

            _lock.WaitOne();

            return true;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            UnlockAsync().Wait();
        }
    }
}
