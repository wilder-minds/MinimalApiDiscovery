using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using WilderMinds.MinimalApiDiscovery;

namespace MinimalApiDiscovery.Tests;

public abstract class BaseApi : IApi
{
  public void Register(IEndpointRouteBuilder builder)
  {
    RegisterGets(builder);
  }

  public abstract IEndpointRouteBuilder RegisterGets(IEndpointRouteBuilder builder);
}
