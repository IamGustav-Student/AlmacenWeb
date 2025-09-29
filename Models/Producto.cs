using System.ComponentModel.DataAnnotations;

namespace AlmacenWeb.Models
{
    public class Producto
    {
        [Key]
        public int PrId { get; set; }
        [Required]
        [MaxLength(100)]
        [Display(Name = "Nombre del Producto")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "El nombre solo puede contener letras, números y espacios.")]
        public string PrNombre { get; set; }
        [StringLength(50)]
        [RegularExpression(@"^[00-99999\s]*$", ErrorMessage = "La categoría solo puede contener números.")]
        public string CodigoBarra { get; set; } // Puede ser null o vacío si no tiene código de barras
        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
        public decimal Precio { get; set; }
        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa")]
        public int CantidadDisponible { get; set; }

    }
}
