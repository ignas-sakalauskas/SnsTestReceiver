using Amazon;
using Amazon.Runtime;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using SnsTestReceiver.Sdk;
using SnsTestReceiver.Sdk.Configuration;
using SnsTestReceiver.Sdk.Extensions;
using System.Text.Json;
using Xunit;

namespace SimpleIntegrationTest
{
    public class NotificationTests
    {
        [Fact]
        public async Task Given_sns_notification_sent_successfully_then_the_notification_should_contain_the_correct_message()
        {
            // Given
            var collection = new ServiceCollection();
            collection.AddSnsTestReceiver(new SnsTestReceiverOptions
            {
                BaseUrl = new Uri("http://localhost:5000/")
            });

            collection.AddAWSService<IAmazonSimpleNotificationService>(new AWSOptions
            {
                Credentials = new BasicAWSCredentials("xx", "xx"),
                DefaultClientConfig =
                {
                    AuthenticationRegion = RegionEndpoint.EUWest1.SystemName,
                    RegionEndpoint = RegionEndpoint.EUWest1,
                    ServiceURL = "http://localhost:4566",
                },
            });

            var expectedId = Guid.NewGuid().ToString();
            var expectedTestObject = new TestObject
            {
                IntProperty = 123,
                StringProperty = expectedId
            };

            var request = new PublishRequest
            {
                TopicArn = "arn:aws:sns:eu-west-1:000000000000:test-notifications",
                Message = JsonSerializer.Serialize(expectedTestObject),
                Subject = "MyTestNotification"
            };

            await using var sp = collection.BuildServiceProvider();
            var sns = sp.GetService<IAmazonSimpleNotificationService>();
            var testReceiver = sp.GetService<ISnsTestReceiverClient>();
            // Needed starting Localstack 1.x
            await testReceiver.ConfirmSubscriptionAsync();

            // When
            await sns.PublishAsync(request);

            // Then
            await Task.Delay(1000); // sometimes localstack is too slow
            var result = await testReceiver.SearchAsync(expectedId);
            result.Should().HaveCount(1);
            result.Single().Subject.Should().Be(request.Subject);
            var testObject = JsonSerializer.Deserialize<TestObject>(result.Single().Message);
            testObject.IntProperty.Should().Be(123);
            testObject.StringProperty.Should().Be(expectedId);
        }

        internal sealed class TestObject
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
        }
    }
}
