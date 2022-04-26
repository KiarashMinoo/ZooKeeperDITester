namespace ZookeeperTester.Zoo.Application
{
    public class ZooKeeperConfiguration
    {
        public int ConnectionTimeOut { get; set; } = 50000;
        public string ConnectionString { get; set; } = "127.0.0.1:2181";
    }
}
