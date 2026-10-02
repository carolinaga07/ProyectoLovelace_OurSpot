using Microsoft.EntityFrameworkCore;
using OurSpot.Models;

namespace OurSpot.Context
{
    public class Contexto: DbContext
    {
        public Contexto(DbContextOptions<Contexto> options): base(options)
        {

        }

         public DbSet<Evento> Eventos{ get; set; } 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
