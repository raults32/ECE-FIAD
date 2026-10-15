using AutoMapper;
using FluentValidation;
using Aplicacion.DTOs.Citas;
using Aplicacion.Helpers;
using Aplicacion.Servicios.Interfaces;
using Dominio.Entidades.Citas;
using Dominio.Enumeraciones;
using Dominio.Interfaces;

namespace Aplicacion.Servicios.Implementaciones
{
    public class CitaService : ICitaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearCitaDTO> _crearValidator;
        private readonly IValidator<ActualizarCitaDTO> _actualizarValidator;

        public CitaService(IUnitOfWork unitOfWork, IMapper mapper,
            IValidator<CrearCitaDTO> crearValidator,
            IValidator<ActualizarCitaDTO> actualizarValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _crearValidator = crearValidator;
            _actualizarValidator = actualizarValidator;
        }

        public async Task<ResultadoAccion<CitaDTO>> ObtenerPorIdAsync(int id)
        {
            var cita = await _unitOfWork.Citas.ObtenerPorIdAsync(id);
            if (cita == null)
                return ResultadoAccion<CitaDTO>.Falla("Cita no encontrada");

            return ResultadoAccion<CitaDTO>.Exito(_mapper.Map<CitaDTO>(cita));
        }

        public async Task<ResultadoAccion<IEnumerable<CitaDTO>>> ObtenerTodosAsync()
        {
            var citas = await _unitOfWork.Citas.ObtenerTodosAsync();
            return ResultadoAccion<IEnumerable<CitaDTO>>.Exito(_mapper.Map<IEnumerable<CitaDTO>>(citas));
        }

        public async Task<ResultadoAccion<CitaDTO>> CrearAsync(CrearCitaDTO dto)
        {
            var validacion = await _crearValidator.ValidateAsync(dto);
            if (!validacion.IsValid)
                return ResultadoAccion<CitaDTO>.Falla("Datos inválidos",
                    validacion.Errors.Select(e => e.ErrorMessage).ToList());

            if (await _unitOfWork.Pacientes.ObtenerPorIdAsync(dto.IdPaciente) == null)
                return ResultadoAccion<CitaDTO>.Falla("El paciente seleccionado no existe");

            if (await _unitOfWork.Doctores.ObtenerPorIdAsync(dto.IdDoctor) == null)
                return ResultadoAccion<CitaDTO>.Falla("El doctor seleccionado no existe");

            if (!await DoctorDisponibleAsync(dto.IdDoctor, dto.FechaHora))
                return ResultadoAccion<CitaDTO>.Falla("El doctor no está disponible en ese horario");

            var cita = _mapper.Map<Cita>(dto);
            cita.Estado = EstadoCita.Pendiente;

            await _unitOfWork.Citas.AgregarAsync(cita);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoAccion<CitaDTO>.Exito(_mapper.Map<CitaDTO>(cita), "Cita creada exitosamente");
        }

        public async Task<ResultadoAccion<CitaDTO>> ActualizarAsync(ActualizarCitaDTO dto)
        {
            var validacion = await _actualizarValidator.ValidateAsync(dto);
            if (!validacion.IsValid)
                return ResultadoAccion<CitaDTO>.Falla("Datos inválidos",
                    validacion.Errors.Select(e => e.ErrorMessage).ToList());

            var cita = await _unitOfWork.Citas.ObtenerPorIdAsync(dto.Id);
            if (cita == null)
                return ResultadoAccion<CitaDTO>.Falla("Cita no encontrada");

            if (!await DoctorDisponibleAsync(cita.IdDoctor, dto.FechaHora, dto.Id))
                return ResultadoAccion<CitaDTO>.Falla("El doctor no está disponible en ese horario");

            _mapper.Map(dto, cita);
            cita.FechaDeModificacion = DateTime.UtcNow;

            _unitOfWork.Citas.Actualizar(cita);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoAccion<CitaDTO>.Exito(_mapper.Map<CitaDTO>(cita), "Cita actualizada exitosamente");
        }

        public async Task<ResultadoAccion> EliminarAsync(int id)
        {
            var cita = await _unitOfWork.Citas.ObtenerPorIdAsync(id);
            if (cita == null)
                return ResultadoAccion.Falla("Cita no encontrada");

            // Borrado lógico
            cita.Eliminado = true;
            cita.Activo = false;
            cita.FechaDeEliminacion = DateTime.UtcNow;

            _unitOfWork.Citas.Actualizar(cita);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoAccion.Exito("Cita eliminada (borrado lógico)");
        }

        public async Task<bool> DoctorDisponibleAsync(int idDoctor, DateTime fechaHora, int? idCitaExcluir = null)
        {
            var ocupadas = await _unitOfWork.Citas.BuscarAsync(c =>
                c.IdDoctor == idDoctor &&
                c.FechaHora == fechaHora &&
                c.Estado != EstadoCita.Cancelada &&
                (idCitaExcluir == null || c.Id != idCitaExcluir));

            return !ocupadas.Any();
        }
    }
}
