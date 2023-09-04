using TIKSN.Data;

namespace Fossa.API.Core.Entities;

public record SystemPropertiesEntity(
  long ID,
  Ulid SystemID) : IEntity<long>;
