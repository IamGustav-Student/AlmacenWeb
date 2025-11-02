using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AlmacenWeb.Models
{
    public class Cliente
    {
        [Key]
        public int ClId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100)]
        [Display(Name = "Nombre")]
        public string ClNombre { get; set; }

        [MaxLength(100)]
        [Display(Name = "Apellido")]
        public string ClApellido { get; set; }

        [Required(ErrorMessage = "El DNI/CUIT es obligatorio")]
        [MaxLength(20)]
        [Display(Name = "DNI o CUIT")]
        public string ClDniCuit { get; set; }

        [MaxLength(255)]
        [Display(Name = "Dirección")]
        public string ClDireccion { get; set; }

        [MaxLength(50)]
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [Display(Name = "Teléfono")]
        public string ClTelefono { get; set; }

        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Email")]
        public string ClEmail { get; set; }

        // Propiedad de navegación: Un cliente puede tener muchas ventas
         public virtual ICollection<Venta> Ventas { get; set; }
    }
}
