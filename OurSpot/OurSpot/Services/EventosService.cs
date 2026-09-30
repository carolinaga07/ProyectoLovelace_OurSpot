using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using OurSpot.Context;
using OurSpot.Models;
using System.Linq.Expressions;

namespace OurSpot.Components.Pages.AdministradorPages.EventosPages
{
    namespace Services
    {
        public class EventosService(IDbContextFactory<Contexto> contextFactory)
            : IService<Evento, int>
        {
            public async Task<bool> Guardar(Evento eventos)
            {
                await using var contexto = await contextFactory.CreateDbContextAsync();

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

            private async Task<bool> Existe(int eventoId)
            {
                await using var contexto = await contextFactory.CreateDbContextAsync();

                return await contexto.Eventos
                    .AnyAsync(e => e.EventoId == eventoId);
            }

            private async Task<bool> Insertar(Evento eventos)
            {
                await using var contexto = await contextFactory.CreateDbContextAsync();

                contexto.Eventos.Add(eventos);

                return await contexto.SaveChangesAsync() > 0;
            }

            private async Task<bool> Modificar(Evento evento)
            {
                await using var contexto = await contextFactory.CreateDbContextAsync();

                contexto.Update(evento);

                return await contexto.SaveChangesAsync() > 0;
            }

            public async Task<Evento?> Buscar(int eventoId)
            {
                await using var contexto = await contextFactory.CreateDbContextAsync();

                return await contexto.Eventos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EventoId == eventoId);
            }

            public async Task<bool> Eliminar(int eventoId)
            {
                await using var contexto = await contextFactory.CreateDbContextAsync();

                var evento = await contexto.Eventos.FindAsync(eventoId);

                if (evento == null)
                {
                    return false;
                }

                contexto.Eventos.Remove(evento);

                return await contexto.SaveChangesAsync() > 0;
            }

            public async Task<List<Evento>> GetList(
                Expression<Func<Evento, bool>> criterio)
            {
                await using var contexto = await contextFactory.CreateDbContextAsync();

                return await contexto.Eventos
                    .Where(criterio)
                    .AsNoTracking()
                    .ToListAsync();
            }

            public async Task<bool> ExisteNombre(string nombre, int eventoId = 0)
            {
                await using var contexto = await contextFactory.CreateDbContextAsync();

                return await contexto.Eventos
                    .AnyAsync(e => e.Nombre == nombre && e.EventoId != eventoId);
            }
        }
    }
}
