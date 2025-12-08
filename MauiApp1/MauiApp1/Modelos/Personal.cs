using SQLite;

namespace MauiApp1.Modelos
{
    public class Personal
    {
        [PrimaryKey, AutoIncrement]
        public int IDPersonal { get; set; }

        // Usuario para login (ej: jperez)
        [MaxLength(50), Unique]
        public string Usuario { get; set; } = string.Empty;

        // Nombre real de la persona (ej: Juan)
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;

        // Contraseña guardada (hash Base64 recomendado)
        public string Contrasena { get; set; } = string.Empty;

        // Nuevo campo: Email (opcional)
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        // Nuevo campo: Departamento (opcional)
        [MaxLength(100)]
        public string Departamento { get; set; } = string.Empty;

        // Cargo / rol
        [MaxLength(50)]
        public string Cargo { get; set; } = string.Empty;

        public string Permisos { get; set; } = string.Empty;
        public DateTime FechaContratacion { get; set; } = DateTime.Now;
    }
}