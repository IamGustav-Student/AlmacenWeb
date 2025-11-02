namespace AlmacenWeb.ViewModels
{
    // Esta clase agrupa todos los datos que el Dashboard necesita mostrar.
    public class DashboardViewModel
    {
        public int TotalProductos { get; set; }
        public int ProductosStockBajo { get; set; }
        public int TotalClientes { get; set; }
        public int VentasHoy { get; set; }
        public decimal TotalVentasHoy { get; set; }
    }
}
