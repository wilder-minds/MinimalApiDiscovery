using System;
using Microsoft.AspNetCore.Builder;

namespace WilderMinds.MinimalApiDiscovery;

/// <summary>
/// 
/// </summary>
public interface IApi
{
  /// <summary>
  /// 
  /// </summary>
  /// <param name="app">The WebApplication object to register the API </param>
  void Register(WebApplication app);
}
