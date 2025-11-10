using System.ComponentModel.DataAnnotations;

namespace AlmacenWeb.ViewModels
{
    
    public class UsuarioEditViewModel
    {
        public int UsId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        public string UsNombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [Display(Name = "Apellido")]
        public string UsApellido { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Email")]
        public string UsEmail { get; set; }

        
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña (Opcional)")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        [Display(Name = "Rol")]
        public int RoId { get; set; }

        [Display(Name = "Usuario Activo")]
        public bool UsActivo { get; set; }
    }
}