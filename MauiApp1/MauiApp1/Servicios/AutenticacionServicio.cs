using MauiApp1.Modelos;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace MauiApp1.Servicios
{
    public class AutenticacionServicio
    {
        private readonly DatabaseService _dbService;

        // El constructor recibe el DatabaseService por Inyección de Dependencias
        public AutenticacionServicio(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        /// Intenta autenticar un usuario usando la cuenta (Usuario o Nombre) y la contraseña.
        /// <param name="cuenta">El nombre de usuario (Usuario) o Nombre en la tabla Personal.</param>
        /// <param name="contrasena">La contraseña sin cifrar.</param>
        /// <returns>El objeto Personal si la autenticación es exitosa, de lo contrario, null.</returns>
        public async Task<Personal?> AuthenticateUserAsync(string cuenta, string contrasena)
        {
            // Asegura que la BD esté inicializada
            await _dbService.InicializarAsync();

            // 1. Intentar encontrar por Usuario (campo Usuario)
            var user = await _dbService.GetPersonalPorUsuarioAsync(cuenta);

            // 2. Si no se encontró, intentar por Nombre (campo Nombre)
            if (user == null)
            {
                var all = await _dbService.GetAllPersonalAsync();
                user = all.FirstOrDefault(p =>
                    !string.IsNullOrWhiteSpace(p.Usuario) && p.Usuario.Equals(cuenta, System.StringComparison.OrdinalIgnoreCase)
                    || !string.IsNullOrWhiteSpace(p.Nombre) && p.Nombre.Equals(cuenta, System.StringComparison.OrdinalIgnoreCase));
            }

            if (user == null) return null;

            // 3. Comparaciones de contraseña:
            // - primer intento: igualdad directa (compatibilidad retroactiva si hay contraseñas en texto plano)
            if (!string.IsNullOrEmpty(user.Contrasena) && user.Contrasena == contrasena)
                return user;

            // - segundo intento: hash SHA-256 -> Base64 para comparar con lo almacenado
            var bytes = Encoding.UTF8.GetBytes(contrasena);
            var hash = SHA256.HashData(bytes);
            var hashed = Convert.ToBase64String(hash);

            if (!string.IsNullOrEmpty(user.Contrasena) && user.Contrasena == hashed)
                return user;

            // no coincide
            return null;
        }
    }
}