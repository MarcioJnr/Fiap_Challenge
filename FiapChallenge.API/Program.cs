using FiapChallenge.API.Authentication;
using FiapChallenge.Application.Services;
using FiapChallenge.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração de serviços
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAlunoService, AlunoService>();
builder.Services.AddScoped<ITurmaService, TurmaService>();
builder.Services.AddScoped<IMatriculaService, MatriculaService>();

builder.Services.AddControllers();

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FIAP - Sistema de Gestão Acadêmica",
        Version = "v1",
        Description = @"API RESTful para gerenciamento de alunos, turmas e matrículas

**Como usar a autenticação:**
1. Faça login em POST /api/v1/Auth/login para obter o token JWT
2. Clique no botão 'Authorize' 🔓 no canto superior direito
3. Cole o token (apenas o token, SEM 'Bearer')
4. Clique em 'Authorize' e depois 'Close'
5. Agora todas as requisições incluirão o token automaticamente"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header. 
                      
**Como usar:** 
1. Faça login em /api/v1/Auth/login
2. Copie o token retornado
3. Clique no botão 'Authorize' acima
4. Cole APENAS o token (sem 'Bearer')
5. Clique em 'Authorize'

Exemplo de token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAuthentication("JwtCustom")
    .AddScheme<AuthenticationSchemeOptions, JwtAuthenticationHandler>("JwtCustom", null);

builder.Services.AddAuthorization();

var app = builder.Build();

// Configuração do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FIAP API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
