using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OurSpot.Components.Models;
using OurSpot.Models;
using OurSpot.Security;

namespace OurSpot.Context;

public class Contexto: IdentityDbContext<ApplicationUser>
{
    public Contexto(DbContextOptions<Contexto> options): base(options)
    {

    }

    public DbSet<Evento> Eventos { get; set; }
    public DbSet<NotificacionEvento> NotificacionesEventos { get; set; }
    public DbSet<Contrataciones> Contrataciones { get; set; }
    public DbSet<Lugares> Lugares { get; set; }
    public DbSet<Proveedores> Proveedores { get; set; }
    public DbSet<Personal> Personales { get; set; }
    public DbSet<Clientes> Clientes { get; set; }
    public DbSet<EventoImagen> EventoImagenes { get; set; }
    public DbSet<Auditoria> Auditorias { get; set; }
    public DbSet<CalificacionEvento> CalificacionEventos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CalificacionEvento>()
       .HasIndex(c => new { c.EventoId, c.ClienteId })
       .IsUnique();

        modelBuilder.Entity<CalificacionEvento>()
            .HasOne(c => c.Evento)
            .WithMany(e => e.calificacionEventos)
            .HasForeignKey(c => c.EventoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CalificacionEvento>()
            .HasOne(c => c.Clientes)
            .WithMany()
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
