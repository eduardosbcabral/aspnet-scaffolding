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
        var result = context.Result;
        if (result is ObjectResult objectResult)
        {
            var r = objectResult.Value;
            diagnosticContext.Set("ResponseBody", r, true);
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
            var bodyParameter = context.ActionDescriptor.Parameters.FirstOrDefault(x => x?.BindingInfo?.BindingSource?.Id == "Body");
            if (bodyParameter is not null)
            {
                var bodyValue = context.ActionArguments.FirstOrDefault(x => x.Key == bodyParameter.Name);
                if (bodyValue.Value is not null)
                {
                    diagnosticContext.Set("RequestBody", bodyValue.Value, true);
                }
            }
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