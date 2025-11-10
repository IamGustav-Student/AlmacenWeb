using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlmacenWeb.Models
{
    public class Usuario
    {
        [Key]
        public int UsId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(50)]
        [Display(Name = "Nombre")]
        public string UsNombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [MaxLength(50)]
        [Display(Name = "Apellido")]
        public string UsApellido { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Email")]
        public string UsEmail { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MaxLength(500)] 
        [Display(Name = "Contraseña")]
        public string UsPassword { get; set; }

        public bool UsActivo { get; set; } = true;

        [Display(Name = "Fecha de Registro")]
        public DateTime UsFechaRegistro { get; set; } = DateTime.Now;

        // --- Relación con Rol ---
        [Required(ErrorMessage = "El rol es obligatorio")]
        [Display(Name = "Rol")]
        public int RoId { get; set; }

        [ForeignKey("RoId")]
        public virtual Rol Rol { get; set; }

        // Estas propiedades venían en tu AppDbContext original, las agregamos
        // para compatibilidad, aunque parecen ser para recuperación de pass.
        public string? Token { get; set; }
        public DateTime? date_created { get; set; }
    }
}