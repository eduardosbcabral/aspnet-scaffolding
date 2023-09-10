using Microsoft.AspNetCore.Mvc;
using Scaffolding.Extensions.GracefulShutdown;
using Scaffolding.Extensions.Json;
using Scaffolding.Extensions.Logging;
using Scaffolding.Models;
using Scaffolding.Utilities;
using Scaffolding.Utilities.Converters;

namespace Scaffolding.Controllers;

[ApiController]
[Route("/")]
public class AppInformationController : ControllerBase
{
    private readonly ApiSettings _apiSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly GracefulShutdownState GracefulShutdownState;

    public AppInformationController(
        ApiSettings apiSettings, 
        ShutdownSettings shutdownSettings,
        GracefulShutdownState gracefulShutdownState,
        IHttpContextAccessor httpContextAccessor)
    {
        this.HandleGracefulShutdown(shutdownSettings);
        _apiSettings = apiSettings;
        _httpContextAccessor = httpContextAccessor;
        this.GracefulShutdownState = gracefulShutdownState;
    }

    [HttpGet]
    [Produces(typeof(HomeDetails))]
    public IActionResult GetAppInfo()
    {
        this.DisableLogging();

        return Ok(new HomeDetails
        {
            RequestsInProgress = GracefulShutdownState.RequestsInProgress,
            BuildVersion = _apiSettings?.BuildVersion,
            Environment = EnvironmentUtility.GetCurrentEnvironment(),
            Application = _apiSettings.Name,
            Domain = _apiSettings.Domain,
            JsonSerializer = _apiSettings.JsonSerializer,
            EnvironmentPrefix = _apiSettings.EnvironmentVariablesPrefix,
            TimezoneInfo = new TimezoneInfo(_apiSettings, _httpContextAccessor),
            CurrentTime = DateTime.UtcNow,
        });
    }

    public class HomeDetails
    {
        public string Application { get; set; }
        public string Domain { get; set; }
        public string BuildVersion { get; set; }
        public string Environment { get; set; }
        public string EnvironmentPrefix { get; set; }
        public long RequestsInProgress { get; set; }
        public JsonSerializerEnum JsonSerializer { get; set; }
        public TimezoneInfo TimezoneInfo { get; set; }
        public DateTime CurrentTime { get; set; }
    }

    public class TimezoneInfo
    {
        private readonly ApiSettings _apiSettings;

        public TimezoneInfo(ApiSettings apiSettings, IHttpContextAccessor httpContextAccessor)
        {
            CurrentTimezone = DateTimeConverter.GetTimeZoneByAspNetHeader(
                httpContextAccessor,
                apiSettings.TimezoneHeader).Id;

            _apiSettings = apiSettings;
        }

        public static string UtcNow => DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");

        public static DateTime CurrentNow => DateTime.UtcNow;

        public string DefaultTimezone => _apiSettings.TimezoneDefaultInfo.Id;

        public static string CurrentTimezone { get; set; }

        public string TimezoneHeader => _apiSettings.TimezoneHeader;
    }

}