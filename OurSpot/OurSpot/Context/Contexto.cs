
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OurSpot.Models;
using OurSpot.Security;

namespace OurSpot.Context;

public class Contexto : IdentityDbContext<ApplicationUser>
{
    public Contexto(DbContextOptions<Contexto> options)
        : base(options)
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

    // Nuevo DbSet
    public DbSet<EventoDetalles> EventosDetalles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // CALIFICACIONES (configuración existente)

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

        // PROVEEDORES - APPLICATIONUSER

        modelBuilder.Entity<Proveedores>()
            .HasOne(p => p.ApplicationUser)
            .WithMany()
            .HasForeignKey(p => p.ApplicationUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Proveedores>()
            .HasIndex(p => p.ApplicationUserId)
            .IsUnique();

        // PROVEEDORES - CONTRATACIONES

        modelBuilder.Entity<Contrataciones>()
            .HasOne(c => c.ProveedorEntidad)
            .WithMany(p => p.Contrataciones)
            .HasForeignKey(c => c.ProveedorId)
            .OnDelete(DeleteBehavior.Restrict);

        // EVENTOS - EVENTODETALLES

        modelBuilder.Entity<EventoDetalles>()
            .HasOne(d => d.Evento)
            .WithMany(e => e.EventosDetalles)
            .HasForeignKey(d => d.EventoId)
            .OnDelete(DeleteBehavior.Restrict);

        // CONTRATACIONES - EVENTODETALLES

        modelBuilder.Entity<EventoDetalles>()
            .HasOne(d => d.Contrataciones)
            .WithMany()
            .HasForeignKey(d => d.ContratacionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
