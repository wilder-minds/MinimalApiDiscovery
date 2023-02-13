using UsingMinimalApiDiscovery.Data;
using WilderMinds.MinimalApiDiscovery;

namespace UsingMinimalApiDiscovery.Apis;

public class StateApi : IApi
{
  public void Register(WebApplication app)
  {
    app.MapGet("/api/states", (StateCollection states) =>
    {
      return states;
    });
  }
}