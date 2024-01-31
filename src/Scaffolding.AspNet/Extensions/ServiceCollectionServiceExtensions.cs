using Microsoft.Extensions.DependencyInjection;
using Scaffolding.Configuration;

namespace Scaffolding.AspNet.Extensions
{
    public static class ServiceCollectionServiceExtensions
    {
        /// <summary>
        /// Adds all scaffolding configuration services as singleton
        /// </summary>
        /// <param name="services"></param>
        /// <param name="scaffoldingConfigurationBuilder"></param>
        public static void AddSingletonScaffoldingServices(this IServiceCollection services, IScaffoldingConfiguration scaffoldingConfiguration)
        {
            foreach (var setting in scaffoldingConfiguration.Settings)
            {
                var interf = setting.GetType().GetInterfaces().FirstOrDefault();
                if (interf is null)
                    services.AddSingleton(setting);
                else
                    services.AddSingleton(interf, setting);
            }
        }
    }
}
