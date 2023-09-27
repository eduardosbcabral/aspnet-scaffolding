using Serilog.Context;
using Serilog;
using System.Net;
using Scaffolding.Utilities.Extractors;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Scaffolding.Extensions.Logging;

public class CommunicationLogger : ICommunicationLogger
{
    /// <summary>
    /// Default Log Information Title
    /// </summary>
    public const string DefaultInformationTitle = "HTTP {Method} {Path} from {Ip} responded {StatusCode} in {ElapsedMilliseconds} ms";

    /// <summary>
    /// Default Log Error Title
    /// </summary>
    public const string DefaultErrorTitle = "HTTP {Method} {Path} from {Ip} responded {StatusCode} in {ElapsedMilliseconds} ms";

    /// <summary>
    ///  Serilog Configuration
    /// </summary>
    public LogConfiguration LogConfiguration { get; set; }

    /// <summary>
    /// Constructor with configuration
    /// </summary>
    /// <param name="logger"></param>
    public CommunicationLogger(LogConfiguration configuration)
    {
        this.SetupCommunicationLogger(configuration);
    }

    /// <summary>
    /// Constructor using global logger definition
    /// </summary>
    public CommunicationLogger()
    {
        this.SetupCommunicationLogger(null);
    }

    /// <summary>
    /// Log context 
    /// </summary>
    /// <param name="context"></param>
    public async Task LogData(HttpContext context)
    {
        var routeDisabled = (this.LogConfiguration.IgnoredRoutes?.ToList()
            .Where(r => context.Request.Path.ToString().StartsWith(r)).Any() == true);

        if ((context?.Items == null || context.Items.TryGetValue(DisableLoggingExtension.ITEM_NAME, out object disableSerilog) == false)
            && routeDisabled == false)
        {
            await this.LogData(context, null);
        }
    }

    /// <summary>
    /// Log context and exception
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    public async Task LogData(HttpContext context, Exception exception)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var statusCode = context.GetStatusCode(exception);

        string exceptionMessage = null;
        string exceptionStackTrace = null;

        if (exception != null)
        {
            exceptionMessage = HandleFieldSize(exception.Message, ExceptionMaxLengthExtension.ErrorMessageLength);
            exceptionStackTrace = HandleFieldSize(exception.StackTrace, ExceptionMaxLengthExtension.ErrorExceptionLength);
        }

        var endpoint = context.GetEndpoint();
        if(endpoint != null)
        {
            var controllerActionDescriptor = endpoint
                .Metadata
                .GetMetadata<ControllerActionDescriptor>();

            if(controllerActionDescriptor != null)
            {
                LogContext.PushProperty("Controller", controllerActionDescriptor.ControllerName);
                LogContext.PushProperty("Action", controllerActionDescriptor.ActionName);
            }
        }

        LogContext.PushProperty("RequestBody", await context.GetRequestBody(this.LogConfiguration.BlacklistRequest));
        LogContext.PushProperty("Method", context.Request.Method);
        LogContext.PushProperty("Path", context.GetPath(this.LogConfiguration.HttpContextBlacklist));
        LogContext.PushProperty("Host", context.GetHost());
        LogContext.PushProperty("Port", context.GetPort());
        LogContext.PushProperty("Url", context.GetFullUrl(this.LogConfiguration.QueryStringBlacklist, this.LogConfiguration.HttpContextBlacklist));
        LogContext.PushProperty("QueryString", context.GetRawQueryString(this.LogConfiguration.QueryStringBlacklist));
        LogContext.PushProperty("Query", context.GetQueryString(this.LogConfiguration.QueryStringBlacklist));
        LogContext.PushProperty("RequestHeaders", context.GetRequestHeaders(this.LogConfiguration.HeaderBlacklist));
        LogContext.PushProperty("Ip", context.GetIp());
        LogContext.PushProperty("User", context.GetUser());
        LogContext.PushProperty("IsSuccessful", statusCode < 400);
        LogContext.PushProperty("StatusCode", statusCode);
        LogContext.PushProperty("StatusDescription", ((HttpStatusCode)statusCode).ToString());
        LogContext.PushProperty("StatusCodeFamily", context.GetStatusCodeFamily(exception));
        LogContext.PushProperty("ProtocolVersion", context.Request.Protocol);
        LogContext.PushProperty("ErrorException", exceptionStackTrace);
        LogContext.PushProperty("ErrorMessage", exceptionMessage);
        LogContext.PushProperty("ResponseContent", await context.GetResponseContent(this.LogConfiguration.BlacklistResponse));
        LogContext.PushProperty("ContentType", context.Response.ContentType);
        LogContext.PushProperty("ContentLength", context.GetResponseLength());
        LogContext.PushProperty("ResponseHeaders", context.GetResponseHeaders(this.LogConfiguration.HeaderBlacklist));
        LogContext.PushProperty("Version", this.LogConfiguration.Version);
        LogContext.PushProperty("ElapsedMilliseconds", context.GetExecutionTime(this.LogConfiguration.TimeElapsedProperty));
        LogContext.PushProperty("RequestKey", context.GetRequestKey(this.LogConfiguration.RequestKeyProperty));
        LogContext.PushProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

        if (context.Items.ContainsKey(LogAdditionalInfo._logAdditionalInfoItemKey))
        {
            var additionalInfo = (LogAdditionalInfo)context.Items[LogAdditionalInfo._logAdditionalInfoItemKey];

            if (additionalInfo?.Data != null)
            {
                foreach (var item in additionalInfo.Data)
                {
                    LogContext.PushProperty(item.Key, item.Value);
                }
            }
        }

        if (exception != null || statusCode >= 500)
        {
            var errorTitle = this.LogConfiguration.ErrorTitle ?? DefaultErrorTitle;
            Log.Logger.Error(errorTitle);
        }
        else
        {
            var informationTitle = this.LogConfiguration.InformationTitle ?? DefaultInformationTitle;
            Log.Logger.Information(informationTitle);
        }
    }

    /// <summary>
    /// Initialize instance
    /// </summary>
    /// <param name="configuration"></param>
    private void SetupCommunicationLogger(LogConfiguration configuration)
    {
        this.LogConfiguration = configuration ?? new LogConfiguration();
    }

    /// <summary>
    /// Handle field size
    /// </summary>
    /// <param name="value"></param>
    /// <param name="maxSize"></param>
    /// <param name="required"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    private static string HandleFieldSize(string value, int maxSize, bool required = false, string defaultValue = "????")
    {
        if (string.IsNullOrWhiteSpace(value) && !required)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            value = defaultValue;
        }

        if (value.Length > maxSize)
        {
            return value.Substring(0, maxSize);
        }

        return value;
    }
}

public interface ICommunicationLogger
{
    Task LogData(HttpContext context);
    Task LogData(HttpContext context, Exception exception);
}