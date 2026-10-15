using AutoMapper;
using FluentValidation;
using Aplicacion.DTOs.Doctores;
using Aplicacion.Helpers;
using Aplicacion.Servicios.Interfaces;
using Dominio.Entidades.Doctores;
using Dominio.Interfaces;

namespace Aplicacion.Servicios.Implementaciones
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearDoctorDTO> _crearValidator;
        private readonly IValidator<ActualizarDoctorDTO> _actualizarValidator;

        public DoctorService(IUnitOfWork unitOfWork, IMapper mapper,
            IValidator<CrearDoctorDTO> crearValidator,
            IValidator<ActualizarDoctorDTO> actualizarValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
        }

        public async Task<ResultadoAccion<DoctorDTO>> ObtenerPorIdAsync(int id)
        {
            var doctor = await _unitOfWork.Doctores.ObtenerPorIdAsync(id);
            if (doctor == null)
                return ResultadoAccion<DoctorDTO>.Falla("Doctor no encontrado");

            return ResultadoAccion<DoctorDTO>.Exito(_mapper.Map<DoctorDTO>(doctor));
        }

        public async Task<ResultadoAccion<IEnumerable<DoctorDTO>>> ObtenerTodosAsync()
        {
            var doctores = await _unitOfWork.Doctores.ObtenerTodosAsync();
            return ResultadoAccion<IEnumerable<DoctorDTO>>.Exito(_mapper.Map<IEnumerable<DoctorDTO>>(doctores));
        }

        public async Task<ResultadoAccion<DoctorDTO>> CrearAsync(CrearDoctorDTO dto)
        {
            var validacion = await _crearValidator.ValidateAsync(dto);
            if (!validacion.IsValid)
                return ResultadoAccion<DoctorDTO>.Falla("Datos inválidos",
                    validacion.Errors.Select(e => e.ErrorMessage).ToList());

            var especialidad = await _unitOfWork.Especialidades.ObtenerPorIdAsync(dto.IdEspecialidad);
            if (especialidad == null)
                return ResultadoAccion<DoctorDTO>.Falla("La especialidad seleccionada no existe");

            if (await ExistePorEmailAsync(dto.Email))
                return ResultadoAccion<DoctorDTO>.Falla("Ya existe un doctor con ese correo");

            var doctor = _mapper.Map<Doctor>(dto);
            await _unitOfWork.Doctores.AgregarAsync(doctor);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoAccion<DoctorDTO>.Exito(_mapper.Map<DoctorDTO>(doctor), "Doctor creado exitosamente");
        }

        public async Task<ResultadoAccion<DoctorDTO>> ActualizarAsync(ActualizarDoctorDTO dto)
        {
            var validacion = await _actualizarValidator.ValidateAsync(dto);
            if (!validacion.IsValid)
                return ResultadoAccion<DoctorDTO>.Falla("Datos inválidos",
                    validacion.Errors.Select(e => e.ErrorMessage).ToList());

            var doctor = await _unitOfWork.Doctores.ObtenerPorIdAsync(dto.Id);
            if (doctor == null)
                return ResultadoAccion<DoctorDTO>.Falla("Doctor no encontrado");

            var especialidad = await _unitOfWork.Especialidades.ObtenerPorIdAsync(dto.IdEspecialidad);
            if (especialidad == null)
                return ResultadoAccion<DoctorDTO>.Falla("La especialidad seleccionada no existe");

            var duplicado = await _unitOfWork.Doctores.BuscarAsync(d => d.Email == dto.Email && d.Id != dto.Id);
            if (duplicado.Any())
                return ResultadoAccion<DoctorDTO>.Falla("Ya existe otro doctor con ese correo");

            _mapper.Map(dto, doctor);
            doctor.FechaDeModificacion = DateTime.UtcNow;

            _unitOfWork.Doctores.Actualizar(doctor);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoAccion<DoctorDTO>.Exito(_mapper.Map<DoctorDTO>(doctor), "Doctor actualizado exitosamente");
        }

        public async Task<ResultadoAccion> EliminarAsync(int id)
        {
            var doctor = await _unitOfWork.Doctores.ObtenerPorIdAsync(id);
            if (doctor == null)
                return ResultadoAccion.Falla("Doctor no encontrado");

            // Borrado lógico
            doctor.Eliminado = true;
            doctor.Activo = false;
            doctor.FechaDeEliminacion = DateTime.UtcNow;

            _unitOfWork.Doctores.Actualizar(doctor);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoAccion.Exito("Doctor eliminado (borrado lógico)");
        }

        public async Task<bool> ExistePorEmailAsync(string email)
        {
            var doctores = await _unitOfWork.Doctores.BuscarAsync(d => d.Email == email);
            return doctores.Any();
        }
    }
}
