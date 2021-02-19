using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace API.Middleware
{
    public class ErrorHandlerMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleWare> _logger;
        public ErrorHandlerMiddleWare(RequestDelegate next, ILogger<ErrorHandlerMiddleWare> logger)
        {
            this._logger = logger;
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, _logger);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger<ErrorHandlerMiddleWare> logger)
        {
            object errors = null;

            switch (ex)
            {
                case RestExeption re:
                    _logger.LogError(ex,"REST ERROR");
                    errors = re.Errors;
                    context.Response.StatusCode = (int)re.Code;
                    break;
                case Exception e:
                    _logger.LogError(ex,"SERVER ERROR");
                    errors = string.IsNullOrWhiteSpace(e.Message) ? "Errors":e.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType="application/json";
            if(errors != null){
                var result = JsonSerializer.Serialize(new {errors});

                await context.Response.WriteAsync(result);
            }
        }
    }
}