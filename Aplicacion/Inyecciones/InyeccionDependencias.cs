using Aplicacion.Servicios.Interfaces;
using Aplicacion.Servicios.Implementaciones;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Aplicacion.Inyecciones
{
    public static class InyeccionDependencias
    {
        public static IServiceCollection AgregarAplicacion(this IServiceCollection services)
        {
            // AutoMapper: escanea y registra todos los Profiles del ensamblado
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // FluentValidation: registra todos los validadores del ensamblado actual
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Servicios de aplicación con ciclo de vida Scoped
            services.AddScoped<IPacienteService, PacienteService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<ICitaService, CitaService>();

            return services;
        }
    }
}