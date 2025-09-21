using Biblioteca.API.Dtos;
using static Biblioteca.API.Dtos.UsuarioDto;

namespace Biblioteca.API.Services
{
    /// <summary>
    /// Define o contrato para os serviços de autenticação.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registra um novo usuário no sistema.
        /// </summary>
        /// <param name="usuarioDto">Dados de registro do usuário.</param>
        /// <returns>Uma tupla indicando se a operação foi bem-sucedida e uma mensagem de resultado.</returns>
        Task<(bool Sucesso, string Mensagem)> RegistrarAsync(UsuarioRegistroDto usuarioDto);

        /// <summary>
        /// Autentica um usuário e gera um token JWT.
        /// </summary>
        /// <param name="usuarioDto">Dados de login do usuário.</param>
        /// <returns>Uma tupla indicando se o login foi bem-sucedido e o token JWT ou uma mensagem de erro.</returns>
        Task<(bool Sucesso, string TokenOuMensagem)> LoginAsync(UsuarioLoginDto usuarioDto);
    }
}
