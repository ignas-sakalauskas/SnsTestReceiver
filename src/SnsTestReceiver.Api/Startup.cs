using Amazon.SQS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SnsTestReceiver.Api.Middleware;
using SnsTestReceiver.Api.Services;
using SnsTestReceiver.Api.SqsPolling;

namespace SnsTestReceiver.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.AddSingleton<IRepository, InMemoryRepository>();
            services.AddControllers();

            // SQS polling
            services.Configure<SqsSettings>(Configuration.GetSection("SQS"));
            services.AddAWSService<IAmazonSQS>(Configuration.GetAWSOptions("SQS"));
            services.AddHostedService<SqsBackgroundService>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
