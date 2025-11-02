using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmacenWeb.Models
{
    public class DetalleVenta
    {
        [Key]
        public int DeId { get; set; }

        // Clave foránea para Venta
        [Required]
        public int VeId { get; set; }

        [ForeignKey("VeId")]
        public virtual Venta Venta { get; set; }

        // Clave foránea para Producto
        [Required]
        public int PrId { get; set; }

        [ForeignKey("PrId")]
        public virtual Producto Producto { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int DeCantidad { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
        public decimal DePrecioUnitario { get; set; }

        [Required]
        public decimal DeSubtotal { get; set; }
    }
}