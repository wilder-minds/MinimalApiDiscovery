using System;
using Microsoft.AspNetCore.Builder;

namespace WilderMinds.MinimalApiDiscovery;

public interface IApi
{
  void Register(WebApplication app);
}
