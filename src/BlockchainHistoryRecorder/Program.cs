using System.Text.Json.Serialization;
using BlockchainHistoryRecorder;
using BlockchainHistoryRecorder.Common.Exceptions;
using BlockchainHistoryRecorder.Common.Extensions;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Abstractions;
using BlockchainHistoryRecorder.Features.Blockchains.Application.Validaton;
using BlockchainHistoryRecorder.Features.Blockchains.Infrastructure.Persistence;
using BlockchainHistoryRecorder.Infrastructure.BlockcypherHttpClient.Extension;
using BlockchainHistoryRecorder.Infrastructure.Persistence.Extension;
using BlockchainHistoryRecorder.Infrastructure.Persistence.Mappings;
using Cassandra.Mapping;
using FluentValidation;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder);

var app = builder.Build();

Configure(app);

app.Run();

void ConfigureServices(WebApplicationBuilder webApplicationBuilder)
{
    // Configure Host
    var appHost = builder.Configuration.GetValue<string>("APP_HOST");
    var appPort = builder.Configuration.GetValue<int>("APP_PORT");
    builder.WebHost.UseUrls($"http://{appHost}:{appPort}");
    
    // Configure Logging
    LoggingConfiguration.ConfigureLogging(webApplicationBuilder);

    // Database and HTTP client configurations
    webApplicationBuilder.Services.AddCassandra(webApplicationBuilder.Configuration);
    webApplicationBuilder.Services.AddBlockcypherHttpClient();

    // Domain services
    webApplicationBuilder.Services.AddHandlers();
    webApplicationBuilder.Services.AddBehaviors();

    // AutoMapper and Validators
    MappingConfiguration.Global.Define<DatabaseMapping>();
    webApplicationBuilder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    webApplicationBuilder.Services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

    // Repository and UnitOfWork
    webApplicationBuilder.Services.AddScoped<IBlockchainRepository, BlockchainRepository>();
    webApplicationBuilder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

    // Exception handling
    webApplicationBuilder.Services.AddExceptionHandler<ExceptionHandler.GlobalExceptionsHandler>();

    // MediatR and controllers
    webApplicationBuilder.Services.AddMediatR(config =>
    {
        config.RegisterServicesFromAssemblyContaining<Program>();
        config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    });

    webApplicationBuilder.Services.AddControllers()
        .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    // Swagger
    webApplicationBuilder.Services.AddSwagger();

    // CORS
    webApplicationBuilder.Services.ConfigureCORS(webApplicationBuilder.Environment,
        webApplicationBuilder.Configuration);

    // Configure routing
    webApplicationBuilder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

    // Health Checks
    webApplicationBuilder.Services.AddHealthChecks()
        .AddCheck("liveness_probe", () => HealthCheckResult.Healthy("OK"));
}

void Configure(WebApplication webApplication)
{
    app.UseDeveloperExceptionPage();

    // Apply CORS and Routing
    webApplication.UseRouting();
    webApplication.UseCors("_allowedOrigins");

    // Swagger 
    webApplication.UseSwagger();
    webApplication.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blockchain History Recorder V1");
        c.RoutePrefix = "api/docs";
    });

    // Exception handler middleware
    webApplication.UseGlobalExceptionHandler();

    // Map controllers
    webApplication.MapControllers();

    // Health Checks
    app.MapHealthChecks("/healthz");
}

public partial class Program
{
}