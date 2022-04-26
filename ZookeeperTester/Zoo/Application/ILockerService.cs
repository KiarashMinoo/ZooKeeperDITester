namespace ZookeeperTester.Zoo.Application
{
    public interface ILockerService : IDisposable
    {
        Task<bool> LockAsync(string key, CancellationToken cancellationToken = default);
        Task UnlockAsync();
    }
}