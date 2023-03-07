using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

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
      foreach (var endpoint  in src.Endpoints)
      {
        Assert.IsAssignableFrom<RouteEndpoint>(endpoint);
        Assert.Equal("/api", ((RouteEndpoint)endpoint).RoutePattern.RawText);
      }
    }
  }

}