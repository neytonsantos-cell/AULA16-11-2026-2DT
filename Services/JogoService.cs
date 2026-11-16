

using GestaoJogosEFCore.Models;
using GestaoJogosEFCore.Repositories;

namespace GestaoJogosEFCore.Services
{
    public class JogoService
    {
        private readonly JogoRepository _jogoRepository;
        public JogoService(JogoRepository jogoRepository)
        {
            _jogoRepository = jogoRepository;
        }
        public List<Jogo> Listar() => _jogoRepository.Listar();
        public Jogo? BuscarPorId(int id) => _jogoRepository.BuscarPorId(id);
        public void Cadastrar(Jogo jogo) => _jogoRepository.Adicionar(jogo);
        public bool Atualizar(Jogo jogoAtualizado) => _jogoRepository.Atualizar(jogoAtualizado);
        public bool Excluir(int id) => _jogoRepository.Excluir(id);
        public List<Desenvolvedora> ListarDesenvolvedoras() => _jogoRepository.ListarDesenvolvedoras();
        public void CadastrarDesenvolvedora(Desenvolvedora desenvolvedora) => _jogoRepository.AdicionarDesenvolvedora(desenvolvedora);

    }
}