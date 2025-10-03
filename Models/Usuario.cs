using System.ComponentModel.DataAnnotations;

namespace AlmacenWeb.Models
{
    public class Usuario
    {
        [Key]
        public int UsId { get; set; }

        public string UsNombre { get; set; }

        public string UsApellido { get; set; }

        public string UsEmail { get; set; }

        public int UsTelefono { get; set; }

        public string UsDireccion { get; set; }

        public bool UsActivo { get; set; } = true;

        public DateTime UsFechaRegistro { get; set; } = DateTime.Now;

        public string UsPassword { get; set; }

        public string? UsToken { get; set; } = "TokenBloqueado";

        public DateTime date_created { get; set; } = DateTime.Now;





    }
}
