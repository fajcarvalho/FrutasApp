using FrutasApp.Data;
using FrutasApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace FrutasApp
{
    class Program
    {
        // Metodo Main
        static async Task Main(string[] args)
        {
            InicializarDados();

            using (var context = new AppDbContext())
            {
                // Exibir o menu e processar as opções do usuário
                await ExibirMenuEProcessarOpcoes(context);
            }

            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        // Método para inicializar dados
        static void InicializarDados()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    Console.WriteLine("Verificando banco de dados...");

                    // Verifica se já existem frutas (ignora se categorias existem ou não)
                    if (!context.Frutas.Any())
                    {
                        Console.WriteLine("Inicializando dados...");

                        // Verificar se precisamos criar categorias
                        if (!context.Categorias.Any())
                        {
                            Console.WriteLine("Criando categorias...");

                            // Adiciona categorias
                            var categorias = new[]
                            {
                                new Categoria { Nome = "Cítricas", Descricao = "Frutas com alto teor de ácido cítrico" },
                                new Categoria { Nome = "Tropicais", Descricao = "Frutas típicas de regiões tropicais" },
                                new Categoria { Nome = "Frutas Vermelhas", Descricao = "Frutas pequenas de cor vermelha ou roxa" }
                            };

                            context.Categorias.AddRange(categorias);
                            context.SaveChanges();
                            Console.WriteLine("Categorias criadas com sucesso!");
                        }
                        else
                        {
                            Console.WriteLine("Categorias já existem, pulando criação de categorias.");
                        }

                        // Buscar IDs das categorias (se elas já existiam ou foram recém-criadas)
                        Console.WriteLine("Buscando categorias existentes...");
                        var categoriaCitrica = context.Categorias.FirstOrDefault(c => c.Nome == "Cítricas");
                        var categoriaTropical = context.Categorias.FirstOrDefault(c => c.Nome == "Tropicais");
                        var categoriaVermelha = context.Categorias.FirstOrDefault(c => c.Nome == "Frutas Vermelhas");

                        // Verificação extra de segurança para garantir que as categorias existam
                        if (categoriaCitrica == null || categoriaTropical == null || categoriaVermelha == null)
                        {
                            Console.WriteLine("AVISO: Algumas categorias não foram encontradas. Verificando detalhes...");

                            // Exibir todas as categorias existentes para debug
                            var todasCategorias = context.Categorias.ToList();
                            Console.WriteLine($"Total de categorias no banco: {todasCategorias.Count}");
                            foreach (var cat in todasCategorias)
                            {
                                Console.WriteLine($"ID: {cat.Id}, Nome: '{cat.Nome}'");
                            }

                            // Criar as categorias faltantes se necessário
                            if (categoriaCitrica == null)
                            {
                                categoriaCitrica = new Categoria { Nome = "Cítricas", Descricao = "Frutas com alto teor de ácido cítrico" };
                                context.Categorias.Add(categoriaCitrica);
                            }

                            if (categoriaTropical == null)
                            {
                                categoriaTropical = new Categoria { Nome = "Tropicais", Descricao = "Frutas típicas de regiões tropicais" };
                                context.Categorias.Add(categoriaTropical);
                            }

                            if (categoriaVermelha == null)
                            {
                                categoriaVermelha = new Categoria { Nome = "Frutas Vermelhas", Descricao = "Frutas pequenas de cor vermelha ou roxa" };
                                context.Categorias.Add(categoriaVermelha);
                            }

                            context.SaveChanges();
                            Console.WriteLine("Categorias faltantes foram criadas.");
                        }

                        Console.WriteLine("Adicionando frutas...");
                        // Adiciona frutas
                        var frutas = new[]
                        {
                            new Fruta
                            {
                                Nome = "Abacaxi",
                                Cor = "Amarelo",
                                Peso = 1.5,
                                Sabor = SaborFruta.Doce,
                                CategoriaId = categoriaTropical.Id
                            },
                            new Fruta
                            {
                                Nome = "Limão",
                                Cor = "Verde",
                                Peso = 0.2,
                                Sabor = SaborFruta.Azedo,
                                CategoriaId = categoriaCitrica.Id
                            },
                            new Fruta
                            {
                                Nome = "Morango",
                                Cor = "Vermelho",
                                Peso = 0.02,
                                Sabor = SaborFruta.Doce,
                                CategoriaId = categoriaVermelha.Id
                            },
                        };

                        context.Frutas.AddRange(frutas);
                        context.SaveChanges();

                        Console.WriteLine("Frutas adicionadas com sucesso!");
                        Console.WriteLine("Inicialização concluída com sucesso!\n");
                    }
                    else
                    {
                        Console.WriteLine("O banco de dados já contém frutas. Pulando inicialização de dados.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("====================================================");
                Console.WriteLine($"ERRO ao inicializar dados: {ex.Message}");
                Console.WriteLine($"Tipo de exceção: {ex.GetType().Name}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Exceção interna: {ex.InnerException.Message}");
                }

                Console.WriteLine("====================================================");
                Console.WriteLine("Pressione qualquer tecla para continuar mesmo assim...");
                Console.ReadKey();
            }
        }

        // Método para exibir o menu e processar as operações CRUD
        static async Task ExibirMenuEProcessarOpcoes(AppDbContext context)
        {
            bool sair = false;

            while (!sair)
            {
                Console.WriteLine("\n========= GERENCIADOR DE FRUTAS =========");
                Console.WriteLine("1. Listar todas as frutas");
                Console.WriteLine("2. Buscar fruta por ID");
                Console.WriteLine("3. Buscar frutas por sabor");
                Console.WriteLine("4. Buscar frutas por categoria");
                Console.WriteLine("5. Adicionar nova fruta");
                Console.WriteLine("6. Atualizar fruta existente");
                Console.WriteLine("7. Excluir fruta");
                Console.WriteLine("8. Limpar banco de dados e reinicializar");
                Console.WriteLine("9. Exibir estatísticas de frutas");
                Console.WriteLine("10. Sair");
                Console.Write("\nEscolha uma opção: ");

                if (int.TryParse(Console.ReadLine(), out int opcao))
                {
                    Console.WriteLine();

                    switch (opcao)
                    {
                        case 1:
                            await ListarFrutas(context);
                            break;
                        case 2:
                            BuscarFrutaPorId(context);
                            break;
                        case 3:
                            BuscarFrutaPorSabor(context);
                            break;
                        case 4:
                            BuscarFrutaPorCategoria(context);
                            break;
                        case 5:
                            AdicionarFruta(context);
                            break;
                        case 6:
                            AtualizarFruta(context);
                            break;
                        case 7:
                            ExcluirFruta(context);
                            break;
                        case 8:
                            LimparBancoDados(context);
                            break;
                        case 9:
                            ExibirEstatisticas(context);
                            break;
                        case 10:
                            sair = true;
                            break;
                        default:
                            Console.WriteLine("Opção inválida. Tente novamente.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Entrada inválida. Tente novamente.");
                }
            }
        }

        // CREATE - Adicionar nova fruta
        static void AdicionarFruta(AppDbContext context)
        {
            // Coletar informações da nova fruta
            Console.Write("Nome da fruta: ");
            string nome = Console.ReadLine();

            Console.Write("Cor: ");
            string cor = Console.ReadLine();

            Console.Write("Peso (kg): ");
            double.TryParse(Console.ReadLine(), //Tratar entrada com '.' e ','
                System.Globalization.NumberStyles.Any, 
                System.Globalization.CultureInfo.InvariantCulture, 
                out double peso);

            // Mostrar opções de sabor
            Console.WriteLine("Sabores disponíveis:");
            foreach (SaborFruta saborEnum in Enum.GetValues(typeof(SaborFruta)))
            {
                Console.WriteLine($"{(int)saborEnum}. {saborEnum}");
            }
            Console.Write("Escolha o sabor (número): ");
            int.TryParse(Console.ReadLine(), out int saborInt);
            SaborFruta sabor = (SaborFruta)saborInt;

            // Listar categorias disponíveis
            var categorias = context.Categorias.ToList();
            Console.WriteLine("Categorias disponíveis:");
            foreach (var cat in categorias)
            {
                Console.WriteLine($"{cat.Id}. {cat.Nome}");
            }
            Console.Write("ID da categoria: ");
            int.TryParse(Console.ReadLine(), out int categoriaId);

            // Criar e adicionar a nova fruta
            var fruta = new Fruta
            {
                Nome = nome,
                Cor = cor,
                Peso = peso,
                Sabor = sabor,
                CategoriaId = categoriaId
            };

            context.Frutas.Add(fruta);
            context.SaveChanges();

            Console.WriteLine($"Fruta '{nome}' adicionada com sucesso! ID: {fruta.Id}");
        }

        // READ - Listar todas as frutas
        static async Task ListarFrutas(AppDbContext context)
        {
            // Buscar todas as frutas incluindo suas categorias
            var frutas = await context.Frutas
                .Include(f => f.Categoria)
                .OrderBy(f => f.Id)
                .ToListAsync();

            if (!frutas.Any())
            {
                Console.WriteLine("Nenhuma fruta cadastrada.");
                return;
            }

            Console.WriteLine("Lista de frutas:");
            foreach (var fruta in frutas)
            {
                Console.WriteLine($"ID: {fruta.Id} | Nome: {fruta.Nome} | Cor: {fruta.Cor} | " +
                         $"Peso: {fruta.Peso}kg | Sabor: {fruta.Sabor} | " +
                         $"Categoria: {fruta.Categoria?.Nome ?? "N/A"}");
            }
        }

        // READ - Buscar fruta por ID
        static void BuscarFrutaPorId(AppDbContext context)
        {
            Console.Write("Digite o ID da fruta: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                // Find busca pela chave primária
                var fruta = context.Frutas
                    .Include(f => f.Categoria)
                    .FirstOrDefault(f => f.Id == id);

                if (fruta != null)
                {
                    Console.WriteLine($"Fruta encontrada:");
                    Console.WriteLine($"ID: {fruta.Id} | Nome: {fruta.Nome} | Cor: {fruta.Cor} | " +
                                     $"Peso: {fruta.Peso}kg | Sabor: {fruta.Sabor} | " +
                                     $"Categoria: {fruta.Categoria?.Nome ?? "N/A"}");
                }
                else
                {
                    Console.WriteLine($"Fruta com ID {id} não encontrada.");
                }
            }
            else
            {
                Console.WriteLine("ID inválido.");
            }
        }

        // READ - Buscar frutas por sabor
        static void BuscarFrutaPorSabor(AppDbContext context)
        {
            // Mostrar opções de sabor
            Console.WriteLine("Sabores disponíveis:");
            foreach (SaborFruta saborEnum in Enum.GetValues(typeof(SaborFruta)))
            {
                Console.WriteLine($"{(int)saborEnum}. {saborEnum}");
            }
            Console.Write("Escolha o sabor (número): ");

            if (int.TryParse(Console.ReadLine(), out int saborInt))
            {
                SaborFruta sabor = (SaborFruta)saborInt;

                // Where aplica um filtro à consulta
                var frutas = context.Frutas
                    .Include(f => f.Categoria)
                    .Where(f => f.Sabor == sabor)
                    .ToList();

                if (frutas.Any())
                {
                    Console.WriteLine($"Frutas com sabor {sabor}:");
                    foreach (var fruta in frutas)
                    {
                        Console.WriteLine($"ID: {fruta.Id} | Nome: {fruta.Nome} | Categoria: {fruta.Categoria?.Nome ?? "N/A"}");
                    }
                }
                else
                {
                    Console.WriteLine($"Nenhuma fruta com sabor {sabor} encontrada.");
                }
            }
            else
            {
                Console.WriteLine("Opção inválida.");
            }
        }

        // READ - Buscar frutas por categoria
        static void BuscarFrutaPorCategoria(AppDbContext context)
        {
            // Listar categorias disponíveis
            var categorias = context.Categorias.ToList();
            Console.WriteLine("Categorias disponíveis:");
            foreach (var cat in categorias)
            {
                Console.WriteLine($"{cat.Id}. {cat.Nome}");
            }

            Console.Write("Digite o ID da categoria: ");
            if (int.TryParse(Console.ReadLine(), out int categoriaId))
            {
                // Buscar a categoria pelo ID
                var categoria = context.Categorias
                    .Include(c => c.Frutas)  // Inclui as frutas relacionadas
                    .FirstOrDefault(c => c.Id == categoriaId);

                if (categoria != null)
                {
                    if (categoria.Frutas.Any())
                    {
                        Console.WriteLine($"Frutas da categoria '{categoria.Nome}':");
                        foreach (var fruta in categoria.Frutas)
                        {
                            Console.WriteLine($"ID: {fruta.Id} | Nome: {fruta.Nome} | Sabor: {fruta.Sabor}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Nenhuma fruta encontrada na categoria '{categoria.Nome}'.");
                    }
                }
                else
                {
                    Console.WriteLine($"Categoria com ID {categoriaId} não encontrada.");
                }
            }
            else
            {
                Console.WriteLine("ID inválido.");
            }
        }

        // UPDATE - Atualizar uma fruta existente
        static void AtualizarFruta(AppDbContext context)
        {
            Console.Write("Digite o ID da fruta a ser atualizada: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var fruta = context.Frutas.Find(id);

                if (fruta != null)
                {
                    Console.WriteLine($"Atualizando fruta: {fruta.Nome}");

                    Console.Write($"Novo nome (atual: {fruta.Nome}): ");
                    string nome = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(nome))
                    {
                        fruta.Nome = nome;
                    }

                    Console.Write($"Nova cor (atual: {fruta.Cor}): ");
                    string cor = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(cor))
                    {
                        fruta.Cor = cor;
                    }

                    // Mostrar opções de sabor
                    Console.WriteLine($"Novo sabor (atual: {fruta.Sabor}):");
                    foreach (SaborFruta sabor in Enum.GetValues(typeof(SaborFruta)))
                    {
                        Console.WriteLine($"{(int)sabor}. {sabor}");
                    }
                    Console.Write("Escolha o sabor (número): ");
                    string saborStr = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(saborStr) && int.TryParse(saborStr, out int saborInt))
                    {
                        fruta.Sabor = (SaborFruta)saborInt;
                    }

                    // Listar categorias disponíveis
                    var categorias = context.Categorias.ToList();
                    Console.WriteLine($"Nova categoria (atual: {fruta.CategoriaId}):");
                    foreach (var cat in categorias)
                    {
                        Console.WriteLine($"{cat.Id}. {cat.Nome}");
                    }
                    Console.Write("ID da nova categoria: ");
                    string categoriaIdStr = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(categoriaIdStr) && int.TryParse(categoriaIdStr, out int categoriaId))
                    {
                        fruta.CategoriaId = categoriaId;
                    }

                    context.SaveChanges();
                    Console.WriteLine($"Fruta '{fruta.Nome}' atualizada com sucesso!");
                }
                else
                {
                    Console.WriteLine($"Fruta com ID {id} não encontrada.");
                }
            }
            else
            {
                Console.WriteLine("ID inválido.");
            }
        }

        // DELETE - Excluir uma fruta
        static void ExcluirFruta(AppDbContext context)
        {
            Console.Write("Digite o ID da fruta a ser excluída: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var fruta = context.Frutas.Find(id);

                if (fruta != null)
                {
                    Console.WriteLine($"Tem certeza que deseja excluir a fruta '{fruta.Nome}'? (S/N)");
                    if (Console.ReadLine().ToUpper() == "S")
                    {
                        context.Frutas.Remove(fruta);
                        context.SaveChanges();
                        Console.WriteLine("Fruta excluída com sucesso!");
                    }
                    else
                    {
                        Console.WriteLine("Operação cancelada.");
                    }
                }
                else
                {
                    Console.WriteLine($"Fruta com ID {id} não encontrada.");
                }
            }
            else
            {
                Console.WriteLine("ID inválido.");
            }
        }

        // Método para limpar o banco de dados e reinicializar os IDs
        static void LimparBancoDados(AppDbContext context)
        {
            Console.WriteLine("ATENÇÃO: Esta operação irá excluir TODOS os dados do banco!");
            Console.WriteLine("Digite 'CONFIRMAR' para prosseguir ou qualquer outra coisa para cancelar:");

            string confirmacao = Console.ReadLine();

            if (confirmacao != "CONFIRMAR")
            {
                Console.WriteLine("Operação cancelada.");
                return;
            }

            try
            {
                // 1. Excluir todos os dados (cuidado com a ordem devido às chaves estrangeiras)
                Console.WriteLine("Removendo todas as frutas...");
                context.Frutas.RemoveRange(context.Frutas.ToList());
                context.SaveChanges();

                Console.WriteLine("Removendo todas as categorias...");
                context.Categorias.RemoveRange(context.Categorias.ToList());
                context.SaveChanges();

                // 2. Resetar as sequências usando SQL direto
                Console.WriteLine("Resetando sequências de IDs...");

                // Executar SQL direto para resetar as sequências
                context.Database.ExecuteSqlRaw("ALTER SEQUENCE \"Frutas_Id_seq\" RESTART WITH 1");
                context.Database.ExecuteSqlRaw("ALTER SEQUENCE \"Categorias_Id_seq\" RESTART WITH 1");

                Console.WriteLine("Banco de dados limpo com sucesso!");

                // 3. Reinicializar os dados padrão
                Console.WriteLine("Reinicializando dados padrão...");
                InicializarDados();

                Console.WriteLine("Operação concluída com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO ao limpar o banco de dados: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Detalhe: {ex.InnerException.Message}");
                }
            }

            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        static void ExibirEstatisticas(AppDbContext context)
        {
            Console.WriteLine("\n=== ESTATÍSTICAS DE FRUTAS ===\n");

            // Total de frutas no banco
            int totalFrutas = context.Frutas.Count();
            Console.WriteLine($"Total de frutas cadastradas: {totalFrutas}");

            if (totalFrutas == 0)
            {
                Console.WriteLine("Não há frutas suficientes para calcular estatísticas.");
                return;
            }

            // Peso médio das frutas
            double pesoMedio = context.Frutas.Average(f => f.Peso);
            Console.WriteLine($"Peso médio das frutas: {pesoMedio:F2} kg");

            // Distribuição por sabor
            Console.WriteLine("\nDistribuição por sabor:");
            var distribuicaoPorSabor = context.Frutas
                .GroupBy(f => f.Sabor)
                .Select(g => new { Sabor = g.Key, Quantidade = g.Count() })
                .ToList();

            foreach (var item in distribuicaoPorSabor)
            {
                Console.WriteLine($"- {item.Sabor}: {item.Quantidade} fruta(s) ({(double)item.Quantidade / totalFrutas * 100:F1}%)");
            }


            // Frutas por categoria
            Console.WriteLine("\nFrutas por categoria:");
            var frutasPorCategoria = context.Frutas
                .Include(f => f.Categoria)
                .GroupBy(f => f.Categoria.Nome)
                .Select(g => new { Categoria = g.Key, Quantidade = g.Count() })
                .ToList();

            foreach (var item in frutasPorCategoria)
            {
                Console.WriteLine($"- {item.Categoria}: {item.Quantidade} fruta(s)");
            }
        }
    }
}