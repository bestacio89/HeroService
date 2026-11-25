using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.GameVersions;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Domain.Heroes.Skills;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace HeroService.Admin.Components.Pages;

public sealed partial class Home : ComponentBase, IDisposable
{
    [Inject]
    private IHeroClient HeroClient { get; set; } = default!;

    [Inject]
    private ISkillClient SkillClient { get; set; } = default!;

    [Inject]
    private IGameVersionClient GameVersionClient { get; set; } = default!;

    [Inject]
    private ILogger<Home> Logger { get; set; } = default!;

    private readonly CancellationTokenSource _loadCancellation = new();
    private IReadOnlyList<HeroDto> _heroes = [];
    private IReadOnlyList<SkillDto> _skills = [];
    private IReadOnlyList<DistributionItem> _heroClassDistribution = [];
    private IReadOnlyList<DistributionItem> _skillTypeDistribution = [];
    private GameVersionDto? _activeGameVersion;
    private bool _isLoading;
    private bool _heroesLoaded;
    private bool _skillsLoaded;
    private bool _gameVersionLoaded;
    private bool _disposed;
    private string? _heroError;
    private string? _skillError;
    private string? _gameVersionError;

    protected override Task OnInitializedAsync() => LoadDashboardAsync();

    private async Task LoadDashboardAsync()
    {
        if (_isLoading || _disposed)
        {
            return;
        }

        _isLoading = true;
        _heroesLoaded = false;
        _skillsLoaded = false;
        _gameVersionLoaded = false;
        _heroError = null;
        _skillError = null;
        _gameVersionError = null;
        _heroes = [];
        _skills = [];
        _heroClassDistribution = [];
        _skillTypeDistribution = [];
        _activeGameVersion = null;

        var cancellationToken = _loadCancellation.Token;

        try
        {
            await Task.WhenAll(
                LoadHeroesAsync(cancellationToken),
                LoadSkillsAsync(cancellationToken),
                LoadActiveGameVersionAsync(cancellationToken));
        }
        finally
        {
            if (!_disposed)
            {
                _isLoading = false;
            }
        }
    }

    private async Task LoadHeroesAsync(CancellationToken cancellationToken)
    {
        try
        {
            var heroes = await HeroClient.BrowseAsync(cancellationToken: cancellationToken);
            _heroes = heroes.ToArray();
            _heroClassDistribution = BuildDistribution(
                _heroes.Select(hero => string.IsNullOrWhiteSpace(hero.Class?.Name)
                    ? "Unclassified"
                    : hero.Class.Name));
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            Logger.LogWarning(exception, "The hero catalogue request timed out while loading the admin overview.");
            _heroError = "The hero catalogue request timed out.";
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Component disposal cancels outstanding requests; no UI state needs to be updated.
        }
        catch (HttpRequestException exception)
        {
            Logger.LogWarning(exception, "The HeroService API could not provide hero catalogue data for the admin overview.");
            _heroError = "Hero catalogue data could not be loaded.";
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Unexpected failure while loading hero catalogue data for the admin overview.");
            _heroError = "An unexpected error prevented hero catalogue data from loading.";
        }
        finally
        {
            if (!_disposed)
            {
                _heroesLoaded = true;
            }
        }
    }

    private async Task LoadSkillsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var skills = await SkillClient.GetAllAsync(cancellationToken);
            _skills = skills.ToArray();
            _skillTypeDistribution = BuildDistribution(
                _skills.Select(skill => skill.SkillType.ToString()));
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            Logger.LogWarning(exception, "The skill catalogue request timed out while loading the admin overview.");
            _skillError = "The skill catalogue request timed out.";
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Component disposal cancels outstanding requests; no UI state needs to be updated.
        }
        catch (HttpRequestException exception)
        {
            Logger.LogWarning(exception, "The HeroService API could not provide skill catalogue data for the admin overview.");
            _skillError = "Skill catalogue data could not be loaded.";
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Unexpected failure while loading skill catalogue data for the admin overview.");
            _skillError = "An unexpected error prevented skill catalogue data from loading.";
        }
        finally
        {
            if (!_disposed)
            {
                _skillsLoaded = true;
            }
        }
    }

    private async Task LoadActiveGameVersionAsync(CancellationToken cancellationToken)
    {
        try
        {
            _activeGameVersion = await GameVersionClient.GetActiveAsync(cancellationToken);
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            Logger.LogWarning(exception, "The active game version request timed out while loading the admin overview.");
            _gameVersionError = "The active game version request timed out.";
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Component disposal cancels outstanding requests; no UI state needs to be updated.
        }
        catch (HttpRequestException exception)
        {
            Logger.LogWarning(exception, "The HeroService API could not provide the active game version for the admin overview.");
            _gameVersionError = "The active game version could not be loaded.";
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Unexpected failure while loading the active game version for the admin overview.");
            _gameVersionError = "An unexpected error prevented the active game version from loading.";
        }
        finally
        {
            if (!_disposed)
            {
                _gameVersionLoaded = true;
            }
        }
    }

    private static IReadOnlyList<DistributionItem> BuildDistribution(IEnumerable<string> labels)
    {
        var groups = labels
            .Where(label => !string.IsNullOrWhiteSpace(label))
            .GroupBy(label => label.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(group => new
            {
                Label = group.Key,
                Count = group.Count()
            })
            .OrderByDescending(group => group.Count)
            .ThenBy(group => group.Label, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var total = groups.Sum(group => group.Count);

        if (total == 0)
        {
            return [];
        }

        return groups
            .Select(group => new DistributionItem(
                group.Label,
                group.Count,
                group.Count * 100d / total))
            .ToArray();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _loadCancellation.Cancel();
        _loadCancellation.Dispose();
    }

    private sealed record DistributionItem(
        string Label,
        int Count,
        double Percentage);
}
