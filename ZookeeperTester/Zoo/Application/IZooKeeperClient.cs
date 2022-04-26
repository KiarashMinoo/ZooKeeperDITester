using org.apache.zookeeper;

namespace ZookeeperTester.Zoo.Application
{
    public interface IZooKeeperClient
    {
        bool Connected { get; }
        ZooKeeper Instance { get; }

        Task Connect(CancellationToken cancellationToken = default);
    }
}
