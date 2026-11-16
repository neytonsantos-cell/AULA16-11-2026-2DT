

using GestaoJogosEFCore.Data;
using GestaoJogosEFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoJogosEFCore.Repositories
{
    public class JogoRepository
    {
        private readonly DbContextOptions<GestaoJogosContext> _options;
        public JogoRepository(DbContextOptions<GestaoJogosContext> options)
        {
            _options = options;
        }
        public List<Jogo> Listar()
        {
            using var context = new GestaoJogosContext(_options);
            // Carrega os dados do jogo e a desenvolvedora associada.
            // Include é usado para carregar dados relacionados.
            // ToList() converte o resultado em uma lista.
            return context.Jogo.Include(j => j.IdDesenvolvedoraNavigation).OrderBy(j => j.IdJogo).ToList();
        }
        public Jogo? BuscarPorId(int id)
        {
            using var context = new GestaoJogosContext(_options);
            // FirstOrDefault retorna o primeiro resultado ou null se não encontrar.
            return context.Jogo.Include(j => j.IdDesenvolvedoraNavigation).FirstOrDefault(j => j.IdJogo == id);
        }
        public void Adicionar(Jogo jogo)
        {
            using var context = new GestaoJogosContext(_options);
            context.Jogo.Add(jogo);
            context.SaveChanges();// sem esse comando os dados não são salvos no banco
        }
        public bool Atualizar(Jogo jogoAtualizar)
        {
            using var context = new GestaoJogosContext(_options);
            var jogoExistente = context.Jogo.Find(jogoAtualizar.IdJogo);
            if (jogoExistente is null)
            {
                return false;
            }
            // copia para a entidade rastreada os valores recebidos pelo parametro da função
            jogoExistente.IdDesenvolvedora = jogoAtualizar.IdDesenvolvedora;
            jogoExistente.Nome = jogoAtualizar.Nome;
            jogoExistente.Genero = jogoAtualizar.Genero;
            jogoExistente.AnoLancamento = jogoAtualizar.AnoLancamento;
            jogoExistente.Preco = jogoAtualizar.Preco;
            context.SaveChanges();
            return true;
        }
        public bool Excluir(int id)
        {
            using var context = new GestaoJogosContext(_options);
            var jogoExistente = context.Jogo.Find(id);
            if (jogoExistente is null)
            {
                return false;
            }
            context.Jogo.Remove(jogoExistente);
            context.SaveChanges();
            return true;
        }

        public List<Desenvolvedora> ListarDesenvolvedoras()
        {
             using var context = new GestaoJogosContext(_options);
             return context.Desenvolvedora.OrderBy(d => d.Nome).ToList();
        }
        public void AdicionarDesenvolvedora(Desenvolvedora desenvolvedora)
        {
            using var context = new GestaoJogosContext(_options);
            context.Desenvolvedora.Add(desenvolvedora);
            context.SaveChanges();
        }
    }
}