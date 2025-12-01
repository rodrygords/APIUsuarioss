using Microsoft.EntityFrameworkCore;
using FluentValidation;
using APIUsuarios.Infrastructure.Persistence;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Infrastructure.Repositories;
using APIUsuarios.Application.Services;
using APIUsuarios.Application.Validators;
using APIUsuarios.Application.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Configurar banco de dados SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar Dependency Injection
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Configurar FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<UsuarioCreateDtoValidator>();

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ============================================
// ENDPOINTS
// ============================================

// GET /usuarios - Listar todos os usuários
app.MapGet("/usuarios", async (IUsuarioService service, CancellationToken ct) =>
{
    try
    {
        var usuarios = await service.ListarAsync(ct);
        return Results.Ok(usuarios);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
})
.WithName("ListarUsuarios")
.WithOpenApi();

// GET /usuarios/{id} - Buscar usuário por ID
app.MapGet("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    try
    {
        var usuario = await service.ObterAsync(id, ct);
        return usuario != null ? Results.Ok(usuario) : Results.NotFound(new { message = "Usuário não encontrado" });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
})
.WithName("ObterUsuario")
.WithOpenApi();

// POST /usuarios - Criar novo usuário
app.MapPost("/usuarios", async (
    UsuarioCreateDto dto,
    IUsuarioService service,
    IValidator<UsuarioCreateDto> validator,
    CancellationToken ct) =>
{
    try
    {
        // Validar DTO
        var validationResult = await validator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new
            {
                message = "Validação falhou",
                errors = validationResult.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }

        var usuario = await service.CriarAsync(dto, ct);
        return Results.Created($"/usuarios/{usuario.Id}", usuario);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
})
.WithName("CriarUsuario")
.WithOpenApi();

// PUT /usuarios/{id} - Atualizar usuário
app.MapPut("/usuarios/{id:int}", async (
    int id,
    UsuarioUpdateDto dto,
    IUsuarioService service,
    IValidator<UsuarioUpdateDto> validator,
    CancellationToken ct) =>
{
    try
    {
        // Validar DTO
        var validationResult = await validator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(new
            {
                message = "Validação falhou",
                errors = validationResult.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
            });
        }

        var usuario = await service.AtualizarAsync(id, dto, ct);
        return Results.Ok(usuario);
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { message = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { message = ex.Message });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
})
.WithName("AtualizarUsuario")
.WithOpenApi();

// DELETE /usuarios/{id} - Remover usuário (soft delete)
app.MapDelete("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    try
    {
        var removido = await service.RemoverAsync(id, ct);
        return removido ? Results.NoContent() : Results.NotFound(new { message = "Usuário não encontrado" });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
})
.WithName("RemoverUsuario")
.WithOpenApi();

app.Run();