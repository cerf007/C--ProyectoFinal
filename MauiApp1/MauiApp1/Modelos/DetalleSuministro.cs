using SQLite;
using System;

namespace MauiApp1.Modelos
{
    public class DetalleSuministro
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Clave primaria para el CRUD de SQLite.NET

        // 🔑 Clave Compuesta de Negocio (Claves Foráneas)
        public int IDProveedor { get; set; }
        public int IDProducto { get; set; }

        // Atributos de la Relación
        public double PrecioCompra { get; set; }
        public int CantidadSuministrada { get; set; }

        // Otros
        public DateTime FechaSuministro { get; set; }
    }
}
