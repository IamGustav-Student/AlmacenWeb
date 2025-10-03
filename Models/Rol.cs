using System.ComponentModel.DataAnnotations;

namespace AlmacenWeb.Models
{
    public class Rol
    {
        [Key]
        public int Id { get; set; }
        public string RoNombre { get; set; }
        public string RoDescripcion { get; set; }
    }
}
