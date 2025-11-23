using SQLite;

namespace MauiApp1.Modelos
{
    public class Proveedor
    {
        [PrimaryKey, AutoIncrement]
        public int IDProveedor { get; set; }

        [MaxLength(250), Unique]
        public string NombreCompania { get; set; }

        [MaxLength(250)]
        public string Email { get; set; }

        public string Telefono { get; set; }
    }
}