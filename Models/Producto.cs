using Microsoft.AspNetCore.Mvc;
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
        [RegularExpression(@"^[a-z-A-Z0-9\s\-]+$", ErrorMessage = "El nombre solo puede contener letras, números y espacios.")]
        public string PrNombre { get; set; }

        [StringLength(50)]
        [Display(Name = "Código de Barras")]
        
        [RegularExpression(@"^[\d\s]*$", ErrorMessage = "El Código de Barras solo puede contener números y espacios.")]
        [Remote(action: "IsCodigoBarraAvailable", controller: "Productos", AdditionalFields = nameof(PrId), ErrorMessage = "Este código de barras ya está registrado.")]
        public string CodigoBarra { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero")]
        [Display(Name = "($) Precio")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa")]
        [Display(Name = "Cantidad Disponible")]
        public int CantidadDisponible { get; set; }
    }
}
