using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WilderMinds.MinimalApiDiscovery;

public static class ExtensionMethods
{
  public static IServiceCollection AddApis(this IServiceCollection coll,
    Assembly assembly,
    ServiceLifetime lifetime = ServiceLifetime.Transient)
  {
    return coll.AddApis(new Assembly[] { assembly });
  }

  public static IServiceCollection AddApis(this IServiceCollection coll,
  Assembly[]? searchAssemblies = null,
  ServiceLifetime lifetime = ServiceLifetime.Transient)
  {
    // Default to all assemblies
    if (searchAssemblies is null) searchAssemblies = AppDomain.CurrentDomain.GetAssemblies();

    // Check all assemblies
    foreach (var assembly in searchAssemblies) 
    {
      // Find the IApi types
      var apis = assembly.GetTypes()
        .Where(t => t.IsAssignableTo(typeof(IApi)) && t.IsClass && !t.IsAbstract)
        .ToArray();

      // Add them all to the Service Collection
      foreach (var api in apis)
      {
        coll.Add(new ServiceDescriptor(typeof(IApi), api, lifetime));
      }
    }
    return coll;
  }


  public static WebApplication UseApis(this WebApplication app)
  {
    var apis = app.Services.GetServices<IApi>();

    foreach (var api in apis)
    {
      if (api is null) throw new InvalidProgramException("Apis not found");

      api.Register(app);
    }

    return app;
  }
}
