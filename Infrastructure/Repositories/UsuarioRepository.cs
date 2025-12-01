using Microsoft.EntityFrameworkCore;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;
using APIUsuarios.Infrastructure.Persistence;

namespace APIUsuarios.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct)
    {
        return await _context.Usuarios
            .Where(u => u.Ativo)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email.ToLower(), ct);
    }

    public async Task AddAsync(Usuario usuario, CancellationToken ct)
    {
        await _context.Usuarios.AddAsync(usuario, ct);
    }

    public Task UpdateAsync(Usuario usuario, CancellationToken ct)
    {
        _context.Usuarios.Update(usuario);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Usuario usuario, CancellationToken ct)
    {
        _context.Usuarios.Update(usuario);
        return Task.CompletedTask;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.Email == email.ToLower(), ct);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        return await _context.SaveChangesAsync(ct);
    }
}