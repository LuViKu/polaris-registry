using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OphthalmicRegistry.Infrastructure.Persistence;
using StackExchange.Redis;

namespace OphthalmicRegistry.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Entity Framework / PostgreSQL
        services.AddDbContext<RegistryDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly(typeof(RegistryDbContext).Assembly.FullName)));

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(
                configuration["Redis:ConnectionString"] ?? "localhost:6379"));

        // RabbitMQ via MassTransit
        services.AddMassTransit(bus =>
        {
            bus.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["RabbitMQ:Host"] ?? "localhost", "/", h =>
                {
                    h.Username(configuration["RabbitMQ:User"] ?? "guest");
                    h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                });
                cfg.ConfigureEndpoints(ctx);
            });
        });

        // Hangfire — use dedicated Hangfire database if configured, fall back to default
        services.AddHangfire(config =>
            config.UsePostgreSqlStorage(c =>
                c.UseNpgsqlConnection(
                    configuration.GetConnectionString("HangfireConnection")
                    ?? configuration.GetConnectionString("DefaultConnection"))));
        services.AddHangfireServer();

        return services;
    }
}
