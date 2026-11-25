using Microsoft.Extensions.Configuration;

namespace HeroService.Admin.Logging;

public static class BlazorLoggingConfiguration
{
    private const string ComponentLoggingMinimumLevelKey =
        "Serilog:MinimumLevel:Override:Microsoft.AspNetCore.Components";

    private const string DefaultComponentLoggingMinimumLevel = "Warning";

    public static void Configure(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (string.IsNullOrWhiteSpace(configuration[ComponentLoggingMinimumLevelKey]))
        {
            configuration[ComponentLoggingMinimumLevelKey] =
                DefaultComponentLoggingMinimumLevel;
        }
    }
}
