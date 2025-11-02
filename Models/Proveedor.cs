using System.ComponentModel.DataAnnotations;

namespace AlmacenWeb.Models
{
    public class Proveedor
    {
        [Key]
        public int ProveedorId { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es obligatorio")]
        [MaxLength(100)]
        [Display(Name = "Nombre de la Empresa")]
        public string NombreEmpresa { get; set; }

        [MaxLength(100)]
        [Display(Name = "Nombre de Contacto")]
        public string NombreContacto { get; set; }

        [Required(ErrorMessage = "El CUIT es obligatorio")]
        [MaxLength(20)]
        [Display(Name = "CUIT")]
        public string Cuit { get; set; }

        [MaxLength(255)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [MaxLength(50)]
        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}