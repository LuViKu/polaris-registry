using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OphthalmicRegistry.Application.Common.Interfaces;
using OphthalmicRegistry.Domain.Repositories;
using OphthalmicRegistry.Infrastructure.Persistence;
using OphthalmicRegistry.Infrastructure.Persistence.Repositories;
using OphthalmicRegistry.Infrastructure.Services;
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

        // Repositories
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IClinicalVisitRepository, ClinicalVisitRepository>();
        services.AddScoped<IImagingStudyRepository, ImagingStudyRepository>();

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

        // MinIO (S3-compatible) storage
        var minioEndpoint = configuration["Minio:Endpoint"] ?? "http://localhost:9000";
        var minioAccess = configuration["Minio:AccessKey"] ?? "minioadmin";
        var minioSecret = configuration["Minio:SecretKey"] ?? "change_me_in_production";
        services.AddSingleton<IAmazonS3>(_ =>
        {
            var config = new AmazonS3Config
            {
                ServiceURL = minioEndpoint,
                ForcePathStyle = true,
            };
            return new AmazonS3Client(new BasicAWSCredentials(minioAccess, minioSecret), config);
        });
        services.AddScoped<IStorageService, MinioStorageService>();

        // Orthanc DICOM service
        var orthancUrl = configuration["Orthanc:Url"] ?? "http://localhost:8042";
        var orthancUser = configuration["Orthanc:Username"] ?? "orthanc";
        var orthancPass = configuration["Orthanc:Password"] ?? "orthanc";
        services.AddHttpClient<IOrthancService, OrthancService>(client =>
        {
            client.BaseAddress = new Uri(orthancUrl.TrimEnd('/') + "/");
            var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{orthancUser}:{orthancPass}"));
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
        });

        // OCT-Converter microservice
        var octConverterUrl = configuration["OctConverter:Url"] ?? "http://oct-converter:8080/";
        services.AddHttpClient<IOctConverterService, OctConverterService>(client =>
        {
            client.BaseAddress = new Uri(octConverterUrl.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        return services;
    }
}

