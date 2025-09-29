//HELITON
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;
using AcademiaDoZe.Infrastructure.Tests;

namespace AcademiaDoZe.Infrastructure.Tests;

public class MatriculaInfrastructureTests : TestBase
{
    [Fact]
    public async Task Matricula_Adicionar()
    {
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
        var logradouro = await repoLogradouro.ObterPorId(4);
        Assert.NotNull(logradouro);

        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });

        // CPF único e válido com exatamente 11 dígitos
        var random = new Random();
        var _cpf = $"116201{random.Next(100, 999):D3}98";

        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);

        // Remove aluno antigo com mesmo CPF se existir
        var alunoExistente = await repoAluno.ObterPorCpf(_cpf);
        if (alunoExistente != null)
            await repoAluno.Remover(alunoExistente.Id);

        var aluno = Aluno.Criar(
            1,
            "Aluno Teste",
            _cpf,
            new DateOnly(2010, 10, 09),
            "49999999999",
            "aluno@teste.com",
            logradouro!,
            "123",
            "Complemento casa",
            "Senha@123",
            arquivo
        );
        await repoAluno.Adicionar(aluno);

        var matricula = Matricula.Criar(
            1,
            aluno,
            EMatriculaPlano.Semestral,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
            "Emagrecer",
            EMatriculaRestricoes.Alergias,
            arquivo,
            "Sem observações"
        );

        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);
        var matriculaInserida = await repoMatricula.Adicionar(matricula);

        Assert.NotNull(matriculaInserida);
        Assert.True(matriculaInserida.Id > 0);
    }


    [Fact]
    public async Task Matricula_ObterPorAluno_Atualizar()
    {
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
        var logradouro = await repoLogradouro.ObterPorId(4);
        Assert.NotNull(logradouro);

        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);
        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);

        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });

        // CPF único para o teste
        var random = new Random();
        var _cpf = $"111406{random.Next(100, 999):D3}81";

        // Remove aluno antigo se existir
        var alunoExistente = await repoAluno.ObterPorCpf(_cpf);
        if (alunoExistente != null)
            await repoAluno.Remover(alunoExistente.Id);

        // Cria aluno novo
        var aluno = Aluno.Criar(
            1,
            "Aluno Teste Atualizar",
            _cpf,
            new DateOnly(2010, 10, 09),
            "49999999999",
            "aluno@teste.com",
            logradouro!,
            "123",
            "Complemento casa",
            "Senha@123",
            arquivo
        );
        await repoAluno.Adicionar(aluno);

        // Cria matrícula para esse aluno
        var matricula = Matricula.Criar(
            1,
            aluno,
            EMatriculaPlano.Semestral,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
            "Emagrecer",
            EMatriculaRestricoes.Alergias,
            arquivo,
            "Sem observações"
        );
        await repoMatricula.Adicionar(matricula);

        // Act
        var matriculas = (await repoMatricula.ObterPorAluno(aluno.Id)).ToList();

        Assert.NotEmpty(matriculas);

        var matriculaParaAtualizar = matriculas.First();

        // Atualiza matrícula
        var matriculaAtualizada = Matricula.Criar(
            1,
            aluno,
            EMatriculaPlano.Anual,
            new DateOnly(2020, 05, 20),
            new DateOnly(2020, 05, 20).AddMonths(12),
            "Hipertrofia",
            EMatriculaRestricoes.Alergias,
            arquivo,
            "Observação atualizada"
        );

        typeof(Entity).GetProperty("Id")?.SetValue(matriculaAtualizada, matriculaParaAtualizar.Id);

        var resultado = await repoMatricula.Atualizar(matriculaAtualizada);

        // Assert final
        Assert.NotNull(resultado);
        Assert.Equal("Hipertrofia", resultado.Objetivo);
        Assert.Equal("Observação atualizada", resultado.ObservacoesRestricoes);
        Assert.Equal(EMatriculaPlano.Anual, resultado.Plano);
    }


    [Fact]
    public async Task Matricula_ObterPorAluno_Remover_ObterPorId()
    {
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
        var logradouro = await repoLogradouro.ObterPorId(4);
        Assert.NotNull(logradouro);

        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });

        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);
        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);

        // CPF único para o teste
        var random = new Random();
        var _cpf = $"111406{random.Next(100, 999):D3}81";

        // Remove aluno antigo se existir
        var alunoExistente = await repoAluno.ObterPorCpf(_cpf);
        if (alunoExistente != null)
            await repoAluno.Remover(alunoExistente.Id);

        // Cria aluno novo
        var aluno = Aluno.Criar(
            1,
            "Aluno Teste Remover",
            _cpf,
            new DateOnly(2010, 10, 09),
            "49999999999",
            "aluno@teste.com",
            logradouro!,
            "123",
            "Complemento casa",
            "Senha@123",
            arquivo
        );
        await repoAluno.Adicionar(aluno);

        // Cria matrícula para esse aluno
        var matricula = Matricula.Criar(
            1,
            aluno,
            EMatriculaPlano.Semestral,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
            "Emagrecer",
            EMatriculaRestricoes.Alergias,
            arquivo,
            "Sem observações"
        );
        await repoMatricula.Adicionar(matricula);

        // Act
        var matriculas = (await repoMatricula.ObterPorAluno(aluno.Id)).ToList();
        Assert.NotEmpty(matriculas);

        var matriculaParaRemover = matriculas.First();

        // Remover
        var resultadoRemocao = await repoMatricula.Remover(matriculaParaRemover.Id);
        Assert.True(resultadoRemocao);

        // Verificar se foi removida
        var matriculaRemovida = await repoMatricula.ObterPorId(matriculaParaRemover.Id);
        Assert.Null(matriculaRemovida);
    }


    [Fact]
    public async Task Matricula_ObterTodos()
    {
        // ObterTodos

        var repoMatriculaTodos = new MatriculaRepository(ConnectionString, DatabaseType);

        var resultado = await repoMatriculaTodos.ObterTodos();
        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task Matricula_ObterPorId()
    {
        var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
        var logradouro = await repoLogradouro.ObterPorId(4);
        Assert.NotNull(logradouro);

        var arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 });

        var repoAluno = new AlunoRepository(ConnectionString, DatabaseType);
        var repoMatricula = new MatriculaRepository(ConnectionString, DatabaseType);

        // CPF único para o teste
        var random = new Random();
        var _cpf = $"123456{random.Next(100, 999):D3}01";

        // Remove aluno antigo se existir
        var alunoExistente = await repoAluno.ObterPorCpf(_cpf);
        if (alunoExistente != null)
            await repoAluno.Remover(alunoExistente.Id);

        // Cria aluno novo
        var aluno = Aluno.Criar(
            1,
            "Aluno Teste ObterPorId",
            _cpf,
            new DateOnly(2010, 10, 09),
            "49999999999",
            "aluno@teste.com",
            logradouro!,
            "123",
            "Complemento casa",
            "Senha@123",
            arquivo
        );
        await repoAluno.Adicionar(aluno);

        // Cria matrícula para esse aluno
        var matricula = Matricula.Criar(
            1,
            aluno,
            EMatriculaPlano.Semestral,
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddMonths(6)),
            "Emagrecer",
            EMatriculaRestricoes.Alergias,
            arquivo,
            "Sem observações"
        );
        await repoMatricula.Adicionar(matricula);

        // Act
        var matriculas = (await repoMatricula.ObterPorAluno(aluno.Id)).ToList();
        Assert.NotEmpty(matriculas);

        var matriculaParaTestar = matriculas.First();

        var matriculaPorId = await repoMatricula.ObterPorId(matriculaParaTestar.Id);
        Assert.NotNull(matriculaPorId);
        Assert.Equal(matriculaParaTestar.Id, matriculaPorId.Id);
    }

}