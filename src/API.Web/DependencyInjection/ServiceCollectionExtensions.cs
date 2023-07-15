using IdGen;
using IdGen.DependencyInjection;

namespace Fossa.API.Web.DependencyInjection;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddIdGen(
    this IServiceCollection services,
    IConfiguration configuration,
    DateOnly initialReleaseDate)
  {
    var epoch = initialReleaseDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
    var idGeneratorOptions = new IdGeneratorOptions(timeSource: new DefaultTimeSource(epoch));

    var generatorId = GetGeneratorId(configuration);

    return services.AddIdGen(generatorId, () => idGeneratorOptions);
  }

  private static int GetGeneratorId(IConfiguration configuration)
  {
    var generatorIdConfiguration = configuration.GetValue<int?>("GeneratorId");

    if (generatorIdConfiguration.HasValue)
    {
      return generatorIdConfiguration.Value;
    }

    throw new InvalidOperationException("GeneratorId Configuration is missing");
  }
}
