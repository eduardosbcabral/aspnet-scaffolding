using AutoMapper;
using System.Reflection;

namespace Scaffolding.Extensions.Mapper
{
    public static class MapperServiceExtension
    {
        public static void SetupAutoMapper(this WebApplicationBuilder builder, params Assembly[] assemblies)
        {
            builder.Services.AddAutoMapper(assemblies);
        }

        public static void UseAutoMapper(this WebApplication app)
        {
            var mapper = app.Services.GetService<IMapper>();
            GlobalMapper.Mapper = mapper;
        }
    }
}
