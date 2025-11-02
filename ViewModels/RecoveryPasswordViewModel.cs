using System.ComponentModel.DataAnnotations;
namespace AlmacenWeb.ViewModels
{
    public class RecoveryPasswordViewModel
    {
        [Required]
        public string Token { get; set; } // El token generado

        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 50 caracteres")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }
    }
}
    