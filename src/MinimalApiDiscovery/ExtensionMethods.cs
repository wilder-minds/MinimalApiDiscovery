using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
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
  [Obsolete("No longer need to use the Service Collection")]
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
  [Obsolete("No longer need to use the Service Collection")]
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
  /// <exception cref="MinimalApiDiscoveryException"></exception>
  [Obsolete("No longer need to use the Service Collection")]
  public static IServiceCollection AddApis(this IServiceCollection coll,
    Assembly[]? searchAssemblies = null,
    ServiceLifetime lifetime = ServiceLifetime.Transient)
  {
    try
    {
      // Default to all assemblies
      if (searchAssemblies is null || !searchAssemblies.Any())
      {
        searchAssemblies = AppDomain.CurrentDomain.GetAssemblies();
      }

      var factory = LoggerFactory.Create(cfg => cfg.AddConsole());
      var logger = factory.CreateLogger("MinimalApiDiscovery");

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
              logger.LogWarning("Using Constructor Injection on classes registered through IApi will cause a long-lived singleton. Please only use parameter injection.");
              break;
            }
          }
          var existingApis = coll.Where(d => d.ServiceType == typeof(IApi)).ToList();
          logger.LogWarning("IApi classes already registered, make sure you're not calling AddApis twice.");
          if (!existingApis.Any(d => d.ImplementationType == api))
          {
            coll.Add(new ServiceDescriptor(typeof(IApi), api, lifetime));
          }
        }
      }
      return coll;
    }
    catch (Exception ex)
    {
      throw new MinimalApiDiscoveryException("Exception thrown while adding IApi Classes to Service Collection", ex);
    }
  }


  /// <summary>
  /// This method allows you to call <seealso cref="IApi.Register"/> 
  /// on all services that implement the IApi type.
  /// </summary>
  /// <param name="app"></param>
  /// <param name="assembly">The assembly to look for IApi types</param>
  /// <returns>The same WebApplication object.</returns>
  /// <exception cref="MinimalApiDiscoveryException"></exception>
  public static IEndpointRouteBuilder MapApis(this IEndpointRouteBuilder app,
    Assembly assembly)
  {
    return app.MapApis(new Assembly[] { assembly });
  }

  /// <summary>
  /// This method allows you to call <seealso cref="IApi.Register"/> 
  /// on all services that implement the IApi type.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="apis">Collection of IApi objects to register.</param>
  /// <returns>The same WebApplication object.</returns>
  /// <exception cref="MinimalApiDiscoveryException"></exception>
  public static IEndpointRouteBuilder MapApis(this IEndpointRouteBuilder builder,
    params Type[] apis)
  {
    return CreateAndRegister(builder, apis);
  }

  /// <summary>
  /// This method allows you to call <seealso cref="IApi.Register"/> 
  /// on all services that implement the IApi type.
  /// </summary>
  /// <param name="builder">
  /// The Web Application to register the 
  /// <seealso cref="IApi"/> derived classes.
  /// </param>
  /// <param name="searchAssemblies">An array of assemblies to search for IApi implemented classes.
  /// </param>
  /// <returns>The same WebApplication object.</returns>
  /// <exception cref="MinimalApiDiscoveryException"></exception>
  public static IEndpointRouteBuilder MapApis(this IEndpointRouteBuilder builder,
    Assembly[]? searchAssemblies = null)
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
        var apiTypes = assembly.GetTypes()
          .Where(t => t.IsAssignableTo(typeof(IApi)) && t.IsClass && !t.IsAbstract)
          .ToArray();


        CreateAndRegister(builder, apiTypes);
      }

      return builder;
    }
    catch (Exception ex)
    {
      throw new MinimalApiDiscoveryException($"Failed to find IApi types: {ex}");
    }
  }


  private static IEndpointRouteBuilder CreateAndRegister(IEndpointRouteBuilder builder,
    Type[] apiTypes)
  {

    try
    {

      // Add them all to the Service Collection
      foreach (var apiType in apiTypes)
      {
        var ctors = apiType.GetConstructors();
        foreach (var c in ctors)
        {
          if (c.GetParameters().Length != 0)
          {
            var factory = LoggerFactory.Create(cfg => cfg.AddConsole());
            var logger = factory.CreateLogger("MinimalApiDiscovery");
            logger.LogWarning("Using Constructor Injection on classes registered through IApi will cause a long-lived singleton. Please only use parameter injection.");
            throw new MinimalApiDiscoveryException("Api classes require an empty constructor.");
          }
        }
        if (apiType is null) throw new MinimalApiDiscoveryException("Apis not found");

        var newInstance = Activator.CreateInstance(apiType);
        if (newInstance is null) throw new MinimalApiDiscoveryException($"Could not create Api class: {apiType.Name}");
        IApi api = (IApi)newInstance;
        api.Register(builder);

      }
      return builder;
    }
    catch (Exception ex)
    {
      throw new MinimalApiDiscoveryException("Exception thrown while registering IApi Classes", ex);
    }
  }
}
