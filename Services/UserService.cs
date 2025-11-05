using software.Models;

namespace software.Services
{
    public interface IUserService
    {
        bool ValidateCredentials(string username, string password, out Usuario? user);
        bool RegisterUser(RegisterViewModel model, out string error);
        void UpdateLastLogin(Usuario user);
        bool IsUserLockedOut(Usuario user);
        void IncrementLoginAttempts(Usuario user);
        void ResetLoginAttempts(Usuario user);
    }

    public class UserService : IUserService
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly List<Usuario> _usuarios;
        private const int MaxLoginAttempts = 5;
        private const int LockoutMinutes = 15;

        public UserService(IPasswordHasher passwordHasher, List<Usuario> usuarios)
        {
            _passwordHasher = passwordHasher;
            _usuarios = usuarios;
        }

        public bool ValidateCredentials(string username, string password, out Usuario? user)
        {
            user = _usuarios.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            
            if (user == null)
                return false;

            if (IsUserLockedOut(user))
                return false;

            if (!_passwordHasher.VerifyPassword(password, user.Password))
            {
                IncrementLoginAttempts(user);
                return false;
            }

            ResetLoginAttempts(user);
            UpdateLastLogin(user);
            return true;
        }

        public bool RegisterUser(RegisterViewModel model, out string error)
        {
            error = string.Empty;

            if (_usuarios.Any(u => u.Username.Equals(model.Username, StringComparison.OrdinalIgnoreCase)))
            {
                error = "El nombre de usuario ya está en uso";
                return false;
            }

            if (_usuarios.Any(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase)))
            {
                error = "El correo electrónico ya está registrado";
                return false;
            }

            var nuevoUsuario = new Usuario
            {
                Id = Guid.NewGuid().ToString(),
                Username = model.Username,
                Password = _passwordHasher.HashPassword(model.Password),
                Email = model.Email,
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            _usuarios.Add(nuevoUsuario);
            return true;
        }

        public void UpdateLastLogin(Usuario user)
        {
            user.LastLoginAt = DateTime.UtcNow;
        }

        public bool IsUserLockedOut(Usuario user)
        {
            return user.IsLockedOut;
        }

        public void IncrementLoginAttempts(Usuario user)
        {
            user.LoginAttempts++;
            if (user.LoginAttempts >= MaxLoginAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(LockoutMinutes);
            }
        }

        public void ResetLoginAttempts(Usuario user)
        {
            user.LoginAttempts = 0;
            user.LockoutEnd = null;
        }
    }
}