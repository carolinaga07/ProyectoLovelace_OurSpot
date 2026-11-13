
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Models;

namespace OurSpot.Services
{
    public class CalificacionesService(
        IDbContextFactory<Contexto> contextFactory)
    {
        public async Task<bool> GuardarCalificacion(
            int eventoId,
            string applicationUserId,
            int estrellas)
        {
            if (estrellas < 1 || estrellas > 5 ||
                string.IsNullOrWhiteSpace(applicationUserId))
                return false;

            await using var contexto =
                await contextFactory.CreateDbContextAsync();

            // Buscar el evento y verificar su propietario.
            var evento = await contexto.Eventos
                .FirstOrDefaultAsync(e =>
                    e.EventoId == eventoId &&
                    !e.Eliminado &&
                    e.Estado == "Finalizado" &&
                    e.Clientes != null &&
                    e.Clientes.ApplicationUserId == applicationUserId);

            if (evento == null || evento.ClienteId == null)
                return false;

            // Buscar una calificación anterior.
            var calificacion = await contexto.CalificacionEventos
                .FirstOrDefaultAsync(c =>
                    c.EventoId == eventoId &&
                    c.ClienteId == evento.ClienteId.Value);

            if (calificacion == null)
            {
                calificacion = new CalificacionEvento
                {
                    EventoId = eventoId,
                    ClienteId = evento.ClienteId.Value,
                    Estrellas = estrellas,
                    FechaCalificacion = DateTime.UtcNow
                };

                contexto.CalificacionEventos.Add(calificacion);
            }
            else
            {
                // El propietario puede modificar su puntuación.
                calificacion.Estrellas = estrellas;
                calificacion.FechaCalificacion = DateTime.UtcNow;
            }

            await contexto.SaveChangesAsync();
            return true;
        }

        public async Task<int?> ObtenerCalificacion(
            int eventoId,
            string applicationUserId)
        {
            if (string.IsNullOrWhiteSpace(applicationUserId))
                return null;

            await using var contexto =
                await contextFactory.CreateDbContextAsync();

            return await contexto.CalificacionEventos
                .Where(c =>
                    c.EventoId == eventoId &&
                    c.Evento.Clientes != null &&
                    c.Evento.Clientes.ApplicationUserId
                        == applicationUserId)
                .Select(c => (int?)c.Estrellas)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> PuedeCalificar(
            int eventoId,
            string applicationUserId)
        {
            if (string.IsNullOrWhiteSpace(applicationUserId))
                return false;

            await using var contexto =
                await contextFactory.CreateDbContextAsync();

            return await contexto.Eventos.AnyAsync(e =>
                e.EventoId == eventoId &&
                !e.Eliminado &&
                e.Estado == "Finalizado" &&
                e.Clientes != null &&
                e.Clientes.ApplicationUserId == applicationUserId);
        }
    }
}


