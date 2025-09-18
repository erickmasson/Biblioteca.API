using Biblioteca.API.Dtos;

namespace Biblioteca.API.Services
{
    public interface IAuthService
    {
        Task<(bool Sucesso, string Mensagem)> RegistrarAsync(UsuarioDto usuarioDto);
        Task<(bool Sucesso, string TokenOuMensagem)> LoginAsync(UsuarioDto usuarioDto);
    }
}
