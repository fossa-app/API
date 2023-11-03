using TIKSN.Data;

namespace Fossa.API.Core.Entities;

public record SystemPropertiesEntity(
  SystemPropertiesId ID,
  Ulid SystemID) : IEntity<SystemPropertiesId>;
