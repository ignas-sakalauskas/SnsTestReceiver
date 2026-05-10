using SnsTestReceiver.Api.Helpers;

namespace SnsTestReceiver.Api.Middleware
{
    public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        private const string MessageTemplate = "Request {method} {path}{query} => {statusCode}";

        private readonly RequestDelegate _next = next;
        private readonly ILogger _logger = logger;

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
                    context.Request?.Method.SanitizeForLog(),
                    context.Request?.Path.Value.SanitizeForLog(),
                    context.Request?.QueryString.Value.SanitizeForLog(),
                    statusCode);
            }
        }
    }
}
