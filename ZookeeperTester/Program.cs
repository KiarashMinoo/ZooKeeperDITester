using ZookeeperTester.Zoo;
using ZookeeperTester.Zoo.Application;
using ZookeeperTester.Zoo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.
    Configure<ZooKeeperConfiguration>(builder.Configuration.GetSection("Zoo")).
    AddSingleton<IZooKeeperClient, ZooKeeperClient>().
    AddScoped<ILockerService, LockerService>();

var app = builder.Build();

var zooKeeperClient = app.Services.GetRequiredService<IZooKeeperClient>();
await zooKeeperClient.Connect();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
var tasks = Enumerable.Range(0, 10).Select(a =>
{
    return Task.Run(async () =>
    {
        using var scope = scopeFactory.CreateScope();
        using var locker = scope.ServiceProvider.GetRequiredService<ILockerService>();

        Console.WriteLine($"getting lock: {DateTime.Now}, thread Id: {Environment.CurrentManagedThreadId}");
        await locker.LockAsync("locks");
        Console.WriteLine($"get the lock successfully: {DateTime.Now}, thread Id: {Environment.CurrentManagedThreadId}");

        Thread.Sleep(1000);

        await locker.UnlockAsync();
    });
}).ToArray();


await Task.WhenAll(tasks);
