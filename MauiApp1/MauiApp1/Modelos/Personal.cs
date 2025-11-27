using SQLite;
using System;

namespace MauiApp1.Modelos
{
    public class Personal
    {
        // Clave Primaria Autoincremental
        [PrimaryKey, AutoIncrement]
        public int IDPersonal { get; set; }

        //Campo usado como "Cuenta" o Username para el inicio de sesión
        [MaxLength(100), Unique]
        public string Nombre { get; set; }

        [MaxLength(100)]
        public string Apellido { get; set; }

        //Campo añadido para la validación de credenciales
        [MaxLength(100)]
        public string Contrasena { get; set; }

        //Campo usado para definir el nivel de acceso (Gerente, Trabajador)
        [MaxLength(50)]
        public string Cargo { get; set; }

        public string Permisos { get; set; } = string.Empty;

        public DateTime FechaContratacion { get; set; } = DateTime.Now;

    }
}