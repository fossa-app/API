﻿using System.Runtime.Serialization;

namespace Fossa.API.Core.Messages;

[Serializable]
public class CrossTenantInboundUnauthorizedAccessException : CrossTenantUnauthorizedAccessException
{
  public CrossTenantInboundUnauthorizedAccessException()
  { }

  public CrossTenantInboundUnauthorizedAccessException(string message) : base(message)
  {
  }

  public CrossTenantInboundUnauthorizedAccessException(string message, Exception inner) : base(message, inner)
  {
  }

  protected CrossTenantInboundUnauthorizedAccessException(
    SerializationInfo info,
    StreamingContext context) : base(info, context) { }
}
