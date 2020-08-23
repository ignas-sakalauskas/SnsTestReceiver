using Microsoft.Extensions.DependencyInjection;
using SnsTestReceiver.Sdk.Configuration;
using System;

namespace SnsTestReceiver.Sdk.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSnsTestReceiver(this IServiceCollection serviceCollection, SnsTestReceiverOptions options)
        {
            if (serviceCollection == null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            serviceCollection.AddHttpClient<ISnsTestReceiver, SnsTestReceiver>(c =>
            {
                c.BaseAddress = options.BaseUrl;
            });
        }
    }
}
