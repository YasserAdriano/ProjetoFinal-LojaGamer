# 🎮 Loja Virtual de Acessórios Gamer (Projeto Final)

Este é o projeto de backend e frontend para a disciplina de Desenvolvimento de Sistemas, criando uma API RESTful e uma interface de administração para um e-commerce de acessórios gamer.

O projeto foi desenvolvido com foco em "Clean Code", arquitetura modular (utilizando Controladores e DTOs) e "Clean Architecture", atendendo a todos os requisitos solicitados, incluindo uma API de autenticação segura, CRUD completo, banco de dados em container e testes automatizados.

## ✒️ Autor

* **Yasser Muhamad Adriano**

## 🛠️ Tecnologias Utilizadas

* **Backend:** C# com .NET 8.0
* **Frontend:** HTML5, CSS3, JavaScript (Vanilla)
* **Banco de Dados:** Microsoft SQL Server (rodando em Docker)
* **ORM:** Entity Framework Core 8.0
* **Testes:** xUnit (Framework de Teste) e Moq (Simulação/Mocking)
* **Autenticação:** JWT (JSON Web Tokens)
* **Documentação:** Swagger (OpenAPI)
* **Containerização:** Docker

## ✅ Requisitos Implementados

* [x] **Backend com API REST:** Criado em C# com .NET 8.
* [x] **Tela e Lógica de Login:** Um frontend em HTML/JS (`http://localhost:8080`) que consome a API.
* [x] **API de Login Segura:** Endpoints `/register` e `/login` com autenticação JWT.
* [x] **CRUD Completo:** API e telas para `Produtos` com Create, Read, Update e Delete.
* [x] **Autorização por Papel (Role-Based):** Apenas usuários "Administrador" podem criar, atualizar ou deletar produtos.
* [x] **Banco de Dados em Container:** O SQL Server roda em um container Docker gerenciado pelo `docker-compose`.
* [x] **ORM:** Uso do Entity Framework Core para todas as operações de banco de dados (Code-First).
* [x] **5 Testes Automatizados:** Projeto de testes (`.Tests`) que valida a lógica de negócios da API.
* [x] **Swagger:** Documentação de API interativa e funcional.
* [x] **Clean Code:** Separação de responsabilidades (Controladores, DTOs, Entidades, Contexto).
* [x] **Tratamento de Erros:** Respostas de API claras (400, 401, 403, 404, 500).

## 🚀 Como Executar o Projeto

Siga estes passos para rodar o projeto localmente. Você precisará de **dois terminais** abertos.

### Pré-requisitos

1.  **SDK do .NET 8.0**
2.  **Docker Desktop** (precisa estar aberto e rodando)
3.  Um editor de código (como VSCode) e um terminal.

### 1. Clonar o Repositório

```bash 
git clone [https://github.com/YasserAdriano/ProjetoFinal-LojaGamer.git](https://github.com/YasserAdriano/ProjetoFinal-LojaGamer.git)
cd ProjetoFinal-LojaGamer
```


### 2. Configurar a Senha do Banco

A API e o Docker precisam usar a **mesma senha** para o banco de dados.

**a) No `docker-compose.yml`:**
Abra o arquivo `docker-compose.yml` (na raiz do projeto) e defina sua senha em `SA_PASSWORD`:

```yaml
services:
  sqlserver:
    environment:
      SA_PASSWORD: "@Suasenha123" # <-- MUDE AQUI PARA SUA SENHA
      ACCEPT_EULA: "Y"
```
**b) No `appsettings.json`:**
Abra o arquivo `LojaGamerApi/appsettings.json` e coloque a **mesma senha** em `DefaultConnection`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=LojaGamerDb;User Id=SA;Password=@Suasenha123;TrustServerCertificate=True"
  },
}
```
### 3. Iniciar o Banco de Dados (Docker) - Terminal 1

No seu primeiro terminal, na pasta **raiz** (`ProjetoFinal-LojaGamer`), rode o comando:

```bash
docker-compose up -d
Aguarde até o container aparecer como "Running" no seu Docker Desktop.
```
### 4. Criar as Tabelas (Migrations) - Terminal 1

# Com o banco rodando, crie as tabelas.

```bash

# Entre na pasta da API
cd LojaGamerApi

# Instale a ferramenta 'dotnet-ef' (se ainda não tiver)
dotnet tool install --global dotnet-ef

# Rode as migrações para criar as tabelas
dotnet ef database update
```
### 5. Executar o Backend (API) - Terminal 1

Agora que o banco está pronto, execute a API:

```bash
# (Ainda na pasta LojaGamerApi)
dotnet run
```
# Deixe este terminal rodando. Ele estará servindo a API em http://localhost:5176.

### 6. Executar o Front-End (Tela de Login) - Terminal 2

1.  Abra um **SEGUNDO TERMINAL** no VSCode.
2.  Navegue até a pasta `frontend`:
    ```bash
    # (Começando da raiz ProjetoFinal-LojaGamer)
    cd frontend
    ```
3.  # Instale o servidor de arquivos (só precisa uma vez):
    ```bash
    dotnet tool install --global dotnet-serve
    ```
4.  # Inicie o servidor do front-end:
    ```bash
    dotnet-serve -p 8080
    ```
# *Deixe este segundo terminal rodando.*

### 7. Acessar a Aplicação

Abra seu navegador e acesse o endereço do **FRONT-END**:

**`http://localhost:8080`**

Você verá a tela de login. O Swagger (documentação da API) ainda está disponível em `http://localhost:5176/swagger`.

## 🧪 Como Rodar os Testes

Para executar os 5 testes automatizados:

1.  Abra um **novo terminal** na pasta raiz (`ProjetoFinal-LojaGamer`).
2. # Navegue até a pasta de testes:
    ```bash
    # cd LojaGamerApi.Tests
    ```
3.  # Execute o comando de teste:
    ```bash
    dotnet test
    ```
4.  # Você deve ver o resultado: `bem-sucedido: 5`.