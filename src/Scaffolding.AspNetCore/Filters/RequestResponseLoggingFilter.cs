using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

using Scaffolding.Logging.Settings.Implementations;

using Serilog;

namespace Scaffolding.AspNetCore.Filters;

public class RequestResponseLoggingFilter(IDiagnosticContext diagnosticContext) : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is not null)
        {
            diagnosticContext.Set("ResponseBody", context.Result, true);
        }

        if (context.HttpContext.Response.Headers.Any())
        {
            var logSettings = context.HttpContext.RequestServices.GetService<LogSettings>();
            if (logSettings is not null)
            {
                var headers = RequestResponseLoggingEndpointFilterHelpers.GetHeaders(context.HttpContext.Response.Headers, logSettings.LogHeaders);
                if (headers.Count > 0)
                    diagnosticContext.Set("ResponseHeaders", headers, true);
            }
        }
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ControllerName;
        var action = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ActionName;

        diagnosticContext.Set("Controller", controller);
        diagnosticContext.Set("Operation", action);

        if (context.ActionArguments is not null)
        {
            diagnosticContext.Set("RequestBody", context.ActionArguments, true);
        }

        if (context.HttpContext.Request.Headers.Any())
        {
            var logSettings = context.HttpContext.RequestServices.GetService<LogSettings>();
            if (logSettings is not null)
            {
                var headers = RequestResponseLoggingEndpointFilterHelpers.GetHeaders(context.HttpContext.Request.Headers, logSettings.LogHeaders);
                if (headers.Count > 0)
                    diagnosticContext.Set("RequestHeaders", headers, true);
            }
        }
    }
}