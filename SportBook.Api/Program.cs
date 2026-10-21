
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SportBook.Core.Interfaces;
using SportBook.Infrastructure.Data;
using SportBook.Infrastructure.Mappings;
using SportBook.Infrastructure.Repositories;
using SportBook.Services.Validators;
using System.Security.Claims;
using System.Text;

namespace SportBook.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configurar la BD MySql
            var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");

            builder.Services.AddDbContext<SportBookDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)));
            #endregion

            // Registrar repositorios
            builder.Services.AddTransient<ICanchaRepository, CanchaRepository>();
            builder.Services.AddTransient<IReservaRepository, ReservaRepository>();
            builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();

            // Add services to the container.
            builder.Services.AddControllers();

            // Configurar OpenAPI
            builder.Services.AddOpenApi();

            // Registrar AutoMapper
            builder.Services.AddAutoMapper(typeof(CanchaProfile).Assembly);

            // Registrar validadores de FluentValidation
            builder.Services.AddScoped<CrearReservaDtoValidator>();
            builder.Services.AddScoped<RegistrarUsuarioDtoValidator>();
            builder.Services.AddScoped<LoginDtoValidator>();

            var jwtKey = builder.Configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(jwtKey) ||
                Encoding.UTF8.GetByteCount(jwtKey) < 32)
            {
                throw new InvalidOperationException(
                    "Configura Jwt:Key con al menos 32 bytes.");
            }

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey)),
                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.Name
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddScoped<RegistrarUsuarioDtoValidator>();
            builder.Services.AddScoped<LoginDtoValidator>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
