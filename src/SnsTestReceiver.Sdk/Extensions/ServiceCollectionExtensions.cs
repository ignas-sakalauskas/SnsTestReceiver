using Microsoft.Extensions.DependencyInjection;
using SnsTestReceiver.Sdk.Configuration;
using System;

namespace SnsTestReceiver.Sdk.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddSnsTestReceiver(this IServiceCollection serviceCollection, SnsTestReceiverOptions options)
        {
            ArgumentNullException.ThrowIfNull(serviceCollection);
            ArgumentNullException.ThrowIfNull(options);

            return serviceCollection.AddHttpClient<ISnsTestReceiverClient, SnsTestReceiverClient>(c =>
            {
                c.BaseAddress = options.BaseUrl;
            });
        }
    }
}
