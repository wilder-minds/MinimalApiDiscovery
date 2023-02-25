# MinimalApiDiscovery

This project is aimed to simplify the registration of Minimal APIs 
as projects grow. This is an idea I've been fumbling with for a few 
months and thought I'd put it into code. The project is open to PRs 
or discussions about how we could do this better or whether this 
even needs to exist. 

Note that if you're building Microservices, using this amount of 
plumbing is probably not required, but for larger projects I think 
provides a cleaner way of handling mapping.

The basic idea of this small library is to allow you to annotate 
a class with an interface that allows simplified mapping of Minimal APIs.

Note: I have a complete write-up and video of this package at: https://wildermuth.com/2023/02/22/minimal-api-discovery/

To get started, you can just install the package from Nuget or using the .NET tool:

```
> dotnet add package WilderMinds.MinimalApiDiscovery
```

Or:

```
> Install-Package WilderMinds.MinimalApiDiscovery
```

To use the package, you can create API classes that implement the IApi interface:

```csharp
/// <summary>
/// An interface for Identifying and registering APIs
/// </summary>
public interface IApi
{
  /// <summary>
  /// This is automatically called by the library to add your APIs
  /// </summary>
  /// <param name="builder">The IEndpointRouteBuilder object to register the API </param>
  void Register(IEndpointRouteBuilder builder);
}
```

This allows you to create classes that can bundle several different APIs together or even use .NET 7's Minimal API Grouping. For example, a simple API class might be:

```csharp
using WilderMinds.MinimalApiDiscovery;

namespace UsingMinimalApiDiscovery.Apis;

public class StateApi : IApi
{
  public void Register(IEndpointRouteBuilder builder)
  {
    builder.MapGet("/api/states", (StateCollection states) =>
    {
      return states;
    });
  }
}
```

Within the Register call, you can simply create your mapped APIs. But 
you can also use non-lambdas if that is easier (though I suggest static methods
to prevent usage of instance data that will become a singleton):

```csharp
using WilderMinds.MinimalApiDiscovery;

namespace UsingMinimalApiDiscovery.Apis;

public class CustomerApi : IApi
{
  public void Register(IEndpointRouteBuilder builder)
  {
    var grp = builder.MapGroup("/api/customers");
    grp.MapGet("", GetCustomers);
    grp.MapGet("", GetCustomer);
    grp.MapPost("{id:int}", SaveCustomer);
    grp.MapPut("{id:int}", UpdateCustomer);
    grp.MapDelete("{id:int}", DeleteCustomer);
  }

  // Using static methods to ensure that the class doesn't hold state
  static async Task<IResult> GetCustomers(CustomerRepository repo)
  {
    return Results.Ok(await repo.GetCustomers());
  }

  static async Task<IResult> GetCustomer(CustomerRepository repo, int id)
  {
    return Results.Ok(await repo.GetCustomer(id));
  }

  static async Task<IResult> SaveCustomer(CustomerRepository repo, Customer model)
  {
    return Results.Created($"/api/customer/{model.Id}", await repo.SaveCustomer(model));
  }

  static async Task<IResult> UpdateCustomer(CustomerRepository repo, Customer model)
  {
    return Results.Ok(await repo.UpdateCustomer(model));
  }

  static async Task<IResult> DeleteCustomer(CustomerRepository repo, int id)
  {
    var result = await repo.DeleteCustomer(id);
    if (result) return Results.Ok();
    return Results.BadRequest();
  }
}
```

In this example, I'm using a Mapping Group as well as just using methods to implement the business logic. To wire it all together there are two calls to make in your startup:

```csharp
//Program.cs
using WilderMinds.MinimalApiDiscovery;

var builder = WebApplication.CreateBuilder(args);

// ...

var app = builder.Build();

// Get all IApi dependencies and call Register on them all.
app.MapApis();

app.Run();
```

The call to `MapApis()` searches assemblies for classes that implement `IApi`. By default `MapApis()` searches all the assemblies in the AppDomain, but you can pass in your own Assemblies to limit the search if you need to:

```csharp
app.MapApis(Assembly.GetEntryAssembly());
```

If you think this is getting closer to just using Controllers, you're right. The line between this idea and controllers is pretty small but does not require a naming convention or limits the namespaces/folders to keep your APIs. You could implement the API near the Razor/Blazor pages if you want. 

