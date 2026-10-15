namespace Aplicacion.DTOs.Citas
{
    public class ActualizarCitaDTO
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Motivo { get; set; } = string.Empty;
        public string? Notas { get; set; } = string.Empty;
        // El paciente y el doctor no se cambian; el estado se maneja con los métodos de la entidad
    }
}
