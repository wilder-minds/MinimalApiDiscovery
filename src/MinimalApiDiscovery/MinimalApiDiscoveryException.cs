using System;
using System.Runtime.Serialization;

namespace WilderMinds.MinimalApiDiscovery
{
  /// <summary>
  /// Exception thrown during Discovery of Minimal API calls
  /// </summary>
  [Serializable]
  public class MinimalApiDiscoveryException : Exception
  {
    private Exception? _ex;

    /// <summary>
    /// Empty Constructor
    /// </summary>
    public MinimalApiDiscoveryException()
    {
    }

    /// <summary>
    /// Passing in an inner exception
    /// </summary>
    /// <param name="ex">Inner Exception</param>
    public MinimalApiDiscoveryException(Exception ex)
    {
      _ex = ex;
    }

    /// <summary>
    /// Message constructor
    /// </summary>
    /// <param name="message">Why the exception was thrown</param>
    public MinimalApiDiscoveryException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Message and inner exception constructor.
    /// </summary>
    /// <param name="message">Why the exception was thrown</param>
    /// <param name="innerException">The inner exception.</param>
    public MinimalApiDiscoveryException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Serializable Exception
    /// </summary>
    /// <param name="info">The serialization type.</param>
    /// <param name="context">The streaming context.</param>
    protected MinimalApiDiscoveryException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}