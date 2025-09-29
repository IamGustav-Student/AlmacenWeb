using System.ComponentModel.DataAnnotations;

namespace AlmacenWeb.Models
{
    public class Cliente
    {
        [Key]
        public int ClId { get; set; }
        [Required]
        [MaxLength(20)]
        [Display (Name = "Nombre de Cliente")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios.")]
        public string ClNombre { get; set; }
        [Required]
        [MaxLength(20)]
        [Display(Name = "Apellido de Cliente")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "El apellido solo puede contener letras y espacios.")]
        public string ClApellido { get; set; }
        [Required]
        [MaxLength(100)]
        [Display(Name = "Dirección de Cliente")]
        [RegularExpression(@"^[a-zA-Z0-9\s,.-]+$", ErrorMessage = "La dirección contiene caracteres no válidos.")]

        public string ClDireccion { get; set; }
        [Required]
        [MaxLength(15)]
        [Display(Name = "Teléfono de Cliente")]
        [RegularExpression(@"^\+?[0-9\s-]{7,15}$", ErrorMessage = "El número de teléfono no es válido.")]
        public string ClTelefono { get; set; }
        [Required]
        [MaxLength(50)]
        [Display(Name = "Email de Cliente")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string ClEmail { get; set; }

        public virtual ICollection<Venta>? Ventas { get; set; } = new List<Venta>();
    }
}
