using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WilderMinds.MinimalApiDiscovery;

/// <summary>
/// Extension Methods for MinimalApiDiscovery
/// </summary>
public static class ExtensionMethods
{

  /// <summary>
  /// Extends the IServiceCollection to support specifying a single assembly
  /// that contains classes that implement the <see cref="IApi"/> interface and 
  /// register them with the service collection with a specific lifetime.
  /// </summary>
  /// <param name="coll">The service collection.</param>
  /// <param name="assembly">A single assembly.</param>
  /// <param name="lifetime">Type of lifetime required for the APIs. Defaults to Transient</param>
  /// <returns>The same service collection.</returns>
  public static IServiceCollection AddApis(this IServiceCollection coll,
    Assembly assembly,
    ServiceLifetime lifetime = ServiceLifetime.Transient)
  {
    return coll.AddApis(new Assembly[] { assembly });
  }

  /// <summary>
  /// Extends the IServiceCollection to support specifying just a lifetime a single assembly
  /// that contains classes that implement the <see cref="IApi"/> interface and 
  /// register them with the service collection with a specific lifetime.
  /// </summary>
  /// <param name="coll">The service collection.</param>
  /// <param name="lifetime">Type of lifetime required for the APIs. Defaults to Transient.</param>
  /// <returns>The same service collection.</returns>
  public static IServiceCollection AddApis(this IServiceCollection coll,
    ServiceLifetime lifetime)
  {
    return coll.AddApis((Assembly[]?)null, lifetime);
  }

  /// <summary>
  /// Extends the IServiceCollection to support specifying a collection of assemblies 
  /// for classes that contains classes that implement the <see cref="IApi"/> 
  /// interface and register them with the service collection with a specific lifetime.
  /// </summary>
  /// <param name="coll">The service collection.</param>
  /// <param name="searchAssemblies">
  ///   A collection of assemblies to search (defaults to all loaded assemblies)
  /// </param>
  /// <param name="lifetime">Type of lifetime required for the APIs. Defaults to Transient</param>
  /// <returns>The same service collection.</returns>
  /// <exception cref="MinimalApiDiscoverException"></exception>
  public static IServiceCollection AddApis(this IServiceCollection coll,
    Assembly[]? searchAssemblies = null,
    ServiceLifetime lifetime = ServiceLifetime.Transient
    )
  {
    try
    {
      // Default to all assemblies
      if (searchAssemblies is null || !searchAssemblies.Any())
      {
        searchAssemblies = AppDomain.CurrentDomain.GetAssemblies();
      }

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
          var ctors = api.GetConstructors();
          foreach (var c in ctors)
          {
            if (c.GetParameters().Length != 0)
            {
              var factory = LoggerFactory.Create(cfg => cfg.AddConsole());
              var logger = factory.CreateLogger("MinimalApiDiscovery");
              logger.LogWarning("Using Constructor Injection on classes registered through IApi will cause a long-lived singleton. Please only use parameter injection.");
              break;
            }
          }
          coll.Add(new ServiceDescriptor(typeof(IApi), api, lifetime));
        }
      }
      return coll;
    }
    catch (Exception ex)
    {
      throw new MinimalApiDiscoverException("Exception thrown while adding IApi Classes to Service Collection", ex);
    }
  }


  /// <summary>
  /// This method allows you to call <seealso cref="IApi.Register"/> 
  /// on all services that implement the IApi type.
  /// </summary>
  /// <param name="app">
  /// The Web Application to register the 
  /// <seealso cref="IApi"/> derived classes.
  /// </param>
  /// <returns>The same WebApplication object.</returns>
  /// <exception cref="MinimalApiDiscoverException"></exception>
  public static WebApplication MapApis(this WebApplication app)
  {
    try
    {
      var apis = app.Services.GetServices<IApi>();

      foreach (var api in apis)
      {
        if (api is null) throw new InvalidProgramException("Apis not found");

        api.Register(app);
      }

      return app;
    }
    catch (Exception ex)
    {
      throw new MinimalApiDiscoverException("Exception thrown while registering IApi Classes", ex);
    }
  }
}
