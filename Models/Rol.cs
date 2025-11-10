using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlmacenWeb.Models
{
    public class Rol
    {
        [Key]
        public int RoId { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [MaxLength(50)]
        [Display(Name = "Nombre del Rol")]
        public string RoNombre { get; set; }

        [MaxLength(255)]
        [Display(Name = "Descripción")]
        public string RoDescripcion { get; set; }

        
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}