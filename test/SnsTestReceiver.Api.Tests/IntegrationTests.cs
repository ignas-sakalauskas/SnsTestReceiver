using Amazon.SimpleNotificationService.Model;
using AutoFixture.Xunit2;
using FluentAssertions;
using SnsTestReceiver.Api.Tests.Infrastructure;
using SnsTestReceiver.Sdk.Models;
using System.Net;
using Xunit;

namespace SnsTestReceiver.Api.Tests
{
    public class IntegrationTests : IntegrationTestsBase
    {
        public IntegrationTests(ApiWebApplicationFactory<Startup> factory)
            : base(factory)
        {
        }

        [Theory, AutoData]
        public async Task Given_message_publish_to_sns_and_forwarded_to_sqs_subscriber_then_should_receive(List<string> ids)
        {
            // Given
            var ping = await HttpClient.GetAsync("/messages");
            ping.StatusCode.Should().Be(HttpStatusCode.OK);

            foreach (var id in ids)
            {
                await NotificationService.PublishAsync(new PublishRequest(TopicArn, id));
            }

            await Task.Delay(5_000);

            // When
            foreach (var id in ids)
            {
                var result = await HttpClient.GetAsync($"/messages?Search={id}");

                // Then
                result.StatusCode.Should().Be(HttpStatusCode.OK);
                var snsMessages = await ReadResponseAsync<List<SnsMessage>>(result.Content);
                snsMessages.Should().HaveCount(1);
                snsMessages.Single().Message.Should().Be(id);
            }
        }
    }
}
