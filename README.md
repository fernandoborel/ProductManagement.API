# 🛍️ ProductManagement API

API RESTful para gerenciamento de produtos, desenvolvida com **.NET 10** utilizando arquitetura em camadas, duplo banco de dados (SQL Server + MongoDB) e os padrões **CQRS**, **Mediator** e **Notification**.

---

## 🏗️ Estrutura da Solution

```
ProductManagement/
├── ProductManagement.API/                        # Camada de apresentação (Web API)
│   ├── Controllers/
│   │   └── ProductsController.cs                 # Endpoints REST
│   ├── DTOs/
│   │   ├── Commands/
│   │   │   ├── CreateProductDto.cs               # Payload de criação de produto
│   │   │   ├── UpdateProductDto.cs               # Payload de atualização de produto
│   │   │   └── UpdateStockDto.cs                 # Payload de atualização de estoque
│   │   └── Queries/
│   │       ├── ProductResponseDto.cs             # Resposta de produto
│   │       └── ProductStockResponseDto.cs        # Resposta de estoque do produto
│   ├── RequestHandlers/
│   │   └── ProductRequestHandler.cs              # Handlers MediatR para Commands
│   ├── Notifications/
│   │   ├── ProductNotification.cs                # Modelo de notificação (INotification)
│   │   └── ProductNotificationHandler.cs         # Handler de notificação → grava no MongoDB
│   └── Program.cs                                # Bootstrap da aplicação
│
├── ProductManagement.Infra.Data.SqlServer/       # Camada de dados — SQL Server
│   ├── Entities/
│   │   └── Product.cs                            # Entidade mapeada pelo EF Core
│   ├── Contexts/
│   │   └── SqlServerContext.cs                   # DbContext do Entity Framework
│   └── Migrations/                               # Migrations EF Core
│
└── ProductManagement.Infra.Data.MongoDB/         # Camada de dados — MongoDB
    ├── Documents/
    │   └── ProductDocument.cs                    # Documento mapeado para o MongoDB
    └── Contexts/
        └── MongoDbContext.cs                     # Contexto MongoDB (MongoClient)
```

---

## 🎯 Design Patterns Utilizados

| Padrão | Onde é aplicado |
|---|---|
| **CQRS** (Command Query Responsibility Segregation) | DTOs separados em `Commands` e `Queries`; commands mudam estado, queries apenas leem |
| **Mediator** | `MediatR` desacopla Controllers de handlers; cada DTO de command implementa `IRequest` |
| **Notification** (Publisher/Subscriber) | Após gravar no SQL Server, o handler publica `ProductNotification`; o `ProductNotificationHandler` escuta e replica para o MongoDB |
| **Repository / DbContext** | `SqlServerContext` (EF Core) e `MongoDbContext` encapsulam o acesso a dados |
| **DTO** (Data Transfer Object) | Dados trafegam via DTOs, nunca expondo entidades diretamente |

---

## 🔄 Fluxo da Aplicação

```
HTTP Request
     │
     ▼
ProductsController
     │  mediator.Send(dto)
     ▼
ProductRequestHandler          ← grava no SQL Server via EF Core
     │  mediator.Publish(notification)
     ▼
ProductNotificationHandler     ← replica/sincroniza no MongoDB
```

> O SQL Server é a fonte primária de escrita. O MongoDB é atualizado de forma reativa via **Notification** do MediatR, funcionando como uma camada de leitura/cache.

---

## 🌐 Endpoints

Base URL: `/api/products`

### `POST /api/products`
Cria um novo produto.

**Request Body:**
```json
{
  "name": "string",
  "description": "string",
  "price": 0.00,
  "initialStock": 0
}
```
**Response:** `200 OK`

---

### `PUT /api/products/{id}`
Atualiza os dados de um produto existente.

**Path Param:** `id` (GUID)

**Request Body:**
```json
{
  "name": "string",
  "description": "string",
  "price": 0.00
}
```
**Response:** `200 OK`

---

### `PUT /api/products/{id}/stock`
Atualiza a quantidade em estoque de um produto.

**Path Param:** `id` (GUID)

**Request Body:**
```json
{
  "quantity": 0
}
```
**Response:** `200 OK`

---

### `GET /api/products/{id}`
Retorna os dados de um produto pelo ID.

**Path Param:** `id` (GUID)

**Response:** `200 OK` → `ProductResponseDto`
```json
{
  "id": "guid",
  "name": "string",
  "description": "string",
  "price": 0.00
}
```

---

### `GET /api/products/{id}/stock`
Retorna o produto e a quantidade em estoque.

**Path Param:** `id` (GUID)

**Response:** `200 OK` → `ProductStockResponseDto`
```json
{
  "product": {
    "id": "guid",
    "name": "string",
    "description": "string",
    "price": 0.00
  },
  "stockQuantity": 0
}
```

---

### `GET /api/products?page=1&pageSize=10`
Retorna a lista paginada de produtos.

**Query Params:**
- `page` (int, default: 1)
- `pageSize` (int, default: 10)

**Response:** `200 OK` → `IEnumerable<ProductResponseDto>`

---

## 🗄️ Bancos de Dados

### SQL Server
- ORM: **Entity Framework Core 10.0.6**
- Tabela: `PRODUCTS`
- Fonte primária de escrita e leitura
- Connection string configurada em `appsettings.json` → `ConnectionStrings:ProductsDB`

### MongoDB
- Driver: **MongoDB.Driver 3.7.1**
- Collection: `products`
- Banco: `products-db`
- Atualizado reativamente via Notifications do MediatR
- Configuração em `appsettings.json` → `MongoDbSettings`

---

## ⚙️ Configuração — `appsettings.json`

```json
{
  "ConnectionStrings": {
    "ProductsDB": "Data Source=localhost,1434;Initial Catalog=master;User ID=sa;Password=...;Encrypt=False"
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "products-db"
  }
}
```

---

## 📦 Principais Pacotes NuGet

| Pacote | Versão | Finalidade |
|---|---|---|
| `MediatR` | 14.1.0 | Mediator pattern (CQRS + Notifications) |
| `Microsoft.EntityFrameworkCore` | 10.0.6 | ORM para SQL Server |
| `Microsoft.EntityFrameworkCore.SqlServer` | 10.0.6 | Provider SQL Server |
| `MongoDB.Driver` | 3.7.1 | Driver oficial MongoDB |
| `Swashbuckle.AspNetCore` | 10.1.7 | Swagger UI |
| `Scalar.AspNetCore` | 2.13.22 | Scalar API Reference (tema Mars) |
| `Microsoft.AspNetCore.OpenApi` | 10.0.5 | Geração de documentação OpenAPI |

---

## 🚀 Como Executar

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (porta `1434`) ou Docker
- MongoDB (porta `27017`) ou Docker

### Docker (bancos de dados)
```bash
# SQL Server
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Coti@2026" -p 1434:1433 -d mcr.microsoft.com/mssql/server:2022-latest

# MongoDB
docker run -p 27017:27017 -d mongo
```

### Aplicar Migrations
```bash
cd ProductManagement.API
dotnet ef database update --project ../ProductManagement.Infra.Data.SqlServer
```

### Executar a API
```bash
cd ProductManagement.API
dotnet run
```

### Documentação interativa
| Interface | URL |
|---|---|
| Swagger UI | `https://localhost:{porta}/swagger` |
| Scalar | `https://localhost:{porta}/scalar/v1` |

---

## 🛠️ Tecnologias

- **Linguagem:** C# 13
- **Framework:** ASP.NET Core 10 (.NET 10)
- **Arquitetura:** Layered Architecture + CQRS
- **IDE recomendada:** Visual Studio 2026

---

## 🧪 Testes Unitários — `ProductManagement.Infra.Tests`

Projeto de testes unitários criado para validar o comportamento dos handlers de comandos MediatR.

### Tecnologias utilizadas nos testes

| Pacote | Versão | Finalidade |
|---|---|---|
| `xUnit` | 2.9.3 | Framework de testes |
| `Moq` | 4.20.72 | Mock de dependências (`IMediator`) |
| `Bogus` | 35.6.3 | Geração de dados falsos e realistas |
| `Microsoft.EntityFrameworkCore.InMemory` | 10.0.6 | Banco de dados em memória para testes isolados |

### Estrutura dos testes

```
ProductManagement.Infra.Tests/
├── Fakers/
│   ├── CreateProductDtoFaker.cs    # Gera CreateProductDto com dados falsos via Bogus
│   ├── UpdateProductDtoFaker.cs    # Gera UpdateProductDto com dados falsos via Bogus
│   └── UpdateStockDtoFaker.cs      # Gera UpdateStockDto com dados falsos via Bogus
└── RequestHandlers/
    └── ProductRequestHandlerTests.cs  # 11 testes unitários do ProductRequestHandler
```

### Casos de teste — `ProductRequestHandlerTests`

#### ✅ CreateProduct (3 testes)
| Teste | Descrição |
|---|---|
| `Handle_CreateProduct_ShouldPersistProductAndReturnId` | Verifica que o produto é salvo no banco e retorna um `Guid` válido |
| `Handle_CreateProduct_ShouldPublishNotification` | Verifica que a notificação `CreateProduct` é publicada via MediatR |
| `Handle_CreateProduct_ShouldSetCreatedAtToNow` | Verifica que `CreatedAt` é preenchido com a data/hora atual |

#### ✅ UpdateProduct (4 testes)
| Teste | Descrição |
|---|---|
| `Handle_UpdateProduct_ShouldUpdateFieldsAndReturnId` | Verifica que nome, descrição e preço são atualizados corretamente |
| `Handle_UpdateProduct_ShouldPublishNotification` | Verifica que a notificação `UpdateProduct` é publicada via MediatR |
| `Handle_UpdateProduct_ShouldThrowWhenProductNotFound` | Verifica que `ApplicationException` é lançada quando o produto não existe |
| `Handle_UpdateProduct_ShouldSetModifiedAt` | Verifica que `ModifiedAt` é atualizado após a operação |

#### ✅ UpdateStock (4 testes)
| Teste | Descrição |
|---|---|
| `Handle_UpdateStock_ShouldUpdateStockAndReturnId` | Verifica que o estoque é atualizado corretamente |
| `Handle_UpdateStock_ShouldPublishNotification` | Verifica que a notificação `UpdateStock` é publicada via MediatR |
| `Handle_UpdateStock_ShouldThrowWhenProductNotFound` | Verifica que `ApplicationException` é lançada quando o produto não existe |
| `Handle_UpdateStock_ShouldSetModifiedAt` | Verifica que `ModifiedAt` é atualizado após a operação |

### Executar os testes

```bash
dotnet test ProductManagement.Infra.Tests/ProductManagement.Infra.Tests.csproj
```

