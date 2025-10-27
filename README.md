# FIAP Challenge - Sistema de Gestão Acadêmica (.NET)

API RESTful e aplicação Web (Razor Pages) desenvolvida como parte do desafio técnico da FIAP para gerenciamento de alunos, turmas e matrículas.

## Funcionalidades Principais

* **API RESTful:**
    * CRUD completo para Alunos.
    * CRUD completo para Turmas.
    * Gerenciamento de Matrículas (matricular aluno em turma, listar alunos por turma).
    * Autenticação via JWT (JSON Web Tokens) para acesso seguro.
    * Validações robustas (tamanho de campos, formato de CPF/Email, data de nascimento, senha forte).
    * Paginação e ordenação alfabética nas listagens.
    * Busca de alunos por nome e CPF.
    * Documentação da API com Swagger/OpenAPI.
* **Aplicação Web (Administrativa):**
    * Interface para consumir a API.
    * Login de administrador.
    * Listagem, criação, edição e exclusão de Alunos e Turmas.
    * Matrícula de alunos em turmas.
    * Visualização de alunos matriculados por turma.

## Pré-requisitos

* [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) (ou superior)
* [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (Express, Developer ou outra edição)
* Um SGBD como [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms) ou Azure Data Studio para executar o script inicial do banco.
* [Git](https://git-scm.com/downloads) (para clonar o repositório)
* Opcional: IDE como Visual Studio 2022 ou Visual Studio Code.

## Configuração e Instalação

1.  **Clonar o Repositório:**
    ```bash
    git clone https://github.com/MarcioJnr/Fiap_Challenge.git
    cd Fiap_Challenge
    ```

2.  **Configurar o Banco de Dados:**
    * Abra o SQL Server Management Studio (ou similar).
    * Crie um novo banco de dados (ex: `FiapChallenge`).
    * Abra o arquivo `Fiap/dump.sql`.
    * **Importante:** No script `dump.sql`, certifique-se de que a linha `USE [FiapChallenge]` corresponde ao nome do banco que você criou. Se for diferente, altere no script ou crie o banco com este nome.
    * Execute o script `dump.sql` completo no banco de dados criado. Isso criará as tabelas e inserirá o usuário administrador inicial (`admin@fiap.com.br`).

3.  **Configurar a String de Conexão da API:**
    * Abra o arquivo `FiapChallenge.API/appsettings.json`.
    * Localize a seção `ConnectionStrings`.
    * Ajuste o valor de `DefaultConnection` para corresponder à sua instância do SQL Server e ao banco de dados criado (ex: `"Server=localhost\\SQLEXPRESS01;Database=FiapChallenge;Integrated Security=True;TrustServerCertificate=True"`). Certifique-se de que a autenticação (Integrada ou SQL User/Password) está correta para seu ambiente.

4.  **Configurar a URL da API na Aplicação Web (Opcional - se necessário):**
    * Abra o arquivo `FiapChallenge.Web/appsettings.json`.
    * Verifique se o valor em `ApiSettings:BaseUrl` corresponde à URL base da sua API quando ela estiver rodando (geralmente `https://localhost:7217/api/v1/` conforme configurado no `launchSettings.json` da API).

## Executando a Aplicação

Você precisará executar a API e a Aplicação Web separadamente.

1.  **Executar a API:**
    * Abra um terminal na pasta `FiapChallenge.API`.
    * Execute o comando:
        ```bash
        dotnet run
        ```
    * A API estará disponível (por padrão) em `https://localhost:7217` (verifique o output do terminal para a URL exata).
    * A documentação Swagger estará disponível na raiz: `https://localhost:7217`.

2.  **Executar a Aplicação Web:**
    * Abra **outro** terminal na pasta `FiapChallenge.Web`.
    * Execute o comando:
        ```bash
        dotnet run
        ```
    * A aplicação web estará disponível (por padrão) em `https://localhost:7142` (verifique o output do terminal para a URL exata).

3.  **Acessar a Aplicação:**
    * Abra o navegador na URL da aplicação web (ex: `https://localhost:7142`).
    * Faça login com as credenciais padrão:
        * **Email:** `admin@fiap.com.br`
        * **Senha:** `Admin@123`

## Uso da API (via Swagger)

1.  Navegue até a URL base da API (ex: `https://localhost:7217`).
2.  Use o endpoint `POST /api/v1/Auth/login` para obter um token JWT usando as credenciais de administrador.
3.  Copie o token JWT retornado no corpo da resposta.
4.  Clique no botão "Authorize" no canto superior direito da página Swagger.
5.  Cole o token (sem a palavra "Bearer ") na caixa de texto e clique em "Authorize".
6.  Agora você pode testar os outros endpoints protegidos da API.

## Testes Unitários

O projeto inclui testes unitários para validar a lógica dos serviços da camada de aplicação (`FiapChallenge.Application`). Os testes cobrem as funcionalidades principais dos serviços `AlunoService`, `TurmaService`, `MatriculaService` e `AuthService`.

**Tecnologias Utilizadas nos Testes:**
* **xUnit:** Framework de testes.
* **FluentAssertions:** Biblioteca para asserções mais legíveis.
* **Entity Framework Core In-Memory Database:** Para simular o banco de dados em memória durante os testes, isolando-os do banco real.

**Executando os Testes:**

1.  Abra um terminal na pasta raiz da solução (onde está o arquivo `FiapChallenge.sln`).
2.  Execute o comando:
    ```bash
    dotnet test
    ```
    * Alternativamente, você pode navegar até a pasta do projeto de testes e executar o mesmo comando:
        ```bash
        cd FiapChallenge.Tests
        dotnet test
        ```
3.  O terminal exibirá o resultado da execução dos testes (quantos passaram, falharam ou foram ignorados).

## Estrutura do Projeto

* `FiapChallenge.API`: Projeto ASP.NET Core Web API (controladores, configuração, autenticação).
* `FiapChallenge.Application`: Lógica de negócios, serviços, DTOs, validações (FluentValidation).
* `FiapChallenge.Domain`: Entidades do domínio (Aluno, Turma, Matrícula, Usuario).
* `FiapChallenge.Infrastructure`: Acesso a dados (Entity Framework Core, DbContext).
* `FiapChallenge.Web`: Aplicação Front-end ASP.NET Core Razor Pages.
* `FiapChallenge.Tests`: Testes unitários (xUnit, FluentAssertions, Moq).

## Tecnologias Utilizadas

* .NET 8 (ou superior)
* ASP.NET Core (Web API, Razor Pages)
* Entity Framework Core 8
* SQL Server
* JWT (JSON Web Tokens) para autenticação
* Swagger/OpenAPI para documentação da API
* FluentValidation para validações
* BCrypt.Net-Next para hashing de senhas
* xUnit, FluentAssertions, Moq para testes unitários
* Bootstrap (no front-end)
