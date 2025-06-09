using FluentValidation;
using UnitGenerator;

namespace Fossa.API.Core.Entities;

[UnitOf(typeof(long), UnitGenerateOptions.Validate)]
public readonly partial struct CompanySettingsId
{
    private partial void Validate()
    {
        if (value <= 0)
        {
            throw new ValidationException("Value should be positive integer.");
        }
    }
}
