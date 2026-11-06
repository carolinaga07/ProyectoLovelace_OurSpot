
using OurSpot.Components.Pages.AdministradorPages.EventosPages.Services;

namespace OurSpot.Services;

public class EventosBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<EventosBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(
            TimeSpan.FromSeconds(5));

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await RevisarEventos();

                if (!await timer.WaitForNextTickAsync(stoppingToken))
                    break;
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
        }
    }

    private async Task RevisarEventos()
    {
        try
        {
            using var scope = scopeFactory.CreateScope();

            var eventosService = scope.ServiceProvider
                .GetRequiredService<EventosService>();

            await eventosService.RevisarEstadosPendientes();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error al revisar los estados pendientes.");
        }
    }
}
