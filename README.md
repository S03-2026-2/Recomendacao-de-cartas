# Recomendação de Cartas

Serviço de recomendação de jogadas para um jogo de cartas Pokémon. O sistema recebe o estado atual da batalha e retorna uma **dica da melhor jogada**, considerando as efetividades entre os tipos das cartas e as regras de batalha.

O objetivo é auxiliar o jogador durante a partida, identificando as opções com maior vantagem de tipo e apresentando uma explicação para a recomendação.

## Stack

- **.NET 10 / ASP.NET Core** — desenvolvimento da API.
- **C#** — linguagem principal.
- **Entity Framework Core** — ORM e acesso ao banco.
- **PostgreSQL 17** — banco de dados.
- **pgvector** — suporte a armazenamento e busca vetorial.
- **Npgsql** — integração entre .NET e PostgreSQL.
- **Docker Compose** — ambiente local do banco de dados.
- **Scalar** — documentação interativa da API.
- **GitHub Actions** — integração contínua (CI).
- **Postman + Newman** — testes de integração.

## Funcionalidades

### Cadastro de jogadores

Recebe eventos de cadastro enviados pelo Sistema de Jogadores através do endpoint `POST /api/eventos/novo-jogador`.

- Verifica se o jogador já está cadastrado.
- Impede o cadastro de nicknames duplicados.
- Cria o jogador e seu limite inicial de dicas.
- Trata eventos repetidos sem duplicar registros (idempotência).

### Recomendação de jogadas

Recebe o estado da partida através do endpoint `POST /api/dica`.

- Autentica o jogador.
- Verifica se ele está autorizado a solicitar dicas.
- Valida o estado da batalha.
- Consulta as regras relevantes para os tipos presentes na partida.
- Calcula a melhor jogada utilizando uma estratégia baseada em score.
- Monta uma explicação para a recomendação.
- Registra o consumo da dica após sua geração.

### Controle de dicas (Rate Limit)

Cada jogador começa com um limite configurável de **5 dicas por dia**.

- O limite é criado durante o cadastro do jogador.
- A janela e a quantidade de dicas são configuráveis por `RateLimitOptions`.
- Pedidos que ultrapassam o limite recebem `429 Too Many Requests`.
- Pedidos inválidos ou que falham antes da geração não consomem a cota.

### Importação de dados da PokeAPI

Na inicialização em ambiente de desenvolvimento, o serviço consulta a PokeAPI para obter as relações de efetividade entre os tipos de Pokémon.

Os dados são transformados e armazenados no PostgreSQL:

- Efetividades entre tipos, com multiplicadores de dano.
- Regras textuais que descrevem as relações entre os tipos.

Após a importação, a API utiliza os dados armazenados localmente, sem precisar consultar a PokeAPI durante cada recomendação.

## Arquitetura

O projeto utiliza MVC como referência arquitetural, com organização em camadas e aplicação de princípios SOLID.

```text
Controller
    ↓
Service de orquestração
    ↓
Services especializados
    ↓
Repositories
    ↓
PostgreSQL
```

Cada componente possui uma responsabilidade específica:

- **Controllers:** recebem requisições HTTP e retornam respostas.
- **Services:** executam as regras de negócio e coordenam os fluxos.
- **Repositories:** isolam o acesso aos dados.
- **Models:** representam as entidades e os objetos utilizados pelo domínio.
- **Integrations:** encapsulam a comunicação com serviços externos.

O projeto também utiliza os padrões Repository, Strategy, DTO e Options, além de injeção de dependência.

Para mais detalhes, consulte [Arquitetura e Padrões de Projeto](docs/Arquitetura-e-Padroes-de-Projeto.md).

## Requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e em execução.
- [.NET SDK 10](https://dotnet.microsoft.com/download).
- Git.

Não é necessário instalar PostgreSQL ou pgvector manualmente: o banco é executado em um container Docker.

## Rodando localmente

Clone o repositório:

```bash
git clone https://github.com/S03-2026-2/Recomendacao-de-cartas.git
cd Recomendacao-de-cartas
```

Inicie o banco de dados:

```bash
docker compose up -d --wait
```

Execute a API:

```bash
dotnet run --project src/PokemonRecommender
```

A API será iniciada na porta configurada pelo perfil de execução do projeto. A URL aparece no terminal.

Em ambiente de desenvolvimento, a aplicação:

1. Aplica as migrations do Entity Framework Core automaticamente.
2. Verifica se os dados da PokeAPI já foram importados.
3. Realiza a importação quando necessário.
4. Disponibiliza os endpoints da API.

Se a PokeAPI estiver indisponível durante a inicialização, a API continua funcionando, mas a recomendação pode retornar `503 Service Unavailable` até que os dados necessários estejam disponíveis.

## Documentação da API

Com a aplicação em execução no ambiente de desenvolvimento:

- **Scalar:** `http://localhost:<porta>/scalar/v1`
- **OpenAPI:** `http://localhost:<porta>/openapi/v1.json`

A porta depende da configuração do perfil de execução.

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/eventos/novo-jogador` | Recebe o evento de cadastro de um jogador. |
| POST | `/api/dica` | Solicita uma recomendação de jogada. |

### Cadastro de jogador

`POST /api/eventos/novo-jogador`

Exemplo de corpo:

```json
{
  "playerId": "00000000-0000-0000-0000-000000000001",
  "nome": "Jogador",
  "nickname": "Treinador01"
}
```

Respostas possíveis:

| Status | Significado |
|---|---|
| 201 Created | Jogador cadastrado. |
| 200 OK | Jogador já existia; evento repetido. |
| 400 Bad Request | Dados inválidos. |
| 409 Conflict | Nickname já está em uso. |

### Solicitar dica

`POST /api/dica`

A requisição utiliza o cabeçalho `Authorization` com o token do jogador e envia o estado da batalha em JSON.

Exemplo simplificado:

```json
{
  "jogador": {
    "campo": [
      {
        "id": "00000000-0000-0000-0000-000000000002",
        "nome": "Charizard",
        "tipos": ["fire", "flying"],
        "movimentos": []
      }
    ],
    "mao": [],
    "hp": 100
  },
  "oponente": {
    "campo": [
      {
        "id": "00000000-0000-0000-0000-000000000003",
        "nome": "Venusaur",
        "tipos": ["grass", "poison"],
        "movimentos": []
      }
    ],
    "mao": [],
    "hp": 100
  }
}
```

Respostas possíveis:

| Status | Significado |
|---|---|
| 200 OK | Recomendação gerada com sucesso. |
| 401 Unauthorized | Jogador não autenticado. |
| 403 Forbidden | Dicas bloqueadas para o jogador. |
| 422 Unprocessable Entity | Estado da batalha inválido. |
| 429 Too Many Requests | Limite de dicas atingido. |
| 503 Service Unavailable | Dados necessários para a recomendação indisponíveis. |

**Observação sobre autenticação:** atualmente, o token é o próprio GUID do jogador. Essa implementação é provisória, enquanto o formato de autenticação é definido com os outros grupos.

## Banco de dados

O projeto utiliza PostgreSQL 17 com a extensão pgvector.

O ambiente local é configurado pelo `docker-compose.yml`.

Credenciais utilizadas exclusivamente no desenvolvimento local:

| Campo | Valor |
|---|---|
| Host | `localhost` |
| Porta | `5432` |
| Banco | `s03dicas` |
| Usuário | `postgres` |
| Senha | `postgres` |

### Comandos úteis

Verificar o container:

```bash
docker compose ps
```

Consultar os logs:

```bash
docker compose logs db
```

Desligar o banco mantendo os dados:

```bash
docker compose down
```

Desligar e apagar os dados persistidos:

```bash
docker compose down -v
```

Listar as tabelas:

```bash
docker exec -it s03dicas-db psql -U postgres -d s03dicas -c '\dt'
```

Se a porta `5432` já estiver em uso, altere o mapeamento no `docker-compose.yml`, por exemplo:

```yaml
ports:
  - "5433:5432"
```

Nesse caso, atualize também a porta da connection string em `appsettings.Development.json`.

### Migrations

As migrations são aplicadas automaticamente quando a API inicia em ambiente de desenvolvimento.

Para criar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigration --project src/PokemonRecommender
```

Para atualizar o banco manualmente:

```bash
dotnet ef database update --project src/PokemonRecommender
```

## Testes e integração contínua

O projeto possui uma coleção do Postman executada pelo Newman.

Atualmente, os testes de integração do cadastro de jogadores verificam:

1. Cadastro de jogador (`201`).
2. Reenvio do mesmo evento (`200`).
3. Nickname duplicado (`409`).
4. Ausência de nickname (`400`).
5. Identificador vazio (`400`).

A integração contínua é executada pelo GitHub Actions em pushes para `main` e em pull requests.

O workflow executa:

1. Restore das dependências.
2. Build da aplicação em Release.
3. Inicialização de um PostgreSQL 17 com pgvector.
4. Inicialização da API.
5. Execução da coleção Postman pelo Newman.

Na branch `main`, o relatório HTML dos testes também é preparado para publicação no GitHub Pages.

## Estrutura do repositório

```text
.
├── .github/
│   └── workflows/
│       └── ci.yml
├── docs/
│   ├── ADR-0001-limite-de-dicas-v1.pdf
│   ├── DiagramaClassesSOLID.svg
│   ├── Diagrama_casos_de_uso.jpg
│   └── Arquitetura-e-Padroes-de-Projeto.md
├── src/
│   └── PokemonRecommender/
│       ├── Controllers/
│       ├── DTOs/
│       ├── Data/
│       ├── Integrations/
│       ├── Migrations/
│       ├── Models/
│       ├── Repositories/
│       ├── Services/
│       ├── Properties/
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── PokemonRecommender.csproj
│       └── Program.cs
├── tests/
│   └── postman/
│       └── recomendacao-de-cartas.postman_collection.json
├── docker-compose.yml
└── README.md
```

## Documentação complementar

- [Diagrama de classes e SOLID](docs/DiagramaClassesSOLID.svg)
- [Diagrama de casos de uso](docs/Diagrama_casos_de_uso.jpg)
- [ADR-0001: limite de dicas](docs/ADR-0001-limite-de-dicas-v1.pdf)
- [Arquitetura e Padrões de Projeto](docs/README.md)