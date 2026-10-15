using Dominio.Enumeraciones;

namespace Aplicacion.DTOs.Citas
{
    public class CitaDTO
    {
        public int Id { get; set; }
        public int IdPaciente { get; set; }
        public int IdDoctor { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Motivo { get; set; } = string.Empty;
        public EstadoCita Estado { get; set; }
        public string? Notas { get; set; } = string.Empty;
        public bool Activo { get; set; }
    }
}
