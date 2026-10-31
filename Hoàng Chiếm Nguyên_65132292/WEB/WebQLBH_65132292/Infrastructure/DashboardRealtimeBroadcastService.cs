using DbQLBH_65132292.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DbQLBH_65132292.Infrastructure;

public sealed class DashboardRealtimeBroadcastService : BackgroundService
{
    private static readonly TimeSpan BroadcastInterval = TimeSpan.FromSeconds(30);

    private readonly IHubContext<DashboardHub> _hubContext;
    private readonly ILogger<DashboardRealtimeBroadcastService> _logger;

    public DashboardRealtimeBroadcastService(
        IHubContext<DashboardHub> hubContext,
        ILogger<DashboardRealtimeBroadcastService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(BroadcastInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await _hubContext.Clients.All.SendAsync(
                    "dashboardPulse",
                    DateTimeOffset.UtcNow,
                    cancellationToken: stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Cannot push dashboard realtime pulse.");
            }
        }
    }
}
