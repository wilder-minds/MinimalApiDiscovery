using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using WilderMinds.MinimalApiDiscovery;

namespace MinimalApiDiscovery.Tests;

public class DumbApi : IApi
{
  public void Register(WebApplication app)
  {
    app.MapGet("/api", () => "Works");
  }


}
