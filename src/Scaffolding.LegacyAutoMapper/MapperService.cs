﻿using AutoMapper;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

using System.Reflection;

namespace Scaffolding.LegacyAutoMapper;

public static class MapperService
{
    public static IServiceCollection AddLegacyAutoMapper(this IServiceCollection services)
    {
        var mapper = GetMapper();

        services.AddSingleton(mapper);

        GlobalMapper.Mapper = mapper;

        return services;
    }

    public static void SetupAutoMapper()
    {
        var mapper = MapperService.GetMapper();

        GlobalMapper.Mapper = mapper;
    }

    private static Assembly[] GetAssemblies()
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

        return assemblies.ToArray();
    }

    private static IMapper GetMapper()
    {
        var profiles = MapperService.GetAssemblies()
            .SelectMany(p => p.GetTypes())
            .Where(p => p.GetTypeInfo().BaseType == typeof(Profile));

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AllowNullCollections = true;
            cfg.AllowNullDestinationValues = true;
            foreach (var profile in profiles)
            {
                cfg.AddProfile(Activator.CreateInstance(profile) as Profile);
            }
        });

        return configuration.CreateMapper();
    }
}