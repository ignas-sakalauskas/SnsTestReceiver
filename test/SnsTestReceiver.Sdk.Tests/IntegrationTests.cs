using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SnsTestReceiver.Sdk.Configuration;
using SnsTestReceiver.Sdk.Extensions;
using SnsTestReceiver.Sdk.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace SnsTestReceiver.Sdk.Tests
{
    public class IntegrationTests : IDisposable
    {
        private readonly ServiceCollection _serviceCollection = new ServiceCollection();
        private readonly ServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration = BuildConfiguration();

        private readonly ISnsTestReceiverClient _sut;

        public IntegrationTests()
        {
            _serviceCollection.AddSnsTestReceiver(new SnsTestReceiverOptions
            {
                BaseUrl = _configuration.GetValue<Uri>("SnsTestReceiver:BaseUrl")
            });

            _serviceProvider = _serviceCollection.BuildServiceProvider();

            _sut = _serviceProvider.GetService<ISnsTestReceiverClient>();
        }

        [Theory, AutoData]
        public async Task Given_message_created_then_should_receive_by_id(SnsMessage message)
        {
            // Given
            var body = JsonSerializer.Serialize(message);
            await _sut.CreateAsync(body);

            // When
            var result = await _sut.GetAsync(message.MessageId);

            // Then
            result.Should().BeEquivalentTo(message);
        }

        [Theory, AutoData]
        public async Task Given_message_created_then_should_find_by_message(SnsMessage message)
        {
            // Given
            var body = JsonSerializer.Serialize(message);
            await _sut.CreateAsync(body);

            // When
            var result = await _sut.SearchAsync(message.Message);

            // Then
            result.Should().HaveCount(1);
            result.Single().Should().BeEquivalentTo(message);
        }

        public void Dispose()
        {
            _serviceProvider?.Dispose();
        }

        private static IConfiguration BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, false);

            return configurationBuilder.Build();
        }
    }
}
