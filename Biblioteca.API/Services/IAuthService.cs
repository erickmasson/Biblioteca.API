using Biblioteca.API.Dtos;
using static Biblioteca.API.Dtos.UsuarioDto;

namespace Biblioteca.API.Services
{
    public interface IAuthService
    {
        Task<(bool Sucesso, string Mensagem)> RegistrarAsync(UsuarioRegistroDto usuarioDto);
        Task<(bool Sucesso, string TokenOuMensagem)> LoginAsync(UsuarioLoginDto usuarioDto);
    }
}
