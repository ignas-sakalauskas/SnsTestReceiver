using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SnsTestReceiver.Sdk.Configuration;
using SnsTestReceiver.Sdk.Extensions;
using SnsTestReceiver.Sdk.Models;
using System.Text.Json;
using Xunit;

namespace SnsTestReceiver.Sdk.Tests
{
    public class IntegrationTests : IDisposable
    {
        private readonly ServiceCollection _serviceCollection = new();
        private readonly ServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration = BuildConfiguration();
        private readonly IAmazonSimpleNotificationService _sns;

        private readonly ISnsTestReceiverClient _sut;

        public IntegrationTests()
        {
            _serviceCollection.AddSnsTestReceiver(new SnsTestReceiverOptions
            {
                BaseUrl = new Uri(_configuration["SnsTestReceiver:BaseUrl"])
            });

            _serviceCollection.AddDefaultAWSOptions(_configuration.GetAWSOptions());
            _serviceCollection.AddAWSService<IAmazonSimpleNotificationService>();

            _serviceProvider = _serviceCollection.BuildServiceProvider();

            _sns = _serviceProvider.GetService<IAmazonSimpleNotificationService>();
            _sut = _serviceProvider.GetService<ISnsTestReceiverClient>();

            // Confirm SNS subscription, otherwise notifications are not forwarded at all
            _sut.ConfirmSubscriptionAsync().GetAwaiter().GetResult();
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

        [Theory, AutoData]
        public async Task Given_notification_published_to_sns_then_the_notification_should_be_found_via_sdk(string message)
        {
            // Given
            var request = new PublishRequest
            {
                TopicArn = "arn:aws:sns:eu-west-1:000000000000:test-notifications",
                Message = message,
                Subject = "MyTestNotification"
            };

            // When
            await _sns.PublishAsync(request);

            // Then
            await Task.Delay(1000); // sometimes localstack is too slow
            var result = await _sut.SearchAsync(request.Message);
            result.Should().HaveCount(1);
            result.Single().Message.Should().Be(request.Message);
        }

        public void Dispose()
        {
            _serviceProvider?.Dispose();
        }

        private static IConfiguration BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.tests.json", false, false);

            return configurationBuilder.Build();
        }
    }
}
