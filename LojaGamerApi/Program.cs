using LojaGamerApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer; // 1. Importa o JWT
using Microsoft.IdentityModel.Tokens; // 2. Importa os Tokens
using System.Text; // 3. Importa o codificador de texto

var builder = WebApplication.CreateBuilder(args);

// --- Início da Configuração dos Serviços ---

// Pega o "endereço" do banco
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registra o "Engenheiro Chefe" (LojaGamerContext)
builder.Services.AddDbContext<LojaGamerContext>(options =>
    options.UseSqlServer(connectionString));

// ---- INÍCIO DA CONFIGURAÇÃO DO JWT (NOVO) ----
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Diz para validar quem "emitiu" o token
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        // Diz para validar para quem o token se "destina"
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        // Diz para validar o "tempo de vida" do token (se ele expirou)
        ValidateLifetime = true,

        // Diz para validar a "chave secreta" (a assinatura)
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();
// ---- FIM DA CONFIGURAÇÃO DO JWT (NOVO) ----

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Fim da Configuração dos Serviços ---

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ---- ADICIONA AUTENTICAÇÃO ANTES DA AUTORIZAÇÃO (ORDEM IMPORTANTE) ----
app.UseAuthentication(); // <-- NOVO
app.UseAuthorization();

app.MapControllers();

app.Run();