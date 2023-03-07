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
  [Obsolete]
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
  [Obsolete]
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
  [Obsolete]
  public static IServiceCollection AddApis(this IServiceCollection coll,
    Assembly[]? searchAssemblies = null,
    ServiceLifetime lifetime = ServiceLifetime.Transient)
  {
    // NOOP
    return coll;
  }

  private static Type[] GetApiTypes(Assembly assembly, ILogger logger)
  {
    // Find the IApi types
    var apis = assembly.GetTypes()
      .Where(t => t.IsAssignableTo(typeof(IApi)) && t.IsClass && !t.IsAbstract)
      .ToArray();

    // Test to make sure they have empty constructors
    foreach (var api in apis)
    {
      var ctors = api.GetConstructors();
      foreach (var c in ctors)
      {
        if (c.GetParameters().Length != 0)
        {
          logger.LogWarning("Using Constructor Injection on classes registered through IApi will cause a long-lived singleton. Please only use parameter injection.");
          break;
        }
      }
    }

    return apis;
  }

  /// <summary>
  /// This method allows you to call <seealso cref="IApi.Register"/> 
  /// on all services that implement the IApi type.
  /// </summary>
  /// <param name="app">
  /// The Web Application to register the 
  /// <seealso cref="IApi"/> derived classes.
  /// </param>
  /// <param name="apiAssemblies">Assemblies to look for IApi classes.</param>
  /// <returns>The same WebApplication object.</returns>
  /// <exception cref="MinimalApiDiscoverException"></exception>
  public static WebApplication MapApis(this WebApplication app, Assembly[]? apiAssemblies = null)
  {
    try
    {

      apiAssemblies = apiAssemblies ?? AppDomain.CurrentDomain.GetAssemblies();

      var factory = LoggerFactory.Create(cfg => cfg.AddConsole());
      var logger = factory.CreateLogger("MinimalApiDiscovery");

      foreach (var assembly in apiAssemblies)
      {
        if (assembly is not null)
        {
          var apis = GetApiTypes(assembly, logger);

          foreach (var apiType in apis)
          {
            var api = Activator.CreateInstance(apiType) as IApi;
            if (api is null) throw new MinimalApiDiscoverException("Apis not found");

            api.Register(app);
          }
        }
      }
      return app;
    }
    catch (Exception ex)
    {
      throw new MinimalApiDiscoverException("Exception thrown while registering IApi Classes", ex);
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
  /// <param name="apiAssembly">The assembly to search.</param>
  /// <returns>The same WebApplication object.</returns>
  /// <exception cref="MinimalApiDiscoverException"></exception>
  public static WebApplication MapApis(this WebApplication app, Assembly apiAssembly)
  {
    return MapApis(app, new Assembly[] { apiAssembly });
  }

}
