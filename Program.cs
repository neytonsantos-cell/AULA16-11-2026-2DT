// Program => 
// Service =>
// Repositories =>
// Context (Models) ou seja, o banco de dados =>
/*
Para visualizar isso facilmente, imagine o funcionamento de um restaurante: cada setor tem uma responsabilidade única para que a cozinha não vire uma bagunça.

1. Program => (A Porta de Entrada e Configuração)
No ecossistema .NET, o Program.cs é o arquivo onde a aplicação "nasce" e é configurada.

O que faz: Prepara o terreno antes de qualquer usuário acessar o sistema. É aqui que configuramos a conexão com o banco de dados, definimos as rotas e registramos a Injeção de Dependência (avisando ao sistema qual Service conversa com qual Repository).

No restaurante: É a abertura das portas, o acender das luzes e a recepção que direciona o pedido do cliente para dentro da cozinha. (Nota: Em APIs maiores, logo após o Program, entra a camada de Controllers, que recebe o pedido específico e o entrega ao Service).

2. Service => (O Cérebro e as Regras de Negócio)
É a camada onde mora a inteligência do seu software. Ela não sabe como os dados são gravados no disco, mas sabe o que deve acontecer com eles.

O que faz: Valida condições, faz cálculos e aplica as regras da empresa. Por exemplo: "Um aluno só pode se matricular se tiver mais de 18 anos e não tiver pendências financeiras". Se estiver tudo certo, o Service passa a ordem adiante.

No restaurante: É o Chef de Cozinha. Ele conhece as receitas, verifica se os ingredientes estão bons e decide a ordem de preparo, mas não vai pessoalmente buscar caixas no estoque.

3. Repositories => (O Intermediário de Dados)
O Repositório funciona como uma ponte isolada entre a regra de negócio e o banco de dados.

O que faz: Executa as operações básicas de leitura e escrita (o famoso CRUD: Criar, Ler, Atualizar e Deletar). Ele esconde a complexidade das consultas (SQL ou LINQ) para que o Service não precise se preocupar com comandos de banco. O Service apenas diz: "Repositório, salve este cliente".

No restaurante: É o Estoquista. Quando o Chef pede farinha, o estoquista sabe exatamente em qual prateleira buscar ou guardar, sem questionar qual bolo será feito.

4. Context (Models) => (O Banco de Dados e seus Moldes)
Aqui chegamos ao destino final da informação, dividido em dois conceitos que trabalham juntos (muito usados no Entity Framework):

Models (Entidades): São as classes que servem de molde para as tabelas. Se você tem uma classe Produto com Id, Nome e Preco, o banco terá uma tabela Produtos com essas exatas colunas.

Context (DbContext): É a sessão ativa com o banco de dados. Ele pega os objetos preenchidos pelos Models, traduz tudo para a linguagem do banco (como SQL Server ou PostgreSQL) e efetiva a gravação no disco.

No restaurante: Os Models são os potes etiquetados (açúcar, sal, farinha) e o Context é a própria despensa física onde tudo fica guardado permanentemente.

O caminho completo na prática
Imagine a ação "Cadastrar um novo usuário":

Program garante que o sistema está no ar e com as peças conectadas.

Service recebe os dados, verifica se o e-mail já não está em uso e se a senha é forte.

Repository recebe o usuário validado e chama o comando de inserção.

Context (Models) encaixa os dados no molde Usuario e grava uma nova linha na tabela do banco de dados.

(E lembre-se: depois que o banco salva, a resposta faz o caminho inverso <= avisando cada camada que deu tudo certo!)*/

// Entity Framework Core => ORM (Mapeador de Objetos Relacionais)
// => Database First
// Code First


using System.Globalization;
using GestaoJogosEFCore.Data;
using GestaoJogosEFCore.Models;
using GestaoJogosEFCore.Repositories;
using GestaoJogosEFCore.Services;
using Microsoft.EntityFrameworkCore;

// Defini a variável com os dados para realizar login no servidor do SQL SERVER
string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=DB_GestaoJogos_2DT;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

// DbContextOptionsBuilder reúne as configurações usadas pelo contexto.
// UseSqlServer seleciona o provedor do SQL Server e recebe a connection string.
var options = new DbContextOptionsBuilder<GestaoJogosContext>()
    .UseSqlServer(connectionString)
    .Options;

// O Program conversa com o serviço; o serviço delega o acesso ao repositório.
var repository = new JogoRepository(options);
var service = new JogoService(repository);

bool executando = true;

// Repete o menu enquanto executando permanecer true.
while (executando)
{
    LimparTela();
    Console.WriteLine("===== GESTÃO DE JOGOS - BANCO DE DADOS =====");
    Console.WriteLine("1 - Cadastrar jogo");
    Console.WriteLine("2 - Listar jogos");
    Console.WriteLine("3 - Consultar jogo");
    Console.WriteLine("4 - Atualizar jogo");
    Console.WriteLine("5 - Excluir jogo");
    Console.WriteLine("6 - Cadastrar desenvolvedora");
    Console.WriteLine("7 - Listar desenvolvedoras");
    Console.WriteLine("0 - Sair");
    Console.Write("\nEscolha uma opção: ");

    // ReadLine pode retornar null se a entrada for encerrada.
    string? opcao = Console.ReadLine();

    // Trata erros esperados para mostrar uma mensagem e manter a aplicação aberta.
    try
    {
        // Compara a opção digitada e chama a função responsável pela ação.
        switch (opcao)
        {
            case "1":
                CadastrarJogo();
                break;
            case "2":
                ListarJogos();
                break;
            case "3":
                ConsultarJogo();
                break;
            case "4":
                AtualizarJogo();
                break;
            case "5":
                ExcluirJogo();
                break;
            case "6":
                CadastrarDesenvolvedora();
                break;
            case "7":
                ListarDesenvolvedoras();
                break;
            case "0":
                // Altera a condição do while para encerrar o menu.
                executando = false;
                break;
            default:
                Console.WriteLine("Opção inválida.");
                Pausar();
                break;
        }
    }
    catch (ArgumentException ex)
    {
        // Exibe erros de validação, como uma desenvolvedora inexistente.
        Console.WriteLine($"\nErro: {ex.Message}");
        Pausar();
    }
}

// Coleta os dados de um jogo e pede ao serviço para cadastrá-lo.
void CadastrarJogo()
{
    LimparTela();
    Console.WriteLine("===== CADASTRAR JOGO =====");

    // O jogo exige uma desenvolvedora existente por causa da chave estrangeira.
    // Any verifica se existe ao menos uma desenvolvedora cadastrada.
    if (!service.ListarDesenvolvedoras().Any())
    {
        Console.WriteLine("Cadastre uma desenvolvedora antes de cadastrar um jogo.");
        Pausar();
        return;
    }

    // A função de leitura monta a entidade; o serviço encaminha o cadastro ao repositório.
    service.Cadastrar(LerDadosJogo());
    Console.WriteLine("\nJogo cadastrado com sucesso!");
    Pausar();
}

// Consulta todos os jogos e apresenta cada resultado.
void ListarJogos()
{
    LimparTela();
    Console.WriteLine("===== JOGOS CADASTRADOS =====");

    // A consulta retorna uma lista de entidades preenchidas pelo EF Core.
    var jogos = service.Listar();
    // Count informa quantos resultados foram retornados.
    if (jogos.Count == 0)
        Console.WriteLine("Nenhum jogo cadastrado.");
    else
        // Percorre a lista e exibe cada jogo.
        foreach (var jogo in jogos)
            ExibirJogo(jogo);

    Pausar();
}

// Busca e exibe um único jogo a partir do ID digitado.
void ConsultarJogo()
{
    LimparTela();
    Console.WriteLine("===== CONSULTAR JOGO =====");
    Console.Write("Informe o ID: ");

    // Converte o ID digitado e consulta o registro no banco.
    int id = int.Parse(Console.ReadLine() ?? "");
    var jogo = service.BuscarPorId(id);

    // null indica que não existe um jogo com o ID informado.
    if (jogo is null)
        Console.WriteLine("Jogo não encontrado.");
    else
        ExibirJogo(jogo);

    Pausar();
}

// Procura o jogo e substitui seus dados pelos valores informados.
void AtualizarJogo()
{
    LimparTela();
    Console.WriteLine("===== ATUALIZAR JOGO =====");
    Console.Write("Informe o ID do jogo: ");

    int id = int.Parse(Console.ReadLine() ?? "");
    // Primeiro confirma que o registro a atualizar existe.
    var jogo = service.BuscarPorId(id);

    if (jogo is null)
    {
        Console.WriteLine("Jogo não encontrado.");
        Pausar();
        return;
    }

    Console.WriteLine("\nInforme os novos dados:");
    // Reaproveita a leitura dos campos usada no cadastro.
    var atualizado = LerDadosJogo();
    // Mantém o ID original para que o repositório atualize o registro correto.
    atualizado.IdJogo = id;

    // O repositório retorna false se o registro não existir mais.
    if (service.Atualizar(atualizado))
        Console.WriteLine("\nJogo atualizado com sucesso!");
    else
        Console.WriteLine("\nJogo não encontrado.");

    Pausar();
}

// Solicita ao serviço a exclusão do jogo indicado pelo ID.
void ExcluirJogo()
{
    LimparTela();
    Console.WriteLine("===== EXCLUIR JOGO =====");
    Console.Write("Informe o ID: ");

    int id = int.Parse(Console.ReadLine() ?? "");
    // O resultado informa se havia um jogo com esse ID para excluir.
    if (service.Excluir(id))
        Console.WriteLine("Jogo excluído com sucesso.");
    else
        Console.WriteLine("Jogo não encontrado.");

    Pausar();
}

// Lê os campos e cadastra uma nova desenvolvedora.
void CadastrarDesenvolvedora()
{
    LimparTela();
    Console.WriteLine("===== CADASTRAR DESENVOLVEDORA =====");
    Console.Write("Nome: ");
    string nome = Console.ReadLine() ?? "";
    Console.Write("País: ");
    string pais = Console.ReadLine() ?? "";

    // A desenvolvedora é gravada antes de poder ser associada a um jogo.
    service.CadastrarDesenvolvedora(new Desenvolvedora
    {
        Nome = nome,
        Pais = pais
    });

    Console.WriteLine("\nDesenvolvedora cadastrada com sucesso!");
    Pausar();
}

// Consulta e exibe as desenvolvedoras disponíveis no banco.
void ListarDesenvolvedoras()
{
    LimparTela();
    Console.WriteLine("===== DESENVOLVEDORAS CADASTRADAS =====");

    var desenvolvedoras = service.ListarDesenvolvedoras();
    if (desenvolvedoras.Count == 0)
        Console.WriteLine("Nenhuma desenvolvedora cadastrada.");
    else
        // Percorre a lista e apresenta os dados de cada desenvolvedora.
        foreach (var desenvolvedora in desenvolvedoras)
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine($"ID: {desenvolvedora.IdDesenvolvedora}");
            Console.WriteLine($"Nome: {desenvolvedora.Nome}");
            Console.WriteLine($"País: {desenvolvedora.Pais}");
        }

    Pausar();
}

// Lê os dados do jogo e valida sua desenvolvedora antes de criar a entidade.
Jogo LerDadosJogo()
{
    // Carrega a lista para exibir opções válidas e validar o ID escolhido.
    var desenvolvedoras = service.ListarDesenvolvedoras();
    Console.WriteLine("\nDesenvolvedoras:");
    // Exibe o ID, que será armazenado como chave estrangeira no jogo.
    foreach (var desenvolvedora in desenvolvedoras)
        Console.WriteLine($"{desenvolvedora.IdDesenvolvedora} - {desenvolvedora.Nome}");

    Console.Write("ID da desenvolvedora: ");
    int idDesenvolvedora = int.Parse(Console.ReadLine() ?? "");
    // Any percorre a lista; d representa cada item testado pela condição.
    if (!desenvolvedoras.Any(d => d.IdDesenvolvedora == idDesenvolvedora))
        throw new ArgumentException("A desenvolvedora informada não existe.");

    Console.Write("Nome do jogo: ");
    string nome = Console.ReadLine() ?? "";
    Console.Write("Gênero: ");
    string genero = Console.ReadLine() ?? "";
    Console.Write("Ano de lançamento: ");
    int ano = int.Parse(Console.ReadLine() ?? "");
    Console.Write("Preço: ");
    // Normaliza a vírgula decimal para que a conversão seja independente da região.
    string valor = (Console.ReadLine() ?? "").Replace(',', '.');
    // InvariantCulture interpreta o ponto como separador decimal.
    decimal preco = decimal.Parse(valor, CultureInfo.InvariantCulture);

    // Cria a entidade que será recebida pelo serviço e pelo repositório.
    return new Jogo
    {
        IdDesenvolvedora = idDesenvolvedora,
        Nome = nome,
        Genero = genero,
        AnoLancamento = ano,
        Preco = preco
    };
}

// Apresenta os dados do jogo e da desenvolvedora relacionada.
void ExibirJogo(Jogo jogo)
{
    Console.WriteLine("------------------------------");
    Console.WriteLine($"ID: {jogo.IdJogo}");
    Console.WriteLine($"Nome: {jogo.Nome}");
    Console.WriteLine($"Desenvolvedora: {jogo.IdDesenvolvedoraNavigation.Nome}");
    Console.WriteLine($"Gênero: {jogo.Genero}");
    Console.WriteLine($"Ano: {jogo.AnoLancamento}");
    Console.WriteLine($"Preço: {jogo.Preco:C}");
}

// Aguarda para que o resultado continue visível antes de retornar ao menu.
void Pausar()
{
    Console.WriteLine("\nPressione ENTER para continuar...");
    Console.ReadLine();
}

// Evita falha ao limpar a tela quando a saída está redirecionada, como em testes.
void LimparTela()
{
    if (!Console.IsOutputRedirected)
        Console.Clear();
}
