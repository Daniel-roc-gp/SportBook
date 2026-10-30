
using Microsoft.EntityFrameworkCore;
using SportBook.Core.Interfaces;
using SportBook.Infrastructure.Data;
using SportBook.Infrastructure.Mappings;
using SportBook.Infrastructure.Repositories;
using SportBook.Services.Interfaces;
using SportBook.Services.Services;
using SportBook.Services.Validators;

namespace SportBook.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configurar la BD MySql

            var connectionString =
                builder.Configuration.GetConnectionString("ConnectionMySql");

            builder.Services.AddDbContext<SportBookDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)));

            #endregion

            // Registrar servicios
            builder.Services.AddTransient<IReservaService, ReservaService>();
            builder.Services.AddTransient<ICanchaService, CanchaService>();
            builder.Services.AddTransient<IUsuarioService, UsuarioService>();

            // Registrar repositorio generico
            builder.Services.AddScoped(
                typeof(IBaseRepository<>),
                typeof(BaseRepository<>));

            // Registrar repositorios especializados
            builder.Services.AddTransient<ICanchaRepository, CanchaRepository>();
            builder.Services.AddTransient<IReservaRepository, ReservaRepository>();
            builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();

            // Add services to the container.
            builder.Services.AddControllers();

            // Configurar autorizacion como en el ejemplo del magister
            builder.Services.AddAuthorization();

            // Configurar OpenAPI
            builder.Services.AddOpenApi();

            // Registrar AutoMapper
            builder.Services.AddAutoMapper(typeof(CanchaProfile).Assembly);

            // Registrar validadores de FluentValidation
            builder.Services.AddScoped<CrearReservaDtoValidator>();
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
