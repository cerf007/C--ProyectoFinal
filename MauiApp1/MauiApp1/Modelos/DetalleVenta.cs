using SQLite;

namespace MauiApp1.Modelos
{
    // Usamos una PK única (Id) para simplificar la lógica de SQLite.NET,
    // pero mantenemos las FKs para representar la clave compuesta
    public class DetalleVenta
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // 🔑 Claves Foráneas que actúan como clave compuesta en el DER
        public int IDVenta { get; set; }
        public int IDProducto { get; set; }

        public int Cantidad { get; set; }

        public double PrecioVenta { get; set; } // Precio al que se vendió

        public double Subtotal { get; set; }
    }
}
