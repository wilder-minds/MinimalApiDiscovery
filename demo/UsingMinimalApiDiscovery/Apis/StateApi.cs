using UsingMinimalApiDiscovery.Data;
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