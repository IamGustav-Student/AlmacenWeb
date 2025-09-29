using System.ComponentModel.DataAnnotations;

namespace AlmacenWeb.Models
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        public DateTime FechaVenta { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El total debe ser un valor positivo.")]
        public decimal Total { get; set; }

        // Relación con el cliente (opcional)
        public int? ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public ICollection<DetalleVenta> DetalleVenta { get; set; }
    }
}
