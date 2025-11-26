using MauiApp1.Modelos;
using System.Threading.Tasks;

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
        /// Intenta autenticar un usuario usando la cuenta (Nombre) y la contraseña.
        /// <param name="cuenta">El nombre de usuario (Nombre en la tabla Personal).</param>
        /// <param name="contrasena">La contraseña sin cifrar.</param>
        /// <returns>El objeto Personal si la autenticación es exitosa, de lo contrario, null.</returns>
        public async Task<Personal> AuthenticateUserAsync(string cuenta, string contrasena)
        {
            // 1. Busca el usuario por su nombre/cuenta usando el método que añadimos al DatabaseService
            var user = await _dbService.GetPersonalPorNombreAsync(cuenta);

            // 2. Verifica si se encontró el usuario
            if (user == null)
            {
                return null; // Usuario no encontrado
            }

            // 3. Verifica la contraseña
            if (user.Contrasena == contrasena)
            {
                return user; // Autenticación exitosa
            }

            return null; // Contraseña incorrecta
        }

    }
}
