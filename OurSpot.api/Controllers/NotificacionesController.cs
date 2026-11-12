using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OurSpot.API.DTOs;
using OurSpot.API.Hubs;

namespace OurSpot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificacionesController : ControllerBase
{
    private readonly IHubContext<NotificacionesHub> hubContext;
    private readonly IConfiguration configuration;

    public NotificacionesController(
        IHubContext<NotificacionesHub> hubContext,
        IConfiguration configuration)
    {
        this.hubContext = hubContext;
        this.configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Notificar(
        [FromBody] NotificacionEventoDto notificacion)
    {
        var claveConfigurada =
            configuration["Notificaciones:ApiKey"];

        var claveRecibida =
            Request.Headers["X-OurSpot-Key"].ToString();

        if (string.IsNullOrWhiteSpace(claveConfigurada) ||
            string.IsNullOrWhiteSpace(claveRecibida) ||
            claveRecibida != claveConfigurada)
        {
            return Unauthorized();
        }

        if (notificacion == null ||
            notificacion.EventoId <= 0 ||
            string.IsNullOrWhiteSpace(notificacion.NombreEvento) ||
            string.IsNullOrWhiteSpace(notificacion.EstadoAnterior) ||
            string.IsNullOrWhiteSpace(notificacion.EstadoNuevo))
        {
            return BadRequest("Datos de notificación inválidos.");
        }

        await hubContext.Clients.All.SendAsync(
            "RecibirNotificacion",
            notificacion);

        return Ok(new
        {
            mensaje = "Notificación enviada correctamente."
        });
    }
}
