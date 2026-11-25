using FluentAssertions;
using HeroService.Admin.Logging;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace HeroService.Testing.Admin;

public sealed class BlazorLoggingConfigurationTests
{
    private const string ComponentLoggingMinimumLevelKey =
        "Serilog:MinimumLevel:Override:Microsoft.AspNetCore.Components";

    [Fact]
    public void Configure_WhenNoComponentOverrideExists_DefaultsToWarning()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

        BlazorLoggingConfiguration.Configure(configuration);

        configuration[ComponentLoggingMinimumLevelKey]
            .Should()
            .Be("Warning");
    }

    [Fact]
    public void Configure_WhenComponentOverrideExists_PreservesExplicitLevel()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [ComponentLoggingMinimumLevelKey] = "Debug"
            })
            .Build();

        BlazorLoggingConfiguration.Configure(configuration);

        configuration[ComponentLoggingMinimumLevelKey]
            .Should()
            .Be("Debug");
    }

    [Fact]
    public void SerilogConfiguration_SuppressesRendererDiagnostics_AndPreservesWarningsApplicationAndAuditEvents()
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        BlazorLoggingConfiguration.Configure(configuration);

        var sink = new CollectingSink();

        using var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .MinimumLevel.Debug()
            .WriteTo.Sink(sink)
            .CreateLogger();

        logger
            .ForContext("SourceContext", "Microsoft.AspNetCore.Components.Server.Circuits.RemoteRenderer")
            .Debug("renderer debug event");

        logger
            .ForContext("SourceContext", "Microsoft.AspNetCore.Components.Server.Circuits.RemoteRenderer")
            .Information("renderer information event");

        logger
            .ForContext("SourceContext", "Microsoft.AspNetCore.Components.Server.Circuits.RemoteRenderer")
            .Warning("renderer warning event");

        logger
            .ForContext("SourceContext", "HeroService.Admin.Components.Pages.Admin.Heroes.Index")
            .Debug("application debug event");

        logger
            .ForContext("SourceContext", "Franz.Common.Mediator.Audit")
            .Debug("Franz audit event");

        sink.Events
            .Select(logEvent => logEvent.MessageTemplate.Text)
            .Should()
            .BeEquivalentTo(
                "renderer warning event",
                "application debug event",
                "Franz audit event");
    }

    private sealed class CollectingSink : ILogEventSink
    {
        public List<LogEvent> Events { get; } = [];

        public void Emit(LogEvent logEvent)
        {
            Events.Add(logEvent);
        }
    }
}
