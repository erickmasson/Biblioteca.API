using Biblioteca.API.Data;
using Biblioteca.API.Dtos;

namespace Biblioteca.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly BibliotecaContext _context;
        private readonly IConfiguration _configuration;
        public AuthService(BibliotecaContext context, IConfiguration configuration)
        {
            context = _context;
            _configuration = configuration;
        }
        public Task<(bool Sucesso, string TokenOuMensagem)> LoginAsync(UsuarioDto usuarioDto)
        {
            throw new NotImplementedException();
        }

        public Task<(bool Sucesso, string Mensagem)> RegistrarAsync(UsuarioDto usuarioDto)
        {
            throw new NotImplementedException();
        }
    }
}
