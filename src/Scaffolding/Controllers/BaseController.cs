using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Scaffolding.Extensions.Cors;
using WebApi.Models.Response;

namespace Scaffolding.Controllers;

[EnableCors(CorsServiceExtension.CorsName)]
public class BaseController : ControllerBase
{
    public BaseController()
    {
    }

    protected IActionResult CreateJsonResponse(ApiResponse response)
    {
        IActionResult result;

        if (response.Content != null)
        {
            result = new JsonResult(response.Content)
            {
                StatusCode = (int)response.StatusCode
            };
        }
        else
        {
            result = new StatusCodeResult((int)response.StatusCode);
            Response.ContentType = "application/json";
        }

        if (response.Headers != null)
        {
            foreach (var header in response.Headers)
            {
                Response.Headers[header.Key] = header.Value;
            }
        }

        return result;
    }
}
