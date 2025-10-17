//Heliton
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories;

public interface IAlunoRepository : IRepository<Aluno>
{
    // Métodos específicos do domínio
    Task<Aluno?> ObterPorCpf(string cpf);
    Task<bool> CpfJaExiste(string cpf, int? id = null);
    Task<bool> TrocarSenha(int id, string novaSenha);
    // highlight-start
    Task<IEnumerable<Aluno>> Buscar(string termoBusca); // <- ADICIONE ESTA LINHA
    // highlight-end
}