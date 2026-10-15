using AutoMapper;
using Aplicacion.DTOs.Citas;
using Dominio.Entidades.Citas;

namespace Aplicacion.Mapeos
{
    public class CitaPerfiles : Profile
    {
        public CitaPerfiles()
        {
            CreateMap<CrearCitaDTO, Cita>().ReverseMap();
            CreateMap<ActualizarCitaDTO, Cita>().ReverseMap();
            CreateMap<Cita, CitaDTO>().ReverseMap();
        }
    }
}
