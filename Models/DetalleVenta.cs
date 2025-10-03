using AlmacenWeb.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace AlmacenWeb.Models
{
    public class DetalleVenta
    {
        [Key]
        public int DVId { get; set; }
        public decimal Total { get; set; }

        public int VentaId { get; set; }
        public Venta Venta { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        

    }
}
