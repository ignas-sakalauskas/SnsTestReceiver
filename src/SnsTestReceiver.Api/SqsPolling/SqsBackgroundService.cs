using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnsTestReceiver.Api.Configuration;
using SnsTestReceiver.Api.Services;
using SnsTestReceiver.Sdk.Models;

namespace SnsTestReceiver.Api.SqsPolling
{
    public class SqsBackgroundService : BackgroundService
    {
        private readonly IRepository _repository;
        private readonly SqsSettings _settings;
        private readonly IAmazonSQS _client;
        private readonly ILogger<SqsBackgroundService> _logger;

        public SqsBackgroundService(IRepository repository, IOptions<SqsSettings> settings, IAmazonSQS client, ILogger<SqsBackgroundService> logger)
        {
            _repository = repository;
            _settings = settings.Value;
            _client = client;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting SQS polling of {_settings.Urls.Count} queues");

            if (!_settings.Urls.Any())
            {
                _logger.LogWarning("No SQS URLs configured, exiting...");
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var tasks = _settings.Urls.Select(url => ProcessMessagesAsync(url, cancellationToken));
                    await Task.WhenAll(tasks);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("An operation has been cancelled");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error while processing messages: {ex.Message}", cancellationToken);
                    await Task.Delay(1_000, cancellationToken);
                }
            }
        }

        private async Task ProcessMessagesAsync(Uri url, CancellationToken cancellationToken)
        {
            var receiveRequest = new ReceiveMessageRequest
            {
                QueueUrl = url.ToString(),
                MaxNumberOfMessages = 10,
                MessageAttributeNames = new List<string> { "All" }
            };

            var response = await _client.ReceiveMessageAsync(receiveRequest, cancellationToken);
            if (!response.Messages.Any())
            {
                _logger.LogDebug($"No messages received at {url}");
                return;
            }

            _logger.LogInformation($"Received {response.Messages.Count} message(s), request ID={response.ResponseMetadata.RequestId}");

            foreach (var message in response.Messages)
            {
                try
                {
                    var body = JsonSerializer.Deserialize<SnsMessage>(message.Body, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    });

                    if (!_repository.TryCreate(body.MessageId, body))
                    {
                        _logger.LogError($"Error persisting {message.MessageId}");
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error deserializing {message.MessageId}");
                }
            }
            
            var entries = response.Messages.Select(message => new DeleteMessageBatchRequestEntry
            {
                Id = message.MessageId,
                ReceiptHandle = message.ReceiptHandle
            });

            var deleteRequest = new DeleteMessageBatchRequest
            {
                QueueUrl = url.ToString(),
                Entries =  new List<DeleteMessageBatchRequestEntry>(entries)
            };

            var deleteResponse = await _client.DeleteMessageBatchAsync(deleteRequest, cancellationToken);

            _logger.LogInformation($"Batch deleted {response.Messages.Count} messages, request ID={deleteResponse.ResponseMetadata.RequestId}");
        }
    }
}
