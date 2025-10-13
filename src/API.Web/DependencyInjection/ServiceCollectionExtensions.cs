using System.IO.Hashing;
using System.Text;
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
    var epoch = new DateTimeOffset(initialReleaseDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc));
    var idGeneratorOptions = new IdGeneratorOptions(timeSource: new DefaultTimeSource(epoch));

    var generatorId = GetGeneratorId(configuration, idGeneratorOptions.IdStructure.MaxGenerators);

    return services.AddIdGen(generatorId, () => idGeneratorOptions);
  }

  private static int GetGeneratorId(IConfiguration configuration, int maxGenerators)
  {
    var generatorIdConfiguration = configuration.GetValue<int?>("GeneratorId");

    if (generatorIdConfiguration.HasValue)
    {
      return generatorIdConfiguration.Value;
    }

    var generatorKeyConfiguration = configuration.GetValue<string?>("GeneratorKey");
    if (!string.IsNullOrWhiteSpace(generatorKeyConfiguration))
    {
      if (uint.TryParse(generatorKeyConfiguration, out uint uintKey))
      {
        return (int)(uintKey % maxGenerators);
      }
      else if (ulong.TryParse(generatorKeyConfiguration, out ulong ulongKey))
      {
        return (int)(ulongKey % (ulong)maxGenerators);
      }
      else if (Guid.TryParse(generatorKeyConfiguration, out Guid guidKey))
      {
        byte[] bytes = guidKey.ToByteArray();
        return (int)(XxHash32.HashToUInt32(bytes) % maxGenerators);
      }
      else
      {
        byte[] bytes = Encoding.UTF8.GetBytes(generatorKeyConfiguration);
        return (int)(XxHash32.HashToUInt32(bytes) % maxGenerators);
      }
    }

    throw new InvalidOperationException("GeneratorId and or GeneratorKey Configuration is missing");
  }
}
