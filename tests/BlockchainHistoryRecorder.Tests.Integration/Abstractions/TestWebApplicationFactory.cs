using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace BlockchainHistoryRecorder.Tests.Integration.Abstractions;

public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly string _rootPath = FindRootDirectory();

    private static readonly INetwork _network = new NetworkBuilder().WithCleanUp(true)
        .WithName("blockchain_network_integraion").Build();

    private readonly IContainer _cassandraContainer = new ContainerBuilder()
        .WithCleanUp(true)
        .WithImage("cassandra:latest")
        .WithName("cassandra_test_integration")
        .WithPortBinding(9043, 9042)
        .WithNetwork(_network)
        .WithBindMount($"{_rootPath}/migrations", "/migrations")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(9042))
        .Build();

    private readonly IContainer _migrationContainer = new ContainerBuilder()
        .WithCleanUp(true)
        .WithImage("cassandra:latest")
        .WithName("migration_test_integration")
        .DependsOn(_network)
        .WithEnvironment("MIGRATION_DIR", "/migrations")
        .WithEnvironment("CASSANDRA_HOST", "cassandra_test_integration")
        .WithEnvironment("CASSANDRA_PORT", "9042")
        .WithBindMount($"{_rootPath}/migrations", "/migrations")
        .WithBindMount($"{_rootPath}/migration_script.sh", "/migration_script.sh")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("bash ./migration_script.sh"))
        .Build();

    private IConfiguration? _config;

    protected IConfiguration Config => _config ??= InitConfiguration();


    public async Task InitializeAsync()
    {
        await _cassandraContainer.StartAsync();
        await _migrationContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _cassandraContainer.DisposeAsync();
        await _migrationContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseConfiguration(Config);
    }

    private static IConfiguration InitConfiguration()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
            .AddEnvironmentVariables()
            .Build();
        return config;
    }

    private static string FindRootDirectory()
    {
        var currentDirectory = new DirectoryInfo(AppContext.BaseDirectory);
        while (currentDirectory != null && !currentDirectory.GetFiles("*.sln").Any())
            currentDirectory = currentDirectory.Parent;
        return currentDirectory?.FullName ?? string.Empty;
    }
}