using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Xunit;

namespace SnsTestReceiver.Api.Tests.Infrastructure
{
    public class IntegrationTestsBase : IClassFixture<ApiWebApplicationFactory<Startup>>
    {
        protected readonly IAmazonSimpleNotificationService NotificationService;
        protected readonly string TopicArn;
        private readonly ServiceCollection _serviceCollection = new();
        private readonly ServiceProvider _serviceProvider;

        protected HttpClient HttpClient { get; }

        protected IntegrationTestsBase(WebApplicationFactory<Startup> factory)
        {
            factory.Server.AllowSynchronousIO = true;
            HttpClient = factory.CreateClient();


            var configuration = factory.Server.Host.Services.GetService<IConfiguration>();
            TopicArn = configuration.GetValue<string>("SNS:TopicArn");

            _serviceCollection.AddAWSService<IAmazonSimpleNotificationService>(configuration.GetAWSOptions("SNS"));
            _serviceProvider = _serviceCollection.BuildServiceProvider();

            NotificationService = _serviceProvider.GetService<IAmazonSimpleNotificationService>();
        }

        public void Dispose()
        {
            _serviceProvider?.Dispose();
        }

        protected static async Task<T> ReadResponseAsync<T>(HttpContent httpContent)
        {
            await using var responseStream = await httpContent.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<T>(responseStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result;
        }
    }
}