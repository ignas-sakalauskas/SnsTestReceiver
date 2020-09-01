using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SnsTestReceiver.Sdk.Models;

namespace SnsTestReceiver.Sdk
{
    public class SnsTestReceiverClient : ISnsTestReceiverClient
    {
        private readonly HttpClient _httpClient;

        public SnsTestReceiverClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<SnsMessage>> SearchAsync(string text = null, int? limit = null)
        {
            using var response = await _httpClient.GetAsync($"/messages/?search={text}&limit={limit}");
            response.EnsureSuccessStatusCode();

            return await DeserializeAsync<List<SnsMessage>>(response.Content);
        }

        public async Task<SnsMessage> GetAsync(string messageId)
        {
            if (string.IsNullOrEmpty(messageId))
                throw new ArgumentNullException(nameof(messageId), "Param must have a value");

            using var response = await _httpClient.GetAsync($"/messages/{messageId}");
            response.EnsureSuccessStatusCode();

            return await DeserializeAsync<SnsMessage>(response.Content);
        }

        public async Task DeleteAsync(string messageId)
        {
            if (string.IsNullOrEmpty(messageId))
                throw new ArgumentNullException(nameof(messageId), "Param must have a value");

            using var response = await _httpClient.DeleteAsync($"/messages/{messageId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task CreateAsync(string body)
        {
            if (string.IsNullOrEmpty(body))
                throw new ArgumentNullException(nameof(body), "Param must have a value");

            var content = new StringContent(body, Encoding.UTF8);
            using var response = await _httpClient.PostAsync("/messages/", content);
            response.EnsureSuccessStatusCode();
        }

        private static async Task<T> DeserializeAsync<T>(HttpContent httpContent)
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