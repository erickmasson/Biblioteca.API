using Biblioteca.API.Data;
using Biblioteca.API.Dtos;
using Biblioteca.API.Models;
using Biblioteca.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Biblioteca.API.Dtos.UsuarioDto;

namespace Biblioteca.API.Tests.Services
{
    public class AuthServiceTests
    {
        // Reintroduzimos nosso método auxiliar para criar um DbSet "mockado" a partir de uma lista
        private Mock<DbSet<T>> CriarMockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(data.GetEnumerator()));
            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(data.Provider));
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }

        [Fact]
        public async Task RegistrarAsync_DeveRetornarFalha_QuandoEmailJaExiste()
        {
            // --- 1. Arrange (Organizar) ---

            // a. Criar os dados falsos que simulam o banco de dados
            var emailExistente = "teste@email.com";
            var usuariosFalsos = new List<Usuario>
            {
                new Usuario { Id = 1, Nome = "Usuário Teste", Email = emailExistente, SenhaHash = "hash123", Role = "Membro" }
            }.AsQueryable();

            // b. Mockar o DbSet<Usuario> usando nossa lista falsa.
            // O AnyAsync vai rodar sobre esta lista.
            var mockDbSetUsuarios = CriarMockDbSet(usuariosFalsos);

            // c. Mockar o DbContext para retornar nosso DbSet falso
            var mockContext = new Mock<BibliotecaContext>(new DbContextOptions<BibliotecaContext>());
            mockContext.Setup(c => c.Usuarios).Returns(mockDbSetUsuarios.Object);

            // d. Mockar as outras dependências
            var mockConfiguration = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<AuthService>>();

            // e. Criar a instância do serviço que vamos testar
            var authService = new AuthService(mockContext.Object, mockConfiguration.Object, mockLogger.Object);

            // f. Criar o DTO de entrada com o e-mail duplicado
            var usuarioDto = new UsuarioRegistroDto { Nome = "Novo Usuario", Email = emailExistente, Senha = "senhaNova" };

            // --- 2. Act (Agir) ---

            // Executar o método
            var resultado = await authService.RegistrarAsync(usuarioDto);

            // --- 3. Assert (Verificar) ---

            // Verificar se o resultado é o esperado
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Be("Este e--mail já está em uso.");
        }
    }
}

#region Helpers para Mock de EF Core Async (Versão Corrigida)
public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;
    public TestAsyncQueryProvider(IQueryProvider inner) { _inner = inner; }
    public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);
    public object Execute(Expression expression) => _inner.Execute(expression);
    public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => Execute<TResult>(expression);
}

public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
    public TestAsyncEnumerable(Expression expression) : base(expression) { }
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;
    public TestAsyncEnumerator(IEnumerator<T> inner) { _inner = inner; }
    public ValueTask DisposeAsync() { _inner.Dispose(); return new ValueTask(); }
    public T Current => _inner.Current;
    public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
}
#endregion