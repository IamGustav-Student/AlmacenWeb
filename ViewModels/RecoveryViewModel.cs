using System.ComponentModel.DataAnnotations;


namespace AlmacenWeb.ViewModels
{
    public class RecoveryViewModel
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email incorrecto")]
        public string Email { get; set; }
    }
};
