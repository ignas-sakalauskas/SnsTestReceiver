using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace SnsTestReceiver.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private const string MessageTemplate = "Request {method} {url} => {statusCode}";

        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                var statusCode = context.Response?.StatusCode;
                var logLevel = LogLevel.Information;

                if (statusCode >= 500)
                {
                    logLevel = LogLevel.Error;
                }
                else if (statusCode >= 400)
                {
                    logLevel = LogLevel.Warning;
                }

                _logger.Log(
                    logLevel, 
                    MessageTemplate,
                    context.Request?.Method,
                    context.Request?.Path.Value, 
                    statusCode);
            }
        }
    }
}
