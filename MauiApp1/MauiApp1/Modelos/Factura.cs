using SQLite;

namespace MauiApp1.Modelos
{
    public class Factura
    {
        [PrimaryKey, AutoIncrement]
        public int IDVenta { get; set; }

        public DateTime FechaEmision { get; set; }

        // La hora se puede manejar como parte del DateTime o como string
        public string Hora { get; set; }

        [MaxLength(50)]
        public string TipoPago { get; set; }

        public double TotalBruto { get; set; }

        public double MontoTotal { get; set; } // Monto final después de descuentos/impuestos

        public int NumeroActualizacion { get; set; }

        //(FK) del Cliente y del Personal
        public int IDCliente { get; set; }
        public int IDPersonal { get; set; }
    }
}