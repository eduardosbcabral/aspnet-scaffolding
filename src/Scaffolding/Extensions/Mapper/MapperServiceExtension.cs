using AutoMapper;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;

namespace Scaffolding.Extensions.Mapper
{
    public static class MapperServiceExtension
    {
        public static void SetupAutoMapper(this WebApplicationBuilder builder, params Assembly[] assemblies)
        {
            builder.Services.AddAutoMapper(GetAssemblies(assemblies));
        }

        public static void UseAutoMapper(this WebApplication app)
        {
            var mapper = app.Services.GetService<IMapper>();
            GlobalMapper.Mapper = mapper;
        }

        private static Assembly[] GetAssemblies(Assembly[] customAssemblies)
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries.Where(p =>
                p.Type.Equals("Project", StringComparison.CurrentCultureIgnoreCase));

            foreach (var library in dependencies)
            {
                var name = new AssemblyName(library.Name);
                var assembly = Assembly.Load(name);
                assemblies.Add(assembly);
            }

            assemblies.AddRange(customAssemblies);

            return assemblies.ToArray();
        }
    }
}
