using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
            diagnosticContext.Set("ResponseHeaders", GetHeaders(context.HttpContext.Response.Headers), true);
        }
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ControllerName;
        var action = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ActionName;

        diagnosticContext.Set("Controller", controller);
        diagnosticContext.Set("Operation", $"{action}");

        if (context.ActionArguments is not null)
        {
            diagnosticContext.Set("RequestBody", context.ActionArguments, true);
        }

        if (context.HttpContext.Request.Headers.Any())
        {
            diagnosticContext.Set("RequestHeaders", GetHeaders(context.HttpContext.Request.Headers), true);
        }
    }

    private static Dictionary<string, string> GetHeaders(IHeaderDictionary headers)
    {
        var dic = new Dictionary<string, string>();
        foreach (var item in headers)
        {
            var value = item.Value.ToString();
            dic[item.Key] = value;
        }

        return dic;
    }
}