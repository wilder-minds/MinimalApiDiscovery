using System;
using Microsoft.AspNetCore.Builder;

namespace WilderMinds.MinimalApiDiscovery;

/// <summary>
/// An interface for Identifying and registering APIs
/// </summary>
public interface IApi
{
  /// <summary>
  /// This is automatically called by the library to add your APIs
  /// </summary>
  /// <param name="app">The WebApplication object to register the API </param>
  void Register(WebApplication app);
}
