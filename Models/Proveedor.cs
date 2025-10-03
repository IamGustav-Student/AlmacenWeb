using System.ComponentModel.DataAnnotations;

namespace AlmacenWeb.Models
{
    public class Proveedor
    {
        [Key]
        public int ProveedorId { get; set; }
        [Required]
        [StringLength(50)]
        [Display (Name = "nombre del Proveedor")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
        public string ProveedorName { get; set; }

    }
}
