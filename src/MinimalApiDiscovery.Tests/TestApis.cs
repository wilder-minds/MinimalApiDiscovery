using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MinimalApiDiscovery.Tests;
using WilderMinds.MinimalApiDiscovery;

namespace WilderMinds.MinimalApiDiscovery.Tests;

public class TestServiceCollectionApis
{
  private WebApplicationBuilder _builder;

  public TestServiceCollectionApis()
  {
    _builder = WebApplication.CreateBuilder();
  }

  [Fact]
  public void TestApiRegistration()
  {
    var app = _builder.Build();
    app.MapApis();
    IEndpointRouteBuilder routes = app;
    Assert.NotEmpty(routes.DataSources);
    var dataSources = routes.DataSources.ToList();
    Assert.NotEmpty(dataSources);
    foreach (var src in dataSources)
    {
      Assert.NotEmpty(src.Endpoints);
      Assert.Contains(src.Endpoints, e =>
      {
       return (e is RouteEndpoint) && ((RouteEndpoint)e).RoutePattern.RawText == "/api";
      });
    }
  }

}