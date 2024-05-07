namespace BlockchainHistoryRecorder.Common.Extensions;

public static class CORSConfiguration
{
    public static void ConfigureCORS(this IServiceCollection services, IHostEnvironment env, IConfiguration config)
    {
        const string allowedOrigins = "_allowedOrigins";
        const string localhost = "localhost";
        var frontendOrigins = config["ALLOWED_ORIGINS"]?.Split(',');

        services.AddCors(options =>
        {
            options.AddPolicy(allowedOrigins,
                policy =>
                {
                    if (env.IsDevelopment())
                    {
                        policy.SetIsOriginAllowed(origin => new Uri(origin).Host == localhost)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                    else
                    {
                        if (frontendOrigins != null)
                            policy.WithOrigins(frontendOrigins)
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                    }
                });
        });
    }
}