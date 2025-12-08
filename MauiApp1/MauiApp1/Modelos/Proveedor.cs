using SQLite;

namespace MauiApp1.Modelos
{
    public class Proveedor
    {
        [PrimaryKey, AutoIncrement]
        public int IDProveedor { get; set; }

        // Nombre de la compañía (coincide con VM.EmpresaNombre -> Nombre)
        [MaxLength(250), Unique]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(50)]
        public string RFC { get; set; } = string.Empty;

        [MaxLength(250)]
        public string NombreContacto { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Categoria { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Direccion { get; set; } = string.Empty;

        public int Calificacion { get; set; } = 5;
    }
}