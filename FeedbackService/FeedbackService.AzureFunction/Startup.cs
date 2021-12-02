using AutoMapper;
using FeedbackService.Core.Config;
using FeedbackService.Core.Interfaces.Repositories;
using FeedbackService.Core.Interfaces.Services;
using FeedbackService.Handlers;
using FeedbackService.Repo;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Utils;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

[assembly: FunctionsStartup(typeof(FeedbackService.AzureFunction.Startup))]
namespace FeedbackService.AzureFunction
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            ExecutionContextOptions executioncontextoptions = builder.Services.BuildServiceProvider()
           .GetService<IOptions<ExecutionContextOptions>>().Value;
            string currentDirectory = executioncontextoptions.AppDirectory;

            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
            .SetBasePath(currentDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

            IConfigurationRoot config = configBuilder.Build();

            Dictionary<HttpClientConfigName, ApiConfig> httpClientConfigs = config.GetSection("Apis").Get<Dictionary<HttpClientConfigName, ApiConfig>>();

            foreach (KeyValuePair<HttpClientConfigName, ApiConfig> httpClientConfig in httpClientConfigs)
            {

                builder.Services.AddHttpClient(httpClientConfig.Key.ToString(), c =>
                {
                    c.BaseAddress = new Uri(httpClientConfig.Value.BaseAddress);

                    c.Timeout = httpClientConfig.Value.Timeout ?? new TimeSpan(0, 0, 0, 15);

                    foreach (KeyValuePair<string, string> header in httpClientConfig.Value.Headers)
                    {
                        c.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                    c.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    c.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));

                }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    MaxConnectionsPerServer = httpClientConfig.Value.MaxConnectionsPerServer ?? 15,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                });

            }

            builder.Services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            builder.Services.TryAdd(ServiceDescriptor.Singleton(typeof(ILoggerWrapper<>), typeof(LoggerWrapper<>)));

            IConfigurationSection connectionStringSettings = config.GetSection("ConnectionStrings");
            builder.Services.Configure<ConnectionStrings>(connectionStringSettings);

            ConnectionStrings connectionStrings = new ConnectionStrings();
            connectionStringSettings.Bind(connectionStrings);

            builder.Services.AddMediatR(typeof(PostRecordFeedbackHandler).Assembly);
            builder.Services.AddSingleton<IHttpClientWrapper, HttpClientWrapper>();
            builder.Services.AddTransient<IRepository, Repository>();
            builder.Services.AddSingleton<IRequestService, RequestService>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    ConfigureDbContextOptionsBuilder(options, connectionStrings.FeedbackService),
                ServiceLifetime.Transient
            );

            // automatically apply EF migrations
            // DbContext is being created manually instead of through DI as it throws an exception and I've not managed to find a way to solve it yet: 
            // 'Unable to resolve service for type 'Microsoft.Azure.WebJobs.Script.IFileLoggingStatusManager' while attempting to activate 'Microsoft.Azure.WebJobs.Script.Diagnostics.HostFileLoggerProvider'.'
            DbContextOptionsBuilder<ApplicationDbContext> dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            ConfigureDbContextOptionsBuilder(dbContextOptionsBuilder, connectionStrings.FeedbackService);
            ApplicationDbContext dbContext = new ApplicationDbContext(dbContextOptionsBuilder.Options);

            dbContext.Database.Migrate();
        }

        private void ConfigureDbContextOptionsBuilder(DbContextOptionsBuilder options, string connectionString)
        {
            options
                .UseSqlServer(connectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
        }
    }
}