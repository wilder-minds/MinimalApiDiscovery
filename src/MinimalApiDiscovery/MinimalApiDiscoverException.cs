using System;
using System.Runtime.Serialization;

namespace WilderMinds.MinimalApiDiscovery
{
  [Serializable]
  internal class MinimalApiDiscoverException : Exception
  {
    private Exception? _ex;

    public MinimalApiDiscoverException()
    {
    }

    public MinimalApiDiscoverException(Exception ex)
    {
      _ex = ex;
    }

    public MinimalApiDiscoverException(string? message) : base(message)
    {
    }

    public MinimalApiDiscoverException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected MinimalApiDiscoverException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}