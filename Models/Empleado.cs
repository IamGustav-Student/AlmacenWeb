namespace AlmacenWeb.Models
{
    public class Empleado : Usuario
    {

        public int EmpNLegajo { get; set; }
        public decimal EmpSueldo { get; set; }
        public bool EmpComisiona { get; set; } = false;

        public decimal EmpComision { get; set; } 



    }
}
