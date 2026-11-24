
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace OurSpot.Hubs;

[Authorize]
public class NotificacionesHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var usuario = Context.User;

        if (usuario?.Identity?.IsAuthenticated != true)
        {
            Context.Abort();
            return;
        }

        if (usuario.IsInRole("Administrador"))
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                "Administradores");
        }

        await base.OnConnectedAsync();
    }
}
