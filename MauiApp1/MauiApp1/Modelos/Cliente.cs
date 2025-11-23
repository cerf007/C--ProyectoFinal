using SQLite;

namespace MauiApp1.Modelos
{
    public class Cliente
    {
        [PrimaryKey, AutoIncrement]
        public int IDCliente { get; set; }

        [MaxLength(100)]
        public string Nombre { get; set; }

        [MaxLength(100)]
        public string Apellido { get; set; }

        public string Telefono { get; set; }

        [Unique]
        public string Email { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string EstatusCuenta { get; set; }
    }
}
