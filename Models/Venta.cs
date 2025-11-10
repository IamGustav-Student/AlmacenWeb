using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmacenWeb.Models
{
    public class Venta
    {
        [Key]
        public int VeId { get; set; }

        [Required]
        [Display(Name = "Fecha de Venta")]
        public DateTime VeFecha { get; set; } = DateTime.Now;

        
        [Required(ErrorMessage = "El cliente es obligatorio")]
        public int ClId { get; set; }

        [ForeignKey("ClId")]
        public virtual Cliente Cliente { get; set; }

        
        [Required]
        public int UsId { get; set; }

        [ForeignKey("UsId")]
        public virtual Usuario Usuario { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor que cero")]
        public decimal VeTotal { get; set; }
        [Required]
        [Display(Name = "Método de Pago")]
        public MetodoPago MetodoPago { get; set; }

        [Required]
        [Display(Name = "Pagado")]
        public bool Pagado { get; set; }

        
        public virtual ICollection<DetalleVenta> DetalleVentas { get; set; }
    }
}