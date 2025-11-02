using System.ComponentModel.DataAnnotations;

namespace AlmacenWeb.Models
{
    public class Producto
    {
        [Key]
        public int PrId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100)]
        [Display(Name = "Nombre del Producto")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "El nombre solo puede contener letras, números y espacios.")]
        public string PrNombre { get; set; }

        [StringLength(50)]
        [Display(Name = "Código de Barras")]
        //ajustada para permitir solo dígitos y espacios para un código de barras estándar.
        [RegularExpression(@"^[\d\s]*$", ErrorMessage = "El Código de Barras solo puede contener números y espacios.")]
        public string CodigoBarra { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa")]
        [Display(Name = "Cantidad Disponible")]
        public int CantidadDisponible { get; set; }
    }
}
