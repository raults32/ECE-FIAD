using AutoMapper;
using Aplicacion.DTOs.Pacientes;
using Dominio.Entidades.Pacientes;

namespace Aplicacion.Mapeos
{
    public class PacientePerfiles : Profile
    {
        public PacientePerfiles()
        {
            // Creación: DTO -> Entidad
            CreateMap<CrearPacienteDTO, Paciente>().ReverseMap();

            // Actualización: DTO -> Entidad
            CreateMap<ActualizarPacienteDTO, Paciente>().ReverseMap();

            // Consulta: Entidad -> DTO
            CreateMap<Paciente, PacienteDTO>().ReverseMap();
        }
    }
}