using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using WilderMinds.MinimalApiDiscovery;

namespace MinimalApiDiscovery.Tests;

public class DerivedApi : BaseApi
{
  public override IEndpointRouteBuilder RegisterGets(IEndpointRouteBuilder builder)
  {
    builder.MapGet("/api/foo", Get);
    return builder;
  }

  static IResult Get() => Results.Ok("Works");
}
