@echo off
echo === Resetando Banco de Dados PostgreSQL ===

echo Parando container (se estiver em execução)...
docker stop postgres-frutas

echo Removendo container...
docker rm postgres-frutas

echo Criando novo container PostgreSQL...
docker run --name postgres-frutas -e POSTGRES_PASSWORD=1234 -e POSTGRES_USER=postgres -e POSTGRES_DB=frutasdb -p 5432:5432 -d postgres

echo Aguardando 3 segundos para o PostgreSQL inicializar...
timeout /t 3 /nobreak > nul

echo === Reset do Banco de Dados Concluído ===
echo Agora você pode executar os comandos Add-Migration e Update-Database