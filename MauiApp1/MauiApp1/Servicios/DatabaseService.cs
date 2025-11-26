using MauiApp1.Modelos; // Asegúrate de que este es tu namespace
using SQLite;

namespace MauiApp1.Servicios
{
    public class DatabaseService
    {
        private async Task CrearUsuarioAdministradorAsync()
        {
            // Verifica si ya existe algún registro en la tabla Personal
            var count = await _database.Table<Personal>().CountAsync();

            // Si la tabla está vacía, crea un usuario con acceso de "Gerente"
            if (count == 0)
            {
                var admin = new Personal
                {
                    Nombre = "gerente",        // ⬅️ CUENTA DE PRUEBA
                    Contrasena = "admin123",   // ⬅️ CONTRASEÑA DE PRUEBA
                    Apellido = "Admin",
                    Cargo = "Gerente",         // ⬅️ Nivel de acceso
                    FechaContratacion = DateTime.Now
                };
                await _database.InsertAsync(admin);
            }
        }
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

            // Llamar a la función que crea el usuario admin
            // NOTA: Usamos .Wait() porque estamos en el constructor, que no puede ser async
            CrearUsuarioAdministradorAsync().Wait();

            _database.CreateTableAsync<Proveedor>().Wait();
            _database.CreateTableAsync<Producto>().Wait();
            _database.CreateTableAsync<Factura>().Wait();
            _database.CreateTableAsync<DetalleVenta>().Wait();
            _database.CreateTableAsync<DetalleSuministro>().Wait();
        }

        // NOTA: SQLite.NET no requiere explícitamente definir Foreign Keys (FK) 
        // en los atributos de la clase, las manejas por la lógica del C# 
        // y los campos que incluyes (e.g., int IDCliente).
        public Task<Personal> GetPersonalPorNombreAsync(string nombre)
        {
            // Esto resuelve el error CS1061 que tenías previamente
            return _database.Table<Personal>()
                            .Where(p => p.Nombre == nombre)
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
        // Obtener todos (READ)
        public Task<List<Cliente>> GetAllClientesAsync()
        {
            return _database.Table<Cliente>().ToListAsync();
        }

        // Obtener por ID (READ)
        public Task<Cliente> GetClienteByIdAsync(int id)
        {
            return _database.Table<Cliente>().Where(c => c.IDCliente == id).FirstOrDefaultAsync();
        }

        // Guardar o Actualizar (CREATE/UPDATE)
        public Task<int> SaveClienteAsync(Cliente cliente)
        {
            if (cliente.IDCliente != 0)
            {
                return _database.UpdateAsync(cliente); // Actualizar
            }
            else
            {
                return _database.InsertAsync(cliente); // Crear nuevo
            }
        }

        // Eliminar (DELETE)
        public Task<int> DeleteClienteAsync(Cliente cliente)
        {
            return _database.DeleteAsync(cliente);
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
                return _database.UpdateAsync(proveedor); // Actualizar
            }
            else
            {
                return _database.InsertAsync(proveedor); // Crear nuevo
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
        // Obtener todos (READ)
        public Task<List<Factura>> GetAllFacturasAsync()
        {
            return _database.Table<Factura>().ToListAsync();
        }

        // Obtener por ID (READ)
        public Task<Factura> GetFacturaByIdAsync(int id)
        {
            return _database.Table<Factura>().Where(f => f.IDFactura == id).FirstOrDefaultAsync();
        }

        // Guardar o Actualizar (CREATE/UPDATE)
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

        // Eliminar (DELETE)
        public Task<int> DeleteFacturaAsync(Factura factura)
        {
            return _database.DeleteAsync(factura);
        }


        // --- DETALLEVENTA CRUD ---
        // Obtener todos (READ)
        public Task<List<DetalleVenta>> GetAllDetallesVentaAsync()
        {
            return _database.Table<DetalleVenta>().ToListAsync();
        }

        // Obtener detalles por ID de Factura (READ)
        public Task<List<DetalleVenta>> GetDetallesByFacturaIdAsync(int idFactura)
        {
            return _database.Table<DetalleVenta>()
                            .Where(dv => dv.IDFactura == idFactura)
                            .ToListAsync();
        }

        // Guardar o Actualizar (CREATE/UPDATE)
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

        // Eliminar (DELETE)
        public Task<int> DeleteDetalleVentaAsync(DetalleVenta detalle)
        {
            return _database.DeleteAsync(detalle);
        }


        // --- DETALLE SUMINISTRO CRUD ---
        // Obtener todos (READ)
        public Task<List<DetalleSuministro>> GetAllDetallesSuministroAsync()
        {
            return _database.Table<DetalleSuministro>().ToListAsync();
        }

        // Obtener detalles por Proveedor o Producto (READ, si es necesario)
        public Task<List<DetalleSuministro>> GetDetallesByProveedorIdAsync(int idProveedor)
        {
            return _database.Table<DetalleSuministro>()
                            .Where(ds => ds.IDProveedor == idProveedor)
                            .ToListAsync();
        }

        // Guardar o Actualizar (CREATE/UPDATE)
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

        // Eliminar (DELETE)
        public Task<int> DeleteDetalleSuministroAsync(DetalleSuministro detalle)
        {
            return _database.DeleteAsync(detalle);
        }

    }
}