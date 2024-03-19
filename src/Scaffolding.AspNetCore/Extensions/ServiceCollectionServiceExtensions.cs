using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using Scaffolding.AspNetCore;
using Scaffolding.AspNetCore.Filters;
using Scaffolding.Configuration;

using System.Text.Json;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionServiceExtensions
    {
        /// <summary>
        /// Adds all scaffolding configuration services as singleton
        /// </summary>
        /// <param name="services"></param>
        /// <param name="scaffoldingConfigurationBuilder"></param>
        public static IServiceCollection AddScaffoldingSettings(this IServiceCollection services, IScaffoldingConfiguration scaffoldingConfiguration)
        {
            services.AddSingleton(scaffoldingConfiguration);
            foreach (var setting in scaffoldingConfiguration.Settings)
            {
                var implSetting = setting.GetType();
                var interf = implSetting.GetInterfaces().FirstOrDefault(x => x.Name != "ISettings");
                if (interf != null)
                    services.AddSingleton(interf, setting);
                
                services.AddSingleton(setting.GetType(), setting);
            }

            return services;
        }

        public static void AddSerilogRequestResponseLogging(this IServiceCollection services)
        {
            services.AddMvcCore(x =>
            {
                x.Filters.Add(typeof(RequestResponseLoggingFilter), 0);
            });
        }

        public static IServiceCollection ConfigureJson(this IServiceCollection services, NamingStrategy namingStrategy, Action<MvcNewtonsoftJsonOptions> configureOptions = null)
        {
            ScaffoldingJsonNamingStrategy.DEFAULT_STRATEGY = namingStrategy;
            services.AddControllers().AddNewtonsoftJson(x =>
            {
                x.SerializerSettings.ContractResolver = new DefaultContractResolver() { NamingStrategy = ScaffoldingJsonNamingStrategy.DEFAULT_STRATEGY };
                x.SerializerSettings.Converters.Add(new StringEnumConverter(ScaffoldingJsonNamingStrategy.GET_DEFAULT_STRATEGY_TYPE));
                x.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                configureOptions?.Invoke(x);
            });
            return services;
        }

        public static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            return services;
        }
    }
}
