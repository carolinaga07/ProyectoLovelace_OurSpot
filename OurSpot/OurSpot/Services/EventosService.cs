using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Models;
using System.Linq.Expressions;

namespace OurSpot.Components.Pages.AdministradorPages.EventosPages.Services
{
    public class EventosService(
        IDbContextFactory<Contexto> contextFactory)
        : IService<Evento, int>
    {
        private static void ActualizarEstado(
            Evento evento,
            DateTime ahora)
        {
            // Conservar los eventos cancelados.
            if (string.Equals(
                evento.Estado,
                "Cancelado",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var inicio = evento.FechaInicio
                .ToDateTime(evento.HoraInicio);

            var cierre = evento.FechaCierre
                .ToDateTime(evento.HoraCierre);

            if (ahora < inicio)
            {
                evento.Estado = "Pendiente";
            }
            else if (ahora < cierre)
            {
                evento.Estado = "Activo";
            }
            else
            {
                evento.Estado = "Finalizado";
            }
        }

        public async Task<bool> Guardar(Evento eventos)
        {
            await using var contexto =
                await contextFactory.CreateDbContextAsync();

            var inicio = eventos.FechaInicio
                .ToDateTime(eventos.HoraInicio);

            var cierre = eventos.FechaCierre
                .ToDateTime(eventos.HoraCierre);

            if (cierre < inicio)
                return false;

            ActualizarEstado(eventos, DateTime.Now);

            if (eventos.EventoId == 0)
            {
                contexto.Eventos.Add(eventos);
            }
            else
            {
                contexto.Update(eventos);
            }

            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<Evento?> Buscar(int eventoId)
        {
            await using var contexto =
                await contextFactory.CreateDbContextAsync();

            var evento = await contexto.Eventos
                .Include(e => e.EventosDetalles)
                .FirstOrDefaultAsync(
                    e => e.EventoId == eventoId);

            if (evento is null)
                return null;

            ActualizarEstado(evento, DateTime.Now);

            // Persistir el cambio de estado, si lo hay.
            await contexto.SaveChangesAsync();

            return evento;
        }

        public async Task<List<Evento>> GetList(
            Expression<Func<Evento, bool>> criterio)
        {
            await using var contexto =
                await contextFactory.CreateDbContextAsync();

            // Actualizar antes de filtrar por estado.
            var eventos = await contexto.Eventos
                .Where(e => !e.Eliminado)
                .ToListAsync();

            var ahora = DateTime.Now;

            foreach (var evento in eventos)
            {
                ActualizarEstado(evento, ahora);
            }

            await contexto.SaveChangesAsync();

            return await contexto.Eventos
                .Where(criterio)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> Eliminar(int idEvento)
        {
            await using var contexto =
                await contextFactory.CreateDbContextAsync();

            var evento = await contexto.Eventos
                .FirstOrDefaultAsync(
                    e => e.EventoId == idEvento
                      && !e.Eliminado);

            if (evento is null)
                return false;

            evento.Eliminado = true;

            return await contexto.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExisteNombre(
            string nombre,
            int eventoId = 0)
        {
            await using var contexto =
                await contextFactory.CreateDbContextAsync();

            return await contexto.Eventos
                .AnyAsync(
                    e => e.Nombre == nombre
                      && e.EventoId != eventoId);
        }

        public async Task<bool> Recuperar(int idEvento)
        {
            await using var contexto =
                await contextFactory.CreateDbContextAsync();

            var evento = await contexto.Eventos
                .FirstOrDefaultAsync(
                    e => e.EventoId == idEvento
                      && e.Eliminado);

            if (evento is null)
                return false;

            evento.Eliminado = false;

            ActualizarEstado(evento, DateTime.Now);

            return await contexto.SaveChangesAsync() > 0;
        }
    }
}