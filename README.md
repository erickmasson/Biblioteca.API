# Biblioteca.API 📚

API RESTful para um sistema de gerenciamento de uma biblioteca online, desenvolvida com .NET e C#. Este projeto serve como um estudo aprofundado de conceitos de desenvolvimento backend, desde a arquitetura até a segurança e funcionalidades essenciais.

**Status do Projeto:** Em Desenvolvimento 🚀

---
## ✨ Funcionalidades

* **Autenticação e Autorização:** Sistema completo com registro de usuários e login via **Tokens JWT**. Endpoints de modificação são protegidos.
* **Gerenciamento de Livros (CRUD):**
    * Criação de novos livros com **upload de arquivos PDF**.
    * Leitura de todos os livros com **paginação**.
    * Leitura de um livro específico por ID.
    * Atualização dos dados de um livro.
    * Deleção de um livro (e seu respectivo arquivo PDF).
* **Download de Arquivos:** Endpoint para baixar o PDF de um livro específico.
* **Busca e Filtragem:** O endpoint de listagem de livros permite filtrar por gênero e buscar por título ou autor.
* **Arquitetura Limpa:** Lógica de negócio separada da camada de API através de uma **Camada de Serviço (Service Layer)**.
* **Tratamento Global de Exceções:** Um **middleware** customizado garante que a API nunca quebre e sempre retorne respostas de erro padronizadas em JSON.

---
## 🛠️ Tecnologias Utilizadas

* **Framework:** .NET 8
* **Linguagem:** C#
* **API:** ASP.NET Core Web API
* **Banco de Dados:**
    * Entity Framework Core 8
    * SQL Server (via LocalDB)
* **Segurança:**
    * JWT (JSON Web Tokens) para autenticação.
    * BCrypt.Net para hashing de senhas.
* **Documentação da API:** Swagger (Swashbuckle)

---
## 🏛️ Arquitetura

O projeto segue o princípio de **Separação de Responsabilidades**, dividindo a lógica em camadas:

* **Controllers:** Responsáveis por receber as requisições HTTP, validar a entrada e retornar as respostas. Atuam como um "maestro", delegando o trabalho pesado.
* **Services:** Contêm toda a lógica de negócio da aplicação (criar um usuário, salvar um arquivo, etc.).
* **Data (DbContext):** A camada de acesso a dados, gerenciada pelo Entity Framework Core.

---
## Endpoints da API

A URL base da API é `/api`.

| Método | Endpoint                    | Descrição                                 | Protegido? |
| :----- | :-------------------------- | :---------------------------------------- | :--------- |
| `POST` | `/api/usuarios/registrar`   | Registra um novo usuário.                 | 🔓 Público |
| `POST` | `/api/usuarios/login`       | Realiza o login e retorna um token JWT.   | 🔓 Público |
| `GET`  | `/api/livros`               | Lista todos os livros (com paginação/busca). | 🔓 Público |
| `GET`  | `/api/livros/{id}`          | Busca um livro específico pelo ID.        | 🔓 Público |
| `GET`  | `/api/livros/{id}/download` | Baixa o arquivo PDF de um livro.          | 🔓 Público |
| `POST` | `/api/livros`               | Adiciona um novo livro (com upload de PDF). | 🔒 Sim     |
| `PUT`  | `/api/livros/{id}`          | Atualiza os dados de um livro.            | 🔒 Sim     |
| `DELETE`| `/api/livros/{id}`         | Deleta um livro.                          | 🔒 Admin   |

---
## 🚀 Como Executar o Projeto

### Pré-requisitos
* [.NET 8 SDK](https://dotnet.microsoft.com/pt-br/download/dotnet/8.0)
* [SQL Server Express LocalDB](https://learn.microsoft.com/pt-br/sql/database-engine/configure-windows/sql-server-express-localdb) (geralmente instalado com o Visual Studio)
* Um cliente de API como [Postman](https://www.postman.com/) ou usar a interface do Swagger.

### Configuração
1.  **Clone o repositório:**
    ```bash
    git clone [https://github.com/seu-usuario/Biblioteca.API.git](https://github.com/seu-usuario/Biblioteca.API.git)
    ```
2.  **Abra o projeto** em seu editor ou IDE de preferência (ex: Visual Studio, VS Code).

3.  **Configure a Connection String:**
    * Abra o arquivo `appsettings.json`.
    * Verifique se a `DefaultConnection` está configurada corretamente para sua instância do SQL Server LocalDB.
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=BibliotecaDB;Integrated Security=True;"
    }
    ```
4.  **Crie o Banco de Dados:**
    * Abra o **Package Manager Console** no Visual Studio.
    * Execute o comando para aplicar as migrations e criar o banco:
    ```powershell
    Update-Database
    ```

### Execução
1.  **Execute o projeto** pelo Visual Studio (pressionando F5) ou pela linha de comando:
    ```bash
    dotnet run
    ```
2.  A API estará rodando. Para acessar a documentação interativa, abra seu navegador e vá para a URL indicada no console, seguida de `/swagger` (ex: `https://localhost:7123/swagger`).

---
## 👨‍💻 Autor

**Erick Masson**

* [LinkedIn](https://www.linkedin.com/in/erickmasson)
* [GitHub](https://github.com/erickmasson)
