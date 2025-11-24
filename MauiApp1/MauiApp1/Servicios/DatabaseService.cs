using MauiApp1.Modelos; // Asegúrate de que este es tu namespace
using SQLite;

namespace MauiApp1.Servicios
{
    public class DatabaseService
    {
        // Declararamos la variable de conexión
        private readonly SQLiteAsyncConnection _database;

        // El constructor recibe la conexión por Inyección de Dependencias
        public DatabaseService(SQLiteAsyncConnection db)
        {
            // Asignamos la conexión inyectada
            _database = db;

            // Crear todas las tablas en el inicio
            _database.CreateTableAsync<Cliente>().Wait();
            _database.CreateTableAsync<Personal>().Wait();
            _database.CreateTableAsync<Proveedor>().Wait();
            _database.CreateTableAsync<Producto>().Wait();
            _database.CreateTableAsync<Factura>().Wait();
            _database.CreateTableAsync<DetalleVenta>().Wait();

            // NOTA: SQLite.NET no requiere explícitamente definir Foreign Keys (FK) 
            // en los atributos de la clase, las manejas por la lógica del C# 
            // y los campos que incluyes (e.g., int IDCliente).
        }
        public Task<Personal> GetPersonalPorNombreAsync(string nombre)
        {
            // Esto resuelve el error CS1061 que tenías previamente
            return _database.Table<Personal>()
                            .Where(p => p.Nombre == nombre)
                            .FirstOrDefaultAsync();
        }

        // ... (Aquí puedes agregar tus métodos CRUD: GetClientes, SaveCliente, etc.)
    }
}