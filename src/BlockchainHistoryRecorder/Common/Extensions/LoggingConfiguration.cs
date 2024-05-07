using System.Reflection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;

namespace BlockchainHistoryRecorder.Common.Extensions;

public static class LoggingConfiguration
{
    public static void ConfigureLogging(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, lc) => lc
            .Enrich.WithProperty("SystemName", Assembly.GetExecutingAssembly().GetName().Name)
#if !DEBUG
	        .WriteTo.Console(new ElasticsearchJsonFormatter())
#else
            .WriteTo.Console()
#endif
            .MinimumLevel.Is(builder.Environment.IsProduction() ? LogEventLevel.Information : LogEventLevel.Debug)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Information)
            .ReadFrom.Configuration(ctx.Configuration)
        );
    }
}