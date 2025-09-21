using Biblioteca.API.Data;
using Biblioteca.API.Dtos;
using Biblioteca.API.Models;
using Biblioteca.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;
using static Biblioteca.API.Dtos.UsuarioDto;

namespace Biblioteca.API.Tests.Services
{
    public class AuthServiceTests
    {
        // Método auxiliar para criar um DbContext em memória para cada teste
        private BibliotecaContext CriarContextoEmMemoria(string nomeBanco)
        {
            var options = new DbContextOptionsBuilder<BibliotecaContext>()
                .UseInMemoryDatabase(databaseName: nomeBanco)
                .Options;

            return new BibliotecaContext(options);
        }

        [Fact]
        public async Task RegistrarAsync_DeveRetornarFalha_QuandoEmailJaExiste()
        {
            // --- 1. Arrange (Organizar) ---

            // a. Criar um contexto em memória único para este teste
            var context = CriarContextoEmMemoria("TesteEmailDuplicadoDB");

            // b. Adicionar um usuário ao "banco de dados" em memória para simular o e-mail existente
            var emailExistente = "teste@email.com";
            context.Usuarios.Add(new Usuario { Id = 1, Nome = "Usuário Teste", Email = emailExistente, SenhaHash = "hash123", Role = "Membro" });
            await context.SaveChangesAsync();

            // c. Mockar as dependências que não são o DbContext
            var mockConfiguration = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<AuthService>>();

            // d. Criar a instância do serviço, passando o CONTEXTO REAL em memória
            var authService = new AuthService(context, mockConfiguration.Object, mockLogger.Object);

            // e. Criar o DTO com o e-mail duplicado
            var usuarioDto = new UsuarioRegistroDto { Nome = "Novo Usuario", Email = emailExistente, Senha = "senhaNova" };

            // --- 2. Act (Agir) ---

            // Executar o método que queremos testar
            var resultado = await authService.RegistrarAsync(usuarioDto);

            // --- 3. Assert (Verificar) ---

            // Verificar se o resultado é o esperado
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Be("Este e-mail já está em uso.");
        }

        [Fact]
        public async Task RegistrarAsync_DeveRetornarSucesso_QuandoEmailNaoExiste()
        {
            // --- 1. Arrange (Organizar) ---
            // a. Criar um contexto em memória limpo e único para este teste
            var context = CriarContextoEmMemoria("TesteSucessoRegistroDb");

            // b. Mockar as dependências que não são o DbContext
            var mockConfiguration = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<AuthService>>();

            // c. Criar a instância do serviço com o contexto em memória
            var authService = new AuthService(context, mockConfiguration.Object, mockLogger.Object);

            // d. Criar o DTO com dados de um novo usuário
            var usuarioDto = new UsuarioRegistroDto { Nome = "Novo Usuario", Email = "novo@email.com", Senha = "senha123" };

            // --- 2. Act (Agir) ---
            var resultado = await authService.RegistrarAsync(usuarioDto);

            // --- 3. Assert (Verificar) ---
            resultado.Sucesso.Should().BeTrue();
            resultado.Mensagem.Should().Be("Usuário registrado com sucesso!");
        }

        [Fact]
        public async Task LoginAsync_DeveRetornarFalha_QuandoSenhaIncorreta()
        {
            // --- 1. Arrange (Organizar) ---
            var context = CriarContextoEmMemoria("TesteLoginSenhaIncorretaDB");

            var email = "usuario.teste@email.com";
            var senhaCorreta = "senhaForte123";
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(senhaCorreta);
            context.Usuarios.Add(new Usuario { Id = 1, Nome = "Usuario Teste", Email = email, SenhaHash = senhaHash, Role = "Membro" });
            await context.SaveChangesAsync();

            var mockConfiguration = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<AuthService>>();

            var authService = new AuthService(context, mockConfiguration.Object, mockLogger.Object);

            var loginDto = new UsuarioLoginDto { Email = email, Senha = "senhaErrada" };

            // --- 2. Act (Agir) ---
            var resultado = await authService.LoginAsync(loginDto);

            // --- 3. Assert (Verificar) ---
            resultado.Sucesso.Should().BeFalse();
            resultado.TokenOuMensagem.Should().Be("E-mail ou senha inválidos.");
        }

        [Fact]
        public async Task LoginAsync_DeveRetornarSucessoEToken_QuandoCredenciaisCorretas()
        {
            // --- 1. Arrange (Organizar) ---

            // a. Criar um contexto em memória
            var context = CriarContextoEmMemoria("TesteLoginSucessoDB");

            // b. Criar e salvar o usuário de teste
            var email = "usuario.teste@email.com";
            var senhaCorreta = "senhaForte123";
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(senhaCorreta);
            context.Usuarios.Add(new Usuario { Id = 1, Email = email, SenhaHash = senhaHash, Role = "Membro", Nome = "Usuario Teste" });
            await context.SaveChangesAsync();

            // c. Mockar a IConfiguration para fornecer as configurações do JWT
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("Este-eh-um-segredo-muito-longo-e-seguro-que-voce-deve-mudar");
            mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("https://localhost:7123");
            mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("https://localhost:7123");

            var mockLogger = new Mock<ILogger<AuthService>>();

            // d. Criar a instância do serviço com o contexto real e os mocks
            var authService = new AuthService(context, mockConfiguration.Object, mockLogger.Object);

            // e. Criar o DTO de login com as CREDENCIAIS CORRETAS
            var loginDto = new UsuarioLoginDto { Email = email, Senha = senhaCorreta };

            // --- 2. Act (Agir) ---
            var resultado = await authService.LoginAsync(loginDto);

            // --- 3. Assert (Verificar) ---
            resultado.Sucesso.Should().BeTrue();
            resultado.TokenOuMensagem.Should().NotBeNullOrEmpty(); // Garante que um token foi gerado
            resultado.TokenOuMensagem.Should().StartWith("eyJ"); // Tokens JWT sempre começam com "eyJ"
        }
    }
}