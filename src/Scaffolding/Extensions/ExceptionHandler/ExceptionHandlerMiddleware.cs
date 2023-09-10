using Newtonsoft.Json;
using Scaffolding.Extensions.Json;
using Scaffolding.Models;
using Scaffolding.Utilities;
using System.Net;
using WebApi.Models.Exceptions;
using WebApi.Models.Helpers;

namespace Scaffolding.Extensions.ExceptionHandler;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly bool _isDevelopment;

    public static Func<ApiException, object> ChangeErrorFormat;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
        _isDevelopment = EnvironmentUtility.IsDevelopment();
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, _isDevelopment);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, bool isDevelopment)
    {
        try
        {
            context.Request.Body.Position = 0;
        }
        catch { }

        if (exception is ApiException apiException)
        {
            return ApiException(context, apiException);
        }
        else
        {
            return GenericError(context, exception, isDevelopment);
        }
    }

    private static async Task GenericError(HttpContext context, Exception exception, bool isDevelopment)
    {
        context.Items.Add("Exception", exception);

        if (isDevelopment)
        {
            var exceptionContainer = new ExceptionContainer(exception);
            var serializedObject = JsonConvert.SerializeObject(exceptionContainer, JsonSerializerServiceExtension.JsonSerializerSettings);
            await context.Response.WriteAsync(serializedObject);
            context.Response.Body.Position = 0;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    }

    private static async Task ApiException(HttpContext context, ApiException exception)
    {
        var apiResponse = exception.ToApiResponse();

        var statusCode = (int)apiResponse.StatusCode;

        if (exception is PermanentRedirectException)
        {
            statusCode = 308;
            var location = $"{context.Request.Path}{context.Request.QueryString}";

            context.Response.Headers["Location"] = location;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        if (apiResponse.Content != null && ChangeErrorFormat == null)
        {
            var serializedObject = JsonConvert.SerializeObject(apiResponse.Content, JsonSerializerServiceExtension.JsonSerializerSettings);
            await context.Response.WriteAsync(serializedObject);
            context.Response.Body.Position = 0;
        }
        else if (ChangeErrorFormat != null)
        {
            var content = ChangeErrorFormat.Invoke(exception);
            var serializedObject = JsonConvert.SerializeObject(content, JsonSerializerServiceExtension.JsonSerializerSettings);
            await context.Response.WriteAsync(serializedObject);
            context.Response.Body.Position = 0;
        }
    }

}

public static class ExceptionHandlerMiddlewareExtension
{
    public static void UseScaffoldingExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}