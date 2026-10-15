using AutoMapper;
using FluentValidation;
using Aplicacion.DTOs.Pacientes;
using Aplicacion.Helpers;
using Aplicacion.Servicios.Interfaces;
using Dominio.Entidades.Pacientes;
using Dominio.Interfaces;

namespace Aplicacion.Servicios.Implementaciones
{
    public class PacienteService : IPacienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CrearPacienteDTO> _crearValidacion;
        private readonly IValidator<ActualizarPacienteDTO> _actualizarValidator;

        public PacienteService(IUnitOfWork unitOfWork, IMapper mapper,
            IValidator<CrearPacienteDTO> crearValidacion,
            IValidator<ActualizarPacienteDTO> actualizarValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _crearValidacion = crearValidacion;
            _actualizarValidator = actualizarValidator;
        }

        public async Task<ResultadoAccion<PacienteDTO>> ObtenerPorIdAsync(int id)
        {
            var paciente = await _unitOfWork.Pacientes.ObtenerPorIdAsync(id);
            if (paciente == null)
                return ResultadoAccion<PacienteDTO>.Falla("Paciente no encontrado");

            var dto = _mapper.Map<PacienteDTO>(paciente);
            return ResultadoAccion<PacienteDTO>.Exito(dto);
        }

        public async Task<ResultadoAccion<IEnumerable<PacienteDTO>>> ObtenerTodosAsync()
        {
            var pacientes = await _unitOfWork.Pacientes.ObtenerTodosAsync();
            var dtos = _mapper.Map<IEnumerable<PacienteDTO>>(pacientes);
            return ResultadoAccion<IEnumerable<PacienteDTO>>.Exito(dtos);
        }

        public async Task<ResultadoAccion<PacienteDTO>> CrearAsync(CrearPacienteDTO dto)
        {
            // Validar con FluentValidation
            var validacion = await _crearValidacion.ValidateAsync(dto);
            if (!validacion.IsValid)
            {
                return ResultadoAccion<PacienteDTO>.Falla("Datos inválidos",
                    validacion.Errors.Select(e => e.ErrorMessage).ToList());
            }

            // Verificar duplicado por número de documento
            if (await ExistePorIdentificacionAsync(dto.NumeroDocumento))
                return ResultadoAccion<PacienteDTO>.Falla("Ya existe un paciente con ese número de identificación");

            // Mapear y persistir
            var paciente = _mapper.Map<Paciente>(dto);
            await _unitOfWork.Pacientes.AgregarAsync(paciente);
            await _unitOfWork.GuardarCambiosAsync();

            var pacienteCreado = await _unitOfWork.Pacientes.ObtenerPorIdAsync(paciente.Id);
            var dtoResultado = _mapper.Map<PacienteDTO>(pacienteCreado);
            return ResultadoAccion<PacienteDTO>.Exito(dtoResultado, "Paciente creado exitosamente");
        }

        public async Task<ResultadoAccion<PacienteDTO>> ActualizarAsync(ActualizarPacienteDTO dto)
        {
            // Validar con FluentValidation
            var validacion = await _actualizarValidator.ValidateAsync(dto);
            if (!validacion.IsValid)
            {
                return ResultadoAccion<PacienteDTO>.Falla("Datos inválidos",
                    validacion.Errors.Select(e => e.ErrorMessage).ToList());
            }

            var paciente = await _unitOfWork.Pacientes.ObtenerPorIdAsync(dto.Id);
            if (paciente == null)
                return ResultadoAccion<PacienteDTO>.Falla("Paciente no encontrado");

            // Mapear cambios sobre la entidad existente y registrar fecha de modificación
            _mapper.Map(dto, paciente);
            paciente.FechaDeModificacion = DateTime.UtcNow;

            _unitOfWork.Pacientes.Actualizar(paciente);
            await _unitOfWork.GuardarCambiosAsync();

            var dtoResultado = _mapper.Map<PacienteDTO>(paciente);
            return ResultadoAccion<PacienteDTO>.Exito(dtoResultado, "Paciente actualizado exitosamente");
        }

        public async Task<ResultadoAccion> EliminarAsync(int id)
        {
            var paciente = await _unitOfWork.Pacientes.ObtenerPorIdAsync(id);
            if (paciente == null)
                return ResultadoAccion.Falla("Paciente no encontrado");

            // Borrado lógico para integridad histórica
            paciente.Eliminado = true;
            paciente.Activo = false;
            paciente.FechaDeEliminacion = DateTime.UtcNow;

            _unitOfWork.Pacientes.Actualizar(paciente);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoAccion.Exito("Paciente eliminado (borrado lógico)");
        }

        public async Task<bool> ExistePorIdentificacionAsync(string identificacion)
        {
            var pacientes = await _unitOfWork.Pacientes.BuscarAsync(p => p.NumeroDocumento == identificacion);
            return pacientes.Any();
        }
    }
}