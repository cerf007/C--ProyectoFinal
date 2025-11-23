using SQLite;

namespace MauiApp1.Modelos
{
    public class Personal
    {
        [PrimaryKey, AutoIncrement]
        public int IDPersonal { get; set; }

        [MaxLength(100)]
        public string Nombre { get; set; }

        [MaxLength(100)]
        public string Apellido { get; set; }

        [MaxLength(50)]
        public string Cargo { get; set; }

        // Campo para manejar los roles o permisos de acceso
        public string Permisos { get; set; }

        public DateTime FechaContratacion { get; set; }
    }
}