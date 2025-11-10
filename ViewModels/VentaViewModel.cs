using System.Collections.Generic;

namespace AlmacenWeb.ViewModels
{
    

    public class VentaViewModel
    {
       
        public int ClId { get; set; }

        
        public int UsId { get; set; }

       
        public decimal Total { get; set; }

       
        public List<DetalleViewModel> Detalles { get; set; }
    }

    
    public class DetalleViewModel
    {
        public int PrId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}