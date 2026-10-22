using Microsoft.EntityFrameworkCore;
using OurSpot.Components.Models;

namespace OurSpot.Context
{
    public class Contexto: DbContext
    {
        public Contexto(DbContextOptions<Contexto> options): base(options)
        {

        }

        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Contrataciones> Contrataciones { get; set; }
        public DbSet<Lugares> Lugares { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Evento>().HasQueryFilter(e => !e.Eliminado);
            modelBuilder.Entity<Contrataciones>().HasQueryFilter(c => !c.Eliminado);
            modelBuilder.Entity<Lugares>().HasQueryFilter(l => !l.Eliminado);
        }
    }
}
