using Microsoft.EntityFrameworkCore;
using APIUsuarios.Domain.Entities;

namespace APIUsuarios.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.Property(e => e.Senha)
                .IsRequired();

            entity.Property(e => e.DataNascimento)
                .IsRequired();

            entity.Property(e => e.Telefone)
                .HasMaxLength(20);

            entity.Property(e => e.Ativo)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.DataCriacao)
                .IsRequired();
        });
    }
}