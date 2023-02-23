using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using WilderMinds.MinimalApiDiscovery;

namespace MinimalApiDiscovery.Tests;

public class BadApi : IApi
{
  private ILogger<BadApi> _logger;

  public BadApi(ILogger<BadApi> logger)
  {
    _logger = logger;
  }

  public void Register(IEndpointRouteBuilder builder)
  {
    _logger.LogInformation("Mapping APIs");
    builder.MapGet("/api/bad", Get);
  }

  static IResult Get() => Results.Ok("Works");
}
