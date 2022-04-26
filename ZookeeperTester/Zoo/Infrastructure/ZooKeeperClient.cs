using Microsoft.Extensions.Options;
using org.apache.zookeeper;
using System.Diagnostics;
using ZookeeperTester.Zoo.Application;

namespace ZookeeperTester.Zoo.Infrastructure
{
    internal class ZooKeeperClient : IZooKeeperClient
    {
        private class NullWatcher : Watcher
        {
            public static readonly NullWatcher Instance = new();

            private NullWatcher() { }

            public override Task process(WatchedEvent @event) => Task.CompletedTask;
        }

        private readonly ZooKeeperConfiguration configuration;

        public bool Connected { get; private set; }
        public ZooKeeper Instance { get; private set; }

        public ZooKeeperClient(IOptions<ZooKeeperConfiguration> configuration) => this.configuration = configuration.Value;

        public Task Connect(CancellationToken cancellationToken = default)
        {
            Connected = false;

            var count = 1;
            Stopwatch sw = new();
            sw.Start();

            var zooKeeper = new ZooKeeper(configuration.ConnectionString, configuration.ConnectionTimeOut, NullWatcher.Instance);
            while (!Connected && sw.ElapsedMilliseconds < configuration.ConnectionTimeOut)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var state = zooKeeper.getState();
                Connected = state == ZooKeeper.States.CONNECTED;

                count++;
            }

            if (!Connected)
                throw new TimeoutException($"After {count} attempts and {sw.Elapsed} time passed client could not be connected.");

            Instance = zooKeeper;

            return Task.CompletedTask;
        }
    }
}
