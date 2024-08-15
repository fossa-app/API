using System.Globalization;
using Microsoft.Extensions.Hosting;
using EnvironmentName = TIKSN.Deployment.EnvironmentName;

namespace Fossa.API.Core.Extensions;

//
// Summary:
//     Extension methods for Microsoft.Extensions.Hosting.IHostEnvironment.
public static class HostEnvironmentExtensions
{
  //
  // Summary:
  //     Checks if the current host environment name is Microsoft.Extensions.Hosting.Environments.Development.
  //
  //
  // Parameters:
  //   hostEnvironment:
  //     An instance of Microsoft.Extensions.Hosting.IHostEnvironment.
  //
  // Returns:
  //     True if the environment name matches to Microsoft.Extensions.Hosting.Environments.Development,
  //     otherwise false.
  public static bool MatchesDevelopment(this IHostEnvironment hostEnvironment)
  {
    return hostEnvironment.MatchesEnvironment(Environments.Development);
  }

  //
  // Summary:
  //     Compares the current host environment name against the specified value.
  //
  // Parameters:
  //   hostEnvironment:
  //     An instance of Microsoft.Extensions.Hosting.IHostEnvironment.
  //
  //   environmentName:
  //     Environment name to validate against.
  //
  // Returns:
  //     True if the current environment matches to the specified name, otherwise
  //     false.
  public static bool MatchesEnvironment(this IHostEnvironment hostEnvironment, string environmentName)
  {
    var specifiedEnvironmentName = EnvironmentName.Parse(hostEnvironment.EnvironmentName, asciiOnly: true, CultureInfo.InvariantCulture);
    return specifiedEnvironmentName.Match(hostEnvironment.MatchesEnvironment, None: false);
  }

  //
  // Summary:
  //     Compares the current host environment name against the specified value.
  //
  // Parameters:
  //   hostEnvironment:
  //     An instance of Microsoft.Extensions.Hosting.IHostEnvironment.
  //
  //   environmentName:
  //     Environment name to validate against.
  //
  // Returns:
  //     True if the current environment matches to the specified name, otherwise
  //     false.
  public static bool MatchesEnvironment(this IHostEnvironment hostEnvironment, EnvironmentName environmentName)
  {
    var hostEnvironmentName = EnvironmentName.Parse(hostEnvironment.EnvironmentName, asciiOnly: true, CultureInfo.InvariantCulture);
    return hostEnvironmentName.Match(e => e.Matches(environmentName), None: false);
  }

  //
  // Summary:
  //     Checks if the current host environment name is Microsoft.Extensions.Hosting.Environments.Production.
  //
  //
  // Parameters:
  //   hostEnvironment:
  //     An instance of Microsoft.Extensions.Hosting.IHostEnvironment.
  //
  // Returns:
  //     True if the environment name matches to Microsoft.Extensions.Hosting.Environments.Production,
  //     otherwise false.
  public static bool MatchesProduction(this IHostEnvironment hostEnvironment)
  {
    return hostEnvironment.MatchesEnvironment(Environments.Production);
  }

  //
  // Summary:
  //     Checks if the current host environment name is Microsoft.Extensions.Hosting.Environments.Staging.
  //
  //
  // Parameters:
  //   hostEnvironment:
  //     An instance of Microsoft.Extensions.Hosting.IHostEnvironment.
  //
  // Returns:
  //     True if the environment name matches to Microsoft.Extensions.Hosting.Environments.Staging,
  //     otherwise false.
  public static bool MatchesStaging(this IHostEnvironment hostEnvironment)
  {
    return hostEnvironment.MatchesEnvironment(Environments.Staging);
  }
}
