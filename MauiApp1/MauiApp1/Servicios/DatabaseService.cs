using SQLite;
using MauiApp1.Modelos;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MauiApp1.Servicios
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        // Variable para saber si ya iniciamos la BD
        private bool _inicializado = false;

        public DatabaseService(SQLiteAsyncConnection db)
        {
            _database = db;
            // El constructor debe quedar limpio y rápido
        }

        // Creamos este nuevo método dedicado
        public async Task InicializarAsync()
        {
            if (_inicializado) return; // Si ya está lista, no hacemos nada

            // Crear tablas (si no existen)
            await _database.CreateTableAsync<Personal>();
            await _database.CreateTableAsync<Cliente>();
            await _database.CreateTableAsync<Proveedor>();
            await _database.CreateTableAsync<Producto>();
            await _database.CreateTableAsync<Factura>();
            await _database.CreateTableAsync<DetalleVenta>();
            await _database.CreateTableAsync<DetalleSuministro>();

            // Intentar añadir columnas nuevas (si la BD ya existía)
            try
            {
                await _database.ExecuteAsync("ALTER TABLE Personal ADD COLUMN Email TEXT");
            }
            catch { /* ignora si ya existe */ }

            try
            {
                await _database.ExecuteAsync("ALTER TABLE Personal ADD COLUMN Departamento TEXT");
            }
            catch { /* ignora si ya existe */ }

            // Intentar añadir columnas nuevas en Proveedor (si se migró el modelo)
            try
            {
                await _database.ExecuteAsync("ALTER TABLE Proveedor ADD COLUMN RFC TEXT");
            }
            catch { /* ignora si ya existe */ }

            try
            {
                await _database.ExecuteAsync("ALTER TABLE Proveedor ADD COLUMN NombreContacto TEXT");
            }
            catch { /* ignora si ya existe */ }

            try
            {
                await _database.ExecuteAsync("ALTER TABLE Proveedor ADD COLUMN Categoria TEXT");
            }
            catch { /* ignora si ya existe */ }

            try
            {
                await _database.ExecuteAsync("ALTER TABLE Proveedor ADD COLUMN Direccion TEXT");
            }
            catch { /* ignora si ya exista */ }

            try
            {
                await _database.ExecuteAsync("ALTER TABLE Proveedor ADD COLUMN Calificacion INTEGER");
            }
            catch { /* ignora si ya exista */ }

            // Crear Admin por defecto (ahora guardamos hash para coherencia)
            await CrearUsuarioAdministradorAsync();

            _inicializado = true;
        }

        private async Task CrearUsuarioAdministradorAsync()
        {
            var count = await _database.Table<Personal>().CountAsync();
            if (count == 0)
            {
                // Hashar la contraseña por defecto (igual que en los ViewModels)
                var pwd = "admin123";
                var bytes = Encoding.UTF8.GetBytes(pwd);
                var hash = SHA256.HashData(bytes);
                var hashed = Convert.ToBase64String(hash);

                var admin = new Personal
                {
                    Nombre = "Gerente General",
                    Usuario = "gerente",
                    Contrasena = hashed,
                    Apellido = "Admin",
                    Cargo = "Gerente",
                    FechaContratacion = DateTime.Now
                };
                await _database.InsertAsync(admin);
            }
        }

        // --- TUS MÉTODOS CRUD (Login, Get, Save, etc.) ---

        public async Task<Personal?> GetPersonalPorUsuarioAsync(string usuario)
        {
            await InicializarAsync(); // Aseguramos que la BD exista antes de buscar
            return await _database.Table<Personal>()
                            .Where(p => p.Usuario == usuario)
                            .FirstOrDefaultAsync();
        }

        // --- PERSONAL (EMPLEADOS) CRUD ---
        // Obtener todos (READ)
        public Task<List<Personal>> GetAllPersonalAsync()
        {
            return _database.Table<Personal>().ToListAsync();
        }

        // Obtener por ID (READ)
        public Task<Personal> GetPersonalByIdAsync(int id)
        {
            return _database.Table<Personal>().Where(p => p.IDPersonal == id).FirstOrDefaultAsync();
        }

        // Guardar o Actualizar (CREATE/UPDATE)
        public Task<int> SavePersonalAsync(Personal personal)
        {
            if (personal.IDPersonal != 0)
            {
                return _database.UpdateAsync(personal); // Actualiza si tiene ID
            }
            else
            {
                return _database.InsertAsync(personal); // Crea nuevo
            }
        }

        // Eliminar (DELETE)
        public Task<int> DeletePersonalAsync(Personal personal)
        {
            return _database.DeleteAsync(personal);
        }

        // --- CLIENTE CRUD ---
        public Task<List<Cliente>> GetAllClientesAsync()
        {
            return _database.Table<Cliente>().ToListAsync();
        }

        // --- PROVEEDOR CRUD ---
        // Obtener todos (READ)
        public Task<List<Proveedor>> GetAllProveedoresAsync()
        {
            return _database.Table<Proveedor>().ToListAsync();
        }

        // Obtener por ID (READ)
        public Task<Proveedor> GetProveedorByIdAsync(int id)
        {
            return _database.Table<Proveedor>().Where(p => p.IDProveedor == id).FirstOrDefaultAsync();
        }

        // Guardar o Actualizar (CREATE/UPDATE)
        public Task<int> SaveProveedorAsync(Proveedor proveedor)
        {
            if (proveedor.IDProveedor != 0)
            {
                return _database.UpdateAsync(proveedor); // Actualiza si tiene ID
            }
            else
            {
                return _database.InsertAsync(proveedor); // Crea nuevo
            }
        }

        // Eliminar (DELETE)
        public Task<int> DeleteProveedorAsync(Proveedor proveedor)
        {
            return _database.DeleteAsync(proveedor);
        }

        // --- PRODUCTO CRUD ---
        // Obtener todos (READ)
        public Task<List<Producto>> GetAllProductosAsync()
        {
            return _database.Table<Producto>().ToListAsync();
        }

        // Obtener por ID (READ)
        public Task<Producto> GetProductoByIdAsync(int id)
        {
            return _database.Table<Producto>().Where(p => p.IDProducto == id).FirstOrDefaultAsync();
        }

        // Guardar o Actualizar (CREATE/UPDATE)
        public Task<int> SaveProductoAsync(Producto producto)
        {
            if (producto.IDProducto != 0)
            {
                return _database.UpdateAsync(producto); // Actualizar
            }
            else
            {
                return _database.InsertAsync(producto); // Crear nuevo
            }
        }

        // Eliminar (DELETE)
        public Task<int> DeleteProductoAsync(Producto producto)
        {
            return _database.DeleteAsync(producto);
        }

        // --- FACTURA CRUD ---
        public Task<List<Factura>> GetAllFacturasAsync()
        {
            return _database.Table<Factura>().ToListAsync();
        }

        public Task<Factura> GetFacturaByIdAsync(int id)
        {
            return _database.Table<Factura>().Where(f => f.IDFactura == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveFacturaAsync(Factura factura)
        {
            if (factura.IDFactura != 0)
            {
                return _database.UpdateAsync(factura); // Actualizar
            }
            else
            {
                return _database.InsertAsync(factura); // Crear nueva
            }
        }

        public Task<int> DeleteFacturaAsync(Factura factura)
        {
            return _database.DeleteAsync(factura);
        }

        // --- DETALLEVENTA CRUD ---
        public Task<List<DetalleVenta>> GetAllDetallesVentaAsync()
        {
            return _database.Table<DetalleVenta>().ToListAsync();
        }

        public Task<List<DetalleVenta>> GetDetallesByFacturaIdAsync(int idFactura)
        {
            return _database.Table<DetalleVenta>()
                            .Where(dv => dv.IDFactura == idFactura)
                            .ToListAsync();
        }

        public Task<int> SaveDetalleVentaAsync(DetalleVenta detalle)
        {
            if (detalle.IDDetalleVenta != 0)
            {
                return _database.UpdateAsync(detalle); // Actualizar
            }
            else
            {
                return _database.InsertAsync(detalle); // Crear nuevo
            }
        }

        public Task<int> DeleteDetalleVentaAsync(DetalleVenta detalle)
        {
            return _database.DeleteAsync(detalle);
        }

        // --- DETALLE SUMINISTRO CRUD ---
        public Task<List<DetalleSuministro>> GetAllDetallesSuministroAsync()
        {
            return _database.Table<DetalleSuministro>().ToListAsync();
        }

        public Task<List<DetalleSuministro>> GetDetallesByProveedorIdAsync(int idProveedor)
        {
            return _database.Table<DetalleSuministro>()
                            .Where(ds => ds.IDProveedor == idProveedor)
                            .ToListAsync();
        }

        public Task<int> SaveDetalleSuministroAsync(DetalleSuministro detalle)
        {
            if (detalle.Id != 0) // Usa el Id de SQLite.NET para el Update
            {
                return _database.UpdateAsync(detalle);
            }
            else
            {
                return _database.InsertAsync(detalle);
            }
        }

        public Task<int> DeleteDetalleSuministroAsync(DetalleSuministro detalle)
        {
            return _database.DeleteAsync(detalle);
        }

    }
}