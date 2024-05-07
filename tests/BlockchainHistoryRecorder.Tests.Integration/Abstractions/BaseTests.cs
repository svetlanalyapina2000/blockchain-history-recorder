using AutoFixture;
using Cassandra;
using Cassandra.Mapping;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Extensions.AssemblyFixture;

namespace BlockchainHistoryRecorder.Tests.Integration.Abstractions;

public abstract class BaseTests : IAssemblyFixture<TestWebApplicationFactory>, IDisposable
{
    public BaseTests(TestWebApplicationFactory factory)
    {
        Fixture = CreateFixture();
        Scope = factory.Services.CreateScope();
        Session = Scope.ServiceProvider.GetRequiredService<ISession>();
        CassandraMapper = new Mapper(Session);
        HttpClient = factory.CreateClient();
    }

    protected IServiceScope Scope { get; }
    protected ISession Session { get; }
    protected IMapper CassandraMapper { get; }
    protected HttpClient HttpClient { get; }
    protected Fixture Fixture { get; }

    public void Dispose()
    {
        Cleanup();
    }

    private Fixture CreateFixture()
    {
        var fixture = new Fixture();
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        return fixture;
    }

    protected virtual void Cleanup()
    {
    }
}