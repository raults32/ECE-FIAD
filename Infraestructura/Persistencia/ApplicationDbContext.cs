using Dominio.Entidades.Citas;
using Dominio.Entidades.Doctores;
using Dominio.Entidades.Especialidades;
using Dominio.Entidades.Evoluciones;
using Dominio.Entidades.HistoriasClinicas;
using Dominio.Entidades.Pacientes;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Infraestructura.Persistencia
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Tablas principales (DbSet)
        public DbSet<Paciente> Pacientes => Set<Paciente>();
        public DbSet<HistoriaClinica> HistoriasClinicas => Set<HistoriaClinica>();
        public DbSet<Doctor> Doctores => Set<Doctor>();
        public DbSet<Especialidad> Especialidades => Set<Especialidad>();
        public DbSet<Cita> Citas => Set<Cita>();
        public DbSet<Evolucion> Evoluciones => Set<Evolucion>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- Configuración guiada: Paciente ----
            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.ToTable("Pacientes");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Nombres).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Apellidos).IsRequired().HasMaxLength(100);
                entity.Property(p => p.NumeroDocumento).IsRequired().HasMaxLength(20);
                entity.HasIndex(p => p.NumeroDocumento).IsUnique();
            });

            // Relación 1 a 1: Paciente <-> HistoriaClinica
            modelBuilder.Entity<Paciente>()
                .HasOne(p => p.HistoriaClinica)
                .WithOne(h => h.Paciente)
                .HasForeignKey<HistoriaClinica>(h => h.IdPaciente)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- 1. Especialidad <-> Doctor (1:N) ----
            modelBuilder.Entity<Especialidad>(entity =>
            {
                entity.ToTable("Especialidades");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.ToTable("Doctores");
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Nombres).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Apellidos).IsRequired().HasMaxLength(100);
                entity.Property(d => d.Email).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.Especialidad)
                .WithMany(e => e.Doctores)
                .HasForeignKey(d => d.IdEspecialidad);

            // ---- 2. Cita <-> Paciente y Cita <-> Doctor (dos relaciones 1:N) ----
            modelBuilder.Entity<Cita>(entity =>
            {
                entity.ToTable("Citas");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Motivo).HasMaxLength(500);
                entity.Property(c => c.Notas).HasMaxLength(1000);
            });

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Paciente)
                .WithMany(p => p.Citas)
                .HasForeignKey(c => c.IdPaciente);

            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Doctor)
                .WithMany(d => d.Citas)
                .HasForeignKey(c => c.IdDoctor);

            // ---- 3. Evolucion <-> HistoriaClinica (1:N) ----
            modelBuilder.Entity<HistoriaClinica>(entity =>
            {
                entity.ToTable("HistoriasClinicas");
                entity.HasKey(h => h.Id);
            });

            modelBuilder.Entity<Evolucion>(entity =>
            {
                entity.ToTable("Evoluciones");
                entity.HasKey(ev => ev.Id);
                entity.Property(ev => ev.Diagnostico).HasMaxLength(500);
                entity.Property(ev => ev.Tratamiento).HasMaxLength(500);
                entity.Property(ev => ev.Notas).HasMaxLength(1000);
            });

            modelBuilder.Entity<Evolucion>()
                .HasOne(ev => ev.HistoriaClinica)
                .WithMany(h => h.Evoluciones)
                .HasForeignKey(ev => ev.IdHistoriaClinica);

            // Relación extra (no está en HistoriaClinica ni en Doctor como colección,
            // pero Evolucion sí referencia a Doctor — se configura sin colección inversa)
            modelBuilder.Entity<Evolucion>()
                .HasOne(ev => ev.Doctor)
                .WithMany()
                .HasForeignKey(ev => ev.IdDoctor);
        }
    }
}