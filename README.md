# Recomendação de Cartas

Serviço de recomendação de jogadas para um jogo de cartas Pokémon. O jogador envia o estado da partida e recebe uma **dica** da melhor jogada, calculada a partir de regras de batalha.

## Stack

- .NET 10 / ASP.NET Core
- Entity Framework Core + Npgsql
- PostgreSQL 17 com a extensão pgvector
- Docker Compose
- Scalar

## Arquitetura

O projeto segue o diagrama de classes em [`docs/DiagramaClassesSOLID.svg`](docs/DiagramaClassesSOLID.svg), organizado em camadas e guiado por SOLID:

```
Controller → Orquestrador → Services (interfaces) → Repositories (interfaces) → Banco
```

Cada etapa do fluxo tem um service próprio, e todas as dependências são interfaces, o que permite trocar implementações (por exemplo, a estratégia de jogada) sem alterar o resto.

Documentos de apoio, na pasta [`docs/`](docs/):

- [Diagrama de classes](docs/DiagramaClassesSOLID.svg)
- [Diagrama de casos de uso](docs/Diagrama_casos_de_uso.jpg)
- [ADR-0001: limite de dicas](docs/ADR-0001-limite-de-dicas-v1.pdf)

## Requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (aberto)
- [.NET SDK 10](https://dotnet.microsoft.com/download)

Não é preciso instalar Postgres nem pgvector: o banco roda em container.

## Rodando localmente

```bash
git clone https://github.com/S03-2026-2/Recomendacao-de-cartas.git
cd Recomendacao-de-cartas

docker compose up -d --wait
dotnet run --project src/PokemonRecommender
```

O primeiro comando sobe o banco e espera ele ficar pronto. Ao iniciar em ambiente de desenvolvimento, a API cria as tabelas automaticamente (migrations).

A URL da API aparece no terminal (por exemplo, `http://localhost:5123`).

### Documentação da API

Com a API rodando (apenas em desenvolvimento):

- Interface Scalar: `http://localhost:<porta>/scalar/v1`
- Documento OpenAPI: `http://localhost:<porta>/openapi/v1.json`

Por enquanto não há rotas, então a lista de endpoints aparece vazia.

## Banco de dados

Credenciais do `docker-compose.yml`, **só para desenvolvimento local**:

| Campo | Valor |
|---|---|
| Host | `localhost` |
| Porta | `5432` |
| Banco | `s03dicas` |
| Usuário | `postgres` |
| Senha | `postgres` |

Comandos úteis:

```bash
docker compose ps                 # estado do container
docker compose logs db            # logs do Postgres
docker compose down               # desliga (mantém os dados)
docker compose down -v            # desliga e apaga os dados
docker exec -it s03dicas-db psql -U postgres -d s03dicas -c '\dt'   # lista as tabelas
```

Se a porta 5432 já estiver em uso, troque `"5432:5432"` por `"5433:5432"` no `docker-compose.yml` e use `Port=5433` na connection string do `appsettings.Development.json`.

### Migrations

Aplicadas automaticamente ao iniciar a API em desenvolvimento. Para criar uma nova:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add NomeDaMigration --project src/PokemonRecommender
```

## Estrutura do repositório

```
.
├── docs/                         # diagramas e ADRs
├── src/
│   └── PokemonRecommender/
│       ├── Data/                 # AppDbContext
│       ├── Migrations/           # migrations do EF Core
│       ├── Models/               # entidades de domínio
│       ├── Properties/           # launchSettings.json (portas e ambiente)
│       ├── appsettings.json      # configuração base
│       ├── appsettings.Development.json  # connection string local
│       └── Program.cs
├── docker-compose.yml
└── README.md
```

As pastas de controllers, services e repositories entram conforme os fluxos forem implementados.