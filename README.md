# FrutasApp

Um aplicativo de console C# para gerenciamento de frutas e categorias, desenvolvido como projeto de aprendizado com Entity Framework Core e PostgreSQL.

## Tecnologias Utilizadas

- C# .NET
- Entity Framework Core
- PostgreSQL em Docker
- Padrão Repository
- Relacionamentos 1:N entre Categorias e Frutas

## Funcionalidades

- CRUD completo de Frutas e Categorias
- Filtros por sabor e categoria
- Persistência de dados em PostgreSQL
- Interface de console interativa

## Configuração

1. Instale Docker Desktop
2. Execute o container PostgreSQL com o comando:
docker run --name postgres-frutas -e POSTGRES_PASSWORD=1234 -e POSTGRES_USER=postgres -e POSTGRES_DB=frutasdb -p 5432:5432 -d postgres
3. Execute o programa