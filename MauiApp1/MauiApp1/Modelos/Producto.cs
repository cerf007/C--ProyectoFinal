using SQLite;

namespace MauiApp1.Modelos
{
    public class Producto
    {
        [PrimaryKey, AutoIncrement]
        public int IDProducto { get; set; }

        [MaxLength(250)]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        [MaxLength(100)]
        public string Categoria { get; set; }

        // Usamos double para valores monetarios
        public double PrecioUnitario { get; set; }

        public double CostoUnitario { get; set; }

        public int StockMinimo { get; set; }

        public int Existencias { get; set; }

        // (FK) del Proveedor
        public int IDProveedor { get; set; }
        public int Existencia { get; internal set; }
    }
}