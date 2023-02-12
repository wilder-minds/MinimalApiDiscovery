using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WilderMinds.MinimalApiDiscovery.Tests;

public class TestServiceCollectionApis
{
  private WebApplicationBuilder _builder;

  public TestServiceCollectionApis()
  {
    _builder = WebApplication.CreateBuilder();
  }

  [Fact]
  public void TestServiceCollectionDefaults()
  {
    _builder.Services.AddApis();
    var app = _builder.Build();
    var apis = app.Services.GetServices<IApi>();
    Assert.Single(apis);
  }
}