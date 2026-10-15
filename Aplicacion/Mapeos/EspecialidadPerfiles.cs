using AutoMapper;
using Aplicacion.DTOs.Especialidades;
using Dominio.Entidades.Especialidades;

namespace Aplicacion.Mapeos
{
    public class EspecialidadPerfiles : Profile
    {
        public EspecialidadPerfiles()
        {
            CreateMap<CrearEspecialidadDTO, Especialidad>().ReverseMap();
            CreateMap<Especialidad, EspecialidadDTO>().ReverseMap();
        }
    }
}
