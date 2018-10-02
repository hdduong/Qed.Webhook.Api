using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Qed.Webhook.Api.Shared.Constants;
using Qed.Webhook.Api.Shared.Loggings;

namespace Qed.Webhook.RedisCache.Api.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private Task HandleException(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            if (exception is ApiException) code = HttpStatusCode.InternalServerError;
            else if (exception is ApiUnauthorizedException) code = HttpStatusCode.Unauthorized;
            else if (exception is ApiNotFoundException) code = HttpStatusCode.NotFound;

            var result = JsonConvert.SerializeObject(new { message = JsonConvert.SerializeObject(exception.Message), stackTrace = exception.StackTrace });
            context.Response.ContentType = ConstantString.JsonContentTypeValue;
            context.Response.StatusCode = (int)code;

            _logger.LogError($"project-name: {ConstantString.RedisCacheApiProjectName} exception: {result} request: {context.Request}");

            return context.Response.WriteAsync(result);
        }
    }
}
