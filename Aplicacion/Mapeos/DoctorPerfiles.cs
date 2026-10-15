using AutoMapper;
using Aplicacion.DTOs.Doctores;
using Dominio.Entidades.Doctores;

namespace Aplicacion.Mapeos
{
    public class DoctorPerfiles : Profile
    {
        public DoctorPerfiles()
        {
            CreateMap<CrearDoctorDTO, Doctor>().ReverseMap();
            CreateMap<ActualizarDoctorDTO, Doctor>().ReverseMap();
            CreateMap<Doctor, DoctorDTO>().ReverseMap();
        }
    }
}
